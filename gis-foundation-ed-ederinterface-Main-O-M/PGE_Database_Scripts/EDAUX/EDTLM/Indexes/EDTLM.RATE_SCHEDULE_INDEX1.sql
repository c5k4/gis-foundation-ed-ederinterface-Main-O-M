Prompt drop Index RATE_SCHEDULE_INDEX1;
DROP INDEX EDTLM.RATE_SCHEDULE_INDEX1
/

Prompt Index RATE_SCHEDULE_INDEX1;
--
-- RATE_SCHEDULE_INDEX1  (Index) 
--
CREATE UNIQUE INDEX EDTLM.RATE_SCHEDULE_INDEX1 ON EDTLM.RATE_SCHEDULE (RATE_SCHEDULE, CUST_TYP) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
