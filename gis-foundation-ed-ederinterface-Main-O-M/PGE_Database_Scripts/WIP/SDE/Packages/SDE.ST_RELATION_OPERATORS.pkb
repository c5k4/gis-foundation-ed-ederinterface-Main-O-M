Prompt drop Package Body ST_RELATION_OPERATORS;
DROP PACKAGE BODY SDE.ST_RELATION_OPERATORS
/

Prompt Package Body ST_RELATION_OPERATORS;
--
-- ST_RELATION_OPERATORS  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_relation_operators
/***********************************************************************
*
*n  {st_Relation_Operators.sps}  --  st_Geometry relation operators.
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*p  purpose:
*     this pl/sql package specification defines relation operators 
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

Function s_get_primary_filter (matrix varchar2)
Return number 
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
      Return(SDE.st_relation_operators.inside_filter);
    End If; 

    pos := instr(buffer,'T*****FF*',1);      --Contains
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If; 

    pos := instr(buffer,'T*F**FFF*',1);      --Equality
    If pos > 0 THEN
      Return(SDE.st_relation_operators.equality_filter);
    End If;

    pos := instr(buffer,'T********',1);      --Intersects
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;
      
    pos := instr(buffer,'*T*******',1);      --Intersects
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'***T*****',1);      --Intersects
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'****T****',1);      --Intersects
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'FF*FF****',1);      --Disjoint
    If pos > 0 THEN
      Return(SDE.st_relation_operators.disjoint_filter);
    End If;

    pos := instr(buffer,'****T****',1);      --Touch
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'F**T*****',1);      --Touch
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'F***T****',1);      --Touch
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'T*T***T**',1);      --Overlap
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'1*T***T**',1);      --Overlap
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'T*T******',1);      --Crosses
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    pos := instr(buffer,'0********',1);      --Crosses
    If pos > 0 THEN
      Return(SDE.st_relation_operators.inside_filter);
    End If;

    Return(SDE.st_relation_operators.no_filter);

  End s_get_primary_filter;

--st_Contains 
Function st_contains_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;
						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

   If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_contains,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_contains,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;		  
						  
    Return has_relation;
	
  End st_contains_f;
  
/****************************************************************************************
*****************************************************************************************/  
Function st_within_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
   						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

   If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_within,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_within,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;
	
    Return has_relation;
	
  End st_within_f;

/****************************************************************************************
*****************************************************************************************/  
Function st_intersects_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin
  
    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
	
	If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;					
					  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

    If (shape1.minx > shape2.maxx OR
        shape1.miny > shape2.maxy OR
        shape1.maxx < shape2.minx OR
        shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;

	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_intersects,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_intersects,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;		  
						  
    Return has_relation;
	
  End st_intersects_f;

/****************************************************************************************
*****************************************************************************************/  
Function st_buffer_intersects_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry, distance number)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin
  
    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;

    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
					  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

    spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
    has_relation := distance;

    If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_buffer_intersects,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
     ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_buffer_intersects,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;		  
						  
    Return has_relation;
	
  End st_buffer_intersects_f;


/****************************************************************************************
*****************************************************************************************/  
Function st_overlaps_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

   If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_overlaps,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_overlaps,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;			  
						  
    Return has_relation;
	
  End st_overlaps_f;

 /****************************************************************************************
*****************************************************************************************/  
Function st_touches_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

   If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_touches,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_touches,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;			  
						  
    Return has_relation;
	
  End st_touches_f;
  
 /****************************************************************************************
*****************************************************************************************/   
Function st_crosses_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;

   If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) THEN
      If shape1.srid = shape2.srid THEN
        Return(0);
      End If;
    End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_crosses,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_crosses,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;
	
    Return has_relation;
	
  End st_crosses_f;

/****************************************************************************************
*****************************************************************************************/  
Function st_orderingequals_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(1) := (' ');
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    if shape1.numpts = 0 and shape1.len = 0 and
       shape2.numpts = 0 and shape2.len = 0 Then
      Return(1);
    End if;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
					  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;

     If (shape1.minx != shape2.minx OR
         shape1.miny != shape2.miny OR
         shape1.maxx != shape2.maxx OR
         shape1.maxy != shape2.maxy) THEN
      Return(0);
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_orderingequals,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_orderingequals,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;			  
						  
    Return has_relation;
	
  End st_orderingequals_f;
 
/****************************************************************************************
*****************************************************************************************/   
Function st_equals_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  has_relation   number;
  rc             number;
  str_val        varchar2(9) := 'T*F**FFF*';
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;
    
    if shape1.numpts = 0 and shape1.len = 0 and
       shape2.numpts = 0 and shape2.len = 0 Then
      Return(1);
    End if;
    
    If (shape1.numpts = 0) AND (shape1.len = 0 OR shape1.len is null) Then
      Return 0;
    End if;
	
	If (shape2.numpts = 0) AND (shape2.len = 0 OR shape2.len is null) Then
	  Return 0;
    End if;				
						  
    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;

     If (shape1.minx != shape2.minx OR
         shape1.miny != shape2.miny OR
         shape1.maxx != shape2.maxx OR
         shape1.maxy != shape2.maxy) THEN
      Return(0);
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_relate,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_relate,
                                              str_val,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,has_relation);
	
	End If;
	
    Return has_relation;
	
  End st_equals_f;
  
/****************************************************************************************
*****************************************************************************************/  
Function st_relate_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,matrix varchar2)
Return number 
IS
  spref1         SDE.spx_util.spatial_ref_record_t;
  spref2         SDE.spx_util.spatial_ref_record_t;
  value          number;
  rc             number;
  filter         number;
  
  Begin

    If shape1 IS NULL OR shape2 IS NULL THEN
      Return NULL;
    End If;

    If shape1.srid = NULL OR shape2.srid = NULL THEN
	  raise_application_error (SDE.st_type_util.st_no_srid,'Shape SRID cannot be NULL');
	End If;
	
    if shape1.entity = SDE.st_geom_util.sg_nil_shape or shape1.numpts = 0 or
       shape2.entity = SDE.st_geom_util.sg_nil_shape or shape2.numpts = 0 then
      return NULL;
    End if;
	
	filter := s_get_primary_filter(matrix);
	
	spref1.srid := shape1.srid;
    rc := SDE.st_spref_util.select_spref(spref1);
    If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref1.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
    End If;

    If(filter = SDE.st_relation_operators.inside_filter) THEN
      If (shape1.minx > shape2.maxx OR
           shape1.miny > shape2.maxy OR
           shape1.maxx < shape2.minx OR
           shape1.maxy < shape2.miny) THEN
        If shape1.srid = shape2.srid THEN
          Return(0);
        End If;
      End If;
    Elsif (filter = SDE.st_relation_operators.equality_filter) THEN
      If (shape1.minx != shape2.minx OR
           shape1.miny != shape2.miny OR
           shape1.maxx != shape2.maxx OR
           shape1.maxy != shape2.maxy) THEN
        Return(0);
      End If;
    Elsif (filter = SDE.st_relation_operators.disjoint_filter) THEN
      If (shape1.minx > shape2.maxx OR
         shape1.miny > shape2.maxy OR
         shape1.maxx < shape2.minx OR
         shape1.maxy < shape2.miny) AND 
         shape1.srid = shape2.srid THEN
        return(1);
      End If;
	  If shape1.srid = shape2.srid THEN
	    SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_relate,
                                                matrix,spref1.srid,spref1.x_offset,
	                                            spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											    spref1.m_offset,spref1.m_scale,spref1.Definition,
											    shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											    shape2.entity,shape2.points,value);
	  ELSE
        spref2.srid := shape2.srid;
        rc := SDE.st_spref_util.select_spref(spref2);
        If rc != SDE.st_type_user.se_success THEN
		    raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                             ' does not exist in ST_SPATIAL_REFERENCES table.');
        End If;
	  
	    SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_relate,
                                              matrix,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,value);
	
	  End If;
	
      Return value;     
    End If;
	
	If shape1.srid = shape2.srid THEN
	  SDE.st_geometry_shapelib_pkg.spatialrelations2 (SDE.st_relation_operators.st_relate,
                                              matrix,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,
											  shape1.numpts,shape1.entity,shape1.points,shape2.numpts,
											  shape2.entity,shape2.points,value);
	ELSE
      spref2.srid := shape2.srid;
      rc := SDE.st_spref_util.select_spref(spref2);
      If rc != SDE.st_type_user.se_success THEN
		  raise_application_error (SDE.st_type_util.st_no_srid,'SRID '||spref2.srid||
		                           ' does not exist in ST_SPATIAL_REFERENCES table.');
      End If;
	  
	  SDE.st_geometry_shapelib_pkg.spatialrelations1 (SDE.st_relation_operators.st_relate,
                                              matrix,spref1.srid,spref1.x_offset,
	                                          spref1.y_offset,spref1.xyunits,spref1.z_offset,spref1.z_scale,
											  spref1.m_offset,spref1.m_scale,spref1.Definition,shape1.numpts,
											  shape1.entity,shape1.points,spref2.srid,spref2.x_offset,
	                                          spref2.y_offset,spref2.xyunits,spref2.z_offset,spref2.z_scale,
											  spref2.m_offset,spref2.m_scale,spref2.Definition,shape2.numpts,
											  shape2.entity,shape2.points,value);
	
	End If;
	
    Return value;
	
  End st_relate_f;



/****************************************************************************************
*****************************************************************************************/ 

End st_relation_operators;

/


Prompt Grants on PACKAGE ST_RELATION_OPERATORS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_RELATION_OPERATORS TO PUBLIC WITH GRANT OPTION
/
