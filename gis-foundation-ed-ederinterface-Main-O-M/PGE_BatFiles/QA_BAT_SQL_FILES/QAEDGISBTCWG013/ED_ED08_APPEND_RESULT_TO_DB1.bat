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
set cmdLine="D:\ED08\load_data.py"


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
set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.6\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.7\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.7\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.9\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.9\python.exe
rem ===========================================================================
set cmdLine1=%pythonexe% load_data.py 1
set cmdLine2=%pythonexe% load_data.py 2
set cmdLine3=%pythonexe% load_data.py 3
set cmdLine4=%pythonexe% load_data.py 4
set cmdLine5=%pythonexe% load_data.py 5
set cmdLine6=%pythonexe% load_data.py 6
set cmdLine7=%pythonexe% load_data.py 7
set cmdLine8=%pythonexe% load_data.py 8
set cmdLine9=%pythonexe% load_data.py 9
set cmdLine10=%pythonexe% load_data.py 10
set cmdLine11=%pythonexe% load_data.py 11
set cmdLine12=%pythonexe% load_data.py 12
set cmdLine13=%pythonexe% load_data.py 13
set cmdLine14=%pythonexe% load_data.py 14
set cmdLine15=%pythonexe% load_data.py 15
set cmdLine16=%pythonexe% load_data.py 16
set cmdLine17=%pythonexe% load_data.py 17
set cmdLine18=%pythonexe% load_data.py 18
set cmdLine19=%pythonexe% load_data.py 19
set cmdLine20=%pythonexe% load_data.py 20
set cmdLine21=%pythonexe% load_data.py 21


rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
start %cmdLine1%   
start %cmdLine2%  
start %cmdLine3%
start %cmdLine4%
start %cmdLine5%
start %cmdLine6%
start %cmdLine7% 
start %cmdLine8%
start %cmdLine9%
start %cmdLine10%
start %cmdLine11%
start %cmdLine12%
start %cmdLine13%
start %cmdLine14%
start %cmdLine15%
start %cmdLine16%
start %cmdLine17%
start %cmdLine18%
start %cmdLine19%
start %cmdLine20%
start %cmdLine21%

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
