Prompt drop Function OGC_RELATE;
DROP FUNCTION MDSYS.OGC_RELATE
/

Prompt Function OGC_RELATE;
--
-- OGC_RELATE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_Relate(
  g1            ST_Geometry,
  g2            ST_Geometry,
  PatternMatrix VARCHAR2)
    RETURN Integer DETERMINISTIC IS
BEGIN
  RETURN g1.ST_Relate(g2, PatternMatrix);
END OGC_Relate;
/


Prompt Synonym RELATE;
--
-- RELATE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM RELATE FOR MDSYS.OGC_RELATE
/


Prompt Grants on FUNCTION OGC_RELATE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_RELATE TO PUBLIC
/
