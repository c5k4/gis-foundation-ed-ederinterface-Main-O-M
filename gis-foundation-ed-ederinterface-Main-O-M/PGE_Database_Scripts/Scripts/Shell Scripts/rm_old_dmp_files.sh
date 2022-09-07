#!/bin/sh
#============================================================================
# rm_old_dmp_files.sh
#----------------------------------------------------------------------------
# This script removes all files older than <days_old> with file names
# that end with .dmp in a directory <dmp_file_path>.
#
# USAGE:  rm_old_dmp_files.sh <dmp_file_path> <days_old>
#           
# Returns:      0=Script completed action Successfully
# Returns:      1=Fail
# MAINTENANCE HISTORY
# Date       Author           Description
# 2013_05_08 vdu1             initial version
#============================================================================

#=============================================================================
# Set script variables and environment
#-----------------------------------------------------------------------------
int_error_code=0
path_dmp_default=/u02/uc4/dmp
days_old_default=10
#=============================================================================

#============================================================================
# Set path_dmp
#----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  path_dmp=$path_dmp_default
else
  path_dmp=$1
fi
echo "path_dmp = $path_dmp"
#===========================================================================

#============================================================================
# Set days_old for removal
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
#===========================================================================

#============================================================================
echo "Removing export (*.dmp) files that are older than $days_old days old..."
#----------------------------------------------------------------------------
for file_dmp in `find $path_dmp/*.dmp -type f -mtime +$days_old`
do
  echo "Executing rm -f $file_dmp..."
                  rm -f $file_dmp
  int_error_code=`expr $int_error_code + $?`
  echo "Executed  rm -f $file_dmp."
  echo
done
echo "Removed  export (*.dmp) files that are older than $days_old days old."
echo
#============================================================================

#============================================================================
# Exit with error code
#----------------------------------------------------------------------------
echo Exiting $0 with error   code $int_error_code.
exit $int_error_code
#============================================================================
