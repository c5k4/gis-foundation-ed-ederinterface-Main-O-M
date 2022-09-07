import arcpy

Data_Connection = "Database Connections\\LBGISQ1Q_EDGIS.sde"

arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", 'PGE_JET_OPERATINGNUMBER', 'PGE_JET_OPERATINGNUMBER')
print "Added JET coded value to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Object Class Model Name", 'PGE_JET_OPERATINGNUMBER', 'PGE_JET_OPERATINGNUMBER')
print "Added JET coded value to domain"