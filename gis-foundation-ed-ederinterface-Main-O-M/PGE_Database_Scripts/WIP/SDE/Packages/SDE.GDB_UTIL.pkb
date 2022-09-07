Prompt drop Package Body GDB_UTIL;
DROP PACKAGE BODY SDE.GDB_UTIL
/

Prompt Package Body GDB_UTIL;
--
-- GDB_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.gdb_util 
/***********************************************************************
*
*N  {gdb_util.spb}  --  Geodatanase PL/SQL utility package
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines useful constants and type
*   definitions for use by other Geodatabase procedures.  
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2010 ESRI
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
*    Kevin Watt             10/28/2010            Original coding.
*E
***********************************************************************/
IS

   /* Local Subprograms */

PROCEDURE L_table_name   (owner_in   IN SDE.table_registry.owner%TYPE,
                          table_in   IN SDE.table_registry.table_name%TYPE,
                          table_out  IN OUT SDE.table_registry.table_name%TYPE)
/***********************************************************************
  *
  *N  {L_table_name}  --  Return the true table name for version view
  *                       named class
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the true table name from the table_registry
  *  when the input table name is a versioned view. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:       
  *     owner_in           <IN>      ==  owner 
  *     table_in           <IN>      ==  table name
  *     table_out          <IN OUT>  ==  true table name
  *
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
  *    Kevin Watt          03/10/2011           Original coding.
  *E
  ***********************************************************************/

  IS
    C_gdb          C_GDB_Items_T;
    owner_l        SDE.table_registry.owner%TYPE;
    c_table        SDE.table_registry.owner%TYPE;
    stmt           varchar2(512);

  Begin

    owner_l   := UPPER(owner_in);
    table_out := UPPER(table_in);

    -- check if the object is a multivesioned view

    stmt := 'SELECT table_name FROM SDE.Table_Registry '||
            'WHERE owner = :owner_in AND imv_view_name = :table_in';

    OPEN C_gdb FOR stmt USING owner_l, table_out;
    FETCH C_gdb INTO c_table;
    If C_gdb%FOUND Then
      table_out := UPPER(c_table);
    End If;
    CLOSE C_gdb;

  End L_table_name;

  PROCEDURE L_stgeom_calc_extent (owner_l     IN nvarchar2,
                                  table_l     IN nvarchar2,
                                  column_l    IN nvarchar2,
                                  fminx       IN OUT number,
                                  fminy       IN OUT number,
                                  fmaxx       IN OUT number,
                                  fmaxy       IN OUT number)
  /***********************************************************************
  *
  *N  {L_stgeom_calc_extent}  --  Calc extent of ST_Geometry using spatial 
  *                               index.
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
  *     owner_l           <IN>      ==  owner 
  *     table_l           <IN>      ==  table name
  *     column_l          <IN>      ==  spatial column
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
    
    C_gdb          C_GDB_Items_T;
    p_name         nvarchar2(64);
    stmt           varchar2(512);
    geom_id        integer;
    xyunits        number;
    x_offset       float;
    y_offset       float;
    res            number;
    iminx          sde.int_array_tab := sde.int_array_tab();
    iminy          sde.int_array_tab := sde.int_array_tab();
    imaxx          sde.int_array_tab := sde.int_array_tab();
    imaxy          sde.int_array_tab := sde.int_array_tab();
    bminx          sde.flt_array_tab := sde.flt_array_tab();
    bminy          sde.flt_array_tab := sde.flt_array_tab();
    bmaxx          sde.flt_array_tab := sde.flt_array_tab();
    bmaxy          sde.flt_array_tab := sde.flt_array_tab();
    spid           dbms_sql.urowid_table;
    eminx          number;
    eminy          number;
    emaxx          number;
    emaxy          number;
    s_table        varchar2(30);

  BEGIN

    p_name := UPPER(owner_l||'.'||table_l);

    stmt := 'SELECT a.geom_id,b.x_offset,b.y_offset,b.xyunits '||
            'FROM SDE.ST_Geometry_Columns a,SDE.ST_Spatial_References b '||
            'WHERE a.owner = :owner_l AND a.table_name = :table_l AND '||
            '      a.column_name = :column_l and a.srid = b.srid';

    OPEN C_gdb FOR stmt USING owner_l, table_l,column_l;
    FETCH C_gdb INTO geom_id,x_offset,y_offset,xyunits;
    If C_gdb%NOTFOUND THEN
      CLOSE C_gdb;
     
      stmt := 'SELECT a.'||column_l||'.minx,a.'||column_l||'.miny,a.'||
                      column_l||'.maxx,a.'||column_l||'.maxy '||
              'FROM '||p_name||' a '||
              ' WHERE a.'||column_l||' IS NOT NULL';

      OPEN C_gdb FOR stmt;
      FETCH C_gdb BULK COLLECT INTO bminx,bminy,bmaxx,bmaxy;
      
      fminx := bminx(1);
      fminy := bminy(1);
      fmaxx := bmaxx(1);
      fmaxy := bmaxy(1);

      FOR i IN 1..bminx.LAST LOOP

        IF bminx(i) < fminx Then
          fminx := bminx(i);
        End If;

        IF bminy(i) < fminy Then
          fminy := bminy(i);
        End If;

        IF bmaxx(i) > fmaxx Then
          fmaxx := bmaxx(i);
        End If;

        IF bmaxy(i) > fmaxy Then
          fmaxy := bmaxy(i);
        End If;

      END LOOP;

      bminx.delete;
      bminy.delete;
      bmaxx.delete;
      bmaxy.delete;

    Else
  
      CLOSE C_gdb;
      s_table := 'S'||geom_id||'_IDX$';
  
      stmt := 'SELECT minx,miny,maxx,maxy FROM '||owner_l||'.'||s_table;

      OPEN C_gdb for stmt;
      FETCH C_gdb BULK COLLECT INTO iminx,iminy,imaxx,imaxy;

      fminx := ((iminx(1) / xyunits) + x_offset) ;
      fminy := ((iminy(1) / xyunits) + y_offset) ;
      fmaxx := ((imaxx(1) / xyunits) + x_offset) ;
      fmaxy := ((imaxy(1) / xyunits) + y_offset) ;

      FOR i IN 1..iminx.LAST LOOP

        eminx := ((iminx(i) / xyunits) + x_offset);
        IF eminx < fminx Then
          fminx := eminx;
        End If;

        eminy := ((iminy(i) / xyunits) + y_offset);
        IF eminy < fminy Then
          fminy := eminy;
        End If;

        emaxx := ((imaxx(i) / xyunits) + x_offset);
        IF emaxx > fmaxx Then
          fmaxx := emaxx;
        End If;

        emaxy := ((imaxy(i) / xyunits) + y_offset);
        IF emaxy > fmaxy Then
          fmaxy := emaxy;
        End If;

      END LOOP;

      iminx.delete;
      iminy.delete;
      imaxx.delete;
      imaxy.delete;

    End If;

    CLOSE C_gdb;
    fminx := trunc(fminx,5);
    fminy := trunc(fminy,5);
    fmaxx := trunc(fmaxx,5);
    fmaxy := trunc(fmaxy,5);

  End L_stgeom_calc_extent;

   /* Public Subprograms. */

  FUNCTION    next_rowid (owner_in     IN SDE.table_registry.owner%TYPE,
                          table_in     IN SDE.table_registry.table_name%TYPE)
    RETURN NUMBER
  /***********************************************************************
  *
  *N  {next_rowid}  --  Returns next ROWID from sequence
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the next sequence value from a registered
  *  table. A global cache is used to maintain sequence values for
  *  multiple tables. ROWID sequences hand out 16 values per call to
  *  nextval. The cache is used to hold values as needed. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb          C_GDB_Items_T;
    C_seq          C_SEQ_T;
    reg_id         pls_integer;
    c_table        SDE.table_registry.table_name%TYPE;
    seq_name       varchar2(64);
    p_name         varchar2(64);
    stmt           varchar2(512);
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
    s_value        number;

  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);
    
    stmt := 'SELECT registration_id FROM SDE.table_registry '||
            'WHERE owner = :owner_l AND table_name = :table_l';

    OPEN C_gdb FOR stmt USING owner_l,table_l;
    FETCH C_gdb INTO reg_id;
    IF C_gdb%NOTFOUND THEN
      raise_application_error(SDE.sde_util.SE_TABLE_NOREGISTERED,
                              'Class '||p_name||' not registered to the Geodatabase.');
    End If;
    CLOSE C_gdb;

    s_value := SDE.version_user_ddl.next_row_id (owner_l,reg_id);
    return(s_value);

  END next_rowid;

  FUNCTION is_simple (owner_in    IN SDE.table_registry.owner%TYPE,
                      table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {is_simple}  --  Returns is_simple(TRUE) or not(FALSE).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function performs a check to see if the table is simple based 
  *  on several criteria from the GDB metadata tables.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb             C_GDB_Items_T;
    c_xmldata         clob;
    c_table           SDE.table_registry.table_name%TYPE;
    pos               pls_integer;
    pos2              pls_integer;
    s_buffer          varchar2(32);
    p_name            varchar2(64);
    stmt              varchar2(1024);
    i_val             integer;
    i_id              integer;
    is_reg            boolean := TRUE;
    owner_l           SDE.table_registry.owner%TYPE;
    table_l           SDE.table_registry.table_name%TYPE;
    i_datasetsubtype1 integer;
    
  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    -- Check ArcSDE metadata first

    stmt := 'SELECT registration_id FROM SDE.Table_Registry WHERE owner = : owner_in AND table_name = :table_in';

    OPEN C_gdb FOR stmt USING owner_l,table_l;
    FETCH C_gdb INTO i_id;
    IF C_gdb%NOTFOUND Then
      is_reg := FALSE;
    End If;
    CLOSE C_gdb;

If is_reg = TRUE Then
  dbms_output.put_line('Is_Reg: TRUE');
Else
  dbms_output.put_line('Is_Reg: FALSE');
End If;

    -- Get XML string from view. Return NOT REGISTERED when not found. 

    stmt := 'SELECT definition,datasetsubtype1 FROM SDE.GDB_Items_vw WHERE physicalname = :name';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO c_xmldata,i_datasetsubtype1;
    IF C_gdb%NOTFOUND Then
      CLOSE C_gdb;
      IF is_reg = TRUE Then
        return('TRUE');
      Else
        return('NOT REGISTERED');
      End If;
    End If;
    CLOSE C_gdb;

dbms_output.put_line('DS: '||i_datasetsubtype1);

    If i_datasetsubtype1 IS NULL OR i_datasetsubtype1 <> 1 Then
dbms_output.put_line('DatasetSubtype is not simple.');
      return('FALSE');
    End If;

    -- Check FeatureType for esriFTSimple. This check also 
    -- covers checks for:
    --
    --   * Dimention feature classes
    --   * Annotation eature classes
    --   * Schematics, Locators, and Toolboxes

    pos := 0;
    pos := instr(c_xmldata,'<FeatureType>',1);
    If pos > 0 Then
      pos := pos + 13;
      pos2 := instr(c_xmldata,'</FeatureType>',1);
      s_buffer := UPPER(substr(c_xmldata,pos,pos2 - pos));
dbms_output.put_line('SIMPLE: '||s_buffer);
      If 'ESRIFTSIMPLE' != s_buffer Then
dbms_output.put_line('ESRIFtSimple is not Simple');
        return('FALSE');
      End If;
    End If;

    i_val := 0;
    stmt := 'SELECT 1 FROM SDE.GDB_Items a '||
            'WHERE a.PhysicalName = :name and a.Type IN '||
                 '(SELECT b.UUID FROM SDE.GDB_ItemTypes b '||
                  'WHERE b.Name in (''Feature Class'',''Feature Dataset'',''Table''))';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO i_val;
    CLOSE C_gdb;

    if i_val != 1 Then
dbms_output.put_line('Not a FC, FD or Table - GDB_ItemTypes');
      return('FALSE');
    End If;

    -- Check if the object participates in a	Parcel Fabric, Networkdataset, 
    -- Geometric Network, Terrain, Networkdataset, Topology or Relationship.  

    i_val := 0;
    stmt := 'SELECT 1 '||
            'FROM '||
                 '(SELECT b.originid '||
                  'FROM SDE.GDB_Items a, SDE.GDB_ItemRelationships b '||
                  'WHERE a.physicalname = :name and a.uuid = b.destid) objclass '||
              'INNER JOIN '||
                'SDE.GDB_Items origin_items '||
              'ON origin_items.uuid = objclass.originid '||
              'INNER JOIN '||
                'SDE.GDB_ItemRelationships  rel1 '||
              'ON rel1.originid = origin_items.uuid and origin_items.physicalname IS NOT NULL '||
              'INNER JOIN '||
                'SDE.GDB_ItemRelationships rel2 '||
              'ON rel2.destid = rel1.destid and '||
                  '((rel2.type = ''{583A5BAA-3551-41AE-8AA8-1185719F3889}'') OR '||
                  '(rel2.type = ''{DC739A70-9B71-41E8-868C-008CF46F16D7}'') OR '||
                  '(rel2.type = ''{55D2F4DC-CB17-4E32-A8C7-47591E8C71DE}'') OR '||
                  '(rel2.type = ''{B32B8563-0B96-4D32-92C4-086423AE9962}'') OR '||
                  '(rel2.type = ''{D088B110-190B-4229-BDF7-89FDDD14D1EA}'') OR '||
                  '(rel2.type = ''{725BADAB-3452-491B-A795-55F32D67229C}''))  and rownum = 1';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO i_val;
    CLOSE C_gdb;

    if i_val = 1 Then
dbms_output.put_line('Object Participates in either a Parcel Fabric, Networkdataset, ');
dbms_output.put_line('       Geometric Network, Terrain, Networkdataset, Topology or Relationship.');
      return('FALSE');
    End If;

    -- Check if Dataset has dependent objects that participate in a Parcel Fabric
    -- Networkdataset, Geometric Network, Terrain, Networkdataset, Topology or Relationship.

    i_val := 0;
    stmt := 'SELECT 1 '||
            'FROM '||
                  '(SELECT rel2.uuid '||
                  'FROM '||
                        '(SELECT UUID, Type FROM SDE.GDB_Items WHERE PhysicalName = :name) src_items '||
                        'INNER JOIN '||
                           'SDE.GDB_Itemrelationships  rel1 '||
                        'ON src_items.uuid = rel1.originid '||
                        'INNER JOIN '||
                           'SDE.GDB_Itemrelationships rel2 '||
                        'ON rel2.originid = rel1.destid and ((rel2.type = ''{583A5BAA-3551-41AE-8AA8-1185719F3889}'') OR '||
 						                                                     '(rel2.type = ''{DC739A70-9B71-41E8-868C-008CF46F16D7}'') OR '||
 						                                                     '(rel2.type = ''{55D2F4DC-CB17-4E32-A8C7-47591E8C71DE}'') OR '||
 						                                                     '(rel2.type = ''{B32B8563-0B96-4D32-92C4-086423AE9962}'') OR '||
 						                                                     '(rel2.type = ''{D088B110-190B-4229-BDF7-89FDDD14D1EA}'') OR '||
                                                            '(rel2.type = ''{725BADAB-3452-491B-A795-55F32D67229C}'')) and rownum = 1)';
    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO i_val;
    CLOSE C_gdb;

    if i_val = 1 Then
dbms_output.put_line('Dataset has dependent class objects that participate in Parcel Fabric, ');
dbms_output.put_line('        Networkdataset, Geometric Network, Terrain, Networkdataset, Topology or Relationship.');
      return('FALSE');
    End If;

    -- Check if Object (No Dataset) has dependent objects that participate in a Parcel Fabric
    -- Networkdataset, Geometric Network, Terrain, Networkdataset, Topology or Relationship.

    i_val := 0;
    stmt := 'SELECT 1 '||
            'FROM '||
                  '(SELECT rel1.type '||
                   'FROM '||
                         '(SELECT UUID, Type FROM SDE.GDB_Items WHERE PhysicalName = :name) src_items '||
                         'INNER JOIN '||
                            'SDE.GDB_Itemrelationships  rel1 '||
                         'ON rel1.originid = src_items.uuid '||
                         'INNER JOIN '||
                             'SDE.GDB_Itemrelationships rel2 '||
                         'ON rel2.destid = rel1.destid and ((rel2.type = ''{583A5BAA-3551-41AE-8AA8-1185719F3889}'') OR '||
 						                                                    '(rel2.type = ''{DC739A70-9B71-41E8-868C-008CF46F16D7}'') OR '||
 						                                                    '(rel2.type = ''{55D2F4DC-CB17-4E32-A8C7-47591E8C71DE}'') OR '||
 						                                                    '(rel2.type = ''{B32B8563-0B96-4D32-92C4-086423AE9962}'') OR '||
 						                                                    '(rel2.type = ''{D088B110-190B-4229-BDF7-89FDDD14D1EA}'') OR '||
                                                           '(rel2.type = ''{725BADAB-3452-491B-A795-55F32D67229C}'')) and rownum = 1)';
    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO i_val;
    CLOSE C_gdb;

    if i_val = 1 Then
dbms_output.put_line('Object is not part of a Dataset (Attachment)but participates in Parcel Fabric, ');
dbms_output.put_line('        Networkdataset, Geometric Network, Terrain, Networkdataset, Topology or Relationship.');
      return('FALSE');
    End If;

    -- Check XML Definition for any one of several .

    pos := 0;
    pos := instr(c_xmldata,'<ControllerMemberships>',1);
    If pos = 0 Then
      pos := instr(c_xmldata,'<ControllerMemberships ',1);
    End If;

    If pos > 0 Then
dbms_output.put_line('Found ControllerMemberships tag.');

      pos := 0;
      pos := instr(c_xmldata,'<GeometricNetworkMembership>',1);
      if pos > 0 Then
dbms_output.put_line('Participates in a	GeometricNetworkMembership');
        return('FALSE');
      End If;

      pos := instr(c_xmldata,'<TopologyMembership>',1);
      if pos > 0 Then
dbms_output.put_line('Participates in a	TopologyMembership');
        return('FALSE');
      End If;

      pos := instr(c_xmldata,'<NetworkDatasetMembership>',1);
      if pos > 0 Then
dbms_output.put_line('Participates in a	NetworkDatasetMembership');
        return('FALSE');
      End If;

      pos := instr(c_xmldata,'<NetworkDatasetName>',1);
      if pos > 0 Then
dbms_output.put_line('Participates in a	NetworkDataset');
        return('FALSE');
      End If;

      pos := instr(c_xmldata,'<TerrainMembership>',1);
      If pos = 0 Then
        pos := instr(c_xmldata,'<TerrainName>',1);
      End If;

      if pos > 0 Then
dbms_output.put_line('Participates in a	GPTerrainMembership');
        return('FALSE');
      End If;

    End If;

    -- Check for Editor Tracking enabled.

    pos := 0;
    pos := instr(c_xmldata,'<EditorTrackingEnabled>',1);
    If pos > 0 Then
      pos := pos + 23;
      pos2 := instr(c_xmldata,'</EditorTrackingEnabled>',1);
      s_buffer := UPPER(substr(c_xmldata,pos,pos2 - pos));
      If 'TRUE' = s_buffer Then
        return('FALSE');
      End If;
    End If;
   
    -- Check for Custom Class Extensions. 

    pos := 0;
    pos := instr(c_xmldata,'<EXTCLSID>',1);
    If pos > 0 Then
      pos := pos + 10;
      pos2 := instr(c_xmldata,'</EXTCLSID>',1);
 dbms_output.put_line('EXTCLSID: Pos: '||pos||' pos2: '||pos2);
      If (pos2 != pos) Then
        return('FALSE');
      End If;
    End If;

    return('TRUE');
    
  End is_simple;

  FUNCTION rowid_name (owner_in    IN SDE.table_registry.owner%TYPE,
                       table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {rowid_name}  --  Returns ROWID name from GDB_Items definition 
  *                     field.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the OIDFieldName value from the GDB_Items
  *  definition (XML) field.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb          C_GDB_Items_T;
    c_xmldata      clob;
    pos            pls_integer;
    pos2           pls_integer;
    c_table        SDE.table_registry.table_name%TYPE;
    rowid_name     varchar2(32);
    p_name         varchar2(64);
    stmt           varchar2(512);
    i_val          pls_integer;
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;

  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    stmt := 'SELECT definition FROM SDE.GDB_Items_vw WHERE physicalname = :name';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO c_xmldata;
    IF C_gdb%NOTFOUND THEN
      raise_application_error(SDE.sde_util.SE_TABLE_NOREGISTERED,
                            'Class '||p_name||' not registered to the Geodatabase.');
    End If;
    CLOSE C_gdb;

    pos := instr(c_xmldata,'<OIDFieldName>',1);
    pos := pos + 14;
    pos2 := instr(c_xmldata,'</OIDFieldName>',1);
    rowid_name := UPPER(substr(c_xmldata,pos,pos2 - pos));

    return(rowid_name);

  END rowid_name;

  FUNCTION is_versioned (owner_in    IN SDE.table_registry.owner%TYPE,
                         table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {is_versioned}  --  Returns versioned status (TRUE) or not(FALSE).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the versioned status from the GDB_Items
  *  definition (XML) field.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb          C_GDB_Items_T;
    c_xmldata      clob;
    c_table        SDE.table_registry.table_name%TYPE;
    pos            pls_integer;
    pos2           pls_integer;
    isversioned    varchar2(32);
    p_name         varchar2(64);
    stmt           varchar2(512);
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
   
  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    -- Get XML string from view. Return NOT REGISTERED when not found. 

    stmt := 'SELECT definition FROM SDE.GDB_Items_vw WHERE physicalname = :name';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO c_xmldata;
    IF C_gdb%NOTFOUND THEN
      return('NOT REGISTERED');
    End If;
    CLOSE C_gdb;

    pos := instr(c_xmldata,'<Versioned>',1);
    pos := pos + 11;
    pos2 := instr(c_xmldata,'</Versioned>',1);
    isversioned := UPPER(substr(c_xmldata,pos,pos2 - pos));

    return(isversioned);

  End is_versioned;

  FUNCTION version_view_name (owner_in    IN SDE.table_registry.owner%TYPE,
                              table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {version_view_name}  --  Returns versioned view nam).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the versiond view name from the ArcSDE
  *   metadata table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb          C_GDB_Items_T;
    c_table        SDE.table_registry.table_name%TYPE;
    p_name         varchar2(64);
    stmt           varchar2(512);
    i_val          pls_integer;
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
    view_l         SDE.table_registry.imv_view_name%TYPE;
     
  BEGIN

    owner_l := UPPER(owner_in);
    table_l := UPPER(table_in);
    p_name := UPPER(owner_in||'.'||table_in);

    -- check if the object is a multivesioned view

    stmt := 'SELECT imv_view_name FROM SDE.Table_Registry '||
            'WHERE owner = :owner_in AND table_name = :table_in '||
            'AND bitand(object_flags,power(2,3)) <> 0';

    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO c_table;
    IF C_gdb%NOTFOUND THEN
      return(NULL);
    End If;
    CLOSE C_gdb;

    view_l := UPPER(c_table);
    i_val  := 0;
    
    stmt := 'SELECT 1 FROM All_Views '||
            'WHERE owner = :owner_in AND view_name = :view_in';
    
    OPEN C_gdb FOR stmt USING owner_l, view_l;
    FETCH C_gdb INTO i_val;
    IF C_gdb%NOTFOUND THEN
      return(NULL);
    End If;
    CLOSE C_gdb;
            
    If i_val = 1 Then   
      return(c_table);
    Else
      return(NULL);  
    End If;
    
  End version_view_name;


  FUNCTION is_archive_enabled (owner_in    IN SDE.table_registry.owner%TYPE,
                               table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {is_archive_enabled}  --  Returns archiving status (TRUE) or not(FALSE).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the archiving status based on the 
  *   ArcSDE metadata.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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
  *    Sanjay Magal          02/2013           Original coding.
  *E
  ***********************************************************************/
  IS 

    C_gdb          C_GDB_Items_T;
    i_val          pls_integer;
    isarchived     varchar2(32);
    stmt           varchar2(512);
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
   
  BEGIN

    isarchived := 'FALSE';  
    i_val   := 0;
    owner_l := UPPER(owner_in);

    -- check if the object is a multiversioned view
    L_table_name(owner_in,table_in,table_l);

    stmt := 'SELECT 1 FROM SDE.Table_Registry '||
            'WHERE owner = :owner_in AND table_name = :table_in';
            
    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO i_val;
    IF C_gdb%NOTFOUND THEN
      return('NOT REGISTERED');
    End If;
    CLOSE C_gdb;

    stmt := 'SELECT 1 FROM SDE.Table_Registry '||
            'WHERE owner = :owner_in AND table_name = :table_in '||
            'AND bitand(object_flags,power(2,18)) <> 0';
            
    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO i_val;
    IF C_gdb%NOTFOUND THEN
      return(isarchived);
    End If;
    CLOSE C_gdb;
            
    If i_val = 1 Then   
      isarchived := 'TRUE';
    Else
      isarchived := 'FALSE';   
    End If;

    return(isarchived);

  End is_archive_enabled;

  FUNCTION archive_view_name (owner_in    IN SDE.table_registry.owner%TYPE,
                              table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {archive_view_name}  --  Returns non-versioned archive view name).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the non-versiond archive view name 
  *   from the ArcSDE metadata table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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
  *    Sanjay Magal          02/2013           Original coding.
  *E
  ***********************************************************************/
  IS 

    C_gdb          C_GDB_Items_T;
    c_table        SDE.table_registry.imv_view_name%TYPE;
    stmt           varchar2(512);
    i_val          pls_integer;
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
    view_l         SDE.table_registry.imv_view_name%TYPE;
     
  BEGIN

    owner_l := UPPER(owner_in);
    table_l := UPPER(table_in);

    stmt := 'SELECT imv_view_name FROM SDE.Table_Registry '||
            'WHERE owner = :owner_in AND table_name = :table_in '||
            'AND bitand(object_flags,power(2,18)) <> 0 '||             
            'AND bitand(object_flags,power(2,3)) = 0';

    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO c_table;
    IF C_gdb%NOTFOUND THEN
      return(NULL);
    End If;
    CLOSE C_gdb;

    view_l := UPPER(c_table);
    i_val  := 0;
    
    stmt := 'SELECT 1 FROM All_Views '||
            'WHERE owner = :owner_in AND view_name = :view_in';
    
    OPEN C_gdb FOR stmt USING owner_l, view_l;
    FETCH C_gdb INTO i_val;
    IF C_gdb%NOTFOUND THEN
      return(NULL);
    End If;
    CLOSE C_gdb;
            
    If i_val = 1 Then   
      return(c_table);
    Else
      return(NULL);  
    End If;
    
  End archive_view_name;


  FUNCTION get_extent (owner_in    IN SDE.layers.owner%TYPE,
                       table_in    IN SDE.layers.table_name%TYPE,
                       column_in   IN SDE.layers.spatial_column%TYPE,
                       op          IN varchar2,
                       minx        IN OUT number,
                       miny        IN OUT number,
                       maxx        IN OUT number,
                       maxy        IN OUT number)
  RETURN pls_integer
/***********************************************************************
  *
  *N  {get_extent}  --  Returns extent).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the extent of a spatial type class 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (layers.owner%TYPE) owner
  *     table_in          <IN>  ==  (layers.table_name%TYPE) table
  *     column_in         <IN>  ==  (layers.spaital_column%TYPE) spatial
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

    CURSOR c_geom_stats (owner_wanted IN VARCHAR2, table_wanted IN VARCHAR2, 
                         column_wanted IN VARCHAR2) IS
      SELECT minx,miny,maxx,maxy
      FROM   SDE.st_geometry_index
      WHERE  owner = owner_wanted AND table_name = table_wanted 
      AND column_name = column_wanted;

    C_gdb          C_GDB_Items_T;
    c_table        SDE.table_registry.table_name%TYPE;
    p_name         nvarchar2(64);
    geom_type      varchar2(32);
    stmt           varchar2(512);
    owner_l        nvarchar2(30);
    table_l        nvarchar2(64);
    column_l       nvarchar2(30);
    option_l       varchar2(32);
    is_registered  boolean;
    i_flags        integer;
    i_base_id      integer;
    extent_str     varchar2(512);

  BEGIN

    column_l := UPPER(column_in);
    option_l := UPPER(op);

    -- check option 

    If 'CALCULATE' != option_l and 'METADATA' != option_l  Then
      raise_application_error (SDE.sde_util.SE_INVALID_PARAM_VALUE,
                               'Invalid option - get_extent options are ''CALCULATE'' or ''METADATA''');
    End If;

    -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    -- Check spatial type 

    stmt := 'SELECT data_type FROM   all_tab_columns '||
            'WHERE  owner = :owner_in AND table_name = :table_in AND '||
                   'column_name = :column_in';

    OPEN C_gdb FOR stmt USING owner_l,table_l,column_l;
    FETCH C_gdb INTO geom_type;
    IF C_gdb%NOTFOUND THEN
      raise_application_error (SDE.sde_util.SE_TABLE_NOEXIST,
                               'Class '||p_name||' does not exist.');
    End If;
    CLOSE C_gdb;

    If geom_type != 'ST_GEOMETRY' and geom_type != 'SDO_GEOMETRY' Then
      raise_application_error (SDE.sde_util.SE_INVALID_SPATIAL_COLUMN,
                               'Table '||p_name||' must have a valid'||
                               ' spatial type (SDE.ST_GEOMETRY or MDSYS.SDO_GEOMETRY.');
    End If;

    -- check if the object is registered

    stmt := 'SELECT table_name, eflags, base_layer_id,minx,miny,maxx,maxy FROM SDE.Layers '||
            'WHERE owner = :owner_in AND table_name = :table_in';

    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO c_table,i_flags,i_base_id,minx,miny,maxx,maxy;
    IF C_gdb%NOTFOUND THEN
      is_registered := FALSE;
    ElsIf C_gdb%FOUND Then
      is_registered := TRUE;
    End If;
    CLOSE C_gdb;
  
    If option_l = 'CALCULATE' Then

      If geom_type = 'ST_GEOMETRY' Then
        L_stgeom_calc_extent (owner_l,table_l,column_l,minx,miny,maxx,maxy);
      Else
        stmt := 'select SDO_GEOM.SDO_MIN_MBR_ORDINATE(a.mbr,1),'||
                'SDO_GEOM.SDO_MIN_MBR_ORDINATE(a.mbr,2),'||
                'SDO_GEOM.SDO_MAX_MBR_ORDINATE(a.mbr,1),'||
                'SDO_GEOM.SDO_MAX_MBR_ORDINATE(a.mbr,2) from '||
               '(select sdo_tune.extent_of('''||p_name||''','''||column_l||''') mbr from dual) a';
     
        OPEN C_gdb FOR stmt;
        FETCH C_gdb INTO minx,miny,maxx,maxy;
        CLOSE C_gdb;

      End If;

    Else

      If is_registered = FALSE Then
        If geom_type = 'ST_GEOMETRY' Then
          OPEN C_geom_stats (owner_l, table_l,column_l);
          FETCH C_geom_stats INTO minx,miny,maxx,maxy;
          IF C_geom_stats%NOTFOUND THEN
            return(NULL);
          End If;
          CLOSE C_geom_stats;

          If minx = SDE.spx_util.int_max And miny = SDE.spx_util.int_max And
             maxx = 0 And maxy = 0 Then
            return(NULL);
          End If;

        Else

          stmt := 'SELECT SDO_GEOM.SDO_MIN_MBR_ORDINATE(a.sdo_root_mbr,1),'||
                   'SDO_GEOM.SDO_MIN_MBR_ORDINATE(a.sdo_root_mbr,2),'||                                       
                   'SDO_GEOM.SDO_MAX_MBR_ORDINATE(a.sdo_root_mbr,1),'||
                   'SDO_GEOM.SDO_MAX_MBR_ORDINATE(a.sdo_root_mbr,2) '||
                   'FROM all_sdo_index_metadata a, all_ind_columns b '||
                   'WHERE b.index_name = a.sdo_index_name AND '||
                   'b.table_owner = a.sdo_index_owner AND '||
                   'b.table_owner = :owner_in AND '|| 
                   'b.table_name = :table_in AND '||
                   'b.column_name = :column_in';

          OPEN C_gdb FOR stmt USING owner_l,table_l,column_l;
          FETCH C_gdb INTO minx,miny,maxx,maxy;
          If  C_gdb%NOTFOUND Then
            return(NULL);
          End If;
          CLOSE C_gdb;

        End If;
      End If;
    End If;

    return(SDE.spx_util.se_success);
  End get_extent;

  FUNCTION get_extent (owner_in    IN SDE.layers.owner%TYPE,
                       table_in    IN SDE.layers.table_name%TYPE,
                       column_in   IN SDE.layers.spatial_column%TYPE,
                       minx        IN OUT number,
                       miny        IN OUT number,
                       maxx        IN OUT number,
                       maxy        IN OUT number)
  RETURN pls_integer
/***********************************************************************
  *
  *N  {get_extent}  --  Returns extent).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the extent of a spatial type class using 
  * the default option 'CALCULATE'.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (layers.owner%TYPE) owner
  *     table_in          <IN>  ==  (layers.table_name%TYPE) table
  *     column_in         <IN>  ==  (layers.spaital_column%TYPE) spatial
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
    ret            pls_integer;

  BEGIN

    ret := SDE.gdb_util.get_extent(owner_in,table_in,column_in,'calculate',minx,miny,maxx,maxy);
    if(ret != SDE.spx_util.se_success) Then
      return(NULL);
    End If;

    return(ret);

  End get_extent;

  FUNCTION geometry_columns (owner_in    IN SDE.table_registry.owner%TYPE,
                             table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN clob 
/***********************************************************************
  *
  *N  {geometry_columns}  --  Returns the shape column names.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedures returns a clob of all shape column names.
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>      ==  owner
  *     table_in          <IN>      ==  table
  *     
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

    C_ref          C_GDB_Items_T;
    p_name         nvarchar2(64);
    stmt           varchar2(1024);
    geom_col_c     clob;
    column_tab     t_varchar2;
    owner_l        nvarchar2(30);
    table_l        nvarchar2(64);
  
  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);


    stmt := 'SELECT column_name '||
            'FROM   all_tab_columns '||
            'WHERE  owner = :owner_l AND '||
                   '(table_name = :table_l AND data_type = ''ST_GEOMETRY'') OR '||
                   '(table_name = :table_l AND data_type = ''SDO_GEOMETRY'')';

    OPEN C_ref FOR stmt USING owner_l,table_l,table_l;
    FETCH c_ref BULK COLLECT INTO column_tab;
    IF C_ref%ROWCOUNT = 0 THEN
      return(NULL);
    End If;
    If NOT c_ref%ISOPEN Then
      raise_application_error (SDE.sde_util.SE_TABLE_NOEXIST,
                               'Class '||p_name||' does not have a spatial column.');
    End If;

    FOR i IN 1..C_ref%ROWCOUNT
    LOOP
      geom_col_c := geom_col_c||column_tab(i);
      If i < C_ref%ROWCOUNT Then
        geom_col_c := geom_col_c||' ';
      End If;
    End LOOP;
 
    Return(geom_col_c);

  END geometry_columns;

  FUNCTION geometry_column_type (owner_in    IN SDE.table_registry.owner%TYPE,
                                 table_in    IN SDE.table_registry.table_name%TYPE,
                                 column_in   IN nvarchar2)
  RETURN varchar2 
/***********************************************************************
  *
  *N  {geometry_column_types}  --  Returns the shape column types.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedures returns a ref cursor for a selection to return
  *  all shape column names and their dbms spatial types 
  *  (ST_Geometry or SDO_Geometry). 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>      ==  owner
  *     table_in          <IN>      ==  table name
  *     column_in         <IN>      ==  shape column
  *     
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

    C_ref          sys_refcursor;
    p_name         nvarchar2(64);
    stmt           varchar2(1024);
    owner_l        nvarchar2(30);
    table_l        nvarchar2(64);
    column_l       nvarchar2(32);
    type_out       varchar2(64);
  
  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);
    column_l := UPPER(column_in);


    stmt := 'SELECT data_type '||
            'FROM   all_tab_columns '||
            'WHERE  owner = :owner_l AND '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''ST_GEOMETRY'') OR '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''SDO_GEOMETRY'')';

    OPEN C_ref FOR stmt USING owner_l,table_l,column_l,table_l,column_l;
    FETCH c_ref INTO type_out;
    IF C_ref%ROWCOUNT = 0 THEN
      return(NULL);
    End If;
    CLOSE C_ref;

    Return(type_out);

  END geometry_column_type;

  FUNCTION geometry_type (owner_in    IN SDE.table_registry.owner%TYPE,
                          table_in    IN SDE.table_registry.table_name%TYPE,
                          column_in   IN nvarchar2)
  RETURN clob 
/***********************************************************************
  *
  *N  {geometry_column_types}  --  Returns the shape column geometry type.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedures returns a ref cursor for a selection to return
  *  shape column geometry type or types depending on the registration 
  *  of the class.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>      ==  owner
  *     table_in          <IN>      ==  table name
  *     column_in         <IN>      ==  shape column
  *     
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

    C_ref          sys_refcursor;
    C_gdb          C_GDB_Items_T;
    c_table        SDE.table_registry.table_name%TYPE;
    p_name         nvarchar2(64);
    i_geom_type    varchar2(32);
    stmt           varchar2(2000);
    owner_l        nvarchar2(30);
    table_l        nvarchar2(64);
    column_l       nvarchar2(32);
    i_column_l     nvarchar2(32);
    is_registered  boolean;
    i_flags        SDE.layers.eflags%TYPE;
    i_cs_id        SDE.st_spatial_references.cs_id%TYPE;
    i_definition   SDE.st_spatial_references.definition%TYPE;
    i_type2        varchar2(32);
    i_type4        varchar2(32);
    i_type8        varchar2(32);
    i_type16       varchar2(32);
    i_part         varchar2(32);
    i_3d           varchar2(32);
    i_measure      varchar2(32);
    i_cad          varchar2(32);
    i_entity       integer;
    i_srid         integer;
    geom_type_l    varchar2(32);
    geom_type_tab  t_varchar2 := t_varchar2();
    entity         varchar2(32);
    minz_t         t_float := t_float();
    minm_t         t_float := t_float();
    sdo_gtype      t_int := t_int();
  
  BEGIN

    -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);
    column_l := UPPER(column_in);


    -- check if the object is registered

    stmt := 'SELECT a.spatial_column,c.srid,c.cs_id,c.definition,'||
                   'decode(bitand(a.eflags,2),2,''POINT'') T_point,'||
                   'decode(bitand(a.eflags,4),4,''LINESTRING'') T_line,'||
                   'decode(bitand(a.eflags,8),8,''SIMPLELINESTRING'') T_sline,'||
                   'decode(bitand(a.eflags,16),16,''POLYGON'') T_poly,'||
                   'decode(bitand(a.eflags,262144),262144,''MULTI'') Part,'||
                   'decode(bitand(a.eflags,65536),65536,''3D'') is_3d,'||
                   'decode(bitand(a.eflags,524288),524288, ''Measure'') is_measure,'||
                   'decode(bitand(a.eflags,4194304),4194304,''CAD'') is_CAD,'||
                   'decode(bitand(a.eflags,486539264),16777216,''SDEBinary'',67108864,''ST_Geometry'','||
                                          '134217728,''SDO_Geometry'',268435456,''SDELOB'') Geom_type '||
            'FROM SDE.layers a, SDE.spatial_references b, SDE.st_spatial_references c '||
            'WHERE a.owner = :owner_in AND a.table_name = :table_in AND '||
                  'a.srid = b.srid AND b.auth_srid = c.srid';

    OPEN C_gdb FOR stmt USING owner_l, table_l;
    FETCH C_gdb INTO i_column_l,i_srid,i_cs_id,i_definition,i_type2,i_type4,i_type8,i_type16,i_part,i_3d,i_measure,i_cad,i_geom_type;
    IF C_gdb%NOTFOUND THEN
      is_registered := FALSE;
    ElsIf C_gdb%FOUND Then
      is_registered := TRUE;
    End If;

    If is_registered = TRUE Then
 
      If i_part IS NOT NULL Then
        entity := i_part;
      End If;

      If i_type2 IS NOT NULL Then
        entity := entity||i_type2||' ';
      Elsif i_type16 IS NOT NULL Then
        entity := entity||i_type16||' ';
      Elsif i_type4 IS NOT NULL AND i_type8 IS NOT NULL Then
        entity := entity||i_type8||' ';
      Elsif i_type4 IS NOT NULL Then
        entity := entity||i_type4||' ';
      Elsif i_type8 IS NOT NULL Then
        entity := entity||i_type4||' ';
      Else
        entity := 'EMPTY';
      End If;

      If i_3d IS NOT NULL Then
        entity := entity||'z';
      End If;

      If i_measure IS NOT NULL Then
        entity := entity||'m';
      End If;

      return(entity);
    
    Else

     stmt := 'SELECT data_type '||
            'FROM   all_tab_columns '||
            'WHERE  owner = :owner_l AND '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''ST_GEOMETRY'') OR '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''SDO_GEOMETRY'')';

      OPEN C_gdb FOR stmt USING owner_l,table_l,column_l,table_l,column_l;
      FETCH C_gdb INTO geom_type_l;
      IF C_gdb%NOTFOUND And C_gdb%ROWCOUNT = 0 THEN
        return(NULL);
      End If;
      CLOSE C_gdb;

      If geom_type_l = 'ST_GEOMETRY' Then
      
        stmt := 'SELECT distinct(decode(a.'||column_l||'.entity,1,''POINT'',2,''LINESTRING'',4,''LINESTRING'',8,''POLYGON'','||
                        '257,''MULTIPOINT'',258,''MULTILINESTRING'',260,''MULTILINESTRING'','||
                        '264,''MULTIPOLYGON'')),a.'||column_l||'.minz,a.'||column_l||'.minm '||
                'FROM '||owner_l||'.'||table_l||' a '||
                'WHERE a.'||column_l||' IS NOT NULL';
                   
        OPEN C_gdb FOR stmt;
        FETCH C_gdb BULK COLLECT INTO geom_type_tab,minz_t,minm_t;
        IF C_gdb%NOTFOUND And C_gdb%ROWCOUNT = 0 THEN
          return(NULL);
        End If;

        stmt := NULL;
    
        FOR i IN 1..C_gdb%ROWCOUNT
        LOOP
 
          stmt := stmt||geom_type_tab(i)||' ';
         
          If minz_t(i) IS NOT NULL Then
            stmt := stmt||'Z';
          End If;

          If minm_t(i) IS NOT NULL Then
            stmt := stmt||'M';
          End If;

        End LOOP;
        CLOSE C_gdb;

      Else

        stmt := 'SELECT distinct(decode(a.'||column_l||'.sdo_gtype,2001,''POINT'',2002,''LINESTRING'',2003,''POLYGON'',2004,''COLLECTION'','||
                                '2005,''MULTIPOINT'',2006,''MULTILINESTRING'',2007,''MULTIPOLYGON'','||
                                '3001,''POINT Z'',3002,''LINESTRING Z'',3003,''POLYGON Z'',3004,''COLLECTION Z'','||
                                '3005,''MULTIPOINT Z'',3006,''MULTILINESTRING Z'',3007,''MULTIPOLYGON Z'','||
                                '3301,''POINT M'',3302,''LINESTRING M'',3303,''POLYGON M'',3304,''COLLECTION M'','||
                                '3305,''MULTIPOINT M'',3306,''MULTILINESTRING M'',3307,''MULTIPOLYGON M'','||
                                '4001,''POINT ZM'',4002,''LINESTRING ZM'',4003,''POLYGON ZM'',4004,''COLLECTION ZM'','||
                                '4005,''MULTIPOINT ZM'',4006,''MULTILINESTRING ZM'',4007,''MULTIPOLYGON ZM'','||
                                '4301,''POINT ZM'',4302,''LINESTRING ZM'',4303,''POLYGON ZM'',4304,''COLLECTION ZM'','||
                                '4305,''MULTIPOINT ZM'',4306,''MULTILINESTRING ZM'',4307,''MULTIPOLYGON ZM'','||
                                '4401,''POINT ZM'',4402,''LINESTRING ZM'',4403,''POLYGON ZM'',4404,''COLLECTION ZM'','||
                                '4405,''MULTIPOINT ZM'',4406,''MULTILINESTRING ZM'',4407,''MULTIPOLYGON ZM'')) GTYPE '||
                'FROM '||owner_l||'.'||table_l||' a '||
                'WHERE a.'||column_l||' IS NOT NULL';

        OPEN C_gdb FOR stmt;
        FETCH C_gdb BULK COLLECT INTO geom_type_tab;
        IF C_gdb%NOTFOUND And C_gdb%ROWCOUNT = 0 THEN
          return(NULL);
        End If;

        stmt := NULL;

        FOR i IN 1..C_gdb%ROWCOUNT 
        LOOP
          stmt := stmt||geom_type_tab(i);
          If i != C_gdb%ROWCOUNT Then
            stmt := stmt||' ';
          End If;

        End LOOP;

      End If;

    End If;

    Return(stmt);

  END geometry_type;

  PROCEDURE spatial_ref_info (owner_in    IN SDE.layers.owner%TYPE,
                              table_in    IN SDE.layers.table_name%TYPE,
                              column_in   IN SDE.layers.spatial_column%TYPE,
                              wkid        IN OUT integer,
                              wkt         IN OUT varchar2,
                              st_srid     IN OUT integer)
/***********************************************************************
  *
  *N  {shape_properties}  --  Returns the shape properties of the class).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedures returns the shape properties if the input class
  *  has a spatial type. Properties are returned for reigstered and
  *  spatially-enabled tables. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>      ==  owner
  *     table_in          <IN>      ==  table
  *     column_out        <IN>      ==  spatial column 
  *     wkid              <IN OUT>  ==  EPSG id
  *     wkt               <IN OUT>  ==  projection text
  *     esri_srid         <IN OUT>  ==  ESRI SRID
  *     
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

    TYPE t_varchar2 IS TABLE OF varchar2(32);
  
    C_gdb          C_GDB_Items_T;
    c_table        SDE.table_registry.table_name%TYPE;
    p_name         nvarchar2(64);
    i_geom_type    varchar2(32);
    stmt           varchar2(1024);
    owner_l        nvarchar2(30);
    table_l        nvarchar2(64);
    column_l       nvarchar2(32);
    is_registered  boolean;
    i_cs_id        SDE.st_spatial_references.cs_id%TYPE;
    i_definition   SDE.st_spatial_references.definition%TYPE;
    t_geom_type    t_varchar2;
    t_column_l     t_varchar2;
    i_entity       integer;
    i_srid         integer;

  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);
    column_l := UPPER(column_in);

    st_srid := NULL;

    -- check if the object is registered

    stmt := 'SELECT c.srid,c.cs_id,c.definition, '||
                   'decode(bitand(a.eflags,486539264),16777216,''SDEBINARY'',67108864,''ST_GEOMETRY'','||
                                          '134217728,''SDO_GEOMETRY'',268435456,''SDELOB'') Geom_type '||
            'FROM SDE.layers a, SDE.spatial_references b, SDE.st_spatial_references c '||
            'WHERE a.owner = :owner_in AND a.table_name = :table_in AND '||
                  'a.spatial_column = :column_in AND '||
                  'a.srid = b.srid AND b.auth_srid = c.srid';

    OPEN C_gdb FOR stmt USING owner_l, table_l,column_l;
    FETCH C_gdb INTO i_srid,i_cs_id,i_definition,i_geom_type;
    IF C_gdb%NOTFOUND THEN
      is_registered := FALSE;
    ElsIf C_gdb%FOUND Then
      is_registered := TRUE;
    End If;
    CLOSE C_gdb;

    If is_registered = TRUE Then

      If 'ST_GEOMETRY' = i_geom_type Then
        st_srid := i_srid;
      End If;

      If i_cs_id IS NOT NULL Then
        wkid := i_cs_id;
      End If;

      If i_definition IS NOT NULL Then
        wkt := i_definition;
      End If;
    
    Else
      stmt := 'SELECT data_type '||
              'FROM   all_tab_columns '||
              'WHERE  owner = :owner_l AND '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''ST_GEOMETRY'') OR '||
                   '(table_name = :table_l AND column_name = :column_in AND data_type = ''SDO_GEOMETRY'')';

      OPEN C_gdb FOR stmt USING owner_l,table_l,column_l,table_l,column_l;
      FETCH C_gdb INTO i_geom_type;
      CLOSE C_gdb;

      If i_geom_type = 'ST_GEOMETRY' Then

        stmt := 'SELECT a.srid,b.cs_id, b.definition '||
                'FROM SDE.st_geometry_columns a,SDE.st_spatial_references b '||
                'WHERE a.owner = :onwer_l And a.table_name = :table_l and a.column_name = :column_l and '||
                      'a.srid = b.srid';
        OPEN C_gdb FOR stmt USING owner_l,table_l,column_l;
        FETCH C_gdb INTO i_srid,i_cs_id,i_definition;
        IF C_gdb%NOTFOUND THEN
          raise_application_error (SDE.sde_util.SE_TABLE_NOEXIST,
                                 'Class '||p_name||' has no registered metadata.');
        End If;

        If i_srid IS NOT NULL Then
          st_srid := i_srid;
        End If;

        If i_cs_id IS NOT NULL Then
          wkid := i_cs_id;
        End If;

        If i_definition IS NOT NULL Then
          wkt := i_definition;
        End If;
        
      Else
        stmt := 'SELECT '||'a.'||column_l||'.sdo_srid '||
                'FROM '||owner_l||'.'||table_l||' a '||
                'WHERE a.'||column_l||' IS NOT NULL AND rownum = 1';

        OPEN C_gdb FOR stmt;
        FETCH C_gdb INTO wkid;
        CLOSE C_gdb;

        If wkid IS NOT NULL Then
          stmt := 'SELECT wktext '||
                  'FROM mdsys.cs_srs '||
                  'WHERE srid = '||wkid;

          OPEN C_gdb FOR stmt;
          FETCH C_gdb INTO wkt;
          CLOSE C_gdb;
        Else
          wkid := NULL;
          wkt := NULL;
        End If;

      End If;

    End If; 
  
  END spatial_ref_info;

  FUNCTION globalid_name (owner_in    IN SDE.table_registry.owner%TYPE,
                          table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {globalid_name}  --  Returns GLOBALID name from GDB_Items definition 
  *                        field.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the GlobalIDFieldName value from the
  *   GDB_Items definition (XML) field.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb          C_GDB_Items_T;
    c_xmldata      clob;
    pos            pls_integer;
    pos2           pls_integer;
    c_table        SDE.table_registry.table_name%TYPE;
    rowid_name     varchar2(32);
    p_name         varchar2(64);
    stmt           varchar2(512);
    i_val          pls_integer;
    owner_l        SDE.table_registry.owner%TYPE;
    table_l        SDE.table_registry.table_name%TYPE;
    strval1        varchar2(32);

  BEGIN

    strval1 := NULL;

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    stmt := 'SELECT definition FROM SDE.GDB_Items_vw WHERE physicalname = :name';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO c_xmldata;
    IF C_gdb%NOTFOUND THEN
      raise_application_error(SDE.sde_util.SE_TABLE_NOREGISTERED,
                            'Class '||p_name||' not registered to the Geodatabase.');
    End If;
    CLOSE C_gdb;

    pos := instr(c_xmldata,'<HasGlobalID>',1);
    pos := pos + 13;
    pos2 := instr(c_xmldata,'</HasGlobalID>',1);
    If (pos2 - pos) < 32 Then
      strval1 := UPPER(substr(c_xmldata,pos,pos2 - pos));
    End If;

    If 'TRUE' = strval1 Then
      pos := instr(c_xmldata,'<GlobalIDFieldName>',1);
      pos := pos + 19;
      pos2 := instr(c_xmldata,'</GlobalIDFieldName>',1);
      strval1 := UPPER(substr(c_xmldata,pos,pos2 - pos));
    Elsif 'FALSE' = strval1 Then
      return(NULL);
    End If;
    
    return(strval1);

  END globalid_name;

  FUNCTION next_globalid
  RETURN nchar
/***********************************************************************
  *
  *N  {globalid_name}  --  Returns GLOBALID
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns a GUID. 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *
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
  
  BEGIN

    return (SDE.version_user_ddl.retrieve_guid());

  End next_globalid;

  FUNCTION is_replicated (owner_in    IN SDE.table_registry.owner%TYPE,
                          table_in    IN SDE.table_registry.table_name%TYPE)
  RETURN varchar2
/***********************************************************************
  *
  *N  {is_replicated}  --  Returns True if object participates in a 
  *                        replication dataset.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function performs a check to see if the table is part of a
  *  replication dataset. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_in          <IN>  ==  (table_registry.owner%TYPE) owner
  *     table_in          <IN>  ==  (table_registry.table_name%TYPE) table
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

    C_gdb             C_GDB_Items_T;
    c_table           SDE.table_registry.table_name%TYPE;
    p_name            varchar2(64);
    stmt              varchar2(1024);
    i_val             integer;
    owner_l           SDE.table_registry.owner%TYPE;
    table_l           SDE.table_registry.table_name%TYPE;
    i_datasetsubtype1 integer;
    
  BEGIN

   -- check if the object is a multivesioned view

    owner_l := UPPER(owner_in);
    L_table_name(owner_in,table_in,table_l);
    p_name := UPPER(owner_in||'.'||table_l);

    i_val := 0;
    stmt := 'SELECT 1 '||
            'FROM '||
                '(SELECT UUID, Type FROM SDE.GDB_Items WHERE PhysicalName = :name) objclass '||
               'INNER JOIN '||
                  'SDE.GDB_Itemrelationships rel1 '||
               'ON rel1.destid = objclass.uuid '||
                  'and ((rel1.type = ''{D022DE33-45BD-424C-88BF-5B1B6B957BD3}'') OR '||
                  '(rel1.type = ''{8DB31AF1-DF7C-4632-AA10-3CC44B0C6914}'')) and rownum = 1';

    OPEN C_gdb FOR stmt USING p_name;
    FETCH C_gdb INTO i_val;
    IF C_gdb%NOTFOUND Then
      return 'FALSE';
    End If;
    CLOSE C_gdb;

    If i_val = 1 Then
      return ('TRUE');
    Else
      return('FALSE');
    End If;

  End is_replicated;

  FUNCTION is_geodatabase
  RETURN varchar2
/***********************************************************************
  *
  *N  {is_geodatabase}  --  Returns True Geodatabase exists.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function performs a check to see if the GDB schema is 
  *  present.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    
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
    C_gdb             C_GDB_Items_T;
    i_val             integer;
    stmt              varchar2(256);

  Begin

    i_val := -1;
    Begin

      stmt := 'SELECT count(*) FROM '||user||'.gdb_tables_last_modified';
      OPEN C_gdb FOR stmt;
      FETCH C_gdb INTO i_val;
      IF C_gdb%NOTFOUND Then
        return 'FALSE';
      End If;
      CLOSE C_gdb;

    Exception 
      WHEN OTHERS THEN
        NULL;
    End;

    If i_val = -1 Then
      stmt := 'SELECT count(*) FROM SDE.gdb_tables_last_modified';
      OPEN C_gdb FOR stmt;
      FETCH C_gdb INTO i_val;
      IF C_gdb%NOTFOUND Then
        return 'FALSE';
      End If;
      CLOSE C_gdb;
    End If;

    return 'TRUE';

    Exception
      WHEN OTHERS THEN
        return 'FALSE';

  End is_geodatabase;

END gdb_util;

/


Prompt Grants on PACKAGE GDB_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.GDB_UTIL TO PUBLIC WITH GRANT OPTION
/
