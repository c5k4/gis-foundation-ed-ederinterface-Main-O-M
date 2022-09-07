echo off
rem ===========================================================================
rem Script Name: ED_DATA_RETEN_ED12.bat
rem Created: July 15, 2021
rem Purpose: process the Interface Data Retention job 
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

set DbType=edgmc
set EDGIS_Database=EDGMC

set uname=INTDATAARCH
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD INTDATAARCH@EDGMC') do set "INTDATAARCH_PWD=%%a"
echo Output of print.exe is: %INTDATAARCH_PWD%

set SQLScript_path=D:\edgisdbmaint\PGE_INT_DATA_RTNTN.sql

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

REM echo Checking for successful EI data delivery  >> %file_log% 2>&1

REM FOR /F "usebackq delims=!" %%i IN (`sqlplus -s %uname%/%INTDATAARCH_PWD%@%EDGIS_Database% @%SQLScriptLock_path% `) DO set x=%%i 
 REM echo EI successful delivery code (0 is success) : %x% >> %file_log% 2>&1
REM IF not %x% == 0 (
  REM set /A error_level=1
  REM echo PGE_CCB_SP_IO_MONITOR / INBOUND Table is being updated.>> %file_log% 2>&1
  REM echo Exiting %file_script% with error_level = 1 >> %file_log% 2>&1
  REM exit /b 1
REM )

rem ===========================================================================

echo Executing Interface Data Retention Extract Processing on %EDGIS_Database%.....  >> %file_log% 2>&1

sqlplus -s %uname%/%INTDATAARCH_PWD%@%EDGIS_Database% @%SQLScript_path% >> %file_log% 'ED12' 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%             


echo Completed executing Interface Data Retention Processing on %EDGIS_Database% >> %file_log% 2>&1


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------

if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%

