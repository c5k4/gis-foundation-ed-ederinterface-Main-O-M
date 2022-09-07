Prompt drop Trigger PIN_SEQUENCES;
DROP TRIGGER EDGIS.PIN_SEQUENCES
/

Prompt Trigger PIN_SEQUENCES;
--
-- PIN_SEQUENCES  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.PIN_SEQUENCES AFTER STARTUP ON DATABASE
DECLARE
OWN VARCHAR2(100);
NAM VARCHAR2(100);
ITEM VARCHAR2(100);

CURSOR SEQS IS
select SEQUENCE_OWNER,
SEQUENCE_NAME from all_sequences
where SEQUENCE_OWNER in ('EDGIS');

BEGIN
  open SEQS;
  loop
	fetch SEQS into own, nam;
	exit when SEQS%notfound;
	ITEM := '' || own || '.' || nam || '';
	SYS.dbms_shared_pool.keep(ITEM, 'Q');
  end loop;
end;
/
