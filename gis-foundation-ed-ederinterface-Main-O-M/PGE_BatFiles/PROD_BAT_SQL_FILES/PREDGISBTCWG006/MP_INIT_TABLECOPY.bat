@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@EDGMC') do set "PASSWORD=%%a"

sqlplus -s EDGIS/%PASSWORD%@EDGMC @"D:\edgisdbmaint\MP_INIT_TABLECOPY.sql"