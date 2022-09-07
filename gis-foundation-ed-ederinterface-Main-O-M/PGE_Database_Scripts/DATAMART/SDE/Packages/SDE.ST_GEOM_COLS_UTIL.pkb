Prompt drop Package Body ST_GEOM_COLS_UTIL;
DROP PACKAGE BODY SDE.ST_GEOM_COLS_UTIL
/

Prompt Package Body ST_GEOM_COLS_UTIL;
--
-- ST_GEOM_COLS_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_geom_cols_util 
/******************************************************************************
   NAME:       st_geom_cols_util
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        4/21/2005             1.     Created this package body.
******************************************************************************/ 
IS
  -- Local / Glaobal Procedures and Functions
  
     /* Package Globals. */

   g_type_dba              boolean NOT NULL DEFAULT False;
   g_current_user          nvarchar2(32);

  Procedure l_user_can_modify (owner_i IN gc_owner_t)
  /***********************************************************************
  *
  *N  {L_user_can_modify}  --  Can current user modify type?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the type metadata exists and is
  *   modifiable by the current user (who must be owner or ISO Admin).
  *
  *   It's likely this procedure will need to check the DBMS permissions
  *  on the object instead of check the owner. If the owner has granted 
  *  DML permissions, the modification should execute regardless of the 
  *  owner. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_i  <IN>  ==  (gc_owner_t) Owner test
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20001                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *   Kevin Watt         04/22/05          Original coding.
  *E
  ***********************************************************************/
   IS

   Begin

     If NOT g_type_dba THEN
       If g_current_user != owner_i THEN
         raise_application_error (SDE.st_type_util.st_no_permissions,
                                  owner_i ||' not owner of ST_GEOMETRY ');
       End If;
     End If;

   End l_user_can_modify;

  -- Public Procedures and Functions 

  Procedure insert_gcol   (owner           IN   gc_owner_t,
                           table_name      IN   gc_table_t,
                           column_name     IN   gc_column_t,
                           geom_type       IN   varchar2,
                           srid            IN   gc_srid_t)
/***********************************************************************
  *
  *N  {insert_gcol}  --  Insert into st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Inserts into st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
IS
  properties     number;
  pos            integer := 0;
  valid_type     boolean := False;
Begin
  Begin
    properties :=0;
    FOR i IN 1..7
    Loop
      pos := instr(upper(geom_type),SDE.st_geom_cols_util.gtype(i),1);
      If pos > 0 THEN
        valid_type := True;
      End If;
    End Loop;

    If valid_type = False THEN
      raise_application_error(SDE.st_type_util.st_geometry_invalid_type,
                              'Geometry Type '||upper(geom_type)||' must be a valid ST_GEOMETRY type.');
    End If;

    INSERT INTO SDE.st_geometry_columns
               (owner,
                table_name,
                column_name,
                geometry_type,
                properties,
                srid)
    VALUES (upper(replace(owner,'"')),
            upper(table_name),
            upper(column_name),
            upper(geom_type),
            properties,
            srid);

  Exception

    When dup_val_on_index THEN

      raise_application_error(SDE.st_type_util.st_table_exists,
                              'Object '||to_char(owner)||'.'||to_char(table_name)||'.'||to_char(column_name)||
                              ' exists in ST_GEOMETRY_COLUMNS.');
    NULL;
  End;

End insert_gcol;

  Procedure insert_gcol   (owner           IN   gc_owner_t,
                           table_name      IN   gc_table_t,
                           column_name     IN   gc_column_t,
                           geom_type       IN   varchar2,
                           properties      IN   gc_properties_t,
                           srid            IN   gc_srid_t)
/***********************************************************************
  *
  *N  {insert_gcol}  --  Insert into st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Inserts into st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal         06/2011          Original coding.
  *E
  ***********************************************************************/
IS
  pos            integer := 0;
  valid_type     boolean := False;
Begin

  Begin

    FOR i IN 1..7
    Loop
      pos := instr(upper(geom_type),SDE.st_geom_cols_util.gtype(i),1);
      If pos > 0 THEN
        valid_type := True;
      End If;
    End Loop;

    If valid_type = False THEN
      raise_application_error(SDE.st_type_util.st_geometry_invalid_type,
                              'Geometry Type '||upper(geom_type)||' must be a valid ST_GEOMETRY type.');
    End If;

    INSERT INTO SDE.st_geometry_columns
               (owner,
                table_name,
                column_name,
                geometry_type,
                properties,
                srid)
    VALUES (upper(replace(owner,'"')),
            upper(table_name),
            upper(column_name),
            upper(geom_type),
            properties,
            srid);

  Exception
    When dup_val_on_index THEN
      raise_application_error(SDE.st_type_util.st_table_exists,
                              'Object '||to_char(owner)||'.'||to_char(table_name)||'.'||to_char(column_name)||
                              ' exists in ST_GEOMETRY_COLUMNS.');
  End;

End insert_gcol;
  
  Procedure delete_gcol   (owner           IN   gc_owner_t,
                           table_name      IN   gc_table_t,
                           column_name     IN   gc_column_t)
/***********************************************************************
  *
  *N  {delete_gcol}  --  Delete record from st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Deletes from the  st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
IS
  stmt1     varchar2(256);

Begin
    -- Check that user can delete entry 
  l_user_can_modify (owner);

  stmt1 := 'DELETE FROM SDE.st_geometry_columns '||
           'WHERE owner = UPPER('''||owner||''') AND table_name = UPPER('''||table_name||
            ''')AND column_name = UPPER('''||column_name||''')';

  EXECUTE IMMEDIATE stmt1;

  If Sql%NOTFOUND THEN
    raise_application_error (SDE.st_type_util.st_table_noexist,
                             'ST_GEOMETRY_COLUMNS entry not found. ');
  End If;

End delete_gcol;

  Procedure update_gcol   (owner           IN   gc_owner_t,
                           table_name      IN   gc_table_t,
                           column_name     IN   gc_column_t,
                           properties      IN   number)
/***********************************************************************
  *
  *N  {update_gcol}  --  Update record from st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Deletes from the  st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
IS

Begin

    -- Check that user can delete entry 
  l_user_can_modify (owner);

  UPDATE SDE.st_geometry_columns l
   SET l.properties = properties 
   WHERE l.owner = upper(replace(owner,'"')) 
   AND l.table_name = upper(table_name) AND
       l.column_name = upper(column_name);

  If Sql%NOTFOUND THEN
    raise_application_error (SDE.st_type_util.st_table_noexist,
                             'ST_GEOMETRY_COLUMNS entry not found. ');
  End If;

End update_gcol;

  Procedure update_gcol_table   (owner_name IN   gc_owner_t,
                                 old_table  IN   gc_table_t,
                                 colname    IN   gc_column_t,
                                 new_table  IN   gc_table_t)
/***********************************************************************
  *
  *N  {update_gcol}  --  Update record from st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Deletes from the  st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *
  ***********************************************************************/
IS
  CURSOR c_geom_idx (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2, column_wanted IN VARCHAR2) IS
        SELECT *
        FROM   SDE.st_geometry_index
        WHERE  owner = owner_wanted AND table_name = table_wanted AND column_name = column_wanted;

  CURSOR c_geom_col (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2, column_wanted IN VARCHAR2) IS
        SELECT *
        FROM   SDE.st_geometry_columns
        WHERE  owner = owner_wanted AND table_name = table_wanted AND column_name = column_wanted;

  stmt1              varchar2(256);
  gindex_rec         gindex_record_t;
  gcol_rec           gc_record_t;

Begin

    -- Check that user can delete entry 

  l_user_can_modify (owner_name);

  LOCK TABLE SDE.st_geometry_columns IN EXCLUSIVE MODE;

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  OPEN c_geom_col (replace(owner_name,'"'), old_table, colname);
  FETCH c_geom_col INTO gcol_rec;
  IF c_geom_col%NOTFOUND THEN
    CLOSE c_geom_col;
  ELSE
    CLOSE c_geom_col;

    OPEN c_geom_idx (replace(owner_name,'"'), old_table, colname);
    FETCH c_geom_idx INTO gindex_rec;
    IF c_geom_idx%NOTFOUND THEN
      CLOSE c_geom_idx;

      UPDATE SDE.st_geometry_columns  
      SET table_name = upper(new_table) 
       WHERE owner = upper(replace(owner_name,'"'))
        AND table_name = upper(old_table) 
        AND column_name = upper(colname);

      COMMIT;

    ELSE
      CLOSE c_geom_idx;

      DELETE FROM SDE.st_geometry_index 
      WHERE owner = upper(replace(owner_name,'"'))
        AND table_name = upper(old_table) 
        AND column_name = upper(colname);

      UPDATE SDE.st_geometry_columns  
      SET table_name = upper(new_table) 
       WHERE owner = upper(replace(owner_name,'"'))
        AND table_name = upper(old_table) 
        AND column_name = upper(colname);

      gindex_rec.table_name := upper(new_table);

      INSERT INTO SDE.st_geometry_index VALUES gindex_rec;

      COMMIT;

    END IF;

  END IF; 

End update_gcol_table;

  Procedure update_gcol_properties   (owner_name    IN   gc_owner_t,
                                      old_table     IN   gc_table_t,
                                      colname       IN   gc_column_t,
                                      properties_in IN   number)
/***********************************************************************
  *
  *N  {update_gcol}  --  Update ST_Geometry_Columns Properties field
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *            The procedure updates the ST_Geometry_Columns Properties
  *  field. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *
  ***********************************************************************/
IS
  stmt1              varchar2(256);
  gindex_rec         gindex_record_t;
  gcol_rec           gc_record_t;

Begin

    -- Check that user can delete entry 

  l_user_can_modify (owner_name);

  LOCK TABLE SDE.st_geometry_columns IN EXCLUSIVE MODE;

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_geometry_columns  
    SET properties = properties_in 
  WHERE owner = upper(replace(owner_name,'"'))
   AND table_name = upper(old_table) 
   AND column_name = upper(colname);

  COMMIT;

End update_gcol_properties;
  
   Procedure delete_st_geom_metadata (owner        IN gc_owner_t,
                                      table_name   IN gc_table_t,
                                      column_name  IN gc_column_t)
   
  /***********************************************************************
  *
  *N  {delete_st_geom_metadatal}  -- Delete st_geom metadata.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Deletes entries from the ST_GEOMETRY_COLUMNS
  *  and ST_GEOMETRY_INDEX tables.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
IS
  stmt1       varchar2(256);
  rc          number;
  spx_rec     SDE.st_type_util.spx_record_t;
Begin

    -- Check that user can delete entry 

  l_user_can_modify (owner);

  spx_rec.index_name := NULL;
  rc := SDE.st_type_util.get_geom_index_rec(owner,table_name,column_name,spx_rec);

  if(spx_rec.index_name IS NOT NULL) then
    dbms_stats.delete_index_stats(owner,spx_rec.index_name);
  end if;

  stmt1 := 'DELETE FROM SDE.st_geometry_index '||
           'WHERE owner = UPPER('''||owner||''') AND table_name = UPPER('''||table_name||
           ''')AND column_name = UPPER('''||column_name||''')';

  EXECUTE IMMEDIATE stmt1;

  stmt1 := 'DELETE FROM SDE.st_geometry_columns '||
           'WHERE owner = UPPER('''||owner||''') AND table_name = UPPER('''||table_name||
           ''')AND column_name = UPPER('''||column_name||''')';

  EXECUTE IMMEDIATE stmt1;

  stmt1 := 'DROP INDEX '||owner||'.'||spx_rec.index_name;
  EXECUTE IMMEDIATE stmt1;

Exception
  When Others Then null;

End delete_st_geom_metadata;
 
  Function select_gcol   (owner           IN   gc_owner_t,
                          table_name      IN   gc_table_t,
                          column_name     IN   gc_column_t,
                          geom_type       IN Out varchar2,
                          properties      IN Out number,
                          srid            IN Out gc_srid_t)
/***********************************************************************
  *
  *N  {select_gcol}  --  Select record from st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Selects from the  st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
  Return number
IS

  Cursor c_select_gcol (owner_in IN gc_owner_t,table_in IN gc_table_t,
                       column_in IN gc_column_t) IS
    SELECT geometry_type,properties,srid 
    FROM SDE.st_geometry_columns  
    WHERE owner = upper(replace(owner_in,'"')) 
    AND table_name = upper(table_in) 
    AND column_name = upper(column_in);

Begin

  Open c_select_gcol (owner,table_name,column_name);
  Fetch c_select_gcol INTO geom_type,properties,srid;
  If c_select_gcol%NOTFOUND THEN
    Close c_select_gcol ;
    Return SDE.st_type_util.st_table_noexist;
  End If;

  Close c_select_gcol ;
  Return(se_success);
  
  End select_gcol;

  Procedure update_gcol_srid   (owner_in           IN   gc_owner_t,
                                table_name_in      IN   gc_table_t,
                                column_name_in     IN   gc_column_t,
                                srid_in            IN   gc_srid_t)
/***********************************************************************
  *
  *N  {update_gcol}  --  Update record from st_geometry_columns table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *                Deletes from the  st_geometry_columns table
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          04/20/05          Original coding.
  *E
  ***********************************************************************/
IS

Begin

    -- Check that user can delete entry 
  l_user_can_modify (owner_in);

  UPDATE SDE.st_geometry_columns l
   SET l.srid = srid_in 
   WHERE l.owner = upper(replace(owner_in,'"')) 
   AND l.table_name = upper(table_name_in) AND
       l.column_name = upper(column_name_in);

  If Sql%NOTFOUND THEN
    raise_application_error (SDE.st_type_util.st_table_noexist,
                             'ST_GEOMETRY_COLUMNS entry not found. ');
  End If;

End update_gcol_srid;


Begin
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   g_current_user := SDE.st_type_util.type_user;
   g_type_dba := g_current_user = SDE.st_type_util.c_type_dba;
   
End st_geom_cols_util;


/


Prompt Grants on PACKAGE ST_GEOM_COLS_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_GEOM_COLS_UTIL TO PUBLIC WITH GRANT OPTION
/
