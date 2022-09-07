echo off

rem ===========================================================================
rem ED_DELETE_VERSION_PUB_DC1B.bat
rem ---------------------------------------------------------------------------
rem This SQL Script
rem SQLPlus Connection with database
rem on a PG&E EDPUB GIS database.
rem 
rem Stop any ArcGIS Server map service that is using the target database
rem prior to running this script.
rem 
rem Open a command window.
rem Navigate to script location.
rem Usage:  ED_DELETE_VERSION_PUB_DC1B.bat
rem         
rem Author: Vedant Sood (V3SF)
rem ===========================================================================

rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
set error_level=0

set file_path="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PublicationUpdate\Delete_Version"
set cmdinifile=ini.cmd
set inifile=EDPUB_DC1Bconfig.ini

for /f "delims=" %%a in ('call %cmdinifile% %inifile% DbConfig sqlUSR') do (
    set sqlUSR=%%a
)

for /f "delims=" %%a in ('call %cmdinifile% %inifile% DbConfig sqlDB') do (
    set sqlDB=%%a
)

for /f %%a in ('PGE_DBPasswordManagement GETPASSWORD SDE@EDPUB_QA_DC1B') do set "sqlPWD=%%a"

rem echo Username:%sqlUSR%
rem echo Password:%sqlPWD%
rem echo Database:%sqlDB%

set path_current="%~dp0"
set filename_current=%~0
set file_current=%path_current%%filename_current%
set path_log=%path_current%log

set prepsqlCommand=EDPUBpreparedeleteVesrions.sql
set delsqlCommand=EDPUBdeleteVersions.sql

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


rem ---------------------------------------------------------------------------
rem Execute Prepare Delete Vesrions List Sql.
rem ===========================================================================

cd /d %file_path%                                              
echo Executing %prepsqlCommand%... >> %file_log_tmp% 2>&1

sqlplus -s %sqlUSR%/"%sqlPWD%"@%sqlDB% @%prepsqlCommand%   >> %file_log_tmp% 2>&1
       
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %prepsqlCommand%.  >> %file_log_tmp% 2>&1                              
echo Error Level: %error_level%   >> %file_log_tmp% 2>&1                          

rem ===========================================================================

rem ---------------------------------------------------------------------------
rem Execute fetched Sql of delete versions.
rem ---------------------------------------------------------------------------
cd /d %file_path%                                             
echo Executing %delsqlCommand%...  >> %file_log_tmp% 2>&1    
                        
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%delsqlCommand%    >> %file_log_tmp% 2>&1

echo Executed  %delsqlCommand%    >> %file_log_tmp% 2>&1                            

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