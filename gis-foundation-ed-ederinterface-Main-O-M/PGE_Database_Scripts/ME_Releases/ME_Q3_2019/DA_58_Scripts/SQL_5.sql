spool D:\DA_58_Scripts\Logs\SQL_5.txt
set serveroutput on
prompt Updating DISTBUSBAR;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.DISTBUSBAR ;
  cursor c2 is select * from a135;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.DISTBUSBAR op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    counter := counter+1;
    if counter = 10000 then
    commit;
    end if;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;  
  --dbms_output.put_line ('Total Update count is : '|| counter );
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a135 op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    commit;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;
END;
/
prompt DISTBUSBAR update complete;
select current_timestamp from dual;

prompt Updating ANCHOR;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.ANCHOR ;
  cursor c2 is select * from a7366252;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.ANCHOR op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    counter := counter+1;
    if counter = 10000 then
    commit;
    end if;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;
 -- dbms_output.put_line ('Total Update count is : '|| counter );
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a7366252 op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    commit;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;
END;
/
prompt ANCHOR update complete;
select current_timestamp from dual;

prompt Updating SECONDARYRISER;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.SECONDARYRISER ;
  cursor c2 is select * from a384;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.SECONDARYRISER op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    counter := counter+1;
    if counter = 10000 then
    commit;
    end if;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;
  --dbms_output.put_line ('Total Update count is : '|| counter );
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a384 op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2)), 
     op.zip = (select zp.zip
     from edgis.MaponicsZip zp
     where  sde.st_intersects(zp.shape,rw.shape)  = 1 and rownum<2)
    where op.objectid= x;
    commit;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;
  END LOOP; 
  commit;
END;
/
prompt SECONDARYRISER update complete;
select current_timestamp from dual;


spool off;