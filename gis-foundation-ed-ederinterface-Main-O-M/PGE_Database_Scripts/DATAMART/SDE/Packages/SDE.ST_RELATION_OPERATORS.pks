Prompt drop Package ST_RELATION_OPERATORS;
DROP PACKAGE SDE.ST_RELATION_OPERATORS
/

Prompt Package ST_RELATION_OPERATORS;
--
-- ST_RELATION_OPERATORS  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.st_relation_operators Authid current_user
/***********************************************************************
*
*n  {st_Relation_Operators.sps}  --  st_Geometry relation functions.
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

  c_package_release              Constant pls_integer := 1016;

  no_filter                      Constant pls_integer := 0;
  equality_filter                Constant pls_integer := 1;
  disjoint_filter                Constant pls_integer := 2;
  inside_filter                  Constant pls_integer := 3;

  -- st_Spatial_Relations must match isocomn.h st_Spatial_Operators enum. 
	
  st_intersects                  Constant pls_integer := SDE.st_geom_util.intersects_c;
  st_within                      Constant pls_integer := SDE.st_geom_util.within_c;
  st_overlaps                    Constant pls_integer := SDE.st_geom_util.overlaps_c;
  st_touches                     Constant pls_integer := SDE.st_geom_util.touches_c;
  st_contains                    Constant pls_integer := SDE.st_geom_util.contains_c;
  st_crosses                     Constant pls_integer := SDE.st_geom_util.crosses_c;
  st_orderingequals              Constant pls_integer := SDE.st_geom_util.orderingequals_c;
  st_equals                      Constant pls_integer := SDE.st_geom_util.equals_c;
  st_disjoint                    Constant pls_integer := SDE.st_geom_util.disjoint_c;
  st_distance                    Constant pls_integer := SDE.st_geom_util.distance_c;
  st_relate                      Constant pls_integer := SDE.st_geom_util.relate_c;
  st_buffer_intersects           Constant pls_integer := SDE.st_geom_util.buffer_intersects_c;
  
  Function st_contains_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
	
  Function st_within_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
	
  Function st_intersects_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;

  Function st_buffer_intersects_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,distance number)
    Return number deterministic;
	
  Function st_overlaps_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;

  Function st_touches_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
	
  Function st_crosses_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;

  Function st_orderingequals_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;
	
  Function st_equals_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry)
    Return number deterministic;

  Function st_relate_f(shape1 SDE.st_geometry,shape2 SDE.st_geometry,matrix varchar2)
    Return number deterministic;
	
   Pragma Restrict_References (st_relation_operators,wnds,wnps);

End st_relation_operators;

/


Prompt Grants on PACKAGE ST_RELATION_OPERATORS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_RELATION_OPERATORS TO PUBLIC WITH GRANT OPTION
/
