rem echo off
rem Script Name: EDER_DATAPROCESSING_02.bat

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName=EDER_DATAPROCESSING_03

rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------


call net stop %ServiceName%

ping -n 4 -w 1000 localhost   >nul

TASKKILL /F /FI "SERVICES eq  %ServiceName%"

call net start %ServiceName%


REM call D:\edgisdbmaint\ReSubmit_ReconcilePost_Failed_Session.bat

Exit