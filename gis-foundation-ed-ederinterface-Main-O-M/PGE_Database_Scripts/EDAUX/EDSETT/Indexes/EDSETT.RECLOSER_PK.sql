Prompt drop Index RECLOSER_PK;
DROP INDEX EDSETT.RECLOSER_PK
/

Prompt Index RECLOSER_PK;
--
-- RECLOSER_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.RECLOSER_PK ON EDSETT.CEDSA_RECLOSER (DEVICE_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/