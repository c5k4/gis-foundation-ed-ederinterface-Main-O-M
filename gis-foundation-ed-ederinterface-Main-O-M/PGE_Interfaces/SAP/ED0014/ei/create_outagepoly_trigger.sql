CREATE OR REPLACE TRIGGER "WEBR"."PGE_OUTAGEPOLY_INSERT" 
BEFORE INSERT ON PGE_OUTAGEPOLY
FOR EACH ROW 
  DECLARE OBJECTID PGE_OUTAGEPOLY.objectid%TYPE;
  
BEGIN
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',201);
  :new.objectid := objectid;
END;