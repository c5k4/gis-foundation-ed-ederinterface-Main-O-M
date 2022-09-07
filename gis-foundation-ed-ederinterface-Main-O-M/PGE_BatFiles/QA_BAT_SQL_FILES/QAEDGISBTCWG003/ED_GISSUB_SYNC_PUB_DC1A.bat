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

rem exit 0
set error_level=0
set filename_current=%~0
set path_current=%~dp0
set file_current=%path_current%%filename_current%
set path_log=%path_current%log
rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion



rem set pythonexe=D:\Python27\ArcGIS10.2\python.exe
rem set sourcesde=D:\UC4Jobs\Databaseconnection\EDSUB\lob_source_edgis_edsub1q.sde
rem set targetsde=D:\UC4Jobs\Databaseconnection\EDSUB\gm_target_edgis_gmess1q.sde
rem set replicaname=GeoMart_EDGIS_Replica
rem set LOB=EDGIS
rem set pythonscript=D:\UC4Jobs\Scripts\LOB_to_GeoMart_ReplicaSync_Disconnected.py
rem set DataPath=D:\LOB_GeoMart\Replication\export

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
set sourcesde=D:\edgisdbmaint\conn_gdb\lob_source_edgis_edgsm1q.sde
set targetsde=D:\edgisdbmaint\conn_gdb\lob_target_edgis_edgsp1aq.sde
set replicaname=Edsubpub_edgsm1q_to_edgsp1aq_Replica
set LOB=EDGIS
set pythonscript=D:\edgisdbmaint\LOB_to_GeoMart_ReplicaSync_Disconnected.py
set DataPath=D:\edgisdbmaint\LOB_GeoMart\Replication\export

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
rem Execute the command
rem ---------------------------------------------------------------------------

set command= call  %pythonexe% %pythonscript% %DataPath% %sourcesde% %targetsde% %replicaname% %LOB% 

echo Executing %command%... >> %file_log_tmp% 2>&1
               %command%    >> %file_log_tmp% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log_tmp% 2>&1


rem ===========================================================================
rem Change the error_level to a positive integer
rem ---------------------------------------------------------------------------
set search_string="ora- sp2- error exception fail Failed Error System.Exception:"
set exclude_string1="Changed  error level to a positive integer for UC4."
set exclude_string2="error_level = 0"
set exclude_string3="if there is an error while rebuilding Geometric Network connectivity"
set exclude_string4="error level:"

set NUM=0

for /F %%N in ('findstr %%search_string%% ^< %file_log_tmp%  ^|findstr /v /C:%exclude_string1% ^|findstr /v /C:%exclude_string2%  ^|findstr /v /C:%exclude_string3% ^|findstr /v /C:%exclude_string4% ^| find /c /v ""') do set NUM=%%N
echo number of error lines : %NUM%  >> %file_log_tmp% 2>&1
if "%NUM%" neq "0" set /A error_level=1
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
