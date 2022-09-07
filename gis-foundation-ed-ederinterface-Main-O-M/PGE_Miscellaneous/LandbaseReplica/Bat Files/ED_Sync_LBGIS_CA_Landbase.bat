echo off
rem ===========================================================================
rem ED_CreateReplica_LBGIS_CA_Landbase.bat
rem ---------------------------------------------------------------------------
rem This script calls a python script performs the
rem ESRI ArcGIS Server function Sync Replica
rem on a PG&E ED GIS database.
rem 
rem Stop any ArcGIS Server map service that is using the target database
rem prior to running this script.
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  ED_CreateReplica_LBGIS_CA_Landbase.bat
rem         
rem Author: Vedant Sood (V3SF)
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set error_level=0

set config_name=LBGIS_CA_Landbase.ini
set filename_script_01=synchronizeReplica.py

set file_path="D:\Vedant\CreateReplica"

set filename_current=%~0
set path_current="%~dp0"
set file_current=%path_current%%filename_current%
set path_log=%path_current%log
set file_script_01=%file_path%\%filename_script_01%

rem ===========================================================================

rem ===========================================================================
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
if not exist %pythonexe% set pythonexe=c:\python27\arcgis10.8\python.exe

rem ===========================================================================

rem ===========================================================================
rem Get the Date&Time into a variable.
rem ---------------------------------------------------------------------------
FOR /f "tokens=2 delims==" %%G in ('wmic os get localdatetime /value') do set datetime=%%G
ECHO dd = %datetime:~6,2%
ECHO mth = %datetime:~4,2% 
ECHO yyyy = %datetime:~0,4%
SET "dd=%datetime:~6,2%"
SET "mth=%datetime:~4,2%"
SET "yyyy=%datetime:~0,4%"
SET "date_log=%yyyy%_%mth%_%dd%"
ECHO Date %date_log%

set "currentTime=%time: =0%"
set hrs=%currentTime:~0,2%
set min=%time:~3,2%
set sec=%time:~6,2%
rem set time_log=%time%
set time_log=%hrs%_%min%_%sec%
echo time_log %time_log%
rem ===========================================================================

rem ===========================================================================
rem Create path to log files if it does not exist
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
rem ===========================================================================

rem ===========================================================================
rem Set log file variables.
rem ---------------------------------------------------------------------------
set filename_log_base=%~n0
set filename_log=%filename_log_base%_%date_log%_%time_log%.log
set file_log_tmp=%path_log%\%filename_log_base%.log
set file_log=%path_log%\%filename_log%
if exist %file_log_tmp% del %file_log_tmp%
rem ===========================================================================

rem ===========================================================================
rem Run the program that does the main Job.
rem ---------------------------------------------------------------------------
cd /d %file_path%
set command=call %pythonexe% %file_script_01% %config_name%
echo Executing %command%... >> %file_log_tmp% 2>&1
               %command%    >> %file_log_tmp% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log_tmp% 2>&1
rem ===========================================================================


rem ===========================================================================
rem Change the error_level to a positive integer
rem ---------------------------------------------------------------------------
set /A error_level_orig=%error_level%
if "%error_level_orig%" neq "0" set /A error_level=1
if "%error_level_orig%" neq "0" (
  echo Original error level: %error_level_orig%               >> %file_log_tmp% 2>&1
  echo Changed  error level to a positive integer for UC4.    >> %file_log_tmp% 2>&1
  echo Current  error level: %error_level%                    >> %file_log_tmp% 2>&1
  )
rem ===========================================================================

rem ===========================================================================
rem Copy temporary log file to date-stamped log file.
rem ---------------------------------------------------------------------------
set command=copy %file_log_tmp% %file_log%
echo Executing %command%... >> %file_log_tmp% 2>&1
               %command%    >nul
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log_tmp% 2>&1
rem ===========================================================================

rem ===========================================================================
rem Return the error level.
rem ---------------------------------------------------------------------------
echo Exiting %file_current% with error_level = %error_level% >> %file_log%     2>&1
echo Exiting %file_current% with error_level = %error_level% >> %file_log_tmp% 2>&1
exit /b %error_level%
rem ===========================================================================
pause