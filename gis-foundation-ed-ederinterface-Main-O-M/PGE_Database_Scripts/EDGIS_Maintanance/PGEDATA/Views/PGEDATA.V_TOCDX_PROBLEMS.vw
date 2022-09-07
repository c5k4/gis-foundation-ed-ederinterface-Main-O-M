Prompt drop View V_TOCDX_PROBLEMS;
DROP VIEW PGEDATA.V_TOCDX_PROBLEMS
/

/* Formatted on 6/27/2019 02:52:10 PM (QP5 v5.313) */
PROMPT View V_TOCDX_PROBLEMS;
--
-- V_TOCDX_PROBLEMS  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_TOCDX_PROBLEMS
(
    OFFICE,
    ACCOUNT_NUMBER,
    NULL1,
    NULL2,
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
    PREM_ID,
    INVENTORY_DATE,
    INVENTORIED_BY,
    GIS_ID,
    NULL3
)
AS
      SELECT transaction || office                office,
             account_number,
             NULL                                 null1,
             NULL                                 null2,
             badge_number,
             fixture_code,
             status,
             TO_CHAR (install_date, 'yyyy-mm-dd') install_date,
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
             PREM_ID,
             TO_CHAR (INVENTORY_DATE, 'yyyy-mm-dd') INVENTORY_DATE,
             INVENTORIED_BY,
             GIS_ID,
             NULL                                 null3
        FROM fieldpts
       WHERE     transaction < '4'
             AND fixture_code <> '0'
             AND office IS NOT NULL
             AND (prem_id = ' ' OR account_number = ' ' OR rate_schedule = ' ')
    ORDER BY account_number, rate_schedule
/


Prompt Grants on VIEW V_TOCDX_PROBLEMS TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_TOCDX_PROBLEMS TO A0SW
/

Prompt Grants on VIEW V_TOCDX_PROBLEMS TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_TOCDX_PROBLEMS TO GIS_I
/

Prompt Grants on VIEW V_TOCDX_PROBLEMS TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_TOCDX_PROBLEMS TO S7MA
/
