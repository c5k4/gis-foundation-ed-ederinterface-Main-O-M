Prompt drop View PGE_ALL_LINE_EQUIP_SETTINGS;
DROP VIEW EDSETT.PGE_ALL_LINE_EQUIP_SETTINGS
/

/* Formatted on 7/2/2019 01:17:56 PM (QP5 v5.313) */
PROMPT View PGE_ALL_LINE_EQUIP_SETTINGS;
--
-- PGE_ALL_LINE_EQUIP_SETTINGS  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.PGE_ALL_LINE_EQUIP_SETTINGS
(
    GLOBAL_ID,
    RADIO_MANF_CD,
    RADIO_MODEL_NUM,
    SCADA,
    SCADA_TYPE,
    SPECIAL_CONDITIONS,
    FEATURE_CLASS_NAME
)
AS
    (SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_capacitor c
      WHERE c.current_future = 'C'
     UNION
     SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_recloser c
      WHERE c.current_future = 'C'
     UNION
     SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_interrupter c
      WHERE c.current_future = 'C'
     UNION
     SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_sectionalizer c
      WHERE c.current_future = 'C'
     UNION
     SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_regulator c
      WHERE c.current_future = 'C'
     UNION
     SELECT c.global_id,
            c.radio_manf_cd,
            c.radio_model_num,
            c.scada,
            c.scada_type,
            c.special_conditions,
            c.feature_class_name
       FROM edsett.sm_switch c
      WHERE c.current_future = 'C')
/


Prompt Grants on VIEW PGE_ALL_LINE_EQUIP_SETTINGS TO BO_USER to BO_USER;
GRANT SELECT ON EDSETT.PGE_ALL_LINE_EQUIP_SETTINGS TO BO_USER
/
