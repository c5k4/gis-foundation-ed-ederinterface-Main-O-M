Prompt drop Procedure PROCESS_RESYNC_GUIDS_CSV;
DROP PROCEDURE EDGIS.PROCESS_RESYNC_GUIDS_CSV
/

Prompt Procedure PROCESS_RESYNC_GUIDS_CSV;
--
-- PROCESS_RESYNC_GUIDS_CSV  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.process_resync_guids_csv (
   guids_list IN VARCHAR2)
IS
  CURSOR guidscursor is
select regexp_substr(guids_list,'[^,]+', 1, level) as guid_val from dual
connect by regexp_substr(guids_list, '[^,]+', 1, level) is not null;
   fc VARCHAR2(200);
   fc2 VARCHAR2(200);
   guid varchar2(40);
   assetid varchar2(40);
    rowcnt number;
BEGIN
	FOR guid_row in guidscursor LOOP
    select guid_row.guid_val into guid from dual;
--    check to see if feature exists
    select get_fc_name(guid) into fc2 from dual;
    fc := fc2;
    dbms_output.put_line('FC [ '||fc||' ]');
    if fc is null then
      dbms_output.put_line('FC Not found for guid ['||guid_row.guid_val||' ]');
    else
      dbms_output.put_line('FC [ '||fc||' ]');
      select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
      if rowcnt<1 then
        dbms_output.put_line('INSERTING [ '||guid||']');
        insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
        values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
        guid,fc,SYSDATE,'SYNC');
      end if;
    end if;

   END LOOP;
END;
/


Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO GISINTERFACE
/

Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO GIS_I
/

Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO MM_ADMIN
/

Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO SDE_EDITOR
/

Prompt Grants on PROCEDURE PROCESS_RESYNC_GUIDS_CSV TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.PROCESS_RESYNC_GUIDS_CSV TO SDE_VIEWER
/
