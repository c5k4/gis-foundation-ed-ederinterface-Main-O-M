Prompt drop Function OGC_CROSS;
DROP FUNCTION MDSYS.OGC_CROSS
/

Prompt Function OGC_CROSS;
--
-- OGC_CROSS  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_Cross(
  g1 ST_Geometry,
  g2 ST_Geometry)
    RETURN Integer DETERMINISTIC IS
BEGIN
  RETURN g1.ST_Cross(g2);
END OGC_Cross;
/


Prompt Synonym CROSS;
--
-- CROSS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM CROSS FOR MDSYS.OGC_CROSS
/


Prompt Grants on FUNCTION OGC_CROSS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_CROSS TO PUBLIC
/
