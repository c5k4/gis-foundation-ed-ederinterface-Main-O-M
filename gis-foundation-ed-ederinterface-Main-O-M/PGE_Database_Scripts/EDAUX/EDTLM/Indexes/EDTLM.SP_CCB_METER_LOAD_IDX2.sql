Prompt drop Index SP_CCB_METER_LOAD_IDX2;
DROP INDEX EDTLM.SP_CCB_METER_LOAD_IDX2
/

Prompt Index SP_CCB_METER_LOAD_IDX2;
--
-- SP_CCB_METER_LOAD_IDX2  (Index) 
--
CREATE INDEX EDTLM.SP_CCB_METER_LOAD_IDX2 ON EDTLM.SP_CCB_METER_LOAD (TRF_CCB_METER_LOAD_ID) NOLOGGING TABLESPACE EDTLM PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
