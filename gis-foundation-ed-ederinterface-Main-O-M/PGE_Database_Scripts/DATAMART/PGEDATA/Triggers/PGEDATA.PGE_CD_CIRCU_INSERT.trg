Prompt drop Trigger PGE_CD_CIRCU_INSERT;
DROP TRIGGER PGEDATA.PGE_CD_CIRCU_INSERT
/

Prompt Trigger PGE_CD_CIRCU_INSERT;
--
-- PGE_CD_CIRCU_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_CIRCU_INSERT AFTER INSERT on PGEDATA.PGEDATA_SM_CIRCUIT_BREAKER_EAD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_CIRCUIT_BREAKER_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	dbms_output.put_line(  'Insert Detected so marking for all interfaces'  )  ;
	FOR  cdtype  in  watch_type_list  LOOP
	    dbms_output.put_line(  'Insert Marking for Interface '||cdtype.watch_type  )  ;
        CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
     END LOOP;
END;
/
