Prompt drop Index CEDSA_SCADA_IDX;
DROP INDEX EDSETT.CEDSA_SCADA_IDX
/

Prompt Index CEDSA_SCADA_IDX;
--
-- CEDSA_SCADA_IDX  (Index) 
--
CREATE INDEX EDSETT.CEDSA_SCADA_IDX ON EDSETT.CEDSA_SCADA (DEVICE_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
