import arcpy
import utility_gis
import getPassword

from arcpy import env
import sys

# For reading arguments
import getopt

def main(argv):
    workspace = ''
    try:
      opts, args = getopt.getopt(argv,"hc:", ["workspace="])
    except getopt.GetoptError:
        sys.exit(2)
    for opt, arg in opts:
        if opt == '-h':
              print 'GetVersionnumber.py -c <sde connection file>'
              sys.exit()
        elif opt in ("-c", "--workspace"):
                workspace = arg
        
    try:
        # Make data path relative
        #
        #env.workspace = sys.path[0]
    
        # Two ways to create the object, which also creates the connection to ArcSDE.
        #   Using the first method, pass a set of strings containing the connection properties:
        #   <serverName>,<portNumber>,<version>,<userName>,<password>
        #   sdeConn = arcpy.ArcSDESQLExecute("gpserver3","5151","#","toolbox","toolbox")
        #   Using the second method pass the path to a valid ArcSDE connection file
        #
        strDbName     = utility_gis.getDbName(workspace)
        strUsername   = 'gis_i'
        strPassword   = getPassword.getPassword(strDbName,strUsername) 
        
        # -------------------------------------------------------------------------
        print 'Creating connection file...'
        # -------------------------------------------------------------------------
        strFileConn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
        # -------------------------------------------------------------------------
    
        # Get the SQL statements, separated by ; from a text string.
        #
        SQLStatement = arcpy.GetParameterAsText(0)
        #SQLStatementList = "SELECT COUNT(*) FROM (select to_char(owner||'.'||name) from sde.versions where creation_time <=  (trunc(sysdate) + interval '17' hour) and upper(name) not in ('DEFAULT','DAILY_CHANGE_DETECTION_SYNC','CCBWITHVIEWS','CHANGE_DETECTION_SYNC_SCHEMATICS','SCHEMATICS_EDIT', 'CHANGE_DETECTION_SYNC_TLM','CHANGE_DETECTION_SYNC_SETTINGS') minus select to_char(a.version_name) from SDE.GDBM_RECONCILE_HISTORY a,SDE.VERSIONS b where b.owner||'.'||b.name=a.VERSION_NAME and a.RECONCILE_START_DT >= trunc(sysdate) and a.SERVICE_NAME like 'GDBMReconcileOnly%' group by a.version_name)"
        SQLStatementList = "SELECT COUNT(*) FROM (select to_char(owner||'.'||name) from sde.versions where upper(name) not in ('DEFAULT','DAILY_CHANGE_DETECTION_SYNC','CCBWITHVIEWS','CHANGE_DETECTION_SYNC_SCHEMATICS','SCHEMATICS_EDIT', 'CHANGE_DETECTION_SYNC_TLM','CHANGE_DETECTION_SYNC_SETTINGS') minus select to_char(a.version_name) from SDE.GDBM_RECONCILE_HISTORY a,SDE.VERSIONS b where b.owner||'.'||b.name=a.VERSION_NAME and a.RECONCILE_START_DT >= trunc(sysdate) and a.SERVICE_NAME like 'GDBMReconcileOnly%' group by a.version_name)"
        sdeConn = arcpy.ArcSDESQLExecute(strFileConn)
        #print "+++++++++++++++++++++++++++++++++++++++++++++\n"

        # For each SQL statement passed in, execute it.
        #
        #for sql in SQLStatementList:
        #print "Execute SQL Statement: " + SQLStatementList
        try:
            # Pass the SQL statement to the database.
            #
            sdeReturn = sdeConn.execute(SQLStatementList)
            int(sdeReturn)            
            print str(int(sdeReturn))
            sys.stdout = str(int(sdeReturn))
        
        except Exception, ErrorDesc:
            print ErrorDesc
            sdeReturn = False        
                 
    except Exception, ErrorDesc:
        print Exception, ErrorDesc
    except:
        print "Problem executing SQL."
    return

# Script start
if __name__ == "__main__":
    main(sys.argv[1:])
