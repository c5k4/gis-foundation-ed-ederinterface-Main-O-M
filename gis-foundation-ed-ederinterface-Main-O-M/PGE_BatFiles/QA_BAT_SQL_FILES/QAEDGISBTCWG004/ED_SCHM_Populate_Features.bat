rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
echo off
rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO


rem Script Name: EDG_SCHM_Populate_Features.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
set sqlUSR=gis_i

set sqlDB=EDER

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD GIS_I@EDER_QA') do set "sqlPWD=%%a"

set sqlCommand="D:\edgisdbmaint\EDGIS_SQL_TO_POPULATE_GIS_FEATURE_LIST_FOR_USE_WITH_SCHEMATICS.sql"

rem This statement is required for variables like (!variable!) inside for loop.
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

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
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommand%...                              >> %file_log% 2>&1
 sqlplus -s %sqlUSR%/"%sqlPWD%"@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1


set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Tracing Cache"
set cmdLine="PGE.BatchApplication.PGE_Cached_Tracing.exe"

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
rem call %cmdLine%                                              
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%