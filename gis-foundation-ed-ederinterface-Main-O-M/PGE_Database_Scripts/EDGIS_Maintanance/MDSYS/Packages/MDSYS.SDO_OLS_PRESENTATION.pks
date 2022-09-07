Prompt drop Package SDO_OLS_PRESENTATION;
DROP PACKAGE MDSYS.SDO_OLS_PRESENTATION
/

Prompt Package SDO_OLS_PRESENTATION;
--
-- SDO_OLS_PRESENTATION  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_ols_presentation AUTHID current_user AS

  FUNCTION makeOpenLS10Request(
    request XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION specify_theme_for_mapviewer(
    openls_layer    XMLTYPE,
    openls_version  VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION specify_poi_for_mapviewer(
    openls_poi      XMLTYPE,
    openls_version  VARCHAR2)
      RETURN XMLTYPE;

END sdo_ols_presentation;
/


Prompt Grants on PACKAGE SDO_OLS_PRESENTATION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_OLS_PRESENTATION TO PUBLIC
/
