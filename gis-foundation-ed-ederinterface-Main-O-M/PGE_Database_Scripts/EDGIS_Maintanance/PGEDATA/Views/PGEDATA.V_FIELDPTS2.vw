Prompt drop View V_FIELDPTS2;
DROP VIEW PGEDATA.V_FIELDPTS2
/

/* Formatted on 6/27/2019 02:52:12 PM (QP5 v5.313) */
PROMPT View V_FIELDPTS2;
--
-- V_FIELDPTS2  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_FIELDPTS2
(
    TRANSACTION,
    OFFICE,
    PERSON_NAME,
    ACCOUNT_NUMBER,
    BADGE_NUMBER,
    FIXTURE_CODE,
    STATUS,
    STATUS_FLAG,
    RECEIVE_DATE,
    RETIRE_DATE,
    INSTALL_DATE,
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
    INVENTORY_DATE,
    INVENTORIED_BY,
    SP_ITEM_HIST,
    UNIQUE_SP_ID,
    GIS_ID,
    NEWBADGE,
    FMETRICOM,
    FAPPLIANCE,
    FREVENUE,
    FAR1,
    FAR2,
    FAR3,
    FAROTHER,
    MAINTNOTE,
    METER,
    OBJECTID,
    DIFFBADGE,
    DIFFFIX,
    DIFFADDR,
    DIFFMAP,
    DIFFRS,
    DIFFIT
)
AS
    SELECT f.transaction,
           f.OFFICE,
           '"' || f.PERSON_NAME || '"'
               PERSON_NAME,
           f.ACCOUNT_NUMBER,
           f.BADGE_NUMBER,
           f.FIXTURE_CODE,
           f.STATUS,
           f.STATUS_FLAG,
           TO_CHAR (f.RECEIVE_DATE, 'yyyy-mm-dd')
               RECEIVE_DATE,
           TO_CHAR (f.RETIRE_DATE, 'yyyy-mm-dd')
               RETIRE_DATE,
           TO_CHAR (f.INSTALL_DATE, 'yyyy-mm-dd')
               INSTALL_DATE,
           TO_CHAR (f.REMOVAL_DATE, 'yyyy-mm-dd')
               REMOVAL_DATE,
           TO_CHAR (f.CHANGE_OF_PARTY_DATE, 'yyyy-mm-dd')
               CHANGE_OF_PARTY_DATE,
           '"' || f.DESCRIPTIVE_ADDRESS || '"'
               DESCRIPTIVE_ADDRESS,
           s.mail_addr1,
           s.mail_addr2,
           s.mail_city,
           s.mail_state,
           s.mail_zip,
           f.MAP_NUMBER,
           f.RATE_SCHEDULE,
           f.ITEM_TYPE_CODE,
           f.OPERATING_SCHEDULE,
           f.SERVICE,
           f.FIXTURE_MANUFACTURER,
           f.POLE_TYPE,
           f.POLE_LENGTH,
           f.SUSPENSION,
           f.POLE_USE,
           f.SP_ID,
           f.SA_ID,
           f.PREM_ID,
           f.TOT_CODE,
           f.TOT_TERR_DESC,
           TO_CHAR (f.INVENTORY_DATE, 'yyyy-mm-dd')
               INVENTORY_DATE,
           f.INVENTORIED_BY,
           f.SP_ITEM_HIST,
           f.UNIQUE_SP_ID,
           f.GIS_ID,
           f.NEWBADGE,
           f.FMETRICOM,
           f.FAPPLIANCE,
           f.FREVENUE,
           f.FAR1,
           f.FAR2,
           f.FAR3,
           f.FAROTHER,
           '"' || f.MAINTNOTE || '"'
               MAINTNOTE,
           f.METER,
           f.OBJECTID,
           f.DIFFBADGE,
           f.DIFFFIX,
           '"' || DIFFADDR || '"'
               DIFFADDR,
           f.DIFFMAP,
           f.DIFFRS,
           f.DIFFIT
      FROM PGEDATA.fieldpts f, PGEDATA.SLCDX_DATA s
     WHERE f.SP_ITEM_HIST = s.SP_ITEM_HIST(+)
/


Prompt Grants on VIEW V_FIELDPTS2 TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_FIELDPTS2 TO A0SW
/

Prompt Grants on VIEW V_FIELDPTS2 TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_FIELDPTS2 TO GIS_I
/

Prompt Grants on VIEW V_FIELDPTS2 TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_FIELDPTS2 TO S7MA
/
