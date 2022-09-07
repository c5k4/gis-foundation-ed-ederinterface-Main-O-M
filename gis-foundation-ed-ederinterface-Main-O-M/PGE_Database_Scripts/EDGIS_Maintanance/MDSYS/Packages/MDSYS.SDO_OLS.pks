Prompt drop Package SDO_OLS;
DROP PACKAGE MDSYS.SDO_OLS
/

Prompt Package SDO_OLS;
--
-- SDO_OLS  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_ols AUTHID current_user AS

  FUNCTION generate_error(
    openls_version  VARCHAR2,
    message         VARCHAR2,
    requestId       VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION generate_error(
    openls_version VARCHAR2,
    requestId      VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION PostClob(
    url       IN VARCHAR2,
    payload   IN CLOB)
      RETURN CLOB;

  FUNCTION PostXml(
    url       IN VARCHAR2,
    payload   IN XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION getNS(
    nsShortcut  VARCHAR2,
    openLsVersion   VARCHAR2)
      RETURN VARCHAR2;

  FUNCTION getNS(
    openLsVersion   VARCHAR2)
      RETURN VARCHAR2;

  FUNCTION extractValue(
    xml   XMLTYPE,
    xPath VARCHAR2,
    ns VARCHAR2)
      RETURN VARCHAR2;

  FUNCTION extract(
    xml   XMLTYPE,
    xPath VARCHAR2,
    ns VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION getBasisXml(
    node_name       VARCHAR2,
    openls_version  VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION makeOpenLSClobRequest(
    request         CLOB)
      RETURN CLOB;

  FUNCTION makeOpenLSSoapRequest(
    request XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION makeOpenLSRequest(
    request XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION makeOpenLS10Request(
    request XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION determine_classification(
    openls                    XMLTYPE,
    classification_criterion  VARCHAR2,
    openls_version            VARCHAR2)
      RETURN VARCHAR2;

  PROCEDURE parse_coord_pair(
    coords  IN  VARCHAR2,
    x       OUT NUMBER,
    y       OUT NUMBER);

  PROCEDURE handle_openls_error(
    specific_error_message  VARCHAR2,
    specific_error_code     NUMBER,
    default_error_message   VARCHAR2);

END sdo_ols;
/


Prompt Synonym SDO_OLS;
--
-- SDO_OLS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_OLS FOR MDSYS.SDO_OLS
/


Prompt Grants on PACKAGE SDO_OLS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_OLS TO PUBLIC
/
