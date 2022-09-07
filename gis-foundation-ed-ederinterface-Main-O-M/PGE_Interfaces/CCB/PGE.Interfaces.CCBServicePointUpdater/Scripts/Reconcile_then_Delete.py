# ---------------------------------------------------------------------------
# Template .py file for PGE TFS Data model changes
# Created by Robert Rader : Oct 28th 2013 Template version 1.0
# ---------------------------------------------------------------------------
# Import system modules
import arcpy, time, smtplib, sys, string, os, arcgisscripting
# Data_Connection = "Database Connections\\edgis@edgisa1t.sde"
Data_Connection = os.environ.get("LOCSDECONN")

try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Initializing the workspace of: ", Data_Connection
	arcpy.Workspace =  Data_Connection
	arcpy.env.workspace = Data_Connection
except:
	# If an error occurred while running a tool, print the messages
	print "UnSuccessfully completed connecting to workspace, error was: "
	print arcpy.GetMessages()
	print "Unexpected error:", sys.exc_info()[0]

try:
	# Set the workspace (to avoid having to type in the full path to the data every time)
	print "Building list of all versions to reconcile : ", Data_Connection
	versionList = arcpy.ListVersions(Data_Connection)
	dataList = ('GIS_I.CCBWithViews02')
	print "Starting the reconcile of these versions... ", Data_Connection
	# ArcGIS 10.1 arcpy.ReconcileVersions_management(Data_Connection, "BLOCKING_VERSIONS", "sde.DEFAULT", "", "NO_LOCK_ACQUIRED", "NO_ABORT", "BY_OBJECT", "FAVOR_EDIT_VERSION", "NO_POST", "KEEP_VERSION", "c:\\temp\\python_reconcilelog.txt")
	for version in versionList:
		if version in dataList :
			print "Reconciling Version : ",version
			try:
				# all_ver = version.split('.')
				# versionname = all_ver[1]
				arcpy.ReconcileVersion_management(Data_Connection, version, "SDE.DEFAULT", "BY_OBJECT", "FAVOR_EDIT_VERSION", "LOCK_AQUIRED", "NO_ABORT", "POST")
				# gp.ReconcileVersion_management(Data_Connection, versionname, "SDE.DEFAULT", "BY_OBJECT", "FAVOR_EDIT_VERSION", "NO_LOCK_AQUIRED", "NO_ABORT", "NO_POST")
				# arcpy.ReconcileVersion_management(Data_Connection, version, "SDE.DEFAULT", "BY_OBJECT", "FAVOR_TARGET_VERSION", "LOCK_AQUIRED", "NO_ABORT", "POST")
				arcpy.DeleteVersion_management(Data_Connection, version)
			except Exception as EXinfo:
				# If an error occurred while running a tool, print the messages
				print "FAILURE: Error was: "
				print arcpy.GetMessages()
				print type(EXinfo)
				print EXinfo.args
				print EXinfo
				print "Unexpected error:", sys.exc_info()[0]
except Exception as EXinfo:
	# If an error occurred while running a tool, print the messages
	print "FAILURE: Error was: "
	print arcpy.GetMessages()
	print type(EXinfo)
	print EXinfo.args
	print EXinfo
	print "Unexpected error:", sys.exc_info()[0]




