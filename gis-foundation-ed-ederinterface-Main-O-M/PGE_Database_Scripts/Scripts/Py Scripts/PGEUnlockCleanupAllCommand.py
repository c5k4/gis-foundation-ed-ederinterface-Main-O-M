import os
import sys
import arcpy

class LicenseError(Exception):
    pass

try:
    if arcpy.CheckExtension("Schematics") == "Available":
        arcpy.CheckOutExtension("Schematics")
    else:
        # raise a custom exception
        raise LicenseError

    # The toolbox containing the Schematic Diagram Update model
    toolbox = "PGESchematicCustomTools.tbx"
    arcpy.ImportToolbox(toolbox, "PGESchematicTools")

    print " "
    print "Starting:            Schematic Diagram Unlock Model"
    print " "
    print "Using Toolbox:      " + toolbox
    result = arcpy.SchematicDiagramUnlockModel_PGESchematicTools()
    print " "
    print "Finished Unlocking"

    # print " "
    # print "Starting:            Posted Session Cleanup Model"
    # print " "
    # print "Using Toolbox:      " + toolbox
    # result = arcpy.PostedSessionCleanupModel_PGESchematicTools()
    # print " "
    # print "Finished Posted Session Cleanup"

    # print " "
    # print "Starting:            Session Zero Cleanup Model"
    # print " "
    # print "Using Toolbox:      " + toolbox
    # result = arcpy.SessionZeroCleanupModel_PGESchematicTools()
    # print " "
    # print "Finished Session Zero Cleanup"

    # print " "
    # print "Finished processing"

    arcpy.CheckInExtension("Schematics")

    if (result.status == 4):
        sys.exit(0)
    else:
        exit(1)

except LicenseError:
    print("Schematics license is unavailable")
    sys.exit(1)
except arcpy.ExecuteError:
    print(arcpy.GetMessages(2))
    sys.exit(2)

