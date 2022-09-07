Prompt drop Index SP_CCB_METER_LOAD_PK;
DROP INDEX EDTLM.SP_CCB_METER_LOAD_PK
/

Prompt Index SP_CCB_METER_LOAD_PK;
--
-- SP_CCB_METER_LOAD_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.SP_CCB_METER_LOAD_PK ON EDTLM.SP_CCB_METER_LOAD (ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/