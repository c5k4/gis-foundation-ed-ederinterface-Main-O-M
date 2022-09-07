--------------------------------------------------------
--  DDL for Procedure UPDATE_FI_MATERIALCODE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."UPDATE_FI_MATERIALCODE" 
IS
  atable_FI VARCHAR2(60);
  regid NUMBER(10);
  sqlstmt1  VARCHAR2(500);

 BEGIN

  update EDGIS.FAULTINDICATOR
  set MATERIALCODE = replace(MATERIALCODE, '-', ''),DATEMODIFIED = SYSDATE, LASTUSER = 'SYNC'  where length(MATERIALCODE)>7 and MATERIALCODE  LIKE '%-%';

  -- FOR RECORDS CONTAINS 'S' IN MATERIAL CODE
  update EDGIS.FAULTINDICATOR
  set MATERIALCODE = replace(MATERIALCODE, 'S', ''),DATEMODIFIED = SYSDATE, LASTUSER = 'SYNC'  where length(MATERIALCODE)>7 and MATERIALCODE  LIKE '%S%';

  -- GET A TABLE FOR FAULTINDICATOR
  SELECT registration_id
  INTO regid
  FROM sde.table_registry
  WHERE table_name= 'FAULTINDICATOR';
  atable_FI      := 'EDGIS.A'|| regid;


  sqlstmt1 :='update '||atable_FI||'
  set MATERIALCODE = replace(MATERIALCODE, ''-'', ''''),DATEMODIFIED = SYSDATE,LASTUSER = ''SYNC'' where length(MATERIALCODE)>7 and MATERIALCODE  LIKE ''%-%''';
    EXECUTE immediate sqlstmt1;

  COMMIT;
  dbms_output.enable(NULL);
  -- PRINT OUTPUT
   FOR v_reg IN
  (SELECT globalid FROM EDGIS.ZZ_MV_FAULTINDICATOR WHERE lastuser='SYNC' AND DATEMODIFIED = SYSDATE
  )
  LOOP
    dbms_output.put_line(v_reg.globalid||',EDGIS.FAULTINDICATOR'); --PRINT OUTPUT
  END LOOP;
EXCEPTION
WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE('Update_FI_MATERIALCODE Error:'||sqlerrm);
  ROLLBACK;
END Update_FI_MATERIALCODE;
