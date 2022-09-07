spool C:\temp\SSTRUCT_UPDATE0.txt
set serveroutput on

select current_timestamp from dual;
Prompt Updating Support Structure OBJECTID starting from 0

Declare 
version_name NVARCHAR2(97) := 'SSTRUCT_UPDATE0'; 
Begin 
sde.version_user_ddl.create_version('SDE.DEFAULT',version_name,sde.version_util.C_take_name_as_given,sde.version_util.C_version_public,'SSTRUCT_UPDATE');
End; 
/ 


call sde.version_util.set_current_version ('GIS_I.SSTRUCT_UPDATE0');
call sde.version_user_ddl.edit_version ('GIS_I.SSTRUCT_UPDATE0', 1);

update EDGIS.ZZ_MV_SUPPORTSTRUCTURE set JPSEQUENCE_1 = JPSEQUENCE, SOURCEACCURACY=NULL WHERE objectid like ('%0');


call sde.version_user_ddl.edit_version ('GIS_I.SSTRUCT_UPDATE0', 2);
COMMIT;
call sde.version_util.set_current_version ('SDE.DEFAULT');
COMMIT;
select current_timestamp from dual;

spool off