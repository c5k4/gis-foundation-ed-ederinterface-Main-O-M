Prompt drop Function CONSOLIDATE_NOTES;
DROP FUNCTION PGEDATA.CONSOLIDATE_NOTES
/

Prompt Function CONSOLIDATE_NOTES;
--
-- CONSOLIDATE_NOTES  (Function) 
--
CREATE OR REPLACE FUNCTION PGEDATA.consolidate_notes(i_type varchar2, i_protection_id NUMBER, i_status_cd varchar2,
         i_manf_cd varchar2, i_model_cd varchar2, i_enos_ref_id number, i_enos_eqp_id number)
RETURN VARCHAR2
IS
/*     consolidate notes information for the passed protection id
        TYPE - which table to consolidate notes for:
               DC - Direct Current
               Inv - inverter

   ******************** MODIFICATION HISTORY *********************
   Person      Date        Comments
   ---------   --------    -------------------------------------------
   Twyla Jay   08/19/16    created
   Twyla Jay   10/24/16    modified to consider manf_cd, model_cd, enos_ref_id,
                           enos_eqp_id, status_cd

*/
CURSOR get_Notes
IS
SELECT distinct notes
   FROM (SELECT trim(notes) notes
           FROM cedsadata.dc_generation dc
          WHERE dc.protection_id = i_protection_id
            and manf_cd = i_manf_cd
            and model_cd = i_model_cd
            and status_cd = i_status_cd
            and nvl(enos_ref_id, 0) = nvl(i_enos_ref_id,0)
            and nvl(enos_eqp_id,0) = nvl(i_enos_eqp_id,0)
            AND i_type = 'DC'
         union
         SELECT trim(notes) notes
           FROM cedsadata.inverter
          WHERE protection_id = i_protection_id
            and manf_cd = i_manf_cd
            and model_cd = i_model_cd
            and status_cd = i_status_cd
            and nvl(enos_ref_id, 0) = nvl(i_enos_ref_id,0)
            and nvl(enos_eqp_id,0) = nvl(i_enos_eqp_id,0)
            AND i_type = 'Inv');

v_type_count NUMBER;
v_note VARCHAR2(2000);
v_kw_out NUMBER(9,1);
v_result varchar2(3000);

BEGIN

OPEN get_Notes;
   LOOP
      FETCH get_Notes INTO v_note;
         EXIT WHEN (get_Notes%NOTFOUND or length(v_result) = 2002);
            v_result := substr(v_result || chr(10) || chr(13) || v_note, 1,2002);
      END LOOP;
      CLOSE get_Notes;
    RETURN substr(v_result,3) ;

EXCEPTION
   WHEN others THEN
       RETURN 'Error' ;
END consolidate_notes;
/
