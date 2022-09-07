spool C:\temp\SSTRUCT_UPDATE8.txt
set serveroutput on

select current_timestamp from dual;
Prompt Updating Support Structure OBJECTID starting from 8

Declare 
version_name NVARCHAR2(97) := 'SSTRUCT_UPDATE8'; 
Begin 
sde.version_user_ddl.create_version('SDE.DEFAULT',version_name,sde.version_util.C_take_name_as_given,sde.version_util.C_version_public,'SSTRUCT_UPDATE');
End; 
/ 


call sde.version_util.set_current_version ('GIS_I.SSTRUCT_UPDATE8');
call sde.version_user_ddl.edit_version ('GIS_I.SSTRUCT_UPDATE8', 1);

update EDGIS.ZZ_MV_SUPPORTSTRUCTURE set JPSEQUENCE_1 = JPSEQUENCE, SOURCEACCURACY=NULL WHERE objectid like ('%8');


call sde.version_user_ddl.edit_version ('GIS_I.SSTRUCT_UPDATE8', 2);
COMMIT;
call sde.version_util.set_current_version ('SDE.DEFAULT');
COMMIT;
select current_timestamp from dual;

spool off