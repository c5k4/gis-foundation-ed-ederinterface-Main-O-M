Prompt drop Trigger REPLACE_NEWLINE_SPACE;
DROP TRIGGER EDGIS.REPLACE_NEWLINE_SPACE
/

Prompt Trigger REPLACE_NEWLINE_SPACE;
--
-- REPLACE_NEWLINE_SPACE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.REPLACE_NEWLINE_SPACE
/******************************************************************************
   NAME:       REPLACE_NEWLINE_SPACE
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        1/18/2017      S2NN       1. Created this trigger.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     REPLACE_NEWLINE_SPACE
      Sysdate:         1/18/2017
      Date and Time:   1/18/2017, 11:51:06 AM, and 1/18/2017 11:51:06 AM
      Username:        S2NN (set in TOAD Options, Proc Templates)
      Table Name:      PGE_GISSAP_ASSETSYNCH (set in the "New PL/SQL Object" dialog)
      Trigger Options:  (set in the "New PL/SQL Object" dialog)
******************************************************************************/
BEFORE INSERT
ON EDGIS.PGE_GISSAP_ASSETSYNCH
REFERENCING NEW AS New OLD AS Old
FOR EACH ROW
BEGIN
    :New.SAPATTRIBUTES := REPLACE(REPLACE(:New.SAPATTRIBUTES, chr(13),' '), chr(10), ' ');
END REPLACE_NEWLINE_SPACE;
/
