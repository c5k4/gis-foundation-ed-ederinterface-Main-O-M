Prompt drop Index TRF_DUPLEX_LOOKUP_TOTAL;
DROP INDEX EDTLM.TRF_DUPLEX_LOOKUP_TOTAL
/

Prompt Index TRF_DUPLEX_LOOKUP_TOTAL;
--
-- TRF_DUPLEX_LOOKUP_TOTAL  (Index) 
--
CREATE UNIQUE INDEX EDTLM.TRF_DUPLEX_LOOKUP_TOTAL ON EDTLM.TRF_DUPLEX_LOOKUP (TOTAL) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/