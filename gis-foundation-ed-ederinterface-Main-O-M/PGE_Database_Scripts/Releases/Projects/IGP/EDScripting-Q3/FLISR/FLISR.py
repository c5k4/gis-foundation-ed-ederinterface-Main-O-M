"""
FLISR_DONOTBACKFEED (JIRA EGIS-345)
    - creates table FLISR_DONOTBACKFEED
    - adds columns (DEVICEGUID,FEATURECLASS,DATECREATED,CREATIONUSER,LASTUPDATED,LASTUSER,TAGNUMBER)
    - registers table for versioned edits
    - assigns role to table (NOT users)
    - enables Edit Tracking
"""
# **************************************************************************************************************
# !!!! IMPORTANT: change the 'Data_connection' variable below to the correct .sde connection file                        !!!!
# !!!!          The user in the connection file must be EDGIS to create the table in the correct schema     !!!!
# !!!!   Close all ESRI desktop applications to help insure there are no locks on the connection            !!!!
# **************************************************************************************************************




# Import system modules
import arcpy
import os

# !!!! point to sde file that is connected using EDGIS user (needed to create table in that schema) !!!!
# Change this value to the location of the sde file you are using.
Data_connection = "Database Connections/PT1T_EDGIS.sde"

table = "FLISR_DONOTBACKFEED"
# schema and connection user must match to create table in correct location
schema = "EDGIS"
dbandtbl = Data_connection + "/" + schema + "." + table

arcpy.env.workspace = Data_connection

# print to screen and add to arcpy in case it's run in ESRI product
def printMessage(m):
    print m
    arcpy.AddMessage(m)

# check if a field exist
def fieldExist(field,fc):
    fields = arcpy.ListFields(fc)
    for f in fields:
        if f.name == field:
            return True
    return False

def insertRecords():
    conn = acrpy.ArcSDESQLExcecute(Data_connection)
    myfile = "FLISRRecords.txt"

    if path.isfile(myfile):
        with open(myfile) as reader:
            row = reader.read()
            printMessage("Working on: " + row)

try:

    if arcpy.Describe(Data_connection).connectionProperties.user == schema:
        if arcpy.Exists(table):
            printMessage("Table Exists.")
        else:   
            printMessage("Creating table ...")
            arcpy.CreateTable_management(Data_connection,table,"#","#")

        printMessage("  Working on Fields ...")

        # check if table is locked (ex: arccatalog is open with the same connection)
        if arcpy.TestSchemaLock(dbandtbl):
            # start adding fields  
            if not fieldExist("DEVICEGUID",dbandtbl):
                printMessage("  Adding field DEVICEGUID")
                arcpy.AddField_management(dbandtbl,"DEVICEGUID","TEXT","#","#","38","Device Global ID","NON_NULLABLE","NON_REQUIRED","#")
            if not fieldExist("FEATURECLASS",dbandtbl):
                printMessage("  Adding field FEATURECLASS")
                arcpy.AddField_management(dbandtbl,"FEATURECLASS","TEXT","#","#","60","Device Feature Class","NON_NULLABLE","NON_REQUIRED","#")
            # tag number is for Job/RW/GIS id of related feature
            if not fieldExist("TAGNUMBER",dbandtbl):
                printMessage("  Adding field TAGNUMBER")
                arcpy.AddField_management(dbandtbl,"TAGNUMBER","TEXT","#","#","20","Tag Number","NULLABLE","NON_REQUIRED","#")
            # new record info
            if not fieldExist("CREATIONUSER",dbandtbl):
                printMessage("  Adding field CREATIONUSER")
                arcpy.AddField_management(dbandtbl,"CREATIONUSER","TEXT","#","#","15","Creation User","NON_NULLABLE","NON_REQUIRED","#")
            if not fieldExist("DATECREATED",dbandtbl):
                printMessage("  Adding field DATECREATED")
                arcpy.AddField_management(dbandtbl,"DATECREATED","DATE","#","#","#","Date Created","NON_NULLABLE","NON_REQUIRED","#")
            
            # modified record info
            if not fieldExist("LASTUSER",dbandtbl):
                printMessage("  Adding field LASTUSER")
                arcpy.AddField_management(dbandtbl,"LASTUSER","TEXT","#","#","15","Last User","NULLABLE","NON_REQUIRED","#")
            if not fieldExist("DATEMODIFIED",dbandtbl):
                printMessage("  Adding field DATEMODIFIED")
                arcpy.AddField_management(dbandtbl,"DATEMODIFIED","DATE","#","#","#","Date Modified","NULLABLE","NON_REQUIRED","#")              

            
            # register table to be used in versioning
            printMessage("Registering table ...")
            arcpy.RegisterAsVersioned_management(dbandtbl, "NO_EDITS_TO_BASE")

            # create role
            # printMessage("Creating role ...")
            # using table name to create role with same name
            # arcpy.CreateRole_management(Data_connection, table, "GRANT")

            # assign role to table (people still need to be added to role)
            printMessage("Adding role to table for ...")
            printMessage("    editors ...")
            # note SDE gets added automatically
            arcpy.ChangePrivileges_management(dbandtbl, "DAT_EDITOR", "GRANT","GRANT")
            arcpy.ChangePrivileges_management(dbandtbl, "DMSSTAGING", "GRANT","GRANT")
            arcpy.ChangePrivileges_management(dbandtbl, "SDE_EDITOR", "GRANT","GRANT")
            # view only roles
            printMessage("    view ...")
            arcpy.ChangePrivileges_management(dbandtbl, "select_catalog_role", "GRANT","AS_IS")
            arcpy.ChangePrivileges_management(dbandtbl, "gisinterface", "GRANT","AS_IS")
            arcpy.ChangePrivileges_management(dbandtbl, "gis_interface", "GRANT","AS_IS")
            arcpy.ChangePrivileges_management(dbandtbl, "sde_viewer", "GRANT","AS_IS")

            # turn on edit tracking
            printMessage("Enabling Edit Tracking ...")
            arcpy.EnableEditorTracking_management(dbandtbl,"CREATIONUSER","DATECREATED","LASTUSER","DATEMODIFIED","NO_ADD_FIELDS","UTC")

            printMessage("Completed successfully.")
        else:
            printMessage("Could not establish lock on table to add fields or register. \n Please all ESRI applications.")
    else:
        printMessage( Data_connection + " file must connect with User: EDGIS.")


except arcpy.ExecuteError:
    print(arcpy.GetMessages())
    ms = arcpy.GetMessages
    for m in ms:
        arcpy.AddError(m)
