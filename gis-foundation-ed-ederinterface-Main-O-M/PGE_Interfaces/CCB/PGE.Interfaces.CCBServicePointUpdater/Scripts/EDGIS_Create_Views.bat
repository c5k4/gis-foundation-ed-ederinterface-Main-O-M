::This script needs to be run as part of the setup and config of the CC&B interface
::Put this file in the same directory as the SERVICEPOINT_TAB_CREATE.sql file
@ECHO off

IF %1.==. IF %2.==. IF %3.==. GOTO NoLoginAndPassAndDB

::Creates a multiversioned view of the EDGIS.Servicepoint table using the specified user (should be EDGIS)
sdetable -o create_mv_view -T zz_mv_servicepoint -t servicepoint -i sde:oracle11g:%1 -u %2 -p %3

::Creates a multiversioned view of the EDGIS.CWOT table using the specified user (should be EDGIS)
sdetable -o create_mv_view -T zz_mv_cwot -t cwot -i sde:oracle11g:%1 -u %2 -p %3

::Creates a multiversioned view of the EDGIS.Transformer table using the specified user (should be EDGIS)
sdetable -o create_mv_view -T zz_mv_transformer -t transformer -i sde:oracle11g:%1 -u %2 -p %3

GOTO End1

::If the number of arguments does not equal 3, go to this error message
:NoLoginAndPassAndDB
	ECHO Incorrect database, login, and/or password. 

ECHO "Usage: <filename.bat> <Database> <Login> <Password>"

:End1