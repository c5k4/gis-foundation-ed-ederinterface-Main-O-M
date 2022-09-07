# ===========================================================================
# deleteGdbVersion.py
# ---------------------------------------------------------------------------
# 
# This script deletes versions in a target workspace.
#
# Open a command window.
# Navigate to d:\edgisdbmaint
#
# Delete connections on the target database prior to running this script
# to ensure the most version deleting; connections can inhibit deleting versions.
# 
# Usage:  c:\python27\arcgis10.0\python    deleteGdbVersion.py strFileConn strDbName
#         <path_to_python.exe>             deleteGdbVersion.py strFileConn strDbName
#         c:\python27\arcgis10.1\python    deleteGdbVersion.py strFileConn strDbName
#         d:\python27\arcgis10.0\python    deleteGdbVersion.py strFileConn strDbName
#         d:\python27\python               deleteGdbVersion.py strFileConn strDbName
#         d:\python27\arcgisx6410.1\python deleteGdbVersion.py strFileConn strDbName
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
  #import arcpy
  import utility_gis
  import getPassword
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting paths to data...'
  # -------------------------------------------------------------------------
  strDbType          = utility_gis.getArg(1)
  strDbName          = utility_gis.getDbName(strDbType)
  #print 'Set     paths to data...'
  #print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting variables for sdemon -o pause / resume...'
  # -------------------------------------------------------------------------
  strUsername   = 'sde'
  strPassword   = getPassword.getPassword(strDbName,strUsername)
  #print strUsername
  #print strPassword
  #print strDbName
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Creating connection file...'
  # -------------------------------------------------------------------------
  strFileConn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Deleting versions...'
  # -------------------------------------------------------------------------
  intReturnCode = 0

  fltSleepSecondsPause   = 3.0
  fltSleepSecondsResume  = 5.0

  #intReturnCode = intReturnCode + utility_gis.sdemonPauseResumeNewThread('pause',fltSleepSecondsPause,strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + utility_gis.deleteGdbVersion(strFileConn,'delete_all_versions',False)
  #intReturnCode = intReturnCode + utility_gis.sdemonPauseResume('resume',fltSleepSecondsResume,strDbName,strUsername,strPassword)
  intReturnCode = intReturnCode + utility_gis.osRemove(strFileConn)
  print 'intReturnCode = ' + str(intReturnCode)
  # -------------------------------------------------------------------------

  return intReturnCode
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
