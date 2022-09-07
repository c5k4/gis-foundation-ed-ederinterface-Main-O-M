--------------------------------------------------------
--  DDL for Function ST_AGGR_UNION
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "SDE"."ST_AGGR_UNION" (input SDE.st_geometry) return SDE.st_geometry AGGREGATE USING stgeom_aggr_union;
