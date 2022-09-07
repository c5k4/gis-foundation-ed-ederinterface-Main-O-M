rem echo off

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName1=EDER_DATAPROCESSING_06
set ServiceName2=EDER_DATAPROCESSING_07
rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------


sc query %ServiceName1%| find "RUNNING" >nul 2>&1 || call net start %ServiceName1%

sc query %ServiceName2%| find "RUNNING" >nul 2>&1 || call net start %ServiceName2%