Prompt drop Function POPULATE_TRF_BANK_PEAK_TABLE;
DROP FUNCTION EDTLM.POPULATE_TRF_BANK_PEAK_TABLE
/

Prompt Function POPULATE_TRF_BANK_PEAK_TABLE;
--
-- POPULATE_TRF_BANK_PEAK_TABLE  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."POPULATE_TRF_BANK_PEAK_TABLE" (
     p_schema VARCHAR2,
     endBatchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(4000);
     v_stmt1 VARCHAR2(4000);
     v_log_status VARCHAR2(10);
     v_dateStr VARCHAR(10) := to_char(endBatchDate, 'dd-mon-yy');

     v_sourceTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_sourceTable1 VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_sourceTable2 VARCHAR2(50) := 'TRF_PEAK';
     v_targetTable VARCHAR2(50) := 'TRF_BANK_PEAK';

     v_targetTable_Field VARCHAR2(50) := 'TRF_PEAK_ID';
     v_sourceTable1_Field VARCHAR2(50) := 'TRF_PEAK_HIST_ID';

     v_totalRowsMigrated NUMBER := 0;
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'TRF_PEAK_GEN_HIST';
          v_sourceTable1 := 'TRF_BANK_PEAK_GEN_HIST';
          v_sourceTable2 := 'TRF_PEAK_GEN';
          v_targetTable  := 'TRF_BANK_PEAK_GEN';

          v_targetTable_Field := 'TRF_PEAK_GEN_ID';
          v_sourceTable1_Field := 'TRF_PEAK_GEN_HIST_ID';
     end if;
     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

     -- create shell TRF_PEAK records
     v_stmt := '
          truncate table '||p_schema||'.'||v_targetTable||'';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     v_stmt := 'INSERT INTO '||p_schema||'.'||v_targetTable||'(NP_KVA, SMR_KVA, WNTR_KVA, SMR_CAP, WNTR_CAP, SMR_PCT, WNTR_PCT, ' || v_targetTable_Field || ', TRF_BANK_ID)
                SELECT
                  smr_peak.np_kva,
                  smr_peak.smr_kva,
                  wntr_peak.wntr_kva,
                  smr_peak.smr_cap,
                  wntr_peak.wntr_cap,
                  smr_peak.smr_pct,
                  wntr_peak.wntr_pct,
                  nvl(smr_peak.peak_hist_id, wntr_peak.peak_hist_id) peak_hist_id,
                  nvl(smr_peak.trf_bank_id,wntr_peak.trf_bank_id) trf_bank_id
                FROM
                (
                   SELECT /*+use_hash(a b smr2) */
                     p.id as peak_hist_id, a.TRF_ID, a.BATCH_DATE smr_peak_date,
                     b.TRF_PEAK_KVA smr_kva, b.TRF_CAP smr_cap, b.trf_bank_id, b.np_kva,
                     round((b.TRF_PEAK_KVA/b.TRF_CAP)*100,1) smr_pct
                   FROM
                     '||p_schema||'.'||v_sourceTable||' a
                   INNER JOIN '||p_schema||'.CALC_TRF_PEAK_SMR_2 smr2
                      ON a.trf_id = smr2.trf_id and  a.BATCH_DATE = smr2.smr_peak_date
                   INNER JOIN '||p_schema||'.'||v_sourceTable1||' b
                      ON a.id = b.' || v_sourceTable1_Field || '
                   INNER JOIN '||p_schema||'.'||v_sourceTable2||' p
                      ON smr2.trf_id = p.trf_id and smr2.smr_peak_date = p.smr_peak_date
                   WHERE b.TRF_CAP > 0
                ) smr_peak
                LEFT OUTER JOIN
                (
                   SELECT /*+use_hash(a b wntr2) full(a b wntr2)*/
                     p.id as peak_hist_id, a.TRF_ID, a.BATCH_DATE wntr_peak_date,
                     b.TRF_PEAK_KVA wntr_kva, b.TRF_CAP wntr_cap, b.trf_bank_id, b.np_kva,
                     round((b.TRF_PEAK_KVA/b.TRF_CAP)*100,1) wntr_pct
                   FROM
                     '||p_schema||'.'||v_sourceTable||' a
                   INNER JOIN '||p_schema||'.CALC_TRF_PEAK_WNTR_2 wntr2
                      ON a.trf_id = wntr2.trf_id and  a.BATCH_DATE = wntr2.wntr_peak_date
                   INNER JOIN '||p_schema||'.'||v_sourceTable1||' b
                      ON a.id = b.' || v_sourceTable1_Field || '
                   INNER JOIN '||p_schema||'.'||v_sourceTable2||' p
                      ON wntr2.trf_id = p.trf_id and wntr2.wntr_peak_date = p.wntr_peak_date
                   WHERE b.TRF_CAP > 0
                ) wntr_peak
                ON smr_peak.trf_bank_id = wntr_peak.trf_bank_id
                AND smr_peak.np_kva = wntr_peak.np_kva
        UNION  ';
        v_stmt1 := '
                SELECT
                  smr_peak.np_kva,
                  smr_peak.smr_kva,
                  wntr_peak.wntr_kva,
                  smr_peak.smr_cap,
                  wntr_peak.wntr_cap,
                  smr_peak.smr_pct,
                  wntr_peak.wntr_pct,
                  nvl(smr_peak.peak_hist_id, wntr_peak.peak_hist_id) peak_hist_id,
                  nvl(smr_peak.trf_bank_id,wntr_peak.trf_bank_id) trf_bank_id
                FROM
                (
                   SELECT /*+use_hash(a b smr2) */
                     p.id as peak_hist_id, a.TRF_ID, a.BATCH_DATE smr_peak_date,
                     b.TRF_PEAK_KVA smr_kva, b.TRF_CAP smr_cap, b.trf_bank_id, b.np_kva,
                     round((b.TRF_PEAK_KVA/b.TRF_CAP)*100,1) smr_pct
                   FROM
                     '||p_schema||'.'||v_sourceTable||' a
                   INNER JOIN '||p_schema||'.CALC_TRF_PEAK_SMR_2 smr2
                      ON a.trf_id = smr2.trf_id and  a.BATCH_DATE = smr2.smr_peak_date
                   INNER JOIN '||p_schema||'.'||v_sourceTable1||' b
                      ON a.id = b.' || v_sourceTable1_Field || '
                  INNER JOIN '||p_schema||'.'||v_sourceTable2||' p
                      ON smr2.trf_id = p.trf_id and smr2.smr_peak_date = p.smr_peak_date
                  WHERE b.TRF_CAP > 0
                ) smr_peak
                RIGHT OUTER JOIN
                (
                   SELECT /*+use_hash(a b wntr2) full(a b wntr2)*/
                     p.id as peak_hist_id, a.TRF_ID, a.BATCH_DATE wntr_peak_date,
                     b.TRF_PEAK_KVA wntr_kva, b.TRF_CAP wntr_cap, b.trf_bank_id, b.np_kva,
                     round((b.TRF_PEAK_KVA/b.TRF_CAP)*100,1) wntr_pct
                   FROM
                     '||p_schema||'.'||v_sourceTable||' a
                   INNER JOIN '||p_schema||'.CALC_TRF_PEAK_WNTR_2 wntr2
                      ON a.trf_id = wntr2.trf_id and  a.BATCH_DATE = wntr2.wntr_peak_date
                   INNER JOIN '||p_schema||'.'||v_sourceTable1||' b
                      ON a.id = b.' || v_sourceTable1_Field || '
                   INNER JOIN '||p_schema||'.'||v_sourceTable2||' p
                      ON wntr2.trf_id = p.trf_id and wntr2.wntr_peak_date = p.wntr_peak_date
                   WHERE b.TRF_CAP > 0
                ) wntr_peak
                ON smr_peak.trf_bank_id = wntr_peak.trf_bank_id
                AND smr_peak.np_kva = wntr_peak.np_kva
                ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt||v_stmt1);

     execute immediate v_stmt||v_stmt1;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
                   ownname => '''||p_schema||''',
               tabname => '''||v_targetTable||''',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

END POPULATE_TRF_BANK_PEAK_TABLE;
/
