Prompt drop Index TRF_BANK_PEAK_GEN_HIST_PK;
DROP INDEX EDTLM.TRF_BANK_PEAK_GEN_HIST_PK
/

Prompt Index TRF_BANK_PEAK_GEN_HIST_PK;
--
-- TRF_BANK_PEAK_GEN_HIST_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.TRF_BANK_PEAK_GEN_HIST_PK ON EDTLM.TRF_BANK_PEAK_GEN_HIST (ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
