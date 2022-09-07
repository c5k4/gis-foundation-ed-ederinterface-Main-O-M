echo off
rem Script Name: EDG_START_GDBM.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------

set ServiceName1=EDER_DATAPROCESSING_04
set ServiceName2=EDER_DATAPROCESSING_05

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

taskkill /F /IM GeodatabaseManagerServices.exe

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------
IF EXIST "D:\edgisdbmaint\track_dppost_stop.txt" (
echo Stopping GDBM Services...    
call sc config %ServiceName1% start= Disabled                 >> %file_log% 2>&1
call net stop %ServiceName1%                                  >> %file_log% 2>&1
call sc config %ServiceName2% start= disabled                 >> %file_log% 2>&1
call net stop %ServiceName2%                                  >> %file_log% 2>&1
echo Stopped  GDBM Services.                                  >> %file_log% 2>&1
del D:\edgisdbmaint\track_dppost_stop.txt
)
rem set cmdPath="D:\GeodatabaseManager\GDBM_Logs" 
rem set batCommand_INIT="D:\GeodatabaseManager\GDBM_Logs\Move.bat"
rem cd /d %cmdPath%                                             >> %file_log% 2>&1

call D:\GeodatabaseManager\GDBM_Logs\Move.bat  >> %file_log% 2>&1
                                

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
