Prompt drop Index SM_RECLOSER_IDX4;
DROP INDEX EDSETT.SM_RECLOSER_IDX4
/

Prompt Index SM_RECLOSER_IDX4;
--
-- SM_RECLOSER_IDX4  (Index) 
--
CREATE INDEX EDSETT.SM_RECLOSER_IDX4 ON EDSETT.SM_RECLOSER (DIVISION) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
