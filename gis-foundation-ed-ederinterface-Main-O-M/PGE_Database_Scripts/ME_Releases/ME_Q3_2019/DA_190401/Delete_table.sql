spool "D:\DA_190401\Logs\TABLE_DELETE.txt"
SET DEFINE OFF;

drop table if EXIT etgis.domain_lookup;
drop table if EXIT etgis.ME_SAP_DATA_LOG;
drop table if EXIT etgis.Me_Insulator_Material_Dm_Up; 

spool off;

 
