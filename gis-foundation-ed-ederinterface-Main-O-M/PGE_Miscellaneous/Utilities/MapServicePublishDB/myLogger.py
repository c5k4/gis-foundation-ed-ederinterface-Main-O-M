import arcpy, os, sys, string, datetime, time, ConfigParser, shutil, traceback
from arcpy import env
from time import strftime
from datetime import date, timedelta
import ntpath


class Logger:
   
    def __init__(self, logPath,callingFilePath,logPrefix,toScreen):
        self.logPath = logPath
        self.callingFilePath = callingFilePath        
        self.callingFileNoExt = os.path.splitext(callingFilePath)[0]
        self.startTime = strftime("%Y-%m-%d-%H-%M-%S")
        self.logFileName = ntpath.basename(self.callingFileNoExt) + '_' + str(self.startTime) + '.log'        
        if (len(logPrefix)>0):            
            self.logFullPath = self.logPath + '\\' + logPrefix + '_' + self.logFileName  
        else:
            self.logFullPath = self.logPath + '\\' + self.logFileName  
        self._createLogFile()
        self.errList=[]
        self.toScreen=toScreen
    
    def close(self):
        self.logFile.close()
        
    def log(self,logMsg,errList=None):    
        timeStamp = strftime("%Y-%m-%d-%H-%M-%S");
        if (self.toScreen):
            arcpy.AddMessage(logMsg)        
        self.logFile.write (str(timeStamp) + ": ")		
        self.logFile.write (logMsg+"\n")	    
        if ( not errList is None):        
            self.errList.append(errList)
    
    def logList(self,msgList,msgTitle=None):  
        if (not msgTitle == None):
            self.log(msgTitle)
        for msg in msgList:        
            arcpy.AddMessage(msg)             
            self.logFile.write (msg+"\n")
    
    def logOnlyList(self,msgList,msgTitle=None):  
        if (not msgTitle == None):
            self.logOnly(msgTitle)
        for msg in msgList:        
            self.logFile.write (msg+"\n")

    def logOnly(self,logMsg):    
        timeStamp = strftime("%Y-%m-%d-%H-%M-%S");
        self.logFile.write (str(timeStamp) + ": ")		
        self.logFile.write (logMsg+"\n")	    
        
    def logGp2(self,gpResult,errList=None):  
        msg = ''
        if (gpResult.status == 4):
            msg = ":-) GP Status Succeeded (" + str(gpResult.status) + ")"
        else:
            s1 = "Error --> GP Error Status = " + str(gpResult.status)
            s2 = ":-( ERROR START ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
            s3 =  gpResult.getMessages()                     
            s4 = ":-( ERROR END   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
            msg = s1 + '\n' + s2 + '\n' + s3 + '\n' + s4    
        self.log(msg,errList)            
            
    def logGp(self,gpResult,errList=None):  
        msg = ''
        if (gpResult.status == 4):
            msg = ":-) GP Status = " + str(gpResult.status) + '\n' + gpResult.getMessages()
        else:
            s1 = "Error --> GP Error Status = " + str(gpResult.status)
            s2 = ":-( ERROR START ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
            s3 =  gpResult.getMessages()                     
            s4 = ":-( ERROR END   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
            msg = s1 + '\n' + s2 + '\n' + s3 + '\n' + s4    
        self.log(msg,errList)

    def logEx(self,exceptionMsg,errList=None):
         self.log('^----Exception-----\n',None)
         self.log(exceptionMsg,errList)
         self.log('^------------------\n',None)

    def logExOnly(self,exceptionMsg):
         self.logOnly('^----Exception-----\n')
         self.logOnly(exceptionMsg)
         self.logOnly('^------------------\n')
         
         
    def errSummary(self):    
        for msg in self.errList:        
            arcpy.AddMessage(msg)             
            self.logFile.write (msg+"\n")	    
            

    def _createLogFile(self):    
        try:       
            #set file path                        
            arcpy.AddMessage('\n---Start---')              
            arcpy.AddMessage(self.logFullPath)
            #open and write
            self.logFile=open (self.logFullPath, 'a')
            self.logFile.write (str(self.startTime) + ": Start\n")		     
        except:
            arcpy.AddError("CreateLogFile Error")
            arcpy.AddError(traceback.format_exc()) 
              
def CreateLogFile(logPath,callingFilePath):    
    try:       
        
        #set file path
        startTime = strftime("%Y-%m-%d-%H-%M-%S")
        callingFileNoExt = os.path.splitext(callingFilePath)[0]
        
        logFileName = ntpath.basename(callingFileNoExt) + '_' + str(startTime) + '.log'        
        logFullPath = logPath + '\\'+ logFileName        
        
        
        arcpy.AddMessage(logFullPath)
        
        #open and write
        logFile=open (logFullPath, 'a')
        logFile.write (str(startTime) + ": Start\n")		     
        #to cmd prompt
        arcpy.AddMessage('Start')        
        
        return logFile,logFullPath,logFileName
    except:
        arcpy.AddError("CreateLogFile Error")
        arcpy.AddError(traceback.format_exc())    
        
def LogMsg(logFile, logMsg,errList=None):    
    timeStamp = strftime("%Y-%m-%d-%H-%M-%S");
    arcpy.AddMessage(logMsg)             
    logFile.write (str(timeStamp) + ": ")		
    logFile.write (logMsg+"\n")	    
    if ( not errList is None):        
        errList.append(logMsg)