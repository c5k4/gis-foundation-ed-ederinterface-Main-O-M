import arcpy
Data_Connection='Database Connections\\PGE1_EDGIS.sde'

print "20072"
#<20072>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Pole Height", "222", "222")
print "added code to domain"
#</20072>

print "20067"
#<20067>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "1", "13\" X 24\"")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "2", "17\" X 30\"")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "3", "24\" X 36\"")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "4", "30\" X 48\"")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "5", "3\' X 5\'")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "5E", "3\' X 5\' with extension")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "6", "4\' X 6\'")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "6E", "4\' X 6\' with extension")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "7", "4\'6\" X 8\'6\"")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Structure Size", "7E", "4\'6\" X 8\'6\" with extension")
print "added code to domain"
#</20067>

print "19516"
#<19516>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Joint Pole Members", "CX", "Colfax Exchange")
print "added code to domain"
#</19516>

print "19469"
#<19469>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Secondary Voltage", "35", "125/216")
print "added code to domain"
#</19469>

print "19468"
#<19468>
for fc in ['MaintenancePlat', 'PGE_LOPC']:
	arcpy.AlterField_management(Data_Connection+'\\'+fc, 'AREANAME', '', 'Region')
	print 'Altered field'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+fc, "AREANAME", "Region", "1: Electric Plat to Electric Distribution Map")
	print 'assigned domain to subtype'
#</19468>

print "19105"
#<19105>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Rated Amps", "4000", "4000A")
print "added code to domain"
#</19105>

print "17716"
#<17716>
arcpy.CreateDomain_management(Data_Connection, "Essential Customer IDC", "Essential Customer IDC", "TEXT", "CODED")
arcpy.AddCodedValueToDomain_management(Data_Connection, "Essential Customer IDC", "N", "No")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Essential Customer IDC", "Y", "Yes")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Essential Customer IDC", "E", "Excluded from ROBC")
print "added code to domain"
arcpy.AssignDomainToField_management(Data_Connection+'\\'+"ServicePoint", "ESSENTIALCUSTOMERIDC", "Essential Customer IDC", "1: Electric Service")
print 'assigned domain to subtype'
arcpy.AssignDomainToField_management(Data_Connection+'\\'+"ServicePoint", "ESSENTIALCUSTOMERIDC", "Essential Customer IDC", "2: Gas Service")
print 'assigned domain to subtype'
arcpy.AssignDomainToField_management(Data_Connection+'\\'+"ServicePoint", "ESSENTIALCUSTOMERIDC", "Essential Customer IDC", "3: DC Service Point")
print 'assigned domain to subtype'
#</17716>

print "20488"
#<20488>
arcpy.AlterField_management(Data_Connection+'\\'+'DuctDefinition', 'DUCTSIZE', '', 'Draw Size')
print 'Altered field'
arcpy.DeleteField_management(Data_Connection+'\\'+'DuctDefinition', 'ACTUALSIZE')
print 'Deleted field'
arcpy.AddField_management (Data_Connection +"\\"+"DuctDefinition", 'ACTUALSIZE', 'TEXT', '', '', '5', 'Actual Size', 'NULLABLE', '', 'ULS Size-coded value')
arcpy.AssignDefaultToField_management(Data_Connection+'\\DuctDefinition', "ACTUALSIZE", '5',["1: Duct Bank", "2: Conduit"])
print "Added field"
#</20488>

print "20459"
#<20459>
from arcpy import env
 
try:
	env.workspace = Data_Connection
	
	# Set local parameters
	domName = "PGE Circuit Color"
	
	print "creating domain "+domName
	# Process: Create the coded value domain
	arcpy.CreateDomain_management(Data_Connection, domName, "PGE Circuit Color", "TEXT", "CODED")

	#Store all the domain values in a dictionary with the domain code as the "key" and the 
	#domain description as the "value" (domDict[code])
	domDict = {"255 0 0":"Red", "0 255 0": "Green", "0 0 255": "Blue", "128 0 128": "Purple", "255 192 203": "Pink", "0 255 255": "Cyan", "0 128 128": "Teal", "255 255 0": "Yellow", "128 128 0": "Olive", "255 165 0": "Orange", "255 140 0": "Dark Orange", "165 42 42": "Brown", "128 128 128": "Gray", "0 0 0": "Black"}

	# Process: Add valid material types to the domain
	#use a for loop to cycle through all the domain codes in the dictionary
	for code in domDict:
		print "adding code "+code+" to domain "+domName
		arcpy.AddCodedValueToDomain_management(Data_Connection, domName, code, domDict[code])

except Exception, e:
	# If an error occurred, print line number and error message
	import traceback, sys
	tb = sys.exc_info()[2]
	print "Line %i" % tb.tb_lineno
	print e.message
	
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", "PGE_CircuitColor", "PGE_CircuitColor")
print "added code to domain"
arcpy.AddField_management (Data_Connection +"\\"+"CircuitSource", 'CIRCUITCOLOR', 'TEXT', '', '', '50', 'Circuit Color', 'NULLABLE', '', 'PGE Circuit Color')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+"PriOHConductor", 'CIRCUITCOLOR', 'TEXT', '', '', '50', 'Circuit Color', 'NULLABLE', '', 'PGE Circuit Color')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+"PriUGConductor", 'CIRCUITCOLOR', 'TEXT', '', '', '50', 'Circuit Color', 'NULLABLE', '', 'PGE Circuit Color')
print "Added field"
#</20459>

print "20553"
#<20553>
arcpy.AddField_management (Data_Connection +"\\"+"Duct", 'ACTUALSIZE', 'TEXT', '', '', '5', 'Actual Size', 'NULLABLE', '', 'ULS Size-coded value')
print "Added field"
#</20553>

print "20060"
#<20060>
arcpy.CreateDomain_management(Data_Connection, "FI and LS Models", "FI and LS Models", "TEXT", "CODED")
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "MM3", "MM3")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "LHMV", "Lighthouse MV")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "A360", "Autoranger 360")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "NLM", "Navigator LM")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "LT", "Load Tracker LP")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "FI and LS Models", "HUG", "Horstmann Underground")
print "added code to domain"
#</20060>

print "20062"
#<20062>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Device Manufacturer", "HM", "Horstmann")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Device Manufacturer", "SL", "Schweitzer Engineering Labs")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Device Manufacturer", "SE", "Sentient")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Device Manufacturer", "SS", "Silver Spring Networks")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Device Manufacturer", "TG", "Tollgrade")
print "added code to domain"
#</20062>

print "20063"
#<20063>
arcpy.AddCodedValueToDomain_management(Data_Connection, "SCADA Communication", "CELL", "Cellular")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "SCADA Communication", "OTH", "Other")
print "added code to domain"
#</20063>

print "20065"
#<20065>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Actuating Current Values", "15", "15-Amp")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Actuating Current Values", "20", "20-Amp")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Actuating Current Values", "300", "300-Amp")
print "added code to domain"
#</20065>

print "20047"
#<20047>
updateFC_name = 'FaultIndicator'
print "Starting fault indicator changes"
arcpy.AddSubtype_management(Data_Connection +"\\"+updateFC_name, "2", "Communicating FI")
print "Added subtype"
arcpy.AddSubtype_management(Data_Connection +"\\"+updateFC_name, "3", "Line Sensor")
print "Added subtype"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MINREQLINEAMPS', 'LONG', '6', '', '', 'Minimum Required Line Current', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MAXRATEDAMPS', 'LONG', '6', '', '', 'Maximum Rated Current', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MANUFACTURER', 'TEXT', '', '', '40', 'Device Manufacturer', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MODELA', 'TEXT', '', '', '40', 'Device Model A', 'NULLABLE', '', 'FI and LS Models')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MODELB', 'TEXT', '', '', '40', 'Device Model B', 'NULLABLE', '', 'FI and LS Models')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MODELC', 'TEXT', '', '', '40', 'Device Model C', 'NULLABLE', '', 'FI and LS Models')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'COMMUNICATION', 'TEXT', '', '', '20', 'Communication Platform', 'NULLABLE', '', 'SCADA Communication')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'CELLULARPROVIDER', 'TEXT', '', '', '40', 'Cellular Provider', 'NULLABLE', '', 'Carrier')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MACADDRESSA', 'TEXT', '', '', '20', 'MAC Address A', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MACADDRESSB', 'TEXT', '', '', '20', 'MAC Address B', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'MACADDRESSC', 'TEXT', '', '', '20', 'MAC Address C', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'NETWORKOPSTATE', 'TEXT', '', '', '10', 'Network Operational State', 'NULLABLE', '', 'Active Indicator')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'SERIALNUMBERB', 'TEXT', '', '', '25', 'Serial Number B', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'SERIALNUMBERC', 'TEXT', '', '', '25', 'Serial Number C', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'BATTERYDATEB', 'DATE', '', '', '', 'Battery Date B', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'BATTERYDATEC', 'DATE', '', '', '', 'Battery Date C', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'YEARMANUFACTUREDB', 'SHORT', '', '', '5', 'Year Manufactured B', 'NULLABLE', '', 'Year')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, 'YEARMANUFACTUREDC', 'SHORT', '', '', '5', 'Year Manufactured C', 'NULLABLE', '', 'Year')
print "Added field"
arcpy.AlterField_management(Data_Connection+'\\'+updateFC_name, 'SERIALNUMBER', '', 'Serial Number A')
print 'Altered field'
arcpy.AlterField_management(Data_Connection+'\\'+updateFC_name, 'BATTERYDATE', '', 'Battery Date A')
print 'Altered field'
arcpy.AlterField_management(Data_Connection+'\\'+updateFC_name, 'YEARMANUFACTURED', '', 'Year Manufactured A')
print 'Altered field'

subList = ['1: Fault Indicator','2: Communicating FI','3: Line Sensor']

for sub in subList:
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "MANUFACTURER", "Device Manufacturer", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "MODELA", "FI and LS Models", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "MODELB", "FI and LS Models", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "MODELC", "FI and LS Models", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "COMMUNICATION", "SCADA Communication", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "CELLULARPROVIDER", "Carrier", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "NETWORKOPSTATE", "Active Indicator", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "YEARMANUFACTUREDB", "Year", sub)
	print 'assigned domain to subtype'
	arcpy.AssignDomainToField_management(Data_Connection+'\\'+updateFC_name, "YEARMANUFACTUREDC", "Year", sub)
	print 'assigned domain to subtype'
#</20047>

print "20611"
#<20611>
# Set the necessary product code
import sys
from arcpy import env

def main():
	print "**************************************"
	print "*** Adding new Source Side Device Fields ***"
	print "**************************************"
	print ""
	print ""

	FeatureClasses = ["EDGIS.ElectricDataset\\EDGIS.CapacitorBank", "EDGIS.ElectricDataset\\EDGIS.DCRectifier", "EDGIS.ElectricDataset\\EDGIS.DynamicProtectiveDevice", "EDGIS.ElectricDataset\\EDGIS.FaultIndicator", "EDGIS.ElectricDataset\\EDGIS.Fuse", "EDGIS.ElectricDataset\\EDGIS.OpenPoint", "EDGIS.ElectricDataset\\EDGIS.SmartMeterNetworkDevice", "EDGIS.ElectricDataset\\EDGIS.StepDown", "EDGIS.ElectricDataset\\EDGIS.Switch", "EDGIS.ElectricDataset\\EDGIS.Tie", "EDGIS.ElectricDataset\\EDGIS.Transformer", "EDGIS.ElectricDataset\\EDGIS.VoltageRegulator"]

	for FeatureClass in FeatureClasses:
		try:
			print "Attemping to add Protective SSD field to: " + Data_Connection + "\\" + FeatureClass
			arcpy.AddField_management(Data_Connection + "\\" + FeatureClass, "PROTECTIVESSD", "TEXT", "", "", "20", "Protective SSD", "NULLABLE", "NON_REQUIRED", "")
		except:
			print "Unexpected error adding field: "
			print arcpy.GetMessages()
		try:
			print "Attemping to add Auto Protective SSD field to: " + Data_Connection + "\\" + FeatureClass
			arcpy.AddField_management(Data_Connection + "\\" + FeatureClass, "AUTOPROTECTIVESSD", "TEXT", "", "", "20", "Auto Protective SSD", "NULLABLE", "NON_REQUIRED", "")
		except:
			print "Unexpected error adding field: "
			print arcpy.GetMessages()

if __name__ == '__main__': main()

arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", "PGE_PROTECTIVESSD", "PGE_PROTECTIVESSD")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", "PGE_AUTOPROTECTIVESSD", "PGE_AUTOPROTECTIVESSD")
print "added code to domain"

#</20611>

print "20768"
#<20768>
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", "PGE_SUBSTATIONID", "PGE_SUBSTATIONID")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Field Model Name", "PGE_SUBSTATIONNAME", "PGE_SUBSTATIONNAME")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "PGE ED Object Class Model Name", "PGE_SUBSTATION", "PGE_SUBSTATION")
print "added code to domain"
#</20768>

print "20949"
#<20949>
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'TIFFMAPSTATUS', 'TEXT', '', '', '255', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PDFMAPSTATUS', 'TEXT', '', '', '255', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'TIFFSTARTDATE', 'DATE', '', '', '', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'TIFFENDDATE', 'DATE', '', '', '', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PDFSTARTDATE', 'DATE', '', '', '', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PDFENDDATE', 'DATE', '', '', '', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'TIFFMXD', 'TEXT', '', '', '200', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PDFMXD', 'TEXT', '', '', '200', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'TIFFMACHINENAME', 'TEXT', '', '', '50', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PDFMACHINENAME', 'TEXT', '', '', '50', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'FAILURECOUNT', 'SHORT', '2', '', '', '', 'NULLABLE', '', '')
print "Added field"
arcpy.AddField_management (Data_Connection +"\\"+'PGE_MAPNUMBERCOORDLUT', 'PRIORITY', 'LONG', '10', '', '', '', 'NULLABLE', '', '')
print "Added field"
#</20949>
