exit 0

rem Not executing in Current Production

echo off
rem ===========================================================================
rem Script Name: EDG_Clean_Schematic_GP_Log.bat
rem Created: Wednesday, July 16, 2014 10:33:33 AM
rem Windows HEAD - BEGIN
rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
rem Windows HEAD - END
set cmdLine=echo Windows
rem Windows TAIL - BEGIN


set error_level=0
set date_time=%date:~10,4%-%date:~4,2%-%date:~7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion

rem ---------------------------------------------------------------------------
rem Set batch file parameters
rem ---------------------------------------------------------------------------

set Python_script=D:\edgisdbmaint\RemoveGPHistory_AGS_10_1VersionOnly.py

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
set pythonexe=D:\python27\ArcGIS10.8\python.exe
if not exist %pythonexe% set pythonexe=C:\ProgramData\Microsoft\Windows\Start Menu\Programs\ArcGIS\Python 2.7
if not exist %pythonexe% set pythonexe=C:\ProgramData\Microsoft\Windows\Start Menu\Programs\ArcGIS\Python 2.7
if not exist %pythonexe% set pythonexe=C:\ProgramData\Microsoft\Windows\Start Menu\Programs\ArcGIS\Python 2.7
if not exist %pythonexe% set pythonexe=C:\ProgramData\Microsoft\Windows\Start Menu\Programs\ArcGIS\Python 2.7
rem ===========================================================================


rem ===========================================================================
rem Run the Python to post and delete the version
rem ---------------------------------------------------------------------------
set command=call %pythonexe% %Python_script% 
echo Executing %command%... >> %file_log% 2>&1
               %command%    >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log% 2>&1
rem ===========================================================================


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END


