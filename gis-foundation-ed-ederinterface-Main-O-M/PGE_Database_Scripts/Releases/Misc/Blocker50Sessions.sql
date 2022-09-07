DROP table version_orders purge;

CREATE TABLE version_orders (
 owner        VARCHAR2(32), 
 name         VARCHAR2(64), 
 state_id     NUMBER(38), 
 ca_state_id  NUMBER(38))
 ;
 
CREATE OR REPLACE PROCEDURE common_ancestor_exe AS

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
/

EXEC sde.common_ancestor_exe;
column tm new_value file_time noprint
select to_char(sysdate, 'MMDDYYYY') tm from dual;
prompt &file_time
set termout off
set pages 999
SET MARKUP HTML ON
set term off
set feed off
spool EDGIS50BlockingSessions_&file_time..html;

select 
SessionID, Current_Owner, Session_Name, Created 
from (
 select 
 substr(v.name, 4) as SessionID, 
 p.current_owner as CURRENT_OWNER, 
 trim(p.session_name) as SESSION_NAME,
 p.create_date as Created  
 from 
 process.mm_session p, 
 sde.version_orders v
 where 
 to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id
 order by v.ca_state_id asc, v.state_id asc) where rownum < 51;

Spool off;
EXIT;
