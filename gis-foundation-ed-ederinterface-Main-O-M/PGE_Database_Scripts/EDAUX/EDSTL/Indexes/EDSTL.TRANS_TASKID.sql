Prompt drop Index TRANS_TASKID;
DROP INDEX EDSTL.TRANS_TASKID
/

Prompt Index TRANS_TASKID;
--
-- TRANS_TASKID  (Index) 
--
CREATE INDEX EDSTL.TRANS_TASKID ON EDSTL.SL_TRANSACTION (TASKID) LOGGING TABLESPACE PGE PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/