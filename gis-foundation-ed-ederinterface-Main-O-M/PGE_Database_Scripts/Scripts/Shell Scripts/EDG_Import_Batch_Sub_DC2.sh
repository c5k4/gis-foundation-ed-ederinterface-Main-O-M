#!/bin/sh
#=============================================================================
# EDG_Import_Batch_Sub_DC2.sh
#-----------------------------------------------------------------------------
# USAGE:        EDG_Import_Batch_Sub_DC2.sh
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#=============================================================================

#=============================================================================
# Set manually-set script variables.
#-----------------------------------------------------------------------------
dbtype_import=edsubbatchdc2
dbtype=edsubbatch
filename_script_2=import_sub.sh
#=============================================================================

#=============================================================================
# Automatically set ORACLE_BASE, ORACLE HOME and TNS_ADMIN.
#-----------------------------------------------------------------------------
machine=`uname -n`

ORACLE_BASE=/u01/app/oracle
export ORACLE_BASE

ORACLE_HOME=$ORACLE_BASE/product/11.2.0.3/db_1
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
file_script_get_oracle_sid=$path_script/get_oracle_sid.sh
file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#=============================================================================
# Automatically set oracle_sid.
#-----------------------------------------------------------------------------
oracle_sid_import=`$file_script_get_oracle_sid $dbtype_import`
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
  rm $file_log_tmp
  int_error_code=`expr $int_error_code + $?`
fi
#=============================================================================

#=============================================================================
# Execute main program.
#-----------------------------------------------------------------------------
command="$file_script_2 $oracle_sid $oracle_sid_import $date_time"
comecho="$file_script_2 $oracle_sid $oracle_sid_import $date_time"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1
#=============================================================================

#============================================================================
# Check log file for problems.
#----------------------------------------------------------------------------
for search_string in "ERROR " "ERROR: " "insufficient privileges"
  do
    if grep -q "$search_string" "$file_log_tmp" ; then
      int_error_code=`expr $int_error_code + 1`
      echo Found search_string $search_string in $file_log_tmp. >> $file_log_tmp 2>&1
    fi
  done
#============================================================================

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

#============================================================================
# Exit with error code
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code. >> $file_log_tmp 2>&1
echo Exiting $0 with error code $int_error_code. >> $file_log     2>&1
exit $int_error_code
#============================================================================
