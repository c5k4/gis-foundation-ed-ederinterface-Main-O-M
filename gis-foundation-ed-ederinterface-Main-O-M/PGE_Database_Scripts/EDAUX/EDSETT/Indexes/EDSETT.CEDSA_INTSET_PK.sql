Prompt drop Index CEDSA_INTSET_PK;
DROP INDEX EDSETT.CEDSA_INTSET_PK
/

Prompt Index CEDSA_INTSET_PK;
--
-- CEDSA_INTSET_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.CEDSA_INTSET_PK ON EDSETT.CEDSA_INTERRUPTER_SETTINGS (DEVICE_ID, CURRENT_FUTURE) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 256K NEXT 256K MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
