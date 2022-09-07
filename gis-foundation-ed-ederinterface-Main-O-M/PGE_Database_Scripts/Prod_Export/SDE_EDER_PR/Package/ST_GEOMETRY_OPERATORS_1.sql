--------------------------------------------------------
--  DDL for Package Body ST_GEOMETRY_OPERATORS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."ST_GEOMETRY_OPERATORS" 
/***********************************************************************
*
*n  {st_Geometry_Operators.sps}  --  st_Geometry operators.  
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this pl/sql package specification defines operators 
*    to support the st_Geometry type.
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
*    kevin watt          12/02/04               original coding.
*e
***********************************************************************/
IS

FUNCTION get_checksum(shape IN SDE.ST_GEOMETRY) RETURN VARCHAR2 
IS 
  checksum VARCHAR2(40);
BEGIN 
  checksum := rawtohex(DBMS_CRYPTO.Hash(shape.points, DBMS_CRYPTO.HASH_MD5));
  RETURN checksum;
END get_checksum;

--st_Astext 
Function st_astext_f(prim SDE.st_geometry)
Return clob 
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  text_clob     clob := empty_clob();
  rc            number; 
Begin
  If prim IS NULL THEN
    Return NULL;
  End If;

  If prim.numpts = 0 AND prim.len = 0 Then
    If prim.entity = 0 Then
      text_clob := 'EMPTY';
    elsif prim.entity = SDE.st_geom_util.multipointzm_type Then  
      text_clob := 'MULTIPOINT ZM EMPTY';  
    elsif prim.entity = SDE.st_geom_util.multipointz_type Then  
      text_clob := 'MULTIPOINT Z EMPTY';
    elsif prim.entity = SDE.st_geom_util.multipointm_type Then  
      text_clob := 'MULTIPOINT M EMPTY';    
    elsif prim.entity = SDE.st_geom_util.multipoint_type Then  
      text_clob := 'MULTIPOINT EMPTY';              
    elsif prim.entity = SDE.st_geom_util.pointzm_type Then  
      text_clob := 'POINT ZM EMPTY';  
    elsif prim.entity = SDE.st_geom_util.pointz_type Then  
      text_clob := 'POINT Z EMPTY';
    elsif prim.entity = SDE.st_geom_util.pointm_type Then  
      text_clob := 'POINT M EMPTY';
    elsif prim.entity = SDE.st_geom_util.point_type Then  
      text_clob := 'POINT EMPTY';          
    elsif prim.entity = SDE.st_geom_util.multilinestringzm_type Then  
      text_clob := 'MULTILINESTRING ZM EMPTY';  
    elsif prim.entity = SDE.st_geom_util.multilinestringz_type Then  
      text_clob := 'MULTILINESTRING Z EMPTY';  
    elsif prim.entity = SDE.st_geom_util.multilinestringm_type Then  
      text_clob := 'MULTILINESTRING M EMPTY';      
    elsif prim.entity = SDE.st_geom_util.multilinestring_type Then  
      text_clob := 'MULTILINESTRING EMPTY';          
    elsif prim.entity = SDE.st_geom_util.linestringzm_type Then  
      text_clob := 'LINESTRING ZM EMPTY';  
    elsif prim.entity = SDE.st_geom_util.linestringz_type Then  
      text_clob := 'LINESTRING Z EMPTY';  
    elsif prim.entity = SDE.st_geom_util.linestringm_type Then  
      text_clob := 'LINESTRING M EMPTY';      
    elsif prim.entity = SDE.st_geom_util.linestring_type Then  
      text_clob := 'LINESTRING EMPTY';                 
    elsif prim.entity = SDE.st_geom_util.multipolygonzm_type Then  
      text_clob := 'MULTIPOLYGON ZM EMPTY';    
    elsif prim.entity = SDE.st_geom_util.multipolygonz_type Then  
      text_clob := 'MULTIPOLYGON Z EMPTY';    
    elsif prim.entity = SDE.st_geom_util.multipolygonm_type Then  
      text_clob := 'MULTIPOLYGON M EMPTY';       
    elsif prim.entity = SDE.st_geom_util.multipolygon_type Then  
      text_clob := 'MULTIPOLYGON EMPTY';           
    elsif prim.entity = SDE.st_geom_util.polygonzm_type Then  
      text_clob := 'POLYGON ZM EMPTY';    
    elsif prim.entity = SDE.st_geom_util.polygonz_type Then  
      text_clob := 'POLYGON Z EMPTY';    
    elsif prim.entity = SDE.st_geom_util.polygonm_type Then  
      text_clob := 'POLYGON M EMPTY';        
    elsif prim.entity = SDE.st_geom_util.polygon_type Then  
      text_clob := 'POLYGON EMPTY';                    
    End If;
        
    return(text_clob);
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  text_clob := ' ';
 
  SDE.st_geometry_shapelib_pkg.astext(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
                                      spref.z_scale,spref.m_offset,spref.m_scale,spref.Definition,prim.numpts,
                                      prim.entity,prim.points,text_clob);

  Return(text_clob);

End st_astext_f;
 /****************************************************************************************
*****************************************************************************************/  
Function st_asbinary_f(prim SDE.st_geometry)
Return blob 
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  wkb_blob      blob;
  rc            number;

Begin
   
  dbms_lob.createtemporary(wkb_blob,TRUE,dbms_lob.session);
  If prim IS NULL THEN
    Return NULL;
  End If;
    
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.asbinary(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
                                        spref.z_scale,spref.m_offset,spref.m_scale,spref.Definition,prim.entity,
                                        prim.numpts,prim.points,wkb_blob);       
  Return(wkb_blob);

End st_asbinary_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_geomfromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_geomfromwkb_f(wkb_blob, 4326));
End st_geomfromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_geomfromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_geomfromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);
    
  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
           
  shape.srid := srid;          
      
  Return(shape);

End st_geomfromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_pointfromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_pointfromwkb_f(wkb_blob, 4326));
End st_pointfromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_pointfromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_pointfromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_pointfromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_linefromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_linefromwkb_f(wkb_blob, 4326));
End st_linefromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_linefromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;
  
  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_linefromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_linefromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_polyfromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_polyfromwkb_f(wkb_blob, 4326));
End st_polyfromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_polyfromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_polyfromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_polyfromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_mpointfromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_mpointfromwkb_f(wkb_blob, 4326));
End st_mpointfromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_mpointfromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_mpointfromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_mpointfromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_mlinefromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_mlinefromwkb_f(wkb_blob, 4326));
End st_mlinefromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_mlinefromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;

Begin
  
  If wkb_blob IS NULL Then
    return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_mlinefromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_mlinefromwkb_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_mpolyfromwkb_f(wkb_blob blob)
Return SDE.st_geometry
IS
Begin 
  Return (st_mpolyfromwkb_f(wkb_blob, 4326));
End st_mpolyfromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_mpolyfromwkb_f(wkb_blob blob,srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  len           integer;
Begin

  If wkb_blob IS NULL Then
    Return NULL;
  End If;
    
  len := dbms_lob.getlength(wkb_blob);
  If len = 0 Then
    return shape;
  End If;

  spref.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.points := empty_blob();
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geomfromshape(SDE.st_geometry_operators.st_mpolyfromwkb,wkb_blob,spref.srid,spref.x_offset,
                                             spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                             spref.m_offset,spref.m_scale,spref.Definition,shape.numpts,
                                             shape.entity,shape.minx,shape.miny,shape.maxx,shape.maxy,
                                             shape.minz,shape.maxz,shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;          
      
  Return(shape);

End st_mpolyfromwkb_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_boundary_f(prim SDE.st_geometry)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1          integer := 0;
  val2          integer := 0;
  val3          integer := 0;
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If prim.entity = SDE.st_geom_util.sg_point_shape OR  
     prim.entity = SDE.st_geom_util.sg_multi_point_shape Then
     shape.srid := prim.srid; 
     shape.numpts    := 0;
     shape.minx      := NULL;
     shape.maxx      := NULL;
     shape.miny      := NULL;
     shape.maxy      := NULL;          
     If prim.minz IS NOT NULL AND prim.minm IS NOT NULL Then
       shape.entity := SDE.st_geom_util.pointzm_type;
     ElsIf prim.minz IS NOT NULL Then
       shape.entity := SDE.st_geom_util.pointz_type;
     ElsIf prim.minm IS NOT NULL Then
       shape.entity := SDE.st_geom_util.pointm_type;
     Else 
       shape.entity := SDE.st_geom_util.point_type;
     End If;
     Return shape;
  End If;
  
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_boundary,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_boundary_f;
  
 /****************************************************************************************
*****************************************************************************************/  
Function st_coorddim_f(prim SDE.st_geometry)
Return number
IS
  typeval    number :=0;
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
     
  If prim.numpts = 0 AND prim.len = 0 Then
    If prim.entity = 0 or
       prim.entity = SDE.st_geom_util.multipoint_type or
       prim.entity = SDE.st_geom_util.point_type or
       prim.entity = SDE.st_geom_util.multilinestring_type or
       prim.entity = SDE.st_geom_util.linestring_type or
       prim.entity = SDE.st_geom_util.multipolygon_type or
       prim.entity = SDE.st_geom_util.polygon_type 
     Then
       typeval := 2;
    elsif prim.entity = SDE.st_geom_util.multipointzm_type or
          prim.entity = SDE.st_geom_util.pointzm_type or
          prim.entity = SDE.st_geom_util.multilinestringzm_type or
          prim.entity = SDE.st_geom_util.linestringzm_type or
          prim.entity = SDE.st_geom_util.multipolygonzm_type or
          prim.entity = SDE.st_geom_util.polygonzm_type 
     Then  
      typeval := 4;  
    elsif prim.entity = SDE.st_geom_util.multipointz_type or
          prim.entity = SDE.st_geom_util.multipointm_type or
          prim.entity = SDE.st_geom_util.pointz_type or
          prim.entity = SDE.st_geom_util.pointm_type or
          prim.entity = SDE.st_geom_util.multilinestringz_type or
          prim.entity = SDE.st_geom_util.multilinestringm_type or
          prim.entity = SDE.st_geom_util.linestringz_type or
          prim.entity = SDE.st_geom_util.linestringm_type or
          prim.entity = SDE.st_geom_util.multipolygonz_type or
          prim.entity = SDE.st_geom_util.multipolygonm_type or
          prim.entity = SDE.st_geom_util.polygonz_type or
          prim.entity = SDE.st_geom_util.polygonm_type
      Then  
       typeval := 3;
    End If;
        
    return(typeval);
  End If;

  If prim.numpts < 0 OR prim.entity = 0 THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'numpots <= 0');
  End If;
 
  If prim.points IS NULL THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'shape is null');
  End If;
 
  SDE.st_geometry_shapelib_pkg.coorddim(prim.points,typeval);
 
  Return(typeval);

End st_coorddim_f;
 /****************************************************************************************
*****************************************************************************************/  
Function st_dimension_f(prim SDE.st_geometry)
Return number
IS
  typeval    number :=0;
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
       
  If prim.numpts = 0 AND prim.len = 0 Then
    typeval := -1;
  Elsif prim.entity = SDE.st_geom_util.sg_point_shape OR 
        prim.entity = SDE.st_geom_util.sg_multi_point_shape Then
    typeval := 0;
  Elsif prim.entity = SDE.st_geom_util.sg_line_shape OR 
        prim.entity = SDE.st_geom_util.sg_multi_line_shape OR
        prim.entity = SDE.st_geom_util.sg_simple_line_shape OR 
        prim.entity = SDE.st_geom_util.sg_multi_simple_line_shape Then
    typeval := 1;
  Elsif prim.entity = SDE.st_geom_util.sg_area_shape OR 
        prim.entity = SDE.st_geom_util.sg_multi_area_shape Then
    typeval := 2;
  Else
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape type'||
                             'entity: '||prim.entity);
  End If;
 
  Return typeval;

End st_dimension_f;  
/****************************************************************************************
*****************************************************************************************/ 
Function st_envelope_f(prim SDE.st_geometry)
Return SDE.st_polygon
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  minz          number;
  maxz          number;
  minm          number;
  maxm          number;
  rc            number;
  shape         SDE.st_polygon := SDE.st_polygon(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := prim.entity;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.srid   := prim.srid;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If prim.numpts > 0 THEN
    SDE.st_geometry_shapelib_pkg.envelope(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.numpts,prim.entity,prim.points,
                                          shape.numpts,shape.entity,shape.minx,shape.miny,shape.maxx,
                                          shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                                          shape.area,shape.len,shape.points);
                                            
    If(shape.numpts IS NULL and shape.entity = 0) Then
      shape.numpts    := 0;
      shape.minx      := NULL;
      shape.maxx      := NULL;
      shape.miny      := NULL;
      shape.maxy      := NULL;
      shape.minz      := NULL;
      shape.maxz      := NULL;
      shape.minm      := NULL;
      shape.maxm      := NULL;
    End if;
         
    shape.srid := prim.srid;          

  End If;
 
  Return(shape);

End st_envelope_f;
 /****************************************************************************************
*****************************************************************************************/  
Function st_geometrytype_f(prim SDE.st_geometry)
Return varchar2
IS
  name     varchar2(128) := NULL;
Begin
  
  If prim IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 AND prim.len = 0 Then
    name := 'EMPTY';
  Else
    SDE.st_geom_util.get_name(prim.entity,name);
 
    If name IS NULL THEN
      raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'st_geometry type must be a valid geometry type.');
    End If;
 
    name := 'ST_'||name;
  End If;
 
  Return name;
 
End st_geometrytype_f;
 /****************************************************************************************
*****************************************************************************************/  
Function st_is3d_f(prim SDE.st_geometry)
Return number
IS
  typeval    number := 0;
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.entity = 0) THEN
    Return NULL;
  End If;
  
  If prim.numpts = 0 AND prim.len = 0 Then
    if prim.entity = SDE.st_geom_util.multipointzm_type OR   
       prim.entity = SDE.st_geom_util.multipointz_type OR 
       prim.entity = SDE.st_geom_util.pointzm_type OR  
       prim.entity = SDE.st_geom_util.pointz_type OR  
       prim.entity = SDE.st_geom_util.multilinestringzm_type OR  
       prim.entity = SDE.st_geom_util.multilinestringz_type OR  
       prim.entity = SDE.st_geom_util.linestringzm_type OR  
       prim.entity = SDE.st_geom_util.linestringz_type OR  
       prim.entity = SDE.st_geom_util.multipolygonzm_type OR  
       prim.entity = SDE.st_geom_util.multipolygonz_type OR  
       prim.entity = SDE.st_geom_util.polygonzm_type OR  
       prim.entity = SDE.st_geom_util.polygonz_type Then
      Return 1;  
    End If;      
  End If;

  If prim.numpts < 0 OR prim.entity = 0 THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'numpots <= 0');
  End If;
 
  If prim.points IS NULL THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'shape is null');
  End If;
 
 
  If prim.numpts > 0 AND prim.entity > 0 THEN
    SDE.st_geometry_shapelib_pkg.getdimtype(prim.points,typeval);
  Else
    Return 0;
  End If;
 
  If typeval = 2  OR typeval = 4 THEN
    Return 1 ;
  Else
    Return 0;
  End If;
  
End st_is3d_f;
/****************************************************************************************
*****************************************************************************************/  
Function st_ismeasured_f(prim SDE.st_geometry)
Return number
IS
  typeval    number := 0;
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.entity = 0) THEN
    Return NULL;
  End If;
 
  If prim.numpts = 0 AND prim.len = 0 Then
    if prim.entity = SDE.st_geom_util.multipointzm_type OR   
       prim.entity = SDE.st_geom_util.multipointm_type OR 
       prim.entity = SDE.st_geom_util.pointzm_type OR  
       prim.entity = SDE.st_geom_util.pointm_type OR  
       prim.entity = SDE.st_geom_util.multilinestringzm_type OR  
       prim.entity = SDE.st_geom_util.multilinestringm_type OR  
       prim.entity = SDE.st_geom_util.linestringzm_type OR  
       prim.entity = SDE.st_geom_util.linestringm_type OR  
       prim.entity = SDE.st_geom_util.multipolygonzm_type OR  
       prim.entity = SDE.st_geom_util.multipolygonm_type OR  
       prim.entity = SDE.st_geom_util.polygonzm_type OR  
       prim.entity = SDE.st_geom_util.polygonm_type Then
      Return 1;  
    End If;      
  End If;

  If prim.numpts < 0 OR prim.entity = 0 Then
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'numpots <= 0');
  End If;
 
  If prim.points IS NULL Then
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'shape is null');
  End If;
 
 
  If prim.numpts > 0 AND prim.entity > 0 Then
    SDE.st_geometry_shapelib_pkg.getdimtype(prim.points,typeval);
  Else
     Return 0;
  End If;
 
  If typeval = 3 OR typeval = 4 Then
    Return 1 ;
  Else
    Return 0;
  End If;
  
  End st_ismeasured_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_isclosed_f(prim SDE.st_geometry)
Return number
IS
  spref    SDE.spx_util.spatial_ref_record_t;
  value    number := 0;
  rc          number;
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 AND prim.len = 0 Then
    Return 0;
  End If;
  
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
  raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                           ' does not exist in st_spatial_references table.');
  End If;
 
  If prim.numpts < 0 OR prim.entity = 0 THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'numpots <= 0');
  End If;
 
  If prim.points IS NULL THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'shape is null');
  End If;
 
  If prim.entity != SDE.st_geom_util.sg_line_shape AND 
     prim.entity != SDE.st_geom_util.sg_multi_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape AND 
     prim.entity != SDE.st_geom_util.sg_multi_simple_line_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'shape type must be '||
                             'a linestring or multilinestring shape.');
  End If;
 
  If prim.numpts > 0 THEN 
    SDE.st_geometry_shapelib_pkg.isclosed(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.numpts,prim.entity,prim.points,
                                          mfalse,value);
  Else
    Return value;                 
  End If;
 
  Return value;
 
End st_isclosed_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_isempty_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL THEN
    Return NULL;
  End If;
 
  If prim.numpts > 0 or (prim.numpts = 0 and prim.len > 0) THEN
    Return 0;
  Else
    Return 1;
  End If;

End st_isempty_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_isring_f(prim SDE.st_geometry)
Return number
IS
  spref    SDE.spx_util.spatial_ref_record_t;
  value    number := 0;
  rc          number;
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 and prim.len = 0 Then
    Return 0;
  End If;
  
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
 
  If prim.numpts < 0 OR prim.entity = 0 THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'numpots <= 0.');
  End If;
 
  If prim.points IS NULL THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'invalid shape '||
                             'shape is null.');
  End If;
 
  If prim.entity != SDE.st_geom_util.sg_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_line_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_shape,'shape type must be '||
                             'a linestring shape.');
  End If;
 
  If prim.numpts > 0 THEN 
    SDE.st_geometry_shapelib_pkg.isclosed(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.numpts,prim.entity,prim.points,
                                          mtrue,value);
  Else
    Return value;                 
  End If;
 
  Return value;
 
End st_isring_f;  
/****************************************************************************************
*****************************************************************************************/ 
Function st_issimple_f(prim SDE.st_geometry)
Return number
IS
  spref    SDE.spx_util.spatial_ref_record_t;
  value    number := 0;
  rc       number;
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 and prim.len = 0 Then
    Return 1;
  End If;

  If prim.entity = SDE.st_geom_util.sg_point_shape OR 
    prim.entity = SDE.st_geom_util.sg_area_shape OR
    prim.entity = SDE.st_geom_util.sg_multi_area_shape OR
    prim.numpts < 2 THEN
    value := 1;
  Else
    spref.srid := prim.srid;
    rc := SDE.st_spref_util.select_spref(spref);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
    End If;
 
    SDE.st_geometry_shapelib_pkg.issimple(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.numpts,prim.entity,prim.points,value);
  End If;

  Return(value);

End st_issimple_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_area_f(prim SDE.st_geometry)
Return number
IS
Begin

  If prim IS NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If  prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  Return prim.area;

End st_area_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_areaunits_f(prim SDE.st_geometry,unit varchar2)
Return number
IS
Begin

  If prim IS NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If  prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  Return prim.area;

End st_areaunits_f;

/****************************************************************************************
*****************************************************************************************/
Function st_area_unit_f(prim SDE.st_geometry, unit_name varchar2)
Return number 
IS
  spref    SDE.spx_util.spatial_ref_record_t;
  value    number := 0;
  rc       number;
  
Begin
  
  If  prim IS NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If  prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  If prim.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If unit_name IS NULL THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Area failed: ' ||
                             'linear unit name is invalid.');
  End If;

  SDE.st_geometry_shapelib_pkg.getnumvalue (SDE.st_geometry_operators.st_area,
                                            spref.srid,spref.x_offset,
                                            spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                            spref.m_offset,spref.m_scale,spref.Definition,
                                            prim.numpts,prim.entity,prim.points,
                                            value,unit_name);

  Return value;
 
End st_area_unit_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_buffer_f(prim SDE.st_geometry,distance number)
Return SDE.st_geometry
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val2         integer := 0;
  val3         integer := 0;

Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  If distance = 0.0 THEN
    Return(prim);
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_buffer,distance,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;       
  Return(shape);

End st_buffer_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_buffer_unit_f(prim SDE.st_geometry,distance number,unit_name varchar2)
Return SDE.st_geometry
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val2         integer := 0;
  val3         integer := 0;

Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If unit_name IS NULL THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Buffer failed: ' ||
                             'linear unit name is invalid.');
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  If distance = 0.0 THEN
    Return(prim);
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_buffer,distance,val2,val3,unit_name,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;       
  Return(shape);

End st_buffer_unit_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_centroid_f(prim SDE.st_geometry)
Return SDE.st_geometry
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  shape := SDE.st_geometry(((prim.minx + prim.maxx)/2),((prim.miny + prim.maxy)/2),NULL,NULL,prim.srid);
                 
  Return(shape);

End st_centroid_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_convexhull_f(prim SDE.st_geometry)
Return SDE.st_geometry
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
 
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_convexhull,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
                 
  shape.srid := prim.srid;          
 
  Return(shape);

End st_convexhull_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_startpoint_f(prim SDE.st_geometry)
Return SDE.st_point
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_point := SDE.st_point(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_LINESTRING type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_startpoint,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_startpoint_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_endpoint_f(prim SDE.st_geometry)
Return SDE.st_point
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_point := SDE.st_point(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_LINESTRING type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_endpoint,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_endpoint_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_linestringn_f(prim SDE.st_geometry)
Return SDE.st_linestring
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_linestring := SDE.st_linestring(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_LINESTRING type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_linestringn,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_linestringn_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointonsurface_f(prim SDE.st_geometry)
Return SDE.st_point
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_point := SDE.st_point(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_Polygon type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_pointonsurface,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_pointonsurface_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_exteriorring_f(prim SDE.st_geometry)
Return SDE.st_linestring
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_linestring := SDE.st_linestring(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_Polygon type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_exteriorring,val1,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_exteriorring_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_interiorringn_f(prim SDE.st_geometry,ring_pos number)
Return SDE.st_linestring
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw      raw(1);
  rc           number;
  shape        SDE.st_linestring := SDE.st_linestring(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val1         integer := 0;
  val2         integer := 0;
  val3         integer := 0;
Begin
 
  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_Polygon type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_interiorringn,ring_pos,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If(shape.numpts IS NULL and shape.entity = 0) then
    return(NULL);
  Else 
    shape.srid := prim.srid;  
  End if;
               
  Return(shape);

End st_interiorringn_f;
/****************************************************************************************
*****************************************************************************************/ 

Function st_numinteriorring_f(prim SDE.st_geometry)
Return number
IS
  spref           SDE.spx_util.spatial_ref_record_t;
  value           number;
  rc              number;
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_POLYGON or ST_MULTIPOLYGON type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.getnumvalue (SDE.st_geometry_operators.st_numinteriorring,
                                            spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
                                            spref.z_scale,spref.m_offset,spref.m_scale,spref.Definition,
                                            prim.numpts,prim.entity,prim.points,value,NULL);
           
  Return(value);

End st_numinteriorring_f;
/****************************************************************************************
*****************************************************************************************/ 

Function st_numgeometries_f(prim SDE.st_geometry)
Return number
IS
  spref           SDE.spx_util.spatial_ref_record_t;
  value           number;
  rc              number;
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_multi_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_point_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_line_shape THEN
    Return 1;
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.getnumvalue (SDE.st_geometry_operators.st_numgeometries,
                                            spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
                                            spref.z_scale,spref.m_offset,spref.m_scale,spref.Definition,
                                            prim.numpts,prim.entity,prim.points,value,NULL);
           
  Return(value);

End st_numgeometries_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_geometryn_f(prim SDE.st_geometry,position number)
Return SDE.st_geometry
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw  raw(1);
  rc           number;
  shape        SDE.st_geometry := SDE.st_linestring(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  val2         integer := 0;
  val3         integer := 0;
Begin

  If prim IS NULL or position < 1 or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_multi_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_point_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_line_shape THEN
    If position = 1 THEN
      Return prim;
    Else
      Return NULL;
    End If;
  End If;
   
  -- Valid SRID is needed to do any more with this geometry

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
  
  -- Build an empty geometry
  
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_geometryn,position,val2,val3,NULL,
                                              spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                              spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                              shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  If shape.entity = SDE.st_geom_util.sg_nil_shape THEN
    Return(NULL);
  End If;

  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          
 
  Return(shape);

End st_geometryn_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_difference_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return SDE.st_geometry
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape          SDE.st_geometry := SDE.st_geometry(0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,empty_blob());
  temp           varchar2(1);
  tempraw        raw(1);
  rc             number;
  op_rc          number  := 0;
Begin
     
  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc :=SDE.st_spref_util.select_spref(spref1);

  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
    
  If shape1.numpts = 0 and shape1.len = 0 Then
    shape.entity := SDE.st_geom_util.point_type;
    shape.srid := shape1.srid;
    Return(shape);
  End If;

  If shape2.numpts = 0 and shape2.len = 0 Then
    If shape1.numpts = 0 Then
   shape.entity := SDE.st_geom_util.point_type;
      shape.srid := shape1.srid;
      Return(shape);
 Else
      Return(shape1);
 End If;
  End if;
  
 
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatial_operations2 (SDE.st_geometry_operators.st_difference,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                      shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
  Else
    spref2.srid := shape2.srid;
    rc := SDE.st_spref_util.select_spref(spref2);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
                               ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
   
    SDE.st_geometry_shapelib_pkg.spatial_operations1 (SDE.st_geometry_operators.st_difference,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
                                                      shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
                                                      spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
                                                      spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
 
  End If;

  if (op_rc = SDE.st_geom_util.st_incompatible_shapes) then
    Return NULL;
  End if;

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.entity    := SDE.st_geom_util.point_type;
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
     
  shape.srid := shape1.srid;          
       
  Return(shape);
 
End st_difference_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_union_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return SDE.st_geometry
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape          SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  temp           varchar2(1);
  tempraw        raw(1);
  rc             number;
  op_rc          number  := 0;
Begin
  
  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc :=SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If shape1.numpts = 0 AND shape2.numpts = 0 THEN
    shape.entity := SDE.st_geom_util.point_type;
    shape.srid := shape1.srid;
    Return(shape);
  End If;

  If (shape1.numpts > 0 AND shape2.numpts = 0) OR
     (shape1.numpts = 0 AND shape2.numpts > 0) THEN
    If shape1.numpts > 0 Then
   return (shape1);
 Else
   return (shape2);
 End If;
  End If;
 
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;

  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If shape1.srid = shape2.srid THEN
 
    SDE.st_geometry_shapelib_pkg.spatial_operations2 (SDE.st_geometry_operators.st_union,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                      shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
  Else
    spref2.srid := shape2.srid;
    rc := SDE.st_spref_util.select_spref(spref2);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
                               ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
   
    SDE.st_geometry_shapelib_pkg.spatial_operations1 (SDE.st_geometry_operators.st_union,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
                                                      shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
                                                      spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
                                                      spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
 
  End If;

  if (op_rc = SDE.st_geom_util.st_incompatible_shapes) then
    Return NULL;
  End if;

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.entity    := SDE.st_geom_util.point_type;
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
     
  shape.srid := shape1.srid;          
      
  Return(shape);
 
End st_union_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_symmetricdiff_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return SDE.st_geometry
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape          SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  temp           varchar2(1);
  tempraw        raw(1);
  rc             number;
  op_rc          number  := 0;
Begin
  
  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc :=SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If shape1.numpts = 0 AND shape2.numpts = 0 THEN
    shape.entity := SDE.st_geom_util.point_type;
    shape.srid := shape1.srid;
    Return(shape);
  End If;
 
  If (shape1.numpts = 0 AND shape2.numpts > 0) OR
     (shape1.numpts > 0 AND shape2.numpts = 0) THEN
    If shape1.numpts > 0 Then 
   Return (shape1);
 Else 
      Return (shape2);
 End If;
  End If;
  
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;

  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If shape1.srid = shape2.srid THEN
 
    SDE.st_geometry_shapelib_pkg.spatial_operations2 (SDE.st_geometry_operators.st_symmetricdiff,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                      shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
  Else
    spref2.srid := shape2.srid;
    rc := SDE.st_spref_util.select_spref(spref2);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
                               ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
   
    SDE.st_geometry_shapelib_pkg.spatial_operations1 (SDE.st_geometry_operators.st_symmetricdiff,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
                                                      shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
                                                      spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
                                                      spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
 
  End If;

  if (op_rc = SDE.st_geom_util.st_incompatible_shapes) then
    Return NULL;
  End if;

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.entity    := SDE.st_geom_util.point_type;
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
     
  shape.srid := shape1.srid;          
     
  Return(shape);
 
End st_symmetricdiff_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointn_f(prim SDE.st_geometry,point_pos number)
Return SDE.st_point
IS
  spref        SDE.spx_util.spatial_ref_record_t;
  temp         varchar2(1);
  tempraw  raw(1);
  rc           number;
  shape        SDE.st_point := SDE.st_point(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
Begin

  If prim IS NULL or (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_point_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'ST_Geometry type'||
                             ' must be an ST_LINESTRING or ST_MULTIPOINT type.');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.geom_operation(SDE.st_geometry_operators.st_pointn,
                                              point_pos,0,0,NULL,spref.srid,spref.x_offset,
                                              spref.y_offset,spref.xyunits,spref.z_offset,
                                              spref.z_scale,spref.m_offset,spref.m_scale,
                                              spref.Definition,prim.numpts,prim.entity,
                                              prim.points,shape.numpts,shape.entity,
                                              shape.minx,shape.miny,shape.maxx,shape.maxy,
                                              shape.minz,shape.maxz,shape.minm,shape.maxm,
                                              shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := prim.srid;          

  Return(shape);

End st_pointn_f;
/****************************************************************************************
*****************************************************************************************/ 

Function st_intersection_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return SDE.st_geometry
IS
  spref1      SDE.spx_util.spatial_ref_record_t;
  spref2      SDE.spx_util.spatial_ref_record_t;
  shape       SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  shape_out   SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  temp        varchar2(1);
  tempraw     raw(1);
  rc          number;
  op_rc       number := 0;
Begin
  
  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
   End If;
 
   spref1.srid := shape1.srid;
   rc :=SDE.st_spref_util.select_spref(spref1);
   If rc != SDE.st_type_user.se_success THEN
     raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                              ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

 If (shape1.numpts = 0 And shape1.len = 0) OR 
    (shape2.numpts = 0 And shape2.len = 0)THEN
    shape.entity := SDE.st_geom_util.point_type;
    shape.srid := shape1.srid;
    Return(shape);
  End if;
    
  spref2.srid := shape2.srid;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
    
  if shape2.srid != shape1.srid Then
    SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;
     
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm   := NULL;

  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  If shape1.srid = shape2.srid THEN
 
    SDE.st_geometry_shapelib_pkg.spatial_operations2 (SDE.st_geometry_operators.st_intersection,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                      shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                      shape2.entity,shape2.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
  Else
   
    SDE.st_geometry_shapelib_pkg.spatial_operations2 (SDE.st_geometry_operators.st_intersection,
                                                      spref1.srid,spref1.x_offset,
                                                      spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                      spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                      shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                      shape_out.entity,shape_out.points,shape.numpts,shape.entity,
                                                      shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                                      shape.minm,shape.maxm,shape.area,shape.len,shape.points,op_rc);
 
  End If;
 
  if (op_rc = SDE.st_geom_util.st_incompatible_shapes) then
    Return NULL;
  End if;

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.entity    := SDE.st_geom_util.point_type;
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
    
  shape.srid := shape1.srid;          
       
  Return(shape);
 
End st_intersection_f;

/****************************************************************************************
*****************************************************************************************/ 

  Function st_transform_f(prim SDE.st_geometry, srid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  spref2        SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  geotranid     number := 0;
Begin

  If prim IS NULL OR srid IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 and prim.len = 0 Then
    shape := prim;
    shape.srid := srid;
    return shape;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  spref2.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref2.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
    
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm  := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.transform(geotranid,0,0,0,0,0,0,
                                         spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                         spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                         spref.Definition,prim.numpts,prim.entity,prim.minx,
                                         prim.miny,prim.maxx,prim.maxy,prim.minz,prim.maxz,prim.minm,prim.maxm,
                                         prim.area,prim.len,prim.points,
                                         spref2.srid,spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                         spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                         spref2.Definition,shape.numpts,shape.entity,
                                         shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                         shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;
              
  Return(shape);

End st_transform_f;

/****************************************************************************************
*****************************************************************************************/ 

  Function st_transform_geotranid_f(prim SDE.st_geometry, srid number, geotranid number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  spref2        SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
Begin

  If prim IS NULL OR srid IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 and prim.len = 0 Then
    shape := prim;
    shape.srid := srid;
    return shape;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  spref2.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref2.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
    
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm  := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.transform(geotranid,0,0,0,0,0,0,
                                         spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                         spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                         spref.Definition,prim.numpts,prim.entity,prim.minx,
                                         prim.miny,prim.maxx,prim.maxy,prim.minz,prim.maxz,prim.minm,prim.maxm,
                                         prim.area,prim.len,prim.points,
                                         spref2.srid,spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                         spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                         spref2.Definition,shape.numpts,shape.entity,
                                         shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                         shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;
              
  Return(shape);

End st_transform_geotranid_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_transform_gcsextent_f(prim SDE.st_geometry, srid number, 
                                  minx number, miny number, maxx number, maxy number, 
          prime_meridian number, unit_factor number)
Return SDE.st_geometry
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  spref2        SDE.spx_util.spatial_ref_record_t;
  temp          varchar2(1);
  tempraw       raw(1);
  rc            number;
  shape         SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
Begin

  If prim IS NULL OR srid IS NULL THEN
    Return NULL;
  End If;
    
  If prim.numpts = 0 and prim.len = 0 Then
    return shape;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  spref2.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref2.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
    
  shape.numpts := 0;
  shape.entity := 0;
  shape.minx   := 0;
  shape.miny   := 0;
  shape.maxx   := 0;
  shape.maxy   := 0;
  shape.area   := 0;
  shape.len    := 0;
  shape.minz   := NULL;
  shape.maxz   := NULL;
  shape.minm   := NULL;
  shape.maxm  := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;

  SDE.st_geometry_shapelib_pkg.transform(0,minx,miny,maxx,maxy,prime_meridian,unit_factor,
                                         spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                         spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                         spref.Definition,prim.numpts,prim.entity,prim.minx,
                                         prim.miny,prim.maxx,prim.maxy,prim.minz,prim.maxz,prim.minm,prim.maxm,
                                         prim.area,prim.len,prim.points,
                                         spref2.srid,spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                         spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                         spref2.Definition,shape.numpts,shape.entity,
                                         shape.minx,shape.miny,shape.maxx,shape.maxy,shape.minz,shape.maxz,
                                         shape.minm,shape.maxm,shape.area,shape.len,shape.points);

  if(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
  End if;
               
  shape.srid := srid;
              
  Return(shape);

End st_transform_gcsextent_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_entity_f(prim SDE.st_geometry)
Return number
IS
Begin

  If prim IS NULL THEN
    Return NULL;
  End If;
 
  If prim.numpts = 0 and prim.len = 0 Then
    Return(0);
  else
    Return(prim.entity);
  end if;
  
End st_entity_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_numpoints_f(prim SDE.st_geometry)
Return number
IS
 spref         SDE.spx_util.spatial_ref_record_t;
 num_points    number;
 rc            number;
Begin
 
  If prim IS NULL THEN
    Return NULL;
  End If;
 
  If prim.numpts = 0 and prim.len = 0 Then
    Return 0;
  End if;
  
  If(prim.entity > SDE.st_geom_util.sg_shape_class_mask) Then
    spref.srid := prim.srid;
    rc := SDE.st_spref_util.select_spref(spref);
    If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
    End If;
      
    SDE.st_geometry_shapelib_pkg.getshape(prim.entity,prim.numpts,prim.points,spref.srid,spref.x_offset,
                                          spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                          spref.m_offset,spref.m_scale,spref.Definition,num_points);
           
    Return(num_points);
  Else
    Return(prim.numpts);
  End if;

End st_numpoints_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_minx_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.minx);

End st_minx_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_maxx_f(prim SDE.st_geometry)
Return number
IS 
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
   End If;

  Return(prim.maxx);

End st_maxx_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_miny_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
   End If;

  Return(prim.miny);

 End st_miny_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_maxy_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.maxy);

End st_maxy_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_minz_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.minz);

End st_minz_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_maxz_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.maxz);

End st_maxz_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_minm_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.minm);

End st_minm_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_maxm_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;

  Return(prim.maxm);

End st_maxm_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_length_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim is NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_simple_line_shape AND 
     prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  Return prim.len;

End st_length_f;

/****************************************************************************************
*****************************************************************************************/
Function st_length_unit_f(prim SDE.st_geometry, unit_name varchar2)
Return number 
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  value          number := 0;
  rc             number;
  
Begin
    
  If prim is NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_line_shape AND
     prim.entity != SDE.st_geom_util.sg_simple_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_line_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_simple_line_shape AND 
     prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  If prim.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If unit_name IS NULL THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Length failed: ' ||
                             'linear unit name is invalid.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.getnumvalue (SDE.st_geometry_operators.st_length,
                                            spref.srid,spref.x_offset,
                                            spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                            spref.m_offset,spref.m_scale,spref.Definition,
                                            prim.numpts,prim.entity,prim.points,
                                            value,unit_name);

  Return value;
 
End st_length_unit_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_perimeter_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL THEN
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;

  If prim.numpts = 0 and prim.len = 0 THEN
    Return NULL;
  End If;

  Return prim.len;

End st_perimeter_f;

/****************************************************************************************
*****************************************************************************************/
Function st_perimeter_unit_f(prim SDE.st_geometry, unit_name varchar2)
Return number 
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  value         number := 0;
  rc            number;

Begin
 
  If prim IS NULL Then
    Return NULL;
  End If;

  If prim.entity != SDE.st_geom_util.sg_area_shape AND
     prim.entity != SDE.st_geom_util.sg_multi_area_shape THEN
    Return NULL;
  End If;
  
  If prim.numpts = 0 and prim.len = 0 Then
    Return NULL;
  End If;

  If prim.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If unit_name IS NULL THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Perimeter failed: ' ||
                             'linear unit name is invalid.');
  End If;

  SDE.st_geometry_shapelib_pkg.getnumvalue (SDE.st_geometry_operators.st_perimeter,
                                            spref.srid,spref.x_offset,
                                            spref.y_offset,spref.xyunits,spref.z_offset,spref.z_scale,
                                            spref.m_offset,spref.m_scale,spref.Definition,
                                            prim.numpts,prim.entity,prim.points,
                                            value,unit_name);
 
  Return value;
 
End st_perimeter_unit_f;

/****************************************************************************************
*****************************************************************************************/ 
Function st_srid_f(prim SDE.st_geometry)
Return number
IS
Begin
 
  If prim IS NULL THEN
    Return NULL;
  End If;

  Return(prim.srid);

End st_srid_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_x_f(prim SDE.st_geometry)
Return number
IS
Begin

  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;
  
  If prim IS NOT NULL AND  
     prim.entity <> SDE.st_geom_util.sg_nil_shape AND
     prim.entity <> SDE.st_geom_util.sg_point_shape THEN
     raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type is not POINT.');
  End if;
 
  Return(prim.minx);

End st_x_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_y_f(prim SDE.st_geometry)
Return number
IS
Begin

  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) THEN
    Return NULL;
  End If;
  
 If prim IS NOT NULL AND 
    prim.entity <> SDE.st_geom_util.sg_nil_shape AND
    prim.entity <> SDE.st_geom_util.sg_point_shape THEN
     raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type is not POINT.');
  End if;

  Return(prim.miny);

End st_y_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_z_f(prim SDE.st_geometry)
Return number
IS
Begin
  
  If prim IS NOT NULL AND (prim.numpts = 0 and prim.len = 0) AND
     (prim.entity = SDE.st_geom_util.point_type OR
      prim.entity = SDE.st_geom_util.pointm_type OR
      prim.entity = SDE.st_geom_util.pointz_type OR
      prim.entity = SDE.st_geom_util.pointzm_type) THEN
    Return NULL;
  End if;
 
  If prim IS NOT NULL AND 
     prim.entity <> SDE.st_geom_util.sg_nil_shape AND
     prim.entity <> SDE.st_geom_util.sg_point_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type is not POINT.');
  End if;
 
  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) OR 
     prim.minz IS NULL THEN
    Return NULL;
  End If;
 
  Return(prim.minz);

End st_z_f;
/****************************************************************************************
*****************************************************************************************/ 
Function st_m_f(prim SDE.st_geometry)
Return number
IS
Begin

  If prim IS NOT NULL AND (prim.numpts = 0 and prim.len = 0) AND
     (prim.entity = SDE.st_geom_util.point_type OR
      prim.entity = SDE.st_geom_util.pointm_type OR
      prim.entity = SDE.st_geom_util.pointz_type OR
      prim.entity = SDE.st_geom_util.pointzm_type) THEN
    Return NULL;
  End if;

  if prim IS NOT NULL AND 
     prim.entity <> SDE.st_geom_util.sg_nil_shape AND
     prim.entity <> SDE.st_geom_util.sg_point_shape THEN
    raise_application_error (SDE.st_type_util.st_geometry_invalid_type,'Geometry type is not POINT.');
  End if;

  If prim IS NULL OR (prim.numpts = 0 and prim.len = 0) OR 
     prim.minm IS NULL THEN
    Return NULL;
  End If;
      
  Return(prim.minm);

End st_m_f;
/****************************************************************************************
*****************************************************************************************/  
Function st_distance_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape_out      SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  value          number;
  rc             number;
  str_val        varchar2(1) := (' ');

Begin
  
  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;
    
  If shape1.numpts = 0 and shape1.len = 0 or
     shape2.numpts = 0 and shape2.len = 0 Then
    Return NULL;
  End If;
     
  If shape1.srid = NULL OR shape2.srid = NULL THEN
   raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc := SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
 
  if shape2.srid != shape1.srid Then
      SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;
  
  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                    shape2.entity,shape2.points,shape2.minx,shape2.miny,shape2.maxx,shape2.maxy,
                                                    value,NULL,get_checksum(shape1));
  ELSE
   
 SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                    shape_out.entity,shape_out.points,shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,
                                                    value,NULL,get_checksum(shape1));
 
  End If;
 
  Return value;
 
End st_distance_f;

/****************************************************************************************
*****************************************************************************************/
Function st_distance_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,unit varchar2)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape_out   SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  value          number;
  rc             number;
  str_val        varchar2(1) := (' ');

Begin

  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;
    
  If shape1.numpts = 0 and shape1.len = 0 or
     shape2.numpts = 0 and shape2.len = 0 Then
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc := SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
 
  if shape2.srid != shape1.srid Then
    SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;

  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                    shape2.entity,shape2.points,shape2.minx,shape2.miny,shape2.maxx,shape2.maxy,
                                                    value,NULL,get_checksum(shape1));
  ELSE
   
   SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                    shape_out.entity,shape_out.points,shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,
                                                    value,NULL,get_checksum(shape1));
 
  End If;
 
  Return value;
 
End st_distance_f;

/****************************************************************************************
*****************************************************************************************/
Function st_distance_unit_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,unit_name varchar2)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape_out      SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  value          number;
  rc             number;
  str_val        varchar2(1) := (' ');

Begin

  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;
    
  If shape1.numpts = 0 and shape1.len = 0 or
     shape2.numpts = 0 and shape2.len = 0 Then
    Return NULL;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc := SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If unit_name IS NULL THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Distance failed: ' ||
                             'linear unit name is invalid.');
  End If;

  if shape2.srid != shape1.srid Then
    SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;


  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                    shape2.entity,shape2.points,shape2.minx,shape2.miny,shape2.maxx,shape2.maxy,
                                                    value,unit_name,get_checksum(shape1));
  ELSE
   
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                    shape_out.entity,shape_out.points,shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,
                                                    value,unit_name,get_checksum(shape1));
  End If;
 
  Return value;
 
End st_distance_unit_f;

/****************************************************************************************
*****************************************************************************************/
Function st_distance_within_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,distance number)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape_out      SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  value          number;
  rc             number;
  str_val        varchar2(1) := (' ');

Begin

  If shape1 IS NULL OR shape2 IS NULL THEN
    Return 0;
  End If;
    
  If shape1.numpts = 0 and shape1.len = 0 or
     shape2.numpts = 0 and shape2.len = 0 Then
    Return 0;
  End If;

  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;
 
  spref1.srid := shape1.srid;
  rc := SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  If distance < 0 THEN
    raise_application_error (SDE.st_type_util.st_invalid_unit_name,'ST_Distance failed: ' ||
                             'linear distance is invalid.');
  End If;

  value := distance;

  if shape2.srid != shape1.srid Then
    SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;


  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_within_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                    shape2.entity,shape2.points,shape2.minx,shape2.miny,shape2.maxx,shape2.maxy,
                                                    value,NULL,get_checksum(shape1));
  ELSE
   
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.distance_within_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                    shape_out.entity,shape_out.points,shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,
                                                    value,NULL,get_checksum(shape1));
  End If;
 
  Return value;
 
End st_distance_within_f;


/****************************************************************************************
*****************************************************************************************/

Function st_disjoint_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  shape_out      SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');

Begin

  If shape1 IS NULL OR shape2 IS NULL THEN
    Return NULL;
  End If;
    
  if(shape1.numpts = 0 and shape1.len = 0 and
     shape2.numpts = 0 and shape2.len = 0) Then
    return(0);
  End If;
    
  if shape1.numpts = 0 and shape1.len = 0 or
     shape2.numpts = 0 and shape2.len = 0 Then
    Return(1);
  End if;
        
  If shape1.srid = NULL OR shape2.srid = NULL THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
  End If;

  If (shape1.minx > shape2.maxx OR
       shape1.miny > shape2.maxy OR
       shape1.maxx < shape2.minx OR
       shape1.maxy < shape2.miny) AND 
       shape1.srid = shape2.srid THEN
    return(1);
  End If;
 
  spref1.srid := shape1.srid;
  rc := SDE.st_spref_util.select_spref(spref1);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
                             ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;

  if shape2.srid != shape1.srid Then
    SDE.st_geometry_operators.transform_srch_shape(shape2,shape_out,shape2.srid,shape1.srid);
  End IF;

  If shape1.srid = shape2.srid THEN
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.disjoint_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
                                                    shape2.entity,shape2.points,shape2.minx,shape2.miny,shape2.maxx,shape2.maxy,
                                                    has_relation,NULL,get_checksum(shape1));
  ELSE
    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_geom_util.disjoint_c,
                                                    str_val,spref1.srid,spref1.x_offset,
                                                    spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
                                                    spref1.m_offset,spref1.m_scale,spref1.Definition,
                                                    shape1.numpts,shape1.entity,shape1.points,shape_out.numpts,
                                                    shape_out.entity,shape_out.points,shape_out.minx,shape_out.miny,shape_out.maxx,shape_out.maxy,
                                                    has_relation,NULL,get_checksum(shape1));
 
  End If;     
  Return has_relation;
 
End st_disjoint_f;
  

 /****************************************************************************************
*****************************************************************************************/  

/***********************************************************************
  *
  *n  {transform_srch_shape}  -- transform shape using SRID 
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
  Procedure transform_srch_shape         (shape             IN  SDE.st_geometry,
                                          shape_out         OUT SDE.st_geometry,
                                          from_srid         IN SDE.st_spref_util.srid_t,
                                          to_srid           IN SDE.st_spref_util.srid_t)
  IS
  BEGIN
  
    shape_out := SDE.st_geometry(0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,empty_blob());
    shape_out.numpts := 0;
    shape_out.entity := 0;
    shape_out := st_transform_f(shape,to_srid);

    if shape_out.numpts = 0 and shape_out.entity = 0 then
      raise_application_error(SDE.st_type_util.spx_invalid_transformation,'Invalid shape transformation between search layer A.srid '||to_srid||' and input shape b.srid '||from_srid);
    end if;

  END transform_srch_shape;
 
/****************************************************************************************
*****************************************************************************************/
 
Function st_verify_f(shape SDE.st_geometry)
Return SDE.st_geometry
IS
  spref    SDE.spx_util.spatial_ref_record_t;
  vshape   SDE.st_geometry := SDE.st_geometry(0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,empty_blob());
  temp     varchar2(1);
  tempraw  raw(1);
  rc       number;
Begin                                    

  If shape IS NULL THEN
   Return NULL;
  End If;
    
  If shape.numpts = 0 and shape.len = 0 Then
    vshape.srid := shape.srid;
    Return(vshape);
  End If;

  vshape.numpts := 0;
  vshape.entity := 0;
  vshape.minx   := 0;
  vshape.miny   := 0;
  vshape.maxx   := 0;
  vshape.maxy   := 0;
  vshape.area   := 0;
  vshape.len    := 0;
  vshape.minz   := NULL;
  vshape.maxz   := NULL;
  vshape.minm   := NULL;
  vshape.maxm   := NULL;
 
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  vshape.points := tempraw;

  spref.srid := shape.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
     raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                             ' does not exist in st_spatial_references table.');
  End If;
 
  SDE.st_geometry_shapelib_pkg.verify(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                      spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                      spref.Definition,shape.numpts,shape.entity,shape.points,
                                      vshape.numpts,vshape.entity,vshape.minx,vshape.miny,
                                      vshape.maxx,vshape.maxy,vshape.minz,vshape.maxz,
                                      vshape.minm,vshape.maxm,vshape.area,vshape.len,vshape.points);         
  if(vshape.numpts IS NULL and vshape.entity = 0) then
    vshape.numpts    := 0;
    vshape.minx      := NULL;
    vshape.maxx      := NULL;
    vshape.miny      := NULL;
    vshape.maxy      := NULL;
    vshape.minz      := NULL;
    vshape.maxz      := NULL;
    vshape.minm      := NULL;
    vshape.maxm      := NULL;
  End if;
     
  vshape.srid := shape.srid;          
       
  Return(vshape);

End st_verify_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohash_f (prim SDE.st_geometry, precision number)
Return varchar2
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  spref2        SDE.spx_util.spatial_ref_record_t;
  rc            number;
  precision_in  number;
  geohash_str   varchar2(32);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  If precision IS NULL Then
    precision_in := 0;
  Else
    precision_in := precision; 
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  spref2.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref2.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.geohash(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                       spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                       spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                       spref2.srid,spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                       spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                       spref2.Definition,precision_in,geohash_str);
              
  Return(geohash_str);

End st_geohash_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohash_np_f (prim SDE.st_geometry)
Return varchar2
IS
  spref         SDE.spx_util.spatial_ref_record_t;
  spref2        SDE.spx_util.spatial_ref_record_t;
  rc            number;
  precision     number := NULL;
  geohash_str   varchar2(32);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  spref2.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref2);
  If rc != SDE.st_type_user.se_success THEN
    raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref2.srid||
                             ' does not exist in st_spatial_references table.');
  End If;

  SDE.st_geometry_shapelib_pkg.geohash(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                       spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                       spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                       spref2.srid,spref2.x_offset,spref2.y_offset,spref2.xyunits,
                                       spref2.z_offset,spref2.z_scale,spref2.m_offset,spref2.m_scale,
                                       spref2.Definition,precision,geohash_str);
              
  Return(geohash_str);

End st_geohash_np_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohash_f (geohash_str varchar2, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geohash_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohash(geohash_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                   spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                        spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                        shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
               shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
  
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_pointfromgeohash_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohash_np_f (geohash_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
Begin
 
  If geohash_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohash(geohash_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                   spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                        spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                        shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
               shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
  
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_pointfromgeohash_np_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohash_f (geohash_str varchar2, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geohash_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohash(geohash_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                  spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                       spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                       shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                 shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_geomfromgeohash_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohash_np_f (geohash_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
Begin
 
  If geohash_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohash(geohash_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                  spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                       spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                       shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                 shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_geomfromgeohash_np_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geosquare_f (prim SDE.st_geometry, precision number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  precision_in   number;
  geosquare_str  varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  If precision IS NULL Then
    precision_in := 0;
  Else
    precision_in := precision; 
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
 
  SDE.st_geometry_shapelib_pkg.geosquare(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                         spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                         spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                         precision_in,geosquare_str);
              
  Return(geosquare_str);

End st_geosquare_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geosquare_np_f (prim SDE.st_geometry)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  precision      number := NULL;
  geosquare_str  varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geosquare(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                         spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                         spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                         precision,geosquare_str);
              
  Return(geosquare_str);

End st_geosquare_np_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeosquare_f (geosquare_str varchar2, precision number, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                 shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
  
  shape.srid := srid;

  Return (shape);

End st_pointfromgeosquare_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeosquare_p_f (geosquare_str varchar2, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                 shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
  
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_pointfromgeosquare_p_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeosquare_np_f (geosquare_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                 shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
  
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_pointfromgeosquare_np_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeosquare_f (geosquare_str varchar2, precision number, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := srid;

  Return (shape);

End st_geomfromgeosquare_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeosquare_p_f (geosquare_str varchar2, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;

  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;

  Return (shape);

End st_geomfromgeosquare_p_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeosquare_np_f (geosquare_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
Begin
 
  If geosquare_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeosquare(geosquare_str,precision,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_geomfromgeosquare_np_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcode_f (prim SDE.st_geometry)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  precision      number := NULL;
  orientation    number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  append         number := 1;    
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                          precision,orientation,append,geohex_str);
              
  Return(geohex_str);

End st_geohexcode_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcode_p_f (prim SDE.st_geometry, precision number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  orientation    number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  append         number := 1;    
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                          precision,orientation,append,geohex_str);
              
  Return(geohex_str);

End st_geohexcode_p_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcode_po_f (prim SDE.st_geometry, precision number, orientation number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  append         number := 1;    
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                          precision,orientation,append,geohex_str);
              
  Return(geohex_str);

End st_geohexcode_po_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcode_poa_f (prim SDE.st_geometry, precision number, orientation number, append number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                          spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                          spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                          precision,orientation,append,geohex_str);
              
  Return(geohex_str);

End st_geohexcode_poa_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohextriangle_f (prim SDE.st_geometry)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  precision      number := NULL;
  orientation    number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohextrianglecode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                                  spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                                  spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                                  precision,orientation,geohex_str);
              
  Return(geohex_str);

End st_geohextriangle_f; 

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohextriangle_p_f (prim SDE.st_geometry, precision number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  orientation    number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohextrianglecode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                                  spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                                  spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                                  precision,orientation,geohex_str);
              
  Return(geohex_str);

End st_geohextriangle_p_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohextriangle_po_f (prim SDE.st_geometry, precision number, orientation number)
Return varchar2
IS
  spref          SDE.spx_util.spatial_ref_record_t;
  rc             number;
  geohex_str     varchar2(50);
Begin

  If prim IS NULL Then
    Return NULL;
  End If;
    
  If prim.numpts = 0 OR prim.points IS NULL Then
    return NULL;
  End If;

  spref.srid := prim.srid;
  rc := SDE.st_spref_util.select_spref(spref);
  If rc != SDE.st_type_user.se_success THEN
      raise_application_error (SDE.st_type_util.st_no_srid,'srid '||spref.srid||
                               ' does not exist in st_spatial_references table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohextrianglecode(spref.srid,spref.x_offset,spref.y_offset,spref.xyunits,
                                                  spref.z_offset,spref.z_scale,spref.m_offset,spref.m_scale,
                                                  spref.Definition,prim.minx,prim.miny,prim.maxx,prim.maxy,
                                                  precision,orientation,geohex_str);
              
  Return(geohex_str);

End st_geohextriangle_po_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohextri_f (geohex_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_geomfromgeohextri_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohextri_s_f (geohex_str varchar2, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_geomfromgeohextri_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohextri_sp_f (geohex_str varchar2, srid number, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_geomfromgeohextri_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geomfromgeohextri_spo_f (geohex_str varchar2, srid number, precision number, orientation number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeomfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                    spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                         spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                         shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                   shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_geomfromgeohextri_spo_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcodefromtcode_f (triangle_str varchar2)
Return varchar2 deterministic
IS
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  append          number := 1;    
  geohex_str      varchar2(50);
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
   
  SDE.st_geometry_shapelib_pkg.geohexcodefromtrianglecode(triangle_str,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,precision,orientation,append,geohex_str);
    
  Return (geohex_str);

End st_geohexcodefromtcode_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcodefromtcode_s_f (triangle_str varchar2, srid number)
Return varchar2 deterministic
IS
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  append          number := 1;    
  geohex_str      varchar2(50);
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcodefromtrianglecode(triangle_str,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,precision,orientation,append,geohex_str);
    
  Return (geohex_str);

End st_geohexcodefromtcode_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcodefromtcode_sp_f (triangle_str varchar2, srid number, precision number)
Return varchar2 deterministic
IS
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
  append          number := 1;    
  geohex_str      varchar2(50);
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcodefromtrianglecode(triangle_str,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,precision,orientation,append,geohex_str);
    
  Return (geohex_str);

End st_geohexcodefromtcode_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcodefromtcode_spo_f (triangle_str varchar2, srid number, precision number, orientation number)
Return varchar2 deterministic
IS
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  rc              number;
  append          number := 1;    
  geohex_str      varchar2(50);
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcodefromtrianglecode(triangle_str,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,precision,orientation,append,geohex_str);
    
  Return (geohex_str);

End st_geohexcodefromtcode_spo_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_geohexcodefromtcode_spoa_f (triangle_str varchar2, srid number, precision number, orientation number, append number)
Return varchar2 deterministic
IS
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  rc              number;
  geohex_str      varchar2(50);
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  SDE.st_geometry_shapelib_pkg.geohexcodefromtrianglecode(triangle_str,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,precision,orientation,append,geohex_str);
    
  Return (geohex_str);

End st_geohexcodefromtcode_spoa_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohextri_f (geohex_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                    shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_pointfromgeohextri_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohextri_s_f (geohex_str varchar2, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                    shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_pointfromgeohextri_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohextri_sp_f (geohex_str varchar2, srid number, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                    shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_pointfromgeohextri_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_pointfromgeohextri_spo_f (geohex_str varchar2, srid number, precision number, orientation number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If geohex_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextri(geohex_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                     spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                          spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                          shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                    shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_pointfromgeohextri_spo_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_ptfromgeohextriangle_f (triangle_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                               shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                         shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_ptfromgeohextriangle_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_ptfromgeohextriangle_s_f (triangle_str varchar2, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                               shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                         shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_ptfromgeohextriangle_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_ptfromgeohextriangle_sp_f (triangle_str varchar2, srid number, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                               shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                         shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_ptfromgeohextriangle_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_ptfromgeohextriangle_spo_f (triangle_str varchar2, srid number, precision number, orientation number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getpointfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                               shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                         shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_ptfromgeohextriangle_spo_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromgeohextriangle_f (triangle_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                             spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                  spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                  shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                            shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_gfromgeohextriangle_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromgeohextriangle_s_f (triangle_str varchar2, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                          spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                               spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                               shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                         shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_gfromgeohextriangle_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromgeohextriangle_sp_f (triangle_str varchar2, srid number, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                             spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                  spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                  shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                            shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_gfromgeohextriangle_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromgeohextriangle_spo_f (triangle_str varchar2, srid number, precision number, orientation number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromgeohextriangle(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                             spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                  spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                  shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                            shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := spref_r.srid;
   
  Return (shape);

End st_gfromgeohextriangle_spo_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromtrianglecode_f (triangle_str varchar2)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromtrianglecode(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                           spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                   spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                          shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := SDE.st_geom_util.ST_SRID_EPSG_WGS84;
   
  Return (shape);

End st_gfromtrianglecode_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromtrianglecode_s_f (triangle_str varchar2, srid number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  precision       number := NULL;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromtrianglecode(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                           spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                   spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                          shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := srid;
   
  Return (shape);

End st_gfromtrianglecode_s_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromtrianglecode_sp_f (triangle_str varchar2, srid number, precision number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
  orientation     number := SDE.st_geom_util.SG_GeoHex_Horizontal;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromtrianglecode(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                           spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                   spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                          shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := srid;
   
  Return (shape);

End st_gfromtrianglecode_sp_f;

/****************************************************************************************
*****************************************************************************************/ 

Function st_gfromtrianglecode_spo_f (triangle_str varchar2, srid number, precision number, orientation number)
Return SDE.st_geometry deterministic
IS
  temp            varchar2(1);
  tempraw         raw(1);
  entity          number;
  spref_r         SDE.st_spref_util.spatial_ref_record_t;
  shape           SDE.st_geometry := SDE.st_geometry(0,0,0,0,0,0,0,0,0,0,0,0,0,empty_blob());
  rc              number;
Begin
 
  If triangle_str is NULL THEN
    Return NULL;
  End if;
 
  spref_r.srid := srid;
  rc := SDE.st_spref_util.select_spref(spref_r);
  If rc != SDE.st_type_user.se_success THEN
 raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref_r.srid||
                          ' does not exist in ST_SPATIAL_REFERENCES table.');
  End If;
  
  temp := lpad('a', 1, 'a');
  tempraw := utl_raw.cast_to_raw (temp);
  shape.points := tempraw;
   
  SDE.st_geometry_shapelib_pkg.getgeometryfromtrianglecode(triangle_str,precision,orientation,spref_r.srid,spref_r.x_offset,spref_r.y_offset,spref_r.xyunits,
                                                           spref_r.z_offset,spref_r.z_scale,spref_r.m_offset,spref_r.m_scale,
                                   spref_r.Definition,shape.numpts,shape.entity,shape.minx,shape.miny,
                                shape.maxx,shape.maxy,shape.minz,shape.maxz,shape.minm,shape.maxm,
                          shape.area,shape.len,shape.points);
  
  If(shape.numpts IS NULL and shape.entity = 0) then
    shape.numpts    := 0;
    shape.minx      := NULL;
    shape.maxx      := NULL;
    shape.miny      := NULL;
    shape.maxy      := NULL;
    shape.minz      := NULL;
    shape.maxz      := NULL;
    shape.minm      := NULL;
    shape.maxm      := NULL;
 shape.srid      := spref_r.srid;            
  end if;
   
  shape.srid := srid;
   
  Return (shape);

End st_gfromtrianglecode_spo_f;

End st_geometry_operators;
