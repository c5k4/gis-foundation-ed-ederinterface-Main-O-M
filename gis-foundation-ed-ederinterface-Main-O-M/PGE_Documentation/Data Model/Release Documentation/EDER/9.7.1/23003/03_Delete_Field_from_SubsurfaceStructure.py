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
Data_Connection = os.environ.get("SCRIPT_GDB_LOCATION")
#change this value to the name of th featureclass we are dropping the field.
updateFC_name_list = ["SubsurfaceStructure"]
#Name of the field to drop
drop_field_list = ["MHCOVERSIZE"] 

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
	for updateFC_name in updateFC_name_list:
		print "Using the geodatabase of : ", Data_Connection, "to update the feature class " ,updateFC_name
		for drop_field_name in drop_field_list:
			print "drop_field_name        " , drop_field_name
			print "dropping ...."
			arcpy.DeleteField_management(Data_Connection +"\\"+updateFC_name, drop_field_name)
			print "SUCCESS : deleted the field on the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
