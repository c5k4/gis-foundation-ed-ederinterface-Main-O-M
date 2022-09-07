spool D:\DA_58_Scripts\Logs\SQL_13.txt
set serveroutput on
prompt Updating SERVICELOCATION ending with Objectid 4,5;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.SERVICELOCATION where objectid like ('%4') OR objectid like ('%5');
  
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.SERVICELOCATION op set op.city  = (select tba.CITY_NAME 
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
  
END;
/
prompt SERVICELOCATION update complete;
select current_timestamp from dual;

prompt Updating DCCONDUCTOR;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.DCCONDUCTOR;
  cursor c2 is select * from a136;
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.DCCONDUCTOR op set op.city  = (select tba.CITY_NAME 
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
     update edgis.a136 op set op.city  = (select tba.CITY_NAME 
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
prompt DCCONDUCTOR update complete;
select current_timestamp from dual;


spool off;