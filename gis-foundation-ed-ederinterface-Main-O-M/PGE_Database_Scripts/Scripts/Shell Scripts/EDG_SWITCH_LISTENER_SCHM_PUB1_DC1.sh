#----------------------------------------------------------------------------
# USAGE:        EDGIS_Switch_Pub_Listener.sh
# Returns:       0 = Script did     complete action successfully.
# Returns:      >0 = Script did not complete action successfully.
# MAINTENANCE HISTORY
# Date         Author    Description
# 06/02/2015   momv Initial version
#============================================================================
#=============================================================================
# Set script variables and environment
#-----------------------------------------------------------------------------
int_error_code=0

ORACLE_BASE=/u01/app/oracle
export ORACLE_BASE
ORACLE_HOME=$ORACLE_BASE/product/11.2.0/db_1
export ORACLE_HOME
#=============================================================================
# Set PATH
#-----------------------------------------------------------------------------
LIBPATH=$ORACLE_HOME/lib
export LIBPATH
PATH=.:/usr/bin:/etc:/usr/sbin:/usr/ucb:$HOME/bin:/usr/bin/X11:/sbin:$ORACLE_HOME/bin
export PATH
#=============================================================================
# Test for the active Pub listener and switch to the opposite listener
#-----------------------------------------------------------------------------

echo "active listener= edscmp7p"
sqlplus UC4ADMIN/uc4admin@edscmp7p<<ENDOFSQL
set echo on term on
set serveroutput on size unlimited
exec set_service_names('edscmp7p,edscmp7p.WORLD');
exit;
ENDOFSQL

sqlplus UC4ADMIN/uc4admin@edscmp1p <<ENDOFSQL
set echo on term on
set serveroutput on size unlimited
exec set_service_names('edscmp1p,edscmp1p.WORLD,EDSCHDC1_P,EDSCHDC1_P.WORLD');
exit;
ENDOFSQL
echo "Deactivated DC1-PUB2-edscmp7p"
echo "Activated DC1-PUB1-edscmp1p"
#============================================================================
# Attempt to Kill Active Sessions Maximum of 3 Times
#----------------------------------------------------------------------------

#============================================================================
# Kill Oracle Connections
#----------------------------------------------------------------------------

echo "** Start Kill Sessions **"

for i in 1 2 3
do

int_error_code=0

sqlplus UC4ADMIN/uc4admin@edscmp7p << EOF_KILL_PROCESS

set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
execute sys.kill_webr_process();
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off

EOF_KILL_PROCESS

echo "Executed  execute sys.kill_webr_process();"


echo "** Kill Sessions try $i complete**"
int_error_code=`expr $int_error_code + $?`

if [ $int_error_code == 0 ]; then
  echo
  echo "******************************"
  echo "** Kill Sessions Successful **"
  echo "******************************"
  echo
else
  echo
  echo "*******************************************"
  echo "** Kill Sessions Failed -- Error Code: $int_error_code **"
  echo "*******************************************"
  echo
  exit $int_error_code
fi


done
exit $int_error_code