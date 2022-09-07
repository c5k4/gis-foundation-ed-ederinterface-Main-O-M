#!/bin/sh
#============================================================================
# EDG_Export_Pub_DC1_2.sh
#----------------------------------------------------------------------------
# USAGE:        EDG_Export_Pub_DC1_2.sh account password ORACLE_SID
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
# MAINTENANCE HISTORY
# Date       Author           Description
# 2013_05_08 vdu1             initial version
#============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------
int_error_code=0

account_uc4admin=uc4admin
account_sde=sde
account_edgis=edgis
account_process=process

oracle_sid_default=foo

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#============================================================================
# Set ORACLE_SID.
#----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  oracle_sid=$oracle_sid_default
else
  oracle_sid=$1
fi
echo
ORACLE_SID=`echo $oracle_sid | tr '[:lower:]' '[:upper:]'`
export ORACLE_SID
echo "oracle_sid = $oracle_sid"
echo "ORACLE_SID = $ORACLE_SID"
echo
#============================================================================

#============================================================================
# Set date_time.
#----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  date_time=`date +%Y_%m_%d_%H_%M_%S`
else
  date_time=$2
fi
echo "date_time   = $date_time"
echo
#===========================================================================

#=============================================================================
# Automatically set Oracle passwords.
#-----------------------------------------------------------------------------
password_uc4admin=`$file_script_get_pwd    $account_uc4admin    $oracle_sid`
password_sde=`$file_script_get_pwd         $account_sde         $oracle_sid`
password_edgis=`$file_script_get_pwd       $account_edgis       $oracle_sid`
password_process=`$file_script_get_pwd     $account_process     $oracle_sid`
#=============================================================================

#=============================================================================
# Connect to Oracle and run SQL
#-----------------------------------------------------------------------------
echo "Script SQL start = `date +%Y_%m_%d_%H_%M_%S`"
echo

$ORACLE_HOME/bin/sqlplus -S << EOF_APPLY_USER_ROLES_EDGIS
$account_edgis/$password_edgis@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
select 'grant select on '||owner||'.'||table_name||' to SDE_VIEWER' from sde.table_registry where owner='EDGIS'
EOF_APPLY_USER_ROLES_EDGIS
int_error_code=`expr $int_error_code + $?`

$ORACLE_HOME/bin/sqlplus -S << EOF_APPLY_USER_ROLES_PROCESS
$account_process/$password_process@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
select 'grant select on '||owner||'.'||table_name||' to SDE_VIEWER' from sde.table_registry where owner='PROCESS'
EOF_APPLY_USER_ROLES_PROCESS
int_error_code=`expr $int_error_code + $?`

$ORACLE_HOME/bin/sqlplus -S << EOF_APPLY_USER_ROLES_SDE
$account_sde/$password_sde@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
select 'grant select on '||owner||'.'||table_name||' to SDE_VIEWER' from sde.table_registry where owner='SDE'
EOF_APPLY_USER_ROLES_SDE
int_error_code=`expr $int_error_code + $?`

$ORACLE_HOME/bin/sqlplus -S << EOF_APPLY_USER_ROLES_M_TO_N_RELATIONSHIP
$account_edgis/$password_edgis@$ORACLE_SID
grant select on edgis.ConduitSystem_DCConductor     to SDE_VIEWER;
grant select on edgis.ConduitSystem_SecUG           to SDE_VIEWER;
grant select on edgis.ConduitSystem_PriUG           to SDE_VIEWER;
EOF_APPLY_USER_ROLES_M_TO_N_RELATIONSHIP
int_error_code=`expr $int_error_code + $?`

echo "Script SQL end   = `date +%Y_%m_%d_%H_%M_%S`"
echo
#============================================================================

#============================================================================
# Exit with error code
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code.
exit $int_error_code
#============================================================================
