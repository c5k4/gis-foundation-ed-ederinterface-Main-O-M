#!/bin/sh
#=============================================================================
# EDG_Gather_Stat_index_ED_Pub_DC1.sh
#-----------------------------------------------------------------------------
# USAGE:        EDG_Gather_Stat_index_ED_Pub_DC1.sh
# Returns:       0 = Script did     complete action successfully.
# Returns:      >0 = Script did not complete action successfully.
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#=============================================================================

#=============================================================================
# Set manually-set script variables.
#-----------------------------------------------------------------------------
dbtype=edpub
filename_script_2=rebuild_index_gather_stats.sh
filename_script_3=create_view_anno.sh
degree=8
#=============================================================================

#=============================================================================
# Automatically set ORACLE_BASE, ORACLE HOME and TNS_ADMIN.
#-----------------------------------------------------------------------------
machine=`uname -n`

ORACLE_BASE=/u01/app/oracle
export ORACLE_BASE

ORACLE_HOME=$ORACLE_BASE/product/11.2.0/db_1
case $machine in
  "edgisdbqa01")     ORACLE_HOME=$ORACLE_BASE/product/11.2.0/db_2
esac
export ORACLE_HOME

TNS_ADMIN=$ORACLE_HOME/network/admin/tnsnames.ora
export TNS_ADMIN
#=============================================================================

#=============================================================================
# Set PATH.
#-----------------------------------------------------------------------------
LIBPATH=$ORACLE_HOME/lib
export LIBPATH
PATH=.:/usr/bin:/etc:/usr/sbin:/usr/ucb:$HOME/bin:/usr/bin/X11:/sbin:$ORACLE_HOME/bin
export PATH
#=============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------
int_error_code=0

account_uc4admin=uc4admin
account_sde=sde
account_edgis=edgis
account_process=process

date_time=`date +%Y_%m_%d_%H_%M_%S`

filename_script_base=`basename $0 | cut -d'.' -f1`
filename_log_tmp=$filename_script_base.log
filename_log=$filename_script_base\_$date_time.log

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_2=$path_script/$filename_script_2
file_script_3=$path_script/$filename_script_3
file_script_get_oracle_sid=$path_script/get_oracle_sid.sh
file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#=============================================================================
# Automatically set oracle_sid.
#-----------------------------------------------------------------------------
oracle_sid=`$file_script_get_oracle_sid $dbtype`
#=============================================================================

#=============================================================================
# Automatically set Oracle passwords.
#-----------------------------------------------------------------------------
password_uc4admin=`$file_script_get_pwd    $account_uc4admin    $oracle_sid`
password_sde=`$file_script_get_pwd         $account_sde         $oracle_sid`
password_edgis=`$file_script_get_pwd       $account_edgis       $oracle_sid`
password_process=`$file_script_get_pwd     $account_process     $oracle_sid`
#=============================================================================

#=============================================================================
# Test whether path_log exists and set path_log, sql file variabless,
# and log file variables.
#-----------------------------------------------------------------------------
if [ ! -d "$path_log" ]; then
  mkdir $path_log
  chmod -R 777 $path_log
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

file_log_tmp=$path_log/$filename_log_tmp
file_log=$path_log/$filename_log

if [ -f "$file_log_tmp" ]; then
  #echo Removing file $file_log_tmp ...
  rm -f $file_log_tmp
  int_error_code=`expr $int_error_code + $?`
fi
#=============================================================================

#=============================================================================
# Execute main program.
#-----------------------------------------------------------------------------
$ORACLE_HOME/bin/sqlplus -S << EOF_PASSWORD
$account_uc4admin/$password_uc4admin@$oracle_sid

alter user $account_sde       account unlock;
alter user $account_edgis     account unlock;
alter user $account_process     account unlock;

EOF_PASSWORD



echo "Executing Gather Stat Fix from DBA..."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo



file_log_gather_fix=$path_log/$filename_script_base\_gatherstat_fix\_$date_time.log
file_gather_fix=$path_log/$filename_script_base\_gatherstat_fix\_$date_time.sql

$ORACLE_HOME/bin/sqlplus -S << EOF_REBUILD_INDEX
$account_uc4admin/$password_uc4admin@$oracle_sid

set pagesize 0
set linesize 200

spool $file_gather_fix
select DISTINCT 'ANALYZE INDEX EDGIS.'||index_name||' ESTIMATE STATISTICS;' from dba_indexes I, DBA_TABLES T, dba_segments s where I.owner = 'EDGIS' AND T.OWNER = 'EDGIS' AND T.TABLE_NAME = I.TABLE_NAME and s.segment_name = i.index_name
AND I.NUM_ROWS = 0 AND T.NUM_ROWS <> 0 and extents > 1 ;

spool off

set echo on term on
set serveroutput on size unlimited
set time on
set timing on
spool $file_log_gather_fix
$file_gather_fix
set wrap off
set echo off term off
set serveroutput off
set time off
set timing off
spool off

EOF_REBUILD_INDEX
int_error_code=`expr $int_error_code + $?`

echo "Executed Gather Stat Fix from DBA...."
echo "date_time  = `date +%Y_%m_%d_%H_%M_%S`"
echo
echo



command="$file_script_2 $account_sde      $password_sde     $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_sde      *************     $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1


command="$file_script_2 $account_process  $password_process $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_process  ***************** $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1


command="$file_script_2 $account_edgis    $password_edgis   $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_edgis    ***************   $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1



#=============================================================================

#=============================================================================
# Check log file for problems.
#-----------------------------------------------------------------------------
for search_string in "ERROR " "ERROR: " "insufficient privileges"
  do
    if grep -q "$search_string" "$file_log_tmp" ; then
      int_error_code=`expr $int_error_code + 1`
      echo Found search_string $search_string in $file_log_tmp. >> $file_log_tmp 2>&1
    fi
  done
#=============================================================================

#=============================================================================
# Copy temporary log file to date-stamped log filename.
#-----------------------------------------------------------------------------
command="cp $file_log_tmp $file_log"

echo "Executing $command..."  >> $file_log_tmp 2>&1
                $command
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."    >> $file_log     2>&1
echo "Executed  $command."    >> $file_log_tmp 2>&1
echo                          >> $file_log     2>&1
echo                          >> $file_log_tmp 2>&1
#=============================================================================

#=============================================================================
# chmod log files.
#-----------------------------------------------------------------------------
command="chmod 777 $file_log_tmp"

echo "Executing $command..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."    >> $file_log     2>&1
echo "Executed  $command."    >> $file_log_tmp 2>&1
echo                          >> $file_log     2>&1
echo                          >> $file_log_tmp 2>&1


command="chmod 777 $file_log"

echo "Executing $command..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."    >> $file_log     2>&1
echo "Executed  $command."    >> $file_log_tmp 2>&1
echo                          >> $file_log     2>&1
echo                          >> $file_log_tmp 2>&1
#=============================================================================

#=============================================================================
if [ $int_error_code == 0 ] ; then
echo  "UC4 WEBR DC1 Maintenance :"$date_time": "$filename_script_base".sh:\\n This is to inform you that, Gather Stat task in WEBR DC1 publication flow is completed.\\n Warm Up Script is in Progress... System would be available in next one and half hour.\\n Please call EDGIS Support for more details." | mailx -s "WEBR DC1 System Maintenace Status." -r "EDGISSupport<EDGISSupport@Exchange.pge.com>" "ITGISINCIDENTTEAM<ITGISINCIDENTTEAM@Exchange.pge.com>" 
fi
#=============================================================================


#=============================================================================
# Exit with error code
#-----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code. >> $file_log_tmp 2>&1
echo Exiting $0 with error code $int_error_code. >> $file_log     2>&1
exit $int_error_code
#=============================================================================
