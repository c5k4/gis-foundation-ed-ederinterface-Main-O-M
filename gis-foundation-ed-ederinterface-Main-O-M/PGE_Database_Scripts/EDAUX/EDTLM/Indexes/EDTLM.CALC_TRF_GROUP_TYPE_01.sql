Prompt drop Index CALC_TRF_GROUP_TYPE_01;
DROP INDEX EDTLM.CALC_TRF_GROUP_TYPE_01
/

Prompt Index CALC_TRF_GROUP_TYPE_01;
--
-- CALC_TRF_GROUP_TYPE_01  (Index) 
--
CREATE INDEX EDTLM.CALC_TRF_GROUP_TYPE_01 ON EDTLM.CALC_TRF_GROUP_TYPE (TRF_BANK_ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
