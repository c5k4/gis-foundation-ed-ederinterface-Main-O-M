@ECHO OFF

call Set_Env_Vars.bat

sqlplus %GIS_UID%/%GIS_PWD%@%DB_INSTANCE% @Servicepoint_Tab_Create_And_Merge_Changes.sql
Reconcile_then_Delete.py
