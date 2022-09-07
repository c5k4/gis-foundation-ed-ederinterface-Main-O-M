Prompt drop Index CALC_TRF_BANK_PEAK_HIST_PK;
DROP INDEX EDTLM.CALC_TRF_BANK_PEAK_HIST_PK
/

Prompt Index CALC_TRF_BANK_PEAK_HIST_PK;
--
-- CALC_TRF_BANK_PEAK_HIST_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.CALC_TRF_BANK_PEAK_HIST_PK ON EDTLM.CALC_TRF_BANK_PEAK_HIST (TRF_PEAK_HIST_ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
