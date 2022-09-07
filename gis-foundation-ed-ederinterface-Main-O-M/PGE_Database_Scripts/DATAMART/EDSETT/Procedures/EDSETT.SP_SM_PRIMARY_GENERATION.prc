Prompt drop Procedure SP_SM_PRIMARY_GENERATION;
DROP PROCEDURE EDSETT.SP_SM_PRIMARY_GENERATION
/

Prompt Procedure SP_SM_PRIMARY_GENERATION;
--
-- SP_SM_PRIMARY_GENERATION  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_PRIMARY_GENERATION"
AS
    No_of_Rec_GIS Number;
    No_of_Rec_Hed Number;
    No_of_Rec_Dtl Number;
    No_of_Rec_Null Number;
BEGIN
  INSERT INTO SM_PRIMARY_GEN
  ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
   SELECT a.GLOBALID,'EDGIS.PrimaryGeneration', a.OPERATINGNUMBER, INITCAP(b.DIV_NAME), INITCAP(c.DIST_NAME), 'C'
     FROM EDSETTGIS.PRIMARYGENERATION A, GIS_DIVISIONS b, GIS_DISTRICTS C
    WHERE b.DIV_#  (+) = A.DIVISION
      AND C.DIST_# (+) = A.DISTRICT;

  INSERT INTO SM_PRIMARY_GEN
  ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
   SELECT a.GLOBALID,'EDGIS.PrimaryGeneration', a.OPERATINGNUMBER, INITCAP(b.DIV_NAME), INITCAP(c.DIST_NAME), 'F'
     FROM EDSETTGIS.PRIMARYGENERATION A, GIS_DIVISIONS b, GIS_DISTRICTS C
    WHERE b.DIV_#  (+)  = A.DIVISION
      AND C.DIST_# (+)  = A.DISTRICT;

  update SM_PRIMARY_GEN a set ( DIVISION, DISTRICT ) = ( select INITCAP(DIVISION),INITCAP(DISTRICT) from GIS_DIVDIST b
                                                             Where a.global_id = b.global_id )
    Where exists ( select DIVISION,DISTRICT from GIS_DIVDIST b Where a.global_id = b.global_id )
     and ( district is Null or division is Null );

  INSERT INTO SM_PRIMARY_GEN_DTL
  ( PRIMARY_GEN_ID,GENERATOR_TYPE,RATED_POWER_KVA,RATED_VOLT_KVLL,POWER_FACTOR_PERC, ACTIVE_GENERATION_KW )
  ( select a.ID, ' ', 0,0,0,nvl(b.Kw,0)
      from SM_PRIMARY_GEN a, EDSETTGIS.PRIMARYGENERATION B
     where a.current_future = 'C'
       and b.globalid=a.global_id);

  select count(*) into No_of_Rec_GIS from EDSETTGIS.PRIMARYGENERATION;
  select count(*) into No_of_Rec_Hed from SM_PRIMARY_GEN;
  select count(*) into No_of_Rec_Dtl from SM_PRIMARY_GEN_DTL;
  select count(*) into No_of_Rec_Null From SM_PRIMARY_GEN where ( district is Null or division is Null );

  DBMS_OUTPUT.PUT_LINE('No of Record in the GIS table             : ' || No_of_Rec_GIS );
  DBMS_OUTPUT.PUT_LINE('No of Record in Primary Generation Header : ' || No_of_Rec_Hed );
  DBMS_OUTPUT.PUT_LINE('No of Record in Primary Generation Detail : ' || No_of_Rec_Dtl );
  DBMS_OUTPUT.PUT_LINE('No of Null Record in Primary Generation for District & Division : ' || No_of_Rec_Null );

END SP_SM_PRIMARY_GENERATION ;

/
