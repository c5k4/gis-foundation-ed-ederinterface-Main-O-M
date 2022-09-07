@echo off


@echo Running 
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 1
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 2
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 3
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 4
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 5
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 6
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 7
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 8
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 9
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 10
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 11
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 12
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 13
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 14
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 15
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 16
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 17
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 18
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 19
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 20
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 21


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
