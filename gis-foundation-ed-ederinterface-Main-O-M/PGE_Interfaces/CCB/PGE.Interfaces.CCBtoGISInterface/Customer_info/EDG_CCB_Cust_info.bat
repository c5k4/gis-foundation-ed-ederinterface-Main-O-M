echo off
rem ===========================================================================
rem Script Name: EDG_CCB_Cust_info.bat
rem Created: October 27, 2015
rem Purpose: process the customer_info data in the ccbtoedgis staging table 
rem ===========================================================================

rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdLine=echo Windows

rem This statement is required for variables like (!variable!) inside for loop.
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

rem ---------------------------------------------------------------------------
rem Set batch file parameters
rem ---------------------------------------------------------------------------

set DbType=edmaint
set EDGIS_Database=LBGISS2Q

set uname=CUSTOMER

for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD CUSTOMER@LBGISS2Q') do set "EDGIS_CUSTOMER_PWD=%%a"

set SQLScript_path=D:\edgisdbmaint\CCB_Cust_info.sql
set SQLScriptLock_path=D:\edgisdbmaint\CCB_Cust_info_lock.sql

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log% 
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%


rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------
rem ===========================================================================

rem first check that EI successfully delivered the data
rem if inbound interface is status Insert-complete proceed
rem else fail with error

echo Checking for successful EI data delivery  >> %file_log% 2>&1

FOR /F "usebackq delims=!" %%i IN (`sqlplus -s %uname%/%EDGIS_CUSTOMER_PWD%@%EDGIS_Database% @%SQLScriptLock_path% `) DO set x=%%i 
 echo EI successful delivery code (0 is success) : %x% >> %file_log% 2>&1
IF not %x% == 0 (
  set /A error_level=1
  echo PGE_CCB_SP_IO_MONITOR / INBOUND Table is being updated.>> %file_log% 2>&1
  echo Exiting %file_script% with error_level = 1 >> %file_log% 2>&1
  exit /b 1
)

rem ===========================================================================

echo Executing CUSTOMER_INFO Extract Processing on %EDGIS_Database%.....  >> %file_log% 2>&1

sqlplus -s %uname%/%EDGIS_CUSTOMER_PWD%@%EDGIS_Database% @%SQLScript_path% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%             


echo Completed executing CUSTOMER_INFO Data Processing on %EDGIS_Database% >> %file_log% 2>&1


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------

if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%

