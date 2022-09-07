Prompt drop Package Body TLM_CD_MGMT_MONTHLY;
DROP PACKAGE BODY EDTLM.TLM_CD_MGMT_MONTHLY
/

Prompt Package Body TLM_CD_MGMT_MONTHLY;
--
-- TLM_CD_MGMT_MONTHLY  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY EDTLM."TLM_CD_MGMT_MONTHLY"
AS
  AppType              CHAR(1);
  ProcedureFor         VARCHAR2(20);
  NoOfRows             NUMBER;
  Old_Service_Point_Id VARCHAR2(10);
  FoundGlobalId        VARCHAR2(38);
PROCEDURE TRANSFORMER_MGMT
  (
    FromDate DATE,
    ToDate DATE,
    ErrorMsg OUT VARCHAR2,
    ErrorCode OUT VARCHAR2 )
AS
  v_Rec_Count  NUMBER;
  v_TRANS_TYPE VARCHAR2(1);
  v_count      NUMBER;
BEGIN
  ErrorMsg     := '';
  ErrorCode    :='';
  AppType      :='M';
  PROCEDUREFOR := 'TRF';
  v_Rec_Count  :=0;
  v_TRANS_TYPE :='';
  /* get all rows from change detection stage table where transaction types are update,replace or delete */

  DECLARE
    CURSOR c_cd_transformer
    IS
      SELECT *
      FROM cd_transformer
      WHERE TRANS_TYPE IN ('I','D','U','R')
      AND PROC_FLG     IS NULL
      AND GLOBAL_ID IS NOT NULL
      AND CGC_ID IS NOT NULL

        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
      ORDER BY Id;
  BEGIN

  -- TLM Changeset#1 Change Trans_date format from YYYY/MM/DD HH:MI:SS.FF AM to YYYY-MM-DD HH24:MI:SS throughout
    FOR cd_trf_rec IN c_cd_transformer
    LOOP
      BEGIN
        v_TRANS_TYPE := cd_trf_rec.trans_type;
        dbms_output.put_line('START -- Processing ID: ' || cd_trf_rec.ID);
        DBMS_OUTPUT.PUT_LINE('TRANS_TYPE: ' || cd_trf_rec.trans_type);
        DBMS_OUTPUT.PUT_LINE('GLOBAL_ID: ' || cd_trf_rec.global_id);
        DBMS_OUTPUT.PUT_LINE('CGC_ID: ' || cd_trf_rec.CGC_ID);
        IF TLM_CD_CHECK_ISNULL (cd_trf_rec.GLOBAL_ID, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-02') = 'TRUE' THEN
          dbms_output.put_line('GLOBAL_ID is null. Skipping the record.');
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_rec.CGC_ID, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-03') = 'TRUE' THEN
          dbms_output.put_line('CGC_ID is null. Skipping the record.');
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_rec.INSTALLATION_TYP, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-04') = 'TRUE' THEN
          -- TLM Changeset#2 - set INSTALLATION_TYP = OH if INSTALLATION_TYP = NULL and remove CONTINUE statement
          cd_trf_rec.INSTALLATION_TYP := 'OH';
          --CONTINUE;
          dbms_output.put_line('INSTALLATION_TYP is NULL. Setting INSTALLATION_TYP to OH');
        ELSIF LENGTH(trim(cd_trf_rec.COAST_INTERIOR_FLG)) > 1 THEN --- change for new CCB interface
          ErrorCode                                      :='CD-TRF-07';
          SELECT DESC_LONG
          INTO ErrorMsg
          FROM CODE_LOOKUP
          WHERE CODE_TYP='ERROR_CODE_CD'
          AND CODE      =ErrorCode;
          dbms_output.put_line('COAST_INTERIOR_FLAG length >1. Skipping the record.');
          DBMS_OUTPUT.PUT_LINE('Error code ' || ErrorCode || ' Error Message ' || ErrorMsg );
          LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
          CONTINUE;
        END IF;
        IF TLM_TRF_ISDUP(cd_trf_rec.CGC_ID, cd_trf_rec.global_id) = 'TRUE' THEN
        V_TRANS_TYPE := 'U';
        ELSE
        V_TRANS_TYPE := 'I';
        END IF;
        IF cd_trf_rec.trans_type='U' OR cd_trf_rec.trans_type='D' THEN
          BEGIN
            -- TLM CHANGESET#3A - Check condition for REC_STATUS. Update record when REC_STATUS <> D. Otherwise Insert.
            SELECT GLOBAL_ID
            INTO FoundGlobalId
            FROM Transformer
            WHERE global_Id = cd_trf_rec.global_id
            AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            dbms_output.put_line('Global_id found in transformer. can proceed for update');
          EXCEPTION
          WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('global_id not found in transformer. ');
            IF cd_trf_rec.trans_type ='U' THEN --Update
              BEGIN
				 -- TLM CHANGESET#3B - Check condition for REC_STATUS.
                SELECT GLOBAL_ID
                INTO FoundGlobalId
                FROM Transformer
                WHERE cgc_id = cd_trf_rec.cgc_id
                AND (REC_STATUS <> 'D' OR REC_STATUS is null)  ;
                -- TLM Changeset#4 - Skip the record when GLOBAL_ID and CGC_ID mismatch. Add continue statement and log as error
                dbms_output.put_line('CGC_ID found in transformer but not for the same global_id. Skip this record and update cgc_id with seq no.');
                LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                 -- TLM Changeset#5A Move duplicate transformer CGC_ID records to TRANSFORMER_DEL_DUP table and update CGC_ID with a new seq
                INSERT INTO transformer_del_dup select * from transformer where  cgc_id = cd_trf_rec.cgc_id and global_id = cd_trf_rec.global_id;
                UPDATE transformer set cgc_id = deleted_cgc_id_seq.nextval where cgc_id = cd_trf_rec.cgc_id and global_id = cd_trf_rec.global_id;
                V_TRANS_TYPE := 'I';
              EXCEPTION
              WHEN NO_DATA_FOUND THEN
              -- TLM Changeset#5 - Convert U to I, if cgc_id and global_id are not found in TLM base table. Remove check for TLM_CD_HAS_INSERT_RECORD
                DBMS_OUTPUT.put_line('Converting U to I');
                V_TRANS_TYPE := 'I';
                /*IF TLM_CD_HAS_INSERT_RECORD('CD_TRANSFORMER_TEMP', cd_trf_rec.global_id, cd_trf_rec.create_dtm) = 'TRUE' THEN
                -- there was a failed insert record in the CD table. So UPSERT.
                DBMS_OUTPUT.PUT_LINE('Setting TRANS_TYPE = I');
                v_TRANS_TYPE := 'I';
                ELSE
                ErrorCode:='CD-TRF-01';
                DBMS_OUTPUT.PUT_LINE('ERROR: ' || ErrorCode);
                SELECT DESC_LONG
                INTO ErrorMsg
                FROM CODE_LOOKUP
                WHERE CODE_TYP='ERROR_CODE_CD'
                AND CODE      =ErrorCode;
                dbms_output.put_line('No insert record found - ' || errorcode || ' ' || errormsg);
                LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,to_timestamp(NVL(cd_trf_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                v_trans_type := 'I';
                --CONTINUE;
                END IF;*/
              END;
            ELSE -- Delete
              -- Update the PROC_FLG flag.
              DBMS_OUTPUT.PUT_LINE('No record found to delete from Transformer table');
              DBMS_OUTPUT.PUT_LINE('Updating CD_TRANSFORMER ID - ' || cd_trf_rec.ID || 'PROC_FLG = P');
              UPDATE CD_TRANSFORMER T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
              -- TLM Changeset#6A Commit changes after successful update
              Commit;
              CONTINUE;
            END IF;
          END;
        END IF;

         IF cd_trf_rec.trans_type='R' THEN
          BEGIN
          -- TLM Changeset#3C - Check condition for REC_STATUS. Update record when REC_STATUS <> D. Otherwise Insert.
            SELECT global_id
            INTO FoundGlobalId
            FROM Transformer
            WHERE global_Id = cd_trf_rec.global_id
            AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            DBMS_OUTPUT.PUT_LINE('FoundGlobalId ' || FoundGlobalId);
          v_trans_type := 'U';
          END;
        END IF;

        IF cd_trf_rec.trans_type='R' THEN
          BEGIN
          -- TLM Changeset#3C - Check condition for REC_STATUS. Update record when REC_STATUS <> D. Otherwise Insert.
            SELECT global_id
            INTO FoundGlobalId
            FROM Transformer
            WHERE global_Id = cd_trf_rec.old_global_id
            AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            DBMS_OUTPUT.PUT_LINE('FoundOldGlobalId ' || FoundGlobalId);
          EXCEPTION
          WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('OLD_GLOBAL_ID not found in transformer table.');
            SELECT global_id INTO FoundGlobalId FROM Transformer WHERE global_Id = cd_trf_rec.global_id AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            DBMS_OUTPUT.PUT_LINE('FoundGlobalId ' || FoundGlobalId);
            v_trans_type := 'U';
          END;
        END IF;

        IF v_TRANS_TYPE='I' THEN -- cd_trf_rec.trans_type='I'
          IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'global_id', cd_trf_rec.global_id) = 'TRUE' THEN
            ErrorCode                                                             :='CD-TRF-05';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            dbms_output.put_line('Duplicate globalid in transformer.');
            DBMS_OUTPUT.PUT_LINE('Error: Globalid ' ||cd_trf_rec.global_id || 'Error code ' || ErrorCode || 'Error Message ' || ErrorMsg );
            LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
            CONTINUE;
          END IF;
          IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'CGC_ID', cd_trf_rec.CGC_ID) = 'TRUE' THEN
            ErrorCode                                                       :='CD-TRF-06';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            dbms_output.put_line('Duplicate cgc_id in transformer');
            ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
            DBMS_OUTPUT.PUT_LINE('Error: Globalid ' ||cd_trf_rec.global_id || 'Error code ' || ErrorCode || 'Error Message ' || ErrorMsg );
            LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
            CONTINUE;
          END IF;
        -- TLM Changeset#8 - Adding separate if conditions for validation block and insert statement blocks
        END IF;
        IF v_trans_type = 'I' THEN
          DBMS_OUTPUT.PUT_LINE('Inserting into Transformer');
          INSERT
          INTO TRANSFORMER
            (
              CGC_ID,
              GLOBAL_ID,
              COAST_INTERIOR_FLG,
              CLIMATE_ZONE_CD,
              INSTALLATION_TYP,
              LOWSIDE_VOLTAGE,
              OPERATING_VOLTAGE,
              CIRCUIT_ID,
              VAULT,
              REGION
            )
            VALUES
            (
              cd_trf_rec.CGC_ID,
              cd_trf_rec.GLOBAL_ID,
              cd_trf_rec.COAST_INTERIOR_FLG,--SUBSTR(TO_CHAR(cd_trf_rec.COAST_INTERIOR_FLG),1,1),
              cd_trf_rec.CLIMATE_ZONE_CD,
              --cd_trf_rec.PHASE_CD,
              cd_trf_rec.INSTALLATION_TYP,
              cd_trf_rec.LOWSIDE_VOLTAGE,
              cd_trf_rec.OPERATING_VOLTAGE,
              cd_trf_rec.CIRCUIT_ID,
              cd_trf_rec.VAULT,
              NULL
            );

          DBMS_OUTPUT.PUT_LINE
          (
            'Insert Completed'
          )
          ;
        END IF;
        IF v_TRANS_TYPE='U' THEN -- cd_trf_rec.trans_type='U'
          v_Rec_Count := 0;
          DBMS_OUTPUT.PUT_LINE
          (
            'v_TRANS_TYPE=U'
          )
          ;
          -- TLM CHANGESET#3D - Check condition for REC_STATUS.
          SELECT COUNT(1)
          INTO v_Rec_Count
          FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
          WHERE CGC_ID   = cd_trf_rec.CGC_ID
          AND global_Id <> cd_trf_rec.global_id
          AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
          IF v_Rec_Count > 0 THEN
            dbms_output.put_line('CGC_ID exists in transformer but with different global_id');
            ErrorCode :='CD-TRF-06';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            DBMS_OUTPUT.PUT_LINE('Duplicate transformer cgc_id');
            ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
            DBMS_OUTPUT.PUT_LINE('Error: Globalid ' ||cd_trf_rec.global_id || 'Error code ' || ErrorCode || 'Error Message ' || ErrorMsg );
            LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
            CONTINUE;
          END IF;
          DBMS_OUTPUT.PUT_LINE('Updating transformer');
          UPDATE TRANSFORMER
          SET CGC_ID           = cd_trf_rec.CGC_ID,
            COAST_INTERIOR_FLG = cd_trf_rec.COAST_INTERIOR_FLG, --SUBSTR(TO_CHAR(cd_trf_rec.COAST_INTERIOR_FLG),1,1), --- change for new CCB interface
            CLIMATE_ZONE_CD    = cd_trf_rec.CLIMATE_ZONE_CD,
            --         PHASE_CD             = cd_trf_rec.PHASE_CD,
            INSTALLATION_TYP  = cd_trf_rec.INSTALLATION_TYP,
            LOWSIDE_VOLTAGE   = cd_trf_rec.LOWSIDE_VOLTAGE,
            OPERATING_VOLTAGE = cd_trf_rec.OPERATING_VOLTAGE,
            CIRCUIT_ID        = cd_trf_rec.CIRCUIT_ID ,
            VAULT             = cd_trf_rec.VAULT
          WHERE GLOBAL_ID     = cd_trf_rec.GLOBAL_ID;
          DBMS_OUTPUT.PUT_LINE('update completed');
        elsif v_TRANS_TYPE='R' THEN -- cd_trf_rec.trans_type='R'
          v_Rec_Count    := 0;
          -- TLM CHANGESET#3E - Check condition for REC_STATUS.
          SELECT COUNT(1)
          INTO v_Rec_Count
          FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
          WHERE CGC_ID   = cd_trf_rec.CGC_ID
          AND global_Id <> cd_trf_rec.Old_global_id
          AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
          IF v_Rec_Count > 0 THEN
            ErrorCode   :='CD-TRF-06';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            dbms_output.put_line('Duplicate cgc_id in transformer');
            ErrorMsg := ErrorMsg || ' :' ||cd_trf_rec.CGC_ID;
            DBMS_OUTPUT.PUT_LINE('Error: Globalid ' ||cd_trf_rec.global_id || 'Error code ' || ErrorCode || 'Error Message ' || ErrorMsg );
            LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
            CONTINUE;
          END IF;

          DBMS_OUTPUT.PUT_LINE('Updating transformer');
          UPDATE TRANSFORMER
          SET CGC_ID           = cd_trf_rec.CGC_ID,
            GLOBAL_ID          = cd_trf_rec.GLOBAL_ID,
            COAST_INTERIOR_FLG = cd_trf_rec.COAST_INTERIOR_FLG, --SUBSTR(TO_CHAR(cd_trf_rec.COAST_INTERIOR_FLG),1,1), --- change for new CCB interface ,
            CLIMATE_ZONE_CD    = cd_trf_rec.CLIMATE_ZONE_CD,
            --PHASE_CD               = cd_trf_rec.PHASE_CD,
            INSTALLATION_TYP  = cd_trf_rec.INSTALLATION_TYP,
            LOWSIDE_VOLTAGE   = cd_trf_rec.LOWSIDE_VOLTAGE,
            OPERATING_VOLTAGE = cd_trf_rec.OPERATING_VOLTAGE,
            CIRCUIT_ID        = cd_trf_rec.CIRCUIT_ID,
            VAULT             = cd_trf_rec.VAULT
          WHERE GLOBAL_ID     = cd_trf_rec.OLD_GLOBAL_ID;

          DBMS_OUTPUT.PUT_LINE('update completed');
          V_Rec_count := 0;
          SELECT COUNT(1)
          INTO v_Rec_Count
          FROM EDSETT.SM_SPECIAL_LOAD -- Check for the Special Load record is exists in setting schema
          WHERE Ref_global_Id = cd_trf_rec.OLD_GLOBAL_ID;
          IF v_Rec_Count     <> 0 THEN
            DBMS_OUTPUT.PUT_LINE('Updating globalid in EDSETT.SM_SPECIAL_LOAD');
            UPDATE EDSETT.SM_SPECIAL_LOAD
            SET REF_GLOBAL_ID  = cd_trf_rec.GLOBAL_ID
            WHERE REF_GLOBAL_ID=cd_trf_rec.OLD_GLOBAL_ID;

            DBMS_OUTPUT.PUT_LINE('completed');
          END IF ;
        elsif v_TRANS_TYPE='D' THEN -- cd_trf_rec.trans_type='D'
          DBMS_OUTPUT.PUT_LINE('v_TRANS_TYPE=D');
          UPDATE TRANSFORMER SET REC_STATUS = 'D' WHERE GLOBAL_ID=cd_trf_rec.GLOBAL_ID;
          UPDATE cd_transformer T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
        ELSE
          NULL;
        END IF;
        --SELECT COUNT(*) into NoOfRows FROM TRANSFORMER T WHERE T.GLOBAL_ID = CD_TRF_REC.GLOBAL_ID;
        IF SQL%ROWCOUNT>0 THEN
          DBMS_OUTPUT.PUT_LINE('UPDATE CD_TRANSFORMER T SET PROC_FLG=P WHERE T.ID= ' || cd_trf_rec.ID);
          UPDATE cd_transformer T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;

          DBMS_OUTPUT.PUT_LINE('done');
        END IF;
      EXCEPTION
      WHEN OTHERS THEN
        ErrorCode := SQLCODE;
        ErrorMsg  := SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE('Error: Globalid ' ||cd_trf_rec.global_id || 'Error code ' || ErrorCode || 'Error Message ' || ErrorMsg );
        LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
        DBMS_OUTPUT.PUT_LINE('ErrorMsg: ' || ErrorMsg);
        CONTINUE;
      END;
      -- TLM Changeset#6 Commit changes after successful update
      COMMIT;
      DBMS_OUTPUT.PUT_LINE('COMMIT DONE for global_id = ' || cd_trf_rec.global_id);
    END LOOP;

  END;
END TRANSFORMER_MGMT;
PROCEDURE TRANSFORMER_BANK_MGMT
  (
    FromDate DATE,
    ToDate DATE,
    ErrorMsg OUT VARCHAR2,
    ErrorCode OUT VARCHAR2 )
AS
  transformer_id NUMBER;
  v_Rec_Count    NUMBER;
  v_TRANS_TYPE   VARCHAR2(1);
  v_count        NUMBER;
BEGIN
  ErrorMsg    := '';
  ErrorCode   :='';
  AppType     :='M';
  ProcedureFor:='TRF-BANK';
  v_Rec_Count :=0;
  v_TRANS_TYPE:='';
  /* get all rows from change detection stage table where transaction types are update,replace or delete */
  DECLARE
    CURSOR c_cd_transformer_bank
    IS
      SELECT *
      FROM cd_transformer_bank
      WHERE TRANS_TYPE IN ('I','D','U','R')
      AND PROC_FLG     IS NULL
        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
      ORDER BY Id;
  BEGIN
  -- TLM Changeset#1 Change Trans_date format from YYYY/MM/DD HH:MI:SS.FF AM to YYYY-MM-DD HH24:MI:SS throughout
    FOR cd_trf_bank_rec IN c_cd_transformer_bank
    LOOP
      BEGIN
        v_TRANS_TYPE := cd_trf_bank_rec.trans_type;
        DBMS_OUTPUT.PUT_LINE('START -- TRANS_TYPE = ' || v_trans_type);
        DBMS_OUTPUT.PUT_LINE('Transformer bank GLOBAL_ID = ' || cd_trf_bank_rec.GLOBAL_ID);
        DBMS_OUTPUT.PUT_LINE('Transformer GLOBAL_ID = ' || cd_trf_bank_rec.TRF_GLOBAL_ID);
        DBMS_OUTPUT.PUT_LINE('BANK_CD = ' || cd_trf_bank_rec.BANK_CD);
        DBMS_OUTPUT.PUT_LINE('NP_KVA = ' || cd_trf_bank_rec.NP_KVA);
        DBMS_OUTPUT.PUT_LINE('PHASE_CD = ' || cd_trf_bank_rec.PHASE_CD);
        IF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.TRF_GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-01') = 'TRUE' THEN
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-04') = 'TRUE' THEN
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.BANK_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-05') = 'TRUE' THEN
          DBMS_OUTPUT.PUT_LINE('BANK_CD is NULL. Skipping this record.');
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.NP_KVA, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-06') = 'TRUE' THEN
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.PHASE_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-07') = 'TRUE' THEN
          -- TLM Changeset#2 - If PHASE_CD is NULL. Set PHASE_CD as 0. Remove CONTINUE statement
          cd_trf_bank_rec.PHASE_CD := 0;
          DBMS_OUTPUT.PUT_LINE('Donot skip thiis record. Set PHASE_CD as 0');
          --CONTINUE;
        ELSIF (( LENGTH(TRIM(TRANSLATE(cd_trf_bank_rec.BANK_CD, ' +-.0123456789', ' '))) IS NOT NULL ) OR To_Number(cd_trf_bank_rec.BANK_CD) > 9 ) THEN --- change for new CCB interface
          transformer_id                                                                 :=0;
          ErrorCode                                                                      :='CD-BNK-10';
          SELECT DESC_LONG
          INTO ErrorMsg
          FROM CODE_LOOKUP
          WHERE CODE_TYP='ERROR_CODE_CD'
          AND CODE      =ErrorCode;

          DBMS_OUTPUT.PUT_LINE('ERROR Bank code translation error: '|| ErrorCode || ErrorMsg);
          ErrorMsg := ErrorMsg || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID;
          LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
          CONTINUE;
        ELSE
          BEGIN
           -- TLM CHANGESET#3A - Check condition for REC_STATUS.
            SELECT ID
            INTO transformer_id
            FROM TRANSFORMER
            WHERE GLOBAL_ID =cd_trf_bank_rec.TRF_GLOBAL_ID
            AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            DBMS_OUTPUT.PUT_LINE('TRF_GLOBAL_ID Found in TRANSFORMER');
          EXCEPTION
          WHEN no_data_found THEN
            IF cd_trf_bank_rec.trans_type <> 'D' THEN
              transformer_id              :=0;
              ErrorCode                   :='CD-BNK-02';
              SELECT DESC_LONG
              INTO ErrorMsg
              FROM CODE_LOOKUP
              WHERE CODE_TYP='ERROR_CODE_CD'
              AND CODE      =ErrorCode;
              DBMS_OUTPUT.PUT_LINE('TRF_GLOBAL_ID not found in Transformer');
              ErrorMsg := ErrorMsg || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID;
              LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
              CONTINUE;
            END IF;
          END;
        END IF;
        IF cd_trf_bank_rec.trans_type='U' OR cd_trf_bank_rec.trans_type='D' THEN
          BEGIN
            -- TLM Changeset#4B - Update Transformer_bank only if rec_status <> D. Otherwise insert or log error
            SELECT Global_Id
            INTO FoundGlobalId
            FROM Transformer_Bank
            WHERE Global_Id = cd_trf_bank_rec.Global_Id
            AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            dbms_output.put_line('global_id found in transformer_bank');
          EXCEPTION
          WHEN NO_DATA_FOUND THEN
            dbms_output.put_line('global_id not found in transformer_bank');
            IF cd_trf_bank_rec.trans_type ='U' THEN -- Update
              -- TLM Changeset#5 - Convert U to I if global_id doesnot exist in TRANSFORMER_BANK. (Check for TLM_CD_HAS_INSERT_RECORD removed)
              DBMS_OUTPUT.PUT_LINE('Converting U to I');
              v_trans_type := 'I';
              /*  IF TLM_CD_HAS_INSERT_RECORD('CD_TRANSFORMER_BANK_TEMP', cd_trf_bank_rec.global_id, cd_trf_bank_rec.create_dtm) = 'TRUE' THEN
              -- there was a failed insert record in the CD table.
              dbms_output.put_line('Has insert record in cd table. Setting U to I');
              v_TRANS_TYPE := 'I'; --UPSERT
              ELSE
              ErrorCode:='CD-BNK-03';
              SELECT DESC_LONG
              INTO ErrorMsg
              FROM CODE_LOOKUP
              WHERE CODE_TYP='ERROR_CODE_CD'
              AND CODE      =ErrorCode;
              dbms_output.put_line('No insert record found in cd table. Skipping the record');
              LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
              CONTINUE;
              END IF; */
            ELSE -- Delete
              -- there was a failed insert in the CD table. So just update the flag.
              UPDATE CD_TRANSFORMER_BANK T
              SET PROC_FLG='P'
              WHERE T.ID  = cd_trf_bank_rec.ID;
              -- TLM Changeset#6A Commit changes after successful update
              COMMIT;
              dbms_output.put_line('Simply updating proc flag to P since D record');
              CONTINUE;
            END IF;
          END;
        END IF;
        IF cd_trf_bank_rec.trans_type='R' THEN
          BEGIN
          -- TLM CHANGESET#4A - Check condition for REC_STATUS.
            SELECT Global_Id
            INTO FoundGlobalId
            FROM Transformer_Bank
            WHERE Global_Id = cd_trf_bank_rec.Old_Global_Id
            AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            dbms_output.put_line('OLD_GLOBAL_ID found in TRANSFORMER_BANK table');
          EXCEPTION
          WHEN no_data_found THEN
            --ErrorCode:='CD-BNK-03';
            --SELECT DESC_LONG
            --INTO ErrorMsg
            --FROM CODE_LOOKUP
            --WHERE CODE_TYP='ERROR_CODE_CD'
            --AND CODE      =ErrorCode;
            dbms_output.put_line('No OLD_GLOBAL_ID found in TRANSFORMER_BANK. Converting this record to U. Remove CONTINUE');
            -- TLM Changeset#7 Convert R to I if OLD_GLOBAL_ID is not found in TRANSFORMER_BANK. Remove CONTINUE
            dbms_output.put_line('Converting R to I');
            v_trans_type := 'I';
            --    ErrorMsg:='No records exists in Transformer Bank table';
			-- LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            -- CONTINUE;
          END;
        END IF;
        IF v_TRANS_TYPE ='I' THEN --cd_trf_bank_rec.trans_type='I'
          IF TLM_CD_CHECK_ISDUP ('TRANSFORMER_BANK', 'global_id', cd_trf_bank_rec.global_id) = 'TRUE' THEN
            --ErrorCode                                                                       :='CD-BNK-08';
            --SELECT DESC_LONG
            --INTO ErrorMsg
            --FROM CODE_LOOKUP
            --WHERE CODE_TYP='ERROR_CODE_CD'
            --AND CODE      =ErrorCode;
            -- TLM Changeset#8 Converting I to U if global_id already exists
            dbms_output.put_line('GLOBAL_ID already exists in Transformer_bank table.');
            dbms_output.put_line('Converting I to U');
            v_trans_type := 'U';
           -- LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
           -- CONTINUE;
          END IF;
          v_Rec_Count := 0;
          -- TLM CHANGESET#4 - Donot check this condition for duplicate transformer bank with TRF_ID/BANK_CD combination
        /*  SELECT COUNT(1)
          INTO v_Rec_Count
          FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
          WHERE TRF_ID   = transformer_id
          AND BANK_CD    = cd_trf_bank_rec.BANK_CD
          AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
          IF v_Rec_Count > 0 THEN
            ErrorCode   :='CD-BNK-09';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;

            dbms_output.put_line('Error : duplicate transformer bank with TRF_ID/BANK_CD combination');
            ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
            dbms_output.put_line(cd_trf_bank_rec.trans_date);
            LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            CONTINUE;
          END IF; */
          -- TLM Changeset#9 Adding separate if conditions for validation block and insert statement blocks
        END IF;
        IF v_trans_type = 'I' THEN
          dbms_output.put_line('INSERTING IN TRANSFORMER_BANK');
          INSERT
          INTO TRANSFORMER_BANK
            (
              TRF_ID,
              BANK_CD,
              NP_KVA,
              GLOBAL_ID,
              PHASE_CD,
              TRF_TYP
            )
            VALUES
            (
              transformer_id,
              to_number(cd_trf_bank_rec.BANK_CD),
              cd_trf_bank_rec.NP_KVA,
              cd_trf_bank_rec.GLOBAL_ID,
              cd_trf_bank_rec.PHASE_CD,
              cd_trf_bank_rec.TRF_TYP
            );
        END IF;
        IF v_TRANS_TYPE='U' THEN --cd_trf_bank_rec.trans_type='U'
          v_Rec_Count := 0;
          -- TLM CHANGESET#4 - Donot check this condition for duplicate transformer bank with TRF_ID/BANK_CD combination
         /* SELECT COUNT(1)
          INTO v_Rec_Count
          FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
          WHERE (TRF_ID  = transformer_id
          AND BANK_CD    = cd_trf_bank_rec.BANK_CD)
          AND global_Id <> cd_trf_bank_rec.global_id
          AND (REC_STATUS <> 'D' OR REC_STATUS is null)
          ;
          IF v_Rec_Count > 0 THEN
            ErrorCode   :='CD-BNK-09';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            dbms_output.put_line('duplicate transformer bank with TRF_ID/BANK_CD combination');
            ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
            LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            CONTINUE;
          END IF;
          */
          dbms_output.put_line('UPDATING TRANSFORMER_BANK');
          UPDATE TRANSFORMER_BANK
          SET TRF_ID      = transformer_Id,
            BANK_CD       = to_number(cd_trf_bank_rec.BANK_CD), --- change for new CCB interface
            NP_KVA        = cd_trf_bank_rec.NP_KVA,
            PHASE_CD      = cd_trf_bank_rec.PHASE_CD,
            TRF_TYP       = cd_trf_bank_rec.TRF_TYP
          WHERE GLOBAL_ID = cd_trf_bank_rec.GLOBAL_ID;
        ELSIF v_TRANS_TYPE='R' THEN -- cd_trf_bank_rec.trans_type='R'
          v_Rec_Count    := 0;
          -- TLM CHANGESET#4 - Donot check this condition for duplicate transformer bank with TRF_ID/BANK_CD combination
        /*  SELECT COUNT(1)
          INTO v_Rec_Count
          FROM TRANSFORMER_BANK -- Check for the duplicate transformer bank with TRF_ID/BANK_CD combination
          WHERE (TRF_ID  = transformer_id
          AND BANK_CD    = cd_trf_bank_rec.BANK_CD)
          AND global_Id  = cd_trf_bank_rec.global_id;
          IF v_Rec_Count > 0 THEN
            ErrorCode   :='CD-BNK-09';
            SELECT DESC_LONG
            INTO ErrorMsg
            FROM CODE_LOOKUP
            WHERE CODE_TYP='ERROR_CODE_CD'
            AND CODE      =ErrorCode;
            DBMS_OUTPUT.put_line('Duplicate TRF_ID/BANK_CD combination. Skipping');
            ErrorMsg := ErrorMsg || ' :' || transformer_id || '/' || cd_trf_bank_rec.BANK_CD;
            LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_trf_bank_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            CONTINUE;
          END IF; */
          dbms_output.put_line('UPDATING TRANSFORMER_BANK');
          UPDATE TRANSFORMER_BANK
          SET TRF_ID      = transformer_id,
            BANK_CD       = to_number(cd_trf_bank_rec.BANK_CD), --- change for new CCB interface
            NP_KVA        = cd_trf_bank_rec.NP_KVA,
            PHASE_CD      = cd_trf_bank_rec.PHASE_CD,
            GLOBAL_ID     = cd_trf_bank_rec.GLOBAL_ID
          WHERE GLOBAL_ID = cd_trf_bank_rec.OLD_GLOBAL_ID;
        ELSIF v_TRANS_TYPE='D' THEN -- cd_trf_bank_rec.trans_type='D'
          dbms_output.put_line('Setting REC_STATUS as D in TRANSFORMER_BANK');
          UPDATE TRANSFORMER_BANK
          SET REC_STATUS = 'D'
          WHERE GLOBAL_ID=cd_trf_bank_rec.GLOBAL_ID;

          dbms_output.put_line('Setting proc_flg as P in CD_TRANSFORMER_BANK');
          UPDATE CD_TRANSFORMER_BANK T
          SET PROC_FLG='P'
          WHERE T.ID  = cd_trf_bank_rec.ID;
        ELSE
          NULL;
        END IF;
        IF SQL%ROWCOUNT>0 THEN
          dbms_output.put_line('SQL % ROWCOUNT >0. Setting proc flag as P');
          UPDATE CD_TRANSFORMER_BANK B
          SET B.PROC_FLG='P'
          WHERE B.ID    =cd_trf_bank_rec.ID;
        END IF;
      EXCEPTION
      WHEN OTHERS THEN
        ErrorCode := SQLCODE;
        ErrorMsg  := SUBSTR(SQLERRM,1,200);
        dbms_output.put_line('Error ' || errormsg);
        LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
        CONTINUE;
      END;
      -- TLM Changeset#4 Commit changes after every record
      dbms_output.put_line('commit change for ' || cd_trf_bank_rec.global_id);
      COMMIT;
      dbms_output.put_line('commit successful' || cd_trf_bank_rec.global_id);
    END LOOP;

  END;
END TRANSFORMER_BANK_MGMT;
/* CHANGE DETECTION PROCEDURE FOR METER  */
PROCEDURE METER_MGMT
  (
    FromDate DATE,
    ToDate DATE,
    ErrorMsg OUT VARCHAR2,
    ErrorCode OUT VARCHAR2 )
AS
  transformer_id NUMBER;
  v_Rec_Count    NUMBER;
  v_TRANS_TYPE   VARCHAR2(1);
  v_count        NUMBER;
BEGIN
  ErrorMsg    := '';
  ErrorCode   :='';
  AppType     :='M';
  ProcedureFor:='METER';
  v_Rec_Count :=0;
  v_TRANS_TYPE:='';
  /* get all rows from change detection stage table where transaction types are update,replace or delete */
  DECLARE
    CURSOR c_cd_meter
    IS
      SELECT *
      FROM cd_meter
      WHERE TRANS_TYPE IN ('I','D','U','R')
      AND PROC_FLG     IS NULL
      AND trf_global_id is not null
      AND GLOBAL_ID IS NOT NULL
      AND SERVICE_POINT_ID IS NOT NULL
      --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
      ORDER BY Id;
  BEGIN
  -- TLM Changeset#1 Change Trans_date format from YYYY/MM/DD HH:MI:SS.FF AM to YYYY-MM-DD HH24:MI:SS throughout
    FOR cd_meter_rec IN c_cd_meter
    LOOP
      BEGIN
         DBMS_OUTPUT.PUT_LINE('START -- TRANS_DATE = ' || cd_meter_rec.trans_date);
        v_TRANS_TYPE:= cd_meter_rec.trans_type;
        dbms_output.put_line('v_trans_type = ' || cd_meter_rec.trans_type);
        dbms_output.put_line('TRF_GLOBAL_ID = ' || cd_meter_rec.TRF_GLOBAL_ID);
        dbms_output.put_line('GLOBAL_ID = ' || cd_meter_rec.GLOBAL_ID);
        dbms_output.put_line('SERVICE_POINT_ID = ' || cd_meter_rec.SERVICE_POINT_ID);
        dbms_output.put_line('TRANS_DATE = ' || cd_meter_rec.trans_date);
      /*  IF TLM_CD_CHECK_ISNULL (cd_meter_rec.TRF_GLOBAL_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-01') = 'TRUE' THEN
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_meter_rec.GLOBAL_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-05') = 'TRUE' THEN
          CONTINUE;
        ELSIF TLM_CD_CHECK_ISNULL (cd_meter_rec.SERVICE_POINT_ID, cd_meter_rec.Id, cd_meter_rec.global_id, cd_meter_rec.trans_type, cd_meter_rec.trans_date, AppType, ProcedureFor, 'CD-MTR-06') = 'TRUE' THEN
          CONTINUE;
        ELSE */
          BEGIN
          -- TLM CHANGESET#2A - Check condition for REC_STATUS. Update record when REC_STATUS <> D. Otherwise Insert.
            SELECT ID
            INTO transformer_id
            FROM TRANSFORMER
            WHERE GLOBAL_ID =cd_meter_rec.TRF_GLOBAL_ID
             AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            dbms_output.put_line('TRF_GLOBAL_ID found in transformer table');
          EXCEPTION
          WHEN no_data_found THEN
            dbms_output.put_line('TRF_GLOBAL_ID not found in transformer table. skipping');
            IF cd_meter_rec.trans_type <> 'D' THEN
              transformer_id           :=0;
              ErrorCode                :='CD-MTR-02';
              SELECT DESC_LONG
              INTO ErrorMsg
              FROM CODE_LOOKUP
              WHERE CODE_TYP='ERROR_CODE_CD'
              AND CODE      =ErrorCode;
              ErrorMsg:= ErrorMsg || '. TRF_GLOBAL_ID:' || cd_meter_rec.TRF_GLOBAL_ID;
              LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
              CONTINUE;
            END IF;
          END;
      /*  END IF; */
        IF cd_meter_rec.trans_type='U' OR cd_meter_rec.trans_type='D' THEN
          BEGIN
          -- TLM CHANGESET#2 - Check condition for REC_STATUS.
            SELECT Global_Id
            INTO FoundGlobalId
            FROM Meter
            WHERE Global_Id = cd_meter_rec.Global_Id
             AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            dbms_output.put_line('GLOBAL_ID found in meter table');
          EXCEPTION
          WHEN NO_DATA_FOUND THEN
            IF cd_meter_rec.trans_type ='U' THEN -- Update
             -- TLM Changeset#3 - Convert U to I if global_id doesnot exist in METER. (Check for TLM_CD_HAS_INSERT_RECORD removed)
             -- IF TLM_CD_HAS_INSERT_RECORD('CD_METER', cd_meter_rec.global_id, cd_meter_rec.create_dtm) = 'TRUE' THEN
                -- there was a failed insert record in the CD table.
                -- TLM Changeset#3A Move duplicate meter GLOBAL_ID records to METER_DEL_DUP table and set service_point_id from DELETED_SERVICE_POINT_ID_SEQ
                INSERT INTO meter_del_dup select * from meter where global_id = cd_meter_rec.global_id;
                UPDATE METER SET service_point_id = DELETED_SERVICE_POINT_ID_SEQ.nextval where global_id = cd_meter_rec.global_id;
                commit;
                dbms_output.put_line('Converting U to I');
                v_TRANS_TYPE := 'I'; --UPSERT
                -- TLM Changeset#4 - Comment Else Code block. Logic- When TRANS_TYPE = U and CD_METER has Insert record, convert U to I. Else do nothing.
                /* ELSE
                ErrorCode:='CD-MTR-03';
                -- TLM in this case the U record was never moving forward. Only in case cd_meter had a I record
                --U would be converted to I, in all other cases, it would skip this record and move on to the next record
                SELECT DESC_LONG
                INTO ErrorMsg
                FROM CODE_LOOKUP
                WHERE CODE_TYP='ERROR_CODE_CD'
                AND CODE      =ErrorCode;
                -- ErrorMsg:='No records exists in Meter table';
                dbms_output.put_line(ErrorMsg || ' or GLOBAL_ID not found in meter table');
                LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_meter_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                CONTINUE;
                */
            --  END IF;
            ELSE -- Delete
              UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
              -- TLM Changeset#5A Commit changes after successful update
              COMMIT;
              dbms_output.put_line('Delete Case. Simply setting PROC_FLG = P');
              CONTINUE;
            END IF;
          END;
        END IF;
        IF cd_meter_rec.trans_type='R' THEN
          BEGIN
          -- TLM CHANGESET#2A - Check condition for REC_STATUS.
            SELECT Global_Id
            INTO FoundGlobalId
            FROM Meter
            WHERE Global_Id = cd_meter_rec.Old_Global_Id
             AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            dbms_output.put_line('OLD_GLOBAL_ID found in meter table.');
          EXCEPTION
          WHEN no_data_found THEN
            --ErrorCode:='CD-MTR-03';
            --SELECT DESC_LONG
            --INTO ErrorMsg
            --FROM CODE_LOOKUP
            --WHERE CODE_TYP='ERROR_CODE_CD'
            --AND CODE      =ErrorCode;
            dbms_output.put_line('OLD_GLOBAL_ID not found in meter table.');
            -- TLM Changeset#6 OLD_GLOBAL_ID not found in meter table Converting R to I. Remove Continue
            v_trans_type := 'I';
           -- ErrorMsg:= ErrorMsg || '. Old_Global_Id: ' || cd_meter_rec.Old_Global_Id;
           -- LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_meter_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
           -- CONTINUE;
          END;
        END IF;
        IF v_TRANS_TYPE='I' THEN --cd_meter_rec.trans_type='I'
          BEGIN
            IF cd_meter_rec.SERVICE_POINT_ID IS NOT NULL THEN
              BEGIN
                v_Rec_Count:=0;
                -- Find Duplicate SERVICE POINT ID
                -- TLM CHANGESET#2C - Check condition for REC_STATUS.
                SELECT COUNT(1)
                INTO v_Rec_Count
                FROM METER
                WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
                AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL)
                ;
                IF (v_Rec_Count        > 0) THEN
                  v_Rec_Count         :=0;
                  --ErrorCode           :='CD-MTR-08';
                  --SELECT DESC_LONG
                  --INTO ErrorMsg
                  --FROM CODE_LOOKUP
                  --WHERE CODE_TYP='ERROR_CODE_CD'
                  --AND CODE      =ErrorCode;
                  dbms_output.put_line('Duplicate SERVICE_POINT_ID in METER. Converting I to U');
                  -- TLM Changeset#7 Duplicate SERVICE_POINT_ID in METER. Converting I to U. Remove Continue
                  v_trans_type := 'U';
                  --ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                  --LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, to_timestamp(NVL(cd_meter_rec.trans_date,sysdate),'YYYY-MM-DD HH24:MI:SS'), cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  --CONTINUE;
                END IF;
              EXCEPTION
              WHEN no_data_found THEN
                -- As expected. So do nothing. The record is good.
                NULL;
              WHEN OTHERS THEN
                ErrorCode := SQLCODE;
                ErrorMsg  := SUBSTR(SQLERRM,1,200);
                LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                CONTINUE;
              END;
            END IF;
            v_Rec_Count := 0;
            -- TLM Changeset#2 Check condition for REC_STATUS
            SELECT COUNT(1)
            INTO v_Rec_Count
            FROM METER -- Check for the duplicate meter
            WHERE global_Id = cd_meter_rec.global_id
             AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL)
            ;
            IF v_Rec_Count  > 0 THEN
              ErrorCode    :='CD-MTR-09';
              SELECT DESC_LONG
              INTO ErrorMsg
              FROM CODE_LOOKUP
              WHERE CODE_TYP='ERROR_CODE_CD'
              AND CODE      =ErrorCode;
              dbms_output.put_line('Duplicate meter exists in meter table');
              LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
              CONTINUE;
            END IF;
            END;
            END IF;
            -- TLM Changeset#8 Adding separate statements for checking TRANSTYPE = I condition
            IF V_TRANS_TYPE = 'I' THEN
            --Insert record
            dbms_output.put_line ('Inserting record into meter table');
            INSERT
            INTO METER
              (
                SERVICE_POINT_ID,
                UNQSPID,
                TRF_ID,
                REV_ACCT_CD,
                SVC_ST_NUM,
                SVC_ST_NAME,
                SVC_ST_NAME2,
                SVC_CITY,
                SVC_STATE,
                SVC_ZIP,
                CUST_TYP,
                RATE_SCHED,
                GLOBAL_ID,
                SM_FLG,
                METER_NUMBER
              )
              VALUES
              (
                cd_meter_rec.SERVICE_POINT_ID,
                cd_meter_rec.UNQSPID,
                transformer_id,
                cd_meter_rec.REV_ACCT_CD,
                SUBSTR(cd_meter_rec.SVC_ST_NUM,1,12),
                cd_meter_rec.SVC_ST_NAME,
                cd_meter_rec.SVC_ST_NAME2,
                cd_meter_rec.SVC_CITY,
                cd_meter_rec.SVC_STATE,
                cd_meter_rec.SVC_ZIP,
                cd_meter_rec.CUST_TYP,
                cd_meter_rec.RATE_SCHED,
                cd_meter_rec.GLOBAL_ID,
                cd_meter_rec.SM_FLG,
                cd_meter_rec.METER_NUMBER
              );

            dbms_output.put_line
            (
              'Record inserted'
            )
            ;
            -- TLM Changing begin code block
         -- END;
        END IF;
        IF v_TRANS_TYPE ='U' THEN -- cd_meter_rec.trans_type='U'
          IF cd_meter_rec.SERVICE_POINT_ID IS NOT NULL THEN
            BEGIN
              -- Find Duplicate SERVICE POINT ID
              v_Rec_Count:=0;
              SELECT COUNT(1)
              INTO v_Rec_Count
              FROM METER
              WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
              AND GLOBAL_ID         <>cd_meter_rec.GLOBAL_ID
              AND ( REC_STATUS <> 'D' OR REC_STATUS IS NULL)
              ;
              IF ( v_Rec_Count       > 0 ) THEN
                v_Rec_Count         :=0;
                ErrorCode           :='CD-MTR-08';
                SELECT DESC_LONG
                INTO ErrorMsg
                FROM CODE_LOOKUP
                WHERE CODE_TYP='ERROR_CODE_CD'
                AND CODE      =ErrorCode;
                dbms_output.put_line('Record could not be found for the combination of global_id and service_point_id in meter table.');
                ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                CONTINUE;

              END IF;
            EXCEPTION
            WHEN no_data_found THEN
              -- As expected. So do nothing. The record is good.
              NULL;
            WHEN OTHERS THEN
              ErrorCode := SQLCODE;
              ErrorMsg  := SUBSTR(SQLERRM,1,200);
              LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
              CONTINUE;
            END;
          END IF;
          dbms_output.put_line('Updating record in meter for U');
          UPDATE METER
          SET TRF_ID         = transformer_id,
            SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID,
            UNQSPID          = cd_meter_rec.UNQSPID,
            REV_ACCT_CD      = cd_meter_rec.REV_ACCT_CD,
            SVC_ST_NUM       = SUBSTR(cd_meter_rec.SVC_ST_NUM,1,12),
            SVC_ST_NAME      = cd_meter_rec.SVC_ST_NAME,
            SVC_ST_NAME2     = cd_meter_rec.SVC_ST_NAME2,
            SVC_CITY         = cd_meter_rec.SVC_CITY,
            SVC_STATE        = cd_meter_rec.SVC_STATE,
            SVC_ZIP          = cd_meter_rec.SVC_ZIP,
            CUST_TYP         = cd_meter_rec.CUST_TYP,
            RATE_SCHED       = cd_meter_rec.RATE_SCHED,
            SM_FLG           = cd_meter_rec.SM_FLG,
            METER_NUMBER     = cd_meter_rec.METER_NUMBER
          WHERE GLOBAL_ID    = cd_meter_rec.GLOBAL_ID;

          dbms_output.put_line('Record updated');
          --TLM add statements to update proc_flg
          dbms_output.put_line('Updating PROC_FLG =P');
          UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
        elsif v_TRANS_TYPE                  ='R' THEN -- cd_meter_rec.trans_type='R'
          IF cd_meter_rec.SERVICE_POINT_ID IS NOT NULL THEN
            BEGIN
              -- Find Duplicate SERVICE POINT ID
              -- TLM CHANGESET#2 - Check condition for REC_STATUS.
              v_Rec_Count:=0;
              SELECT COUNT(1)
              INTO v_Rec_Count
              FROM METER
              WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
              AND GLOBAL_ID         <>cd_meter_rec.OLD_GLOBAL_ID
              AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL)
              ;
              IF ( v_Rec_Count       > 0 ) THEN
                v_Rec_Count         :=0;
                ErrorCode           :='CD-MTR-08';
                SELECT DESC_LONG
                INTO ErrorMsg
                FROM CODE_LOOKUP
                WHERE CODE_TYP='ERROR_CODE_CD'
                AND CODE      =ErrorCode;
                dbms_output.put_line('Duplicate combination of SERVICE_POINT_ID and OLD_GLOBAL_ID');
                ErrorMsg:= ErrorMsg || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID;
                LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                CONTINUE;
              END IF;
            EXCEPTION
            WHEN no_data_found THEN
              -- As expected. So do nothing. The record is good.
              NULL;
            WHEN OTHERS THEN
              ErrorCode := SQLCODE;
              ErrorMsg  := SUBSTR(SQLERRM,1,200);
              dbms_output.put_line('Some Error occurred : ' || errorcode || ' ' || errormsg);
              LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
              CONTINUE;
            END;
          END IF;
          dbms_output.put_line('Updating meter record for R');
          UPDATE METER
          SET TRF_ID         = transformer_id,
            SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID,
            UNQSPID          = cd_meter_rec.UNQSPID,
            REV_ACCT_CD      = cd_meter_rec.REV_ACCT_CD,
            SVC_ST_NUM       = SUBSTR(cd_meter_rec.SVC_ST_NUM,1,12),
            SVC_ST_NAME      = cd_meter_rec.SVC_ST_NAME,
            SVC_ST_NAME2     = cd_meter_rec.SVC_ST_NAME2,
            SVC_CITY         = cd_meter_rec.SVC_CITY,
            SVC_STATE        = cd_meter_rec.SVC_STATE,
            SVC_ZIP          = cd_meter_rec.SVC_ZIP,
            CUST_TYP         = cd_meter_rec.CUST_TYP,
            RATE_SCHED       = cd_meter_rec.RATE_SCHED,
            SM_FLG           = cd_meter_rec.SM_FLG,
            METER_NUMBER     = cd_meter_rec.METER_NUMBER,
            GLOBAL_ID        = cd_meter_rec.GLOBAL_ID
          WHERE GLOBAL_ID    = cd_meter_rec.OLD_GLOBAL_ID;
          dbms_output.put_line('record updated');
          dbms_output.put_line('setting old_service_point_id value in code = null');
          Old_Service_Point_Id := NULL;
          -- TLM add statements for updaing proc_flg to p
          dbms_output.put_line('Updating PROC_FLG =P');
          UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
          BEGIN
            SELECT service_point_id
            INTO Old_Service_Point_Id
            FROM meter
            WHERE GLOBAL_ID = cd_meter_rec.old_global_id
            AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
            dbms_output.put_line('OLD_SERVICE_POINT_ID found');
          EXCEPTION
          WHEN no_data_found THEN
            --TLM
            dbms_output.put_line('OLD_SERVICE_POINT_ID not found. Setting it as NULL');
            Old_Service_Point_Id := NULL;
          END;
          dbms_output.put_line('Updating SP_PEAK_HIST');
          UPDATE SP_PEAK_HIST S
          SET S.SERVICE_POINT_ID   =cd_meter_rec.Service_Point_Id
          WHERE S.SERVICE_POINT_ID = Old_Service_Point_Id;

          dbms_output.put_line('SP_PEAK_HIST updated');
        elsif v_TRANS_TYPE ='D' THEN -- cd_meter_rec.trans_type='D'
          dbms_output.put_line('Updating REC_STATUS in METER = D');
          UPDATE METER SET REC_STATUS = 'D' WHERE GLOBAL_ID=cd_meter_rec.GLOBAL_ID;

          dbms_output.put_line('Updating PROC_FLG =P');
          UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
        ELSE
          NULL;
        END IF;
        IF SQL%ROWCOUNT>0 THEN
          -- dbms_output.put_line('SQL%ROWCOUNT >0. Updating PROC_FLG = P for  ' || cd_meter_rec.ID);
          UPDATE CD_METER M
          SET M.PROC_FLG = 'P'
          WHERE M.ID     =cd_meter_rec.ID;
        END IF;
      EXCEPTION
      WHEN OTHERS THEN
        ErrorCode := SQLCODE;
        ErrorMsg  := SUBSTR(SQLERRM,1,200);
        dbms_output.put_line(cd_meter_rec.trans_date || ' Some error occurred -  ' || Errorcode || ' ' || errormsg);
        LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
        CONTINUE;
      END;
      dbms_output.put_line('Commit changes');
      COMMIT;
      dbms_output.put_line('Done');
    END LOOP;

  END;
END METER_MGMT;
PROCEDURE LOG_ERRORS
  (
    GLOBALID  VARCHAR2,
    ERRORCODE VARCHAR2,
    ERRORMSG  VARCHAR2,
    TRANSDATE TIMESTAMP,
    TRANSTYPE    CHAR,
    APPTYPE      CHAR,
    PROCEDUREFOR VARCHAR2,
    ColumnId     NUMBER)
AS
BEGIN
  dbms_output.put_line('logging');
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
      PROC_FOR,
      CD_TABLE_ID
    )
    VALUES
    (
      GLOBALID,
      ErrorCode,
      ErrorMsg,
      TRANSDATE,
      TRANSTYPE,
      CURRENT_TIMESTAMP,
      AppType,
      ProcedureFor,
      ColumnId
    );

  dbms_output.put_line
  (
    'done'
  )
  ;
END;
FUNCTION TLM_CD_CHECK_ISDUP
  (
    table_name  VARCHAR2,
    field_name  VARCHAR2,
    field_value VARCHAR2
  )
  RETURN VARCHAR2
AS
  v_Rec_Count NUMBER;
  v_stmt      VARCHAR2
  (
    2000
  )
  ;
  v_retval VARCHAR2
  (
    500
  )
  := 'FALSE';
BEGIN
  v_Rec_Count := 0;
  -- TLM Changeset#1 Check condition for REC_STATUS = D FOR DUPLICATE RECORDS
  -- SQL for for duplicate data
  v_stmt := 'SELECT COUNT(1) FROM ' || table_name || ' WHERE ' || field_name || ' = ''' || field_value || '''' || ' AND (REC_STATUS <> ''D'' OR REC_STATUS IS NULL)';
  DBMS_OUTPUT.PUT_LINE
  (
    'DEBUG - '||v_stmt
  )
  ;
  EXECUTE immediate v_stmt INTO v_Rec_Count;
  DBMS_OUTPUT.PUT_LINE
  (
    'DEBUG - v_Rec_Count = ' || v_Rec_Count
  )
  ;
  IF v_Rec_Count > 0 THEN
    v_retval    := 'TRUE';
  ELSE
    v_retval := 'FALSE';
  END IF;
  DBMS_OUTPUT.PUT_LINE
  (
    'TLM_CD_CHECK_ISDUP V_RETVAL = ' || v_retval
  )
  ;
  RETURN v_retval;
END TLM_CD_CHECK_ISDUP;
FUNCTION TLM_CD_CHECK_ISNULL
  (
    val        VARCHAR2,
    rec_id     NUMBER,
    global_id  VARCHAR2,
    trans_type VARCHAR2,
    trans_date DATE,
    app_type      VARCHAR2,
    procedure_for VARCHAR2,
    error_code    VARCHAR2
  )
  RETURN VARCHAR2
AS
  ErrorMsg VARCHAR2(1000) := '';
  v_retval VARCHAR2
  (
    500
  )
  := 'FALSE';
BEGIN
  IF trans_type<>'D' THEN
    IF(val IS NULL)
      OR (TRIM(TO_CHAR(val)) = '') THEN
      SELECT DESC_LONG
      INTO ErrorMsg
      FROM CODE_LOOKUP
      WHERE CODE_TYP='ERROR_CODE_CD'
      AND CODE      =error_code;
      DBMS_OUTPUT.PUT_LINE('Error Message ' || ErrorMsg );
      LOG_ERRORS(global_id,error_code,ErrorMsg,sysdate, trans_type,app_type,procedure_for,rec_id);
      DBMS_OUTPUT.PUT_LINE('Error: Value is NULL');
      DBMS_OUTPUT.PUT_LINE('logged');
      v_retval := 'TRUE';
    ELSE
    DBMS_OUTPUT.PUT_LINE(val ||  ' is not null. OK.');
      v_retval := 'FALSE';
    END IF;
  END IF;
  DBMS_OUTPUT.PUT_LINE('TLM_CD_CHECK_ISNULL V_RETVAL = ' || v_retval);
  RETURN v_retval;
END TLM_CD_CHECK_ISNULL;
FUNCTION TLM_CD_HAS_INSERT_RECORD
  (
    table_name VARCHAR2,
    global_id  VARCHAR2,
    create_dt DATE)
  RETURN VARCHAR2
AS
  v_retval VARCHAR2(500) := 'FALSE';
  v_stmt   VARCHAR2(2000);
BEGIN
  -- TLM Changeset#1 - Date format in string edited for from create_dt to to_char(create_dt,'DD-MON-YY')
  v_stmt := 'UPDATE ' || table_name || ' SET PROC_FLG=''P''
WHERE global_Id = '''|| global_id ||'''
AND CREATE_DTM < to_date(to_char(''' || TO_CHAR(create_dt,'DD-MON-YY') || ''', ''DD-MON-YY''), ''dd-MON-YY'')' || '
AND trans_type = ''I''';
  DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
  EXECUTE immediate v_stmt;
  IF SQL%ROWCOUNT > 0 THEN -- UPSERT
    v_retval     := 'TRUE';
  ELSE
    v_retval := 'FALSE';
  END IF;
  DBMS_OUTPUT.PUT_LINE('TLM_CD_HAS_INSERT_RECORD V_RETVAL = ' || v_retval);
  RETURN v_retval;
END TLM_CD_HAS_INSERT_RECORD;

FUNCTION TLM_TRF_ISDUP
  (
    field_CGC_ID  NUMBER,
    field_GLOBAL_ID  VARCHAR2)
  RETURN VARCHAR2
AS
  v_Rec_Count NUMBER;
  v_stmt VARCHAR2(2000);
  v_retval VARCHAR2(500) := 'FALSE';
BEGIN
  v_Rec_Count := 0;
  -- TLM Changeset#1 Check condition for REC_STATUS = D FOR DUPLICATE RECORDS
  -- SQL for for duplicate data
  v_stmt := 'SELECT COUNT(1) FROM  EDTLM.TRANSFORMER WHERE CGC_ID =  ' || field_CGC_ID || ' AND GLOBAL_ID = '''|| field_GLOBAL_ID||'''';
  DBMS_OUTPUT.PUT_LINE( 'DEBUG - '||v_stmt);
  EXECUTE immediate v_stmt INTO v_Rec_Count;
  DBMS_OUTPUT.PUT_LINE('DEBUG - v_Rec_Count = ' || v_Rec_Count);
  IF v_Rec_Count > 0 THEN
    v_retval    := 'TRUE';
  ELSE
    v_retval := 'FALSE';
  END IF;
  DBMS_OUTPUT.PUT_LINE('TLM_CD_CHECK_ISDUP V_RETVAL = ' || v_retval);
  RETURN v_retval;
END TLM_TRF_ISDUP;
END TLM_CD_MGMT_MONTHLY;
/


Prompt Grants on PACKAGE TLM_CD_MGMT_MONTHLY TO GIS_I to GIS_I;
GRANT EXECUTE ON EDTLM.TLM_CD_MGMT_MONTHLY TO GIS_I
/
