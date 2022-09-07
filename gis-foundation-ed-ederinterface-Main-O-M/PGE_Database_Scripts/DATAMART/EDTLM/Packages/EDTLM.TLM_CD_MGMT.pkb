Prompt drop Package Body TLM_CD_MGMT;
DROP PACKAGE BODY EDTLM.TLM_CD_MGMT
/

Prompt Package Body TLM_CD_MGMT;
--
-- TLM_CD_MGMT  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY EDTLM."TLM_CD_MGMT"
AS
PROCEDURE ProcessXML(
    XmlContent varchar2
    --,ErrorMessage OUT VARCHAR2,
    --IsSuccess OUT CHAR
    )
AS
  DB_ERROR_CODE VARCHAR2(20);
  DB_ERROR_MSG  VARCHAR2(200);
  AppType       CHAR(1);
  ProcessFor    VARCHAR2(20);
  XmlFile XmlType;

BEGIN
 -- ErrorMessage:='';
 -- IsSuccess   :='S';
  AppType     :='D';
  ProcessFor  := 'Transformer';
   XmlFile := XMLType(XmlContent);
  -----------Transformer-------------------------
  FOR r IN
  (SELECT ExtractValue(Value(p),'/ROW/CGC12/text()')            AS CGC_ID,
          ExtractValue(Value(p),'/ROW/GLOBALID/text()')         AS GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/COASTALIDC/text()')       AS COAST_INTERIOR_FLG,
          ExtractValue(Value(p),'/ROW/CLIMATEZONE/text()')      AS CLIMATE_ZONE_CD,
    --ExtractValue(Value(p),'/ROW/NUMBEROFPHASES/text()')           AS PHASE_CD,
          ExtractValue(Value(p),'/ROW/INSTALLATIONTYPE/text()') AS INSTALLATION_TYP,
          ExtractValue(Value(p),'/ROW/@TRANSACTIONTYPE')        AS TRANS_TYPE,
          ExtractValue(Value(p),'/ROW/DATEMODIFIED/text()')     AS TRANS_DATE,
          ExtractValue(Value(p),'/ROW/REPLACEGUID/text()')      AS OLD_GLOBAL_ID,
    --ExtractValue(Value(p),'/ROW/HIGHSIDEPROTECTION/text()')  AS TRANSFORMER_PROTECTION,
          ExtractValue(Value(p),'/ROW/LOWSIDEVOLTAGE/text()')   AS LOWSIDE_VOLTAGE,
          ExtractValue(Value(p),'/ROW/OPERATINGVOLTAGE/text()') AS OPERATING_VOLTAGE,
          ExtractValue(Value(p),'/ROW/VAULT/text()')            AS VAULT,
          ExtractValue(Value(p),'/ROW/CIRCUITID/text()')        AS CIRCUIT_ID
     FROM TABLE(XMLSequence(Extract(XmlFile,'/ENTITIES/ENTITY[@TYPE=''TRANSFORMER'']/ROW'))) p
  )
  LOOP
     BEGIN
---     added CIRCUIT_ID in the insert statement 06/18/2015.
        INSERT INTO cd_transformer
        ( cgc_id, global_id, coast_interior_flg, climate_zone_cd, installation_typ,trans_type,
          trans_date, old_global_id,  lowside_voltage, OPERATING_VOLTAGE, VAULT, CIRCUIT_ID )
        VALUES
        ( to_number(r.cgc_id), r.global_id,
          case r.coast_interior_flg when 'Y' then 1 when 'N' then 0   else null end,
          r.climate_zone_cd, r.installation_typ, r.trans_type,
          to_timestamp(r.trans_date,'MM/DD/YYYY HH:MI:SS AM'),
          r.old_global_id, r.lowside_voltage, r.OPERATING_VOLTAGE, r.vault, r.CIRCUIT_ID   );
     EXCEPTION
     WHEN OTHERS THEN
  --    IsSuccess    := 'N';
  --    ErrorMessage :='ERROR CODE: ' || SQLCODE || ' ERROR MESSAGE: ' || SQLERRM;
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        LogErrors(r.global_id, DB_ERROR_CODE, DB_ERROR_MSG, to_timestamp(r.trans_date,'MM/DD/YYYY HH:MI:SS AM'), r.trans_type, AppType,ProcessFor);
        CONTINUE;
     END;
  END LOOP;
  ---------------Transformer bank-----------------------------
  ProcessFor:='Transformer Bank';
  FOR r IN
  (SELECT ExtractValue(Value(p),'/ROW/TRANSFORMERGUID/text()')   AS TRF_GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/BANKCODE/text()')          AS BANK_CD,
          ExtractValue(Value(p),'/ROW/RATEDKVA/text()')          AS NP_KVA,
          ExtractValue(Value(p),'/ROW/GLOBALID/text()')          AS GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/NUMBEROFPHASES/text()')    AS PHASE_CD,
          ExtractValue(Value(p),'/ROW/@TRANSACTIONTYPE')         AS TRANS_TYPE,
          ExtractValue(Value(p),'/ROW/DATEMODIFIED/text()')      AS TRANS_DATE,
          ExtractValue(Value(p),'/ROW/REPLACEGUID/text()')       AS OLD_GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/TRANSFORMERTYPE/text()')   AS TRF_TYP
     FROM TABLE(XMLSequence(Extract(XmlFile,'/ENTITIES/ENTITY[@TYPE=''TRANSFORMERUNIT'']/ROW'))) p
  )
  LOOP
     BEGIN
---     added TRF_TYP in the insert statement 06/18/2015.
        INSERT INTO CD_TRANSFORMER_BANK
        ( TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_TYP)
        VALUES
        ( r.TRF_GLOBAL_ID, to_number(r.BANK_CD), to_number(r.NP_KVA), r.GLOBAL_ID, to_number(r.PHASE_CD), r.TRANS_TYPE,
          to_timestamp(r.TRANS_DATE,'MM/DD/YYYY HH:MI:SS AM'), r.OLD_GLOBAL_ID, r.TRF_TYP  );
      --IsSuccess:='S';
     EXCEPTION
     WHEN OTHERS THEN
    --  IsSuccess    := 'N';
    --  ErrorMessage :='ERROR CODE: ' || SQLCODE || ' ERROR MESSAGE: ' || SQLERRM;
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        LogErrors(r.global_id, DB_ERROR_CODE, DB_ERROR_MSG, to_timestamp(r.trans_date,'MM/DD/YYYY HH:MI:SS AM'), r.trans_type, AppType,ProcessFor);
        CONTINUE;
     END;
  END LOOP;
  --------------Meter -----------------------------
  ProcessFor:='Meter';
  FOR r IN
  (SELECT ExtractValue(Value(p),'/ROW/TRANSFORMERGUID/text()')   AS TRF_GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/GLOBALID/text()')          AS GLOBAL_ID,
          ExtractValue(Value(p),'/ROW/SERVICEPOINTID/text()')    AS SERVICE_POINT_ID,
          ExtractValue(Value(p),'/ROW/UNIQUESPID/text()')          AS UNQSPID,
          ExtractValue(Value(p),'/ROW/REVENUEACCOUNTCODE/text()')  AS REV_ACCT_CD,
          ExtractValue(Value(p),'/ROW/STREETNUMBER/text()')  AS SVC_ST_NUM,
          ExtractValue(Value(p),'/ROW/STREETNAME1/text()')   AS SVC_ST_NAME,
          ExtractValue(Value(p),'/ROW/STREETNAME2/text()')   AS SVC_ST_NAME2,
          ExtractValue(Value(p),'/ROW/CITY/text()')          AS SVC_CITY,
          ExtractValue(Value(p),'/ROW/STATE/text()')         AS SVC_STATE,
          ExtractValue(Value(p),'/ROW/ZIP/text()')           AS SVC_ZIP,
          ExtractValue(Value(p),'/ROW/CUSTOMERTYPE/text()')  AS CUST_TYP,
          ExtractValue(Value(p),'/ROW/RATESCHEDULE/text()')  AS RATE_SCHED,
          ExtractValue(Value(p),'/ROW/SMFLAG/text()')        AS SM_FLG,  --- This field is not present in the xml file.
          ExtractValue(Value(p),'/ROW/METERNUMBER/text()')   AS METER_NUMBER,
          ExtractValue(Value(p),'/ROW/@TRANSACTIONTYPE')     AS TRANS_TYPE,
          ExtractValue(Value(p),'/ROW/DATEMODIFIED/text()')  AS TRANS_DATE,
          ExtractValue(Value(p),'/ROW/REPLACEGUID/text()')   AS OLD_GLOBAL_ID
     FROM TABLE(XMLSequence(Extract(XmlFile,'/ENTITIES/ENTITY[@TYPE=''SERVICEPOINT'']/ROW'))) p
  )
  LOOP
    BEGIN
      INSERT
      INTO CD_METER
        ( SERVICE_POINT_ID, UNQSPID, TRF_GLOBAL_ID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2,
          SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID, SM_FLG, METER_NUMBER,
          TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID )
      VALUES
        ( r.SERVICE_POINT_ID, r.UNQSPID, r.TRF_GLOBAL_ID, r.REV_ACCT_CD, r.SVC_ST_NUM, r.SVC_ST_NAME, r.SVC_ST_NAME2,
          r.SVC_CITY, r.SVC_STATE, r.SVC_ZIP, r.CUST_TYP, r.RATE_SCHED, r.GLOBAL_ID, TRIM(r.SM_FLG), r.METER_NUMBER,
          r.TRANS_TYPE, to_timestamp(r.TRANS_DATE,'MM/DD/YYYY HH:MI:SS AM'), r.OLD_GLOBAL_ID
        );
      --IsSuccess:='S';
    EXCEPTION
    WHEN OTHERS THEN
      --IsSuccess    := 'N';
      --ErrorMessage :='ERROR CODE: ' || SQLCODE || ' ERROR MESSAGE: ' || SQLERRM;
       DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
       DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
       LogErrors(r.global_id, DB_ERROR_CODE, DB_ERROR_MSG, to_timestamp(r.trans_date,'MM/DD/YYYY HH:MI:SS AM'), r.trans_type, AppType,ProcessFor);
       CONTINUE;
    END;
  END LOOP;
END ProcessXML;
PROCEDURE LogErrors
  (
    GLOBALID  VARCHAR2,
    ERRORCODE VARCHAR2,
    ERRORMSG  VARCHAR2,
    TRANSDATE TIMESTAMP,
    TRANSTYPE CHAR,
    APPTYPE   CHAR,
    PROCFOR   VARCHAR2
  )
AS
BEGIN
  INSERT
  INTO CD_ERRORS
    (
      GLOBAL_ID,
      ERROR_CODE,
      ERROR_MSG,
      TRANS_DATE,
      TRANS_TYPE,
      CREATE_DTM,
      APP_TYPE,
      PROC_FOR
    )
    VALUES
    (
      GLOBALID,
      ERRORCODE,
      ERRORMSG,
      transdate,
      TRANSTYPE,
      CURRENT_TIMESTAMP,
      APPTYPE,
      PROCFOR
    );
END;
END TLM_CD_MGMT;
/


Prompt Grants on PACKAGE TLM_CD_MGMT TO GIS_I to GIS_I;
GRANT EXECUTE ON EDTLM.TLM_CD_MGMT TO GIS_I
/
