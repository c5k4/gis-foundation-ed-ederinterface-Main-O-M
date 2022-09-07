import arcpy
import os
import sys

try:

    print "inside run lotline dissolve"
    SDE_FILE_PATH = sys.argv[1] 
    GDB_FILE_PATH = sys.argv[2]
    LOG_FILE_PATH = sys.argv[3] 
    ONE_TIME_VERSION_NAME = sys.argv[4]
    GDB_LOTLINE_DISSOLVED_FC = sys.argv[5]

    print "args read"
    print SDE_FILE_PATH
    print GDB_FILE_PATH
    print LOG_FILE_PATH
    print ONE_TIME_VERSION_NAME
    print GDB_LOTLINE_DISSOLVED_FC
   
    arcpy.env.overwriteOutput = True 
    LogFile = open(LOG_FILE_PATH, "a")
    LogFile.write("\n\nSTART \n")
    
    # Local variables:
    LBGIS_ROW_FC = SDE_FILE_PATH + "\\LBGIS.PGE_Common_Landbase\\LBGIS.PGE_RightofWay"
    LBGIS_LOTLINES_FC = SDE_FILE_PATH + "\\LBGIS.PGE_landbase\\LBGIS.LotLines"
    
    LBGIS_ROW_LAYER = "LBGIS_ROW_LAYER"
    LBGIS_LOTLINES_LAYER = "LBGIS_LOTLINES_LAYER"
    
    GDB_LOTLINES_FC = GDB_FILE_PATH + "\\GDB_LOTLINES_FC"
    ONE_TIME_VERSION_NAME = "SDE." + ONE_TIME_VERSION_NAME
    
    arcpy.MakeFeatureLayer_management(LBGIS_LOTLINES_FC, LBGIS_LOTLINES_LAYER, "", "", "")
    arcpy.ChangeVersion_management(LBGIS_LOTLINES_LAYER, "TRANSACTIONAL", ONE_TIME_VERSION_NAME, "")
    
    #LogFile.write("\nDissolving lotlines" + GDB_LOTLINES_FC + " into " + GDB_LOTLINE_DISSOLVED_FC)
    print("Dissolving lotlines - " + GDB_LOTLINES_FC)
    arcpy.Delete_management(GDB_LOTLINE_DISSOLVED_FC)
    arcpy.Dissolve_management(LBGIS_LOTLINES_LAYER,GDB_LOTLINE_DISSOLVED_FC,"#","#","SINGLE_PART","UNSPLIT_LINES")
    LogFile.write(" completed")
    print("Completed")

    LogFile.write("\nLotLines Dissolve completed")
    LogFile.write("\nEND \n\n")

except Exception:
    LogFile.write("ERROR")
    e = sys.exc_info()[1]
    LogFile.write(e.args[0])
    LogFile.write(arcpy.GetMessages() + "\n\n")
finally:
    LogFile.close()