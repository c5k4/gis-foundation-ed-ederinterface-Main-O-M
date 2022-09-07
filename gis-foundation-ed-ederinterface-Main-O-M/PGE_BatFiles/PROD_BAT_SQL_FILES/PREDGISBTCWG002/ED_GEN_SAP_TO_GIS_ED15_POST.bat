echo off
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

REM echo "Exit 0 is placed" >> %file_log% 2>&1

rem Script Name: EDG_GEN_GIS_to_SAP_ED15.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ED0015"
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ED0015\PGE.Interfaces.SAP.LoadData.exe" POSTPROCESS
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

rem echo "exit 0 is placed"  >> %file_log% 2>&1
rem exit 0

rem ---------------------------------------------------------------------------
rem Execution Validation.
rem ---------------------------------------------------------------------------

IF EXIST "D:\edgisdbmaint\log\ED15Count.txt" DEL /F "D:\edgisdbmaint\log\ED15Count.txt"

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD PGEDATA@EDER') do set "PGEDATA_PWD=%%a"

sqlplus -S PGEDATA/%PGEDATA_PWD%@EDER @"D:\edgisdbmaint\ED15CountCheck.sql" > D:\edgisdbmaint\log\ED15Count.txt

set /p str=< "D:\edgisdbmaint\log\ED15Count.txt"
set str=%str: =%

if %str%==0 (
	echo "No record for ED15 received today and hence exiting" >> %file_log% 2>&1
	exit 0
) 
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %cmdLine%                                              >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1


:End
rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
