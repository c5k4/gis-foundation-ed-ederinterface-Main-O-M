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

echo off
rem Script Name: EDG_SCHM_Unlock_Clean_All.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.Desktop.SchematicGPTools\GeoprocessingTools"
set cmdLine="PGEUnlockCleanupAllCommand.py"

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
if not exist %pythonexe% set pythonexe=c:\python27\arcgisx6410.1\python.exe
if not exist %pythonexe% set pythonexe=d:\arcgis10.0\python.exe
if not exist %pythonexe% set pythonexe=c:\arcgis10.0\python.exe
if not exist %pythonexe% set pythonexe=d:\python26\arcgis10.0\python.exe
if not exist %pythonexe% set pythonexe=c:\python26\arcgis10.0\python.exe
if not exist %pythonexe% set pythonexe=d:\python27\arcgis10.1\python.exe
if not exist %pythonexe% set pythonexe=c:\python27\arcgis10.1\python.exe
if not exist %pythonexe% set pythonexe=d:\python27\arcgis10.2\python.exe
if not exist %pythonexe% set pythonexe=c:\python27\arcgis10.2\python.exe
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

if "%error_level%" equ "-2146234327" set /A error_level=0

REM rem ---------------------------------------------------------------------------

echo ""Executing MP_INIT_TABLECOPY"" 						>> %file_log% 2>&1

 set cmdPath="D:\edgisdbmaint"
 set batCommand_INIT="D:\edgisdbmaint\MP_INIT_TABLECOPY.bat"
 cd /d %cmdPath%                                             >> %file_log% 2>&1
rem # echo Executing %sqlCommand%...                              >> %file_log% 2>&1
 call %batCommand_INIT%     									>> %file_log% 2>&1
REM #set /A error_level=%error_level% + %ERRORLEVEL%
 echo Executed  %batCommand_INIT%.                           >> %file_log% 2>&1
 echo Error Level: %error_level%                             >> %file_log% 2>&1
REM rem ---------------------------------------------------------------------------


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
 

echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1

if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
