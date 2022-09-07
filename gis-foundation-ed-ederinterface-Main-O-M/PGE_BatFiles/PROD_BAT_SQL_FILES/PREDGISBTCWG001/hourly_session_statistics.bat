echo off
rem ===========================================================================
rem hourly_session_statistics.bat
rem ---------------------------------------------------------------------------
rem This script produces a report of blocked sessions and emails 
rem it to the Business.
rem 
rem MAINTENANCE HISTORY
rem Date         Author    Description
rem 09/17/2019   a7m6     Initial version
rem         
rem ===========================================================================
D:
set subject_mail="Electric Distribution GIS Session Statistics Report:Production"
cd\EDGISDBMAINT\Log\Session_Info

zip -m Session_Info_Log_%date:~10,4% Session_Info*

cd\EDGISDBMAINT
echo Executing Query
for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD UC4ADMIN@EDER_PR') do set "PASSWORD=%%a"
sqlplus -S uc4admin/%PASSWORD%@eder_pr @"D:\edgisdbmaint\hourly_session_statistics.sql" >> D:\edgisdbmaint\log\Session_Info\Session_Info_Log_%date:~4,2%%date:~7,2%%date:~10,4%.csv 2>&1

echo Query Execution completed

if ERRORLEVEL 1 goto ReportError

   echo Session Info Log Created Successfully
   echo.
   goto SendReport  
:ReportError  
   echo Error Creating Session Info Report - Code: %ERRORLEVEL%
   goto Done

:SendReport
"D:\Python27\ArcGIS10.8\python.exe" "hourly_session_statistics.py" %subject_mail% "D:\edgisdbmaint\Log\Session_Info\Session_Info_Log_%date:~4,2%%date:~7,2%%date:~10,4%.csv" 2>&1

if ERRORLEVEL 1 goto SendReportError

   echo Session Info Report Successfully Sent
   echo.
   goto Done
  
:SendReportError  
   echo Error Sending Session Info Report - Code: %ERRORLEVEL%
    
:Done

exit /b %ERRORLEVEL%