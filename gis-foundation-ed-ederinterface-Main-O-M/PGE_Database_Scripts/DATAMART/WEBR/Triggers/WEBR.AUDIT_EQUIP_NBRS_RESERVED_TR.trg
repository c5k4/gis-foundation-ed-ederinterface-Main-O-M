Prompt drop Trigger AUDIT_EQUIP_NBRS_RESERVED_TR;
DROP TRIGGER WEBR.AUDIT_EQUIP_NBRS_RESERVED_TR
/

Prompt Trigger AUDIT_EQUIP_NBRS_RESERVED_TR;
--
-- AUDIT_EQUIP_NBRS_RESERVED_TR  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.AUDIT_EQUIP_NBRS_RESERVED_TR
 BEFORE
   UPDATE OF id_typ, oper_#, address
 ON WEBR.EQUIP_NBRS_RESERVED
REFERENCING NEW AS NEW OLD AS OLD
 FOR EACH ROW
Begin
   :new.Last_modified := SYSDATE;
   :new.User_audit := user_audit_text(USER, :old.user_audit);
End;
/
