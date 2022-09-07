Prompt drop Function UPD_TRF_PK_HIST_UND_TRF_SP_REL;
DROP FUNCTION EDTLM.UPD_TRF_PK_HIST_UND_TRF_SP_REL
/

Prompt Function UPD_TRF_PK_HIST_UND_TRF_SP_REL;
--
-- UPD_TRF_PK_HIST_UND_TRF_SP_REL  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPD_TRF_PK_HIST_UND_TRF_SP_REL" (
    p_schema VARCHAR2,
    endBatchDate DATE,
    p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_batchDateStr VARCHAR2(10);
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_sourceTable  VARCHAR2(50) := 'STG_SM_SP_LOAD';
BEGIN

-- Update Load Indeterminable flag for Transformer/Service Point Relationship Error

    v_batchDateStr := to_char(endBatchDate, 'dd-mon-yy');

    if p_generated = 'GEN' then
          v_targetTable := 'TRF_PEAK_GEN_HIST';
          v_sourceTable := 'STG_SM_SP_GEN_LOAD';
    end if;

 -- create migration log record
    v_log_status := INSERT_MONTHLY_LOAD_LOG('UPD_'||v_targetTable||'UND_TRF_SP_REL_'||to_char(endBatchDate,'mmddyyyy'));

    v_stmt := 'update '||p_schema||'.'||v_targetTable||' p
                set p.LOAD_UNDETERMINED = NULL
                where p.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
                and p.LOAD_UNDETERMINED = 5';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;

    v_stmt := '
        update '||p_schema||'.'||v_targetTable||' ph
        set ph.LOAD_UNDETERMINED = ''5''
        where (ph.ID) in (
          SELECT DISTINCT hist.ID
          FROM '||p_schema||'.'||v_targetTable||' hist
          INNER JOIN
          (
            select distinct trf.id from '||p_schema||'.transformer trf
            inner join '||p_schema||'.'||v_sourceTable||' a
            on trf.cgc_id = a.cgc
            where a.service_point_id not in (select service_point_id from '||p_schema||'.meter)
          ) trf
          on hist.trf_id = trf.id
          where hist.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy''))
          and ph.LOAD_UNDETERMINED IS NULL';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;

    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPD_'||v_targetTable||'UND_TRF_SP_REL_'||to_char(endBatchDate,'mmddyyyy'),v_totalRowsMigrated);

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('UPD_'||v_targetTable||'UND_TRF_SP_REL_'||to_char(endBatchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=UPD_UND_TRF_SP_REL_'||to_char(endBatchDate,'mmddyyyy'));
END UPD_TRF_PK_HIST_UND_TRF_SP_REL;
/
