Prompt drop Index SP_PEAK_GEN_HIST_UK1;
DROP INDEX EDTLM.SP_PEAK_GEN_HIST_UK1
/

Prompt Index SP_PEAK_GEN_HIST_UK1;
--
-- SP_PEAK_GEN_HIST_UK1  (Index) 
--
CREATE UNIQUE INDEX EDTLM.SP_PEAK_GEN_HIST_UK1 ON EDTLM.SP_PEAK_GEN_HIST (SERVICE_POINT_ID, TRF_PEAK_GEN_HIST_ID) NOLOGGING TABLESPACE EDTLMIDX PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
