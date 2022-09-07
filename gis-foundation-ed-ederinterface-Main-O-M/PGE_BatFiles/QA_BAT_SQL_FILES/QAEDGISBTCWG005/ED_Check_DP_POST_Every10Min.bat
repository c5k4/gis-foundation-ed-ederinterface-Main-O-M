rem echo off

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName1=EDER_DATAPROCESSING_04
set ServiceName2=EDER_DATAPROCESSING_05
rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------


sc query %ServiceName1%| find "RUNNING" >nul 2>&1 || call net start %ServiceName1%

sc query %ServiceName2%| find "RUNNING" >nul 2>&1 || call net start %ServiceName2%