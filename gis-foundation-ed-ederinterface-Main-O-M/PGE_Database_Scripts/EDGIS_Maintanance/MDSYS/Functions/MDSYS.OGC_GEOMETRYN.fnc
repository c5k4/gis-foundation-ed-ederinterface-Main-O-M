Prompt drop Function OGC_GEOMETRYN;
DROP FUNCTION MDSYS.OGC_GEOMETRYN
/

Prompt Function OGC_GEOMETRYN;
--
-- OGC_GEOMETRYN  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_GeometryN(
  g ST_GeomCollection,
  n Integer)
    RETURN ST_Geometry IS
  arr ST_GEOMETRY_ARRAY;
BEGIN
  arr := g.ST_Geometries();
  RETURN arr(n);
END OGC_GeometryN;
/


Prompt Synonym GEOMETRYN;
--
-- GEOMETRYN  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM GEOMETRYN FOR MDSYS.OGC_GEOMETRYN
/


Prompt Grants on FUNCTION OGC_GEOMETRYN TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_GEOMETRYN TO PUBLIC
/
