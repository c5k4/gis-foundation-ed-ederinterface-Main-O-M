Prompt drop Index GIS_CEDSADEVICEID_CB_PK;
DROP INDEX EDSETT.GIS_CEDSADEVICEID_CB_PK
/

Prompt Index GIS_CEDSADEVICEID_CB_PK;
--
-- GIS_CEDSADEVICEID_CB_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.GIS_CEDSADEVICEID_CB_PK ON EDSETT.GIS_CEDSADEVICEID_CB (GLOBAL_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
