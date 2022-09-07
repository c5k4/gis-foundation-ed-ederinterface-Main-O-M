--------------------------------------------------------
--  DDL for Function ST_AGGR_INTERSECTION
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "SDE"."ST_AGGR_INTERSECTION" (input SDE.st_geometry) return SDE.st_geometry AGGREGATE USING stgeom_aggr_intersection;
