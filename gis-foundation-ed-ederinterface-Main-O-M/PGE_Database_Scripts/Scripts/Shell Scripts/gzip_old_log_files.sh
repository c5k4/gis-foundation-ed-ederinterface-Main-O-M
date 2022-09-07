#!/bin/sh
#=============================================================================
# gzip_old_log_files.sh
#-----------------------------------------------------------------------------
# This script compresses all files older than <days_old> with file names
# that end with .log in a directory <log_file_path>.
#
# USAGE:  gzip_old_log_files.sh <log_file_path> <days_old>
#           
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
#
# Author:       Vince Ulfig vulfig@gmail.com 4157101998
#=============================================================================

#=============================================================================
# Set script variables.
#-----------------------------------------------------------------------------
int_error_code=0
path_log_default=/u02/uc4/export
days_old_default=10
#=============================================================================

#=============================================================================
# Set path_log.
#-----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  path_log=$path_log_default
else
  path_log=$1
fi
echo "path_log = $path_log"
#============================================================================

#============================================================================
# Set days_old for removal.
#----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  days_old=$days_old_default
else
  days_old=$2
fi
# This converts the string variable $days_old to a number:
days_old=$( echo "$days_old + 0" | bc )
echo "days_old = $days_old"
echo
#============================================================================

#============================================================================
echo "gzipping *.log files that are older than $days_old days old..."
#----------------------------------------------------------------------------
for file_log in `find $path_log/*.log -type f -mtime +$days_old`
do
  echo "Executing gzip $file_log..."
                  gzip $file_log
  int_error_code=`expr $int_error_code + $?`
  echo "Executed  gzip $file_log."
  echo
done
echo "gzipped  *.log files that are older than $days_old days old."
echo
#============================================================================

#============================================================================
# Exit with error code.
#----------------------------------------------------------------------------
echo Exiting $0 with error   code $int_error_code.
exit $int_error_code
#============================================================================
