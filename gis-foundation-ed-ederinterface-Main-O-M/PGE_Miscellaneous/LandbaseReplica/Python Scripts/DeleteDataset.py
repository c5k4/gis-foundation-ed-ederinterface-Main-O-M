import sys, os, string
import arcpy
import subprocess

def main():
# ---------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # Import Modules
  # -------------------------------------------------------------------------
  strFileScript    = os.path.abspath(__file__)
  print 'Imported  system modules into ' + strFileScript + '.'
  print ''

  from datetime import datetime
  import ConfigParser
  Config = ConfigParser.ConfigParser()
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting variables...'
  # -------------------------------------------------------------------------
  intReturnCode = 0
  
  timeStart     = datetime.now()
  printTime(timeStart)

  strConfigfile     = getArg(1)
  
  Config.read(strConfigfile)
  
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting variables for sdemon -o pause / resume...'
  # -------------------------------------------------------------------------
  blnPause               = True
  fltSleepSecondsPause   = 3.0
  fltSleepSecondsResume  = 5.0
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Reading config for connection file...'
  # -------------------------------------------------------------------------
  strFileConn = Config.get('Replica_Config', 'targetFileConn')
  
  print 'SDE Path: ' + strFileConn
    
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # print Running the Delete DataSet/Feature Class...
  # -------------------------------------------------------------------------
  import arcpy
  try:
    arcpy.env.workspace = strFileConn
    dataSets = Config.get('Replica_Config', 'in_data')
    dataSetType = Config.get('Replica_Config', 'data_type')
	
    print 'DataSets / Feature Class: ' + dataSets
    data_Sets = dataSets.split(",")
    for dataSet in data_Sets:
      print strFileConn + '\\' + dataSet
      arcpy.Delete_management((strFileConn + '\\' + dataSet),dataSetType)
      print dataSet + ' Deleted'
  except Exception, ex:
          print "Error in Deleting DataSets / Feature Class " + dataSets + " Exception " + str(ex)
          intReturnCode = 1
                
  timeEnd         = datetime.now()
  printTime(timeEnd)
  printTimeDelta(timeStart,timeEnd)
  #print 'ReturnCode from sdemon -o pause thread = ' + str(intReturnCodeNewThread)
  return intReturnCode
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
def getArg(intArg):
# ---------------------------------------------------------------------------
  from datetime import datetime
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

#============================================================================
if (__name__ == "__main__"):
# ---------------------------------------------------------------------------
  intReturnCode = main()
  sys.exit(intReturnCode)
# ===========================================================================




			

