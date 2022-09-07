Prompt drop Index CEDSA_CAPSETT_IDX;
DROP INDEX EDSETT.CEDSA_CAPSETT_IDX
/

Prompt Index CEDSA_CAPSETT_IDX;
--
-- CEDSA_CAPSETT_IDX  (Index) 
--
CREATE INDEX EDSETT.CEDSA_CAPSETT_IDX ON EDSETT.CEDSA_CAPACITOR_SETTINGS (DEVICE_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/