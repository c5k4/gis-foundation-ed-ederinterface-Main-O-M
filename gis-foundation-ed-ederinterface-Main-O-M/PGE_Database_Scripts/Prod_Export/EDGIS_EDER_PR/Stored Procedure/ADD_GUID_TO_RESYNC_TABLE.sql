--------------------------------------------------------
--  DDL for Procedure ADD_GUID_TO_RESYNC_TABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."ADD_GUID_TO_RESYNC_TABLE" (
   guid IN VARCHAR2, fc in varchar2)
IS
  rowcnt number;
  fc_final varchar2(64);
BEGIN
  select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
  if rowcnt<1 then
    dbms_output.put_line('INSERTING [ '||guid||']');
    if fc not like '%.%' then
      fc_final := 'EDGIS.'||fc;
    else
      fc_final := fc;
    end if;
    insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
    values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
    guid,fc_final,SYSDATE,'SYNC');
  end if;
END;
