--------------------------------------------------------
--  DDL for Procedure UPDATESAPEQUIPID_ED006TABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."UPDATESAPEQUIPID_ED006TABLE" AS
BEGIN
for cur in ( select PGE_SAPEQUIPID,PGE_GLOBALID  from PGEDATA.ED07_GISPLDB_INTERFACE)
loop
BEGIN
update PGEDATA.ED06_GISPLDB_INTERFACE set PGE_SAPEQUIPID= cur.PGE_SAPEQUIPID where PGE_GLOBALID = cur.PGE_GLOBALID and INTERFACE_STATUS in ('NEW','ERROR') and EDIT_Type='I';
end;
end loop;
delete from PGEDATA.ED07_GISPLDB_INTERFACE;
dbms_output.put_line('done');
commit;
END UPDATESAPEQUIPID_ED006TABLE;
