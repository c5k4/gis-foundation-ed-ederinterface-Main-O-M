Prompt drop Index CEDSA_INT_PK;
DROP INDEX EDSETT.CEDSA_INT_PK
/

Prompt Index CEDSA_INT_PK;
--
-- CEDSA_INT_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.CEDSA_INT_PK ON EDSETT.CEDSA_INTERRUPTER (DEVICE_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 256K NEXT 256K MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/