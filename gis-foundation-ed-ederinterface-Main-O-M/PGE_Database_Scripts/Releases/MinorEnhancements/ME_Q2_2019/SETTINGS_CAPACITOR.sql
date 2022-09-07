set define off;
set escape on;
set serverout on;

VARIABLE MAXID NUMBER;

BEGIN
	SELECT MAX(ID) INTO :MAXID FROM sm_table_lookup;
END;
/

Insert into sm_table_lookup values(:MAXID+1,'SM_CAPACITOR','CONTROL_TYPE','BECK',2,'Beckwith','All');
Insert into sm_table_lookup values(:MAXID+2,'SM_CAPACITOR','CONTROL_TYPE','SC',8,'S&C','All');

update SM_TABLE_LOOKUP set SORT_NUM = 3 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='EN';
update SM_TABLE_LOOKUP set SORT_NUM = 4 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='FSPR';
update SM_TABLE_LOOKUP set SORT_NUM = 5 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='LNCT';
update SM_TABLE_LOOKUP set SORT_NUM = 6 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='MS';
update SM_TABLE_LOOKUP set SORT_NUM = 7 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='SNGM';
update SM_TABLE_LOOKUP set SORT_NUM = 9 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROL_TYPE' and CODE='UNSP';

Insert into sm_table_lookup values(:MAXID+3,'SM_CAPACITOR','CONTROLLER_UNIT_MODEL','7',2,'Beckwith M-6280A','All');
Insert into sm_table_lookup values(:MAXID+4,'SM_CAPACITOR','CONTROLLER_UNIT_MODEL','8',7,'S&C Intellicap 2000','All');

update SM_TABLE_LOOKUP set SORT_NUM = 3 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='4';
update SM_TABLE_LOOKUP set SORT_NUM = 4 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='2';
update SM_TABLE_LOOKUP set SORT_NUM = 5 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='5';
update SM_TABLE_LOOKUP set SORT_NUM = 6 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='1';
update SM_TABLE_LOOKUP set SORT_NUM = 8 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='3';
update SM_TABLE_LOOKUP set SORT_NUM = 9 where device_name='SM_CAPACITOR' and FIELD_NAME='CONTROLLER_UNIT_MODEL' and CODE='6';

commit;



declare
cursor c1 is select field_name,required from sm_capacitor_required where control_type='MS';
temp_id number;
begin
select max(id) into temp_id from sm_capacitor_required;
for rec in c1
loop
insert into sm_capacitor_required(id,field_name,required,control_type) values (temp_id+1,rec.field_name, rec.required,'BECK');
insert into sm_capacitor_required(id,field_name,required,control_type) values (temp_id+2,rec.field_name, rec.required,'SC');
temp_id:=temp_id+2;
end loop;
commit;
end;
/

declare
cursor c2 is select device_name,field_name,code,sort_num,description from 
sm_table_lookup where control_type='MS' and device_name='SM_CAPACITOR';
temp_id2 number;
begin
select max(id) into temp_id2 from sm_table_lookup;
for rec2 in c2
loop
insert into sm_table_lookup (id,device_name,field_name,code,sort_num,description,control_type) values 
(temp_id2+1,rec2.device_name,rec2.field_name,rec2.code,rec2.sort_num,rec2.description,'BECK');
insert into sm_table_lookup (id,device_name,field_name,code,sort_num,description,control_type) values 
(temp_id2+2,rec2.device_name,rec2.field_name,rec2.code,rec2.sort_num,rec2.description,'SC');
temp_id2:=temp_id2+2;
end loop;
commit;
end;
/

set define on;
set escape off;