@echo off 

rem ---------------------------------------------------------------------------
rem ---- Check case sensitivity (lower case preferable) ----
set ShrDir=\\RcShare03-NAS2\Gis_DBArchive
set DBInstance=%1
set DBUser=%6
set DBPass=%7
rem set DBUser=uc4admin
rem set DBPass=uc4admin
set UXUser=%2
set UXPass=%3
set UXPath=%4
set DBServer=%5
set IsState0=%8

rem ---------------------------------------------------------------------------

rem This statement is required for variables like (!variable!) inside for loop.
setlocal enabledelayedexpansion
set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%_%datetime:~10,2%
set file_script=%~f0
set path_log=%~dp0log
rem set file_log=%path_log%\%~n0_%DBInstance%_%date_time%.log
set file_log=%path_log%\FFExport_DB_%DBInstance%_%date_time%.log
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%
rem ---------------------------------------------------------------------------
CD /D %~dp0

set filename_dmp=%DBInstance%_%date_time%.dmp
set filename_log=%DBInstance%_%date_time%.log

echo Creating Directory in Oracle....                                             	>> %file_log% 2>&1
rem  sqlplus -S %DBUser%/%DBPass%@%DBInstance% @1_CreateDirectory.sql %UXPath%       >> %file_log% 2>&1

echo Exporting DB to dump file....                                                  >> %file_log% 2>&1
expdp %DBUser%/%DBPass%@%DBInstance% compression=all full=y reuse_dumpfiles=y directory=UC4_EXPORT_DIR dumpfile=%filename_dmp% logfile=%filename_log% flashback_time=systimestamp   >> %file_log% 2>&1

rem expdp %DBUser%/%DBPass%@%DBInstance% compression=all schemas=sde reuse_dumpfiles=y directory=UC4_EXPORT_DIR dumpfile=%filename_dmp% logfile=%filename_log% flashback_time=systimestamp   >> %file_log% 2>&1

rem expdp %DBUser%/%DBPass%@%DBInstance% compression=all tables=edgis.switch reuse_dumpfiles=y directory=UC4_EXPORT_DIR dumpfile=%filename_dmp% logfile=%filename_log% flashback_time=systimestamp   >> %file_log% 2>&1
rem etgis.t_fuse LBGIS.ZIPCODE

rem set /A error_level=%error_level% + %ERRORLEVEL%
rem if "%error_level%" neq "0" goto :EOF
rem ---------------------------------------------------------------------------

echo Transferring dump file....                                                  >> %file_log% 2>&1
rem if not exist B:\  NET USE B: %ShrDir%
set yyyymmdd=%datetime:~0,4%%datetime:~4,2%%datetime:~6,2%
for /f "tokens=2 delims==" %%Q in ('wmic path win32_localtime get quarter /value') do set Qtr=%%Q
set syst=%DBInstance:~0,5%
set yyyy=%datetime:~0,4%

if %IsState0%==S0  set DestiDir=%ShrDir%\Data_Historian\%yyyymmdd%_S0\%syst%
if %IsState0%==QN  set DestiDir=%ShrDir%\Data_Historian\%yyyymmdd%_Q%Qtr%\%syst%

set ADir=%ShrDir%\Data_Historian\Annual_%yyyy%\%syst%
echo %DestiDir% 
mkdir %DestiDir%

echo Transfer Files from DB server to Shared folder....                                                         	>> %file_log% 2>&1
"WinSCP\winscp" /command "open scp://%UXUser%:%UXPass%@%DBServer%/" "cd %UXPath%" "get %filename_dmp% %DestiDir%\" "get %filename_log% %DestiDir%\" "close" "exit"  >> %file_log% 2>&1
 set /A error_level=%error_level% + %ERRORLEVEL%
 if "%error_level%" neq "0" goto :EOF

REM if "%Qtr%"=="4" ( if not exist %ADir% mkdir %ADir% ) 
REM if "%Qtr%"=="4" ( echo Copy files to Annual Archive Directory .... )								>> %file_log% 2>&1
REM if "%Qtr%"=="4" ( copy %DestiDir%\*.* %ADir%\  )													>> %file_log% 2>&1

echo Files for deletion from Quaterly folder....                                                         	>> %file_log% 2>&1
for /d %%a in ( %ShrDir%\Data_Historian\*Q* ) do forfiles /p %%a /s /m *.* /c "cmd /c echo @path" /d -730     >> %file_log% 2>&1 				
for /d %%a in ( %ShrDir%\Data_Historian\*Q* ) do forfiles /p %%a /s /m *.* /c "cmd /c del @path"  /d -730	 	>> %file_log% 2>&1

REM echo Files for deletion from Annual folder.... 															>> %file_log% 2>&1
REM for /d %%a in (%ShrDir%\Data_Historian\A*) do forfiles /p %%a /s /m *.* /c "cmd /c echo @path"    /d -3650	>> %file_log% 2>&1
REM for /d %%a in (%ShrDir%\Data_Historian\A*) do forfiles /p %%a /s /m *.* /c "cmd /c del @path"     /d -3650	>> %file_log% 2>&1

rem ---------------------------------------------------------------------------
echo Deleting dump log files from export folder....                                                         	>> %file_log% 2>&1
if exist %DestiDir%\%filename_dmp% ( "WinSCP\winscp" /command "open scp://%UXUser%:%UXPass%@%DBServer%/" "cd %UXPath%" "rm %filename_dmp%" "rm %filename_log%" "close" "exit" ) >> %file_log% 2>&1
 set /A error_level=%error_level% + %ERRORLEVEL%
 if "%error_level%" neq "0" goto :EOF
rem ---------------------------------------------------------------------------

:End
rem set /A error_level=%error_level% + %ERRORLEVEL%
rem ---------------------------------------------------------------------------
rem if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% 							>> %file_log% 2>&1	
rem exit /b %error_level%

rem ---------------------------------------------------------------------------

