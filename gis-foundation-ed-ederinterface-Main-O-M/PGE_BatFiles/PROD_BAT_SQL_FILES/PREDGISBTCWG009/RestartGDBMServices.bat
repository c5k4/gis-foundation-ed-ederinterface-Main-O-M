@Echo Off
rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName1=EDER_RECONCILE_04
set ServiceName2=EDERSUB_RECONCILE_04


rem This statement is required for variables like (!variable!) inside for loop.
rem setlocal enabledelayedexpansion
rem set error_level=0
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set date_time=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2%_%datetime:~8,2%-%datetime:~10,2%-%datetime:~12,2%
set file_script=%~f0
set path_log=%~dp0log
set file_log=%path_log%\%~n0_%date_time%.log
echo %file_script%
echo %path_log%
echo %file_log%

rem ---------------------------------------------------------------------------
rem Create path to log file, delete log if exists
rem ---------------------------------------------------------------------------
if not exist %path_log% mkdir %path_log%
rem set /A error_level=%error_level% + %ERRORLEVEL%
if not exist %path_log% set path_log=%path_current%
if exist %file_log% del %file_log%


rem ---------------------------------------------------------------------------

SC queryex "%ServiceName1%"|Find "STATE"|Find /v "Started">Nul&&(

echo Starting EDER_RECONCILE_04 Services...                              >> %file_log% 2>&1
call net start %ServiceName1%                           >> %file_log% 2>&1
echo Started EDER_RECONCILE_04 Services...     >> %file_log% 2>&1   

)
SC queryex "%ServiceName2%"|Find "STATE"|Find /v "Started">Nul&&(

echo Starting EDERSUB_RECONCILE_04 Services...                              >> %file_log% 2>&1
call net start %ServiceName2%                               >> %file_log% 2>&1
echo Started EDERSUB_RECONCILE_04 Services...     >> %file_log% 2>&1 

)


set /A error_level=%error_level% + %ERRORLEVEL%



    rem Net start "%ServiceName%">nul||(
        rem Echo "%ServiceName%" wont start 
       rem  exit /b 1
    )
    echo "%ServiceName1%" started
    exit /b 0
)||(
    echo "%ServiceName1%" working
    exit /b 0
)
echo Error Level: %error_level%                             >> %file_log% 2>&1

rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% >> %file_log% 2>&1
exit /b %error_level%