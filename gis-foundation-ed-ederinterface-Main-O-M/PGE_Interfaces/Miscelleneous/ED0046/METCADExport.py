import smtplib, os
import logging
import ConfigParser
from email.mime.text import MIMEText
from email.Utils import COMMASPACE, formatdate
from email import Encoders
import time
import shutil
import uuid
import sys
import math
import platform
import zipfile
import multiprocessing
import urllib
import ConfigParser
import unittest
import json

# multiprocessing workaround so script will run in GP framework (ArcCatalog toolbox and AGS service)
try:
	import arcpy
except Exception as e:
	sys.exit(0)

logger = logging.getLogger('extract_poc')
config = ConfigParser.SafeConfigParser()
section = "General Settings"	
univUniqID = str(uuid.uuid1())
scratchDir = os.path.join(arcpy.env.scratchFolder, univUniqID)
if not os.path.exists(scratchDir): os.makedirs(scratchDir)
tempWS = os.path.join(scratchDir, "scratch" + str(os.getpid()) + ".gdb")
print(tempWS)
#tempWS = scratchDir + "\\" + "scratch" + univUniqID + ".gdb"
##outputLog = os.path.join(scratchDir,str(os.getpid()) + ".log")
outputLog = os.path.join(scratchDir,univUniqID + "_" + str(os.getpid()) + ".log")
#logging.basicConfig(filename=outputLog, format='%(asctime)s %(levelname)s\t%(message)s', level=logging.DEBUG)

if __name__ != '__main__':
	createGDB()
	#TODO do we need to copy contents back upon publish or run thru Catalog?
	
def createGDB():
	folderScratch = "scratch"
	try:
		arcpy.CreateFileGDB_management(scratchDir,(folderScratch + str(os.getpid())), "CURRENT")
	except Exception as e:
		msg = "ERROR:  CANNOT CREATE FileGDB - " + e.message
		arcpy.SetParameterAsText(4, msg)
		sys.exit(0)

def getConfigOption(config, section, option):
	
	try:
		configValue = config.get(section, option)
	except Exception as e:
		raise e
	
	return configValue

def initialize():

	setupLogging()
	config.optionxform = str
	appPath = os.path.dirname(__file__)
	config.read(os.path.join(appPath,"settingsMETCADExport.cfg"))	

		
def setupLogging():
	# create logger with 'spam_application'
	logger.setLevel(logging.DEBUG)
	# create file handler which logs even debug messages
	fh = logging.FileHandler(outputLog)
	fh.setLevel(logging.DEBUG)
	# create console handler with a higher log level
	ch = logging.StreamHandler()
	ch.setLevel(logging.DEBUG)
	# create formatter and add it to the handlers
	formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
	fh.setFormatter(formatter)
	ch.setFormatter(formatter)
	# add the handlers to the logger
	logger.addHandler(fh)
	logger.addHandler(ch)

	logger.debug('setup debug')

		
def runExport(params):
	logger.debug('Running Export...')

	start = time.time()

	currentWS = getConfigOption(config, section, "currentWS")
	logger.debug(currentWS)
	arcpy.env.workspace = currentWS
	
	exportAnnoMBRs(params)

	end = time.time()
	elapsedTime = str(end - start)
	logger.debug("Elapsed Time [ " + elapsedTime + " ]");
	arcpy.AddMessage("Elapsed Time [ " + elapsedTime + " ]");

	return "exported"

def exportAnnoMBRs(params):
	logger.debug('Exporting MBRs...')

	datasetList = arcpy.ListDatasets("")
	annoFD = getConfigOption(config,section, "annoFD")

	featureclasses = arcpy.ListFeatureClasses("*","ANNOTATION",annoFD)
	
	logger.debug('Reprojecting Extent...')
	spatialReference = arcpy.Describe(featureclasses[0]).spatialReference
	projectedExtent = params.extent.projectAs(spatialReference)
	arcpy.env.extent = projectedExtent.extent
	
	for fc in featureclasses:
		#TODO: read from cfg
		if "100" in fc or "500" in fc: continue
		print(fc)
		mbrFc = fc[fc.find(".")+1:]
		logger.debug("Exporting Minimum Bounding Rectangle to GDB [ " + mbrFc + " ]")
		output = os.path.join(tempWS,mbrFc)
		try:
			arcpy.MinimumBoundingGeometry_management(fc, output, 'RECTANGLE_BY_AREA', 'LIST', 'FeatureID;AnnotationClassID', 'MBG_FIELDS')
		except Exception as e:
			logger.debug(e.message)

	
def arcgis_parameter_bootstrap():
	logger.debug('Bootstrapping parameters...')

	# Check for required number of arguments
	if arcpy.GetArgumentCount() < 4:
		print("Usage: extent cadFileName emailAddresses scale")
		sys.exit(1)

	params = ToolParameters()
	
	extentJSON = arcpy.GetParameterAsText(0)
	params.load_extent(extentJSON)

	cadFileName = arcpy.GetParameterAsText(1)
	params.load_cadFileName(cadFileName)
	emailAddresses = arcpy.GetParameterAsText(2)
	params.load_emailAddresses(emailAddresses)
	scale = arcpy.GetParameterAsText(3)
	params.scale = scale
	
	arcpy.AddMessage("EXTENT_JSON: " + extentJSON)
	arcpy.AddMessage("CAD: " + cadFileName)
	arcpy.AddMessage("EMAIL: " + emailAddresses)
	arcpy.AddMessage("SCALE: " + str(scale))

	return params

class ToolParameters(object):
		'''value object for storing export tool parameters'''

		def __init__(self):
			self.extent = None
			self.emailAddresses = None
			self.cadFileName = None
			self.scale = None

		def load_extent(self, extentJSON):
			logger.debug("About to load JSON into geometry for [ " + extentJSON + " ]")
#			geom = json.loads(extentJSON)
			geom = json.loads('{"rings":[[[2812758,12845426],[2812758,12845734],[2813337,12845734],[2813337, 12845426]]]}')
			self.extent = arcpy.AsShape(geom, True)

		def load_emailAddresses(self, rawInput):
			self.emailAddresses = rawInput

		def load_cadFileName(self, cadFileName):
			logger.debug("Ensuring CAD file for [ " + cadFileName + " ]")
			if not "DWG" in cadFileName.upper():
				cadFileName = cadFileName + ".dwg"
			self.cadFileName = cadFileName
		

if __name__ == '__main__':
	initialize()
	logger.debug('Starting...')
	createGDB()
	
	if arcpy.GetParameterAsText(0):
			params = arcgis_parameter_bootstrap()
			params.result_file = runExport(params)
			#arcpy.SetParameterAsText(4, virtual_result_file)
	else:
			runExport(None)