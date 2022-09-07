Prompt drop View VW_DEVICES;
DROP VIEW EDGISBO.VW_DEVICES
/

/* Formatted on 6/27/2019 02:52:08 PM (QP5 v5.313) */
PROMPT View VW_DEVICES;
--
-- VW_DEVICES  (View)
--

CREATE OR REPLACE FORCE VIEW EDGISBO.VW_DEVICES
(
    CEDSADEVICEID,
    DEVICETYPE,
    CIRCUITID,
    CIRCUITID2,
    OPERATINGNUMBER,
    STATUS,
    DIVISION,
    CGC12
)
AS
    (SELECT CEDSADEVICEID,
            '4'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.CAPACITORBANK
     UNION
     SELECT CEDSADEVICEID,
            '3'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.VOLTAGEREGULATOR
      WHERE SUBTYPECD IN ('1', '2')
     UNION
     SELECT CEDSADEVICEID,
            '5'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.VOLTAGEREGULATOR
      WHERE SUBTYPECD = 3
     UNION
     SELECT CEDSADEVICEID,
            '6'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.FUSE
     UNION
     SELECT CEDSADEVICEID,
            '7'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.DYNAMICPROTECTIVEDEVICE
      WHERE SUBTYPECD = 3
     UNION
     SELECT CEDSADEVICEID,
            '8'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.DYNAMICPROTECTIVEDEVICE
      WHERE SUBTYPECD = 8
     UNION
     SELECT CEDSADEVICEID,
            '12' DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.SWITCH
     UNION
     SELECT CEDSADEVICEID,
            '13' DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.DYNAMICPROTECTIVEDEVICE
      WHERE SUBTYPECD = 2
     UNION
     SELECT CEDSADEVICEID,
            '12' DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.OPENPOINT
     UNION
     SELECT CEDSADEVICEID,
            '9'  DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            NULL AS CGC12
       FROM EDGIS.STEPDOWN
     UNION
     SELECT CEDSADEVICEID,
            '15' DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            CGC12
       FROM EDGIS.TRANSFORMER
     UNION
     SELECT CEDSADEVICEID,
            '15' DEVICETYPE,
            CIRCUITID,
            CIRCUITID2,
            OPERATINGNUMBER,
            STATUS,
            DIVISION,
            CGC12
       FROM EDGIS.PRIMARYMETER)
/
