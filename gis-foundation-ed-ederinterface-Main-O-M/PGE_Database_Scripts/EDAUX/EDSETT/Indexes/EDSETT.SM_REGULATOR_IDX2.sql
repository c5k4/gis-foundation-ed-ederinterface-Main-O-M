Prompt drop Index SM_REGULATOR_IDX2;
DROP INDEX EDSETT.SM_REGULATOR_IDX2
/

Prompt Index SM_REGULATOR_IDX2;
--
-- SM_REGULATOR_IDX2  (Index) 
--
CREATE INDEX EDSETT.SM_REGULATOR_IDX2 ON EDSETT.SM_REGULATOR (SOFTWARE_VERSION) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
