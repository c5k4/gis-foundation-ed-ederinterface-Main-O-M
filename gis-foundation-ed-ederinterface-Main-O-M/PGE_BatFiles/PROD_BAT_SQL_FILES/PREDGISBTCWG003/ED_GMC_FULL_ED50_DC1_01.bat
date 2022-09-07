echo off
rem Script Name: EDG_FULL_ED50_DC1_01.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\DMS"
set cmdLine=PGE.Interface.Integration.DMS.Extractor.exe -a

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

tasklist /fi "imagename eq PGE.Interface.Integration.DMS.Extractor.exe" |find ":" > nul  >> %file_log% 2>&1

if errorlevel 1 taskkill /f /im "PGE.Interface.Integration.DMS.Extractor.exe"    >> %file_log% 2>&1

cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %cmdLine%                                              >> %file_log% 2>&1
if "%ERRORLEVEL%" neq "2" set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%                              >> %file_log% 2>&1

set EDGIS_Database=EDGMC_PR_DC1B
set uname=UC4ADMIN
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD UC4ADMIN@EDGMC_PR_DC1B') do set "UC4ADMIN_PWD=%%a"

set SQLScript_path=D:\edgisdbmaint\ED_DMS_TYPE_UPDATE.sql

echo Executing ED_DMS_TYPE_UPDATE Processing on %EDGIS_Database%.....  >> %file_log% 2>&1

sqlplus -s %uname%/%UC4ADMIN_PWD%@%EDGIS_Database% @%SQLScript_path% >> %file_log%  2>&1
set /A error_level=%error_level% + %ERRORLEVEL% 

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
