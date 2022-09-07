echo off
rem ===========================================================================
rem Script Name: EDG_GIS_to_OMS_ED51.bat
rem Created: Wednesday, July 16, 2014 10:33:34 AM
rem Windows HEAD - BEGIN
rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
rem Windows HEAD - END
set cmdLine=echo Windows
rem Windows TAIL - BEGIN


set sqlUSR=edsett

set sqlDB=EDAUX
set sqlCommand=D:\edgisdbmaint\ED51_OMS_DMS_FULL_RUN.sql

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDSETT@EDAUX') do set "sqlPWD=%%a"


rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log


rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
rem set cmdPath="D:\TLM\Utility\"
REM set cmdPath="D:\TLM\Utility\"
REM set cmdLine=SMUtilityExport.exe Settings OMSDMS

REM rem ---------------------------------------------------------------------------
REM rem Set script variables.
REM rem ---------------------------------------------------------------------------
set cmdPath="D:\edgisdbmaint"
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %sqlCommand%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
REM set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
REM echo Error Level: %error_level%                             >> %file_log% 2>&1
REM rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Run the executable.






set cmdPath="D:\TLM\Utility\"
set cmdLine=SMUtilityExport.exe Settings OMSDMS


rem ---------------------------------------------------------------------------

cd /d %cmdPath%  
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %cmdLine%                                              >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END


