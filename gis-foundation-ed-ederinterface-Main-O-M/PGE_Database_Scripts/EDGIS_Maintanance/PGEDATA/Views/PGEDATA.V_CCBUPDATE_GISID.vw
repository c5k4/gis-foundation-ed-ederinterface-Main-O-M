Prompt drop View V_CCBUPDATE_GISID;
DROP VIEW PGEDATA.V_CCBUPDATE_GISID
/

/* Formatted on 6/27/2019 02:52:12 PM (QP5 v5.313) */
PROMPT View V_CCBUPDATE_GISID;
--
-- V_CCBUPDATE_GISID  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_CCBUPDATE_GISID
(
    OBJECTID,
    NEWBADGE,
    FMETRICOM,
    FREVENUE,
    FAR1,
    FAR2,
    FAR3,
    FAROTHER,
    MAINTNOTE,
    FAPPLIANCE1,
    FAPPLIANCE2,
    FAPPLIANCE3,
    FAPPLIANCE4,
    FAPPLIANCE5,
    METER,
    PAINTPOLE,
    TRANSACTION_,
    FAR4,
    FAR5,
    DIFFBADGE,
    DIFFFIX,
    DIFFADDR,
    DIFFMAP,
    DIFFRS,
    DIFFIT,
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
    BALLAST_CH_DT,
    LAMP_CH_DT,
    LINE_SHARE_SW,
    LITE_SIZE,
    LITE_TYPE,
    LUMN_CH_DT,
    PCELL_CH_DT,
    PCELL,
    POLE_CH_DT,
    POLE_PT_DT,
    STRT_CH_DT,
    LITESIZE_TYPE,
    LITETYPE_TYPE,
    HALFHRADJ_TYPE,
    MAIL_ADDR1,
    MAIL_ADDR2,
    MAIL_CITY,
    MAIL_STATE,
    MAIL_ZIP,
    HIST_GEMS_MAP_RATE,
    HIST_GEMS_BADGENUM,
    HIST_GEMS_AKA,
    USERID,
    HIST_GEMS_MAP_NUM,
    NEAREST_ST,
    CITYNAME,
    STREETNM,
    ALTNM,
    NEW_GRID_MAPNUM,
    GEMS_DISTR_MAPNUM,
    CCB_OVERWRITE_FLAG,
    LAST_MODIFY_DATE,
    SHAPE,
    ACCOUNT_NUMBER2,
    BADGE_NUMBER2,
    CHANGE_OF_PARTY_DATE2,
    DESCRIPTIVE_ADDRESS2,
    FIXTURE_CODE2,
    FIXTURE_MANUFACTURER2,
    INSTALL_DATE2,
    ITEM_TYPE_CODE2,
    MAP_NUMBER2,
    OFFICE2,
    OPERATING_SCHEDULE2,
    PERSON_NAME2,
    POLE_LENGTH2,
    POLE_TYPE2,
    POLE_USE2,
    SP_ID2,
    PREM_ID2,
    RATE_SCHEDULE2,
    RECEIVE_DATE2,
    REMOVAL_DATE2,
    RETIRE_DATE2,
    SA_ID2,
    SERVICE2,
    SP_ITEM_HIST2,
    STATUS2,
    STATUS_FLAG2,
    SUSPENSION2,
    TOT_CODE2,
    TOT_TERR_DESC2,
    UNIQUE_SP_ID2,
    BALLAST_CH_DT2,
    HALFHRADJ_TYPE2,
    LAMP_CH_DT2,
    LINE_SHARE_SW2,
    LITE_SIZE2,
    LITE_TYPE2,
    LITESIZE_TYPE2,
    LITETYPE_TYPE2,
    LUMN_CH_DT2,
    MAIL_ADDR12,
    MAIL_ADDR22,
    MAIL_CITY2,
    MAIL_STATE2,
    MAIL_ZIP2,
    PCELL2,
    PCELL_CH_DT2,
    POLE_CH_DT2,
    POLE_PT_DT2,
    STRT_CH_DT2
)
AS
    SELECT a."OBJECTID",
           a."NEWBADGE",
           a."FMETRICOM",
           a."FREVENUE",
           a."FAR1",
           a."FAR2",
           a."FAR3",
           a."FAROTHER",
           a."MAINTNOTE",
           a."FAPPLIANCE1",
           a."FAPPLIANCE2",
           a."FAPPLIANCE3",
           a."FAPPLIANCE4",
           a."FAPPLIANCE5",
           a."METER",
           a."PAINTPOLE",
           a."TRANSACTION_",
           a."FAR4",
           a."FAR5",
           a."DIFFBADGE",
           a."DIFFFIX",
           a."DIFFADDR",
           a."DIFFMAP",
           a."DIFFRS",
           a."DIFFIT",
           a."OFFICE",
           a."PERSON_NAME",
           a."ACCOUNT_NUMBER",
           a."BADGE_NUMBER",
           a."FIXTURE_CODE",
           a."STATUS",
           a."STATUS_FLAG",
           a."RECEIVE_DATE",
           a."RETIRE_DATE",
           a."INSTALL_DATE",
           a."REMOVAL_DATE",
           a."CHANGE_OF_PARTY_DATE",
           a."DESCRIPTIVE_ADDRESS",
           a."MAP_NUMBER",
           a."RATE_SCHEDULE",
           a."ITEM_TYPE_CODE",
           a."OPERATING_SCHEDULE",
           a."SERVICE",
           a."FIXTURE_MANUFACTURER",
           a."POLE_TYPE",
           a."POLE_LENGTH",
           a."SUSPENSION",
           a."POLE_USE",
           a."SP_ID",
           a."SA_ID",
           a."PREM_ID",
           a."TOT_CODE",
           a."TOT_TERR_DESC",
           a."INVENTORY_DATE",
           a."INVENTORIED_BY",
           a."SP_ITEM_HIST",
           a."UNIQUE_SP_ID",
           a."GIS_ID",
           a."BALLAST_CH_DT",
           a."LAMP_CH_DT",
           a."LINE_SHARE_SW",
           a."LITE_SIZE",
           a."LITE_TYPE",
           a."LUMN_CH_DT",
           a."PCELL_CH_DT",
           a."PCELL",
           a."POLE_CH_DT",
           a."POLE_PT_DT",
           a."STRT_CH_DT",
           a."LITESIZE_TYPE",
           a."LITETYPE_TYPE",
           a."HALFHRADJ_TYPE",
           a."MAIL_ADDR1",
           a."MAIL_ADDR2",
           a."MAIL_CITY",
           a."MAIL_STATE",
           a."MAIL_ZIP",
           a."HIST_GEMS_MAP_RATE",
           a."HIST_GEMS_BADGENUM",
           a."HIST_GEMS_AKA",
           a."USERID",
           a."HIST_GEMS_MAP_NUM",
           a."NEAREST_ST",
           a."CITYNAME",
           a."STREETNM",
           a."ALTNM",
           a."NEW_GRID_MAPNUM",
           a."GEMS_DISTR_MAPNUM",
           a."CCB_OVERWRITE_FLAG",
           a."LAST_MODIFY_DATE",
           a."SHAPE",
           b.account_number       AS account_number2,
           b.badge_number         AS badge_number2,
           b.change_of_party_date AS change_of_party_date2,
           b.descriptive_address  AS descriptive_address2,
           b.fixture_code         AS fixture_code2,
           b.fixture_manufacturer AS fixture_manufacturer2,
           b.install_date         AS install_date2,
           b.item_type_code       AS item_type_code2,
           b.map_number           AS map_number2,
           b.office               AS office2,
           b.operating_schedule   AS operating_schedule2,
           b.person_name          AS person_name2,
           b.pole_length          AS pole_length2,
           b.pole_type            AS pole_type2,
           b.pole_use             AS pole_use2,
           b.sp_id                AS sp_id2,
           b.prem_id              AS prem_id2,
           b.rate_schedule        AS rate_schedule2,
           b.receive_date         AS receive_date2,
           b.removal_date         AS removal_date2,
           b.retire_date          AS retire_date2,
           b.sa_id                AS sa_id2,
           b.service              AS service2,
           b.sp_item_hist         AS sp_item_hist2,
           b.status               AS status2,
           b.status_flag          AS status_flag2,
           b.suspension           AS suspension2,
           b.tot_code             AS tot_code2,
           b.tot_terr_desc        AS tot_terr_desc2,
           b.unique_sp_id         AS unique_sp_id2,
           b.ballast_ch_dt        AS ballast_ch_dt2,
           b.halfhradj_type       AS halfhradj_type2,
           b.lamp_ch_dt           AS lamp_ch_dt2,
           b.line_share_sw        AS line_share_sw2,
           b.lite_size            AS lite_size2,
           b.lite_type            AS lite_type2,
           b.litesize_type        AS litesize_type2,
           b.litetype_type        AS litetype_type2,
           b.lumn_ch_dt           AS lumn_ch_dt2,
           b.mail_addr1           AS mail_addr12,
           b.mail_addr2           AS mail_addr22,
           b.mail_city            AS mail_city2,
           b.mail_state           AS mail_state2,
           b.mail_zip             AS mail_zip2,
           b.pcell                AS pcell2,
           b.pcell_ch_dt          AS pcell_ch_dt2,
           b.pole_ch_dt           AS pole_ch_dt2,
           b.pole_pt_dt           AS pole_pt_dt2,
           b.strt_ch_dt           AS strt_ch_dt2
      FROM EDGIS.STREETLIGHT_INV a, PGEDATA.SLCDX_UNIQUEDATA_GISID b
     WHERE a.gis_id = b.gis_id
/


Prompt Grants on VIEW V_CCBUPDATE_GISID TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_CCBUPDATE_GISID TO A0SW
/

Prompt Grants on VIEW V_CCBUPDATE_GISID TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_CCBUPDATE_GISID TO GIS_I
/

Prompt Grants on VIEW V_CCBUPDATE_GISID TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_CCBUPDATE_GISID TO S7MA
/