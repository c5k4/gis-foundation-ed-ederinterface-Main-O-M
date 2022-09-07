Prompt drop Function UPDATE_TRF_PEAK_HIST;
DROP FUNCTION EDTLM.UPDATE_TRF_PEAK_HIST
/

Prompt Function UPDATE_TRF_PEAK_HIST;
--
-- UPDATE_TRF_PEAK_HIST  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPDATE_TRF_PEAK_HIST" (
     p_schema VARCHAR2,
     batchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
     v_sourceTable VARCHAR2(50) := 'SP_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'SP_PEAK_GEN_HIST';
          v_targetTable := 'TRF_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;

     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'));

     -- truncate work tables
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_SMART_COUNT';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_HIST';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_HIST_DOM';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => '''||v_sourceTable||''',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- insert into work tables
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_SMART_COUNT
          SELECT
               b.'||v_targetTableFkColumn||',  b.SM_FLG,  COUNT(*) sm_count, sum(b.sp_peak_kw) kw_sum
          FROM
               '||p_schema||'.'||v_sourceTable||' b
          WHERE
               b.SM_FLG = ''S''
          GROUP BY b.'||v_targetTableFkColumn||', b.SM_FLG';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''CALC_TRF_SMART_COUNT'',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_HIST_DOM
          select a.'||v_targetTableFkColumn||',
               sum(a.SP_KVA_TRF_PEAK)
          from
               '||p_schema||'.'||v_targetTable||' d,
               '||p_schema||'.METER b,
               '||p_schema||'.TRANSFORMER e,
               '||p_schema||'.'||v_sourceTable||' a
          where
               a.'||v_targetTableFkColumn||'=d.ID  and
               d.TRF_ID=e.ID and
               a.SERVICE_POINT_ID = b.SERVICE_POINT_ID and
               b.cust_typ = ''DOM''
          group by a.'||v_targetTableFkColumn||'
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_HIST
               select a.'||v_targetTableFkColumn||',
               sum(a.SP_KVA_TRF_PEAK) total_trf_peak_kva,
               count(*) all_count,
               c.sm_count
          from
               '||p_schema||'.'||v_targetTable||' d,
               '||p_schema||'.TRANSFORMER e,
               '||p_schema||'.'||v_sourceTable||' a
                    left outer join
                         '||p_schema||'.CALC_TRF_SMART_COUNT c
                    on c.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||'
          where
               a.'||v_targetTableFkColumn||'=d.ID  and
               d.TRF_ID=e.ID and
               d.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')
          group by a.'||v_targetTableFkColumn||', c.sm_count
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => '''||v_targetTable||''',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''CALC_TRF_PEAK_HIST'',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''CALC_TRF_PEAK_HIST_DOM'',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update TRF_PEAK_HIST
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' a set
               a.TRF_PEAK_KVA = NVL((select b.TOTAL_TRF_PEAK_KVA from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.TRF_PEAK_KVA_DOM = NVL((select b.TOTAL_TRF_PEAK_KVA from CALC_TRF_PEAK_HIST_DOM b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.TRF_PEAK_KW = NVL((select b.KW_SUM from CALC_TRF_SMART_COUNT b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.CCB_CUST_TOTAL = NVL((select b.ALL_COUNT from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0),
               a.SM_CUST_TOTAL = NVL((select b.sm_count from CALC_TRF_PEAK_HIST b where a.ID = b.TRF_PEAK_HIST_ID),0)
          where
               a.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'),SQL%ROWCOUNT);

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'));

END UPDATE_TRF_PEAK_HIST;
/
