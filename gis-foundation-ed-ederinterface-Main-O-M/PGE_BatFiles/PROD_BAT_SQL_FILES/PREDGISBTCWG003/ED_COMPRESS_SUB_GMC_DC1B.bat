﻿echo off

rem ===========================================================================
rem EDG_Compress_Maint.bat
rem ---------------------------------------------------------------------------
rem This script calls a python script performs the
rem ESRI ArcGIS Server function compress
rem on a PG&E ED GIS database.
rem 
rem Stop any ArcGIS Server map service that is using the target database
rem prior to running this script.
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  EDG_Compress_Maint.bat
rem         
rem Author: Vince Ulfig vulfig@gmail.com 4157101998
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set error_level=0

set DbType=EDSUBGMC_PR_DC1B

set filename_script_01=EDG_sdemon_pause_ed_maint.bat
set filename_script_02=compressGdb.py

set filename_current=%~0
set path_current=%~dp0
set file_current=%path_current%%filename_current%
set path_log=%path_current%log
set file_script_01=%path_current%%filename_script_01%
set file_script_02=%path_current%%filename_script_02%
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

rem ===========================================================================

rem ===========================================================================
rem Get the Date into a variable.
rem ---------------------------------------------------------------------------
reg copy       "HKCU\Control Panel\International" "HKCU\Control Panel\International-Temp" /f >nul
reg add        "HKCU\Control Panel\International" /v sShortDate /d "yyyy_MM_dd"           /f >nul
@REM reg query "HKCU\Control Panel\International" /v sShortDate                              >nul
set date_log=%date%
reg copy       "HKCU\Control Panel\International-Temp" "HKCU\Control Panel\International" /f >nul
reg delete     "HKCU\Control Panel\International-Temp"                                    /f >nul
rem ===========================================================================

rem ===========================================================================
rem Get the Time into a variable.
rem ---------------------------------------------------------------------------
reg copy       "HKCU\Control Panel\International" "HKCU\Control Panel\International-Temp" /f >nul
reg add        "HKCU\Control Panel\International" /v sTimeFormat /d "HH:mm:ss"            /f >nul
@REM reg query "HKCU\Control Panel\International" /v sTimeFormat                             >nul
set "currentTime=%time: =0%"
set hrs=%currentTime:~0,2%
set min=%time:~3,2%
set sec=%time:~6,2%
rem set time_log=%time%
set time_log=%hrs%_%min%_%sec%
reg copy       "HKCU\Control Panel\International-Temp" "HKCU\Control Panel\International" /f >nul
reg delete     "HKCU\Control Panel\International-Temp"                                    /f >nul
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
rem Run sdemon -o pause to prevent new connections during compress.
rem ---------------------------------------------------------------------------
rem Can't pipe standard out and standard error from this script because
rem it is also being piped in the script that is being called,
rem so the command below is not piped to standard out.
rem 
rem The commented code below is example code for
rem starting a new thread (process) in case we want to do this somewhere.
rem 
rem set command=start /b cmd /c call %file_script_01%
rem echo Executing %command%... >> %file_log_tmp% 2>&1
rem               %command%
rem set /A error_level=%error_level% + %ERRORLEVEL%
rem echo Executed  %command%.   >> %file_log_tmp% 2>&1
rem ===========================================================================

rem ===============================================================================================
rem Run the script to drop the orphan key set tables based on Esri’s knowledge base article.
rem -----------------------------------------------------------------------------------------------
echo Executing  drop the orphan key set tables...   >> %file_log_tmp% 2>&1

@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD SDE@EDSUBGMC_PR_DC1B') do set "PASSWORD=%%a"
echo Output of print.exe is: %PASSWORD%

sqlplus -s sde/%PASSWORD%@EDSUBGMC_PR_DC1B @"D:\edgisdbmaint\16_AfterOracle_restart_KeysetTableDrop_v2.sql"
echo Executed  drop the orphan key set tables.   >> %file_log_tmp% 2>&1
rem ===============================================================================================

rem ===========================================================================
rem Run the program that does the main Job.
rem ---------------------------------------------------------------------------
set command=call %pythonexe% %file_script_02% %DbType%
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
