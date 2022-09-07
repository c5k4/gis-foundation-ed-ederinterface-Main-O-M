--------------------------------------------------------
--  DDL for Package Body SDE_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."SDE_UTIL" 
/***********************************************************************
*
*N  {sde_util.spb}  --  Implementation for globally useful functions
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements globally useful functions.  
*   It should be compiled by the SDE DBA user; security is by user name.   
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
*    Peter Aronson             06/04/04               Original coding.
*E
***********************************************************************/
IS

   G_current_user          identifier_t;
   tmp_user                identifier_t;

   FUNCTION sde_user RETURN NVARCHAR2
  /***********************************************************************
  *
  *N  {sde_user}  --  Return the current user name, quoted if necessary
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     Return the current user's user name, quoted if it contains 
  *   that call for it characters.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN     <OUT>  ==  (NVARCHAR2) The current user name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             06/03/04                 Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      RETURN G_current_user;

   END sde_user;

   PROCEDURE set_table_last_modified (tlm IN tlm_record_t)
  /***********************************************************************
  *
  *N  {set_table_last_modified}  --  updates the time_last_modified
  *        value in the sde_tables_modified table, and inserts an 
  *        entry into the sde_tables_modified table if necessary.  
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the time_last_modified column of the 
  *   sde_tables_modified for a specified table, and inserts a new 
  *   entry for the table if necessary.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     tlm  <IN>         == (tlm_record_t) The sde_tables_modified record.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago          04/03/06              Original coding.
  *E
  ***********************************************************************/
   IS
      PRAGMA AUTONOMOUS_TRANSACTION;
  
      -- Update the time_last_modified column for this table.

      CURSOR check_tlm_cursor(check_table_name IN VARCHAR2) IS 
       SELECT time_last_modified
       FROM SDE.sde_tables_modified
       WHERE table_name = check_table_name;
       
      found_time_last_modified DATE;
      new_time_last_modified   DATE;
      a_second                 NUMBER := 1/(24 * 60 * 60);
      tlm_found                BOOLEAN;
      
   BEGIN
      OPEN check_tlm_cursor(tlm.table_name); 
      FETCH check_tlm_cursor INTO found_time_last_modified;
      tlm_found := check_tlm_cursor%FOUND;
      CLOSE check_tlm_cursor;
       
      IF tlm_found THEN         
      
       IF tlm.time_last_modified > found_time_last_modified THEN
         new_time_last_modified := tlm.time_last_modified;
       ELSE
         new_time_last_modified := found_time_last_modified + a_second;
       END IF;   
          
       UPDATE SDE.sde_tables_modified 
       SET sde_tables_modified.time_last_modified = new_time_last_modified
       WHERE sde_tables_modified.table_name = tlm.table_name;         

      ELSE
 
       INSERT INTO SDE.sde_tables_modified
           (table_name, time_last_modified)
       VALUES (tlm.table_name, tlm.time_last_modified);

      END IF;

      COMMIT;
       
      EXCEPTION
       WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END set_table_last_modified;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   -- See if there are any characters in the user name that need to be quoted.

   tmp_user := TO_NCHAR( TRANSLATE (USER,
                          ' %&()*+,-./:;<=>?@[]\^`abcdefghijklmnopqrstuvwxyz{|}~',
                          '|!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!') );
   IF INSTR (tmp_user, TO_NCHAR('!') ) = 0 THEN
      IF INSTR (TRANSLATE (SUBSTR (tmp_user,1,1),
                        N'123456789$#',
                           N'00000000000'),'0') = 0 THEN
         G_current_user := TO_NCHAR(USER);
      END IF;
   END IF;

   IF G_current_user IS NULL THEN
      G_current_user := TO_NCHAR('"' || USER || '"');
   END IF;

END sde_util;
