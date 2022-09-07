@ECHO OFF
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD EDGIS@PGE1') do set "SCRIPT_GDB_LOCATION=%%a"
01_Add_VersionDeletePoint_To_DB.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
02_Add_VersionDeleteLine_To_DB.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
03_Add_Field_To_Object_FeatureClassID.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
04_Add_Field_To_Object_FeatureGUID.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
05_Add_Field_To_Object_DateDeleted.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
06_Add_Field_To_Object_CircuitID.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
07_Add_Field_To_Object_InstallJobNumber.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
08_Add_Field_To_Objects_VersionName.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
09_Add_Field_To_Objects_VersionName_new_fcs.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause
10_create_schematics_tables_20140108.py
echo "Press any key to continue ONLY IF no errors occurred above."
pause