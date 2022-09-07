# ===========================================================================
# unregisterAsVersioned.py
# ---------------------------------------------------------------------------
# 
# This script performs the ESRI ArcGIS Server function
# unregister as versioned on PG&E ED GIS layers.
#
# Open a command window.
# Navigate to d:\edgisdbmaint
#
# Stop any ArcGIS Server map service that is using the target database
# prior to running this script.
# 
# Usage:  c:\python27\arcgis10.0\python    unregisterAsVersioned.py conn_filename schema_name
#         <path_to_python.exe>             unregisterAsVersioned.py conn_filename schema_name
#         c:\python27\arcgis10.1\python    unregisterAsVersioned.py conn_filename schema_name
#         d:\python27\arcgis10.0\python    unregisterAsVersioned.py conn_filename schema_name
#         d:\python27\python               unregisterAsVersioned.py conn_filename schema_name
#         d:\python27\arcgisx6410.1\python unregisterAsVersioned.py conn_filename schema_name
#         
#         The path to python.exe might be different on your computer.
#
#         If you add the path to python.exe to your system path variable,
#         you don't need to include it on the command line.
#
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

# ===========================================================================
def main():
# ---------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # Import Modules
  # -------------------------------------------------------------------------
  strFileScript    = os.path.abspath(__file__)
  print 'Imported  system modules into ' + strFileScript + '.'
  #print ''
  #print 'Importing arcpy  module into ' + strFileScript + '...'
  import arcpy
  #print 'Imported  arcpy  module...'
  #print ''
  #print 'Importing utility_gis.py module...'
  import utility_gis
  #print 'Imported  utility_gis.py module.'
  #print ''
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  #print 'Setting paths to data...'
  # -------------------------------------------------------------------------
  strDbType        = utility_gis.getArg(1)
  strUsername      = utility_gis.getArg(2)
  strDbName        = utility_gis.getDbName(strDbType)
  strSchemaName    = strUsername
  strPassword      = ''
  strFileConn      = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  utility_gis.setWorkspace(strFileConn)
  #print 'Set     paths to data...'
  #print ''
  # -------------------------------------------------------------------------

  intReturnCode       = 0
  strDatasetFilter    = strSchemaName + "*"
  
  listFd              = arcpy.ListDatasets(strDatasetFilter,"Feature")
  listTbl             = arcpy.ListTables(strDatasetFilter,"ALL")
  listFdAndTbl        = listFd + listTbl

  #print str(listFdAndTbl)

  for strObjName in listFdAndTbl:
    strObj            = os.path.abspath(os.path.join(strFileConn,strObjName))
    intReturnCodeTmp  = utility_gis.unregisterAsVersioned(strObj)
    if (intReturnCodeTmp <> 0):
      intReturnCode   = intReturnCode + intReturnCodeTmp
  intReturnCode       = intReturnCode + utility_gis.osRemove(strFileConn)
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
