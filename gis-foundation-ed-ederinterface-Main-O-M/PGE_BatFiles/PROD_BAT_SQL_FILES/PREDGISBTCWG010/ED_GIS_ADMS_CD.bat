echo off
setlocal DisableDelayedExpansion enableextensions
REM Refer link for using the above set local statement with DisableDelayedExpansion - https://stackoverflow.com/questions/22278456/enable-and-disable-delayed-expansion-what-does-it-do
rem Script Name: EDG_ChangeDetection_ADMS.bat

rem exit 0

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Added by Kritika on 9/5/2014
rem ---------------------------------------------------------------------------
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdLine=echo Windows
set error_level=0

set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Change Detection"
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Change Detection\PGE.BatchApplication.ChangeDetection.exe" -c ADMS_CD.config

set edsett_uname=EDSETT
set EDSETT_PWD=""
set EDGIS_Database=EDGMC_PR
set SQLScript_path=D:\edgisdbmaint\EDSETT.sql

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

REM rem ---------------------------------------------------------------------------
REM rem Create path to log file, delete log if exists
REM rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

REM REM echo "Exit 0 is placed" >> %file_log% 2>&1
REM REM exit 0

REM rem ---------------------------------------------------------------------------
REM rem Run the executable.
REM rem ---------------------------------------------------------------------------
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDSETT@EDGMC_PR') do set "EDSETT_PWD=%%a"
sqlplus -s %edsett_uname%/%EDSETT_PWD%@%EDGIS_Database% @%SQLScript_path% >> %file_log%  2>&1

set /A error_level=%error_level% + %ERRORLEVEL%

cd /d %cmdPath%                                             >> %file_log% 2>&1

echo Executing %cmdLine%...                                 >> %file_log% 2>&1
rem call %cmdLine%                                          >> %file_log% 2>&1
%cmdLine%                                                   >> %file_log% 2>&1
echo Successfully returned from application code            >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1

echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
