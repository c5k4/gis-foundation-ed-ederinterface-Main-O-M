rem echo off
rem Script Name: EDG_EDER_GDBM_Reconcile_All.bat
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName1=GDBMReconcileOnlyEDGIS1P
set ServiceName2=GDBMReconcileOnlyEDGIS1P_1
set ServiceName3=GDBMReconcileOnlyEDGIS1P_2
set ServiceName4=GDBMReconcileOnlyEDGIS1P_3
set ServiceName5=GDBMReconcileOnlyEDGIS1P_4
set ServiceName6=GDBMReconcileOnlyEDGIS1P_5
set ServiceName7=GDBMReconcileOnlyEDGIS1P_6
set ServiceName8=GDBMReconcileOnlyEDGIS1P_7
set ServiceName9=GDBMReconcileOnlyEDGIS1P_8
set ServiceName10=GDBMReconcileOnlyEDGIS1P_9
set ServiceName11=GDBMReconcileOnlyEDGIS1P_10
set ServiceName12=GDBMReconcileOnlyEDGIS1P_11
set ServiceNameED006=GDBMServiceED006
set ServiceNameSubstationPost=GDBMPostService_EDSUB1P

set sqlCommand="D:\edgisdbmaint\Kill_Connection.sql"
set DbType=edmaint
set sqlUSR=uc4admin
set sqlPWD=uc4admin
set sqlDB=edgis1p

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
rem set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
rem set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Run Clean Session Manager
rem ---------------------------------------------------------------------------

set path_exe="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Orphan Version Cleanup Tool"
set file_name_exe=Telvent.PGE.OrphanVersionCleanup.exe -c Electric
set file_exe=%path_exe%\%file_name_exe%

set command=call %file_exe%
echo Executing %command%... >> %file_log% 2>&1
               %command%    >> %file_log% 2>&1
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %command%.   >> %file_log% 2>&1

rem ---------------------------------------------------------------------------

echo Stopping GDBMPost Services...                              >> %file_log% 2>&1
call net stop %ServiceNameED006%                                >> %file_log% 2>&1
call net stop %ServiceNameSubstationPost%						>> %file_log% 2>&1
echo Stopped GDBMPost Services.                                 >> %file_log% 2>&1

echo Starting GDBM Services...                              >> %file_log% 2>&1
call net start %ServiceName1%                               >> %file_log% 2>&1
call net start %ServiceName2%                               >> %file_log% 2>&1
call net start %ServiceName3%                               >> %file_log% 2>&1
call net start %ServiceName4%                               >> %file_log% 2>&1
call net start %ServiceName5%                               >> %file_log% 2>&1
call net start %ServiceName6%                               >> %file_log% 2>&1

call sc \\edgisbtcprd03 start %ServiceName7%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd03 start %ServiceName8%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd03 start %ServiceName9%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 start %ServiceName10%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 start %ServiceName11%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 start %ServiceName12%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

set /A error_level=%error_level% + %ERRORLEVEL%
echo Started GDBM Services.                                 >> %file_log% 2>&1



echo Executing %sqlCommand%...                              >> %file_log% 2>&1  
sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
 
rem Maximum value in below for loop indicates number of minutes to wait to stop service
for /L %%A IN (1,1,30) do (      
     for /f %%i in ('D:\Python27\ArcGIS10.2\python.exe D:\edgisdbmaint\GetVersionnumber.py -c %DbType% ') do ( 
echo Version left to reconcile  %%i 			>> %file_log% 2>&1
	 if %%i == 0 goto Stop_Service 
	 if %%i == "0" goto Stop_Service 
	 )
     ping -n 60 -w 1000 localhost   >nul	 
)

:Stop_Service
rem set /A error_level=%error_level% + %ERRORLEVEL%
echo Stopping GDBM Reconcile Services...                              >> %file_log% 2>&1
call net stop %ServiceName1%                                >> %file_log% 2>&1
call net stop %ServiceName2%                                >> %file_log% 2>&1
call net stop %ServiceName3%                                >> %file_log% 2>&1
call net stop %ServiceName4%                                >> %file_log% 2>&1
call net stop %ServiceName5%                                >> %file_log% 2>&1
call net stop %ServiceName6%                                >> %file_log% 2>&1

call sc \\edgisbtcprd03 stop %ServiceName7%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd03 stop %ServiceName8%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd03 stop %ServiceName9%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 stop %ServiceName10%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 stop %ServiceName11%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

call sc \\edgisbtcprd04 stop %ServiceName12%                                >> %file_log% 2>&1
ping -n 30 -w 1000 localhost   >nul

set /A error_level=%error_level% + %ERRORLEVEL%
echo Stopped GDBM Reconcile Services.                                 >> %file_log% 2>&1

echo Starting GDBMPost  Services...                              >> %file_log% 2>&1
call net start %ServiceNameED006%                                >> %file_log% 2>&1
call net start %ServiceNameSubstationPost%						>> %file_log% 2>&1
echo Started GDBMPost Services.                                 >> %file_log% 2>&1

echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
