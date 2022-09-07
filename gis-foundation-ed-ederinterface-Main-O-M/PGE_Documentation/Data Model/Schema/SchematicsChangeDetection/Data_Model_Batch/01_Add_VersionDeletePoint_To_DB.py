# ---------------------------------------------------------------------------
# Template .py file for PGE TFS Data model changes
# Created by Robert Rader : Jan 24th 2013 Template version 1.0
# ---------------------------------------------------------------------------
# Import system modules
import sys, string, os, arcgisscripting
import arcpy
# from arcpy import env

# Local variables...
# Change this value to the location of the sde file you are using.
# Data_Connection = "Database Connections\EDGIS@pge1.sde"
Data_Connection = os.environ.get("SCRIPT_GDB_LOCATION")
#change this value to the name of th featureclass we are adding.
FC_name = "PGE_VERSIONDELETEPOINT"

# Change the FC_type variable to one of the below commented out options
# to choose the type of feature class to be created
#FC_type = "POINT"
#FC_type = "POLYGON"
#FC_type = "MULTIPOINT"
#FC_type = "POLYLINE"
FC_type = "POINT"

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
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,FC_name
	print "new_fc_name        " , FC_name
	print "new_fc_type        " , FC_type,
	print "adding ...."
	arcpy.CreateFeatureclass_management (Data_Connection, FC_name, FC_type)
	print "SUCCESS : created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
