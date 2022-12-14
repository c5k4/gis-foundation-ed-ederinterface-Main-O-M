Prompt drop Function OGC_DISTANCE;
DROP FUNCTION MDSYS.OGC_DISTANCE
/

Prompt Function OGC_DISTANCE;
--
-- OGC_DISTANCE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_Distance(
  g1 ST_Geometry,
  g2 ST_Geometry)
    RETURN NUMBER DETERMINISTIC IS
BEGIN
  RETURN g1.ST_Distance(g2);
END OGC_Distance;
/


Prompt Synonym DISTANCE;
--
-- DISTANCE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM DISTANCE FOR MDSYS.OGC_DISTANCE
/


Prompt Grants on FUNCTION OGC_DISTANCE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_DISTANCE TO PUBLIC
/
