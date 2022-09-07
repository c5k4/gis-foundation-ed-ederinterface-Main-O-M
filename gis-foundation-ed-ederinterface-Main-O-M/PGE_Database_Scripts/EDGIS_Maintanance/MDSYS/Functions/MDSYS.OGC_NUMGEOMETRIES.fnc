Prompt drop Function OGC_NUMGEOMETRIES;
DROP FUNCTION MDSYS.OGC_NUMGEOMETRIES
/

Prompt Function OGC_NUMGEOMETRIES;
--
-- OGC_NUMGEOMETRIES  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_NumGeometries(
  g ST_GeomCollection)
    RETURN Integer IS
BEGIN
  RETURN g.ST_Geometries().LAST;
END OGC_NumGeometries;
/


Prompt Synonym NUMGEOMETRIES;
--
-- NUMGEOMETRIES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM NUMGEOMETRIES FOR MDSYS.OGC_NUMGEOMETRIES
/


Prompt Grants on FUNCTION OGC_NUMGEOMETRIES TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_NUMGEOMETRIES TO PUBLIC
/
