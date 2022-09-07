import sys
import time
# Import arcpy module
import arcpy, math
from arcpy import env
import os

def main():
	scriptStart = time.clock()
	print "**************************************"
	print "*** Creating spatial indices ***"
	print "**************************************"
	print ""
	print ""
	# Local variables:
	DataConnection = sys.argv[1]
	env.workspace = DataConnection
	datasets = arcpy.ListDatasets("EDGIS.*", "Feature")
	if datasets is None:
	   print "datasets was returned empty, check workspace"
	else:
	 for DataSource in datasets:
		print "*********** Points : Minimum 200, Maximum: 500 ************"	 
		fcList = list((set(arcpy.ListFeatureClasses("*","Point",DataSource)) | set(arcpy.ListFeatureClasses("*","Junction",DataSource)))- set(arcpy.ListFeatureClasses("EDGIS.Z*","Point",DataSource)))
		if fcList is None:
		  print "fcList was returned empty, check list workspace"
		else:
		  for fc in fcList:
			start = time.clock()
			try:
				try:
					print fc + ":"
					print "Calculating default grid indices"
					result = arcpy.CalculateDefaultGridIndex_management(fc)
					print "Adjusting Calculated values"
					grid1 = long(float(result.getOutput(0)))
					grid2 = 0
					grid3 = 0
					if grid1 < 200 :
					   print "Grid1 value was: {0} so setting to 200".format(str(grid1))
					   grid1 = 200
					if grid1 > 500 :
					   print "Grid1 value was: {0} so setting to 500".format(str(grid1))
					   grid1 = 500				   
				except :
					print "Error calculating grid index, using defaults of 500,0,0"
					grid1 = 500
					grid2 = 0
					grid3 = 0
				try:
					print "Deleting current spatial index"
					arcpy.RemoveSpatialIndex_management(fc)
				except:
					print "No spatial index to remove"
				try:
					print "Creating spatial index: " + fc + " Grid1:" + str(grid1)
					arcpy.AddSpatialIndex_management(fc, grid1, grid2, grid3)
				except Exception as EXinfo:
					print "Failed to create spatial index"
					print arcpy.GetMessages()
					print "FAILURE: Error was: "
					print type(EXinfo)
					print EXinfo.args
					print EXinfo
					print "Unexpected error:", sys.exc_info()[0]					
			except Exception as EXinfo:
				print "@@@@@@@@ERROR@@@@@@@@@@@@@@@@ Unhandled Exception encountered!!!!!!"
				print arcpy.GetMessages()
				print "FAILURE: Error was: "
				print type(EXinfo)
				print EXinfo.args
				print EXinfo
				print "Unexpected error:", sys.exc_info()[0]
				elapsed = (time.clock() - start)
				print "Elapsed time: ", elapsed
			print ""
			print ""
		print "*********** Lines : Minimum 600, Maximum: 1200 ************"	 
		fcList = list((set(arcpy.ListFeatureClasses("*","Line",DataSource)) | set(arcpy.ListFeatureClasses("*","Edge",DataSource)))- set(arcpy.ListFeatureClasses("EDGIS.Z*","Line",DataSource)))
		if fcList is None:
		  print "fcList was returned empty, check list workspace"
		else:
		  for fc in fcList:
			start = time.clock()
			try:
				try:
					print fc + ":"
					print "Calculating default grid indices"
					result = arcpy.CalculateDefaultGridIndex_management(fc)
					print "Adjusting Calculated values"
					print "Calculation returned: "+str(long(float(result.getOutput(0))))+" , "+str(long(float(result.getOutput(1))))+" , "+str(long(float(result.getOutput(2))))
					grid1 = long(float(result.getOutput(0)))
					grid2 = 0
					grid3 = 0
					if grid1 < 600 :
					   print "Grid1 value was: {0} so setting to 600".format(str(grid1))
					   grid1 = 600
					   if long(float(result.getOutput(1))) > 32000 :
					      grid2 = long(float(result.getOutput(1)))
					      print "Grid2 value was >32000 so setting to {0}".format(str(grid2))
					if grid1 > 1200 :
					   print "Grid1 value was: {0} so setting to 1200".format(str(grid1))
					   grid1 = 1200				   
				except :
					print "Error calculating grid index, using defaults of 1200,0,0"
					grid1 = 1200
					grid2 = 0
					grid3 = 0
				try:
					print "Deleting current spatial index"
					arcpy.RemoveSpatialIndex_management(fc)
				except:
					print "No spatial index to remove"
				try:
					print "Creating spatial index: " + fc + " Grid1:" + str(grid1) + ","+str(grid2)+","+str(grid3)
					arcpy.AddSpatialIndex_management(fc, grid1, grid2, grid3)
				except Exception as EXinfo:
					print "Failed to create spatial index"
					print arcpy.GetMessages()
					print "FAILURE: Error was: "
					print type(EXinfo)
					print EXinfo.args
					print EXinfo
					print "Unexpected error:", sys.exc_info()[0]					
			except Exception as EXinfo:
				print "@@@@@@@@ERROR@@@@@@@@@@@@@@@@ Unhandled Exception encountered!!!!!!"
				print arcpy.GetMessages()
				print "FAILURE: Error was: "
				print type(EXinfo)
				print EXinfo.args
				print EXinfo
				print "Unexpected error:", sys.exc_info()[0]
				elapsed = (time.clock() - start)
				print "Elapsed time: ", elapsed
			print ""
			print ""
		print "*********** Annotation : Minimum 200, Maximum: 1200 ************"	 
		fcList = list(set(arcpy.ListFeatureClasses("*","Annotation",DataSource))- set(arcpy.ListFeatureClasses("EDGIS.Z*","Annotation",DataSource)))
		if fcList is None:
		  print "fcList was returned empty, check list workspace"
		else:
		  for fc in fcList:
			start = time.clock()
			try:
				try:
					print fc + ":"
					print "Calculating default grid indices"
					result = arcpy.CalculateDefaultGridIndex_management(fc)
					print "Adjusting Calculated values"
					grid1 = long(float(result.getOutput(0)))
					grid2 = 0
					grid3 = 0
					if grid1 < 200 :
					   print "Grid1 value was: {0} so setting to 200".format(str(grid1))
					   grid1 = 200
					if grid1 > 1200 :
					   print "Grid1 value was: {0} so setting to 1200".format(str(grid1))
					   grid1 = 1200				   
				except :
					print "Error calculating grid index, using defaults of 1200,0,0"
					grid1 = 1200
					grid2 = 0
					grid3 = 0
				try:
					print "Deleting current spatial index"
					arcpy.RemoveSpatialIndex_management(fc)
				except:
					print "No spatial index to remove"
				try:
					print "Creating spatial index: " + fc + " Grid1:" + str(grid1)
					arcpy.AddSpatialIndex_management(fc, grid1, grid2, grid3)
				except Exception as EXinfo:
					print "Failed to create spatial index"
					print arcpy.GetMessages()
					print "FAILURE: Error was: "
					print type(EXinfo)
					print EXinfo.args
					print EXinfo
					print "Unexpected error:", sys.exc_info()[0]					
			except Exception as EXinfo:
				print "@@@@@@@@ERROR@@@@@@@@@@@@@@@@ Unhandled Exception encountered!!!!!!"
				print arcpy.GetMessages()
				print "FAILURE: Error was: "
				print type(EXinfo)
				print EXinfo.args
				print EXinfo
				print "Unexpected error:", sys.exc_info()[0]
				elapsed = (time.clock() - start)
				print "Elapsed time: ", elapsed
			print ""
			print ""			
	scriptElapsed = (time.clock() - scriptStart)
	print ""
	print ""
	print "**************************************"
	print "Script Elapsed time: ", scriptElapsed
	print "*** Finished creating spatial indices ***"
	print "**************************************"
	print ""
	print ""

if __name__ == '__main__': main()
