echo off
rem Script Name: EDG_MAPPROD_INTIALIZE.bat
rem exit 0
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Added by Venkat on 9/5/2014
rem ---------------------------------------------------------------------------
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
REM exit 0

set sqlUSR=gis_i
set sqlPWD=GIS!wrz39%ged1t
set sqlDB=EDGED1T
set sqlCommand="D:\edgisdbmaint\UpdateSchematicChangeDetectionGrid.sql"

rem set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Change Detection"
rem set cmdLine2="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 1.0\MapProduction.exe" 1
set cmdLine3="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.exe" -c "D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.exe.config" -i
rem set cmdLine4="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.exe" -c "D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Map Production 2.0\PGE.Interfaces.MapProductionAutomation.Schematics.exe.config" -i

rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
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

REM rem  ---------------------------------------------------------------------------
rem map_init_copy
rem echo ""Executing MP_INIT_TABLECOPY from EDGEM1T to EDGED1T"" 						>> %file_log% 2>&1

rem set cmdPath="D:\edgisdbmaint"
rem set batCommand_INIT="D:\edgisdbmaint\MP_INIT_TABLECOPY.bat"
rem cd /d %cmdPath%                                             >> %file_log% 2>&1
rem # echo Executing %sqlCommand%...                              >> %file_log% 2>&1
rem call %batCommand_INIT%     									>> %file_log% 2>&1
REM #set /A error_level=%error_level% + %ERRORLEVEL%
rem echo Executed  %batCommand_INIT%.                           >> %file_log% 2>&1
rem echo Error Level: %error_level%                             >> %file_log% 2>&1
REM rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------
rem cd /d %cmdPath%                                             >> %file_log% 2>&1


REM echo Executing %cmdLine2%...                                >> %file_log% 2>&1
REM call %cmdLine2%                                             >> %file_log% 2>&1
REM echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
REM echo Executed  %cmdLine2%.                                  >> %file_log% 2>&1

REM echo Error Level: %error_level%                             >> %file_log% 2>&1

echo Executing %cmdLine3%...                                >> %file_log% 2>&1
call %cmdLine3%                                             >> %file_log% 2>&1
echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
echo Executed  %cmdLine3%.                                  >> %file_log% 2>&1

echo Error Level: %error_level%                             >> %file_log% 2>&1


rem echo Executing %cmdLine2%...                                >> %file_log% 2>&1
rem call %cmdLine2%                                             >> %file_log% 2>&1
rem echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
rem echo Executed  %cmdLine2%.                                  >> %file_log% 2>&1

rem echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ******************** Commented on 19th July 2022 to skip map initialization of schematic map production********************

rem echo Executing %cmdLine4%...                                >> %file_log% 2>&1
rem call %cmdLine4%                                             >> %file_log% 2>&1
rem echo Error: %ERRORLEVEL%                                    >> %file_log% 2>&1
rem echo Executed  %cmdLine4%.                                  >> %file_log% 2>&1

rem echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ***************************************************************************************************************************

rem ---------------------------------------------------------------------------
rem Added by Tarique on 4/15/2016
rem Run the executable.

REM rem ---------------------------------------------------------------------------
REM rem Set script variables.
REM rem ---------------------------------------------------------------------------

rem set cmdPath="D:\edgisdbmaint"
rem cd /d %cmdPath%                                             >> %file_log% 2>&1
rem echo Executing %sqlCommand%...                              >> %file_log% 2>&1
rem sqlplus -s %sqlUSR%/%sqlPWD%@%sqlDB% @%sqlCommand%          >> %file_log% 2>&1
REM set /A error_level=%error_level% + %ERRORLEVEL%
rem echo Executed  %sqlCommand%.                                >> %file_log% 2>&1
rem echo Error Level: %error_level%                             >> %file_log% 2>&1

REM rem ---------------------------------------------------------------------------


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%
