Prompt drop Function LOAD_SP_PEAK_HIST_SMART;
DROP FUNCTION EDTLM.LOAD_SP_PEAK_HIST_SMART
/

Prompt Function LOAD_SP_PEAK_HIST_SMART;
--
-- LOAD_SP_PEAK_HIST_SMART  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_SP_PEAK_HIST_SMART" (
     p_schema VARCHAR2,
     batchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_seasonal_pf NUMBER;
     v_sourceTable VARCHAR2(50) := 'STG_SM_SP_LOAD';
     v_targetTable VARCHAR2(50) := 'SP_PEAK_HIST';
     v_targetTrfTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'STG_SM_SP_GEN_LOAD';
          v_targetTable := 'SP_PEAK_GEN_HIST';
          v_targetTrfTable := 'TRF_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;

     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     if (to_char(batchDate, 'mm') in ('11','12','01','02','03')) then
          v_seasonal_pf := .95;
     else
          v_seasonal_pf := .85;
     end if;

     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'));

     ----------------------------------------------------------------------------------------------
     -- Scnenario 1 - Smart, domestic/non-domestic, with KVAR
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||' (
               SERVICE_POINT_ID,
               '||v_targetTableFkColumn||',
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id,
               sp_peak_kw,
               sp_kw_trf_peak)
          select
               b.service_point_id,
               (select id from '||p_schema||'.'||v_targetTrfTable||' x where x.trf_id=c.id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME,
               round(SQRT(POWER(B.SP_PEAK_KW,2) + POWER(B.SP_PEAK_KVAR,2)),1),
               round(SQRT(POWER(B.SP_KW_TRF_PEAK,2) + POWER(B.TRF_PEAK_KVAR,2)),1),
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG,
               B.INT_LEN,
               a.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               a.ID,
               --NULL, --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               b.sp_peak_kw,
               b.sp_kw_trf_peak
          FROM '
               ||p_schema||'.METER a, ' --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               ||p_schema||'.'||v_sourceTable||' b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
               b.CGC = c.CGC_ID and
               (B.SP_PEAK_KVAR is not null) and --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               a.service_point_id = b.service_point_id
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
               ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;


     ----------------------------------------------------------------------------------------------
     -- Scnenario 2 - Smart, domestic, no KVAR
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||' (
               SERVICE_POINT_ID,
               '||v_targetTableFkColumn||',
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id,
               sp_peak_kw,
               sp_kw_trf_peak)
          select
               b.service_point_id,
               (select id from '||p_schema||'.'||v_targetTrfTable||' x where x.trf_id=c.id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME,
               round(B.SP_PEAK_KW/'||v_seasonal_pf||',1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/'||v_seasonal_pf||',1) as sp_kva_trf_peak,
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG,
               B.INT_LEN,
               a.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               a.ID, --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               --NULL, --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               b.sp_peak_kw,
               b.sp_kw_trf_peak
          FROM '
               ||p_schema||'.METER a, ' --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               ||p_schema||'.'||v_sourceTable||' b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
               b.CGC = c.CGC_ID and
               a.cust_typ = ''DOM'' and --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               (B.SP_PEAK_KVAR is null) and --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               a.service_point_id = b.service_point_id
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     ----------------------------------------------------------------------------------------------
     -- Scnenario 3 Smart, non-domestic, no KVAR, cust type COM
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||' (
               SERVICE_POINT_ID,
               '||v_targetTableFkColumn||',
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id,
               sp_peak_kw,
               sp_kw_trf_peak)
          select
               b.service_point_id,
               (select id from '||p_schema||'.'||v_targetTrfTable||' x where x.trf_id=c.id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME,
               round(B.SP_PEAK_KW/'||v_seasonal_pf||',1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/'||v_seasonal_pf||',1) as sp_kva_trf_peak,
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG,
               B.INT_LEN,
               a.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               a.ID, --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               --NULL,
               b.sp_peak_kw,
               b.sp_kw_trf_peak
          FROM '
               ||p_schema||'.METER a, ' --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               ||p_schema||'.'||v_sourceTable||' b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
               b.CGC = c.CGC_ID and
               a.cust_typ = ''COM'' and
               (B.SP_PEAK_KVAR is null) and --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               a.service_point_id = b.service_point_id
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;


     ----------------------------------------------------------------------------------------------
     -- Scnenario 4 - Smart, non-domestic, no KVAR, cust type ARG, IND, OTH
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||' (
               SERVICE_POINT_ID,
               '||v_targetTableFkColumn||',
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               vee_sp_kw_flg,
               int_len,
               cust_typ,
               vee_trf_kw_flg,
               sm_flg,
               meter_id,
               sp_peak_kw,
               sp_kw_trf_peak)
          select
               b.service_point_id,
               (select id from '||p_schema||'.'||v_targetTrfTable||' x where x.trf_id=c.id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               B.SP_PEAK_TIME,
               round(B.SP_PEAK_KW/.85,1) as sp_peak_kva,
               round(B.SP_KW_TRF_PEAK/.85,1) as sp_kva_trf_peak,
               B.VEE_SP_KW_FLAG as VEE_SP_KW_FLG,
               B.INT_LEN,
               a.CUST_TYP,
               B.VEE_TRF_KW_FLAG as VEE_TRF_KW_FLG,
               ''S'' as sm_flg,
               a.ID, --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               --NULL,
               b.sp_peak_kw,
               b.sp_kw_trf_peak
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.'||v_sourceTable||' b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
               b.CGC = c.CGC_ID and
               a.cust_typ in (''AGR'',''IND'',''OTH'') and
               (B.SP_PEAK_KVAR is null) and --s2nn 05142015 --TEMPORARY, UNTIL BUG WHERE METER_ID IN RAW METER TABLE DOES NOT EXIST IN TARGET METER
               a.service_point_id = b.service_point_id
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     -- clean up.
     -- convert negative calculated SP_PEAK_KVA to .1
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' set SP_PEAK_KVA = .1
          where
               SP_PEAK_KVA < 0 and
               '||v_targetTableFkColumn||' in
               (select ID from '||p_schema||'.'||v_targetTrfTable||' where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- convert negative calculated SP_KVA_TRF_PEAK to .1
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' set SP_KVA_TRF_PEAK = .1
          where
               SP_KVA_TRF_PEAK < 0 and
               '||v_targetTableFkColumn||' in
               (select ID from '||p_schema||'.'||v_targetTrfTable||' where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''SP_PEAK_HIST'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'Load failed.  Please check entry in MONTHLY_LOAD_LOG for tablename='||v_targetTable||'_SMART'||to_char(batchDate,'mmddyyyy'));

END LOAD_SP_PEAK_HIST_SMART;
/
