Prompt drop Index R137_SDE_ROWID_UK;
DROP INDEX EDTLM.R137_SDE_ROWID_UK
/

Prompt Index R137_SDE_ROWID_UK;
--
-- R137_SDE_ROWID_UK  (Index) 
--
CREATE UNIQUE INDEX EDTLM.R137_SDE_ROWID_UK ON EDTLM.EDGIS_PRIUGCONDUCTOR (OBJECTID) NOLOGGING TABLESPACE ARCFM_IX PCTFREE 0 INITRANS 4 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
