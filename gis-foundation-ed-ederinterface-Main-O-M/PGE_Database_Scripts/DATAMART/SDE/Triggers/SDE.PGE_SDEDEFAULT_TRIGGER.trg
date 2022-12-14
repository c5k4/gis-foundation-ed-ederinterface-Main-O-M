Prompt drop Trigger PGE_SDEDEFAULT_TRIGGER;
DROP TRIGGER SDE.PGE_SDEDEFAULT_TRIGGER
/

Prompt Trigger PGE_SDEDEFAULT_TRIGGER;
--
-- PGE_SDEDEFAULT_TRIGGER  (Trigger) 
--
CREATE OR REPLACE TRIGGER SDE.PGE_SDEDEFAULT_TRIGGER
AFTER UPDATE
 ON SDE.VERSIONS
 FOR EACH ROW
WHEN (
'DEFAULT'=NEW.NAME and 'SDE'=NEW.OWNER and NEW.STATE_ID<>OLD.STATE_ID
      )
DECLARE
   PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
 -- Insert the new row from VERSIONS table into table for permanent history
 INSERT INTO SDE.PGE_DEFAULT_SAVED_HIST
 ( VERSION_NAME,
VERSION_OWNER,
VERSION_ID,
VERSION_STATUS,
VERSION_STATE_ID,
VERSION_DESCRIPTION,
VERSION_PARENT_NAME,
VERSION_PARENT_OWNER,
VERSION_PARENT_VERSION_ID,
VERSION_CREATION_TIME,
STATE_ID,
STATE_OWNER,
STATE_CREATION_TIME,
STATE_CLOSING_TIME,
STATE_PARENT_STATE_ID,
STATE_LINEAGE_NAME)
SELECT
:NEW.NAME,
:NEW.OWNER,
:NEW.VERSION_ID,
:NEW.STATUS,
:NEW.STATE_ID,
:NEW.DESCRIPTION,
:NEW.PARENT_NAME,
:NEW.PARENT_OWNER,
:NEW.PARENT_VERSION_ID,
:NEW.CREATION_TIME,
ST.STATE_ID,
ST.OWNER,
ST.CREATION_TIME,
ST.CLOSING_TIME,
ST.PARENT_STATE_ID,
ST.LINEAGE_NAME
from
SDE.STATES ST
WHERE
ST.STATE_ID=:NEW.STATE_ID;
commit;
END;
/
