Prompt drop Function ST_AGGR_INTERSECTION;
DROP FUNCTION SDE.ST_AGGR_INTERSECTION
/

Prompt Function ST_AGGR_INTERSECTION;
--
-- ST_AGGR_INTERSECTION  (Function) 
--
CREATE OR REPLACE FUNCTION SDE.st_aggr_intersection (input SDE.st_geometry) return SDE.st_geometry AGGREGATE USING stgeom_aggr_intersection;
 
/


Prompt Grants on FUNCTION ST_AGGR_INTERSECTION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_AGGR_INTERSECTION TO PUBLIC
/
