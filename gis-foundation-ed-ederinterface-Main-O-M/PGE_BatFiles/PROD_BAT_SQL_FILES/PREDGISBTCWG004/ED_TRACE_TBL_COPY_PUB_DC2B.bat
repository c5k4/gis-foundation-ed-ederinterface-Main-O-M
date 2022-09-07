echo off
rem ===========================================================================
rem Script Name: ED_GIS_TRACE_TBL_COPY_PUB_DC1A.bat
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
if not exist %path_log% (
	mkdir %path_log%
	set /A error_level=%error_level% + %ERRORLEVEL%
)
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------


rem cd /d %cmdPath%  
echo Executing %cmdLine%...                                 >> %file_log% 2>&1

call %cmdLine%                                              >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

set sqlUSR=uc4admin

set sqlDB=EDPUB_PR_DC2B

@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD uc4admin@EDPUB_PR_DC2B') do set "sqlPWD=%%a"


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
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @"D:\edgisdbmaint\Copy_trace_tbl_edpub_prod_dc2b1.sql"          >> %file_log% 2>&1
echo Error level from script 1: %ERRORLEVEL% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @"D:\edgisdbmaint\Copy_trace_tbl_edpub_prod_dc2b2.sql"          >> %file_log% 2>&1
echo Error level from script 2: %ERRORLEVEL% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @"D:\edgisdbmaint\Copy_trace_tbl_edpub_prod_dc2b3.sql"          >> %file_log% 2>&1
echo Error level from script 3: %ERRORLEVEL% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @"D:\edgisdbmaint\Copy_trace_tbl_edpub_prod_dc2b4.sql"          >> %file_log% 2>&1
echo Error level from script 4: %ERRORLEVEL% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1
rem ---------------------------------------------------------------------------

echo Checking log file for errors after the script execution	>> %file_log% 2>&1
set search_string="ORA- SP2-"
for /F %%N in ('findstr %%search_string%% ^< %file_log%  ^| find /c /v ""') do set NUM=%%N
echo Total Errors identified: %NUM%	>> %file_log% 2>&1
if "%NUM%" neq "0" set /A error_level=1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END


