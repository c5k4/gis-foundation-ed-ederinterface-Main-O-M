#!/bin/sh
#set -x

. /u02/uc4/edgisdbmaint/common.sh

# LOOP OVER ALL OF THE .SQL FILES IN THE SQL FOLDER UNDER THE MAIN PATH 
# AND EXECUTE EACH OF THEM INDIVIDUALLY
main()
{

  if [ -d $ORACLE_BIN ]
  then
    for file in $EDVD_THIS_DIR/SQL/*.sql
        do
          echo Executing file $file out of the $path_script/SQL/ directory

$ORACLE_BIN/sqlplus -S<<EOF
$useruc4/$pwduc4@$oraclesid
@$file
EOF

            exitcode=`expr $exitcode + $?`  
            echo Finished with $file
        done
  else
    echo Unable to find SQL directory : $ORACLE_BIN
    return  1
  fi

  return $exitcode
}

mainsub()
{

  if [ -d $ORACLE_BIN ]
  then
    for file in $EDVD_THIS_DIR/SQL/Sub/*.sql
        do
          echo Executing file $file out of the $path_script/SQL/Sub directory

$ORACLE_BIN/sqlplus -S<<EOF
$useruc4/$pwduc4@$oraclesid
@$file
EOF

            exitcode=`expr $exitcode + $?`  
            echo Finished with $file
        done
  else
    echo Unable to find SQL directory : $ORACLE_BIN
    return  1
  fi

  return $exitcode
}

echo ============================================================================
echo ============================================================================
echo ============================================================================
echo ============================================================================
echo                            $0
echo ============================================================================
echo ============================================================================
echo ============================================================================
echo ============================================================================

createPathVariables  

echo "\n"
echo "LOG FILE :" >> $filename_log 2>&1
echo "This computer is :  $USER" >> $filename_log 2>&1
echo "This user is     :  "`uname -n` >> $filename_log 2>&1
echo "This script is   :  $0" >> $filename_log 2>&1
echo "This directory is:  $EDVD_THIS_DIR" >> $filename_log 2>&1


createConnectionVariables >> $filename_log 2>&1

echo "===================================================" >> $filename_log 2>&1
echo "Main (logic)" >> $filename_log 2>&1
echo "===================================================" >> $filename_log 2>&1

main >> $filename_log 2>&1

createConnectionVariablesforsub >> $filename_log 2>&1

echo "===================================================" >> $filename_log 2>&1
echo "Mainsub (logic)" >> $filename_log 2>&1
echo "===================================================" >> $filename_log 2>&1

mainsub >> $filename_log 2>&1

exitcode=`expr $exitcode + $?`

chmod 777 "$filename_log"
            
errorCheck "$filename_log" >> $filename_log 2>&1

exitcode=`expr $exitcode + $?`

echo "Exiting with code : $exitcode" >> $filename_log 2>&1

# NOTE : "$?" IS FOR THE LAST RETURNED VALUE
# echo "  " > $filename_log
echo "\n$(cat $filename_log)" > $filename_log
echo "Process ended    :  `date +%H:%M:%S`  `date +%m.%d.%Y` $(cat $filename_log)"  > $filename_log
echo "\n$(cat $filename_log)" > $filename_log
echo "Process started  :  `date +%H:%M:%S`  `date +%m.%d.%Y` $(cat $filename_log)" > $filename_log
echo "\n$(cat $filename_log)" > $filename_log
echo "Exited with code :  $exitcode $(cat $filename_log)" > $filename_log
echo "\n$(cat $filename_log)" > $filename_log

cat $filename_log

exit $exitcode
