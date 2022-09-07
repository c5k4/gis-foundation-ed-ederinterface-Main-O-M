echo off
rem ===========================================================================
rem Script Name: ED_ED08_CHANGEIDENTIFICATION.bat
rem Created: May 25, 2021

rem ===========================================================================

rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdLine=echo Windows

rem This statement is required for variables like (!variable!) inside for loop.
REM setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log


rem ---------------------------------------------------------------------------
rem Set batch file parameters
rem ---------------------------------------------------------------------------

set DbType=EDGMC
set Database=EDGMC_QA

set uname= EDGISBO


set SQLScript_path1=D:\ED08\ED08RunCompleted.sql
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDGISBO@EDGMC') do set "PASSWORD=%%a"

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

rem ===========================================================================

echo Executing ED08RunCompleted Process on %Database%.....  >> %file_log% 2>&1

sqlplus  -s %uname%/%PASSWORD%@%Database% @%SQLScript_path1% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%             

echo Completed executing ED08RunCompleted Data Processing on %Database%>> %file_log% 2>&1


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------

if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%

