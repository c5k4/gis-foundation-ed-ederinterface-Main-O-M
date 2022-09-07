import arcpy
import os.path
import sys
from arcpy import env
import ConfigParser


config = ConfigParser.ConfigParser()
config.read(r"D:\EDGISPub\RegisterDataSource\DataSource_landbase.cfg")
#config.read(r"D:\EDGISPub\RegisterDataSource\DataSource.cfg")
try:
     rootSDEPath  = config.get('General','rootSDEPath')
     rootAGSPath  = config.get('General','rootAGSPath')
     #agsPath = "\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Test\EDGIS\AGSFiles\AGS_WEBR_TSEDGMAPPWX001_6080.ags"
     #datastore_name ="edschmpub"
     #sde_path ="D:\EDGISPub\SDEFiles\edschmpub_webr.sde"
     datasourceColls = []
     skippedDSs = []
     successDSs = []
     failedDSs = []
     for datasource in config.options("Datasource"):
          datasourceColls.append(config.get("Datasource" , datasource))
     for ds in datasourceColls :
          datastore_name,agsFilePth, sdeFilePath,datastore_type,skipFlag = ds.split(",")
          folderPath = sdeFilePath
          agsFilePth = rootAGSPath + agsFilePth
          sdeFilePath = rootSDEPath + sdeFilePath
          if skipFlag == "true" :
               try:
                    if datastore_type == "DATABASE"  :
                         arcpy.AddDataStoreItem(connection_file=agsFilePth,datastore_type=datastore_type,connection_name=datastore_name,server_path=sdeFilePath)
                         successDSs.append(datastore_name)
                         print "Registered "+datastore_name+" successfully"
                    else :
                         #arcpy.AddDataStoreItem(agsFilePth, "FOLDER", "Localdatafolder", sdeFilePath)
                         arcpy.AddDataStoreItem(connection_file=agsFilePth,datastore_type=datastore_type,connection_name=datastore_name,server_path=folderPath)
                         successDSs.append(datastore_name)
                         print "Registered "+datastore_name+" successfully"
               except Exception,e:
                    print datastore_name + str(e)
                    failedDSs.append(datastore_name+" exception:" + str(e))
          else :
               print "skipped:" +datastore_name
               skippedDSs.append(datastore_name)
     print "Register Datasource summary status"
     print "Success: " + str(len(successDSs)) + "||Skipped " + str(len(skippedDSs)) +"||Failed : " + str(len(failedDSs))
     print "---------------Failed  list :------------"
     print failedDSs
     print "---------------skippedMXds list :------------"
     print skippedDSs
except Exception, e:

    print e
    exit(1)
