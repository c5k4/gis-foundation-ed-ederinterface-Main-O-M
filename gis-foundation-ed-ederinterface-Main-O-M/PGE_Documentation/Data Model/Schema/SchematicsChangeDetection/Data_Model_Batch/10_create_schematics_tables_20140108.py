#Initial script information supplied by Phil Penn
#Edited and formated to run as a script by Rob Rader 
#Reviewed and Approved by Gareth Thompson
import sys, string, os, arcgisscripting
import arcpy


#  ******************************************************************
#      EDIT THESE SETTINGS
#  ******************************************************************
# Data_Connection = os.environ.get("SDECONN_FILE_LOCATION")
# Data_Connection = "Database Connections\EDGIS@pge1.sde"
Data_Connection = os.environ.get("SCRIPT_GDB_LOCATION")

#Set the table names here for the table creation script.
PGE_CHANGE_SET_POINT_TABLENAME       ='PGE_EDERChangeSetPoint'
PGE_CHANGE_SET_LINE_TABLENAME        ='PGE_EDERChangeSetLine'
PGE_CHANGE_SESS0_MAPGRID_TABLENAME   ='PGE_EDERSession0MapGrid'
PGE_CHANGE_POSTED_SESSIONS_TABLENAME ='PGE_EDERPostedSession'
# ******************************************************************
# No need to edit anything below this point in the script.
# ******************************************************************

# use the table names defined and the connection to create object pointers.
PGE_CHANGE_SESS0_MAPGRID_OBJECT = Data_Connection + "\\" + PGE_CHANGE_SESS0_MAPGRID_TABLENAME
PGE_CHANGE_SET_LINE_OBJECT = Data_Connection + "\\" + PGE_CHANGE_SET_LINE_TABLENAME
PGE_CHANGE_SET_POINT_OBJECT = Data_Connection + "\\" + PGE_CHANGE_SET_POINT_TABLENAME
PGE_CHANGE_POSTED_SESSIONS_OBJECT = Data_Connection + "\\" + PGE_CHANGE_POSTED_SESSIONS_TABLENAME

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
	print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,PGE_CHANGE_SET_POINT_TABLENAME
	print "creating ...."
	arcpy.CreateFeatureclass_management(Data_Connection, PGE_CHANGE_SET_POINT_TABLENAME, 'POINT', '#', 'DISABLED', 'DISABLED', "PROJCS['NAD_1983_UTM_Zone_10N',GEOGCS['GCS_North_American_1983',DATUM['D_North_American_1983',SPHEROID['GRS_1980',6378137.0,298.257222101]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Transverse_Mercator'],PARAMETER['False_Easting',1640416.666666667],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-123.0],PARAMETER['Scale_Factor',0.9996],PARAMETER['Latitude_Of_Origin',0.0],UNIT['Foot_US',0.3048006096012192]];-450359962737.05 -450359962737.05 10000;0 100000;0 100000;0.0002;0.00002;0.00002;IsHighPrecision", '#', '0', '0', '0')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'FeatureGUID', 'GUID', '#', '#', '38', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'ActionType', 'TEXT', '#', '#', '2', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'FeatureClassID', 'LONG', '#', '#', '#', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'CircuitID', 'TEXT', '#', '#', '9', '#', 'NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'SessionID', 'TEXT', '#', '#', '64', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SET_POINT_OBJECT, 'MapNo', 'TEXT', '#', '#', '12', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,PGE_CHANGE_SET_LINE_TABLENAME
	print "creating ...."
	arcpy.CreateFeatureclass_management(Data_Connection, PGE_CHANGE_SET_LINE_TABLENAME, 'POLYLINE', PGE_CHANGE_SET_POINT_OBJECT, 'DISABLED', 'DISABLED', "PROJCS['NAD_1983_UTM_Zone_10N',GEOGCS['GCS_North_American_1983',DATUM['D_North_American_1983',SPHEROID['GRS_1980',6378137.0,298.257222101]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Transverse_Mercator'],PARAMETER['False_Easting',1640416.666666667],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-123.0],PARAMETER['Scale_Factor',0.9996],PARAMETER['Latitude_Of_Origin',0.0],UNIT['Foot_US',0.3048006096012192]];-450359962737.05 -450359962737.05 10000;0 100000;0 100000;0.0002;0.00002;0.00002;IsHighPrecision", '#', '0', '0', '0')
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to add globalids to the feature class " ,PGE_CHANGE_SET_POINT_TABLENAME
	print "ADDING GLOBALIDS ...."	
	arcpy.AddGlobalIDs_management(PGE_CHANGE_SET_POINT_OBJECT)
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to add globalids to the feature class " ,PGE_CHANGE_SET_LINE_TABLENAME
	print "ADDING GLOBALIDS ...."	
	arcpy.AddGlobalIDs_management(PGE_CHANGE_SET_LINE_OBJECT)
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]	

try:
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,PGE_CHANGE_SESS0_MAPGRID_TABLENAME
	print "creating ...."
	arcpy.CreateTable_management(Data_Connection, PGE_CHANGE_SESS0_MAPGRID_TABLENAME, '#', '#')
	arcpy.AddField_management(PGE_CHANGE_SESS0_MAPGRID_OBJECT, 'SessionID', 'TEXT', '#', '#', '64', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SESS0_MAPGRID_OBJECT, 'SessionPostingTime', 'DATE', '#', '#', '#', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SESS0_MAPGRID_OBJECT, 'MapNo', 'TEXT', '#', '#', '10', '#', 'NON_NULLABLE', 'NON_REQUIRED', '#')
	arcpy.AddField_management(PGE_CHANGE_SESS0_MAPGRID_OBJECT, 'ProcessID', 'TEXT', '#', '#', '30', '#', 'NULLABLE', 'NON_REQUIRED', '#')
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Execute Add Field
	print "Using the geodatabase of : ", Data_Connection, "to create the feature class " ,PGE_CHANGE_SESS0_MAPGRID_TABLENAME
	print "creating ...."
	arcpy.CreateTable_management(Data_Connection,PGE_CHANGE_POSTED_SESSIONS_TABLENAME,"#","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"SessionID","TEXT","#","#","64","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"PMOrderNumber","TEXT","#","#","255","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"Region","TEXT","#","#","32","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"District","TEXT","#","#","32","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"Division","TEXT","#","#","32","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"LocalOffice","TEXT","#","#","32","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"LANID","TEXT","#","#","8","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"Status","SHORT","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"STATUS","1","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"SessionDate","DATE","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"FeatureClassID","LONG","#","#","#","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"FeatureGlobalID","GUID","#","#","#","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NumAdds","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NUMADDS","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NumUA","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NUMUA","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NumUG","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NUMUG","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NumU","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NUMU","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NumDeletes","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"NUMDELETES","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"TotalNumEdits","LONG","#","#","#","#","NON_NULLABLE","NON_REQUIRED","#")
	arcpy.AssignDefaultToField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"TOTALNUMEDITS","0","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"ProcessID","TEXT","#","#","30","#","NULLABLE","NON_REQUIRED","#")
	arcpy.AddField_management(PGE_CHANGE_POSTED_SESSIONS_OBJECT,"SessionTitle","TEXT","#","#","100","#","NULLABLE","NON_REQUIRED","#")
	print "SUCCESS : Created the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]


