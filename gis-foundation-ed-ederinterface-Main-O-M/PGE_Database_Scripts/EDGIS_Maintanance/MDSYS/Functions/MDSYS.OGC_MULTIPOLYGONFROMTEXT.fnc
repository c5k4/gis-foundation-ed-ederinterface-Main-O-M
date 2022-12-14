Prompt drop Function OGC_MULTIPOLYGONFROMTEXT;
DROP FUNCTION MDSYS.OGC_MULTIPOLYGONFROMTEXT
/

Prompt Function OGC_MULTIPOLYGONFROMTEXT;
--
-- OGC_MULTIPOLYGONFROMTEXT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_MultiPolygonFromText(
  wkt   IN VARCHAR2,
  srid  IN INTEGER DEFAULT NULL)
    RETURN ST_MULTIPOLYGON IS
BEGIN
  RETURN TREAT(ST_GEOMETRY.FROM_WKT(wkt, srid) AS ST_MULTIPOLYGON);
END OGC_MultiPolygonFromText;
/


Prompt Synonym MULTIPOLYGONFROMTEXT;
--
-- MULTIPOLYGONFROMTEXT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM MULTIPOLYGONFROMTEXT FOR MDSYS.OGC_MULTIPOLYGONFROMTEXT
/


Prompt Grants on FUNCTION OGC_MULTIPOLYGONFROMTEXT TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_MULTIPOLYGONFROMTEXT TO PUBLIC
/
