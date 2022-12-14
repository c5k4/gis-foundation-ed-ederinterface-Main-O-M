Prompt drop Function OGC_NUMPOINTS;
DROP FUNCTION MDSYS.OGC_NUMPOINTS
/

Prompt Function OGC_NUMPOINTS;
--
-- OGC_NUMPOINTS  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_NumPoints(
  c ST_Curve)
    RETURN Integer IS
BEGIN
  RETURN c.ST_NumPoints();
END OGC_NumPoints;
/


Prompt Synonym NUMPOINTS;
--
-- NUMPOINTS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM NUMPOINTS FOR MDSYS.OGC_NUMPOINTS
/


Prompt Grants on FUNCTION OGC_NUMPOINTS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_NUMPOINTS TO PUBLIC
/
