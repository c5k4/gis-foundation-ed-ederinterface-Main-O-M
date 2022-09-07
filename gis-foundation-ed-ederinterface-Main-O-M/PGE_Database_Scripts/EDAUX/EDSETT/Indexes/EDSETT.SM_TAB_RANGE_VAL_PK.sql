Prompt drop Index SM_TAB_RANGE_VAL_PK;
DROP INDEX EDSETT.SM_TAB_RANGE_VAL_PK
/

Prompt Index SM_TAB_RANGE_VAL_PK;
--
-- SM_TAB_RANGE_VAL_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.SM_TAB_RANGE_VAL_PK ON EDSETT.SM_TABLE_RANGE_VALUE (ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
