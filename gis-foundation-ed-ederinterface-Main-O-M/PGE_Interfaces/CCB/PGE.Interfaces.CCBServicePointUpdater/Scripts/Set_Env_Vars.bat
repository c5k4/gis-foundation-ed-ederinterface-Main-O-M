@ECHO OFF

set DB_INSTANCE=PGE1

set EDGIS_UID=EDGIS
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@PGE1') do set "EDGIS_PWD=%%a"

set GIS_UID=GIS_I
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD GIS_I@PGE1') do set "GIS_PWD=%%a"

for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETSDEFILE SDE@EDGISA1D') do set "LOCSDECONN=%%a"
set CCBVERSION=CCBWithViews

echo DB_INSTANCE  is %DB_INSTANCE%
echo EDGIS_UID    is %EDGIS_UID%
echo EDGIS_PWD    is %EDGIS_PWD%
echo GIS_UID      is %GIS_UID%
echo GIS_PWD      is %GIS_PWD%
echo LOCSDECONN   is %LOCSDECONN%
echo CCBVERSION   is %CCBVERSION%
