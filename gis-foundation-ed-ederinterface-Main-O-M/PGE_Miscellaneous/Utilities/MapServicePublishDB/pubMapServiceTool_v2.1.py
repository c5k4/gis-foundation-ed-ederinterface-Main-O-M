# Publishes a service to machine myserver using USA.mxd
# A connection to ArcGIS Server must be established in the
#  Catalog window of ArcMap before running this script
from configparser import SafeConfigParser
from time import sleep
import arcpy
import xml.dom.minidom as DOM
import codecs
import ast,os,sys
import csv
import myLogger
import traceback
import cx_Oracle
import socket
from database import DatabaseConnection
from publishMapService import PublishMap


#Overwrite the output
arcpy.env.overwriteOutput = True


config=SafeConfigParser()
dirname, filename = os.path.split(os.path.abspath(__file__))
configFileName = dirname + "\\" + 'pubMapServiceTool.ini'
print configFileName
config.read(configFileName)
# Define local variables
#wrkspc  = config.get('General','workspace')
#arcGISConnectionPath  = config.get('General','ArcGISConnection')
#mxdRootFolder  = config.get('General','mxdRootFolder')
#mapServiceInputCSV  = config.get('General','MapServiceInput')
logFolder  = config.get('General','logFolder')
#createSDFileOnly  = config.get('General','createSDFileOnly')
#setup logger
logger = myLogger.Logger(logFolder, __file__,"service",True)
logger.log("Tool started...")
try:
    
    database_connect = None
    database_connection_name = config.get('database', 'database_connection_name')
    username = config.get('database', 'username')
    password = config.get('database', 'password') 
    
    #global database_connection_object
    database_connection_object = DatabaseConnection(username, password, database_connection_name)
    database_connect = database_connection_object.oracle_connection
    curSites = database_connect.cursor()
    curSites.execute("Select ENVIRONMENT,SYSTEM,AGS_FILE_PATH,MXD_ROOT_PATH,SD_FOLDER_PATH,CREATESDFILEONLY,HOST_NAME from PUB_ArcGIS_SITES where Active = 'TRUE' AND LOWER(PUBLISHER_HOST) = LOWER('" + socket.gethostname() + "')")
    resSites = curSites.fetchall()
    if len(resSites) == 0 :
        print "No Active Site Record found in PUB_ArcGIS_SITES table."
        logger.log("No Active Site Record found in PUB_ArcGIS_SITES table.")
        
    
    for rowSite in resSites:
        print str(rowSite[1])
        ENVIRONMENT = str(rowSite[0])
        SYSTEM = str(rowSite[1])
        AGS_FILE_PATH = str(rowSite[2])
        MXD_ROOT_PATH = str(rowSite[3])
        SD_FOLDER_PATH = str(rowSite[4])
        HOST_NAME = str(rowSite[6]).upper()
        curMapProperties = database_connect.cursor()
        curMapStatusUpdate = database_connect.cursor()
        curArcgisHosts = database_connect.cursor()
        #Status 0-Ready to publish, 1- IN Progress, 2 PUblished 3 failed
        #Active = TRUE (To be published) , False = Not to be published
        #Fetch map servcie properties details
        #qry = "Select * from PUB_MAP_PROPERTIES where ENVIRONMENT = '" + ENVIRONMENT + "' and System = '" + SYSTEM +"'"
        qry = "Select m.*,h.s_n,h.publish_method from pub_arcgis_hosts h,PUB_MAP_PROPERTIES m where h.System = m.System and h.ENVIRONMENT = m.ENVIRONMENT and h.service_name = m.service_name and h.ENVIRONMENT = '" + ENVIRONMENT + "' and h.System = '" + SYSTEM +"' and  h.ACTIVE = 'TRUE'  AND h.STATUS = 0 and upper(h.host_name) = '"+ HOST_NAME +"'"
        
        logger.log(qry)
        
        curMapProperties.execute(qry)
        resMapProperties = curMapProperties.fetchall()
        if len(resMapProperties) == 0 :
            print "no  record found in PUB_MAP_PROPERTIES table to publish map service"
            logger.log("no  record found in PUB_MAP_PROPERTIES table to publish map service")
            continue
        for rowMapService in resMapProperties:
            
            try :
                # Set Status to in progress Status = "1"
                qry = "Update pub_arcgis_hosts Set Status = 1 , LAST_RUN_DATE = sysdate,LAST_RUN_MESSAGE = '' where S_N =  " + str(rowMapService[33] )
                curMapStatusUpdate.execute(qry)
                database_connect.commit()
                
                #Publish Map Service 
                obj = PublishMap(rowMapService,rowSite,logger,SD_FOLDER_PATH)
                result = obj.publish().replace("'","")
                print result
                if result.upper() == "SUCCESS" :
                    # Set Status to complete Status = "2" 
                    qry = "Update pub_arcgis_hosts Set Status = 2,LAST_RUN_DATE = sysdate,LAST_RUN_MESSAGE = 'Success' where S_N =  " + str(rowMapService[33])
                else :
                    qry = "Update pub_arcgis_hosts Set Status = 3,LAST_RUN_DATE = sysdate,LAST_RUN_MESSAGE = '" + result + "' where S_N =  " + str(rowMapService[33])
                curMapStatusUpdate.execute(qry)
                database_connect.commit()
                
            except Exception as exc:
                print str(exc)
                qry = "Update pub_arcgis_hosts Set Status = 3,LAST_RUN_DATE = sysdate,LAST_RUN_MESSAGE = '" + str(exc) + "' where S_N =  " + str(rowMapService[33])
                curMapStatusUpdate.execute(qry)
                database_connect.commit()
            
        curMapProperties.close()
        curMapStatusUpdate.close()
    curSites.close()
    database_connect.close()

except Exception as exc:
    # Get the traceback object
    print str(exc)
finally:
     # finally block
     #database_connect.close()
     logger.log("Program ended")
     print ("Program ended")

