Prompt drop Function LOAD_TRF_PEAK_HIST_LEGACY;
DROP FUNCTION EDTLM.LOAD_TRF_PEAK_HIST_LEGACY
/

Prompt Function LOAD_TRF_PEAK_HIST_LEGACY;
--
-- LOAD_TRF_PEAK_HIST_LEGACY  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_TRF_PEAK_HIST_LEGACY" (
     p_schema VARCHAR2,
     batchDate DATE)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
-- Changes created by Jim Moore 8/20/2014 IBM for TLM2
BEGIN
     -- create migration log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('LOAD_TRF_PEAK_HIST_LEGACY_'||to_char(batchDate,'mmddyyyy'));

           v_stmt := '
                insert into '||p_schema||'.TRF_PEAK_HIST (
                     trf_id,
                     batch_date,
                     trf_peak_time,
                     trf_peak_kva,
                     sm_cust_total,
                     ccb_cust_total,
                     --Jim Moore removed from query because the field was removed from the table
                     --, trf_cap
                     TRF_PEAK_KW,
		     TRF_AVE_KW
                    )
                select distinct
                     a.TRF_ID,
                     to_date('''||v_batchDateStr||''',''dd-mon-yy''),
                     to_date('''||v_batchDateStr||''',''dd-mon-yy''),
                     0,
                     NULL,
                     NULL,
                     --Jim Moore removed from query because the field was removed from the table
                     --,0
                     0 TRF_PEAK_KW,
                     0 TRF_AVG_KW
                FROM '
                     ||p_schema||'.METER a, '
                     ||p_schema||'.TRANSFORMER b, '
                     ||p_schema||'.STG_CCB_METER_LOAD c
                WHERE
                     a.SERVICE_POINT_ID    = c.SERVICE_POINT_ID  AND
                     a.TRF_ID      = b.ID        AND
                     not exists (select 1 from '||p_schema||'.trf_peak_hist x where a.trf_id = x.trf_id and x.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy''))
                     ';
           DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('LOAD_TRF_PEAK_HIST_LEGACY_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''TRF_PEAK_HIST'',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('LOAD_TRF_PEAK_HIST_LEGACY_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'Load failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=LOAD_TRF_PEAK_HIST_LEGACY_'||to_char(batchDate,'mmddyyyy'));

END LOAD_TRF_PEAK_HIST_LEGACY;
/
