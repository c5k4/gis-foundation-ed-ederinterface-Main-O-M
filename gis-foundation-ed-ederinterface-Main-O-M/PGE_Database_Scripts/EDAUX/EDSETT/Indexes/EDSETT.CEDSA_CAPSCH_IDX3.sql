Prompt drop Index CEDSA_CAPSCH_IDX3;
DROP INDEX EDSETT.CEDSA_CAPSCH_IDX3
/

Prompt Index CEDSA_CAPSCH_IDX3;
--
-- CEDSA_CAPSCH_IDX3  (Index) 
--
CREATE INDEX EDSETT.CEDSA_CAPSCH_IDX3 ON EDSETT.CEDSA_CAPACITOR_SCHEDULES (CURRENT_FUTURE) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
