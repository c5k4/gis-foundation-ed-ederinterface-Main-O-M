"""
FLISR_DONOTBACKFEED (JIRA EGIS-345)
    - deletes table FLISR_DONOTBACKFEED
    - script is to undo the creation script.
"""
# **************************************************************************************************************
# !!!! IMPORTANT: change the 'db' variable below to the correct .sde connection file                        !!!!
# !!!!          The user in the connection file must be EDGIS to create the table in the correct schema     !!!!
# !!!!   Close all ESRI desktop applications to help insure there are no locks on the connection            !!!!
# **************************************************************************************************************


# Import system modules
import arcpy
import os

# !!!! point to sde file that is connected using EDGIS user (needed to delete table in that schema) !!!!
db = "Database Connections/PT1D_EDGIS.sde"
table = "FLISR_DONOTBACKFEED"
# schema and connection user must match to delete table in correct location
schema = "EDGIS"
dbandtbl = db + "/" + schema + "." + table

arcpy.env.workspace = db

# print to screen and add to arcpy in case it's run in ESRI product
def printMessage(m):
    print m
    arcpy.AddMessage(m)

try:
    if arcpy.Describe(db).connectionProperties.user == schema:
        if arcpy.Exists(table):
            printMessage("Deleting Table ...")
            if arcpy.TestSchemaLock(dbandtbl):
                arcpy.Delete_management(dbandtbl)
                printMessage("Finished deleting table.")
            else:
                printMessage("Could not establish lock on table to delete table. \n Please close all ESRI applications.")
        else:   
            printMessage("Table does not exist ...")
    else:
        printMessage( db + " file must connect with User: EDGIS.")


except arcpy.ExecuteError:
    print(arcpy.GetMessages())
    ms = arcpy.GetMessages
    for m in ms:
        arcpy.AddError(m)
