Prompt drop Package Body SPX_UTIL;
DROP PACKAGE BODY SDE.SPX_UTIL
/

Prompt Package Body SPX_UTIL;
--
-- SPX_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.spx_util
/***********************************************************************
*
*n  {spx_util.spb}  --  utility procs/funct for st_spatial_index. 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*   this pl/sql package specification defines procedures and functions
*   to perform utility operations to support the st_Spatial_Index domain 
*   index and st_Geometry type.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  legalese:
*
*   copyright 1992-2004 esri
*
*   trade secrets: esri proprietary and confidential
*   unpublished material - all rights reserved under the
*   copyright laws of the united states.
*
*   for additional information, contact:
*   environmental systems research institute, inc.
*   attn: contracts dept
*   380 new york street
*   redlands, california, usa 92373
*
*   email: contracts@esri.com
*   
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt         12/02/04               original coding.
*e
***********************************************************************/
IS

   -- package globals. --

   g_type_dba            boolean NOT NULL DEFAULT False;
   g_current_user        varchar2(32);
   
   -- local procedures/functions --

Procedure parse_params (params            IN  varchar2,
                        spx_info_r        IN Out  spx_record_t)
  /***********************************************************************
  *
  *n  {parse_Params}  --  parse parameters list from domain index.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure parses the parameters string from a domain index
  *  create index statement.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     params  <in>        ==  (varchar2) parameters string.
  *     spx_Info_R <in out> ==  (spx_Record_T) st_Geometry_Index record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
  buffer             clob;
  buf2               clob;
  equal_pos          pls_integer := 0;
  space_pos          pls_integer := 0;
  tot_length         pls_integer := 0;
  pos                pls_integer := 0;
  st_pos             pls_integer := 0;
  strval             nvarchar2(256);
  dp_num_charset     varchar2(2);
  sp_num_charset     varchar2(2);
  dp_nls_dec         varchar2(1);
  dp_nls_sep         varchar2(1);
  sp_nls_dec         varchar2(1);
  sp_nls_sep         varchar2(1);
  colon_pos1         pls_integer := 0;
  colon_pos2         pls_integer := 0;
  dec_pos            pls_integer := 0;
  comma_pos          pls_integer := 0;
  gs1                varchar2(64);
  gs2                varchar2(64);
  gs3                varchar2(64);
  numeric_overflow   exception;
  pragma             exception_init(numeric_overflow,-6502);
Begin
  
  --Parse strings
  --ST_GRIDS
  --ST_SRID
  --ST_COMMIT_ROWS

  buffer := upper(params);
  buffer := REPLACE(buffer, chr(10), ' ');

  buffer := ltrim(buffer);
  buffer := rtrim(buffer);
  tot_length := length(buffer);

  pos := instr(buffer,'ST_GRIDS');
  If(pos > 0) Then

    spx_info_r.grid := SDE.sp_grid_info(0,0,0);
    equal_pos := instr(buffer,'=',pos);
    buf2 := substr(buffer,equal_pos+1,tot_length - pos);
    buf2 := ltrim(buf2);
    st_pos := instr(buf2,' ST');
    If(st_pos > 0) Then
      buf2 := substr(buf2,1,(length(buf2) - (length(buf2) - st_pos)));
    End If;
    
    SELECT value into dp_num_charset 
    FROM NLS_DATABASE_PARAMETERS  
    WHERE parameter='NLS_NUMERIC_CHARACTERS';

    SELECT value into sp_num_charset 
    FROM NLS_SESSION_PARAMETERS  
    WHERE parameter='NLS_NUMERIC_CHARACTERS';

    dp_nls_dec := substr(dp_num_charset,1,1);
    dp_nls_sep := substr(dp_num_charset,2,1);

    sp_nls_dec := substr(sp_num_charset,1,1);
    sp_nls_sep := substr(sp_num_charset,2,1);

    /* If the client session uses a dot for the decimal character, 
         then the grid separators may be commas or colons.
         Replace the commas with colons if needed.
         Otherwise, the session decimal character is a comma or another character,
         and the grid separators are required to be colons.
         There is nothing to replace. */
    
    If (sp_nls_dec = '.') Then
      colon_pos1 := instr(buf2,':');
      If(colon_pos1 = 0) Then
      buf2 := REPLACE(buf2,',',':');
    End If;
    End If;

    colon_pos1 := instr(buf2,':');
    If(colon_pos1 > 0) Then
      gs1 := substr(buf2,1,colon_pos1 - 1);
      colon_pos2 := instr(buf2,':',colon_pos1 + 1);
      
      If(colon_pos2 > 0) Then
        gs2 := substr(buf2,colon_pos1 + 1,(colon_pos2 - colon_pos1) -1);
        gs3 := substr(buf2,colon_pos2 + 1);
       
        Begin
          spx_info_r.grid.grid1 := to_number(gs1);
          spx_info_r.grid.grid2 := to_number(gs2);
          spx_info_r.grid.grid3 := to_number(gs3);
        Exception
          When numeric_overflow Then
            /* If input has inconsistencies, raise an error. */
            raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
        End;
       
      Else
        gs2 := substr(buf2,colon_pos1 + 1);

        Begin
          spx_info_r.grid.grid1 := to_number(gs1);
          spx_info_r.grid.grid2 := to_number(gs2);
        Exception
          When numeric_overflow Then
            /* If input has inconsistencies, raise an error. */
            raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
        End;
        
      End If;
    Else
      gs1 := buf2;

      Begin
        spx_info_r.grid.grid1 := to_number(gs1);
      Exception
        When numeric_overflow Then
          /* If input has inconsistencies, raise an error. */
          raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
      End;

    End if;

  Else
    raise_application_error (SDE.st_type_util.spx_no_grids,'No ST_GRIDS supplied in PARAMETERS clause.');
  End If;

  pos := instr(buffer,'ST_SRID');
  If(pos > 0) Then

    equal_pos := instr(buffer,'=',pos);
    buf2 := substr(buffer,equal_pos+1,tot_length - pos);
    buf2 := ltrim(buf2);
    space_pos := instr(buf2,' ');

    If(space_pos > 0) Then
      strval := substr(buf2,1,space_pos);
    Else 
      strval := substr(buf2,1);
    End if;

    strval := rtrim(strval);
    spx_info_r.srid := to_number(strval);

  Else
    raise_application_error (SDE.st_type_util.spx_no_srid,'No ST_SRID supplied in PARAMETERS clause.');
  End If;

  pos := instr(buffer,'ST_COMMIT_ROWS');
  If(pos > 0) then

    equal_pos := instr(buffer,'=',pos);
    buf2 := substr(buffer,equal_pos+1,tot_length - pos);
    buf2 := ltrim(buf2);

    space_pos := instr(buf2,' ');
    If(space_pos > 0) Then
      strval := substr(buf2,1,space_pos);
    Else 
      strval := substr(buf2,1);
    End If;
    strval := rtrim(strval);
    spx_info_r.commit_int := to_number(strval);
    
  Else
    spx_info_r.commit_int := 10000;
  End if;

End parse_params;


Procedure parse_params2                (params            IN  OUT varchar2)
  /***********************************************************************
  *
  *n  {parse_Params}  --  parse parameters list from domain index.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This procedure parses the ST_GRIDS parameters string from a 
  *  domain index create index DDL statement, to specify the grid sizes
  *  in the numeric format appropriate to the NLS_DATABASE_PARAMETERS.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     params  <in out>        ==  (varchar2) parameters string.
  *  
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    Sanjay Magal         10/2014           original coding.
  *e
  ***********************************************************************/
IS
  buffer             clob;
  buf2               clob;
  buf3               clob;
  buf4               clob;
  equal_pos          pls_integer := 0;
  tot_length         pls_integer := 0;
  pos                pls_integer := 0;
  st_pos             pls_integer := 0;
  colon_pos1         pls_integer := 0;
  colon_pos2         pls_integer := 0;
  grids_pos2         pls_integer := 0;
  space_pos          pls_integer := 0;
  gs1                varchar2(64);
  gs2                varchar2(64);
  gs3                varchar2(64);
  g1                 number := 0;
  g2                 number := 0;
  g3                 number := 0; 
  eur_g1             number := 0;
  eur_g2             number := 0;
  eur_g3             number := 0;  
  grid_clause        varchar2(256);
  dp_num_charset     varchar2(2);
  dp_nls_dec         varchar2(1);
  dp_nls_sep         varchar2(1);
  numeric_overflow   exception;
  pragma             exception_init(numeric_overflow,-6502);
Begin
  
  
  SELECT value into dp_num_charset 
  FROM NLS_DATABASE_PARAMETERS  
  WHERE parameter='NLS_NUMERIC_CHARACTERS';
          
  dp_nls_dec := substr(dp_num_charset,1,1);
  dp_nls_sep := substr(dp_num_charset,2,1);
  
  -- Parse strings
  -- ST_GRIDS

  buffer := upper(params);
  buffer := REPLACE(buffer, chr(10), ' ');

  buffer := ltrim(buffer);
  buffer := rtrim(buffer);
  tot_length := length(buffer);

  pos := instr(buffer,'ST_GRIDS');
  If(pos > 0) Then

  equal_pos := instr(buffer,'=',pos);
  buf2 := substr(buffer,equal_pos+1,tot_length - pos);
  buf2 := ltrim(buf2);
  st_pos := instr(buf2,' ST');
  If(st_pos > 0) Then
    buf2 := substr(buf2,1,(length(buf2) - (length(buf2) - st_pos)));
  End If;
    
  -- The exported Params string contains ASCII numeric characters.
  -- If the database being imported into has a different setting,
  -- temporarily set the NLS_NUMERIC_CHARACTERS to ASCII
  -- to extract the grid_sizes from the strings into ASCII formated numbers.
            
  If (dp_nls_dec = ',') Then
    DBMS_SESSION.SET_NLS('NLS_NUMERIC_CHARACTERS','''.,'''); 
  End If;
          
  colon_pos1 := instr(buf2,':');
  If(colon_pos1 > 0) Then
     gs1 := substr(buf2,1,colon_pos1 - 1);
     
     colon_pos2 := instr(buf2,':',colon_pos1 + 1);
      
     If(colon_pos2 > 0) Then
        gs2 := substr(buf2,colon_pos1 + 1,(colon_pos2 - colon_pos1) -1);
        gs3 := substr(buf2,colon_pos2 + 1);
     
        Begin
          
          g1 := to_number(gs1);
          g2 := to_number(gs2);
          g3 := to_number(gs3);
        Exception
          When numeric_overflow Then
            /* If input has inconsistencies, raise an error. */
            raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
        End;
       
     Else
        gs2 := substr(buf2,colon_pos1 + 1);
               
        Begin
          g1 := to_number(gs1);
          g2 := to_number(gs2);
        Exception
          When numeric_overflow Then
            /* If input has inconsistencies, raise an error. */
            raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
        End;
        
     End If;
    
  Else
      gs1 := buf2;

      Begin
        g1 := to_number(gs1);
      Exception
        When numeric_overflow Then
          /* If input has inconsistencies, raise an error. */
          raise_application_error (SDE.st_type_util.spx_invalid_grid_format,'Invalid format for grids supplied in ST_GRIDS parameter.');
      End;

  End if;

  Else
    raise_application_error (SDE.st_type_util.spx_no_grids,'No ST_GRIDS supplied in PARAMETERS clause.');
  End If;
  

  -- If the database being imported into has a non-ASCII setting for NLS_NUMERIC_CHARACTERS,
  -- Extract the ASCII formated grid size numbers into their native database format.
  
  If (dp_nls_dec = ',') Then
     DBMS_SESSION.SET_NLS('NLS_NUMERIC_CHARACTERS',''',.'''); 
  End If;

  eur_g1 := g1;
  eur_g2 := g2;
  eur_g3 := g3;
    
  grid_clause := 'ST_GRIDS= '||eur_g1;
  
  If g2 > 0 Then
     grid_clause := grid_clause ||':'||eur_g2;
     
     If g3 > 0 Then
       grid_clause := grid_clause ||':'||eur_g3;       
      End If;
      
  End If;

  grid_clause := grid_clause || ' ';
  pos := instr(buffer,'ST_SRID');
  
  If(pos > 0) Then
    params := grid_clause || substr(buffer,pos);
  Else
    raise_application_error (SDE.st_type_util.spx_no_srid,'No ST_SRID supplied in PARAMETERS clause.');
  End If;
  
  -- Does the Params string contain 2 instances of ST_GRIDS ?
  -- This can occur if the PARAMS clause from the export
  -- does not specify the ST_GRIDS parameter in the default (first) position. 
  
  grids_pos2 := INSTR(params,'ST_GRIDS=',2);
  
  If grids_pos2 > 0 Then
  
    -- PARAMS string upto and excluding 2nd occurrence of ST_GRIDS  
    buf3 := SUBSTR(params,1, grids_pos2 - 1); 
  
    -- Remaining PARAMS string including 2nd occurrence of ST_GRIDS  
    buf4 := SUBSTR(params, grids_pos2);   

    -- Position of 1st space in Remaining PARAMS substr  
    space_pos := INSTR(buf4, ' ');

    -- Remaining PARAMS string excluding 2nd occurrence of ST_GRIDS
    buf4 := SUBSTR (buf4, space_pos);
    
    params := buf3 || buf4; 
    
  End If;
 
End parse_params2;


   Procedure get_storage_info (params            IN  varchar2,
                               st_storage        IN Out  clob,
                               st_tablespace     IN Out  varchar2,
                               st_pctthreshold   IN Out  pls_integer)
  /***********************************************************************
  *
  *n  {parse_Params}  --  parse parameters list from domain index.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure parses the parameters string from a domain index
  *  create index statement.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     params  <in>        ==  (varchar2) parameters string.
  *     spx_Info_R <in out> ==  (spx_Record_T) st_Geometry_Index record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
   IS
     buffer       clob;
     buf2         clob;
     pos          pls_integer := 0;
     lparen_pos   pls_integer := 0;
     rparen_pos   pls_integer := 0;
     tot_length   pls_integer := 0;
     space_pos    pls_integer := 0;
     space2_pos   pls_integer := 0;
     temp_size    pls_integer := 0;

     Cursor c_block_size (tbspace_name IN varchar2) IS
     SELECT block_size
     FROM   user_tablespaces
     WHERE  tablespace_name = UPPER(tbspace_name);

Begin

    -- params:
      -- STORAGE
      -- TABLESPACE

  buffer := upper(params);
  tot_length := length(buffer);
  pos := instr(buffer,'STORAGE');
  if (pos > 0) then
    buf2 := substr(buffer,pos,tot_length - (pos-1));
    lparen_pos := instr(buf2,'(',1);
    if(lparen_pos > 0) then
      rparen_pos := instr(buf2,')',1);
      st_storage := substr(buf2,lparen_pos,rparen_pos - (lparen_pos-1));
    end if;
    
  end if;

  pos := instr(buffer,'TABLESPACE');
  if(pos > 0) then
 
    space_pos := instr(buffer,pos);
    buf2 := substr(buffer,pos+10);
    buf2 := ltrim(buf2);
    space2_pos := instr(buf2,' ',1);
    if(space2_pos > 0) then
      st_tablespace := substr(buf2,1,space2_pos);
    else
      st_tablespace := buf2;
    end if;
  end if;

  IF st_tablespace IS NOT NULL THEN
    Open c_block_size (st_tablespace);
    Fetch c_block_size INTO temp_size;
    If c_block_size%NOTFOUND THEN    
      temp_size := 0;
    End If;
    Close c_block_size;
      
    IF temp_size = 0 THEN 
      st_pctthreshold := 5;         -- default
    Elsif temp_size = 2048 THEN
      st_pctthreshold := 20;        -- 2k     
    Elsif temp_size = 4096 THEN
      st_pctthreshold := 10;        -- 4k
    Elsif temp_size = 8192 Then
      st_pctthreshold := 5;         -- 8k 
    Elsif temp_size = 16384 Then
      st_pctthreshold := 3;         -- 16k 
    Elsif temp_size = 32768 Then
      st_pctthreshold := 2;         -- 32k 
    Else
      st_pctthreshold := 5;         -- default 
    End If;
  ELSE 
    st_pctthreshold := 5;  -- (8k default)
  END IF;
  
End get_storage_info;
  
Function check_cache  (in_owner       IN varchar2,
                       in_table       IN varchar2,
                       in_spatial     IN varchar2,
                       spx_info_r     IN OUT spx_record_t,
                       sp_ref_r       IN OUT spatial_ref_record_t,
                       properties     IN OUT st_geom_prop_t)
  /***********************************************************************
  *
  *n  {check_Cache}  --  checks the cursor_Cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this function checks the cache for a layer match.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     spx_Record_T <in out> ==  (layer_Record_T) spatial index record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
Return number
IS
 
Begin
  
    If nlayers > 0 THEN
    FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).table_name = in_table AND 
           cursor_cache(id).owner = in_owner AND 
           cursor_cache(id).COLUMN = in_spatial AND 
           cursor_cache(id).index_id <> 0 THEN 

          spx_info_r.owner        := in_owner;
          spx_info_r.table_name   := in_table;
          spx_info_r.column_name  := in_spatial;
          spx_info_r.index_id     := cursor_cache(id).index_id;
          spx_info_r.grid         := SDE.sp_grid_info(0,0,0);
          spx_info_r.grid.grid1   := cursor_cache(id).gsize1;
          spx_info_r.grid.grid2   := cursor_cache(id).gsize2;
          spx_info_r.grid.grid3   := cursor_cache(id).gsize3;
          spx_info_r.srid         := cursor_cache(id).srid;
          properties              := cursor_cache(id).properties;
          sp_ref_r.x_offset       := cursor_cache(id).falsex;
          sp_ref_r.y_offset       := cursor_cache(id).falsey;
          sp_ref_r.xyunits        := cursor_cache(id).xyunits;
          sp_ref_r.srid           := cursor_cache(id).srid;

          Return(se_success);
        End If;
      End Loop;
    End If;
  Return(se_failure);

End check_cache;
  
Procedure add_cache_info  (in_owner        IN varchar2,
                           in_table        IN varchar2,
                           in_spatial      IN varchar2,
                           spx_info_r      IN spx_record_t,
                           sp_ref_r        IN spatial_ref_record_t,
                           properties      IN st_geom_prop_t)
  /***********************************************************************
  *
  *n  {add_Cache_Info}  --  adds layer/sp_Ref info to cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure adds the layer and spatial_Ref info to the cache.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     layer_R <in out> ==  (layer_Record_T) layer record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
    spatial_col varchar2(32);
    b_add       boolean := False;
 
Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = 0 THEN
          cursor_cache(id).table_name    := in_table;
          cursor_cache(id).owner         := in_owner;
          cursor_cache(id).COLUMN        := in_spatial;
          cursor_cache(id).index_id      := spx_info_r.index_id;
          cursor_cache(id).gsize1        := spx_info_r.grid.grid1;
          cursor_cache(id).gsize2        := spx_info_r.grid.grid2;
          cursor_cache(id).gsize3        := spx_info_r.grid.grid3;
          cursor_cache(id).falsex        := sp_ref_r.x_offset;
          cursor_cache(id).falsey        := sp_ref_r.y_offset;
          cursor_cache(id).xyunits       := sp_ref_r.xyunits;
          cursor_cache(id).srid          := sp_ref_r.srid;
          cursor_cache(id).properties    := properties;
          cursor_cache(id).ncurs_array := 0;
          cursor_cache(id).curs_array(1).curs := 0;
          cursor_cache(id).curs_array(1).curs_insert := 0;
          cursor_cache(id).curs_array(1).curs_update := 0;
          cursor_cache(id).curs_array(1).curs_delete := 0;
          cursor_cache(id).curs_array(1).curs_spatial_join := 0;
          cursor_cache(id).curs_array(1).curs_grid1 := 0;
          cursor_cache(id).curs_array(1).tab_object := NULL;
          b_add := True;
          Exit;
        End If;
      End Loop nlayers;
    End If;

    If b_add = False THEN
      nlayers := nlayers + 1;
      cursor_cache(nlayers).table_name    := in_table;
      cursor_cache(nlayers).owner         := in_owner;
      cursor_cache(nlayers).COLUMN        := in_spatial;
      cursor_cache(nlayers).index_id      := spx_info_r.index_id;
      cursor_cache(nlayers).gsize1        := spx_info_r.grid.grid1;
      cursor_cache(nlayers).gsize2        := spx_info_r.grid.grid2;
      cursor_cache(nlayers).gsize3        := spx_info_r.grid.grid3;
      cursor_cache(nlayers).falsex        := sp_ref_r.x_offset;
      cursor_cache(nlayers).falsey        := sp_ref_r.y_offset;
      cursor_cache(nlayers).xyunits       := sp_ref_r.xyunits;
      cursor_cache(nlayers).properties    := properties;
      cursor_cache(nlayers).srid          := sp_ref_r.srid;
      cursor_cache(nlayers).ncurs_array  := 0;
      cursor_cache(nlayers).curs_array(1).curs := 0;
      cursor_cache(nlayers).curs_array(1).curs_insert := 0;
      cursor_cache(nlayers).curs_array(1).curs_update := 0;
      cursor_cache(nlayers).curs_array(1).curs_delete := 0;
      cursor_cache(nlayers).curs_array(1).curs_spatial_join := 0;
      cursor_cache(nlayers).curs_array(1).curs_grid1 := 0;
      cursor_cache(nlayers).curs_array(1).tab_object := NULL;
  End If;

End add_cache_info;

Procedure update_cache_info  (in_owner     IN varchar2,
                              in_table     IN varchar2,
                              in_spatial   IN varchar2,
                              spx_info_r   IN spx_record_t,
                              sp_ref_r     IN spatial_ref_record_t,
                              properties   IN st_geom_prop_t)
  /***********************************************************************
  *
  *n  {update_Cache_Info}  --  updates layer/sp_Ref info in cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure updates the layer and spatial_Ref info.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     layer_R <in out> ==  (layer_Record_T) layer record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS

Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = spx_info_r.index_id THEN
          cursor_cache(id).table_name    := in_table;
          cursor_cache(id).owner         := in_owner;
          cursor_cache(id).column        := in_spatial;
          cursor_cache(id).index_id      := spx_info_r.index_id;
          cursor_cache(id).gsize1        := spx_info_r.grid.grid1;
          cursor_cache(id).gsize2        := spx_info_r.grid.grid2;
          cursor_cache(id).gsize3        := spx_info_r.grid.grid3;
          cursor_cache(id).falsex        := sp_ref_r.x_offset;
          cursor_cache(id).falsey        := sp_ref_r.y_offset;
          cursor_cache(id).xyunits       := sp_ref_r.xyunits;
          cursor_cache(id).srid          := sp_ref_r.srid;
          cursor_cache(id).properties    := properties;
          Exit;
        End If;
      End Loop nlayers;
    End If;

  End update_cache_info;
  
Procedure delete_cache_info  (spx_info_r      IN spx_record_t)
  /***********************************************************************
  *
  *n  {delete_Cache_Info}  --  delete entry from cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure deletes an entry from the cache.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     spx_info_r <in > ==  (spx_record_t) spatial index record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS

Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = spx_info_r.index_id THEN
          cursor_cache(id).table_name    := '';
          cursor_cache(id).owner         := '';
          cursor_cache(id).COLUMN        := '';
          cursor_cache(id).index_id      := 0;
          cursor_cache(id).gsize1        := 0;
          cursor_cache(id).gsize2        := 0;
          cursor_cache(id).gsize3        := 0;
          cursor_cache(id).falsex        := 0;
          cursor_cache(id).falsey        := 0;
          cursor_cache(id).xyunits       := 0;
          cursor_cache(id).srid          := 0;
          cursor_cache(id).properties    := 0;
          If cursor_cache(id).ncurs_array > 0 THEN
            FOR pos IN 1 .. cursor_cache(id).ncurs_array
            Loop
                if cursor_cache(id).curs_array(pos).curs <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs);
                  cursor_cache(id).curs_array(pos).curs := 0;
                end if;
              
                if cursor_cache(id).curs_array(pos).curs_insert <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs_insert);
                  cursor_cache(id).curs_array(pos).curs_insert := 0;
                end if;
              
                if cursor_cache(id).curs_array(pos).curs_update <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs_update);
                  cursor_cache(id).curs_array(pos).curs_update := 0;
                end if;
              
                if cursor_cache(id).curs_array(pos).curs_delete <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs_delete);
                  cursor_cache(id).curs_array(pos).curs_delete := 0;
                end if;
                
                if cursor_cache(id).curs_array(pos).curs_spatial_join <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs_spatial_join);
                  cursor_cache(id).curs_array(pos).curs_spatial_join := 0;
                end if;
                
                if cursor_cache(id).curs_array(pos).curs_grid1 <> 0 then
                  dbms_sql.close_cursor(cursor_cache(id).curs_array(pos).curs_grid1);
                  cursor_cache(id).curs_array(pos).curs_grid1 := 0;
                end if;
                
              End Loop;
          End If;
        end if;
      End Loop nlayers;
    End If;   
         
 End delete_cache_info;
 
Function check_search_geom_srid      (ia          IN sys.odciindexinfo,
                                      srch_geom   IN SDE.st_geometry,
                                      shape_out   IN OUT SDE.st_geometry)
  /***********************************************************************
  *
  *n  {check_search_geom_srid}  --  Convert search shape if srid's are
  *                                  different.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This procedure will convert the input search geometry into
  *  the suppied SRID/spatial reference if the search geometry's SRID
  *  is different.
  *  
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     ia         <in>     ==  (sys.odciindexinfo) index info.
  *     srch_geom  <in out> ==  SDE.st_geometry
  *     shape_out  <in out> ==  SDE.st_geometry
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/11/09          original coding.
  *e
  ***********************************************************************/
Return number 
IS
  
    spx_info_r  spx_record_t;
    sp_ref_r    spatial_ref_record_t;
    sp_ref2_r   spatial_ref_record_t;
    properties  st_geom_prop_t;
    optype      pls_integer;
    rc          number;   
    temp        varchar2(1);
    tempraw     raw(1);
    geotranid   number := 0;
    
Begin
    
    spx_info_r.index_id := 0;
    spx_info_r.grid := SDE.sp_grid_info(0,0,0);
    shape_out.numpts := 0;
    shape_out.entity := 0;

    optype := SDE.spx_util.st_geom_operation_select;
    rc := SDE.spx_util.get_object_info(ia,optype,NULL,spx_info_r,sp_ref_r,properties);
    If SE_SUCCESS != rc Then
      return(rc);
    End If;

    If srch_geom.srid != spx_info_r.srid Then

      rc := SDE.st_spref_util.select_spref(sp_ref_r);
      If rc != SDE.st_type_user.se_success THEN
        raise_application_error (SDE.st_type_util.st_no_srid,'srid '||sp_ref_r.srid||
                                 ' does not exist in st_spatial_references table.');
      End If;

      sp_ref2_r.srid := srch_geom.srid;
      rc := SDE.st_spref_util.select_spref(sp_ref2_r);
      If rc != SDE.st_type_user.se_success THEN
        raise_application_error (SDE.st_type_util.st_no_srid,'srid '||sp_ref2_r.srid||
                                 ' does not exist in st_spatial_references table.');
      End If;

      shape_out.numpts := 0;
      shape_out.entity := 0;
      shape_out.minx   := 0;
      shape_out.miny   := 0;
      shape_out.maxx   := 0;
      shape_out.maxy   := 0;
      shape_out.area   := 0;
      shape_out.len    := 0;
      shape_out.minz   := NULL;
      shape_out.maxz   := NULL;
      shape_out.minm   := NULL;
      shape_out.maxm   := NULL;
      shape_out.srid   := 0;

      temp := lpad('a', 1, 'a');
      tempraw := utl_raw.cast_to_raw (temp);
      shape_out.points := tempraw;
      
      If (srch_geom.numpts = 0) AND 
         (srch_geom.len = 0 OR srch_geom.len IS NULL) Then
            
        shape_out.numpts := 0;
        shape_out.entity := 0;
        shape_out.area   := 0;
	    shape_out.len    := 0;
        shape_out.minx   := NULL;
	    shape_out.miny   := NULL;
	    shape_out.maxx   := NULL;
	    shape_out.maxy   := NULL;
	    shape_out.minz   := NULL;
	    shape_out.maxz   := NULL;
	    shape_out.minm   := NULL;
	    shape_out.maxm   := NULL;
        shape_out.points := empty_blob();	    
	    shape_out.srid   := sp_ref_r.srid;
          
        return(SE_SUCCESS);
             
      End If;

      SDE.st_geometry_shapelib_pkg.transform(geotranid,sp_ref2_r.srid,sp_ref2_r.x_offset,sp_ref2_r.y_offset,sp_ref2_r.xyunits,
                                             sp_ref2_r.z_offset,sp_ref2_r.z_scale,sp_ref2_r.m_offset,sp_ref2_r.m_scale,
                                             sp_ref2_r.Definition,srch_geom.numpts,srch_geom.entity,srch_geom.minx,
                                             srch_geom.miny,srch_geom.maxx,srch_geom.maxy,srch_geom.minz,srch_geom.maxz,
                                             srch_geom.minm,srch_geom.maxm,srch_geom.area,srch_geom.len,srch_geom.points,
                                             sp_ref_r.srid,sp_ref_r.x_offset,sp_ref_r.y_offset,sp_ref_r.xyunits,
                                             sp_ref_r.z_offset,sp_ref_r.z_scale,sp_ref_r.m_offset,sp_ref_r.m_scale,
                                             sp_ref_r.Definition,shape_out.numpts,shape_out.entity,
                                             shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,shape_out.minz,shape_out.maxz,
                                             shape_out.minm,shape_out.maxm,shape_out.area,shape_out.len,shape_out.points);

      if(shape_out.numpts IS NULL and shape_out.entity = 0) then
        shape_out.numpts    := 0;
        shape_out.minx      := NULL;
        shape_out.maxx      := NULL;
        shape_out.miny      := NULL;
        shape_out.maxy      := NULL;
        shape_out.minz      := NULL;
        shape_out.maxz      := NULL;
        shape_out.minm      := NULL;
        shape_out.maxm      := NULL;
      End if;
    End IF;

    shape_out.srid := sp_ref_r.srid;
      
    return(SE_SUCCESS);

End check_search_geom_srid;

Function get_object_info (ia          IN sys.odciindexinfo,
                          optype      IN Out pls_integer,
                          params      IN varchar2,
                          spx_info_r  IN OUT spx_record_t,
                          spref_r     IN OUT spatial_ref_record_t,
                          properties  IN OUT st_geom_prop_t)
  /***********************************************************************
  *
  *n  {get_object_info}  --  get layer info 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure verifies the existence of the layer in order to
  *  retrieve the layer_Id. if the layer does not exist, it's likely
  *  being created from the sql interface. if so, it will be added to 
  *  the layers and geometry_Columns tables. if it already exists, it's
  *  layer record is fetched to be used during the domnain index 
  *  creation. 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     params  <in>     ==  (varchar2) parameters string.
  *    layer_R <in out> ==  (layer_Record_T) layer record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
 Return number 
IS
    spatial_col spx_column_t;
    owner       spx_owner_t;
    table_name  spx_table_t;
    srid        srid_t;
    gsize1      spx_gsize_t;
    gsize2      spx_gsize_t;
    gsize3      spx_gsize_t;
    falsex      sr_falsex_t;
    falsey      sr_falsey_t;
    xyunits     sr_xyunits_t;
    new_g_table table_name_t;
    index_id    spx_index_id_t;
    geom_type   varchar2(32);
    rc          number;

    Cursor c_spx_info_get (in_owner IN spx_owner_t,in_table IN spx_table_t,in_spcol IN spx_column_t) IS
           SELECT  s.index_id,s.grid.grid1,s.grid.grid2,s.grid.grid3,
                   sr.srid,sr.x_offset,sr.y_offset,sr.xyunits,st.properties
           FROM   SDE.st_geometry_index s,SDE.st_geometry_columns st,SDE.st_spatial_references sr
           WHERE  s.owner = in_owner AND s.table_name = in_table AND s.column_name = in_spcol 
                  AND s.index_id = st.geom_id AND sr.srid = s.srid;

    no_layer_sequence           Exception;
    no_spatial_ref              Exception;
    no_layer_information_found  Exception;

Begin

    spatial_col := REPLACE(ia.indexcols(1).colname,'"','');
    spx_info_r.index_id := 0;
    spx_info_r.grid := SDE.sp_grid_info(0,0,0);

        -- check the cache first--
    rc := SDE.spx_util.check_cache(ia.indexcols(1).tableschema,ia.indexcols(1).tablename,
                                   spatial_col,spx_info_r,spref_r,properties);


    If rc = se_success then
      if optype != SDE.spx_util.st_geom_operation_create THEN
        Return(se_success);
      else
        if (bitand(ia.IndexInfoFlags, ODCIConst.Parallel) = ODCIConst.Parallel) then 
          SDE.spx_util.delete_cache_info(spx_info_r);
        Else
          SDE.spx_util.delete_cache_info(spx_info_r);
          SDE.spx_util.delete_index(spx_info_r.index_id);
        End If;
      End If;
    End if;
      
    gsize1 := 0;
    gsize2 := 0;
    gsize3 := 0;

     -- check if layer exists. if not, create it and the geometry_Columns --

    If optype = st_geom_operation_create THEN
      LOCK TABLE SDE.st_geometry_columns IN EXCLUSIVE MODE;
    End If;

    Open c_spx_info_get (ia.indexcols(1).tableschema,ia.indexcols(1).tablename,spatial_col);
    Fetch c_spx_info_get INTO index_id,gsize1,gsize2,gsize3,srid,falsex,falsey,xyunits,properties;

    If c_spx_info_get%NOTFOUND THEN
      Close c_spx_info_get;

      If optype = st_geom_operation_create THEN
        --parse the parameters list for version table info --

        If params IS NOT NULL THEN
          parse_params(params,spx_info_r);
        End If;
      End If;

      If optype != st_geom_operation_create THEN
        spx_info_r.index_id := -1;
        Return(se_success);
      End If;

      If spx_info_r.srid = -1 THEN
        raise_application_error (SDE.st_type_util.spx_no_srid,'No ST_SRID supplied in PARAMETERS clause.');
      End If;

      owner := ia.indexcols(1).tableschema;
      table_name := ia.indexcols(1).tablename;
      spref_r.srid := spx_info_r.srid;

      rc := SDE.st_spref_util.select_spref(spref_r);

      If rc <> se_success THEN
        raise_application_error (SDE.st_type_util.spx_no_srid,'Parameter ST_SRID '||spref_r.srid||
                                 ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;

      rc := SDE.st_geom_cols_util.select_gcol(owner,table_name,spatial_col,geom_type,properties,srid);

      If rc = SDE.st_type_util.st_table_noexist THEN
        geom_type := ia.indexcols(1).coltypename;
        srid := spx_info_r.srid;
        SDE.st_geom_cols_util.insert_gcol(owner,table_name,spatial_col,geom_type,srid);
        optype := st_geom_operation_new;
      ELSE
        If spx_info_r.srid <> srid THEN
          raise_application_error (SDE.st_type_util.spx_diff_srids,'Parameter ST_SRID '||spx_info_r.srid||
                                   ' is different from ST_GEOMETRY_COLUMNS srid ('||srid||').');
        End If;
      End If;

      spx_info_r.owner := owner;
      spx_info_r.table_name := table_name;
      spx_info_r.column_name := spatial_col;
      spx_info_r.index_name := ia.indexname;
  
      spx_util.insert_index(spx_info_r);
      
      IF spx_info_r.index_id IS NULL THEN
        SELECT index_id INTO spx_info_r.index_id 
        FROM SDE.st_geometry_index
        WHERE owner = spx_info_r.owner AND table_name = spx_info_r.table_name;
      END IF;

      SDE.spx_util.add_cache_info (ia.indexschema,ia.indexcols(1).tablename,spatial_col,
                                     spx_info_r,spref_r,properties);
      
      If optype = st_geom_operation_new THEN
        COMMIT;
      End If;
    ELSE
      spx_info_r.owner := ia.indexschema;
      spx_info_r.table_name := ia.indexcols(1).tablename;
      spx_info_r.column_name := spatial_col;
      spx_info_r.index_id   := index_id;
      spx_info_r.grid.grid1 := gsize1;
      spx_info_r.grid.grid2 := gsize2;
      spx_info_r.grid.grid3 := gsize3;
      spx_info_r.srid       := srid;
      spref_r.x_offset      := falsex;
      spref_r.y_offset      := falsey;
      spref_r.xyunits       := xyunits;
      spref_r.srid          := srid;

      IF ((bitand(ia.IndexInfoFlags, ODCIConst.Parallel) != ODCIConst.Parallel) AND
          (optype != SDE.spx_util.st_geom_operation_drop)) THEN
        SDE.spx_util.add_cache_info (ia.indexschema,ia.indexcols(1).tablename,spatial_col,
                                     spx_info_r,spref_r,properties);
      END IF;

      Close c_spx_info_get;
    End If;
    Return(se_success);

End get_object_info;

Procedure compute_feat_grid_envp (gsize1    IN integer,
                                  gsize2    IN integer,
                                  gsize3    IN integer,
                                  e_minx    IN integer,
                                  e_miny    IN integer,
                                  e_maxx    IN integer,
                                  e_maxy    IN integer,
                                  g_minx    Out integer,
                                  g_miny    Out integer,
                                  g_maxx    Out integer,
                                  g_maxy    Out integer)
  /***********************************************************************
  *
  *n  {compute_Feat_Grid_Envp}  --  compute the feature grid envelop 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure calculates the grid envelope of a feature.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     gsize1      <in>     ==  (pls_Integer)  grid size1
  *     gsize2      <in>     ==  (pls_Integer)  grid size2
  *     gsize3      <in>     ==  (pls_Integer)  grid size3
  *     int_Env     <in>     ==  (t_Env) feature envelope/sysunits
  *     f_Row       <in>     ==  (pls_Integer) feat row index
  *     grid        <in out> ==  (r_Env) grid envelope/sysunits
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
  
Begin
  
    g_minx := trunc(e_minx / gsize1);
    g_miny := trunc(e_miny / gsize1);
    g_maxx := trunc(e_maxx / gsize1);
    g_maxy := trunc(e_maxy / gsize1);

    If ((g_maxx - g_minx + 1) * (g_maxy - g_miny + 1) > max_grids_per_level
         AND gsize2 > 0) THEN
      g_minx := trunc(e_minx / gsize2);
      g_miny := trunc(e_miny / gsize2);
      g_maxx := trunc(e_maxx / gsize2);
      g_maxy := trunc(e_maxy / gsize2);

      If ((g_maxx - g_minx + 1) * (g_maxy - g_miny + 1) > max_grids_per_level AND gsize3 > 0) THEN
        g_minx := trunc(e_minx / gsize3);
        g_miny := trunc(e_miny / gsize3);
        g_maxx := trunc(e_maxx / gsize3);
        g_maxy := trunc(e_maxy / gsize3);
        g_minx := g_minx + grid_level_mask_2;
        g_miny := g_miny + grid_level_mask_2;
        g_maxx := g_maxx + grid_level_mask_2;
        g_maxy := g_maxy + grid_level_mask_2;
      ELSE
        g_minx := g_minx + grid_level_mask_1;
        g_miny := g_miny + grid_level_mask_1;
        g_maxx := g_maxx + grid_level_mask_1;
        g_maxy := g_maxy + grid_level_mask_1;
      End If;
    End If;

End compute_feat_grid_envp;

Procedure set_partition_curs (pos              IN integer,
                              curs             IN number,
                              partition_name   IN varchar2,
                              spx_info_r       IN spx_record_t,
                              Type             IN pls_integer)
  /***********************************************************************
  *
  *n  {set_partition_curs}  --  Sets the partition cursor
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
  
  bpartition_found boolean := False;
  i                pls_integer;
  new_pos          integer;

Begin

    If pos > 0 THEN
      If cursor_cache(pos).ncurs_array > 0 THEN
      FOR id IN 1 .. cursor_cache(pos).ncurs_array
      Loop
          If partition_name = cursor_cache(pos).curs_array(id).tab_object THEN
            bpartition_found := True;
            If Type = curs_type_select THEN
              cursor_cache(pos).curs_array(id).curs := curs;
            Elsif Type = curs_type_insert THEN
              cursor_cache(pos).curs_array(id).curs_insert := curs;
            Elsif Type = curs_type_update THEN
              cursor_cache(pos).curs_array(id).curs_update := curs;
            Elsif Type = curs_type_delete THEN
              cursor_cache(pos).curs_array(id).curs_delete := curs;
            Elsif Type = curs_type_select_spatial_join THEN
              cursor_cache(pos).curs_array(id).curs_spatial_join := curs;
            Elsif Type = curs_type_grid1 THEN
              cursor_cache(pos).curs_array(id).curs_grid1 := curs;
            End If;
            cursor_cache(pos).curs_array(id).gsize1 := spx_info_r.grid.grid1;
            cursor_cache(pos).curs_array(id).gsize2 := spx_info_r.grid.grid2;
            cursor_cache(pos).curs_array(id).gsize3 := spx_info_r.grid.grid3;
          End If;
        End Loop;
      End If;
    End If;

    If bpartition_found = False THEN
      if pos = 0 then
        cursor_cache(1).ncurs_array := 1;
        new_pos := 1;
      else
        cursor_cache(pos).ncurs_array := cursor_cache(pos).ncurs_array + 1;
        new_pos := pos;
      end if;
      
      i := cursor_cache(new_pos).ncurs_array;
      cursor_cache(new_pos).curs_array(i).curs := 0;
      cursor_cache(new_pos).curs_array(i).curs_update := 0;
      cursor_cache(new_pos).curs_array(i).curs_insert := 0;
      cursor_cache(new_pos).curs_array(i).curs_delete := 0;
      cursor_cache(new_pos).curs_array(i).curs_spatial_join := 0;
      cursor_cache(new_pos).curs_array(i).curs_grid1 := 0;
      cursor_cache(new_pos).curs_array(i).tab_object := partition_name;
      If Type = curs_type_select THEN
        cursor_cache(new_pos).curs_array(i).curs := curs;
      Elsif Type = curs_type_insert THEN
        cursor_cache(new_pos).curs_array(i).curs_insert := curs;
      Elsif Type = curs_type_update THEN
        cursor_cache(new_pos).curs_array(i).curs_update := curs;
      Elsif Type = curs_type_delete THEN
        cursor_cache(new_pos).curs_array(i).curs_delete := curs;
      Elsif Type = curs_type_select_spatial_join THEN
        cursor_cache(new_pos).curs_array(i).curs_spatial_join := curs;
      Elsif Type = curs_type_grid1 THEN
        cursor_cache(new_pos).curs_array(i).curs_grid1 := curs;
      End If;
      cursor_cache(new_pos).curs_array(i).gsize1 := spx_info_r.grid.grid1;
      cursor_cache(new_pos).curs_array(i).gsize2 := spx_info_r.grid.grid2;
      cursor_cache(new_pos).curs_array(i).gsize3 := spx_info_r.grid.grid3;
    End If;

End set_partition_curs;

Function get_partition_curs (pos             IN     integer,
                             partition_name  IN     varchar2,
                             Type            IN     pls_integer,
                             grid_info       IN OUT sp_grid_info)
  /***********************************************************************
  *
  *n  {get_partition_curs}  --  Gets the partition cursor
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
  Return number
  IS
  
  bpartition_found boolean := False;
  curs             number := 0;

  Begin

    If cursor_cache(pos).ncurs_array > 0 THEN
    FOR id IN 1 .. cursor_cache(pos).ncurs_array
    Loop
        If partition_name = cursor_cache(pos).curs_array(id).tab_object THEN
          bpartition_found := True;
         
          If Type = curs_type_select THEN
            curs := cursor_cache(pos).curs_array(id).curs;
          Elsif Type = curs_type_insert THEN
            curs := cursor_cache(pos).curs_array(id).curs_insert;
          Elsif Type = curs_type_update THEN
            curs := cursor_cache(pos).curs_array(id).curs_update;
          Elsif Type = curs_type_delete THEN
            curs := cursor_cache(pos).curs_array(id).curs_delete;
          Elsif Type = curs_type_select_spatial_join THEN
            curs := cursor_cache(pos).curs_array(id).curs_spatial_join;
          Elsif Type = curs_type_grid1 THEN
            curs := cursor_cache(pos).curs_array(id).curs_grid1;
          End If;
          grid_info.grid1 := cursor_cache(pos).curs_array(id).gsize1;
          grid_info.grid2 := cursor_cache(pos).curs_array(id).gsize2;
          grid_info.grid3 := cursor_cache(pos).curs_array(id).gsize3;
        End If;
      End Loop;
    End If;

    Return(curs);
End get_partition_curs;

Procedure set_curs (pos           IN integer,
                    curs          IN number,
                    Type          IN pls_integer)
  /***********************************************************************
  *
  *n  {set_curs}  --  Sets the cursor based on type.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
  
Begin

    If Type = curs_type_select THEN
      cursor_cache(pos).curs_array(1).curs := curs;
    Elsif Type = curs_type_insert THEN
      cursor_cache(pos).curs_array(1).curs_insert := curs;
    Elsif Type = curs_type_update THEN
      cursor_cache(pos).curs_array(1).curs_update := curs;
    Elsif Type = curs_type_delete THEN
      cursor_cache(pos).curs_array(1).curs_delete := curs;
    Elsif Type = curs_type_select_spatial_join THEN
      cursor_cache(pos).curs_array(1).curs_spatial_join := curs;
    Elsif Type = curs_type_grid1 THEN
      cursor_cache(pos).curs_array(1).curs_grid1 := curs;
    End If;

End set_curs;

Function get_curs (pos           IN integer,
                   Type          IN   pls_integer)
  /***********************************************************************
  *
  *n  {get_curs}  --  Get cursor based on type
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
Return number
IS
  
  curs        integer := 0;

Begin

     If Type = curs_type_select THEN
       curs := cursor_cache(pos).curs_array(1).curs;
     Elsif Type = curs_type_insert THEN
       curs := cursor_cache(pos).curs_array(1).curs_insert;
     Elsif Type = curs_type_update THEN
       curs := cursor_cache(pos).curs_array(1).curs_update;
     Elsif Type = curs_type_delete THEN
       curs := cursor_cache(pos).curs_array(1).curs_delete;
     Elsif Type = curs_type_select_spatial_join THEN
       curs := cursor_cache(pos).curs_array(1).curs_spatial_join;
     Elsif Type = curs_type_grid1 THEN
       curs := cursor_cache(pos).curs_array(1).curs_grid1;
     End If;
 
    Return(curs);
End get_curs;

Procedure execute_spatial (ia              IN  sys.odciindexinfo,
                           table_name      IN  varchar2,
                           spx_info_r      IN OUT  SDE.spx_util.spx_record_t,
                           sp_ref_r        IN  SDE.spx_util.spatial_ref_record_t,
                           int_env_r       IN  SDE.spx_util.r_env,
                           curs            OUT integer)
  /***********************************************************************
  *
  *n  {exec_Spatial}  --  executes spatial selection
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure executes a spatial select. the call to execute
  *  this query is made outside the static odci function to preserve 
  *  the cursor. 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     ia          <in>   ==  (sys.odciindexinfo) table/index info
  *     table_name  <in>   ==  (varchar2) S-table name
  *     spx_info_r  <in>   ==  (spx_util.spx_record_t) spatial index 
  *                                                    metadata
  *     sp_ref_r    <in>   ==  (spx_util.spatial_ref_record_t) sp_ref info
  *     int_env_r   <in>   ==  (spx_util.r_env) b.shape/(srch_shape) MBR
  *     curs        <out>  ==  (integer) grid envelope/sysunits
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
    rid             rowid;
    stmt            varchar2(512);
    nrows           integer;
    curs1           integer := 0;
    pos             pls_integer := 0;
    bcached         boolean := False;
    grids           SDE.spx_util.v_grids := v_grids(0,0,0);
    levels          SDE.spx_util.v_levels := v_levels(0,0,0);
    grid_env        SDE.spx_util.r_grid_env := SDE.spx_util.r_grid_env();
    n_grids         integer;
    pgrid_info      sp_grid_info := sp_grid_info(0,0,0);
    frows           number;
  
Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = spx_info_r.index_id THEN
        pos := id;

        If (ia.indexpartition IS NOT NULL) THEN
          curs1 := get_partition_curs (pos,ia.indexpartition,curs_type_select,pgrid_info);
          if(curs1 <> 0) Then
            spx_info_r.grid.grid1 := pgrid_info.grid1;
            spx_info_r.grid.grid2 := pgrid_info.grid2;
            spx_info_r.grid.grid3 := pgrid_info.grid3;
          End If;
        ELSE
          If cursor_cache(id).curs_array(1).curs  > 0 THEN
            curs1 := cursor_cache(id).curs_array(1).curs;
            End If;
            Exit;
          End If;
        End If;
      End Loop nlayers;
    End If;

    n_grids := 1;
    grid_env.extend(3);

    If spx_info_r.grid.grid2 > 0 THEN
      If spx_info_r.grid.grid3 > 0 THEN
        n_grids := 3;
      ELSE
        n_grids := 2;
      End If;
    End If;

    If curs1 = 0 THEN
      If n_grids = 1 THEN
        stmt := 'SELECT distinct sp_id FROM '||table_name||' WHERE ((gx >= :1 AND gx <= :2 AND '||
                      'gy >= :3 AND gy <= :4) OR (gx < 0 and gy < 0)) AND minx <= :e1 AND miny <= :e2 AND '||
                      'maxx >= :e3 AND maxy >= :e4';

      Elsif n_grids = 2 THEN
        stmt := 'SELECT distinct sp_id FROM '||table_name||
                ' WHERE ((gx >= :1 AND gx <= :2 AND gy >= :3 AND gy <= :4) OR '||
                        '(gx >= :5 AND gx <= :6 AND gy >= :7 AND gy <= :8) OR (gx < 0 and gy < 0)) '||
                        'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4';
      Elsif n_grids = 3 THEN
        stmt := 'SELECT distinct sp_id FROM '||table_name||
                ' WHERE ((gx >= :1 AND gx <= :2 AND gy >= :3 AND gy <= :4) OR '||
                        '(gx >= :5 AND gx <= :6 AND gy >= :7 AND gy <= :8) OR '||
                        '(gx >= :9 AND gx <= :10 AND gy >= :11 AND gy <= :12) OR (gx < 0 and gy < 0)) '||
                        'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4';
      ELSE
        raise_application_error (SDE.st_type_util.spx_invalid_number_of_grids,
                                 'ST_GEOMETRY_INDEX Invalid number of Grids');
      End If;

      curs1 := dbms_sql.open_cursor;

      dbms_sql.parse(curs1,stmt,dbms_sql.native);
      SDE.spx_util.fetch_env(curs1).curs := curs1;
      SDE.spx_util.fetch_env(curs1).fetch_pos := 1;
      SDE.spx_util.fetch_env(curs1).first_fetch := TRUE;
      SDE.spx_util.fetch_env(curs1).fetch_state := 'FIRST';
      SDE.spx_util.fetch_env(curs1).total_rows := 0;
    End If;

    grids(1) := spx_info_r.grid.grid1 * sp_ref_r.xyunits;
    levels(1) := 0;

    If spx_info_r.grid.grid2 > 0 THEN
      grids(2) := spx_info_r.grid.grid2 * sp_ref_r.xyunits;
      levels(2) := grid_level_mask_1;
    ELSE
      grids(2) := 0;
    End If;

    If spx_info_r.grid.grid3 > 0 THEN
      grids(3) := spx_info_r.grid.grid3 * sp_ref_r.xyunits;
      levels(3) := grid_level_mask_2;
    ELSE
      grids(3) := 0;
    End If;

    FOR x IN 1 .. 3
    Loop
      If grids(x) > 0 THEN
        grid_env(x).minx := trunc(int_env_r.minx / grids(x)) + levels(x);
        grid_env(x).miny := trunc(int_env_r.miny / grids(x)) + levels(x);
        grid_env(x).maxx := trunc(int_env_r.maxx / grids(x)) + levels(x);
        grid_env(x).maxy := trunc(int_env_r.maxy / grids(x)) + levels(x);
      End If;
    End Loop;

    dbms_sql.bind_variable(curs1, ':1',grid_env(1).minx);
    dbms_sql.bind_variable(curs1, ':2',grid_env(1).maxx);
    dbms_sql.bind_variable(curs1, ':3',grid_env(1).miny);
    dbms_sql.bind_variable(curs1, ':4',grid_env(1).maxy);

    If grids(2) > 0 THEN
      dbms_sql.bind_variable(curs1, ':5',grid_env(2).minx);
      dbms_sql.bind_variable(curs1, ':6',grid_env(2).maxx);
      dbms_sql.bind_variable(curs1, ':7',grid_env(2).miny);
      dbms_sql.bind_variable(curs1, ':8',grid_env(2).maxy);
    End If;

    If grids(3) > 0 THEN
      dbms_sql.bind_variable(curs1, ':9',grid_env(3).minx);
      dbms_sql.bind_variable(curs1, ':10',grid_env(3).maxx);
      dbms_sql.bind_variable(curs1, ':11',grid_env(3).miny);
      dbms_sql.bind_variable(curs1, ':12',grid_env(3).maxy);
    End If;

    dbms_sql.bind_variable(curs1, ':e1',int_env_r.maxx);
    dbms_sql.bind_variable(curs1, ':e2',int_env_r.maxy);
    dbms_sql.bind_variable(curs1, ':e3',int_env_r.minx);
    dbms_sql.bind_variable(curs1, ':e4',int_env_r.miny);

    dbms_sql.define_array(curs1, 1,SDE.spx_util.fetch_env(curs1).rid_t, 100, 1);

    frows := dbms_sql.execute(curs1);

    curs := curs1;

End execute_spatial;
  
  
Procedure execute_spatial_join (ia              IN  sys.odciindexinfo,
                                op              IN  sys.odcipredinfo,
                                table_name      IN  varchar2,
                                spx_info_r      IN OUT  SDE.spx_util.spx_record_t,
                                sp_ref_r        IN  SDE.spx_util.spatial_ref_record_t,
                                int_env_r       IN  SDE.spx_util.r_env,
                                curs            OUT integer)
  /***********************************************************************
  *
  *n  {exec_spatial_join}  --  executes spatial selection
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure executes a spatial join using the base and 
  *  spatail index table. The call to execute
  *  this query is made outside the static odci function to preserve 
  *  the cursor. 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
IS
    rid             rowid;
    stmt            varchar2(512);
    spcol           varchar2(32);
    nrows           integer;
    curs1           integer := 0;
    pos             pls_integer := 0;
    bcached         boolean := False;
    pgrid_info      sp_grid_info := sp_grid_info(0,0,0);
  
Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = spx_info_r.index_id THEN
          pos := id;

          If (ia.indexpartition IS NOT NULL) THEN
            curs1 := get_partition_curs (pos,ia.indexpartition,curs_type_select_spatial_join,pgrid_info);
            if (curs1 <> 0) Then
              spx_info_r.grid.grid1 := pgrid_info.grid1;
              spx_info_r.grid.grid2 := pgrid_info.grid2;
              spx_info_r.grid.grid3 := pgrid_info.grid3;
            End If;
          ELSE
            If cursor_cache(id).curs_array(1).curs_spatial_join  > 0 THEN
              curs1 := cursor_cache(id).curs_array(1).curs_spatial_join;
            End If;
            Exit;
          End If;
        End If;
      End Loop nlayers;
    End If;

    spcol := spx_info_r.column_name;

    If curs1 = 0 THEN
      IF spx_info_r.grid.grid3 > 0 THEN
        stmt := 'SELECT a.'||spcol||'.entity,a.'||spcol||'.numpts,a.'||spcol||'.minx,a.'||spcol||
                '.maxx,a.'||spcol||'.miny,a.'||spcol||'.maxy,a.'||spcol||'.points,a.'||spcol||'.srid,a.rowid '||
                 'FROM '||spx_info_r.owner||'.'||spx_info_r.table_name||' a '||
                 'WHERE a.rowid IN '||
                   '(SELECT distinct sp_id FROM '||table_name||
                   ' WHERE ((gx >= :b1 AND gx <= :b2 AND gy >= :b3 AND gy <= :b4) OR '||
                           '(gx >= :b5 AND gx <= :b6 AND gy >= :b7 AND gy <= :b8) OR '||
                           '(gx >= :b9 AND gx <= :b10 AND gy >= :b11 AND gy <= :b12) OR (gx < 0 and gy < 0)) '||
                           'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4)';

      ELSIF spx_info_r.grid.grid2 > 0 THEN
        stmt := 'SELECT a.'||spcol||'.entity,a.'||spcol||'.numpts,a.'||spcol||'.minx,a.'||spcol||
                '.maxx,a.'||spcol||'.miny,a.'||spcol||'.maxy,a.'||spcol||'.points,a.'||spcol||'.srid,a.rowid '||
                 'FROM '||spx_info_r.owner||'.'||spx_info_r.table_name||' a '||
                 'WHERE a.rowid IN '||
                   '(SELECT distinct  sp_id FROM '||table_name||
                   ' WHERE ((gx >= :b1 AND gx <= :b2 AND gy >= :b3 AND gy <= :b4) OR '||
                           '(gx >= :b5 AND gx <= :b6 AND gy >= :b7 AND gy <= :b8) OR (gx < 0 and gy < 0)) '||
                           'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4)';

      ELSE
        stmt := 'SELECT a.'||spcol||'.entity,a.'||spcol||'.numpts,a.'||spcol||'.minx,a.'||spcol||
                '.maxx,a.'||spcol||'.miny,a.'||spcol||'.maxy,a.'||spcol||'.points,a.'||spcol||'.srid,a.rowid '||
                 'FROM '||spx_info_r.owner||'.'||spx_info_r.table_name||' a '||
                 'WHERE a.rowid IN '||
                   '(SELECT distinct sp_id FROM '||table_name||' WHERE ((gx >= :b1 AND gx <= :b2 AND '||
                    'gy >= :b3 AND gy <= :b4) OR (gx < 0 and gy < 0)) AND minx <= :e1 AND miny <= :e2 AND '||
                    'maxx >= :e3 AND maxy >= :e4)';
      END IF;

      curs1 := dbms_sql.open_cursor;

      dbms_sql.parse(curs1,stmt,dbms_sql.native);
    End If;

    curs := curs1;
End execute_spatial_join;
  
Procedure execute_spatial_gridorder (ia              IN  sys.odciindexinfo,
                                     table_name      IN  varchar2,
                                     spx_info_r      IN  SDE.spx_util.spx_record_t,
                                     sp_ref_r        IN  SDE.spx_util.spatial_ref_record_t,
                                     int_env_r       IN  SDE.spx_util.r_env,
                                     curs            OUT integer)
  /***********************************************************************
  *
  *n  {exec_spatial_gridorder}  --  executes spatial select in grid order
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure executes a spatial select in spatial index order.
  *  the call to execute this query is made outside the static odci 
  *  function to preserve the cursor. 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     gsize1      <in>     ==  (pls_Integer)  grid size1
  *     gsize2      <in>     ==  (pls_Integer)  grid size2
  *     gsize3      <in>     ==  (pls_Integer)  grid size3
  *     int_Env     <in>     ==  (t_Env) feature envelope/sysunits
  *     f_Row       <in>     ==  (pls_Integer) feat row index
  *     grid        <in out> ==  (r_Env) grid envelope/sysunits
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    Sanjay Magal          03/20/08           original coding.
  *e
  ***********************************************************************/
IS
    rid             rowid;
    stmt            varchar2(512);
    nrows           integer;
    curs1           integer := 0;
    pos             pls_integer := 0;
    bcached         boolean := False;
    grids           SDE.spx_util.v_grids := v_grids(0,0,0);
    levels          SDE.spx_util.v_levels := v_levels(0,0,0);
    grid_env        SDE.spx_util.r_grid_env := SDE.spx_util.r_grid_env();
    n_grids         integer;
  
Begin

    n_grids := 1;
    grid_env.extend(3);

    If spx_info_r.grid.grid2 > 0 THEN
      If spx_info_r.grid.grid3 > 0 THEN
        n_grids := 3;
      ELSE
        n_grids := 2;
      End If;
    End If;

    If n_grids = 1 THEN
      stmt :=  'SELECT sp_id FROM ('||
               'SELECT sp_id,gx,gy,row_number() over (partition by sp_id order by gx,gy) rn '||
               'FROM '||table_name||' WHERE gx >= :1 AND gx <= :2 AND '||
                 'gy >= :3 AND gy <= :4 AND minx <= :e1 AND miny <= :e2 AND '||
                 'maxx >= :e3 AND maxy >= :e4) s_ '||
                 'where s_.rn = 1 '||
                 'order by s_.gx,s_.gy';
    Elsif n_grids = 2 THEN
      stmt := 'SELECT sp_id FROM ('||
                'SELECT sp_id,gx,gy,row_number() over (partition by sp_id order by gx,gy) rn '||
                'FROM '||table_name||
                ' WHERE ((gx >= :1 AND gx <= :2 AND gy >= :3 AND gy <= :4) OR '||
                        '(gx >= :5 AND gx <= :6 AND gy >= :7 AND gy <= :8)) '||
                        'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4) s_ '||
                'where s_.rn = 1 '||
                      'order by s_.gx,s_.gy';
    Elsif n_grids = 3 THEN
      stmt := 'SELECT sp_id FROM ('||
                 'SELECT sp_id,gx,gy,row_number() over (partition by sp_id order by gx,gy) rn '|| 
                 'FROM '||table_name||
                 ' WHERE ((gx >= :1 AND gx <= :2 AND gy >= :3 AND gy <= :4) OR '||
                         '(gx >= :5 AND gx <= :6 AND gy >= :7 AND gy <= :8) OR '||
                         '(gx >= :9 AND gx <= :10 AND gy >= :11 AND gy <= :12)) '||
                         'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4) s_ '||
              'where s_.rn = 1 '||
                       'order by s_.gx,s_.gy';
    Else
      raise_application_error (SDE.st_type_util.spx_invalid_number_of_grids,
                               'ST_GEOMETRY_INDEX Invalid number of Grids');
    End If;

    curs1 := dbms_sql.open_cursor;  
    dbms_sql.parse(curs1,stmt,dbms_sql.native);
    
    SDE.spx_util.fetch_env(curs1).curs := curs1;

    grids(1) := spx_info_r.grid.grid1 * sp_ref_r.xyunits;
    levels(1) := 0;
    If spx_info_r.grid.grid2 > 0 THEN
      grids(2) := spx_info_r.grid.grid2 * sp_ref_r.xyunits;
      levels(2) := grid_level_mask_1;
    Else
      grids(2) := 0;
    End If;

    If spx_info_r.grid.grid3 > 0 THEN
      grids(3) := spx_info_r.grid.grid3 * sp_ref_r.xyunits;
      levels(3) := grid_level_mask_2;
    ELSE
      grids(3) := 0;
    End If;

    FOR x IN 1 .. 3
    Loop
      If grids(x) > 0 THEN
        grid_env(x).minx := trunc(int_env_r.minx / grids(x)) + levels(x);
        grid_env(x).miny := trunc(int_env_r.miny / grids(x)) + levels(x);
        grid_env(x).maxx := trunc(int_env_r.maxx / grids(x)) + levels(x);
        grid_env(x).maxy := trunc(int_env_r.maxy / grids(x)) + levels(x);
      End If;
    End Loop;

    dbms_sql.bind_variable(curs1, ':1',	grid_env(1).minx);
    dbms_sql.bind_variable(curs1, ':2',	grid_env(1).maxx);
    dbms_sql.bind_variable(curs1, ':3',	grid_env(1).miny);
    dbms_sql.bind_variable(curs1, ':4',	grid_env(1).maxy);

    If grids(2) > 0 THEN
      dbms_sql.bind_variable(curs1, ':5',	grid_env(2).minx);
      dbms_sql.bind_variable(curs1, ':6',	grid_env(2).maxx);
      dbms_sql.bind_variable(curs1, ':7',	grid_env(2).miny);
      dbms_sql.bind_variable(curs1, ':8',	grid_env(2).maxy); 
    End If;

    If grids(3) > 0 THEN
      dbms_sql.bind_variable(curs1, ':9',	grid_env(3).minx);
      dbms_sql.bind_variable(curs1, ':10',	grid_env(3).maxx);
      dbms_sql.bind_variable(curs1, ':11',	grid_env(3).miny);
      dbms_sql.bind_variable(curs1, ':12',	grid_env(3).maxy);  
    End If;

    dbms_sql.bind_variable(curs1, ':e1',int_env_r.maxx);
    dbms_sql.bind_variable(curs1, ':e2',int_env_r.maxy);
    dbms_sql.bind_variable(curs1, ':e3',int_env_r.minx);
    dbms_sql.bind_variable(curs1, ':e4',int_env_r.miny);

    dbms_sql.define_array(curs1, 1,SDE.spx_util.fetch_env(curs1).rid_t, 100, 1);

    nrows := dbms_sql.execute(curs1);

    curs := curs1;
End execute_spatial_gridorder;

Function exec_delete (ia              sys.odciindexinfo,
                      idx_name        IN varchar2,
                      spx_info_r      IN spx_record_t,
                      rid             IN varchar2)
  /***********************************************************************
  *
  *n  {exec_Delete}  --  exec delete
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this function delete records from the domain index table by rowid
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
Return number 
IS
    pos          binary_integer := 0;
    curs1        integer := 0;
    stmt         varchar2(128);
    rc           number;
    pgrid_info   sp_grid_info := sp_grid_info(0,0,0);
  
Begin

    If nlayers > 0 THEN
      FOR id IN 1 .. nlayers
      Loop
        If cursor_cache(id).index_id = spx_info_r.index_id THEN
        pos := id;

        If (ia.indexpartition IS NOT NULL) THEN
            curs1 := get_partition_curs (pos,ia.indexpartition,curs_type_delete,pgrid_info);
        ELSE
          If cursor_cache(id).curs_array(1).curs_delete  > 0 THEN
            curs1 := cursor_cache(id).curs_array(1).curs_delete;
            End If;
            Exit;
          End If;
        End If;
      End Loop nlayers;
    End If;

    If curs1 = 0 THEN
      stmt := 'DELETE FROM '||idx_name ||' WHERE sp_id = :sp_row';

      curs1 := dbms_sql.open_cursor;
      dbms_sql.parse(curs1,stmt,dbms_sql.native);

      If (ia.indexpartition IS NOT NULL) THEN
         SDE.spx_util.set_partition_curs(pos,curs1,ia.indexpartition,spx_info_r,curs_type_delete);
      ELSE
        SDE.spx_util.cursor_cache(pos).curs_array(1).curs_delete := curs1;
      End If;
    End If;

    dbms_sql.bind_variable_rowid(curs1, 'sp_row', rid);
    rc := dbms_sql.execute(curs1);

    Return se_success;

End exec_delete;
  
Procedure insert_index (spx_info_r     IN Out spx_record_t)
/***********************************************************************
  *
  *n  {insert_Index}  --  insert into st_Spatail_Index table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              insert's into st_Geometry_Index table
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/		 
IS
  
Begin

    INSERT INTO SDE.st_geometry_index 
         (owner,table_name,column_name,grid,srid,commit_int,index_name)
    VALUES (spx_info_r.owner,spx_info_r.table_name,spx_info_r.column_name,
            spx_info_r.grid,spx_info_r.srid,spx_info_r.commit_int,
           spx_info_r.index_name)
    returning index_id INTO spx_info_r.index_id;

    COMMIT;

  EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      NULL;

End insert_index;

Procedure update_index (spx_info_r   IN  spx_record_t,
                        owner_in     IN spx_owner_t,
                        table_in     IN spx_table_t,
                        column_in    IN spx_column_t)
/***********************************************************************
  *
  *n  {update_Index}  --  Update st_Spatail_Index table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              Updates's into st_Geometry_Index table
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/		 
IS
  
Begin
  
    UPDATE SDE.st_geometry_index 
      SET grid = spx_info_r.grid, commit_int = spx_info_r.commit_int
    WHERE owner = owner_in AND table_name = table_in AND
          column_name = column_in;

End update_index;
  
  
Procedure delete_index (del_index_id     IN integer)
/***********************************************************************
  *
  *n  {delete_Index}  --  delete from st_Spatail_Index table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              delete from st_Geometry_Index table
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/		 
IS
  
Begin
  
    DELETE FROM SDE.st_geometry_index
    WHERE index_id = del_index_id;

End delete_index;
  
/***********************************************************************
  *
  *n  {set_column_stats}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure set_column_stats (owner          IN VARCHAR2,
                              table_name     IN VARCHAR2,
                              column_name    IN VARCHAR2,
                              distcnt        IN NUMBER,
                              nullcnt        IN NUMBER)		 
  IS

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
  
  BEGIN

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20000, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' column '||UPPER(column_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;
  
    IF distcnt IS NOT NULL THEN

      IF distcnt < 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'distcnt value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET distinct_keys = ROUND(distcnt),
      user_stats = 'YES', last_analyzed = SYSDATE 
      WHERE owner = UPPER(set_column_stats.owner)
      AND table_name = UPPER(set_column_stats.table_name)
      AND column_name = UPPER(set_column_stats.column_name);
    END IF;

    IF nullcnt IS NOT NULL THEN

      IF nullcnt < 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'nullcnt value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET num_nulls = ROUND(nullcnt),
      user_stats = 'YES', last_analyzed = SYSDATE
      WHERE owner = UPPER(set_column_stats.owner)
      AND table_name = UPPER(set_column_stats.table_name)
      AND column_name = UPPER(set_column_stats.column_name);
    END IF;

    COMMIT;
    
  END set_column_stats;

/***********************************************************************
  *
  *n  {set_index_stats}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure set_index_stats (owner          IN VARCHAR2,
                             table_name     IN VARCHAR2,
                             index_name     IN VARCHAR2,
                             numrows        IN NUMBER,
                             numlblks       IN NUMBER,
                             clstfct        IN NUMBER,
                             density        IN NUMBER,
                             indlevel       IN NUMBER)		 
  IS

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         index_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND index_name = index_wanted;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
  
  BEGIN

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(index_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20000, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' index '||UPPER(index_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;
  
    IF numrows IS NOT NULL THEN

      IF numrows < 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'numrows value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET num_rows = numrows,
      user_stats = 'YES', last_analyzed = SYSDATE  
      WHERE owner = UPPER(set_index_stats.owner)
      AND table_name = UPPER(set_index_stats.table_name)
      AND index_name = UPPER(set_index_stats.index_name);
    END IF;

    IF numlblks IS NOT NULL THEN

      IF numlblks < 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'numlblks value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET leaf_blocks = ROUND(numlblks),
      user_stats = 'YES', last_analyzed = SYSDATE
      WHERE owner = UPPER(set_index_stats.owner)
      AND table_name = UPPER(set_index_stats.table_name)
      AND index_name = UPPER(set_index_stats.index_name);
    END IF;

    IF clstfct IS NOT NULL THEN

      IF clstfct < 0 THEN
        RAISE_APPLICATION_ERROR(-20003, 'clstfct value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET clustering_factor = clstfct,
      user_stats = 'YES', last_analyzed = SYSDATE 
      WHERE owner = UPPER(set_index_stats.owner)
      AND table_name = UPPER(set_index_stats.table_name)
      AND index_name = UPPER(set_index_stats.index_name);
    END IF;

    IF density IS NOT NULL THEN

      IF density < 0 THEN
        RAISE_APPLICATION_ERROR(-20004, 'density value must be equal or greater than 0.');
      END IF;

      UPDATE SDE.st_geometry_index SET density = ROUND(set_index_stats.density,2),
      user_stats = 'YES', last_analyzed = SYSDATE
      WHERE owner = UPPER(set_index_stats.owner)
      AND table_name = UPPER(set_index_stats.table_name)
      AND index_name = UPPER(set_index_stats.index_name);
    END IF;

    IF indlevel IS NOT NULL THEN

      IF indlevel < 1 THEN
        RAISE_APPLICATION_ERROR(-20005, 'indlevel value must be equal or greater than 1.');
      END IF;

      UPDATE SDE.st_geometry_index SET blevel = ROUND(indlevel),
      user_stats = 'YES', last_analyzed = SYSDATE
      WHERE owner = UPPER(set_index_stats.owner)
      AND table_name = UPPER(set_index_stats.table_name)
      AND index_name = UPPER(set_index_stats.index_name);
    END IF;

    COMMIT;

  END set_index_stats;

/***********************************************************************
  *
  *n  {set_operator_cost}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure set_operator_cost (owner          IN VARCHAR2,
                               table_name     IN VARCHAR2,
                               column_name    IN VARCHAR2,
                               operator_name  IN VARCHAR2,
                               cost           IN INTEGER)		 
  IS

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    loc_st_funcs  SDE.st_funcs_array;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
    pos1        NUMBER;
    pos2        NUMBER;
    lng         NUMBER;
    op_found    BOOLEAN DEFAULT FALSE;
    sel         VARCHAR2(28);
  
  BEGIN

    IF UPPER(operator_name) <> 'ST_ENVINTERSECTS' AND 
       UPPER(operator_name) <> 'ST_INTERSECTS' AND
       UPPER(operator_name) <> 'ST_OVERLAPS' AND
       UPPER(operator_name) <> 'ST_CONTAINS' AND
       UPPER(operator_name) <> 'ST_WITHIN' AND
       UPPER(operator_name) <> 'ST_TOUCHES' AND 
       UPPER(operator_name) <> 'ST_DISTANCE' AND 
       UPPER(operator_name) <> 'ST_ORDERINEQUALS' AND 
       UPPER(operator_name) <> 'ST_EQUALS' AND
       UPPER(operator_name) <> 'ST_DISJOINT' AND 
       UPPER(operator_name) <> 'ST_CROSSES' AND
       UPPER(operator_name) <> 'ST_RELATE' THEN
      RAISE_APPLICATION_ERROR(-20000, 'Invalid operator_name specified, '||operator_name||'.');
    END IF;

    IF cost <= 0 THEN
      RAISE_APPLICATION_ERROR(-20001, 'COST value must be greater than 0.');
    END IF;

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20002, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;

    loc_st_funcs := SDE.st_funcs_array();
  
    IF geom_stats.st_funcs IS NULL THEN 
      UPDATE SDE.st_geometry_index 
      SET st_funcs = SDE.ST_FUNCS_ARRAY(''||UPPER(operator_name)||':'||TO_CHAR(ROUND(cost))||',NULL')
      WHERE owner = UPPER(set_operator_cost.owner)
      AND table_name = UPPER(set_operator_cost.table_name)
      AND column_name = UPPER(set_operator_cost.column_name);
     ELSE
      FOR i IN geom_stats.st_funcs.FIRST..geom_stats.st_funcs.LAST LOOP
        pos1 := INSTR(geom_stats.st_funcs(i),':');
        IF SUBSTR(geom_stats.st_funcs(i),1,pos1 - 1) = UPPER(operator_name) THEN
          pos2 := INSTR(geom_stats.st_funcs(i),',');
          lng := LENGTH(geom_stats.st_funcs(i));
          sel := SUBSTR(geom_stats.st_funcs(i),-(lng - pos2),lng - pos2);
          geom_stats.st_funcs(i) := ''||UPPER(operator_name)||':'||TO_CHAR(ROUND(cost))||','||sel||'';
          UPDATE SDE.st_geometry_index SET st_funcs = geom_stats.st_funcs
          WHERE owner = UPPER(set_operator_cost.owner) 
          AND table_name = UPPER(set_operator_cost.table_name)
          AND column_name = UPPER(set_operator_cost.column_name);
          op_found := TRUE;     
          EXIT;
         ELSE
          loc_st_funcs.EXTEND(1);
          loc_st_funcs(loc_st_funcs.COUNT) := geom_stats.st_funcs(i);
        END IF;
      END LOOP;
    END IF;

    IF op_found = FALSE AND loc_st_funcs.COUNT > 0 THEN
      loc_st_funcs.EXTEND(1);
      loc_st_funcs(loc_st_funcs.COUNT) := ''||UPPER(operator_name)||':'||TO_CHAR(ROUND(cost))||',NULL';
      UPDATE SDE.st_geometry_index SET st_funcs = loc_st_funcs
      WHERE owner = UPPER(set_operator_cost.owner) 
      AND table_name = UPPER(set_operator_cost.table_name)
      AND column_name = UPPER(set_operator_cost.column_name);
    END IF;

    COMMIT;

  END set_operator_cost;

/***********************************************************************
  *
  *n  {delete_operator_cost}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure delete_operator_cost (owner          IN VARCHAR2,
                                  table_name     IN VARCHAR2,
                                  column_name    IN VARCHAR2,
                                  operator_name  IN VARCHAR2)		 
  IS

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    loc_st_funcs  SDE.st_funcs_array;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
    pos1        NUMBER;
    pos2        NUMBER;
    lng         NUMBER;
    op_found    BOOLEAN DEFAULT FALSE;
    sel         VARCHAR2(28);
  
  BEGIN

    IF UPPER(operator_name) <> 'ST_ENVINTERSECTS' AND 
       UPPER(operator_name) <> 'ST_INTERSECTS' AND
       UPPER(operator_name) <> 'ST_OVERLAPS' AND
       UPPER(operator_name) <> 'ST_CONTAINS' AND
       UPPER(operator_name) <> 'ST_WITHIN' AND
       UPPER(operator_name) <> 'ST_TOUCHES' AND 
       UPPER(operator_name) <> 'ST_DISTANCE' AND 
       UPPER(operator_name) <> 'ST_ORDERINEQUALS' AND 
       UPPER(operator_name) <> 'ST_EQUALS' AND
       UPPER(operator_name) <> 'ST_DISJOINT' AND 
       UPPER(operator_name) <> 'ST_CROSSES' AND
       UPPER(operator_name) <> 'ST_RELATE' THEN
      RAISE_APPLICATION_ERROR(-20000, 'Invalid operator_name specified, '||operator_name||'.');
    END IF;

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20001, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;

    loc_st_funcs := SDE.st_funcs_array();

    IF geom_stats.st_funcs IS NOT NULL THEN
      FOR i IN geom_stats.st_funcs.FIRST..geom_stats.st_funcs.LAST LOOP
        pos1 := INSTR(geom_stats.st_funcs(i),':');
        IF SUBSTR(geom_stats.st_funcs(i),1,pos1 - 1) = UPPER(operator_name) THEN
          op_found := TRUE;
          pos2 := INSTR(geom_stats.st_funcs(i),',');
          lng := LENGTH(geom_stats.st_funcs(i));
          sel := SUBSTR(geom_stats.st_funcs(i),-(lng - pos2),lng - pos2);
          IF sel <> 'NULL' THEN
            loc_st_funcs.EXTEND(1);
            loc_st_funcs(loc_st_funcs.COUNT) := ''||UPPER(operator_name)||':NULL,'||sel||'';
          END IF;
         ELSE
          loc_st_funcs.EXTEND(1);
          loc_st_funcs(loc_st_funcs.COUNT) := geom_stats.st_funcs(i);
        END IF;
      END LOOP;

      IF op_found = FALSE THEN
        RAISE_APPLICATION_ERROR(-20002, 'Operator '||UPPER(operator_name)||' not found.');
      END IF;

      IF loc_st_funcs.COUNT = 0 THEN
        UPDATE SDE.st_geometry_index SET st_funcs = NULL
        WHERE owner = UPPER(delete_operator_cost.owner) 
        AND table_name = UPPER(delete_operator_cost.table_name)
        AND column_name = UPPER(delete_operator_cost.column_name);
       ELSE
        UPDATE SDE.st_geometry_index SET st_funcs = loc_st_funcs
        WHERE owner = UPPER(delete_operator_cost.owner) 
        AND table_name = UPPER(delete_operator_cost.table_name)
        AND column_name = UPPER(delete_operator_cost.column_name);
      END IF;
      
      COMMIT;
     ELSE
      RAISE_APPLICATION_ERROR(-20002, 'Operator '||UPPER(operator_name)||' not found.');
    END IF;

  END delete_operator_cost;

/***********************************************************************
  *
  *n  {set_operator_selectivity}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure set_operator_selectivity (owner          IN VARCHAR2,
                                      table_name     IN VARCHAR2,
                                      column_name    IN VARCHAR2,
                                      operator_name  IN VARCHAR2,
                                      sel            IN INTEGER)		 
  IS

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    loc_st_funcs  SDE.st_funcs_array;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
    pos1        NUMBER;
    pos2        NUMBER;
    lng         NUMBER;
    op_found    BOOLEAN DEFAULT FALSE;
    cost        VARCHAR2(28);
  
  BEGIN

    IF UPPER(operator_name) <> 'ST_ENVINTERSECTS' AND 
       UPPER(operator_name) <> 'ST_INTERSECTS' AND
       UPPER(operator_name) <> 'ST_OVERLAPS' AND
       UPPER(operator_name) <> 'ST_CONTAINS' AND
       UPPER(operator_name) <> 'ST_WITHIN' AND
       UPPER(operator_name) <> 'ST_TOUCHES' AND 
       UPPER(operator_name) <> 'ST_DISTANCE' AND 
       UPPER(operator_name) <> 'ST_ORDERINEQUALS' AND 
       UPPER(operator_name) <> 'ST_EQUALS' AND
       UPPER(operator_name) <> 'ST_DISJOINT' AND 
       UPPER(operator_name) <> 'ST_CROSSES' AND
       UPPER(operator_name) <> 'ST_RELATE' THEN
      RAISE_APPLICATION_ERROR(-20000, 'Invalid operator_name specified, '||operator_name||'.');
    END IF;

    IF sel < 0 OR sel > 100 THEN
      RAISE_APPLICATION_ERROR(-20001, 'SEL value must be between 0 and 100.');
    END IF;
 
    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20002, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;

    loc_st_funcs := SDE.st_funcs_array();
  
    IF geom_stats.st_funcs IS NULL THEN 
      UPDATE SDE.st_geometry_index 
      SET st_funcs = SDE.ST_FUNCS_ARRAY(''||UPPER(operator_name)||':NULL,'||TO_CHAR(sel))
      WHERE owner = UPPER(set_operator_selectivity.owner) 
      AND table_name = UPPER(set_operator_selectivity.table_name)
      AND column_name = UPPER(set_operator_selectivity.column_name);
     ELSE
      FOR i IN geom_stats.st_funcs.FIRST..geom_stats.st_funcs.LAST LOOP
        pos1 := INSTR(geom_stats.st_funcs(i),':');
        IF SUBSTR(geom_stats.st_funcs(i),1,pos1 - 1) = UPPER(operator_name) THEN
          pos2 := INSTR(geom_stats.st_funcs(i),',');
          lng := LENGTH(geom_stats.st_funcs(i));
          cost := SUBSTR(geom_stats.st_funcs(i),-(lng - pos1),pos2 - (pos1 + 1));
          geom_stats.st_funcs(i) := ''||UPPER(operator_name)||':'||cost||','||TO_CHAR(sel)||'';
          UPDATE SDE.st_geometry_index SET st_funcs = geom_stats.st_funcs
          WHERE owner = UPPER(set_operator_selectivity.owner) 
          AND table_name = UPPER(set_operator_selectivity.table_name)
          AND column_name = UPPER(set_operator_selectivity.column_name);
          op_found := TRUE;
          EXIT;
         ELSE
          loc_st_funcs.EXTEND(1);
          loc_st_funcs(loc_st_funcs.COUNT) := geom_stats.st_funcs(i);
        END IF;
      END LOOP;
    END IF;

    IF op_found = FALSE AND loc_st_funcs.COUNT > 0 THEN
      loc_st_funcs.EXTEND(1);
      loc_st_funcs(loc_st_funcs.COUNT) := ''||UPPER(operator_name)||':NULL,'||TO_CHAR(sel)||'';
      UPDATE SDE.st_geometry_index SET st_funcs = loc_st_funcs
      WHERE owner = UPPER(set_operator_selectivity.owner) 
      AND table_name = UPPER(set_operator_selectivity.table_name)
      AND column_name = UPPER(set_operator_selectivity.column_name);
    END IF;

    COMMIT;

  END set_operator_selectivity;

/***********************************************************************
  *
  *n  {delete_operator_selectivity}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure delete_operator_selectivity (owner          IN VARCHAR2,
                                         table_name     IN VARCHAR2,
                                         column_name    IN VARCHAR2,
                                         operator_name  IN VARCHAR2)		 
  IS
  
    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2,
                         column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    loc_st_funcs  SDE.st_funcs_array;

    geom_stats  SDE.st_geometry_index%ROWTYPE;
    pos1        NUMBER;
    pos2        NUMBER;
    lng         NUMBER;
    op_found    BOOLEAN DEFAULT FALSE;
    cost        VARCHAR2(28);

  BEGIN

    IF UPPER(operator_name) <> 'ST_ENVINTERSECTS' AND 
       UPPER(operator_name) <> 'ST_INTERSECTS' AND
       UPPER(operator_name) <> 'ST_OVERLAPS' AND
       UPPER(operator_name) <> 'ST_CONTAINS' AND
       UPPER(operator_name) <> 'ST_WITHIN' AND
       UPPER(operator_name) <> 'ST_TOUCHES' AND 
       UPPER(operator_name) <> 'ST_DISTANCE' AND 
       UPPER(operator_name) <> 'ST_ORDERINEQUALS' AND 
       UPPER(operator_name) <> 'ST_EQUALS' AND
       UPPER(operator_name) <> 'ST_DISJOINT' AND 
       UPPER(operator_name) <> 'ST_CROSSES' AND
       UPPER(operator_name) <> 'ST_RELATE' THEN
      RAISE_APPLICATION_ERROR(-20000, 'Invalid operator_name specified, '||operator_name||'.');
    END IF;

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20001, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;

    loc_st_funcs := SDE.st_funcs_array();

    IF geom_stats.st_funcs IS NOT NULL THEN
      FOR i IN geom_stats.st_funcs.FIRST..geom_stats.st_funcs.LAST LOOP
        pos1 := INSTR(geom_stats.st_funcs(i),':');
        IF SUBSTR(geom_stats.st_funcs(i),1,pos1 - 1) = UPPER(operator_name) THEN
          op_found := TRUE;
          pos2 := INSTR(geom_stats.st_funcs(i),',');
          lng := LENGTH(geom_stats.st_funcs(i));
          cost := SUBSTR(geom_stats.st_funcs(i),-(lng - pos1),pos2 - (pos1 + 1));
          IF cost <> 'NULL' THEN
            loc_st_funcs.EXTEND(1);
            loc_st_funcs(loc_st_funcs.COUNT) := ''||UPPER(operator_name)||':'||cost||',NULL';
          END IF;
         ELSE
          loc_st_funcs.EXTEND(1);
          loc_st_funcs(loc_st_funcs.COUNT) := geom_stats.st_funcs(i);
        END IF;
      END LOOP;

      IF op_found = FALSE THEN
        RAISE_APPLICATION_ERROR(-20002, 'Operator '||UPPER(operator_name)||' not found.');
      END IF;

      IF loc_st_funcs.COUNT = 0 THEN
        UPDATE SDE.st_geometry_index SET st_funcs = NULL
        WHERE owner = UPPER(delete_operator_selectivity.owner) 
        AND table_name = UPPER(delete_operator_selectivity.table_name)
        AND column_name = UPPER(delete_operator_selectivity.column_name);
       ELSE
        UPDATE SDE.st_geometry_index SET st_funcs = loc_st_funcs
        WHERE owner = UPPER(delete_operator_selectivity.owner) 
        AND table_name = UPPER(delete_operator_selectivity.table_name)
        AND column_name = UPPER(delete_operator_selectivity.column_name);
      END IF;
     
      COMMIT;
     ELSE
      RAISE_APPLICATION_ERROR(-20002, 'Operator '||UPPER(operator_name)||' not found.');
    END IF;

  End delete_operator_selectivity;

/***********************************************************************
  *
  *n  {rename_spatial_index}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure rename_spatial_index (owner          IN varchar2,
                                  table_name     IN varchar2,
                                  column_name    IN varchar2,
                                  new_index_name IN varchar2)		 
  IS

   CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2, column_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted AND column_name = column_wanted;

   CURSOR c1 (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2, index_wanted IN VARCHAR2) IS
      SELECT COUNT(*)
      FROM all_indexes
      WHERE owner = owner_wanted AND table_name = table_wanted AND index_name = index_wanted
      AND ityp_name = 'ST_SPATIAL_INDEX';

    geom_stats  SDE.st_geometry_index%ROWTYPE;
    cnt         NUMBER;
  
  BEGIN

    OPEN c_geom_stats (UPPER(owner), UPPER(table_name), UPPER(column_name));
    FETCH c_geom_stats INTO geom_stats;
    IF c_geom_stats%NOTFOUND THEN
      CLOSE c_geom_stats;
      RAISE_APPLICATION_ERROR(-20000, 'Table '||LOWER(owner)||'.'||UPPER(table_name)||' not found.');
    END IF;
    CLOSE c_geom_stats;
  
    IF USER != geom_stats.owner THEN
      IF USER != 'SDE' THEN
        RAISE_APPLICATION_ERROR(-20001, 'User does not have privileges to rename index.');
      END IF;
    END IF;

    cnt := 0;
    OPEN c1 (UPPER(owner), UPPER(table_name), UPPER(new_index_name));
    FETCH c1 INTO cnt;
    IF c1%NOTFOUND OR cnt <> 1 THEN
      CLOSE c1;
      RAISE_APPLICATION_ERROR(-20002, 'Index '||UPPER(new_index_name)||' not found.'); 
    END IF;
    CLOSE c1;

    UPDATE SDE.st_geometry_index SET index_name = new_index_name
    WHERE owner = UPPER(rename_spatial_index.owner)
    AND table_name = UPPER(rename_spatial_index.table_name)
    AND column_name = UPPER(rename_spatial_index.column_name);

    COMMIT;

  END rename_spatial_index;

/***********************************************************************
  *
  *n  {rename_spatial_table}  --  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure rename_spatial_table (owner  IN varchar2,
                                  table_name IN varchar2,
                                  new_table_name IN varchar2)		 
  IS

   CURSOR c_geom_indx (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted;

   CURSOR c_geom_cols (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2) IS
      SELECT *
      FROM   SDE.st_geometry_columns
      WHERE  owner = owner_wanted AND table_name = table_wanted;

   CURSOR c1 (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2) IS
      SELECT COUNT(*)
      FROM all_tables
      WHERE owner = owner_wanted AND table_name = table_wanted;

   geom_stats  SDE.st_geometry_index%ROWTYPE;
   geom_cols  SDE.st_geometry_columns%ROWTYPE;
   cnt         NUMBER;
   spx         BOOLEAN DEFAULT FALSE;
  
  BEGIN

    OPEN c1 (UPPER(owner), UPPER(new_table_name));
    FETCH c1 INTO cnt;
    CLOSE c1;

    IF cnt = 0 THEN
      RAISE_APPLICATION_ERROR(-20002, 'Table '||LOWER(owner)||'.'||UPPER(new_table_name)||' not found.');
    END IF;

    OPEN c_geom_indx (UPPER(owner), UPPER(table_name));
    FETCH c_geom_indx INTO geom_stats;

    IF c_geom_indx%FOUND THEN

      IF USER != geom_stats.owner THEN
        IF USER != 'SDE' THEN
        RAISE_APPLICATION_ERROR(-20001, 'User does not have privileges to rename table, must be the owner  or SDE.');
         END IF;
      END IF;

      UPDATE SDE.st_geometry_index SET table_name = NULL
      WHERE owner = UPPER(rename_spatial_table.owner)
      AND table_name = UPPER(rename_spatial_table.table_name);

      spx := TRUE;

    END IF;
    CLOSE c_geom_indx;
  
    OPEN c_geom_cols (UPPER(owner), UPPER(table_name));
    FETCH c_geom_cols INTO geom_cols;

    IF c_geom_cols%FOUND THEN

      IF USER != geom_cols.owner THEN
         IF USER != 'SDE' THEN
        RAISE_APPLICATION_ERROR(-20001, 'User does not have privileges to rename table, must be the owner  or SDE.');
         END IF;
      END IF;

      UPDATE SDE.ALL_ST_GEOMETRY_COLUMNS_V SET table_name = new_table_name
      WHERE owner = UPPER(rename_spatial_table.owner)
      AND table_name = UPPER(rename_spatial_table.table_name);

      IF spx = TRUE THEN
        UPDATE SDE.st_geometry_index SET table_name = new_table_name
        WHERE owner = UPPER(rename_spatial_table.owner)
        AND table_name IS NULL;
      END IF;

    END IF;
    CLOSE c_geom_cols;

    COMMIT;

  END rename_spatial_table;

/***********************************************************************
  *
  *n  {gen_cell_arrays}  --  Generate parent and tesselated cells 
  *                          containing Exterior, Boundary and Interior
  *                          information based on the input shape and 
  *                          grid_info. 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Procedure gen_cell_arrays              (shape               IN SDE.st_geometry,
                                          operation           IN Integer,
                                          distance            IN number,
                                          gsize1              IN Integer,
                                          gsize2              IN Integer,
                                          gsize3              IN Integer,
                                          num_grid_cell       IN Out Integer,
                                          grid_cell_dem       IN Out SDE.spx_util.grid_cell_t,
                                          gen_grid1           IN OUT number,
                                          gen_grid2           IN OUT number,
                                          gen_grid3           IN OUT number)
IS
  parent_cell_t       SDE.int_array_tab := SDE.int_array_tab();
  tess_cell_t         SDE.int_array_tab := SDE.int_array_tab();
  spref               SDE.spx_util.spatial_ref_record_t;
  mbr                 SDE.spx_util.r_env;
  num_tess_cell       integer;
  value               number;
  next_pos            pls_integer; 
  rc                  pls_integer;
  pos                 pls_integer := 0;
  lgx                 integer;
  lgy                 integer;
  t                   pls_integer;
  grids               SDE.spx_util.v_grids := v_grids(0,0,0);
  levels              SDE.spx_util.v_levels := v_levels(0,0,0);
  grid_env            SDE.spx_util.r_grid_env := SDE.spx_util.r_grid_env();
  minx                integer;
  miny                integer;
  maxx                integer;
  maxy                integer;
  f_minx              float(64);
  f_miny              float(64);
  f_maxx              float(64);
  f_maxy              float(64);
  gx_min              integer;
  gx_max              integer;
  gy_min              integer;
  gy_max              integer;
  a_val               integer;
  b_val               integer;
  z_val               integer;
  y_val               integer;
  grid_cell_zero      integer := 0;

BEGIN

  num_grid_cell := NULL;
  spref.srid := shape.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  gen_grid1 := NULL;
  gen_grid2 := NULL;
  gen_grid3 := NULL;

  SDE.st_geometry_shapelib_pkg.gen_cell_arrays(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                               spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                               spref.Definition,shape.numpts,shape.entity,shape.minx,
                                               shape.miny,shape.maxx,shape.maxy,shape.points,operation,
                                               distance,gsize1,gsize2,gsize3,num_grid_cell,parent_cell_t,
                                               num_tess_cell,tess_cell_t,gen_grid1,gen_grid2,gen_grid3);
  If num_grid_cell IS NOT NULL Then
    
    If operation = SDE.st_geom_util.buffer_intersects_c Then
      f_minx := shape.minx - distance;
      f_miny := shape.miny - distance;
      f_maxx := shape.maxx + distance;
      f_maxy := shape.maxy + distance;
    Else
      f_minx := shape.minx;
      f_miny := shape.miny;
      f_maxx := shape.maxx;
      f_maxy := shape.maxy;
    End If;

    minx := ((f_minx - spref.x_offset) * spref.xyunits + 0.5);
    miny := ((f_miny - spref.y_offset) * spref.xyunits + 0.5);
    maxx := ((f_maxx - spref.x_offset) * spref.xyunits + 0.5);
    maxy := ((f_maxy - spref.y_offset) * spref.xyunits + 0.5);

    if (gen_grid1 IS NOT NULL) Then
      grids(1) := gen_grid1 * spref.xyunits;
      gen_grid1 := gen_grid1 * spref.xyunits;
    Else
      grids(1) := gsize1 * spref.xyunits;
    End If;
    levels(1) := 0;

    If gsize2 > 0 THEN
      If (gen_grid2 IS NOT NULL) Then
        grids(2) := gen_grid2 * spref.xyunits;
        gen_grid2 := gen_grid2 * spref.xyunits;
      Else
        grids(2) := gsize2 * spref.xyunits;
      End If;
      levels(2) := grid_level_mask_1;
    Else
      grids(2) := 0;
    End If;

    If gsize3 > 0 THEN
      If (gen_grid3 IS NOT NULL) Then
        grids(3) := gen_grid3 * spref.xyunits;
        gen_grid3 := gen_grid3 * spref.xyunits;
      Else
        grids(3) := gsize3 * spref.xyunits;
      End If;
      levels(3) := grid_level_mask_2;
    Else
      grids(3) := 0;
    End If;
    
    grid_env.extend(3);
    SDE.spx_util.hashgrid_key_min := v_grids(16777216,33554432,2147483647);
    SDE.spx_util.hashgrid_key_max := SDE.spx_util.v_grids(0,0,0);
    
    FOR x IN 1 .. 3
    Loop
      If grids(x) > 0 THEN
        grid_env(x).minx := trunc(minx / grids(x)) + levels(x);
        grid_env(x).miny := trunc(miny / grids(x)) + levels(x);
        grid_env(x).maxx := trunc(maxx / grids(x)) + levels(x);
        grid_env(x).maxy := trunc(maxy / grids(x)) + levels(x);
      End If;
    End Loop;
  
    For i in 0..num_grid_cell - 1 Loop
    
      lgx := parent_cell_t((i * 5) + 1);
      lgy := parent_cell_t((i * 5) + 2);

      If(gsize1  > 0 AND gsize2 = 0 AND gsize3 = 0) Then
        gx_min := grid_env(1).minx;
        gx_max := grid_env(1).maxx;
        gy_min := grid_env(1).miny;
        gy_max := grid_env(1).maxy;
      Elsif(gsize2 > 0 AND gsize3 = 0) Then
        If (lgx < grid_level_mask_1) Then
          gx_min := grid_env(1).minx;
          gx_max := grid_env(1).maxx;
          gy_min := grid_env(1).miny;
          gy_max := grid_env(1).maxy;
        Else
          gx_min := grid_env(2).minx;
          gx_max := grid_env(2).maxx;
          gy_min := grid_env(2).miny;
          gy_max := grid_env(2).maxy;
        End If;
      Else 
        If (lgx < grid_level_mask_1) Then
          gx_min := grid_env(1).minx;
          gx_max := grid_env(1).maxx;
          gy_min := grid_env(1).miny;
          gy_max := grid_env(1).maxy;
        Elsif (lgx < grid_level_mask_2) Then
          gx_min := grid_env(2).minx;
          gx_max := grid_env(2).maxx;
          gy_min := grid_env(2).miny;
          gy_max := grid_env(2).maxy;
        Else
          gx_min := grid_env(3).minx;
          gx_max := grid_env(3).maxx;
          gy_min := grid_env(3).miny;
          gy_max := grid_env(3).maxy;  
        End if;    
      End If;

      a_val := (lgx - gx_min) + 1;
      b_val := (lgy - gy_min) + 1;

      z_val := b_val * ((gx_max - gx_min) +1);
      y_val := ((gx_max - gx_min) + 1) - a_val;

      If(lgx < grid_level_mask_1) Then
        value := z_val - y_val;

        if value < SDE.spx_util.hashgrid_key_min(1) Then
          SDE.spx_util.hashgrid_key_min(1) := value;
        End if;
        If value > SDE.spx_util.hashgrid_key_max(1) Then
          SDE.spx_util.hashgrid_key_max(1) := value;
        End If;
        
      Elsif (lgx < grid_level_mask_2) Then
        value := (z_val - y_val) + grid_level_mask_1;
        
        if value < SDE.spx_util.hashgrid_key_min(2) Then
          SDE.spx_util.hashgrid_key_min(1) := value;
        End if;
        If value > SDE.spx_util.hashgrid_key_max(2) Then
          SDE.spx_util.hashgrid_key_max(1) := value;
        End If;
        
      Else
        value := (z_val - y_val) + grid_level_mask_2;
        if value < SDE.spx_util.hashgrid_key_min(3) Then
          SDE.spx_util.hashgrid_key_min(1) := value;
        End if;
        If value > SDE.spx_util.hashgrid_key_max(3) Then
          SDE.spx_util.hashgrid_key_max(1) := value;
        End If;    
      End If;
      
      if value > SDE.spx_util.max_hash_value Then
        SDE.spx_util.max_hash_value := value;
      End If;
     
      if(value < 0) then
        raise_application_error (SDE.st_type_util.spx_invalid_tess_grid,'Invalid grid tessellation sequence. Hash value is 0.');
      End If;  
      
      If (grid_cell_dem.exists(value)) Then
        next_pos := grid_cell_dem(value).t_cell.last;
                
        t := next_pos + 1;
        For x in parent_cell_t((i * 5) + 4)..parent_cell_t((i * 5) + 5) - 1 Loop

          If lgx = tess_cell_t((x * 7) + 1) And lgy = tess_cell_t((x * 7) + 2) Then
            grid_cell_dem(value).t_cell(t).dem := tess_cell_t((x * 7) + 3);
            grid_cell_dem(value).t_cell(t).minx := tess_cell_t((x * 7) + 4);
            grid_cell_dem(value).t_cell(t).miny := tess_cell_t((x * 7) + 5);
            grid_cell_dem(value).t_cell(t).maxx := tess_cell_t((x * 7) + 6);
            grid_cell_dem(value).t_cell(t).maxy := tess_cell_t((x * 7) + 7);
              
            t := t + 1;
          Else
            raise_application_error (SDE.st_type_util.spx_invalid_tess_grid,'Invalid grid tessellation sequence.');
          End If;

        End Loop;
      
      Else
        grid_cell_dem(value).hashkey := value;
        grid_cell_dem(value).dem := parent_cell_t((i * 5) + 3);
         
        If grid_cell_dem(value).dem = SDE.spx_util.boundary_case Then
          t := 1;
          
          For x in parent_cell_t((i * 5) + 4)..parent_cell_t((i * 5) + 5) - 1 Loop
            If lgx = tess_cell_t((x * 7) + 1) And lgy = tess_cell_t((x * 7) + 2) Then
              grid_cell_dem(value).t_cell(t).dem := tess_cell_t((x * 7) + 3);
              grid_cell_dem(value).t_cell(t).minx := tess_cell_t((x * 7) + 4);
              grid_cell_dem(value).t_cell(t).miny := tess_cell_t((x * 7) + 5);
              grid_cell_dem(value).t_cell(t).maxx := tess_cell_t((x * 7) + 6);
              grid_cell_dem(value).t_cell(t).maxy := tess_cell_t((x * 7) + 7);
              
              t := t + 1;
            Else
              raise_application_error (SDE.st_type_util.spx_invalid_tess_grid,'Invalid grid tessellation sequence.');
            End If;

          End Loop;
        
        Else
     
          grid_cell_dem(value).t_cell(1).dem := SDE.spx_util.interior_case;
        
        End If;
      End If; 
    End Loop;
    
  Else
    grid_cell_zero := grid_cell_zero + 1;
  End If;

End gen_cell_arrays;

Procedure grid_search_prepare (ia              IN  sys.odciindexinfo,
                               table_name      IN  varchar2,
                               spx_info_r      IN OUT  SDE.spx_util.spx_record_t,
                               sp_ref_r        IN  SDE.spx_util.spatial_ref_record_t,
                               srch_shape      IN  SDE.st_geometry,
                               operation       IN  Integer,
                               distance        IN  number,
                               curs_select     OUT integer)
  /***********************************************************************
  *
  *n  {grid_search_prepare}  --  Prepares grid search selection
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure prepare a grid search selection using the 
  *  a.shape spatial index. 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     ia          <in>   ==  (sys.odciindexinfo) table/index info
  *     table_name  <in>   ==  (varchar2) S-table name
  *     spx_info_r  <in>   ==  (spx_util.spx_record_t) spatial index 
  *                                                    metadata
  *     sp_ref_r    <in>   ==  (spx_util.spatial_ref_record_t) sp_ref info
  *     int_env_r   <in>   ==  (spx_util.r_env) b.shape/(srch_shape) MBR
  *     curs        <out>  ==  (integer) grid envelope/sysunits
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          06/05/08           original coding.
  *e
  ***********************************************************************/
IS
  rid             rowid;
  stmt            varchar2(512);
  nrows           integer;
  curs1           integer := 0;
  pos             pls_integer := 0;
  grids           SDE.spx_util.v_grids := v_grids(0,0,0);
  levels          SDE.spx_util.v_levels := v_levels(0,0,0);
  n_grids         integer;
  minx            integer;
  miny            integer;
  maxx            integer;
  maxy            integer;
  f_minx          float(64);
  f_miny          float(64);
  f_maxx          float(64);
  f_maxy          float(64);
  pgrid_info      sp_grid_info := sp_grid_info(0,0,0);
  gen_grid1       number := NULL;
  gen_grid2       number := NULL;
  gen_grid3       number := NULL;

Begin
     
      -- Generate the grid/tessellation array
  
  If (spx_info_r.grid.grid1 > 0) Then    
    SDE.spx_util.gen_cell_arrays(srch_shape,
                                 operation,
                                 distance,
                                 spx_info_r.grid.grid1,
                                 spx_info_r.grid.grid2,
                                 spx_info_r.grid.grid3,
                                 SDE.spx_util.num_grid_cell,
                                 SDE.spx_util.grid_cell_dem_t,
                                 gen_grid1,
                                 gen_grid2,
                                 gen_grid3);
  End If;

      -- Get cached cursor 

  If nlayers > 0 Then
    FOR id IN 1 .. nlayers
    Loop
      If cursor_cache(id).index_id = spx_info_r.index_id Then
        pos := id;

        If (ia.indexpartition IS NOT NULL) Then
          curs1 := get_partition_curs (pos,ia.indexpartition,curs_type_grid1,pgrid_info);
          If (curs1 <> 0) Then
            spx_info_r.grid.grid1 := pgrid_info.grid1;
            spx_info_r.grid.grid2 := pgrid_info.grid2;
            spx_info_r.grid.grid3 := pgrid_info.grid3;
          End If;
        Else
          If cursor_cache(id).curs_array(1).curs_grid1  > 0 Then
            curs1 := cursor_cache(id).curs_array(1).curs_grid1;
          End If;
          Exit;
        End If;
      End If;
    End Loop nlayers;
  End If;

  n_grids := 1;
  SDE.spx_util.srch_grid_env.delete;

  If spx_info_r.grid.grid2 > 0 Then
    If spx_info_r.grid.grid3 > 0 Then
      n_grids := 3;
    Else
      n_grids := 2;
    End If;
  End If;

  If curs1 = 0 Then
    IF spx_info_r.grid.grid3 > 0 Then
      stmt := 'SELECT min(gx), max(gx), min(gy), max(gy), min(minx), min(miny), min(maxx), min(maxy), sp_id '||
              'FROM '||table_name||' '||
              'WHERE ((gx >= :b1 AND gx <= :b2 AND gy >= :b3 AND gy <= :b4) OR '||
                     '(gx >= :b5 AND gx <= :b6 AND gy >= :b7 AND gy <= :b8) OR '||
                     '(gx >= :b9 AND gx <= :b10 AND gy >= :b11 AND gy <= :b12) OR (gx < 0 and gy < 0)) '||
                     'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4 GROUP BY sp_id';

    Elsif spx_info_r.grid.grid2 > 0 THEN
      stmt := 'SELECT min(gx), max(gx), min(gy), max(gy), min(minx), min(miny), min(maxx), min(maxy), sp_id '||
           'FROM '||table_name||' '||
           'WHERE ((gx >= :b1 AND gx <= :b2 AND gy >= :b3 AND gy <= :b4) OR '||
                   '(gx >= :b5 AND gx <= :b6 AND gy >= :b7 AND gy <= :b8) OR (gx < 0 and gy < 0)) '||
                   'AND minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4 GROUP BY sp_id';

    Else
      stmt := 'SELECT min(gx), max(gx), min(gy), max(gy), min(minx), min(miny), min(maxx), min(maxy), sp_id '||
              'FROM '||table_name||' '||
              'WHERE ((gx >= :b1 AND gx <= :b2 AND gy >= :b3 AND gy <= :b4) OR (gx < 0 and gy < 0)) AND '||
                     'minx <= :e1 AND miny <= :e2 AND maxx >= :e3 AND maxy >= :e4 GROUP BY sp_id';
    End If;

    curs1 := dbms_sql.open_cursor;
    cursor_cache(pos).index_id := spx_info_r.index_id;
    dbms_sql.parse(curs1,stmt,dbms_sql.native);

    SDE.spx_util.fetch_env(curs1).curs := curs1;
    SDE.spx_util.fetch_env(curs1).fetch_pos := 1;
    SDE.spx_util.fetch_env(curs1).first_fetch := TRUE;
    SDE.spx_util.fetch_env(curs1).fetch_state := 'FIRST';
    SDE.spx_util.fetch_env(curs1).total_rows := 0;
    
  End If;
  
  If operation = SDE.st_geom_util.buffer_intersects_c Then
    f_minx := srch_shape.minx - distance;
    f_miny := srch_shape.miny - distance;
    f_maxx := srch_shape.maxx + distance;
    f_maxy := srch_shape.maxy + distance;
  Else
    f_minx := srch_shape.minx;
    f_miny := srch_shape.miny;
    f_maxx := srch_shape.maxx;
    f_maxy := srch_shape.maxy;
  End If;
  
  minx := ((f_minx - sp_ref_r.x_offset) * sp_ref_r.xyunits + 0.5);
  miny := ((f_miny - sp_ref_r.y_offset) * sp_ref_r.xyunits + 0.5);
  maxx := ((f_maxx - sp_ref_r.x_offset) * sp_ref_r.xyunits + 0.5);
  maxy := ((f_maxy - sp_ref_r.y_offset) * sp_ref_r.xyunits + 0.5);
 
  grids(1) := spx_info_r.grid.grid1 * sp_ref_r.xyunits;
  levels(1) := 0;

  If spx_info_r.grid.grid2 > 0 THEN
    grids(2) := spx_info_r.grid.grid2 * sp_ref_r.xyunits;
    levels(2) := grid_level_mask_1;
  Else
    grids(2) := 0;
  End If;
 
  If spx_info_r.grid.grid3 > 0 THEN
    grids(3) := spx_info_r.grid.grid3 * sp_ref_r.xyunits;
    levels(3) := grid_level_mask_2;
  Else
    grids(3) := 0;
  End If;
  
  If (spx_info_r.grid.grid1 > 0) Then
    FOR x IN 1 .. 3
    Loop
      If grids(x) > 0 THEN
        SDE.spx_util.srch_grid_env.extend(1);
        SDE.spx_util.srch_grid_env(x).minx := trunc(minx / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env(x).miny := trunc(miny / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env(x).maxx := trunc(maxx / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env(x).maxy := trunc(maxy / grids(x)) + levels(x);
      End If;
    End Loop;
  Else
    SDE.spx_util.srch_grid_env.extend(1);
    SDE.spx_util.srch_grid_env(1).minx := 0;
    SDE.spx_util.srch_grid_env(1).miny := 0;
    SDE.spx_util.srch_grid_env(1).maxx := 0;
    SDE.spx_util.srch_grid_env(1).maxy := 0;
  End If;

  If (gen_grid1 IS NOT NULL) Then
    grids(1) := gen_grid1;
    SDE.spx_util.fetch_env(curs1).gen_grid1 := gen_grid1;
  End If;
 
  If spx_info_r.grid.grid2 > 0 THEN
    If (gen_grid2 IS NOT NULL) Then
      grids(2) := gen_grid2;
      SDE.spx_util.fetch_env(curs1).gen_grid2 := gen_grid2;
    End If;
  End If;
 
  If spx_info_r.grid.grid3 > 0 THEN
    If gen_grid3 IS NOT NULL Then
      grids(3) := gen_grid3;
      SDE.spx_util.fetch_env(curs1).gen_grid3 := gen_grid3;
    End If;
  End If;

  If (gen_grid1 IS NOT NULL OR gen_grid2 IS NOT NULL OR gen_grid3 IS NOT NULL) Then
    SDE.spx_util.srch_grid_env_new_grid.delete;
    FOR x IN 1 .. 3
    Loop
      If grids(x) > 0 THEN
        SDE.spx_util.srch_grid_env_new_grid.extend(1);
        SDE.spx_util.srch_grid_env_new_grid(x).minx := trunc(minx / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env_new_grid(x).miny := trunc(miny / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env_new_grid(x).maxx := trunc(maxx / grids(x)) + levels(x);
        SDE.spx_util.srch_grid_env_new_grid(x).maxy := trunc(maxy / grids(x)) + levels(x);
      End If;
    End Loop;
  End If;

  dbms_sql.bind_variable(curs1, ':b1', SDE.spx_util.srch_grid_env(1).minx);
  dbms_sql.bind_variable(curs1, ':b2', SDE.spx_util.srch_grid_env(1).maxx);
  dbms_sql.bind_variable(curs1, ':b3', SDE.spx_util.srch_grid_env(1).miny);
  dbms_sql.bind_variable(curs1, ':b4', SDE.spx_util.srch_grid_env(1).maxy);

  If grids(2) > 0 THEN
    dbms_sql.bind_variable(curs1, ':b5', SDE.spx_util.srch_grid_env(2).minx);
    dbms_sql.bind_variable(curs1, ':b6', SDE.spx_util.srch_grid_env(2).maxx);
    dbms_sql.bind_variable(curs1, ':b7', SDE.spx_util.srch_grid_env(2).miny);
    dbms_sql.bind_variable(curs1, ':b8', SDE.spx_util.srch_grid_env(2).maxy);
  End If;

  If grids(3) > 0 THEN 
    dbms_sql.bind_variable(curs1, ':b9', SDE.spx_util.srch_grid_env(3).minx);
    dbms_sql.bind_variable(curs1, ':b10', SDE.spx_util.srch_grid_env(3).maxx);
    dbms_sql.bind_variable(curs1, ':b11', SDE.spx_util.srch_grid_env(3).miny);
    dbms_sql.bind_variable(curs1, ':b12', SDE.spx_util.srch_grid_env(3).maxy);
  End If;

  dbms_sql.bind_variable(curs1, ':e1',maxx);
  dbms_sql.bind_variable(curs1, ':e2',maxy);
  dbms_sql.bind_variable(curs1, ':e3',minx);
  dbms_sql.bind_variable(curs1, ':e4',miny);

  dbms_sql.define_array(curs1, 1,SDE.spx_util.fetch_env(curs1).min_gx_t, 100, 1);
  dbms_sql.define_array(curs1, 2,SDE.spx_util.fetch_env(curs1).max_gx_t, 100, 1);
  dbms_sql.define_array(curs1, 3,SDE.spx_util.fetch_env(curs1).min_gy_t, 100, 1);
  dbms_sql.define_array(curs1, 4,SDE.spx_util.fetch_env(curs1).max_gy_t, 100, 1);
  dbms_sql.define_array(curs1, 5,SDE.spx_util.fetch_env(curs1).minx_t, 100, 1);
  dbms_sql.define_array(curs1, 6,SDE.spx_util.fetch_env(curs1).miny_t, 100, 1);
  dbms_sql.define_array(curs1, 7,SDE.spx_util.fetch_env(curs1).maxx_t, 100, 1);
  dbms_sql.define_array(curs1, 8,SDE.spx_util.fetch_env(curs1).maxy_t, 100, 1);
  dbms_sql.define_array(curs1, 9,SDE.spx_util.fetch_env(curs1).sp_id_t, 100, 1);

  curs_select := curs1;
  SDE.spx_util.grid1 := spx_info_r.grid.grid1;
  
End grid_search_prepare;


Function grid_search_execute             (curs1           IN number,
                                          owner_name      IN varchar2,
                                          table_name      IN varchar2,
                                          sp_col          IN varchar2,
                                          operation       IN varchar2,
                                          srch_geom       IN SDE.st_geometry,
                                          distance        IN number,
                                          rids            OUT NOCOPY sys.odciridlist,
                                          env             IN sys.odcienv)
Return number
IS
  row_cnt            pls_integer := 0;
  tot_rows           pls_integer := 0;
  t_cell_pos         integer;
  t_cell_dem         integer;
  frows              integer;
  grid_pos           integer := 1;
  hash_value         number;
  is_boundary        Boolean := FALSE;
  is_interior        Boolean := FALSE;
  next_fetch         Boolean := TRUE;
  high_pos           integer := 0;
  tot_row            integer := 0;
  boundary_rids      SDE.bnd_rowid_tab := SDE.bnd_rowid_tab();
  boundary_rids_cnt  integer;
  ext_tab_cnt        integer;
  ext2               integer;
  gx_min             integer;
  gx_max             integer;
  gy_min             integer;
  gy_max             integer;
  srch_gx_min        integer;
  srch_gx_max        integer;
  srch_gy_min        integer;
  srch_gy_max        integer;
  is_row             integer := 0;
  hv                 number;
  a_val              integer;
  b_val              integer;
  z_val              integer;
  y_val              integer;
  x                  number;
  y                  number;

Begin

  If srch_geom IS NULL Or srch_geom.numpts = 0 Then
    rids := sys.odciridlist();
    rids.EXTEND(1);
    rids(1) := NULL;
    If SDE.spx_util.max_hash_value > 0 Then
      <<grid_delete>>
      For i IN 1..3 Loop
        If SDE.spx_util.hashgrid_key_max(i) > 0 Then
          For x IN SDE.spx_util.hashgrid_key_min(i)..SDE.spx_util.hashgrid_key_max(i) Loop
            If SDE.spx_util.grid_cell_dem_t.exists(x) Then
              SDE.spx_util.grid_cell_dem_t(x).t_cell.delete;
            End If;
          End Loop;
        End If;
      End Loop grid_delete;
    End If;
    
    SDE.spx_util.max_hash_value := 0;
    SDE.spx_util.grid_cell_dem_t.delete;
    SDE.spx_util.hashgrid_key_min := v_grids(16777216,33554432,2147483647);
    SDE.spx_util.hashgrid_key_max := SDE.spx_util.v_grids(0,0,0);
    Return odciconst.success;
  End If;

  If SDE.spx_util.fetch_env(curs1).fetch_state = 'FIRST' Then
    frows := dbms_sql.execute(curs1);
    SDE.spx_util.grid_rows_fetched := 0;
    boundary_rids_cnt := 0;
    SDE.spx_util.fetch_env(curs1).interior_rids_cnt := 0;
    SDE.spx_util.fetch_env(curs1).test_boundary_fetch := 'FIRST';
    SDE.spx_util.fetch_env(curs1).interior_rids := SDE.bnd_rowid_tab();
    SDE.spx_util.fetch_env(curs1).interior_rid_pos := 0;
    grid_pos := 1;
    high_pos := 0;
    ext_tab_cnt := 0;
  End If;
        
  <<rids_fetch_loop>>
  While SDE.spx_util.fetch_env(curs1).fetch_state = 'FIRST' OR SDE.spx_util.fetch_env(curs1).fetch_state = 'NEXT' Loop

    row_cnt := dbms_sql.fetch_rows(curs1);
    high_pos := high_pos + row_cnt;

    If row_cnt = 0  AND high_pos = 0 Then
      rids := sys.odciridlist();
      rids.EXTEND(1);
      rids(1) := NULL;
      If SDE.spx_util.max_hash_value > 0 Then
        <<grid_delete>>
        For i IN 1..3 Loop
          If SDE.spx_util.hashgrid_key_max(i) > 0 Then
            For x IN SDE.spx_util.hashgrid_key_min(i)..SDE.spx_util.hashgrid_key_max(i) Loop
              If SDE.spx_util.grid_cell_dem_t.exists(x) Then
                SDE.spx_util.grid_cell_dem_t(x).t_cell.delete;
              End If;
            End Loop;
          End If;
        End Loop grid_delete;
      End If;
    
      SDE.spx_util.max_hash_value := 0;
      SDE.spx_util.hashgrid_key_min := v_grids(16777216,33554432,2147483647);
      SDE.spx_util.hashgrid_key_max := SDE.spx_util.v_grids(0,0,0);
      SDE.spx_util.grid_cell_dem_t.delete;
      Return odciconst.success;
    Elsif row_cnt = 0 and high_pos > 0 Then
      SDE.spx_util.fetch_env(curs1).fetch_state := 'LAST';
      EXIT rids_fetch_loop;
    End If;
    
    If row_cnt > 0 Then
      dbms_sql.column_value(curs1, 1, SDE.spx_util.fetch_env(curs1).min_gx_t);
      dbms_sql.column_value(curs1, 2, SDE.spx_util.fetch_env(curs1).max_gx_t);
      dbms_sql.column_value(curs1, 3, SDE.spx_util.fetch_env(curs1).min_gy_t);
      dbms_sql.column_value(curs1, 4, SDE.spx_util.fetch_env(curs1).max_gy_t);
      dbms_sql.column_value(curs1, 5, SDE.spx_util.fetch_env(curs1).minx_t);
      dbms_sql.column_value(curs1, 6, SDE.spx_util.fetch_env(curs1).miny_t);
      dbms_sql.column_value(curs1, 7, SDE.spx_util.fetch_env(curs1).maxx_t);
      dbms_sql.column_value(curs1, 8, SDE.spx_util.fetch_env(curs1).maxy_t);
      dbms_sql.column_value(curs1, 9, SDE.spx_util.fetch_env(curs1).sp_id_t);
    End If;
      
    If row_cnt = 100 Then
      SDE.spx_util.fetch_env(curs1).fetch_state := 'NEXT';
    ElsIf row_cnt < 100 Then
      SDE.spx_util.fetch_env(curs1).fetch_state := 'LAST';
    End If;

    <<grid_search>>
    For i In grid_pos..high_pos Loop
      tot_row := tot_row + 1;

      If (SDE.spx_util.fetch_env(curs1).min_gx_t(i) < 0 AND 
          SDE.spx_util.fetch_env(curs1).min_gy_t(i) < 0) Then
        gx_min := -1;
        gx_max := -1;
        gy_min := -1;
        gy_max := -1;
      Else
        If (SDE.spx_util.fetch_env(curs1).gen_grid1 IS NOT NULL OR 
            SDE.spx_util.fetch_env(curs1).gen_grid2 IS NOT NULL OR 
            SDE.spx_util.fetch_env(curs1).gen_grid3 IS NOT NULL) Then
          If(SDE.spx_util.srch_grid_env.count = 1) Then
            gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
            gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
            gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
            gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
          Elsif (SDE.spx_util.srch_grid_env.count = 2) Then
            If (SDE.spx_util.fetch_env(curs1).min_gx_t(i) < 16777216) Then
              gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
            Else
              If(SDE.spx_util.fetch_env(curs1).gen_grid2 IS NOT NULL) Then
                gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
              Else
                gx_min := SDE.spx_util.fetch_env(curs1).min_gx_t(i);
                gx_max := SDE.spx_util.fetch_env(curs1).max_gx_t(i);
                gy_min := SDE.spx_util.fetch_env(curs1).min_gy_t(i);
                gy_max := SDE.spx_util.fetch_env(curs1).max_gy_t(i);
              End If;
            End If;
          Elsif (SDE.spx_util.srch_grid_env.count = 3) Then
            If (SDE.spx_util.fetch_env(curs1).min_gx_t(i) < 16777216) Then
              gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
              gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid1);
            Elsif (SDE.spx_util.fetch_env(curs1).min_gx_t(i) < 33554432) Then
              If(SDE.spx_util.fetch_env(curs1).gen_grid2 IS NOT NULL) Then
                gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
                gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid2) + grid_level_mask_1;
              Else
                gx_min := SDE.spx_util.fetch_env(curs1).min_gx_t(i);
                gx_max := SDE.spx_util.fetch_env(curs1).max_gx_t(i);
                gy_min := SDE.spx_util.fetch_env(curs1).min_gy_t(i);
                gy_max := SDE.spx_util.fetch_env(curs1).max_gy_t(i);
              End If;
            Else
              If(SDE.spx_util.fetch_env(curs1).gen_grid3 IS NOT NULL) Then
                gx_min := trunc(SDE.spx_util.fetch_env(curs1).minx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid3) + grid_level_mask_2;
                gy_min := trunc(SDE.spx_util.fetch_env(curs1).miny_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid3) + grid_level_mask_2;
                gx_max := trunc(SDE.spx_util.fetch_env(curs1).maxx_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid3) + grid_level_mask_2;
                gy_max := trunc(SDE.spx_util.fetch_env(curs1).maxy_t(i) / SDE.spx_util.fetch_env(curs1).gen_grid3) + grid_level_mask_2;
              Else
                gx_min := SDE.spx_util.fetch_env(curs1).min_gx_t(i);
                gx_max := SDE.spx_util.fetch_env(curs1).max_gx_t(i);
                gy_min := SDE.spx_util.fetch_env(curs1).min_gy_t(i);
                gy_max := SDE.spx_util.fetch_env(curs1).max_gy_t(i);
              End If;
            End If;
          Else
            raise_application_error (SDE.st_type_util.spx_invalid_number_of_grids,'Invalid number of grids');
          End If;
        Else
          gx_min := SDE.spx_util.fetch_env(curs1).min_gx_t(i);
          gx_max := SDE.spx_util.fetch_env(curs1).max_gx_t(i);
          gy_min := SDE.spx_util.fetch_env(curs1).min_gy_t(i);
          gy_max := SDE.spx_util.fetch_env(curs1).max_gy_t(i);
        End If;
      End If;

      is_boundary := FALSE;
      is_interior := FALSE;
      
      If (gx_max = -1) Then
        x := 0;
      Else
        x := gx_min;
      End If;

     <<gx_loop>>
      WHILE x <= gx_max Loop

        <<gy_loop>>
        y := gy_min;
        WHILE y <= gy_max Loop

          -- the input spatial index could be in any grid. Use the grid count
          -- and the grid_mask to determine which grid search envelope from 
          -- the search shape should be used. 

          If(SDE.spx_util.srch_grid_env.count = 1) Then
            If (SDE.spx_util.fetch_env(curs1).gen_grid1 IS NOT NULL) Then
              srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(1).minx;
              srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(1).maxx;
              srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(1).miny;
              srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(1).maxy;
            Else
              srch_gx_min := SDE.spx_util.srch_grid_env(1).minx;
              srch_gx_max := SDE.spx_util.srch_grid_env(1).maxx;
              srch_gy_min := SDE.spx_util.srch_grid_env(1).miny;
              srch_gy_max := SDE.spx_util.srch_grid_env(1).maxy;
            End If;

          Elsif(SDE.spx_util.srch_grid_env.count = 2) Then
            If x < grid_level_mask_1 Then
              If (SDE.spx_util.fetch_env(curs1).gen_grid1 IS NOT NULL) Then
                srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(1).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(1).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(1).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(1).maxy;
              Else
                srch_gx_min := SDE.spx_util.srch_grid_env(1).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env(1).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env(1).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env(1).maxy;
              End If;
            Else
              If (SDE.spx_util.fetch_env(curs1).gen_grid2 IS NOT NULL) Then
                srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(2).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(2).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(2).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(2).maxy;
              Else
                srch_gx_min := SDE.spx_util.srch_grid_env(2).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env(2).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env(2).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env(2).maxy;
              End If;
            End If;
          Else
            If x < grid_level_mask_1 Then
              If (SDE.spx_util.fetch_env(curs1).gen_grid1 IS NOT NULL) Then
                srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(1).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(1).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(1).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(1).maxy;
              Else
                srch_gx_min := SDE.spx_util.srch_grid_env(1).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env(1).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env(1).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env(1).maxy;
              End If;
            Elsif x < grid_level_mask_2 Then
              If (SDE.spx_util.fetch_env(curs1).gen_grid2 IS NOT NULL) Then
                srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(2).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(2).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(2).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(2).maxy;
              Else
                srch_gx_min := SDE.spx_util.srch_grid_env(2).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env(2).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env(2).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env(2).maxy;
              End If;
            Else
              If (SDE.spx_util.fetch_env(curs1).gen_grid3 IS NOT NULL) Then
                srch_gx_min := SDE.spx_util.srch_grid_env_new_grid(3).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env_new_grid(3).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env_new_grid(3).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env_new_grid(3).maxy;
              Else
                srch_gx_min := SDE.spx_util.srch_grid_env(3).minx;
                srch_gx_max := SDE.spx_util.srch_grid_env(3).maxx;
                srch_gy_min := SDE.spx_util.srch_grid_env(3).miny;
                srch_gy_max := SDE.spx_util.srch_grid_env(3).maxy;
              End If;
            End If;     
          End If;

          a_val := (x - srch_gx_min) + 1;
          b_val := (y - srch_gy_min) + 1;

          z_val := b_val * ((srch_gx_max - srch_gx_min) + 1);
          y_val := ((srch_gx_max - srch_gx_min) + 1) - a_val;

          If(x < grid_level_mask_1) Then
            hash_value := z_val - y_val;
          Elsif (x < grid_level_mask_2) Then
            hash_value := (z_val - y_val) + grid_level_mask_1;
          Else
            hash_value := (z_val - y_val) + grid_level_mask_2;
          End If;

          If (SDE.spx_util.grid_cell_dem_t.exists(hash_value)) Then

            If SDE.spx_util.grid_cell_dem_t(hash_value).dem = SDE.spx_util.boundary_case Then
            <<t_cell_loop>>
              For t IN 1..SDE.spx_util.grid_cell_dem_t(hash_value).t_cell.LAST Loop
                t_cell_dem := SDE.spx_util.grid_cell_dem_t(hash_value).t_cell(t).dem;
                
                If SDE.spx_util.fetch_env(curs1).minx_t(i) > SDE.spx_util.grid_cell_dem_t(hash_value).t_cell(t).maxx OR 
                   SDE.spx_util.fetch_env(curs1).miny_t(i) > SDE.spx_util.grid_cell_dem_t(hash_value).t_cell(t).maxy OR 
                   SDE.spx_util.fetch_env(curs1).maxx_t(i) < SDE.spx_util.grid_cell_dem_t(hash_value).t_cell(t).minx OR 
                   SDE.spx_util.fetch_env(curs1).maxy_t(i) < SDE.spx_util.grid_cell_dem_t(hash_value).t_cell(t).miny Then
                 
                   -- Exterior case - do nothing.
                   t_cell_pos := t_cell_pos;

                Else
                  IF t_cell_dem = SDE.spx_util.boundary_case THEN
                    boundary_rids_cnt := boundary_rids_cnt + 1;
                    boundary_rids.extend(1);
                    boundary_rids(boundary_rids_cnt) := SDE.spx_util.fetch_env(curs1).sp_id_t(i);
                    
                    is_boundary := TRUE;
                    is_interior := FALSE;
                    
                   -- Since we have a boundary cell condition, there's no need to iterate through all
                   -- tessalated cells, therefore we exit the loop.
                 
                    --EXIT gx_Loop;
                    EXIT;
                    
                  Else
                   -- T-cell intersection case - Keep looping
                    is_interior := TRUE;
                  End If;
                
                End If;                 -- t_cell_MBR check

              End Loop t_cell_loop;
              
            Else
              -- interior grid cell case 
              
              is_interior := TRUE;

            End If;                     -- Boundary case
            
            If is_boundary = TRUE Then
              EXIT;
            End If;
  
          End If;                       -- grid_cell_dem_t.exists(hash)
        
          If is_boundary = TRUE Then
            EXIT;
          End If;
          
          y := y + 1;
        End Loop gy_loop;
      
        If is_boundary = TRUE Then
          EXIT;
        End If;
        
        x := x + 1;
      End Loop gx_loop;

      If gx_min = -1 AND gy_min = -1 Then
        boundary_rids_cnt := boundary_rids_cnt + 1;
        boundary_rids.extend(1);
        boundary_rids(boundary_rids_cnt) :=SDE.spx_util.fetch_env(curs1).sp_id_t(i);
      End If;
      
      If is_interior = TRUE And is_boundary = FALSE And 
        'ST_OVERLAPS' != operation And 'ST_TOUCHES' != operation  Then
        SDE.spx_util.fetch_env(curs1).interior_rids_cnt := SDE.spx_util.fetch_env(curs1).interior_rids_cnt + 1;
        SDE.spx_util.fetch_env(curs1).interior_rids.extend(1);
        SDE.spx_util.fetch_env(curs1).interior_rids(SDE.spx_util.fetch_env(curs1).interior_rids_cnt) := SDE.spx_util.fetch_env(curs1).sp_id_t(i);
      End If;

      is_boundary := FALSE;
      is_interior := FALSE;
      
      is_row := 0;
    
    End Loop grid_search;
   
    SDE.spx_util.fetch_env(curs1).min_gx_t.delete;
    SDE.spx_util.fetch_env(curs1).max_gx_t.delete;
    SDE.spx_util.fetch_env(curs1).min_gy_t.delete;
    SDE.spx_util.fetch_env(curs1).max_gy_t.delete;
    SDE.spx_util.fetch_env(curs1).minx_t.delete;
    SDE.spx_util.fetch_env(curs1).miny_t.delete;
    SDE.spx_util.fetch_env(curs1).maxx_t.delete;
    SDE.spx_util.fetch_env(curs1).maxy_t.delete;
    SDE.spx_util.fetch_env(curs1).sp_id_t.delete;
    
    grid_pos := grid_pos + row_cnt;
    
  End Loop rids_fetch_loop;
  
  If boundary_rids_cnt > 0 And SDE.spx_util.fetch_env(curs1).test_boundary_fetch = 'FIRST' Then
    SDE.spx_util.test_boundary_rids (owner_name,
                                     table_name,
                                     sp_col,
                                     srch_geom,
                                     distance,
                                     operation,
                                     boundary_rids,
                                     boundary_rids_cnt,
                                     SDE.spx_util.fetch_env(curs1).interior_rids,
                                     SDE.spx_util.fetch_env(curs1).interior_rids_cnt);
    SDE.spx_util.fetch_env(curs1).test_boundary_fetch := 'LAST';
  End If;
  
  grid_pos := 0;
  rids := sys.odciridlist();

  While rids.count < 2000 And 
    SDE.spx_util.fetch_env(curs1).interior_rid_pos < SDE.spx_util.fetch_env(curs1).interior_rids_cnt Loop
    SDE.spx_util.fetch_env(curs1).interior_rid_pos := SDE.spx_util.fetch_env(curs1).interior_rid_pos + 1;
    grid_pos := grid_pos + 1;
    rids.extend(1);
    rids(grid_pos) := SDE.spx_util.fetch_env(curs1).interior_rids(SDE.spx_util.fetch_env(curs1).interior_rid_pos);
  End Loop;

  If rids.count < 2000 Then
    grid_pos := rids.count;
    rids.extend(1);
    rids(grid_pos + 1) := NULL;
    If SDE.spx_util.fetch_env(curs1).interior_rids.exists(1) Then
      SDE.spx_util.fetch_env(curs1).interior_rids.delete;
    End If;
  End If;

  If SDE.spx_util.max_hash_value > 0 Then
    <<grid_delete>>
    For i IN 1..3 Loop
      If SDE.spx_util.hashgrid_key_max(i) > 0 Then
        For x IN SDE.spx_util.hashgrid_key_min(i)..SDE.spx_util.hashgrid_key_max(i) Loop
          If SDE.spx_util.grid_cell_dem_t.exists(x) Then
            SDE.spx_util.grid_cell_dem_t(x).t_cell.delete;
          End If;
        End Loop;
      End If;
    End Loop grid_delete;
    
    SDE.spx_util.max_hash_value := 0;
    SDE.spx_util.hashgrid_key_min := v_grids(16777216,33554432,2147483647);
    SDE.spx_util.hashgrid_key_max := SDE.spx_util.v_grids(0,0,0);
    SDE.spx_util.grid_cell_dem_t.delete;
  End If;

  Return odciconst.success;

End grid_search_execute;

Procedure test_boundary_rids             (owner_name         IN varchar2,
                                          table_name         IN varchar2,
                                          sp_col             IN varchar2,
                                          bshape             IN SDE.st_geometry,
                                          distance           IN number,
                                          operation          IN varchar2,
                                          boundary_rids      IN SDE.bnd_rowid_tab,
                                          boundary_rids_cnt  IN pls_integer,
                                          interior_rids      IN OUT SDE.bnd_rowid_tab,
                                          interior_rids_cnt  IN OUT pls_integer)
IS
  stmt            varchar2(512);
  entity          SDE.int_array_tab := SDE.int_array_tab();
  numpts          SDE.int_array_tab := SDE.int_array_tab();
  srid            SDE.int_array_tab := SDE.int_array_tab();
  points          SDE.blob_array_tab := SDE.blob_array_tab();
  row_id          SDE.bnd_rowid_tab := SDE.bnd_rowid_tab();
  rc              integer;
  rid_array_cnt   pls_integer := 0;
  spref           SDE.spx_util.spatial_ref_record_t;
  spref2          SDE.spx_util.spatial_ref_record_t;
  rid_array       SDE.bnd_rowid_tab := SDE.bnd_rowid_tab();
  Type t_ref      IS Ref Cursor;
  f_curs          t_ref;
  rel_op          integer;

Begin
    
  stmt := 'SELECT /*+ first_rows(1) */ a.'||sp_col||'.entity, a.'||sp_col||'.numpts,'||
                 'a.'||sp_col||'.srid, a.'||sp_col||'.points, a.rowid '||
                 'FROM '||owner_name||'.'||table_name||' a,'||
                   '(SELECT t.column_value AS ID '||
                   'FROM TABLE (CAST (:bnd_tab AS SDE.bnd_rowid_tab)) t) bnd '||
                   'WHERE a.rowid = bnd.id';
 
  spref.srid := bshape.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  open f_curs for stmt USING boundary_rids;
  Fetch f_curs Bulk Collect INTO entity, numpts, srid, points, row_id;
  
  If operation = 'ST_INTERSECTS' OR operation = 'ST_BUFFER_INTERSECTS' Then
    rel_op := SDE.st_geom_util.intersects_c;
  Elsif operation = 'ST_WITHIN' Then
    rel_op := SDE.st_geom_util.within_c;
  Elsif operation = 'ST_CONTAINS' Then
    rel_op := SDE.st_geom_util.contains_c;
  Elsif operation = 'ST_CROSSES' Then
    rel_op := SDE.st_geom_util.crosses_c;
  Elsif operation = 'ST_TOUCHES' Then
    rel_op := SDE.st_geom_util.touches_c;
  Elsif operation = 'ST_OVERLAPS' Then
    rel_op := SDE.st_geom_util.overlaps_c;
  Elsif operation = 'ST_EQUALS' Or operation = 'ST_ORDERINGEQUALS' Then
    rel_op := SDE.st_geom_util.equals_c;
  Else
    raise_application_error (SDE.st_type_util.spx_invalid_rel_operation,'Invalid domain index operation.');
  End If;
  
  if spref.srid = srid(1) Then
  
    spref2.srid := srid(1);
  
    SDE.st_geometry_shapelib_pkg.test_features (rel_op,spref.srid,
                                                spref.x_offset,spref.y_offset,spref.xyunits,
                                                spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                                spref.Definition,bshape.numpts,bshape.entity,bshape.minx,
                                                bshape.miny,bshape.maxx,bshape.maxy,bshape.points,distance,
                                                boundary_rids_cnt,entity,numpts,srid,points,row_id,rid_array,
                                                rid_array_cnt);
  Else
    spref2.srid := srid(1);
    rc := SDE.st_spref_util.select_spref(spref2);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
                               ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
    
    SDE.st_geometry_shapelib_pkg.test_features2 (rel_op,spref.srid,
                                                 spref.x_offset,spref.y_offset,spref.xyunits,
                                                 spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                                 spref.Definition,spref2.srid,
                                                 spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                                 spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                                 spref2.Definition,bshape.numpts,bshape.entity,bshape.minx,
                                                 bshape.miny,bshape.maxx,bshape.maxy,bshape.points,distance,
                                                 boundary_rids_cnt,entity,numpts,srid,points,row_id,rid_array,
                                                 rid_array_cnt);
  End If;

  If rid_array_cnt > 0 Then
    For i IN 1..rid_array_cnt Loop
      interior_rids_cnt := interior_rids_cnt + 1;
      interior_rids.extend(1);
      interior_rids(interior_rids_cnt) := rid_array(i);
    End Loop;
    
    rid_array.delete;
    entity.delete;
    numpts.delete;
    srid.delete;
    points.delete;
    row_id.delete;
  End If;
 
End test_boundary_rids;

  Procedure insert_partition             (owner_name         IN varchar2,
                                          table_name         IN varchar2,
                                          column_name        IN varchar2,
                                          partition_name     IN varchar2,
                                          spx_info_r         IN SDE.spx_util.spx_record_t)
  /***********************************************************************
  *
  *n  {insert_partition}  --  Inserts partition metadata to the
  *                           ST_Partition_Index and ST_Geometry tables.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This procedure inserts partition metadata for a given partition
  *  table assocaited to a spatial index. The ST_Geometry_Columns 
  *  Properties fields is also updated to reflect the maintenance 
  *  of partition metadat at 10.1 to allow for backward compatibility 
  *  of partition spatial indexes. 
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     owner_name     <in>   ==  (varchar2) owner name
  *     table_name     <in>   ==  (varchar2) table_name
  *     column_name    <in>   ==  (varchar2) spatial column
  *     partition_name <in>   ==  (varchar2) partition name
  *     spx_info_r     <in>   ==  (spx_util.spx_record_t) spatial index 
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          08/27/11           original coding.
  *e
  ***********************************************************************/
IS
  geom_type         nvarchar2(32);
  properties        st_geom_prop_t;
  srid              integer;
  res               integer := 0;
  mask              raw(64);
  raw1              raw(64);
  raw2              raw(64);
  properties_mask   raw(64);

Begin

  LOCK TABLE SDE.st_geometry_columns IN EXCLUSIVE MODE;

  LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

  INSERT INTO SDE.st_partition_index
       (owner,table_name,column_name,geom_id,partition_name,grid)
  VALUES (owner_name,table_name,column_name,spx_info_r.index_id,
          partition_name,spx_info_r.grid);

  res := SDE.st_geom_cols_util.select_gcol(owner_name,
                                           table_name,
                                           column_name,
                                           geom_type,
                                           properties,
                                           srid);
  If (res != SDE.st_geom_cols_util.se_success) Then
    raise_application_error (SDE.st_type_util.spx_partition_index_insert,'ST_PARTITION_INDEX insert error.');
  End If;

  If(properties != 0) Then
    res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
    If(res = 0) Then
      raw1 := utl_raw.cast_from_number(properties);
      raw2 := utl_raw.cast_from_number(SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
      properties_mask := utl_raw.bit_or(raw1,raw2);
      properties := utl_raw.cast_to_number(properties_mask);

      SDE.st_geom_cols_util.update_gcol_properties(owner_name,table_name,column_name,properties);
    End If;
  Else
    properties := SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX;

    SDE.st_geom_cols_util.update_gcol_properties(owner_name,table_name,column_name,properties);
  End If;

  COMMIT;

  EXCEPTION
    WHEN OTHERS THEN
      raise_application_error (SDE.st_type_util.spx_partition_index_insert,'ST_PARTITION_INDEX insert error.'||SQLERRM||' '||SQLCODE);


End insert_partition;

  Procedure delete_partition             (owner_in           IN varchar2,
                                          table_in           IN varchar2,
                                          column_in          IN varchar2,
                                          partition_in       IN varchar2,
                                          index_id           IN spx_index_id_t,
                                          properties         IN st_geom_prop_t)
  /***********************************************************************
  *
  *n  {delete_partition}  --  Deletes partition metadata from the
  *                           ST_Partition_Index table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This procedure deletes partition metadata for a given partition
  *  table assocaited to a spatial index.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     owner_in     <in>   ==  (varchar2) owner name
  *     table_in     <in>   ==  (varchar2) table_name
  *     column_in    <in>   ==  (varchar2) spatial column
  *     partition_in <in>   ==  (varchar2) partition name
  *     index_id     <in>   ==  (spx_idex_id_t) st_geometry index id
  *     properties   <in>   ==  (st_geom_prop_t) partiton properties
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          08/27/11           original coding.
  *e
  ***********************************************************************/
IS
  res           integer;
  table_name    varchar2(32);
  stmt          varchar2(1000);
Begin

  res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
  If(res != 0) Then
    LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

    stmt := 'DELETE FROM SDE.st_partition_index '|| 
            'WHERE owner = '''||owner_in||''' AND table_name = '''||table_in||
            ''' AND column_name = '''||column_in||''' AND partition_name = '''||partition_in||'''';

    EXECUTE IMMEDIATE stmt;

    COMMIT;
  End If;

End delete_partition;

Function get_partition_name (partition_name  IN varchar2,
                             properties      IN st_geom_prop_t,
                             index_id        IN spx_index_id_t)
  /***********************************************************************
  *
  *n  {get_partition_name}  --  Gets partition name based on 
  *                             ST_Geometry_Columns Properties value.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This functions gets the partition name based on the properties
  *  value. 10.1 partition names are unique whereas previous versions
  *  used non-unique names. 
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     partition_name     <in>   ==  (varchar2) partition name
  *     properties         <in>   ==  (st_geom_prop_t) ST_Geometry 
  *                                                   properties
  *     index_id           <in>   ==  (spx_index_id_t) index_id
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          08/27/11           original coding.
  *e
  ***********************************************************************/

Return varchar2
IS
  res           integer;
  table_name    varchar2(256);
Begin
  res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
  If(res != 0) Then
    table_name := 'S'||index_id||partition_name;
  Else
    table_name := 'S'||index_id||'_IDX$';
  End If;
  return(table_name);
End get_partition_name;

Procedure get_partition_grids (owner_in       IN varchar2,
                              table_in        IN varchar2,
                              column_in       IN varchar2,
                              partition_in    IN varchar2,
                              properties      IN st_geom_prop_t,
                              spx_info_r      IN OUT SDE.spx_util.spx_record_t)
  /***********************************************************************
  *
  *n  {get_partition_grids}  --  Gets partition grids metadata.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This functions gets the partition grids info from the 
  *  ST_PARTITION_INDEX metadata table. 
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *     owner_name       <in>   ==  (varchar2) owner
  *     table_in         <in>   ==  (varchar2)  table name
  *     column_in        <in>   ==  (varchar2) spatial column
  *     partition_in     <in>   ==  (varchar2) partition name
  *     properties       <in>   ==  (st_geom_prop_t) ST_Geometry 
  *                                                properties
  *     spx_info_r     <in/out> ==  (spx_record_t) spatial index metadata
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          08/27/11           original coding.
  *e
  ***********************************************************************/
IS
  res             integer;
  grid_info       SDE.sp_grid_info := SDE.sp_grid_info(0,0,0);

Begin
  res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
  If(res != 0) Then

    SELECT grid INTO grid_info
    FROM SDE.st_partition_index
    WHERE owner = owner_in AND table_name = table_in AND
          column_name = column_in AND partition_name = partition_in;

    spx_info_r.grid.grid1 := grid_info.grid1;
    spx_info_r.grid.grid2 := grid_info.grid2;
    spx_info_r.grid.grid3 := grid_info.grid3;

  End If;

Exception
  When NO_DATA_FOUND Then
    raise_application_error (SDE.st_type_util.spx_partition_not_found,'ST_PARTITION_INDEX '||partition_in||' not in ST_PARTITION_INDEX.');
  When OTHERS Then
    raise_application_error (SDE.st_type_util.spx_partition_error,'ST_PARTITION_INDEX (select) error.'||SQLERRM||' '||SQLCODE);

End get_partition_grids;

  Procedure update_partition_stats       (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          partition_in    IN varchar2,
                                          blevel_in       IN number,
                                          leafblocks_in   IN number,
                                          clstfct_in      IN number,
                                          avgcellcnt_in   IN number,
                                          numrows_in      IN number,
                                          samplesize_in   IN number,
                                          distkeys_in     IN number,
                                          minx_in         IN number,
                                          miny_in         IN number,
                                          maxx_in         IN number,
                                          maxy_in         IN number)

IS
  Cursor c_geom_stats (owner_wanted IN varchar2, table_wanted IN varchar2, 
                       column_wanted IN varchar2) IS
    SELECT *
    FROM   SDE.st_geometry_index
    WHERE  owner = owner_wanted AND table_name = table_wanted 
    AND column_name = column_wanted;

  geom_stats     SDE.st_geometry_index%Rowtype;

Begin

  Open c_geom_stats (owner_in, table_in,column_in);
  Fetch c_geom_stats INTO geom_stats;
  Close c_geom_stats;

  LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_partition_index SET blevel = blevel_in, 
         leaf_blocks = leafblocks_in, clustering_factor = clstfct_in, 
         density = round(avgcellcnt_in,2), num_rows = numrows_in, last_analyzed = SYSDATE,
         sample_size = samplesize_in, user_stats = 'NO', distinct_keys = distkeys_in,
         minx = minx_in, miny = miny_in, maxx = maxx_in, maxy = maxy_in 
  WHERE owner = owner_in AND table_name = table_in AND 
        column_name = column_in AND partition_name = partition_in;
  COMMIT;

Exception
  When NO_DATA_FOUND Then
    raise_application_error (SDE.st_type_util.spx_partition_not_found,'ST_PARTITION_INDEX '||partition_in||' not in ST_PARTITION_INDEX.');

End update_partition_stats;

  Procedure update_index_table           (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          index_in        IN varchar2)

IS
Begin

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_geometry_index SET table_name = table_in 
  WHERE owner = owner_in AND column_name = column_in
        AND index_name = index_in;
  COMMIT;

End update_index_table;

  Procedure update_index_stats           (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          blevel_in       IN number,
                                          leafblocks_in   IN number,
                                          clstfct_in      IN number,
                                          avgcellcnt_in   IN number,
                                          numrows_in      IN number,
                                          samplesize_in   IN number,
                                          distkeys_in     IN number)

IS
  date_in       date := NULL;
  userstats_in  varchar2(3) := NULL;
   
Begin

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_geometry_index SET blevel = blevel_in, 
         leaf_blocks = leafblocks_in, clustering_factor = clstfct_in, 
         density = round(avgcellcnt_in,2), num_rows = numrows_in, last_analyzed = SYSDATE,
         sample_size = samplesize_in, user_stats = 'NO', distinct_keys = distkeys_in
  WHERE owner = owner_in AND table_name = table_in AND 
        column_name = column_in;
  COMMIT;

End update_index_stats;

  Procedure update_index_mbr             (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          eminx           IN number,
                                          eminy           IN number,
                                          emaxx           IN number,
                                          emaxy           IN number)
IS
  date_in       date := NULL;
  userstats_in  varchar2(3) := NULL;
   
Begin

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_geometry_index SET minx = eminx, miny = eminy,
         maxx = emaxx, maxy = emaxy
  WHERE owner = owner_in AND table_name = table_in AND 
        column_name = column_in;
  COMMIT;

End update_index_mbr;

  Procedure update_partition_index_mbr   (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          partition_in    IN varchar2,
                                          eminx           IN number,
                                          eminy           IN number,
                                          emaxx           IN number,
                                          emaxy           IN number)
IS
  date_in       date := NULL;
  userstats_in  varchar2(3) := NULL;
   
Begin

  LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_partition_index SET minx = eminx, miny = eminy,
         maxx = emaxx, maxy = emaxy
  WHERE owner = owner_in AND table_name = table_in AND 
        partition_name = partition_in;
  COMMIT;

End update_partition_index_mbr;



  Procedure update_index_numnulls        (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          num_nulls       IN number)

IS

Begin

  LOCK TABLE SDE.st_geometry_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_geometry_index SET num_nulls = num_nulls, last_analyzed = SYSDATE
  WHERE owner = owner_in AND table_name = table_in AND 
        column_name = column_in;

End update_index_numnulls;

  Procedure update_partition_numnulls    (owner_in        IN varchar2,
                                          table_in        IN varchar2,
                                          column_in       IN varchar2,
                                          partition_in    IN varchar2,
                                          num_nulls       IN number)

IS

Begin
  LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

  UPDATE SDE.st_partition_index SET num_nulls = num_nulls, last_analyzed = SYSDATE
  WHERE owner = owner_in AND table_name = table_in AND 
        column_name = column_in AND partition_name = partition_in;
  COMMIT;

Exception
  When NO_DATA_FOUND Then
    raise_application_error (SDE.st_type_util.spx_partition_not_found,'ST_PARTITION_INDEX '||partition_in||' not in ST_PARTITION_INDEX.');

End update_partition_numnulls;

  Procedure update_partition_name        (spx_info_r      IN SDE.spx_util.spx_record_t,
                                          properties      IN SDE.spx_util.st_geom_prop_t,
                                          partition_in    IN varchar2,
                                          new_partition   IN varchar2)
IS
  res           integer;
  table_name    varchar2(256);
  new_table     varchar2(256);
  idx2_name     varchar2(256);
  idx3_name     varchar2(256);
  new_idx2      varchar2(256);
  new_idx3      varchar2(256);
  stmt1         varchar2(1000);
  part_name     varchar2(32);
Begin

  res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
  If(res != 0) Then
    LOCK TABLE SDE.st_partition_index IN EXCLUSIVE MODE;

    table_name := spx_info_r.owner||'.'||'S'||spx_info_r.index_id||partition_in;
    new_table := 'S'||spx_info_r.index_id||new_partition;
    idx2_name := 'S'||spx_info_r.index_id||partition_in||'P';
    new_idx2 := 'S'||spx_info_r.index_id||new_partition||'P';
    idx3_name := spx_info_r.owner||'.'||'S'||spx_info_r.index_id||partition_in||'X';
    new_idx3 := 'S'||spx_info_r.index_id||new_partition||'X';
    Begin
      stmt1 := 'ALTER TABLE '||table_name||' RENAME CONSTRAINT '||idx2_name||' TO '||new_idx2;
      EXECUTE IMMEDIATE stmt1;
      stmt1 := 'ALTER TABLE '||table_name||' RENAME TO '||new_table;
      EXECUTE IMMEDIATE stmt1;

      stmt1 := 'ALTER INDEX '||idx3_name||' RENAME TO '||new_idx3;
      EXECUTE IMMEDIATE stmt1;

      table_name := 'S'||spx_info_r.index_id||partition_in;
      stmt1 := 'UPDATE SDE.st_partition_index SET partition_name = '''||new_partition||
      ''' WHERE owner = '''||spx_info_r.owner||''' AND table_name = '''||spx_info_r.table_name||
      ''' AND column_name = '''||spx_info_r.column_name||''' AND partition_name = '''||partition_in||'''';

      EXECUTE IMMEDIATE stmt1;
      COMMIT;

    Exception
      When Others then 
        dbms_output.put_line('ERROR '||sqlerrm||' '||sqlcode);
    End;

  Else
    table_name := 'S'||partition_in||'_PX$';
    new_table := 'S'||new_partition||'_PX$';

    stmt1 := 'ALTER TABLE '||table_name||' RENAME TO '||new_table;
    EXECUTE IMMEDIATE stmt1;
  End If;

Exception
  When NO_DATA_FOUND Then
    raise_application_error (SDE.st_type_util.spx_partition_not_found,'ST_PARTITION_INDEX '||partition_in||' not in ST_PARTITION_INDEX.');
 When Others then 
        dbms_output.put_line('ERROR '||sqlerrm||' '||sqlcode);

End update_partition_name;

Procedure calc_extent (ia           IN sys.odciindexinfo,
                       eminx        IN OUT number,
                       eminy        IN OUT number,
                       emaxx        IN OUT number,
                       emaxy        IN OUT number)
  /***********************************************************************
  *
  *N  {calc_extent}  --  Calc extent of ST_Geometry using spatial 
  *                      index.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function calculates the extent of an ST_Geometry class using
  *  the spatial index table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:       
  *     ia                <IN>      ==  sys.odciindexinfo,
  *     fminx             <IN OUT>  ==  extent info number
  *     fminy             <IN OUT>  ==  extent info number
  *     fmaxx             <IN OUT>  ==  extent info number
  *     fmaxy             <IN OUT>  ==  extent info number
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  * 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          11/01/2010           Original coding.
  *E
  ***********************************************************************/
  IS
    
    TYPE C_Ref_T IS REF CURSOR;
    TYPE T_ptable IS TABLE OF varchar2(32);

    ptable_t       T_ptable := T_ptable();
    C_Ref          C_Ref_T;
    stmt           varchar2(512);
    geom_id        integer;
    xyunits        number;
    x_offset       float;
    y_offset       float;
    res            number;
    iminx          number;
    iminy          number;
    imaxx          number;
    imaxy          number;
    fminx          number;
    fminy          number;
    fmaxx          number;
    fmaxy          number;
    spid           dbms_sql.urowid_table;
    s_table        varchar2(32);

    rc                pls_integer;
    optype            pls_integer;
    spx_info_r        SDE.spx_util.spx_record_t;
    sp_ref_r          SDE.spx_util.spatial_ref_record_t;
    properties        SDE.spx_util.st_geom_prop_t;
    spatial_column    varchar2(32);

  Begin

    spatial_column := REPLACE(ia.indexcols(1).colname,'"','');

    optype := SDE.spx_util.st_geom_operation_dml;
    rc := SDE.spx_util.get_object_info(ia,optype,NULL,spx_info_r,sp_ref_r,properties);
    If rc != SDE.spx_util.se_success THEN
      raise_application_error (SDE.st_type_util.spx_object_noexist,'Object '||ia.indexcols(1).tableschema||
                               '.'||ia.indexcols(1).tablename||'.'||spatial_column||' not found in ST_GEOMETRY_INDEX.');
    End If;

    stmt := 'SELECT a.geom_id,b.x_offset,b.y_offset,b.xyunits '||
            'FROM SDE.ST_Geometry_Columns a,SDE.ST_Spatial_References b '||
            'WHERE a.owner = :owner_l AND a.table_name = :table_l AND '||
            '      a.column_name = :column_l and a.srid = b.srid';

    OPEN C_Ref FOR stmt USING ia.indexcols(1).tableschema, ia.indexcols(1).tablename,spatial_column;
    FETCH C_Ref INTO geom_id,x_offset,y_offset,xyunits;
    CLOSE C_Ref;

    res := bitand(properties,SDE.st_geom_util.ST_GEOM_PROP_PARTITION_INDEX);
    If(res != 0) Then
      stmt := 'SELECT partition_name FROM SDE.st_partition_index '||
              'WHERE owner = '''||ia.indexcols(1).tableschema||''' AND table_name = '''||ia.indexcols(1).tablename||''' AND '||
              'column_name = '''||spatial_column||'''';
      OPEN C_Ref FOR stmt;
      FETCH C_Ref BULK COLLECT INTO ptable_t;
      CLOSE C_Ref;

    Else
      stmt := 'SELECT partition_name FROM all_tab_partitions '||
              'WHERE table_owner = '''||ia.indexcols(1).tableschema||''' AND table_name = '''||ia.indexcols(1).tablename||'''';
      OPEN C_Ref FOR stmt;
      FETCH C_Ref BULK COLLECT INTO ptable_t;
      If C_Ref%NOTFOUND Then
        ptable_t.extend(1);
        ptable_t(1) := 'S'||spx_info_r.index_id||'_IDX$';
      Else 
        CLOSE C_Ref;
      End If;
    End If;

    FOR i IN 1..ptable_t.COUNT LOOP

      s_table := SDE.spx_util.get_partition_name(ptable_t(i),properties,spx_info_r.index_id);
      stmt := 'SELECT min(minx), min(miny), max(maxx), max(maxy) '||
              'FROM '||ia.indexcols(1).tableschema||'.'||s_table;
      OPEN C_Ref FOR stmt;
      FETCH C_Ref INTO iminx,iminy,imaxx,imaxy;
      CLOSE C_Ref;

      If i = 1 Then
        eminx := ((iminx / xyunits) + x_offset);
        eminy := ((iminy / xyunits) + y_offset);
        emaxx := ((imaxx / xyunits) + x_offset);
        emaxy := ((imaxy / xyunits) + y_offset);
        If(res != 0) Then
          update_partition_index_mbr(ia.indexcols(1).tableschema,
                                     ia.indexcols(1).tablename,
                                     ptable_t(i),
                                     trunc(eminx,9),
                                     trunc(eminy,9),
                                     trunc(emaxx,9),
                                     trunc(emaxy,9));
        End If;
      Else
        fminx := ((iminx / xyunits) + x_offset);
        fminy := ((iminy / xyunits) + y_offset);
        fmaxx := ((imaxx / xyunits) + x_offset);
        fmaxy := ((imaxy / xyunits) + y_offset);

        If(res != 0) Then
          update_partition_index_mbr(ia.indexcols(1).tableschema,
                                     ia.indexcols(1).tablename,
                                     ptable_t(i),
                                     trunc(fminx,9),
                                     trunc(fminy,9),
                                     trunc(fmaxx,9),
                                     trunc(fmaxy,9));
        End If;

        IF fminx < eminx Then
          eminx := fminx;
        End If;
        IF fminy < eminy Then
          eminy := fminy;
        End If;
        IF fmaxx > emaxx Then
          emaxx := fmaxx;
        End If;
        IF fmaxy > emaxy Then
          emaxy := fmaxy;
        End If;
      End If;
    End Loop;

    ptable_t.delete;

    eminx := trunc(eminx,9);
    eminy := trunc(eminy,9);
    emaxx := trunc(emaxx,9);
    emaxy := trunc(emaxy,9);

Exception
 When Others then 
        dbms_output.put_line('ERROR '||sqlerrm||' '||sqlcode);
  End calc_extent;

  Procedure update_layers_sp_mode (owner_in       IN      varchar2,
                                   table_in       IN      varchar2,
                                   column_in      IN      varchar2,
                                   sp_mode        IN      pls_integer)
 /***********************************************************************
*
*n  {update_layers_eflags}  --  Update LAYERS eflags spatial index 
*                               status. 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     This function checks the LAYERS eflags status of the spatial index
*  and updates it to reflect an active index. 
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     owner              IN    varchar2    owner name
*     table_name         IN    varchar2    table name
*     column_name        IN    varchar2    spatial column
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          08/25/11           original coding.
*e
***********************************************************************/
IS
  C_layers_sel      SDE.spx_util.C_Ref_T;
  stmt1             varchar2(1000);
  eflags            number;
  mask_eflags       number;
  hex_eflags        varchar2(30);
  hex_load          varchar2(30);
  hex_no_index      varchar2(30);
  hex_mask          varchar2(30);
  layer_id          integer;
  res               number;

Begin

  stmt1 := 'SELECT eflags,layer_id '||
           'FROM SDE.Layers '||
           'WHERE owner = :owner_in AND table_name = :table_in AND '||
                 'spatial_column = :column_in';

  Open c_layers_sel For stmt1 Using owner_in,table_in,column_in;

  Fetch c_layers_sel INTO eflags,layer_id;
  If c_layers_sel%NOTFOUND THEN
    Raise NO_DATA_FOUND;
  End If;
  Close c_layers_sel;

  mask_eflags  := eflags;
  hex_eflags   := lpad(trim(to_char(eflags,'XXXXXXXXXXXXXXXXXXXX')),20,'0');
  hex_load     := lpad(trim(to_char(SDE.spx_util.mask_load_only,'XXXXXXXXXXXXXXXXXXXX')),20,'0');
  hex_no_index := lpad(trim(to_char(SDE.spx_util.mask_has_no_index,'XXXXXXXXXXXXXXXXXXXX')),20,'0');

  res := to_number(utl_raw.bit_and(hex_load,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');
  
  If (sp_mode = SDE.st_geom_util.LAYERS_HAS_INDEX) Then
    if res != 0 Then
      mask_eflags := to_number(utl_raw.bit_xor(hex_load,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');
      hex_eflags  := lpad(trim(to_char(mask_eflags,'XXXXXXXXXXXXXXXXXXXX')),20,'0');
    End If;

    res := to_number(utl_raw.bit_and(hex_no_index,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');

    if res != 0 Then
      mask_eflags := to_number(utl_raw.bit_xor(hex_no_index,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');
    End If;

    If mask_eflags != eflags Then
      Begin
        stmt1 := 'BEGIN SDE.layers_util.update_layer_eflags('||''''||owner_in||''''||','||
                        ''''||table_in||''''||','||''''||column_in||''''||','||mask_eflags||'); END;';

        EXECUTE IMMEDIATE stmt1;
      Exception
        When Others Then 
          NULL;
      End;
    End If;
  Elsif (sp_mode = SDE.st_geom_util.LAYERS_NO_INDEX) Then
    if res = 0 Then
      mask_eflags := to_number(utl_raw.bit_xor(hex_load,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');
      hex_eflags  := lpad(trim(to_char(mask_eflags,'XXXXXXXXXXXXXXXXXXXX')),20,'0');
    End If;

    res := to_number(utl_raw.bit_and(hex_no_index,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');

    if res = 0 Then
      mask_eflags := to_number(utl_raw.bit_xor(hex_no_index,hex_eflags),'XXXXXXXXXXXXXXXXXXXX');
    End If;

    If mask_eflags != eflags Then
      Begin
        stmt1 := 'BEGIN SDE.layers_util.update_layer_eflags('||''''||owner_in||''''||','||
                        ''''||table_in||''''||','||''''||column_in||''''||','||mask_eflags||'); END;';

        EXECUTE IMMEDIATE stmt1;

      Exception
        When Others Then 
          NULL;
      End;
    End If;
  End if;

Exception 
  When NO_DATA_FOUND Then
    NULL;

  When Others Then
    NULL;
End update_layers_sp_mode;

  Begin
/***********************************************************************
 *
 *n  {global initialization}  --  initialize global state
 *
 ***********************************************************************/

   g_current_user := SDE.st_type_user.type_user;
   g_type_dba := g_current_user = SDE.st_type_user.c_type_dba;
  
End spx_util;

/


Prompt Grants on PACKAGE SPX_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SPX_UTIL TO PUBLIC WITH GRANT OPTION
/
