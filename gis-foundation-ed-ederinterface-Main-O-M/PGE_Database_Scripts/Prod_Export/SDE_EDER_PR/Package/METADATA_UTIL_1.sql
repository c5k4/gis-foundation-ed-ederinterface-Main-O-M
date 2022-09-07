--------------------------------------------------------
--  DDL for Package Body METADATA_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."METADATA_UTIL" 
/***********************************************************************
*
*N  {metadata_util.sps}  --  Implementation for metadata DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on the SDE metadata table.  It should be compiled by 
*   the SDE DBA user; security is by user name.   
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
*    Peter Aronson             12/09/98               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;

   /* Local Subprograms. */

   PROCEDURE L_metadata_user_can_modify (record_id  IN  record_id_t)
  /***********************************************************************
  *
  *N  {L_metadata_user_can_modify}  --  Can current user modify metadata record?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the metadata record specified by ID exists 
  *   and is modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     record_id  <IN>  ==  (record_id_t) Metadata record to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20???                SE_METADATA_RECORD_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/19/99           Original coding.
  *E
  ***********************************************************************/
   IS

      CURSOR metadata_owner_cursor (record_wanted_id  IN  record_id_t) IS
        SELECT m.object_owner
        FROM   SDE.metadata m
        WHERE  m.record_id = record_wanted_id;
      metadata_record_owner           metadata_owner_cursor%ROWTYPE;

   BEGIN

      -- Make sure that the metadata record exists, and that the current 
      -- user can write to it.

      OPEN metadata_owner_cursor (record_id);
      FETCH metadata_owner_cursor INTO metadata_record_owner;
      IF metadata_owner_cursor%NOTFOUND THEN
         CLOSE metadata_owner_cursor;
         raise_application_error (sde_util.SE_METADATA_RECORD_NOEXIST,
                                  'Metadata Record ' || TO_CHAR (record_id) ||
                                  ' not found.');
      END IF;
      CLOSE metadata_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != metadata_record_owner.object_owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of metadata_record ' ||
                                     TO_CHAR (record_id) || '.');
         END IF;
      END IF;
    
   END L_metadata_user_can_modify;

   
   /* Public Subprograms. */

   PROCEDURE  insert_metadata (metadata_list  IN  metadata_record_list_t)
  /***********************************************************************
  *
  *N  {insert_metadata}  --  Insert list of metadata records into metadata
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied list of metadata records
  *   into the SDE.SDEMETADATA table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     metadata_list    <IN>  ==  (metadata_record_list_t) The list of
  *                                 metadata records to INSERT.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20096                SE_NOT_TABLE_OWNER.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/08/99           Original coding.
  *E
  ***********************************************************************/
   IS
 
      metadata_record_index  INTEGER;

   BEGIN

      -- Loop through the metadata records, inserting each in turn.

      metadata_record_index := metadata_list.FIRST;
      WHILE metadata_record_index IS NOT NULL LOOP
         IF metadata_list(metadata_record_index).object_owner <> 
            G_current_user THEN
            raise_application_error (sde_util.SE_NOT_TABLE_OWNER,
                                     'Current user ' || TO_CHAR (G_current_user) ||
                                     ' is not the owner of object ' ||
                           TO_CHAR (metadata_list(metadata_record_index).object_owner) ||
                           TO_CHAR (metadata_list(metadata_record_index).object_name) ||
                                     '.');
         END IF;
         INSERT INTO SDE.metadata
         VALUES (metadata_list(metadata_record_index).record_id,
                 metadata_list(metadata_record_index).object_name,
                 metadata_list(metadata_record_index).object_owner,
                 metadata_list(metadata_record_index).object_type,
                 metadata_list(metadata_record_index).class_name,
                 metadata_list(metadata_record_index).property,
                 metadata_list(metadata_record_index).prop_value,
                 metadata_list(metadata_record_index).description,
                 metadata_list(metadata_record_index).creation_date);
         metadata_record_index := metadata_list.NEXT (metadata_record_index);
      END LOOP;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END insert_metadata;


   PROCEDURE  delete_metadata (record_id_list  IN  record_id_list_t)
  /***********************************************************************
  *
  *N  {delete_metadata}  --  Delete an arbitary list of metadata records
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary list of metadata records.  All 
  *   checking and locking should be performed by the gsrvr, except that we
  *   will check metadata record ownership if the invoking user is not the
  *   SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     record_id_list  <IN>  ==  (record_id_list_t) List of metadata 
  *                                records (by id) to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20???                SE_METADATA_RECORD_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/15/99           Original coding.
  *E
  ***********************************************************************/
   IS

      record_id_index        INTEGER;
      
   BEGIN

      -- Loop through the table of record ids, fetching each metadata record
      -- in turn to make sure it exists, and (if we are not the SDE DBA) that
      -- the current user owns it.

      record_id_index := record_id_list.FIRST;
      WHILE record_id_index IS NOT NULL LOOP
         L_metadata_user_can_modify (record_id_list (record_id_index));
         record_id_index := record_id_list.NEXT (record_id_index);
      END LOOP;

      -- We've verified our permissions and the metadata records' existence, 
      -- so it must be OK to delete them.  Do it.

      record_id_index := record_id_list.FIRST;
      WHILE record_id_index IS NOT NULL LOOP
         DELETE FROM SDE.metadata 
         WHERE   record_id = record_id_list (record_id_index);
         record_id_index := record_id_list.NEXT (record_id_index);
      END LOOP;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_metadata;


   PROCEDURE  delete_metadata (delete_select_cursor  IN  delete_info_cursor_t)
  /***********************************************************************
  *
  *N  {delete_metadata}  --  Delete an arbitary list of metadata records
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary list of metadata records, as 
  *   indicated by the supplied cursor.  All checking and locking should be
  *   performed by the gsrvr, except that we will check metadata record 
  *   ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     delete_select_cursor  <IN>  ==  (delete_info_cursor_t) A cursor
  *                                      for a query that will supply a
  *                                      list of metadata record ids.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20???                SE_METADATA_RECORD_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/20/99           Original coding.
  *E
  ***********************************************************************/
   IS

      record_id_list        record_id_list_t;
      null_list             record_id_list_t;
      record_id_list_index  BINARY_INTEGER;
      metadata_info         metadata_info_t;

   BEGIN

      -- Use the cursor to build a table/list of the ids of the metadata 
      -- records to be deleted.

      record_id_list_index := 1;
      FETCH delete_select_cursor INTO metadata_info;
      WHILE delete_select_cursor%FOUND LOOP
         record_id_list (record_id_list_index) := metadata_info.record_id;
         record_id_list_index := record_id_list_index + 1;
         FETCH delete_select_cursor INTO metadata_info;
      END LOOP;

      -- Use the other metadata_delete procedure to do the actual deletion
      -- and error checking.

      delete_metadata (record_id_list);

      -- If the list ended up humongous, free up space.

      IF record_id_list_index > SDE.sde_util.C_free_threshold THEN
         record_id_list := null_list;
         DBMS_SESSION.FREE_UNUSED_USER_MEMORY;
      END IF;

   END delete_metadata;


   PROCEDURE  update_metadata (new_metadata  IN  metadata_record_t)
  /***********************************************************************
  *
  *N  {update_metadata}  --  Update a specified (by id) metadata record
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the updatable fields of the supplied metadata
  *   record (as identified by id).  All checking and locking should be
  *   performed by the gsrvr, except that we will check metadata record 
  *   ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     metadata  <IN>  ==  (metadata_record_t) The metadata record 
  *                          containing id and new values for the metadata
  *                          record to update.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20???                SE_METADATA_RECORD_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/20/99           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Check permissions.

      L_metadata_user_can_modify (new_metadata.record_id);

      -- Perform the update.

      UPDATE SDE.metadata
      SET    class_name    = new_metadata.class_name,
             property      = new_metadata.property,
             prop_value    = new_metadata.prop_value,
             description   = new_metadata.description,
             creation_date = SYSDATE
      WHERE  record_id = new_metadata.record_id;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END update_metadata;


BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END metadata_util;
