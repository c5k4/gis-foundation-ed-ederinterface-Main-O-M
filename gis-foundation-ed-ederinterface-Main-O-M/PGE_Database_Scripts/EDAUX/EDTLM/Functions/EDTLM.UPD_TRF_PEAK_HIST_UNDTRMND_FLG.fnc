Prompt drop Function UPD_TRF_PEAK_HIST_UNDTRMND_FLG;
DROP FUNCTION EDTLM.UPD_TRF_PEAK_HIST_UNDTRMND_FLG
/

Prompt Function UPD_TRF_PEAK_HIST_UNDTRMND_FLG;
--
-- UPD_TRF_PEAK_HIST_UNDTRMND_FLG  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPD_TRF_PEAK_HIST_UNDTRMND_FLG" (
    p_schema VARCHAR2,
    endBatchDate DATE,
    p_generated VARCHAR2)
RETURN VARCHAR2
IS
     --v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_dateStr VARCHAR(10) := to_char(endBatchDate, 'dd-mon-yy');
     v_totalRowsMigrated NUMBER := 0;
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_sourceTable VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_sourceField VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
     v_batchDateStr VARCHAR2(10);
BEGIN

    v_batchDateStr := to_char(endBatchDate, 'dd-mon-yy');

    if p_generated = 'GEN' then
          v_targetTable := 'TRF_PEAK_GEN_HIST';
         -- v_sourceTable := 'TRF_BANK_PEAK_GEN_HIST';
         -- v_sourceField := 'TRF_PEAK_GEN_HIST_ID';
    end if;
 -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('UPDATE UNDETERMINED FLG_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

     -- Clean LOAD_UNDETERMINED field before update. Value -- 5 is being handled in seperate function
     v_stmt := 'update '||p_schema||'.'||v_targetTable||' p
                set p.LOAD_UNDETERMINED = NULL
                where p.LOAD_UNDETERMINED IS NOT NULL
                and p.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
                and p.LOAD_UNDETERMINED <> 5';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;

    -- Multi-Banked units with greater than 3 transformers
    v_stmt := '
      update '||p_schema||'.'||v_targetTable||' ph
      set ph.LOAD_UNDETERMINED = ''1''
      where (ph.ID) in (
      select p.id
      from '||p_schema||'.'||v_targetTable||' p
      inner join '||p_schema||'.TRANSFORMER tr
      on p.TRF_ID = tr.ID
      inner join '||p_schema||'.TRANSFORMER_BANK trb
      on p.TRF_ID = trb.TRF_ID
      group by p.id, p.BATCH_DATE
      having count(trb.ID) > 3)
      and ph.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
      and ph.LOAD_UNDETERMINED IS NULL
    ';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;
    DBMS_OUTPUT.PUT_LINE('EXECUTED - ph.LOAD_UNDETERMINED = 1');
    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;


    -- Duplex units (type 21 or 32) where the physical transformer's name plate kVA
    -- does not match the allowed combination
     v_stmt := '
      update '||p_schema||'.'||v_targetTable||' ph
      set ph.LOAD_UNDETERMINED = ''3''
      where (ph.ID) in (
      select p.id
      from '||p_schema||'.'||v_targetTable||' p
      inner join '||p_schema||'.TRANSFORMER tr
      on p.TRF_ID = tr.ID
      inner join '||p_schema||'.TRANSFORMER_BANK trb
      on p.TRF_ID = trb.TRF_ID
      where trb.trf_typ in (21,32)
      and trb.np_kva not in (35, 60, 90, 125, 150)
      group by p.id, p.BATCH_DATE)
      and ph.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
      and ph.LOAD_UNDETERMINED IS NULL
    ';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;
    DBMS_OUTPUT.PUT_LINE('EXECUTED - ph.LOAD_UNDETERMINED = 3');
    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;


    -- Duplex Transformers - Installations tagged as Duplex (type 21 or 32)
    -- that have two physical transformer records
    v_stmt := '
      update '||p_schema||'.'||v_targetTable||' ph
      set ph.LOAD_UNDETERMINED = ''2''
      where (ph.ID) in (
      select p.id
      from '||p_schema||'.'||v_targetTable||' p
      inner join '||p_schema||'.TRANSFORMER tr
      on p.TRF_ID = tr.ID
      inner join '||p_schema||'.TRANSFORMER_BANK trb
      on p.TRF_ID = trb.TRF_ID
      where trb.trf_typ in (21,32)
      group by p.id, p.BATCH_DATE
      having count(trb.ID) = 2)
      and ph.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
      and ph.LOAD_UNDETERMINED IS NULL
    ';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;
    DBMS_OUTPUT.PUT_LINE('EXECUTED - ph.LOAD_UNDETERMINED = 2');
    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;


    -- Duplex Transformers - Invalid multi-banked units
    -- where one transformer is type duplex
    -- and the other is a non-duplex type.
    v_stmt := '
      update '||p_schema||'.'||v_targetTable||' ph
      set ph.LOAD_UNDETERMINED = ''2''
      where (ph.ID) in (
        select DISTINCT dup_1.id from
        (
          select x.id from
          (
            select p.id, p.TRF_ID
            from '||p_schema||'.'||v_targetTable||' p
            inner join '||p_schema||'.TRANSFORMER tr
            on p.TRF_ID = tr.ID
            inner join '||p_schema||'.TRANSFORMER_BANK trb
            on p.TRF_ID = trb.TRF_ID
            group by p.id, p.TRF_ID, p.BATCH_DATE
            having count(trb.trf_id) = 2
          ) x
          inner join '||p_schema||'.TRANSFORMER tr
          on x.TRF_ID = tr.ID
          inner join '||p_schema||'.TRANSFORMER_BANK tb
          on x.TRF_ID = tb.TRF_ID
          where tb.trf_typ not in (21,32)
        ) dup_1
        inner join
        (
          select x.id from
          (
            select p.id, p.TRF_ID
            from '||p_schema||'.'||v_targetTable||' p
            inner join '||p_schema||'.TRANSFORMER tr
            on p.TRF_ID = tr.ID
            inner join '||p_schema||'.TRANSFORMER_BANK trb
            on p.TRF_ID = trb.TRF_ID
            group by p.id, p.TRF_ID, p.BATCH_DATE
            having count(trb.trf_id) = 2
          ) x
          inner join '||p_schema||'.TRANSFORMER tr
          on x.TRF_ID = tr.ID
          inner join '||p_schema||'.TRANSFORMER_BANK tb
          on x.TRF_ID = tb.TRF_ID
          where tb.trf_typ in (21,32)
        ) dup_2
        on dup_1.id = dup_2.id)
        and ph.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
        and ph.LOAD_UNDETERMINED IS NULL
    ';
    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;
    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;


    -- Three transformers each with a different kVA (e.g., 25/10/5)
    v_stmt := '
        update '||p_schema||'.'||v_targetTable||' target
        set target.LOAD_UNDETERMINED = ''4''
        where (target.ID) in (
          SELECT DISTINCT hist.ID
          FROM '||p_schema||'.'||v_targetTable||' hist
          INNER JOIN
          (
            SELECT y.ID, x.NP_KVA, y.BATCH_DATE, COUNT(x.NP_KVA) AS BankCount
            FROM '||p_schema||'.TRANSFORMER_BANK x
            INNER JOIN
            (
              SELECT ph.ID , ph.TRF_ID, ph.BATCH_DATE, COUNT(trb.ID)
              FROM '||p_schema||'.'||v_targetTable||' ph
              INNER JOIN '||p_schema||'.TRANSFORMER tr
              ON ph.TRF_ID = tr.ID
              INNER JOIN '||p_schema||'.TRANSFORMER_BANK trb
              ON ph.TRF_ID = trb.TRF_ID
              GROUP BY ph.ID , ph.TRF_ID, ph.BATCH_DATE
              HAVING COUNT(trb.ID) = 3
            ) y
            ON x.TRF_ID = y.TRF_ID
            GROUP BY y.ID, x.NP_KVA, y.BATCH_DATE
            HAVING count(NP_KVA) = 1
          ) NP
          ON hist.ID = NP.ID
          GROUP BY hist.ID
          HAVING COUNT(hist.ID) = 3)
          and target.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
          and target.LOAD_UNDETERMINED IS NULL
      ';

    DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
    execute immediate v_stmt;
    v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;


    -- Three transformers of which two have the same name plate kVA
    -- and their name plate kVA is greater than the third transformer (e.g., 50/50/10)
    v_stmt := '
        update '||p_schema||'.'||v_targetTable||' target
        set target.LOAD_UNDETERMINED = ''4''
        where (target.ID) in (
          SELECT DISTINCT hist.ID
          FROM '||p_schema||'.'||v_targetTable||' hist
          INNER JOIN
          (
            SELECT y.ID, x.NP_KVA, y.BATCH_DATE, COUNT(x.NP_KVA) AS BankCount
            FROM '||p_schema||'.TRANSFORMER_BANK x
            INNER JOIN
            (
              SELECT ph.ID, ph.TRF_ID, ph.BATCH_DATE, COUNT(trb.ID)
              FROM '||p_schema||'.'||v_targetTable||' ph
              INNER JOIN '||p_schema||'.TRANSFORMER tr
              ON ph.TRF_ID = tr.ID
              INNER JOIN '||p_schema||'.TRANSFORMER_BANK trb
              ON ph.TRF_ID = trb.TRF_ID
              GROUP BY ph.ID, ph.TRF_ID, ph.BATCH_DATE
              HAVING COUNT(trb.ID) = 3
            ) y
            ON x.TRF_ID = y.TRF_ID
            GROUP BY y.ID, x.NP_KVA, y.BATCH_DATE
            HAVING count(x.NP_KVA) = 1
          ) Lighter
          ON hist.ID = Lighter.ID
          INNER JOIN
          (
            SELECT y.ID, x.NP_KVA, y.BATCH_DATE, COUNT(x.NP_KVA) AS BankCount
            FROM '||p_schema||'.TRANSFORMER_BANK x
            INNER JOIN
            (
              SELECT ph.ID, ph.TRF_ID, ph.BATCH_DATE, COUNT(trb.ID)
              FROM '||p_schema||'.'||v_targetTable||' ph
              INNER JOIN '||p_schema||'.TRANSFORMER tr
              ON ph.TRF_ID = tr.ID
              INNER JOIN '||p_schema||'.TRANSFORMER_BANK trb
              ON ph.TRF_ID = trb.TRF_ID
              GROUP BY ph.ID, ph.TRF_ID, ph.BATCH_DATE
              HAVING COUNT(trb.ID) = 3
            ) y
            ON x.TRF_ID = y.TRF_ID
            GROUP BY y.ID, x.NP_KVA, y.BATCH_DATE
            HAVING count(x.NP_KVA) = 2
          ) Power
          ON Lighter.ID = Power.ID
          WHERE Lighter.NP_KVA < Power.NP_KVA)
          and target.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
          and target.LOAD_UNDETERMINED IS NULL
        ';

      DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
      execute immediate v_stmt;
      v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- gather stats
      v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => '''||v_targetTable||''',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update monthly load log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPDATE UNDETERMINED FLG_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'),v_totalRowsMigrated);

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
         v_log_status := LOG_MONTHLY_LOAD_ERROR('UPDATE UNDETERMINED FLG_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=UPDATE UNDETERMINED FLG_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));
END UPD_TRF_PEAK_HIST_UNDTRMND_FLG;
/
