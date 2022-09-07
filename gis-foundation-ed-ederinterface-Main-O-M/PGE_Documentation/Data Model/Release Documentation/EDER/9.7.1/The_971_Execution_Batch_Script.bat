@echo off
set /a X=0
echo ****************************************************
echo     This Script will run the 9.7.1 Patch against the 
echo     Database specified on the next line:
set SCRIPT_GDB_LOCATION=Database Connections\\lbgisq1q.sde
set SCRIPT_SQL_CONNECTION=edgis/edgis!Q1Qi@lbgisq1q
echo %SCRIPT_GDB_LOCATION%
echo. 
echo  If this is the incorrect database location, please stop
echo  this program now with a control-C or closing the pop-up window.
echo Written by Rob Rader for PGE on 8/10/2015
echo ***************************************************
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - create temp field
echo ************************
.\23003\01_Add_Temp_Field_to_SubsurfaceStructure_Template.py
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - populate temp field
echo ************************
sqlplus %SCRIPT_SQL_CONNECTION% @23003\02_copy_values_to_tempfield.sql
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - drop existing field MHCOVERSIZE
echo ************************
.\23003\03_Delete_Field_from_SubsurfaceStructure.py
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - create field MHCOVERSIZE
echo ************************
.\23003\04_Add_Temp_Field_to_SubsurfaceStructure_Template.py
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - populate MHCOVERSIZE field from temp field
echo ************************
sqlplus %SCRIPT_SQL_CONNECTION% @23003\05_copy_values_to_tempfield.sql
pause
set /A X=%X% + 1
echo ************************
echo Step %X% of 25, patch CR23003 - drop temp field
echo ************************
.\23003\06_Delete_Field_from_SubsurfaceStructure.py
pause
echo.
echo. 
pause
echo --------------------------------------
echo   You have completed the 9.7.1 patch
echo --------------------------------------
echo.
@echo on
