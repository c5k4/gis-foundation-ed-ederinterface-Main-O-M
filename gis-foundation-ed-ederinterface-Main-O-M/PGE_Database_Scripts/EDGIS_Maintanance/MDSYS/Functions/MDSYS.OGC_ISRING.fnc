Prompt drop Function OGC_ISRING;
DROP FUNCTION MDSYS.OGC_ISRING
/

Prompt Function OGC_ISRING;
--
-- OGC_ISRING  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_IsRing(
  c ST_Curve)
    RETURN Integer DETERMINISTIC IS
BEGIN
  RETURN c.ST_IsRing();
END OGC_IsRing;
/


Prompt Synonym ISRING;
--
-- ISRING  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ISRING FOR MDSYS.OGC_ISRING
/


Prompt Grants on FUNCTION OGC_ISRING TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_ISRING TO PUBLIC
/