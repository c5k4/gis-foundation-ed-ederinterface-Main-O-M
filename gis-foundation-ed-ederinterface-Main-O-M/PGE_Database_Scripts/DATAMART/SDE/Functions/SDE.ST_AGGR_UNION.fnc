Prompt drop Function ST_AGGR_UNION;
DROP FUNCTION SDE.ST_AGGR_UNION
/

Prompt Function ST_AGGR_UNION;
--
-- ST_AGGR_UNION  (Function) 
--
CREATE OR REPLACE FUNCTION SDE.st_aggr_union (input SDE.st_geometry) return SDE.st_geometry AGGREGATE USING stgeom_aggr_union;
 
/


Prompt Grants on FUNCTION ST_AGGR_UNION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_AGGR_UNION TO PUBLIC
/
