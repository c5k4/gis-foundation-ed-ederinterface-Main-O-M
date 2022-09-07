Prompt drop Index TRF_PEAK_PK;
DROP INDEX EDTLM.TRF_PEAK_PK
/

Prompt Index TRF_PEAK_PK;
--
-- TRF_PEAK_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.TRF_PEAK_PK ON EDTLM.TRF_PEAK (ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
