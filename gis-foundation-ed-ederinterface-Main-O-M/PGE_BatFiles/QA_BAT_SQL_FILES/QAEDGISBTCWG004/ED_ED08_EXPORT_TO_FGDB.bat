@echo off


@echo Running 
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" export_tofgdb.py ED08LINE_T ED08STRUCT_T
timeout /t 20 /nobreak > NUL


if %errorlevel% EQU 4 goto lowmemory
if %errorlevel% EQU 2 goto abort
if %errorlevel% EQU 1 goto exit 1

@echo "Data moved to s3 bucket successfully"

goto success

:lowmemory
echo invalid drive or command-line syntax. 
goto exit 1

:abort
echo You pressed CTRL+C to end the copy operation.
goto exit 1

:exit %~1
exit %~1

:success
echo WSOC FGDB AND CSV  loaded to s3 bucket Completed Successfully.
