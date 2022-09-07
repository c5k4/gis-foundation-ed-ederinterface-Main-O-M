Prompt drop Index DEVICE_PK;
DROP INDEX EDSETT.DEVICE_PK
/

Prompt Index DEVICE_PK;
--
-- DEVICE_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.DEVICE_PK ON EDSETT.CEDSA_DEVICE_TEMP (DEVICE_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
