Prompt drop Index RTU_PK;
DROP INDEX EDSETT.RTU_PK
/

Prompt Index RTU_PK;
--
-- RTU_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.RTU_PK ON EDSETT.CEDSA_REMOTE_TERM_UNIT (DEVICE_ID) NOLOGGING TABLESPACE CEDSA_DATA PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 256K NEXT 256K MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
