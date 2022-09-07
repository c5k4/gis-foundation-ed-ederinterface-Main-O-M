@echo off
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%a-%%b)
For /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a:%%b)
echo Start Time: %mydate% %mytime%
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETSDEFILE SDE@PGE1') do set "DataConnection=%%a"
C:\Python27\ArcGIS10.2\Python.exe CompressDatabase.py %DataConnection%
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETSDEFILE EDGIS@PGE1') do set "DataConnection=%%a"
C:\Python27\ArcGIS10.2\Python.exe UnregisterAsVersioned.py %DataConnection%
C:\Python27\ArcGIS10.2\Python.exe DropUnnecessaryFields.py %DataConnection%
C:\Python27\ArcGIS10.2\Python.exe MigrateStorage.py %DataConnection%
C:\Python27\ArcGIS10.2\Python.exe RegisterAsVersioned.py %DataConnection%
C:\Python27\ArcGIS10.2\Python.exe CreateSpatialIndexes.py %DataConnection%
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%a-%%b)
For /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a:%%b)
echo End Time: %mydate% %mytime%