Prompt drop Function LOAD_TRF_PEAK_HIST_SMART;
DROP FUNCTION EDTLM.LOAD_TRF_PEAK_HIST_SMART
/

Prompt Function LOAD_TRF_PEAK_HIST_SMART;
--
-- LOAD_TRF_PEAK_HIST_SMART  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_TRF_PEAK_HIST_SMART" (
     p_schema VARCHAR2,
     batchDate DATE,
     p_generated VARCHAR2
)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
     v_sourceTable VARCHAR2(50) := 'STG_SM_TRF_LOAD';
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_totalRowsMigrated NUMBER := 0;
-- Changes created by Jim Moore 8/27/2014 IBM for TLM2
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'STG_SM_TRF_GEN_LOAD';
          v_targetTable := 'TRF_PEAK_GEN_HIST';
     end if;

     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'));
     v_stmt := '
               insert into '||p_schema||'.'||v_targetTable||' (
               trf_id,
               batch_date,
               trf_peak_time,
               trf_peak_kva,
               sm_cust_total,
               ccb_cust_total,
               --Jim Moore removed from query because the field was removed from the table
               --,trf_cap
               TRF_PEAK_KW,
               TRF_AVE_KW
                )
          select
               b.ID,
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               a.TRF_PEAK_TIME,
               0,
               NULL,
               NULL,
               TRF_PEAK_KW,
               TRF_AVG_KW
               --  0,
         FROM
               '||p_schema||'.'||v_sourceTable||' a,
               '||p_schema||'.TRANSFORMER b

          WHERE
               a.CGC = b.CGC_ID
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     -- update monthly load log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => '''||v_targetTable||''',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'Load failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=LOAD_'||v_targetTable||'_SMART_'||to_char(batchDate,'mmddyyyy'));
END LOAD_TRF_PEAK_HIST_SMART;
/
