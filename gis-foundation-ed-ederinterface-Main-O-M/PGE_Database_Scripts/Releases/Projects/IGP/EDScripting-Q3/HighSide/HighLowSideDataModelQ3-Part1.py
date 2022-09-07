# ---------------------------------------------------------------------------
# PG&E ADMS Q3 DataModel Changes py script - HighSideConfiguration/LowSideConfiguration
# Part 1 - Additions and modifications to data model - run before Bulk Updates
# ---------------------------------------------------------------------------

# Import system modules
import sys, string, os, arcgisscripting
import arcpy

# Change this value to the location of the sde file you are using.
Data_Connection = "Database Connections\PT1T_EDGIS.sde"
#Data_Connection = "C:\\3QDataModel\\DEV.gdb"

try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Initializing the workspace of: ", Data_Connection
	arcpy.Workspace = Data_Connection
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
	# Update Domains
	print "Adding/Updating Geodatabase ", Data_Connection, " domains to data model"
	domain = "Trans Bank Low Side Config"
	print "Modifying Domain -- " + domain
	arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, ["3D","4D","SP","Y","UNK"])
	domDict = { 'OD':'Open Delta (3-wire)', 'OY':'Open Wye', 'CD':'Closed Delta (3-Wire)', 'YG':'Wye Grounded', 'YU':'Wye Ungrounded (Floating)', 'ZZ':'Zig Zag', '2W':'Two-Wire Single Phase', '3W':'Three-Wire Single Phase', 'CDCG':'Closed Delta Corner Grounded', 'CDCT':'Closed Delta Center Tapped (4-Wire)', 'ODCT':'Open Delta Center Tapped (4-Wire)' }
	for code in domDict:
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
	arcpy.SortCodedValueDomain_management(Data_Connection,domain,"CODE","ASCENDING")
	domain = "Trans Bank High Side Config"
	print "Modifying Domain -- " + domain
	arcpy.DeleteCodedValueFromDomain_management(Data_Connection, domain, [ 'D', 'Y' ] )
	domDict = { 'CD':'Closed Delta', 'YG':'Wye Grounded', 'YU':'Wye Ungrounded (Floating)', 'ZZ':'Zig Zag', 'CDCG':'Closed Delta Corner Grounded' }
	for code in domDict:
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
	arcpy.SortCodedValueDomain_management(Data_Connection,domain,"CODE","ASCENDING")
	domain = "Cap Bank High Side Config"
	print "Adding Domain -- " + domain
	arcpy.CreateDomain_management(Data_Connection, domain, domain, 'TEXT', 'CODED')
	domDict = { 'CD':'Closed Delta', 'YG':'Wye Grounded', 'YU':'Wye Ungrounded (Floating)'}
	for code in domDict:        
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
	arcpy.SortCodedValueDomain_management(Data_Connection,domain,"CODE","ASCENDING")
	domain = "Volt Reg Bank High Side Config"
	print "Adding Domain -- " + domain
	arcpy.CreateDomain_management(Data_Connection, domain, domain, 'TEXT', 'CODED')
	domDict = { 'LG':'Single Phase Line - Ground','LL':'Single Phase Line - Line','OD':'Open Delta','OY':'Open Wye','CD':'Closed Delta', 'YG':'Wye Grounded'}
	for code in domDict:        
		arcpy.AddCodedValueToDomain_management(Data_Connection, domain, code, domDict[code])
	arcpy.SortCodedValueDomain_management(Data_Connection,domain,"CODE","ASCENDING")
	# Add Fields
	print "Updating Geodatabase ", Data_Connection, " to add new fields to data model"
	feature = dataOwner + "Transformer"
	print "-- Adding new LOWSIDECONFIGURATION field to " + feature
	arcpy.AddField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "TEXT", "", "", "5", "Low Side Configuration", "NULLABLE", "NON_REQUIRED", "")
	feature = dataOwner + "CapacitorBank"
	print "-- Adding new HIGHSIDECONFIGURATION field to " + feature
	arcpy.AddField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION", "TEXT", "", "", "5", "High Side Configuration", "NULLABLE", "NON_REQUIRED", "")
	feature = dataOwner + "VoltageRegulator"
	print "-- Adding new HIGHSIDECONFIGURATION_NEW field to " + feature
	arcpy.AddField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION_NEW", "TEXT", "", "", "5", "High Side Configuration", "NULLABLE", "NON_REQUIRED", "")
	
	#Assigning Domains
	print "Updating Geodatabase ", Data_Connection, " to Assign domains to new fields"
	feature = dataOwner + "Transformer"
	print "-- Assigning Domain to new LOWSIDECONFIGURATION field on " + feature
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "1: Distribution Overhead")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "2: Distribution Subsurface")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "3:Distribution Padmount")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "4: Equipment")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "5:Network")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "7: Street Light")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "LOWSIDECONFIGURATION", "Trans Bank Low Side Config", "8: Secondary")
	
	feature = dataOwner + "CapacitorBank"
	print "-- Assigning Domain to new HIGHSIDECONFIGURATION field on " + feature
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION", "Cap Bank High Side Config", "1: Fixed Bank Capacitor")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION", "Cap Bank High Side Config", "2: Switched Bank Capacitor")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION", "Cap Bank High Side Config", "3: Series Capacitor")
	
	feature = dataOwner + "VoltageRegulator"
	print "-- Assigning Domain to new HIGHSIDECONFIGURATION_NEW field on " + feature
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION_NEW", "Volt Reg Bank High Side Config", "1: Overhead Regulator")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION_NEW", "Volt Reg Bank High Side Config", "2: Pad Mounted Regulator")
	arcpy.AssignDomainToField_management (Data_Connection + elecDataset + feature, "HIGHSIDECONFIGURATION_NEW", "Volt Reg Bank High Side Config", "3: Booster")
	print "HighLowSideDataModelQ3-Part1 Python Script Finished Successfully!"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]
