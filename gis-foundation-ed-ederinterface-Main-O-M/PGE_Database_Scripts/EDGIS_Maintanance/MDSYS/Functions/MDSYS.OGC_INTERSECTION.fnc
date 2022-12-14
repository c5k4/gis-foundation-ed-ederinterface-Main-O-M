Prompt drop Function OGC_INTERSECTION;
DROP FUNCTION MDSYS.OGC_INTERSECTION
/

Prompt Function OGC_INTERSECTION;
--
-- OGC_INTERSECTION  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_Intersection(
  g1 ST_Geometry,
  g2 ST_Geometry)
    RETURN ST_Geometry IS
BEGIN
  RETURN g1.ST_Intersection(g2);
END OGC_Intersection;
/


Prompt Synonym INTERSECTION;
--
-- INTERSECTION  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM INTERSECTION FOR MDSYS.OGC_INTERSECTION
/


Prompt Grants on FUNCTION OGC_INTERSECTION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_INTERSECTION TO PUBLIC
/
