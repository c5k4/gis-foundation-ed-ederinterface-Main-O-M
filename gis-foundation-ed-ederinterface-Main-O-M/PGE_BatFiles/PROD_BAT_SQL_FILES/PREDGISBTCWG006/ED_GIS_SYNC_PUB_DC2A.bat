echo on
rem ===========================================================================
rem GM_ED_REPLICA_SYNC.bat
rem ---------------------------------------------------------------------------
rem This script starts the web servers and app servers on the specified machines
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  ReplicaSync_EDGIS.bat
rem This script will be run from UC4.
rem         
rem Author: Venkat Nittala

set error_level=0

set filename_current=%~0
set path_current=%~dp0
set file_current=%path_current%%filename_current%
set path_log=%path_current%log
rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion



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

for /f %%a in ('PGE_DBPasswordManagement.exe GETSDEFILE EDGIS@EDER_PR') do set "sourcesde=%%a"

for /f %%a in ('PGE_DBPasswordManagement.exe GETSDEFILE EDGIS@EDPUB_PR_DC2A') do set "targetsde=%%a"

rem set replicaname=EDER_DEFAULT_TO_EDPUB_DC2A
set replicaname=EDER_TO_EDPUB_DC2A
set LOB=EDPUB
rem set pythonscript=D:\edgisdbmaint\LOB_to_Publication_ReplicaSync_Disconnected_Reconcile.py
set pythonscript=D:\edgisdbmaint\LOB_to_PublicationDc2a_ReplicaSync_Disconnected_Reconcile.py
set DataPath=D:\edgisdbmaint\LOB_Publication\Replication\export

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
rem ========================================================================
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
rem Set output file for the python script
rem ========================================================================
set filename_out=%filename_log_base%_%date_log%_%time_log%.txt
set file_out=%path_log%\%filename_out%

rem ===========================================================================

rem ===========================================================================
rem Execute the command
rem ---------------------------------------------------------------------------

set command= call  %pythonexe% %pythonscript% %DataPath% %sourcesde% %targetsde% %replicaname% %LOB% %file_out%

echo Executing %command%... >> %file_log_tmp% 2>&1
               %command%    >> %file_log_tmp% 2>&1
FOR /F "tokens=* delims=" %%x in (%file_out%) DO set /A error_level=%%x
rem set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log_tmp% 2>&1
echo error_level after synch script = %error_level% >> %file_log_tmp% 2>&1

rem ===========================================================================
rem Change the error_level to a positive integer
rem ---------------------------------------------------------------------------
rem JFJ9 (08 Feb): temporarily added the below line newly to override the error level, since from ArcFM auto updater it is getting negative value (-2146234327).
rem if %error_level% lss 0 set error_level=0
rem JFJ9 (12 Mar): commented out the temporary patch since not working properly. even if python returns error code 1, in bat file it is showing the same negative value.

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
rem Removing the out file
rem ========================================================================
del %file_out% >> %file_log% 2>&1
echo Removed out file : %file_out%
rem ===========================================================================

rem ===========================================================================
rem Return the error level.
rem ---------------------------------------------------------------------------
echo Exiting %file_current% with error_level = %error_level% >> %file_log%     2>&1
echo Exiting %file_current% with error_level = %error_level% >> %file_log_tmp% 2>&1
exit /b %error_level%
rem ===========================================================================
