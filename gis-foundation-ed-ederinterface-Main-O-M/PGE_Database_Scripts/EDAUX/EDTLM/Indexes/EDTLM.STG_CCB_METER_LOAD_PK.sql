Prompt drop Index STG_CCB_METER_LOAD_PK;
DROP INDEX EDTLM.STG_CCB_METER_LOAD_PK
/

Prompt Index STG_CCB_METER_LOAD_PK;
--
-- STG_CCB_METER_LOAD_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.STG_CCB_METER_LOAD_PK ON EDTLM.STG_CCB_METER_LOAD (SERVICE_POINT_ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
