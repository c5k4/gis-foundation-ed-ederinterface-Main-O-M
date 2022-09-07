Prompt drop Package SDO_OLS_LOCUTL;
DROP PACKAGE MDSYS.SDO_OLS_LOCUTL
/

Prompt Package SDO_OLS_LOCUTL;
--
-- SDO_OLS_LOCUTL  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_ols_locutl AUTHID current_user AS

  FUNCTION geocodeSingleAdr(
    addressInRequest  XMLTYPE,
    openLsVersion     VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION makeOpenLS10Request(
    request XMLTYPE)
      RETURN XMLTYPE;

END sdo_ols_locutl;
/


Prompt Grants on PACKAGE SDO_OLS_LOCUTL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_OLS_LOCUTL TO PUBLIC
/
