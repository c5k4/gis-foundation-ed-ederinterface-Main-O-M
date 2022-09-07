Prompt drop Function POPULATE_TRF_PEAK_BY_CUST_TYP;
DROP FUNCTION EDTLM.POPULATE_TRF_PEAK_BY_CUST_TYP
/

Prompt Function POPULATE_TRF_PEAK_BY_CUST_TYP;
--
-- POPULATE_TRF_PEAK_BY_CUST_TYP  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."POPULATE_TRF_PEAK_BY_CUST_TYP" (
     p_schema VARCHAR2,
     endBatchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_trfPeakTable VARCHAR2(50) := 'TRF_PEAK';
     v_trfPeakHistTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_spPeakHistTable VARCHAR2(50) := 'SP_PEAK_HIST';
     v_trfPeakFkColumn VARCHAR2(50) := 'TRF_PEAK_ID';
     v_trfPeakHistFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_BY_CUST_TYP';
BEGIN
     if p_generated = 'GEN' then
          v_trfPeakTable := 'TRF_PEAK_GEN';
          v_trfPeakHistTable := 'TRF_PEAK_GEN_HIST';
          v_spPeakHistTable := 'SP_PEAK_GEN_HIST';
          v_trfPeakFkColumn := 'TRF_PEAK_GEN_ID';
          v_trfPeakHistFkColumn := 'TRF_PEAK_GEN_HIST_ID';
          v_targetTable := 'TRF_PEAK_GEN_BY_CUST_TYP';
     end if;
     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

     -- delete old records
     v_stmt := '
          truncate table '||p_schema||'.'||v_targetTable||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- insert summer peak records
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||'
               ('||v_trfPeakFkColumn||',SEASON,CUST_TYP, season_cust_cnt, season_total_kva)
          select c.ID, ''S'', b.CUST_TYP, count(*), NVL(sum(b.SP_KVA_TRF_PEAK),0)
          from
               '||p_schema||'.'||v_trfPeakHistTable||' a,
               '||p_schema||'.'||v_spPeakHistTable||' b,
               '||p_schema||'.'||v_trfPeakTable||' c
          where
               a.ID = b.'||v_trfPeakHistFkColumn||' and
               c.TRF_ID = a.TRF_ID and
               a.BATCH_DATE = c.SMR_PEAK_DATE
          group by c.ID, ''S'', b.CUST_TYP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- insert winter peak records
     v_stmt := '
          insert into '||p_schema||'.'||v_targetTable||'
               ('||v_trfPeakFkColumn||',SEASON,CUST_TYP, season_cust_cnt, season_total_kva)
          select c.ID, ''W'', b.CUST_TYP, count(*), NVL(sum(b.SP_KVA_TRF_PEAK),0)
          from
               '||p_schema||'.'||v_trfPeakHistTable||' a,
               '||p_schema||'.'||v_spPeakHistTable||' b,
               '||p_schema||'.'||v_trfPeakTable||' c
          where
               a.ID = b.'||v_trfPeakHistFkColumn||' and
               c.TRF_ID = a.TRF_ID and
               a.BATCH_DATE = c.WNTR_PEAK_DATE
          group by c.ID, ''W'', b.CUST_TYP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- update monthly load log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'),SQL%ROWCOUNT);

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

END POPULATE_TRF_PEAK_BY_CUST_TYP;
/
