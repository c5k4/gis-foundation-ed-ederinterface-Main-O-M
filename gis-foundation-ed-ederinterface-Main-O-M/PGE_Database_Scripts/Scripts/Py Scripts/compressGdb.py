# ===========================================================================
# compressGdb.py
# ---------------------------------------------------------------------------
# 
# This script performs the ESRI ArcGIS Server function
# compress on GeoDataBases.
#
# Open a command window.
# Navigate to d:\edgisdbmaint
#
# Delete connections on the target database prior to running this script
# to ensure the most complete compress.
# 
# Usage:  c:\python27\arcgis10.0\python    compressGdb.py fileConn DbName
#         <path_to_python.exe>             compressGdb.py fileConn DbName
#         c:\python27\arcgis10.1\python    compressGdb.py fileConn DbName
#         d:\python27\arcgis10.0\python    compressGdb.py fileConn DbName
#         d:\python27\python               compressGdb.py fileConn DbName
#         d:\python27\arcgisx6410.1\python compressGdb.py fileConn DbName
#         
#         The path to python.exe might be different on your computer.
#
#         If you add the path to python.exe to your system path variable,
#         you don't need to include it on the command line.
# 
# Author: Vince Ulfig vulfig@gmail.com 4157101998
# ===========================================================================
# Import system modules...
import sys, os, string

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
  import arcpy
  import utility_gis
  import getPassword
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting variables...'
  # -------------------------------------------------------------------------
  intReturnCode = 0

  timeStart     = datetime.now()
  utility_gis.printTime(timeStart)

  strDbType     = utility_gis.getArg(1)
  strDbName     = utility_gis.getDbName(strDbType)
  strUsername   = 'sde'
  strPassword   = getPassword.getPassword(strDbName,strUsername)
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting variables for sdemon -o pause / resume...'
  # -------------------------------------------------------------------------
  blnPause               = True
  fltSleepSecondsPause   = 3.0
  fltSleepSecondsResume  = 5.0
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Creating connection file...'
  # -------------------------------------------------------------------------
  strFileConn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # print Running the compress...
  # -------------------------------------------------------------------------
  #intReturnCode = intReturnCode + utility_gis.sdemonPauseResumeNewThread('pause',fltSleepSecondsPause,strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + utility_gis.compressGdb(strFileConn,True)
  #intReturnCode = intReturnCode + utility_gis.sdemonPauseResume('resume',fltSleepSecondsResume,strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + utility_gis.osRemove(strFileConn)

  timeEnd         = datetime.now()
  utility_gis.printTime(timeEnd)
  utility_gis.printTimeDelta(timeStart,timeEnd)
  #print 'ReturnCode from sdemon -o pause thread = ' + str(intReturnCodeNewThread)
  return intReturnCode
  # ===========================================================================

#============================================================================
if (__name__ == "__main__"):
# ---------------------------------------------------------------------------
  intReturnCode = main()
  sys.exit(intReturnCode)
# ===========================================================================
