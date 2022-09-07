Prompt drop Procedure GENERATE_ECTPSSD_REPORT;
DROP PROCEDURE ROBCAPP.GENERATE_ECTPSSD_REPORT
/

Prompt Procedure GENERATE_ECTPSSD_REPORT;
--
-- GENERATE_ECTPSSD_REPORT  (Procedure) 
--
CREATE OR REPLACE PROCEDURE ROBCAPP.GENERATE_ECTPSSD_REPORT
(ErrorCode OUT varchar2, ErrorMsg OUT varchar2 )
AS
     v_rowsProcessed Number;
     V_Run_date      date;
     v_log_status VARCHAR2(10);
     v_log_String VARCHAR2(50);
Begin
    V_Run_date := sysdate;

--  Generating log for EEP Report table truncation
    V_Log_String :='Truncating table ECTPSSD_REPORT ' || to_char(Sysdate,'mmddyyyy');

    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String);

--  Truncating Report table
    Execute immediate 'Truncate table ECTPSSD_REPORT';
    Execute immediate 'Truncate table CALC_ECTPSSD_REPORT';

--   Updating log successfully completion for Transformer Load Header truncation
    v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String,99999);

    V_Log_String :='Generating table ECTPSSD_REPORT ' || to_char(V_Run_date,'mmddyyyy');
    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String);

    Insert into ECTPSSD_REPORT
    (DIVISION,
    DISTRICT,
    CONTROL_CENTER,
    CIRCUITNAME,
    OPERATINGNUMBER,
    SSDGUID,
    SERVICEPOINTID,
    METERNUMBER,
    CUSTOMER_NAME  )
    select SP_Source.division,
    SP_Source.district,
    CC.CONTROL_CENTER,
    SP_Source.CIRCUITNAME,
    SP_Source.SOURCESIDEDEVICEID,
    SP_Source.SSDGUID,
    SP_Source.SERVICEPOINTID,
    SP_Source.METERNUMBER,
    ci.MAILNAME1
    from
    (select nvl(tr.division, LO.division) division, nvl(tr.district, LO.district) district, cs.SUBSTATIONNAME || ' ' || cs.FEEDERID CIRCUITNAME, tr.SOURCESIDEDEVICEID, tr.SSDGUID, sp.SERVICEPOINTID, sp.METERNUMBER
      from
      (select * from edgis.zz_mv_servicepoint where ESSENTIALCUSTOMERIDC = 'Y') sp
      left outer join
      ( select circuitid, globalid XMFR_GUID, '' Pri_GUID, Division, district, To_char(SOURCESIDEDEVICEID) SOURCESIDEDEVICEID, SSDGUID from edgis.zz_mv_transformer
        union all
        select circuitid, '' XMFR_GUID, globalid Pri_GUID,  Division, district, To_char(SOURCESIDEDEVICEID) SOURCESIDEDEVICEID, SSDGUID from edgis.zz_mv_primarymeter
        where globalid in (select primarymeterguid from edgis.zz_mv_servicepoint where transformerguid is null) --where NVL(CGC12,000000000000) not in (select NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER)
      ) tr
      on nvl(trim(sp.transformerguid), 'NODATA') = nvl(trim(tr.XMFR_GUID), 'NODATA') and nvl(trim(sp.primarymeterguid), 'NODATA') = nvl(trim(tr.Pri_GUID), 'NODATA')
      left outer join
      EDGIS.zz_mv_CIRCUITSOURCE cs
      on tr.circuitid=cs.circuitid
      left outer join LOOKUP.LOCAL_OFFICE LO
      on sp.LocalOfficeID = LO.LOCAL_OFFICE
    ) SP_Source
    left outer join LOOKUP.DISTRICTS DT
    on DT.DIST_# = SP_Source.DISTRICT --and DT.DIVISION = SP_Source.DIVISION
    left outer join
    LOOKUP.CONTROL_CENTERS CC
    on CC.ID = DT.CONTROL_CENTER
    left outer join
    Customer.customer_info ci
    on ci.SERVICEPOINTID = SP_Source.SERVICEPOINTID;
   /*
    select tr.division, tr.district, CC.CONTROL_CENTER,  cs.SUBSTATIONNAME || ' ' || cs.FEEDERID, tr.SOURCESIDEDEVICEID, tr.SSDGUID, sp.SERVICEPOINTID, sp.METERNUMBER, ci.MAILNAME1
      from edgis.zz_mv_servicepoint sp ,
           ( Select Division,district, Globalid, CIRCUITID,  To_char(SOURCESIDEDEVICEID) SOURCESIDEDEVICEID, SSDGUID  From  EdGIS.zz_mv_TRANSFORMER
             union all
             select Division,district,Globalid, CIRCUITID, to_char(SOURCESIDEDEVICEID) SOURCESIDEDEVICEID, SSDGUID  From  EdGIS.zz_mv_dcrectifier
             union all
             select Division,district,Globalid, CIRCUITID, ' ' SOURCESIDEDEVICEID,SSDGUID  From  EdGIS.zz_mv_Primarymeter ) tr,
           EDGIS.zz_mv_circuitsource cs,
           LOOKUP.DISTRICTS DT,
           LOOKUP.CONTROL_CENTERS CC,
           Customer.customer_info ci
     where sp.ESSENTIALCUSTOMERIDC = 'Y'
       and tr.globalid   (+) = sp.transformerguid
       and cs.CIRCUITID  (+) = tr.CIRCUITID
       and DT.DIST_#     (+) = tr.DISTRICT
       and DT.DIVISION   (+) = tr.DIVISION
       and CC.ID         (+) = DT.CONTROL_CENTER
       and ci.SERVICEPOINTID (+) = sp.SERVICEPOINTID;
  */
    v_rowsProcessed := SQL%ROWCOUNT;


    insert into CALC_ECTPSSD_REPORT
    select distinct c.ssdguid,
    decode(
        decode(instr(upper(physicalname),'EDGIS.'),1,substr(physicalname,7),physicalname),'DYNAMICPROTECTIVEDEVICE',
            decode((select subtypecd from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE where globalid=c.ssdguid),
            2, 'INTERRUPTER', '3', 'RECLOSER','8', 'SECTIONALIZER','DYNAMICPROTECTIVEDEVICE'),
        decode(instr(upper(a.physicalname),'EDGIS.'),1,substr(a.physicalname,7),a.physicalname)) physicalname
    from  ectpssd_report c ,
    sde.gdb_items a,
    edgis.pge_feederfednetwork_trace b
    where b.to_feature_globalid = c.ssdguid
    and a.objectid = b.to_feature_fcid ;

 /*
    select distinct c.ssdguid, a.physicalname
    from  ectpssd_report c ,
    sde.gdb_items a,
    edgis.pge_feederfednetwork_trace b
    where b.to_feature_globalid = c.ssdguid
    and a.objectid = b.to_feature_fcid ;
  */

    Update ECTPSSD_REPORT a set EQUIPMENT_TYPE =  (select physicalname From CALC_ECTPSSD_REPORT b Where b.ssdguid = a.SSDGUID);
--    v_rowsProcessed := SQL%ROWCOUNT;
    DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);

--  Updating success status for ECTPSSD Report Generation.
    v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String,v_rowsProcessed);
EXCEPTION
   WHEN OTHERS THEN
--   Updating Error status for Feeder Load Detail Generation.
        ErrorMsg  :=SQLERRM;
        ErrorCode :=SQLCODE;
     V_log_status := LOG_MIGRATION_ERROR('I', V_Log_String, sqlerrm);
     raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG '||to_char(V_Run_date,'mmddyyyy'));
END;
/
