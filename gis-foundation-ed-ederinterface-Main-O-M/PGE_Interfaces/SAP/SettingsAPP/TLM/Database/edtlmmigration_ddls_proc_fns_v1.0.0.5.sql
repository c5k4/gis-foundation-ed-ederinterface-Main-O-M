-----------------v1.0.0.5------------------------------

spool  edtlmmigration_ddls_proc_fns_v1.0.0.5.log


--------------------------------------------------------
--  File created - Thursday-July-31-2014   
--------------------------------------------------------
DROP FUNCTION "EDTLMMIGRATION"."INSERT_MIGRATION_LOG";
DROP FUNCTION "EDTLMMIGRATION"."LOG_MIGRATION_ERROR";
DROP FUNCTION "EDTLMMIGRATION"."LOG_MIGRATION_SUCCESS";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_METER";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_SP_PEAK_HIST_LEGACY";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_SP_PEAK_HIST_SMART";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_TRANSFORMER";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_TRANSFORMER_BANK";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_TRF_PEAK_HIST_LEGACY";
DROP FUNCTION "EDTLMMIGRATION"."MIGRATE_TRF_PEAK_HIST_SMART";
DROP FUNCTION "EDTLMMIGRATION"."MIG_SP_PEAK_HST_INS_BATCH_LOGS";
DROP FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_METER_MAPPING";
DROP FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_TRF_BANK_MAPPING";
DROP FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_TRF_MAPPING";
DROP FUNCTION "EDTLMMIGRATION"."POPULATE_TRF_PEAK_BY_CUST_TYP";
DROP FUNCTION "EDTLMMIGRATION"."POPULATE_TRF_PEAK_TABLE";
DROP FUNCTION "EDTLMMIGRATION"."UPDATE_TRF_PEAK_HIST";
DROP FUNCTION "EDTLMMIGRATION"."UPDATE_TRF_PEAK_HIST_CAP";
DROP FUNCTION "EDTLMMIGRATION"."UPD_MIGRATION_LOG_STARTDATE";
DROP PROCEDURE "EDTLMMIGRATION"."CLEANUP_EDER_TRF_BANK_MAPPING";
DROP PROCEDURE "EDTLMMIGRATION"."MIGRATE_CEDSA";
--------------------------------------------------------
--  DDL for Function INSERT_MIGRATION_LOG
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."INSERT_MIGRATION_LOG" (wave VARCHAR2, tableName VARCHAR2) RETURN VARCHAR2 AS 
BEGIN
  -- DBMS_OUTPUT.PUT_LINE('DEBUG - before function INSERT_MIGRATION_LOG');

  Insert into EDTLMMIGRATION.MIGRATION_LOG (WAVE,TABLE_NAME) values (wave, tableName);
  -- DBMS_OUTPUT.PUT_LINE('DEBUG - after function INSERT_MIGRATION_LOG');
  commit;
  return 'SUCCESS';
END INSERT_MIGRATION_LOG;

/
--------------------------------------------------------
--  DDL for Function LOG_MIGRATION_ERROR
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."LOG_MIGRATION_ERROR" (wave_param VARCHAR2, table_name_param VARCHAR2, error_text_param VARCHAR2) RETURN VARCHAR2 AS 
BEGIN
  update EDTLMMIGRATION.MIGRATION_LOG set ERROR_TEXT = error_text_param 
  where
     WAVE = wave_param and
     TABLE_NAME = table_name_param;
  commit;
  return 'SUCCESS';
END LOG_MIGRATION_ERROR;

/
--------------------------------------------------------
--  DDL for Function LOG_MIGRATION_SUCCESS
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."LOG_MIGRATION_SUCCESS" (
    wave_param                 VARCHAR2,
    table_name_param           VARCHAR2,
    num_records_migrated_param NUMBER)
  RETURN VARCHAR2
AS
BEGIN
  DBMS_OUTPUT.PUT_LINE('DEBUG - Wave: '||wave_param||' Table Name: '||table_name_param||' Records updated: '||num_records_migrated_param);
  UPDATE EDTLMMIGRATION.MIGRATION_LOG
  SET NUM_RECORDS_MIGRATED = num_records_migrated_param,
    MIG_END_TS             = sysdate
  WHERE WAVE               = wave_param
  AND TABLE_NAME           = table_name_param;
  COMMIT;
  RETURN 'SUCCESS';
END LOG_MIGRATION_SUCCESS;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_METER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_METER" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
      p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'METER');

     v_stmt := '
          insert into '||targetSchema||'.METER (
               ID,
               SERVICE_POINT_ID,
               UNQSPID,
               TRF_ID,
               REV_ACCT_CD,
               SVC_ST_NUM,
               SVC_ST_NAME,
               SVC_ST_NAME2,
               SVC_CITY,
               SVC_STATE,
               SVC_ZIP,
               CUST_TYP,
               RATE_SCHED,
               GLOBAL_ID,
               SM_FLG,
               METER_NUMBER)
          select 
               f.METER_ID,
               f.SERVICE_POINT_ID,
               f.UNQSPID,
               f.TRF_ID,
               f.REV_ACCT_CD,
               f.SVC_ST_#,
               f.SVC_ST_NAME,
               f.SVC_ST_NAME2,
               f.SVC_CITY,
               f.SVC_STATE,
               f.SVC_ZIP,
               f.CUST_TYP,
               f.RATE_SCHED,
               e.GLOBAL_ID,
               decode(f.sm_sp_status, ''30'', ''S'', ''40'', ''S'',''L''),
               f.METER_NUMBER          
          FROM '
               ||sourceSchema||'.EDER_METER_MAPPING e, '
               ||sourceSchema||'.METER F, '
               ||targetSchema||'.TRANSFORMER d 
          WHERE 
               f.SERVICE_POINT_ID = e.SERVICE_POINT_ID AND
               d.ID               = f.TRF_ID AND
               d.REGION IN ('||p_region||')
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'METER',SQL%ROWCOUNT);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''METER'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;  
   
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'METER', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=METER');
        
END MIGRATE_METER;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_SP_PEAK_HIST_LEGACY
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_SP_PEAK_HIST_LEGACY" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     batchDate DATE,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_rowsProcessed NUMBER := 0;
     v_totalRowsMigrated NUMBER := 0;
     v_s_pf CONSTANT NUMBER := 0.85;
     v_w_pf CONSTANT NUMBER := 0.95;
     v_season VARCHAR2(1);
     v_seasonal_pf NUMBER;
     
BEGIN
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     v_batchMonth := SUBSTR((to_char(batchDate,'MONTH')),1,3);
     if (upper(v_batchMonth) in ('NOV','DEC','JAN','FEB','MAR')) then 
          v_season := 'W';
          v_seasonal_pf := v_w_pf;
     else
          v_season := 'S';
          v_seasonal_pf := v_s_pf;
     end if;

     -- update migration log start date
     v_log_status := UPD_MIGRATION_LOG_STARTDATE(wave, 'MIG_SP_PEAK_HST_LGCY_'||to_char(batchDate,'mmddyyyy'));
     
     ----------------------------------------------------------------------------------------
     -- Scnenario 1 - Legacy, domestic/non-domestic with KVAR (for legacy, kw and pf are > 0)
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select 
               a.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
               round(b.'||v_batchMonth||'_REV_KW/(b.'||v_batchMonth||'_PFACTOR/100),1),
               round(b.'||v_batchMonth||'_REV_KW/(b.'||v_batchMonth||'_PFACTOR/100),1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.METER_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
               a.trf_id = c.id and
               c.region in ('||p_region||') and
               b.'||v_batchMonth||'_PFACTOR > 0 and 
               b.'||v_batchMonth||'_REV_KW > 0 and
               (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
               a.meter_id = b.meter_id
               ';
                     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;     
 
     ----------------------------------------------------------------------------------------
     -- Scnenario 2 - Legacy, domestic withOUT KVAR (for legacy, pf = 0 or null)
     ----------------------------------------------------------------------------------------
     
     -- populate temporary tables used for calculation
     v_stmt := 'delete from '||sourceSchema||'.calc_trf_dom_count';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := 
     'insert into '||sourceSchema||'.calc_trf_dom_count
          select 
               a.id as device_id, 
               count(distinct b.ID) as dom_count 
          from '
               ||targetSchema||'.TRANSFORMER a,'
               ||targetSchema||'.METER b
          where 
               a.ID = b.TRF_ID and
               b.CUST_TYP = ''DOM'' 
          group by b.TRF_ID';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
      
     -- populate temporary tables used for calculation
     v_stmt := 'delete from '||sourceSchema||'.calc_trf_ab_value';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := 
     'insert into '||sourceSchema||'.calc_trf_ab_value
          select 
               T.ID as device_id,
               KCCS.A_value summer_a_value, 
               KCCS.B_value summer_b_value, 
               KCCW.A_value winter_a_value, 
               KCCW.B_value winter_b_value 
          from '
               ||targetSchema||'.TRANSFORMER t,'
               ||sourceSchema||'.KVA_conversion_coefficients KCCS,'
               ||sourceSchema||'.KVA_conversion_coefficients KCCW,'
               ||sourceSchema||'.CALC_TRF_DOM_COUNT TDC
          where 
               T.ID = TDC.DEVICE_ID AND 
               KCCS.Climate_zone = T.CLIMATE_ZONE_CD AND 
               KCCS.Season = ''S'' AND 
               KCCS.customer_count_gt = 
                  DECODE(TDC.DOM_COUNT, 1, 0, 2, 1, 3, 2, 4, 2, 5, 2,
                     DECODE(TRUNC(TDC.DOM_COUNT/13), 0, 5,
                        DECODE(TRUNC(TDC.DOM_COUNT/21), 0, 12,
                                             20))) AND 
               KCCW.Climate_zone = T.CLIMATE_ZONE_CD AND 
               KCCW.Season = ''W'' AND 
               KCCW.customer_count_gt = 
                  DECODE(TDC.DOM_COUNT, 1, 0, 2, 1, 3, 2, 4, 2, 5, 2,
                     DECODE(TRUNC(TDC.DOM_COUNT/13), 0, 5,
                        DECODE(TRUNC(TDC.DOM_COUNT/21), 0, 12, 20)))';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     if (v_season = 'S') THEN     
         v_stmt := '
              insert into '||targetSchema||'.SP_PEAK_HIST (
                   SERVICE_POINT_ID,
                   TRF_PEAK_HIST_ID,
                   SP_PEAK_TIME,
                   sp_peak_kva,
                   sp_kva_trf_peak,
                   cust_typ,
                   sm_flg,
                   meter_id)
              select 
                   a.service_point_id, 
                   (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
                   to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
                   round((((b.'||v_batchMonth||'_REV_KWHR/10) * d.summer_a_value) + d.summer_b_value)/'||v_s_pf||',1),
                   round((((b.'||v_batchMonth||'_REV_KWHR/10) * d.summer_a_value) + d.summer_b_value)/'||v_s_pf||',1),
                   a.CUST_TYP,
                   ''L'' as sm_flg,
                   NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
              FROM '
                   ||sourceSchema||'.METER a, '
                   ||sourceSchema||'.METER_LOAD b, '
                   ||sourceSchema||'.calc_trf_ab_value d, '
                   ||targetSchema||'.TRANSFORMER c
              WHERE 
                   a.trf_id = c.id and
                   a.cust_typ = ''DOM'' and
                   a.trf_id = d.device_id and
                   c.region in ('||p_region||') and not
                   (b.'||v_batchMonth||'_PFACTOR > 0 and 
                   b.'||v_batchMonth||'_REV_KW > 0) and
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
     ELSE
         v_stmt := '
              insert into '||targetSchema||'.SP_PEAK_HIST (
                   SERVICE_POINT_ID,
                   TRF_PEAK_HIST_ID,
                   SP_PEAK_TIME,
                   sp_peak_kva,
                   sp_kva_trf_peak,
                   cust_typ,
                   sm_flg,
                   meter_id)
              select 
                   a.service_point_id, 
                   (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
                   to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
                   round((((b.'||v_batchMonth||'_REV_KWHR/10) * d.winter_a_value) + d.winter_b_value)/'||v_w_pf||',1),
                   round((((b.'||v_batchMonth||'_REV_KWHR/10) * d.winter_a_value) + d.winter_b_value)/'||v_w_pf||',1),
                   a.CUST_TYP,
                   ''L'' as sm_flg,
                   NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
              FROM '
                   ||sourceSchema||'.METER a, '
                   ||sourceSchema||'.METER_LOAD b, '
                   ||sourceSchema||'.calc_trf_ab_value d, '
                   ||targetSchema||'.TRANSFORMER c
              WHERE 
                   a.trf_id = c.id and
                   a.cust_typ = ''DOM'' and
                   a.trf_id = d.device_id and
                   c.region in ('||p_region||') and 
                   (b.'||v_batchMonth||'_REV_KWHR) > 0 and 
                   not (b.'||v_batchMonth||'_PFACTOR > 0 and b.'||v_batchMonth||'_REV_KW > 0) and
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
     END IF;
     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;     

     ----------------------------------------------------------------------------------------
     -- Scnenario 3 - If the SP has a non-zero kW (demand) value and customer type is Commercial
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select 
               a.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
               round(b.'||v_batchMonth||'_REV_KW/'||v_seasonal_pf||',1),
               round(b.'||v_batchMonth||'_REV_KW/'||v_seasonal_pf||',1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.METER_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
                   a.trf_id = c.id and
                   a.cust_typ = ''COM'' and
                   c.region in ('||p_region||') and 
                   b.'||v_batchMonth||'_REV_KW > 0 and
                   (b.'||v_batchMonth||'_PFACTOR is null or b.'||v_batchMonth||'_PFACTOR = 0) and 
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
                     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;  

     ----------------------------------------------------------------------------------------
     -- Scnenario 4 - If the SP has a non-zero kW (demand) value and customer type is Agriculture, Industrial or Other
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select 
               a.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
               round(b.'||v_batchMonth||'_REV_KW/.85,1),
               round(b.'||v_batchMonth||'_REV_KW/.85,1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.METER_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
                   a.trf_id = c.id and
                   a.cust_typ in (''AGR'',''IND'',''OTH'') and
                   c.region in ('||p_region||') and 
                   b.'||v_batchMonth||'_REV_KW > 0 and
                   (b.'||v_batchMonth||'_PFACTOR is null or b.'||v_batchMonth||'_PFACTOR = 0) and 
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
                     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;  
 
     ----------------------------------------------------------------------------------------
     -- Scnenario 5 - If the SP has a zero or null kW (demand) value and customer type is Commercial
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select 
               a.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
               round((b.'||v_batchMonth||'_REV_KWHR/10)*(1.0 /(24.0 * '||v_seasonal_pf||' * .48)),1),
               round((b.'||v_batchMonth||'_REV_KWHR/10)*(1.0 /(24.0 * '||v_seasonal_pf||' * .48)),1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.METER_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
                   a.trf_id = c.id and
                   a.cust_typ in (''COM'') and
                   c.region in ('||p_region||') and 
                   (b.'||v_batchMonth||'_REV_KW is null or b.'||v_batchMonth||'_REV_KW = 0) and
                   (b.'||v_batchMonth||'_REV_KWHR) > 0 and 
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
                     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;  
     
     ----------------------------------------------------------------------------------------
     -- Scnenario 6 - If the SP has a zero or null kW (demand) value and customer type is Agricultural, Industrial or Other
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select 
               a.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
               round((b.'||v_batchMonth||'_REV_KWHR/10)*(1.0 /(24.0 * .85 * .48)),1),
               round((b.'||v_batchMonth||'_REV_KWHR/10)*(1.0 /(24.0 * .85 * .48)),1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.METER_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
                   a.trf_id = c.id and
                   a.cust_typ in (''AGR'',''IND'',''OTH'') and
                   c.region in ('||p_region||') and 
                   (b.'||v_batchMonth||'_REV_KW is null or b.'||v_batchMonth||'_REV_KW = 0) and
                   (b.'||v_batchMonth||'_REV_KWHR) > 0 and 
                   (a.sm_sp_status is null or a.sm_sp_status in (''10'',''20'')) and
                   a.meter_id = b.meter_id
                   ';
                     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;  

     DBMS_OUTPUT.PUT_LINE('Total rows migrated - '||v_totalRowsMigrated);

     -- clean up.  
     -- convert negative calculated SP_PEAK_KVA to .1
     v_stmt := '
          update '||targetSchema||'.SP_PEAK_HIST set SP_PEAK_KVA = .1
          where
               SP_PEAK_KVA < 0 and 
               TRF_PEAK_HIST_ID in
               (select ID from '||targetSchema||'.trf_peak_hist where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- convert negative calculated SP_KVA_TRF_PEAK to .1
     v_stmt := '
          update '||targetSchema||'.SP_PEAK_HIST set SP_KVA_TRF_PEAK = .1
          where
               SP_KVA_TRF_PEAK < 0 and 
               TRF_PEAK_HIST_ID in
               (select ID from '||targetSchema||'.trf_peak_hist where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'MIG_SP_PEAK_HST_LGCY_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''SP_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;  
     
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'MIG_SP_PEAK_HST_LGCY_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=MIG_SP_PEAK_HST_LGCY_'||to_char(batchDate,'mmddyyyy'));
        
END MIGRATE_SP_PEAK_HIST_LEGACY;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_SP_PEAK_HIST_SMART
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_SP_PEAK_HIST_SMART" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     batchDate DATE,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_seasonal_pf NUMBER;
BEGIN
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     if (to_char(batchDate, 'mm') in ('11','12','01','02','03')) then 
          v_seasonal_pf := .95;
     else
          v_seasonal_pf := .85;
     end if;

     -- update migration log start date
     v_log_status := UPD_MIGRATION_LOG_STARTDATE(wave, 'MIG_SP_PEAK_HST_SMART_'||to_char(batchDate,'mmddyyyy'));

     ----------------------------------------------------------------------------------------------
     -- Scnenario 1 - Smart, domestic/non-domestic, with KVAR
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id)
          select 
               b.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=b.device_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME, 
               round(SQRT(POWER(B.SP_PEAK_KW,2) + POWER(B.SP_PEAK_KVAR,2)),1),
               round(SQRT(POWER(B.SP_KW_TRF_PEAK,2) + POWER(B.TRF_PEAK_KVAR,2)),1), 
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG, 
               B.INT_LEN, 
               B.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.SM_SP_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
               b.device_id = c.id and
               c.region in ('||p_region||') and
               sm_sp_status in (''30'',''40'') and
               (B.SP_PEAK_KVAR is not null) and
               a.service_point_id = b.service_point_id and
               b.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'')           
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;
     
     
     ----------------------------------------------------------------------------------------------
     -- Scnenario 2 - Smart, domestic, no KVAR
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id)
          select 
               b.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=b.device_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME, 
               round(B.SP_PEAK_KW/'||v_seasonal_pf||',1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/'||v_seasonal_pf||',1) as sp_kva_trf_peak, 
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG, 
               B.INT_LEN, 
               B.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.SM_SP_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
               b.device_id = c.id and
               c.region in ('||p_region||') and
               sm_sp_status in (''30'',''40'') and
               a.cust_typ = ''DOM'' and
               (B.SP_PEAK_KVAR is null) and
               a.service_point_id = b.service_point_id and
               b.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'')           
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     ----------------------------------------------------------------------------------------------
     -- Scnenario 3 Smart, non-domestic, no KVAR, cust type COM
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id)
          select 
               b.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=b.device_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME, 
               round(B.SP_PEAK_KW/'||v_seasonal_pf||',1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/'||v_seasonal_pf||',1) as sp_kva_trf_peak, 
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG, 
               B.INT_LEN, 
               B.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.SM_SP_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
               b.device_id = c.id and
               c.region in ('||p_region||') and
               sm_sp_status in (''30'',''40'') and
               a.cust_typ = ''COM'' and
               (B.SP_PEAK_KVAR is null) and
               a.service_point_id = b.service_point_id and
               b.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'')           
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;


     ----------------------------------------------------------------------------------------------
     -- Scnenario 4 - Smart, non-domestic, no KVAR, cust type ARG, IND, OTH
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||targetSchema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id)
          select 
               b.service_point_id, 
               (select id from '||targetSchema||'.trf_peak_hist x where x.trf_id=b.device_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME, 
               round(B.SP_PEAK_KW/.85,1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/.85,1) as sp_kva_trf_peak, 
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG, 
               B.INT_LEN, 
               B.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               NULL --- a.METER_ID TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
          FROM '
               ||sourceSchema||'.METER a, '
               ||sourceSchema||'.SM_SP_LOAD b, '
               ||targetSchema||'.TRANSFORMER c
          WHERE 
               b.device_id = c.id and
               c.region in ('||p_region||') and
               sm_sp_status in (''30'',''40'') and
               a.cust_typ in (''AGR'',''IND'',''OTH'') and
               (B.SP_PEAK_KVAR is null) and
               a.service_point_id = b.service_point_id and
               b.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'')           
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;
     
     -- clean up.  
     -- convert negative calculated SP_PEAK_KVA to .1
     v_stmt := '
          update '||targetSchema||'.SP_PEAK_HIST set SP_PEAK_KVA = .1
          where
               SP_PEAK_KVA < 0 and 
               TRF_PEAK_HIST_ID in
               (select ID from '||targetSchema||'.TRF_PEAK_HIST where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- convert negative calculated SP_KVA_TRF_PEAK to .1
     v_stmt := '
          update '||targetSchema||'.SP_PEAK_HIST set SP_KVA_TRF_PEAK = .1
          where
               SP_KVA_TRF_PEAK < 0 and 
               TRF_PEAK_HIST_ID in
               (select ID from '||targetSchema||'.TRF_PEAK_HIST where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'MIG_SP_PEAK_HST_SMART_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''SP_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;  
     
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'MIG_SP_PEAK_HST_SMART_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=MIG_SP_PEAK_HST_SMART_'||to_char(batchDate,'mmddyyyy'));
        
END MIGRATE_SP_PEAK_HIST_SMART;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_TRANSFORMER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_TRANSFORMER" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'TRANSFORMER');

     v_stmt := '
          insert into '||targetSchema||'.TRANSFORMER (ID,CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD, PHASE_CD, INSTALLATION_TYP, REGION)
          select 
               d.device_id,
               d.cgc#12,
               e.global_id, 
               d.coast_interior, 
               d.climate_zone,
               e.phase_cd,
               a.install_cd,
               c.region_id
          FROM '
               ||sourceSchema||'.DEVICE a, '
               ||sourceSchema||'.LOCAL_OFFICE b, '
               ||sourceSchema||'.DIVISION_REGION_MAPPING c, '
               ||sourceSchema||'.EDER_TRANSFORMER_MAPPING e, '
               ||sourceSchema||'.TRANSFORMER d 
          WHERE 
               a.DEVICE_ID    = d.DEVICE_ID  AND 
               a.LOCAL_OFFICE = b.LOCAL_OFFICE  AND 
               b.DIVISION     = c.DIVISION_ID  AND 
               d.CGC#12       = e.CGC_NUM AND
               c.REGION_ID   IN ('||p_region||')
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'TRANSFORMER',SQL%ROWCOUNT);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRANSFORMER'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt; 
     
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'TRANSFORMER', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=TRANSFORMER');
        
END MIGRATE_TRANSFORMER;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_TRANSFORMER_BANK
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_TRANSFORMER_BANK" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'TRANSFORMER_BANK');

     v_stmt := '
          insert into '||targetSchema||'.TRANSFORMER_BANK (ID,TRF_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD)
          select 
               d.trf_id,
               d.device_id,
               e.bank_cd,
               NVL(e.ratedkva,0),
               e.global_id, 
               f.phase
          FROM '
               ||targetSchema||'.TRANSFORMER a, '
               ||sourceSchema||'.EDER_TRANSFORMER_BANK_MAPPING e, '
               ||sourceSchema||'.TRF_TYPE f, '
               ||sourceSchema||'.TRANSFORMER_BANK d 
          WHERE 
               a.ID           = d.DEVICE_ID  AND 
               d.DEVICE_ID    = e.DEVICE_ID AND
               d.BANK_CD      = e.BANK_CD AND
               d.trf_typ      = f.type_trf_cd AND
               a.REGION   IN ('||p_region||')
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'TRANSFORMER_BANK',SQL%ROWCOUNT);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRANSFORMER_BANK'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt; 
     
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'TRANSFORMER_BANK', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=TRANSFORMER_BANK');
        
END MIGRATE_TRANSFORMER_BANK;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_TRF_PEAK_HIST_LEGACY
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_TRF_PEAK_HIST_LEGACY" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     batchEndDate DATE,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_cnt NUMBER := 12; -- CEDSA only stores 12 months of legacy meter data
     v_date DATE := batchEndDate;
     v_batchDateStr VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'MIGRATE_TRF_PEAK_HIST_LEGACY');


      while v_cnt > 0 loop
          v_batchDateStr := to_char(v_date, 'dd-mon-yy');
          v_cnt := v_cnt - 1;

           v_stmt := '
                insert into '||targetSchema||'.TRF_PEAK_HIST (
                     trf_id, 
                     batch_date,
                     trf_peak_time,
                     trf_peak_kva,
                     sm_cust_total, 
                     ccb_cust_total, 
                     trf_cap)
                select distinct
                     a.TRF_ID,
                     to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
                     to_date('''||v_batchDateStr||''',''dd-mon-yy''), 
                     0,
                     NULL,
                     NULL,
                     0          
                FROM '
                     ||sourceSchema||'.METER a, '
                     ||targetSchema||'.TRANSFORMER b, '
                     ||sourceSchema||'.METER_LOAD c
                WHERE 
                     a.METER_ID    = c.METER_ID  AND 
                     a.TRF_ID      = b.ID        AND
                     b.REGION  IN ('||p_region||')          AND
                     not exists (select 1 from '||targetSchema||'.trf_peak_hist x where a.trf_id = x.trf_id and x.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy''))
                     ';

           DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
           execute immediate v_stmt;
           v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

          v_date := ADD_MONTHS(v_date, -1);
      end loop;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'MIGRATE_TRF_PEAK_HIST_LEGACY',v_totalRowsMigrated);
     
     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRF_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;       
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'MIGRATE_TRF_PEAK_HIST_LEGACY', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=MIGRATE_TRF_PEAK_HIST_LEGACY');
        
END MIGRATE_TRF_PEAK_HIST_LEGACY;

/
--------------------------------------------------------
--  DDL for Function MIGRATE_TRF_PEAK_HIST_SMART
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIGRATE_TRF_PEAK_HIST_SMART" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'TRF_PEAK_HIST_SMART');

     v_stmt := '
          insert into '||targetSchema||'.TRF_PEAK_HIST (
               trf_id, 
               batch_date,
               trf_peak_time,
               trf_peak_kva,
               sm_cust_total, 
               ccb_cust_total, 
               trf_cap)
          select 
               a.DEVICE_ID,
               a.BATCH_DATE, 
               a.TRF_PEAK_TIME, 
               0,
               NULL,
               NULL,
               a.TRF_CAP          
          FROM '
               ||sourceSchema||'.SM_TRF_LOAD a, '
               ||targetSchema||'.TRANSFORMER b
          WHERE 
               a.DEVICE_ID    = b.ID  AND 
               b.REGION  IN ('||p_region||')             
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'TRF_PEAK_HIST_SMART',SQL%ROWCOUNT);
     
     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRF_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;  
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'TRF_PEAK_HIST_SMART', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=TRF_PEAK_HIST_SMART');
        
END MIGRATE_TRF_PEAK_HIST_SMART;

/
--------------------------------------------------------
--  DDL for Function MIG_SP_PEAK_HST_INS_BATCH_LOGS
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."MIG_SP_PEAK_HST_INS_BATCH_LOGS" (
     wave         VARCHAR2,
     batchEndDate DATE)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_cnt INTEGER := 36;
     v_date DATE := batchEndDate;
     v_tableName VARCHAR2(30);
     v_cnt_lgcy INTEGER := 12;

BEGIN    
     while v_cnt > 0 loop
          v_cnt := v_cnt - 1;
          v_tableName := 'MIG_SP_PEAK_HST_SMART_'||TO_CHAR(v_date,'MMDDYYYY');
         
           v_stmt := '
                insert into EDTLMMIGRATION.MIGRATION_LOG (
                     WAVE, 
                     TABLE_NAME,
                     BATCH_PROCESSED)
                values ( 
                    '''||wave||''',
                    '''||v_tableName||''', 
                    '''||TO_CHAR(v_date)||''')
                     ';
           DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
           execute immediate v_stmt;

          -- only create 12 records for legacy since CEDSA only maintains 12 month history
          if v_cnt_lgcy > 0 then
              v_tableName := 'MIG_SP_PEAK_HST_LGCY_'||TO_CHAR(v_date,'MMDDYYYY');
             
               v_stmt := '
                    insert into EDTLMMIGRATION.MIGRATION_LOG (
                         WAVE, 
                         TABLE_NAME,
                         BATCH_PROCESSED)
                    values ( 
                        '''||wave||''',
                        '''||v_tableName||''', 
                        '''||TO_CHAR(v_date)||''')
                         ';
               DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
               execute immediate v_stmt;

               v_cnt_lgcy := v_cnt_lgcy - 1;
               
           end if;
           
          v_date := ADD_MONTHS(v_date, -1);

     end loop;     
 
     RETURN ('TRUE');
        
END MIG_SP_PEAK_HST_INS_BATCH_LOGS;

/
--------------------------------------------------------
--  DDL for Function POPULATE_EDER_METER_MAPPING
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_METER_MAPPING" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'POPULATE_EDER_METER_MAPPING');

     v_stmt := '
          truncate table '||sourceSchema||'.EDER_METER_MAPPING';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into '||sourceSchema||'.EDER_METER_MAPPING 
          select 
               b.SERVICEPOINTID,
               b.GLOBALID
          FROM '
               ||sourceSchema||'.EDGIS_SERVICEPOINT b
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'POPULATE_EDER_METER_MAPPING',SQL%ROWCOUNT);
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'POPULATE_EDER_METER_MAPPING', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=POPULATE_EDER_METER_MAPPING');
        
END POPULATE_EDER_METER_MAPPING;

/
--------------------------------------------------------
--  DDL for Function POPULATE_EDER_TRF_BANK_MAPPING
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_TRF_BANK_MAPPING" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'POPULATE_EDER_TRF_BANK_MAPPING');

     v_stmt := '
          truncate table '||sourceSchema||'.EDER_TRANSFORMER_BANK_MAPPING'; 
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||sourceSchema||'.EDER_TRANSFORMER_BANK_MAPPING (DEVICE_ID, GLOBAL_ID, BANK_CD, RATEDKVA)
          select 
               c.DEVICE_ID, 
               a.GLOBALID, 
               a.BANKCODE,
               a.RATEDKVA
          FROM '
               ||sourceSchema||'.EDGIS_TRANSFORMERUNIT a, '||sourceSchema||'.EDGIS_TRANSFORMER b, '||sourceSchema||'.TRANSFORMER c
          WHERE
               a.TRANSFORMERGUID = b.GLOBALID and
               b.CGC12 = c.CGC#12               
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'POPULATE_EDER_TRF_BANK_MAPPING',SQL%ROWCOUNT);
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'POPULATE_EDER_TRF_BANK_MAPPING', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=POPULATE_EDER_TRF_BANK_MAPPING');
        
END POPULATE_EDER_TRF_BANK_MAPPING;

/
--------------------------------------------------------
--  DDL for Function POPULATE_EDER_TRF_MAPPING
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."POPULATE_EDER_TRF_MAPPING" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'POPULATE_EDER_TRF_MAPPING');

     v_stmt := '
          truncate table '||sourceSchema||'.EDER_TRANSFORMER_MAPPING'; 
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into '||sourceSchema||'.EDER_TRANSFORMER_MAPPING 
          select 
               b.CGC12,
               b.GLOBALID,
               b.NUMBEROFPHASES
          FROM '
               ||sourceSchema||'.EDGIS_TRANSFORMER b
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'POPULATE_EDER_TRF_MAPPING',SQL%ROWCOUNT);
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'POPULATE_EDER_TRF_MAPPING', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=POPULATE_EDER_TRF_MAPPING');
        
END POPULATE_EDER_TRF_MAPPING;

/
--------------------------------------------------------
--  DDL for Function POPULATE_TRF_PEAK_BY_CUST_TYP
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."POPULATE_TRF_PEAK_BY_CUST_TYP" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'POPULATE_TRF_PEAK_BY_CUST_TYP');

     -- delete old records
     v_stmt := '
          delete from '||targetSchema||'.TRF_PEAK_BY_CUST_TYP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- insert summer peak records
     v_stmt := '
          insert into '||targetSchema||'.TRF_PEAK_BY_CUST_TYP 
               (TRF_PEAK_ID,SEASON,CUST_TYP, season_cust_cnt, season_total_kva)
          select c.ID, ''S'', b.CUST_TYP, count(*), NVL(sum(b.SP_KVA_TRF_PEAK),0) 
          from 
               '||targetSchema||'.TRF_PEAK_HIST a, 
               '||targetSchema||'.SP_PEAK_HIST b,
               '||targetSchema||'.TRF_PEAK c
          where 
               a.ID = b.TRF_PEAK_HIST_ID and
               c.TRF_ID = a.TRF_ID and
               a.BATCH_DATE = c.SMR_PEAK_DATE 
          group by c.ID, ''S'', b.CUST_TYP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- insert winter peak records
     v_stmt := '
          insert into '||targetSchema||'.TRF_PEAK_BY_CUST_TYP 
               (TRF_PEAK_ID,SEASON,CUST_TYP, season_cust_cnt, season_total_kva)
          select c.ID, ''W'', b.CUST_TYP, count(*), NVL(sum(b.SP_KVA_TRF_PEAK),0) 
          from 
               '||targetSchema||'.TRF_PEAK_HIST a, 
               '||targetSchema||'.SP_PEAK_HIST b,
               '||targetSchema||'.TRF_PEAK c
          where 
               a.ID = b.TRF_PEAK_HIST_ID and
               c.TRF_ID = a.TRF_ID and
               a.BATCH_DATE = c.WNTR_PEAK_DATE
          group by c.ID, ''W'', b.CUST_TYP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'POPULATE_TRF_PEAK_BY_CUST_TYP',v_totalRowsMigrated);
  
     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'POPULATE_TRF_PEAK_BY_CUST_TYP', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=POPULATE_TRF_PEAK_BY_CUST_TYP');
        
END POPULATE_TRF_PEAK_BY_CUST_TYP;

/
--------------------------------------------------------
--  DDL for Function POPULATE_TRF_PEAK_TABLE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."POPULATE_TRF_PEAK_TABLE" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     endBatchDate DATE,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_dateStr VARCHAR(10) := to_char(endBatchDate, 'dd-mon-yy');
     v_totalRowsMigrated NUMBER := 0;
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'POPULATE_TRF_PEAK_TABLE');

     -- create shell TRF_PEAK records
     v_stmt := '
          delete from '||targetSchema||'.TRF_PEAK';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into 
               '||targetSchema||'.TRF_PEAK (TRF_ID) 
          select ID 
          from '||targetSchema||'.TRANSFORMER a 
          where 
               a.REGION in ('||p_region||') ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- Determine the summer peaks.  Note that this could produce duplicate records
     -- if the peak loads are the same for two or more months
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_PEAK_SMR';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_PEAK_SMR
          select 
               a.TRF_ID, a.BATCH_DATE smr_peak_date, a.TRF_PEAK_KVA smr_kva, a.TRF_CAP smr_cap, 
               round((a.TRF_PEAK_KVA/a.TRF_CAP)*100,1) smr_pct,
               a.SM_CUST_TOTAL smr_peak_sm_cust_cnt, a.CCB_CUST_TOTAL smr_peak_total_cust_cnt
          from 
               '||targetSchema||'.TRF_PEAK_HIST a, 
               (
               select 
                    b.trf_id, max(b.TRF_PEAK_KVA) smr_kva from '||targetSchema||'.TRF_PEAK_HIST b 
               where 
                    b.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
                    SUBSTR((to_char(b.BATCH_DATE,''MONTH'')),1,3) in (''APR'',''MAY'',''JUN'',''JUL'',''AUG'',''SEP'',''OCT'')
               group by b.TRF_ID
               ) Y
          where 
               a.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
               a.TRF_ID = y.trf_id and 
               a.TRF_PEAK_KVA=y.smr_kva and
               SUBSTR((to_char(a.BATCH_DATE,''MONTH'')),1,3) in (''APR'',''MAY'',''JUN'',''JUL'',''AUG'',''SEP'',''OCT'') and
               a.trf_cap > 0 --- will this ever happen?
     '; 
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- If there are duplicate records, find the record with latest batch date
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_PEAK_SMR_2';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_PEAK_SMR_2
               select
                    a.TRF_ID, a.SMR_CAP,a.SMR_KVA,a.SMR_PCT,a.SMR_PEAK_DATE,a.SMR_PEAK_SM_CUST_CNT,a.SMR_PEAK_TOTAL_CUST_CNT 
               from 
                    CALC_TRF_PEAK_SMR a
               join
                    (
                    select 
                         c.TRF_ID, 
                         max(c.smr_peak_date) smr_peak_date 
                    from '||sourceSchema||'.CALC_TRF_PEAK_SMR c 
                    group by c.trf_id
                    ) b
                on 
                    a.TRF_ID=b.TRF_ID and 
                    a.SMR_PEAK_DATE=b.smr_peak_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
  
     -- update TRF_PEAK summer peak values
     v_stmt := '
          update 
          (
               select 
                    c.smr_peak_date old_smr_peak_date, c.smr_kva old_smr_kva, c.smr_cap old_smr_cap, c.smr_pct old_smr_pct, c.smr_peak_sm_cust_cnt old_smr_peak_sm_cust_cnt, c.smr_peak_total_cust_cnt old_smr_peak_total_cust_cnt,
                    b.smr_peak_date new_smr_peak_date, b.smr_kva new_smr_kva, b.smr_cap new_smr_cap, b.smr_pct new_smr_pct, b.smr_peak_sm_cust_cnt new_smr_peak_sm_cust_cnt, b.smr_peak_total_cust_cnt new_smr_peak_total_cust_cnt 
               from
                    '||targetSchema||'.trf_peak C,
                    '||sourceSchema||'.CALC_TRF_PEAK_SMR_2 B
               where 
                    C.TRF_ID = B.TRF_ID
          )
          set 
               old_smr_peak_date=new_smr_peak_date, 
               old_smr_kva=new_smr_kva, 
               old_smr_cap=new_smr_cap, 
               old_smr_pct=new_smr_pct, 
               old_smr_peak_sm_cust_cnt=new_smr_peak_sm_cust_cnt, 
               old_smr_peak_total_cust_cnt=new_smr_peak_total_cust_cnt';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     -- Determine the winter peaks.  Note that this could produce duplicate records
     -- if the peak loads are the same for two or more months
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_PEAK_WNTR';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_PEAK_WNTR
          select 
               a.TRF_ID, a.BATCH_DATE wntr_peak_date, a.TRF_PEAK_KVA wntr_kva, a.TRF_CAP wntr_cap, 
               round((a.TRF_PEAK_KVA/a.TRF_CAP)*100,1) wntr_pct,
               a.SM_CUST_TOTAL wntr_peak_sm_cust_cnt, a.CCB_CUST_TOTAL wntr_peak_total_cust_cnt
          from 
               '||targetSchema||'.TRF_PEAK_HIST a, 
               (
               select 
                    b.trf_id, max(b.TRF_PEAK_KVA) wntr_kva from '||targetSchema||'.TRF_PEAK_HIST b 
               where 
                    b.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
                    SUBSTR((to_char(b.BATCH_DATE,''MONTH'')),1,3) in (''NOV'',''DEC'',''JAN'',''FEB'',''MAR'')
               group by b.TRF_ID
               ) Y
          where 
               a.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
               a.TRF_ID = y.trf_id and 
               a.TRF_PEAK_KVA=y.wntr_kva and
               SUBSTR((to_char(a.BATCH_DATE,''MONTH'')),1,3) in (''NOV'',''DEC'',''JAN'',''FEB'',''MAR'') and
               a.trf_cap > 0 --- will this ever happen?
     '; 
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- If there are duplicate records, find the record with latest batch date
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_PEAK_WNTR_2';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_PEAK_WNTR_2
               select
                    a.TRF_ID, a.WNTR_CAP,a.WNTR_KVA,a.WNTR_PCT,a.WNTR_PEAK_DATE,a.WNTR_PEAK_SM_CUST_CNT,a.WNTR_PEAK_TOTAL_CUST_CNT 
               from 
                    CALC_TRF_PEAK_WNTR a
               join
                    (
                    select 
                         c.TRF_ID, 
                         max(c.wntr_peak_date) wntr_peak_date 
                    from '||sourceSchema||'.CALC_TRF_PEAK_WNTR c 
                    group by c.trf_id
                    ) b
                on 
                    a.TRF_ID=b.TRF_ID and 
                    a.WNTR_PEAK_DATE=b.wntr_peak_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
  
     -- update TRF_PEAK winter peak values
     v_stmt := '
          update 
          (
               select 
                    c.wntr_peak_date old_wntr_peak_date, c.wntr_kva old_wntr_kva, c.wntr_cap old_wntr_cap, c.wntr_pct old_wntr_pct, c.wntr_peak_sm_cust_cnt old_wntr_peak_sm_cust_cnt, c.wntr_peak_total_cust_cnt old_wntr_peak_total_cust_cnt,
                    b.wntr_peak_date new_wntr_peak_date, b.wntr_kva new_wntr_kva, b.wntr_cap new_wntr_cap, b.wntr_pct new_wntr_pct, b.wntr_peak_sm_cust_cnt new_wntr_peak_sm_cust_cnt, b.wntr_peak_total_cust_cnt new_wntr_peak_total_cust_cnt 
               from
                    '||targetSchema||'.trf_peak C,
                    '||sourceSchema||'.CALC_TRF_PEAK_WNTR_2 B
               where 
                    C.TRF_ID = B.TRF_ID
          )
          set 
               old_wntr_peak_date=new_wntr_peak_date, 
               old_wntr_kva=new_wntr_kva, 
               old_wntr_cap=new_wntr_cap, 
               old_wntr_pct=new_wntr_pct, 
               old_wntr_peak_sm_cust_cnt=new_wntr_peak_sm_cust_cnt, 
               old_wntr_peak_total_cust_cnt=new_wntr_peak_total_cust_cnt';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update Data Start Date
     v_stmt := '
          truncate table '||sourceSchema||'.calc_trf_data_start_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||sourceSchema||'.calc_trf_data_start_date
          select 
               trf_id, 
               min(BATCH_DATE) data_start_date 
          from '||targetSchema||'.TRF_PEAK_HIST 
          group by trf_id';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          update
          (
               select a.data_start_date old_data_start_date, b.data_start_date new_data_start_date
               from
                    '||targetSchema||'.TRF_PEAK a,
                    '||sourceSchema||'.CALC_TRF_DATA_START_DATE b
               where
                    a.trf_id = b.trf_id
          )
          set
               old_data_start_date = new_data_start_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- delete TRFs with null winter and summer timestamp
     v_stmt := '
          delete from 
               '||targetSchema||'.TRF_PEAK
          where 
               SMR_PEAK_DATE is null and WNTR_PEAK_DATE is null';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     -- deduct rows deleted from total count
     v_totalRowsMigrated := v_totalRowsMigrated - SQL%ROWCOUNT;
     
     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'POPULATE_TRF_PEAK_TABLE',v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRF_PEAK'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;  
     
     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'POPULATE_TRF_PEAK_TABLE', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=POPULATE_TRF_PEAK_TABLE');
        
END POPULATE_TRF_PEAK_TABLE;

/
--------------------------------------------------------
--  DDL for Function UPDATE_TRF_PEAK_HIST
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."UPDATE_TRF_PEAK_HIST" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     p_region VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
BEGIN
     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'UPDATE_TRF_PEAK_HIST');

     -- truncate work tables
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_SMART_COUNT';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_PEAK_HIST';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''SP_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;     
          
     -- insert into work tables
     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_SMART_COUNT
          SELECT 
               b.TRF_PEAK_HIST_ID,  b.SM_FLG,  COUNT(*) sm_count
          FROM 
               '||targetSchema||'.SP_PEAK_HIST b
          WHERE 
               b.SM_FLG = ''S''
          GROUP BY b.TRF_PEAK_HIST_ID, b.SM_FLG';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||sourceSchema||''',
               tabname => ''CALC_TRF_SMART_COUNT'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_PEAK_HIST
               select a.TRF_PEAK_HIST_ID, 
               sum(a.SP_KVA_TRF_PEAK) total_trf_peak_kva, 
               count(*) all_count, 
               c.sm_count 
          from
               '||targetSchema||'.TRF_PEAK_HIST d, 
               '||targetSchema||'.TRANSFORMER e,
               '||targetSchema||'.SP_PEAK_HIST a
                    left outer join
                         '||sourceSchema||'.CALC_TRF_SMART_COUNT c
                    on c.TRF_PEAK_HIST_ID = a.TRF_PEAK_HIST_ID
          where 
               a.TRF_PEAK_HIST_ID=d.ID  and
               d.TRF_ID=e.ID and
               e.REGION in ('||p_region||')
          group by a.TRF_PEAK_HIST_ID, c.sm_count          
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats 
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''TRF_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||sourceSchema||''',
               tabname => ''CALC_TRF_PEAK_HIST'',
               estimate_percent => 1
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- update TRF_PEAK_HIST
     v_stmt := '
          update '||targetSchema||'.TRF_PEAK_HIST a set
               a.TRF_PEAK_KVA = NVL((select b.TOTAL_TRF_PEAK_KVA from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.CCB_CUST_TOTAL = NVL((select b.ALL_COUNT from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.SM_CUST_TOTAL = NVL((select b.sm_count from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0)
          where 
               a.TRF_ID in (select b.ID from '||targetSchema||'.TRANSFORMER b where b.region in ('||p_region||'))
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- delete records with 0 or null service point counts
     -- need to investigate why there are a lot of sm_sp_load records where SP ID do not exist in meter table
     v_stmt := '
          delete from 
               '||targetSchema||'.TRF_PEAK_HIST
          where 
               CCB_CUST_TOTAL is null or CCB_CUST_TOTAL = 0';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     -- deduct rows deleted from total count
     v_totalRowsMigrated := v_totalRowsMigrated - SQL%ROWCOUNT;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'UPDATE_TRF_PEAK_HIST',v_totalRowsMigrated);
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'UPDATE_TRF_PEAK_HIST', sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=UPDATE_TRF_PEAK_HIST');
        
END UPDATE_TRF_PEAK_HIST;

/
--------------------------------------------------------
--  DDL for Function UPDATE_TRF_PEAK_HIST_CAP
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."UPDATE_TRF_PEAK_HIST_CAP" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     batchDate Date)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_season VARCHAR2(1);
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
BEGIN
     --determine season
     v_batchMonth := SUBSTR((to_char(batchDate,'MONTH')),1,3);
     if (upper(v_batchMonth) in ('NOV','DEC','JAN','FEB','MAR')) then 
          v_season := 'W';
     else
          v_season := 'S';
     end if;
     DBMS_OUTPUT.PUT_LINE('SEASON - '||v_season);

     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'UPDATE_TRF_PEAK_HIST_CAP_'||to_char(batchDate, 'mmddyyyy'));

     -- truncate work tables
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_TRF_HIST_CAP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     v_stmt := '
          truncate table '||sourceSchema||'.CALC_CUST_TYP_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
          
     -- insert into work tables
     v_stmt := '
          insert into '||sourceSchema||'.CALC_TRF_HIST_CAP (TRF_PEAK_HIST_ID)
               select a.ID 
               from 
                    '||targetSchema||'.TRF_PEAK_HIST a
               where 
                    a.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||sourceSchema||'.CALC_CUST_TYP_TOTAL_KVA
               select 
                    a.TRF_PEAK_HIST_ID, 
                    ''DOMESTIC'' cust_typ, 
                    sum(a.SP_KVA_TRF_PEAK) total_kva
               from 
                    '||targetSchema||'.SP_PEAK_HIST a, 
                    '||targetSchema||'.TRF_PEAK_HIST b 
               where 
                    a.TRF_PEAK_HIST_ID = b.ID and
                    b.BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                    a.CUST_TYP = ''DOM'' 
               group by a.TRF_PEAK_HIST_ID
               union all
               select 
                    a.TRF_PEAK_HIST_ID, 
                    ''NON_DOMESTIC'' cust_typ, 
                    sum(a.SP_KVA_TRF_PEAK) total_kva
               from 
                    '||targetSchema||'.SP_PEAK_HIST a, 
                    '||targetSchema||'.TRF_PEAK_HIST b 
               where 
                    a.TRF_PEAK_HIST_ID = b.ID and
                    b.BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                    a.CUST_TYP <> ''DOM'' 
               group by a.TRF_PEAK_HIST_ID';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
                  
     -- update TRF_PEAK_HIST
     -- set residential total KVA
     v_stmt := '
          update '||sourceSchema||'.CALC_TRF_HIST_CAP a set 
               DOM_TOTAL_KVA = 
                    NVL
                    (
                          (
                          select 
                               b.total_kva 
                          from 
                               '||sourceSchema||'.CALC_CUST_TYP_TOTAL_KVA b
                          where 
                               a.TRF_PEAK_HIST_ID = b.TRF_PEAK_HIST_ID and
                               b.CUST_TYP=''DOMESTIC''
                          ),
                          0
                     )';     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- set non-residential total KVA
     v_stmt := '
          update '||sourceSchema||'.CALC_TRF_HIST_CAP a set 
               NON_DOM_TOTAL_KVA = 
                    NVL
                    (
                          (
                          select 
                               b.total_kva 
                          from 
                               '||sourceSchema||'.CALC_CUST_TYP_TOTAL_KVA b
                          where 
                               a.TRF_PEAK_HIST_ID = b.TRF_PEAK_HIST_ID and
                               b.CUST_TYP=''NON_DOMESTIC''
                          ),
                          0
                     )';     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     --if residential has more load overall than non-residential, set CUST_TYP_WITH_GREATER_LOAD to 'DOMESTIC'
     v_stmt := '
          update '||sourceSchema||'.CALC_TRF_HIST_CAP a set 
               a.CUST_TYP_WITH_GREATER_LOAD = ''DOMESTIC''
          where 
               a.DOM_TOTAL_KVA > a.NON_DOM_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate name plate total kva
     v_stmt := '    
          update '||sourceSchema||'.CALC_TRF_HIST_CAP a set 
          a.TRF_NP_TOTAL_KVA = 
          NVL
          (
               (
               select b.TOTAL_NP_CAP 
               from (
                    select 
                         a.ID TRF_PEAK_HIST_ID, 
                         sum(c.NP_KVA) TOTAL_NP_CAP 
                    from 
                         '||targetSchema||'.TRF_PEAK_HIST a,
                         '||targetSchema||'.TRANSFORMER b,
                         '||targetSchema||'.TRANSFORMER_BANK c
                    where 
                         a.TRF_ID = b.ID and
                         c.TRF_ID = b.ID
                    group by a.ID
                    ) b
               where 
                    a.TRF_PEAK_HIST_ID = b.TRF_PEAK_HIST_ID
               ),
               0
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- finally, update TRF_PEAK_HIST.TRF_CAP
     v_stmt := '    
          update '||targetSchema||'.TRF_PEAK_HIST x set
          TRF_CAP = 
          NVL
          (
               (
               select y.trf_cap 
               from (
                    select 
                         b.ID, 
                         a.TRF_NP_TOTAL_KVA*decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'', d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from 
                         '||sourceSchema||'.CALC_TRF_HIST_CAP a,
                         '||targetSchema||'.TRF_PEAK_HIST b,
                         '||targetSchema||'.TRANSFORMER c,
                         '||sourceSchema||'.TRF_NP_CAP_MULT d
                    where
                         a.TRF_PEAK_HIST_ID = b.id and
                         b.TRF_ID = c.ID and
                         d.SEASON = '''||v_season||''' and
                         a.TRF_NP_TOTAL_KVA >= d.KVA_LOW and
                         a.TRF_NP_TOTAL_KVA <= d.KVA_HIGH and
                         d.PHASE_CD = c.PHASE_CD and
                         d.INSTALLATION_TYP = c.INSTALLATION_TYP and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG
                    ) Y
               where 
                    x.id = y.id
               ),
               0
          )
          where x.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')';     
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     
     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'UPDATE_TRF_PEAK_HIST_CAP_'||to_char(batchDate, 'mmddyyyy'),SQL%ROWCOUNT);
  
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'UPDATE_TRF_PEAK_HIST_CAP_'||to_char(batchDate, 'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=UPDATE_TRF_PEAK_HIST_CAP_'||to_char(batchDate, 'mmddyyyy'));
        
END UPDATE_TRF_PEAK_HIST_CAP;

/
--------------------------------------------------------
--  DDL for Function UPD_MIGRATION_LOG_STARTDATE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "EDTLMMIGRATION"."UPD_MIGRATION_LOG_STARTDATE" (
    wave_param                 VARCHAR2,
    table_name_param           VARCHAR2)
  RETURN VARCHAR2
AS
BEGIN
  DBMS_OUTPUT.PUT_LINE('DEBUG - Wave: '||wave_param||' Table Name: '||table_name_param);
  UPDATE EDTLMMIGRATION.MIGRATION_LOG
  SET 
    MIG_START_TS             = sysdate
  WHERE WAVE               = wave_param
  AND TABLE_NAME           = table_name_param;
  COMMIT;
  RETURN 'SUCCESS';
END UPD_MIGRATION_LOG_STARTDATE;

/
--------------------------------------------------------
--  DDL for Procedure CLEANUP_EDER_TRF_BANK_MAPPING
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDTLMMIGRATION"."CLEANUP_EDER_TRF_BANK_MAPPING" 
IS
     CURSOR deviceid_cur
     IS
          select distinct device_id from EDER_TRANSFORMER_BANK_MAPPING where bank_cd is null;
          
     CURSOR deviceid_cur2
     IS
          select device_id from EDER_TRANSFORMER_BANK_MAPPING group by DEVICE_ID,BANK_CD
               having count(*) > 1;
     v_device_id  NUMBER;
BEGIN
     ---
     --- clean up null bank codes
     ---
     ---
     OPEN deviceid_cur;

     LOOP
          FETCH deviceid_cur INTO v_device_id;
          EXIT WHEN deviceid_cur%NOTFOUND;
          
          -- first, set all bank codes for this TRF to null
          update EDER_TRANSFORMER_BANK_MAPPING set BANK_CD = NULL
          where 
               DEVICE_ID = v_device_id;
               
          -- then renumber the bank codes
          update EDER_TRANSFORMER_BANK_MAPPING set BANK_CD = ROWNUM
          where 
               DEVICE_ID = v_device_id;
     END LOOP;
     CLOSE deviceid_cur;
     
     --- clean up duplicate deviceid+bankcode, pick the first one
     OPEN deviceid_cur2;

     LOOP
          FETCH deviceid_cur2 INTO v_device_id;
          EXIT WHEN deviceid_cur2%NOTFOUND;
          
          -- first, set all bank codes for this TRF to null
          update EDER_TRANSFORMER_BANK_MAPPING set BANK_CD = NULL
          where 
               DEVICE_ID = v_device_id;
               
          -- then renumber the bank codes
          update EDER_TRANSFORMER_BANK_MAPPING set BANK_CD = ROWNUM
          where 
               DEVICE_ID = v_device_id;
     END LOOP;
     CLOSE deviceid_cur2;
     
END CLEANUP_EDER_TRF_BANK_MAPPING;

/
--------------------------------------------------------
--  DDL for Procedure MIGRATE_CEDSA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDTLMMIGRATION"."MIGRATE_CEDSA" (
        wave         VARCHAR2,
        sourceSchema VARCHAR2,
        targetSchema VARCHAR2,
        batchEndDate DATE,
        region VARCHAR2)
    AS
      -- This procedure accepts four arguments:
      -- 1. wave - whether this migration is I - initial, W - wave, F - final
      -- 2. sourceSchema - where the source CEDSA tables reside, e.g., edtlmmigration schema
      -- 3. targetSchema - where the TLM tables reside, e.g., edtlm schema
      -- 4. batchEndDate - latest batch date to process, usually a month or two prior, TBD
      v_log_status VARCHAR2(10);
      v_cnt INTEGER;
      v_date DATE := batchEndDate;
    BEGIN

      --
      -- Populate EDER mapping tables
      --
      IF EDTLMMIGRATION.POPULATE_EDER_TRF_MAPPING (wave, sourceSchema, targetSchema) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table EDER_TRANSFORMER_MAPPING successfully migrated');
      END IF;
      
      IF EDTLMMIGRATION.POPULATE_EDER_TRF_BANK_MAPPING (wave, sourceSchema, targetSchema) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table EDER_TRANSFORMER_BANK_MAPPING successfully migrated');
      END IF;
      -- there are records coming in from EDGIS without bank code so assign them
      CLEANUP_EDER_TRF_BANK_MAPPING();
      
      IF EDTLMMIGRATION.POPULATE_EDER_METER_MAPPING (wave, sourceSchema, targetSchema) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table EDER_METER_MAPPING successfully migrated');
      END IF;
      --
      -- END Populate EDER mapping tables
      --      
      
      IF EDTLMMIGRATION.MIGRATE_TRANSFORMER (wave, sourceSchema, targetSchema, region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRANSFORMER successfully migrated');
      END IF;
      IF EDTLMMIGRATION.MIGRATE_TRANSFORMER_BANK (wave, sourceSchema, targetSchema,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRANSFORMER_BANK successfully migrated');
      END IF;
      IF EDTLMMIGRATION.MIGRATE_METER (wave, sourceSchema, targetSchema,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table METER successfully migrated');
      END IF;
 
      IF MIGRATE_TRF_PEAK_HIST_SMART (wave, sourceSchema, targetSchema,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - SMART METERS successfully migrated');
      END IF;
      IF MIGRATE_TRF_PEAK_HIST_LEGACY (wave, sourceSchema, targetSchema, batchEndDate,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - LEGACY METERS successfully migrated');
      END IF;

      IF MIG_SP_PEAK_HST_INS_BATCH_LOGS(wave, batchEndDate) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - Batch Records successfully created');
      END IF;
    
      v_cnt := 36;
      v_date := batchEndDate;
      while v_cnt > 0 loop
         v_cnt := v_cnt - 1;
          IF MIGRATE_SP_PEAK_HIST_SMART(wave, sourceSchema, targetSchema, v_date,region) = 'TRUE' THEN
               DBMS_OUTPUT.PUT_LINE('Table SP_PEAK_HIST (SMART) Batch '||v_date||' Records successfully migrated');
          END IF;    
          v_date := ADD_MONTHS(v_date, -1);
      end loop;

      v_cnt := 12;      
      v_date := batchEndDate;
      while v_cnt > 0 loop
          v_cnt := v_cnt - 1;
          IF MIGRATE_SP_PEAK_HIST_LEGACY(wave, sourceSchema, targetSchema, v_date,region) = 'TRUE' THEN
               DBMS_OUTPUT.PUT_LINE('Table SP_PEAK_HIST (LEGACY) - Batch '||v_date||' Records successfully migrated');
          END IF;    
          v_date := ADD_MONTHS(v_date, -1);
      end loop;

      -- update fields (except CAP) in TRF_PEAK_HIST
      IF UPDATE_TRF_PEAK_HIST (wave, sourceSchema, targetSchema,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST successfully updated');
      END IF;

      -- update CAP in TRF_PEAK_HIST by month
      v_cnt := 36;
      v_date := batchEndDate;  -- reset batch date
      while v_cnt > 0 loop
         v_cnt := v_cnt - 1;
          IF UPDATE_TRF_PEAK_HIST_CAP(wave, sourceSchema, targetSchema, v_date) = 'TRUE' THEN
               DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST (Capacity) '||v_date||' successfully updated');
          END IF;    
          v_date := ADD_MONTHS(v_date, -1);
      end loop;
      
      -- Populate 12 month peak summary table
      IF POPULATE_TRF_PEAK_TABLE (wave, sourceSchema, targetSchema, batchEndDate,region) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK successfully populated');
      END IF;
      
      IF POPULATE_TRF_PEAK_BY_CUST_TYP (wave, sourceSchema, targetSchema) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_BY_CUST_TYP successfully populated');
      END IF;

    END MIGRATE_CEDSA;

/


spool off
