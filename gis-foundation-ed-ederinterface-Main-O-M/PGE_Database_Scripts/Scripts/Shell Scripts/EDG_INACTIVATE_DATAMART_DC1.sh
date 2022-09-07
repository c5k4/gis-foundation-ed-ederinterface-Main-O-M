#!/bin/sh

. /u02/uc4/edgisdbmaint/common.sh 


# LOOP OVER ALL OF THE .SQL FILES IN THE SQL FOLDER UNDER THE MAIN PATH 
# AND EXECUTE EACH OF THEM INDIVIDUALLY
main()
{
$ORACLE_BIN/sqlplus -S<<EOF
$useruc4/$pwduc4@$oraclesid
exec sys.PGE_STOP_SERVICE('eddatm_prd');
EOF
}

#################################   MAIN ENTRY   #####################################

# USE PREFIX SO THAT THESE VARIABLES ARE MOST LIKELY NOT IN OTHER SCRIPTS

createPathVariables  

echo "\n"
echo "LOG FILE :" $filename_log 
echo "This computer is :  `uname -n`" >> $filename_log 2>&1
echo "This user is     :  $USER" >> $filename_log 2>&1
echo "This script is   :  $EDVD_THIS_SCR" >> $filename_log 2>&1
echo "This directory is:  $EDVD_THIS_DIR" >> $filename_log 2>&1

echo ""
echo "===================================================">> $filename_log 2>&1
echo "createConnectionVariables">>$filename_log 2>&1
echo "===================================================">> $filename_log 2>&1
createConnectionVariables >> $filename_log 2>&1

echo "===================================================" >> $filename_log 2>&1
echo "Main (logic)" >> $filename_log 2>&1
echo "===================================================" >> $filename_log 2>&1

main >> $filename_log 2>&1

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

