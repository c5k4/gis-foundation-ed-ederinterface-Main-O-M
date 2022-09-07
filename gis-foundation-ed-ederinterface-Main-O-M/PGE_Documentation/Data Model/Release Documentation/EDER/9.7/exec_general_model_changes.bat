general_data_model_changes.py
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@PGE1 ') do set "Password=%%a"
sqlplus EDGIS/"%Password%"@PGE1 @general_model_name_changes.sql
sqlplus EDGIS/"%Password%"@PGE1 @circuitnames.sql