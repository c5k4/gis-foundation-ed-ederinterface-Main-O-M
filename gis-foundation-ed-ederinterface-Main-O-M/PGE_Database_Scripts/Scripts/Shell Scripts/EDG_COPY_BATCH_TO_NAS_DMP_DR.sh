#!/bin/sh
#============================================================================
# EDG_COPY_BATCH_TO_NAS_DMP_DR.sh
#----------------------------------------------------------------------------
# USAGE:        EDG_COPY_BATCH_TO_NAS_DMP_DR.sh
# Returns:       0 = Script did     complete action successfully.
# Returns:      >0 = Script did not complete action successfully.
#
# Author:       Saurabh Gupta
#============================================================================

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

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_2=$path_script/$filename_script_2
file_script_get_oracle_sid=$path_script/get_oracle_sid.sh
file_script_get_pwd=$path_script/get_pwd.sh
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


#============================================================================
# Copy dump file from 
#----------------------------------------------------------------------------
echo " Copying edgisp4p dump file to NAS Location .." >> $file_log_tmp 2>&1
echo " Copying edgisp4p dump file to NAS Location .." >> $file_log 2>&1
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log_tmp 2>&1
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log 2>&1
echo $(date) >> $file_log 2>&1
echo $(date) >> $file_log_tmp 2>&1

max=4
i=0
dmpname=.dmp
while [ $((i+=1)) -le $max ]
do
srcfile=/u02/uc4/edgisp4p/export/edgisp4p\_0$i.dmp
scp  $srcfile  EDGISBTCPRD02:\\fairfield07\GIS\public\exta\EDGIS >> $file_log_tmp 2>&1
done


int_error_code=`expr $int_error_code + $?`
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log_tmp 2>&1
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log 2>&1
echo $(date) >> $file_log 2>&1
echo $(date) >> $file_log_tmp  2>&1
echo Error Code  $int_error_code >> $file_log_tmp 2>&1
echo Error Code  $int_error_code >> $file_log 2>&1
#============================================================================


#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error code $int_error_code. >> $file_log_tmp 2>&1
echo Exiting $0 with error code $int_error_code. >> $file_log     2>&1
exit $int_error_code
#============================================================================