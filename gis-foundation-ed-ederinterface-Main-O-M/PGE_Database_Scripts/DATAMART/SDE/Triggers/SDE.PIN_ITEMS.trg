Prompt drop Trigger PIN_ITEMS;
DROP TRIGGER SDE.PIN_ITEMS
/

Prompt Trigger PIN_ITEMS;
--
-- PIN_ITEMS  (Trigger) 
--
CREATE OR REPLACE TRIGGER SDE.PIN_ITEMS AFTER STARTUP ON DATABASE
DECLARE
OWN VARCHAR2(100);
NAM VARCHAR2(100);
ITEM VARCHAR2(100);

CURSOR PKGS IS
SELECT OWNER, OBJECT_NAME FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'PACKAGE'
AND OBJECT_NAME IN (
'STANDARD',
'DBMS_DESCRIBE',
'DBMS_OUTPUT',
'DBMS_PIPE',
'DBMS_STANDARD',
'DBMS_UTILITY',
'DIUTIL',
'LOCK_UTIL',
'VERSION_UTIL',
'SDE_UTIL',
'LAYERS_UTIL',
'XML_UTIL',
'VERSION_USER_DDL',
'LOGFILE_UTIL',
'LOCK_UTIL',
'LOCATOR_UTIL',
'KEYSET_UTIL',
'SREF_UTIL',
'SPX_UTIL',
'METADATA_UTIL',
'REGISTRY_UTIL',
'PINFO_UTIL');


CURSOR SEQS IS
select SEQUENCE_OWNER,
SEQUENCE_NAME from all_sequences
where SEQUENCE_OWNER in ('SDE');

BEGIN
  open pkgs;
  loop
	fetch pkgs into own, nam;
	exit when pkgs%notfound;
	ITEM := '' || own || '.' || nam || '';
	SYS.dbms_shared_pool.keep(ITEM, 'P');
  end loop;
  open SEQS;
  loop
	fetch SEQS into own, nam;
	exit when SEQS%notfound;
	ITEM := '' || own || '.' || nam || '';
	SYS.dbms_shared_pool.keep(ITEM, 'Q');
  end loop;
end;
/
