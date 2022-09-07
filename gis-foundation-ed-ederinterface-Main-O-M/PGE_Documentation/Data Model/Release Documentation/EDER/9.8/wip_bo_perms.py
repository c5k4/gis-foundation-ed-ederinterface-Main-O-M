import arcpy
#TODO: change this connection to the target WIP database, password-saved connection
dataConnection = "Database Connections/webr@edgisw1d.sde"

arcpy.ChangePrivileges_management("'" + dataConnection + "/WEBR.JET_EquipIDType';'" + dataConnection + "/WEBR.JET_Equipment';'" + dataConnection + "/WEBR.JET_EquipTypeRules';'" + dataConnection + "/WEBR.JET_EquipTypeSelection';'" + dataConnection + "/WEBR.JET_Jobs'","EDGISBO","GRANT","#")
arcpy.ChangePrivileges_management("'" + dataConnection + "/WEBR.JET_EquipIDType';'" + dataConnection + "/WEBR.JET_Equipment';'" + dataConnection + "/WEBR.JET_EquipTypeRules';'" + dataConnection + "/WEBR.JET_EquipTypeSelection';'" + dataConnection + "/WEBR.JET_Jobs'","EDGIS_BOBJ","GRANT","#")