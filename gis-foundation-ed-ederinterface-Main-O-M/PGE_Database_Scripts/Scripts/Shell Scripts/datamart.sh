

importDmp()
{

	echo =============================================================================
	echo Import Oracle schemas.
	echo "Importing oracle export file from $oracle_sid_import into $oracle_sid..."
	echo impdp $account_uc4admin/$password_uc4admin@$oracle_sid schemas=edsett directory=$UC4_IMPORT_DIR dumpfile=$filename_import logfile=$filename_import_log
	echo =============================================================================

	# REMBER THE DIRECTORY IS AN ORACLE DIRECTORY THAT POINTS TO THE IMPORT LOCATION
	# impdp $account_uc4admin/$password_uc4admin@$oracle_sid schemas=edsett directory=ORA_UC4_DM_SETTING_IMPORT dumpfile=$filename_import logfile=$filename_import
	impdp uc4admin/uc4admin@eddatamq schemas=edsett directory=ORA_UC4_DM_SETTING_IMPORT dumpfile=edgisspt.dmp logfile=impdmp.log
	int_warning_code=`expr $int_warning_code + $?`

	echo "Imported  oracle export file from $oracle_sid_import into $oracle_sid."

	echo $int_warning_code
	return $int_warning_code
}


killSql()
{

user=$1
pass=$2
sid=$3
echo 
echo $user
echo $sid
echo ============================================================================
echo Kill Oracle connections.
echo ============================================================================
echo "Executing execute sys.KILL_PROCESS();"
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $user/pass@$sid"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
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
EOF
int_error_code=`expr $int_error_code + $?`
echo "Executed  execute sys.KILL_PROCESS();"

return $?
}

dropUsers(){


user=$1
pass=$2
sid=$3
echo 
echo $user
echo $sid
echo ============================================================================
echo Drop user EDSETT
echo ============================================================================
echo "Dropping users (schemas) in $oracle_sid..."
echo
echo Executing $ORACLE_HOME/bin/sqlplus -S
echo   $account_uc4admin/password_uc4admin@$ORACLE_SID ...
echo
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
drop user edsett cascade;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF
int_error_code=`expr $int_error_code + $?`
echo "Dropped  users (schemas) in $oracle_sid."
echo


return $?
}




alterSql()
{

user=$1
pass=$2
sid=$3
edsettUser=$4
edsettPass=$5

echo 
echo ============================================================================
echo Set Oracle passwords.
echo ============================================================================

echo "Setting Oracle passwords on imported ED Maintenance DB..."
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $user/pass@$sid"
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
alter user  $edsettUser identified by "$edsettPass"     account unlock;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF
int_error_code=`expr $int_error_code + $?`
echo "Set     Oracle passwords on imported ED Maintenance DB."
echo


return $?
}

truncateSql()
{

user=$1
pass=$2
sid=$3
echo 
echo ============================================================================
echo Truncate dynamic SDE tables.
echo ============================================================================
echo "Truncating dynamic SDE tables..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $user/pass@$sid"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
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
EOF
int_error_code=`expr $int_error_code + $?`
echo "Truncated  dynamic SDE tables."
echo


return $?
}


dropOrphanTablesSql()
{
user=$1
pass=$2
sid=$3
echo SID=$sid
file_sql_drop_orphaned_keyset_tables=$4
file_log_drop_orphaned_keyset_tables=$5

# Drop orphaned keyset tables.
#----------------------------------------------------------------------------
echo 
echo "Dropping orphaned keyset tables..."
echo
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $user/pass@$sid"
echo
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
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
EOF
int_error_code=`expr $int_error_code + $?`
echo "Dropped  orphaned keyset tables."
echo


return $?
}


dropUsersTlm(){


user=$1
pass=$2
sid=$3
echo 
echo $user
echo $sid
echo ============================================================================
echo Drop user TLM
echo ============================================================================
echo "Dropping users (schemas) in $oracle_sid..."
echo
echo Executing $ORACLE_HOME/bin/sqlplus -S
echo   $account_uc4admin/password_uc4admin@$ORACLE_SID ...
echo
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
drop user edtlm cascade;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF
int_error_code=`expr $int_error_code + $?`
echo "Dropped  users (schemas) in $oracle_sid."
echo


return $?
}


alterSqlTlm()
{

user=$1
pass=$2
sid=$3
alterUser=$4
alterPass=$5

echo 
echo ============================================================================
echo Set Oracle passwords.
echo ============================================================================

echo "Setting Oracle passwords on imported TLM DB..."
echo "Executing $ORACLE_HOME/bin/sqlplus -S"
echo "  $user/pass@$sid"
$ORACLE_HOME/bin/sqlplus -S << EOF
$user/$pass@$sid
set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on
alter user  $alterUser identified by "$alterPass"     account unlock;
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
EOF
int_error_code=`expr $int_error_code + $?`
echo "Set     Oracle passwords on imported ED Maintenance DB."
echo


return $?
}