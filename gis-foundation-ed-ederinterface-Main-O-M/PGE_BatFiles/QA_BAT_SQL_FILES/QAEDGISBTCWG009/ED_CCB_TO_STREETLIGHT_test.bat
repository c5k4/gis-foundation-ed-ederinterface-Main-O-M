echo off
rem Script Name: EDG_CCB_to_Streetlight.bat

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
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
set emailUtilPath="send_email.py Streetlight_Weekly_data email_msg.txt"

set sqlDB=eder

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD PGEDATA@EDER') do set "sqlPWD=%%a"

set cmdPath="%~dp0"
set emailPath = "D:\edgisdbmaint"
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




REM echo Extracting DBF files from geodatabase...           >> %file_log% 2>&1
REM call %cmdLine2%                                             >> %file_log% 2>&1
REM set /A error_level=%error_level% + %ERRORLEVEL%
REM echo Executed SHP/DBF files extraction process.             >> %file_log% 2>&1
REM echo Error Level: %ERRORLEVEL%                              >> %file_log% 2>&1

REM echo Extracting SHP files from geodatabase...           >> %file_log% 2>&1
REM sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @ExportStlShp.sql 
REM call %cmdLine3%                                        >> %file_log% 2>&1
REM set /A error_level=%error_level% + %ERRORLEVEL%
REM echo Executed SHP/DBF files extraction process.        >> %file_log% 2>&1
REM echo Error Level: %ERRORLEVEL%  					   >> %file_log% 2>&1

REM ABHIJIT to ADD [START]
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CCB_STL_EXPORT"
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\CCB_STL_EXPORT\ccbstl.py"

rem ===========================================================================
rem Set path to Python
rem ---------------------------------------------------------------------------
set pythonexe=D:\Python27\ArcGIS10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.6\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.7\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.7\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.8\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.8\python.exe
if not exist %pythonexe% set pythonexe=D:\python27\arcgis10.9\python.exe
if not exist %pythonexe% set pythonexe=D:\Python27\ArcGISx6410.9\python.exe
rem ===========================================================================

cd /d %cmdPath%                                             >> %file_log% 2>&1
echo Executing %cmdLine%...                                 >> %file_log% 2>&1
call %pythonexe% %cmdLine%                                  >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Error Level: %ERRORLEVEL%  					   >> %file_log% 2>&1

REM ABHIJIT to ADD [END]

rem -------------------------------------------------------------------------

:End
rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
rem :ITGISIncidentTeam@pge.com