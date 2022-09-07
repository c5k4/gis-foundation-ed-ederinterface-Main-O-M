Prompt drop Procedure RECALC_LOCALOFFICEID;
DROP PROCEDURE EDGIS.RECALC_LOCALOFFICEID
/

Prompt Procedure RECALC_LOCALOFFICEID;
--
-- RECALC_LOCALOFFICEID  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.recalc_localofficeid(globalidIn IN VARCHAR2,fcnameIn varchar2)
IS
    objectid number;
    distmap VARCHAR2(60);
    localofficeid VARCHAR2(60);
    fshape sde.st_geometry;
    SQLSTMT    VARCHAR2(2000);
    mp_row maintenanceplat%rowtype;
    first_lo varchar2(200);
    first_lo_maptype number := 0;
    calculated_lo varchar2(20) := NULL;
    elop_lo varchar2(20);
    multiple_los BOOLEAN := FALSE;
    lo_intersects BOOLEAN := FALSE;
    TYPE my_assoc_array_typ IS TABLE OF maintenanceplat%rowtype INDEX BY PLS_INTEGER;
    mps my_assoc_array_typ;
    mps_lo my_assoc_array_typ;
    atable VARCHAR2(60);
    fcname varchar2(60);
    regid number(10);
    field_mapnumber VARCHAR2(20) := 'DISTMAP';
    table_name_no_owner varchar2(60);

BEGIN
  if (fcnameIn is null or fcnameIn = '') then
    select get_fc_name(''||globalidIn||'') into fcname from dual;
    if (fcname is null) then
      dbms_output.put_line('BOGUS globalid');
      return;
    end if;
    dbms_output.put_line('FCNAME [ '||fcname||' ]');
  else
    fcname := fcNameIn;
  end if;

  if upper(fcname) like '%STREETLIGHT%' then
    dbms_output.put_line('Using MAPNUMBER field');
    field_mapnumber := 'MAPNUMBER';
  end if;

    --newly added
  if upper(fcname) like '%CAPACITORBANK%' OR  upper(fcname) like '%DYNAMICPROTECTIVEDEVICE%' OR  upper(fcname) like '%FAULTINDICATOR%' OR  upper(fcname) like '%FUSE%' OR  upper(fcname) like '%NETWORKPROTECTOR%' OR upper(fcname) like '%OPENPOINT%' OR upper(fcname) like '%SMARTMETERNETWORKDEVICE%'  OR upper(fcname) like '%STEPDOWN%' OR upper(fcname) like '%SWITCH%'  OR upper(fcname)  like '%VOLTAGEREGULATOR%'
then
    dbms_output.put_line('Using GEMSDISTMAPNUM field');
    field_mapnumber := 'GEMSDISTMAPNUM';
  end if;


  select substr(fcname, - instr(reverse(fcname), '.') + 1) into table_name_no_owner from dual;
  select registration_id into regid from sde.table_registry where OWNER||'.'||table_name= ''||fcname||'';
  atable := 'EDGIS.A'|| regid;
  dbms_output.put_line('atable [ '||atable||' ]');

  SQLSTMT :=  'select objectid, '||field_mapnumber||', localofficeid, shape from edgis.zz_mv_'||table_name_no_owner||' where globalid='''||globalidIn||'''';
  dbms_output.put_line('sqlstmt [ '||sqlstmt||' ]');
  begin
  execute immediate sqlstmt into objectid, distmap, localofficeid, fshape;
  EXCEPTION
   WHEN NO_DATA_FOUND THEN
    dbms_output.put_line('NOT FOUND IN SDE.DEFAULT');
    return;
   END;

  dbms_output.put_line('**************OID [ '||objectid||' ]');
  dbms_output.put_line('MAPNO [ '||distmap||' ]');
  dbms_output.put_line('LO [ '||localofficeid||' ]');

  begin
  -- grab all intersections of MP at lowest scale, matching LO or not
  select m.* bulk collect into mps from maintenanceplat m
    where sde.st_relation_operators.st_intersects_f(fshape, m.shape) = 1 and
    m.maptype = (select min(mp.maptype) from (select mp.* from maintenanceplat mp
    where sde.st_relation_operators.st_intersects_f(fshape, mp.shape) = 1) mp);

    if mps.COUNT = 0 then
      dbms_output.put_line('Logical NOMAP!');
--      return;
    else -- FOUND something
      FOR i IN mps.FIRST..mps.LAST
      LOOP
        dbms_output.put_line('OBJECTID [ '||i||' ] [ '||mps(i).objectid||' ]');
        dbms_output.put_line('MAPNO [ '||i||' ] [ '||mps(i).mapnumber||' ]');
        dbms_output.put_line('MAPTYPE [ '||i||' ] [ '||mps(i).maptype||' ]');
        dbms_output.put_line('LOCALOFFICEID [ '||i||' ] [ '||mps(i).localofficeid||' ]');
        if i = 1 then
          first_lo := mps(i).localofficeid;
          lo_intersects := TRUE;
        else -- subsequent
          if (first_lo <> mps(i).localofficeid) then
            dbms_output.put_line('Multiple overlapping mps with same maptype/lo!');
            multiple_los := TRUE;
          end if;
        end if;
        if localofficeid is not null then
          if localofficeid = mps(i).localofficeid then
            mps_lo(i) := mps(i);
            -- Just return here -- we have an LO of an intersecting thing
            dbms_output.put_line('LO on intersecting MP matches existing, nothing to do here');
            return;
          end if;
        end if;
      END LOOP;
    end if;

  select g.localofficeid into elop_lo from edgis.eleclocaloffice g
    where sde.st_relation_operators.st_intersects_f(fshape, g.shape) = 1;
    dbms_output.put_line('elop_lo [ '||elop_lo||' ]');

  EXCEPTION
   WHEN NO_DATA_FOUND THEN
    dbms_output.put_line('MAPNO [ ***NULL*** ]');
    return;
   END;

  if mps_lo.COUNT = 0 then
    dbms_output.put_line('Logically WRONG LO -- going to set it');
  end if;

  dbms_output.put_line('MULTIPLE_LOS [ '||case when multiple_los = true then 'true' else 'false' end||' ]');
  dbms_output.put_line('LO_INTERSECTS [ '||case when lo_intersects = true then 'true' else 'false' end||' ]');

  if (localofficeid is null or mps.COUNT = 0) then -- if LO initially unset or no plats found then default to GP
    dbms_output.put_line('LO NOT SET BUT NO PLAT FOUND...USING GABES POLYS');
    calculated_lo := elop_lo;
  elsif (localofficeid is null or lo_intersects = true) then
    if multiple_los = false then
      dbms_output.put_line('Only one LO found, using first MP LO');
      calculated_lo := mps(1).localofficeid;
    elsif multiple_los = true then -- dubious branch
      dbms_output.put_line('MULTIPLE LOS...choosing whatever matches with GLOP');
      FOR i IN mps.FIRST..mps.LAST
      LOOP
        if mps(i).localofficeid = elop_lo then
          dbms_output.put_line('GLOP matches plat...');
          calculated_lo := elop_lo;
        end if;
      end loop;
      if (calculated_lo is null) then
        dbms_output.put_line('GLOP doesnt match any existing but using first maintenanceplat');
        calculated_lo := mps(1).localofficeid;
      end if;
    end if;
  end if;

  if (calculated_lo is not null and localofficeid is null) or
    (calculated_lo is not null and localofficeid <> calculated_lo) then
    dbms_output.put_line('UPDATING LO to [ '||calculated_lo||' ]');
    sqlstmt := 'update '||fcname||' set localofficeid='''||calculated_lo||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    sqlstmt := 'update '||atable||' set localofficeid='''||calculated_lo||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    ADD_GUID_TO_RESYNC_TABLE(globalIdIn, fcname);
  else
    dbms_output.put_line('NO UPDATE');
  end if;

END;

/


Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO GISINTERFACE
/

Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO GIS_I
/

Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO MM_ADMIN
/

Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO SDE_EDITOR
/

Prompt Grants on PROCEDURE RECALC_LOCALOFFICEID TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.RECALC_LOCALOFFICEID TO SDE_VIEWER
/
