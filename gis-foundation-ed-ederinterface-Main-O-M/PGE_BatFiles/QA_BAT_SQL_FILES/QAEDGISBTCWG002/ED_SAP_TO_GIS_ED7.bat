echo off
rem Script Name: EDG_SAP_to_GIS_ED7.bat

rem exit 0

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ED07\PGE.Interfaces.LoadingDataInOracle.exe"
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ED07"

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
rem set Trigger_file=\\sfshare04-nas2\sfgispoc_data\INTERFACE\EDAMGIS_ED07_CONSUMER\INBOUND\*.csv
rem set Trigger_file_txt=\\sfshare04-nas2\sfgispoc_data\INTERFACE\EDAMGIS_ED07_CONSUMER\INBOUND\ED07_C_TRIGGER.TXT

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Set the cmdPath.
rem ---------------------------------------------------------------------------
cd /d %cmdPath%                                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Check Trigger file is present.
rem ---------------------------------------------------------------------------
rem if not exist %Trigger_file% (
rem  set /A error_level=1    
rem  echo Trigger file-csv is not present at %Trigger_file%... >> %file_log% 2>&1
rem echo %error_level%)
rem if not exist %Trigger_file_txt% (
rem  set /A error_level=2  
rem  echo %Trigger_file_txt% is not present. >> %file_log% 2>&1
rem  echo %error_level%)
rem if "%error_level%" neq "0" (set /A error_level=1
rem echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%)

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %cmdLine%                                              >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
