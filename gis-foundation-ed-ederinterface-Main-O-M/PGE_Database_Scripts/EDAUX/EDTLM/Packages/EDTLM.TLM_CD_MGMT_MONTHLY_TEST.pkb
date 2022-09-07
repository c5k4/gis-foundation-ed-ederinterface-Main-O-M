Prompt drop Package Body TLM_CD_MGMT_MONTHLY_TEST;
DROP PACKAGE BODY EDTLM.TLM_CD_MGMT_MONTHLY_TEST
/

Prompt Package Body TLM_CD_MGMT_MONTHLY_TEST;
--
-- TLM_CD_MGMT_MONTHLY_TEST  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY EDTLM."TLM_CD_MGMT_MONTHLY_TEST"
AS
  AppType CHAR(1);
  ProcedureFor varchar2(20);
  NoOfRows number;
  Old_Service_Point_Id VARCHAR2(10);
  FoundGlobalId varchar2(38);
PROCEDURE TRANSFORMER_MGMT(
    FromDate DATE,
    ToDate   DATE,
    ErrorMsg OUT VARCHAR2,
    ErrorCode OUT VARCHAR2 )
AS
    v_Rec_Count NUMBER;
    v_TRANS_TYPE VARCHAR2(1);
BEGIN
    ErrorMsg  := '';
    ErrorCode :='';
    AppType   :='M';
    PROCEDUREFOR := 'TRF';
    v_Rec_Count :=0;
    v_TRANS_TYPE :='';
   /* get all rows from change detection stage table where transaction types are update,replace or delete */
    DECLARE
       CURSOR c_cd_transformer
       IS
       SELECT * FROM cd_transformer
        WHERE TRANS_TYPE IN ('I','D','U','R') AND PROC_FLG is null
        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
        ORDER BY Id;
    BEGIN
       FOR cd_trf_rec IN c_cd_transformer
       LOOP
          BEGIN
             v_TRANS_TYPE := cd_trf_rec.trans_type;
             --DBMS_OUTPUT.PUT_LINE('v_TRANS_TYPE: ' || cd_trf_rec.trans_type);
             IF TLM_CD_CHECK_ISNULL (cd_trf_rec.GLOBAL_ID, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-02') = 'TRUE' THEN
                CONTINUE;
             ELSIF TLM_CD_CHECK_ISNULL (cd_trf_rec.CGC_ID, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-03') = 'TRUE' THEN
                CONTINUE;
             ELSIF TLM_CD_CHECK_ISNULL (cd_trf_rec.INSTALLATION_TYP, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-04') = 'TRUE' THEN
                CONTINUE;
             ELSIF length(trim(cd_trf_rec.COAST_INTERIOR_FLG)) > 1 THEN   --- change for new CCB interface
                   ErrorCode:='CD-TRF-07';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
             END IF;
             IF cd_trf_rec.trans_type='U' or cd_trf_rec.trans_type='D'  THEN
                begin
                   select GLOBAL_ID into FoundGlobalId from Transformer where global_Id = cd_trf_rec.global_id;
                exception
                when NO_DATA_FOUND THEN
                   IF cd_trf_rec.trans_type='U' THEN --Update
                      IF TLM_CD_HAS_INSERT_RECORD('CD_TRANSFORMER', cd_trf_rec.global_id, cd_trf_rec.create_dtm) = 'TRUE' THEN
                         -- there was a failed insert record in the CD table. So UPSERT.
                         v_TRANS_TYPE := 'I';
                      ELSE
                         ErrorCode:='CD-TRF-01';
                         SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                         LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                         cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                         continue;
                      END IF;
                   ELSE -- Delete
                        -- Update the PROC_FLG flag.
                      UPDATE CD_TRANSFORMER T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
                      continue;
                   END IF;
                end;
             END IF;

             IF cd_trf_rec.trans_type='R' THEN
                begin
                   select global_id into FoundGlobalId from Transformer where global_Id = cd_trf_rec.old_global_id;
                exception
                when NO_DATA_FOUND THEN
                   ErrorCode:='CD-TRF-01';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
           --      ErrorMsg := 'No records exists in Transformer!';
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
                end;
             END IF;

             IF v_TRANS_TYPE='I' THEN -- cd_trf_rec.trans_type='I'
                IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'global_id', cd_trf_rec.global_id) = 'TRUE' THEN
                   ErrorCode:='CD-TRF-05';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
                END IF;

                IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'CGC_ID', cd_trf_rec.CGC_ID) = 'TRUE' THEN
                   ErrorCode:='CD-TRF-06';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                   ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
                END IF;

                INSERT INTO TRANSFORMER
                ( CGC_ID, GLOBAL_ID, COAST_INTERIOR_FLG, CLIMATE_ZONE_CD,
                  INSTALLATION_TYP, LOWSIDE_VOLTAGE, OPERATING_VOLTAGE, CIRCUIT_ID, VAULT, REGION )
                VALUES
                ( cd_trf_rec.CGC_ID, cd_trf_rec.GLOBAL_ID, substr(to_char(cd_trf_rec.COAST_INTERIOR_FLG),1,1), cd_trf_rec.CLIMATE_ZONE_CD,
              --cd_trf_rec.PHASE_CD,
                  cd_trf_rec.INSTALLATION_TYP, cd_trf_rec.LOWSIDE_VOLTAGE, cd_trf_rec.OPERATING_VOLTAGE, cd_trf_rec.CIRCUIT_ID, cd_trf_rec.VAULT,
                  NULL  );
             END IF;

             IF v_TRANS_TYPE='U' THEN -- cd_trf_rec.trans_type='U'
                v_Rec_Count := 0;
                SELECT COUNT(1) INTO v_Rec_Count FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
                 WHERE CGC_ID = cd_trf_rec.CGC_ID AND global_Id <> cd_trf_rec.global_id;
                IF v_Rec_Count > 0 THEN
                   ErrorCode:='CD-TRF-06';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                   ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
                END IF;

                UPDATE TRANSFORMER
                   SET CGC_ID               = cd_trf_rec.CGC_ID,
                       COAST_INTERIOR_FLG   = substr(to_char(cd_trf_rec.COAST_INTERIOR_FLG),1,1),   --- change for new CCB interface
                       CLIMATE_ZONE_CD      = cd_trf_rec.CLIMATE_ZONE_CD,
            --         PHASE_CD             = cd_trf_rec.PHASE_CD,
                       INSTALLATION_TYP     = cd_trf_rec.INSTALLATION_TYP,
                       LOWSIDE_VOLTAGE      = cd_trf_rec.LOWSIDE_VOLTAGE,
                       OPERATING_VOLTAGE    = cd_trf_rec.OPERATING_VOLTAGE,
                       CIRCUIT_ID           = cd_trf_rec.CIRCUIT_ID,
                       VAULT                = cd_trf_rec.VAULT
                 WHERE GLOBAL_ID            = cd_trf_rec.GLOBAL_ID;

             elsif v_TRANS_TYPE='R' THEN -- cd_trf_rec.trans_type='R'
                v_Rec_Count := 0;
                SELECT COUNT(1) INTO v_Rec_Count FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
                 WHERE CGC_ID = cd_trf_rec.CGC_ID AND global_Id <> cd_trf_rec.Old_global_id;
                IF v_Rec_Count > 0 THEN
                   ErrorCode:='CD-TRF-06';
                   SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                   ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
                   LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
                   cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                   continue;
                END IF;

                UPDATE TRANSFORMER
                   SET CGC_ID             = cd_trf_rec.CGC_ID,
                       GLOBAL_ID          = cd_trf_rec.GLOBAL_ID,
                       COAST_INTERIOR_FLG = substr(to_char(cd_trf_rec.COAST_INTERIOR_FLG),1,1),   --- change for new CCB interface ,
                       CLIMATE_ZONE_CD    = cd_trf_rec.CLIMATE_ZONE_CD,
            --PHASE_CD               = cd_trf_rec.PHASE_CD,
                       INSTALLATION_TYP   = cd_trf_rec.INSTALLATION_TYP,
                       LOWSIDE_VOLTAGE    = cd_trf_rec.LOWSIDE_VOLTAGE,
                       OPERATING_VOLTAGE  = cd_trf_rec.OPERATING_VOLTAGE,
                       CIRCUIT_ID         = cd_trf_rec.CIRCUIT_ID,
                       VAULT              = cd_trf_rec.VAULT
                 WHERE GLOBAL_ID          = cd_trf_rec.OLD_GLOBAL_ID;

                V_Rec_count := 0;
                SELECT COUNT(1) INTO v_Rec_Count FROM EDSETT.SM_SPECIAL_LOAD -- Check for the Special Load record is exists in setting schema
                 WHERE Ref_global_Id = cd_trf_rec.OLD_GLOBAL_ID;
                IF v_Rec_Count <> 0 THEN
                   dbms_output.put_line(cd_trf_rec.GLOBAL_ID || cd_trf_rec.old_GLOBAL_ID );
                   Update EDSETT.SM_SPECIAL_LOAD Set REF_GLOBAL_ID = cd_trf_rec.GLOBAL_ID
                    where REF_GLOBAL_ID=cd_trf_rec.OLD_GLOBAL_ID;
                End if ;

             elsif v_TRANS_TYPE='D' THEN -- cd_trf_rec.trans_type='D'
                UPDATE TRANSFORMER SET REC_STATUS = 'D' WHERE GLOBAL_ID=cd_trf_rec.GLOBAL_ID;
                UPDATE CD_TRANSFORMER T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
             ELSE
                NULL;
             END IF;

        --SELECT COUNT(*) into NoOfRows FROM TRANSFORMER T WHERE T.GLOBAL_ID = CD_TRF_REC.GLOBAL_ID;
             IF SQL%ROWCOUNT>0 THEN
                UPDATE CD_TRANSFORMER T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
             END IF;
          EXCEPTION
          WHEN OTHERS THEN
             ErrorCode := SQLCODE;
             ErrorMsg  := SUBSTR(SQLERRM,1,200);
             LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
             cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
        --   DBMS_OUTPUT.PUT_LINE('ErrorMsg: ' || ErrorMsg);
             continue;
          END;
       END LOOP;
       COMMIT;
    END;
END TRANSFORMER_MGMT;

PROCEDURE TRANSFORMER_BANK_MGMT(
   FromDate DATE,
   ToDate   DATE,
   ErrorMsg OUT VARCHAR2,
   ErrorCode OUT VARCHAR2 )
AS
   transformer_id NUMBER;
   v_Rec_Count NUMBER;
   v_TRANS_TYPE VARCHAR2(1);
BEGIN
   ErrorMsg  := '';
   ErrorCode :='';
   AppType   :='M';
   ProcedureFor:='TRF-BANK';
   v_Rec_Count:=0;
   v_TRANS_TYPE:='';
  /* get all rows from change detection stage table where transaction types are update,replace or delete */
   DECLARE
      CURSOR c_cd_transformer_bank
      IS
      SELECT * FROM cd_transformer_bank
       WHERE TRANS_TYPE IN ('I','D','U','R') AND PROC_FLG IS NULL
        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
       ORDER BY Id;
   BEGIN
      FOR cd_trf_bank_rec IN c_cd_transformer_bank
      LOOP
         BEGIN
            v_TRANS_TYPE := cd_trf_bank_rec.trans_type;
            IF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.TRF_GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-01') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-04') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.BANK_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-05') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.NP_KVA, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-06') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.PHASE_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-07') = 'TRUE' THEN
               CONTINUE;
            ELSIF (( LENGTH(TRIM(TRANSLATE(cd_trf_bank_rec.BANK_CD, ' +-.0123456789', ' '))) is not Null ) OR To_Number(cd_trf_bank_rec.BANK_CD) > 9 ) then  --- change for new CCB interface
                transformer_id:=0;
                ErrorCode:='CD-BNK-10';
                SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                ErrorMsg := ErrorMsg || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID;
                LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                continue;
            ELSE
               begin
                  SELECT ID INTO transformer_id
                    FROM TRANSFORMER
                   WHERE GLOBAL_ID =cd_trf_bank_rec.TRF_GLOBAL_ID;
               exception
               when no_data_found then
                  IF cd_trf_bank_rec.trans_type <> 'D'  THEN
                     transformer_id:=0;
                     ErrorCode:='CD-BNK-02';
                     SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                     ErrorMsg := ErrorMsg || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID;
                     LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                     to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                     continue;
                  END IF;
               end;
            END IF;

            IF cd_trf_bank_rec.trans_type='U' OR cd_trf_bank_rec.trans_type='D'  THEN
               begin
                  select Global_Id into FoundGlobalId from Transformer_Bank where Global_Id = cd_trf_bank_rec.Global_Id;
               exception
               when NO_DATA_FOUND THEN
                  IF cd_trf_bank_rec.trans_type='U' THEN -- Update
                     IF TLM_CD_HAS_INSERT_RECORD('CD_TRANSFORMER_BANK', cd_trf_bank_rec.global_id, cd_trf_bank_rec.create_dtm) = 'TRUE' THEN
                        -- there was a failed insert record in the CD table.
                        v_TRANS_TYPE := 'I'; --UPSERT
                     ELSE
                        ErrorCode:='CD-BNK-03';
                        SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                        LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                                   to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                        continue;
                     END IF;
                  ELSE -- Delete
                     -- there was a failed insert in the CD table. So just update the flag.
                     UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG='P' WHERE T.ID= cd_trf_bank_rec.ID;
                     continue;
                  END IF;
               end;
            END IF;
            IF cd_trf_bank_rec.trans_type='R' THEN
               begin
                  select Global_Id into FoundGlobalId from Transformer_Bank where Global_Id = cd_trf_bank_rec.Old_Global_Id;
               exception
               when no_data_found then
                  ErrorCode:='CD-BNK-03';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
            --    ErrorMsg:='No records exists in Transformer Bank table';
                  LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                  continue;
               end;
            END IF;

            IF v_TRANS_TYPE='I' THEN --cd_trf_bank_rec.trans_type='I'

               IF TLM_CD_CHECK_ISDUP ('TRANSFORMER_BANK', 'global_id', cd_trf_bank_rec.global_id) = 'TRUE' THEN
                  ErrorCode:='CD-BNK-08';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                  LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                  continue;
               END IF;

               v_Rec_Count := 0;
               SELECT COUNT(1) INTO v_Rec_Count FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
                WHERE TRF_ID = transformer_id AND BANK_CD = cd_trf_bank_rec.BANK_CD;
               IF v_Rec_Count > 0 THEN
                  ErrorCode:='CD-BNK-09';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                  ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
                  LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                  continue;
               END IF;

               INSERT INTO TRANSFORMER_BANK
               ( TRF_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD,TRF_TYP )
                VALUES ( transformer_id, to_number(cd_trf_bank_rec.BANK_CD), cd_trf_bank_rec.NP_KVA, cd_trf_bank_rec.GLOBAL_ID, cd_trf_bank_rec.PHASE_CD, cd_trf_bank_rec.TRF_TYP );
            END IF;

            IF v_TRANS_TYPE='U' THEN --cd_trf_bank_rec.trans_type='U'
               v_Rec_Count := 0;
               SELECT COUNT(1) INTO v_Rec_Count FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
                WHERE (TRF_ID = transformer_id AND BANK_CD = cd_trf_bank_rec.BANK_CD)
                  AND global_Id <> cd_trf_bank_rec.global_id;
               IF v_Rec_Count > 0 THEN
                  ErrorCode:='CD-BNK-09';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                  ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
                  LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                  continue;
               END IF;

               UPDATE TRANSFORMER_BANK
                  SET TRF_ID    = transformer_Id,
                      BANK_CD   = to_number(cd_trf_bank_rec.BANK_CD),      --- change for new CCB interface
                      NP_KVA    = cd_trf_bank_rec.NP_KVA,
                      PHASE_CD  = cd_trf_bank_rec.PHASE_CD,
                      TRF_TYP   = cd_trf_bank_rec.TRF_TYP
                WHERE GLOBAL_ID = cd_trf_bank_rec.GLOBAL_ID;

            ELSIF v_TRANS_TYPE='R' THEN -- cd_trf_bank_rec.trans_type='R'
               v_Rec_Count := 0;
               SELECT COUNT(1) INTO v_Rec_Count FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
                WHERE (TRF_ID = transformer_id AND BANK_CD = cd_trf_bank_rec.BANK_CD)
                  AND global_Id = cd_trf_bank_rec.global_id;
               IF v_Rec_Count > 0 THEN
                  ErrorCode:='CD-BNK-09';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                  ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
                  LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
                  continue;
               END IF;

               UPDATE TRANSFORMER_BANK
                  SET TRF_ID    = transformer_id,
                      BANK_CD   = to_number(cd_trf_bank_rec.BANK_CD),      --- change for new CCB interface
                      NP_KVA    = cd_trf_bank_rec.NP_KVA,
                      PHASE_CD  = cd_trf_bank_rec.PHASE_CD,
                      GLOBAL_ID = cd_trf_bank_rec.GLOBAL_ID
                WHERE GLOBAL_ID = cd_trf_bank_rec.OLD_GLOBAL_ID;
            ELSIF v_TRANS_TYPE='D' THEN -- cd_trf_bank_rec.trans_type='D'
               UPDATE TRANSFORMER_BANK
                  SET REC_STATUS = 'D'
                WHERE GLOBAL_ID=cd_trf_bank_rec.GLOBAL_ID;
               UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG='P' WHERE T.ID= cd_trf_bank_rec.ID;
            ELSE
               NULL;
            END IF;

            IF SQL%ROWCOUNT>0 THEN
                  UPDATE CD_TRANSFORMER_BANK B SET B.PROC_FLG='P' WHERE B.ID=cd_trf_bank_rec.ID;
            END IF;
         EXCEPTION
            WHEN OTHERS THEN
            ErrorCode := SQLCODE;
            ErrorMsg  := SUBSTR(SQLERRM,1,200);
            LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_trf_bank_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
            cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            continue;
         END;
      END LOOP;
      COMMIT;
   END;
END TRANSFORMER_BANK_MGMT;
/* CHANGE DETECTION PROCEDURE FOR METER  */
PROCEDURE METER_MGMT(
   FromDate DATE,
   ToDate   DATE,
   ErrorMsg OUT VARCHAR2,
   ErrorCode OUT VARCHAR2 )
AS
   transformer_id NUMBER;
   v_Rec_Count NUMBER;
   v_TRANS_TYPE VARCHAR2(1);
BEGIN
   ErrorMsg  := '';
   ErrorCode :='';
   AppType   :='M';
   ProcedureFor:='METER';
   v_Rec_Count:=0;
   v_TRANS_TYPE:='';

  /* get all rows from change detection stage table where transaction types are update,replace or delete */
   DECLARE
      CURSOR c_cd_meter
      IS
      SELECT * FROM cd_meter
      WHERE TRANS_TYPE IN ('I','D','U','R') AND PROC_FLG IS NULL
        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
      ORDER BY Id;
   BEGIN
      FOR cd_meter_rec IN c_cd_meter
      LOOP
         BEGIN
            v_TRANS_TYPE := cd_meter_rec.trans_type;
            IF TLM_CD_CHECK_ISNULL (cd_meter_rec.TRF_GLOBAL_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-01') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_meter_rec.GLOBAL_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-05') = 'TRUE' THEN
               CONTINUE;
            ELSIF TLM_CD_CHECK_ISNULL (cd_meter_rec.SERVICE_POINT_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-06') = 'TRUE' THEN
               CONTINUE;
            ELSE
               begin
                  SELECT ID INTO transformer_id
                    FROM TRANSFORMER
                   WHERE GLOBAL_ID =cd_meter_rec.TRF_GLOBAL_ID;
               exception
               when no_data_found then
                  IF cd_meter_rec.trans_type <> 'D'  THEN
                     transformer_id:=0;
                     ErrorCode:='CD-MTR-02';
                     SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                     ErrorMsg:= ErrorMsg || '. TRF_GLOBAL_ID:' || cd_meter_rec.TRF_GLOBAL_ID;
                     LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                     to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                     continue;
                  END IF;
               end;
            END IF;

            IF cd_meter_rec.trans_type='U' OR cd_meter_rec.trans_type='D'  THEN
               begin
                  select Global_Id into FoundGlobalId from Meter where Global_Id = cd_meter_rec.Global_Id;
               exception
               when NO_DATA_FOUND THEN
                  IF cd_meter_rec.trans_type='U' THEN -- Update
                     IF TLM_CD_HAS_INSERT_RECORD('CD_METER', cd_meter_rec.global_id, cd_meter_rec.create_dtm) = 'TRUE' THEN
                        -- there was a failed insert record in the CD table.
                        v_TRANS_TYPE := 'I'; --UPSERT
                     ELSE
                        ErrorCode:='CD-MTR-03';
                        SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                        -- ErrorMsg:='No records exists in Meter table';
                        LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                        to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                        continue;
                     END IF;
                  ELSE -- Delete
                     UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
                     continue;
                  END IF;
               end;
            END IF;

            IF cd_meter_rec.trans_type='R' THEN
               begin
                  select Global_Id into FoundGlobalId from Meter where Global_Id = cd_meter_rec.Old_Global_Id;
               exception
               when no_data_found then
                  ErrorCode:='CD-MTR-03';
                  SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                  ErrorMsg:= ErrorMsg || '. Old_Global_Id: ' || cd_meter_rec.Old_Global_Id;
                  LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                  to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  continue;
               end;
            END IF;

            IF v_TRANS_TYPE='I' THEN --cd_meter_rec.trans_type='I'
               BEGIN
                  IF cd_meter_rec.SERVICE_POINT_ID  IS NOT NULL THEN
                     BEGIN
                        v_Rec_Count:=0;
                        -- Find Duplicate SERVICE POINT ID
                        SELECT Count(1)
                          INTO v_Rec_Count
                          FROM METER
                         WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID;
                        IF (v_Rec_Count > 0) THEN
                           v_Rec_Count:=0;
                           ErrorCode:='CD-MTR-08';
                           SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                           ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                           LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                           to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                           continue;
                        END IF;
                     EXCEPTION
                     when no_data_found then
                     -- As expected. So do nothing. The record is good.
                        NULL;
                     when others then
                        ErrorCode := SQLCODE;
                        ErrorMsg  := SUBSTR(SQLERRM,1,200);
                        LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                        continue;
                     END;
                  END IF;

                  v_Rec_Count := 0;
                  SELECT COUNT(1) INTO v_Rec_Count FROM METER -- Check for the duplicate meter
                   WHERE global_Id = cd_meter_rec.global_id;
                  IF v_Rec_Count > 0 THEN
                     ErrorCode:='CD-MTR-09';
                     SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                     LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                     to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                     continue;
                  END IF;

            --Insert record
                  INSERT INTO METER
                  ( SERVICE_POINT_ID, UNQSPID, TRF_ID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2,
                    SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED, GLOBAL_ID, SM_FLG, METER_NUMBER )
                  VALUES
                  ( cd_meter_rec.SERVICE_POINT_ID, cd_meter_rec.UNQSPID, transformer_id, cd_meter_rec.REV_ACCT_CD,
                    substr(cd_meter_rec.SVC_ST_NUM,1,12), cd_meter_rec.SVC_ST_NAME, cd_meter_rec.SVC_ST_NAME2, cd_meter_rec.SVC_CITY,
                    cd_meter_rec.SVC_STATE, cd_meter_rec.SVC_ZIP, cd_meter_rec.CUST_TYP, cd_meter_rec.RATE_SCHED,
                    cd_meter_rec.GLOBAL_ID, cd_meter_rec.SM_FLG, cd_meter_rec.METER_NUMBER );
               END;
            END IF;
            IF v_TRANS_TYPE='U' THEN -- cd_meter_rec.trans_type='U'
               IF cd_meter_rec.SERVICE_POINT_ID  IS NOT NULL THEN
                  BEGIN
                -- Find Duplicate SERVICE POINT ID
                     v_Rec_Count:=0;
                     SELECT Count(1)
                       INTO v_Rec_Count
                       FROM METER
                      WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID and GLOBAL_ID<>cd_meter_rec.GLOBAL_ID;
                     IF ( v_Rec_Count > 0 ) THEN
                        v_Rec_Count:=0;
                        ErrorCode:='CD-MTR-08';
                        SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                        ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                        LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                        to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                        continue;
                     END IF;
                  EXCEPTION
                  when no_data_found then
                  -- As expected. So do nothing. The record is good.
                     NULL;
                  when others then
                     ErrorCode := SQLCODE;
                     ErrorMsg  := SUBSTR(SQLERRM,1,200);
                     LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                     continue;
                  END;
               END IF;

               UPDATE METER
                  SET TRF_ID                 = transformer_id,
                      SERVICE_POINT_ID         = cd_meter_rec.SERVICE_POINT_ID,
                      UNQSPID                  = cd_meter_rec.UNQSPID,
                      REV_ACCT_CD              = cd_meter_rec.REV_ACCT_CD,
                      SVC_ST_NUM               = substr(cd_meter_rec.SVC_ST_NUM,1,12),
                      SVC_ST_NAME              = cd_meter_rec.SVC_ST_NAME,
                      SVC_ST_NAME2             = cd_meter_rec.SVC_ST_NAME2,
                      SVC_CITY                 = cd_meter_rec.SVC_CITY,
                      SVC_STATE                = cd_meter_rec.SVC_STATE,
                      SVC_ZIP                  = cd_meter_rec.SVC_ZIP,
                      CUST_TYP                 = cd_meter_rec.CUST_TYP,
                      RATE_SCHED               = cd_meter_rec.RATE_SCHED,
                      SM_FLG                   = cd_meter_rec.SM_FLG,
                      METER_NUMBER             = cd_meter_rec.METER_NUMBER
                WHERE GLOBAL_ID            = cd_meter_rec.GLOBAL_ID;

            elsif v_TRANS_TYPE='R' THEN -- cd_meter_rec.trans_type='R'
               IF cd_meter_rec.SERVICE_POINT_ID  IS NOT NULL THEN
                  BEGIN
                   -- Find Duplicate SERVICE POINT ID
                     v_Rec_Count:=0;
                     SELECT Count(1)
                       INTO v_Rec_Count
                       FROM METER
                      WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID and GLOBAL_ID<>cd_meter_rec.OLD_GLOBAL_ID;
                     IF ( v_Rec_Count > 0 ) THEN
                        v_Rec_Count:=0;
                        ErrorCode:='CD-MTR-08';
                        SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=ErrorCode;
                        ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                        LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,
                        to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                        continue;
                     END IF;
                  EXCEPTION
                  when no_data_found then
                  -- As expected. So do nothing. The record is good.
                     NULL;
                  when others then
                     ErrorCode := SQLCODE;
                     ErrorMsg  := SUBSTR(SQLERRM,1,200);
                     LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                     continue;
                  END;
               END IF;

               UPDATE METER
                  SET TRF_ID               = transformer_id,
                      SERVICE_POINT_ID     = cd_meter_rec.SERVICE_POINT_ID,
                      UNQSPID              = cd_meter_rec.UNQSPID,
                      REV_ACCT_CD          = cd_meter_rec.REV_ACCT_CD,
                      SVC_ST_NUM           = substr(cd_meter_rec.SVC_ST_NUM,1,12),
                      SVC_ST_NAME          = cd_meter_rec.SVC_ST_NAME,
                      SVC_ST_NAME2         = cd_meter_rec.SVC_ST_NAME2,
                      SVC_CITY             = cd_meter_rec.SVC_CITY,
                      SVC_STATE            = cd_meter_rec.SVC_STATE,
                      SVC_ZIP              = cd_meter_rec.SVC_ZIP,
                      CUST_TYP             = cd_meter_rec.CUST_TYP,
                      RATE_SCHED           = cd_meter_rec.RATE_SCHED,
                      SM_FLG               = cd_meter_rec.SM_FLG,
                      METER_NUMBER         = cd_meter_rec.METER_NUMBER,
                      GLOBAL_ID            = cd_meter_rec.GLOBAL_ID
                WHERE GLOBAL_ID            = cd_meter_rec.OLD_GLOBAL_ID;
               Old_Service_Point_Id := null;
               begin
                  select service_point_id into Old_Service_Point_Id  from meter where GLOBAL_ID = cd_meter_rec.old_global_id;
               exception
               when no_data_found then
                  Old_Service_Point_Id := null;
               end;
               update SP_PEAK_HIST S set S.SERVICE_POINT_ID=cd_meter_rec.Service_Point_Id where S.SERVICE_POINT_ID = Old_Service_Point_Id;
            elsif v_TRANS_TYPE='D' THEN -- cd_meter_rec.trans_type='D'
                UPDATE METER SET REC_STATUS = 'D' WHERE GLOBAL_ID=cd_meter_rec.GLOBAL_ID;
                UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
            ELSE
               NULL;
            END IF;

            IF SQL%ROWCOUNT>0 THEN
               UPDATE CD_METER M SET M.PROC_FLG = 'P' WHERE M.ID=cd_meter_rec.ID;
            END IF;

      EXCEPTION
      WHEN OTHERS THEN
          ErrorCode := SQLCODE;
          ErrorMsg  := SUBSTR(SQLERRM,1,200);
          LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(nvl(cd_meter_rec.trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
          continue;
          END;
      END LOOP;
      COMMIT;
   END;
END METER_MGMT;
PROCEDURE LOG_ERRORS(
    GLOBALID  VARCHAR2,
    ERRORCODE VARCHAR2,
    ERRORMSG  VARCHAR2,
    TRANSDATE TIMESTAMP,
    TRANSTYPE CHAR,
    APPTYPE   CHAR,
    PROCEDUREFOR VARCHAR2,ColumnId Number)
AS
BEGIN
   INSERT
   INTO CD_ERRORS_TEMP
    ( GLOBAL_ID, ERROR_CODE, ERROR_MSG, TRANS_DATE, TRANS_TYPE, CREATE_DTM, APP_TYPE, PROC_FOR, CD_TABLE_ID )
   VALUES
    ( GLOBALID, ErrorCode, ErrorMsg, TRANSDATE, TRANSTYPE, CURRENT_TIMESTAMP, AppType, ProcedureFor, ColumnId );
END;

FUNCTION TLM_CD_CHECK_ISDUP (
   table_name varchar2,
   field_name varchar2,
   field_value varchar2)
RETURN varchar2 AS
   v_Rec_Count number;
   v_stmt varchar2(2000);
   v_retval varchar2(500) := 'FALSE';
BEGIN
   v_Rec_Count := 0;
   -- SQL for for duplicate data
   v_stmt := 'SELECT COUNT(1) FROM ' || table_name || ' WHERE ' || field_name || ' = ''' || field_value || '''';
   --DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
   execute immediate v_stmt into v_Rec_Count;
   --DBMS_OUTPUT.PUT_LINE('DEBUG - v_Rec_Count = ' || v_Rec_Count);
   IF v_Rec_Count > 0 THEN
      v_retval := 'TRUE';
   ELSE
      v_retval := 'FALSE';
   END IF;
   RETURN v_retval;
END TLM_CD_CHECK_ISDUP;

FUNCTION TLM_CD_CHECK_ISNULL (
   val varchar2,
   rec_id number,
   global_id varchar2,
   trans_type varchar2,
   trans_date date,
   app_type varchar2,
   procedure_for varchar2,
   error_code varchar2)
RETURN varchar2 AS
   ErrorMsg varchar2(1000) := '';
   v_retval varchar2(500) := 'FALSE';
BEGIN
   IF trans_type<>'D' THEN
      IF (val IS NULL) OR (TRIM(TO_CHAR(val)) = '') THEN
         SELECT DESC_LONG INTO ErrorMsg FROM CODE_LOOKUP WHERE CODE_TYP='ERROR_CODE_CD' AND CODE=error_code;
         LOG_ERRORS(global_id,error_code,ErrorMsg,to_timestamp(nvl(trans_date,sysdate),'YYYY/MM/DD HH:MI:SS.FF AM'),
         trans_type,app_type,procedure_for,rec_id);
         v_retval := 'TRUE';
      ELSE
         v_retval := 'FALSE';
      END IF;
   END IF;
   RETURN v_retval;
END TLM_CD_CHECK_ISNULL;

FUNCTION TLM_CD_HAS_INSERT_RECORD(
   table_name varchar2,
   global_id varchar2,
   create_dt date)
RETURN varchar2 AS
   v_retval varchar2(500) := 'FALSE';
   v_stmt varchar2(2000);
BEGIN
   v_stmt := 'UPDATE ' || table_name || ' SET PROC_FLG=''P''
              WHERE global_Id = '''|| global_id ||'''
              AND CREATE_DTM < to_date(to_char(''' || create_dt || ''', ''DD-MON-YY''), ''dd-MON-YY'')' || '
              AND trans_type = ''I''';

  --DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
   execute immediate v_stmt;

   IF SQL%ROWCOUNT > 0 THEN    -- UPSERT
      v_retval := 'TRUE';
   ELSE
      v_retval := 'FALSE';
   END IF;
   RETURN v_retval;
END TLM_CD_HAS_INSERT_RECORD;

END TLM_CD_MGMT_MONTHLY_TEST;

/
