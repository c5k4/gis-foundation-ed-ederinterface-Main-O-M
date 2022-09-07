--------------------------------------------------------
--  DDL for Function ST_AGGR_CONVEXHULL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "SDE"."ST_AGGR_CONVEXHULL" (input SDE.st_geometry) return SDE.st_geometry AGGREGATE USING stgeom_aggr_convexhull;
