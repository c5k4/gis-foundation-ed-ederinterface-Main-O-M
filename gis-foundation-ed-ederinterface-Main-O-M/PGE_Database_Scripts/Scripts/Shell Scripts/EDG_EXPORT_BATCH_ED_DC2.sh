#!/bin/sh
#============================================================================
# EDG_EXPORT_BATCH_ED_DC2.SH
#----------------------------------------------------------------------------
# USAGE:        EDG_EXPORT_BATCH_ED_DC2.SH
# Returns:       0 = Script did     complete action successfully.
# Returns:      >0 = Script did not complete action successfully.
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#============================================================================

# exit 1

#=============================================================================
# Set manually-set script variables.
#-----------------------------------------------------------------------------
dbtype=edbatch
filename_script_export=export.sh
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
#============================================================================

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
filename_script_2=$filename_script_base\_2.sh

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_2=$path_script/$filename_script_2
file_script_export=$path_script/$filename_script_export
file_script_get_oracle_sid=$path_script/get_oracle_sid.sh
file_script_get_pwd=$path_script/get_pwd.sh
#=============================================================================

#=============================================================================
# Automatically set oracle_sid.
#-----------------------------------------------------------------------------
oracle_sid=`$file_script_get_oracle_sid $dbtype`
#=============================================================================

#=============================================================================
# Automatically set passwords.
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
    echo "ERROR: Please create this directory:"
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
  rm -f $file_log_tmp
  int_error_code=`expr $int_error_code + $?`
fi
#=============================================================================

#=============================================================================
# Execute Main Program.
#-----------------------------------------------------------------------------
command="$file_script_2 $oracle_sid $date_time"
comecho="$file_script_2 $oracle_sid $date_time"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1


command="$file_script_export $account_uc4admin $password_uc4admin $oracle_sid $date_time"
comecho="$file_script_export $account_uc4admin ****************** $oracle_sid $date_time"

echo "Executing $comecho..."  >> $file_log_tmp 2>&1
                $command      >> $file_log_tmp 2>&1
int_error_code=`expr $int_error_code + $?`
echo "Executed  $comecho."    >> $file_log_tmp 2>&1
echo                          >> $file_log_tmp 2>&1
#=============================================================================

#=============================================================================
# Copy export files to destinations on local machine.
#-----------------------------------------------------------------------------
if   [ "$oracle_sid" == 'edgist2q'        ] ||
     [ "$oracle_sid" == 'edgisp1q'        ] ||
     [ "$oracle_sid" == 'edgisp2q'        ] ; then 
  file_fr=/u02/uc4/$oracle_sid/export/$oracle_sid.dmp
  file_to=/u02/uc4/$oracle_sid/import/$oracle_sid.dmp
  if [ "$machine"    == 'edgisdboraprd01' ] ; then
    file_to=/u02/uc4/edgisp1p/import/$oracle_sid.dmp
  fi
  if [ -f "$file_to" ]; then
    command="rm -f $file_to"
    echo "Executing $command..."  >> $file_log_tmp 2>&1
                    $command      >> $file_log_tmp 2>&1
    int_error_code=`expr $int_error_code + $?`
    echo "Executed  $command."    >> $file_log_tmp 2>&1
    echo                          >> $file_log_tmp 2>&1
  fi
  command="cp $file_fr $file_to"
  echo "Executing $command..."  >> $file_log_tmp 2>&1
                  $command      >> $file_log_tmp 2>&1
  int_error_code=`expr $int_error_code + $?`
  echo "Executed  $command."    >> $file_log_tmp 2>&1
  echo                          >> $file_log_tmp 2>&1
fi
#=============================================================================

#============================================================================
# Check log file for problems.
#----------------------------------------------------------------------------
for search_string in "ERROR " "ERROR: " "insufficient privileges" "ORA-31693" "ORA-02354"
  do
    if grep -q "$search_string" "$file_log_tmp" ; then
      int_error_code=`expr $int_error_code + 1`
      echo Found search_string $search_string in $file_log_tmp. >> $file_log_tmp 2>&1
    fi
  done
#============================================================================

#============================================================================
# Copy temporary log file to date-stamped log filename.
#----------------------------------------------------------------------------
command="cp $file_log_tmp $file_log"

echo "Executing $command..."  >> $file_log_tmp 2>&1
                $command
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."    >> $file_log     2>&1
echo "Executed  $command."    >> $file_log_tmp 2>&1
echo                          >> $file_log     2>&1
echo                          >> $file_log_tmp 2>&1
#============================================================================

#============================================================================
# chmod log files.
#----------------------------------------------------------------------------
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
#============================================================================


#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code. >> $file_log_tmp 2>&1
echo Exiting $0 with error code $int_error_code. >> $file_log     2>&1
exit $int_error_code
#============================================================================
