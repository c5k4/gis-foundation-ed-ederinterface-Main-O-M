exit 0
echo off
rem ===========================================================================
rem Script Name: ED_GISSUB_TRACE_TBL_COPY_PUB_DC1B.bat
rem Created: June 02, 2015
rem Windows HEAD - BEGIN
rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
rem Windows HEAD - END
set cmdLine=echo Windows
rem Windows TAIL - BEGIN

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
REM echo "exit 0 is placed"  >> %file_log% 2>&1
REM exit 0



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


cd /d %cmdPath%  
echo Executing %cmdLine%...                                 >> %file_log% 2>&1

call %cmdLine%                                              >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

set sqlUSR=uc4admin

set sqlDB=edsubgmc_QA_DC2B

@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD uc4admin@edsubgmc_QA_DC1B') do set "sqlPWD=%%a"


rem ---------------------------------------------------------------------------
rem Added by Saurabh on 2/3/2016 to not export 25,50,100 scale maps for map prod 1.0 
rem Run the executable.

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommand%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @"D:\edgisdbmaint\Copy_trace_tbl_edsubgmc_qa_dc2b.sql"          >> %file_log% 2>&1

set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1
rem ---------------------------------------------------------------------------


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END


