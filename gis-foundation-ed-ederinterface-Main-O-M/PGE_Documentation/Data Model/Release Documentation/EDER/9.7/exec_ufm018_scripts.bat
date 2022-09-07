ufm018_datamodel_update_script.py
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@EDGISA1D') do set "Password=%%a"
sqlplus EDGIS/"%Password%"@EDGISA1D @ufm018_modelnames_update_script.sql