# ===========================================================================
# synchronizeGdbReplica.py
# ---------------------------------------------------------------------------
# This script performs the ESRI ArcGIS Server function
# Synchronize Changes on ESRI GDB Replicas.
#
# Usage:  python.exe synchronizeGdbReplica.py
#                    conn_filename_from
#                    replica_name
#                    conn_filename_to
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
  print 'Importing arcpy  module  into ' + strFileScript + '...'
  import arcpy
  print 'Imported  arcpy  module...'
  print ''
  print 'Importing utility_gis.py module into ' + strFileScript + '...'
  import utility_gis
  print 'Imported  utility_gis.py module into ' + strFileScript + '.'
  print ''
  import getPassword
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Setting paths to data...'
  # -------------------------------------------------------------------------
  strDbTypeFrom        = utility_gis.getArg(1)
  strReplicaName       = utility_gis.getArg(2)
  strDbTypeTo          = utility_gis.getArg(3)

  strDbNameFrom        = utility_gis.getDbName(strDbTypeFrom)
  strDbNameTo          = utility_gis.getDbName(strDbTypeTo)
  strUsername          = 'lbgis'
  strPasswordFrom      = getPassword.getPassword(strDbNameFrom,strUsername)
  strPasswordTo        = getPassword.getPassword(strDbNameTo,strUsername)

  strFileConnFrom      = utility_gis.createArcSDEConnectionFileBasic('',strDbNameFrom,strUsername,strPasswordFrom)
  strFileConnTo        = utility_gis.createArcSDEConnectionFileBasic('',strDbNameTo,strUsername,strPasswordTo)

  print 'Set paths to data...'
  print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Setting arcpy workspace    to ' + strFileConnFrom + ' ...'
  arcpy.env.workspace     = strFileConnFrom
  print 'Set     arcpy workspace    to ' + strFileConnFrom + ' .'
  print ''
  # -------------------------------------------------------------------------

  print 'Set strFileConnFrom = ' + strFileConnFrom
  print 'Set strReplicaName  = ' + strReplicaName
  print 'Set strFileConnTo   = ' + strFileConnTo
  print ''

  intReturnCode = 0
  intReturnCode = utility_gis.synchronizeGdbReplica(strFileConnFrom,strReplicaName,strFileConnTo)
  intReturnCode = intReturnCode + utility_gis.osRemove(strFileConnFrom)
  intReturnCode = intReturnCode + utility_gis.osRemove(strFileConnTo)
  print 'intReturnCode = ' + str(intReturnCode)
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
