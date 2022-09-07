set heading off
set pages 0
set lines 200
set feedback off
set trimspool on
set serverout on
set tab off
select a.count1 + b.count2 from 
(select count(*) count1 from pgedata.GEN_EQUIPMENT_STAGE) a ,
(select count(*) count2 from pgedata.GEN_SUMMARY_STAGE) b
;
exit;
