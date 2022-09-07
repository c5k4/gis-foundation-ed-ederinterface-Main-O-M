spool D:\Temp\PB\DBChanges_pgedata.txt

create table PGEDATA_SM_SCADA_EAD_BKP as select * from PGEDATA.PGEDATA_SM_SCADA_EAD;

delete from PGEDATA.PGEDATA_SM_SCADA_EAD;
commit;

spool off