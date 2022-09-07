rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
echo off
rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
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
rem echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

rem exit 0

echo off
rem Script Name: EDG_SCHM_Session0_Update_Command.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
rem set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.Desktop.SchematicGPTools\GeoprocessingTools\PGESession0UpdateCommand.py"
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.Desktop.SchematicGPTools\GeoprocessingTools"
set cmdLine="PGESession0UpdateCommand.py"

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
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
rem Set path to Python
rem ---------------------------------------------------------------------------
set pythonexe=D:\python27\ArcGIS10.8\python.exe
rem if not exist %pythonexe% set pythonexe=c:\python27\arcgisx6410.1\python.exe
rem if not exist %pythonexe% set pythonexe=d:\arcgis10.0\python.exe
rem if not exist %pythonexe% set pythonexe=c:\arcgis10.0\python.exe
rem if not exist %pythonexe% set pythonexe=d:\python26\arcgis10.0\python.exe
rem if not exist %pythonexe% set pythonexe=c:\python26\arcgis10.0\python.exe
rem if not exist %pythonexe% set pythonexe=d:\python27\arcgis10.1\python.exe
rem if not exist %pythonexe% set pythonexe=c:\python27\arcgis10.1\python.exe
set cmdLine=%pythonexe% %cmdLine%

rem ---------------------------------------------------------------------------
rem Run the executable.
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
