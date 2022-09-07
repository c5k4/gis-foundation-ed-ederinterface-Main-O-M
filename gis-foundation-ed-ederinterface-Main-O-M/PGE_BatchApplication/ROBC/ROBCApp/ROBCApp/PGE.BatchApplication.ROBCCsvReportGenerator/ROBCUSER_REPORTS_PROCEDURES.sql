Set echo on 
Set pagesize 0
Spool ROBCUSER_Reports_Procedures.log 

--------------------------------------------------------
--  DDL for Procedure GENERATE_ECTPSSD_REPORT
--------------------------------------------------------
set define off;

CREATE OR REPLACE PROCEDURE "ROBCAPP"."GENERATE_ECTPSSD_REPORT" 
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
--------------------------------------------------------
--  DDL for Procedure GENERATE_EEP_REPORT
--------------------------------------------------------
set define off;


CREATE OR REPLACE PROCEDURE "ROBCAPP"."GENERATE_EEP_REPORT" 
(ErrorCode OUT varchar2, ErrorMsg OUT varchar2 )
AS
    V_rowsProcessed Number;       
    V_Run_date      date; 
    V_log_status VARCHAR2(10);
    V_Log_String Varchar2(50);
    
    Cursor Cur_Circuit Is  
    Select EE.Circuitid, EE.FeederID, PARTCURTAILPOINTGUID 
    from EEP_REPORT EE, EDGIS.PGE_FEEDERFEDNETWORK_TRACE FT ,  EDGIS.zz_mv_PARTIALCURTAILPOINT PC
     where EE.PARTCURTAILPOINTGUID is not null 
       and pc.globalid = EE.PARTCURTAILPOINTGUID
       and FT.to_feature_globalid = pc.deviceguid 
       and FT.FEEDERID = EE.Circuitid; 

    Cursor Cur_CirID Is  
    Select distinct Circuitid from EEP_REPORT;

    Cursor Cur_Dep_Feeder is 
    Select distinct FM.FROM_CIRCUITID, FM1.TO_CIRCUITID 
    from EDGIS.PGE_FEEDERFEDNETWORK_MAP FM, EEP_REPORT EE,EDGIS.PGE_FEEDERFEDNETWORK_MAP FM1
    where EE.CIRCUITID = FM.FROM_CIRCUITID
    and FM.TO_CIRCUITID = FM1.FROM_CIRCUITID;

    V_circuitid  EEP_REPORT.circuitid%type;   
    V_FeederID   EEP_REPORT.FeederID%type;
    V_PARTCURTAILPOINTGUID EEP_REPORT.PARTCURTAILPOINTGUID%type;
    V_FROM_CIRCUITID  EDGIS.PGE_FEEDERFEDNETWORK_MAP.FROM_CIRCUITID%type;
    V_TO_CIRCUITID    EDGIS.PGE_FEEDERFEDNETWORK_MAP.TO_CIRCUITID%type;       
    
Begin 
    
--  Generating log for EEP Report table truncation
    V_Log_String :='Truncating table EEP_REPORT ' || to_char(Sysdate,'mmddyyyy');
    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

--  Truncating Report table
    Execute immediate 'Truncate table EEP_REPORT';
    Execute immediate 'Truncate table CALC_EEP_REPORT';
    
--   Updating log successfully completion for Transformer Load Header truncation
    v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,99999);
    
    V_Log_String :='Generating EEP_REPORT ' || to_char(Sysdate,'mmddyyyy');
    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

    Insert into EEP_REPORT( DIVISION, DISTRICT, DIVISION_NAME, DISTRINCT_NAME, CONTROL_CENTER_NAME, SUBID, FEEDERID,
    CIRCUITID, CIRCUITNAME, EQUIPMENT_TYPE, OPERATINGNUMBER, ROBC, SUBBLOCK, PARTCURTAILPOINTGUID  )
    select cs.division, cs.district,dv.DIV_NAME  , dt.DIST_NAME,CC.CONTROL_CENTER, substr(cs.circuitid,3,3) ,  cs.FeederId, 
           cs.circuitid,cs.SUBSTATIONNAME ||  '  ' || cs.circuitname, nvl(dpd.type,'OCB'),  dpd.OPERATINGNUMBER,  
           nvl(ROBC.ESTABLISHEDROBC, decode(nvl(EssentialCust_Cnt, 0), 0, null, 50)) ROBC, robc.ESTABLISHEDSUBBLOCK, robc.PARTCURTAILPOINTGUID
     from 
     EDGIS.zz_mv_circuitsource cs
     left outer join
      EDGIS.zz_mv_ROBC robc
      on cs.globalid =  ROBC.CIRCUITSOURCEGUID -- and ROBC.CIRCUITSOURCEGUID is not null
     left outer join
     (    
        select cs.circuitid, count(sp.GlobalID) EssentialCust_Cnt 
        from
        ( select circuitid, globalid XMFR_GUID, '' Pri_GUID from edgis.zz_mv_transformer
        union all
        select circuitid, '' XMFR_GUID, globalid Pri_GUID from edgis.zz_mv_primarymeter 
          where globalid in (select primarymeterguid from edgis.zz_mv_servicepoint where transformerguid is null) --where NVL(CGC12,000000000000) not in (select NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER)
        ) tr 
        left outer join (select * from edgis.zz_mv_servicepoint where ESSENTIALCUSTOMERIDC = 'Y')sp 
        on nvl(sp.transformerguid, 'NULL') = nvl(tr.XMFR_GUID, 'NULL') and nvl(sp.primarymeterguid, 'NULL') = nvl(tr.Pri_GUID, 'NULL')
        left outer join EDGIS.zz_mv_CIRCUITSOURCE cs 
        on tr.circuitid=cs.circuitid 
        group by cs.circuitid
      ) EssentialCust  /* When ROBC is NULL then check for Essential Customer. If Essential customer count is greater than 0 then ROBC = "E" */
      on cs.circuitid = EssentialCust.circuitid
      left outer join
      EDGIS.zz_mv_PARTIALCURTAILPOINT pc on pc.globalid = ROBC.PARTCURTAILPOINTGUID 
      left outer join
   ( select GLOBALID, decode(subtypecd, 2, 'Interrupter', '3', 'Recloser','8', 'Sectionalizer','DPD') TYPE, 
        CIRCUITID,OPERATINGNUMBER,subtypecd from edgis.zz_mv_dynamicprotectivedevice
     union all
     select GLOBALID,'SWITCH' TYPE, CIRCUITID,OPERATINGNUMBER,subtypecd from edgis.zz_mv_switch
    ) dpd on dpd.globalid =  pc.DEVICEGUID
    left outer join 
    LOOKUP.DISTRICTS DT on DT.DIST_# = CS.DISTRICT
    left outer join 
    lookup.divisions DV on DV.DIV_#  = CS.DIVISION
    left outer join 
    LOOKUP.CONTROL_CENTERS CC on CC.ID = DT.CONTROL_CENTER order by cs.CIRCUITID;
    
    
/*  
      select cs.division, cs.district,dv.DIV_NAME  , dt.DIST_NAME,CC.CONTROL_CENTER, substr(cs.circuitid,3,3) , 
      cs.FeederId, cs.circuitid,cs.SUBSTATIONNAME ||  '  ' || cs.circuitname, nvl(dpd.type,'OCB'),  dpd.OPERATINGNUMBER,  
      ROBC.ESTABLISHEDROBC, robc.ESTABLISHEDROBC, robc.ESTABLISHEDSUBBLOCK, robc.PARTCURTAILPOINTGUID
      from EDGIS.zz_mv_ROBC ROBC,
           EDGIS.zz_mv_circuitsource cs,
           EDGIS.zz_mv_ELECTRICSTITCHPOINT esp,
           EDGIS.zz_mv_PARTIALCURTAILPOINT pc,
           ( select GLOBALID, decode(subtypecd, 2, 'Interrupter', '3', 'Recloser','8', 'Sectionalizer','DPD') TYPE, CIRCUITID,OPERATINGNUMBER,subtypecd from edgis.zz_mv_dynamicprotectivedevice
             union all
             select GLOBALID,'SWITCH' TYPE, CIRCUITID,OPERATINGNUMBER,subtypecd from edgis.zz_mv_switch
            ) dpd, 
           LOOKUP.DISTRICTS DT, 
           lookup.divisions DV,
           LOOKUP.CONTROL_CENTERS CC
     Where ROBC.CIRCUITSOURCEGUID  = cs.globalid
       and ROBC.CIRCUITSOURCEGUID is not null
       and cs.deviceguid (+)    = esp.globalid
       and pc.globalid   (+)    = ROBC.PARTCURTAILPOINTGUID 
       and dpd.globalid  (+)    =  pc.DEVICEGUID
       and DT.DIST_#     (+) = CS.DISTRICT
       and DT.DIVISION   (+) = CS.DIVISION
       and DV.DIV_#      (+) = CS.DIVISION
       and CC.ID         (+) = DT.CONTROL_CENTER;
       
       */
     v_rowsProcessed := SQL%ROWCOUNT;

--  Updating success status for EEP Report Generation.
    v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,v_rowsProcessed);

    V_Log_String :='Generating Temp Table ' || to_char(Sysdate,'mmddyyyy');
    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

    Insert into CALC_EEP_REPORT ( circuitid , GLOBALID, NO_OF_CUST, SUMMERKVA, WINTERKVA )
    Select c.circuitid  , c.GLOBALID, max(NoofCust), sum(nvl(f.rev_kw/1000,0)) summ_peak_kw,sum(nvl(g.rev_kw/1000,0)) wntr_peak_kw
    from edtlm.trf_peak a, 
    edtlm.transformer b, 
    edgis.zz_mv_transformer c, 
    edtlm.trf_ccb_meter_load d, 
    edtlm.trf_ccb_meter_load e,
    edtlm.sp_ccb_meter_load f, 
    edtlm.sp_ccb_meter_load g, 
    (Select trf_id, count(*) NoofCust From EDtlm.meter group by Trf_id) h
     where a.trf_id = b.id 
       and b.global_id = c.globalid
       and d.TRF_ID (+)= a.trf_id
       and d.batch_date (+) = a.SMR_PEAK_DATE
       and e.TRF_ID (+)= a.trf_id
       and e.batch_date (+) = a.WNTR_PEAK_DATE
       and f.TRF_CCB_METER_LOAD_ID (+) = d.id 
       and g.TRF_CCB_METER_LOAD_ID (+) = e.id
       and h.trf_id (+) =  b.id
       group by c.circuitid  , c.GLOBALID;
     v_rowsProcessed := SQL%ROWCOUNT;

    Insert into CALC_EEP_REPORT ( circuitid , GLOBALID, NO_OF_CUST, SUMMERKVA, WINTERKVA )
    Select c.circuitid  , c.GLOBALID, max(NoofCust), sum(nvl(f.rev_kw/1000,0)) summ_peak_kw,sum(nvl(g.rev_kw/1000,0)) wntr_peak_kw
    from edtlm.trf_peak a, 
    edtlm.transformer b, 
    edgis.zz_mv_primarymeter c, 
    edtlm.trf_ccb_meter_load d, 
    edtlm.trf_ccb_meter_load e,
    edtlm.sp_ccb_meter_load f, 
    edtlm.sp_ccb_meter_load g, 
    (Select trf_id, count(*) NoofCust From EDtlm.meter group by Trf_id) h
     where a.trf_id = b.id 
       and b.cgc_id = c.CGC12
       and d.TRF_ID (+)= a.trf_id
       and d.batch_date (+) = a.SMR_PEAK_DATE
       and e.TRF_ID (+)= a.trf_id
       and e.batch_date (+) = a.WNTR_PEAK_DATE
       and f.TRF_CCB_METER_LOAD_ID (+) = d.id 
       and g.TRF_CCB_METER_LOAD_ID (+) = e.id
       and h.trf_id (+) =  b.id
     group by c.circuitid  , c.GLOBALID;
     v_rowsProcessed :=      v_rowsProcessed + SQL%ROWCOUNT;

    v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,v_rowsProcessed);

    V_Log_String :='Updating EEP Report ' || to_char(Sysdate,'mmddyyyy');
    v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

    Update EEP_REPORT EE Set (NO_OF_CUST,SUMMER_SIMUL_PEAK,SUMMER_PROJ_MW,SUMMER_MIN_MW,WINTER_SIM_PEAK,WINTER_PROJ_MW,WINTER_MIN_MW)
     = ( Select Sum(NO_OF_CUST), sum(SUMMERKVA),  sum(SUMMERKVA * 0.9),  sum(SUMMERKVA * 0.5),  sum(WINTERKVA), sum(WINTERKVA * 0.9), sum(WINTERKVA  * 0.5)
          From CALC_EEP_REPORT CEE
          Where EE.CIRCUITID = CEE.CIRCUITID )
     Where  PARTCURTAILPOINTGUID is Null;
     v_rowsProcessed := SQL%ROWCOUNT;

     Open Cur_Circuit; 
     Fetch Cur_Circuit into V_circuitid,V_FeederID, V_PARTCURTAILPOINTGUID;
     WHILE  Cur_Circuit%FOUND  LOOP
         Update EEP_REPORT EE Set (NO_OF_CUST,SUMMER_SIMUL_PEAK,SUMMER_PROJ_MW,SUMMER_MIN_MW,WINTER_SIM_PEAK,WINTER_PROJ_MW,WINTER_MIN_MW) 
          = ( Select Sum(NO_OF_CUST), sum(SUMMERKVA),  sum(SUMMERKVA * 0.9),  sum(SUMMERKVA * 0.5),  sum(WINTERKVA), sum(WINTERKVA * 0.9), sum(WINTERKVA * 0.5)
                From CALC_EEP_REPORT CEE
                where CEE.GLOBALID in ( select to_feature_globalid 
                                          from  edgis.pge_feederfednetwork_trace trc, 
                                            ( Select FeederID,treelevel,min_branch,max_branch,order_num 
                                              from EDGIS.PGE_FEEDERFEDNETWORK_TRACE FT , 
                                              EDGIS.zz_mv_PARTIALCURTAILPOINT PC
                                              where pc.globalid = V_PARTCURTAILPOINTGUID
                                              and FT.to_feature_globalid = pc.deviceguid 
                                              and FT.FEEDERID = V_circuitid ) TR
                                          where trc.TO_FEATURE_FCID in (select objectid from sde.gdb_items where physicalname in ('EDGIS.TRANSFORMER','EDGIS.PRIMARYMETER'))
                                          and trc.feederid=TR.FEEDERID 
                                          and trc.treelevel > tr.treelevel 
                                          and trc.min_branch >=  tr.min_branch 
                                          and trc.max_branch <=  tr.max_branch 
                                          and trc.order_num  <  tr.order_num ))
          Where  PARTCURTAILPOINTGUID = V_PARTCURTAILPOINTGUID
          and  FeederId = V_FeederID
          and  CircuitID = V_circuitid;  
            
          Fetch Cur_Circuit into V_circuitid,V_FeederID, V_PARTCURTAILPOINTGUID;
     END LOOP;
     Close Cur_Circuit; 

     v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,v_rowsProcessed);

     V_Log_String :='Updating SCADA  ' || to_char(Sysdate,'mmddyyyy');
     v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

     Open Cur_CirID; 
     Fetch Cur_CirID into V_circuitid;
     WHILE  Cur_CirID%FOUND  LOOP

          update EEP_REPORT EE set SCADA = ( select scada from edsett.sm_circuit_breaker cb, 
                                                    ( Select to_feature_globalid,order_num from edgis.pge_feederfednetwork_trace 
                                                       Where feederid in ( Select feederfedby from edgis.pge_feederfednetwork_trace 
                                                                            where feederid=V_circuitid 
                                                                            group by feederfedby )
                                                    ) nt
                                         Where cb.global_id=nt.to_feature_globalid
                                           and current_future ='C'
                                           and nt.order_num in 
                                                       ( Select min(order_num) From edsett.sm_circuit_breaker cb, 
                                                               ( select to_feature_globalid,order_num  from edgis.pge_feederfednetwork_trace 
                                                                  where feederid in ( select feederfedby from edgis.pge_feederfednetwork_trace 
                                                                                       where feederid=V_circuitid group by feederfedby )
                                                                ) nt
                                                          Where cb.global_id   = nt.to_feature_globalid
                                                            and current_future = 'C'
                                                            and order_num is not null ) )
           Where  EE.circuitid= V_circuitid;
          Fetch Cur_CirID into V_circuitid;
--          dbms_output.put_line(V_circuitid);
     END LOOP;
     Close Cur_CirID; 
     update EEP_REPORT EE set SCADA = 'N' where scada is null; 
     v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,0);
     
     V_Log_String :='Updating Supplier/Dependent Feeder ' || to_char(Sysdate,'mmddyyyy');
     v_log_status := INSERT_MIGRATION_LOG('I', V_Log_String );

 /*    update EEP_REPORT EE set ( SUP_FEEDER_NAME, SUP_FEEDER_ROBC, SUP_FEEDER_SUBBLOCK, SUPPLY_DEPEND ) =
         ( Select  CS.SUBSTATIONNAME || ' ' ||  cs.CIRCUITNAME, rb.DESIREDROBC, rb.DESIREDSUBBLOCK, 'Dependant' 
             from edgis.circuitsource CS, EDGIS.robc rb , EDGIS.PGE_FEEDERFEDNETWORK_MAP FM, EDGIS.PGE_FEEDERFEDNETWORK_MAP FM1
            Where RB.CIRCUITSOURCEGUID  = cs.globalid
              and RB.CIRCUITSOURCEGUID is not null
              and rb.PARTCURTAILPOINTGUID is null
              and CS.circuitid (+) = FM1.FROM_CIRCUITID 
              and ee.circuitid = FM.TO_CIRCUITID
              and FM1.TO_CIRCUITID = FM.FROM_CIRCUITID  ); 
*/

 /*
     update EEP_REPORT EE set ( SUP_FEEDER_NAME, SUP_FEEDER_ROBC, SUP_FEEDER_SUBBLOCK, SUPPLY_DEPEND ) =
         ( Select  distinct CS.SUBSTATIONNAME || ' ' ||  cs.CIRCUITNAME, rb.ESTABLISHEDROBC, rb.ESTABLISHEDSUBBLOCK, 'Dependant'
            from edgis.zz_mv_circuitsource CS, EDGIS.zz_mv_robc rb ,
                  ( Select max(FM1.FROM_CIRCUITID) FROM_CIRCUITID, fm.to_circuitid from EDGIS.PGE_FEEDERFEDNETWORK_MAP FM, EDGIS.PGE_FEEDERFEDNETWORK_MAP FM1
                     where FM1.TO_CIRCUITID = FM.FROM_CIRCUITID
                     group by fm.to_circuitid  )  Fm2
            Where RB.CIRCUITSOURCEGUID  = cs.globalid
            and CS.circuitid (+) = FM2.FROM_CIRCUITID
            and ee.circuitid = fm2.to_CIRCUITID);
*/

  -- If the current circuit ee.circuitid in (50,60) then use the max(rb.ESTABLISHEDROBC) for the parent and min(rb.ESTABLISHEDSUBBLOCK) for the parent.
  -- If the current circuit ee.circuitid is null  then use the  max(rb.ESTABLISHEDROBC) for the parent and min(rb.ESTABLISHEDSUBBLOCK) for the parent.
  -- If the current circuit ee.circuitid <> 50,60,null then use the  min(rb.ESTABLISHEDROBC) for the parent and max(rb.ESTABLISHEDSUBBLOCK) for the parent. 
    update EEP_REPORT EE set (SUP_FEEDER_NAME, SUP_FEEDER_ROBC, SUP_FEEDER_SUBBLOCK, SUPPLY_DEPEND ) =
    (Select distinct  CS.SUBSTATIONNAME || ' ' ||  cs.CIRCUITNAME SUP_FEEDER_NAME, 
     decode
    (
      nvl((select distinct 50 from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid) and nvl(ESTABLISHEDROBC, 50) in (50,60)), 50),
       50,(select nvl(max(r.ESTABLISHEDROBC), 50) from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid)),
      (select min(r.ESTABLISHEDROBC) from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid))
    ) SUP_FEEDER_ROBC,
    decode
    (
      nvl((select distinct 50 from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid) and nvl(ESTABLISHEDROBC, 50) in (50,60)), 50),
       50, null, --(select min(r.ESTABLISHEDSUBBLOCK) from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid)),
      (select max(r.ESTABLISHEDSUBBLOCK) from EDGIS.zz_mv_robc r where r.CIRCUITSOURCEGUID = (select globalid from edgis.zz_mv_circuitsource where circuitid=CS.circuitid))
    ) SUP_FEEDER_SUBBLOCK,
    'Dependant' SUPPLY_DEPEND
    from edgis.zz_mv_circuitsource CS, EDGIS.zz_mv_robc rb,
    ( Select max(FM1.FROM_CIRCUITID) FROM_CIRCUITID, fm.to_circuitid 
      from EDGIS.PGE_FEEDERFEDNETWORK_MAP FM, EDGIS.PGE_FEEDERFEDNETWORK_MAP FM1
      where FM1.TO_CIRCUITID = FM.FROM_CIRCUITID
      group by fm.to_circuitid  
    ) Fm2
    Where RB.CIRCUITSOURCEGUID  = cs.globalid
    and CS.circuitid (+) = FM2.FROM_CIRCUITID
    and ee.circuitid = fm2.to_CIRCUITID);
             
     v_rowsProcessed :=      SQL%ROWCOUNT;

     Open Cur_Dep_Feeder; 
     Fetch Cur_Dep_Feeder into V_FROM_CIRCUITID,V_TO_CIRCUITID;
     
     WHILE Cur_Dep_Feeder%FOUND  LOOP 
         Update EEP_REPORT EE set ( DEP_FEEDER_NAME, SUPPLY_DEPEND ) =
              ( Select Decode(EE.DEP_FEEDER_NAME , Null,CS.SUBSTATIONNAME || ' ' || cs.CIRCUITNAME, EE.DEP_FEEDER_NAME || ',' || CS.SUBSTATIONNAME || ' ' || cs.CIRCUITNAME),'Supply' 
                from edgis.zz_mv_circuitsource CS 
                Where cs.circuitid = V_To_CircuitID   )
          Where ee.circuitid = V_From_CircuitID; 
          v_rowsProcessed :=  v_rowsProcessed +    SQL%ROWCOUNT;

          Fetch Cur_Dep_Feeder into V_FROM_CIRCUITID,V_TO_CIRCUITID;
--          dbms_output.put_line(V_circuitid);
     END LOOP;
     
     v_log_status := LOG_MIGRATION_SUCCESS('I', V_Log_String ,v_rowsProcessed);

EXCEPTION
   WHEN OTHERS THEN
--   Updating Error status for Feeder Load Detail Generation.
        ErrorMsg  :=SQLERRM;
        ErrorCode :=SQLCODE;
        v_log_status := LOG_MIGRATION_ERROR('I', V_Log_String, sqlerrm);
        raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG '||to_char(V_Run_date,'mmddyyyy'));
END;
/

spool off 
