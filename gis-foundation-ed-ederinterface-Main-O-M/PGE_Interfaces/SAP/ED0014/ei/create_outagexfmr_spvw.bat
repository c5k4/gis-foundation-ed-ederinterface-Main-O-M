REM Create WEBR.WIP_SPVW

set WEBR_USERNAME=WEBR
set WIP_INSTANCE=WIP
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD WEBR@WIP') do set "WEBR_PASSWORD=%%a"

REM sdetable -o delete -t PGE_OUTAGEXFMR_SPVW -N -i sde:oracle11g:%WIP_INSTANCE% -u %WEBR_USERNAME% -p %WEBR_PASSWORD%
sdetable -o create_view -T PGE_OUTAGEXFMR_SPVW -t PGE_TRANSFORMER,PGE_OUTAGEXFMR_VW -c PGE_OUTAGEXFMR_VW.OBJECTID,PGE_TRANSFORMER.SHAPE,PGE_OUTAGEXFMR_VW.OUTAGE_NO,PGE_OUTAGEXFMR_VW.XFMR_GUID,PGE_OUTAGEXFMR_VW.XFMR_ID,PGE_OUTAGEXFMR_VW.CUST_AFF,PGE_OUTAGEXFMR_VW.ESSENTIAL_CUST_AFF,PGE_OUTAGEXFMR_VW.CRITICAL_CUST_AFF,PGE_OUTAGEXFMR_VW.XFMR_KEY,PGE_OUTAGEXFMR_VW.XFMR_INT_ID,PGE_OUTAGEXFMR_VW.DISTRICT_NAME,PGE_OUTAGEXFMR_VW.FEEDER_ID,PGE_OUTAGEXFMR_VW.FEEDER_DESC,PGE_OUTAGEXFMR_VW.ETOR,PGE_OUTAGEXFMR_VW.OUTAGE_START_TIME,PGE_OUTAGEXFMR_VW.OIS_IVR_CAUSE_DESC,PGE_OUTAGEXFMR_VW.OUTAGE_LEVEL,PGE_OUTAGEXFMR_VW.OUTAGE_STATUS,PGE_OUTAGEXFMR_VW.CREW_STATUS_DESC,PGE_OUTAGEXFMR_VW.CUR_CUST_AFF,PGE_OUTAGEXFMR_VW.EQUIP_GUID,PGE_OUTAGEXFMR_VW.EQUIP_TYPE,PGE_OUTAGEXFMR_VW.EQUIP_NAME,PGE_OUTAGEXFMR_VW.EQUIP_CAT,PGE_OUTAGEXFMR_VW.HAZARD_LEVEL_CODE,PGE_OUTAGEXFMR_VW.HAZARD_LEVEL_DESC,PGE_OUTAGEXFMR_VW.LATITUDE,PGE_OUTAGEXFMR_VW.LONGITUDE -w "PGE_OUTAGEXFMR_VW.XFMR_GUID=PGE_TRANSFORMER.GLOBALID" -i sde:oracle11g:%WIP_INSTANCE% -u %WEBR_USERNAME% -p %WEBR_PASSWORD%

sdelayer -o register -l WEBR.PGE_OUTAGENOTIFICATION,SHAPE -e p -t ST_GEOMETRY -C outage_no,USER  -i sde:oracle11g:%WIP_INSTANCE% -u %WEBR_USERNAME% -p %WEBR_PASSWORD%

pause


