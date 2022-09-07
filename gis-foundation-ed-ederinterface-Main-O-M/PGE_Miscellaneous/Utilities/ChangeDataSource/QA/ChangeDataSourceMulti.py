"""
Developed by : Mohammad Kaleem Anjum(m4a1)
Last Modified by : Mohammad Kaleem Anjum(m4a1)
Last Modified on : 05/27/2021 
Version : 1.00
This scripts is developed to update the data source of the table and layers in the MXD

"""

import arcpy
import os.path
import sys
from arcpy import env
import ConfigParser
import subprocess


config = ConfigParser.ConfigParser()
config.read(r"D:\EDGISPub\ChangeDataSource\QA\DataSource.cfg")

try:
     rootSourcePath  = config.get('General','rootSourcePath')
     rootTargetPath  = config.get('General','rootTargetPath')
     #f = open(mxdDetails, 'rt')
     #reader = csv.reader(f)
     mxdColls = []
     skippedMXds = []
     successMXds = []
     failedMXDs = []
     docDefrag = r"D:\Program Files (x86)\ArcGIS\Desktop10.8\Tools\DocDefragmenter.exe"
     for mxd in config.options("MXDs"):
          mxdColls.append(config.get("MXDs" , mxd))
     loopCount = 0
     print "starting...."
     for mxd in mxdColls:
          
          sourceMxdPth,targetMxdPath,runFlag = mxd.split(",")
          sourceMxdPth = rootSourcePath+sourceMxdPth
          targetMxdPth = rootTargetPath+targetMxdPath
          targetMxdSaveAsPth =r"D:\EDGISPub\ChangeDataSource\Dev" + targetMxdPath
          if runFlag == "false" :
               skippedMXds.append(sourceMxdPth)
               #print "runflag set as " + runFlag + ".skipped mxd:" + sourceMxdPth 
               continue
          #print "sourceMxdPth :"+ sourceMxdPth
          #print "targetMxdPth :"+ targetMxdPth

          mxd = arcpy.mapping.MapDocument(sourceMxdPth)
          sourceConns = []
          sourcePaths = []
          
          
          for lyr in arcpy.mapping.ListLayers(mxd):
             if lyr.supports("DATASOURCE"):
                datasource = lyr.dataSource
                
                #print sourcePaths
                i = datasource.index('.sde')
                datasource = datasource[0:i+4]
                if sourcePaths.count(datasource) < 1 :
                    sourcePaths.append(datasource)

          print sourcePaths
          print sourceMxdPth
          connections = []
          # get the list of tables and associated data source
          for lyr in arcpy.mapping.ListTableViews(mxd):
             datasource = lyr.dataSource
             
             i = datasource.index('.sde')
             datasource = datasource[0:i+4]
             if sourcePaths.count(datasource) < 1 :
                  sourcePaths.append(datasource)
                  #sourceLyrPaths.append(datasource)
          
          for con in config.options("DATASOURCEs"):
               connections.append(config.get("DATASOURCEs" , con))

          totalpathfoundinConfig = 0               
          for srcsdefilepath in sourcePaths :
               currentpathfoundinConfig=0
               i = srcsdefilepath.index('.sde')
               onlysdefilename = srcsdefilepath[0:i+4]
               l = onlysdefilename.rindex("\\")
               
               onlysdefilename = onlysdefilename[l+1:len(onlysdefilename)]
               print "looking for sde file config: "+onlysdefilename
               for con in connections:
                    connstrng1,connstrng2 = con.split(",")
                    if connstrng1.upper() == onlysdefilename.upper() :
                         print "found in config: " +connstrng1
                         print "oldconnection :"+ srcsdefilepath
                         print "newconnection :"+ connstrng2
                         totalpathfoundinConfig =totalpathfoundinConfig+1
                         currentpathfoundinConfig =currentpathfoundinConfig+1
                         mxd.findAndReplaceWorkspacePaths(srcsdefilepath, connstrng2 , False )
                         print "Datasource changed "
                         break;
                    #else :
                    #     print ""
               if currentpathfoundinConfig == 0 : break
          
          if totalpathfoundinConfig ==len(sourcePaths) :
               mxd.saveACopy(targetMxdPth)
               successMXds.append(targetMxdPth)
               argList = []
               argList.append(docDefrag)
               argList.append(targetMxdPth)
               argList.append(targetMxdSaveAsPth)
               #subprocess.call(argList)
               #subprocess.call(['C:\Program Files (x86)\ArcGIS\Desktop10.2\Tools\DocDefragmenter.exe', r'sourcefilepath', r'targetfilepath'])
               print "SaveAsCopy complete at : "+targetMxdPth
          else :
               print "configuration is missing for the mxd " +sourceMxdPth
               failedMXDs.append(sourceMxdPth)
          #loopCount = loopCount +1
          #if loopCount >5 :
               #exit(0)
     print "Change MXD Datasource Summary Status"
     print "Success: " + str(len(successMXds)) + "||Skipped " + str(len(skippedMXds)) +"||Failed : " + str(len(failedMXDs))
     print "---------------Failed MXD list :------------"
     print failedMXDs
     #print "---------------skippedMXds MXD list :------------"
     #print skippedMXds


except Exception, e:

    print e.message
    exit(1)
