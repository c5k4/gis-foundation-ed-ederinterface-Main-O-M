Prompt drop View TRF_PEAK_GIS_V_UNFILTERED;
DROP VIEW EDTLM.TRF_PEAK_GIS_V_UNFILTERED
/

/* Formatted on 7/2/2019 01:17:59 PM (QP5 v5.313) */
PROMPT View TRF_PEAK_GIS_V_UNFILTERED;
--
-- TRF_PEAK_GIS_V_UNFILTERED  (View)
--

CREATE OR REPLACE FORCE VIEW EDTLM.TRF_PEAK_GIS_V_UNFILTERED
(
    GLOBALID,
    SUMMERKVA,
    WINTERKVA,
    SUMMERPCT,
    WINTERPCT,
    CGC_ID
)
AS
      SELECT CASE
                 WHEN B.GLOBAL_ID LIKE 'GLOBAL_ID%'
                 THEN
                     '{00000000-0000-0000-0000-000000000000}'
                 ELSE
                     B.GLOBAL_ID
             END,
             NVL (A.SMR_KVA, 0),
             NVL (A.WNTR_KVA, 0),
             NVL (MAX (C.SMR_PCT), 0),
             NVL (MAX (C.WNTR_PCT), 0),
             B.CGC_ID
        FROM TRF_PEAK A
             INNER JOIN TRANSFORMER B ON A.TRF_ID = B.ID
             LEFT OUTER JOIN TRF_BANK_PEAK C ON A.ID = C.TRF_PEAK_ID
    GROUP BY A.TRF_ID,
             B.GLOBAL_ID,
             A.SMR_KVA,
             A.WNTR_KVA,
             B.CGC_ID
/
