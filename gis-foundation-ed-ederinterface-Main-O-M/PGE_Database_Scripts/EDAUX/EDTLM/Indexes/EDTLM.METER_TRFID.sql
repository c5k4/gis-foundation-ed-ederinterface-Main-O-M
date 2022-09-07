Prompt drop Index METER_TRFID;
DROP INDEX EDTLM.METER_TRFID
/

Prompt Index METER_TRFID;
--
-- METER_TRFID  (Index) 
--
CREATE INDEX EDTLM.METER_TRFID ON EDTLM.METER (TRF_ID) NOLOGGING TABLESPACE EDTLM PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/