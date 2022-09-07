import os
# Import arcpy module
import arcpy
arcpy.env.overwriteOutput = True
import sys
try:
    # Input Parameters
    LOG_FILE_PATH = sys.argv[2]
    GDB_FILE_PATH = sys.argv[1]
    
    # Local variables:
    GDB_LOTLINE_BORDER_FC = GDB_FILE_PATH + "\\GDB_LOTLINE_BORDER_FC"
    GDB_PVT_ROADS_ADD_FC = GDB_FILE_PATH + "\\GDB_PVT_ROADS_ADD_FC"
    GDB_MISC_LINES_ADD_FC = GDB_FILE_PATH + "\\GDB_MISC_LINES_ADD_FC"
    
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\nSTART\n " )
    
    # dissolve pvt_road features
    LogFile.write("\nDissolving pvt_roads")
    GDB_PVT_ROADS_DISSOLVED_FC = GDB_FILE_PATH + "\\GDB_PVT_ROADS_DISSOLVED_FC" 
    arcpy.Delete_management(GDB_PVT_ROADS_DISSOLVED_FC, "")
    arcpy.Dissolve_management(GDB_PVT_ROADS_ADD_FC,GDB_PVT_ROADS_DISSOLVED_FC,"#","#","SINGLE_PART","UNSPLIT_LINES")
    LogFile.write(" completed")

except Exception:
    LogFile.write("ERROR")
    e = sys.exc_info()[1]
    LogFile.write(e.args[0])
    LogFile.write(arcpy.GetMessages() + "\n\n")
finally:
    LogFile.close()