Prompt drop View V_BADSPIDS;
DROP VIEW PGEDATA.V_BADSPIDS
/

/* Formatted on 6/27/2019 02:52:14 PM (QP5 v5.313) */
PROMPT View V_BADSPIDS;
--
-- V_BADSPIDS  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.V_BADSPIDS
(
    SP_ID
)
AS
      SELECT sp_id
        FROM SLCDX_DATA
       WHERE (sp_id,
              install_date,
              status,
              status_flag) IN
                 (SELECT DISTINCT
                         sp_id,
                         MAX (INSTALL_DATE) OVER (PARTITION BY sp_id)
                             install_date,
                         MIN (status) OVER (PARTITION BY sp_id)
                             status,
                         MIN (status_flag) OVER (PARTITION BY sp_id)
                             status_flag
                    FROM SLCDX_DATA)
    GROUP BY sp_id,
             install_date,
             status,
             status_flag
      HAVING COUNT (*) > 1
/


Prompt Grants on VIEW V_BADSPIDS TO A0SW to A0SW;
GRANT SELECT ON PGEDATA.V_BADSPIDS TO A0SW
/

Prompt Grants on VIEW V_BADSPIDS TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.V_BADSPIDS TO GIS_I
/

Prompt Grants on VIEW V_BADSPIDS TO S7MA to S7MA;
GRANT SELECT ON PGEDATA.V_BADSPIDS TO S7MA
/
