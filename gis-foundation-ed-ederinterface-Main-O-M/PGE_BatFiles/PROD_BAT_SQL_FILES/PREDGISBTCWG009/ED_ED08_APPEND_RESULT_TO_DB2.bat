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
set pythonexe=D:\Python27\ArcGIS10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.6\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.7\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.7\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.9\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.9\python.exe
rem ===========================================================================

set cmdLine2=%pythonexe% load_data.py 22
set cmdLine3=%pythonexe% load_data.py 23
set cmdLine4=%pythonexe% load_data.py 24
set cmdLine5=%pythonexe% load_data.py 25
set cmdLine6=%pythonexe% load_data.py 26
set cmdLine7=%pythonexe% load_data.py 27
set cmdLine8=%pythonexe% load_data.py 28
set cmdLine9=%pythonexe% load_data.py 29
set cmdLine10=%pythonexe% load_data.py 30
set cmdLine11=%pythonexe% load_data.py 31
set cmdLine12=%pythonexe% load_data.py 32
set cmdLine13=%pythonexe% load_data.py 33
set cmdLine14=%pythonexe% load_data.py 34
set cmdLine15=%pythonexe% load_data.py 35
set cmdLine16=%pythonexe% load_data.py 36
set cmdLine17=%pythonexe% load_data.py 37
set cmdLine18=%pythonexe% load_data.py 38
set cmdLine19=%pythonexe% load_data.py 39
set cmdLine20=%pythonexe% load_data.py 40
set cmdLine21=%pythonexe% load_data.py 41
set cmdLine22=%pythonexe% load_data.py 42


rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
 
start %cmdLine2% 
timeout /t 20 
start %cmdLine3%
timeout /t 20 
start %cmdLine4%
timeout /t 20 
start %cmdLine5%
timeout /t 20 
start %cmdLine6%
timeout /t 20 
start %cmdLine7% 
timeout /t 20 
start %cmdLine8%
timeout /t 20 
start %cmdLine9%
timeout /t 20 
start %cmdLine10%
timeout /t 20 
start %cmdLine11%
timeout /t 20 
start %cmdLine12%
timeout /t 20 
start %cmdLine13%
timeout /t 20 
start %cmdLine14%
timeout /t 20 
start %cmdLine15%
timeout /t 20 
start %cmdLine16%
timeout /t 20 
start %cmdLine17%
timeout /t 20 
start %cmdLine18%
timeout /t 20 
start %cmdLine19%
timeout /t 20 
start %cmdLine20%
timeout /t 20 
start %cmdLine21%
timeout /t 20 
start %cmdLine22%
timeout /t 20 

ping -n 60 127.0.0.1
rem Get start time:
for /F "tokens=1-4 delims=:.," %%a in ("%time%") do (
   set /A "start=(((%%a*60)+1%%b %% 100)*60+1%%c %% 100)*100+1%%d %% 100"
)
echo starttime: %start%                             >> %file_log% 2>&1
:loop
rem Get start time:
for /F "tokens=1-4 delims=:.," %%a in ("%time%") do (
   set /A "end=(((%%a*60)+1%%b %% 100)*60+1%%c %% 100)*100+1%%d %% 100"
)
echo end: %end%                             >> %file_log% 2>&1
rem Get elapsed time:
set /A elapsed=end-start                                >> %file_log% 2>&1
rem Show elapsed time:
set /A hh=elapsed/(60*60*100), rest=elapsed%%(60*60*100), mm=rest/(60*100), rest%%=60*100 >> %file_log% 2>&1
echo hh: %hh%                             >> %file_log% 2>&1
echo mm: %mm%                             >> %file_log% 2>&1

@echo Processing......                        >> %file_log% 2>&1
if not exist *.txt goto :next
@echo Existing                              >> %file_log% 2>&1
if  not %hh% GEQ 1 goto :loop
set /A error_level=1              %file_log% 2>&1
@echo Existing %file_script% with error_level = %error_level% >> %file_log% 2>&1

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