Prompt drop Function OGC_LINESTRINGFROMTEXT;
DROP FUNCTION MDSYS.OGC_LINESTRINGFROMTEXT
/

Prompt Function OGC_LINESTRINGFROMTEXT;
--
-- OGC_LINESTRINGFROMTEXT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_LineStringFromText(
  wkt   IN VARCHAR2,
  srid  IN INTEGER DEFAULT NULL)
    RETURN ST_LineString IS
BEGIN
  RETURN TREAT(ST_GEOMETRY.FROM_WKT(wkt, srid) AS ST_LineString);
END OGC_LineStringFromText;
/


Prompt Synonym LINESTRINGFROMTEXT;
--
-- LINESTRINGFROMTEXT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM LINESTRINGFROMTEXT FOR MDSYS.OGC_LINESTRINGFROMTEXT
/


Prompt Grants on FUNCTION OGC_LINESTRINGFROMTEXT TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_LINESTRINGFROMTEXT TO PUBLIC
/
