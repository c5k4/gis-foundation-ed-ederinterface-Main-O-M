import arcpy
import os.path
import sys
from arcpy import env
import ConfigParser


config = ConfigParser.ConfigParser()
config.read(r"D:\EDGISPub\ChangeDataSource\QA\DataSource.cfg")

try:
     rootSourcePath  = config.get('General','rootSourcePath')
    
     #f = open(mxdDetails, 'rt')
     #reader = csv.reader(f)
     mxdColls = []
     skippedMXds = []
     successMXds = []
     failedMXDs = []
     for mxd in config.options("MXDs"):
          mxdColls.append(config.get("MXDs" , mxd))
     loopCount = 0
     print "starting...."
     
     onlysdefilenames = []
     for mxd in mxdColls:
          
          sourceMxdPth,targetMxdPth,runFlag = mxd.split(",")
          
         
          
          if runFlag == "false" :
               skippedMXds.append(sourceMxdPth)
               #print "runflag set as " + runFlag + ".skipped mxd:" + sourceMxdPth 
               continue

          print sourceMxdPth
          sourceMxdPth = rootSourcePath+sourceMxdPth
          
          mxd = arcpy.mapping.MapDocument(sourceMxdPth)
          sourceConns = []
          sourcePaths = []
          sourceLyrPaths = []
          #onlysdefilenames = []
          
          
          for lyr in arcpy.mapping.ListLayers(mxd):
             if lyr.supports("DATASOURCE"):
                datasource = lyr.dataSource
                #sourcePaths.append(datasource)
                #print sourcePaths
                #print datasource
                i = datasource.index('.sde')
                if i <0 :
                    print "error in layer sde substring " + datasource
                    continue 
                datasource = datasource[0:i+4]
                if sourcePaths.count(datasource) < 1 :
                    sourcePaths.append(datasource)
                    sourceLyrPaths.append(datasource+":"+lyr.name)

          for lyr in arcpy.mapping.ListTableViews(mxd):
             datasource = lyr.dataSource
             #print datasource
             #sourcePaths.append(datasource) #print sourcePaths i =
             i = datasource.index('.sde')
             if i <0 :
                    print "error in table sde substring " + datasource
                    continue 
             datasource = datasource[0:i+4]
             if sourcePaths.count(datasource) < 1 :
                  sourcePaths.append(datasource)
                  sourceLyrPaths.append(datasource+":"+lyr.name)
          
          print " Full SDE File path"
          print sourceLyrPaths        

                
          for srcsdefilepath in sourcePaths :
               pathfoundinConfig = 0
               i = srcsdefilepath.index('.sde')
               onlysdefilename = srcsdefilepath[0:i+4]
               l = onlysdefilename.rindex("\\")
               if l <0 :
                    print "error in substring " + onlysdefilename
                    continue
               onlysdefilename = onlysdefilename[l+1:len(onlysdefilename)]
               if onlysdefilenames.count(onlysdefilename) < 1 :
                    onlysdefilenames.append(onlysdefilename)
                    print onlysdefilename
          
         
          #loopCount = loopCount +1
          #if loopCount >5 :
               #exit(0)
     
     print " Only SDE File Name"
     i = 0
     for sdefile in onlysdefilenames :
          i = i+1
          print 'oldConnection'+str(i)+',newConnection'+str(i)+':'+sdefile+',D:\\DatabaseConnectionFiles\\Int\\' + sdefile
except Exception, e:

    print e.message
    exit(1)
