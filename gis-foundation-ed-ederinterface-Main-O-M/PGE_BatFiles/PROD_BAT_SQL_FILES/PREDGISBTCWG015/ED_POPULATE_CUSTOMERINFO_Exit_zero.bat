EXIT 0
echo off
rem ===========================================================================
rem Script Name: EDG_ED_Populate_CustomerInfo.bat
rem Windows HEAD - BEGIN
rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
rem Windows HEAD - END
set cmdLine=echo Windows
rem Windows TAIL - BEGIN

rem This statement is required for variables like (!variable!) inside for loop.
REM setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log


rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CCB\Populate_CustomerInfo"
rem set cmdLine=EDG_CCB_Cust_info.bat

set SQLScript_path="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CCB\Populate_CustomerInfo\CCB_Cust_info.sql"

set CUSTOMER_UNAME=CCB_EI_EDGIS_RW

@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD CCB_EI_EDGIS_RW@EDER') do set "CUSTOMER_PWD=%%a"
set Database=EDER

set DbType=edmaint

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Run Analyse on T_CUSTADD, TRANSFORMER, SERVICEPOINT
rem ---------------------------------------------------------------------------
echo Executing Analyse on tables.....  >> %file_log% 2>&1
call D:\edgisdbmaint\PopulateInfoGatherStat_Analyze.bat
echo Executed Analyse on tables>> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

cd /d %cmdPath% 

REM echo Executing %cmdLine%...                                 >> %file_log% 2>&1
REM call %cmdLine%                                              >> %file_log% 2>&1
REM set /A error_level=%error_level% + %ERRORLEVEL%

echo Executing CUSTOMER_INFO Extract Processing on %Database%.....  >> %file_log% 2>&1
sqlplus -s %CUSTOMER_UNAME%/"%CUSTOMER_PWD%"@%Database% @%SQLScript_path% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Completed executing CUSTOMER_INFO Data Processing on %Database%>> %file_log% 2>&1

rem echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

REM --------------------------------------------------------------------------------

set cmdPath="D:\edgisdbmaint"

set SQLScript_path=D:\edgisdbmaint\TX_Search_Populate.sql

set TX_UNAME=EDGIS
set TX_Database=EDER

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@EDER') do set "TX_PWD=%%a"

set DbType=edmaint

echo Executing Trasnformer Address Processing on %Database%.....  >> %file_log% 2>&1
sqlplus -s %TX_UNAME%/"%TX_PWD%"@%TX_Database% @%SQLScript_path% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Completed executing Trasnformer Address Data Processing on %Database%>> %file_log% 2>&1

rem echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END
