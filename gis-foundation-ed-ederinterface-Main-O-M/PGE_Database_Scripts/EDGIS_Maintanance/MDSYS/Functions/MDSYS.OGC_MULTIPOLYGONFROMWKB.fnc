Prompt drop Function OGC_MULTIPOLYGONFROMWKB;
DROP FUNCTION MDSYS.OGC_MULTIPOLYGONFROMWKB
/

Prompt Function OGC_MULTIPOLYGONFROMWKB;
--
-- OGC_MULTIPOLYGONFROMWKB  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_MultiPolygonFromWKB(
  wkb   IN BLOB,
  srid  IN INTEGER DEFAULT NULL)
    RETURN ST_MULTIPOLYGON IS
BEGIN
  RETURN TREAT(ST_GEOMETRY.FROM_WKB(wkb, srid) AS ST_MULTIPOLYGON);
END OGC_MultiPolygonFromWKB;
/


Prompt Synonym MULTIPOLYGONFROMWKB;
--
-- MULTIPOLYGONFROMWKB  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM MULTIPOLYGONFROMWKB FOR MDSYS.OGC_MULTIPOLYGONFROMWKB
/


Prompt Grants on FUNCTION OGC_MULTIPOLYGONFROMWKB TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_MULTIPOLYGONFROMWKB TO PUBLIC
/
