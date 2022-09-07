CREATE OR REPLACE 
PACKAGE BODY edtlm.tlm_cd_mgmt_monthly
AS
  AppType              CHAR(1);
  ProcedureFor         VARCHAR2(20);
  NoOfRows             NUMBER;
  Old_Service_Point_Id VARCHAR2(10);
  FoundGlobalId        VARCHAR2(38);

PROCEDURE Delete_Transformer
  (i_GLOBALID  VARCHAR2, i_cgc_id NUMBER, ErrorMsg OUT VARCHAR2, ErrorCode OUT VARCHAR2 )
/*
Purpose:  Mark the transformer with the passed global id with Rec_status = D

          Manage the data to not violate the unique constraint on cgc and rec_status


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        06/24/19    Initial coding
*/
AS
  v_Rec_Count     NUMBER;
  v_FoundGlobalId VARCHAR2(38);
  v_count         NUMBER;
BEGIN
   -- check to see if this is a reused cgc, and if so move the oldest one
   -- to del_dup before proceeding
   SELECT count(*), min(GLOBAL_ID)
     INTO v_rec_count, v_FoundGlobalId
     FROM Transformer
    WHERE cgc_id = i_cgc_id
      AND global_id <> i_GLOBALID
      AND REC_STATUS = 'D';
   IF v_rec_count > 0 THEN
      INSERT INTO transformer_del_dup
      select *
        from transformer
       where  cgc_id = i_cgc_id
         and global_id = v_FoundGlobalId;
      -- set the cgc to the sequence value so the new record can be processed
      UPDATE transformer
         set cgc_id = deleted_cgc_id_seq.nextval
       where cgc_id = i_cgc_id
         and global_id = v_FoundGlobalId;
   END IF;
   UPDATE TRANSFORMER SET REC_STATUS = 'D' WHERE GLOBAL_ID=i_GLOBALID;

END Delete_Transformer;

PROCEDURE TRANSFORMER_MGMT
  ( FromDate DATE, ToDate DATE, ErrorMsg OUT VARCHAR2, ErrorCode OUT VARCHAR2 )
/*
Purpose:  Process all the change records in the cd_transformer table to synchronize
          EDTLM's data to EDGIS's.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   ???          ???        Initial coding - IBM I belive
   TJJ4        05/01/19    refactored the code to remove redundancies
                           addressed issue with D's and R's becoming I's and U's
                           marked records with errors --> proc_flg = E
                           removed code that no longer makes sense:
                            * no records with null global_id or cgc_id are selected
                              for processing so there's no need to do a null check
                            * Coastal_interior_flag is defined to be 1 number long,
                              no need to check for a length > 1
                           add error handling for records with null trans_type,
                           global_id, cgc_id
   TJJ4        05/21/19    added code to move all records processed to the archive
                           table.  Records with proc_flg = P or E are moved
                           records with unhandled errors are left in the table
   TJJ4        06/04/19    when mappers reuse cgc's we end up with 2 records in trf
                           with the same cgc, when we then try to delete it (rec_status = D)
                           the unique index prevents this; so move the old deleted
                           record to transformer_del_dup, set the cgc on the old record
                           and then mark the latest incantation to deleted.
   TJJ4        02/24/20    Sort the records for processing first by the day the
                           change was created, then by trans_type R, D, U then I
  Note: changes are identified and delivered weekly, this code will handle a daily
        delivery, but anything more frequent then that would need to be re-examined
   TJJ4        03/17/20    when updating records don't update those that have rec_status = D
*/

AS
  v_Rec_Count  NUMBER;
  v_TRANS_TYPE VARCHAR2(1);
  v_count      NUMBER;
BEGIN
  ErrorMsg     := '';
  ErrorCode    := '';
  AppType      := 'M';
  PROCEDUREFOR := 'TRF';
  v_Rec_Count  := 0;
  v_TRANS_TYPE := '';

  /* get all rows from stage table where transaction types are Insert, update,
     replace or delete */
  /* note: there's an asumption that changes are stored in increasing ID order,
           so one should not see an update, delete record with an ID less then the Insert
           presumably this is handled by eder's creation of these records*/

  DECLARE
    CURSOR c_cd_transformer
    IS
      SELECT *
        FROM cd_transformer
       WHERE TRANS_TYPE IN ('I','D','U','R')
         AND PROC_FLG   IS NULL
         AND GLOBAL_ID  IS NOT NULL
         AND CGC_ID     IS NOT NULL
        --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
--          ORDER BY Id;
          ORDER BY trunc(create_dtm), decode(trans_type, 'R',1,'D',2,'U',3,'I',4,5);
  BEGIN

    FOR cd_trf_rec IN c_cd_transformer
    LOOP
      BEGIN
        v_TRANS_TYPE := cd_trf_rec.trans_type;
        IF TLM_CD_CHECK_ISNULL (cd_trf_rec.INSTALLATION_TYP, cd_trf_rec.Id, cd_trf_rec.global_id, cd_trf_rec.trans_type, cd_trf_rec.trans_date, AppType, ProcedureFor, 'CD-TRF-04') = 'TRUE' THEN
          cd_trf_rec.INSTALLATION_TYP := 'OH';
        END IF;
/* Check for anomolous conditions and where possible alter tran_type to handle them.
   The following will be handled here:
      Update where globalid doesn't exist becomes an Insert
         Note: if the CGC of this Update-->Insert record exists, log the original record as
               a duplicate, give it a temporary cgc_id from a sequence, presumably there's
               another update/delete for the existing record to set the cgc to something new
               or to retire the record.
      Delete where globalid doesn't exists becomes a processed change
*/

        IF cd_trf_rec.trans_type='U' OR cd_trf_rec.trans_type='D' THEN
           BEGIN
              SELECT GLOBAL_ID
                INTO FoundGlobalId
                FROM Transformer
               WHERE global_Id = cd_trf_rec.global_id
                 AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
           EXCEPTION
           WHEN NO_DATA_FOUND THEN
              IF cd_trf_rec.trans_type ='U' THEN --Update where guid does not exist
                 BEGIN
                    SELECT GLOBAL_ID
                      INTO FoundGlobalId
                      FROM Transformer
                     WHERE cgc_id = cd_trf_rec.cgc_id
                       AND (REC_STATUS <> 'D' OR REC_STATUS is null)  ;
                    -- if the cgc already exists, copy the duplicate
                    INSERT INTO transformer_del_dup
                    select *
                      from transformer
                     where  cgc_id = cd_trf_rec.cgc_id
                       and global_id = FoundGlobalId;
                    -- set the cgc to the sequence value so the new record can be processed
                    UPDATE transformer
                       set cgc_id = deleted_cgc_id_seq.nextval
                     where cgc_id = cd_trf_rec.cgc_id
                       and global_id = FoundGlobalId;
                    -- chenge the transaction to an I-insert
                    V_TRANS_TYPE := 'I';
                    -- log the duplicate to the error table for user validation
                    ErrorCode   :='CD-TRF-06';  -- Duplicate Transformer CGC ID
                    SELECT DESC_LONG || ' : ' ||cd_trf_rec.CGC_ID
                      INTO ErrorMsg
                      FROM CODE_LOOKUP
                     WHERE CODE_TYP = 'ERROR_CODE_CD'
                       AND CODE = ErrorCode;
                    LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                 EXCEPTION
                 WHEN NO_DATA_FOUND THEN
                    -- the cgc doesn't already exist, simply change the transaction to an I-insert
                    V_TRANS_TYPE := 'I';
                 END;
              ELSE -- Delete trans_type and global_id doesn't exist
                 -- Update the PROC_FLG flag.
                 UPDATE CD_TRANSFORMER T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
                 Commit;
                 CONTINUE;
              END IF;
           END;
        END IF;

/* Truth table for replaces:
                       Old GUID  New GUID
                        Exists    Exists    action to take
   Normal Case:          TRUE     FALSE     handle as a Replace
   Handled exception 1: FALSE     FALSE     Change transaction to I-insert
   Handled exception 2: FALSE      TRUE     Change transaction to U-update
   Error Condition       TRUE      TRUE     Log it as an error

*/
        IF cd_trf_rec.trans_type='R' THEN
           BEGIN
              -- all branches of logic depend on whether the new guid exists or not, so find out
              SELECT count(*)
                INTO v_count
                FROM Transformer
               WHERE global_Id = cd_trf_rec.global_id
                 AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
              -- now, does the old guid exist ?
              SELECT global_id
                INTO FoundGlobalId
                FROM Transformer
               WHERE global_Id = cd_trf_rec.old_global_id
                 AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
              -- the old guid exists, branch based on the new guids existance
               IF v_count > 0 THEN -- the new guid already exists
               -- this is the error condition, both guids exist, need a human to
               -- look at this and untangle it, especially the bank and meter records
                  ErrorCode := 'CD-TRF-07';
                  SELECT DESC_LONG
                    INTO ErrorMsg
                    FROM CODE_LOOKUP
                   WHERE CODE_TYP = 'ERROR_CODE_CD'
                     AND CODE = ErrorCode;
                  LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                  UPDATE CD_TRANSFORMER T SET PROC_FLG='E' WHERE T.ID= cd_trf_rec.ID;
                  Commit;
                  CONTINUE;
               END IF;
               -- if the code gets here, this is the expected condition, do nothing to trans_type
           EXCEPTION
           WHEN NO_DATA_FOUND THEN  -- the old guid doesn't exist
              IF v_count > 0 THEN -- the new guid already exists
                 v_trans_type := 'U';
              ELSE
                 v_trans_type := 'I';
              END IF;
           END;
        END IF;

        IF v_TRANS_TYPE = 'I' THEN -- cd_trf_rec.trans_type='I'
           IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'global_id', cd_trf_rec.global_id) = 'TRUE' THEN
              ErrorCode := 'CD-TRF-05';
              SELECT DESC_LONG
                INTO ErrorMsg
                FROM CODE_LOOKUP
               WHERE CODE_TYP = 'ERROR_CODE_CD'
                 AND CODE = ErrorCode;
              LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
              UPDATE CD_TRANSFORMER T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_rec.ID;
              commit;
              CONTINUE;
           END IF;
           IF TLM_CD_CHECK_ISDUP ('TRANSFORMER', 'CGC_ID', cd_trf_rec.CGC_ID) = 'TRUE' THEN
              -- change detection in EDER doesn't always order the records correctly, occationally
              -- a cgc will have an I on one guid followed by a D on another globalid.
              -- the contraints expect the D to occur first
              -- if there is a delete in cd_trf for this cgc coming up, with a different guid
              -- for a non-deleted trf record, then process the D first,
              select count(*), min(global_id)
                into v_Rec_Count, FoundGlobalId
                from cd_transformer
               where cgc_id = cd_trf_rec.CGC_ID
                 and trans_type = 'D'
                 and global_id <> cd_trf_rec.global_id;
              IF v_Rec_Count = 1 then
                 Delete_Transformer(FoundGlobalId, cd_trf_rec.cgc_id, ErrorMsg, ErrorCode );
                 commit;
              else
                 ErrorCode := 'CD-TRF-06';
                 SELECT DESC_LONG || ' : ' ||cd_trf_rec.CGC_ID
                   INTO ErrorMsg
                   FROM CODE_LOOKUP
                  WHERE CODE_TYP = 'ERROR_CODE_CD'
                    AND CODE = ErrorCode;
                 LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                 UPDATE CD_TRANSFORMER T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_rec.ID;
                 commit;
                 CONTINUE;
              END IF;
           END IF;
        END IF;
        -- got through the error block so it's okay to insert the record
        IF v_trans_type = 'I' THEN
           INSERT INTO TRANSFORMER
              ( CGC_ID,                        GLOBAL_ID,
                COAST_INTERIOR_FLG,            CLIMATE_ZONE_CD,
                INSTALLATION_TYP,              LOWSIDE_VOLTAGE,
                OPERATING_VOLTAGE,             CIRCUIT_ID,
                VAULT)
              VALUES
              ( cd_trf_rec.CGC_ID,             cd_trf_rec.GLOBAL_ID,
                cd_trf_rec.COAST_INTERIOR_FLG, cd_trf_rec.CLIMATE_ZONE_CD,
                cd_trf_rec.INSTALLATION_TYP,   cd_trf_rec.LOWSIDE_VOLTAGE,
                cd_trf_rec.OPERATING_VOLTAGE,  cd_trf_rec.CIRCUIT_ID,
                cd_trf_rec.VAULT );
        END IF;

        IF v_TRANS_TYPE = 'U' THEN
           SELECT COUNT(1)
             INTO v_Rec_Count
             FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
            WHERE CGC_ID = cd_trf_rec.CGC_ID
              AND global_Id <> cd_trf_rec.global_id
              AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
           IF v_Rec_Count > 0 THEN  -- found another record with this cgc, an error
              ErrorCode := 'CD-TRF-06';
              SELECT DESC_LONG || ' : ' ||cd_trf_rec.CGC_ID
                INTO ErrorMsg
                FROM CODE_LOOKUP
               WHERE CODE_TYP = 'ERROR_CODE_CD'
                 AND CODE = ErrorCode;
              LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
              UPDATE CD_TRANSFORMER T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_rec.ID;
              commit;
              CONTINUE;
           END IF;

           UPDATE TRANSFORMER SET
              CGC_ID             = cd_trf_rec.CGC_ID,
              COAST_INTERIOR_FLG = cd_trf_rec.COAST_INTERIOR_FLG,
              CLIMATE_ZONE_CD    = cd_trf_rec.CLIMATE_ZONE_CD,
              INSTALLATION_TYP   = cd_trf_rec.INSTALLATION_TYP,
              LOWSIDE_VOLTAGE    = cd_trf_rec.LOWSIDE_VOLTAGE,
              OPERATING_VOLTAGE  = cd_trf_rec.OPERATING_VOLTAGE,
              CIRCUIT_ID         = cd_trf_rec.CIRCUIT_ID ,
              VAULT              = cd_trf_rec.VAULT
           WHERE GLOBAL_ID       = cd_trf_rec.GLOBAL_ID
             AND nvl(rec_status,'t') <> 'D';
        END IF;

        IF v_TRANS_TYPE = 'R' THEN
           SELECT COUNT(1)
             INTO v_Rec_Count
             FROM TRANSFORMER -- Check for the duplicate transformer CGC_ID
            WHERE CGC_ID   = cd_trf_rec.CGC_ID
              AND global_Id <> cd_trf_rec.Old_global_id
              AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
             IF v_Rec_Count > 0 THEN
                ErrorCode := 'CD-TRF-06';
                SELECT DESC_LONG || ' : ' ||cd_trf_rec.CGC_ID
                  INTO ErrorMsg
                  FROM CODE_LOOKUP
                 WHERE CODE_TYP='ERROR_CODE_CD'
                   AND CODE = ErrorCode;
                LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
                UPDATE CD_TRANSFORMER T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_rec.ID;
                commit;
                CONTINUE;
             END IF;

             UPDATE TRANSFORMER
                SET CGC_ID             = cd_trf_rec.CGC_ID,
                    GLOBAL_ID          = cd_trf_rec.GLOBAL_ID,
                    COAST_INTERIOR_FLG = cd_trf_rec.COAST_INTERIOR_FLG,
                    CLIMATE_ZONE_CD    = cd_trf_rec.CLIMATE_ZONE_CD,
                    INSTALLATION_TYP   = cd_trf_rec.INSTALLATION_TYP,
                    LOWSIDE_VOLTAGE    = cd_trf_rec.LOWSIDE_VOLTAGE,
                    OPERATING_VOLTAGE  = cd_trf_rec.OPERATING_VOLTAGE,
                    CIRCUIT_ID         = cd_trf_rec.CIRCUIT_ID,
                    VAULT              = cd_trf_rec.VAULT
              WHERE GLOBAL_ID          = cd_trf_rec.OLD_GLOBAL_ID
                AND nvl(rec_status,'t') <> 'D';

             SELECT COUNT(1)
               INTO v_Rec_Count
               FROM EDSETT.SM_SPECIAL_LOAD -- Check for the Special Load record is exists in setting schema
              WHERE Ref_global_Id = cd_trf_rec.OLD_GLOBAL_ID;
             IF v_Rec_Count <> 0 THEN
                UPDATE EDSETT.SM_SPECIAL_LOAD
                   SET REF_GLOBAL_ID  = cd_trf_rec.GLOBAL_ID
                 WHERE REF_GLOBAL_ID  = cd_trf_rec.OLD_GLOBAL_ID;
             END IF ;
        END IF;

        IF v_TRANS_TYPE='D' THEN
           Delete_Transformer(cd_trf_rec.Global_id, cd_trf_rec.cgc_id, ErrorMsg, ErrorCode );
       END IF;

        UPDATE cd_transformer T SET PROC_FLG='P' WHERE T.ID= cd_trf_rec.ID;
        commit;

      EXCEPTION
      WHEN OTHERS THEN
         ErrorCode := SQLCODE;
         ErrorMsg  := SUBSTR(SQLERRM,1,200);
         LOG_ERRORS(cd_trf_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_rec.trans_type,AppType,ProcedureFor,cd_trf_rec.Id);
         UPDATE CD_TRANSFORMER T SET PROC_FLG='E' WHERE T.ID= cd_trf_rec.ID;
         CONTINUE;
      END;
      COMMIT;
    END LOOP;
  -- add error handling for records with invalid trans_types, null globalid's, null cgc's
    INSERT INTO CD_ERRORS (
           GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
           TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
    select GLOBAL_ID,     'PROC1',       'Invalid Transaction Type', TRANS_DATE,
           TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
      from cd_transformer t
     where nvl(trans_type, 'X') not in ('I', 'U','D','R')
       and proc_flg is null;
    update cd_transformer t
       set proc_flg = 'E'
     where nvl(trans_type, 'X') not in ('I', 'U','D','R')
       and proc_flg is null;

   INSERT INTO CD_ERRORS (
           GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
           TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
    select GLOBAL_ID,     'CD-TRF-02',   'GLOBAL_ID is null in CD_TRANSFORMER', TRANS_DATE,
           TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
      from cd_transformer t
     where global_id is null
       and proc_flg is null;
    update cd_transformer t
       set proc_flg = 'E'
     where global_id is null
       and proc_flg is null;

   INSERT INTO CD_ERRORS (
           GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
           TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
    select GLOBAL_ID,     'CD-TRF-03',   'CGC_ID is null in CD_TRANSFORMER', TRANS_DATE,
           TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
      from cd_transformer t
     where cgc_id is null
       and proc_flg is null;
    update cd_transformer t
       set proc_flg = 'E'
     where cgc_id is null
       and proc_flg is null;

   INSERT INTO cd_transformer_archive
        ( id, cgc_id, global_id, coast_interior_flg,
          climate_zone_cd, installation_typ, create_dtm,
          create_userid, update_dtm, update_userid, region,
          trans_type, trans_date, old_global_id, proc_flg,
          lowside_voltage, operating_voltage, circuit_id, vault)
   SELECT id, cgc_id, global_id, coast_interior_flg,
          climate_zone_cd, installation_typ, create_dtm,
          create_userid, update_dtm, update_userid, region,
          trans_type, trans_date, old_global_id, proc_flg,
          lowside_voltage, operating_voltage, circuit_id, vault
     FROM cd_transformer
    WHERE proc_flg is not null;

   DELETE FROM cd_transformer WHERE proc_flg is not null;

  END;
END TRANSFORMER_MGMT;

PROCEDURE TRANSFORMER_BANK_MGMT
  ( FromDate DATE, ToDate DATE, ErrorMsg OUT VARCHAR2, ErrorCode OUT VARCHAR2 )
/*
Purpose:  Process all the change records in the cd_transformer_bank table to
          synchronize EDTLM's data to EDGIS's.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   ???          ???        Initial coding - IBM I belive
   TJJ4        05/02/19    refactored the code to remove redundancies
                           marked records with errors --> proc_flg = E
                           Bank_code is just an attribute, modified TLM to
                           accept any valid values between 1 and 9, defaulting
                           null bank codes to 0
   TJJ4        05/16/19    changed the code to process transaction type D
                           that have null transformer_global_id's allowing
                           us to delete orphaned unit records
   TJJ4        05/21/19    added code to move all records processed to the archive
                           table.  Records with proc_flg = P or E are moved
                           records with unhandled errors are left in the table
   TJJ4        06/19/19    fixed issue where both bank_cd and phase_cd are null
                           throwing can not insert null error
   TJJ4        02/24/20    Sort the records for processing first by the day the
                           change was created, then by trans_type R, D, U then I
  Note: changes are identified and delivered weekly, this code will handle a daily
        delivery, but anything more frequent then that would need to be re-examined
   TJJ4        03/17/20    when updating records don't update those that have rec_status = D
*/
AS
   transformer_id NUMBER;
   v_Rec_Count    NUMBER;
   v_TRANS_TYPE   VARCHAR2(1);
   v_count        NUMBER;
BEGIN
   ErrorMsg     := '';
   ErrorCode    := '';
   AppType      := 'M';
   ProcedureFor := 'TRF-BANK';
   v_Rec_Count  := 0;
   v_TRANS_TYPE := '';
  /* get all rows from change detection stage table where transaction types are
     insert, update, replace or delete */
   DECLARE
      CURSOR c_cd_transformer_bank
      IS
         SELECT *
           FROM cd_transformer_bank
          WHERE TRANS_TYPE IN ('I','D','U','R')
            AND PROC_FLG   IS NULL
          --AND (TRANS_DATE BETWEEN FromDate AND ToDate)
--          ORDER BY Id;
          ORDER BY trunc(create_dtm), decode(trans_type, 'R',1,'D',2,'U',3,'I',4,5);
   BEGIN
      FOR cd_trf_bank_rec IN c_cd_transformer_bank
      LOOP
      BEGIN
         v_TRANS_TYPE := cd_trf_bank_rec.trans_type;
         -- if it's a delete ignore null trf_global_id's
         IF (v_TRANS_TYPE <> 'D' and TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.TRF_GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-01') = 'TRUE') THEN
            UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
            COMMIT;
            CONTINUE;
         ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.GLOBAL_ID, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-04') = 'TRUE' THEN
            UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
            COMMIT;
            CONTINUE;
         ELSIF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.NP_KVA, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-06') = 'TRUE' THEN
            UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
            COMMIT;
            CONTINUE;
         ELSIF ((LENGTH(TRIM(TRANSLATE(cd_trf_bank_rec.BANK_CD, ' +-.0123456789', ' '))) IS NOT NULL ) OR To_Number(cd_trf_bank_rec.BANK_CD) > 9 ) THEN
            ErrorCode := 'CD-BNK-10';
            SELECT DESC_LONG || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID
              INTO ErrorMsg
              FROM CODE_LOOKUP
             WHERE CODE_TYP = 'ERROR_CODE_CD'
               AND CODE = ErrorCode;
            LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
            UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
            COMMIT;
            CONTINUE;
         ELSE
           null; -- no other errors to check for
         END IF;

         -- these checks moved here to make sure both fields are set if they're null
         IF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.BANK_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-05') = 'TRUE' THEN
            cd_trf_bank_rec.Bank_cd := '0';
         END IF;
         IF TLM_CD_CHECK_ISNULL (cd_trf_bank_rec.PHASE_CD, cd_trf_bank_rec.Id, cd_trf_bank_rec.global_id, cd_trf_bank_rec.trans_type, cd_trf_bank_rec.trans_date, AppType, ProcedureFor, 'CD-BNK-07') = 'TRUE' THEN
            cd_trf_bank_rec.PHASE_CD := 0;
         END IF;

-- this code means only I,U,R's with existing parent transformers will be processed,
-- everything else will throw an error. Deletes will still be processed
         BEGIN -- get the transformer_id for this unit/bank
            SELECT ID
              INTO transformer_id
              FROM TRANSFORMER
             WHERE GLOBAL_ID =cd_trf_bank_rec.TRF_GLOBAL_ID
               AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
         EXCEPTION
         WHEN no_data_found THEN
            IF cd_trf_bank_rec.trans_type <> 'D' THEN
               transformer_id := 0;
               ErrorCode := 'CD-BNK-02';
               SELECT DESC_LONG || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID
                 INTO ErrorMsg
                 FROM CODE_LOOKUP
                WHERE CODE_TYP = 'ERROR_CODE_CD'
                  AND CODE = ErrorCode;
               LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
               UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
               COMMIT;
               CONTINUE;
            END IF;
         END;

         IF cd_trf_bank_rec.trans_type = 'U' OR cd_trf_bank_rec.trans_type = 'D' THEN
            BEGIN
            -- TLM Changeset#4B - Update Transformer_bank only if rec_status <> D. Otherwise insert or log error
               SELECT Global_Id
                 INTO FoundGlobalId
                 FROM Transformer_Bank
                WHERE Global_Id = cd_trf_bank_rec.Global_Id
                  AND (REC_STATUS <> 'D' OR REC_STATUS is null) ;
            EXCEPTION
            WHEN NO_DATA_FOUND THEN
               IF cd_trf_bank_rec.trans_type = 'U' THEN -- Update of a non-existant record
                  -- convert to Insert
                  v_trans_type := 'I';
               ELSE -- trans type was Delete
                  UPDATE CD_TRANSFORMER_BANK T
                     SET PROC_FLG = 'P'
                   WHERE T.ID = cd_trf_bank_rec.ID;
                   COMMIT;
                   CONTINUE;
               END IF;
            END;
         END IF;

         IF cd_trf_bank_rec.trans_type='R' THEN
            BEGIN
               SELECT Global_Id
                 INTO FoundGlobalId
                 FROM Transformer_Bank
                WHERE Global_Id = cd_trf_bank_rec.Old_Global_Id
                  AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            EXCEPTION
            WHEN no_data_found THEN
               -- convert to Insert
               v_trans_type := 'I';
            END;
         END IF;

         IF v_TRANS_TYPE ='I' THEN
            IF TLM_CD_CHECK_ISDUP ('TRANSFORMER_BANK', 'global_id', cd_trf_bank_rec.global_id) = 'TRUE' THEN
               -- Converting I to U if global_id already exists
               v_trans_type := 'U';
            END IF;
         END IF;

         IF v_trans_type = 'I' THEN
            INSERT INTO TRANSFORMER_BANK
              (TRF_ID,                   BANK_CD,
               NP_KVA,                   GLOBAL_ID,
               PHASE_CD,                 TRF_TYP)
            VALUES
              (transformer_id,           to_number(cd_trf_bank_rec.BANK_CD),
               cd_trf_bank_rec.NP_KVA,   cd_trf_bank_rec.GLOBAL_ID,
               cd_trf_bank_rec.PHASE_CD, cd_trf_bank_rec.TRF_TYP );
         END IF;

         IF v_TRANS_TYPE='U' THEN
            UPDATE TRANSFORMER_BANK
               SET TRF_ID    = transformer_Id,
                   BANK_CD   = to_number(cd_trf_bank_rec.BANK_CD),
                   NP_KVA    = cd_trf_bank_rec.NP_KVA,
                   PHASE_CD  = cd_trf_bank_rec.PHASE_CD,
                   TRF_TYP   = cd_trf_bank_rec.TRF_TYP
             WHERE GLOBAL_ID = cd_trf_bank_rec.GLOBAL_ID
               AND nvl(rec_status,'t') <> 'D';
         ELSIF v_TRANS_TYPE='R' THEN
            -- add an error check for the corner case where the global_id already exists
            -- throw an error don't update the record
            SELECT count(*)
              INTO v_Rec_Count
              FROM TRANSFORMER_BANK
             WHERE global_id = cd_trf_bank_rec.global_id
               AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            IF v_Rec_Count = 0 THEN -- we're good do the update
               UPDATE TRANSFORMER_BANK
                  SET TRF_ID    = transformer_id,
                      BANK_CD   = to_number(cd_trf_bank_rec.BANK_CD),
                      NP_KVA    = cd_trf_bank_rec.NP_KVA,
                      PHASE_CD  = cd_trf_bank_rec.PHASE_CD,
                      GLOBAL_ID = cd_trf_bank_rec.GLOBAL_ID
                WHERE GLOBAL_ID = cd_trf_bank_rec.OLD_GLOBAL_ID
                  AND nvl(rec_status,'t') <> 'D';
            ELSE -- throw an error
               ErrorCode := 'CD-BNK-11';
               SELECT DESC_LONG || '. TRF_GLOBAL_ID: ' || cd_trf_bank_rec.TRF_GLOBAL_ID
                 INTO ErrorMsg
                 FROM CODE_LOOKUP
                WHERE CODE_TYP = 'ERROR_CODE_CD'
                  AND CODE = ErrorCode;
               LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
               UPDATE CD_TRANSFORMER_BANK T SET PROC_FLG = 'E' WHERE T.ID = cd_trf_bank_rec.ID;
               COMMIT;
               CONTINUE;
            END IF;
         ELSIF v_TRANS_TYPE='D' THEN
            UPDATE TRANSFORMER_BANK
               SET REC_STATUS = 'D'
             WHERE GLOBAL_ID  = cd_trf_bank_rec.GLOBAL_ID
               AND nvl(rec_status,'t') <> 'D';
            UPDATE CD_TRANSFORMER_BANK T
               SET PROC_FLG = 'P'
             WHERE T.ID = cd_trf_bank_rec.ID;
         ELSE
            NULL;
         END IF;
         IF SQL%ROWCOUNT>0 THEN
            UPDATE CD_TRANSFORMER_BANK B
               SET B.PROC_FLG = 'P'
             WHERE B.ID = cd_trf_bank_rec.ID;
         END IF;
      EXCEPTION
      WHEN OTHERS THEN
         ErrorCode := SQLCODE;
         ErrorMsg  := SUBSTR(SQLERRM,1,200);
         LOG_ERRORS(cd_trf_bank_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_trf_bank_rec.trans_type,AppType,ProcedureFor,cd_trf_bank_rec.Id);
         CONTINUE;
      END;
      COMMIT;
      END LOOP;
        -- add error handling for records with invalid trans_types
    INSERT INTO CD_ERRORS (
           GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
           TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
    select GLOBAL_ID,     'PROC1',       'Invalid Transaction Type', TRANS_DATE,
           TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, ID
      from cd_transformer_bank
     where nvl(trans_type, 'X') not in ('I', 'U','D','R')
       and proc_flg is null;
    update cd_transformer_bank
       set proc_flg = 'E'
     where nvl(trans_type, 'X') not in ('I', 'U','D','R')
       and proc_flg is null;

   INSERT INTO cd_transformer_bank_archive
        ( id, trf_global_id, bank_cd, np_kva, global_id,
          phase_cd, create_dtm, create_userid, update_dtm,
          update_userid, trans_type, trans_date, old_global_id,
          proc_flg, trf_typ)
   SELECT id, trf_global_id, bank_cd, np_kva, global_id,
          phase_cd, create_dtm, create_userid, update_dtm,
          update_userid, trans_type, trans_date, old_global_id,
          proc_flg, trf_typ
     FROM cd_transformer_bank a
    WHERE proc_flg is not null;

   DELETE FROM cd_transformer_bank WHERE proc_flg is not null;

   END;
END TRANSFORMER_BANK_MGMT;


PROCEDURE METER_MGMT
  (FromDate DATE, ToDate DATE, ErrorMsg OUT VARCHAR2, ErrorCode OUT VARCHAR2 )
/*
Purpose:  Process all the change records in the cd_meter table to
          synchronize EDTLM's data to EDGIS's.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   ???          ???        Initial coding - IBM I belive
   TJJ4        05/10/19    removed old commented out code, reformatted sql
                           to improve readability/supportability
                           marked records with errors --> proc_flg = E
                           added error reporting for cd_meter records with null
                           trf_global_id, global_id, or service_point_id as well
                           as invalid Trans_types
   TJJ4        05/16/19    updated select cursor to select Delete records with
                           null trf_global_id's allowing us to delete orphaned meters
   TJJ4        05/21/19    added code to move all records processed to the archive
                           table.  Records with proc_flg = P or E are moved
                           records with unhandled errors are left in the table
   TJJ4        05/30/19    added code to delete, to detect if there is an old already
                           deleted meter record w/this spid, if so move the old
                           record to meter_del_dup and set the spid to a sequence
                           <need to address the creation of these records to>
   TJJ4        09/12/19    <temp fix> populate the svc_phase column for newly
                           added spids, full fix will come when EI delivers the data
                           from CCB, for now use the heuristic:
                             if the cust_typ is DOM svc_phase = 1 else it's 3
   TJJ4        02/24/20    Sort the records for processing first by the day the
                           change was created, then by trans_type R, D, U then I
  Note: changes are identified and delivered weekly, this code will handle a daily
        delivery, but anything more frequent then that would need to be re-examined

   TJJ4        03/17/20    when updating records don't update those that have rec_status = D
*/
AS
   transformer_id NUMBER;
   v_Rec_Count    NUMBER;
   v_TRANS_TYPE   VARCHAR2(1);
   v_count        NUMBER;
BEGIN
   ErrorMsg     := '';
   ErrorCode    := '';
   AppType      := 'M';
   ProcedureFor := 'METER';
   v_Rec_Count  := 0;
   v_TRANS_TYPE := '';
  /* get all rows from change detection stage table where transaction types are update,replace or delete */
  /* note: this code does not handle spid's becoming cwots... */
   DECLARE
      CURSOR c_cd_meter
      IS
         SELECT *
           FROM cd_meter
          WHERE TRANS_TYPE IN ('I','D','U','R')
            AND PROC_FLG IS NULL
            AND decode(trans_type, 'D', 'include me', trf_global_id) is not null
            AND GLOBAL_ID IS NOT NULL
            AND SERVICE_POINT_ID IS NOT NULL
--          ORDER BY Id;
          ORDER BY trunc(create_dtm), decode(trans_type, 'R',1,'D',2,'U',3,'I',4,5);
   BEGIN
      FOR cd_meter_rec IN c_cd_meter
      LOOP
         BEGIN
            v_TRANS_TYPE:= cd_meter_rec.trans_type;
            BEGIN -- Get the trf id for this spid
               SELECT ID
                 INTO transformer_id
                 FROM TRANSFORMER
                WHERE GLOBAL_ID = cd_meter_rec.TRF_GLOBAL_ID
                  AND (REC_STATUS <> 'D' OR REC_STATUS is null);
            EXCEPTION
            WHEN no_data_found THEN  -- if the trf does not exist... throw an error
               IF cd_meter_rec.trans_type <> 'D' THEN
                  transformer_id := 0;
                  ErrorCode := 'CD-MTR-02';
                  SELECT DESC_LONG || '. TRF_GLOBAL_ID:' || cd_meter_rec.TRF_GLOBAL_ID
                    INTO ErrorMsg
                    FROM CODE_LOOKUP
                   WHERE CODE_TYP='ERROR_CODE_CD'
                     AND CODE = ErrorCode;
                  LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  UPDATE CD_METER M SET PROC_FLG = 'E' WHERE M.ID = cd_meter_rec.ID;
                  COMMIT;
                  CONTINUE;
               END IF;
            END;
            IF cd_meter_rec.trans_type = 'U' OR cd_meter_rec.trans_type = 'D' THEN
               BEGIN
                  SELECT Global_Id
                    INTO FoundGlobalId
                    FROM Meter
                   WHERE Global_Id = cd_meter_rec.Global_Id
                     AND (REC_STATUS <> 'D' OR REC_STATUS is null);
               EXCEPTION
               WHEN NO_DATA_FOUND THEN
                  IF cd_meter_rec.trans_type ='U' THEN -- Update but meter doesn't exist
                     -- Convert U to I
                     -- copy any rec_status D records with this global id to the meter_del_dup table
                     INSERT INTO meter_del_dup
                        select * from meter
                         where global_id = cd_meter_rec.global_id;
                     -- change the service point id of the old deleted record to
                     -- allow the new one to be inserted and the uniqueness to be retained
                     UPDATE METER
                        SET service_point_id = DELETED_SERVICE_POINT_ID_SEQ.nextval
                      where global_id = cd_meter_rec.global_id;
                     commit;
                     v_TRANS_TYPE := 'I'; --UPSERT
                  ELSE -- Delete
                    UPDATE CD_METER SET PROC_FLG='P' WHERE ID= cd_meter_rec.ID;
                    COMMIT;
                    CONTINUE;
                  END IF;
               END;
            END IF;
            IF cd_meter_rec.trans_type='R' THEN
            -- no history of a spid ever getting an R transaction...
               BEGIN
                  SELECT Global_Id
                    INTO FoundGlobalId
                    FROM Meter
                   WHERE Global_Id = cd_meter_rec.Old_Global_Id
                     AND (REC_STATUS <> 'D' OR REC_STATUS is null);
               EXCEPTION
               WHEN no_data_found THEN
                  v_trans_type := 'I';
               END;
            END IF;

/* Truth table for insert:
                         GUID     SPID
                        Exists    Exists*   action to take
   Normal Case:         FALSE     FALSE     handle as an I-insert
   Handled exception 1:  TRUE     FALSE     Change transaction to U-update
   Error Condition      FALSE      TRUE     Log it as an error
   Error Condition       TRUE      TRUE     Log it as an error

                                        *SPID exists on a different meter record
*/

            IF v_TRANS_TYPE = 'I' THEN
               -- Does the SERVICE POINT ID exist on a meter record?
               SELECT COUNT(1)
                 INTO v_Rec_Count
                 FROM METER
                WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
                  AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL)
                  AND GLOBAL_ID <> cd_meter_rec.GLOBAL_ID;
               IF (v_Rec_Count > 0) THEN
                  ErrorCode := 'CD-MTR-08';
                  SELECT DESC_LONG || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID
                    INTO ErrorMsg
                    FROM CODE_LOOKUP
                   WHERE CODE_TYP = 'ERROR_CODE_CD'
                     AND CODE = ErrorCode;
                  LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  UPDATE CD_METER M SET PROC_FLG = 'E' WHERE M.ID = cd_meter_rec.ID;
                  COMMIT;
                  CONTINUE;
               END IF;
               SELECT count(*)
                 INTO v_Rec_Count
                 FROM meter
                WHERE global_id = cd_meter_rec.global_id
                  AND nvl(rec_status, 'I') <> 'D';
               IF v_Rec_Count > 0 THEN -- the global_id already exists, change to Update
                  v_trans_type := 'U';
               END IF;
            END IF;

            IF V_TRANS_TYPE = 'I' THEN
               INSERT INTO METER
                 (SERVICE_POINT_ID,              UNQSPID,
                  REV_ACCT_CD,                   SVC_ST_NUM,
                  SVC_ST_NAME,                   SVC_ST_NAME2,
                  SVC_CITY,                      SVC_STATE,
                  SVC_ZIP,                       CUST_TYP,
                  RATE_SCHED,                    GLOBAL_ID,
                  TRF_ID,                        SM_FLG,
                  METER_NUMBER,
                  SVC_PHASE )
               VALUES
                 (cd_meter_rec.SERVICE_POINT_ID, cd_meter_rec.UNQSPID,
                  cd_meter_rec.REV_ACCT_CD,      SUBSTR(cd_meter_rec.SVC_ST_NUM,1,12),
                  cd_meter_rec.SVC_ST_NAME,      cd_meter_rec.SVC_ST_NAME2,
                  cd_meter_rec.SVC_CITY,         cd_meter_rec.SVC_STATE,
                  cd_meter_rec.SVC_ZIP,          cd_meter_rec.CUST_TYP,
                  cd_meter_rec.RATE_SCHED,       cd_meter_rec.GLOBAL_ID,
                  transformer_id,                cd_meter_rec.SM_FLG,
                  cd_meter_rec.METER_NUMBER,     decode(cd_meter_rec.CUST_TYP, 'DOM', 1, 3));
               UPDATE CD_METER SET PROC_FLG = 'P' WHERE ID = cd_meter_rec.ID;
               COMMIT;
            END IF;

            IF v_TRANS_TYPE ='U' THEN
               -- count any other records with this service point id (should be none!)
               SELECT COUNT(1)
                INTO v_Rec_Count
                FROM METER
               WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
                 AND GLOBAL_ID       <> cd_meter_rec.GLOBAL_ID
                 AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
               IF (v_Rec_Count > 0 ) THEN
                  ErrorCode := 'CD-MTR-08';
                  SELECT DESC_LONG || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID
                    INTO ErrorMsg
                    FROM CODE_LOOKUP
                   WHERE CODE_TYP = 'ERROR_CODE_CD'
                     AND CODE = ErrorCode;
                  LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  UPDATE CD_METER M SET PROC_FLG = 'E' WHERE M.ID = cd_meter_rec.ID;
                  COMMIT;
                  CONTINUE;
               END IF;
               UPDATE METER
                  SET TRF_ID           = transformer_id,
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
                WHERE GLOBAL_ID        = cd_meter_rec.GLOBAL_ID
                  AND nvl(rec_status,'t') <> 'D';
               UPDATE CD_METER SET PROC_FLG = 'P' WHERE ID = cd_meter_rec.ID;
               COMMIT;
            ELSIF v_TRANS_TYPE = 'R' THEN
               -- Find Duplicate SERVICE POINT ID
               SELECT COUNT(1)
                 INTO v_Rec_Count
                 FROM METER
                WHERE SERVICE_POINT_ID = cd_meter_rec.SERVICE_POINT_ID
                  AND GLOBAL_ID       <> cd_meter_rec.OLD_GLOBAL_ID
                  AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
               IF ( v_Rec_Count > 0 ) THEN  --the spid is on another record, an error
                  v_Rec_Count := 0;
                  ErrorCode :='CD-MTR-08';
                  SELECT DESC_LONG || '. SERVICE_POINT_ID: ' || cd_meter_rec.SERVICE_POINT_ID
                    INTO ErrorMsg
                    FROM CODE_LOOKUP
                   WHERE CODE_TYP = 'ERROR_CODE_CD'
                     AND CODE = ErrorCode;
                  LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg, sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
                  UPDATE CD_METER M SET PROC_FLG = 'E' WHERE M.ID = cd_meter_rec.ID;
                  COMMIT;
                  CONTINUE;
               END IF;
               -- evidently someone thought a replace operation should update the history tables...
               -- retaining this code but... it needs to be checked if Change Detection ever
               -- sends Replace transaction types for service points...
               SELECT count(*), min(service_point_id)
                 INTO v_Rec_Count, Old_Service_Point_Id
                 FROM meter
                WHERE GLOBAL_ID = cd_meter_rec.old_global_id
                  AND (REC_STATUS <> 'D' OR REC_STATUS IS NULL);
               UPDATE METER
                  SET TRF_ID           = transformer_id,
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
                WHERE GLOBAL_ID        = cd_meter_rec.OLD_GLOBAL_ID
                  AND nvl(rec_status,'t') <> 'D';
               UPDATE CD_METER SET PROC_FLG = 'P' WHERE ID = cd_meter_rec.ID;
               COMMIT;

               UPDATE SP_PEAK_HIST S
                  SET S.SERVICE_POINT_ID = cd_meter_rec.Service_Point_Id
                WHERE S.SERVICE_POINT_ID = Old_Service_Point_Id;

            ELSIF v_TRANS_TYPE = 'D' THEN
            -- is there an already deleted record with this records spid ?
            -- if so move it to meter_del_dup and give it a fake spid, this lets the
            -- join on service point id for cyme find the record only 1 time
               select count(*)
                 into v_count
                 from meter
                where service_point_id = cd_meter_rec.service_point_id
                  and rec_status = 'D';
               IF V_count > 0 THEN
                  insert into meter_del_dup
                  select * from meter
                   where service_point_id = cd_meter_rec.service_point_id
                     and rec_status = 'D';
                  update meter set
                         service_point_id = DELETED_SERVICE_POINT_ID_SEQ.NEXTVAL,
                         global_id = decode(global_id,cd_meter_rec.GLOBAL_ID,
                            'GLOBAL_ID-'|| DELETED_SERVICE_POINT_ID_SEQ.NEXTVAL, global_id)
                   where service_point_id = cd_meter_rec.service_point_id
                     and rec_status = 'D';
               END IF;
               UPDATE METER SET REC_STATUS = 'D' WHERE GLOBAL_ID = cd_meter_rec.GLOBAL_ID
                  AND nvl(rec_status,'t') <> 'D';
               UPDATE CD_METER SET PROC_FLG= 'P' WHERE ID = cd_meter_rec.ID;
               COMMIT;
            ELSE
               NULL;
            END IF;
         EXCEPTION
         WHEN OTHERS THEN
            ErrorCode := SQLCODE;
            ErrorMsg  := SUBSTR(SQLERRM,1,200);
            LOG_ERRORS(cd_meter_rec.global_id,ErrorCode,ErrorMsg,sysdate, cd_meter_rec.trans_type,AppType,ProcedureFor,cd_meter_rec.Id);
            CONTINUE;
         END;
      COMMIT;
      END LOOP;
        -- add error handling for records with invalid trans_types
      INSERT INTO CD_ERRORS (
             GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
             TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
      select GLOBAL_ID,     'PROC1',       'Invalid Transaction Type', TRANS_DATE,
             TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, ID
        from cd_meter
       where nvl(trans_type, 'X') not in ('I','U','D','R')
         and proc_flg is null;
      update cd_meter
         set proc_flg = 'E'
       where nvl(trans_type, 'X') not in ('I','U','D','R')
         and proc_flg is null;

        -- add error handling for records with null trf_global_id's
      INSERT INTO CD_ERRORS (
              GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
              TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
       select GLOBAL_ID,     'CD-MTR-01',   'Transformer Global Id is null', TRANS_DATE,
              TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
         from cd_meter t
        where trf_global_id is null
          and proc_flg is null;
       update cd_meter t
          set proc_flg = 'E'
        where trf_global_id is null
          and proc_flg is null;

        -- add error handling for records with null global_id's
      INSERT INTO CD_ERRORS (
              GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
              TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
       select GLOBAL_ID,     'CD-MTR-05',   'GLOBAL_ID is null in CD_METER', TRANS_DATE,
              TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
         from cd_meter t
        where global_id is null
          and proc_flg is null;
       update cd_meter t
          set proc_flg = 'E'
        where global_id is null
          and proc_flg is null;

        -- add error handling for records with null service_point_id's
      INSERT INTO CD_ERRORS (
              GLOBAL_ID,     ERROR_CODE,    ERROR_MSG,    TRANS_DATE,
              TRANS_TYPE,    CREATE_DTM,    APP_TYPE,     PROC_FOR,     CD_TABLE_ID )
       select GLOBAL_ID,     'CD-MTR-06',   'SERVICE_POINT_ID is null in CD_METER', TRANS_DATE,
              TRANS_TYPE,    CURRENT_TIMESTAMP, AppType,  ProcedureFor, t.ID
         from cd_meter t
        where service_point_id is null
          and proc_flg is null;
       update cd_meter t
          set proc_flg = 'E'
        where service_point_id is null
          and proc_flg is null;

   INSERT INTO cd_meter_archive
        ( id, service_point_id, unqspid, rev_acct_cd, svc_st_num,
          svc_st_name, svc_st_name2, svc_city, svc_state,
          svc_zip, cust_typ, rate_sched, global_id, sm_flg,
          create_dtm, create_userid, update_dtm, update_userid,
          meter_number, trans_type, trans_date, old_global_id,
          trf_global_id, proc_flg)
   SELECT id, service_point_id, unqspid, rev_acct_cd, svc_st_num,
          svc_st_name, svc_st_name2, svc_city, svc_state,
          svc_zip, cust_typ, rate_sched, global_id, sm_flg,
          create_dtm, create_userid, update_dtm, update_userid,
          meter_number, trans_type, trans_date, old_global_id,
          trf_global_id, proc_flg
     FROM cd_meter
    WHERE proc_flg is not null;

   DELETE FROM cd_meter WHERE proc_flg is not null;

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

  dbms_output.put_line ('done' ) ;
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
  v_stmt      VARCHAR2 (2000);
  v_retval    VARCHAR2 ( 500)  := 'FALSE';
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
