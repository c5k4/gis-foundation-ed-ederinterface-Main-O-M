Prompt drop Index SM_RECLOSER_IDX6;
DROP INDEX EDSETT.SM_RECLOSER_IDX6
/

Prompt Index SM_RECLOSER_IDX6;
--
-- SM_RECLOSER_IDX6  (Index) 
--
CREATE INDEX EDSETT.SM_RECLOSER_IDX6 ON EDSETT.SM_RECLOSER (SOFTWARE_VERSION) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
