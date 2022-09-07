#!/bin/sh
#=============================================================================
# import.sh
#-----------------------------------------------------------------------------
# USAGE:        import.sh oracle_sid oracle_sid_import datetime
#
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#=============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------

account_uc4admin=uc4admin
account_edgis=edgis
account_process=process
account_sde=sde
account_webr=webr

password_uc4admin_default=foo

oracle_sid_default=foo

int_error_code=0
int_warning_code=0

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

filename_script_base=`basename $0 | cut -d'.' -f1`
file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#============================================================================
# Set ORACLE_SID.
# THIS IS THE DB IT'S GOING TO
#----------------------------------------------------------------------------
# DB FROM 
if [ -z "$1" ] ; then
  oracle_sid=$oracle_sid_default
else
  oracle_sid=$1
fi
ORACLE_SID=`echo $oracle_sid | tr '[:lower:]' '[:upper:]'`
export ORACLE_SID
echo "oracle_sid          = $oracle_sid"
echo "ORACLE_SID          = $ORACLE_SID"
#============================================================================

#============================================================================
# Set oracle_sid_import.
# THIS IS THE DATABASE THAT IT IS COMING FROM 
#----------------------------------------------------------------------------
# 
if [ -z "$2" ] ; then
  oracle_sid_import=foo
else
  oracle_sid_import=$2
fi
filename_import=$oracle_sid_import.dmp
filename_import_parallel=$oracle_sid_import\_
echo "oracle_sid_import   = $oracle_sid_import"
#============================================================================

#============================================================================
# Set date_time.
#----------------------------------------------------------------------------
if [ -z "$3" ] ; then
  date_time=`date +%Y_%m_%d_%H_%M_%S`
else
  date_time=$3
fi
echo "date_time           = $date_time"
#===========================================================================

#=============================================================================
# Automatically set Oracle passwords.
#-----------------------------------------------------------------------------
password_uc4admin=`$file_script_get_pwd    $account_uc4admin    $oracle_sid`
password_sde=`$file_script_get_pwd         $account_sde         $oracle_sid`
password_edgis=`$file_script_get_pwd       $account_edgis       $oracle_sid`
password_process=`$file_script_get_pwd     $account_process     $oracle_sid`
password_webr=`$file_script_get_pwd        $account_webr        $oracle_sid`
#=============================================================================

#============================================================================
# Set Oracle import path.
#----------------------------------------------------------------------------
# UC4_IMPORT_DIR=/u02/uc4/$oracle_sid/import
# export UC4_IMPORT_DIR

# LOCATION OF THE FILE TO IMPORT
# this should be $oracle_sid_import
file_import=/u02/uc4/$oracle_sid/import/$filename_import

chmod g+s $UC4_IMPORT_DIR
chmod 777 $file_import

filename_import_log=$oracle_sid\_$date_time.log

echo "UC4_IMPORT_DIR      = $UC4_IMPORT_DIR"
echo "filename_import     = $filename_import"
echo "file_import         = $file_import"
echo
#============================================================================




#=============================================================================
# Test whether path_log exists and set path_log, sql file variabless,
# and log file variables.
#-----------------------------------------------------------------------------
if [ ! -d "$path_log" ]; then
  mkdir $path_log
  if [ ! -d "$path_log" ]; then
    int_error_code=`expr $int_error_code + $?`
    echo
    echo "Please create directory"
    echo "$path_log"
    echo "to capture log files."
    path_log=.
    echo "Writing log files to $path_log."
    echo
  fi
fi
file_log_drop_orphaned_keyset_tables=$path_log/$filename_script_base\_drop_orphaned_keyset_tables\_$date_time.log
file_sql_drop_orphaned_keyset_tables=$path_log/$filename_script_base\_drop_orphaned_keyset_tables\_$date_time.sql
file_sql_drop_sde_dependent_datamart_objects=$path_script/drop_datamart_sde_depend_objects.sql
file_log_drop_sde_dependent_datamart_objects=$path_log/$filename_script_base\drop_datamart_sde_depend_objects\_$date_time.log
file_sql_create_datamart_objects=$path_script/create_datamart_objects.sql

file_log_create_datamart_objects=$path_log/$filename_script_base\create_datamart_objects\_$date_time.log



file_log_drop_user=$path_log/import_datamart_drop_user.log
#=============================================================================

# FOR TESTING - DON'T RUN THE STUFF BELOW AND HOSE THE DB

echo $account_uc4admin/$password_uc4admin@$ORACLE_SID
#============================================================================
# Kill Oracle connections.
#----------------------------------------------------------------------------
echo "Executing execute sys.KILL_PROCESS();"
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $account_uc4admin/password_uc4admin@ORACLE_SID"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_KILL_PROCESS
$account_uc4admin/$password_uc4admin@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
execute sys.KILL_PROCESS();
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF_KILL_PROCESS
int_error_code=`expr $int_error_code + $?`
echo "Executed  execute sys.KILL_PROCESS();" 
echo
#============================================================================

#============================================================================
# Drop users in target DB.
#----------------------------------------------------------------------------
echo "Dropping users (schemas) in $oracle_sid..."
echo
echo Executing $ORACLE_HOME/bin/sqlplus -S
echo   $account_uc4admin/password_uc4admin@$ORACLE_SID ...
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_DROP_USER
$account_uc4admin/$password_uc4admin@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_drop_user
drop user webr cascade;
drop user customer cascade;
drop user process cascade;
drop user edgis   cascade;
@$file_sql_drop_sde_dependent_datamart_objects
drop user sde     cascade;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_DROP_USER
int_error_code=`expr $int_error_code + $?`
echo "Dropped  users (schemas) in $oracle_sid."
echo
#============================================================================

#============================================================================
# Check log file for problems.
#----------------------------------------------------------------------------
log_error_count=`grep -c "ORA-" "$file_log_drop_user"`

if [ $log_error_count != 0 ] ; then
echo Exiting $0 with error code $log_error_count.
exit $log_error_count 
 
fi

if [ $int_error_code != 0 ] ; then
echo "int_error    code" $int_error_code
echo Exiting $0 with error code $int_error_code.
exit $int_error_code 
fi


#============================================================================
# Import Oracle schemas.
#----------------------------------------------------------------------------
echo "Importing oracle export file from $oracle_sid_import into $oracle_sid..."
echo
command="impdp $account_uc4admin/$password_uc4admin@$ORACLE_SID schemas=sde,edgis,process,customer,webr directory=UC4_IMPORT_DIR dumpfile=$filename_import_parallel\%U.dmp logfile=$filename_import_log CLUSTER=N PARALLEL=4"
comecho="impdp $account_uc4admin/ password_uc4admin@$ORACLE_SID schemas=sde,edgis,process,customer,webr directory=UC4_IMPORT_DIR dumpfile=$filename_import_parallel\%U.dmp logfile=$filename_import_log CLUSTER=N PARALLEL=4"
echo "Executing $comecho..."
                $command
echo "Executed  $comecho."
echo
int_warning_code=`expr $int_warning_code + $?`
#Do not check for error here because warnings are being sent as errors
#in the status variable $?
echo "Imported  oracle export file from $oracle_sid_import into $oracle_sid."
echo
#============================================================================

#============================================================================
# Set Oracle passwords.
#----------------------------------------------------------------------------
echo "Setting Oracle passwords on imported ED Maintenance DB..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $account_uc4admin/password_uc4admin@ORACLE_SID"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_PASSWORD
$account_uc4admin/$password_uc4admin@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
alter user $account_sde     identified by "$password_sde"     account unlock;
alter user $account_edgis   identified by "$password_edgis"   account unlock;
alter user $account_process identified by "$password_process" account unlock;
alter user $account_webr    identified by "$password_webr"    account unlock;
GRANT SELECT ON EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW TO SDE_VIEWER;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF_PASSWORD
int_error_code=`expr $int_error_code + $?`
echo "Set     Oracle passwords on imported ED Maintenance DB."
echo
#============================================================================

#============================================================================
# Truncate dynamic SDE tables.
#----------------------------------------------------------------------------
echo "Truncating dynamic SDE tables..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $account_sde/password_sde@ORACLE_SID"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_TRUNCATE
$account_sde/$password_sde@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
truncate table sde.sde_logfile_data; 
truncate table sde.state_locks;
truncate table sde.table_locks;
truncate table sde.object_locks;
truncate table sde.layer_locks;
truncate table sde.process_information;
truncate table edgis.sde_logfile_data;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF_TRUNCATE
int_error_code=`expr $int_error_code + $?`
echo "Truncated  dynamic SDE tables."
echo
#============================================================================

#============================================================================
# Drop orphaned keyset tables.
#----------------------------------------------------------------------------
echo "Dropping orphaned keyset tables..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $account_sde/password_sde@ORACLE_SID"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_DROP_ORPHANED_KEYSET_TABLES
$account_sde/$password_sde@$ORACLE_SID
set pagesize 0
set linesize 200
spool $file_sql_drop_orphaned_keyset_tables

DECLARE 

CURSOR all_keysets IS 
SELECT owner, table_name 
FROM dba_tables 
WHERE table_name LIKE 'KEYSET_%';

sess_id INTEGER; 
valid_sess INTEGER; 
lock_name VARCHAR2(30); 
lock_handle VARCHAR2(128); 
lock_status INTEGER; 
cnt INTEGER DEFAULT 0; 

BEGIN 

FOR drop_keysets IN all_keysets LOOP 

sess_id := TO_NUMBER(SUBSTR(drop_keysets.table_name, 8)); 

SELECT COUNT(*) INTO valid_sess FROM sde.process_information WHERE owner = drop_keysets.owner AND sde_id = sess_id; 

IF valid_sess = 1 THEN 
lock_name := 'SDE_Connection_ID#' || TO_CHAR (sess_id); 
DBMS_LOCK.ALLOCATE_UNIQUE (lock_name,lock_handle); 
lock_status := DBMS_LOCK.REQUEST (lock_handle,DBMS_LOCK.X_MODE,0,TRUE); 

IF lock_status = 0 THEN 
DELETE FROM sde.process_information WHERE sde_id = sess_id; 
DELETE FROM sde.state_locks WHERE sde_id = sess_id; 
DELETE FROM sde.table_locks WHERE sde_id = sess_id; 
DELETE FROM sde.object_locks WHERE sde_id = sess_id; 
DELETE FROM sde.layer_locks WHERE sde_id = sess_id; 
dbms_output.put_line('Removed orphaned process_information entry ('||sess_id||')'); 

EXECUTE IMMEDIATE 'DROP TABLE '||drop_keysets.owner||'.'||drop_keysets.table_name; 
cnt := cnt + 1; 
END IF; 

ELSE 
EXECUTE IMMEDIATE 'DROP TABLE '||drop_keysets.owner||'.'||drop_keysets.table_name; 
cnt := cnt + 1; 

END IF; 
END LOOP; 

dbms_output.put_line('Dropped '||cnt||' keyset tables.'); 

END; 
spool off

set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_drop_orphaned_keyset_tables
@$file_sql_drop_orphaned_keyset_tables
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_DROP_ORPHANED_KEYSET_TABLES
int_error_code=`expr $int_error_code + $?`
echo "Dropped  orphaned keyset tables."
echo



#============================================================================

#============================================================================
# Create datamart objects
#----------------------------------------------------------------------------
echo "Create datamart objects in $oracle_sid..."
echo
echo Executing $ORACLE_HOME/bin/sqlplus -S
echo   $account_uc4admin/password_uc4admin@$ORACLE_SID ...
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_DATAMART_OBJECT
$account_uc4admin/$password_uc4admin@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_create_datamart_objects
@$file_sql_create_datamart_objects
grant select on edgis.pge_elecdistnetwork_trace to mdssuser_ro, mdmsuser_ro, cyme_ro, j2t4, majc, dhn4, krbk, jbj8;
grant select on edgis.pge_feederfednetwork_trace to mdssuser_ro, mdmsuser_ro, cyme_ro, j2t4, majc, dhn4, krbk, jbj8;
grant select on edsett.sm_circuit_breaker to mdssuser_ro, cyme_ro, j2t4, majc, dhn4, krbk, jbj8;
grant select on CUSTOMER.CUSTOMER_INFO to ROBCAPP;
grant select on edgis.pge_circuitsource to sde_viewer;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_DATAMART_OBJECT
int_error_code=`expr $int_error_code + $?`
echo "Created datamart objects in $oracle_sid."
echo
#============================================================================



#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_warning_code.
echo Exiting $0 with error code $int_error_code.
exit $int_error_code
#============================================================================
