@echo off

for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@EDGMC_QA') do set "PASSWORD=%%a"

sqlplus -s EDGIS/%PASSWORD%@EDGMC @"D:\edgisdbmaint\ROBC_INIT_TABLECOPY.sql"