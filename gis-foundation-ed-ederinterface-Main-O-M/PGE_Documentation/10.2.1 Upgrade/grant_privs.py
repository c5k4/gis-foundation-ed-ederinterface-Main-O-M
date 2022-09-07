# ---------------------------------------------------------------------------
# grant_privs.py version 3
# Created by Robert Rader, updated: 6/9/2015 
# ---------------------------------------------------------------------------

# Import system modules
import sys, string, os, arcgisscripting
import arcpy
import time
from arcpy import env
# Local variables...
ORACLE_hiall_ROB_sde = "C:\EDER\data\connections\LBGISQ1Q_EDGIS.sde"
dataOwnerPrefixToFilter='EDGIS';
FilterPrefixList=['EDGIS.AAA_ANNO'];
userListEditorsFDs = ['SDE_EDITOR','DAT_EDITOR',];
userListViewersFDs=['SDE_VIEWER','GISINTERFACE','BO_USER'];

userListEditorsRootTables = ['SDE_EDITOR','DAT_EDITOR'];
userListViewersRootTables = ['SDE_VIEWER','BO_USER','GISINTERFACE'];

userListEditorsRootPGETables = ['GISINTERFACE'];
userListViewersRootPGETables = ['SDE_VIEWER','BO_USER'];

GISI_WRITABLE_TABLE_LIST=[ORACLE_hiall_ROB_sde+'\\EDGIS.ServicePoint',ORACLE_hiall_ROB_sde+'\\EDGIS.Transformer',ORACLE_hiall_ROB_sde+'\\EDGIS.ServiceLocation'];
GISI_WRITEABLE_USERS=['GISINTERFACE'];

start = time.clock()
try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	arcpy.workspace =  ORACLE_hiall_ROB_sde
	arcpy.env.workspace =  ORACLE_hiall_ROB_sde
	print arcpy.env.workspace
	print arcpy.workspace
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "UnSuccessfully completed connecting to workspace"
	print arcpy.GetMessages()
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]

print "**************** Root Table list building************"
tables = [c.name for c in arcpy.Describe(ORACLE_hiall_ROB_sde).children if c.datatype == "Table" and not c.name.startswith(dataOwnerPrefixToFilter + ".ZZ_MV") and not c.name.startswith(dataOwnerPrefixToFilter + ".N_") and c.name.startswith(dataOwnerPrefixToFilter + ".")]
tables.sort();
# tables = list(set(arcpy.ListTables(dataOwnerPrefixToFilter+".*","")) - set(arcpy.ListTables(dataOwnerPrefixToFilter+".PGE_*","")))
if tables is None:
	print "Tables list was returned empty, check connection"
else:
	print "Granting privileges to the following tables"
	for table in tables:
		print table
	for users in userListEditorsRootTables:
		try:
			print "Granting View and Edit to "+ users
			arcpy.ChangePrivileges_management(tables,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()
	for users in userListViewersRootTables:
		try:
			print "Revoking edit privileges for user: " + users
			arcpy.ChangePrivileges_management(tables,users,"REVOKE","REVOKE");
			print "Granting View to "+ users
			arcpy.ChangePrivileges_management(tables,users,"GRANT","AS_IS");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()

print "**************** DATASET list building************"
datasets = arcpy.ListDatasets(dataOwnerPrefixToFilter+".*", "Feature")
datasets.sort();
if datasets is None:
	print "DataSets list was returned empty, check connection"
else:
	for dataset in datasets:
		print dataset
		for users in userListEditorsFDs:
			try:
				print "Granting View and Edit on "+dataset+ " to "+ users
				arcpy.ChangePrivileges_management(dataset,users,"GRANT","GRANT");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + dataset
				print arcpy.GetMessages()
		for users in userListViewersFDs:
			try:
				print "Revoking edit privileges on: " + dataset + " for user: " + users
				arcpy.ChangePrivileges_management(dataset,users,"REVOKE","REVOKE");
				print "Granting View on: " + dataset + " for "+ users
				arcpy.ChangePrivileges_management(dataset,users,"GRANT","AS_IS");
				print "Permissions have been changed"
			except:
				print "Unable to grant access to: " + dataset
				print arcpy.GetMessages()

print "**************** ROOT Feature Class list building************"
featureclasses = arcpy.ListFeatureClasses(dataOwnerPrefixToFilter+".*","")
featureclasses.sort();
if featureclasses is None:
	print "Feature Classes list was returned empty, check connection"
else:
	print "Granting privileges to the following feature classes"
	for table in featureclasses:
		print table
	for users in userListEditorsRootTables:
		try:
			print "Granting View and Edit to "+ users
			arcpy.ChangePrivileges_management(featureclasses,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()
	for users in userListViewersRootTables:
		try:
			print "Revoking edit privileges for user: " + users
			arcpy.ChangePrivileges_management(featureclasses,users,"REVOKE","REVOKE");
			print "Granting View to "+ users
			arcpy.ChangePrivileges_management(featureclasses,users,"GRANT","AS_IS");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()

print "**************** ROOT PGE ONLY Tables list building************"
toDelete = [];
PGEtables = arcpy.ListTables(dataOwnerPrefixToFilter+".PGE_*","")
for table in PGEtables:
	if table.endswith('EDGIS.PGE_UNIFIEDGRIDMAP_20150303'):
		toDelete.append(table)
	if table.endswith('EDGIS.PGE_UNIFIEDGRIDMAP_20150314'):
		toDelete.append(table)
	if table.endswith('EDGIS.PGE_UNIFIEDGRIDMAP_BACKUP'):
		toDelete.append(table)
for deleteObject in toDelete:
	if deleteObject in PGEtables:
		PGEtables.remove(deleteObject)
PGEtables.sort();
if PGEtables is None:
	print "PGE Tables list was returned empty, check connection"
else:
	print "Granting privileges to the following tables"
	for table in PGEtables:
		print table
	for users in userListEditorsRootPGETables:
		try:
			print "Granting View and Edit to "+ users
			arcpy.ChangePrivileges_management(PGEtables,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()
	for users in userListViewersRootPGETables:
		try:
			print "Revoking edit privileges for user: " + users
			arcpy.ChangePrivileges_management(PGEtables,users,"REVOKE","REVOKE");
			print "Granting View to "+ users
			arcpy.ChangePrivileges_management(PGEtables,users,"GRANT","AS_IS");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()

print "**************** GISI_WRITABLE_TABLE_LIST list building************"
PGEtables = GISI_WRITABLE_TABLE_LIST 
PGEtables.sort();
if PGEtables is None:
	print "GISI_WRITABLE_TABLE_LIST was returned empty, check list values"
else:
	print "Granting privileges to the following tables"
	for table in PGEtables:
		print table
	for users in GISI_WRITEABLE_USERS:
		try:
			print "Granting View and Edit to "+ users
			arcpy.ChangePrivileges_management(PGEtables,users,"GRANT","GRANT");
			print "Permissions have been changed"
		except:
			print "Unable to grant access for user: " + users
			print arcpy.GetMessages()

elapsed = (time.clock() - start)
print "Elapsed time: ", elapsed
print ""
print ""
print "**************************************"
print "Script Elapsed time: ", elapsed
print "*** Finished Granting privileges Database ***"
print "**************************************"
print ""
print ""
