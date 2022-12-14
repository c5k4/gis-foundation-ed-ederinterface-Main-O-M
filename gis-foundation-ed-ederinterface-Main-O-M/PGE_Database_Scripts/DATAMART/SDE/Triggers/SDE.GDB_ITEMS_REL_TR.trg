Prompt drop Trigger GDB_ITEMS_REL_TR;
DROP TRIGGER SDE.GDB_ITEMS_REL_TR
/

Prompt Trigger GDB_ITEMS_REL_TR;
--
-- GDB_ITEMS_REL_TR  (Trigger) 
--
CREATE OR REPLACE TRIGGER SDE.GDB_ITEMS_REL_TR AFTER INSERT OR UPDATE OR DELETE ON SDE.GDB_ITEMRELATIONSHIPS FOR EACH ROW
DECLARE passed_table_name  VARCHAR2 (32) := 'GDB_ITEMRELATIONSHIPS';v_update BOOLEAN := FALSE; BEGIN IF (INSERTING OR UPDATING) AND :new.TYPE NOT IN ('{8DB31AF1-DF7C-4632-AA10-3CC44B0C6914}','{CC28387C-441F-4D7C-A802-41A160317FE0}','{79CC71C8-B7D9-4141-9014-B6373E236ABB}','{D022DE33-45BD-424C-88BF-5B1B6B957BD3}') THEN v_update := TRUE; ELSIF DELETING AND :old.TYPE NOT IN ('{8DB31AF1-DF7C-4632-AA10-3CC44B0C6914}','{CC28387C-441F-4D7C-A802-41A160317FE0}','{79CC71C8-B7D9-4141-9014-B6373E236ABB}','{D022DE33-45BD-424C-88BF-5B1B6B957BD3}')THEN v_update := TRUE; END IF; IF v_update = TRUE THEN UPDATE SDE.GDB_TABLES_LAST_MODIFIED SET last_modified_count = last_modified_count + 1 WHERE table_name = passed_table_name; IF SQL%NOTFOUND THEN INSERT INTO SDE.GDB_TABLES_LAST_MODIFIED VALUES (passed_table_name, 1); END IF; END IF; END;
/
