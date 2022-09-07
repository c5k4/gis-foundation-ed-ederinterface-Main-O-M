import arcpy
import os
import sys

try:

    print "inside daily process "
    SDE_FILE_PATH = sys.argv[1]
    GDB_FILE_LOCATION = sys.argv[2] 
    GDB_FILE_NAME = sys.argv[3]
    LOG_FILE_PATH = sys.argv[4]
    WHERE_CLAUSE = sys.argv[5]
    ROW_UPDATE_VERSION = sys.argv[6]
    print str(datetime.datetime.now())
    print "args read"
    arcpy.env.overwriteOutput = True 
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\n\nSTART\n")
    LogFile.write(str(datetime.datetime.now()))
    
    GDB_FILE_PATH = GDB_FILE_LOCATION + "\\" + GDB_FILE_NAME + ".gdb"
    LogFile.write("\nDeleting " + GDB_FILE_PATH)
    arcpy.Delete_management(GDB_FILE_PATH)
    LogFile.write(" completed")
    LogFile.write("\nCreating GDB - " + GDB_FILE_NAME + " at " + GDB_FILE_LOCATION)
    arcpy.CreateFileGDB_management(GDB_FILE_LOCATION, GDB_FILE_NAME, "CURRENT")
    LogFile.write(" Completed")
    print "GDB created"

    SDE_ROW_FC = SDE_FILE_PATH + "\\LBGIS.PGE_Common_Landbase\\LBGIS.PGE_RightofWay"
    SDE_LOTLINES_FC = SDE_FILE_PATH + "\\LBGIS.PGE_landbase\\LBGIS.LotLines"
   
    SDE_ROW_LAYER = "SDE_ROW_LAYER"
    SDE_LOTLINES_LAYER = "SDE_LOTLINES_LAYER"
    WHERE_CLAUSE = "OBJECTID IN (" + WHERE_CLAUSE + ")"; 
    arcpy.MakeFeatureLayer_management(SDE_ROW_FC, SDE_ROW_LAYER, "", "", "")
    LogFile.write("\nChanging version of " + SDE_ROW_LAYER + " to " + ROW_UPDATE_VERSION)
    arcpy.ChangeVersion_management(SDE_ROW_LAYER, "TRANSACTIONAL", ROW_UPDATE_VERSION, "")

    arcpy.MakeFeatureLayer_management(SDE_LOTLINES_FC, SDE_LOTLINES_LAYER, "", "", "")
    LogFile.write("\nChanging version of " + SDE_LOTLINES_LAYER + " to " + ROW_UPDATE_VERSION)
    arcpy.ChangeVersion_management(SDE_LOTLINES_LAYER, "TRANSACTIONAL", ROW_UPDATE_VERSION, "")
    
    EDITED_LOTLINES_FC = GDB_FILE_PATH + "\\EDITED_LOTLINES_FC"
    arcpy.Delete_management(EDITED_LOTLINES_FC, "")
    arcpy.Select_analysis(SDE_LOTLINES_LAYER, EDITED_LOTLINES_FC, WHERE_CLAUSE)
    print("EDITED_LOTLINES_FC created from lotlines where "+ WHERE_CLAUSE)
    LogFile.write("\nEDITED_LOTLINES_FC created from lotlines where "+ WHERE_CLAUSE)
        
    EDITED_LOTLINES_LAYER = "EDITED_LOTLINES_LAYER"
    arcpy.MakeFeatureLayer_management(EDITED_LOTLINES_FC, EDITED_LOTLINES_LAYER, "", "", "")
    print("Select layer by location")
    arcpy.SelectLayerByLocation_management(SDE_ROW_LAYER, "INTERSECT", EDITED_LOTLINES_FC, "", "NEW_SELECTION")

    SELECTED_ROW_FC = GDB_FILE_PATH + "\\SELECTED_ROW_FC"
    arcpy.Select_analysis(SDE_ROW_LAYER, SELECTED_ROW_FC);

    SYM_DIFF_FC = GDB_FILE_PATH + "\\SYM_DIFF_AFTER_EDITS_FC"
    print("Creating symmetric difference of edited lotlines and ROW")
    arcpy.SymDiff_analysis(EDITED_LOTLINES_LAYER, SELECTED_ROW_FC, SYM_DIFF_FC,"","")
    print("Completed")
    LogFile.write("\nSYM_DIFF_FC created")

    TO_DELETE_FC = GDB_FILE_PATH + "\\TO_DELETE_FC"
    where_clause = "FID_EDITED_LOTLINES_FC=-1"
    arcpy.Select_analysis(SYM_DIFF_FC, TO_DELETE_FC, where_clause)
    LogFile.write("\nTO_DELETE_FC created")

    TO_ADD_FC = GDB_FILE_PATH + "\\TO_ADD_FC"
    where_clause = "FID_SELECTED_ROW_FC=-1"
    arcpy.Select_analysis(SYM_DIFF_FC, TO_ADD_FC, where_clause)
    LogFile.write("\nTO_ADD_FC created")

    arcpy.Append_management(TO_ADD_FC,SDE_ROW_LAYER, "NO_TEST","","")
    LogFile.write("\nTO_ADD_FC Rows appended")

    GDB_ROW_AFTER_ERASE = GDB_FILE_PATH + "\\GDB_ROW_AFTER_ERASE"
    arcpy.Erase_analysis(SDE_ROW_LAYER,TO_DELETE_FC, GDB_ROW_AFTER_ERASE,"")
    LogFile.write("\nGDB_ROW_AFTER_ERASE created")
    
    print("Dissolving ROW")
    SDE_ROW_DISSOLVED_FC = GDB_FILE_PATH + "\\SDE_ROW_DISSOLVED_FC"
    arcpy.Delete_management(SDE_ROW_DISSOLVED_FC)
    arcpy.Dissolve_management(GDB_ROW_AFTER_ERASE,SDE_ROW_DISSOLVED_FC,"#","#","SINGLE_PART","UNSPLIT_LINES")
    print("completed")
    LogFile.write("\nDissolving ROW competed")
    
    LogFile.write("\nTruncating LBGIS.RightOfWay")
    arcpy.DeleteFeatures_management(SDE_ROW_LAYER)
    LogFile.write(" Completed")
    print "ROW fc truncated in version"

    LogFile.write("\nAppending new Features to LBGIS.PGE_RightofWay")
    arcpy.Append_management(SDE_ROW_DISSOLVED_FC, SDE_ROW_LAYER, "NO_TEST", "", "")    
    LogFile.write(" Completed")
    LogFile.write("\nNew ROW records populated in version " + ROW_UPDATE_VERSION)
    print "New ROW records populated in version " + ROW_UPDATE_VERSION
    LogFile.write("\n" + str(datetime.datetime.now()))
    LogFile.write("\nEND\n")

except Exception:
    LogFile.write(str(datetime.datetime.now()))
    LogFile.write("\nERROR\n")
    e = sys.exc_info()[1]
    LogFile.write(e.args[0])
    LogFile.write(arcpy.GetMessages())
finally:
    LogFile.close()