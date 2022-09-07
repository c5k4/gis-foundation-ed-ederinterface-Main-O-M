--------------------------------------------------------
--  DDL for Function BACKUP_FULL_LOAD
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "PGEDATA"."BACKUP_FULL_LOAD" 
RETURN number
IS
/*
 Purpose:  Store a copy of the monthly full synch data in a backup table
           for troubleshooting and analysis purposes.

           the table pge_ccbtogis_stg_bkup will be truncated/dropped and recreated
           using the existing pge_ccbtogis_stg table.

           The backup will only be created when a full extract of ccb data is
           sent.  This will be determined based on the existance of more then
           5.5 million service point records with null actions in the staging
           table.

           Currently, a failure to create the backup is not a cause for the
           interface to stop.

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        02/07/20    Initial coding

need priv granted to do this:  GRANT CREATE TABLE TO pgedata;
*/

   v_count     NUMBER := 0;

   v_routine   VARCHAR2(64) := 'PGEDATA.Backup_full_load';
   v_hint      INTEGER := 0;

BEGIN
   -- do we have 5.5m records staged with action null?
   SELECT count(*)
     INTO v_count
     FROM pge_ccbtoedgis_stg
    WHERE action IS null ;

   IF v_count > 5500000 THEN
      -- drop & rebuild table - self manages cases when the table definition changes
      -- does the table exist?
      SELECT count(*)
        INTO v_count
        FROM user_tables
       WHERE table_name = 'PGE_CCBTOEDGIS_STG_BKUP';
      IF v_count = 1 THEN -- yes the table exists
         v_hint := 10;
         EXECUTE IMMEDIATE 'drop table PGE_CCBTOEDGIS_STG_BKUP purge';
      END IF;
      v_hint := 20;
      EXECUTE IMMEDIATE 'create table PGE_CCBTOEDGIS_STG_BKUP as select * from PGE_CCBTOEDGIS_STG where action is null' ;
   END IF;

   RETURN 0;
EXCEPTION
    WHEN OTHERS THEN
        dbms_output.put_line('Found Oracle error in '||v_routine || ' at hint ' || v_hint || ' ' || SQLERRM);
        RETURN -1;
END Backup_full_load;
