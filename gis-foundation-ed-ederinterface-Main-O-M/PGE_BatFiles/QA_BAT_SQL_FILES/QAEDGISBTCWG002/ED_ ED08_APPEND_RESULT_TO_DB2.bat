@echo off


@echo Running 


start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 22
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 23
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 24
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 25
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 26
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 27
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 28
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 29
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 30
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 31
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 32
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 33
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 34
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 35
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 36
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 37
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 38
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 39
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 40
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 41
start "" /d  "D:\ED08" cmd /C "D:\python27\ArcGIS10.8\python.exe" load_data.py 42



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
