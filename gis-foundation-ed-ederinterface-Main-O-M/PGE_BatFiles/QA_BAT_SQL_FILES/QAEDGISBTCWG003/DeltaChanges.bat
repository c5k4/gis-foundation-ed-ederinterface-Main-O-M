echo off
rem Script Name: DetlaChanges.bat
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set uname=INTDATAARCHset Database=EDGMC
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD INTDATAARCH@EDGMC') do set "PWD=%%a"
set SQLScript_path=D:\edgisdbmaint\DeltaChanges.sql
rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%
echo Executing DeltaChanges on %Database%..... >> %file_log% 2>&1
sqlplus -s %uname%/%PWD%@%Database% @%SQLScript_path% >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Completed executing DeltaChanges on %Database% >> %file_log% 2>&1
rem ---------------------------------------------------------------------------
rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %cmdLine1% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%

