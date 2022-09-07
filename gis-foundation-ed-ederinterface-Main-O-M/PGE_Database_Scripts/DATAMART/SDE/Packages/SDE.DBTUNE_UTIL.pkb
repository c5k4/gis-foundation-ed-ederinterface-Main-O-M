Prompt drop Package Body DBTUNE_UTIL;
DROP PACKAGE BODY SDE.DBTUNE_UTIL
/

Prompt Package Body DBTUNE_UTIL;
--
-- DBTUNE_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.dbtune_util
/***********************************************************************
*
*N  {dbtune_util.spb}  --  Implementation for dbtune DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on the DBTune table.  It should be 
*   compiled by the SDE DBA user; security is by user name.   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2004 ESRI
*
*   TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
*   Unpublished material - all rights reserved under the
*   Copyright Laws of the United States.
*
*   For additional information, contact:
*   Environmental Systems Research Institute, Inc.
*   Attn: Contracts Dept
*   380 New York Street
*   Redlands, California, USA 92373
*
*   email: contracts@esri.com
*   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Jose Kuruvilla       04/14/2000               Original coding.
*E
***********************************************************************/
IS

   /* Public Subprograms. */

   PROCEDURE update_dbtune (in_keyword        IN  keyword_t,
                            in_parameter_name IN parameter_name_t,
                            in_config_string  IN NCLOB)
  /***********************************************************************
  *
  *N  {update_dbtune}  --  Update dbtune record 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a dbtune record in DBTUNE table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    keyword <IN>        == (keyword_t)  keyword value to be entered.
  *    parameter_name <IN> == (parameter_name_t)  Parameter_name to be entered.
  *    config_string  <IN> == (NCLOB)  config_string value to be entered.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jose Kuruvilla            07/20/2004               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      UPDATE SDE.dbtune
      SET    config_string = in_config_string
      WHERE keyword = in_keyword AND
            parameter_name = in_parameter_name;
      COMMIT;

   END update_dbtune;

   PROCEDURE insert_dbtune (in_keyword        IN  keyword_t,
                            in_parameter_name IN parameter_name_t,
                            in_config_string  IN NCLOB)

  /***********************************************************************
  *
  *N  {insert_dbtune}  --  Add record to dbtune 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure adds an entry to the <schema>.dbtune table. We do not call 
  *   COMMIT in this function because we wait until all data are 
  *   successfully inserted into the dbtune table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    keyword <IN>        == (keyword_t)  keyword value to be entered.
  *    parameter_name <IN> == (parameter_name_t)  Parameter_name to be entered.
  *    config_string  <IN> == (NCLOB)  config_string value to be entered.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jose Kuruvilla            04/14/2000               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      BEGIN
      
        INSERT INTO SDE.dbtune
                (keyword,
                 parameter_name,
                 config_string)
         VALUES (in_keyword,
                 in_parameter_name,
                 in_config_string);
         COMMIT;

         EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
      END;

   END insert_dbtune;

   PROCEDURE truncate_dbtune 
  /***********************************************************************
  *
  *N  {truncate_dbtune}  --  Truncate dbtune 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure delete all data from dbtune table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jose Kuruvilla         04/17/2000               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Delete all rows from dbtune table.  We use DELETE instead of
      -- TRUNCATE as TRUNCATE would cause a commit. For DBTUNE table, please note: 
      -- we commit only after inserting all records successfully by calling 
      -- insert_dbtune.

     BEGIN
      DELETE FROM SDE.dbtune;
      EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
     END;

   END truncate_dbtune;

   PROCEDURE delete_dbtune (in_keyword        IN  keyword_t,
                            in_parameter_name IN parameter_name_t)
  /***********************************************************************
  *
  *N  {delete_dbtune}  --  Delete dbtune record 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure delete dbtune records in DBTUNE table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    keyword <IN>        == (keyword_t)  keyword value to be entered.
  *    parameter_name <IN> == (parameter_name_t)  Parameter_name to be entered.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jose Kuruvilla            08/20/2004               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      IF in_parameter_name IS NOT NULL THEN
        DELETE FROM SDE.dbtune
        WHERE keyword = in_keyword AND
            parameter_name = in_parameter_name;
      ELSE
        DELETE FROM SDE.dbtune
        WHERE keyword = in_keyword;
      END IF;

      -- Make sure that something was deleted.

      IF SQL%NOTFOUND THEN
         RAISE NO_DATA_FOUND;
      END IF;
      COMMIT; 
   END delete_dbtune;

END dbtune_util;

/


Prompt Grants on PACKAGE DBTUNE_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.DBTUNE_UTIL TO PUBLIC WITH GRANT OPTION
/
