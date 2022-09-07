# ===========================================================================
# reconcileGdb.py
# ---------------------------------------------------------------------------
# 
# This script will populate a list of all version names
# and reconcile all versions. 
#
# Open a command window.
# Navigate to d:\edgisdbmaint
#
# Usage:  python.exe reconcileGdb.py conn_file
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
  strFileScript = os.path.abspath(__file__)
  print 'Imported  system  modules into ' + strFileScript + '.'
  print ''
  #print 'Importing arcpy   module  into ' + strFileScript + '...'
  import arcpy
  #print 'Imported  arcpy   module...'
  #print ''
  #print 'Importing arcinfo module  into ' + strFileScript + '...'
  import arcinfo
  #print 'Imported  arcinfo module...'
  #print ''
  #print 'Importing utility_gis.py module  into ' + strFileScript + '...'
  import utility_gis
  #print 'Imported  utility_gis.py module.'
  #print ''
  import getPassword
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Setting paths to data...'
  # -------------------------------------------------------------------------
  intReturnCode = 0

  strDbType     = utility_gis.getArg(1)
  print 'strDbType = ' + strDbType
  strDbName     = utility_gis.getDbName(strDbType)
  print 'strDbName = ' + strDbName
  strUsername   = 'sde'
  strPassword   = getPassword.getPassword(strDbName,strUsername)

  # -------------------------------------------------------------------------
  #print 'Creating connection file...'
  # -------------------------------------------------------------------------
  strFileConn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,'')
  # -------------------------------------------------------------------------

  print 'Set paths to data...'
  print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Executing common ancestor stored procedure...'
  # -------------------------------------------------------------------------
  strSql = 'call common_ancestor_exe()'
  intReturnCode = utility_gis.sdeSqlExecute(strFileConn,strSql)
  print 'Executed  common ancestor stored procedure.'
  print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Executng SQL to return a list of versions to reconcile...'
  # -------------------------------------------------------------------------
  versionList = {}
  if (intReturnCode == 0):
    strSql = "SELECT ''||OWNER||'.'||name||'' FROM sde.version_order ORDER BY ca_state_id, state_id"
    sdeReturn = utility_gis.sdeSqlExecuteReturn(strFileConn,strSql,False)
    try:
      if isinstance(sdeReturn, list):
        for row in sdeReturn:
          name = row[0]
          versionList[name] = row
      else:
          exit
    except Exception as e:
      print "ERROR: Exception:", sys.exc_info()[0]
      print 'ERROR: Exception = ' + str(e.args)
      intReturnCode = intReturnCode + 1
  print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Reconciling versions...'
  # -------------------------------------------------------------------------
  intCountVersionsReconciledSuccessfully   = 0
  intCountVersionsReconciledUnSuccessfully = 0
  for strVersion in versionList:
    intSuccess = utility_gis.reconcileVersion(strFileConn,strVersion)
    # The code below is the initial baby step for reconciling versions
    # in paralell.
    # The code below needs to monitor callbacks from these reconciles,
    # and limit the number of concurrent threads to 8 or 16.
    #intSuccess = utility_gis.reconcileVersionNewThread(strFileConn,strVersion)
    if (intSuccess == 0):
      intCountVersionsReconciledSuccessfully   +=1
    else:
      intCountVersionsReconciledUnSuccessfully +=1
  
  print ''
  print 'Versions     reconciled successfully:' + str(intCountVersionsReconciledSuccessfully)
  print 'Versions not reconciled successfully:' + str(intCountVersionsReconciledUnSuccessfully)
  print ''
  utility_gis.osRemove(strFileConn)
  # -------------------------------------------------------------------------

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
