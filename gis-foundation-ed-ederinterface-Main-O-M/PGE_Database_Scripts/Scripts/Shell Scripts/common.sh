# INITIALIZE ALL THE PATH/LOGGING VARIABLES AND EXPORT THEM TO GLOBAL
createPathVariables()
{
  date_time=`date +%Y_%m_%d_%H_%M_%S`

  export EDVD_THIS_SCR=`basename $0 | cut -d'.' -f1`
  export EDVD_THIS_DIR="/u02/uc4/edgisdbmaint" 

  export filename_log=$EDVD_THIS_DIR"/log/"$EDVD_THIS_SCR\_$date_time.log
  
  echo created >> $filename_log
   
  chmod 777 "$filename_log"
  
  export exitcode=0
  
  return $?
 }

createConnectionVariables(){
echo ============================================================================
echo  INITIALIZE ALL THE ORACLE CONNECTION VARIABLES AND EXPORT THEM TO GLOBAL
echo ============================================================================
  # SET UP OUR CONNNECTION VARIABLES
  export oraclesid=`$EDVD_THIS_DIR/get_oracle_sid.sh datamart`
  export useruc4=uc4admin
  #RETURNS THE PASSWORD ASSOCIATED WITHTHE USER ACCOUNT AND SID PASSED IN
  export pwduc4=`$EDVD_THIS_DIR/get_pwd.sh $useruc4 $oraclesid`

  echo "Oracle SID :" $oraclesid
  echo "User :" $useruc4

  export ORACLE_BIN=/u01/app/oracle/product/11.2.0.3/db_1/bin
  echo "Oracle bin directory : " $ORACLE_BIN
  export ORACLE_HOME=/u01/app/oracle/product/11.2.0.3/db_1
  LIBPATH=$ORACLE_HOME/lib
  export LIBPATH
  PATH=.:/usr/bin:/etc:/usr/sbin:/usr/ucb:$HOME/bin:/usr/bin/X11:/sbin:$ORACLE_HOME/bin
  export PATH

  return $?
  
}

createConnectionVariablesforsub(){
echo ============================================================================
echo  INITIALIZE ALL THE ORACLE CONNECTION VARIABLES AND EXPORT THEM TO GLOBAL
echo ============================================================================
  # SET UP OUR CONNNECTION VARIABLES
  export oraclesid=`$EDVD_THIS_DIR/get_oracle_sid.sh datamartsub`
  export useruc4=uc4admin
  #RETURNS THE PASSWORD ASSOCIATED WITHTHE USER ACCOUNT AND SID PASSED IN
  export pwduc4=`$EDVD_THIS_DIR/get_pwd.sh $useruc4 $oraclesid`

  echo "Oracle SID :" $oraclesid
  echo "User :" $useruc4

  export ORACLE_BIN=/u01/app/oracle/product/11.2.0.3/db_1/bin
  echo "Oracle bin directory : " $ORACLE_BIN
  export ORACLE_HOME=/u01/app/oracle/product/11.2.0.3/db_1
  LIBPATH=$ORACLE_HOME/lib
  export LIBPATH
  PATH=.:/usr/bin:/etc:/usr/sbin:/usr/ucb:$HOME/bin:/usr/bin/X11:/sbin:$ORACLE_HOME/bin
  export PATH

  return $?
  
}

errorCheck(){
    
    filename_log="$1"
    
    echo "===================================================" >> $filename_log 2>&1
    echo "Error check - reading log file for possible problems" >> $filename_log 2>&1
    echo "Checking this file" "$filename_log" >> $filename_log 2>&1
    echo "===================================================" >> $filename_log 2>&1

    int_error_code=0
    for search_string in "ERROR " "ERROR: " "insufficient privileges"
    do
      if grep -q "$search_string" "$filename_log" ; then
        #echo $int_error_code
        int_error_code=`expr $int_error_code + 1`
        #echo $int_error_code
        echo "Found search_string $search_string in $filename_log"  2>&1
      fi
    done

  #echo "error code : $int_error_code"
  # echo $int_error_code
  return $int_error_code
}