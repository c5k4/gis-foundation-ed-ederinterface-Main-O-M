import arcpy
Data_Connection='Database Connections\\PGE1_EDGIS.sde'

#<20106>
arcpy.AddField_management (Data_Connection +"\\"+"NETWORKPROTECTOR", 'LOCALOFFICEID', 'TEXT', '', '', '4', 'Local Office ID', 'NULLABLE', '', 'Local Offices')
print "Added localofficeid to networkprotector"
#</20106>

#<21258>
arcpy.AddField_management (Data_Connection +"\\"+"CIRCUITSOURCE", 'CIRCUITABBREVNAME', 'TEXT', '', '', '50', 'Circuit Abbreviated Name', 'NULLABLE', '', '')
print "Added circuitabbrevname to circuitsource"
#</21258>

#<21962>
arcpy.AddCodedValueToDomain_management(Data_Connection, "Joint Pole Members", "RV", "City of Roseville")
print "added code to domain"
arcpy.AddCodedValueToDomain_management(Data_Connection, "Joint Pole Members", "TU", "Tuolumne Telephone")
print "added code to domain"
#</21962>