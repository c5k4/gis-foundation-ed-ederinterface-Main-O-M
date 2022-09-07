--------------------------------------------------------
--  DDL for Procedure PROCESS_RESYNC_GUID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."PROCESS_RESYNC_GUID" (
   guid IN VARCHAR2, fc VARCHAR2)
IS
    rowcnt number;
BEGIN
  dbms_output.put_line('FC [ '||fc||' ]');
  select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
  if rowcnt<1 then
    dbms_output.put_line('INSERTING [ '||guid||']');
    insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
    values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
    guid,fc,SYSDATE,'SYNC');
  end if;
END;
