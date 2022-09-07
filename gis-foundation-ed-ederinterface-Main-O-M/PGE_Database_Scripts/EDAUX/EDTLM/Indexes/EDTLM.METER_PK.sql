Prompt drop Index METER_PK;
DROP INDEX EDTLM.METER_PK
/

Prompt Index METER_PK;
--
-- METER_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.METER_PK ON EDTLM.METER (ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/