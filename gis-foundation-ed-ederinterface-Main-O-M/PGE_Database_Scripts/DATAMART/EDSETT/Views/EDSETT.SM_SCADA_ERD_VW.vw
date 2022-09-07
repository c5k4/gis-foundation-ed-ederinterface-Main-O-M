Prompt drop View SM_SCADA_ERD_VW;
DROP VIEW EDSETT.SM_SCADA_ERD_VW
/

/* Formatted on 7/2/2019 01:18:01 PM (QP5 v5.313) */
PROMPT View SM_SCADA_ERD_VW;
--
-- SM_SCADA_ERD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_SCADA_ERD_VW
(
    GLOBAL_ID,
    FEATURE_CLASS_NAME,
    CONTROL_TYPE,
    CONTROL_SERIAL_NUM,
    RADIO_MANF_CD,
    RADIO_MODEL_NUM,
    RADIO_SERIAL_NUM,
    SPECIAL_CONDITIONS,
    DATE_MODIFIED
)
AS
    SELECT "GLOBAL_ID",
           "FEATURE_CLASS_NAME",
           "CONTROL_TYPE",
           "CONTROL_SERIAL_NUM",
           "RADIO_MANF_CD",
           "RADIO_MODEL_NUM",
           "RADIO_SERIAL_NUM",
           "SPECIAL_CONDITIONS",
           "DATE_MODIFIED"
      FROM ((SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_CAPACITOR
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    PHA_PR_RELAY_TYPE AS CONTROL_TYPE,
                    PHA_PR_CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_CIRCUIT_BREAKER
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_INTERRUPTER
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            /*            UNION ALL
                       (SELECT GLOBAL_ID,
                               FEATURE_CLASS_NAME,
                               RELAY_TYPE AS CONTROL_TYPE,
                               CONTROL_SERIAL_NUM,
                               RADIO_MANF_CD,
                               RADIO_MODEL_NUM,
                               RADIO_SERIAL_NUM,
                               SPECIAL_CONDITIONS,
                               DATE_MODIFIED
                          FROM SM_NETWORK_PROTECTOR
                         WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                               AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                               AND CURRENT_FUTURE = 'C') */
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_TYPE AS CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_RECLOSER
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_TYPE AS CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_REGULATOR
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_TYPE AS CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_SECTIONALIZER
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')
            UNION ALL
            (SELECT GLOBAL_ID,
                    FEATURE_CLASS_NAME,
                    CONTROL_UNIT_TYPE AS CONTROL_TYPE,
                    CONTROL_SERIAL_NUM,
                    RADIO_MANF_CD,
                    RADIO_MODEL_NUM,
                    RADIO_SERIAL_NUM,
                    SPECIAL_CONDITIONS,
                    DATE_MODIFIED
               FROM SM_SWITCH
              WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
                    AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
                    AND CURRENT_FUTURE = 'C')) SCADA
/


Prompt Grants on VIEW SM_SCADA_ERD_VW TO EDGIS to EDGIS;
GRANT SELECT ON EDSETT.SM_SCADA_ERD_VW TO EDGIS
/

Prompt Grants on VIEW SM_SCADA_ERD_VW TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDSETT.SM_SCADA_ERD_VW TO GIS_I
/
