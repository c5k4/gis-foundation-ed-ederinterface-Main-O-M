echo off

rem Script Name: EDG_ChangeDetection_EDER.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set Database=EDGMC_QA_DC1A
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CDHFTD"
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CDHFTD\PGE.BatchApplication.PGE_ChangeDetection_HFTD_FIA.exe"
set cmdLine1="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CDHFTD\PGE.BatchApplication.PGE_ChangeDetection_HFTD_FIA.exe"

set uname=UC4ADMIN
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD UC4ADMIN@EDGMC_QA_DC1A') do set "PWD=%%a"

set SQLScript_path=D:\edgisdbmaint\Copy_HFTD_FIA_EDER_to_EDGMC.sql

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
rem cd /d %cmdPath%                                             >> %file_log% 2>&1

echo Executing %cmdLine1%...                                >> %file_log% 2>&1
call %cmdLine1%                                             >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
echo Executed  %cmdLine1%.                                  >> %file_log% 2>&1

echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ===========================================================================

echo Executing Copy_HFTD_FIA_EDER_to_EDGMC on %Database%.....  >> %file_log% 2>&1

sqlplus -s %uname%/%PWD%@%Database% @%SQLScript_path% >> %file_log%  2>&1
set /A error_level=%error_level% + %ERRORLEVEL%             


echo Completed executing Copy_HFTD_FIA_EDER_to_EDGMC on %Database% >> %file_log% 2>&1


rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %cmdLine1% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
