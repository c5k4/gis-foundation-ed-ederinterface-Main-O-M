#!/bin/sh
#============================================================================
# create_view_anno.sh
#----------------------------------------------------------------------------
# This script creates views for annotation that are used by EDDR and WEBR.
# This script is run after versions are deleted from the ED Pub DB,
# because these views use the versions in the ED Pub DB,
# and these views are destroyed when the versions are dropped during
# the nightly UC4 ED Publication Flow.
#
# USAGE:        create_view_anno.sh account password oracle_sid date_time
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#============================================================================

#=============================================================================
# Set script variables and environment
#-----------------------------------------------------------------------------
int_error_code=0

account_edgis_default=edgis
password_edgis_default=foo

oracle_sid_default=foo

filename_script_base=`basename $0 | cut -d'.' -f1`

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_sql_create_view_anno=$path_script/create_view_anno.sql
#=============================================================================

#============================================================================
# Set account_edgis
#----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  account_edgis=$account_edgis_default
else
  account_edgis=$1
fi
export account_edgis
echo
echo "account_edgis    = $account_edgis"
schema_name=`echo $account_edgis | tr '[:lower:]' '[:upper:]'`
#============================================================================

#============================================================================
# Set password_edgis
#----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  password_edgis=$password_edgis_default
else
  password_edgis=$2
fi
export password_edgis
echo "password_edgis   = *******"
#============================================================================

#============================================================================
# Set ORACLE_SID
#----------------------------------------------------------------------------
if [ -z "$3" ] ; then
  oracle_sid=$oracle_sid_default
else
  oracle_sid=$3
fi
ORACLE_SID=`echo $oracle_sid | tr '[:lower:]' '[:upper:]'`
export ORACLE_SID
echo "ORACLE_SID = $ORACLE_SID"
echo
#============================================================================

#============================================================================
# Set date_time
#----------------------------------------------------------------------------
if [ -z "$4" ] ; then
  date_time=`date +%Y_%m_%d_%H_%M_%S`
else
  date_time=$4
fi
echo
echo "date_time  = $date_time"
#===========================================================================

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

file_log_view_create=$path_log/$filename_script_base\_create_view_anno\_$date_time.log
#=============================================================================

#=============================================================================
# Connect to Oracle and run SQL.
#-----------------------------------------------------------------------------
echo "Executing $file_sql_create_view_anno..."
echo

$ORACLE_HOME/bin/sqlplus -S << EOF_VIEW_CREATE
$account_edgis/$password_edgis@$ORACLE_SID

set pagesize 0
set linesize 200

set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_view_create
@$file_sql_create_view_anno
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off

EOF_VIEW_CREATE
int_error_code=`expr $int_error_code + $?`

echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo "Executed  $file_sql_create_view_anno..."
echo
#============================================================================

#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code.
exit $int_error_code
#============================================================================
