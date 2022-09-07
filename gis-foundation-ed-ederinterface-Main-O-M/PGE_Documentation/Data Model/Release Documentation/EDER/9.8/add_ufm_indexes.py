import arcpy

Data_Connection = "Database Connections\\edgis@edgisp2d.sde"

arcpy.AddIndex_management(Data_Connection + "\\EDGIS.ElectricDataset\\EDGIS.CrossSection10Anno","FACILITYID","CROSS10ANNO_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.DuctAnnotation","FACILITYID","DUCTANNO_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.Duct","FACILITYID","DUCT_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.DuctBank","FACILITYID","DUCTBANK_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.ElectricConnector","FACILITYID","ECONNECTOR_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.UFMFloor","FACILITYID","UFMFLOOR_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")
arcpy.AddIndex_management(Data_Connection + "\\EDGIS.UFMDataset\\EDGIS.UFMWall","FACILITYID","UFMWall_FACILITYID_IDX","NON_UNIQUE","NON_ASCENDING")

