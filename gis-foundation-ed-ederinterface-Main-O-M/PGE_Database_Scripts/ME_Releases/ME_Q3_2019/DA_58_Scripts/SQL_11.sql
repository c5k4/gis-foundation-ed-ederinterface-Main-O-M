spool D:\DA_58_Scripts\Logs\SQL_11.txt
set serveroutput on
prompt Updating SUPPORTSTRUCTURE ending with Objecid 9;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.SUPPORTSTRUCTURE where objectid like ('%9');
  
BEGIN
  For rw in c1
  LOOP
  BEGIN
    x := rw.objectid;    
     update edgis.SUPPORTSTRUCTURE op set op.city  = (select tba.CITY_NAME 
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
prompt SUPPORTSTRUCTURE update complete;
select current_timestamp from dual;

prompt Updating SERVICELOCATION;
select current_timestamp from dual;

DECLARE
  x NUMBER := 0;  
  counter NUMBER := 0;
  cursor c1 is select * from edgis.SERVICELOCATION where objectid like ('%0') OR objectid like ('%1');
  
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


spool off;