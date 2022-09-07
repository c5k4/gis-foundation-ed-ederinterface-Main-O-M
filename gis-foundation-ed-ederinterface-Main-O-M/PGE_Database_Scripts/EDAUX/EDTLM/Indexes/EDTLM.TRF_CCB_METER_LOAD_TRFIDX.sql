Prompt drop Index TRF_CCB_METER_LOAD_TRFIDX;
DROP INDEX EDTLM.TRF_CCB_METER_LOAD_TRFIDX
/

Prompt Index TRF_CCB_METER_LOAD_TRFIDX;
--
-- TRF_CCB_METER_LOAD_TRFIDX  (Index) 
--
CREATE INDEX EDTLM.TRF_CCB_METER_LOAD_TRFIDX ON EDTLM.TRF_CCB_METER_LOAD (TRF_ID) NOLOGGING TABLESPACE EDTLM PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
