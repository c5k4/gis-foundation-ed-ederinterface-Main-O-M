rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
echo off

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
rem echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
rem exit /b %error_level%
rem OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

echo off
rem Script Name: EDG_SCHM_EDER_PostVersion.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ReconcileVersion"
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ReconcileVersion\PGE.BatchApplication.ReconcilVersions.exe" 
set file_Result_and_Exception_File_src="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.ReconcileVersion\Result_and_Exception_File.txt"
set file_Result_and_Exception_File_dst=D:\edgisdbmaint\log\Result_and_Exception_File.txt
set file_Result_and_Exception_File=Result_and_Exception_File


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
cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1

REM exit 0

rem call %cmdLine%                                          >> %file_log% 2>&1
%cmdLine%                                                   >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   >> %file_log% 2>&1
echo Error Level: %error_level%                             >> %file_log% 2>&1
if exist %file_Result_and_Exception_File_dst% (
   echo Executing ren %file_Result_and_Exception_File_dst% %file_Result_and_Exception_File%_%date_time%.txt ... >> %file_log% 2>&1
                 ren %file_Result_and_Exception_File_dst% %file_Result_and_Exception_File%_%date_time%.txt     >> %file_log% 2>&1
   echo Executed  ren %file_Result_and_Exception_File_dst% %file_Result_and_Exception_File%_%date_time%.txt .   >> %file_log% 2>&1
   set /A error_level=%error_level% + %ERRORLEVEL%
  )


rem ===========================================================================
rem Copy the local Result_and_Exception_File  file to log location.
rem ---------------------------------------------------------------------------

    echo Executing copy %file_Result_and_Exception_File_src% %file_Result_and_Exception_File_dst% /Y... >> %file_log% 2>&1
                   copy %file_Result_and_Exception_File_src% %file_Result_and_Exception_File_dst% /Y    >> %file_log% 2>&1
    echo Executed  copy %file_Result_and_Exception_File_src% %file_Result_and_Exception_File_dst% /Y.   >> %file_log% 2>&1
    set /A error_level=!error_level! + !ERRORLEVEL!

rem ===========================================================================

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
