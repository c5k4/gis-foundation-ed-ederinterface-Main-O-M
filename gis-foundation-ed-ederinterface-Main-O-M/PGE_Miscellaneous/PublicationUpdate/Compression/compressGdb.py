# ===========================================================================
# compressGdb.py
# ---------------------------------------------------------------------------
# 
# This script performs the ESRI ArcGIS Server function
# compress on GeoDataBases.
#
# Open a command window.
#
# Delete connections on the target database prior to running this script
# to ensure the most complete compress.
# 
# Usage:  c:\python27\arcgis10.8\python    compressGdb.py fileConn DbName
#         <path_to_python.exe>             compressGdb.py fileConn DbName
#         d:\python27\arcgis10.8\python    compressGdb.py fileConn DbName
#         d:\python27\python               compressGdb.py fileConn DbName
#         d:\python27\arcgisx6410.8\python compressGdb.py fileConn DbName
#         
#         The path to python.exe might be different on your computer.
#
#         If you add the path to python.exe to your system path variable,
#         you don't need to include it on the command line.
# 
# Author: Vedant Sood (V3SF)
# ===========================================================================
# Import system modules...
import sys, os, string
import subprocess
import arcpy
# ===========================================================================
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
  strFileConn = Config.get('DB_Conn', 'workspace')
  
  mycmd = r'D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe'
  myarg = "GETSDEFILE" + " " + strFileConn
  p = subprocess.Popen(mycmd + " " + myarg, stdout=subprocess.PIPE, shell=True)

  (output, err) = p.communicate()
 
  ## Wait for EXE to terminate. Get return returncode ##
  p_status = p.wait()

  
  print 'SDE Path: ' + output

  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # print Running the compress...
  # -------------------------------------------------------------------------
  intReturnCode = intReturnCode + compressGdb(output,True)
  
  timeEnd         = datetime.now()
  printTime(timeEnd)
  printTimeDelta(timeStart,timeEnd)
  return intReturnCode
  # ===========================================================================

def compressGdb(connGdb,blnTargetIsSDE):
# ---------------------------------------------------------------------------
  from datetime import datetime

  try:
    timeStart = datetime.now()
    printTime(timeStart)
    connstr = "C:\\GitHub\\Publication_DB_Update\\3_Deleting _Geometric_Network\\conn_gdb\\edgep1d_10_8_1_oracle_19c_dc_sde_2021_04_14_10_50_09_440.sde"
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
    timenow = datetime.now()
    printTime(timenow)
    print 'Did NOT compress ' + connGdb
    print ''
    return 1
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
