#!/bin/sh
#============================================================================
# export_parallel.sh
#----------------------------------------------------------------------------
# USAGE:        export_parallel.sh
#                  account
#                  password
#                  oracle_sid
#                  date_time
#
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------
account_default=foo
password_default=foo
oracle_sid_default=foo

int_error_code=0
int_warning_code=0

path_script=$(dirname $0)
cd $path_script
path_script=`pwd`

path_log=$path_script/log
machine=`uname -n`
machine_num=`echo $machine | awk '{ print substr( $0, length($0) - 1, 2 ) }'`
#=============================================================================

#============================================================================
# Set account.
#----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  account=$account_default
else
  account=$1
fi
echo
echo "Account     = $account"
#============================================================================

#============================================================================
# Set password.
#----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  password=$password_default
else
  password=$2
fi
echo "Password    = ***************" 
#============================================================================

#============================================================================
# Set ORACLE_SID.
#----------------------------------------------------------------------------
if [ -z "$3" ] ; then
  oracle_sid=$oracle_sid_default
else
  oracle_sid=$3
fi
ORACLE_SID=`echo $oracle_sid | tr '[:lower:]' '[:upper:]'`

case $machine_num in
  "11") ORACLE_SID=$ORACLE_SID\1   ;;
  "12") ORACLE_SID=$ORACLE_SID\2   ;;
  "21") ORACLE_SID=$ORACLE_SID\1   ;;
  "22") ORACLE_SID=$ORACLE_SID\2   ;;
esac

export $ORACLE_SID

echo "machine     = $machine"
echo "machine_num = $machine_num"
echo "oracle_sid  = $oracle_sid"
echo "ORACLE_SID  = $ORACLE_SID"
#============================================================================

#============================================================================
# Set date_time.
#----------------------------------------------------------------------------
if [ -z "$4" ] ; then
  date_time=`date +%Y_%m_%d_%H_%M_%S`
else
  date_time=$4
fi
echo "date_time   = $date_time"
echo
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
#=============================================================================

#============================================================================
# Set Oracle Export Path
#----------------------------------------------------------------------------
UC4_EXPORT_DIR=/u02/uc4/$oracle_sid/export
UC4_IMPORT_DIR=/u02/uc4/$oracle_sid/import
export UC4_EXPORT_DIR
chmod g+s $UC4_EXPORT_DIR
chmod g+s $UC4_IMPORT_DIR

filename_export_tmp=$oracle_sid.dmp



filename_export_parallel_tmp=$oracle_sid\_


filename_export=$oracle_sid\_$date_time.dmp

filename_export_parallel=$oracle_sid\_$date_time\_


file_export_tmp=$UC4_EXPORT_DIR/$filename_export_tmp

file_export=$UC4_EXPORT_DIR/$filename_export

file_export_parallel=$UC4_EXPORT_DIR/$filename_export




filename_export_log=$oracle_sid\_$date_time.log
file_export_log=$path_log/$filename_export_log

echo "UC4_EXPORT_DIR   = $UC4_EXPORT_DIR"
echo "filename_export  = $filename_export"
echo "file_export      = $file_export"
echo "file_export_tmp  = $file_export_tmp"
echo
#============================================================================

#============================================================================
# Remove temp dump file, if it exists.
#----------------------------------------------------------------------------
if [ -f "$file_export_tmp" ]; then
  echo "Executing rm -f $file_export_tmp..."
                  rm -f $file_export_tmp
  int_error_code=`expr $int_error_code + $?`
  echo "Executed  rm -f $file_export_tmp."
  echo
fi
#============================================================================

#============================================================================
# Export oracle schema.
#----------------------------------------------------------------------------
command="expdp $account/$password@$ORACLE_SID compression=all schemas=sde,edgis,process,edgisbo,CEDSADATA,PGEDATA  reuse_dumpfiles=y directory=UC4_EXPORT_DIR dumpfile=$filename_export_parallel_tmp\%U.dmp logfile=$filename_export_log flashback_time=systimestamp CLUSTER=N PARALLEL=4"
comecho="expdp $account/password@$ORACLE_SID compression=all schemas=sde,edgis,process,edgisbo,CEDSADATA,PGEDATA   reuse_dumpfiles=y  directory=UC4_EXPORT_DIR dumpfile=$filename_export_parallel_tmp\%U.dmp logfile=$filename_export_log flashback_time=systimestamp CLUSTER=N PARALLEL=4"

#command="expdp $account/$password@$ORACLE_SID compression=all full=y reuse_dumpfiles=y directory=UC4_EXPORT_DIR dumpfile=$filename_export_tmp logfile=$filename_export_log flashback_time=systimestamp"
#comecho="expdp $account/password@$ORACLE_SID compression=all full=y reuse_dumpfiles=y  directory=UC4_EXPORT_DIR dumpfile=$filename_export_tmp logfile=$filename_export_log flashback_time=systimestamp"


#command="expdp $account/$password@$ORACLE_SID compression=all full=y reuse_dumpfiles=y exclude=statistics directory=UC4_EXPORT_DIR dumpfile=$filename_export_tmp logfile=$filename_export_log"
#comecho="expdp $account/password@$ORACLE_SID compression=all full=y reuse_dumpfiles=y exclude=statistics directory=UC4_EXPORT_DIR dumpfile=$filename_export_tmp logfile=$filename_export_log"



echo "Executing $comecho..."
               $command
#int_error_code=`expr $int_error_code + $?`
echo error   code $int_error_code.
echo "Executed  $comecho."
echo

sleep 300
#============================================================================

#============================================================================
# Check log file for problems.
#----------------------------------------------------------------------------
log_error_count=`grep -c "ORA-" "$UC4_EXPORT_DIR/$filename_export_log"`

if [ $log_error_count != 0 ] ; then
 
 for search_string in "ORA-31693" "ORA-02354" "ORA-39826" "ORA-06512" "ORA-39086" "ORA-31626" "ORA-01466" "ORA-31693" "ORA-02354"
  do
    log_error_count1=`grep -c $search_string "$UC4_EXPORT_DIR/$filename_export_log"`  
  
if [ $log_error_count1 != 0 ] ; then
    log_error_count=`expr $log_error_count - $log_error_count1`
    
     echo Found search_string $search_string in $UC4_EXPORT_DIR/$filename_export_log. 
fi
  done
     
fi

if [ $log_error_count != 0 ] ; then
int_error_code=`expr $int_error_code + $log_error_count`
echo "int_error    code" $int_error_code 
fi

#============================================================================


#============================================================================
# Copy Tmp Export File to Export File.
#----------------------------------------------------------------------------
max=4
i=0
dmpname=.dmp


while [ $((i+=1)) -le $max ]
do
srcfile=$UC4_EXPORT_DIR/$filename_export_parallel_tmp\0$i.dmp
destfile=$UC4_EXPORT_DIR/$filename_export_parallel\0$i.dmp
  command="cp $srcfile $destfile"
echo "Executing $command..."
               $command

int_warning_code=`expr $int_warning_code + $?`

echo "Executed  $command."


commandpermission="chmod 777 $srcfile"

echo "Executing $commandpermission..."
               $commandpermission

int_warning_code=`expr $int_warning_code + $?`

echo "Executed  $commandpermission."

commandpermission="chmod 777 $destfile"

echo "Executing $commandpermission..."
              $commandpermission

int_error_code=`expr $int_error_code + $?`

echo "Executed  $commandpermission."

done


#============================================================================


#============================================================================
# Remove old export (*.dmp) files.
#----------------------------------------------------------------------------
command="$path_script/rm_old_dmp_files.sh $UC4_EXPORT_DIR 5"

echo "Executing $command..."
                $command
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."
echo
#============================================================================


#============================================================================
# gzip old log (*.log) files.
#----------------------------------------------------------------------------
command="$path_script/gzip_old_log_files.sh $UC4_EXPORT_DIR 5"

echo "Executing $command..."
                $command
int_error_code=`expr $int_error_code + $?`
echo "Executed  $command."
echo
#============================================================================


#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with warning code $int_warning_code.
echo Exiting $0 with error   code $int_error_code.
exit $int_error_code
#============================================================================
