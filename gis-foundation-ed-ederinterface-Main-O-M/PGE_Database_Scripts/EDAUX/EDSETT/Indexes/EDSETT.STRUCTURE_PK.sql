Prompt drop Index STRUCTURE_PK;
DROP INDEX EDSETT.STRUCTURE_PK
/

Prompt Index STRUCTURE_PK;
--
-- STRUCTURE_PK  (Index) 
--
CREATE UNIQUE INDEX EDSETT.STRUCTURE_PK ON EDSETT.CEDSA_STRUCTURE (STRUC_ID) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 256K NEXT 256K MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/