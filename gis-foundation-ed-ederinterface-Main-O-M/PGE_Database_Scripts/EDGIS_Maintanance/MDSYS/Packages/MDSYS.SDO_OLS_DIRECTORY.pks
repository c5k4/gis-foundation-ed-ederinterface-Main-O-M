Prompt drop Package SDO_OLS_DIRECTORY;
DROP PACKAGE MDSYS.SDO_OLS_DIRECTORY
/

Prompt Package SDO_OLS_DIRECTORY;
--
-- SDO_OLS_DIRECTORY  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_ols_directory AUTHID current_user AS

  FUNCTION isBusinessOfCategoryId(
    businessId      VARCHAR2,
    categoryId      VARCHAR2,
    categoryTypeId  VARCHAR2)
      RETURN NUMBER;

  FUNCTION isBusinessOfCategoryName(
    businessId      VARCHAR2,
    categoryName    VARCHAR2,
    categoryTypeId  VARCHAR2)
      RETURN NUMBER;

  FUNCTION makeOpenLS10Request(
    request XMLTYPE)
      RETURN XMLTYPE;

  FUNCTION getOpenLsForBusiness(
    BUSINESS_ID                   VARCHAR2,
    BUSINESS_NAME                 VARCHAR2,
    DESCRIPTION                   VARCHAR2,
    PHONE                         VARCHAR2,
    COUNTRY                       VARCHAR2,
    COUNTRY_SUBDIVISION           VARCHAR2,
    COUNTRY_SECONDARY_SUBDIVISION VARCHAR2,
    MUNICIPALITY                  VARCHAR2,
    MUNICIPALITY_SUBDIVISION      VARCHAR2,
    POSTAL_CODE                   VARCHAR2,
    POSTAL_CODE_EXT               VARCHAR2,
    STREET                        VARCHAR2,
    INTERSECTING_STREET           VARCHAR2,
    BUILDING                      VARCHAR2,
    x                             NUMBER,
    y                             NUMBER,
    openLsVersion                 VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION getOpenLsForCategory(
    CATEGORY_ID       VARCHAR2,
    CATEGORY_TYPE_ID  NUMBER,
    CATEGORY_NAME     VARCHAR2,
    PARENT_ID         VARCHAR2,
    openLsVersion     VARCHAR2)
      RETURN XMLTYPE;

  FUNCTION getOpenLsForCategories(
    BUSINESS_ID   VARCHAR2,
    openLsVersion VARCHAR2)
      RETURN XMLTYPE;

END sdo_ols_directory;
/


Prompt Grants on PACKAGE SDO_OLS_DIRECTORY TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_OLS_DIRECTORY TO PUBLIC
/
