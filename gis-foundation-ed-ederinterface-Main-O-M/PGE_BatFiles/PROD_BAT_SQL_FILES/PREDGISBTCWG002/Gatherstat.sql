exec DBMS_STATS.GATHER_TABLE_STATS('SDE','state_lineages',estimate_percent=>100,DEGREE=>8,CASCADE=>TRUE,No_Invalidate=>false);
EXIT;