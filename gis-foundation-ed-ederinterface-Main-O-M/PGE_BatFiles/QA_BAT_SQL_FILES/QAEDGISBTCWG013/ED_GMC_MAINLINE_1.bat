rem exit 0

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set cmdLine="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE Mainline Indicator Batch\PGE.BatchApplication.MainlineIndicator.exe"
set cmdPath="D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE Mainline Indicator Batch"

rem ---------------------------------------------------------------------------
rem Set the cmdPath.
rem ---------------------------------------------------------------------------
cd /d %cmdPath%                                             

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------

echo Executing %cmdLine%...                                 
call %cmdLine%                                              
set /A error_level=%error_level% + %ERRORLEVEL%
echo Executed  %cmdLine%.                                   
echo Error Level: %error_level%                             


rem ---------------------------------------------------------------------------
rem Return the error level. Convert non-zero error to 1 for UC4 purpose.
rem ---------------------------------------------------------------------------
if "%error_level%" neq "0" set /A error_level=1
echo Exiting %file_script% with error_level = %error_level% 
exit /b %error_level%
