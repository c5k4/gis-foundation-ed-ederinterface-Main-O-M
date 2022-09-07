--------------------------------------------------------
--  DDL for Package Body SVR_CONFIG_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."SVR_CONFIG_UTIL" 
/***********************************************************************
*
*N  {svr_config_util.spb}  --  Implementation for server config DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on the server config table.  It should be 
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
*    Jose Kuruvilla       04/14/2002               Original coding.
*E
***********************************************************************/
IS

   /* Public Subprograms. */

   PROCEDURE insert_server_config (in_prop_name       IN prop_name_t,
                                   in_char_prop_value IN char_prop_value_t, 
                                   in_num_prop_value  IN num_prop_value_t) 
  /***********************************************************************
  *
  *N  {insert_server_config}  --  Add record to server_config table *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure adds an entry to the SDE.server_config table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    in_prop_name       <IN> == (prop_name_t)  Property name to be entered.
  *    in_char_prop_value <IN> == (char_prop_value_t)  Character type property
  *                                                value to be entered.
  *    in_num_prop_value  <IN> == (num_prop_value_t)  Numeric type property
  *                                                   value to be entered.
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
  *    Jose Kuruvilla            04/14/2002               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      BEGIN
      
        INSERT INTO SDE.server_config
                (prop_name, 
                 char_prop_value,
                 num_prop_value)
         VALUES (in_prop_name, 
                 in_char_prop_value,
                 in_num_prop_value);
         EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
      END;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END insert_server_config;

   PROCEDURE truncate_server_config
 /***********************************************************************
  *
  *N  {truncate_server_config}  --  Truncate server_config table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure delete all data from server_config table.
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
  *    Jose Kuruvilla         04/14/2002               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Delete all rows from server_config table.  We use DELETE instead of
      -- TRUNCATE as TRUNCATE would cause a commit. For SERVER_CONFIG table,
      -- please note: 
      -- we commit only after inserting all records successfully by calling 
      -- truncate_server_config.

     BEGIN
      DELETE FROM SDE.server_config;
      EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
     END;
   END truncate_server_config;

   PROCEDURE update_server_config (in_prop_name       IN prop_name_t,
                                   in_char_prop_value IN char_prop_value_t, 
                                   in_num_prop_value  IN num_prop_value_t) 
  /***********************************************************************
  *
  *N  {update_server_config}  --  Update a server_config column entry. *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a column entry in the SDE.server_config table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    in_prop_name       <IN> == (prop_name_t)  Property name to be entered.
  *    in_char_prop_value <IN> == (char_prop_value_t)  Character type property
  *                                                value to be entered.
  *    in_num_prop_value  <IN> == (num_prop_value_t)  Numeric type property
  *                                                   value to be entered.
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
  *    Jose Kuruvilla            09/30/2003               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      BEGIN
      
        UPDATE SDE.server_config
        SET prop_name   = in_prop_name,
        char_prop_value = in_char_prop_value,
        num_prop_value  = in_num_prop_value 
        WHERE prop_name = in_prop_name;
         EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
      END;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END update_server_config;

   PROCEDURE delete_server_config (in_prop_name       IN prop_name_t) 
 /***********************************************************************
  *
  *N  {delete_server_config}  --  Delete an server_config table entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an entry in the server_config table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    in_prop_name       <IN> == (prop_name_t)  Property name to be entered.
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
  *    Jose Kuruvilla         09/30/2003               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Deletes one row from server_config table.

     BEGIN
      DELETE FROM SDE.server_config WHERE prop_name = in_prop_name;
      EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
     END;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END delete_server_config;

END svr_config_util;
