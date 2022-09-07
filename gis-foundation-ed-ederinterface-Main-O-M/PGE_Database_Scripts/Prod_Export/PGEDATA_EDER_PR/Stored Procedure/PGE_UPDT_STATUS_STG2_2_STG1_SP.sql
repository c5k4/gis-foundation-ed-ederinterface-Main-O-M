--------------------------------------------------------
--  DDL for Procedure PGE_UPDT_STATUS_STG2_2_STG1_SP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."PGE_UPDT_STATUS_STG2_2_STG1_SP" 
IS


	TYPE t_bulk_collect IS TABLE OF PGEDATA.PGEDATA_GENERATIONINFO_STAGE%ROWTYPE;
		v_gen_row    t_bulk_collect := t_bulk_collect();


	--TYPE t_bulk_collect_prot IS TABLE OF PGEDATA.PGEDATA_SM_PROTECTION_STAGE%ROWTYPE;
	--	v_prot_row    t_bulk_collect_prot := t_bulk_collect_prot();


	TYPE t_bulk_collect_eqp IS TABLE OF PGEDATA.PGEDATA_SM_GENERATOR_STAGE%ROWTYPE;
		v_eqp_row    t_bulk_collect_eqp := t_bulk_collect_eqp();

	TYPE t_bulk_collect_eqp_2 IS TABLE OF PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE%ROWTYPE;
		v_eqp_row_2    t_bulk_collect_eqp_2 := t_bulk_collect_eqp_2();



	BEGIN
			select * bulk collect into  v_gen_row from PGEDATA.PGEDATA_GENERATIONINFO_STAGE ;
      --where UPDATED_IN_MAIN is not null order by UPDATED_IN_MAIN


			--SAVEPOINT err_found_2 ;
			  begin
				--dbms_output.put_line ('loop_1 count is - '||v_gen_row.COUNT) ;

					for indx in 1 .. v_gen_row.COUNT loop

					  update PGEDATA.GEN_SUMMARY_STAGE set
						  UPDATED_IN_MAIN = v_gen_row(indx).UPDATED_IN_MAIN
						  ,UPDATED_IN_ED_MAIN = (select UPDATED_IN_ED_MAIN from PGEDATA.PGEDATA_SM_GENERATION_STAGE where GLOBAL_ID = v_gen_row(indx).GLOBALID)
						  ,UPDATED_IN_SETTINGS = ( select A.UPDATED_IN_SETTINGS from PGEDATA.PGEDATA_SM_PROTECTION_STAGE A, PGEDATA.PGEDATA_SM_GENERATION_STAGE B where  A.parent_id =B.ID and upper(A.PARENT_TYPE)='GENERATION'  and B.SERVICE_POINT_ID = v_gen_row(indx).SERVICE_POINT_ID )
					  where SERVICE_POINT_ID = v_gen_row(indx).SERVICE_POINT_ID ;


						select G.* bulk collect into v_eqp_row from PGEDATA.PGEDATA_SM_GENERATOR_STAGE G,PGEDATA.PGEDATA_SM_PROTECTION_STAGE A, PGEDATA.PGEDATA_SM_GENERATION_STAGE B where G.PROTECTION_ID= A.ID and A.parent_id =B.ID and upper(A.PARENT_TYPE)='GENERATION'  and B.SERVICE_POINT_ID = v_gen_row(indx).SERVICE_POINT_ID ;

							for k in 1 .. v_eqp_row.COUNT loop

								update PGEDATA.GEN_EQUIPMENT_STAGE set
								UPDATED_IN_SETTINGS = v_eqp_row(k).UPDATED_IN_SETTINGS
								where SAP_EQUIPMENT_ID = v_eqp_row(k).SAP_EQUIPMENT_ID and  SERVICE_POINT_ID = v_gen_row(indx).SERVICE_POINT_ID ;

								select * bulk collect into v_eqp_row_2 from PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE where GENERATOR_ID = v_eqp_row(k).ID ; --SAP_EQUIPMENT_ID ;

									for t in 1 .. v_eqp_row_2.COUNT loop

										update PGEDATA.GEN_EQUIPMENT_STAGE set
										UPDATED_IN_SETTINGS = v_eqp_row_2(t).UPDATED_IN_SETTINGS
										where SAP_EQUIPMENT_ID = v_eqp_row_2(t).SAP_EQUIPMENT_ID and  SERVICE_POINT_ID = v_gen_row(indx).SERVICE_POINT_ID ;

									end loop ;

							end loop;

						commit;

					end loop;
			  end ;
			  commit;

			EXCEPTION WHEN OTHERS THEN
			--ROLLBACK TO err_found_2 ;
			dbms_output.put_line(SQLERRM);


	END PGE_UPDT_STATUS_STG2_2_STG1_SP;
