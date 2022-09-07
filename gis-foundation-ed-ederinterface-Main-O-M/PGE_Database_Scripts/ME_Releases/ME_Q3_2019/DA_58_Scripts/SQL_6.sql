spool D:\DA_58_Scripts\Logs\SQL_6.txt
set serveroutput on
prompt Updating PRIOHCONDUCTOR ending with ObjectId 0,1,2,3,4,5,6;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.PRIOHCONDUCTOR where objectid like ('%0') OR objectid like ('%1') OR objectid like ('%2') OR objectid like ('%3') OR objectid like ('%4') OR objectid like ('%5') OR objectid like ('%6');
  cursor c2 is select * from a139;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.PRIOHCONDUCTOR op set op.city  = (select tba.CITY_NAME 
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
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a139 op set op.city  = (select tba.CITY_NAME 
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
prompt PRIOHCONDUCTOR update complete;
select current_timestamp from dual;

prompt Updating VAULTPOLY;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.VAULTPOLY ;
  cursor c2 is select * from a147;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.VAULTPOLY op set op.city  = (select tba.CITY_NAME 
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
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a147 op set op.city  = (select tba.CITY_NAME 
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
prompt VAULTPOLY update complete;
select current_timestamp from dual;

prompt Updating CAPACITORBANK;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.CAPACITORBANK ;
  cursor c2 is select * from a124;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.CAPACITORBANK op set op.city  = (select tba.CITY_NAME 
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
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a124 op set op.city  = (select tba.CITY_NAME 
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
prompt CAPACITORBANK update complete;
select current_timestamp from dual;

prompt Updating DYNAMICPROTECTIVEDEVICE;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.DYNAMICPROTECTIVEDEVICE ;
  cursor c2 is select * from a114;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.DYNAMICPROTECTIVEDEVICE op set op.city  = (select tba.CITY_NAME 
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
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a114 op set op.city  = (select tba.CITY_NAME 
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
prompt DYNAMICPROTECTIVEDEVICE update complete;
select current_timestamp from dual;


spool off;