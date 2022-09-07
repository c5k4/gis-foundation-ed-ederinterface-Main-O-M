rem echo off

rem ---------------------------------------------------------------------------
rem Set script variables.
rem ---------------------------------------------------------------------------
set ServiceName1=EDER_DATAPROCESSING_01
set ServiceName2=EDERSUB_DATAPROCESSING_01
set ServiceName3=EDER_POST_01
set ServiceName4=EDERSUB_POST_01
rem ---------------------------------------------------------------------------
rem Run the executable.
rem ---------------------------------------------------------------------------


sc query %ServiceName1%| find "RUNNING" >nul 2>&1 || call net start %ServiceName1%

sc query %ServiceName2%| find "RUNNING" >nul 2>&1 || call net start %ServiceName2%

sc query %ServiceName2%| find "RUNNING" >nul 2>&1 || call net start %ServiceName3%

sc query %ServiceName2%| find "RUNNING" >nul 2>&1 || call net start %ServiceName4%