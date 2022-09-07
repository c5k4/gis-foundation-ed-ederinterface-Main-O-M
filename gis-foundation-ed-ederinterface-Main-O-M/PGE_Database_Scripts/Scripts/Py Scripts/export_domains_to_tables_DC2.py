# -*- coding: cp1252 -*-
# ---------------------------------------------------------------------------
# export_domains_to_tables.py 
# Created by Robert Rader, updated: Thursday April 16th 2015 
# ---------------------------------------------------------------------------
# create table PGE_CODES_AND_DESCRIPTIONS( DOMAIN_NAME NVARCHAR2(200),CODE NVARCHAR2(200),DESCRIPTION NVARCHAR2(200))
# Import system modules
import sys, string, os, arcpy, numbers, re

# Local variables...
Data_Connection = "C:\\temp\\domain_to_table\\edgis@a1d.sde"


# Local variables:
Output_Table = "PGE_CODES_AND_DESCRIPTIONS"
bypass=0
try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Initializing the workspace of: ", Data_Connection
	arcpy.Workspace =  Data_Connection
	sde_conn = arcpy.ArcSDESQLExecute(Data_Connection);
except:
	# If an error occurred while running a tool, print the messages
	bypass=1
	print "UnSuccessfully completed connecting to workspace, error was: "
	print arcpy.GetMessages()
	print "Unexpected error:", sys.exc_info()[0]

if bypass==0:
   try:
     print "Initializing Domain List"
     domains = arcpy.da.ListDomains(Data_Connection)
     print "Initialized Domain List, getting first one"	 
     for domain in domains:
       print('Domain name: {0}'.format(domain.name))
       if domain.domainType == 'CodedValue':
         coded_values = domain.codedValues
         for val, desc in coded_values.iteritems():
           try:
             #desc_safe = desc.translate({ord(char): "~" for char in "',–"})
             desc_safe = re.sub('[^a-zA-Z0-9\- "_(){}\\/\n\.]', "~",desc)
             print('{0} : {1}'.format(val, desc_safe))
             sql = "Insert into {0} (DOMAIN_NAME,DESCRIPTION,CODE) values ('{1}','{2}','{3}')".format(Output_Table,domain.name,desc_safe,val)
             print sql
             sql_return = sde_conn.execute(sql)
             print "executed command"
             print "SUCCESS: Inserted values into table"				  
           except Exception as EXinfo2:
             print "ERROR: ***** unable to insert values *******"
             print arcpy.GetMessages()
             print type(EXinfo2)
             print EXinfo2.args
             print EXinfo2             
             print "Unexpected error:", sys.exc_info()[0]
     print "Domain Completed"
     #sde_conn.execute("commit")
   except Exception as EXinfo:
     print "Error encountered, unexpected so stopping all further work"
     print arcpy.GetMessages()
     print type(EXinfo)
     print EXinfo.args
     print EXinfo	 
     print "Unexpected error:", sys.exc_info()[0]	 
	 
