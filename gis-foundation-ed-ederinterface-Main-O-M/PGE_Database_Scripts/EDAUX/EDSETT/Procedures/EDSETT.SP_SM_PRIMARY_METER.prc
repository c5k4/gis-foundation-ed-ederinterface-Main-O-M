Prompt drop Procedure SP_SM_PRIMARY_METER;
DROP PROCEDURE EDSETT.SP_SM_PRIMARY_METER
/

Prompt Procedure SP_SM_PRIMARY_METER;
--
-- SP_SM_PRIMARY_METER  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_PRIMARY_METER"
AS
    No_of_Rec_GIS Number;
    No_of_Rec_Hed Number;
    No_of_Rec_Null Number;

BEGIN
  INSERT INTO SM_PRIMARY_METER
  ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
   SELECT a.GLOBALID,'EDGIS.PrimaryMeter', a.OPERATINGNUMBER, INITCAP(b.DIV_NAME), INITCAP(c.DIST_NAME), 'C'
     FROM EDSETTGIS.PRIMARYMETER A, GIS_DIVISIONS b, GIS_DISTRICTS C
    WHERE b.DIV_#  (+)     = A.DIVISION
      AND C.DIST_# (+)     = A.DISTRICT;

  INSERT INTO SM_PRIMARY_METER
  ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
   SELECT a.GLOBALID,'EDGIS.PrimaryMeter', a.OPERATINGNUMBER, INITCAP(b.DIV_NAME), INITCAP(c.DIST_NAME), 'F'
     FROM EDSETTGIS.PRIMARYMETER A, GIS_DIVISIONS b, GIS_DISTRICTS C
    WHERE b.DIV_# (+)      = A.DIVISION
      AND C.DIST_# (+)      = A.DISTRICT;

   update SM_PRIMARY_METER a set ( DIVISION, DISTRICT ) = ( select INITCAP(DIVISION),INITCAP(DISTRICT) from GIS_DIVDIST b
                                                             Where a.global_id = b.global_id )
    Where exists ( select DIVISION,DISTRICT from GIS_DIVDIST b Where a.global_id = b.global_id )
      and ( district is Null or division is Null );

  select count(*) into No_of_Rec_GIS from EDSETTGIS.PRIMARYMeter;
  select count(*) into No_of_Rec_Hed from SM_PRIMARY_Meter;
  select count(*) into No_of_Rec_Null From SM_PRIMARY_Meter where ( district is Null or division is Null );

  DBMS_OUTPUT.PUT_LINE('No of Record in the GIS table        : ' || No_of_Rec_GIS );
  DBMS_OUTPUT.PUT_LINE('No of Record in Primary Meter Header : ' || No_of_Rec_Hed );
  DBMS_OUTPUT.PUT_LINE('No of Null Record in Primary Meter for District & Division : ' || No_of_Rec_Null );


END SP_SM_PRIMARY_METER ;

/
