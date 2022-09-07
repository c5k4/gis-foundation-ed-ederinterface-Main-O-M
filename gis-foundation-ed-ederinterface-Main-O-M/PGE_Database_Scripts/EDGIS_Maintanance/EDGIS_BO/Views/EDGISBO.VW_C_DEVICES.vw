Prompt drop View VW_C_DEVICES;
DROP VIEW EDGISBO.VW_C_DEVICES
/

/* Formatted on 6/27/2019 02:52:09 PM (QP5 v5.313) */
PROMPT View VW_C_DEVICES;
--
-- VW_C_DEVICES  (View)
--

CREATE OR REPLACE FORCE VIEW EDGISBO.VW_C_DEVICES
(
    CGC12,
    CEDSADEVICEID,
    CIRCUITID,
    CIRCUITID2,
    DEVICETYPE,
    DIVISION,
    OPERATINGNUMBER,
    STATUS
)
AS
    (  SELECT MAX (CGC12)
                  AS CGC12,
              LISTAGG (TRIM (CAST (CEDSADEVICEID AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS CEDSADEVICEID,
              LISTAGG (TRIM (CAST (CIRCUITID AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS CIRCUITID,
              LISTAGG (TRIM (CAST (CIRCUITID2 AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS CIRCUITID2,
              LISTAGG (TRIM (CAST (DEVICETYPE AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS DEVICETYPE,
              LISTAGG (TRIM (CAST (DIVISION AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS DIVISION,
              LISTAGG (TRIM (CAST (OPERATINGNUMBER AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS OPERATINGNUMBER,
              LISTAGG (TRIM (CAST (STATUS AS VARCHAR2 (100))), ';')
                  WITHIN GROUP (ORDER BY CGC12)
                  AS STATUS
         FROM EDGISBO.vw_devices
        WHERE devicetype = 15 AND cgc12 IS NOT NULL AND cgc12 <> 9999
     GROUP BY CGC12)
/
