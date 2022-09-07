#!/bin/sh
#=============================================================================
# gunzip_old_files.sh
#-----------------------------------------------------------------------------
# This script uncompresses all files older than <days_old> with file names
# that end with .gz in a directory <file_path>.
#
# USAGE:  gunzip_old_files.sh <file_path> <days_old>
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
path_file_default=/u02/uc4/export
days_old_default=10
#=============================================================================

#=============================================================================
# Set path_file.
#-----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  path_file=$path_file_default
else
  path_file=$1
fi
echo "path_file = $path_file"
#=============================================================================

#=============================================================================
# Set days_old for gunzip.
#-----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  days_old=$days_old_default
else
  days_old=$2
fi
# This converts the string variable $days_old to a number:
days_old=$( echo "$days_old + 0" | bc )
echo "days_old = $days_old"
echo
#=============================================================================

#=============================================================================
echo "gunzipping *.gz files that are older than $days_old days old..."
#-----------------------------------------------------------------------------
for file_gz in `find $path_file/*.gz -type f -mtime +$days_old`
do
  echo "Executing gunzip $file_gz..."
                  gunzip $file_gz
  int_error_code=`expr $int_error_code + $?`
  echo "Executed  gunzip $file_gz."
  echo
done
echo "gunzipped  *.gz files that are older than $days_old days old."
echo
#=============================================================================

#=============================================================================
# Exit with error code.
#-----------------------------------------------------------------------------
echo Exiting $0 with error   code $int_error_code.
exit $int_error_code
#=============================================================================
