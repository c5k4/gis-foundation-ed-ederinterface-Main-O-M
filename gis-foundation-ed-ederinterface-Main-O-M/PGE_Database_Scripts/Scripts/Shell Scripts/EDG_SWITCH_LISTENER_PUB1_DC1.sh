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

echo "active listener= EDGISP7P"
sqlplus UC4ADMIN/uc4admin@EDGISP7P <<ENDOFSQL
set echo on term on
set serveroutput on size unlimited
exec set_service_names('EDGISP7P,EDGISP7P.WORLD');

exit;
ENDOFSQL

sqlplus UC4ADMIN/uc4admin@EDGISP1P <<ENDOFSQL
set echo on term on
set serveroutput on size unlimited
exec set_service_names('EDGISP1P,EDGISP1P.WORLD,EDGISDC1_P,EDGISDC1_P.WORLD');
exit;
ENDOFSQL
echo "Deactivated DC1-PUB2-EDGISP7P"
echo "Activated DC1-PUB1-EDGISP1P"
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

sqlplus UC4ADMIN/uc4admin@EDGISP7P << EOF_KILL_PROCESS

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

#============================================================================
# Rename control file.This file allow event object to decide which flow (pub1 or pub2) needs to be triggered
#----------------------------------------------------------------------------

##change file name to PUB1 to tell that today PUB1 is complete and next day PUB2 flow will be refreshed

ssh -q itgisadm@edgisdboraprd02 '/u02/uc4/edgisdbmaint/remcontrolfile_PUB1.sh'

int_error_code=0
int_error_code=`expr $int_error_code + $?`

if [ $int_error_code == 0 ]; then
  echo
  echo "******************************"
  echo "Control file renamed to PUB1 on edgisdboraprd02. Next day DC1 PUB2(edgisdboraprd02) will be refreshed"
  echo "******************************"
  echo
else
  echo
  echo "*******************************************"
  echo "**Control file could not be renamed to PUB1 on edgisdboraprd02. Error Code: $int_error_code**"
  echo "*******************************************"
  echo
  exit $int_error_code
fi


exit $int_error_code