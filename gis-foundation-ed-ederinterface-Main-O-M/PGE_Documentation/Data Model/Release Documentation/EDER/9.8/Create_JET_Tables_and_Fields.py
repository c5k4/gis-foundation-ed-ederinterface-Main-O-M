import arcpy

Data_Connection = "Database Connections\\EDGISW1D_WEBR.sde"

arcpy.CreateDomain_management(Data_Connection, "JetJobStatus", "JetJobStatus", "SHORT", "CODED")
print "Created JetJobStatus domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetJobStatus", 1, "Active")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetJobStatus", 2, "Retired")
print "Added coded value to domain"

arcpy.CreateDomain_management(Data_Connection, "JetEquipmentStatus", "JetEquipmentStatus", "SHORT", "CODED")
print "Created JetJobStatus domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetEquipmentStatus", 1, "Reserved")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetEquipmentStatus", 2, "Active")
print "Added coded value to domain"

arcpy.CreateDomain_management(Data_Connection, "JetInstallType", "JetInstallType", "TEXT", "CODED")
print "Created JetInstallType domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetInstallType", "OH", "Overhead")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetInstallType", "PM", "Padmounted")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetInstallType", "UG", "Underground")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "JetInstallType", "SS", "Subsurface")
print "Added coded value to domain"

arcpy.CreateDomain_management(Data_Connection, "Division Name", "Division Name", "SHORT", "CODED")
print "Created Division Name domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 17, "Central Coast")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 16, "De Anza")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 9, "Diablo")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 6, "East Bay")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 20, "Humboldt")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 12, "Fresno")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 13, "Kern")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 18, "Los Padres")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 14, "Mission")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 5, "North Bay")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 1, "Sonoma")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 2, "North Valley")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 8, "Peninsula")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 3, "Sacramento")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 7, "San Francisco")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 15, "San Jose")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 4, "Sierra")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 19, "Skyline")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 10, "Stockton")
print "Added coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Division Name", 11, "Yosemite")
print "Added coded value to domain"

Table_name = "JET_Jobs"	
arcpy.CreateTable_management (Data_Connection, Table_name)
print "Created table "+Table_name

Field_name="JOBNUMBER"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '12', 'Job Number', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="RESERVEDBY"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '32', 'Reserved By', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="DIVISION"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '', '', '5', 'Division', 'NON_NULLABLE', '', 'Division Name')
print "Added " + Field_name + " to " + Table_name
Field_name="DESCRIPTION"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '80', 'Description', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="ENTRYDATE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DATE', '', '', '', 'Entry Date', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="LASTMODIFIEDDATE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DATE', '', '', '', 'Last Modified Date', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="USERAUDIT"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '113', 'User Audit', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="STATUS"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '', '', '5', 'Status', 'NON_NULLABLE', '', 'JetJobStatus')
print "Added " + Field_name + " to " + Table_name
arcpy.AssignDefaultToField_management(Data_Connection+"\\"+Table_name, 'STATUS', '1')
print "Assigned default to "+Field_name

Table_name = "JET_Equipment"	
arcpy.CreateTable_management (Data_Connection, Table_name)
print "Created table "+Table_name

Field_name="EQUIPTYPEID"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '2', '', '', 'Equipment Type ID', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="JOBNUMBER"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '12', 'Job Number', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="OPERATINGNUMBER"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '9', 'Operating Number', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="CGC12"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '12', 'CGC12', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="SKETCHLOC"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '4', 'Sketch Location', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="INSTALLCD"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '4', 'Install CD', 'NULLABLE', '', 'JetInstallType')
print "Added " + Field_name + " to " + Table_name
Field_name="ADDRESS"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '128', 'Address', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="CITY"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '50', 'City', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="LATITUDE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DOUBLE', '10', '5', '', 'Latitude', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="LONGITUDE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DOUBLE', '10', '5', '', 'Longitude', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="ENTRYDATE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DATE', '', '', '', 'Entry Date', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="LASTMODIFIEDDATE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'DATE', '', '', '', 'Last Modified Date', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="USERAUDIT"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '113', 'User Audit', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="CUSTOWNED"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '1', 'Customer Owned', 'NON_NULLABLE', '', '')
arcpy.AssignDefaultToField_management(Data_Connection+"\\"+Table_name, Field_name, '0')
print "Added " + Field_name + " to " + Table_name + " and assigned default"
Field_name="STATUS"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '', '', '5', 'Status', 'NON_NULLABLE', '', 'JetEquipmentStatus')
arcpy.AssignDefaultToField_management(Data_Connection+"\\"+Table_name, Field_name, '1')
print "Added " + Field_name + " to " + Table_name



Table_name = "JET_EquipIDType"	
arcpy.CreateTable_management (Data_Connection, Table_name)
print "Created table "+Table_name

Field_name="EQUIPTYPEID"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '2', '', '', 'Equipment Type ID', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="EQUIPTYPEDESC"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '50', 'Equipment Type Description', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="HASOPERATINGNUM"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '1', 'Has Operating Num', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="HASCGC12"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '1', 'Has CGC12', 'NON_NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name

Table_name = "JET_EquipTypeSelection"	
arcpy.CreateTable_management (Data_Connection, Table_name)
print "Created table "+Table_name

Field_name="EQUIPTYPEID"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '2', '', '', 'Equipment Type ID', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="OBJECTCLASSID"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'LONG', '8', '', '', 'Object Class ID', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="SUBTYPECD"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '3', '', '', 'Subtype CD', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="FIELDNAME"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '50', 'Field Name', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="FIELDVALUE"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '50', 'Field Value', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name

Table_name = "JET_EquipTypeRules"	
arcpy.CreateTable_management (Data_Connection, Table_name)
print "Created table "+Table_name

Field_name="EQUIPTYPEID"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'SHORT', '2', '', '', 'Equipment Type ID', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="ODDEVEN"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '2', 'Odd or Even', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name
Field_name="PREFIXCHARS"
arcpy.AddField_management (Data_Connection +"\\"+Table_name, Field_name, 'TEXT', '', '', '9', 'Prefix Characters', 'NULLABLE', '', '')
print "Added " + Field_name + " to " + Table_name

arcpy.CreateRelationshipClass_management(Data_Connection+"\\"+"JET_Jobs", Data_Connection+"\\"+"JET_Equipment", "JetJobs_JetEquip", "SIMPLE", "Equipment", "Jobs", "NONE", "ONE_TO_MANY", "NONE", "JOBNUMBER", "JOBNUMBER")
print "Created relationship class Jobs/equipment"
arcpy.CreateRelationshipClass_management(Data_Connection+"\\"+"JET_EquipIDType", Data_Connection+"\\"+"JET_Equipment", "EquipIDType_JetEquip", "SIMPLE", "Equipment", "Equipment ID Type", "NONE", "ONE_TO_MANY", "NONE", "EQUIPTYPEID", "EQUIPTYPEID")
print "Created relationship class EquipmentIDType/Equipment"
arcpy.CreateRelationshipClass_management(Data_Connection+"\\"+"JET_EquipIDType", Data_Connection+"\\"+"JET_EquipTypeSelection", "EquipIDType_EquipTypeSelection", "SIMPLE", "Equipment Type Selection", "Equipment ID Type", "NONE", "ONE_TO_ONE", "NONE", "EQUIPTYPEID", "EQUIPTYPEID")
print "Created relationship class EquipmentIDType/EquipmentTypeSelection"
arcpy.CreateRelationshipClass_management(Data_Connection+"\\"+"JET_EquipIDType", Data_Connection+"\\"+"JET_EquipTypeRules", "EquipIDType_EquipTypeRules", "SIMPLE", "Equipment Type Rules", "Equipment ID Type", "NONE", "ONE_TO_ONE", "NONE", "EQUIPTYPEID", "EQUIPTYPEID")
print "Created relationship class EquipmentIDType/EquipmentTypeSelection"

arcpy.ChangePrivileges_management("'"+Data_Connection+"\\"+"WEBR.JET_EquipIDType';'"+Data_Connection+"\\"+"WEBR.JET_Equipment';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeRules';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeSelection';'"+Data_Connection+"\\"+"WEBR.JET_Jobs'","WIP_RW","GRANT","GRANT")
arcpy.ChangePrivileges_management("'"+Data_Connection+"\\"+"WEBR.JET_EquipIDType';'"+Data_Connection+"\\"+"WEBR.JET_Equipment';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeRules';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeSelection';'"+Data_Connection+"\\"+"WEBR.JET_Jobs'","SDE_VIEWER","GRANT","GRANT")
arcpy.ChangePrivileges_management("'"+Data_Connection+"\\"+"WEBR.JET_EquipIDType';'"+Data_Connection+"\\"+"WEBR.JET_Equipment';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeRules';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeSelection';'"+Data_Connection+"\\"+"WEBR.JET_Jobs'","SDE_EDITOR","GRANT","GRANT")
arcpy.ChangePrivileges_management("'"+Data_Connection+"\\"+"WEBR.JET_EquipIDType';'"+Data_Connection+"\\"+"WEBR.JET_Equipment';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeRules';'"+Data_Connection+"\\"+"WEBR.JET_EquipTypeSelection';'"+Data_Connection+"\\"+"WEBR.JET_Jobs'","WIP_RO","GRANT","")
