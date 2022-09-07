Prompt drop Package SDO_OLS_ROUTE;
DROP PACKAGE MDSYS.SDO_OLS_ROUTE
/

Prompt Package SDO_OLS_ROUTE;
--
-- SDO_OLS_ROUTE  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_ols_route AUTHID current_user AS

  FUNCTION maneuverOracleToOpenLS(
    oracle          XMLTYPE,
    openls_version  VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION makeOpenLS10Request(
    request XMLTYPE)
      RETURN XMLTYPE;

END sdo_ols_route;
/


Prompt Grants on PACKAGE SDO_OLS_ROUTE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_OLS_ROUTE TO PUBLIC
/
