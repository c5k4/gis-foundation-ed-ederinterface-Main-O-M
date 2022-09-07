Prompt drop Package SDO_SEM_DOWNGRADE;
DROP PACKAGE MDSYS.SDO_SEM_DOWNGRADE
/

Prompt Package SDO_SEM_DOWNGRADE;
--
-- SDO_SEM_DOWNGRADE  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_sem_downgrade authid current_user as

  /**
   * This method will save all model data, rulebase data before an actual
   * database downgrade.
   */
  procedure prepare_downgrade_from_11;

END;
/


Prompt Synonym SDO_SEM_DOWNGRADE;
--
-- SDO_SEM_DOWNGRADE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_SEM_DOWNGRADE FOR MDSYS.SDO_SEM_DOWNGRADE
/
