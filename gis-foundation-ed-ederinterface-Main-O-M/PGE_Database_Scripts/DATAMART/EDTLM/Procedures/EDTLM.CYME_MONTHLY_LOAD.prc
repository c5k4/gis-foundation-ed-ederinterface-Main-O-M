Prompt drop Procedure CYME_MONTHLY_LOAD;
DROP PROCEDURE EDTLM.CYME_MONTHLY_LOAD
/

Prompt Procedure CYME_MONTHLY_LOAD;
--
-- CYME_MONTHLY_LOAD  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDTLM."CYME_MONTHLY_LOAD" (
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

      -- Always assume the process is restarted.  Do some cleanup first

      delete from SP_CCB_METER_LOAD where TRF_CCB_METER_LOAD_ID in ( select id from TRF_CCB_METER_LOAD where batch_date = v_date);
      delete from TRF_CCB_METER_LOAD where batch_date = v_date;
      delete from MONTHLY_LOAD_LOG where table_name='TRF_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy');
      delete from MONTHLY_LOAD_LOG where table_name='SP_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy');

      IF LOAD_TRF_CCB_METER(p_schema, v_date) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table TRF_CCB_METER_LOAD successfully loaded');
      END IF;

      IF LOAD_SP_CCB_METER(p_schema, v_date) = 'TRUE' THEN
        DBMS_OUTPUT.PUT_LINE('Table SP_CCB_METER_LOAD successfully loaded');
      END IF;


END CYME_MONTHLY_LOAD;
/
