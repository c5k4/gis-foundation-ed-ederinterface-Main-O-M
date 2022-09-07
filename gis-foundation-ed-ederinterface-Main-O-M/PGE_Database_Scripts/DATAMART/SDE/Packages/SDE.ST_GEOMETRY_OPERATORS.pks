Prompt drop Package ST_GEOMETRY_OPERATORS;
DROP PACKAGE SDE.ST_GEOMETRY_OPERATORS
/

Prompt Package ST_GEOMETRY_OPERATORS;
--
-- ST_GEOMETRY_OPERATORS  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.st_geometry_operators Authid current_user
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

  c_package_release       Constant pls_integer := 1126;
  
  mtrue                   Constant pls_integer := 1;
  mfalse                  Constant pls_integer := 0;

   -- st_Geometry operator functions (isocomn.h st_Geometry_Operators enum) 
   
  geomfromshape                Constant pls_integer := 1;
  pointfromshape               Constant pls_integer := 2;
  linefromshape                Constant pls_integer := 3;
  polyfromshape                Constant pls_integer := 4;
  mpointfromshape              Constant pls_integer := 5;
  mlinefromshape               Constant pls_integer := 6;
  mpolyfromshape               Constant pls_integer := 7;
  st_geomfromwkb               Constant pls_integer := 8;  
  st_pointfromwkb              Constant pls_integer := 9;
  st_linefromwkb               Constant pls_integer := 10;
  st_polyfromwkb               Constant pls_integer := 11;
  st_mpointfromwkb             Constant pls_integer := 12;
  st_mlinefromwkb              Constant pls_integer := 13;
  st_mpolyfromwkb              Constant pls_integer := 14;
  st_boundary                  Constant pls_integer := 15;
  st_buffer                    Constant pls_integer := 16;
  st_centroid                  Constant pls_integer := 17;
  st_convexhull                Constant pls_integer := 18;
  st_startpoint                Constant pls_integer := 19;
  st_endpoint                  Constant pls_integer := 20;
  st_linestringn               Constant pls_integer := 21;
  st_pointonsurface            Constant pls_integer := 22;
  st_exteriorring              Constant pls_integer := 23;
  st_interiorringn             Constant pls_integer := 24;
  st_numinteriorring           Constant pls_integer := 25;
  st_numgeometries             Constant pls_integer := 26;
  st_geometryn                 Constant pls_integer := 27;
  st_difference                Constant pls_integer := 28;
  st_union                     Constant pls_integer := 29;
  st_symmetricdiff             Constant pls_integer := 30;
  st_pointn                    Constant pls_integer := 31;
  st_intersection              Constant pls_integer := 32;
  st_transform                 Constant pls_integer := 33;
  st_verify                    Constant pls_integer := 34;

  Function st_astext_f(prim SDE.st_geometry)
    Return clob deterministic;

  Function st_asbinary_f(prim SDE.st_geometry)
    Return blob deterministic;

  Function st_geomfromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_pointfromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_linefromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_polyfromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
   
  Function st_mpointfromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_mlinefromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_mpolyfromwkb_f(wkb_blob blob,srid number)
    Return SDE.st_geometry deterministic;
    
  Function st_boundary_f(prim SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_coorddim_f(prim SDE.st_geometry)
    Return number deterministic;
    
  Function st_envelope_f(prim SDE.st_geometry)
    Return SDE.st_polygon deterministic;

  Function st_geometrytype_f(prim SDE.st_geometry)
    Return varchar2 deterministic;

  Function st_is3d_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_ismeasured_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_isclosed_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_isempty_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_isring_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_issimple_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_area_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_areaunits_f(prim SDE.st_geometry,unit varchar2)
    Return number deterministic;

  Function st_buffer_f(prim SDE.st_geometry,distance number)
    Return SDE.st_geometry deterministic;

  Function st_centroid_f(prim SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_convexhull_f(prim SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_dimension_f(prim SDE.st_geometry)
    Return number deterministic;

  Function st_startpoint_f (prim SDE.st_geometry)
    Return SDE.st_point deterministic;

  Function st_endpoint_f (prim SDE.st_geometry)
    Return SDE.st_point deterministic;

  Function st_pointonsurface_f (prim SDE.st_geometry)
    Return SDE.st_point deterministic;

  Function st_exteriorring_f (prim SDE.st_geometry)
    Return SDE.st_linestring deterministic;

  Function st_interiorringn_f (prim SDE.st_geometry,ring_pos number)
    Return SDE.st_linestring deterministic;

  Function st_numinteriorring_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_numgeometries_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_geometryn_f (prim SDE.st_geometry,position number)
    Return SDE.st_geometry;

  Function st_difference_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_union_f (shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_symmetricdiff_f (shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_pointn_f (prim SDE.st_geometry,point_pos number)
    Return SDE.st_point deterministic;

  Function st_intersection_f (shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return SDE.st_geometry deterministic;

  Function st_transform_f(prim SDE.st_geometry,srid number)
    Return SDE.st_geometry deterministic;

  Function st_transform_geotranid_f(prim SDE.st_geometry,srid number,geotranid number)
    Return SDE.st_geometry deterministic;

  Function st_entity_f (prim SDE.st_geometry)
    Return number;

  Function st_numpoints_f (prim SDE.st_geometry)
    Return number;

  Function st_minx_f (prim SDE.st_geometry)
    Return number;

  Function st_maxx_f (prim SDE.st_geometry)
    Return number;

  Function st_miny_f (prim SDE.st_geometry)
    Return number;

  Function st_maxy_f (prim SDE.st_geometry)
    Return number;

  Function st_minz_f (prim SDE.st_geometry)
    Return number;

  Function st_maxz_f (prim SDE.st_geometry)
    Return number;

  Function st_minm_f (prim SDE.st_geometry)
    Return number;

  Function st_maxm_f (prim SDE.st_geometry)
    Return number;

  Function st_length_f (prim SDE.st_geometry)
    Return number;

  Function st_srid_f (prim SDE.st_geometry)
    Return number;

  Function st_x_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_y_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_z_f (prim SDE.st_geometry)
    Return number deterministic;

  Function st_m_f (prim SDE.st_geometry)
    Return number deterministic;
    
  Function st_distance_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
    
  Function st_distance_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,unit varchar2)
    Return number deterministic;
    
  Function st_disjoint_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
    
  Procedure transform_srch_shape         (shape             IN  SDE.st_geometry,
                                          shape_out         OUT SDE.st_geometry,
                                          from_srid         IN SDE.st_spref_util.srid_t,
                                          to_srid           IN SDE.st_spref_util.srid_t);

  Function st_verify_f (shape SDE.st_geometry)
    Return SDE.st_geometry deterministic;                                       

  Pragma Restrict_References (st_geometry_operators,wnds);

End st_geometry_operators;

/


Prompt Grants on PACKAGE ST_GEOMETRY_OPERATORS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_GEOMETRY_OPERATORS TO PUBLIC WITH GRANT OPTION
/
