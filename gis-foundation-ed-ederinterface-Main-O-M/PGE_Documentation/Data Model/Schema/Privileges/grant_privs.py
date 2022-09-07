# ---------------------------------------------------------------------------
# grant_privs.py version 2
# Created by Robert Rader, updated: Sat Jan 15 2015 
# ---------------------------------------------------------------------------

# Import system modules
import sys, string, os, arcgisscripting

# Create the Geoprocessor object
gp = arcgisscripting.create(9.3)

# Local variables...
ORACLE_hiall_ROB_sde = "Database Connections\\edgis@a1d.sde"
dataOwnerPrefixToFilter='EDGIS';
FilterPrefixList=['EDGIS.AAA_ANNO'];
userListEditorsFDs = ['SDE_EDITOR','DAT_EDITOR',];
userListViewersFDs=['SDE_VIEWER','GIS_INTERFACE','GISINTERFACE','BO_USER'];

userListEditorsRootFeatureClasses = ['SDE_EDITOR','DAT_EDITOR','GIS_INTERFACE','GISINTERFACE'];
userListViewersRootFeatureClasses = ['SDE_VIEWER','BO_USER'];

userListEditorsRootTables = ['SDE_EDITOR','DAT_EDITOR','DMSSTAGING'];
userListViewersRootTables = ['SDE_VIEWER','GIS_INTERFACE','GISINTERFACE','BO_USER'];

userListEditorsRootPGETables = ['GIS_INTERFACE','GISINTERFACE'];
userListViewersRootPGETables = ['SDE_VIEWER','BO_USER'];

GISI_WRITABLE_TABLE_LIST=[ORACLE_hiall_ROB_sde+'\\EDGIS.ServicePoint',ORACLE_hiall_ROB_sde+'\\EDGIS.Transformer',ORACLE_hiall_ROB_sde+'\\EDGIS.ServiceLocation'];
GISI_WRITEABLE_USERS=['GIS_INTERFACE','GISINTERFACE'];

try:
    # Set the workspace (to avoid having to type in the full path to the data every time)
    gp.Workspace =  ORACLE_hiall_ROB_sde
except:
    # If an error occurred while running a tool, print the messages
    print "UnSuccessfully completed connecting to workspace for the stats"
    print gp.GetMessages()


print "**************** Root Table list building************"
tables = gp.ListTables(dataOwnerPrefixToFilter+".*","")
for table in tables:
	print table
	bypass=0;
	for tablenameFilter in FilterPrefixList:
		if tablenameFilter in table:
			bypass=1;
	if bypass==0 :
		for users in userListEditorsRootTables:
			try:
				print "++Granting View and Edit on "+table+ " to "+ users		
				gp.ChangePrivileges_management(table,users,"GRANT","GRANT");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + table
				print gp.GetMessages()
		for users in userListViewersRootTables:
			try:
				print "*Granting View on "+table+ " to "+ users
				gp.ChangePrivileges_management(table,users,"GRANT","AS_IS");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + table
				print gp.GetMessages()
	else:
		print "##### filtered and not applying privledges on "+	table

print "**************** DATASET list building************"
datasets = gp.listdatasets(dataOwnerPrefixToFilter+".*", "")    
for dataset in datasets:
	print dataset
	for users in userListEditorsFDs:
		try:
			print "++Granting View and Edit on "+dataset+ " to "+ users		
			gp.ChangePrivileges_management(dataset,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access to: " + dataset
			print gp.GetMessages()
	for users in userListViewersFDs:
		try:
			print "*Granting View on "+dataset+ " to "+ users
			gp.ChangePrivileges_management(dataset,users,"GRANT","AS_IS");
			print "Permissions have been changed"
		except:
			print "Unable to grant access to: " + dataset
			print gp.GetMessages()

print "**************** ROOT Feature Class list building************"
featureclasses = gp.ListFeatureClasses(dataOwnerPrefixToFilter+".*","")
for featureclass in featureclasses:
	print featureclass
	for users in userListEditorsRootFeatureClasses:
		try:
			print "++Granting View and Edit on "+featureclass+ " to "+ users		
			gp.ChangePrivileges_management(featureclass,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access to: " + featureclass
			print gp.GetMessages()
	for users in userListViewersRootFeatureClasses:
		try:
			print "*Granting View on "+featureclass+ " to "+ users
			gp.ChangePrivileges_management(featureclass,users,"GRANT","AS_IS");
			print "Permissions have been changed"
		except:
			print "Unable to grant access to: " + featureclass
			print gp.GetMessages()

print "**************** ROOT PGE ONLY Tables list building************"
PGEtables = gp.ListTables(dataOwnerPrefixToFilter+".PGE_*","")
for table in PGEtables:
	print table
	for tablenameFilter in FilterPrefixList:
		if tablenameFilter in table:
			bypass=1;
	if bypass==0 :
		for users in userListEditorsRootPGETables:
			try:
				print "++Granting View and Edit on "+table+ " to "+ users		
				gp.ChangePrivileges_management(table,users,"GRANT","GRANT");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + table
				print gp.GetMessages()
		for users in userListViewersRootPGETables:
			try:
				print "*Granting View on "+table+ " to "+ users
				gp.ChangePrivileges_management(table,users,"GRANT","AS_IS");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + table
				print gp.GetMessages()
	else:
		print "##### filtered and not applying privledges on "+	table

print "**************** GISI_WRITABLE_TABLE_LIST list building************"
PGEtables = GISI_WRITABLE_TABLE_LIST 
for table in PGEtables:
	print table
	for tablenameFilter in FilterPrefixList:
		if tablenameFilter in table:
			bypass=1;
	if bypass==0 :	
		for users in GISI_WRITEABLE_USERS:
			try:
				print "++Granting View and Edit on "+table+ " to "+ users		
				gp.ChangePrivileges_management(table,users,"GRANT","GRANT");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + table
				print gp.GetMessages()
	else:
		print "##### filtered and not applying privledges on "+	table
