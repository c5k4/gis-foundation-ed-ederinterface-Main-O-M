# Name: UploadServiceDefinition_example3.py
# Description: Upload and publish all service definitions contained in a folder
# Requirements: Connection to an ArcGIS Server or My Hosted Services

# Import system modules
import arcpy
from arcpy import env

# Set environment settings
env.workspace = "D:\DC2"

# Set local variable
inServer = "arcgis on edgisappprd52_6443 (admin).ags"
print "Publishing to " + inServer

# Find all the service definitions (.sd or .sds) in a workspace and 
# upload\publish each one to an ArcGIS Server, Spaital Data Server, or My Hosted Services
sdList = arcpy.ListFiles("*.sd")
for inSdFile in sdList:
    print "Publishing " + inSdFile
    try:
        arcpy.UploadServiceDefinition_server(inSdFile, inServer)        
    except Exception, e:
        print e.message
