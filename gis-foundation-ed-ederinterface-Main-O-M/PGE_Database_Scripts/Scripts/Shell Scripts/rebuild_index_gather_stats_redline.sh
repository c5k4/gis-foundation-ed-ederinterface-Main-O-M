#!/bin/sh
#=============================================================================
# rebuild_index_gather_stats.sh
#-----------------------------------------------------------------------------
# USAGE:        rebuild_index_gather_stats.sh account password oracle_sid
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#=============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------
int_error_code=0
account_default=foo
password_default=foo
oracle_sid_default=foo
degree_default=21

filename_script_base=`basename $0 | cut -d'.' -f1`

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log
#=============================================================================

#=============================================================================
# Set account.
#-----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  account=$account_default
else
  account=$1
fi
export account
echo
echo "account    = $account"
schema_name=`echo $account | tr '[:lower:]' '[:upper:]'`
#=============================================================================

#=============================================================================
# Set password.
#-----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  password=$password_default
else
  password=$2
fi
export password
echo "password   = *********"
#=============================================================================

#=============================================================================
# Set ORACLE_SID.
#-----------------------------------------------------------------------------
if [ -z "$3" ] ; then
  oracle_sid=$oracle_sid_default
else
  oracle_sid=$3
fi
ORACLE_SID=`echo $oracle_sid | tr '[:lower:]' '[:upper:]'`
export ORACLE_SID
echo "ORACLE_SID = $ORACLE_SID"
echo
#=============================================================================

#=============================================================================
# Set date_time.
#-----------------------------------------------------------------------------
if [ -z "$4" ] ; then
  date_time=`date +%Y_%m_%d_%H_%M_%S`
else
  date_time=$4
fi
echo
echo "date_time  = $date_time"
#=============================================================================

#=============================================================================
# Set degree.
#-----------------------------------------------------------------------------
if [ -z "$5" ] ; then
  degree=$degree_default
else
  degree=$5
fi
export degree
echo "degree     = $degree"
echo
#=============================================================================

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

file_log_rebuild_index=$path_log/$filename_script_base\_rebuild_index_$account\_$date_time.log
file_sql_rebuild_index=$path_log/$filename_script_base\_rebuild_index_$account\_$date_time.sql

file_log_reset_noparallel=$path_log/$filename_script_base\_reset_noparallel_$account\_$date_time.log
file_sql_reset_noparallel=$path_log/$filename_script_base\_reset_noparallel_$account\_$date_time.sql

file_log_gather_stats=$path_log/$filename_script_base\_gather_stats_$account\_$date_time.log
file_sql_gather_stats=$path_log/$filename_script_base\_gather_stats_$account\_$date_time.sql
#=============================================================================

#=============================================================================
# Connect to Oracle and run SQL.
#-----------------------------------------------------------------------------
echo "Executing alter index rebuild online parallel..."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo

$ORACLE_HOME/bin/sqlplus -S << EOF_REBUILD_INDEX
$account/$password@$ORACLE_SID

set pagesize 0
set linesize 200

spool $file_sql_rebuild_index
SELECT 'ALTER INDEX '||owner||'.'||index_name||' '||'REBUILD ONLINE parallel (degree $degree ) NOLOGGING;'FROM all_indexes WHERE owner ='$schema_name' and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' ORDER BY owner, index_name;
spool off

set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_rebuild_index
@$file_sql_rebuild_index
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off

EOF_REBUILD_INDEX
int_error_code=`expr $int_error_code + $?`

echo "Executed  alter index rebuild online parallel."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo
echo


echo "Executing alter index noparallel..."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo

$ORACLE_HOME/bin/sqlplus -S << EOF_ALTER_INDEX
$account/$password@$ORACLE_SID

set pagesize 0
set linesize 200

spool $file_sql_reset_noparallel
SELECT 'ALTER INDEX '||owner||'.'||index_name||' '||'NOPARALLEL;'FROM all_indexes WHERE owner ='$schema_name' and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' ORDER BY owner, index_name;
spool off

set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_reset_noparallel
@$file_sql_reset_noparallel
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_ALTER_INDEX
int_error_code=`expr $int_error_code + $?`

echo "Executed  alter index noparallel."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo
echo


echo "Executing dbms_stats.gather_schema_stats..."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo

$ORACLE_HOME/bin/sqlplus -S << EOF_GATHER_STATS
$account/$password@$ORACLE_SID

set pagesize 0
set linesize 200

spool $file_sql_gather_stats
prompt exec DBMS_STATS.GATHER_SCHEMA_STATS('$schema_name',estimate_percent=>100,DEGREE=> $degree,CASCADE=>TRUE,No_Invalidate=>false);
spool off

set echo on term on
set serveroutput on
set time on
set timing on
spool $file_log_gather_stats
@$file_sql_gather_stats
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off
EOF_GATHER_STATS
int_error_code=`expr $int_error_code + $?`

echo "Executed  dbms_stats.gather_schema_stats."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo

echo
echo "Script SQL end   = `date +%Y_%m_%d_%H_%M_%S`"
echo
#============================================================================

#============================================================================
# Exit with error code
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code.
exit $int_error_code
#============================================================================
