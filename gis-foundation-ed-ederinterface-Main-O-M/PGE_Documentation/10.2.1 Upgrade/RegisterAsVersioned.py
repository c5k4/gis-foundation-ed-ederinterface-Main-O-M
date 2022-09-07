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

def main():
	scriptStart = time.clock()
	print "**************************************"
	print "*** Registering as versioned ***"
	print "**************************************"
	print ""
	print ""
	# Local variables:
	DataConnection = sys.argv[1]
	EDGIS_ConversionFeaturesDataset = DataConnection + "\\EDGIS.ConversionFeaturesDataset"
	EDGIS_ElectricDataset = DataConnection + "\\EDGIS.ElectricDataset"
	EDGIS_PGEElectricLandbase = DataConnection + "\\EDGIS.PGEElectricLandbase"
	EDGIS_SubstationDataset = DataConnection + "\\EDGIS.SubstationDataset"
	EDGIS_UFMDataset = DataConnection + "\\EDGIS.UFMDataset"

	# Process: Migrate Storage

	DataSources = [EDGIS_ConversionFeaturesDataset, EDGIS_ElectricDataset, EDGIS_PGEElectricLandbase, EDGIS_SubstationDataset, EDGIS_UFMDataset]
	for DataSource in DataSources:
		start = time.clock()
		try:
			print ""
			print ""
			print "Attempting to version " + DataSource + "..."
			arcpy.RegisterAsVersioned_management(DataSource, "NO_EDITS_TO_BASE")
			print "Successfully versioned " + DataSource
		except:
			print "Unexpected Error versioning " + DataSource + ": "
			print arcpy.GetMessages()
		elapsed = (time.clock() - start)
		print "Elapsed time: ", elapsed

	TablesInRelationships = ["none"]
	start = time.clock()
	try:
		print ""
		print ""
		print "Building list of tables involved in relationships..."
		print sys.argv[1]
		arcpy.env.workspace = sys.argv[1]
		relationshipList = [c.name for c in arcpy.Describe(DataConnection).children if c.datatype == "RelationshipClass"]
		for relationship in relationshipList: 
			desc = arcpy.Describe(relationship)
			for destinationClass in desc.destinationClassNames:
				TablesInRelationships.append(destinationClass.upper())
			for originClass in desc.originClassNames:
				TablesInRelationships.append(originClass.upper())
	except:
		print "Unexpected Error determining tables in relationships: "
		print arcpy.GetMessages()

	start = time.clock()
	try:
		print ""
		print ""
		print "Versioning remaining tables involved in relationships..."
		print sys.argv[1]
		arcpy.env.workspace = sys.argv[1]
		tableList = arcpy.ListTables()
		for table in tableList:
			if table.upper() in TablesInRelationships:
				try:
					print "Registering "+table+ " as versioned"
					arcpy.RegisterAsVersioned_management(table, "NO_EDITS_TO_BASE");
				except:
					ErrorMessage = arcpy.GetMessages()
					if "The dataset is already registered as version" not in ErrorMessage:
						print "Unable to register as versioned: " + table
						print ErrorMessage
	except:
		print "Unexpected Error registering relationship tables as versioned: "
		print arcpy.GetMessages()
	elapsed = (time.clock() - start)
	print "Elapsed time: ", elapsed
	
	scriptElapsed = (time.clock() - scriptStart)
	print ""
	print ""
	print "**************************************"
	print "Script Elapsed time: ", scriptElapsed
	print "*** Finished registering as versioned ***"
	print "**************************************"
	print ""
	print ""

if __name__ == '__main__': main()