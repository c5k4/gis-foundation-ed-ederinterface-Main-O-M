echo off
rem Script Name: EDG_START_GDBM.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------

set ServiceName1=EDER_DATAPROCESSING_06
set ServiceName2=EDER_DATAPROCESSING_07

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
rem taskkill /S edgisbtcprd01 /U pge\itgisrelpradmin /P itgisRe!14 /F  /IM GeodatabaseManagerServices.exe /T

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
echo Starting GDBM Services...    
call sc config %ServiceName1% start= demand                  >> %file_log% 2>&1
call net start %ServiceName1%                                >> %file_log% 2>&1
call sc config %ServiceName2% start= demand                  >> %file_log% 2>&1
call net start %ServiceName2%                                >> %file_log% 2>&1
echo Started GDBM Services.                                  >> %file_log% 2>&1

                                
rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
