Prompt drop Trigger SM_SPECIAL_LOAD_AFT_UPD;
DROP TRIGGER EDSETT.SM_SPECIAL_LOAD_AFT_UPD
/

Prompt Trigger SM_SPECIAL_LOAD_AFT_UPD;
--
-- SM_SPECIAL_LOAD_AFT_UPD  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_SPECIAL_LOAD_AFT_UPD
AFTER UPDATE ON EDSETT.SM_SPECIAL_LOAD FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
     INSERT INTO SM_SPECIAL_LOAD_HIST
         ( REF_GLOBAL_ID,LOAD_DESCRIPTION,S_KW,W_KW,S_KVAR,W_KVAR, DEVICE_TYPE, SM_SPECIAL_LOAD_ID,CREATE_DTM,CREATE_USERID ,UPDATE_DTM ,UPDATE_USERID )
         values ( :OLD.REF_GLOBAL_ID,:OLD.LOAD_DESCRIPTION,:OLD.S_KW,:OLD.W_KW,:OLD.S_KVAR,:OLD.W_KVAR, :OLD.DEVICE_TYPE, :OLD.ID, :OLD.CREATE_DTM,:OLD.CREATE_USERID ,:OLD.UPDATE_DTM ,:OLD.UPDATE_USERID );


   END COLUMN_SEQUENCES;
END;
/