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
account_dmsstaging=dmsstaging

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
#----------------------------------------------------------------------------
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
#----------------------------------------------------------------------------
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
#=============================================================================

#============================================================================
# Set Oracle import path.
#----------------------------------------------------------------------------
UC4_IMPORT_DIR=/u02/uc4/$oracle_sid/import
export UC4_IMPORT_DIR
file_import=$UC4_IMPORT_DIR/$filename_import
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

#=============================================================================

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


file_log_drop_user=$path_log/import_batch_drop_user.log
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

Drop package body PONS_RO.PONS
/

Drop package PONS_RO.PONS
/


DECLARE 
v_username VARCHAR2(300); 
v_count INTEGER DEFAULT 0; 
BEGIN 
for rec in (SELECT username FROM dba_users WHERE username  in  ('EDGIS','EDGISBO','PROCESS','CUSTOMER')
		    ) loop	
       v_username := rec.username; 
	execute immediate('drop user '||v_username  ||' cascade');		
	end loop;
SELECT COUNT(1) INTO v_count FROM dba_users WHERE username = 'SDE' ;
	IF v_count != 0  THEN
	execute immediate('drop user SDE cascade');
	END IF;
EXCEPTION
     WHEN OTHERS THEN
       RAISE;
END;
/ 
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

#============================================================================
# Check user exists in target DB.
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
DECLARE 
v_count INTEGER DEFAULT 0; 
BEGIN 
SELECT COUNT(1) INTO v_count 
       FROM dba_users WHERE username in  ('EDGIS','SDE','EDGISBO','PROCESS','CUSTOMER');
     IF v_count != 0 
     THEN
         v_count := -1;
        Raise_Application_Error (-20001, 'ORA- All users not yet dropped');
   END IF;
     v_count := 0;
 EXCEPTION
     WHEN OTHERS THEN
       RAISE;
END;
/ 
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


#============================================================================
# Import Oracle schemas.
#----------------------------------------------------------------------------
echo "Importing oracle export file from $oracle_sid_import into $oracle_sid..."
echo
command="impdp $account_uc4admin/$password_uc4admin@$ORACLE_SID schemas=sde,edgis,process,edgisbo,CUSTOMER  directory=UC4_IMPORT_DIR dumpfile=$filename_import_parallel\
%U.dmp logfile=$filename_import_log exclude=statistics CLUSTER=N PARALLEL=4 METRICS=Y TRACE=1ff0300"
comecho="impdp $account_uc4admin/ password_uc4admin@$ORACLE_SID schemas=sde,edgis,process,edgisbo,CUSTOMER  directory=UC4_IMPORT_DIR dumpfile=$filename_import_parallel\
%U.dmp logfile=$filename_import_log exclude=statistics CLUSTER=N PARALLEL=4 METRICS=Y TRACE=1ff0300"
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
grant select on PGEDATA.SAP_NOTIFICATIONHEADER to sde_viewer, sde_editor, mm_admin, edgis, gis_i_write;

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
/ 
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
# Clear  DBMS PIPE for RAC to NON RAC
#----------------------------------------------------------------------------
echo "Clear DBMS PIPE in $oracle_sid..."
echo
echo Executing $ORACLE_HOME/bin/sqlplus -S
echo   $account_uc4admin/password_uc4admin@$ORACLE_SID ...
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_DBMS_PIPE
$account_uc4admin/$password_uc4admin@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on

DECLARE
CURSOR pipe_list IS 
SELECT name FROM v\$db_pipes;
pipe_result NUMBER;
BEGIN
dbms_output.put_line('Cleaning up old pipes in instance');
FOR del_pipes IN pipe_list LOOP
pipe_result := DBMS_PIPE.REMOVE_PIPE(del_pipes.name);
END LOOP;
END;
/

@PONS_PACKAGE.sql
@PONS_PACKAGE_BODY.sql
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_DBMS_PIPE

int_error_code=`expr $int_error_code + $?`
echo "Cleared DMS PIPE   in $oracle_sid."
echo
#============================================================================

#============================================================================
# Alter Service point Table to run in parallel
#----------------------------------------------------------------------------
echo "Altering Service point Table to run in parallel..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $account_edgis/password_edgis@ORACLE_SID"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF_TRUNCATE
$account_edgis/$password_edgis@$ORACLE_SID
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
ALTER TABLE EDGIS.SERVICEPOINT PARALLEL 4;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
int_error_code=`expr $int_error_code + $?`
echo "Altered  Service point Table."
echo
#============================================================================


#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_warning_code.
echo Exiting $0 with error code $int_error_code.
exit $int_error_code
#============================================================================
