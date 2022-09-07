# Publishes a service to machine myserver using USA.mxd
# A connection to ArcGIS Server must be established in the
#  Catalog window of ArcMap before running this script
from ConfigParser import SafeConfigParser
from time import sleep
import arcpy
import xml.dom.minidom as DOM
import codecs
import ast,os,sys
import csv
import myLogger
import traceback

class PublishMap :
    
    def __init__(self,rowMapService,rowSite,logger,workspace):
        try:
            
            self.logger = logger
            self.arcGISConnectionPath = rowSite[2]
            self.mxdRootFolder = rowSite[3]
            self.wrkspc = workspace
            self.createSDFileOnly = rowSite[5]
            

            self.sno = rowMapService[0]
            
            if rowMapService[1] is None :
                self.folder = ''
            else:
               self.folder = rowMapService[1]
               
           
            self.mxd = rowMapService[2]
            self.service = rowMapService[3]
            self.summary = ''
            self.tags = ''
            self.servertype = rowMapService[4]
            self.description = rowMapService[5]
            self.ClusterName = rowMapService[6]
            self.minInstances = rowMapService[7]
            self.maxInstances = rowMapService[8]
            self.InstancesPerContainer = rowMapService[9]
            self.WaitTimeout = rowMapService[10]
            self.IdleTimeout = rowMapService[11]
            #cleanupTimeout = row[11]
            self.keepAliveInterval = rowMapService[12]
            self.startupTimeout = rowMapService[13]
            self.isolation = rowMapService[14]
            self.configuredState = rowMapService[15]
            self.recyclingInterval = rowMapService[16]
            
            self.maxRecordCount = rowMapService[17]
            self.MaxImageHeight = rowMapService[18]
            self.MaxImageWidth = rowMapService[19]
            self.antialiasingMode = rowMapService[20]
            self.textAntialiasingMode = rowMapService[21]
            self.usageTimeout = rowMapService[22]
            self.runningStatus = rowMapService[23] # not in use
            self.enablefeatureService = rowMapService[24] 
            self.enableKml = rowMapService[25]
            self.enableWms = rowMapService[26]
            self.recycleStartTime = rowMapService[27]
            self.enableArcFMMapServer="false"
            #28 - CAPABILITIES
            #29-SCHEMALOCKINGENABLED
            self.schemaLockingEnabled = rowMapService[29] 
            self.maxDomainCodeCount=rowMapService[30]
            
            #31-ENVIRONMENT
            #32-SYSTEM
            
            #33 S_N from host table
            self.publishMethod = rowMapService[34]
            print "Done reading propoerties"
            
        except Exception as exc:
            print ("initialization failed" ,exc)
            #self.search_logger.writeTransactionLog(self.transaction_name, self.transaction_input, 'DatabaseConnection()', 'Failed', str(exc), 'error', False, False)

    # Define local variables mxdRootFolder
    def publish(self):

        try:
            con = self.arcGISConnectionPath
            sddraft = self.wrkspc  + self.service + 'temp.sddraft'
            sd = self.wrkspc +  self.service + 'temp.sd'
            # if publishMethod is set to SD in PUB_MAP_PROPERTIES table for this service
            # the service will be published using existing SD file in configured SD Folder
            print "Started Map Service# Mxd=" + self.mxd
            self.logger.log("Started Map Service# Mxd=" + self.mxd)
            
            print "Publish Method :" + self.publishMethod
            print "createSDFileOnly flag :" + self.createSDFileOnly
            #return "SUCCESS"
            if self.publishMethod == "SD" and self.createSDFileOnly.upper() == "FALSE" :
                try:
                    
                    self.logger.log("Start Publishing using SD File")
                    arcpy.UploadServiceDefinition_server(sd, con)
                    self.logger.log(arcpy.GetMessages())
                    print "Service successfully published using existing SD file"
                    return "SUCCESS"
                except :
                    # Get the traceback object
                    tb = sys.exc_info()[2]
                    tbinfo = traceback.format_tb(tb)[0]
                    # Concatenate information together concerning the error into a message string
                    pymsg = "ERRORS:\nTraceback info:\n" + tbinfo + "\nError Info:\n" + str(sys.exc_info()[1])
                    # Return python error messages for use in script tool or Python window
                    ##    arcpy.AddError(pymsg)
                    self.logger.log(pymsg)
                    return str(sys.exc_info()[1])       
            
            # To pubish service using MXD file 

            self.logger.log(os.path.join(self.mxdRootFolder,self.folder,self.mxd))
            #mxdpath = '\\\\SFSHARE04-NAS2\\sfgispoc_data\\ApplicationDevelopment\\EDGIS_ReArchitecture\\MapDeployment\\Test\\EDGIS\\MXDs\\WEBR\\Data\\ElectricDistribution.mxd'
            #mxdpath = r"\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Test\EDGIS\MXDs\WEBR\Data\ElectricDistribution.mxd"
            #self.logger.log(mxdpath)
            #mapDoc = arcpy.mapping.MapDocument(mxdpath)
            #return "Failed"
            print "mxd path" + os.path.join(self.mxdRootFolder,self.folder,self.mxd)
            mapDoc = arcpy.mapping.MapDocument(os.path.join(self.mxdRootFolder,self.folder,self.mxd))
            
            print "Start creating SDF file"
            # Create service definition draft
            print "ags connection : " + con
            print "sd file : " + sd
            print "sd draft file : " + sddraft
            arcpy.mapping.CreateMapSDDraft(mapDoc, sddraft, self.service, 'ARCGIS_SERVER', con, False, self.folder, self.summary, self.tags)
            print "Finish creating SDF file"

            # set properties
            # find the Item Information Description element
            # the new description
            
            doc = DOM.parse(sddraft)
            
            #Set cluster 
            clusters = doc.getElementsByTagName('Cluster')
            for desc in clusters:
                if desc.parentNode.tagName == 'ItemInfo':
                    # modify the Description
                    if desc.hasChildNodes():
                        desc.firstChild.data = self.ClusterName
                    else:
                        txt = doc.createTextNode(self.ClusterName)
                        desc.appendChild(txt)
            #Set recycling interval 
            recyclingIntervals = doc.getElementsByTagName('RecyclingInterval')
            for node in recyclingIntervals:
                if node.parentNode.tagName == 'ItemInfo':
                    # modify the Description
                    print "found recycleInterval"
                    if node.hasChildNodes():
                        node.firstChild.data = self.recyclingInterval
                    else:
                        txt = doc.createTextNode(self.recyclingInterval)
                        #node.appendChild(txt)

            recycleStartTimes = doc.getElementsByTagName('RecycleStartTime')
            for node in recycleStartTimes:
                if node.parentNode.tagName == 'ItemInfo':
                    # modify the Descriptiona
                    print "found recycle Start time"
                    if node.hasChildNodes():
                        node.firstChild.data = self.recycleStartTime
                    else:
                        txt = doc.createTextNode(self.recycleStartTime)
                        #node.appendChild(txt)            
            #Prop section of configuration file                                   
            props = doc.getElementsByTagName('Props')[0]
            propArray = props.firstChild
            propSets = propArray.childNodes
            for propSet in propSets:
                keyValues = propSet.childNodes
                #print keyValues
                for keyValue in keyValues:
                    if keyValue.tagName == 'Key':
                        if keyValue.firstChild.data == "schemaLockingEnabled":
                            keyValue.nextSibling.firstChild.data = 'true'
                        elif keyValue.firstChild.data == "recycleInterval":
                            txt = doc.createTextNode(str(self.recyclingInterval))
                            keyValue.nextSibling.appendChild(txt)
                        elif keyValue.firstChild.data == "recycleStartTime":
                            txt = doc.createTextNode(str(self.recycleStartTime))
                            keyValue.nextSibling.appendChild(txt)
                        elif keyValue.firstChild.data == "MinInstances":
                            keyValue.nextSibling.firstChild.data = self.minInstances
                        elif keyValue.firstChild.data == "MaxInstances":
                            keyValue.nextSibling.firstChild.data = self.maxInstances
                        elif keyValue.firstChild.data == "InstancesPerContainer":
                            keyValue.nextSibling.firstChild.data = self.InstancesPerContainer
                        elif keyValue.firstChild.data == "WaitTimeout":
                            keyValue.nextSibling.firstChild.data = self.WaitTimeout
                        elif keyValue.firstChild.data == "IdleTimeout":
                            keyValue.nextSibling.firstChild.data = self.IdleTimeout
                        elif keyValue.firstChild.data == "UsageTimeout":
                            keyValue.nextSibling.firstChild.data = self.usageTimeout
                        elif keyValue.firstChild.data == "keepAliveInterval":
                            keyValue.nextSibling.firstChild.data = self.keepAliveInterval
                        elif keyValue.firstChild.data == "StartupTimeout":
                            keyValue.nextSibling.firstChild.data = self.startupTimeout
                        elif keyValue.firstChild.data == "Isolation":
                            keyValue.nextSibling.firstChild.data = self.isolation
                        elif keyValue.firstChild.data == "configuredState":
                            keyValue.nextSibling.firstChild.data = self.configuredState
                        elif keyValue.firstChild.data == "maxDomainCodeCount":
                            keyValue.nextSibling.firstChild.data = self.maxDomainCodeCount
                        elif keyValue.firstChild.data == "dynamicDataWorkspaces":
                            keyValue.nextSibling.firstChild.data ='false'
                            
                            
                                            
            #ConfigurationProperties section of configuration file                                   
            ConfigurationProperties = doc.getElementsByTagName('ConfigurationProperties')[0]
            ConfigurationPropertiesArray = ConfigurationProperties.firstChild
            ConfigurationPropertiesSets = ConfigurationPropertiesArray.childNodes
            for propSet in ConfigurationPropertiesSets:
                keyValues = propSet.childNodes
                for keyValue in keyValues:
                    if keyValue.tagName == 'Key':
                        if keyValue.firstChild.data == "maxRecordCount":
                            keyValue.nextSibling.firstChild.data = self.maxRecordCount
                        elif keyValue.firstChild.data == "MaxImageHeight":
                            keyValue.nextSibling.firstChild.data = self.MaxImageHeight
                        elif keyValue.firstChild.data == "MaxImageWidth":
                            keyValue.nextSibling.firstChild.data = self.MaxImageWidth
                        elif keyValue.firstChild.data == "antialiasingMode":
                            keyValue.nextSibling.firstChild.data = self.antialiasingMode
                        elif keyValue.firstChild.data == "textAntialiasingMode":
                            keyValue.nextSibling.firstChild.data = self.textAntialiasingMode
                        elif keyValue.firstChild.data == "schemaLockingEnabled":
                            keyValue.nextSibling.firstChild.data = 'true'
                        elif keyValue.firstChild.data == "maxDomainCodeCount":
                            keyValue.nextSibling.firstChild.data = self.maxDomainCodeCount
                            
                                            
            #Enabled disabled KML map service capability
            services = doc.getElementsByTagName('TypeName')
            for service_ in services:
                print "Capability: " + service_.firstChild.data
                if service_.firstChild.data == 'KmlServer':
                    print 'Kml Server : '+ self.enableKml
                    service_.parentNode.getElementsByTagName('Enabled')[0].firstChild.data = self.enableKml.lower()
                if service_.firstChild.data == 'WMSServer':
                    print 'WMSServer Server : '+ self.enableWms
                    service_.parentNode.getElementsByTagName('Enabled')[0].firstChild.data = self.enableWms.lower()
                if service_.firstChild.data == 'FeatureServer':
                    print 'Feature Server : '+self.enablefeatureService
                    service_.parentNode.getElementsByTagName('Enabled')[0].firstChild.data = self.enablefeatureService.lower()
                if service_.firstChild.data == 'ArcFMMapServer':
                    print 'ArcFMMap Server : '+self.enableArcFMMapServer
                    service_.parentNode.getElementsByTagName('Enabled')[0].firstChild.data = self.nableArcFMMapServer.lower()    
                                             
            # output to a new sddraft
            outXml = self.wrkspc  + self.service + '.sddraft'
            f = open(outXml, 'w')     
            doc.writexml( f )     
            f.close() 

            print "Start Analyzing SDF file"
            # Analyze the service definition draft
            analysis = arcpy.mapping.AnalyzeForSD(outXml)

            # Print errors, warnings, and messages returned from the analysis
            print "The following information was returned during analysis of the MXD:"
            for key in ('messages', 'warnings', 'errors'):
                #print '----' + key.upper() + '---'
                self.logger.log('----' + key.upper() + '---')
                vars = analysis[key]
                for ((message, code), layerlist) in vars.iteritems():
                    #print '    ', message, ' (CODE %i)' % code
                    self.logger.log('----' + key.upper() + '---')
                    # logger.log(message, ' (CODE %i)' % code)
                    self.logger.log('       applies to:')
                    #print '       applies to:',
                    for layer in layerlist:
                        #print layer.name,
                        self.logger.log(layer.name+"/n")
                
            #print "Finished Analyzing map"
            self.logger.log("Finished Analyzing map")
            # Stage and upload the service if the sddraft analysis did not contain errors
            if analysis['errors'] == {}:
                self.logger.log(outXml)
                # Execute StageService. This creates the service definition.
                arcpy.StageService_server(outXml, sd)
                print "Finished staging service"

                # Execute UploadServiceDefinition. This uploads the service definition and publishes the service.
                if self.createSDFileOnly.upper() == "FALSE" :
                    
                    arcpy.UploadServiceDefinition_server(sd, con)
                    self.logger.log(arcpy.GetMessages())
                    print "Service successfully published using MXD"
                else :
                    print "created SD File only. exiting.."
                    return "SUCCESS"
            else:
                self.logger.log(str(analysis['errors']))
                self.logger.log("Service could not be published because errors were found during analysis.")
                print "Service could not be published because errors were found during analysis."
                print "Exiting, please fix the error in the mxd"
                #print arcpy.GetMessages()
                return str(analysis['errors'])
            
            self.logger.log("Complete process for Map Service# Mxd=" + self.mxd)
            print "sleeping for 5 seconds"
            sleep(5)
            return "SUCCESS"
        except:
            # Get the traceback object
            tb = sys.exc_info()[2]
            tbinfo = traceback.format_tb(tb)[0]

            # Concatenate information together concerning the error into a message string
            pymsg = "ERRORS:\nTraceback info:\n" + tbinfo + "\nError Info:\n" + str(sys.exc_info()[1])
            # Return python error messages for use in script tool or Python window
            ##    arcpy.AddError(pymsg)
            self.logger.log(pymsg)
            return str(sys.exc_info()[1])
        finally:
            print "Done Publishing"
            

    
