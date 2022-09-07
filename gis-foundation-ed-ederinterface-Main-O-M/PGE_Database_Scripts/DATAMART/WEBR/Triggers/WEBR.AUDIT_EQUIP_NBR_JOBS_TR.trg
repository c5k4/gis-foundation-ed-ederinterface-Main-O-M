Prompt drop Trigger AUDIT_EQUIP_NBR_JOBS_TR;
DROP TRIGGER WEBR.AUDIT_EQUIP_NBR_JOBS_TR
/

Prompt Trigger AUDIT_EQUIP_NBR_JOBS_TR;
--
-- AUDIT_EQUIP_NBR_JOBS_TR  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.AUDIT_EQUIP_NBR_JOBS_TR
 BEFORE
   UPDATE OF job_#, div_#, description
 ON WEBR.EQUIP_NBR_JOBS
REFERENCING NEW AS NEW OLD AS OLD
 FOR EACH ROW
Begin
   :new.Last_modified := SYSDATE;
   :new.User_audit := user_audit_text(USER, :old.user_audit);
End;
/
