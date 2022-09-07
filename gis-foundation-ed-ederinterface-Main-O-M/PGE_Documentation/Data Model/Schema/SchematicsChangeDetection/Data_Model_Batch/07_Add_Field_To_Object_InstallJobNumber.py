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
#change this value to the name of th featureclass we are adding the field too.
updateFC_name_list = ["PGE_VERSIONDELETEPOINT","PGE_VERSIONDELETELINE"]
#Name of the new field
new_field_name = "INSTALLJOBNUMBER"

#New Field Alias, is optional, can be set to "" when not used.The alternate name given to the field name. 
#    This name is used to give more descriptive names to cryptic field names. 
#    The field alias parameter only applies to geodatabases and coverages.
new_field_alias = "Job Number"

# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 
# new_field_type = "FLOAT" # Numeric values with fractional values within a specific range. 
# new_field_type = "DOUBLE" # Numeric values with fractional values within a specific range. 
# new_field_type = "SHORT" # Numeric values without fractional values within a specific range; coded values. 
# new_field_type = "LONG" # Numeric values without fractional values within a specific range. 
# new_field_type = "DATE" # Date and/or Time. 
# new_field_type = "BLOB" # Images or other multimedia. 
# new_field_type = "RASTER" # Raster images. 
# new_field_type = "GUID" # GUID values 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "14"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 
#new_field_is_nullable = ""

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""



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
		print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,updateFC_name
		print "new_field_name        " , new_field_name
		print "new_field_type        " , new_field_type,
		print "new_field_precision   " , new_field_precision
		print "new_field_scale       " , new_field_scale
		print "new_field_length      " , new_field_length
		print "new_field_alias       " , new_field_alias
		print "new_field_is_nullable " , new_field_is_nullable
		print "new_field_is_required " , new_field_is_required
		print "new_field_domain      " , new_field_domain
		print "adding ...."
		arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
		print "SUCCESS : created the field on the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
