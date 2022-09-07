Prompt drop Function OGC_UNION;
DROP FUNCTION MDSYS.OGC_UNION
/

Prompt Function OGC_UNION;
--
-- OGC_UNION  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_Union(
  g1 ST_Geometry,
  g2 ST_Geometry)
    RETURN ST_Geometry IS
BEGIN
  RETURN g1.ST_Union(g2);
END OGC_Union;
/


Prompt Synonym OGC_UNION;
--
-- OGC_UNION  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM OGC_UNION FOR MDSYS.OGC_UNION
/


Prompt Grants on FUNCTION OGC_UNION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_UNION TO PUBLIC
/