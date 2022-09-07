Prompt drop Procedure CREATE_PRTCN_4_PRMY_MTR_SP;
DROP PROCEDURE EDSETT.CREATE_PRTCN_4_PRMY_MTR_SP
/

Prompt Procedure CREATE_PRTCN_4_PRMY_MTR_SP;
--
-- CREATE_PRTCN_4_PRMY_MTR_SP  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT.CREATE_PRTCN_4_PRMY_MTR_SP
IS

    /*
	TYPE T_PRTCT_ID is TABLE OF NUMBER;
    v_id T_PRTCT_ID;
    */

    TYPE t_bulk_collect IS TABLE OF EDSETT.SM_PRIMARY_METER%ROWTYPE;
		v_id    t_bulk_collect := t_bulk_collect();

	h_user varchar2(35) := null ;
	h_max_id_PROTECTION NUMBER(38) :=0 ;
	h_prtcn_typ nvarchar2(10) := null ;

	begin

		select user into h_user from dual;

		delete from EDSETT.sm_primary_meter where upper(current_future)='F';
		commit;


		select * BULK COLLECT into v_id from EDSETT.SM_PRIMARY_METER where upper(current_future) ='C' ;

		--select code into h_prtcn_typ from edsett.sm_table_lookup where device_name='SM_PROTECTION' and field_name='PROTECTION_TYPE' and upper(description)='UNSPECIFIED';
		h_prtcn_typ :='UNSP' ;

		SAVEPOINT err_found_2 ;

		begin
			--FORALL indx IN 1 .. v_id.COUNT
			 for indx in 1 .. v_id.COUNT loop
				select max(ID) into h_max_id_PROTECTION from EDSETT.SM_PROTECTION ;

				insert into EDSETT.SM_PROTECTION
				(ID,PARENT_TYPE,PARENT_ID,DATECREATED,CREATEDBY,DATE_MODIFIED,MODIFIEDBY,PROTECTION_TYPE,CURRENT_FUTURE		--,EXPORT_KW,FUSE_SIZE,FUSE_TYPE,MANUFACTURER,CERTIFIED_INVERTER,PHA_MIN_AMPS_TRIP,PHA_SLOW_CURVE,PHA_FAST_CURVE,PHA_TIME_DIAL,PHA_INST,GRD_MIN_AMPS_TRIP,GRD_SLOW_CURVE,GRD_FAST_CURVE,GRD_TIME_DIAL,GRD_INST,NOTES
				)
				values
				(h_max_id_PROTECTION+1,'PRIMARYMETER',v_id(indx).ID,sysdate,h_user,sysdate,h_user,h_prtcn_typ,v_id(indx).CURRENT_FUTURE) ;


			end loop ;

		 EXCEPTION WHEN OTHERS THEN
				ROLLBACK TO err_found_2 ;
				--dbms_output.put_line (SQLERRM) ;

		end;
end ;

/
