create or replace PROCEDURE ADD_GUID_TO_RESYNC_TABLE (
   guid IN VARCHAR2, fc in varchar2)
IS
  rowcnt number;
BEGIN
  select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
  if rowcnt<1 then
    dbms_output.put_line('INSERTING [ '||guid||']');
    insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
    values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
    guid,fc,SYSDATE,'P1PC');
  end if;
END;

create or replace PROCEDURE SHOW_GUID_EQUIPID (guid IN VARCHAR2)
AS
BEGIN
  DECLARE
    CURSOR ALL_TABLES_WE_CARE_ABOUT
    IS
      SELECT a.owner,
        a.table_name
      FROM sde.column_registry a
      WHERE a.column_name='GLOBALID'
      AND a.table_name  IN
        (SELECT b.table_name
        FROM sde.column_registry b
        WHERE b.column_name='SAPEQUIPID'
        )and a.table_name NOT LIKE 'ZPGEVW%' and a.table_name NOT LIKE 'ZZ_MV%';

    sqlstmt    VARCHAR2(2000);
    reg_num   NUMBER;
    count_num NUMBER;
    rowcnt number;
    equipid number;
  BEGIN
    dbms_output.put_line('GUID [ '||guid||' ]');
    FOR tables IN ALL_TABLES_WE_CARE_ABOUT
    LOOP
--      dbms_output.put_line('TABLE [ '||tables.table_name||' ]');
      sqlstmt := 'select count(*) from '||tables.table_name||' where globalid='''||guid||'''  ';
      execute immediate sqlstmt into rowcnt;
      if rowcnt>0 then
       dbms_output.put_line('BINGO!!! [ '||tables.table_name||' ]');
        sqlstmt := 'select sapequipid from '||tables.table_name||' where globalid='''||guid||'''  ';
        execute immediate sqlstmt into equipid;
        dbms_output.put_line('SAPEQUIPID [ '||equipid||' ]');
      END IF;

    END LOOP;
  END;
END SHOW_GUID_EQUIPID;
/

create or replace
function GET_FC_NAME (guid IN VARCHAR2)
RETURN nvarchar2
is
BEGIN
  DECLARE
    CURSOR ALL_TABLES_WE_CARE_ABOUT
    IS
      SELECT a.owner,
        a.table_name
      FROM sde.column_registry a
      WHERE a.column_name='GLOBALID'
      AND a.table_name  IN
        (SELECT b.table_name
        FROM sde.column_registry b
        WHERE b.column_name='SAPEQUIPID'
        )and a.table_name NOT LIKE 'ZPGEVW%' and a.table_name NOT LIKE 'ZZ_MV%';

    sqlstmt    VARCHAR2(2000);
    reg_num   NUMBER;
    count_num NUMBER;
    rowcnt number;
    rowcnt2 number;
    equipid varchar2(20);
  BEGIN
    dbms_output.put_line('GUID [ '||guid||' ]');
    FOR tables IN ALL_TABLES_WE_CARE_ABOUT
    LOOP
--      dbms_output.put_line('TABLE [ '||tables.table_name||' ]');
      sqlstmt := 'select count(*) from zz_mv_'||tables.table_name||' where globalid='''||guid||'''  ';
      execute immediate sqlstmt into rowcnt;
      if rowcnt>0 then
       dbms_output.put_line('BINGO!!! [ '||tables.table_name||' ]');
        sqlstmt := 'select sapequipid from zz_mv_'||tables.table_name||' where globalid='''||guid||'''  ';
        dbms_output.put_line(sqlstmt);
        execute immediate sqlstmt into equipid;
        dbms_output.put_line('SAPEQUIPID [ '||equipid||' ]');
        return 'EDGIS.'||tables.table_name;
      END IF;

    END LOOP;
    return null;
  END;
END GET_FC_NAME;
/

CREATE OR REPLACE PROCEDURE process_resync_guids_csv (
   guids_list IN VARCHAR2)
IS
  CURSOR guidscursor is 
select regexp_substr(guids_list,'[^,]+', 1, level) as guid_val from dual
connect by regexp_substr(guids_list, '[^,]+', 1, level) is not null;
   fc VARCHAR2(200);
   guid varchar2(40);
   assetid varchar2(40);
    rowcnt number;
BEGIN
	FOR guid_row in guidscursor LOOP
    select guid_row.guid_val into guid from dual;
--    check to see if feature exists
    select get_fc_name(guid) into fc from dual;
    if fc is not null then
      dbms_output.put_line('FC [ '||fc||' ]');
      select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
      if rowcnt<1 then
        dbms_output.put_line('INSERTING [ '||guid||']');
        insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
        values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
        guid,fc,SYSDATE,'P1PC');
      end if;
    end if;
   END LOOP;
END;

create or replace PROCEDURE process_resync_guid (
   guid IN VARCHAR2, fc VARCHAR2)
IS
    rowcnt number;
BEGIN
  dbms_output.put_line('FC [ '||fc||' ]');
  select count(*) into rowcnt from EDGIS.PGE_GISSAP_REPROCESSASSETSYNC where assetid=guid;
  if rowcnt<1 then
    dbms_output.put_line('INSERTING [ '||guid||']');
    insert into EDGIS.PGE_GISSAP_REPROCESSASSETSYNC(OBJECTID,ASSETID,FEATURECLASSNAME,DATECREATED,CREATEDUSER)
    values((select SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS',(select registration_id from sde.table_registry where table_name='PGE_GISSAP_REPROCESSASSETSYNC')) from dual),
    guid,fc,SYSDATE,'P1PC');
  end if;
END;

GRANT EXECUTE ON GET_FC_NAME TO  GIS_I, GISINTERFACE,SDE_EDITOR;
GRANT EXECUTE ON process_resync_guids_csv TO  GIS_I, GISINTERFACE,SDE_EDITOR;
GRANT EXECUTE ON process_resync_guid TO  GIS_I, GISINTERFACE,SDE_EDITOR;
GRANT EXECUTE ON SHOW_GUID_EQUIPID TO  GIS_I, GISINTERFACE,SDE_EDITOR;
GRANT ALL ON EDGIS.PGE_GISSAP_REPROCESSASSETSYNC TO GIS_I, GISINTERFACE,SDE_EDITOR;

create or replace PROCEDURE recalc_localofficeid(globalidIn IN VARCHAR2,fcnameIn varchar2)
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

  select substr(fcname, - instr(reverse(fcname), '.') + 1) into table_name_no_owner from dual;
  select registration_id into regid from sde.table_registry where OWNER||'.'||table_name= ''||fcname||'';
  atable := 'EDGIS.A'|| regid;
  dbms_output.put_line('atable [ '||atable||' ]');
  
  SQLSTMT :=  'select objectid, '||field_mapnumber||', localofficeid, shape from edgis.zz_mv_'||table_name_no_owner||' where globalid='''||globalidIn||'''';
  dbms_output.put_line('sqlstmt [ '||sqlstmt||' ]');
  execute immediate sqlstmt into objectid, distmap, localofficeid, fshape;

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
      dbms_output.put_line('Logical NOMAP, RETURNING (not setting distmap!');
      return;
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

create or replace
PROCEDURE recalc_mapnumber(globalidIn IN VARCHAR2,fcnameIn varchar2)
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
    field_mapnumber VARCHAR2(20) := 'DISTMAP';
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

  select substr(fcname, - instr(reverse(fcname), '.') + 1) into table_name_no_owner from dual;
  select registration_id into regid from sde.table_registry where OWNER||'.'||table_name= ''||fcname||'';
  atable := 'EDGIS.A'|| regid;
  dbms_output.put_line('atable [ '||atable||' ]');
  
  SQLSTMT :=  'select objectid, '||field_mapnumber||', localofficeid, shape from edgis.zz_mv_'||table_name_no_owner||' where globalid='''||globalidIn||'''';
  dbms_output.put_line('sqlstmt [ '||sqlstmt||' ]');
  execute immediate sqlstmt into objectid, distmap, localofficeid, fshape;

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

  if (calculated_mapno is not null and distmap is null) or
     (calculated_mapno is not null and distmap <> calculated_mapno) then
    dbms_output.put_line('UPDATING DISTMAP/MAPNUMBER TO [ '||calculated_mapno||' ]');
    sqlstmt := 'update '||fcname||' set '||field_mapnumber||'='''||calculated_mapno||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    sqlstmt := 'update '||atable||' set '||field_mapnumber||'='''||calculated_mapno||''',datemodified=sysdate where globalid='''||globalidIn||'''';
    execute immediate sqlstmt;
    ADD_GUID_TO_RESYNC_TABLE(globalIdIn, fcname);
  else
    dbms_output.put_line('NO UPDATE');
  end if;
  
END;
