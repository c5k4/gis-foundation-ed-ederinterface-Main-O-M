Prompt drop Index GIS_IDX3;
DROP INDEX EDSETT.GIS_IDX3
/

Prompt Index GIS_IDX3;
--
-- GIS_IDX3  (Index) 
--
CREATE INDEX EDSETT.GIS_IDX3 ON EDSETT.GIS_CEDSADEVICEID (GLOBAL_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
