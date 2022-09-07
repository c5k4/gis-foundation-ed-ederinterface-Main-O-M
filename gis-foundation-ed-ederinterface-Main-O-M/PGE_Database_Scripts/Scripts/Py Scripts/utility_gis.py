# ===========================================================================
# utility_gis.py
# ---------------------------------------------------------------------------
# Need to uncomment these after testing:
# compress
# delete version
# unregister as versioned.
# kill connection
# truncate tables
# 
# This script contains useful GIS functions.
# Usage: import utility_gis
#        Then call a function in this script via
#        returnedObject = functionName(argument)
#
# Example: strArg1 = utility_gis.getArg(1)
# Example: strArg2 = utility_gis.getArg(2)
#
# Note that this script can import the arcpy library from
# either ArcGIS Desktop or ArcGIS Server.
# If you install both Desktop and Server on the machine
# running this script, the Desktop library and license will be used,
# unless you rename the file
# C:\Python26\ArcGIS10.0\Lib\site-packages\Desktop10.pth
#   to
# C:\Python26\ArcGIS10.0\Lib\site-packages\Desktop10.txt.
# You need to change the filename extension.
# 
# Author: Vince Ulfig vulfig@gmail.com 4157101998
# ===========================================================================
# Import system modules...
import sys, os, string
strFileScript    = os.path.abspath(__file__)
print 'Imported sys, os, string into ' + strFileScript + '.'
print ''

import arcpy
from   datetime import datetime

# ===========================================================================
def main():
# ---------------------------------------------------------------------------
  #strFileScript = os.path.abspath(__file__)
  #print 'Imported  modules into ' + strFileScript + '.'
  #print ''
  intReturnCode = 0
  strFunctionName = getArg(1)
  #print 'strFunctionName = ' + strFunctionName
  #print ''

  #print 'Building list of arguments...'
  strArgList      = ''
  for intArgNum in range(2,10):
    strArg        = getArg(intArgNum)
    if (strArg != ''):
      strArgList = strArgList + ',' + strArg
      #listStrArg.append(strArg)

  # Trim leading comma from strArgList
  if (strArgList != ''):
    strArgList = strArgList[1:]

  if (strFunctionName != ''):
    try:
      #print 'Executing ' + strFunctionName + '(' + strArgList + ')...' 
      #returnValue = eval(strFunctionName+"(strArgList)")
      print 'Executing ' + strFunctionName + '(' + str(sys.argv[2:]) + ')...' 
      print ''
      returnValue = eval(strFunctionName+"(*sys.argv[2:])")
      print 'Executed  ' + strFunctionName + '(' + str(sys.argv[2:]) + ').' 
      #print 'Executed  ' + strFunctionName + '(' + strArgList + ').' 
      print ''
      if (returnValue == 0):
        return 0
      else:
        return 1
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print ''
      return 1
# ===========================================================================

# ===========================================================================
def getArg(intArg):
# ---------------------------------------------------------------------------
  strArg = ''
  if (intArg == ''):
    intArg = 1
  try:
    #print 'Testing for and getting argument ' + str(intArg) + '...'
    listArg = sys.argv[1:]
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Error in reading arguments.'
    return strArg
  
  try:
    i = 1
    if (len(listArg) > 0):
      for strArgTmp in listArg:
        if (i == intArg):
          strArg = sys.argv[intArg]
          #print 'Read into argument ' + str(intArg) + ': "' + strArgTmp + '".'
          break
        i = i + 1
    else:
      print 'No arguments found.'
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    print 'Error in processing arguments.'
    print 'Returning "' + strArg + '"...'
    printTimeNow()
    return strArg
  
  #print 'Returning "' + strArg + '"...'
  return strArg
# ===========================================================================

# ===========================================================================
def isNumber(object):
# ---------------------------------------------------------------------------
  try:
    object + 1
    return True
  except TypeError:
    return False
# ===========================================================================

# ===========================================================================
def getStrTimeUnderscoreMilliSec(dateTimeIn):
#  ---------------------------------------------------------------------------
  strTimeFormat = '%Y_%m_%d_%H_%M_%S.%f'
  strTime       = dateTimeIn.strftime(strTimeFormat)
  intLen        = len(strTime)
  intPos        = intLen - 3
  strTime       = strTime[:intPos]
  strTime       = strTime.replace(".","_")
  return strTime
# ===========================================================================

# ===========================================================================
def getStrTimeUnderscore(dateTimeIn):
#  ---------------------------------------------------------------------------
  strTimeFormat = '%Y_%m_%d_%H_%M_%S'
  strTime       = dateTimeIn.strftime(strTimeFormat)
  return strTime
# ===========================================================================

# ===========================================================================
def getStrTimeNowUnderscore():
#  ---------------------------------------------------------------------------
  strTime       = getStrTimeUnderscore(datetime.now())
  return strTime
# ===========================================================================

# ===========================================================================
def getStrTime(dateTimeIn):
#  ---------------------------------------------------------------------------
  strTimeFormat = '%Y_%m_%d %H:%M:%S.%f'
  strTime       = dateTimeIn.strftime(strTimeFormat)
  intLen        = len(strTime)
  intPos        = intLen - 3
  strTime       = strTime[:intPos]
  return strTime
# ===========================================================================

# ===========================================================================
def getStrTimeNow():
#  ---------------------------------------------------------------------------
  strTime       = getStrTime(datetime.now())
  return strTime
# ===========================================================================

# ===========================================================================
def printTimeNow():
#  ---------------------------------------------------------------------------
  strTime       = getStrTimeNow()
  print 'Time:\t\t' + strTime
# ===========================================================================

# ===========================================================================
def printTime(dateTimeIn):
# ---------------------------------------------------------------------------
  strTime       = getStrTime(dateTimeIn)
  print 'Time:\t\t' + strTime
# ===========================================================================

# ===========================================================================
def printTimeDelta(timeStart,timeEnd):
# ---------------------------------------------------------------------------
  timeDeltaElapsed = (timeEnd - timeStart)
  strTimeDelta     = timeDeltaToString(timeDeltaElapsed)
  print 'Elapsed:\t' + strTimeDelta
# ===========================================================================;

# ===========================================================================
def timeDeltaToString(TimeDeltaIn):
# ---------------------------------------------------------------------------
  strTimeDeltaOut = ''
  intDays         = TimeDeltaIn.days
  intSecs         = TimeDeltaIn.seconds
  intMics         = TimeDeltaIn.microseconds

  intMins,intSecs = divmod(intSecs,60)
  intHrrs,intMins = divmod(intMins,60)

  intYrrs,intDays = divmod(intDays,365)
  intMons,intDays = divmod(intDays,30)

  strYrrs         = str(intYrrs).zfill(4)
  strMons         = str(intMons).zfill(2)
  strDays         = str(intDays).zfill(2)
  strHrrs         = str(intHrrs).zfill(2)
  strMins         = str(intMins).zfill(2)
  strSecs         = str(intSecs).zfill(2)
  strMics         = str(intMics)
  strMics         = strMics[:3]

  strTimeDeltaOut = strYrrs + '_' + strMons + '_' + strDays + ' ' + strHrrs + ':' + strMins + ':' + strSecs + '.' + strMics
  return strTimeDeltaOut
# ===========================================================================

# ===========================================================================
def getHostname():
# ---------------------------------------------------------------------------
  import socket
  strHostname = socket.gethostname().lower()
  return strHostname
# ===========================================================================

# ===========================================================================
def WriteLineToFile(strFile,strLine,blnAppend):
# ---------------------------------------------------------------------------
  intReturnCode = 0
  strLine = strLine + '\n'
  strAppend = 'w'
  if (blnAppend):
    strAppend = 'a'
  try:
    fileAppend = open(strFile, strAppend)
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    intReturnCode = intReturnCode + 1
    return intReturnCode

  try:
    fileAppend.write(strLine)
    fileAppend.close
    del fileAppend
  except Exception as e:
    fileAppend.close
    del fileAppend
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    intReturnCode = intReturnCode + 1
    return intReturnCode
  return intReturnCode
# ===========================================================================

# ===========================================================================
def Execute(strCommand,blnPrintTime):
# ---------------------------------------------------------------------------
  intReturnCode       = 0
  import subprocess
  try:
    timeStart = datetime.now()
    if (blnPrintTime):
      print 'Executing ' + strCommand + '...'
      printTime(timeStart)
    if (isinstance(strCommand, basestring)):
      if (blnPrintTime):
        print 'Executing subprocess.call(' + strCommand + ', shell=True)...'
      intReturnCode = intReturnCode + subprocess.call(strCommand, shell=True)
      intReturnCode = intReturnCode + subprocess.check_output(strCommand, shell=True)
      if (blnPrintTime):
        print 'Executed  subprocess.call(' + strCommand + ', shell=True).'
    else:
      # If you want to pass a list of arguments, do it this way with shell=False:
      intReturnCode = intReturnCode + subprocess.call([strFileExe,strArg1,strArg2], shell=False)
      intReturnCode = intReturnCode + subprocess.call(strCommand, shell=False)
    timeEnd   = datetime.now()
    if (blnPrintTime):
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Executed  ' + strCommand + '.'
      print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    print 'ERROR: Exception during execution of ' + strCommand
    printTimeNow()
    return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def sendEmail(strEmailFrom,lstStrEmailTo,strSubject,strMsg,lstStrFileAttach):
# ---------------------------------------------------------------------------
  # This function can accept a list of strings or a comma-separated string
  # for the arguments lstStrEmailTo and lstStrFileAttach.

  import smtplib
  
  from email.mime.text     import MIMEText
  from email.MIMEMultipart import MIMEMultipart
  from email.MIMEImage     import MIMEImage
  from email.MIMEBase      import MIMEBase
  from email               import Encoders

  strListEmailTo      = ''
  if (isinstance(lstStrEmailTo, basestring)):
    strListEmailTo    = lstStrEmailTo
    lstStrEmailTo     = strListEmailTo.split(',')
  else:
    # If a list is passed in, convert it to a comma-separated string.
    for strEmailTo in lstStrEmailTo:
      strListEmailTo  = strListEmailTo + ',' + strEmailTo + ''
    strListEmailTo    = strListEmailTo.strip(',')

  if (lstStrFileAttach != ''):
    # If a comma-separated string is passed in, convert it to a list.
    if (isinstance(lstStrFileAttach, basestring)):
       lstStrFileAttach = lstStrFileAttach.split(',')

  intReturnCode       = 0
  try:
    print 'Sending email...'
    msgOuter = MIMEMultipart()
    msgOuter['Subject'] = strSubject
    msgOuter['From']    = strEmailFrom
    msgOuter['To']      = strListEmailTo
    #msgOuter.preamble   = 'Preamble: You will not see this Premable in a MIME-aware mail reader.\n'
    msgOuter.preamble   = strSubject
    mimeTextPlainStrMsg = MIMEText(strMsg, 'plain')
    msgOuter.attach(mimeTextPlainStrMsg)
  
    if (lstStrFileAttach != ''):
      for strFileAttach in lstStrFileAttach:
        fileAttach = open(strFileAttach,'rb')
        msgTxtFile = MIMEText(fileAttach.read())
        fileAttach.close()
        strFilenameAttach = os.path.basename(strFileAttach)
        msgTxtFile.add_header('Content-Disposition', 'attachment', filename=strFilenameAttach)
        msgOuter.attach(msgTxtFile)

    strMsgOuter = msgOuter.as_string()
    server = smtplib.SMTP('mailhost.utility.pge.com')
    server.sendmail(strEmailFrom,lstStrEmailTo,strMsgOuter)
    server.quit()
    print 'Sent    email.'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    print 'ERROR: Did NOT send email.'
    print ''
    return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def makeFileBatFromFileScript(strFile):
# ---------------------------------------------------------------------------
  strFileBat        = ''
  try:
    strPathScript     = os.path.abspath(os.path.dirname(os.path.abspath(__file__)))
    strFileNameScript = os.path.abspath(os.path.basename(os.path.abspath(__file__)))
    intLen            = len(strFileNameScript)
    intLen3           = intLen - 3
    strFileNameScriptBase = strFileNameScript[:intLen3]
    strFileNameBat    = strFileNameScriptBase + '.bat'
    strFileBat        = os.path.abspath(os.path.join(strPathScript,strFileNameBat))
    return strFileBat
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    return 1
# ===========================================================================

# ===========================================================================
def isDescribable(strObj):
# ---------------------------------------------------------------------------
  # This function returns True  if the object is     describable.
  # This function returns False if the object is not describable.
  try:
    timeStart = datetime.now()
    printTime(timeStart)
    print 'Describing object ' + strObj + ' to determine whether it is describable.'
    objDescribe = arcpy.Describe(strObj)
    print 'Described  object ' + strObj + ' to determine whether it is describable.'
    print '           Object ' + strObj + ' is describable.'
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print ''
    return True
  except Exception as e:
    printTimeNow()
    print '           Object ' + strObj + ' is not describable.'
    print ''
    return False
# ===========================================================================

# ===========================================================================
def describe(strObj):
# ---------------------------------------------------------------------------
  objDescribe      = ''
  blnIsDescribable = isDescribable(strObj)
  if (blnIsDescribable):
    timeStart = datetime.now()
    printTime(timeStart)
    print 'Describing object ' + strObj + '...'
    objDescribe = arcpy.Describe(strObj)
    print 'Described  object ' + strObj + '.'
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print ''
  return objDescribe
# ===========================================================================

# ===========================================================================
def isWorkspace(strWorkspace):
# ---------------------------------------------------------------------------
  blnIsWorkspace = False
  intReturnCode  = 0
  strWorkspaceType        = ''
  try:
    objDescribe           = describe(strWorkspace)
    if (not isinstance(objDescribe, basestring)):
      strWorkspaceType    = objDescribe.workspaceType
    if ((strWorkspaceType == 'RemoteDatabase') or (strWorkspaceType == 'LocalDatabase') or (strWorkspaceType == 'FileSystem')):
      print 'Path ' + strWorkspace + ' is a workspace with type ' + strWorkspaceType + '.'
      print ''
      blnIsWorkspace      = True
    else:
      print 'Path ' + strWorkspace + ' is not a workspace.'
      print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
  return blnIsWorkspace
# ===========================================================================

# ===========================================================================
def isFileGdb(strWorkspace):
# ---------------------------------------------------------------------------
  blnIsFileGdb   = False
  intReturnCode  = 0
  #print 'strWorkspace     = ' + strWorkspace
  strWorkspaceType        = ''
  try:
    objDescribe           = describe(strWorkspace)
    if (not isinstance(objDescribe, basestring)):
      strWorkspaceType    = objDescribe.workspaceType
    #print 'strWorkspaceType = ' + strWorkspaceType
    if (strWorkspaceType == 'LocalDatabase'):
      strWorkspaceFactoryProgID = objDescribe.workspaceFactoryProgID
      #print 'strWorkspaceFactoryProgID = ' + strWorkspaceFactoryProgID
      if (strWorkspaceFactoryProgID == 'esriDataSourcesGDB.FileGDBWorkspaceFactory'):
        blnIsFileGdb      = True
      if (strWorkspaceFactoryProgID == 'esriDataSourcesGDB.FileGDBWorkspaceFactory.1'):
        blnIsFileGdb      = True

    if (blnIsFileGdb):
      print 'Workspace ' + strWorkspace + ' is a fileGdb.'
      print ''
    else:
      print 'Workspace ' + strWorkspace + ' is not a fileGdb.'
      print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
  return blnIsFileGdb
# ===========================================================================

# ===========================================================================
def setWorkspace(strWorkspace):
# ---------------------------------------------------------------------------
  intFoundPath = 1
  try:
    if (os.path.isdir(strWorkspace)):
      print strWorkspace + ' is a directory.'
      intFoundPath = 0
    if (os.path.isfile(strWorkspace)):
      print strWorkspace + ' is a file.'
      intFoundPath = 0
    if (intFoundPath == 1):
      print 'Did NOT find a file or directory at ' + strWorkspace + '.'
      print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT find a file or directory at ' + strWorkspace + '.'
    print ''
    return 1

  if (intFoundPath == 0):
    try:
      print 'Setting workspace to ' + strWorkspace + '...'
      timeStart = datetime.now()
      printTime(timeStart)
      arcpy.env.workspace = strWorkspace
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Set     workspace to ' + strWorkspace + '.'
      print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print 'Did NOT set workspace to ' + strWorkspace + '.'
      print ''
      return 1
  return 0
# ===========================================================================

# ===========================================================================
def createArcSDEConnectionFileStringBasic(strDbName,strUsername,blnTimeStamp):
# ---------------------------------------------------------------------------
  strFileConn          = ''
  strDbSoftware        = 'oracle'
  strDbSoftwareVersion = '11g'
  strFileConn          = createArcSDEConnectionFileString(strDbName,strDbSoftware,strDbSoftwareVersion,strUsername,blnTimeStamp)
  return strFileConn
# ===========================================================================

# ===========================================================================
def createArcSDEConnectionFileString(strDbName,strDbSoftware,strDbSoftwareVersion,strUsername,blnTimeStamp):
# ---------------------------------------------------------------------------
  strFileConn      = ''

  strPathScript     = os.path.dirname(os.path.abspath(__file__))
  strPathConn       = os.path.join(strPathScript,'conn_gdb')

  if ((os.path.isdir(strPathConn)) == False):
    try:
      print 'Creating directory ' + strPathConn + '...'
      os.mkdir(strPathConn)
      print 'Created  directory ' + strPathConn + '.'
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      print 'Did NOT Create directory ' + strPathConn + '.'
  if ((os.path.isdir(strPathConn)) == False):
    strPathConn = strPathScript

  installInfo      = arcpy.GetInstallInfo()
  strEsriVersion   = "{0}".format(installInfo["Version"])
  strEsriVersion   = strEsriVersion.replace('.','_')

  strFilenameConn   = strDbName + '_' + strEsriVersion + '_' + strDbSoftware + '_' + strDbSoftwareVersion + '_dc_' + strUsername
  if (blnTimeStamp):
    strTime         = getStrTimeUnderscoreMilliSec(datetime.now())
    strFilenameConn = strFilenameConn + '_' + strTime
  strFilenameConn   = strFilenameConn + '.sde'

  strFileConn   = os.path.join(strPathConn,strFilenameConn)
  return strFileConn
# ===========================================================================

# ===========================================================================
def createArcSDEConnectionFileBasic(strFileConn,strDbName,strUsername,strPassword):
# ---------------------------------------------------------------------------
  strServerName   = ''
  if (strPassword == ''):
    import getPassword
    strPassword   = getPassword.getPassword(strDbName,strUsername)
  strGdbVersion   = 'sde.default'
  strDbSoftware   = 'oracle'
  strDbSoftwareVersion = '11g'
  strFileConn     = createArcSDEConnectionFile(strFileConn,strDbName,strServerName,strUsername,strPassword,strGdbVersion,strDbSoftware,strDbSoftwareVersion)
  return strFileConn
# ===========================================================================

# ===========================================================================
def createArcSDEConnectionFile(strFileConn,strDbName,strServerName,strUsername,strPassword,strGdbVersion,strDbSoftware,strDbSoftwareVersion):
# ---------------------------------------------------------------------------
  intReturnCode   = 0

  strAccountAuth  = 'database_auth'
  strSaveUserInfo = 'save_username' # or 'do_not_save_username'
  strSaveVerInfo  = 'save_version'  # or 'do_not_save_version'

  if (strGdbVersion == ''):
    strGdbVersion = 'sde.default'

  if (strDbSoftware == ''):
    strDbSoftware = 'oracle'

  if (strServerName == ''):
    strServerName = 'oracle'

  # Should test whether these uppers and lowers are required some time.
  strGdbVersion   = strGdbVersion.upper()
  strDbName       = strDbName.lower()
  strDbSoftware   = strDbSoftware.lower()
  strAccountAuth  = strAccountAuth.upper()
  strSaveUserInfo = strSaveUserInfo.upper()
  strSaveVerInfo  = strSaveVerInfo.upper()

  strService      = 'sde:sql:' + strServerName 
  if (strDbSoftware == 'oracle'):
    #strService    = 'sde:' + strDbSoftware + strDbSoftwareVersion + ':/;local=' + strDbName
    strService =  strDbName
    print strService

  if (strFileConn == ''):
    strFileConn   = createArcSDEConnectionFileString(strDbName,strDbSoftware,strDbSoftwareVersion,strUsername,True)

  strPathConn     = os.path.dirname(strFileConn)
  strFilenameConn = os.path.basename(strFileConn)

  # Check for the .sde file and delete it if present
  arcpy.env.overwriteOutput = True

  print strPathConn
  print strFilenameConn
  print strFileConn
  print strServerName
  print strService
  print strAccountAuth
  print 'strDbName   = ' + strDbName
  print 'strUsername = ' + strUsername
  print 'strPassword = '
  print strSaveUserInfo
  print strGdbVersion
  print strSaveVerInfo

  try:
    print 'Creating ArcSDE Connection file ' + strFileConn + '...'
    timeStart = datetime.now()
    printTime(timeStart)
    #arcpy.CreateArcSDEConnectionFile_management(strPathConn, strFilenameConn, strServerName, strService, strDbName, strAccountAuth, strUsername, strPassword, strSaveUserInfo, strGdbVersion, strSaveVerInfo)
    #arcpy.CreateDatabaseConnection_management(strPathConn, strFilenameConn, strServerName.upper(), strService, strAccountAuth, strUsername, strPassword, strSaveUserInfo, "", "#", "TRANSACTIONAL", strGdbVersion)
    arcpy.CreateDatabaseConnection_management(strPathConn,
                                          strFilenameConn,
                                          strServerName,
                                          strService,
                                          strAccountAuth,
                                          strUsername,
                                          strPassword,
                                          strSaveUserInfo,
                                          "",
                                          "#",
                                          "TRANSACTIONAL",
                                          strGdbVersion)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Created  ArcSDE Connection file ' + strFileConn + '.'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    print 'Did NOT Create   ArcSDE Connection file ' + strFileConn + '.'

  return strFileConn
# ===========================================================================

# ===========================================================================
def getListFeatureDatasets(strWorkspace,strFilter):
# ---------------------------------------------------------------------------
  intReturnCode = 0
  intReturnCode = intReturnCode + setWorkspace(strWorkspace)
  lstFdName     = []
  if (intReturnCode == 0):
    try:
      print 'Creating dataset list for workspace ' + strWorkspace + '...'
      timeStart = datetime.now()
      printTime(timeStart)
      lstFdName = arcpy.ListDatasets(strFilter,"Feature")
      print 'Feature Dataset List: ' + str(lstFdName)
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Created  dataset list for workspace ' + strWorkspace + '.'
      print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      print 'Did NOT create dataset list for workspace ' + strWorkspace + '.'
      intReturnCode = intReturnCode + 1
      print ''
      return intReturnCode

    try:
      if (lstFdName.count == 0):
        intReturnCode = intReturnCode + 1
        print 'ERROR: Feature Dataset List Count = 0.'
        print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      print ''
      intReturnCode = intReturnCode + 1
      return intReturnCode
  return lstFdName
# ===========================================================================

# ===========================================================================
def osRemove(strObject):
# ---------------------------------------------------------------------------
  intReturnCode = 0
  try:
    print 'Executing os.remove(' + strObject + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    os.remove(strObject)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  os.remove(' + strObject + ').'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute os.remove(' + strObject + ').'
    print ''
    return 1
  return 0
# ===========================================================================

# =============================================================================
def delete(strObj):
# ---------------------------------------------------------------------------
  # This function deletes an Object.
  intReturnCode = 0
  blnDelete = False

  try:
    objDescribe = describe(strObj)
    if (not isinstance(objDescribe, basestring)):
      blnDelete = True
      strDatasetType = objDescribe.datasetType
      print 'DatasetType = ' + strDatasetType
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)

  if (blnDelete):
    try:
      print 'Executing arcpy.Delete_management(' + strObj + ',"")...'
      timeStart = datetime.now()
      printTime(timeStart)
      arcpy.Delete_management(strObj,"")
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Executed  arcpy.Delete_management(' + strObj + ',"").'
      print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print 'Did NOT execute arcpy.Delete_management(' + strObj + ',"").'
      print ''
      return 1
  return intReturnCode
# ===========================================================================

# =============================================================================
def deleteFc(strFc):
# ---------------------------------------------------------------------------
  # This function deletes a featureclass.
  intReturnCode = 0
  blnDelete = False

  try:
    objDescribe = describe(strFc)
    if (not isinstance(objDescribe, basestring)):
      strDatasetType = objDescribe.datasetType
      if (strDatasetType == 'FeatureClass'):
        blnDelete = True
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)

  if (blnDelete):
    try:
      print 'Executing arcpy.Delete_management(' + strFc + ',"FeatureClass")...'
      timeStart = datetime.now()
      printTime(timeStart)
      arcpy.Delete_management(strFc,"FeatureClass")
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Executed  arcpy.Delete_management(' + strFc + ',"FeatureClass").'
      print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print 'Did NOT execute arcpy.Delete_management(' + strFc + ',"FeatureClass").'
      print ''
      return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def deleteFd(strFd):
# ---------------------------------------------------------------------------
  # This function deletes a feature dataset.
  intReturnCode = 0
  blnDelete = False

  try:
    objDescribe = describe(strFd)
    if (not isinstance(objDescribe, basestring)):
      strDatasetType = objDescribe.datasetType
      if (strDatasetType == 'FeatureDataset'):
        blnDelete = True
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)

  if (blnDelete):
    try:
      print 'Executing arcpy.Delete_management(' + strFd + ',"FeatureDataset")...'
      timeStart = datetime.now()
      printTime(timeStart)
      arcpy.Delete_management(strFd,"FeatureDataset")
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Executed  arcpy.Delete_management(' + strFd + ',"FeatureDataset").'
      print ''
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print 'Did NOT execute arcpy.Delete_management(' + strFd + ',"FeatureDataset").'
      print ''
      return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def stripSchemaName(strObj):
# ---------------------------------------------------------------------------
  intPos = strObj.find('.')
  if (intPos >= 0):
    intPos = intPos + 1
    strObj = strObj[intPos:]
  return strObj
# ===========================================================================

# ===========================================================================
def sleepFunc(fltSleepSeconds):
# ---------------------------------------------------------------------------
  intReturnCode = 0
  try:
    import time
    print 'Sleeping for ' + str(fltSleepSeconds) + ' seconds...'
    timeStart = datetime.now()
    printTime(timeStart)
    time.sleep(fltSleepSeconds)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Slept    for ' + str(fltSleepSeconds) + ' seconds.'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT Sleeep for ' + str(fltSleepSeconds) + ' seconds.'
    print ''
    return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def copyFdList(strWorkspaceIn,strWorkspaceOut,strListFd):
# ---------------------------------------------------------------------------
  intReturnCode   = 0
  timeStart = datetime.now()
  printTime(timeStart)
  if (isinstance(strListFd, basestring)):
    strListFd     = strListFd.lower()
    if ((strListFd == 'all') or (strListFd == '*') or (strListFd == '')):
      lstFdName   = getListFeatureDatasets(strWorkspaceIn,'')
    else:
      lstFdName   = strListFd.split()
  else:
    lstFdName     = strListFd
  blnIsWorkspace  = isWorkspace(strWorkspaceOut)
  if (not blnIsWorkspace):
    intReturnCode = intReturnCode + createFileGdb(strWorkspaceOut)
    blnIsWorkspace  = isWorkspace(strWorkspaceOut)
    # The sleep below is required because subsequent describe does not
    # see the newly created object immediately upon creation.
    intReturnCode = intReturnCode + sleepFunc(1)
  if (blnIsWorkspace):
    blnIsFileGdb  = isFileGdb(strWorkspaceOut)
    for strFdName in lstFdName:
      strFdNameOut  = strFdName
      if (blnIsFileGdb):
        strFdNameOut  = stripSchemaName(strFdName)
      strFdIn       = os.path.abspath(os.path.join(strWorkspaceIn,strFdName))
      strFdOut      = os.path.abspath(os.path.join(strWorkspaceOut,strFdNameOut))

      intReturnCodeTmp = 1
      intAttempt       = 1
      intAttemptLimit  = 1
      while ((intAttempt <= intAttemptLimit) and (intReturnCodeTmp !=0)):    
        intReturnCode    = intReturnCode + deleteFd(strFdOut)
        intReturnCode    = intReturnCode + sleepFunc(3)
        intReturnCodeTmp = copyFd(strFdIn,strFdOut)
        if (intReturnCodeTmp != 0):
          intReturnCode  = intReturnCode + sleepFunc(3)
        intAttempt       = intAttempt + 1
      if (intReturnCodeTmp != 0):
        print 'Did NOT execute  arcpy.Copy_management(' + strFdIn + ',' + strFdOut + ', "FeatureDataset") in  ' + str(intAttemptLimit) + ' attempts.'
        print ''
        print ''
        intReturnCode    = intReturnCode  + intReturnCodeTmp
  else:
    intReturnCode = intReturnCode + 1
  timeEnd   = datetime.now()
  printTime(timeEnd)
  printTimeDelta(timeStart,timeEnd)
  print ''
  return intReturnCode
# ===========================================================================

# ===========================================================================
def copyFd(strFdIn,strFdOut):
# ---------------------------------------------------------------------------
  # This function copies a feature dataset from strFdIn to strFdOut.
  try:
    print 'Executing arcpy.Copy_management(' + strFdIn + ',' + strFdOut + ',"FeatureDataset")...'
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.Copy_management(strFdIn,strFdOut,"FeatureDataset")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  arcpy.Copy_management(' + strFdIn + ',' + strFdOut + ',"FeatureDataset").'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT execute arcpy.Copy_management(' + strFdIn + ',' + strFdOut + ',"FeatureDataset").'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def copyFc(strFcIn,strFcOut):
# ---------------------------------------------------------------------------
  # This function copies a featureclass from strFcIn to strFcOut.
  try:
    print 'Executing arcpy.CopyFeatures_management(' + strFcIn + ',' + strFcOut + ',"","0","0","0")...'
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.CopyFeatures_management(strFcIn,strFcOut,"","0","0","0")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  arcpy.CopyFeatures_management(' + strFcIn + ',' + strFcOut + ',"","0","0","0").'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT execute arcpy.CopyFeatures_management(' + strFcIn + ',' + strFcOut + ',"","0","0","0").'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def copyTbl(strTblGdbIn,strTblGdbOut):
# ---------------------------------------------------------------------------
  # This function copies a table from strTblGdbIn to strTblGdbOut.
  dirName = os.path.dirname(strTblGdbIn)
  tblName = os.path.basename(strTblGdbIn)
  print 'Copying table ' + strTblGdbIn + ' to ' + strTblGdbOut + '...'
  try:
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.Copy_management(strTblGdbIn, strTblGdbOut)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Copied table ' + strTblGdbIn + ' to ' + strTblGdbOut + '.'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT copy table ' + strTblGdbIn + ' to ' + strTblGdbOut + '.'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def createFileGdb(strWorkspace):
# ---------------------------------------------------------------------------
  intReturnCode      = 0
  try:
    strPathWorkspace   = os.path.dirname(strWorkspace)
    strNameWorkspace   = os.path.basename(strWorkspace)
    if (os.path.isdir(strPathWorkspace)):
      timeStart = datetime.now()
      printTime(timeStart)
      print 'Creating FileGDB ' + strWorkspace + '...'
      result = arcpy.CreateFileGDB_management(strPathWorkspace,strNameWorkspace)
      print 'Result = ' + str(result)
      print 'Created  FileGDB ' + strWorkspace + '.'
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print ''
    else:
      intReturnCode = intReturnCode + 1
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    intReturnCode = intReturnCode + 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def synchronizeGdbReplica(strFileConnFrom,strNameReplica,strFileConnTo):
# ---------------------------------------------------------------------------
  # This function synchronizes one GeoDataBase with another.
  try:
    print 'Synchronizing replica ' + strNameReplica + ' from ' + strFileConnFrom + ' to ' + strFileConnTo + ' ...'
    print 'Executing arcpy.SynchronizeChanges_management(' + strFileConnFrom + ',' + strNameReplica + ',' + strFileConnTo + ', "FROM_GEODATABASE1_TO_2", "IN_FAVOR_OF_GDB1", "BY_OBJECT", "DO_NOT_RECONCILE")'
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.SynchronizeChanges_management(strFileConnFrom, strNameReplica, strFileConnTo, "FROM_GEODATABASE1_TO_2", "IN_FAVOR_OF_GDB1", "BY_OBJECT", "DO_NOT_RECONCILE")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  arcpy.SynchronizeChanges_management(' + strFileConnFrom + ',' + strNameReplica + ',' + strFileConnTo + ', "FROM_GEODATABASE1_TO_2", "IN_FAVOR_OF_GDB1", "BY_OBJECT", "DO_NOT_RECONCILE")'
    print 'Synchronized  replica ' + strNameReplica + ' from ' + strFileConnFrom + ' to ' + strFileConnTo + ' .'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT synchronize replica ' + strNameReplica + ' from ' + strFileConnFrom + ' to ' + strFileConnTo + ' .'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def reconcileVersionNewThread(strFileConn,strVersion):
# ---------------------------------------------------------------------------
  intReturnCode       = 0
  try:
    import thread
    intReturnCodeNewThread = thread.start_new_thread(reconcileVersion,(strFileConn,strVersion))
    print 'ReturnCode from reconcileVersion(' + strVersion + ') = ' + str(intReturnCodeNewThread)
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT reconcile version ' + strVersion + '.'
    print ''
    return 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def reconcileVersion(strFileConn,strVersion):
# ---------------------------------------------------------------------------
  try:
    print 'Reconciling version ' + strVersion + ' in ' + strFileConn + ' ...'
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.ReconcileVersion_management(strFileConn, strVersion, "SDE.DEFAULT", "BY_OBJECT", "FAVOR_TARGET_VERSION", "NO_LOCK_AQUIRED", "ABORT_CONFLICTS", "NO_POST")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Reconciled  version ' + strVersion + ' in ' + strFileConn + ' ...'
    print ''
    return 0
  except Exception as e:
    strEArgs = str(e.args)
    intPos = strEArgs.find('ERROR 000084: Conflicts detected')
    if (intPos >= 0):
      print 'Conflict detected under version: ' + strVersion + " " + e.message.encode('utf-8').strip() + "\n"
    else:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT reconcile version ' + strVersion + ' in ' + strFileConn + ' ...'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def compressGdb(connGdb,blnTargetIsSDE):
# ---------------------------------------------------------------------------
  try:
    timeStart = datetime.now()
    printTime(timeStart)
    if (blnTargetIsSDE):
      print 'Executing arcpy.Compress_management(' + connGdb + ')...'
      arcpy.Compress_management(connGdb)
      print 'Executed  arcpy.Compress_management(' + connGdb + ').'
    else:
      print 'Executing arcpy.CompressFileGeodatabaseData_management(' + connGdb + ')...'
      #arcpy.CompressFileGeodatabaseData_management(connGdb)
      print 'Executed  arcpy.CompressFileGeodatabaseData_management(' + connGdb + ').'
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Compressed ' + connGdb + '.'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT compress ' + connGdb
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def analyzeTbl(strTblGdb):
# ---------------------------------------------------------------------------
  # This function analyzes (updates DBMS statistics) on tables
  try:
    print 'Analyzing (updating stats) on ' + strTblGdb + '...'
    timeStart = datetime.now()
    printTime(timeStart)
    #arcpy.Analyze_management(strTblGdb,"BUSINESS;ADDS;DELETES;FEATURES")
    arcpy.Analyze_management(strTblGdb, "BUSINESS;FEATURE;ADDS;DELETES")
    #arcpy.Analyze_management(strTblGdb)
    #arcpy.Analyze_management(strTblGdb,"")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Analyzed  (updated  stats) on ' + strTblGdb + '.'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT analyze ' + strTblGdb + '.'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def deleteGdbVersion(strWorkspace, strVersionName, blnDeleteDefaultVersion):
# ---------------------------------------------------------------------------
  # This function deletes database versions.
  # Pass strVersionName argument 'delete_all_versions' to delete all versions.
  intReturnCode = 0
  print ' '

  if (strWorkspace != '') and (strWorkspace != '#'):
    intReturnCode = intReturnCode + setWorkspace(strWorkspace)

  if (strVersionName == 'delete_all_versions') and (intReturnCode == 0):
    print 'Creating list of all versions in workspace ' + strWorkspace + '...'
    timeStart = datetime.now()
    printTime(timeStart)
    listStrVersionName = arcpy.ListVersions(strWorkspace)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Created  list of all versions in workspace ' + strWorkspace + '.'
  else:
    listStrVersionName = []
    listStrVersionName.append(strVersionName)

  if (blnDeleteDefaultVersion == False) and (intReturnCode == 0):
    #print str(listStrVersionName)
    #print str(len(listStrVersionName))
    strDefaultVersionName = 'SDE.DEFAULT'
    if (len(listStrVersionName) > 0):
      listStrVersionName.remove(strDefaultVersionName)

  if (intReturnCode == 0):
    for strVersionName in listStrVersionName:
      try:
        print ' '
        print 'Deleting version ' + strVersionName + '...'
        timeStart = datetime.now()
        printTime(timeStart)
        arcpy.DeleteVersion_management(strWorkspace, strVersionName)
        timeEnd   = datetime.now()
        printTime(timeEnd)
        printTimeDelta(timeStart,timeEnd)
        print 'Deleted  version ' + strVersionName + '.'
      except Exception as e:
        print ' '
        print "ERROR: Exception:", sys.exc_info()[0]
        print 'Exception = ' + str(e.args)
        printTimeNow()
        print 'Did NOT Delete version ' + strVersionName + '.'
        print ''
        intReturnCode = intReturnCode + 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def registerAsVersioned(strTblGdb):
# ---------------------------------------------------------------------------
  # This function registers a table as versioned.
  try:
    print 'Registering ' + strTblGdb + ' as versioned...'
    timeStart = datetime.now()
    printTime(timeStart)
    result = arcpy.RegisterAsVersioned_management(strTblGdb, "EDITS_TO_BASE")
    arcpy.RegisterAsVersioned_management(strTblGdb, "EDITS_TO_BASE")
    timeEnd   = datetime.now()
    #print 'Result = ' + result.GetOutput(0)
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Registered  ' + strTblGdb + ' as versioned...'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT Register  ' + strTblGdb + ' as versioned...'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def unregisterAsVersioned(strObjGdb):
# ---------------------------------------------------------------------------
  # This function unregisters a GDB Object as versioned.
  objDescribe = describe(strObjGdb)
  if (not isinstance(objDescribe, basestring)):
    if (objDescribe.canVersion):
      try:
        if (objDescribe.isVersioned):
          print 'Unregistering GDB Object ' + strObjGdb + ' as versioned...'
          timeStart = datetime.now()
          printTime(timeStart)
          arcpy.UnregisterAsVersioned_management(strObjGdb, "NO_KEEP_EDIT", "COMPRESS_DEFAULT")
          timeEnd   = datetime.now()
          printTime(timeEnd)
          printTimeDelta(timeStart,timeEnd)
          print 'Unregistered  GDB Object ' + strObjGdb + ' as versioned...'
          print ''
        else:
          print '              GDB Object ' + strObjGdb + ' is not versioned.'
          print ''
        return 0
      except Exception as e:
        print "ERROR: Exception:", sys.exc_info()[0]
        print 'Exception = ' + str(e.args)
        printTimeNow()
        print 'Did NOT Unregister  ' + strObjGdb + ' as versioned...'
        print ''
        return 1
    else:
      print 'Object ' + strObjGdb + ' can not be versioned.'
      print ''
      return 0
  return 0
# ===========================================================================

# ===========================================================================
def sdemonPauseResumeNewThread(strPauseResume,strSleep,strDbName,strUsername,strPassword):
# ---------------------------------------------------------------------------
  intReturnCode       = 0
  import thread
  intReturnCodeNewThread = thread.start_new_thread(sdemonPauseResume,(strPauseResume,strSleep,strDbName,strUsername,strPassword))
  print 'ReturnCode from sdemon -o pause thread = ' + str(intReturnCodeNewThread)
  return intReturnCode
# ===========================================================================

# ===========================================================================
def sdemonPauseResume(strPauseResume,strSleep,strDbName,strUsername,strPassword):
# ---------------------------------------------------------------------------
  # The argument strPauseResume can be these values:
  # 'pause','resume','status'

  #Example Commands
  #sdemon -o pause  -i sde:oracle11g -u sde -p xxx@edgis1p
  #sdemon -o resume -i sde:oracle11g -u sde -p xxx@edgis1p
  #sdemon -o status -i sde:oracle11g -u sde -p xxx@edgis1p

  intReturnCode       = 0

  strUsernameDefault = 'sde'
  fltSleepDefault    = 0.0

  if (strUsername    == ''):
    strUsername      = strUsernameDefault
  strDbName          = getDbName(strDbName)
  if (strPassword    == ''):
    import getPassword
    strPassword      = getPassword.getPassword(strDbName,strUsername)

  if (isinstance(strSleep, basestring)):
    if (strSleep    == ''):
      strSleep      = fltSleepDefault
    fltSleep        = float(strSleep)
  else:
    fltSleep        = strSleep
    strSleep        = str(fltSleep)

  if (fltSleep > 0):
    intReturnCode = intReturnCode + sleepFunc(fltSleep)

  strCommand      = 'sdemon -o '    + strPauseResume + ' -i sde:oracle11g'
  strCommand      = strCommand      + ' -u ' + strUsername
  strCommand      = strCommand      + ' -p ' + strPassword
  strCommand      = strCommand      + '@'    + strDbName

  strCommandPrint = 'sdemon -o '    + strPauseResume + ' -i sde:oracle11g'
  strCommandPrint = strCommandPrint + ' -u ' + strUsername
  strCommandPrint = strCommandPrint + ' -p ' + '*********'
  strCommandPrint = strCommandPrint + '@'    + strDbName

  try:
    print 'Executing ' + strCommandPrint + '...'
    #if (strPauseResume != 'pause'): # Uncomment for testing
    intReturnCode = intReturnCode + Execute(strCommand,True)
    print 'Executed  ' + strCommandPrint + '.'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    return intReturnCode
  return intReturnCode
# ===========================================================================

# ===========================================================================
def dbKillConnectionsDbType(strDbType):
# ---------------------------------------------------------------------------
  intReturnCode   = 0
  import getPassword
  strDbName     = getDbName(strDbType)
  strUsername   = 'sde'
  strPassword   = getPassword.getPassword(strDbName,strUsername)
  strFileConn   = createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + dbKillConnections(strFileConn)
  intReturnCode = intReturnCode + osRemove(strFileConn)
  return intReturnCode
# ===========================================================================

# ===========================================================================
def dbKillConnections(strFileConn):
# ---------------------------------------------------------------------------
  # This function will kill connections in a target database
  # where the stored procedure has been stored.
  intReturnCode = 0
  strSql = "call sys.kill_process()"
  try:
    print 'Executing intReturnCode = sdeSqlExecute(' + strFileConn + ',' + strSql + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    intReturnCode = intReturnCode + sdeSqlExecute(strFileConn,strSql)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  intReturnCode = sdeSqlExecute(' + strFileConn + ',' + strSql + ').'
    print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute  intReturnCode = sdeSqlExecute(' + strFileConn + ',' + strSql + ').'
    print ''
    return 1
  return 0
# ===========================================================================

# ===========================================================================
def gdbTruncateDynamicTablesDbType(strDbType):
# ---------------------------------------------------------------------------
  intReturnCode   = 0
  import getPassword
  strDbName     = getDbName(strDbType)
  strUsername   = 'sde'
  strPassword   = getPassword.getPassword(strDbName,strUsername)
  strFileConn   = createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + gdbTruncateDynamicTables(strFileConn)
  intReturnCode = intReturnCode + osRemove(strFileConn)
  return intReturnCode
# ===========================================================================

# ===========================================================================
def gdbTruncateDynamicTables(strFileConn):
# ---------------------------------------------------------------------------
  # This function will truncate dynamic SDE tables in a target SDE database.
  intReturnCode   = 0
  lstStrTableName = ['sde.sde_logfile_data','sde.state_locks','sde.table_locks','sde.object_locks','sde.layer_locks','sde.process_information','edgis.sde_logfile_data']
  lstStrTableName = ['sde.sde_logfile_data','sde.state_locks','sde.table_locks','sde.object_locks','sde.layer_locks','sde.process_information']
  try:
    for strTableName in lstStrTableName:
      strSql = 'truncate table ' + strTableName
      print 'Executing intReturnCode = intReturnCode + sdeSqlExecute(' + strFileConn + ',' + strSql + ')...'
      timeStart = datetime.now()
      printTime(timeStart)
      intReturnCode = intReturnCode + sdeSqlExecute(strFileConn,strSql)
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Executed  intReturnCode = intReturnCode + sdeSqlExecute(' + strFileConn + ',' + strSql + ').'
      print ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print ''
    intReturnCode = intReturnCode + 1
  return intReturnCode
# ===========================================================================

# ===========================================================================
def RemoveDuplicateRows(strFileConnMetrics,strTableName,strColName01,strColName02,strColName03,strColName04,strColName05):
# ---------------------------------------------------------------------------
  # This function removes duplicate rows in an Oracle table.
  # You must pass at least 1 column name.
  # You may pass additional column names for determining duplicate data.
  # Pass a blank string for unused column name arguments.
  intReturnCode            = 0

  try:
    print 'Executing remove duplicates...'
    strSql    = 'delete from ' + strTableName + ' where rowid not in '
    strSql    = strSql + '(select min(rowid) from ' + strTableName + ' '
    strSql    = strSql + 'group by ' + strColName01
    if (strColName02 != ''):
      strSql  = strSql + ',' + strColName02
    if (strColName03 != ''):
      strSql  = strSql + ',' + strColName03
    if (strColName04 != ''):
      strSql  = strSql + ',' + strColName04
    if (strColName05 != ''):
      strSql  = strSql + ',' + strColName05
    strSql    = strSql + ')'
    intReturnCode = intReturnCode + sdeSqlExecute(strFileConnMetrics,strSql)
    print 'Executed  remove duplicates.'
    print ''

  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    intReturnCode = intReturnCode + 1
    return intReturnCode

  return intReturnCode
# ===========================================================================

# ===========================================================================
def sdeSqlExecuteQuiet(strFileConn,strSql):
# ---------------------------------------------------------------------------
  try:
    #print 'Executing sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    #timeStart = datetime.now()
    #printTime(timeStart)
    sdeConn = arcpy.ArcSDESQLExecute(strFileConn)
    #timeEnd   = datetime.now()
    #printTime(timeEnd)
    #printTimeDelta(timeStart,timeEnd)
    #print 'Executed  sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    #print ''

    #print 'Executing  sdeConn.execute(' + strSql + ')...'
    #timeStart = datetime.now()
    #printTime(timeStart)
    sdeConn.execute(strSql)
    #timeEnd   = datetime.now()
    #print 'Executed   sdeConn.execute(' + strSql + ').'
    #printTime(timeEnd)
    #printTimeDelta(timeStart,timeEnd)
    #print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute  ' + strSql + ' in connection ' + strFileConn + '...'
    print 'Did NOT successfully execute sdeConn.execute(' + strSql + ').'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def sdeSqlExecute(strFileConn,strSql):
# ---------------------------------------------------------------------------
  try:
    print 'Executing sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    sdeConn = arcpy.ArcSDESQLExecute(strFileConn)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    print ''

    print 'Executing  sdeConn.execute(' + strSql + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    sdeConn.execute(strSql)
    timeEnd   = datetime.now()
    print 'Executed   sdeConn.execute(' + strSql + ').'
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute  ' + strSql + ' in connection ' + strFileConn + '...'
    print 'Did NOT successfully execute sdeConn.execute(' + strSql + ').'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def sdeSqlExecuteReturn(strFileConn,strSql,blnReturnAsString):
# ---------------------------------------------------------------------------
  # This function returns the SQL results of strSql.
  try:
    print 'Executing sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    sdeConn   = arcpy.ArcSDESQLExecute(strFileConn)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    print ''

    print 'Executing sdeReturn = sdeConn.execute(' + strSql + ')...'
    timeStart = datetime.now()
    printTime(timeStart)
    sdeReturn = sdeConn.execute(strSql)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Executed  sdeReturn = sdeConn.execute(' + strSql + ').'
    print 'sdeReturn = ' + str(sdeReturn)
    print ''
    if (blnReturnAsString):
      return str(sdeReturn)
    else:
      return sdeReturn
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute  sdeReturn = sdeConn.execute(' + strSql + ').'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def sdeSqlExecuteReturnQuiet(strFileConn,strSql,blnReturnAsString):
# ---------------------------------------------------------------------------
  # This function returns the SQL results of strSql.
  try:
    #print 'Executing sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    timeStart = datetime.now()
    #printTime(timeStart)
    sdeConn   = arcpy.ArcSDESQLExecute(strFileConn)
    timeEnd   = datetime.now()
    #printTime(timeEnd)
    #printTimeDelta(timeStart,timeEnd)
    #print 'Executed  sdeConn = arcpy.ArcSDESQLExecute(' + strFileConn + ')...'
    #print ''

    #print 'Executing sdeReturn = sdeConn.execute(' + strSql + ')...'
    timeStart = datetime.now()
    #printTime(timeStart)
    sdeReturn = sdeConn.execute(strSql)
    timeEnd   = datetime.now()
    #printTime(timeEnd)
    #printTimeDelta(timeStart,timeEnd)
    #print 'Executed  sdeReturn = sdeConn.execute(' + strSql + ').'
    #print 'sdeReturn = ' + str(sdeReturn)
    #print ''
    if (blnReturnAsString):
      return str(sdeReturn)
    else:
      return sdeReturn
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT successfully execute  sdeReturn = sdeConn.execute(' + strSql + ').'
    print ''
    return '1'
# ===========================================================================

# ===========================================================================
def sqlPlusExecute(strUsername,strPassword,strTnsName,strSql):
# ---------------------------------------------------------------------------
  from subprocess import Popen, PIPE

  (username,password,tnsname) = (strUsername,strPassword,strTnsName) 
  strConnect = " %s/%s@%s "% (username,password,tnsname)

  print 'strConnect = ' + strConnect

  try:
    session = Popen(['sqlplus', '-S', strConnect], stdin=PIPE, stdout=PIPE, stderr=PIPE)
    session.stdin.write(strSql)
    return session.communicate()
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    return 1
# ===========================================================================

# ===========================================================================
def addSpatialIndex(strFcName, strWorkspace):
# ---------------------------------------------------------------------------
  intReturnCode = 0
  if (strWorkspace != '') and (strWorkspace != '#'):
    intReturnCode = intReturnCode + setWorkspace(strWorkspace)

  if (intReturnCode == 0):
    try:
      print 'Adding spatial index on featureclass' + strFcName + '...'
      timeStart = datetime.now()
      printTime(timeStart)
      arcpy.AddSpatialIndex_management(strFcName, "0","0","0")
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
      print 'Added  spatial index on featureclass' + strFcName + '.'
      print ''
      return 0
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      print 'Did NOT add spatial index on featureclass' + strFcName + '.'
      print ''
      return 1
# ===========================================================================

# ===========================================================================
def addIndex(strTbl, strColName, strIndexName):
# ---------------------------------------------------------------------------
  print 'Adding index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
  try:
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.AddIndex_management(strTbl, strColName, strIndexName, "NON_UNIQUE", "ASCENDING")
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Added  index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT add index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def dropIndex(strTbl, strColName, strIndexName):
# ---------------------------------------------------------------------------
  print 'Dropping index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
  try:
    timeStart = datetime.now()
    printTime(timeStart)
    arcpy.RemoveIndex_management(strTbl,strIndexName)
    timeEnd   = datetime.now()
    printTime(timeEnd)
    printTimeDelta(timeStart,timeEnd)
    print 'Dropped  index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
    print ''
    return 0
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    printTimeNow()
    print 'Did NOT drop  index ' + strIndexName + ' on column ' + strColName + ' on table ' + strTbl + ' ...'
    print ''
    return 1
# ===========================================================================

# ===========================================================================
def stopStartAgsService(server, port, adminUser, adminPass, stopStart, serviceList, token=None):  
# ---------------------------------------------------------------------------
  # This function stops, starts or deletes an AGS service.
  # Requires Admin user/password, as well as server and port
  #   (necessary to construct token if one does not exist).
  # stopStart = Stop|Start|Delete
  # serviceList = List of services. A service must be in the <name>.<type> notation
  # If a token exists, you can pass one in for use.  
  
  intReturnCode = 0
  # Moved this import into this function, because it is only required
  # for this function.
  # I think this import requires that the current user is authorized
  # to for an arcinfo license.
  #print 'Importing arcinfo module  into ' + strFileScript + '...'
  #import arcpy
  #print 'Imported  arcinfo module...'
  #print ''
  # Get and set the token
  if token is None:       
    token = gentoken(server, port, adminUser, adminPass)
  
  # Getting services from tool validation creates a semicolon delimited list that needs to be broken up
  services = serviceList.split(';')
  
  #modify the services(s)    
  for service in services:        
    try:
      timeStart = datetime.now()
      printTime(timeStart)
      service = urllib.quote(service.encode('utf8'))
      op_service_url = "http://{}:{}/arcgis/admin/services/{}/{}?token={}&f=json".format(server, port, service, stopStart, token)        
      status = urllib2.urlopen(op_service_url, ' ').read()
    
      if 'success' in status:
        arcpy.AddMessage(str(service) + " === " + str(stopStart))
      else:
        arcpy.AddWarning(status)
        intReturnCode = intReturnCode + 1
      timeEnd   = datetime.now()
      printTime(timeEnd)
      printTimeDelta(timeStart,timeEnd)
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'Exception = ' + str(e.args)
      printTimeNow()
      intReturnCode = intReturnCode + 1

  return intReturnCode
# ===========================================================================

# ===========================================================================
def getDbName(strDbType):
# ---------------------------------------------------------------------------
  # These getDbName functions should be put in a new script getDbName.py.
  strDbName          =  strDbType
  strDbType          = strDbType.lower()
  if (strDbType      == 'edmaint'):
    strDbName        = getDbNameEdMaint('')
  if (strDbType      == 'edbatch'):
    strDbName        = getDbNameEdBatch('')
  if (strDbType      == 'edbatchdc1'):
    strDbName        = getDbNameEdBatchDc1('')
  if (strDbType      == 'edbatchdc2'):
    strDbName        = getDbNameEdBatchDc2('')
  if (strDbType      == 'edpub'):
    strDbName        = getDbNameEdPub('')
  if (strDbType      == 'edpubdc1'):
    strDbName        = getDbNameEdPubDc1('')
  if (strDbType      == 'edpubdc2'):
    strDbName        = getDbNameEdPubDc2('')
  if (strDbType      == 'redline'):
    strDbName        = getDbNameRedline('')
  if (strDbType      == 'lbmaint'):
    strDbName        = getDbNameLbMaint('')
  if (strDbType      == 'lbpub'):
    strDbName        = getDbNameLbPub('')
  if (strDbType      == 'lbpubdc1'):
    strDbName        = getDbNameLbPubDc1('')
  if (strDbType      == 'lbpubdc2'):
    strDbName        = getDbNameLbPubDc2('')
  if (strDbType      == 'edschm'):
    strDbName        = getDbNameSchmaint('')
  if (strDbType      == 'edsub'):
    strDbName        = getDbNameEdSub('')
  if (strDbType      == 'edsubbatch'):
    strDbName        = getDbNameEdSubBatch('')
  if (strDbType      == 'edsubbatchdc1'):
    strDbName        = getDbNameEdSubBatchDc1('')
  if (strDbType      == 'edsubbatchdc2'):
    strDbName        = getDbNameEdSubBatchDc2('')
  if (strDbType      == 'edsubpub'):
    strDbName        = getDbNameEdSubPub('')
  if (strDbType      == 'edsubpubdc1'):
    strDbName        = getDbNameEdSubPubDc1('')
  if (strDbType      == 'edsubpubdc2'):
    strDbName        = getDbNameEdSubPubDc2('')
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdMaint(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgist2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edgis1q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edgis1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgis1pdg'
  return strDbName
# ===========================================================================


# ===========================================================================
def getDbNameSchmaint(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname  == 'edgisbtcqa04'):
    strDbName        = 'edscmp1t'
  elif (strHostname  in ['edgisbtcprd05','edgisbtcprd06']):
    strDbName        = 'edscmm1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edscmm1p'
  return strDbName
# ===========================================================================



# ===========================================================================
def getDbNameEdPub(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgist2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edgisp1q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edgisp1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgisp3p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdPubDc1(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdPub(strHostname)
  if (strHostname    in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgisp1p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdPubDc2(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdPubDc1(strHostname)
  if (strHostname    in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edgisp3p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdBatch(strHostname):
# ---------------------------------------------------------------------------
  # The machine names and database names need to be set below.
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgisp2p'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edgisp2p'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edgisp2p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgisp4p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdBatchDc1(strHostname):
# ---------------------------------------------------------------------------
  # The machine names and database names need to be set below.
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdBatch(strHostname)
  if   (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgisp2p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdBatchDc2(strHostname):
# ---------------------------------------------------------------------------
  # The machine names and database names need to be set below.
  strDbName        = 'edgisp2p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSub(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgist2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edsub1q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edsub1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edsub1pdg'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubBatch(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgist2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edsubp1q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edsubp2p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edsubp4p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubBatchDc1(strHostname):
# ---------------------------------------------------------------------------
  # The machine names and database names need to be set below.
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdBatch(strHostname)
  if   (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edsubp2p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubBatchDc2(strHostname):
# ---------------------------------------------------------------------------
  # The machine names and database names need to be set below.
  strDbName        = 'edsubp4p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubPub(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgist2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edsubp1q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edsubp1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edsubp3p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubPubDc1(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdPub(strHostname)
  if (strHostname    in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edsubp1p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameEdSubPubDc2(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  strDbName          = getDbNameEdPubDc1(strHostname)
  if (strHostname    in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edsubp3p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameRedline(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'edgisw2q'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'edgisw2q'
  elif (strHostname  in ['edgisbtcprd01','edgisbtcprd03']):
    strDbName        = 'edgisw1p'
  elif (strHostname  in ['edgisbtcprd02','edgisbtcprd04']):
    strDbName        = 'edgisw1pdg'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameLbMaint(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'lbgism1p'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'lbgism1p'
  elif (strHostname  in ['lbgisappprd01','lbgisappprd03']):
    strDbName        = 'lbgism1p'
  elif (strHostname  in ['lbgisappprd02','lbgisappprd04']):
    strDbName        = 'lbgism1pdg'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameLbPub(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'lbgis1p'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'lbgis1p'
  elif (strHostname  in ['lbgisappprd01','lbgisappprd03']):
    strDbName        = 'lbgis1p'
  elif (strHostname  in ['lbgisappprd02','lbgisappprd04']):
    strDbName        = 'lbgis2p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameLbPubDc1(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'lbgis1p'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'lbgis1p'
  elif (strHostname  in ['lbgisappprd01','lbgisappprd03']):
    strDbName        = 'lbgis1p'
  elif (strHostname  in ['lbgisappprd02','lbgisappprd04']):
    strDbName        = 'lbgis1p'
  return strDbName
# ===========================================================================

# ===========================================================================
def getDbNameLbPubDc2(strHostname):
# ---------------------------------------------------------------------------
  strDbName          =  ''
  if   (strHostname  == ''):
    strHostname      = getHostname()
  if (strHostname    == 'edgisappqa01'):
    strDbName        = 'lbgis2p'
  elif (strHostname  == 'eobtcgisqa01'):
    strDbName        = 'lbgis2p'
  elif (strHostname  in ['lbgisappprd01','lbgisappprd03']):
    strDbName        = 'lbgis2p'
  elif (strHostname  in ['lbgisappprd02','lbgisappprd04']):
    strDbName        = 'lbgis2p'
  return strDbName
# ===========================================================================

#============================================================================
if (__name__ == "__main__"):
# ---------------------------------------------------------------------------
  intReturnCode = main()
  sys.exit(intReturnCode)
  # Note that the returned error code from this script propagates to a
  # calling .bat script only when the ERRORLEVEL variable has not previously
  # been set in the calling window.
# ===========================================================================
