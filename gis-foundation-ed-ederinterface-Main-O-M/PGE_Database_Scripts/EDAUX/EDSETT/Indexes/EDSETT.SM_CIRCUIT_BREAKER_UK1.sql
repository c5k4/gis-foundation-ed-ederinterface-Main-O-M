Prompt drop Index SM_CIRCUIT_BREAKER_UK1;
DROP INDEX EDSETT.SM_CIRCUIT_BREAKER_UK1
/

Prompt Index SM_CIRCUIT_BREAKER_UK1;
--
-- SM_CIRCUIT_BREAKER_UK1  (Index) 
--
CREATE UNIQUE INDEX EDSETT.SM_CIRCUIT_BREAKER_UK1 ON EDSETT.SM_CIRCUIT_BREAKER (GLOBAL_ID, CURRENT_FUTURE) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
