# ===========================================================================
# compressGdb.py
# ---------------------------------------------------------------------------
# 
# This script performs SQL update to reallocate map production 1.0 processes.
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
  strSql500 = """ declare i number := 1;
                  begin
                  for r in (select mapid from EDGIS.PGE_MAPNUMBERCOORDLUT  where
                  mapoffice in ('ANGELS CAMP','BAKERSFIELD','FRESNO','JACKSON','MERCED','STOCKTON') and mapscale = 500 and EXPORTSTATE =1 )
                  loop
                  update EDGIS.PGE_MAPNUMBERCOORDLUT
                  set SERVICETOPROCESS = i
                  where mapid = r.mapid;
                  i := i+1;
                  if i = 15 THEN
                  i := 1;
                  end if;
                  end loop;
                  COMMIT;
                  end;"""
  
  strSql100 = """ declare i number := 15;
                  begin
                  for r in (select mapid from EDGIS.PGE_MAPNUMBERCOORDLUT  where
                  mapoffice in ('ANGELS CAMP','BAKERSFIELD','FRESNO','JACKSON','MERCED','STOCKTON') and mapscale = 100 and EXPORTSTATE =1 )
                  loop
                  update EDGIS.PGE_MAPNUMBERCOORDLUT
                  set SERVICETOPROCESS = i
                  where mapid = r.mapid;
                  i := i+1;
                  if i = 24 THEN
                  i := 15;
                  end if;
                  end loop;
                  COMMIT;
                  end;"""

  strSql250 = """ declare i number := 24;
                  begin
                  for r in (select mapid from EDGIS.PGE_MAPNUMBERCOORDLUT  where
                  mapoffice in ('ANGELS CAMP','BAKERSFIELD','FRESNO','JACKSON','MERCED','STOCKTON') and mapscale = 250 and EXPORTSTATE =1 )
                  loop
                  update EDGIS.PGE_MAPNUMBERCOORDLUT
                  set SERVICETOPROCESS = i
                  where mapid = r.mapid;
                  i := i+1;
                  if i = 29 THEN
                  i := 24;
                  end if;
                  end loop;
                  COMMIT;
                  end;"""
  
  timeStart     = datetime.now()
  utility_gis.printTime(timeStart)

  strDbType     = utility_gis.getArg(1)
  strDbName     = utility_gis.getDbName(strDbType)
  strUsername   = 'edgis'
  strPassword   = getPassword.getPassword(strDbName,strUsername)
  # -------------------------------------------------------------------------

 
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  print 'Creating connection file...'
  # -------------------------------------------------------------------------
  strFileConn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
  # -------------------------------------------------------------------------

  # -------------------------------------------------------------------------
  # print Running the Map Production reconfigure...
  # -------------------------------------------------------------------------
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
