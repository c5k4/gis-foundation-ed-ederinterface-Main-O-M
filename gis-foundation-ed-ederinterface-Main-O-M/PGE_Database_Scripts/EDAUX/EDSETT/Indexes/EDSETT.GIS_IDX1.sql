Prompt drop Index GIS_IDX1;
DROP INDEX EDSETT.GIS_IDX1
/

Prompt Index GIS_IDX1;
--
-- GIS_IDX1  (Index) 
--
CREATE INDEX EDSETT.GIS_IDX1 ON EDSETT.GIS_CEDSADEVICEID (FEATURE_CLASS_NAME) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
