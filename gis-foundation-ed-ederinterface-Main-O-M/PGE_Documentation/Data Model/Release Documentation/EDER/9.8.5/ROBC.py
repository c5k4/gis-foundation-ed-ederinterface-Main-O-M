import arcpy
import sys, string, os, arcgisscripting

Data_Connection='Database Connections\\EDGIS@A1D.sde'
arcpy.Workspace =  Data_Connection

#<25339>
print ""
print "*** CR 25339 ***"
try:

     arcpy.DeleteField_management(Data_Connection+"\\ROBC", "ROBC")
     arcpy.DeleteField_management(Data_Connection+"\\ROBC", "SUBBLOCK")
     print "ROBC and SUBBLOCK fields deleted in ROBC table..."

     arcpy.AddField_management(Data_Connection+"\\ROBC", "DESIREDROBC", "SHORT", 2, "", "", "Desired ROBC", "NULLABLE", "", "Rotating Outage Block Codes")
     arcpy.AddField_management(Data_Connection+"\\ROBC", "ESTABLISHEDROBC", "SHORT", 2, "", "", "Established ROBC", "NULLABLE", "", "Rotating Outage Block Codes")
     arcpy.AddField_management(Data_Connection+"\\ROBC", "DESIREDSUBBLOCK", "TEXT", "", "", "2", "Desired Sub Block", "NULLABLE", "", "Sub Block Codes")
     arcpy.AddField_management(Data_Connection+"\\ROBC", "ESTABLISHEDSUBBLOCK", "TEXT", "", "", "2", "Established Sub Block", "NULLABLE", "", "Sub Block Codes")
     print "DESIREDROBC, ESTABLISHEDROBC, DESIREDSUBBLOCK and ESTABLISHEDSUBBLOCK fields added in ROBC table..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-25339: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-25339: Unexpected error:", sys.exc_info()[0]
#<25339>


#<25587>
print ""
print "*** CR 25587 ***"
try:
     arcpy.AddField_management(Data_Connection+"\\ROBC", "HASESSENTIALIDC", "SHORT", 1, "", "", "HASESSENTIALIDC", "NULLABLE", "", "Yes No Numeric")
     arcpy.AddField_management(Data_Connection+"\\ROBC", "LASTCHECKEDDATE", "DATE", "", "", "", "Last Checked Date", "NULLABLE", "", "")
     
     print "HASESSENTIALIDC and LASTCHECKEDDATE fields added in ROBC table..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-25587: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-25587: Unexpected error:", sys.exc_info()[0]
#<25587>
	

#<22399>
print ""
print "*** CR 25671 ***"
try:
     #Store all the domain values in a dictionary with the domain code as the "key" and the 
     #domain description as the "value" (domDict[code])

     domDict = {'PGE_ROBC':'PGE_ROBC', 'PGE_PERTIALCURTAILPOINT':'PGE_PERTIALCURTAILPOINT'}
    
     # Process: Add valid code/values to the domain 
     #use a for loop to cycle through all the domain codes in the dictionary
     print "Adding values to PGE ED Object Class Model Name domain..."
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'PGE ED Object Class Model Name', code, domDict[code])

     print "PGE ED Object Class Model Name domain code/value added..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-25671: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-25671: Unexpected error:", sys.exc_info()[0]
#<25671>

#<25559>
print ""
print "*** CR 25559 ***"
try:
     arcpy.AddField_management(Data_Connection+"\\Transformer", "SSDGUID", "TEXT", "", "", 38, "Source Side Device GUID", "NULLABLE", "", "")
     arcpy.AddField_management(Data_Connection+"\\PrimaryMeter", "SSDGUID", "TEXT", "", "", 38, "Source Side Device GUID", "NULLABLE", "", "")
     arcpy.AddField_management(Data_Connection+"\\DCRectifier", "SSDGUID", "TEXT", "", "", 38, "Source Side Device GUID", "NULLABLE", "", "")
     
     print "SSDGUID field added in Transformer, PrimaryMeter and DCRectifier..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-25559: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-25559: Unexpected error:", sys.exc_info()[0]
#<25559>
