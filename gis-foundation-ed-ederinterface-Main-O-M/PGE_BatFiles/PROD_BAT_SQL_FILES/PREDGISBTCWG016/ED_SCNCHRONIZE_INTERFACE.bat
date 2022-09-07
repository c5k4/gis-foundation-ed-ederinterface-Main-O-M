rem echo off
rem Script Name: EDG_SCNCHRONIZE_INTERFACE.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Added by Venkat on 9/5/2014
rem ---------------------------------------------------------------------------
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
set sqlUSR=gis_i

set sqlDB=EDER

set sqlUSR2=gis_i

set sqlDB2=EDER

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD GIS_I@EDER') do set "sqlPWD=%%a"
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD GIS_I@EDER') do set "sqlPWD2=%%a"

set sqlCommandPGE_DMS_TO_PROCESS="D:\edgisdbmaint\PrepareDeleteCircuitids.sql"
set sqlCommandPGE_DMS_TO_PROCESS2="D:\edgisdbmaint\PrepareDeleteSUBSTATIONIds.sql"
set sqlCommandPGE_DMS_TO_PROCESS3="D:\edgisdbmaint\UpdateSchematicChangeDetectionGrid.sql"

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
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
echo %sqlPWD%


rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommandPGE_DMS_TO_PROCESS%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/"%sqlPWD%"@%sqlDB% @%sqlCommandPGE_DMS_TO_PROCESS%          >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommandPGE_DMS_TO_PROCESS%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommandPGE_DMS_TO_PROCESS2%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/"%sqlPWD%"@%sqlDB% @%sqlCommandPGE_DMS_TO_PROCESS2%          >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommandPGE_DMS_TO_PROCESS2%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommandPGE_DMS_TO_PROCESS2%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/"%sqlPWD%"@%sqlDB% @%sqlCommandPGE_DMS_TO_PROCESS3%          >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommandPGE_DMS_TO_PROCESS2%.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS.sql...                              >> %file_log% 2>&1
 sqlplus -s %sqlUSR2%/%sqlPWD2%@%sqlDB2% @D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS.sql           >> %file_log% 2>&1
echo Executing D:\edgisdbmaint\log\DELETEPROCESSEDSUBSTATION.sql...                              >> %file_log% 2>&1
 sqlplus -s %sqlUSR2%/%sqlPWD2%@%sqlDB2% @D:\edgisdbmaint\log\DELETEPROCESSEDSUBSTATION.sql           >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
RENAME D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS.sql   DELETEPROCESSEDCIRCUITS%date_time%.sql
RENAME D:\edgisdbmaint\log\DELETEPROCESSEDSUBSTATION.sql   DELETEPROCESSEDSUBSTATION%date_time%.sql
echo Executed  D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS.sql.                                >> %file_log% 2>&1
echo Executed  D:\edgisdbmaint\log\DELETEPROCESSEDSUBSTATION.sql.                                >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1



