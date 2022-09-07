Prompt drop Procedure RUN_MONTHLY_LOAD_PARTIAL;
DROP PROCEDURE EDTLM.RUN_MONTHLY_LOAD_PARTIAL
/

Prompt Procedure RUN_MONTHLY_LOAD_PARTIAL;
--
-- RUN_MONTHLY_LOAD_PARTIAL  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDTLM."RUN_MONTHLY_LOAD_PARTIAL" (
        p_schema VARCHAR2,
        batchDate DATE)
    AS
      -- This procedure accepts three arguments:
      -- 1. p_schema - where the source/target schema
      -- 2. batchDate - the processing month - 2 months prior to current month since CCB data is 2 months behind
      v_log_status VARCHAR2(10);
      v_cnt INTEGER;
      v_date DATE := batchDate;
    BEGIN
/*
      -- Always assume the process is restarted.  Do some cleanup first
      delete from SP_PEAK_GEN_HIST where TRF_PEAK_GEN_HIST_ID in (select id from trf_peak_gen_hist where batch_date = v_date);
      delete from TRF_BANK_PEAK_GEN_HIST where TRF_PEAK_GEN_HIST_ID in (select id from trf_peak_gen_hist where batch_date = v_date);
      delete from  trf_peak_gen_hist where batch_date = v_date;
      delete from SP_PEAK_HIST where TRF_PEAK_HIST_ID in (select id from trf_peak_hist where batch_date = v_date);
      delete from TRF_BANK_PEAK_HIST where TRF_PEAK_HIST_ID in (select id from trf_peak_hist where batch_date = v_date);
      delete from  trf_peak_hist where batch_date = v_date;
      delete from MONTHLY_LOAD_LOG where to_char(LOAD_START_TS,'mm') =  to_char(SYSDATE,'mm');

      IF LOAD_TRF_PEAK_HIST_SMART(p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - SMART successfully loaded');
      END IF;
      IF LOAD_TRF_PEAK_HIST_LEGACY(p_schema, v_date) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - SMART successfully loaded');
      END IF;

      IF UPD_TRF_PEAK_HIST_UNDTRMND_FLG (p_schema, v_date, 'LOAD')  = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - UNDETERMINABLE FLAG successfully updated');
      END IF;

      IF UPD_TRF_PK_HIST_UND_TRF_SP_REL (p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST - Transformer/Service Point Relationship Error '||v_date||' Records successfully migrated');
      END IF;

      IF LOAD_TRF_BANK_PEAK_HIST(p_schema, v_date, 'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_BANKPEAK_HIST - SMART successfully loaded');
      END IF;

      IF LOAD_SP_PEAK_HIST_SMART(p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table SP_PEAK_HIST - SMART successfully loaded');
      END IF;
      IF LOAD_SP_PEAK_HIST_LEGACY(p_schema, v_date) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table SP_PEAK_HIST - LEGACY successfully loaded');
      END IF;

      -- update fields (except CAP) in TRF_PEAK_HIST
      IF UPDATE_TRF_PEAK_HIST (p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_HIST successfully updated');
      END IF;

      -- update fields (except CAP) in TRF_BANK_PEAK_HIST
       IF UPDATE_TRF_BANK_PEAK_HIST(p_schema, v_date,'LOAD') = 'TRUE' THEN
           DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK_HIST successfully updated');
      END IF;

      -- update CAP in TRF_PEAK_HIST
      IF UPDATE_TRF_BANK_PEAK_HIST_CAP(p_schema, v_date,'LOAD') = 'TRUE' THEN
           DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK_HIST (Capacity) successfully updated');
      END IF;

      -- Populate 12 month peak summary table
      IF POPULATE_TRF_PEAK_TABLE (p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK successfully populated');
      END IF;

      IF POPULATE_TRF_BANK_PEAK_TABLE (p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK successfully populated');
      END IF;

      IF POPULATE_TRF_PEAK_BY_CUST_TYP (p_schema, v_date,'LOAD') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_BY_CUST_TYP successfully populated');
      END IF;

      -- Generated load processing
      IF LOAD_TRF_PEAK_HIST_SMART(p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_HIST - SMART successfully loaded');
      END IF;
      IF LOAD_SP_PEAK_HIST_SMART(p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table SP_PEAK_GEN_HIST - SMART successfully loaded');
      END IF;

       IF UPD_TRF_PEAK_HIST_UNDTRMND_FLG (p_schema, v_date, 'GEN')  = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_HIST - UNDETERMINABLE FLAG successfully updated');
      END IF;

      IF UPD_TRF_PK_HIST_UND_TRF_SP_REL (p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_HIST - Transformer/Service Point Relationship Error '||v_date||' Records successfully migrated');
      END IF;

      IF LOAD_TRF_BANK_PEAK_HIST(p_schema, v_date, 'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_HIST - SMART successfully loaded');
      END IF;

      -- update fields (except CAP) in TRF_PEAK_HIST
      IF UPDATE_TRF_PEAK_HIST (p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_HIST successfully updated');
      END IF;

      -- update fields (except CAP) in TRF_BANK_PEAK_HIST
      IF UPDATE_TRF_BANK_PEAK_HIST(p_schema, v_date,'GEN') = 'TRUE' THEN
           DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK_GEN_HIST successfully updated');
      END IF;

      -- update CAP in TRF_PEAK_HIST
      IF UPDATE_TRF_BANK_PEAK_HIST_CAP(p_schema, v_date,'GEN') = 'TRUE' THEN
           DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK_GEN_HIST (Capacity) successfully updated');
      END IF;
      -- Populate 12 month peak summary table
      IF POPULATE_TRF_PEAK_TABLE (p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN successfully populated');
      END IF;
*/
      IF POPULATE_TRF_BANK_PEAK_TABLE (p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_BANK_PEAK_GEN successfully populated');
      END IF;

      IF POPULATE_TRF_PEAK_BY_CUST_TYP (p_schema, v_date,'GEN') = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_PEAK_GEN_BY_CUST_TYP successfully populated');
      END IF;

    END RUN_MONTHLY_LOAD_PARTIAL;
/
