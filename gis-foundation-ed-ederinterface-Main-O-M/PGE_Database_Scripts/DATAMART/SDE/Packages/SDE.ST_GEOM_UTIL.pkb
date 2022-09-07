Prompt drop Package Body ST_GEOM_UTIL;
DROP PACKAGE BODY SDE.ST_GEOM_UTIL
/

Prompt Package Body ST_GEOM_UTIL;
--
-- ST_GEOM_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_geom_util
/***********************************************************************
*
*n  {st_Geom_Util.sps}  --  st_Geometry type functions and procedures.  
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this pl/sql package specification defines st_Geometry type 
*    constructor functions/procedures to instantiate a st_Geometry 
*    row.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  legalese:
*
*   copyright 1992-2005 esri
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
*    kevin watt          12/02/04               original coding.
*   
*e
***********************************************************************/
IS

   Procedure get_type     (geom_str      IN Out  clob,
                           entity        IN Out  number,
                           geom_type     IN Out  number,
                           is_empty      IN Out  boolean,
                           type_in       IN      number)
/***********************************************************************
*
*n  {get_Type}  --  parse entity type from geom string 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this procedure parses the entity type from the geometry
*  string.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     geom_Str    <in>     ==  (clob) geom string.
*     entity      <in out> ==  (number) entity 
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          12/02/04           original coding.
*e
***********************************************************************/
  IS
   lt_param    pls_integer := 0;
   first_space pls_integer := 0;
   second_space pls_integer := 0;
   dim1_space  pls_integer := 0;
   tot_len     pls_integer := 0;
   is_zm       pls_integer := 0;
   is_z        pls_integer := 0;
   is_m        pls_integer := 0;
   empty_pos   pls_integer := 0;

   dtype       varchar2(128);
   dimension   varchar2(128);
   str1        varchar2(128);

begin

   geom_str := upper(geom_str);
   geom_str := ltrim(geom_str,' ');
   
   empty_pos := instr(geom_str,'EMPTY');
   if(empty_pos > 0) then
     geom_str := rtrim(geom_str);

     if(length(geom_str) > 5) then

       first_space := instr(geom_str,' ');
       second_space := instr(geom_str,' ',1,2);
       
       if(second_space > 0 and second_space < empty_pos) then
         dtype := substr(geom_str,1,second_space);
         if(instr(dtype,'MULTIPOINT ZM') > 0) then
           geom_type := SDE.st_geom_util.multipointzm_type;
           entity := sg_multi_point_shape;
         elsif(instr(dtype,'MULTIPOINT Z') > 0) then
           geom_type := SDE.st_geom_util.multipointz_type;
           entity := sg_multi_point_shape;
         elsif(instr(dtype,'MULTIPOINT M') > 0) then
           geom_type := SDE.st_geom_util.multipointm_type;
           entity := sg_multi_point_shape;                  
         elsif(instr(dtype,'POINT ZM') > 0) then
           geom_type := SDE.st_geom_util.pointzm_type;
           entity := sg_point_shape;
         elsif(instr(dtype,'POINT Z') > 0) then
           geom_type := SDE.st_geom_util.pointz_type;
           entity := sg_point_shape;
         elsif(instr(dtype,'POINT M') > 0) then
           geom_type := SDE.st_geom_util.pointm_type;
           entity := sg_point_shape;                     
         elsif(instr(dtype,'MULTILINESTRING ZM') > 0) then
           geom_type := SDE.st_geom_util.multilinestringzm_type;
           entity := sg_multi_simple_line_shape;
         elsif(instr(dtype,'MULTILINESTRING Z') > 0) then
           geom_type := SDE.st_geom_util.multilinestringz_type;
           entity := sg_multi_simple_line_shape;
         elsif(instr(dtype,'MULTILINESTRING M') > 0) then
           geom_type := SDE.st_geom_util.multilinestringm_type;
           entity := sg_multi_simple_line_shape;                      
         elsif(instr(dtype,'LINESTRING ZM') > 0) then
           geom_type := SDE.st_geom_util.linestringzm_type;
           entity := sg_simple_line_shape;
         elsif(instr(dtype,'LINESTRING Z') > 0) then
           geom_type := SDE.st_geom_util.linestringz_type;
           entity := sg_simple_line_shape;
         elsif(instr(dtype,'LINESTRING M') > 0) then
           geom_type := SDE.st_geom_util.linestringm_type;
           entity := sg_simple_line_shape;                                 
         elsif(instr(dtype,'MULTIPOLYGON ZM') > 0) then       
           geom_type := SDE.st_geom_util.multipolygonzm_type;
           entity := sg_multi_area_shape;
         elsif(instr(dtype,'MULTIPOLYGON Z') > 0) then       
           geom_type := SDE.st_geom_util.multipolygonz_type;
           entity := sg_multi_area_shape;
         elsif(instr(dtype,'MULTIPOLYGON M') > 0) then       
           geom_type := SDE.st_geom_util.multipolygonm_type;
           entity := sg_multi_area_shape;                   
         elsif(instr(dtype,'POLYGON ZM') > 0) then  
           geom_type := SDE.st_geom_util.polygonzm_type;
           entity := sg_area_shape;
         elsif(instr(dtype,'POLYGON Z') > 0) then  
           geom_type := SDE.st_geom_util.polygonz_type;
           entity := sg_area_shape;
         elsif(instr(dtype,'POLYGON M') > 0) then  
           geom_type := SDE.st_geom_util.polygonm_type;
           entity := sg_area_shape;             
         else
           raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type "'||dtype||'" is not a valid shape type.');
         end if;
              
       elsif(first_space < empty_pos) then
         dtype := substr(geom_str,1,first_space);
         if(instr(dtype,'MULTIPOINT') > 0) then
           geom_type := SDE.st_geom_util.multipoint_type;
           entity := sg_multi_point_shape;
         elsif(instr(dtype,'POINT') > 0) then
           geom_type := SDE.st_geom_util.point_type;
           entity := sg_point_shape;
         elsif(instr(dtype,'MULTILINESTRING') > 0) then
           geom_type := SDE.st_geom_util.multilinestring_type;
           entity := sg_multi_simple_line_shape;
         elsif(instr(dtype,'LINESTRING') > 0) then       
           geom_type := SDE.st_geom_util.linestring_type;
           entity := sg_simple_line_shape;
         elsif(instr(dtype,'MULTIPOLYGON') > 0) then       
           geom_type := SDE.st_geom_util.multipolygon_type;
           entity := sg_multi_area_shape;
         elsif(instr(dtype,'POLYGON') > 0) then  
           geom_type := SDE.st_geom_util.polygon_type;
           entity := sg_area_shape;
         else
           raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type "'||dtype||'" is not a valid shape type.');
         end if;

       else
         raise_application_error (SDE.st_type_util.st_geom_text_invalid,'Invalid EMPTY Geometry text format "'||geom_str||'".');
       end if;
     else
       if(type_in = SDE.st_geom_util.multipoint_type) then
         geom_type := SDE.st_geom_util.multipoint_type;
         entity := sg_multi_point_shape;
         geom_str := 'MULTIPOINT EMPTY';
       elsif(type_in = SDE.st_geom_util.point_type) then
         geom_type := SDE.st_geom_util.point_type;
         entity := sg_point_shape;
         geom_str := 'POINT EMPTY';
       elsif(type_in = SDE.st_geom_util.multilinestring_type) then
         geom_type := SDE.st_geom_util.multilinestring_type;
         entity := sg_multi_simple_line_shape;
         geom_str := 'MULTILINESTRING EMPTY';
       elsif(type_in = SDE.st_geom_util.linestring_type) then       
         geom_type := SDE.st_geom_util.linestring_type;
         entity := sg_simple_line_shape;
         geom_str := 'LINESTRING EMPTY';
       elsif(type_in = SDE.st_geom_util.multipolygon_type) then       
         geom_type := SDE.st_geom_util.multipolygon_type;
         entity := sg_multi_area_shape;
         geom_str := 'MULTIPOLYGON EMPTY';
       elsif(type_in = SDE.st_geom_util.polygon_type) then  
         geom_type := SDE.st_geom_util.polygon_type;
         entity := sg_area_shape;
         geom_str := 'POLYGON EMPTY';
       elsif(type_in = SDE.st_geom_util.st_geometry_type) then
         raise_application_error (SDE.st_type_util.st_geom_text_invalid_empty,'Invalid ST_GEOMETRY EMPTY text. Must suppy valid geometry type.');
       else
         raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'EMPTY Geometry type is not a valid shape type.');
       end if;
     end if;
     is_empty := TRUE;
   else

     is_empty := FALSE;
     tot_len := length(geom_str);
     lt_param := instr(geom_str,'(');
     first_space := instr(geom_str,' ');

     if(lt_param < first_space) then
       dtype := substr(geom_str,1,lt_param-1);
     else
       dtype := substr(geom_str,1,first_space);
       dtype := rtrim(dtype);
     end if;
     dimension := substr(geom_str,first_space,(lt_param - first_space));
     dimension := ltrim(dimension);
     if(length(dimension) > 0) then
       if(instr(dimension,'MZ') > 0) then
         raise_application_error (SDE.st_type_util.st_geom_text_invalid_dimension,'Invalid Geometry text dimension. "'||dimension||'"');
       elsif(instr(dimension,'ZM') > 0) then
         is_zm := 3;
         geom_type := 3;
       elsif(instr(dimension,'Z') > 0) then
         is_z := 2;
         geom_type := 2;
       elsif(instr(dimension,'M') > 0) then
         is_m := 1;
         geom_type := 1;
       else
         raise_application_error (SDE.st_type_util.st_geom_text_invalid_dimension,'Invalid Geometry text dimension. "'||dimension||'"');
       end if;
     else
       geom_type := 0;
     end if;

     if(instr(dtype,'MULTIPOINT') > 0) then
       entity := sg_multi_point_shape;
       geom_type := geom_type + SDE.st_geom_util.multipoint_type;
     elsif(instr(dtype,'POINT') > 0) then
       entity := sg_point_shape;
       geom_type := geom_type + SDE.st_geom_util.point_type;
     elsif(instr(dtype,'MULTILINESTRING') > 0) then
       entity := sg_multi_simple_line_shape;
       geom_type := geom_type + SDE.st_geom_util.multilinestring_type;
     elsif(instr(dtype,'LINESTRING') > 0) then       
       entity := sg_simple_line_shape;
       geom_type := geom_type + SDE.st_geom_util.linestring_type;
     elsif(instr(dtype,'MULTIPOLYGON') > 0) then       
       entity := sg_multi_area_shape;
       geom_type := geom_type + SDE.st_geom_util.multipolygon_type;
     elsif(instr(dtype,'POLYGON') > 0) then  
       entity := sg_area_shape;
       geom_type := geom_type + SDE.st_geom_util.polygon_type;
     else
       raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type "'||dtype||'" is not a valid shape type.');
     end if;

     if(is_zm > 0) then
       dtype := dtype||' ZM ';
     elsif(is_z > 0) then
       dtype := dtype||' Z ';
     elsif(is_m > 0) then
       dtype := dtype||' M ';
     else
       dtype := dtype||' ';
     end if;
     geom_str := dtype||substr(geom_str,lt_param,(tot_len - (lt_param - 1)));
   end if; 
  
end;

  Procedure get_name      (entity        IN      number,
                           name          IN Out  varchar2)
/***********************************************************************
*
*n  {get_Name}  --  get geometry type name from entity
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this procedure defines the geometry type name from the
*  entity type.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     entity      <in>     ==  (number) entity 
*     name        <in out> ==  (varchar2)  name 
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
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

    If entity = sg_point_shape THEN
      name := 'POINT';
    Elsif entity = sg_simple_line_shape OR entity = sg_line_shape THEN
      name := 'LINESTRING';
    Elsif entity = sg_area_shape THEN
      name := 'POLYGON';
    Elsif entity = sg_multi_point_shape THEN
      name := 'MULTIPOINT';
    Elsif entity = sg_multi_simple_line_shape OR entity = sg_multi_line_shape THEN
      name := 'MULTILINESTRING';
    Elsif entity = sg_multi_area_shape THEN
      name := 'MULTIPOLYGON';

    End If;
	
  End get_name;
  
Function get_relation_operation (matrix  varchar2)
Return varchar2
IS
  buffer      varchar2(32);
  buf2        varchar2(9);
  pos         number;
  len         number;

  Begin
    
    if length(matrix) <> 9 then
      raise_application_error (SDE.st_type_util.st_relate_invalid_matrix,'Pattern matrix must be 3-by-3 matrix (interior,boundary,exterior) composed of 9 field descriptors.');
    end if;
    
    buffer := upper(matrix);
    buf2 := translate(buffer,'012TF*','******');
    if(buf2 != '*********') then
      raise_application_error (SDE.st_type_util.st_relate_invalid_matrix,'Pattern matrix must use acceptable DE-9IM pattern values (''T'' ''F'' ''*'' ''1'' ''2'' ''0'').');
    end if;
    
    pos := instr(buffer,'T*F**F***',1);      --Within
    If pos > 0 THEN
      return('ST_WITHIN');
    End If; 

    pos := instr(buffer,'T*****FF*',1);      --Contains
    If pos > 0 THEN
      return('ST_CONTAINS');
    End If; 

    pos := instr(buffer,'T*F**FFF*',1);      --Equality
    If pos > 0 THEN
      return('ST_EQUALS');
    End If;

    pos := instr(buffer,'T********',1);      --Intersects
    If pos > 0 THEN
      return('ST_INTERSECTS');
    End If;
      
    pos := instr(buffer,'*T*******',1);      --Intersects
    If pos > 0 THEN
      return('ST_INTERSECTS');
    End If;

    pos := instr(buffer,'***T*****',1);      --Intersects
    If pos > 0 THEN
      return('ST_INTERSECTS');
    End If;

    pos := instr(buffer,'****T****',1);      --Intersects
    If pos > 0 THEN
      return('ST_INTERSECTS');
    End If;

    pos := instr(buffer,'FF*FF****',1);      --Disjoint
    If pos > 0 THEN
      return('ST_DISJOINT');
    End If;

    pos := instr(buffer,'****T****',1);      --Touch
    If pos > 0 THEN
      return('ST_TOUCHES');
    End If;

    pos := instr(buffer,'F**T*****',1);      --Touch
    If pos > 0 THEN
      return('ST_TOUCHES');
    End If;

    pos := instr(buffer,'F***T****',1);      --Touch
    If pos > 0 THEN
      return('ST_TOUCHES');
    End If;

    pos := instr(buffer,'T*T***T**',1);      --Overlap
    If pos > 0 THEN
      return('ST_OVERLAPS');
    End If;

    pos := instr(buffer,'1*T***T**',1);      --Overlap
    If pos > 0 THEN
      return('ST_OVERLAPS');
    End If;

    pos := instr(buffer,'T*T******',1);      --Crosses
    If pos > 0 THEN
      return('ST_CROSSES');
    End If;

    pos := instr(buffer,'0********',1);      --Crosses
    If pos > 0 THEN
      return('ST_CROSSES');
    End If;

    return('UNDEFINED');

  End get_relation_operation;
  
 Procedure validate_geom_srid (owner         IN      varchar2,
                               table_name    IN      varchar2,
                               column_name   IN      varchar2,
                               srid          IN OUT  integer)
/***********************************************************************
*
*n  {validate_geom_srid}  --  get geometry SRID from an st_geometry column
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this procedure gets the srid from the st_geometry SRID value. 
*  The number of SRID's is also verified. An error is returned for
*  an st_geometry column with more than 1 srid value.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     owner        <in>     ==  (varcahr2) owner 
*     table        <in>     ==  (varchar2) table 
*     column       <in>     ==  (varchar2) column
*     srid         <in out> ==  (integer) srid
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          10/08/07           original coding.
*e
***********************************************************************/
IS
  stmt        varchar2(512);
  table2      varchar2(128);
  type        ref_curs IS REF CURSOR;
  curs1       ref_curs;
  
  Begin
     
    stmt := 'select a.'||column_name||'.srid,count(*) from '||owner||'.'||table_name||
            ' a WHERE a.'||column_name||'.srid IS NOT NULL group by a.'||column_name||'.srid';
            
    open curs1 for stmt;
    loop
      fetch curs1 bulk collect into SDE.st_geom_util.srid_tab,SDE.st_geom_util.count_tab limit 200;
      if SDE.st_geom_util.srid_tab.count > 1 then
        close curs1;
        table2 := owner||'.'||table_name||'.'||column_name;
        raise_application_error (SDE.st_type_util.st_geom_multiple_srids,'Table '||table2||' has multiple SRID''s defined to different geometries.');
      end if;
      exit when curs1%notfound;      
    end loop;
    
    srid := SDE.st_geom_util.srid_tab(1);
    close curs1;
    
  exception
    when no_data_found then
      null;
    
  End validate_geom_srid;
  
  Function convert_to_system   (planeValue    IN      number,
                                false_origin  IN      number,
                                units         IN      number)
/***********************************************************************
*
*n  {validate_geom_srid}  --  get geometry SRID from an st_geometry column
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this procedure gets the srid from the st_geometry SRID value. 
*  The number of SRID's is also verified. An error is returned for
*  an st_geometry column with more than 1 srid value.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     owner        <in>     ==  (varcahr2) owner 
*     table        <in>     ==  (varchar2) table 
*     column       <in>     ==  (varchar2) column
*     srid         <in out> ==  (integer) srid
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          10/08/07           original coding.
*e
***********************************************************************/
Return number 
IS
  systemValue  number; 
  
Begin

  systemValue := (planeValue - false_origin) * units + 0.5; 

  If (systemValue <= 0 Or systemValue > SDE.st_geom_util.sys_units_limit) Then
    return(SDE.st_geom_util.sg_coordref_out_of_bounds);
  End If;
    
  return(trunc(systemValue));

End convert_to_system;

Procedure  encode_var   (value_in  IN         number,
                         r_final   IN Out     raw)
/***********************************************************************
*
*n  {encode_var}  --  Encodes system value into variable byte array
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     This procedure takes as input an system unit value and returns
*  the encoded value. 
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     SysValue        <in>         ==  (number) system value 
*     byte_buf        <in/out>     ==  (raw) var len byte buffer 
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          10/08/07           original coding.
*e
***********************************************************************/
IS
   value            number;
   str_buf          varchar2(32);
   r_temp           raw(8);
   r2_temp          raw(2);
   rt_temp          raw(2);
   r8_temp          raw(8);
   r_cont           raw(2);
   r_low6           raw(8);
   r_low7           raw(8);
   sign_bit_mask    integer := 64;
   cont_bit_mask    integer := 128;
   LOW6_BITS_MASK   integer := 63;
   LOW7_BITS_MASK   integer := 127;

Begin

    -- Initialize bit masks.

  str_buf := lpad(trim(to_char(cont_bit_mask,'XX')),2,'0');
  r_cont := hextoraw(str_buf);

  str_buf := lpad(trim(to_char(0,'XX')),2,'0');
  r_final := hextoraw(str_buf);

  str_buf := lpad(trim(to_char(0,'XX')),2,'0');
  r2_temp := hextoraw(str_buf);

  str_buf := lpad(trim(to_char(LOW6_BITS_MASK,'XXXXXXXXXXXXXXXX')),16,'0');
  r_low6 := hextoraw(str_buf);

  str_buf := lpad(trim(to_char(LOW7_BITS_MASK,'XXXXXXXXXXXXXXXX')),16,'0');
  r_low7 := hextoraw(str_buf);
 
    -- If the input value is negative, set the sign bit and make the value
    -- positive.
 
  value := value_in;
   
  if (value < 0) Then   

    str_buf := lpad(trim(to_char(sign_bit_mask,'XX')),2,'0');
    r2_temp := hextoraw(str_buf);         
    value := -value;    

  End If;

    -- Set the low order 6 bits and shift the input value.
  
  if (value > 63) Then
    r2_temp := UTL_RAW.BIT_XOR (r_cont,r2_temp);
  End If;

  str_buf := lpad(trim(to_char(value,'XXXXXXXXXXXXXXXX')),16,'0');
  r8_temp := hextoraw(str_buf);

  if UTL_RAW.BIT_AND(r8_temp,r_low6) != '0000000000000000' Then

    r_temp   := UTL_RAW.BIT_AND(r8_temp,r_low6);
    str_buf := ltrim(rawtohex(r_temp),0);
    
    rt_temp := hextoraw(str_buf);
    r2_temp := UTL_RAW.BIT_XOR (r2_temp, rt_temp);

  End If;
  
  r_final := utl_raw.concat(r2_temp);
    
  value := trunc(value/power(2,6));
  
    -- Compress the remaining high-order bits.

  while (value > 0)
  Loop

    If value > 127 Then

      str_buf := lpad(trim(to_char(0,'XX')),2,'0');
      r2_temp := hextoraw(str_buf);

      str_buf := lpad(trim(to_char(0,'XX')),2,'0');
      r_temp := hextoraw(str_buf);

      str_buf := lpad(trim(to_char(0,'XXXXXXXXXXXXXXXX')),16,'0');
      r8_temp := hextoraw(str_buf);

      r2_temp := UTL_RAW.BIT_XOR (r_cont,r2_temp);

      str_buf := lpad(trim(to_char(value,'XXXXXXXXXXXXXXXX')),16,'0'); 
      r8_temp := hextoraw(str_buf);

    -- If the current value extends beyond the current byte, set the
    -- continuation bit, encode the low-order 7 bits. If it doesn't
    -- extend beyond the current byte, just store the value.
    
      if UTL_RAW.BIT_AND(r8_temp,r_low7) != '0000000000000000' Then
        r_temp   := UTL_RAW.BIT_AND(r8_temp,r_low7);
        str_buf := ltrim(rawtohex(r_temp),0);
 
        rt_temp := hextoraw(str_buf);
        r2_temp := UTL_RAW.BIT_XOR (rt_temp, r2_temp);

        r_final := utl_raw.concat(r_final,r2_temp);
        
      Else
        r_final := utl_raw.concat(r_final,r2_temp);
      End If;

    Else

      str_buf := ltrim(lpad(trim(to_char(value,'XXXXXXXXXXXXXXXX')),16,'0'),0); 
      r_temp := hextoraw(str_buf);

      r_final := utl_raw.concat(r_final,r_temp);

    End If;  

    value := trunc(value/power(2,7));
    
  End Loop;

End encode_var;

  
 Function getLibraryVersion (component in binary_integer) 
  return number 
  AS
  language C
  library  st_shapelib	  
  name "getVersion"
  WITH CONTEXT
  parameters (
    CONTEXT,
    component int,
    return indicator 
  );


 Function isWindows 
  return number is

   Begin
    /* Is the library for Windows or Unix?
         0 - Unix
         1 - Windows */

    return getLibraryVersion (3);
    
 End isWindows;



 Function getLibraryVersion 
   return varchar2 is
   
    major  integer;
    minor  integer;
    bugfix integer;
    
  Begin
    major  := getLibraryVersion (0);
    minor  := getLibraryVersion (1);
    bugfix := getLibraryVersion (2);

    return major || '.' || minor || '.' || bugfix;
    
  End getLibraryVersion;



  Function checkLibraryVersion 
    return varchar2 is
    
    major      integer;
    minor      integer;
    bugfix     integer;
    compatible integer := 1;
  
  Begin
    major  := getLibraryVersion (0);
    minor  := getLibraryVersion (1);
    bugfix := getLibraryVersion (2);

    if major < libMajor then
      compatible := 0;
    elsif minor < libMinor then
      compatible := 0;
    elsif bugfix < libBug then
      compatible := 0;
    end if;

    if compatible > 0 then
      return 'Current St_Geometry library version is Compatible';
    else
      return 'Incompatible. ' ||
             'Current St_Geometry library version is ' || getLibraryVersion() || 
             ', ' || libMajor || '.' || libMinor || '.' || libBug ||
             ' or higher required.';
    end if;
  End checkLibraryVersion;

  Function sdexml_to_text_f(xml blob)
 /***********************************************************************
*
*n  {sdexml_to_text_f}  --  returns CLOB/text representation of an
*                           SDE XML compressed binary BLOB. 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     This function takes as input an SDE XML BLOB and returns the 
*  uncompressed text version as a CLOB.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*a  parameters:
*     xml_blob        <in>     ==  (BLOB) SDE XML BLOB.
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*x  SDE exceptions:
*e
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*h  history:
*
*    kevin watt          4/01/10           original coding.
*e
***********************************************************************/
Return clob 
IS 
    
    text_clob     clob := empty_clob();
  Begin
  
    text_clob := ' ';
 
    If xml IS NOT NULL Then    
      SDE.st_geometry_shapelib_pkg.sdexml_to_text(xml,text_clob);
    End If;

    Return(text_clob);
  
  End sdexml_to_text_f;

End st_geom_util;

/


Prompt Grants on PACKAGE ST_GEOM_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_GEOM_UTIL TO PUBLIC WITH GRANT OPTION
/
