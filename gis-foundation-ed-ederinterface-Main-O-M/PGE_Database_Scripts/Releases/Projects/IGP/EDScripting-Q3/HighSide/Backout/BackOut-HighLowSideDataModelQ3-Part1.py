# ---------------------------------------------------------------------------
# PG&E ADMS Q3 DataModel Changes py script - HighSideConfiguration/LowSideConfiguration
# Part 1 - Additions and modifications to data model - run before Bulk Updates
# ---------------------------------------------------------------------------

# Import system modules
import sys, string, os, arcgisscripting
import arcpy

# Change this value to the location of the sde file you are using.
#Data_Connection = "Database Connections\EDGIS_PT1D.sde"
Data_Connection = "C:\\3QDataModel\\DEV.gdb"

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
	# Lets get to work      
	#standalone = "\\"
	elecDataset = "\\EDGIS.ElectricDataset\\"
	dataOwner = "EDGIS."
	# Delete Added Fields
	print "Updating Geodatabase ", Data_Connection, " to remove added fields to data model"
	
	feature = dataOwner + "Transformer"
	print "-- Backing out LOWSIDECONFIGURATION field from " + feature
	arcpy.DeleteField_management(Data_Connection + elecDataset + feature, 'LOWSIDECONFIGURATION' )
	
	feature = dataOwner + "CapacitorBank"
	print "-- Backing out HIGHSIDECONFIGURATION field from " + feature
	arcpy.DeleteField_management(Data_Connection + elecDataset + feature, 'HIGHSIDECONFIGURATION' )
	
	feature = dataOwner + "VoltageRegulator"
	print "-- Backing out HIGHSIDECONFIGURATION_NEW char field from " + feature
	x = False  
	lstFields = arcpy.ListFields(Data_Connection + elecDataset + feature)
	for field in lstFields:
		if field.name == "HIGHSIDECONFIGURATION_NEW":  
			print "HIGHSIDECONFIGURATION_NEW Field exists"  
			x = True  
	if x == True:  
		print "-- Backing out HIGHSIDECONFIGURATION_NEW field from " + feature
		arcpy.DeleteField_management(Data_Connection + elecDataset + feature, 'HIGHSIDECONFIGURATION_NEW' )
	else:
		print "HIGHSIDECONFIGURATION_NEW Field does not exist"
		
	#Update Domains
	print "Adding/Updating Geodatabase ", Data_Connection, " domains to data model"
	
	domain = "Trans Bank Low Side Config"
	print "Modifying Domain -- " + domain
	
	# Backing out Adds - domDict = { 'OD':'Open Delta (3-wire)', 'OY':'Open Wye', 'CD':'Closed Delta (3-Wire)', 'YG':'Wye Grounded', 'YU':'Wye Ungrounded (Floating)', 'ZZ':'Zig Zag', '2W':'Two-Wire Single Phase', '3W':'Three-Wire Single Phase', 'CDCG':'Closed Delta Corner Grounded', 'CDCT':'Closed Delta Center Tapped (4-Wire)', 'ODCT':'Open Delta Center Tapped (4-Wire)' }
	arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, [ 'OD', 'OY', 'CD', 'YG', 'YU', 'ZZ', '2W', '3W', 'CDCG', 'CDCT', 'ODCT'  ] )
	# Backing out deletes - arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, [ '3D', '4D', 'SP', 'Y', 'UNK' ] )
	domDict = { '3D':'3 Wire delta', '4D':'4 Wire delta', 'SP':'SinglePhase', 'Y':'Wye', 'UNK':'Unknown'}
	for code in domDict:        
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
		
	domain = "Trans Bank High Side Config"
	print "Backing Domain Changes for -- " + domain
	# backing out Added domDict = { 'CD':'Closed Delta', 'YG':'Wye Grounded', 'YU':'Wye Ungrounded (Floating)', 'ZZ':'Zig Zag', 'CDCG':'Closed Delta Corner Grounded' }
	arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, [ 'CD', 'YG', 'YU', 'ZZ', 'CDCG' ] )
	# backing out deletes arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, [ 'D', 'Y' ] )
	domDict = { 'D':'Delta', 'Y':'Wye' }
	
	for code in domDict:        
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
		
	#Delete  Domain
	domain = "Cap Bank High Side Config"
	print "Backing out Added Domain -- " + domain
	arcpy.DeleteDomain_management(Data_Connection, domain)
	
	domain = "Volt Reg Bank High Side Config"
	print "Backing out Added Domain -- " + domain
	arcpy.DeleteDomain_management(Data_Connection, domain)
	print "BackOut-HighLowSideDataModelQ3-Part1 Python Script Finished Successfully!"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
