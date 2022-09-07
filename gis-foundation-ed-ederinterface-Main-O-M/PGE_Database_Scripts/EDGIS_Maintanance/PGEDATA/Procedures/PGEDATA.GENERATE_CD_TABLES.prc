Prompt recreate Procedure GENERATE_CD_TABLES;
set define off;

CREATE OR REPLACE 
PROCEDURE         pgedata.generate_cd_tables
As
/*
Purpose:  Generate change records for transformer, transformer unit, and
          service point for consumption by TLM in the cd_transformer,
          cd_transformer_bank and cd_meter tables to synchronize EDTLM's data
          to EDGIS's.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   ???          ???        Initial coding - IBM I belive
   TJJ4        06/25/19    added header to document the procedure, formatted the sql
                           updated the join condition between service point and
                           primary meter to use the primaryMeterGuid column instead
                           of cgc12
   TJJ4        01/15/20    remove the reliance on the Version.  The process fails
                           to detect some changes that should be going to EDTLM
                           by removing the Version we can identify if it's the
                           source of the problem
   TJJ4        01/28/20    Not create a Replace action when the replaceGuid is
                           still a record
                           Create a Delete action for deleted cwots/orphaned trf units
                           Address issue when transformerguid in servicepoint no
                           longer exists
                           Treat a cwot added to a Trf as an Insert
                           Do not send updates for Orphaned records/or update
                           TLM CD code to update everything EXCEPT the relationship
                           (you many not orphan records in tlm)
                           Do not send both a R and a D, or (an I, R and D)
   TJJ4        05/06/20    Code merge for Svc_phase

*/
   V_Date          date;
   DB_ERROR_CODE   VARCHAR2(20);
   DB_ERROR_MSG    VARCHAR2(200);
   V_Action_name   Varchar2(50);
   V_Status        Varchar2(50);
   v_rowsProcessed Number;
   v_count         NUMBER;

   V_Default_version sde.versions.State_id%type;
   V_Chgdtl_version  sde.versions.State_id%type;
begin
   V_Action_name := 'Truncating Before  Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed := 0 ;
   dbms_output.put_line(V_Action_name||' '||V_Status);
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_b';
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_BANK_b' ;
   execute immediate 'truncate table PGEDATA.Temp_CD_METER_b' ;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Generating Before Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed := 0 ;
   DBMS_OUTPUT.PUT_LINE('0');
-- a rough count of what's in the a tables, if they're empty somethings probably
-- wrong but we can use the version to populate the b tables instead of the a tables
-- to autocorrect
   SELECT count(1)
     INTO v_count
     FROM (SELECT 1
             FROM pgedata.temp_cd_transformer_a
           UNION ALL
           SELECT 1
             FROM pgedata.temp_cd_transformer_bank_a
           UNION ALL
           SELECT 1
             FROM pgedata.temp_cd_meter_a) tmp;
   IF v_count < 7000000 THEN   -- use the version
      sde.version_util.set_current_version('GIS_I.Change_Detection_Sync_Tlm');
      DBMS_OUTPUT.PUT_LINE('1.1');

      Insert into PGEDATA.Temp_CD_transformer_b
             (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
              INSTALLATION_TYP, TRANS_DATE, OLD_GLOBAL_ID, LOWSIDE_VOLTAGE,
              OPERATING_VOLTAGE, CIRCUIT_ID, VAULT )
      (SELECT CGC12, GLOBALID, COASTALIDC, CLIMATEZONE,
              INSTALLATIONTYPE, DATEMODIFIED, REPLACEGUID, LOWSIDEVOLTAGE,
              OPERATINGVOLTAGE, CIRCUITID, VAULT
         from edgis.zz_mv_transformer
       UNION
       SELECT CGC12, GLOBALID, COASTALIDC, CLIMATEZONE,
              INSTALLATIONTYPE, DATEMODIFIED, REPLACEGUID, SECONDARYVOLTAGE,
              OPERATINGVOLTAGE, CIRCUITID, VAULT
         from edgis.zz_mv_primarymeter);

      DBMS_OUTPUT.PUT_LINE('2.1');

      Insert into PGEDATA.Temp_CD_transformer_BANK_b
             (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_DATE,
              OLD_GLOBAL_ID, TRF_TYP )
      (SELECT TRANSFORMERGUID, BANKCODE, RATEDKVA, GLOBALID, NUMBEROFPHASES, DATEMODIFIED,
              REPLACEGUID, TRANSFORMERTYPE
         from edgis.zz_mv_transformerunit
       UNION
       SELECT GLOBALID, NVL(BANKCODE, 1), NVL(RATEDKVA,9999), REPLACE(GLOBALID,'-','_'),
              NUMBEROFPHASES, DATEMODIFIED,
              REPLACE(REPLACEGUID,'-','_'),NVL(TRANSFORMERTYPE,'99')
         from edgis.zz_mv_primarymeter);

      DBMS_OUTPUT.PUT_LINE('3.1');

      -- Insert TRF_GLOBAL_ID from servicepoint when TRANSFORMERGUID in Service Point is not null,
      -- else use primarymeterguid
      Insert into PGEDATA.TEMP_CD_METER_B
            (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
             SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
             GLOBAL_ID, METER_NUMBER, TRANS_DATE, TRF_GLOBAL_ID, SVC_PHASE)
      SELECT SERVICEPOINTID, UNIQUESPID, REVENUEACCOUNTCODE, STREETNUMBER, STREETNAME1,
             STREETNAME2, CITY, STATE, ZIP, CUSTOMERTYPE, RATESCHEDULE,
             GLOBALID, METERNUMBER, DATEMODIFIED, nvl(TRANSFORMERGUID,PRIMARYMETERGUID),
             METER_PHASE
        from edgis.zz_mv_servicepoint;


      sde.version_util.set_current_version('SDE.DEFAULT');
   ELSE
      DBMS_OUTPUT.PUT_LINE('1.2');

     Insert into PGEDATA.Temp_CD_transformer_b
             (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
              INSTALLATION_TYP, TRANS_DATE, OLD_GLOBAL_ID, LOWSIDE_VOLTAGE,
              OPERATING_VOLTAGE, CIRCUIT_ID, VAULT )
       SELECT CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
              INSTALLATION_TYP, TRANS_DATE, OLD_GLOBAL_ID, LOWSIDE_VOLTAGE,
              OPERATING_VOLTAGE, CIRCUIT_ID, VAULT
         from PGEDATA.Temp_CD_transformer_a;

      DBMS_OUTPUT.PUT_LINE('2.2');

      Insert into PGEDATA.Temp_CD_transformer_BANK_b
             (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_DATE,
              OLD_GLOBAL_ID, TRF_TYP )
       SELECT TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_DATE,
              OLD_GLOBAL_ID, TRF_TYP
         from PGEDATA.Temp_CD_transformer_BANK_a;

      DBMS_OUTPUT.PUT_LINE('3.2');

      Insert into PGEDATA.TEMP_CD_METER_B
            (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
             SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID,
             METER_NUMBER, TRANS_DATE, TRF_GLOBAL_ID, SVC_PHASE)
      SELECT SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
             SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID,
             METER_NUMBER, TRANS_DATE, TRF_GLOBAL_ID, SVC_PHASE
        from PGEDATA.TEMP_CD_METER_A;
   END IF;

   DBMS_OUTPUT.PUT_LINE('4');
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Generating After Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   -- clear them first; then populate
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_a';
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_BANK_a' ;
   execute immediate 'truncate table PGEDATA.Temp_CD_METER_a' ;

   Insert into PGEDATA.Temp_CD_transformer_a
          (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD, INSTALLATION_TYP,
           TRANS_DATE, OLD_GLOBAL_ID, LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT)
   (SELECT CGC12, GLOBALID, COASTALIDC, CLIMATEZONE, INSTALLATIONTYPE,
           DATEMODIFIED, REPLACEGUID, LOWSIDEVOLTAGE, OPERATINGVOLTAGE, CIRCUITID, VAULT
      from edgis.zz_mv_transformer
    UNION
    SELECT CGC12, GLOBALID, COASTALIDC, CLIMATEZONE, INSTALLATIONTYPE,
           DATEMODIFIED, REPLACEGUID, SECONDARYVOLTAGE, OPERATINGVOLTAGE, CIRCUITID, VAULT
      from edgis.zz_mv_primarymeter);

   DBMS_OUTPUT.PUT_LINE('5');
   Insert into PGEDATA.Temp_CD_transformer_BANK_a
          (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID,
           PHASE_CD, TRANS_DATE, OLD_GLOBAL_ID, TRF_TYP )
   (SELECT TRANSFORMERGUID, BANKCODE, RATEDKVA, GLOBALID,
           NUMBEROFPHASES, DATEMODIFIED, REPLACEGUID, TRANSFORMERTYPE
      from edgis.zz_mv_transformerunit
    UNION
    SELECT GLOBALID, NVL(BANKCODE, 1), NVL(RATEDKVA,9999), REPLACE(GLOBALID,'-','_'),
           NUMBEROFPHASES, DATEMODIFIED, REPLACE(REPLACEGUID,'-','_'), NVL(TRANSFORMERTYPE,'99')
      from edgis.zz_mv_primarymeter);
   DBMS_OUTPUT.PUT_LINE('6');

   -- Populate TRF_GLOBAL_ID with transformer guid when that's populated, otherwise use
   -- primary meter guid
   Insert into PGEDATA.TEMP_CD_METER_A
         (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
          SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, METER_NUMBER, TRANS_DATE, TRF_GLOBAL_ID, SVC_PHASE)
   SELECT SERVICEPOINTID, UNIQUESPID, REVENUEACCOUNTCODE, STREETNUMBER, STREETNAME1,
          STREETNAME2, CITY, STATE, ZIP, CUSTOMERTYPE, RATESCHEDULE,
          GLOBALID, METERNUMBER, DATEMODIFIED, nvl(TRANSFORMERGUID,PRIMARYMETERGUID),
          METER_PHASE
     from edgis.zz_mv_servicepoint;

    -- clear up the instances of trf_global_id's what are no longer valid features
   UPDATE PGEDATA.TEMP_CD_METER_A
      SET trf_global_id = NULL
    WHERE trf_global_id IN (SELECT DISTINCT transformerguid
                              FROM edgis.zz_mv_servicepoint s, edgis.zz_mv_transformer t
                             WHERE s.transformerguid = t.globalid (+)
                               AND t.globalid IS NULL
                               AND s.transformerguid IS NOT NULL
                             UNION
                             SELECT primarymeterguid
                               FROM edgis.zz_mv_servicepoint s, edgis.zz_mv_primarymeter t
                              WHERE s.primarymeterguid = t.globalid (+)
                                AND t.globalid IS NULL
                                AND s.primarymeterguid IS NOT NULL);


   dbms_stats.gather_table_stats('pgedata','Temp_cd_meter_a');
   dbms_stats.gather_table_stats('pgedata','Temp_cd_meter_b');
   dbms_stats.gather_table_stats('pgedata','Temp_cd_Transformer_a');
   dbms_stats.gather_table_stats('pgedata','Temp_cd_transformer_b');
   dbms_stats.gather_table_stats('pgedata','Temp_cd_Transformer_bank_a');
   dbms_stats.gather_table_stats('pgedata','Temp_cd_transformer_bank_b');
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Insert CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   -- to be an Insert the guid in A should have a trf_global_id and not exist in B
   -- OR the guid can exist in A and B but the trf_global_id is in A and not B
   insert into PGEDATA.pge_cd_meter
         (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2,
          SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID, SM_FLG,
          METER_NUMBER, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID, SVC_PHASE )
   select SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2,
          SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID, SM_FLG,
          METER_NUMBER, 'I', TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID, SVC_PHASE
     from PGEDATA.TEMP_CD_METER_A
    where global_id in ( SELECT a.global_id
                           FROM temp_cd_meter_a a, temp_cd_meter_b b
                          WHERE a.global_id = b.global_id (+)
                            AND b.global_id IS null
                            AND a.trf_global_id IS NOT NULL
                          UNION
                         SELECT a.global_id
                           FROM temp_cd_meter_a a, temp_cd_meter_b b
                          WHERE a.global_id = b.global_id
                            AND a.trf_global_id IS NOT NULL
                            AND b.trf_global_id IS null);
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Update CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
            (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
             SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
             GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE,
             OLD_GLOBAL_ID, TRF_GLOBAL_ID, SVC_PHASE )
      select a.SERVICE_POINT_ID, a.UNQSPID, a.REV_ACCT_CD, a.SVC_ST_NUM, a.SVC_ST_NAME,
             a.SVC_ST_NAME2, a.SVC_CITY, a.SVC_STATE, a.SVC_ZIP, a.CUST_TYP, a.RATE_SCHED,
             a.GLOBAL_ID, a.SM_FLG, a.METER_NUMBER, 'U', a.TRANS_DATE,
             a.OLD_GLOBAL_ID, a.TRF_GLOBAL_ID, a.SVC_PHASE
        from PGEDATA.TEMP_CD_METER_A a, PGEDATA.TEMP_CD_METER_B b
       where a.global_id = b.global_id
         AND a.trf_global_id IS NOT NULL -- don't update orphaned meters!!
         AND b.trf_global_id IS NOT NULL -- if before is Null this is an Insert or nothing
         and (nvl(a.SERVICE_POINT_ID,' ')  <> nvl(b.SERVICE_POINT_ID,' ')
          or nvl(a.UNQSPID, ' ')           <> nvl(b.UNQSPID, ' ')
          or nvl(a.REV_ACCT_CD,' ')        <> nvl(b.REV_ACCT_CD, ' ')
          or nvl(a.SVC_ST_NUM, ' ')        <> nvl(b.SVC_ST_NUM, ' ')
          or nvl(a.SVC_ST_NAME, ' ')       <> nvl(b.SVC_ST_NAME, ' ')
          or nvl(a.SVC_ST_NAME2, ' ')      <> nvl(b.SVC_ST_NAME2, ' ')
          or nvl(a.SVC_CITY,  ' ')         <> nvl(b.SVC_CITY,  ' ')
          or nvl(a.SVC_STATE, ' ')         <> nvl(b.SVC_STATE, ' ')
          or nvl(a.SVC_ZIP, ' ')           <> nvl(b.SVC_ZIP, ' ')
          or nvl(a.CUST_TYP, ' ')          <> nvl(b.CUST_TYP, ' ')
          or nvl(a.RATE_SCHED, ' ')        <> nvl(b.RATE_SCHED, ' ')
          or nvl(a.SM_FLG, ' ')            <> nvl(b.SM_FLG, ' ')
          or nvl(a.METER_NUMBER, ' ')      <> nvl(b.METER_NUMBER, ' ')
          or nvl(a.TRANS_DATE, '01-Jan-1900') <> nvl(b.TRANS_DATE,'01-Jan-1900')
          or nvl(a.OLD_GLOBAL_ID, ' ')     <> nvl(b.OLD_GLOBAL_ID, ' ')
          or nvl(a.TRF_GLOBAL_ID, ' ')     <> nvl(b.TRF_GLOBAL_ID, ' ') );
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   -- There is no such thing as a 'replaced' meter/service point code for R action removed

   V_Action_name := 'Delete CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
         (SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
          SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE,
          OLD_GLOBAL_ID, TRF_GLOBAL_ID, SVC_PHASE )
   select SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME,
          SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, SM_FLG, METER_NUMBER, 'D', TRANS_DATE,
          OLD_GLOBAL_ID, TRF_GLOBAL_ID, SVC_PHASE
     from PGEDATA.TEMP_CD_METER_B
    where GLOBAL_ID in ( SELECT b.global_id
                           FROM pgedata.temp_cd_meter_b b, pgedata.temp_cd_meter_a a
                          WHERE b.global_id = a.global_id (+)
                            AND a.global_id IS NULL);
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Generating CD_Transformer Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
/*
   a replace for transformer is:
   a transformer record that has not yet been sent to tlm (in A not B)
   where the old_global_id (replaceguid) is not null
   and the value in old_global_id has been sent to tlm (in B as global Id)
   and the value in replaceguid no longer exists in edgis (not in A as global id)
*/

   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.PGE_CD_TRANSFORMER
         (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
          INSTALLATION_TYP, REGION, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT )
   select CGC_ID,GLOBAL_ID,
          case to_char(coast_interior_flg)
               when 'Y' then 1
               when 'N' then 0
               else null end,
          CLIMATE_ZONE_CD, INSTALLATION_TYP, REGION, 'R' ,TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT
     from TEMP_CD_TRANSFORMER_A
    where GLOBAL_ID in ( SELECT a.global_id
                           FROM pgedata.temp_cd_transformer_b b,
                                pgedata.temp_cd_transformer_a a
                          WHERE a.global_id = b.global_id (+)
                            AND b.global_id IS NULL
                            AND a.old_global_id IS NOT NULL
                            AND EXISTS (SELECT 's'
                                          FROM pgedata.temp_cd_transformer_b t
                                         WHERE t.global_id = a.old_global_id)
                            AND NOT EXISTS (SELECT 'x'
                                              FROM pgedata.temp_cd_transformer_a ta
                                             WHERE ta.global_id = a.old_global_id));

/* an insert is simply a record we've not sent to TLM before (in A not B) that
   is NOT a replace
*/

   insert into PGEDATA.PGE_CD_TRANSFORMER
         (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
          INSTALLATION_TYP, REGION, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT )
   select CGC_ID,GLOBAL_ID,
          case to_char(coast_interior_flg)
               when 'Y' then 1
               when 'N' then 0
               else null end,
          CLIMATE_ZONE_CD, INSTALLATION_TYP,REGION, 'I', TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT
     from PGEDATA.TEMP_CD_TRANSFORMER_A
    where global_id in ( SELECT a.global_id
                           FROM pgedata.temp_cd_transformer_b b,
                                pgedata.temp_cd_transformer_a a
                          WHERE a.global_id = b.global_id (+)
                            AND b.global_id IS NULL
                          MINUS
                         SELECT global_id
                           FROM pgedata.pge_cd_transformer
                          WHERE trans_type = 'R');

   insert into PGEDATA.pge_cd_transformer
         (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
          INSTALLATION_TYP, REGION, TRANS_TYPE, TRANS_DATE,
          OLD_GLOBAL_ID, LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT)
   select a.CGC_ID,a.GLOBAL_ID,
          case to_char(a.coast_interior_flg)
               when 'Y' then 1
               when 'N' then 0
               else null end,
          a.CLIMATE_ZONE_CD, a.INSTALLATION_TYP, a.REGION, 'U', a.TRANS_DATE,
          a.OLD_GLOBAL_ID, a.LOWSIDE_VOLTAGE, a.OPERATING_VOLTAGE, a.CIRCUIT_ID, a.VAULT
     from PGEDATA.TEMP_CD_TRANSFORMER_A a, PGEDATA.TEMP_CD_TRANSFORMER_B b
    where a.global_id = b.global_id
      and (nvl(a.CGC_ID,0)                  <> nvl(b.CGC_ID,0)
       or nvl(a.COAST_INTERIOR_FLG, ' ')    <> nvl(b.COAST_INTERIOR_FLG, ' ')
       or nvl(a.CLIMATE_ZONE_CD,' ')        <> nvl(b.CLIMATE_ZONE_CD, ' ')
       or nvl(a.INSTALLATION_TYP, ' ')      <> nvl(b.INSTALLATION_TYP, ' ')
       or nvl(a.REGION, 0)                  <> nvl(b.REGION, 0)
       or nvl(a.TRANS_DATE, '01-Jan-1900')  <> nvl(b.TRANS_DATE,'01-Jan-1900')
       or nvl(a.OLD_GLOBAL_ID,  ' ')        <> nvl(b.OLD_GLOBAL_ID,  ' ')
       or nvl(a.LOWSIDE_VOLTAGE, 0)         <> nvl(b.LOWSIDE_VOLTAGE, 0)
       or nvl(a.OPERATING_VOLTAGE, 0)       <> nvl(b.OPERATING_VOLTAGE, 0)
       or nvl(a.CIRCUIT_ID, ' ')            <> nvl(b.CIRCUIT_ID, ' ')
       or nvl(a.VAULT, ' ')                 <> nvl(b.VAULT, ' ') );


   insert into PGEDATA.PGE_CD_TRANSFORMER
         (CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
          INSTALLATION_TYP, REGION, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT )
   select CGC_ID,GLOBAL_ID,
          case to_char(coast_interior_flg)
               when 'Y' then 1
               when 'N' then 0
               else null end,
          CLIMATE_ZONE_CD, INSTALLATION_TYP, REGION, 'D', TRANS_DATE, OLD_GLOBAL_ID,
          LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT
     from PGEDATA.TEMP_CD_TRANSFORMER_B
    where GLOBAL_ID in ( SELECT b.global_id
                           FROM pgedata.temp_cd_transformer_b b,
                                pgedata.temp_cd_transformer_a a
                          WHERE b.global_id = a.global_id (+)
                            AND a.global_id IS NULL
                          MINUS
                         SELECT old_global_id
                           FROM pgedata.pge_cd_transformer
                          WHERE trans_type = 'R');

   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Generating CD_Tran_Bank Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;

-- a transformer bank replace follows the same rules as a transformer

    insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
         (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE,
          TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, 'R',
          TRANS_DATE,OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A
    where GLOBAL_ID in ( SELECT a.global_id
                           FROM pgedata.temp_cd_transformer_bank_b b,
                                pgedata.temp_cd_transformer_bank_a a
                          WHERE a.global_id = b.global_id (+)
                            AND b.global_id IS NULL
                            AND a.old_global_id IS NOT NULL
                            AND EXISTS (SELECT 's'
                                          FROM pgedata.temp_cd_transformer_bank_b t
                                         WHERE t.global_id = a.old_global_id)
                            AND NOT EXISTS (SELECT 'x'
                                              FROM pgedata.temp_cd_transformer_bank_a ta
                                             WHERE ta.global_id = a.old_global_id));

   insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
         (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE,
          TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, 'I', TRANS_DATE,
          OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A
    where trf_global_id IS NOT NULL
      and global_id in ( SELECT a.global_id
                           FROM pgedata.temp_cd_transformer_bank_b b,
                                pgedata.temp_cd_transformer_bank_a a
                          WHERE a.global_id = b.global_id (+)
                            AND b.global_id IS NULL
                          MINUS
                         SELECT global_id
                           FROM pgedata.pge_cd_transformer_bank
                          WHERE trans_type = 'R');

   insert into PGEDATA.pge_cd_transformer_BANK
         (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE,
          TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select A.TRF_GLOBAL_ID, A.BANK_CD, A.NP_KVA, A.GLOBAL_ID, A.PHASE_CD, 'U',
          A.TRANS_DATE, A.OLD_GLOBAL_ID, A.TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A A, PGEDATA.TEMP_CD_TRANSFORMER_BANK_B B
    where a.global_id = b.global_id
      and a.trf_global_id IS NOT null
      and (nvl(a.TRF_GLOBAL_ID,' ')   <> nvl(b.TRF_GLOBAL_ID,' ')
       or  nvl(a.BANK_CD, ' ')        <> nvl(b.BANK_CD, ' ')
       or  nvl(a.NP_KVA,0)            <> nvl(b.NP_KVA, 0)
       or  nvl(a.PHASE_CD, 0)         <> nvl(b.PHASE_CD,0)
       or  nvl(a.TRF_TYP, ' ')        <> nvl(b.TRF_TYP, ' ')
       or  nvl(a.TRANS_DATE, '01-Jan-1900') <> nvl(b.TRANS_DATE,'01-Jan-1900')
       or  nvl(a.OLD_GLOBAL_ID,  ' ') <> nvl(b.OLD_GLOBAL_ID,  ' ') );


   insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
         (TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE,
          TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, to_number(BANK_CD), NP_KVA, GLOBAL_ID, PHASE_CD, 'D',
          TRANS_DATE,OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B
    where GLOBAL_ID in ( SELECT b.global_id
                           FROM pgedata.temp_cd_transformer_bank_b b,
                                pgedata.temp_cd_transformer_bank_a a
                          WHERE b.global_id = a.global_id (+)
                            AND a.global_id IS NULL
                          MINUS
                         SELECT old_global_id
                           FROM pgedata.pge_cd_transformer_bank
                          WHERE trans_type = 'R');
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   select state_id
     into V_Default_version
     from sde.versions
    where upper(name)='DEFAULT'
      and owner='SDE';

   select state_id
     into V_Chgdtl_version
     from sde.versions
    where upper(name)='CHANGE_DETECTION_SYNC_TLM'
      and owner='GIS_I';

  sde.version_util.change_version_state('GIS_I.Change_Detection_Sync_Tlm',V_Chgdtl_version,V_Default_version);

EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);
     V_Status := log_process_status('E',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);

End generate_cd_tables;
/



Prompt Grants on PROCEDURE GENERATE_CD_TABLES TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.GENERATE_CD_TABLES TO EDGIS
/

Prompt Grants on PROCEDURE GENERATE_CD_TABLES TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.GENERATE_CD_TABLES TO GIS_I
/

Prompt Grants on PROCEDURE GENERATE_CD_TABLES TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.GENERATE_CD_TABLES TO GIS_I_WRITE
/

-- 7/11/19 removed inappropriate grants on procedure to igp users

Prompt Grants on PROCEDURE GENERATE_CD_TABLES TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.GENERATE_CD_TABLES TO SDE
/
commit;