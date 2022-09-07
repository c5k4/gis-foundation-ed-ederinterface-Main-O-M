rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
rem echo off
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
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
rem exit /b 0


rem echo off
rem Script Name: GM_EDSUB_TBL_SYNC.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\ED08"
set cmdLine="D:\ED08\Spatialjoin.py"


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

rem ===========================================================================
rem Set path to Python
rem ---------------------------------------------------------------------------
set pythonexe=D:\Python27\ArcGIS10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.6\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.7\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.7\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.9\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.9\python.exe
rem ===========================================================================

set cmdLine2=%pythonexe% Spatialjoin.py 22
set cmdLine3=%pythonexe% Spatialjoin.py 23
set cmdLine4=%pythonexe% Spatialjoin.py 24
set cmdLine5=%pythonexe% Spatialjoin.py 25
set cmdLine6=%pythonexe% Spatialjoin.py 26
set cmdLine7=%pythonexe% Spatialjoin.py 27
set cmdLine8=%pythonexe% Spatialjoin.py 28
set cmdLine9=%pythonexe% Spatialjoin.py 29
set cmdLine10=%pythonexe% Spatialjoin.py 30
set cmdLine11=%pythonexe% Spatialjoin.py 31
set cmdLine12=%pythonexe% Spatialjoin.py 32
set cmdLine13=%pythonexe% Spatialjoin.py 33
set cmdLine14=%pythonexe% Spatialjoin.py 34
set cmdLine15=%pythonexe% Spatialjoin.py 35
set cmdLine16=%pythonexe% Spatialjoin.py 36
set cmdLine17=%pythonexe% Spatialjoin.py 37
set cmdLine18=%pythonexe% Spatialjoin.py 38
set cmdLine19=%pythonexe% Spatialjoin.py 39
set cmdLine20=%pythonexe% Spatialjoin.py 40
set cmdLine21=%pythonexe% Spatialjoin.py 41
set cmdLine22=%pythonexe% Spatialjoin.py 42



rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
 
start %cmdLine2%  
start %cmdLine3%
start %cmdline4%
start %cmdline5%
start %cmdline6%
start %cmdline7% 
start %cmdline8%
start %cmdline9%
start %cmdline10%
start %cmdline11%
start %cmdline12%
start %cmdline13%
start %cmdline14%
start %cmdline15%
start %cmdline16%
start %cmdline17%
start %cmdline18%
start %cmdline19%
start %cmdline20%
start %cmdline21%
start %cmdline22%
                                         
rem Running Ping to check all processed has completed
rem @ping -n 1 127.0.0.1 > nul
ping -n 30 127.0.0.1
:loop
@echo Processing......
if not exist *.txt goto :next

    rem @ping -n 61 127.0.0.1 > nul
goto loop
:next
@echo Done Processing!

set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
