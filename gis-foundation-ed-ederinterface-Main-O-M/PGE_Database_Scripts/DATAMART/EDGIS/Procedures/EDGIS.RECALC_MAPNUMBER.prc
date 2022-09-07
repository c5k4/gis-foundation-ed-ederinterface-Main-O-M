Prompt drop Procedure RECALC_MAPNUMBER;
DROP PROCEDURE EDGIS.RECALC_MAPNUMBER
/

Prompt Procedure RECALC_MAPNUMBER;
--
-- RECALC_MAPNUMBER  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.recalc_mapnumber(globalidIn IN VARCHAR2,fcnameIn varchar2)
IS
    objectid number;
    distmap VARCHAR2(60);
    localofficeid VARCHAR2(60);
    fshape sde.st_geometry;
    SQLSTMT    VARCHAR2(2000);
    FLD_MAPNO    VARCHAR2(20);
    mp_row maintenanceplat%rowtype;
    first_mapno varchar2(200);
    first_lo_maptype number := 0;
    calculated_mapno varchar2(20) := NULL;
    mp_lo_mapno varchar2(20) := NULL;
    mps_lo_maptype number;
    mps_lo_mapnumber varchar2(20);
    multiple_lo_mapnos BOOLEAN := FALSE;
    lo_intersects BOOLEAN := FALSE;
    TYPE my_assoc_array_typ IS TABLE OF maintenanceplat%rowtype INDEX BY PLS_INTEGER;
    mps my_assoc_array_typ;
    mps_lo my_assoc_array_typ;
    mps_lo_lowest_scale my_assoc_array_typ;
    atable VARCHAR2(60);
    fcname varchar2(60);
    regid number(10);
    field_mapnumber VARCHAR2(20) := 'GEMSDISTMAPNUM';
    table_name_no_owner varchar2(60);
BEGIN
  if (fcnameIn is null  or fcnameIn = '') then
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
  if upper(fcname) like '%SUPPORTSTRUCTURE%' OR upper(fcname) like '%DEVICEGROUP%' OR upper(fcname) like '%PADMOUNTSTRUCTURE%' OR upper(fcname) like '%SUBSURFACESTRUCTURE%' then
    dbms_output.put_line('Using MAPNUMBER field');
    field_mapnumber := 'DISTMAP';
  end if;
  --newly added  to update gemsdistmapnum field
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

  dbms_output.put_line('Spatial Search against MaintenancePlat...');
  select m.* bulk collect into mps from maintenanceplat m
    where sde.st_relation_operators.st_intersects_f(fshape, m.shape) = 1
    order by maptype asc;

    if mps.COUNT = 0 then
      dbms_output.put_line('NOMAP');
      calculated_mapno := 'NOMAP';
    else
      FOR i IN mps.FIRST..mps.LAST
      LOOP
        dbms_output.put_line('OBJECTID [ '||i||' ] [ '||mps(i).objectid||' ]');
        dbms_output.put_line('MAPNO [ '||i||' ] [ '||mps(i).mapnumber||' ]');
        dbms_output.put_line('MAPTYPE [ '||i||' ] [ '||mps(i).maptype||' ]');
        dbms_output.put_line('LOCALOFFICEID [ '||i||' ] [ '||mps(i).localofficeid||' ]');
        -- find maps with matching LO, and maps with matching lo at lowest scale e.g. 100
        if mps(i).localofficeid = localofficeid then
          if first_lo_maptype = 0 then
            first_lo_maptype := mps(i).maptype;
            mps_lo_lowest_scale(i) := mps(i);
          elsif first_lo_maptype = mps(i).maptype then -- we have multiple overlapping mps with same maptype
            dbms_output.put_line('Multiple overlapping mps with same maptype/lo!');
            mps_lo_lowest_scale(i) := mps(i);
          end if;
          mps_lo(i) := mps(i);
        end if;
      END LOOP;
    end if;

  EXCEPTION
   WHEN NO_DATA_FOUND THEN
    dbms_output.put_line('MAPNO [ ***NULL*** ]');
    return;
   END;

  if mps.COUNT > 0 then
    if mps_lo.COUNT = 0 then
      dbms_output.put_line('NO PLATS WITH MATCHING LO -> WRONG LO');
      calculated_mapno := 'WRONG LO';
    else
      dbms_output.put_line('COMPILING LOWEST SCALE MATCHING LO');
      FOR i IN mps_lo.FIRST..mps_lo.LAST
      LOOP
        if mp_lo_mapno is null then
          mp_lo_mapno := mps_lo(i).mapnumber;
          mps_lo_maptype := mps_lo(i).maptype;
          mps_lo_mapnumber := mps_lo(i).mapnumber;
          dbms_output.put_line('mps_lo(i).mapnumber [ '||mps_lo(i).mapnumber||' ] first_lo_maptype [ '||first_lo_maptype||' ]');
          dbms_output.put_line('mp_lo_mapno [ '||mp_lo_mapno||' ]');
          dbms_output.put_line('mps_lo(i).maptype [ '||mps_lo(i).maptype||' ].');
        elsif ((mps_lo_mapnumber <> mp_lo_mapno) and (mps_lo_maptype=first_lo_maptype)) then
          dbms_output.put_line('MULTIPLE DISTINCT MAPNOS AT SAME MAPTYPE');
         multiple_lo_mapnos := TRUE;
        end if;
     end loop;
    end if;

    if multiple_lo_mapnos = FALSE then
      calculated_mapno := mp_lo_mapno;
      dbms_output.put_line('calculated_mapno [ '||calculated_mapno||' ]');
    else
      dbms_output.put_line('MULTIPLE DISTINCT LO MAPNOS AT LOWEST SCALE => DUPMAP');
      calculated_mapno := 'DUPMAP';
    end if;
  end if;

  if (calculated_mapno is not null and distmap is null) or
     (calculated_mapno is not null and distmap <> calculated_mapno) then
    dbms_output.put_line('UPDATING DISTMAP/MAPNUMBER/GEMSDISTMAPNUM TO [ '||calculated_mapno||' ]');
    sqlstmt := 'update '||fcname||' set '||field_mapnumber||'='''||calculated_mapno||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    sqlstmt := 'update '||atable||' set '||field_mapnumber||'='''||calculated_mapno||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    ADD_GUID_TO_RESYNC_TABLE(globalIdIn, fcname);
  else
    dbms_output.put_line('NO UPDATE');
  end if;

END;

/


Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO GISINTERFACE
/

Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO GIS_I
/

Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO MM_ADMIN
/

Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO SDE_EDITOR
/

Prompt Grants on PROCEDURE RECALC_MAPNUMBER TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.RECALC_MAPNUMBER TO SDE_VIEWER
/
