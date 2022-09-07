echo off
rem ===========================================================================
rem EDG_Map_Prod_02_DC1_01.bat
rem ---------------------------------------------------------------------------
rem This script starts the Map Production 2.0
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  EDG_Map_Prod_02_DC1_01.bat
rem This script will be run from UC4.
rem
rem Author: Vince Ulfig vulfig@gmail.com 4157101998
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set error_level=0

set path_exe="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0"
set filename_exe=PGE.Interfaces.MapProductionAutomation.exe
rem set filename_conn=edgmc_gisi.sde

for /f %%a in ('PGE_DBPasswordManagement.exe GETSDEFILE GIS_I@EDGMC_PR') do set "filename_conn=%%a"


rem This command could work if the creating the sde conn file automatically does not work.
set command=call "D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.exe" -c "D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.exe.config" -s 1 -e 17 

rem This command could work if the creating the sde conn file automatically does not work.
rem This is the currently used command. vdu1 2013_12_30_17_21
set file_exe=%path_exe%\%filename_exe%
rem set file_conn=%path_exe%\%filename_conn%
set file_conn=%filename_conn%
rem set command=call %file_exe% -c %file_conn% -O

rem Comment the command in the script below,
rem if you plan to use one of the commands above.

set dbtype=edgmc
set username=gis_i

set filename_script_01=mapProduction02.py

set filename_current=%~0

set path_current=%~dp0
set path_log=%path_current%log

set file_current=%path_current%%filename_current%
set file_script_01=%path_current%%filename_script_01%
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
rem Run the executable.
rem ---------------------------------------------------------------------------
rem set command=call %pythonexe% %file_script_01% %dbtype% %username% %file_exe%
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
