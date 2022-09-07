echo on
rem ===========================================================================
rem mxdperfstat.bat
rem ---------------------------------------------------------------------------
rem This script gathers metrics on the edgis system.
rem 
rem Usage:
rem   Open a command window.
rem   Navigate to script location.
rem   Usage:  mxdperfstat.bat <file_mxd> <threshold_seconds> <email_list>
rem
rem Requires:
rem   ArcGIS Desktop 10.0
rem   ArcEngine 10.0
rem   Place these together in a directory:
rem     mxdperfstat.bat
rem     mxdperfstat.py
rem     commonFunc.py
rem     mxdperfstat10.exe
rem     mxdperfstat.xsl
rem     <file_mxd>.mxd
rem
rem Note:
rem   mxdperfstat10.exe does not accept <file_mxd> with spaces
rem     in the file name.
rem   mxdperfstat10.exe probably does not accept <file_mxd> with spaces
rem     in the path to the file.
rem         
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set filename_mxd_01=Schematics_DC1B.mxd
rem set filename_mxd_02=EDMaster_EDGIS1P.mxd

set fltSecSlowThresholdDefault=5.0
set strListEmailDefault=d21v@pge.com
set strPutResultsInDB=False

rem Arguments for Testing
rem set fltSecSlowThresholdDefault=0.001
rem set strListEmailDefault=d6hd@pge.com
rem set strPutResultsInDB=False

set filename_current=%~0

set path_current=%~dp0
set path_log=%path_current%log

set file_mxd_01="%path_current%%filename_mxd_01%"
set file_mxd_02="%path_current%%filename_mxd_02%"

set filename_script=mxdperfstat_SCHM_1.py
set file_current=%path_current%%filename_current%
set file_script="%path_current%%filename_script%"

set file_mxd=%1
set fltSecSlowThreshold=%2
set strListEmail=%3

if [%1]==[] set file_mxd=%file_mxd_01%
if [%2]==[] set fltSecSlowThreshold=%fltSecSlowThresholdDefault%
if [%3]==[] set strListEmail=%strListEmailDefault%
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
echo "The path used"
echo %pythonexe%
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
rem Run the program that does the main Job.
rem ---------------------------------------------------------------------------

set command=call %pythonexe% %file_script% %file_mxd% %fltSecSlowThreshold% %strListEmail% %strPutResultsInDB%
echo Executing %command%... >> %file_log_tmp% 2>&1
               %command%    >> %file_log_tmp% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log_tmp% 2>&1

rem ===========================================================================

rem ===========================================================================
rem Copy temporary log file to date-stamped log file.
rem ---------------------------------------------------------------------------
echo Executing copy %file_log_tmp% %file_log% ... >> %file_log_tmp% 2>&1
               copy %file_log_tmp% %file_log%     >nul
echo Executed  copy %file_log_tmp% %file_log%.    >> %file_log%     2>&1
echo Executed  copy %file_log_tmp% %file_log%.    >> %file_log_tmp% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
rem ===========================================================================

rem ===========================================================================
rem Return the error level.
rem ---------------------------------------------------------------------------
echo Exiting %file_current% with error_level = %error_level% >> %file_log%     2>&1
echo Exiting %file_current% with error_level = %error_level% >> %file_log_tmp% 2>&1
exit /b %error_level%
rem ===========================================================================
