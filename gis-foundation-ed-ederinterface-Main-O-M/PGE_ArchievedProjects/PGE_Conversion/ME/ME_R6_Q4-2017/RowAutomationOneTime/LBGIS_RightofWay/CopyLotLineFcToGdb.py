import arcpy
import os
import sys

try:

    print "inside CopyLotLineFcToGdb.py"
    #SDE_FILE_PATH = "Database Connections\\Connection to EDGISQ3Q@lbgis.sde"
    #GDB_FILE_LOCATION = "C:\\Users\\t1gx\\Documents\\ArcGIS"
    #GDB_FILE_NAME = "RUN_NO_1"
    #LOG_FILE_PATH = "C:\\NOV 2017 EDGIS\\CopyFeatureClassesToGDB_log.txt"
    
    SDE_FILE_PATH = sys.argv[1] 
    GDB_FILE_LOCATION = sys.argv[2] 
    GDB_FILE_NAME = sys.argv[3] 
    LOG_FILE_PATH = sys.argv[4]
    
    print "Arguments"
    print SDE_FILE_PATH
    print GDB_FILE_LOCATION
    print GDB_FILE_NAME
    print LOG_FILE_PATH

    arcpy.env.overwriteOutput = True 
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\n\nSTART")

    # Local variables:
    LBGIS_LOTLINES_FC = SDE_FILE_PATH + "\\LBGIS.PGE_landbase\\LBGIS.LotLines"
    GDB_LOTLINES_FC = "GDB_LOTLINES_FC"
    
    # Process: Create File GDB
    GDB_FILE_PATH = GDB_FILE_LOCATION + "\\" + GDB_FILE_NAME + ".gdb"
    arcpy.Delete_management(GDB_FILE_PATH)
    LogFile.write("\n Creating GDB - " + GDB_FILE_NAME + " at " + GDB_FILE_LOCATION)
    arcpy.CreateFileGDB_management(GDB_FILE_LOCATION, GDB_FILE_NAME, "CURRENT")
    LogFile.write(" Completed")
    print "GDB created"

    arcpy.FeatureClassToFeatureClass_conversion(LBGIS_LOTLINES_FC,GDB_FILE_PATH, GDB_LOTLINES_FC,"","","")
    LogFile.write("created GDB_LOTLINES_FC \n" )
    print "FC created " + GDB_LOTLINES_FC

    LogFile.write("END \n \n")

except Exception:
    LogFile.write("ERROR")
    e = sys.exc_info()[1]
    LogFile.write(e.args[0])
    LogFile.write(arcpy.GetMessages() + "\n\n")
finally:
    LogFile.close()