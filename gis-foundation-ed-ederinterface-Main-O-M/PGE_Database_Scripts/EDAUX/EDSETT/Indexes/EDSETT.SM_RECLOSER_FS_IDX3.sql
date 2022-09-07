Prompt drop Index SM_RECLOSER_FS_IDX3;
DROP INDEX EDSETT.SM_RECLOSER_FS_IDX3
/

Prompt Index SM_RECLOSER_FS_IDX3;
--
-- SM_RECLOSER_FS_IDX3  (Index) 
--
CREATE INDEX EDSETT.SM_RECLOSER_FS_IDX3 ON EDSETT.SM_RECLOSER_FS (OPERATING_NUM) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/