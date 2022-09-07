echo off
rem ===========================================================================
rem Script Name: EDG_Delete_Anno_ED.bat
rem Created: June 02, 2015
rem Windows HEAD - BEGIN
rem ===========================================================================
rem Set script variables.
rem ---------------------------------------------------------------------------
rem Windows HEAD - END
set cmdLine=echo Windows
rem Windows TAIL - BEGIN

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

set ServiceName=EDER_POST_01

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\DELETEANNO"
set cmdLine=PGE.BatchApplication.DeleteAnnos.exe -50


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


echo Stopping GDBMPost EDER_POST_01 Services...                              >> %file_log% 2>&1
call net stop %ServiceNameED006%                                >> %file_log% 2>&1
echo Stopped GDBMPost EDER_POST_01 Services.                                 >> %file_log% 2>&1

cd /d %cmdPath%  
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %cmdLine%                                              >> %file_log% 2>&1

rem echo Stating GDBMPost ED006 Services...                              >> %file_log% 2>&1
rem call net start %ServiceNameED006%                                >> %file_log% 2>&1
rem echo Started GDBMPost ED006 Services.                                 >> %file_log% 2>&1

set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem Windows TAIL - END


