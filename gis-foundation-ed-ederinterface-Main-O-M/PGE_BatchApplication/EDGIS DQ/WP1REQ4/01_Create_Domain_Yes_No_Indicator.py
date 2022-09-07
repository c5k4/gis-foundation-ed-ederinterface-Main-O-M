# Import system modules
import sys, string, os, arcgisscripting
import arcpy
# from arcpy import env

# Local variables...
# Change this value to the location of the sde file you are using.
Data_Connection = "Database Connections\\WEBR.sde"
# Change this value to the name of the domain to add.
Domain_Name_To_ADD = "Yes No Indicator"
Domain_Description_To_ADD = "Yes No Indicator"
#Store all the domain values in a dictionary as the "code":"description" 
Domain_Code_and_Description_Dictionary = {"N" : "No","Y" : "Yes"}
		   
# Domain_Storage_Type = "SHORT"
# Domain_Storage_Type = "LONG"
Domain_Storage_Type = "TEXT"
# Domain_Storage_Type = "FLOAT"
# Domain_Storage_Type = "DOUBLE"
# Domain_Storage_Type = "DATE"
Domain_Type= "CODED"
#Domain_Type= "RANGE"
try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Initializing the workspace of: ", Data_Connection
	arcpy.Workspace =  Data_Connection
except:
	# If an error occurred while running a tool, print the messages
	print "UnSuccessfully completed connecting to workspace, error was: "
	print arcpy.GetMessages()
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Try to delete the domain
	print "Creating the Domain of : ", Domain_Name_To_ADD
	arcpy.CreateDomain_management(Data_Connection, Domain_Name_To_ADD, Domain_Description_To_ADD, Domain_Storage_Type, Domain_Type)
	print "SUCCESS: CREATED the Domain of : ", Domain_Name_To_ADD
	# Add Values to domain
	print "Adding Values to the domain of : ", Domain_Name_To_ADD
	keyList=Domain_Code_and_Description_Dictionary.keys()
	for code in sorted(keyList):         
		value=Domain_Code_and_Description_Dictionary[code]
		print "Adding the code:value of ", code,":",value
		arcpy.AddCodedValueToDomain_management(Data_Connection, Domain_Name_To_ADD, code, value)
		print "SUCCESS: Added the code:value of ", code,":",value
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
