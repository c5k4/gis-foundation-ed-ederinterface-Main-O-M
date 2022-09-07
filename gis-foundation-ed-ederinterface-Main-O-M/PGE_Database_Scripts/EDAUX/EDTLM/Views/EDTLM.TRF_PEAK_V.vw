Prompt drop View TRF_PEAK_V;
DROP VIEW EDTLM.TRF_PEAK_V
/

/* Formatted on 7/1/2019 10:06:43 PM (QP5 v5.313) */
PROMPT View TRF_PEAK_V;
--
-- TRF_PEAK_V  (View)
--

CREATE OR REPLACE FORCE VIEW EDTLM.TRF_PEAK_V
(
    GLOBAL_ID,
    CGC_NUM,
    COAST_INTERIOR_FLAG,
    TRF_ID,
    SMR_CAP,
    SMR_KVA,
    SMR_PCT,
    SMR_PEAK_DATE,
    SMR_PEAK_SM_CUST_CNT,
    SMR_PEAK_TOTAL_CUST_CNT,
    WNTR_CAP,
    WNTR_KVA,
    WNTR_PCT,
    WNTR_PEAK_DATE,
    WNTR_PEAK_SM_CUST_CNT,
    WNTR_PEAK_TOTAL_CUST_CNT,
    SMR_LF,
    WNTR_LF,
    NP_KVA
)
AS
      SELECT B.global_id,
             B.cgc_id CGC_NUM,
             B.coast_interior_flg,
             A.trf_id,
             MAX (C.smr_cap),
             NVL (A.smr_kva, 0),
             NVL (MAX (C.smr_pct), 0),
             A.smr_peak_date,
             A.smr_peak_sm_cust_cnt,
             A.smr_peak_total_cust_cnt,
             MAX (C.wntr_cap),
             NVL (A.wntr_kva, 0),
             NVL (MAX (C.wntr_pct), 0),
             A.wntr_peak_date,
             A.wntr_peak_sm_cust_cnt,
             A.wntr_peak_total_cust_cnt,
             A.smr_lf,
             A.wntr_lf,
             C.np_kva
        FROM TRF_PEAK A
             INNER JOIN TRANSFORMER B ON A.trf_id = B.id
             INNER JOIN TRF_BANK_PEAK C ON A.id = C.trf_peak_id
    GROUP BY B.global_id,
             B.cgc_id,
             B.coast_interior_flg,
             A.trf_id,
             A.smr_kva,
             A.smr_peak_date,
             A.smr_peak_sm_cust_cnt,
             A.smr_peak_total_cust_cnt,
             A.wntr_kva,
             A.wntr_peak_date,
             A.wntr_peak_sm_cust_cnt,
             A.wntr_peak_total_cust_cnt,
             A.smr_lf,
             A.wntr_lf,
             C.np_kva
/


Prompt Grants on VIEW TRF_PEAK_V TO EDGISBO to EDGISBO;
GRANT SELECT ON EDTLM.TRF_PEAK_V TO EDGISBO
/

Prompt Grants on VIEW TRF_PEAK_V TO EDSETT_VIEWER to EDSETT_VIEWER;
GRANT SELECT ON EDTLM.TRF_PEAK_V TO EDSETT_VIEWER
/
