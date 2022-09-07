echo off

rem ===========================================================================
rem InterfaceIntegrationGatewayED15INDV.bat
rem ---------------------------------------------------------------------------
rem This script calls a InterfaceIntegrationGateway.exe performs the
rem GET/POST Operation for Interface Jobs using Layer 7  
rem on a PG&E ED GIS database.
rem 
rem For Configuration SET:
rem Interface_Name = ED06,ED08,...
rem Interface_Type = OutBound/InBound (Depending on Interface Job)
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  InterfaceIntegrationGatewayED15INDV.bat
rem         
rem Author: Vandana Tanwar (v1t8)
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set error_level=0

set Interface_Name=ED15INDV
set Interface_Type=InBound
set filename_script_01=PGE.Interfaces.Integration.Gateway.exe
set file_path="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.InterfaceIntegrationGateway"

set filename_current=%~0
set path_current="%~dp0"
set file_current=%path_current%%filename_current%
set path_log=%path_current%log
set file_script_01=%file_path%\%filename_script_01% %Interface_Name% %Interface_Type%
rem ===========================================================================

rem ===========================================================================
rem Get the Date into a variable.
rem ---------------------------------------------------------------------------
reg copy       "HKCU\Control Panel\International" "HKCU\Control Panel\Int-Temp-ED15-INDV" /f >nul
reg add        "HKCU\Control Panel\International" /v sShortDate /d "yyyy_MM_dd"           /f >nul
@REM reg query "HKCU\Control Panel\International" /v sShortDate                              >nul
set date_log=%date%
reg copy       "HKCU\Control Panel\Int-Temp-ED15-INDV" "HKCU\Control Panel\International" /f >nul
reg delete     "HKCU\Control Panel\Int-Temp-ED15-INDV"                                    /f >nul
rem ===========================================================================

rem ===========================================================================
rem Get the Time into a variable.
rem ---------------------------------------------------------------------------
reg copy       "HKCU\Control Panel\International" "HKCU\Control Panel\Int-Temp-ED15-INDV" /f >nul
reg add        "HKCU\Control Panel\International" /v sTimeFormat /d "HH:mm:ss"            /f >nul
@REM reg query "HKCU\Control Panel\International" /v sTimeFormat                             >nul
set "currentTime=%time: =0%"
set hrs=%currentTime:~0,2%
set min=%time:~3,2%
set sec=%time:~6,2%
rem set time_log=%time%
set time_log=%hrs%_%min%_%sec%
reg copy       "HKCU\Control Panel\Int-Temp-ED15-INDV" "HKCU\Control Panel\International" /f >nul
reg delete     "HKCU\Control Panel\Int-Temp-ED15-INDV"                                    /f >nul
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

rem ---------------------------------------------------------------------------
rem Run the program that does the main Job.
rem ---------------------------------------------------------------------------
cd /d %file_path%                                           >> %file_log_tmp% 2>&1

echo Executing %file_script_01%...                          >> %file_log_tmp% 2>&1
%file_script_01%                                            >> %file_log_tmp% 2>&1
echo Successfully returned from application code            >> %file_log_tmp% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Error: %ERRORLEVEL%                                    >> %file_log_tmp% 2>&1
echo Executed  %file_script_01%.                            >> %file_log_tmp% 2>&1


echo Error Level: %error_level%                             >> %file_log_tmp% 2>&1

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