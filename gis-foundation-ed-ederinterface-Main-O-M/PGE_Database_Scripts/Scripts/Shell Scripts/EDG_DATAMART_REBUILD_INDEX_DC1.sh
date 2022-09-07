#!/bin/sh
#=============================================================================
# FileName:     EDG_Rebuild_Index_Setting.sh
# Usage:        EDG_Rebuild_Index_Setting.sh
# Returns:      0=Script completed action Successfully; 1=Fail
#=============================================================================

#=============================================================================
# Set manually-set script variables.
#-----------------------------------------------------------------------------
dbtype=datamart
filename_script_2=rebuild_index_gather_stats.sh
#filename_script_1=gather_stats.sh
degree=8
#=============================================================================

#=============================================================================
# Automatically set ORACLE_BASE, ORACLE HOME and TNS_ADMIN.
#-----------------------------------------------------------------------------
machine=`uname -n`

ORACLE_BASE=/u01/app/oracle
export ORACLE_BASE

ORACLE_HOME=$ORACLE_BASE/product/11.2.0.3/db_1
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
account_edsett=edsett
account_sde=sde
account_edgis=edgis


date_time=`date +%Y_%m_%d_%H_%M_%S`

filename_script_base=`basename $0 | cut -d'.' -f1`
filename_log=$filename_script_base\_$date_time.log

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_2=$path_script/$filename_script_2
file_script_get_oracle_sid=$path_script/get_oracle_sid.sh
file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#=============================================================================
# Check path_log exists, sql file variabless, and log file variables.
#-----------------------------------------------------------------------------
if [ ! -d "$path_log" ]; then
  mkdir $path_log
  chmod -R 777 $path_log
  if [ ! -d "$path_log" ]; then
    int_error_code=`expr $int_error_code + $?`
    echo
    echo "ERROR: Please create this directory:"
    echo "$path_log"
    echo "to capture log files."
    path_log=.
    echo "Writing log files to $path_log."
    echo
  fi
fi

file_log=$path_log/$filename_log

if [ -f "$file_log" ]; then
  rm -f $file_log
  int_error_code=`expr $int_error_code + $?`
fi
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
password_edsett=`$file_script_get_pwd         $account_edsett   $oracle_sid`
password_sde=`$file_script_get_pwd         $account_sde   $oracle_sid`
password_edgis=`$file_script_get_pwd         $account_edgis   $oracle_sid`

#=============================================================================

#=============================================================================
# Execute main program.
#-----------------------------------------------------------------------------
command="$file_script_2 $account_edsett      $password_edsett      $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_edsett       password_edsett      $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log 2>&1
                $command      >> $file_log 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log 2>&1
echo                          >> $file_log 2>&1

command="$file_script_2 $account_edgis      $password_edgis      $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_edgis       password_edgis      $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log 2>&1
                $command      >> $file_log 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log 2>&1
echo                          >> $file_log 2>&1

command="$file_script_2 $account_sde      $password_sde      $oracle_sid $date_time $degree"
comecho="$file_script_2 $account_sde       password_sde      $oracle_sid $date_time $degree"

echo "Executing $comecho..."  >> $file_log 2>&1
                $command      >> $file_log 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log 2>&1
echo                          >> $file_log 2>&1





#============================================================================
# Check log file for errors.
#----------------------------------------------------------------------------
for search_string in "ERROR " "ERROR: " "insufficient privileges"
  do
    if grep -q "$search_string" "$file_log" ; then
      int_error_code=`expr $int_error_code + 1`
      echo Found search_string $search_string in $file_log. >> $file_log 2>&1
    fi
  done
#============================================================================

#============================================================================
# chmod log files.
#----------------------------------------------------------------------------
command="chmod 777 $file_log"

echo "Executing $command..."  >> $file_log 2>&1
                $command      >> $file_log 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."    >> $file_log 2>&1
echo                          >> $file_log 2>&1
#============================================================================

#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code. >> $file_log 2>&1
exit $int_error_code
#============================================================================
