Prompt drop Procedure EDER_TO_SAP_UNIQUE_RCRD;
DROP PROCEDURE PGEDATA.EDER_TO_SAP_UNIQUE_RCRD
/

Prompt Procedure EDER_TO_SAP_UNIQUE_RCRD;
--
-- EDER_TO_SAP_UNIQUE_RCRD  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA."EDER_TO_SAP_UNIQUE_RCRD"
  (
  Unique_recordset OUT SYS_REFCURSOR,
    input_dm_di_flag IN VARCHAR2
     )
AS
BEGIN
  IF input_dm_di_flag = 'DI' THEN --Run for Daily Interface
  OPEN unique_recordset FOR SELECT EQUIPMENTID ,GUID,SPID,CURRENT_PROJECT ,STATUS_MESSAGE,STATUS ,CGC,CIRCUITID FROM
  (select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'STAGE'
    UNION
     select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'SPID'
    and GUID not in(select GUID from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate)
    and inserted_by = 'STAGE' and guid is not null)
    UNION
     select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'CID'
    and GUID not in(select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'STAGE'
    UNION
     select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'SPID'
    and GUID not in(select GUID from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate)
    and inserted_by = 'STAGE' and guid is not null))
    UNION
     select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'CGC'
    and GUID not in(select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'STAGE'
    UNION
     select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'SPID'
    and GUID not in(select GUID from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate)
    and inserted_by = 'STAGE' and guid is not null)
    UNION
     select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'CID'
    and GUID not in(select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'STAGE'
    UNION
     select GUID
    from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate) and inserted_by = 'SPID'
    and GUID not in(select GUID from pgedata.EDER_TO_SAP_status where date_inserted= TRUNC(sysdate)
    and inserted_by = 'STAGE' and guid is not null)))
    union
    select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS from pgedata.EDER_TO_SAP_status s1,
    (select guid g1,count(*) from pgedata.EDER_TO_SAP_status  where guid in(
    select guid from pgedata.EDER_TO_SAP_status
    where date_inserted= TRUNC(sysdate)
    and inserted_by = 'SPID') group by guid having count(*) = 1) s2 where s1.guid=s2.g1
    union
     select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS from pgedata.EDER_TO_SAP_status s1,
    (select guid g1,count(*) from pgedata.EDER_TO_SAP_status  where guid in(
    select guid from pgedata.EDER_TO_SAP_status
    where date_inserted= TRUNC(sysdate)
    and inserted_by = 'CID') group by guid having count(*) = 1) s2 where s1.guid=s2.g1
    union
     select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS from pgedata.EDER_TO_SAP_status s1,
    (select guid g1,count(*) from pgedata.EDER_TO_SAP_status  where guid in(
    select guid from pgedata.EDER_TO_SAP_status
    where date_inserted= TRUNC(sysdate)
    and inserted_by = 'CGC') group by guid having count(*) = 1) s2 where s1.guid=s2.g1
  );
  --Run for Data Migration
  ELSIF input_dm_di_flag = 'DM' THEN
  OPEN unique_recordset FOR SELECT EQUIPMENTID ,GUID,SPID,CURRENT_PROJECT ,STATUS_MESSAGE,STATUS ,CGC,CIRCUITID FROM
  (
  select EQUIPMENTID,    GUID,    SPID,    CURRENT_PROJECT,    STATUS_MESSAGE, STATUS  ,    CGC,
    CIRCUITID,    DATE_INSERTED,    INSERTED_BY,    FINAL_STATUS
    from pgedata.EDER_TO_SAP_status
    );
    END IF;
EXCEPTION
WHEN no_data_found THEN
  dbms_output.put_line('Error');
END EDER_TO_SAP_UNIQUE_RCRD;

/


Prompt Grants on PROCEDURE EDER_TO_SAP_UNIQUE_RCRD TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.EDER_TO_SAP_UNIQUE_RCRD TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE EDER_TO_SAP_UNIQUE_RCRD TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.EDER_TO_SAP_UNIQUE_RCRD TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE EDER_TO_SAP_UNIQUE_RCRD TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.EDER_TO_SAP_UNIQUE_RCRD TO IGPEDITOR
/
