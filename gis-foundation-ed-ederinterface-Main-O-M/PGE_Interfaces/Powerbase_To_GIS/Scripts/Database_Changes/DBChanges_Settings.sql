spool D:\Temp\PB\DBChanges_Settings.txt

------------------------- Taking back up for existing settings tables : START -------------------------------

--------------------------- Creating back up tables ----------------------------------

create table SM_CAPACITOR_pbkp as select * from SM_CAPACITOR;
create table SM_CAPACITOR_HIST_pbkp as select * from SM_CAPACITOR_HIST;

create table SM_INTERRUPTER_pbkp as select * from SM_INTERRUPTER;
create table SM_INTERRUPTER_HIST_pbkp as select * from SM_INTERRUPTER_HIST;

create table SM_RECLOSER_pbkp as select * from SM_RECLOSER;
create table SM_RECLOSER_HIST_pbkp as select * from SM_RECLOSER_HIST;

create table SM_RECLOSER_FS_pbkp as select * from SM_RECLOSER_FS;
create table SM_RECLOSER_FS_HIST_pbkp as select * from SM_RECLOSER_FS_HIST;

create table SM_RECLOSER_TS_pbkp as select * from SM_RECLOSER_TS;
create table SM_RECLOSER_TS_HIST_pbkp as select * from SM_RECLOSER_TS_HIST;

create table SM_SECTIONALIZER_pbkp as select * from SM_SECTIONALIZER;
create table SM_SECTIONALIZER_HIST_pbkp as select * from SM_SECTIONALIZER_HIST;

create table SM_SWITCH_pbkp as select * from SM_SWITCH;
create table SM_SWITCH_HIST_pbkp as select * from SM_SWITCH_HIST;

create table SM_SWITCH_MSO_pbkp as select * from SM_SWITCH_MSO;
create table SM_SWITCH_MSO_HIST_pbkp as select * from SM_SWITCH_MSO_HIST;

create table SM_REGULATOR_pbkp as select * from SM_REGULATOR;
create table SM_REGULATOR_HIST_pbkp as select * from SM_REGULATOR_HIST;

create table SM_PRIMARY_METER_pbkp as select * from SM_PRIMARY_METER;
create table SM_PRIMARY_METER_HIST_pbkp as select * from SM_PRIMARY_METER_HIST;

create table SM_CIRCUIT_BREAKER_pbkp as select * from SM_CIRCUIT_BREAKER;
create table SM_CIRCUIT_BREAKER_HIST_pbkp as select * from SM_CIRCUIT_BREAKER_HIST;

create table FEATURES_TO_NOTIFY_pbkp as select * from FEATURES_TO_NOTIFY;

------------------------- Taking back up for existing settings tables : END -----------------------------------

----------------- dropping tables --------------------------

drop table SM_CAPACITOR_HIST;
drop table SM_CAPACITOR;

drop table SM_INTERRUPTER_HIST;
drop table SM_INTERRUPTER;

drop table SM_RECLOSER_HIST;
drop table SM_RECLOSER;

drop table SM_RECLOSER_FS_HIST;
drop table SM_RECLOSER_FS;

drop table SM_RECLOSER_TS_HIST;
drop table SM_RECLOSER_TS;

drop table SM_SECTIONALIZER_HIST;
drop table SM_SECTIONALIZER;

drop table SM_SWITCH_HIST;
drop table SM_SWITCH;

drop table SM_SWITCH_MSO_HIST;
drop table SM_SWITCH_MSO;

drop table SM_REGULATOR_HIST;
drop table SM_REGULATOR;

drop table SM_PRIMARY_METER_HIST;
drop table SM_PRIMARY_METER;

drop table SM_CIRCUIT_BREAKER;
drop table SM_CIRCUIT_BREAKER_HIST;

----------------- dropping tables --------------------------


----------------- dropping views --------------------------

drop view SM_CAPACITOR_EAD_VW;
drop view SM_CAPACITOR_ERD_VW;
drop view SM_CIRCUIT_BREAKER_EAD_VW;
drop view SM_CIRCUIT_BREAKER_ERD_VW;
drop view SM_RECLOSER_EAD_VW;
drop view SM_RECLOSER_ERD_VW;
drop view SM_RECLOSER_TS_EAD_VW;
drop view SM_RECLOSER_TS_ERD_VW;
drop view SM_REGULATOR_EAD_VW;
drop view SM_REGULATOR_ERD_VW;
drop view SM_SCADA_EAD_VW;
drop view SM_SCADA_ERD_VW;
drop view SM_SECTIONALIZER_EAD_VW;
drop view SM_SECTIONALIZER_ERD_VW;
drop view SM_INTERRUPTER_EAD_VW;
drop view SM_INTERRUPTER_ERD_VW;
drop view SM_PRIMARY_METER_EAD_VW;
drop view SM_PRIMARY_METER_ERD_VW;
drop view SM_SWITCH_EAD_VW;
drop view SM_SWITCH_ERD_VW;
drop view SM_SWITCH_MSO_EAD_VW;
drop view SM_SWITCH_MSO_ERD_VW;

----------------- dropping views --------------------------

spool off

