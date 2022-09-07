Prompt drop View VW_ENGINEERING_300KW_REPORT;
DROP VIEW EDGIS.VW_ENGINEERING_300KW_REPORT
/

/* Formatted on 7/2/2019 01:18:08 PM (QP5 v5.313) */
PROMPT View VW_ENGINEERING_300KW_REPORT;
--
-- VW_ENGINEERING_300KW_REPORT  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.VW_ENGINEERING_300KW_REPORT
(
    "Service_Point_ID",
    "Division",
    YR,
    MO,
    "Meter_Number",
    "Account_ID",
    "Service_Agreement_ID",
    "Customer_Name",
    "Service_City",
    "Service_Zip",
    "Type_of_Business",
    "Rev_Kwh",
    "Current_Rev_Power_Factor",
    "Customer_Type",
    "
Rate_Schedule",
    "Maximum_12_MonthKWDemand",
    "Minimum_12_MonthKWDemand",
    "
Jan_Dmd",
    "Feb_Dmd",
    "Mar_Dmd",
    "Apr_Dmd",
    "
May_Dmd",
    "Jun_Dmd",
    "Jul_Dmd",
    "Aug_Dmd",
    "
Sep_Dmd",
    "Oct_Dmd",
    "Nov_Dmd",
    "Dec_Dmd",
    "
Transformer_Operating#",
    "
Transformer_Coordinate",
    "Nameplate_KVA",
    "
Trans_Year_Installed",
    "Trans_Type",
    "
Tran_Secondary_Voltage_Code",
    "SSD_Operating_Number",
    "SSD_Equipment",
    "Summer_Load_Factor",
    "Winter_Load_Factor",
    "
Feeder_Name",
    "Feeder",
    DPA,
    "Summer_Transformer_Load_Status",
    "Winter_Transformer_Load_Status",
    "Address"
)
AS
    SELECT Service_Point_ID,
           Division,
           YR,
           MO,
           Meter_Number,
           Account_ID,
           Service_Agreement_ID,
           Customer_Name,
           Service_City,
           Service_Zip,
           Type_of_Business,
           Rev_Kwh,
           Current_Rev_Power_Factor,
           Customer_Type,
           Rate_Schedule,
           Maximum_12_MonthKWDemand,
           Minimum_12_MonthKWDemand,
           Jan_Dmd,
           Feb_Dmd,
           Mar_Dmd,
           Apr_Dmd,
           May_Dmd,
           Jun_Dmd,
           Jul_Dmd,
           Aug_Dmd,
           Sep_Dmd,
           Oct_Dmd,
           Nov_Dmd,
           Dec_Dmd,
           Transformer_Operating#,
           Transformer_Coordinate,
           Nameplate_KVA,
           Trans_Year_Installed,
           Trans_Type,
           Tran_Secondary_Voltage_Code,
           SSD_Operating_Number,
           SSD_Equipment,
           Summer_Load_Factor,
           Winter_Load_Factor,
           Feeder_Name,
           Feeder,
           DPA,
           Summer_Transformer_Load_Status,
           Winter_Transformer_Load_Status,
           Address
      FROM (WITH
                rundate
                AS
                    (SELECT ADD_MONTHS (
                                MAX (
                                    TO_DATE (rev_month || rev_year, 'mmyyyy')),
                                -11)
                                beg_date
                       FROM edtlm.ext_ccb_meter_load),
                recs
                AS
                    ( /* Narrow the table down to just service points/transformer months that are greater than or equal to 300kw, and max year, and the 1 previous months */
                     SELECT DISTINCT service_point_id
                       FROM edtlm.ext_ccb_meter_load
                      WHERE     TO_DATE (rev_month || rev_year, 'mmyyyy') >=
                                (SELECT beg_date FROM rundate)
                            AND rev_kw >= 300),
                spdata
                AS
                    ( /* consolidate the customer/servicepoint related data here */
                     SELECT sp.servicepointid,
                            sp.meternumber,
                            sp.accountnum,
                            sp.serviceagreementid,
                            cust.mailname1 || ' ' || cust.mailname2
                                customer_name,
                            sp.city,
                            sp.zip,
                            sp.naics,
                            sp.customertype,
                            sp.rateschedule,
                            la.TYPE_OF_BUSINESS
                                Type_of_Business,
                            div.description
                                cust_division,
                            NVL (sp.transformerguid, sp.primarymeterguid)
                                feature_guid
                       FROM edgis.zz_mv_servicepoint          sp,
                            customer.customer_info            cust,
                            lookup.local_office               lo,
                            LOOKUP.NAICS_LOOKUP               la,
                            edgis.pge_codes_and_descriptions  div
                      WHERE     sp.servicepointid IN
                                    (SELECT service_point_id FROM recs)
                            AND sp.servicepointid = cust.servicepointid
                            AND lo.local_office = sp.localofficeid
                            AND lo.division = div.code
                            AND la.NAICS(+) = sp.naics
                            AND div.domain_name = 'Division Name'),
                loads
                AS
                    (SELECT SERVICE_POINT_ID, /* If there is no data, then use 0 so least/greatest work below */
                            NVL (JAN_MAX_SP_KW, 0) AS JAN_KW,
                            NVL (FEB_MAX_SP_KW, 0) AS FEB_KW,
                            NVL (MAR_MAX_SP_KW, 0) AS MAR_KW,
                            NVL (APR_MAX_SP_KW, 0) AS APR_KW,
                            NVL (MAY_MAX_SP_KW, 0) AS MAY_KW,
                            NVL (JUN_MAX_SP_KW, 0) AS JUN_KW,
                            NVL (JUL_MAX_SP_KW, 0) AS JUL_KW,
                            NVL (AUG_MAX_SP_KW, 0) AS AUG_KW,
                            NVL (SEP_MAX_SP_KW, 0) AS SEP_KW,
                            NVL (OCT_MAX_SP_KW, 0) AS OCT_KW,
                            NVL (NOV_MAX_SP_KW, 0) AS NOV_KW,
                            NVL (DEC_MAX_SP_KW, 0) AS DEC_KW
                       FROM (SELECT service_point_id,
                                    TO_CHAR (TO_DATE (rev_month, 'MM'),
                                             'mon')
                                        mnth,
                                    TO_NUMBER (rev_kw)
                                        rev_kw
                               FROM edtlm.ext_ccb_meter_load
                              WHERE     TO_DATE (rev_month || rev_year,
                                                 'mmyyyy') >=
                                        (SELECT beg_date FROM rundate)
                                    AND service_point_id IN
                                            (SELECT service_point_id
                                               FROM recs))
                            PIVOT
                                (MAX (rev_kw)
                                AS max_sp_kw
                                FOR mnth
                                IN ('jan' AS Jan,
                                   'feb' AS Feb,
                                   'mar' AS Mar,
                                   'apr' AS apr,
                                   'may' AS may,
                                   'jun' AS Jun,
                                   'jul' AS jul,
                                   'aug' AS aug,
                                   'sep' AS sep,
                                   'oct' AS oct,
                                   'nov' AS nov,
                                   'dec' AS dec))),
                trfdata
                AS
                    (  SELECT trf.operatingnumber
                                  operatingnumber,
                              trf.CGC12
                                  cgc12,
                              TO_CHAR (trf.ratedkva)
                                  ratedkva,
                              MIN (TO_CHAR (unit.installationdate, 'yyyy'))
                                  installyear,
                              TO_CHAR (MIN (trftyp.description))
                                  TRANSFORMERTYPE,
                              TO_CHAR (lsv.description)
                                  AS LSV,
                              trf.circuitid,
                              trf.locdesc
                                  address,
                              trf.globalid
                                  globalid,
                              div.description
                                  DIVISION,
                              trf.ssdguid
                                  ssdguid
                         FROM EDGIS.zz_mv_transformer         trf,
                              EDGIS.zz_mv_transformerunit     unit,
                              EDGIS.pge_codes_and_descriptions lsv,
                              EDGIS.pge_codes_and_descriptions trftyp,
                              EDGIS.pge_codes_and_descriptions div
                        WHERE     trf.globalid = unit.transformerguid
                              AND trf.lowsidevoltage = lsv.code(+)
                              AND lsv.domain_name(+) = 'Secondary Voltage'
                              AND unit.transformertype = trftyp.code(+)
                              AND trftyp.domain_name(+) =
                                  'Trans Unit Short Desc'
                              AND trf.division = div.code(+)
                              AND div.domain_name(+) = 'Division Name'
                     GROUP BY trf.operatingnumber,
                              trf.CGC12,
                              TO_CHAR (trf.ratedkva),
                              TO_CHAR (lsv.description),
                              trf.circuitid,
                              trf.locdesc,
                              trf.globalid,
                              div.description,
                              trf.ssdguid
                     UNION
                     SELECT PM.operatingnumber
                                operatingnumber,
                            PM.CGC12
                                cgc12,
                            TO_CHAR (PM.ratedkva)
                                ratedkva,
                            TO_CHAR (pm.installationdate, 'yyyy')
                                installyear,
                            TO_CHAR ('Primary Meter')
                                TRANSFORMERTYPE,
                            TO_CHAR ('Primary Meter')
                                LSV,
                            PM.circuitid
                                feeder,
                            PM.locationdesc
                                address,
                            PM.globalid
                                globalid,
                            div.description
                                DIVISION,
                            PM.ssdguid
                                ssdguid
                       FROM EDGIS.zz_mv_PrimaryMeter          PM,
                            EDGIS.pge_codes_and_descriptions  div
                      WHERE     pm.division(+) = div.code
                            AND div.domain_name = 'Division Name'),
                feederData
                AS
                    (SELECT ckt.circuitid,
                            substationname || ' ' || circuitname feedername,
                            dpa.description                      dpa
                       FROM edgis.zz_mv_circuitsource         ckt,
                            edgis.zz_mv_electricstitchpoint   s,
                            edgis.pge_codes_and_descriptions  dpa
                      WHERE     ckt.deviceguid = s.globalid(+)
                            AND s.distributionplanningarea = dpa.code(+)
                            AND dpa.domain_name(+) =
                                'Distribution Planning Area'),
                lfs
                AS
                    (SELECT tp.SMR_LF, tp.WNTR_LF, tr.GLOBAL_ID
                       FROM EDTLM.TRF_PEAK tp, edtlm.transformer tr
                      WHERE tp.trf_id = tr.id),
                pct
                AS
                    (  SELECT tr.global_id,
                              MAX (tbp.SMR_KVA / SMR_CAP) SMR_PCT,
                              MAX (tbp.WNTR_KVA / WNTR_CAP) WNTR_PCT
                         FROM EDTLM.TRF_BANK_PEAK   tbp,
                              edtlm.transformer_bank tb,
                              EDTLM.transformer     tr
                        WHERE tb.trf_id = tr.id AND tbp.TRF_BANK_ID = tb.id
                     GROUP BY tr.global_id),
                ssd
                AS
                    (SELECT globalid, operatingnumber, 'Fuse' AS DEVICETYPE
                       FROM EDGIS.zz_mv_fuse
                     UNION
                     SELECT globalid, operatingnumber, 'Switch'
                       FROM EDGIS.zz_mv_switch
                     UNION
                     SELECT globalid,
                            operatingnumber,
                            DECODE (subtypecd,
                                    2, 'Interrupter',
                                    3, 'Recloser',
                                    8, 'Sectionalizer',
                                    'Data Problem')
                       FROM EDGIS.zz_mv_dynamicprotectivedevice),
                Curr_load_data
                AS
                    (SELECT service_point_id,
                            TO_CHAR (TO_DATE (rev_month, 'mm'), 'MON')
                                rev_month,
                            rev_year,
                            rev_kwhr,
                            pfactor
                       FROM edtlm.ext_ccb_meter_load
                      WHERE (service_point_id, rev_year || rev_month) IN
                                (  SELECT service_point_id,
                                          MAX (rev_year || rev_month)
                                              recent_usage
                                     FROM edtlm.ext_ccb_meter_load
                                    WHERE     TO_DATE (rev_month || rev_year,
                                                       'mmyyyy') >=
                                              (SELECT beg_date FROM rundate)
                                          AND service_point_id IN
                                                  (SELECT SERVICE_POINT_ID
                                                     FROM RECS)
                                 GROUP BY service_point_id))
            SELECT spdata.servicepointid
                       Service_Point_ID,
                   NVL (trfdata.Division, spdata.cust_division)
                       Division,
                   rev_month
                       MO,
                   rev_year
                       YR,
                   spdata.meternumber
                       Meter_Number,
                   spdata.accountnum
                       Account_ID,
                   spdata.serviceagreementid
                       Service_Agreement_ID,
                   spdata.customer_name
                       Customer_Name,
                   spdata.city
                       Service_City,
                   spdata.zip
                       Service_Zip,
                   spdata.type_of_business
                       Type_of_Business,
                   Curr_load_data.rev_kwhr
                       Rev_Kwh,
                   Curr_load_data.pfactor
                       Current_Rev_Power_Factor,
                   spdata.customertype
                       Customer_Type,
                   spdata.rateschedule
                       Rate_Schedule,
                   ROUND (GREATEST (JAN_KW,
                                    FEB_KW,
                                    MAR_KW,
                                    APR_KW,
                                    MAY_KW,
                                    JUN_KW,
                                    JUL_KW,
                                    AUG_KW,
                                    SEP_KW,
                                    OCT_KW,
                                    NOV_KW,
                                    DEC_KW))
                       Maximum_12_MonthKWDemand,
                   ROUND (LEAST (JAN_KW,
                                 FEB_KW,
                                 MAR_KW,
                                 APR_KW,
                                 MAY_KW,
                                 JUN_KW,
                                 JUL_KW,
                                 AUG_KW,
                                 SEP_KW,
                                 OCT_KW,
                                 NOV_KW,
                                 DEC_KW))
                       Minimum_12_MonthKWDemand,
                   ROUND (JAN_KW)
                       Jan_Dmd,
                   ROUND (FEB_KW)
                       Feb_Dmd,
                   ROUND (MAR_KW)
                       Mar_Dmd,
                   ROUND (APR_KW)
                       Apr_Dmd,
                   ROUND (MAY_KW)
                       May_Dmd,
                   ROUND (JUN_KW)
                       Jun_Dmd,
                   ROUND (JUL_KW)
                       Jul_Dmd,
                   ROUND (AUG_KW)
                       Aug_Dmd,
                   ROUND (SEP_KW)
                       Sep_Dmd,
                   ROUND (OCT_KW)
                       Oct_Dmd,
                   ROUND (NOV_KW)
                       Nov_Dmd,
                   ROUND (DEC_KW)
                       Dec_Dmd,
                   trfdata.operatingnumber
                       Transformer_Operating#,
                   ' ' || trfdata.cgc12
                       Transformer_Coordinate,
                   trfdata.ratedkva
                       Nameplate_KVA,
                   trfdata.installyear
                       Trans_Year_Installed,
                   trfdata.transformertype
                       Trans_Type,
                   trfdata.lsv
                       Tran_Secondary_Voltage_Code,
                   ssd.operatingnumber
                       SSD_Operating_Number,
                   ssd.DEVICETYPE
                       SSD_Equipment,
                   lfs.SMR_LF
                       Summer_Load_Factor,
                   lfs.WNTR_LF
                       Winter_Load_Factor,
                   feederData.feedername
                       Feeder_Name,
                   feederData.circuitid
                       Feeder,
                   feederData.dpa
                       DPA,
                   CASE
                       WHEN pct.SMR_PCT IS NULL THEN ''
                       WHEN pct.SMR_PCT > 1 THEN 'OVERLOADED'
                       ELSE 'CORRECT'
                   END
                       Summer_Transformer_Load_Status,
                   CASE
                       WHEN pct.WNTR_PCT IS NULL THEN ''
                       WHEN pct.WNTR_PCT > 1 THEN 'OVERLOADED'
                       ELSE 'CORRECT'
                   END
                       Winter_Transformer_Load_Status,
                   trfdata.address
                       Address
              FROM spdata,
                   loads,
                   trfdata,
                   feederData,
                   lfs,
                   pct,
                   ssd,
                   Curr_load_data
             WHERE     spdata.servicepointid = loads.service_point_id
                   AND spdata.feature_guid = trfdata.globalid(+) -- not all spids have a feature
                   AND trfdata.circuitid = feederData.circuitid(+)
                   AND trfdata.globalid = lfs.global_id(+)
                   AND trfdata.globalid = pct.global_id(+)
                   AND trfdata.ssdguid = ssd.globalid(+)
                   AND spdata.servicepointid =
                       Curr_load_data.service_point_id(+))
/


Prompt Grants on VIEW VW_ENGINEERING_300KW_REPORT TO GEOMART_RO to GEOMART_RO;
GRANT SELECT ON EDGIS.VW_ENGINEERING_300KW_REPORT TO GEOMART_RO WITH GRANT OPTION
/
