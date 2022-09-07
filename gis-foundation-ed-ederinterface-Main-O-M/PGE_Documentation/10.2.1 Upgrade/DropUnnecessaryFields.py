# -*- coding: utf-8 -*-
# ---------------------------------------------------------------------------
# MigrateStorage.py
# Created on: 2014-05-07 08:16:24.00000
#   (generated by ArcGIS/ModelBuilder)
# Description: 
# ---------------------------------------------------------------------------

# Set the necessary product code
import sys
import time
# Import arcpy module
import arcpy
from arcpy import env
import os

def main():
	scriptStart = time.clock()
	print "**************************************"
	print "*** Removing unnecessary fields ***"
	print "**************************************"
	print ""
	print ""
	# Local variables:
	DataConnection = sys.argv[1]
	EDGIS_All = DataConnection
	EDGIS_CommonFeaturesDataset = DataConnection + "\\EDGIS.CommonFeaturesDataset"
	EDGIS_ConversionFeaturesDataset = DataConnection + "\\EDGIS.ConversionFeaturesDataset"
	EDGIS_ElectricDataset = DataConnection + "\\EDGIS.ElectricDataset"
	EDGIS_PGEElectricLandbase = DataConnection + "\\EDGIS.PGEElectricLandbase"
	EDGIS_SubstationDataset = DataConnection + "\\EDGIS.SubstationDataset"
	EDGIS_UFMDataset = DataConnection + "\\EDGIS.UFMDataset"
	drop_field_list = ["FEEDERID", "FEEDERID2", "DMSFEEDERID"]

	DataSources = [EDGIS_All, EDGIS_CommonFeaturesDataset, EDGIS_ConversionFeaturesDataset, EDGIS_ElectricDataset, EDGIS_PGEElectricLandbase, EDGIS_SubstationDataset, EDGIS_UFMDataset]
	for DataSource in DataSources:
		start = time.clock()
		env.workspace = DataSource
		fcList = arcpy.ListFeatureClasses()
		for fc in fcList:
			try:
				print fc + ":"
				fieldObjList = arcpy.ListFields(fc)
				fieldNameList = []
				for field in fieldObjList:
					fieldNameList.append(field.name)
				for drop_field_name in drop_field_list:
					if drop_field_name in fieldNameList:
						try:
							arcpy.DeleteField_management(fc, drop_field_name)                    
							print "     " + drop_field_name + " dropped"
						except:
							print "     Unexpected Error dropping field on " + fc + ": " + drop_field_name
							print "     " + arcpy.GetMessages()
			except:
				print "     Unexpected Error dropping fields on " + fc
				print arcpy.GetMessages()
		elapsed = (time.clock() - start)
		print "Elapsed time: ", elapsed
	scriptElapsed = (time.clock() - scriptStart)
	print ""
	print ""
	print "**************************************"
	print "Script Elapsed time: ", scriptElapsed
	print "*** Finished removing unnecessary fields ***"
	print "**************************************"
	print ""
	print ""

if __name__ == '__main__': main()