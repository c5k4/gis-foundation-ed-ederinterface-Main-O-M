#!/bin/sh
#============================================================================
# EDG_Copy_WIP_to_DataMart_DC2.sh
#----------------------------------------------------------------------------
# USAGE:        EDG_Copy_WIP_to_DataMart_DC1.sh
# Returns:       0 = Script did     complete action successfully.
# Returns:      >0 = Script did not complete action successfully.
#
# Author:       Saurabh Gupta converted for WIP  by Twyla 
#============================================================================
dbtype=redline
machine=`uname -n`

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

date_time=`date +%Y_%m_%d_%H_%M_%S`

filename_script_base=`basename $0 | cut -d'.' -f1`
filename_log_tmp=$filename_script_base.log
filename_log=$filename_script_base\_$date_time.log

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log

file_script_get_oracle_sid=$path_script/get_oracle_sid.sh

#=============================================================================
# Automatically set oracle_sid.
#-----------------------------------------------------------------------------
oracle_sid=`$file_script_get_oracle_sid $dbtype`
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
# Removing destination dump file 
#----------------------------------------------------------------------------
echo $(date) >> $file_log_tmp 2>&1
echo $(date) >>  $file_log 2>&1

if [ "$machine" == 'edgisdboraprd01' ] ; then
   echo  " Removing $oracle_sid dump file from edgisdboraprd06 .." >> $file_log_tmp 2>&1
   echo  " Removing $oracle_sid dump file from edgisdboraprd06 .." >> $file_log 2>&1
   ssh -q edgisdboraprd06 'rm /u02/uc4/eddatm2p/import/$oracle_sid.dmp' >> $file_log_tmp 2>&1
else
   if [ "$machine" == 'edgisdboraqa01' ] ; then
      echo  " Removing $oracle_sid dump file from edgisdboraqa04  .." >> $file_log_tmp 2>&1
      echo  " Removing $oracle_sid dump file from edgisdboraqa04  .." >> $file_log 2>&1
      ssh -q edgisdboraqa04  'rm /u02/uc4/eddatamq/import/$oracle_sid.dmp' >> $file_log_tmp 2>&1
   else
      echo  " Machine not configured for remote file removal .. " >> $file_log_tmp 2>&1
      echo  " Machine not configured for remote file removal .. " >> $file_log 2>&1
      int_error_code=`expr $int_error_code + 1`
   fi
fi

#============================================================================
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log_tmp 2>&1
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log 2>&1

#============================================================================
# Copy dump file from 
#----------------------------------------------------------------------------

if [ "$machine" == 'edgisdboraprd01' ] ; then
   echo  " Copying $oracle_sid dump file to edgisdboraprd06 .." >> $file_log_tmp 2>&1
   echo  " Copying $oracle_sid dump file to edgisdboraprd06 .." >> $file_log 2>&1
   scp  /u02/uc4/$oracle_sid/export/$oracle_sid.dmp  edgisdboraprd06:/u02/uc4/eddatm2p/import >> $file_log_tmp 2>&1
else
   if [ "$machine" == 'edgisdboraqa01' ] ; then
      echo  " Copying $oracle_sid dump file to edgisdboraqa04  .." >> $file_log_tmp 2>&1
      echo  " Copying $oracle_sid dump file to edgisdboraqa04  .." >> $file_log 2>&1
      scp  /u02/uc4/$oracle_sid/export/$oracle_sid.dmp  edgisdboraqa04:/u02/uc4/eddatamq/import >> $file_log_tmp 2>&1
   else
      echo  " Machine not configured for rcp, copying from export to import directory .. " >> $file_log_tmp 2>&1
      echo  " Machine not configured for rcp, copying from export to import directory .. " >> $file_log 2>&1
      # cp for testing since keys not exchanged to make ssh work
      cp /u02/uc4/$oracle_sid/export/$oracle_sid.dmp /u02/uc4/$oracle_sid/import >> $file_log_tmp 2>&1
      int_error_code=`expr $int_error_code + 1`
   fi
fi
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log_tmp 2>&1
echo `date +%Y_%m_%d_%H_%M_%S` >> $file_log 2>&1
int_error_code=`expr $int_error_code + $?`
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

