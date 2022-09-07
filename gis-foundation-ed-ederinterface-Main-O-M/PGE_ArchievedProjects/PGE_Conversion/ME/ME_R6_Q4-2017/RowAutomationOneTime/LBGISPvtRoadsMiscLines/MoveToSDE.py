import arcpy
import os
import sys

try:

    print "inside move to SDE"
    SDE_FILE_PATH = sys.argv[1] 
    GDB_FILE_PATH = sys.argv[2]
    LOG_FILE_PATH = sys.argv[3] 
    ONE_TIME_VERSION_NAME = sys.argv[4]
    
    print "args read"
    print SDE_FILE_PATH
    print GDB_FILE_PATH
    print LOG_FILE_PATH
    print ONE_TIME_VERSION_NAME
    
    arcpy.env.overwriteOutput = True 
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\n\nSTART \n")
    
    # Local variables:
    LBGIS_PVT_ROADS_FC = SDE_FILE_PATH + "\\LBGIS.PGE_Common_Landbase\\LBGIS.PVT_Road"
    LBGIS_MISC_LINES_FC = SDE_FILE_PATH + "\\LBGIS.PGE_Common_Landbase\\LBGIS.MiscLines"
    
    LBGIS_PVT_ROADS_LAYER = "LBGIS_PVT_ROADS_LAYER"
    LBGIS_MISC_LINES_LAYER = "LBGIS_MISC_LINES_LAYER"

    GDB_PVT_ROADS_ADD_FC = GDB_FILE_PATH + "\\GDB_PVT_ROADS_ADD_FC"
    GDB_MISC_LINES_ADD_FC = GDB_FILE_PATH + "\\GDB_MISC_LINES_ADD_FC"
    #ONE_TIME_VERSION_NAME = "SDE." + ONE_TIME_VERSION_NAME
    
    arcpy.MakeFeatureLayer_management(LBGIS_PVT_ROADS_FC, LBGIS_PVT_ROADS_LAYER, "", "", "")
    arcpy.ChangeVersion_management(LBGIS_PVT_ROADS_LAYER, "TRANSACTIONAL", ONE_TIME_VERSION_NAME, "")
    LogFile.write("\nAppending new Features to Pvt Roads")
    arcpy.Append_management(GDB_PVT_ROADS_ADD_FC, LBGIS_PVT_ROADS_LAYER, "NO_TEST", "", "")    
    LogFile.write(" Completed")

    arcpy.MakeFeatureLayer_management(LBGIS_MISC_LINES_FC, LBGIS_MISC_LINES_LAYER, "", "", "")
    arcpy.ChangeVersion_management(LBGIS_MISC_LINES_LAYER, "TRANSACTIONAL", ONE_TIME_VERSION_NAME, "")
    LogFile.write("\nAppending new Features to Misc Lines")
    arcpy.Append_management(GDB_MISC_LINES_ADD_FC, LBGIS_MISC_LINES_LAYER, "NO_TEST", "", "")    
    LogFile.write(" Completed")
   
    LogFile.write("\nOne Time activity completed")
    LogFile.write("\nEND \n\n")

except Exception:
    LogFile.write("\nERROR")
    e = sys.exc_info()[1]
    LogFile.write(e.args[0])
    LogFile.write(arcpy.GetMessages() + "\n\n")
finally:
    LogFile.close()