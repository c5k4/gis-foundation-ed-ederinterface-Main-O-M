Prompt drop Index SM_PRIMARY_METER_HIST_PK;
DROP INDEX EDSETT.SM_PRIMARY_METER_HIST_PK
/

Prompt Index SM_PRIMARY_METER_HIST_PK;
--
-- SM_PRIMARY_METER_HIST_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.SM_PRIMARY_METER_HIST_PK ON EDSETT.SM_PRIMARY_METER_HIST (ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
