Prompt drop Function OGC_X;
DROP FUNCTION MDSYS.OGC_X
/

Prompt Function OGC_X;
--
-- OGC_X  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_X(
  p ST_Point)
    RETURN NUMBER IS
BEGIN
  RETURN p.ST_X();
END OGC_X;
/


Prompt Synonym OGC_X;
--
-- OGC_X  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM OGC_X FOR MDSYS.OGC_X
/


Prompt Grants on FUNCTION OGC_X TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_X TO PUBLIC
/