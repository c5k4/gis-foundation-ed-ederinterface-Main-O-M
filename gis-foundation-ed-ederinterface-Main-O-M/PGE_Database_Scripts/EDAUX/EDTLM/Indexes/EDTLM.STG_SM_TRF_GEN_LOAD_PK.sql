Prompt drop Index STG_SM_TRF_GEN_LOAD_PK;
DROP INDEX EDTLM.STG_SM_TRF_GEN_LOAD_PK
/

Prompt Index STG_SM_TRF_GEN_LOAD_PK;
--
-- STG_SM_TRF_GEN_LOAD_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.STG_SM_TRF_GEN_LOAD_PK ON EDTLM.STG_SM_TRF_GEN_LOAD (CGC) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
