--------------------------------------------------------
--  DDL for Package Body LOCATOR_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."LOCATOR_UTIL" 
/***********************************************************************
*
*N  {locator_util.spb}  --  Implementation for locator DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on locator.  It should be compiled by the
*   SDE DBA user; security is by user name.   
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
*    Yung-Ting Chen           05/03/99               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;

   CURSOR G_locator_owner_cursor (wanted_locator_id  IN  locator_id_t) IS
     SELECT owner
     FROM   SDE.locators 
     WHERE  locator_id = wanted_locator_id;

   /* Local Subprograms. */

   PROCEDURE L_locator_user_can_modify (wanted_locator_id IN locator_id_t)
  /***********************************************************************
  *
  *N  {L_locator_user_can_modify}  --  Can current user modify locator?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the locator specified by ID exists and is
  *   modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_locator_id  <IN>  ==  (locator_id_t) Locator to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20275                SE_LOCATOR_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen          05/03/99           Original coding.
  *E
  ***********************************************************************/
   IS

      locator_owner           SDE.locators.owner%TYPE;

   BEGIN

      -- Make sure that the locator exists, and that the current user can
      -- write to it.

      OPEN G_locator_owner_cursor (wanted_locator_id);
      FETCH G_locator_owner_cursor INTO locator_owner;
      IF G_locator_owner_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_LOCATOR_NOEXIST,
                                  'Locator ' || 
                                  TO_CHAR (wanted_locator_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_locator_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != locator_owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of locator ' ||
                                     TO_CHAR (wanted_locator_id) || '.');
         END IF;
      END IF;
    
   END L_locator_user_can_modify;

   /* Public Subprograms. */

   PROCEDURE insert_locator (locator            IN  locator_record_t,
                             num_properties     IN  integer,
                             record_id_list     IN  record_id_list_t,
                             property_list      IN  property_list_t,
                             value_list         IN  value_list_t)
  /***********************************************************************
  *
  *N  {insert_locator}  --  insert a locator entry into the SDELOCATORS
  *                       table and SDEMETADATA table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the
  *   <schema>.SDELOCATORS table and <schema>.SDEMETADATA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     locator           <IN>  == (locator_record_t) The new locator to be
  *                               inserted.
  *     num_properties    <IN>  == (integer) Number of property/value pairs.
  *     record_id_list    <IN>  == (record_id_list_t) Record Id list of the
  *                               metadata records.
  *     property_list     <IN>  == (property_list_t) Property list of the
  *                               metadata records.
  *     value_list        <IN>  == (value_list_t) Value list of the
  *                               metadata records.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen          05/03/99           Original coding.
  *E
  ***********************************************************************/
   IS

     i INTEGER;
     metadata_list SDE.metadata_util.metadata_record_list_t;

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a locator is inserted

      BEGIN

      -- Insert the record into the locators table.

        INSERT INTO SDE.locators 
         (locator_id,name,owner,category,type,description)
        VALUES (locator.locator_id,
                locator.name,
                locator.owner,
                locator.category,
                locator.type,
                locator.description);

      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error(sde_util.SE_LOCATOR_EXISTS,
                                     'Locator ' ||
                                     TO_CHAR (locator.locator_id) ||
                                     ' already exists.');
      END;

      -- Insert the record into the metadata table

      IF num_properties > 0 THEN
         FOR i in 1..num_properties LOOP
           metadata_list(i).record_id     := record_id_list(i);
           metadata_list(i).object_name   := locator.name;
           metadata_list(i).object_owner  := locator.owner;
           metadata_list(i).object_type   := 
                          SDE.metadata_util.C_metadata_object_type_locator;
           metadata_list(i).class_name    := C_metadata_class_name_locator;
           metadata_list(i).property      := property_list(i);
           metadata_list(i).prop_value    := value_list(i);
           metadata_list(i).description   := NULL;
           metadata_list(i).creation_date := SYSDATE;
         END LOOP;
 
         SDE.metadata_util.insert_metadata(metadata_list);
      END IF;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;


   END insert_locator;


   PROCEDURE delete_locator (old_locator_id     IN  locator_id_t)
  /***********************************************************************
  *
  *N  {delete_locator}  --  Delete an arbitary locator and metadata.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary locator and metadata entry.
  *   All checking and locking should be performed by the gsrvr, except
  *   that we will check locator ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_locator_id  <IN>  ==  (locator_id_t)  locator to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20275                SE_LOCATOR_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen         05/03/99           Original coding.
  *E
  ***********************************************************************/
   IS

      delete_select_cursor SDE.metadata_util.delete_info_cursor_t;

   BEGIN

      -- Make sure locator exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_locator_user_can_modify (old_locator_id);

      -- We've verified our permissions and the locator's existence,
      -- so it must be OK to delete them.  Do it.


      -- Delete property/values pairs entries from metadata table.

      OPEN delete_select_cursor FOR
        SELECT record_id 
        FROM SDE.metadata WHERE
        object_type = SDE.metadata_util.C_metadata_object_type_locator
        AND (object_name,object_owner) =
         (SELECT name,owner from SDE.locators WHERE
          locator_id = old_locator_id);

      SDE.metadata_util.delete_metadata(delete_select_cursor);

      CLOSE delete_select_cursor;


      -- Delete table metadata records that associated with the locator.

      OPEN delete_select_cursor FOR
        SELECT record_id
        FROM SDE.metadata WHERE
        object_type = SDE.metadata_util.C_metadata_object_type_table
        AND class_name = C_metadata_class_name_locator
        AND property = 'Locator ID' and prop_value = TO_CHAR(old_locator_id);

      SDE.metadata_util.delete_metadata(delete_select_cursor);

      CLOSE delete_select_cursor;

      -- Delete locator from locators.

      DELETE FROM SDE.locators WHERE
             locator_id = old_locator_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END delete_locator;



   PROCEDURE update_locator (locator            IN  locator_record_t,
                             num_properties     IN  integer,
                             record_id_list     IN  record_id_list_t,
                             property_list      IN  property_list_t,
                             value_list         IN  value_list_t)
  /***********************************************************************
  *
  *N  {update_locator}  --  Update a locator
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a locator.
  *   All checking and locking should be performed by the gsrvr,
  *   except that we will check locator ownership if the invoking user
  *   is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     locator           <IN>  == (locator_record_t) The new locator to be
  *                               updated.
  *     num_properties    <IN>  == (integer) Number of property/value pairs.
  *     record_id_list    <IN>  == (record_id_list_t) Record id list of the
  *                               metadata records.
  *     property_list     <IN>  == (property_list_t) Property list of the
  *                               metadata records.
  *     value_list        <IN>  == (value_list_t) Value list of the
  *                               metadata records.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20275                SE_LOCATOR_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen           05/03/99           Original coding.
  *E
  ***********************************************************************/
   IS

     i                    INTEGER;
     metadata_list        SDE.metadata_util.metadata_record_list_t;
     delete_select_cursor SDE.metadata_util.delete_info_cursor_t;
     old_locator_name     SDE.locators.name%TYPE;

   BEGIN

      -- Make sure that the locator exists, and that the current user can
      -- write to it.

       L_locator_user_can_modify (locator.locator_id);

      -- Get the old locator name.
 
       SELECT name   
       INTO   old_locator_name
       FROM   SDE.locators
       WHERE  locator_id = locator.locator_id;

      -- Update the locators.

      UPDATE SDE.locators
      SET   name = locator.name,
            category = locator.category,
            type = locator.type,
            description = locator.description
      WHERE locator_id = locator.locator_id;

      -- Modify the property/value pairs entries in metadata table.

      OPEN delete_select_cursor FOR
       SELECT record_id
       FROM SDE.metadata WHERE
       object_type = SDE.metadata_util.C_metadata_object_type_locator
       AND object_name = old_locator_name 
       AND object_owner = locator.owner;

      SDE.metadata_util.delete_metadata(delete_select_cursor);

      CLOSE delete_select_cursor;

      IF  num_properties > 0 THEN
        FOR i in 1..num_properties LOOP
          metadata_list(i).record_id     := record_id_list(i);
          metadata_list(i).object_name   := locator.name;
          metadata_list(i).object_owner  := locator.owner;
          metadata_list(i).object_type   := 
                        SDE.metadata_util. C_metadata_object_type_locator;
          metadata_list(i).class_name    := C_metadata_class_name_locator;
          metadata_list(i).property      := property_list(i);
          metadata_list(i).prop_value    := value_list(i);
          metadata_list(i).description   := NULL;
          metadata_list(i).creation_date := SYSDATE;
        END LOOP;

        SDE.metadata_util.insert_metadata(metadata_list); 
      END IF;

    
      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_locator;

BEGIN

/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END locator_util;
