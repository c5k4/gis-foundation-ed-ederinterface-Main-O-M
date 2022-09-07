-- INSERTS are new objects that did not exist in the previous schematics at all. Using the UGUID to find whole new objects.
-- Version 1.1 RR 11/7/2014 fixed I and U along with table create for table.
-- Version 1.2 RR 11/18/2014 fixed update so that it records the action table as well in the summary table.
-- Version 1.3 RR 11/18/2014 fixed update to ignore OID changes. If everything else stays the same, we don't mind if schematics is changing the OID.

CREATE TABLE sde.SCHEM_GUID_DELTA_TEMP (UGUID NVARCHAR2(38), SCH_TABLE NVARCHAR2(160), ACTION NVARCHAR2(1));
grant all on sde.SCHEM_GUID_DELTA_TEMP to public;

-- DELETES are where objects where removed from the UGUID and missing now. So they existing in pre and not in post.
insert into sde.SCHEM_GUID_DELTA_TEMP (UGUID, SCH_TABLE, ACTION)
select tab3.UGUID, tab3.SCH_TABLE, 'D' from 
(select distinct tab.UGUID, tab.SCH_TABLE from SCHEM_GUID_PRE_DROP_TEMP tab where tab.UGUID is not null 
minus 
select distinct tab2.UGUID, tab2.SCH_TABLE from SCHEM_GUID_POST_DROP_TEMP tab2 where tab2.UGUID is not null
) tab3;

-- INSERTS are new objects that did not exist in the previous schematics at all. so they existed in post and not in pre.
insert into sde.SCHEM_GUID_DELTA_TEMP (UGUID, SCH_TABLE, ACTION)
select tab3.UGUID, tab3.SCH_TABLE, 'I' from (
select distinct tab2.UGUID, tab2.SCH_TABLE from SCHEM_GUID_POST_DROP_TEMP tab2 
minus 
select distinct tab.UGUID, tab.SCH_TABLE from SCHEM_GUID_PRE_DROP_TEMP tab
) tab3;

-- UPDATES are detected where the UGUID has a different : OWNER,SCH_TABLE,UCID,UOID,UGUID,EMINX,EMINY,EMAXX,EMAXY,OID,USID
insert into sde.SCHEM_GUID_DELTA_TEMP (UGUID,SCH_TABLE,ACTION) 
SELECT unique_list.UGUID,unique_list.SCH_TABLE,'U'
FROM 
( 
   Select total.UGUID,total.SCH_TABLE from 
(
SELECT 
   tab1.OWNER,tab1.SCH_TABLE,tab1.UCID,tab1.UOID,tab1.UGUID,
   tab1.EMINX,tab1.EMINY,tab1.EMAXX,tab1.EMAXY,
   tab1.USID, 1 AS src1, TO_NUMBER(NULL) AS src2
from ( select * from SDE.SCHEM_GUID_PRE_DROP_TEMP t1 where t1.UGUID is not null and t1.UGUID in (select t2.UGUID from SDE.SCHEM_GUID_POST_DROP_TEMP t2 group by t2.UGUID) ) tab1
    UNION ALL
SELECT 
   tab2.OWNER,tab2.SCH_TABLE,tab2.UCID,tab2.UOID,tab2.UGUID,
   tab2.EMINX,tab2.EMINY,tab2.EMAXX,tab2.EMAXY,
   tab2.USID, TO_NUMBER(NULL) AS src1, 2 AS src2 
FROM ( select * from SDE.SCHEM_GUID_POST_DROP_TEMP t3 where t3.UGUID is not null and t3.UGUID in (select t4.UGUID from SDE.SCHEM_GUID_PRE_DROP_TEMP t4 group by t4.UGUID ) ) tab2
) TOTAL
GROUP BY 
OWNER,SCH_TABLE,UCID,UOID,UGUID,
EMINX,EMINY,EMAXX,EMAXY,
  USID HAVING COUNT(src1) <> COUNT(src2)
) 
unique_list
group by unique_list.UGUID,unique_list.SCH_TABLE;

/*
SQL> desc SCHEM_GUID_POST_DROP_TEMP
 Name                                      Null?    Type
 ----------------------------------------- -------- ---------------------
 OWNER                                              NVARCHAR2(160)
 SCH_TABLE                                          NVARCHAR2(160)
 UCID                                               NUMBER(38)
 UOID                                               NUMBER(38)
 UGUID                                              NVARCHAR2(38)
 EMINX                                              FLOAT(64)
 EMINY                                              FLOAT(64)
 EMAXX                                              FLOAT(64)
 EMAXY                                              FLOAT(64)
 OID                                       NOT NULL NUMBER(38)
 SCHEMATICTID                                       NVARCHAR2(256)
 STATUS                                             NUMBER(22)
 CIRCUITID                                          NVARCHAR2(18)
 OPERATINGVOLTAGE                                   NUMBER(22)
 SYMBOLNUMBER                                       NUMBER(22)
 CONVERSIONID                                       NUMBER(22)
 OPERATINGNUMBER                                    NVARCHAR2(18)
 DISTRICT                                           NUMBER(22)
 DIVISION                                           NUMBER(22)
 INSTALLATIONTYPE                                   NVARCHAR2(10)
 NORMALPOSITIONB                                    NUMBER(22)
 NORMALPOSITIONC                                    NUMBER(22)
 NORMALPOSITIONA                                    NUMBER(22)
 LABELTEXT2                                         NVARCHAR2(200)
 SUBTYPECD                                          NUMBER(22)
 CONVCIRCUITID                                      NVARCHAR2(18)
 WINDSPEEDCODE                                      NUMBER(22)
 RISERUSAGE                                         NUMBER(22)
 NAMEOFCOGENERATOR                                  NVARCHAR2(100)
 RATEDAMPS                                          NUMBER(22)
 LINKRATING                                         NUMBER(22)
 LINKTYPE                                           NUMBER(22)
 COMPLEXDEVICEIDC                                   NVARCHAR2(10)
 DEVICEGROUPNAME                                    NVARCHAR2(100)
 DEVICEGROUPTYPE                                    NUMBER(22)
 TOTALKVAR                                          NUMBER(22)
 TOTALKVR                                           NUMBER(22)
 
 SQL> select count(*) from SDE.SCHEM_GUID_PRE_DROP_TEMP ;

  COUNT(*)
----------
   1666596

SQL> select count(*) from SDE.SCHEM_GUID_POST_DROP_TEMP ;

  COUNT(*)
----------
   1666596   
   
set pagesize 10000
set linesize 120
column sch_table format A30
select action,sch_table,count(*) from sde.schem_guid_delta_temp group by action,sch_table;
ACTION  SCH_TABLE                        COUNT(*)
-       ------------------------------ ----------
U       SCH1284E_PRIUGCONDUCTOR             99734
U       SCH1284E_FAULTINDICATOR              6419
U       SCH1284E_ELECTRICSTITCHPOINT          968
U       SCH1284E_OPENPOINT                  61421
U       SCH1284E_DYNAMICPROTECTIVEDEVI       2398
U       SCH1284E_PRIMARYGENERATION            444
U       SCH1284E_STEPDOWN                     126
U       SCH1284E_PRIMARYRISER               19384
U       SCH1284E_ELECTRICDISTNETWORK_J     128687
U       SCH1284E_DISTBUSBAR                 92167
U       SCH1284E_PRIOHCONDUCTOR            424036
U       SCH1284E_CAPACITORBANK               4229
U       SCH1284E_VOLTAGEREGULATOR            2223
U       SCH1284E_TRANSFORMER               287038
U       SCH1284E_SWITCH                     39344
U       SCH1284E_FUSE                       45136
U       SCH1284E_SCHEMATICS_UNIFIED_GR       4126
U       SCH1284E_DEVICEGROUP                80316
U       SCH1284E_TIE                         8821
U       SCH1284E_PRIMARYMETER                 697
 
 */
 
 

