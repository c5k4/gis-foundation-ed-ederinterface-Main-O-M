@ECHO OFF

call Set_Env_Vars.bat

call EDGIS_Create_Views.bat %DB_INSTANCE% %EDGIS_UID% %EDGIS_PWD%
sqlplus %EDGIS_UID%/%EDGIS_PWD%@%DB_INSTANCE% @Set_Permissions_And_Initialize.sql