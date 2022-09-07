CREATE OR REPLACE PACKAGE LAYERS_INFO AS
	FUNCTION GETMINXFORSHAPE(t_owner IN NVARCHAR2,t_name IN NVARCHAR2,t_shapenum IN NUMBER) RETURN NUMBER;	
	FUNCTION GETMINYFORSHAPE(t_owner IN NVARCHAR2,t_name IN NVARCHAR2,t_shapenum IN NUMBER) RETURN NUMBER;
    FUNCTION GETNEXTROWID(t_owner IN NVARCHAR2,t_name IN NVARCHAR2) RETURN NUMBER;	
END LAYERS_INFO;
/
CREATE OR REPLACE PACKAGE BODY LAYERS_INFO AS
FUNCTION GETMINXFORSHAPE (t_owner IN NVARCHAR2, t_name IN NVARCHAR2, t_shapenum IN NUMBER)
RETURN NUMBER
    IS      
      CURSOR FEATURE_TABLE_NAME is 
	     select OWNER||'.F'||LAYER_ID FTABLE from sde.layers where table_name=t_name and owner=t_owner ;
	  XMIN NUMBER;
	  sql_stmt VARCHAR2(1000);
    BEGIN
      XMIN:=0;
	  FOR i IN FEATURE_TABLE_NAME
      LOOP
        sql_stmt := 'select EMINX from '||i.FTABLE||' where FID='||t_shapenum ;
		execute immediate sql_stmt into XMIN ;
	  END LOOP;
	  RETURN XMIN;
END GETMINXFORSHAPE;
FUNCTION GETMINYFORSHAPE (t_owner IN NVARCHAR2, t_name IN NVARCHAR2, t_shapenum IN NUMBER)
RETURN NUMBER
    IS      
      CURSOR FEATURE_TABLE_NAME is 
	     select OWNER||'.F'||LAYER_ID FTABLE from sde.layers where table_name=t_name and owner=t_owner ;
	  XMIN NUMBER;
	  sql_stmt VARCHAR2(1000);
    BEGIN
      XMIN:=0;
	  FOR i IN FEATURE_TABLE_NAME
      LOOP
        sql_stmt := 'select EMINY from '||i.FTABLE||' where FID='||t_shapenum ;
		execute immediate sql_stmt into XMIN ;
	  END LOOP;
	  RETURN XMIN;
END GETMINYFORSHAPE;
FUNCTION GETNEXTROWID (t_owner IN NVARCHAR2, t_name IN NVARCHAR2)
RETURN NUMBER
    IS      
      CURSOR TABLE_SEQUENCES_NUM is 
	     select OWNER||'.R'||REGISTRATION_ID SEQ_NAME from sde.table_registry where table_name=t_name and owner=t_owner ;
	  row_num NUMBER;
	  sql_stmt VARCHAR2(1000);
    BEGIN
      row_num:=0;
	  FOR i IN TABLE_SEQUENCES_NUM
      LOOP
        sql_stmt := 'select '||i.SEQ_NAME||'.nextval from dual' ;
		execute immediate sql_stmt into row_num ;
	  END LOOP;
	  RETURN row_num;
END GETNEXTROWID;
END LAYERS_INFO;
/

grant all on layers_info to public;
-- test with
-- select sde.layers_info.getminxforshape('EDGIS','TRANSFORMER',14) MINX,sde.layers_info.getminyforshape('EDGIS','TRANSFORMER',14) MINY,sde.layers_info.getnextrowid('EDGIS','TRANSFORMER') from dual;
