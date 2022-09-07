Prompt drop Index TRF_BANK_PEAK_TBID;
DROP INDEX EDTLM.TRF_BANK_PEAK_TBID
/

Prompt Index TRF_BANK_PEAK_TBID;
--
-- TRF_BANK_PEAK_TBID  (Index) 
--
CREATE INDEX EDTLM.TRF_BANK_PEAK_TBID ON EDTLM.TRF_BANK_PEAK (TRF_BANK_ID) NOLOGGING TABLESPACE EDTLM PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/