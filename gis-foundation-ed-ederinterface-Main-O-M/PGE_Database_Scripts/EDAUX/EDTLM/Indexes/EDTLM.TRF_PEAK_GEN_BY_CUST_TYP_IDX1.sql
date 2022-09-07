Prompt drop Index TRF_PEAK_GEN_BY_CUST_TYP_IDX1;
DROP INDEX EDTLM.TRF_PEAK_GEN_BY_CUST_TYP_IDX1
/

Prompt Index TRF_PEAK_GEN_BY_CUST_TYP_IDX1;
--
-- TRF_PEAK_GEN_BY_CUST_TYP_IDX1  (Index) 
--
CREATE UNIQUE INDEX EDTLM.TRF_PEAK_GEN_BY_CUST_TYP_IDX1 ON EDTLM.TRF_PEAK_GEN_BY_CUST_TYP (TRF_PEAK_GEN_ID, SEASON, CUST_TYP) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/