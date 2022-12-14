execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'PGE_SAPNOTIFICATION', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'PGEDATA', tabname => 'SAP_NOTIFICATIONHEADER', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'SUBSURFACESTRUCTURE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'SWITCH', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'STREETLIGHT', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'DEVICEGROUP', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'NETWORKPROTECTOR', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'PADMOUNTSTRUCTURE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'FAULTINDICATOR', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'OPENPOINT', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'CAPACITORBANK', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'SUPPORTSTRUCTURE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'SUBSURFACESTRUCTURE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'DYNAMICPROTECTIVEDEVICE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'VOLTAGEREGULATOR', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'VOLTAGEREGULATORUNIT', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'STEPDOWNUNIT', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'TRANSFORMERUNIT', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
execute dbms_stats.gather_table_stats(ownname => 'EDGIS', tabname => 'TRANSFORMERDEVICE', estimate_percent => DBMS_STATS.AUTO_SAMPLE_SIZE, cascade=>TRUE, method_opt=>'FOR ALL COLUMNS SIZE AUTO', No_Invalidate=>false);
exit;

