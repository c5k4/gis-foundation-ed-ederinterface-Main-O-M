
import os
# Import arcpy module
import arcpy
arcpy.env.overwriteOutput = True
import sys
try:
    print "inside Append New to GDB FC" 
    i = sys.argv[1] #DIVISION NUMBER
    LOG_FILE_PATH = sys.argv[2]
    GDB_FILE_LOCATION = sys.argv[3]
    
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\nSTART\n " )
    LogFile.write("\n" + sys.argv[0])
    LogFile.write("\n" + sys.argv[1])
    LogFile.write("\n" + sys.argv[2])
    LogFile.write("\n" + sys.argv[3])
    
    GDB_DIV_FILE_PATH = GDB_FILE_LOCATION + "\\GDB_DIV_" + str(i) + ".gdb"
    GDB_FILE_PATH = GDB_FILE_LOCATION + "\\GDB_MAIN" + ".gdb";

    GDB_LOTLINES_FC = GDB_FILE_PATH + "\\GDB_LOTLINES_FC"
    GDB_ROW_FC = GDB_FILE_PATH + "\\GDB_ROW_FC"
    GDB_LOTLINE_BORDER_FC = GDB_FILE_PATH + "\\GDB_LOTLINE_BORDER_FC"
    GDB_PVT_ROADS_ADD_FC = GDB_FILE_PATH + "\\GDB_PVT_ROADS_ADD_FC"
    GDB_MISC_LINES_ADD_FC = GDB_FILE_PATH + "\\GDB_MISC_LINES_ADD_FC"
	
    POLYLINE_ONLY_ROWS_FC = GDB_DIV_FILE_PATH + "\\POLYLINE_ONLY_ROWS_FC"
    LogFile.write("\nNo of new polyline features = " + str(arcpy.GetCount_management(POLYLINE_ONLY_ROWS_FC)))
    LogFile.write("\nNo of features in GDB_MISC_LINES_ADD_FC (before append) = " )
    LogFile.write(str(arcpy.GetCount_management(GDB_MISC_LINES_ADD_FC)))
    arcpy.Append_management(POLYLINE_ONLY_ROWS_FC, GDB_MISC_LINES_ADD_FC, "NO_TEST", "DESCRIPTION \"Description\" true true false 25 Text 0 0 ,First,#;GLOBALID \"GLOBALID\" false false false 38 GlobalID 0 0 ,First,#;CREATED_USER \"CREATED_USER\" true true false 255 Text 0 0 ,First,#;CREATED_DATE \"CREATED_DATE\" true true false 8 Date 0 0 ,First,#;LAST_EDITED_USER \"LAST_EDITED_USER\" true true false 255 Text 0 0 ,First,#;LAST_EDITED_DATE \"LAST_EDITED_DATE\" true true false 8 Date 0 0 ,First,#;Shape_Length \"Shape_Length\" false true true 8 Double 0 0 ,First,#", "")
    LogFile.write("\nNo of features in GDB_MISC_LINES_ADD_FC (after append) = " )
    LogFile.write(str(arcpy.GetCount_management(GDB_MISC_LINES_ADD_FC)))
    LogFile.write("\nAppend features to GDB_MISC_LINES_ADD_FC completed for Division " + str(i))
 
    GDB_PVT_ROADS_DISSOLVED_FC = GDB_DIV_FILE_PATH + "\\GDB_PVT_ROADS_DISSOLVED_FC" + str(i)
    LogFile.write("\n\n Append Dissolved Features to GDB_PVT_ROADS_ADD_FC")
    LogFile.write("\nNo. of features to be appended = " + str(arcpy.GetCount_management(GDB_PVT_ROADS_DISSOLVED_FC)))
    LogFile.write("\nNo. of features in GDB_PVT_ROADS_ADD_FC before append " + str(arcpy.GetCount_management(GDB_PVT_ROADS_ADD_FC)))
    arcpy.Append_management(GDB_PVT_ROADS_DISSOLVED_FC, GDB_PVT_ROADS_ADD_FC, "NO_TEST", "", "")    
    LogFile.write("\nNo. of features in GDB_PVT_ROADS_ADD_FC after append " + str(arcpy.GetCount_management(GDB_PVT_ROADS_ADD_FC)))
    
except Exception:
    LogFile.write("\nERROR")
    e = sys.exc_info()[1]
    LogFile.write("\n" + e.args[0])
    LogFile.write("\n" + arcpy.GetMessages() + "\n\n")
finally:
    LogFile.close()