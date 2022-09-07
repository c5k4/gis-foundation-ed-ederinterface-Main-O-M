# ---------------------------------------------------------------------------
# PG&E ADMS Q3 DataModel Changes py script - Voltage Regulator HighSideConfiguration
# Part 2 - Delete old integer HighSideConfiguration field and rename the new char field - run after Bulk Updates are completed
# ---------------------------------------------------------------------------

# Import system modules
import sys, string, os, arcgisscripting
import arcpy

# Change this value to the location of the sde file you are using.
Data_Connection = "Database Connections\PT1T_EDGIS.sde"
#Data_Connection = "C:\\3QDataModel\\DEV.gdb"

try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Initializing the workspace of: ", Data_Connection
	arcpy.Workspace =  Data_Connection
except:
	# If an error occurred while running a tool, print the messages
	print "UnSuccessfully completed connecting to workspace, error was: "
	print arcpy.GetMessages()
	print "Unexpected error:", sys.exc_info()[0]
try:
	# Lets get to work      
	#standalone = "\\"
	elecDataset = "\\EDGIS.ElectricDataset\\"
	dataOwner = "EDGIS."
	
	#Delete Field 
	feature = dataOwner + "VoltageRegulator"
	print "Removing HIGHSIDECONFIGURATION field from -- " + feature
	arcpy.DeleteField_management(Data_Connection + elecDataset + feature, 'HIGHSIDECONFIGURATION' )
	#Renaming Field 
	feature = dataOwner + "VoltageRegulator"
	print "Renaming HIGHSIDECONFIGURATION_New field to HIGHSIDECONFIGURATION on -- " + feature
	arcpy.AlterField_management(Data_Connection + elecDataset + feature, 'HIGHSIDECONFIGURATION_NEW', 'HIGHSIDECONFIGURATION' )
	print "VoltageRegulatorHighSideDataModelQ3-Part2 Python Script Finished Successfully!"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
