Prompt drop View V_TOCDX_MC2;
DROP VIEW PGEDATA.V_TOCDX_MC2
/

/* Formatted on 6/27/2019 02:52:10 PM (QP5 v5.313) */
PROMPT View V_TOCDX_MC2;
--
-- V_TOCDX_MC2  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_TOCDX_MC2
(
    OFFICE,
    PERSON_NAME,
    ACCOUNT_NUMBER,
    BADGE_NUMBER,
    NEWBADGE,
    FIXTURE_CODE,
    STATUS,
    STATUS_FLAG,
    INSTALL_DATE,
    RECEIVE_DATE,
    RETIRE_DATE,
    REMOVAL_DATE,
    CHANGE_OF_PARTY_DATE,
    DESCRIPTIVE_ADDRESS,
    MAIL_ADDR1,
    MAIL_ADDR2,
    MAIL_CITY,
    MAIL_STATE,
    MAIL_ZIP,
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
    SP_ID,
    SA_ID,
    PREM_ID,
    TOT_CODE,
    TOT_TERR_DESC,
    SP_ITEM_HIST,
    INVENTORY_DATE,
    INVENTORIED_BY,
    DIFFBADGE,
    DIFFFIX,
    DIFFADDR,
    DIFFMAP,
    DIFFRS,
    DIFFIT,
    UNIQUE_SP_ID,
    GIS_ID,
    FMETRICOM
)
AS
      SELECT transaction || office   Office,
             '"' || PERSON_NAME || '"' PERSON_NAME,
             ACCOUNT_NUMBER,
             BADGE_NUMBER,
             NEWBADGE,
             FIXTURE_CODE,
             STATUS,
             STATUS_FLAG,
             install_date,
             RECEIVE_DATE,
             RETIRE_DATE,
             REMOVAL_DATE,
             CHANGE_OF_PARTY_DATE,
             DESCRIPTIVE_ADDRESS,
             mail_addr1,
             mail_addr2,
             mail_city,
             mail_state,
             mail_zip,
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
             SP_ID,
             SA_ID,
             PREM_ID,
             TOT_CODE,
             TOT_TERR_DESC,
             SP_ITEM_HIST,
             INVENTORY_DATE,
             INVENTORIED_BY,
             DIFFBADGE,
             DIFFFIX,
             DIFFADDR,
             DIFFMAP,
             DIFFRS,
             DIFFIT,
             UNIQUE_SP_ID,
             GIS_ID,
             FMETRICOM
        FROM PGEDATA.v_fieldpts2
    ORDER BY transaction, office, account_number
/


Prompt Grants on VIEW V_TOCDX_MC2 TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_TOCDX_MC2 TO A0SW
/

Prompt Grants on VIEW V_TOCDX_MC2 TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_TOCDX_MC2 TO GIS_I
/

Prompt Grants on VIEW V_TOCDX_MC2 TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_TOCDX_MC2 TO S7MA
/
