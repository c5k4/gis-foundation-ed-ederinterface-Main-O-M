echo off
rem Script Name: EDG_CCB_to_Streetlight.bat

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
setlocal DisableDelayedExpansion enableextensions
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set csvFile=\\rcnas01-smb\edgisrearch-fs01\Streetlight\data_from_rss\SL-3679.csv
set sqlCtlFile=load_cdxdata.ctl
set fieldDataPath=\\rcnas01-smb\edgisrearch-fs01\Streetlight\data_to_field
set sqlCommand=update_ccb_data_stl.sql
set sqlUSR=pgedata

set sqlDB=eder

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD PGEDATA@EDER') do set "sqlPWD=%%a"

set cmdPath="%~dp0"
set sqlCtlFile="%~dp0%sqlCtlFile%"
set cmdLine1=sqlldr "%sqlUSR%/%sqlPWD%@%sqlDB% control=%sqlCtlFile% log=%file_log% direct=true errors=1000000"
set cmdLine2=ExportStlDbf.bat %fieldDataPath% %sqlUSR% %sqlPWD% %sqlDB%
set cmdLine3=ExportStlShp.bat %fieldDataPath% %sqlUSR% %sqlPWD% %sqlDB%

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

echo Loading CSV file using sqlldr and ctl file...          >> %file_log% 2>&1
rem setlocal DISABLEDELAYEDEXPANSION
call %cmdLine1%                 
rem setlocal ENABLEDELAYEDEXPANSION                             
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  CSV file loading process.                    >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%                              >> %file_log% 2>&1
if "%error_level%" neq "0" goto End

echo Executing %sqlCommand%...                              >> %file_log% 2>&1
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%                              >> %file_log% 2>&1
if "%error_level%" neq "0" goto End

echo Extracting DBF files from geodatabase...           >> %file_log% 2>&1
call %cmdLine2%                                             >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed SHP/DBF files extraction process.             >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%                              >> %file_log% 2>&1

echo Extracting SHP files from geodatabase...           >> %file_log% 2>&1
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @ExportStlShp.sql 
call %cmdLine3%                                        >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed SHP/DBF files extraction process.        >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%  					   >> %file_log% 2>&1

echo Copying csv file to backup file...           >> %file_log% 2>&1
set Ftime=%datetime:~0,4%_%datetime:~4,2%_%datetime:~6,2%_%datetime:~8,2%%datetime:~10,2%%datetime:~12,2%
For %%A in ("%csvFile%") do (
    Set Folder=%%~dpA
    Set Name=%%~nxA
)
REM echo path is %Folder%
REM echo Filename is %Name%
copy %csvFile% %Folder%backup\%Name%_%Ftime%
set /A error_level=%error_level% + %ERRORLEVEL%
echo Copied csv file to back file		        >> %file_log% 2>&1
echo Error Level: %ERRORLEVEL%  					   >> %file_log% 2>&1

rem -------- email notification for users  ----------------------------------
echo Email Notification for users...           		>> %file_log% 2>&1
D:\Python27\ArcGIS10.8\python send_email.py  "Streetlight Weekly data"  email_msg.txt
set /A error_level=%error_level% + %ERRORLEVEL%
echo Error Level: %ERRORLEVEL%  					   >> %file_log% 2>&1
rem -------------------------------------------------------------------------

:End
rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
