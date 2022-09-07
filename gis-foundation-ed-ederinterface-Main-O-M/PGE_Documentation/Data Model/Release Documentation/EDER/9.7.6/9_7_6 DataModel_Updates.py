import arcpy
import sys, string, os, arcgisscripting

Data_Connection='Database Connections\\EDGIS@LBGISS2Q.sde'
arcpy.Workspace =  Data_Connection

#<24646>
print ""
print "*** CR 24646 ***"

arcpy.CreateDomain_management(Data_Connection, 'Spot or Grid', 'Spot or Grid', 'TEXT', 'CODED')
print "Spot or Grid domain created..."

#Store all the domain values in a dictionary with the domain code as the "key" and the 
#domain description as the "value" (domDict[code])

domDict = {'S':'Spot', 'G': 'Grid'}

    
# Process: Add code/values to the domain
#use a for loop to cycle through all the domain codes in the dictionary
try:
     #Remove existing domain from field
     print "Removing existing domain from SPOTIDC field..."
     arcpy.RemoveDomainFromField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC')
     arcpy.RemoveDomainFromField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC', [1, 2, 3, 4, 5, 7, 8])
     print "Existing domain from SPOTIDC field removed..."
     
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'Spot or Grid', code, domDict[code])

     print "Spot or Grid domain code/value added..."
     
     # AssignDomainToField:
     print "Assigning Spot or Grid domain to SPOTIDC field..."
     arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC', 'Spot or Grid', 5)
     arcpy.AssignDefaultToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC', 'G', 5)

     print "Spot or Grid domain assigned to SpotIDC field of Transformer"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24646: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24646: Unexpected error:", sys.exc_info()[0]
#<24646>


#<24647>
print ""
print "*** CR 24647 ***"
try:
     arcpy.CreateDomain_management(Data_Connection, 'Insulating Fluid Type', 'Insulating Fluid Type', 'TEXT', 'CODED')
     print "Insulating Fluid Type domain created..."

     #Store all the domain values in a dictionary with the domain code as the "key" and the 
     #domain description as the "value" (domDict[code])

     domDict = {'219':'G&W 219 Fluid', 'BT': 'Biotemp (natural ester base)', 'DRY':'Dry Type', \
                'MIN': 'Mineral Oil', 'NE':'Natural Ester Fluid', 'PB': 'Polybutene Fluid', \
                'RT': ' R-Temp', 'SIL':'Silicone Fluid', 'UK': 'Unknown', 'FR3': 'FR3'}
    
     # Process: Add valid code/values to the domain 
     #use a for loop to cycle through all the domain codes in the dictionary
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'Insulating Fluid Type', code, domDict[code])

     print "Insulating Fluid Type domain code/value added..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24647: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24647: Unexpected error:", sys.exc_info()[0]
#<24647>

#<24673>
print ""
print "*** CR 24673 ***"
updateFC_name_list = ["EDGIS.SWITCH","EDGIS.OPENPOINT"]
#Name of the new field
new_field_name = "INSULATINGFLUIDTYPE"
new_field_alias = "Insulating Fluid Type"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 
# new_field_type = "FLOAT" # Numeric values with fractional values within a specific range. 
# new_field_type = "DOUBLE" # Numeric values with fractional values within a specific range. 
# new_field_type = "SHORT" # Numeric values without fractional values within a specific range; coded values. 
# new_field_type = "LONG" # Numeric values without fractional values within a specific range. 
# new_field_type = "DATE" # Date and/or Time. 
# new_field_type = "BLOB" # Images or other multimedia. 
# new_field_type = "RASTER" # Raster images. 
# new_field_type = "GUID" # GUID values

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "50"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
	for updateFC_name in updateFC_name_list:
		print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class " ,updateFC_name
		print "new_field_name        " , new_field_name
		print "new_field_type        " , new_field_type,
		print "new_field_precision   " , new_field_precision
		print "new_field_scale       " , new_field_scale
		print "new_field_length      " , new_field_length
		print "new_field_alias       " , new_field_alias
		print "new_field_is_nullable " , new_field_is_nullable
		print "new_field_is_required " , new_field_is_required
		print "new_field_domain      " , new_field_domain
		print "adding ...."
		arcpy.AddField_management (Data_Connection +"\\"+updateFC_name, new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
		print "SUCCESS : created the field on the featureclass"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24673: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24673: Unexpected error:", sys.exc_info()[0]
#<24673>


#<24674>
print ""
print "*** CR 24674 ***"
#use a for loop to cycle through all the domain codes in the dictionary
try:
      # AssignDomainToField: 
      arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'INSULATINGFLUIDTYPE', 'Insulating Fluid Type', '5')
      print "Insulating Fluid Type domain assigned to INSULATINGFLUIDTYPE field of Transformer"
      arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMERDEVICE", 'INSULATINGFLUIDTYPE', 'Insulating Fluid Type', '1')
      print "Insulating Fluid Type domain assigned to INSULATINGFLUIDTYPE field of TransformerDevice"
      arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.SWITCH", 'INSULATINGFLUIDTYPE', 'Insulating Fluid Type', '7')
      print "Insulating Fluid Type domain assigned to INSULATINGFLUIDTYPE field of Switch"
      arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.OPENPOINT", 'INSULATINGFLUIDTYPE', 'Insulating Fluid Type', '2')
      print "Insulating Fluid Type domain assigned to INSULATINGFLUIDTYPE field of OpenPoint"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24674: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24674: Unexpected error:", sys.exc_info()[0]
#<24674>
	
#<24692>
print ""
print "*** CR 24692 ***"
#Name of the new field
new_field_name = "SUPERVISORYCONTROL"
new_field_alias = "Supervisory Control"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "1"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
        print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class EDGIS.TRANSFORMERDEVICE"
        print "new_field_name        " , new_field_name
        print "new_field_type        " , new_field_type,
        print "new_field_precision   " , new_field_precision
        print "new_field_scale       " , new_field_scale
        print "new_field_length      " , new_field_length
        print "new_field_alias       " , new_field_alias
        print "new_field_is_nullable " , new_field_is_nullable
        print "new_field_is_required " , new_field_is_required
        print "new_field_domain      " , new_field_domain
        print "adding ...."
        arcpy.AddField_management (Data_Connection +"\\"+"EDGIS.TRANSFORMERDEVICE", new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
        print "SUCCESS : created the field on the featureclass"

        # AssignDomainToField
        arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMERDEVICE", new_field_name, 'Yes No Indicator')
        print "Yes No Indicator domain assigned to SUPERVISORYCONTROL field of TRANSFORMERDEVICE"
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24692: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24692: Unexpected error:", sys.exc_info()[0]
#<24692>


#<24696>
print ""
print "*** CR 24696 ***"
arcpy.CreateDomain_management(Data_Connection, 'Bottom or Top', 'Bottom or Top', 'TEXT', 'CODED')
print "Bottom or Top domain created..."

#Store all the domain values in a dictionary with the domain code as the "key" and the 
#domain description as the "value" (domDict[code])

domDict = {'BOT':'Bottom', 'TOP': 'Top'}
    
# Process: Add valid material types to the domain
#use a for loop to cycle through all the domain codes in the dictionary
try:
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'Bottom or Top', code, domDict[code])

     print "Bottom or Top domain code/value added..."

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24696: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24696: Unexpected error:", sys.exc_info()[0]
#<24696>

#<24698>
print ""
print "*** CR 24698 ***"

#Name of the new field
new_field_name = "LINKLOCATION"
new_field_alias = "Link Location"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "5"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
        print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class EDGIS.NETWORKPROTECTOR"
        print "new_field_name        " , new_field_name
        print "new_field_type        " , new_field_type,
        print "new_field_precision   " , new_field_precision
        print "new_field_scale       " , new_field_scale
        print "new_field_length      " , new_field_length
        print "new_field_alias       " , new_field_alias
        print "new_field_is_nullable " , new_field_is_nullable
        print "new_field_is_required " , new_field_is_required
        print "new_field_domain      " , new_field_domain
        print "adding ...."
        arcpy.AddField_management (Data_Connection +"\\"+"EDGIS.NETWORKPROTECTOR", new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
        print "SUCCESS : created the field on the featureclass"

        # AssignDomainToField:
        arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.NETWORKPROTECTOR", 'LINKLOCATION', 'Bottom or Top')
        print "Bottom or Top domain assigned to LINKLOCATION field of NETWORKPROTECTOR"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24698: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24698: Unexpected error:", sys.exc_info()[0]
	
#<24698>

#<24700>
print ""
print "*** CR 24700 ***"

#Name of the new field
new_field_name = "SUPERVISORYCONTROL"
new_field_alias = "Supervisory Control"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "1"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
        print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class EDGIS.NETWORKPROTECTOR"
        print "new_field_name        " , new_field_name
        print "new_field_type        " , new_field_type,
        print "new_field_precision   " , new_field_precision
        print "new_field_scale       " , new_field_scale
        print "new_field_length      " , new_field_length
        print "new_field_alias       " , new_field_alias
        print "new_field_is_nullable " , new_field_is_nullable
        print "new_field_is_required " , new_field_is_required
        print "new_field_domain      " , new_field_domain
        print "adding ...."
        arcpy.AddField_management (Data_Connection +"\\"+"EDGIS.NETWORKPROTECTOR", new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
        print "SUCCESS : created the field on the featureclass"

        # AssignDomainToField:
        arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.NETWORKPROTECTOR", new_field_name, 'Yes No Indicator')
        print "Yes No Indicator domain assigned to SUPERVISORYCONTROL field of NETWORKPROTECTOR"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24700: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24700: Unexpected error:", sys.exc_info()[0]
#<24700>

#<24701>
print ""
print "*** CR 24701 ***"

#Name of the new field
new_field_name = "SUPERVISORYCONTROL"
new_field_alias = "Supervisory Control"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "1"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
        print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class EDGIS.OPENPOINT"
        print "new_field_name        " , new_field_name
        print "new_field_type        " , new_field_type,
        print "new_field_precision   " , new_field_precision
        print "new_field_scale       " , new_field_scale
        print "new_field_length      " , new_field_length
        print "new_field_alias       " , new_field_alias
        print "new_field_is_nullable " , new_field_is_nullable
        print "new_field_is_required " , new_field_is_required
        print "new_field_domain      " , new_field_domain
        print "adding ...."
        arcpy.AddField_management (Data_Connection +"\\"+"EDGIS.OPENPOINT", new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
        print "SUCCESS : created the field on the featureclass"

        # AssignDomainToField:
        arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.OPENPOINT", new_field_name, 'Yes No Indicator')
        print "Yes No Indicator domain assigned to SUPERVISORYCONTROL field of OPENPOINT"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24701: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24701: Unexpected error:", sys.exc_info()[0]

#<24701>

#<24702>
print ""
print "*** CR 24702 ***"
arcpy.CreateDomain_management(Data_Connection, 'Enclosure Type', 'Enclosure Type', 'TEXT', 'CODED')
print "Enclosure Type domain created..."

#Store all the domain values in a dictionary with the domain code as the "key" and the 
#domain description as the "value" (domDict[code])

domDict = {'DT':'Dust Tight', 'SB': 'Submersible'}
    
# Process: Add valid material types to the domain
#use a for loop to cycle through all the domain codes in the dictionary
try:
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'Enclosure Type', code, domDict[code])

     print "Enclosure Type domain code/value added..."

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24702: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24702: Unexpected error:", sys.exc_info()[0]

#<24702>

#<24703>
print ""
print "*** CR 24703 ***"

#Name of the new field
new_field_name = "ENCLOSURETYPE"
new_field_alias = "Enclosure Type"
# Type of the new field, just uncomment the correct one and comment out the others
new_field_type = "TEXT" # Names or other textual qualities. 

# New field precision, is optional, can be set to "" when not used. Describes the number of digits that can be stored in the field.
#      All digits are counted no matter what side of the decimal they are on.
new_field_precision = ""

# New field scale,  is optional, can be set to "" when not used. Sets the number of decimal places stored in a field. 
#     This parameter is only used in Float and Double data field types.
new_field_scale = ""

# New Field Length, is optional, can be set to "" when not used. The length of the field being added. 
#   This sets the maximum number of allowable characters for each record of the field. 
#   This option is only applicable on fields of type text or blob.
new_field_length = "5"

#New field is nullable, uncomment one of the options provided. A geographic feature where there is no associated attribute information. 
#     These are different from zero or empty fields and are only supported for fields in a geodatabase.
new_field_is_nullable = "NULLABLE" #  The field will allow null values. This is the default. 
# new_field_is_nullable = "NON_NULLABLE" #  The field will not allow null values. 

#New field is required, is optional, can be set to "" when not used. Uncomment one of the below options to use.
#     Specifies whether the field being created is a required field for the table; only supported for fields in a geodatabase.
# new_field_is_required = "NON_REQUIRED" # The field is not a required field. This is the default. 
# new_field_is_required = "REQUIRED" # The field is a required field. Required fields are permanent and can not be deleted.  
new_field_is_required = ""

#new_field_domain, is optional can be set to ## when it does not apply.
new_field_domain = ""

try:
	# Execute Add Field
        print "Using the geodatabase of : ", Data_Connection, "to create new field in the feature class EDGIS.NETWORKPROTECTOR"
        print "new_field_name        " , new_field_name
        print "new_field_type        " , new_field_type,
        print "new_field_precision   " , new_field_precision
        print "new_field_scale       " , new_field_scale
        print "new_field_length      " , new_field_length
        print "new_field_alias       " , new_field_alias
        print "new_field_is_nullable " , new_field_is_nullable
        print "new_field_is_required " , new_field_is_required
        print "new_field_domain      " , new_field_domain
        print "adding ...."
        arcpy.AddField_management (Data_Connection +"\\"+"EDGIS.NETWORKPROTECTOR", new_field_name, new_field_type, new_field_precision, new_field_scale, new_field_length, new_field_alias, new_field_is_nullable, new_field_is_required, new_field_domain)
        print "SUCCESS : created the field on the featureclass"

        # AssignDomainToField:
        arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.NETWORKPROTECTOR", new_field_name, 'Enclosure Type')
        print "Enclosure Type domain assigned to ENCLOSURETYPE field of NETWORKPROTECTOR"

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24703: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24703: Unexpected error:", sys.exc_info()[0]
#<24703>

#<24170>
print ""
print "*** CR 24170 ***"
    
# Process: Add code/value to Trans Unit Type Sub Surface domain
#use a for loop to cycle through all the domain codes in the dictionary
try:      
     arcpy.AddCodedValueToDomain_management(Data_Connection, 'Trans Unit Type Sub Surface', '16', '16 Underground Single Phase Self-Protected')

     print "Trans Unit Type Sub Surface domain code/value added..."

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-24170: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24170: Unexpected error:", sys.exc_info()[0]
#<24170>


#<22399>
print ""
print "*** CR 22399 ***"
try:
     #Store all the domain values in a dictionary with the domain code as the "key" and the 
     #domain description as the "value" (domDict[code])

     domDict = {'S- Schematic Appearance':'S- Schematic Appearance', 'S- Edgematch': 'S- Edgematch', \
                'S- Anno to Feature Association':'S- Anno to Feature Association', \
                'S- Anno or Feature Overstrike': 'S- Anno or Feature Overstrike', \
                'S- Symbol Rotation':'S- Symbol Rotation', \
                'S- Symbol & Line Spacing': 'S- Symbol & Line Spacing'}
    
     # Process: Add valid code/values to the domain 
     #use a for loop to cycle through all the domain codes in the dictionary
     print "Adding values to Error Type domain..."
     for code in domDict:        
          arcpy.AddCodedValueToDomain_management(Data_Connection, 'Error Type', code, domDict[code])

     print "Error Type domain code/value added..."
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-22399: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-24647: Unexpected error:", sys.exc_info()[0]
#<22399>

#<25349>
print ""
print "*** CR 25349 ***"

# Process: Add code/values to the domain
#use a for loop to cycle through all the domain codes in the dictionary
try:    
     # AssignDomainToField:
     print "Assigning 'Not Applicable' - String domain to SPOTIDC field for non-network Transformer subtypes..."

     arcpy.AssignDomainToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC', 'Not Applicable - String', [1, 2, 3, 4, 7, 8])

     arcpy.AssignDefaultToField_management(Data_Connection + "\\" + "EDGIS.TRANSFORMER", 'SPOTIDC', 'NA',[1, 2, 3, 4, 7, 8])

     print "'Not Applicable - String' domain assigned to SpotIDC field of Transformer for non-network Transformer subtypes..."

except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "CR-25349: FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "CR-25349: Unexpected error:", sys.exc_info()[0]
#<25349>
