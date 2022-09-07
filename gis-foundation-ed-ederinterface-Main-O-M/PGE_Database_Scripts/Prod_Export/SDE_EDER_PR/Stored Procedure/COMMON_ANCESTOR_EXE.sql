--------------------------------------------------------
--  DDL for Procedure COMMON_ANCESTOR_EXE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "SDE"."COMMON_ANCESTOR_EXE" AS

 CURSOR ver_list IS
   SELECT name, owner, state_id
   FROM sde.versions
   WHERE name <> 'DEFAULT'
   AND name NOT LIKE 'SYNC_%'
   AND name NOT LIKE 'REP_CO_SYNC_%'
   ORDER BY state_id;

 default_id          NUMBER;
 default_lin         NUMBER;
 source_lin          NUMBER;
 common_ancestor_id  NUMBER;

BEGIN

 SELECT state_id, lineage_name INTO default_id, default_lin FROM sde.states WHERE state_id =
  (SELECT state_id FROM sde.versions WHERE name = 'DEFAULT' AND owner = 'SDE');

 DELETE FROM version_orders;

 FOR ver_info IN ver_list LOOP
  BEGIN
  SELECT lineage_name INTO source_lin FROM sde.states WHERE state_id = ver_info.state_id;

  SELECT MAX(lineage_id) INTO common_ancestor_id FROM
  (SELECT lineage_id FROM sde.state_lineages WHERE lineage_name = default_lin AND lineage_id <= default_id
   INTERSECT
   SELECT lineage_id FROM sde.state_lineages WHERE lineage_name = source_lin AND lineage_id <= ver_info.state_id);

  IF common_ancestor_id < default_id THEN
    INSERT INTO version_orders VALUES (ver_info.owner, ver_info.name, ver_info.state_id, common_ancestor_id);
  END IF;

  EXCEPTION
   WHEN NO_DATA_FOUND THEN
   CONTiNUE;
  END;
 END LOOP;

 COMMIT;

END;
