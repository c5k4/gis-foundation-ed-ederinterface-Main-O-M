Prompt drop Index REV_ACCT_PK;
DROP INDEX EDTLM.REV_ACCT_PK
/

Prompt Index REV_ACCT_PK;
--
-- REV_ACCT_PK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.REV_ACCT_PK ON EDTLM.REV_ACCT (REV_ACCT_CD) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
