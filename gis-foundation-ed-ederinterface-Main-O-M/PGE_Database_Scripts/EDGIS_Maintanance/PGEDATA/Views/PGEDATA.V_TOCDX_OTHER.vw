Prompt drop View V_TOCDX_OTHER;
DROP VIEW PGEDATA.V_TOCDX_OTHER
/

/* Formatted on 6/27/2019 02:52:10 PM (QP5 v5.313) */
PROMPT View V_TOCDX_OTHER;
--
-- V_TOCDX_OTHER  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_TOCDX_OTHER
(
    OFFICE,
    ACCOUNT_NUMBER,
    NULL1,
    NEWBADGE,
    BADGE_NUMBER,
    FIXTURE_CODE,
    STATUS,
    INSTALL_DATE,
    DESCRIPTIVE_ADDRESS,
    MAP_NUMBER,
    RATE_SCHEDULE,
    ITEM_TYPE_CODE,
    OPERATING_SCHEDULE,
    SERVICE,
    FIXTURE_MANUFACTURER,
    POLE_TYPE,
    POLE_LENGTH,
    SUSPENSION,
    POLE_USE,
    NULL2,
    PERSON_NAME,
    DIFFBADGE,
    DIFFFIX,
    DIFFADDR,
    DIFFMAP,
    DIFFRS,
    DIFFIT,
    SP_ID,
    INVENTORY_DATE,
    INVENTORIED_BY,
    GIS_ID,
    NULL3
)
AS
    SELECT transaction || office                  office,
           account_number,
           NULL                                   null1,
           newbadge,
           badge_number,
           fixture_code,
           status,
           TO_CHAR (install_date, 'yyyy-mm-dd')   install_date,
           descriptive_address,
           map_number,
           rate_schedule,
           item_type_code,
           operating_schedule,
           service,
           FIXTURE_MANUFACTURER,
           POLE_TYPE,
           POLE_LENGTH,
           SUSPENSION,
           POLE_USE,
           NULL                                   null2,
           PERSON_NAME,
           diffBadge,
           diffFix,
           diffAddr,
           DiffMap,
           DiffRs,
           diffIt,
           sp_id,
           TO_CHAR (INVENTORY_DATE, 'yyyy-mm-dd') INVENTORY_DATE,
           INVENTORIED_BY,
           GIS_ID,
           NULL                                   null3
      FROM fieldpts
     WHERE transaction > '3'
/


Prompt Grants on VIEW V_TOCDX_OTHER TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_TOCDX_OTHER TO A0SW
/

Prompt Grants on VIEW V_TOCDX_OTHER TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_TOCDX_OTHER TO GIS_I
/

Prompt Grants on VIEW V_TOCDX_OTHER TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_TOCDX_OTHER TO S7MA
/
