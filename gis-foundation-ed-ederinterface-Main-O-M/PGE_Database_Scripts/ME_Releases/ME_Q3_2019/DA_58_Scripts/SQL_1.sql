spool D:\DA_58_Scripts\Logs\SQL_1.txt
set serveroutput on
prompt Updating PHOTOVOLTAICCELL;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  cursor c1 is select * from edgis.PHOTOVOLTAICCELL ;
  cursor c2 is select * from a1727;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.PHOTOVOLTAICCELL op set op.city  = (select tba.CITY_NAME 
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
  
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a1727 op set op.city  = (select tba.CITY_NAME 
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
END;
/
prompt PHOTOVOLTAICCELL update complete;
select current_timestamp from dual;

prompt Updating DCRECTIFIER;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  cursor c1 is select * from edgis.DCRECTIFIER ;
  cursor c2 is select * from a123;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.DCRECTIFIER op set op.city  = (select tba.CITY_NAME 
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
  
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a123 op set op.city  = (select tba.CITY_NAME 
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
END;
/
prompt DCRECTIFIER update complete;
select current_timestamp from dual;

prompt Updating STEPDOWN;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  cursor c1 is select * from edgis.STEPDOWN ;
  cursor c2 is select * from a118;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.STEPDOWN op set op.city  = (select tba.CITY_NAME 
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
  
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a118 op set op.city  = (select tba.CITY_NAME 
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
END;
/
prompt STEPDOWN update complete;
select current_timestamp from dual;

prompt Updating SUBSTATION;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  cursor c1 is select * from edgis.SUBSTATION ;
  cursor c2 is select * from a143;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.SUBSTATION op set op.city  = (select tba.CITY_NAME 
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
  
   For rw in c2
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.a143 op set op.city  = (select tba.CITY_NAME 
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
END;
/
prompt SUBSTATION update complete;
select current_timestamp from dual;


prompt Updating SUBSURFACESTRUCTURE;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;
  counter NUMBER := 0;	
  cursor c1 is select * from edgis.SUBSURFACESTRUCTURE ;
  cursor c2 is select * from a133;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.SUBSURFACESTRUCTURE op set op.city  = (select tba.CITY_NAME 
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
     update edgis.a133 op set op.city  = (select tba.CITY_NAME 
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
END;
/
prompt SUBSURFACESTRUCTURE update complete;
select current_timestamp from dual;

prompt Updating DCSERVICELOCATION;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;
  counter NUMBER :=0;  
  cursor c1 is select * from edgis.DCSERVICELOCATION ;
  cursor c2 is select * from a16727;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.DCSERVICELOCATION op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2))
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
     update edgis.a16727 op set op.city  = (select tba.CITY_NAME 
     from edgis.city tba
     where  sde.st_intersects(tba.shape,rw.shape)  = 1 and rownum<2),
     op.county = ( select cnty.code from edgis.countyname_lookup cnty where county_name in 
     (select clp.cnty_name 
     from edgis.countyunclipped clp
     where  sde.st_intersects(clp.shape,rw.shape)  = 1 and rownum<2))
    where op.objectid= x;
	COMMIT;
    EXCEPTION
	WHEN OTHERS THEN 
	dbms_output.put_line ('Error Occured in OBJECTID: '|| x);
	CONTINUE;
	END;

  END LOOP; 
END;
/
prompt DCSERVICELOCATION update complete;
select current_timestamp from dual;


spool off;