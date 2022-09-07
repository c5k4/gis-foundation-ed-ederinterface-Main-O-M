Prompt drop Function OGC_ISCLOSED;
DROP FUNCTION MDSYS.OGC_ISCLOSED
/

Prompt Function OGC_ISCLOSED;
--
-- OGC_ISCLOSED  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_IsClosed(
  g ST_Geometry)
    RETURN Integer DETERMINISTIC IS
BEGIN
  IF(UPPER(OGC_GeometryType(g)) IN ('LINESTRING', 'ST_LINESTRING', 'ST_CIRCULARSTRING', 'ST_COMPOUNDCURVE')) THEN
    RETURN TREAT(g AS ST_Curve).ST_IsClosed();
  END IF;
  IF(UPPER(OGC_GeometryType(g)) IN ('MULTILINESTRING', 'ST_MULTILINESTRING', 'ST_MULTICURVE')) THEN
    RETURN TREAT(g AS ST_MultiCurve).ST_IsClosed();
  END IF;
  RETURN NULL;
END OGC_IsClosed;
/


Prompt Synonym ISCLOSED;
--
-- ISCLOSED  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ISCLOSED FOR MDSYS.OGC_ISCLOSED
/


Prompt Grants on FUNCTION OGC_ISCLOSED TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_ISCLOSED TO PUBLIC
/
