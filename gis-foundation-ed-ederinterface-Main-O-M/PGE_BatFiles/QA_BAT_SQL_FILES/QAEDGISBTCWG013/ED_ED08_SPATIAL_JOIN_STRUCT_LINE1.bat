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
set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.6\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.7\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.7\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.9\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.9\python.exe
rem ===========================================================================
set cmdLine1=%pythonexe% Spatialjoin.py 1
set cmdLine2=%pythonexe% Spatialjoin.py 2
set cmdLine3=%pythonexe% Spatialjoin.py 3
set cmdLine4=%pythonexe% Spatialjoin.py 4
set cmdLine5=%pythonexe% Spatialjoin.py 5
set cmdLine6=%pythonexe% Spatialjoin.py 6
set cmdLine7=%pythonexe% Spatialjoin.py 7
set cmdLine8=%pythonexe% Spatialjoin.py 8
set cmdLine9=%pythonexe% Spatialjoin.py 9
set cmdLine10=%pythonexe% Spatialjoin.py 10
set cmdLine11=%pythonexe% Spatialjoin.py 11
set cmdLine12=%pythonexe% Spatialjoin.py 12
set cmdLine13=%pythonexe% Spatialjoin.py 13
set cmdLine14=%pythonexe% Spatialjoin.py 14
set cmdLine15=%pythonexe% Spatialjoin.py 15
set cmdLine16=%pythonexe% Spatialjoin.py 16
set cmdLine17=%pythonexe% Spatialjoin.py 17
set cmdLine18=%pythonexe% Spatialjoin.py 18
set cmdLine19=%pythonexe% Spatialjoin.py 19
set cmdLine20=%pythonexe% Spatialjoin.py 20
set cmdLine21=%pythonexe% Spatialjoin.py 21


rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

cd /d %cmdPath%                                             >> %file_log% 2>&1

echo Executing %cmdLine%...                                 >> %file_log% 2>&1
start %cmdLine1%
timeout /t 20   
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
rem Running Ping to check all processed has completed
rem @ping -n 1 127.0.0.1 > nul
ping -n 1 127.0.0.1
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
if  not %hh% GTR 2 goto :loop
set /A error_level=1              %file_log% 2>&1
@echo Existing %file_script% with error_level = %error_level% >> %file_log% 2>&1

:next

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
