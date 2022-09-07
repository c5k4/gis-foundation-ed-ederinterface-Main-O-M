Prompt drop Package Body ST_TYPE_EXPORT;
DROP PACKAGE BODY SDE.ST_TYPE_EXPORT
/

Prompt Package Body ST_TYPE_EXPORT;
--
-- ST_TYPE_EXPORT  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_type_export 
/***********************************************************************
*
*N  {st_type_util.spb}  --  Implementation for globally useful functions
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements globally useful functions.  
*   It should be compiled by the ST_GEOEMTRY type DBA user; 
*   security is by user name.   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2008 ESRI
*
*   TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
*   Unpublished material - all rights reserved under the
*   Copyright Laws of the United States.
*
*   For additional information, contact:
*   Environmental Systems Research Institute, Inc.
*   Attn: Contracts Dept
*   380 New York Street
*   Redlands, California, USA 92373
*
*   email: contracts@esri.com
*   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Kevin Watt             04/22/05               Original coding.
*    Thomas Brown           01/18/08              
*E
***********************************************************************/
IS

  Iterate     number := 0;

Function export_info  (idxschema  IN varchar2,
                       idxname    IN varchar2,
                       spx_info   SDE.spx_util.spx_record_t,
                       spref      SDE.spx_util.spatial_ref_record_t,
                       expversion IN varchar2,
                       newblock   Out pls_integer) 
Return varchar2
  IS

  Cursor c_idx_id (owner_wanted IN varchar2, index_wanted IN varchar2) IS
    SELECT table_name, column_name, index_id, index_name
    FROM   SDE.st_geometry_index
    WHERE  owner = owner_wanted AND index_name = index_wanted;

  CURSOR c_idxparam (owner_wanted IN varchar2, index_wanted IN varchar2) IS
    SELECT parameters
    FROM all_indexes
    WHERE owner = owner_wanted AND index_name = index_wanted;

  CURSOR c_tbsname (owner_wanted IN varchar2, index_wanted IN varchar2) IS
    SELECT tablespace_name
    FROM all_indexes
    WHERE owner = owner_wanted AND index_name = index_wanted;

  tab_name     varchar2(30);
  col_name     varchar2(30);
  idx_name     varchar2(30);
  tbs_name     varchar2(30);
  geom_id      number;
  stmt         varchar2(4000);
  params       CLOB;
  old_param CLOB;
  temp_param CLOB;
  ALPHABETS VARCHAR2(250) := 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
  this_char VARCHAR2(1);
  pos NUMBER := 0;
  st_grid_pos NUMBER := 0;
  other_char_pos NUMBER := 0; 
  first_char_pos NUMBER := 0;

  Begin

    CASE 
      WHEN Iterate = 0 THEN
        Iterate := Iterate + 1;
        newblock := 1;
        Return 'SDE.st_type_export.checkversion(''9.2'');';
      WHEN Iterate = 1 THEN
        Iterate := Iterate + 1;
        newblock := 2;

        Open c_idx_id (idxschema, idxname);
        Fetch c_idx_id INTO tab_name, col_name, geom_id, idx_name;
        If c_idx_id%NOTFOUND THEN
          Close c_idx_id;
          Iterate := 0;
          Return '';
        End If;
        Close c_idx_id;

        Return 'SDE.st_type_export.validate_sidx('''||idxschema||''','''||tab_name||''','||geom_id||');';
      WHEN Iterate = 2 THEN
        Iterate := Iterate + 1;
        newblock := 3;

        -- Fetch the properties of the index.
        Open c_idx_id (idxschema, idxname);
        Fetch c_idx_id INTO tab_name, col_name, geom_id, idx_name;
        If c_idx_id%NOTFOUND THEN
          Close c_idx_id;
          Iterate := 0;
          Return '';
        End If;
        Close c_idx_id;

        -- Fetch the parameters used to create the domain index.
        Open c_idxparam (idxschema, idxname);
        Fetch c_idxparam INTO params;
        If c_idxparam%NOTFOUND THEN
          Close c_idxparam;
          Iterate := 0;
          Return '';
        End If;
        Close c_idxparam;

        -- Fetch the tablespace name where the index exists.
        Open c_tbsname (idxschema, 'S'||geom_id||'$_IX1');
        Fetch c_tbsname INTO tbs_name;
        If c_tbsname%NOTFOUND THEN
          Close c_tbsname;
          Iterate := 0;
          Return '';
        End If;
        Close c_tbsname;

        stmt := 'SDE.st_type_export.validate_spref('''||idxschema||''','''||spref.sr_name||''','||spref.srid||','
                ||spref.x_offset||','||spref.y_offset||','||spref.xyunits||','||spref.z_offset||','
                ||spref.z_scale||','||spref.m_offset||','||spref.m_scale||','||spref.min_x||','
                ||spref.max_x||','||spref.min_y||','||spref.max_y||',';

        -- Must account for NULL values in the spatial reference and pass NULL to the procedure.
        IF spref.min_z IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.min_z||',';
        END IF;

        IF spref.max_z IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.max_z||',';
        END IF;

        IF spref.min_m IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.min_m||',';
        END IF;

        IF spref.max_m IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.max_m||',';
        END IF;

        IF spref.cs_id IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.cs_id||',';
        END IF;

        stmt := stmt || ''''||spref.cs_name||''','''||spref.cs_type||''',';

        IF spref.organization IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt || ''''||spref.organization||''',';
        END IF;

        IF spref.org_coordsys_id IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt ||spref.org_coordsys_id||',';
        END IF;
                
        stmt := stmt || ''''||spref.definition||''',';

        IF spref.description IS NULL THEN
          stmt := stmt ||'NULL,';
         ELSE
          stmt := stmt || ''''||spref.description||''',';
        END IF;
               
        IF params IS NOT NULL THEN       
        
          old_param := params;
          temp_param := UPPER(params);
          st_grid_pos := INSTR(temp_param,'ST_GRIDS');
          temp_param := SUBSTR(temp_param,st_grid_pos+8);
          FOR i in 1..26 LOOP
            pos := pos + 1;
            this_char := SUBSTR(ALPHABETS,pos,1); 
            other_char_pos := INSTR(temp_param,this_char);
            IF first_char_pos = 0 OR 
              (other_char_pos > 0 AND
               first_char_pos > other_char_pos) THEN
              first_char_pos := other_char_pos;      
            END IF;
          END LOOP;
  
          params := SUBSTR(old_param,1,st_grid_pos-1);
          
          params :=  params || 'ST_GRIDS=' || spx_info.grid.grid1;
        
          IF spx_info.grid.grid2 > 0 THEN
            params := params || ':' || spx_info.grid.grid2;   
            IF spx_info.grid.grid3 > 0 THEN
              params := params || ':' || spx_info.grid.grid3;
            END IF;
          END IF; 
        
          IF first_char_pos > 0 THEN
            params := params ||' '|| SUBSTR(temp_param,first_char_pos);
          END IF;  

        END IF;  
                      
        stmt := stmt || ''''||tab_name||''','''||col_name||''','''||idx_name||''','''||params||''','''||tbs_name||''');';

        Return stmt;

       ELSE
        Iterate := 0;
        Return '';
      END CASE;
    
End export_info;

Procedure checkversion (version IN varchar2)
  IS

  Begin

    If version != '9.2' THEN
      raise_application_error (SDE.st_type_util.spx_invalid_release,'st_spatial_index domain index release '''||version||
                               ' is not compatible with the current release.');
    End If;

End checkversion;

Procedure validate_sidx (table_owner IN nvarchar2, table_name IN nvarchar2, geom_id IN number)
  IS

    CURSOR c_sidx (owner_wanted IN nvarchar2, geom_id_wanted IN number) IS
      SELECT table_name
      FROM SDE.st_geometry_columns
      WHERE owner = owner_wanted AND geom_id = geom_id_wanted;

    CURSOR c_tabcnt (table_wanted IN varchar2) IS
      SELECT COUNT(*)
      FROM user_tables
      WHERE table_name = table_wanted;

    st_tab_name  VARCHAR2(30) DEFAULT NULL;
    tab_cnt      INTEGER;
    pos          INTEGER;

  Begin
    
    -- Check to see if the geom_id for the user performing the import already exists in the st_geometry_columns table.
    -- If it exists, we'll fetch the table_name for the current geom_id being imported. If the table_name
    -- is not the same as the table being imported, then we have a geom_id collision (and if the existing
    -- geom_id table has a spatial index, the import will have a collision creating the domain index table), 
    -- if the table_name is the same then we just need to verify the SRIDs are identical.

    Open c_sidx (table_owner, geom_id);
    Fetch c_sidx INTO st_tab_name;
    Close c_sidx;
 
    IF st_tab_name IS NULL THEN
     
      Open c_tabcnt ('S'||geom_id||'_IDX$');
      Fetch c_tabcnt INTO tab_cnt;
      Close c_tabcnt;

      IF tab_cnt = 1 THEN
        -- Get the metadata for the domain index table just created, before its dropped. The storage clause
        -- will be used as an argument in the parameters clause when created.
        domain_storage := dbms_metadata.get_ddl('TABLE','S'||geom_id||'_IDX$');

        -- Remove carriage rerutns
        domain_storage := REPLACE(domain_storage,chr(10),' ');

        -- Remove redundant spaces
        pos := 1;
        WHILE pos > 0 LOOP
          pos := INSTR(domain_storage, '  ');
          IF pos > 0 THEN          
            domain_storage := SUBSTR(domain_storage,1,pos - 1) || SUBSTR(domain_storage,pos + 1);
          END IF;
        END LOOP;

        domain_dropped := TRUE;
        EXECUTE IMMEDIATE 'DROP TABLE s'||geom_id||'_idx$';
       ELSE
        domain_storage := ' ';
        domain_dropped := FALSE;
      END IF;

     ELSE
      domain_storage := ' ';
      domain_dropped := FALSE;
    END IF;

  End validate_sidx;
 
Procedure validate_spref (table_owner IN nvarchar2, sr_name IN nvarchar2, srid IN number, x_offset IN float, y_offset IN float, xyunits IN float,
                          z_offset IN float, z_scale IN float, m_offset IN float, m_scale IN float, min_x IN float,
                          max_x IN float, min_y IN float, max_y IN float, min_z IN float, max_z IN float, min_m IN float,
                          max_m IN float, cs_id IN number, cs_name IN nvarchar2, cs_type IN nvarchar2, organization IN nvarchar2,
                          org_coordsys_id IN number, definition IN varchar2, description IN nvarchar2, 
                          table_name IN varchar2, column_name IN varchar2, index_name IN varchar2, param IN clob, tbs_name IN varchar2)
  IS

  CURSOR c_objcnt (object_wanted IN varchar2) IS
    SELECT COUNT(*) 
    FROM user_objects
    WHERE object_name = object_wanted;

  CURSOR c_tbscnt (tablespace_wanted IN varchar2) IS
    SELECT COUNT(*) 
    FROM user_tablespaces
    WHERE tablespace_name = tablespace_wanted;

  CURSOR c_geom (owner_wanted IN nvarchar2, table_wanted IN nvarchar2, column_wanted IN nvarchar2) IS
    SELECT COUNT(*)
    FROM SDE.st_geometry_columns
    WHERE owner = owner_wanted AND table_name = table_wanted AND column_name = column_wanted;

  obj_cnt               INTEGER;
  tbs_cnt               INTEGER;
  geom_cnt              INTEGER;
  spref                 SDE.spx_util.spatial_ref_record_t;
  local_srid            NUMBER;
  local_param           CLOB;
  idx_storage           CLOB;
  pos                   INTEGER;
  pos2                  INTEGER;
  stmt                  VARCHAR2(4000);
  tab_owner             VARCHAR2(30);
  tab_name              VARCHAR2(30);

  Begin
     
    spref.sr_name := sr_name;
    spref.srid := srid;
    spref.x_offset := x_offset;
    spref.y_offset := y_offset;
    spref.xyunits := xyunits;
    spref.z_offset := z_offset;
    spref.z_scale := z_scale;
    spref.m_offset := m_offset;
    spref.m_scale := m_scale;
    spref.min_x := min_x;
    spref.max_x := max_x;
    spref.min_y := min_y;
    spref.max_y := max_y;
    spref.min_z := min_z;
    spref.max_z := max_z;
    spref.min_m := min_m;
    spref.max_m := max_m;
    spref.cs_id := cs_id;
    spref.cs_name := cs_name;
    spref.cs_type := cs_type;
    spref.organization := organization;
    spref.org_coordsys_id := org_coordsys_id;
    spref.definition := definition;
    spref.description := description;

    local_srid := SDE.st_spref_util.exists_spref (spref);

    local_param := LTRIM(UPPER(REPLACE(param, chr(10),' ')));
    local_param := RTRIM(local_param);

    -- Remove redundant spaces
    pos := 1;
    WHILE pos > 0 LOOP
      pos := INSTR(local_param, '  ');
      IF pos > 0 THEN          
        local_param := SUBSTR(local_param,1,pos - 1) || SUBSTR(local_param,pos + 1);
      END IF;
    END LOOP;

    tab_owner := USER;
    tab_name := table_name;

        
    -- If the local_srid returned from comparing the importing SRID equals 0 then the SRID
    -- does not currently exist, or if a SRID value is returned (meaning all properties matched)
    -- but the SRID values do not match, we must create the spatial reference and attempt to
    -- create the domain index with the new SRID value

    If local_srid = 0 THEN
      SDE.st_spref_util.insert_spref (spref, local_srid);
    End If;
    
    IF local_srid <> spref.srid THEN

      -- Knowing the SRID is different then the importing SRID value, we must update all rows 
      -- in the table setting the value for the new SRID.

      EXECUTE IMMEDIATE 'UPDATE '||tab_owner||'.'||tab_name||' t SET t.'||column_name||'.srid = '||local_srid||' WHERE t.'||column_name||' IS NOT NULL' ;

      -- Since we know the table did not exist, we generate an entry in st_geometry_columns
      
      SDE.st_geom_cols_util.insert_gcol (tab_owner, table_name, column_name, 'ST_GEOMETRY', local_srid);

      COMMIT;

      -- Next we check if the importing index_name already exists in the schema performing the import.
      -- As long as it doesn't exist we'll proceed and create the domain index.

      Open c_objcnt (UPPER(index_name));
      Fetch c_objcnt INTO obj_cnt;
      Close c_objcnt;

      IF obj_cnt = 0 THEN

        -- Replace the existing SRID with the new SRID value
        pos := INSTR(local_param,'ST_SRID') + 7;
        pos2 := INSTR(local_param,'ST_COMMIT_ROWS');
        local_param := SUBSTR(local_param,1,pos) || ' = ' || local_srid ||' '||SUBSTR(local_param,pos2);

        -- Remove the tablespace clause if present
        IF INSTR(local_param,'TABLESPACE') > 0 THEN
          pos := INSTR(local_param,'TABLESPACE') + 11;
          IF pos > 0 THEN
            pos2 := INSTR(SUBSTR(local_param,pos),' ');
            -- Need to account if the tablespace name is the last token.
            IF pos2 = 0 THEN
              local_param := SUBSTR(local_param,1,pos - 12);
             ELSE
              local_param := SUBSTR(local_param,1,pos - 12) || SUBSTR(local_param,(pos + pos2) - 1);
            END IF;
          END IF;
        END IF;


        -- Correct the numeric format of the grid sizes
        -- in the index parameters, if necessary.
        SDE.spx_util.parse_params2(local_param);

        -- Start constructing the CREATE INDEX statement

        stmt := 'CREATE INDEX '||tab_owner||'.'||index_name||' ON '||tab_owner||'.'||tab_name||' ('||column_name||') ' 
                || 'INDEXTYPE IS SDE.st_spatial_index PARAMETERS ('''||local_param||' ';

        Open c_tbscnt (UPPER(tbs_name));
        Fetch c_tbscnt INTO tbs_cnt;
        Close c_tbscnt;

        -- If the tablespace from the export file exists in the import database, append it to the statement
        IF obj_cnt = 1 THEN
          stmt := stmt || 'TABLESPACE '||tbs_name||' ';
        END IF;

        -- Append the storage clause from the metadata of the imported domain index which was dropped
        IF domain_dropped = TRUE THEN
          pos := INSTR(domain_storage,'STORAGE');
          domain_storage := SUBSTR(domain_storage,pos);
          pos := INSTR(domain_storage,')');
          idx_storage := SUBSTR(domain_storage,1,pos);
          stmt := stmt || ''||idx_storage||''')';
         ELSE
          stmt := stmt || ''')';
        END IF;

        EXECUTE IMMEDIATE stmt;

      END IF;

     ELSE -- The local_srid equals spref.srid.

      -- Since the import SRID matches the existing SRID value, we check if the index name 
      -- exists, if not, we'll create the index if the input tablespace does NOT exist by removing
      -- the tablespace argument in the parameters clause.
      -- This avoids raising the error tablespace doesn't exist, instead the import
      -- will error with index already exists (but at least we have an index).

      Open c_objcnt (UPPER(index_name));
      Fetch c_objcnt INTO obj_cnt;
      Close c_objcnt;

      Open c_tbscnt (UPPER(tbs_name));
      Fetch c_tbscnt INTO tbs_cnt;
      Close c_tbscnt;

      OPEN c_geom (USER, UPPER(table_name), UPPER(column_name));
      FETCH c_geom INTO geom_cnt;
      CLOSE c_geom;

      IF geom_cnt = 0 THEN
        SDE.st_geom_cols_util.insert_gcol (tab_owner, table_name, column_name, 'ST_GEOMETRY', local_srid);
        COMMIT;
      END IF;

      IF obj_cnt = 0 THEN 

         IF tbs_cnt = 0 THEN
        -- Remove the tablespace clause if present
        IF INSTR(local_param,'TABLESPACE') > 0 THEN
          pos := INSTR(local_param,'TABLESPACE') + 11;
          IF pos > 0 THEN
            pos2 := INSTR(SUBSTR(local_param,pos),' ');
            -- Need to account if the tablespace name is the last token.
            IF pos2 = 0 THEN
              local_param := SUBSTR(local_param,1,pos - 12);
             ELSE
              local_param := SUBSTR(local_param,1,pos - 12) || SUBSTR(local_param,(pos + pos2) - 1);
            END IF;
          END IF;
        END IF;
         END IF;
         

        -- Correct the numeric format of the grid sizes
        -- in the index parameters, if necessary.
        SDE.spx_util.parse_params2(local_param);

        stmt := 'CREATE INDEX '||tab_owner||'.'||index_name||' ON '||tab_owner||'.'||table_name||' ('||column_name||') ' 
                || 'INDEXTYPE IS SDE.st_spatial_index PARAMETERS ('''||local_param||' ';

        IF domain_dropped = TRUE THEN
          pos := INSTR(domain_storage,'STORAGE');
          domain_storage := SUBSTR(domain_storage,pos);
          pos := INSTR(domain_storage,')');
          idx_storage := SUBSTR(domain_storage,1,pos);
          stmt := stmt || ''||idx_storage||''')';
         ELSE
          stmt := stmt || ''')';
        END IF;

        EXECUTE IMMEDIATE stmt;

      END IF;
    END IF;

    domain_dropped := FALSE;
    domain_storage := ' ';
    
  End validate_spref;

End st_type_export;

/


Prompt Grants on PACKAGE ST_TYPE_EXPORT TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_TYPE_EXPORT TO PUBLIC WITH GRANT OPTION
/
