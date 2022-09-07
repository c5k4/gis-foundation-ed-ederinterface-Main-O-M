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

class RePublishMap :
    
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

    # publish the service with the SD file
    def _publishfromSD(self, sd, con):
        try:
            self.logger.log("Start Publishing using SD File")
            arcpy.UploadServiceDefinition_server(sd, con)
            self.logger.log(arcpy.GetMessages())
            print "Service successfully published using SD file"
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

    # Analyze, Stage and upload the service if the sddraft analysis did not contain errors
    def _analyzeStageAndUploadService(self, outXml, sd, con):
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
                
                self._publishfromSD(sd, con)
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
        
        return "SUCCESS"

    # update the service parameters
    def _updateServiceParameters(self, doc):
        #Prop section of configuration file                                   
        props = doc.getElementsByTagName('Props')[0]
        propArray = props.firstChild
        propSets = propArray.childNodes
        for propSet in propSets:
            keyValues = propSet.childNodes
            #print keyValues
            for keyValue in keyValues:
                if keyValue.tagName == 'Key':
                    if keyValue.firstChild.data == "MinInstances":
                        keyValue.nextSibling.firstChild.data = self.minInstances
                    elif keyValue.firstChild.data == "MaxInstances":
                        keyValue.nextSibling.firstChild.data = 3 #self.maxInstances

    # republish the service with updated parameters
    def republish(self):
    
        try:
            sddraft = self.wrkspc  + self.service + 'temp.sddraft'
            sd = self.wrkspc +  self.service + 'temp.sd'
            
            print "Started updating Map Service# sddraft=" + sddraft
            self.logger.log("Started updating Map Service# sd=" + sd)
            print "Publish Method :" + self.publishMethod

            doc = DOM.parse(sddraft)
            # change the xml as required
            self._updateServiceParameters(doc)

            # output to a new sddraft
            outXml = self.wrkspc  + self.service + '.sddraft'
            if os.path.exists(outXml):
                print "removing the existing sddraft file: " + outXml
                os.remove(outXml)
            if os.path.exists(sd):
                print "removing the existing sd file: " + sd
                os.remove(sd)
            
            print "creating new sddraft: " + outXml
            f = open(outXml, 'w')     
            doc.writexml( f )     
            f.close() 

            con = self.arcGISConnectionPath
            self._analyzeStageAndUploadService(outXml, sd, con)
            
            self.logger.log("Complete process for Map Service using the sd file# sd=" + sd)
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
            print pymsg
            return str(sys.exc_info()[1])
        finally:
            print "Done Republishing"
            

    
