Prompt drop Index FEATURES_NOTI_HIST_IDX2;
DROP INDEX EDSETT.FEATURES_NOTI_HIST_IDX2
/

Prompt Index FEATURES_NOTI_HIST_IDX2;
--
-- FEATURES_NOTI_HIST_IDX2  (Index) 
--
CREATE INDEX EDSETT.FEATURES_NOTI_HIST_IDX2 ON EDSETT.FEATURES_NOTIFICATION_HISTORY (RECIPIENT) NOLOGGING TABLESPACE EDSETT PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )
/
