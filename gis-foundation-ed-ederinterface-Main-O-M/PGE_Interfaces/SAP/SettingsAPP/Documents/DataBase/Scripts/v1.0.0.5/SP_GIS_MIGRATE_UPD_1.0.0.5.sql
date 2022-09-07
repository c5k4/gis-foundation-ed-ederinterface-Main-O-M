create or replace PROCEDURE    SP_GIS_MIGRATE_UPD
AS
NUM  NUMBER;

BEGIN


---- Insert DEVICE_ID, OPER_NUM into GIS tables for Devices where (DEVICE_ID is NULL or 9999) from CEDSA

DELETE from CEDSA_DEVICE_TMP;
DELETE from GIS_CEDSADEVICEID_CB;
DELETE from GIS_CEDSADEVICEID_VR;

INSERT into GIS_CEDSADEVICEID_CB (GLOBAL_ID,OPERATING_NUM,DEVICE_ID)
select G.global_id, operating_num, CC.DEVICE_ID from gis_cedsadeviceid G, CEDSA_CIRCUIT CC,
  (select * from edsettgis.subinterruptingdevice
    where  SUBSTATIONID is not null
    AND OPERATINGNUMBER is not null) SI
where feature_class_name = 'CircuitBreaker'
and (G.device_id is null or G.device_id = '9999')
and G.global_id = SI.globalid
and CC.fdr_# is not null
and TO_CHAR(CC.FDR_#) = TO_CHAR(SI.SUBSTATIONID) || TO_CHAR(SUBSTR(SI.OPERATINGNUMBER,0,4))
order by operating_num;





INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,OPERATING_NUM)
(
select GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,
   replace(OPERATING_NUM, ' - Bank Code 3') from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
and feature_class_name = 'Regulator' and operating_num like '%Bank Code 3%'

  union all

select GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,
   replace(OPERATING_NUM, ' - Bank Code 2') from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
and feature_class_name = 'Regulator' and operating_num like '%Bank Code 2%'

  union all

select GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,
   replace(OPERATING_NUM, ' - Bank Code 1') from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
and feature_class_name = 'Regulator' and operating_num like '%Bank Code 1%'

  union all

select GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,
   replace(OPERATING_NUM, ' - Bank Code ') from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
and feature_class_name = 'Regulator' and operating_num like '%Bank Code '
);






INSERT into CEDSA_DEVICE_TMP (device_id,oper_#)
(
select distinct device_id,oper_# from
(
select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
  (select oper_# from (select  cd.device_id,cd.oper_#   from  CEDSA_DEVICE CD,
  (select OPERATING_NUM from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
  and  feature_class_name = 'Regulator'
    GROUP BY OPERATING_NUM) GD
  where  CD.OPER_#=  replace (GD.operating_num, ' - Bank Code 3'))  GDO
  group by GDO.oper_# having count(*)  =1 )

  union all

  select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
  (select oper_# from (select  cd.device_id,cd.oper_#   from  CEDSA_DEVICE CD,
  (select OPERATING_NUM from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999) and  feature_class_name = 'Regulator'
    GROUP BY OPERATING_NUM) GD
  where  CD.OPER_#=  replace (GD.operating_num, ' - Bank Code 2'))  GDO
  group by GDO.oper_# having count(*)  =1 )

    union all

  select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
  (select oper_# from (select  cd.device_id,cd.oper_#   from  CEDSA_DEVICE CD,(select OPERATING_NUM from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999)
  and  feature_class_name = 'Regulator'
    GROUP BY OPERATING_NUM) GD
  where  CD.OPER_#=  replace (GD.operating_num, ' - Bank Code 1'))  GDO
  group by GDO.oper_# having count(*)  =1 )

  union all

  select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
  (select oper_# from (select  cd.device_id,cd.oper_#   from  CEDSA_DEVICE CD,(select OPERATING_NUM from GIS_CEDSADEVICEID
   where ((DEVICE_ID is null OR  DEVICE_ID=9999) AND (OPERATING_NUM like '%Bank Code '))
  and  feature_class_name = 'Regulator'
    GROUP BY OPERATING_NUM) GD
  where  CD.OPER_#=  replace (GD.operating_num, ' - Bank Code '))  GDO
  group by GDO.oper_# having count(*)  =1 )
)
);



INSERT into CEDSA_DEVICE_TMP (device_id,oper_#)
  (select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
    (select oper_# from (select cd.device_id,cd.oper_#  from  CEDSA_DEVICE CD,(select * from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999) and feature_class_name <> 'Regulator') GD
     where  CD.OPER_#= GD.operating_num ) GDO
  group by GDO.oper_# having count(*)  =1 ) ) ;

COMMIT;

---- Update DEVICE_ID for Devices where DEVICE_ID is NULL or 9999

UPDATE   (
select GD.device_id old_device_id, UGD.device_id new_device_id
from GIS_CEDSADEVICEID GD, CEDSA_DEVICE_TMP UGD
where GD.operating_num=UGD.oper_# )
set old_device_id=new_device_id;


UPDATE    (
select GD.device_id old_device_id, UGD.device_id new_device_id
from GIS_CEDSADEVICEID_VR GD, CEDSA_DEVICE_TMP UGD
where GD.operating_num=UGD.oper_# )
set old_device_id=new_device_id;


UPDATE    (
select GD.device_id old_device_id, GDR.device_id new_device_id
from GIS_CEDSADEVICEID_VR GDR,  GIS_CEDSADEVICEID GD
where GD.GLOBAL_ID=GDR.GLOBAL_ID)
set old_device_id=new_device_id;


---- Updating DEVICE_ID for Ciruit Breaker where DEVICE_ID is NULL or 9999


UPDATE   (
select GD.device_id old_device_id, GDC.device_id new_device_id
from GIS_CEDSADEVICEID_CB GDC,  GIS_CEDSADEVICEID GD
where GD.GLOBAL_ID=GDC.GLOBAL_ID)
set old_device_id=new_device_id;


COMMIT;


--- Below code is to update the BANKCODE field values from NULL to values 1/2/3 in GIS_CEDSADEVICEID tables

DECLARE
CURSOR BANKCODE_LIST IS
  select OPERATING_NUM,global_id from edsett.gis_cedsadeviceid where bankcode is null and FEATURE_CLASS_NAME='Regulator' and GIS_FEATURE_CLASS_NAME='EDGIS.VoltageRegulatorUnit';  
 sqlstr VARCHAR2(10000);
sqlstr2 VARCHAR2(10000);
row_cnt varchar(2000);
bankcode number;
TYPE bankCodes IS REF CURSOR;
bankCodeCursor bankCodes;
BEGIN
  FOR regulator_info IN BANKCODE_LIST LOOP
   bankcode := 0;   
   LOOP
             bankcode := bankcode+1;
             sqlstr := 'select OPERATING_NUM from edsett.gis_cedsadeviceid where operating_num='''||regulator_info.operating_num||' '||bankcode||''' ';                                            
             dbms_output.put_line('sqlstr is :'||sqlstr);
             OPEN bankCodeCursor for sqlstr;
             FETCH bankCodeCursor INTO row_cnt;
             EXIT WHEN bankCodeCursor%NOTFOUND;
   END LOOP;
   sqlstr2 := 'update edsett.gis_cedsadeviceid set operating_num='''||regulator_info.operating_num||' '||bankcode||''' where GLOBAL_ID='''||regulator_info.global_id||''' ';   
   --dbms_output.put_line('sqlstmt2 is :'||sqlstr2);
  EXECUTE IMMEDIATE sqlstr2;  
   sqlstr2 := 'update edsett.gis_cedsadeviceid set bankcode='''||bankcode||''' where GLOBAL_ID='''||regulator_info.global_id||''' ';   
   --dbms_output.put_line('sqlstr2 is :'||sqlstr2);
  EXECUTE IMMEDIATE sqlstr2;
  commit;
  END LOOP;  
END;



END SP_GIS_MIGRATE_UPD ;