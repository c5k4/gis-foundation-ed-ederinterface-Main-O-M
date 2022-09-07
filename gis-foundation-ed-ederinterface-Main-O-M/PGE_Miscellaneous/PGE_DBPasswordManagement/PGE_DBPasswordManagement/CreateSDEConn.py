import sys, os, string
import arcpy
from   datetime import datetime
from arcpy import env

def main():
# ---------------------------------------------------------------------------

  strFileScript    = os.path.abspath(__file__)
  print  'Imported sys, os, string, arcpy into ' + strFileScript + '.'
  print  ''

  try:
    strDbName     = getArg(1) #'edgep1d'
    strUsername   = getArg(2) #'sde'
    strPassword   = getArg(3) #'password'
    strFileConn   = getArg(4) #'path'	
    strFileConn   = createArcSDEConnectionFileBasic(strFileConn,strDbName,strUsername,strPassword)
  except Exception as e:
      print  "ERROR: Exception:", sys.exc_info()[0]
      print  'Exception = ' + str(e.args)
      printTimeNow()
      print  ''
      return 1

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
  #strDbSoftwareVersion = '11g'
  strDbSoftwareVersion = '19c'
  print  strPassword
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
    print  strService

  if (strFileConn == ''):
    strFileConn   = createArcSDEConnectionFileString(strDbName,strDbSoftware,strDbSoftwareVersion,strUsername,True)

  strPathConn     = os.path.dirname(strFileConn)
  strFilenameConn = os.path.basename(strFileConn)

  # Check for the .sde file and delete it if present
  arcpy.env.overwriteOutput = True

  print  strPathConn
  print  strFilenameConn
  print  strFileConn
  print  strServerName
  print  strService
  print  strAccountAuth
  print  'strDbName   = ' + strDbName
  print  'strUsername = ' + strUsername
  print  'strPassword = '
  print  strSaveUserInfo
  print  strGdbVersion
  print  strSaveVerInfo

  try:
    print  'Creating ArcSDE Connection file ' + strFileConn + '...'
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
    print strFileConn
    print  ''
  except Exception as e:
    print "ERROR: Exception:", sys.exc_info()[0]
    print 'Exception = ' + str(e.args)
    print 'Did NOT Create   ArcSDE Connection file ' + strFileConn + '.'

  return strFileConn
# ===========================================================================

# ===========================================================================
def createArcSDEConnectionFileStringBasic(strDbName,strUsername,blnTimeStamp):
# ---------------------------------------------------------------------------
  strFileConn          = ''
  strDbSoftware        = 'oracle'
  #strDbSoftwareVersion = '11g'
  strDbSoftwareVersion = '19c'
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
      print  'Creating directory ' + strPathConn + '...'
      os.mkdir(strPathConn)
      print  'Created  directory ' + strPathConn + '.'
    except Exception as e:
      print  "ERROR: Exception:", sys.exc_info()[0]
      print  'Exception = ' + str(e.args)
      print  'Did NOT Create directory ' + strPathConn + '.'
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
def getHostname():
# ---------------------------------------------------------------------------
  import socket
  strHostname = socket.gethostname().lower()
  return strHostname
# ===========================================================================

# ===========================================================================
def printTimeNow():
#  ---------------------------------------------------------------------------
  strTime       = getStrTimeNow()
  print  'Time:\t\t' + strTime
# ===========================================================================

# ===========================================================================
def printTime(dateTimeIn):
# ---------------------------------------------------------------------------
  strTime       = getStrTime(dateTimeIn)
  print  'Time:\t\t' + strTime
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
def printTimeDelta(timeStart,timeEnd):
# ---------------------------------------------------------------------------
  timeDeltaElapsed = (timeEnd - timeStart)
  strTimeDelta     = timeDeltaToString(timeDeltaElapsed)
  print  'Elapsed:\t' + strTimeDelta
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
def getArg(intArg):
# ---------------------------------------------------------------------------
  from datetime import datetime
  strArg = ''
  if (intArg == ''):
    intArg = 1
  try:
    #print  'Testing for and getting argument ' + str(intArg) + '...'
    listArg = sys.argv[1:]
  except Exception as e:
    print  "ERROR: Exception:", sys.exc_info()[0]
    print  'Exception = ' + str(e.args)
    printTimeNow()
    print  'Error in reading arguments.'
    return strArg
  
  try:
    i = 1
    if (len(listArg) > 0):
      for strArgTmp in listArg:
        if (i == intArg):
          strArg = sys.argv[intArg]
          #print  'Read into argument ' + str(intArg) + ': "' + strArgTmp + '".'
          break
        i = i + 1
    else:
      print  'No arguments found.'
  except Exception as e:
    print  "ERROR: Exception:", sys.exc_info()[0]
    print  'Exception = ' + str(e.args)
    print  'Error in processing arguments.'
    print  'Returning "' + strArg + '"...'
    printTimeNow()
    return strArg
  
  #print  'Returning "' + strArg + '"...'
  return strArg
# ===========================================================================


#============================================================================
if (__name__ == "__main__"):
# ---------------------------------------------------------------------------
  intReturnCode = main()
  sys.exit(intReturnCode)
# ===========================================================================
