Prompt drop Index CEDSA_CAPSETT_IDX2;
DROP INDEX EDSETT.CEDSA_CAPSETT_IDX2
/

Prompt Index CEDSA_CAPSETT_IDX2;
--
-- CEDSA_CAPSETT_IDX2  (Index) 
--
CREATE INDEX EDSETT.CEDSA_CAPSETT_IDX2 ON EDSETT.CEDSA_CAPACITOR_SETTINGS (CURRENT_FUTURE) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
