Prompt drop Index SM_RECLOSER_TS_IDX5;
DROP INDEX EDSETT.SM_RECLOSER_TS_IDX5
/

Prompt Index SM_RECLOSER_TS_IDX5;
--
-- SM_RECLOSER_TS_IDX5  (Index) 
--
CREATE INDEX EDSETT.SM_RECLOSER_TS_IDX5 ON EDSETT.SM_RECLOSER_TS (DISTRICT) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
