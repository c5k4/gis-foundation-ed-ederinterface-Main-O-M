Prompt drop Package Body REGISTRY_UTIL;
DROP PACKAGE BODY SDE.REGISTRY_UTIL
/

Prompt Package Body REGISTRY_UTIL;
--
-- REGISTRY_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.registry_util
/***********************************************************************
*
*N  {registry_util.spb}  --  Implementation for registry DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on table_registry.  It should be compiled by the
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
*    Yung-Ting Chen            03/03/99               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          NVARCHAR2(32);

   CURSOR G_registration_owner_cursor (wanted_reg_id  IN  registration_id_t) IS
     SELECT owner
     FROM   SDE.table_registry
     WHERE  registration_id = wanted_reg_id;

   CURSOR G_registration_exist_cursor (wanted_owner  IN  table_name_t,
                                       wanted_table  IN  table_name_t) IS
     SELECT table_name
     FROM   SDE.table_registry
     WHERE  owner = wanted_owner AND
            table_name = wanted_table;

   /* Local Subprograms. */

   PROCEDURE L_registration_user_can_modify (wanted_reg_id  IN  registration_id_t)
  /***********************************************************************
  *
  *N  {L_registration_user_can_modify}  --  Can current user modify 
  *                                         registration?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the registration specified by ID exists and is
  *   modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_reg_id  <IN>  ==  (registration_id_t) registration to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen             03/09/99           Original coding.
  *E
  ***********************************************************************/
   IS

      registration_owner           SDE.table_registry.owner%TYPE;

   BEGIN

      -- Make sure that the table exists, and that the current user can write
      -- to it.

      OPEN G_registration_owner_cursor (wanted_reg_id);
      FETCH G_registration_owner_cursor INTO registration_owner;
      IF G_registration_owner_cursor%NOTFOUND THEN
         CLOSE G_registration_owner_cursor;
         raise_application_error (sde_util.SE_TABLE_NOREGISTERED,
                                  'Registration ' || TO_CHAR (wanted_reg_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_registration_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != registration_owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of registration ' ||
                                     TO_CHAR (wanted_reg_id) || '.');
         END IF;
      END IF;
    
   END L_registration_user_can_modify;

   

   PROCEDURE L_column_entry_access_OK (column  IN  registered_column_record_t)
  /***********************************************************************
  *
  *N  {L_column_entry_access_OK}  --  Can current user add or modify this column
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the registration specified by a column in
  *   exist and are modifiable by the current user (who must be owner or 
  *   SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column  <IN>  ==  (registered_column_record_t) The list of column 
  *                        entries to test.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                  03/29/02           Original coding.
  *E
  ***********************************************************************/
   IS

      table_name                   table_name_t;

   BEGIN

      -- Make sure we are table owner or DBA.

      IF NOT G_sde_dba THEN
         IF G_current_user != column.owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of table ' || 
                                     TO_CHAR(column.owner) || '.' ||
                                     TO_CHAR(column.table_name) || '.');
         END IF;
      END IF;

      -- Make sure that the table exists.

      OPEN G_registration_exist_cursor (column.owner,column.table_name);
      FETCH G_registration_exist_cursor INTO table_name;
      IF G_registration_exist_cursor%NOTFOUND THEN
         CLOSE G_registration_exist_cursor;
         raise_application_error (sde_util.SE_TABLE_NOREGISTERED,
                                  'Registration ' || TO_CHAR(column.owner) || '.' ||
                                  TO_CHAR(column.table_name) || ' not found.');
      END IF;
      CLOSE G_registration_exist_cursor;
    
   END L_column_entry_access_OK;


   /* Public Subprograms. */

   PROCEDURE insert_registration (registration IN registration_record_t)
  /***********************************************************************
  *
  *N  {insert_registration}  --  insert a registration entry into the 
  *                              TABLE_REGISTRY table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the
  *   SDE.TABLE_REGISTRY table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     registration  <IN>  ==  (registration_record_t) The new registration
  *                                      to be inserted. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen                 03/09/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Insert the record into the table_registry table; use a block to
      -- catch the DUP_VAL_ON_INDEX exception that occurs when a 
      -- registration is inserted.

      BEGIN

        INSERT INTO SDE.table_registry
         (registration_id,table_name,owner,rowid_column,description,
          object_flags,registration_date,config_keyword,minimum_id,
          imv_view_name)
        VALUES (registration.registration_id,
                registration.table_name,
                registration.owner,
                registration.rowid_column,
                registration.description,
                registration.object_flags,
                registration.registration_date,
                registration.config_keyword,
                registration.minimum_id,
                registration.imv_view_name);
      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
           raise_application_error(sde_util.SE_TABLE_REGISTERED,
                                  'Registration ' ||
                                  TO_CHAR (registration.registration_id) ||
                                  ' already exists.');
      END;


      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END insert_registration;


   PROCEDURE delete_registration (old_reg_id  IN  registration_id_t)
  /***********************************************************************
  *
  *N  {delete_registration}  --  Delete an arbitary registration
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary registration.  All checking
  *   and locking should be performed by the gsrvr, except that we will
  *   check registration ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_reg_id  <IN>  ==  (registration_id_t)  registration to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen              03/09/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- make sure registration exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_registration_user_can_modify (old_reg_id);

      -- We've verified our permissions and the registration's existence,
      -- so it must be OK to delete them.  Do it, but first delete any
      -- registered column entries from the table.

      DELETE FROM SDE.column_registry 
      WHERE  (owner,table_name) IN
               (SELECT owner,table_name 
                FROM   SDE.table_registry
                WHERE  registration_id = old_reg_id);

      DELETE FROM SDE.table_registry WHERE registration_id = old_reg_id;

      -- Delete any entries associated with this registration entry from
      -- the mvtables_modified table (this used to be done by a table
      -- constraint with DELETE CASCADE, but that proved too buggy at 
      -- Oracle 8.0.*, so we pulled it in favor of this approach).

      DELETE FROM SDE.mvtables_modified WHERE registration_id = old_reg_id;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_registration;

   PROCEDURE update_registration   (registration  IN registration_record_t)
  /***********************************************************************
  *
  *N  {update_registration}  --  Update a registration
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a registration's record.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check registration ownership if the invoking 
  *   user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     registration   <IN>  ==  (registration_record_t) registration to
  *                               be updated
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen              03/09/99           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the registration exists, and that the current
      -- user can write to it.

      L_registration_user_can_modify (registration.registration_id);
    
      -- Update the table_registry.

      UPDATE SDE.table_registry
      SET   rowid_column = registration.rowid_column,
            description = registration.description,
            object_flags = registration.object_flags,
            config_keyword = registration.config_keyword,
            minimum_id = registration.minimum_id,
            imv_view_name = registration.imv_view_name
      WHERE registration_id = registration.registration_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_registration;

PROCEDURE update_registration   (registration  IN registration_record_t,
                                    txn_commit    IN NUMBER)
  /***********************************************************************
  *
  *N  {update_registration}  --  Update a registration
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a registration's record.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check registration ownership if the invoking 
  *   user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     registration   <IN>  ==  (registration_record_t) registration to
  *                               be updated
  *     txn_commit     <IN>  ==  (NUMBER) commit if value is 1
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen       03/09/99    Original coding.
  *    Josefina Santiago    12/10/07    Added option to commit txn.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the registration exists, and that the current
      -- user can write to it.

      L_registration_user_can_modify (registration.registration_id);
    
      -- Update the table_registry.

      UPDATE SDE.table_registry
      SET   rowid_column = registration.rowid_column,
            description = registration.description,
            object_flags = registration.object_flags,
            config_keyword = registration.config_keyword,
            minimum_id = registration.minimum_id,
            imv_view_name = registration.imv_view_name
      WHERE registration_id = registration.registration_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      IF txn_commit = 1 THEN 
       COMMIT;
      END IF;

   END update_registration;

   PROCEDURE change_registration_table_name (new_table_name IN table_name_t,
                                             wanted_reg_id  IN registration_id_t)
  /***********************************************************************
  *
  *N  {change_registration_table_name}  --  change an arbitary registration's 
  *                                         table name
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure changes an arbitary registration's table name.  All
  *   checking and locking should be performed by the gsrvr, except that we
  *   will check registry ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     new_table_name  <IN>  ==  (table_name_t)  new table name.
  *     wanted_reg_id   <IN>  ==  (registration_id_t)  registration to change.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen             03/09/99           Original coding.
  *E
  ***********************************************************************/
   IS

   CURSOR name_owner_cursor (wanted_reg_id  IN  registration_id_t) IS
     SELECT table_name,owner
     FROM   SDE.table_registry
     WHERE  registration_id = wanted_reg_id;

    reginfo   name_owner_cursor%ROWTYPE;

   BEGIN

      -- make sure registration exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      OPEN name_owner_cursor (wanted_reg_id);
      FETCH name_owner_cursor INTO reginfo;
      IF name_owner_cursor%NOTFOUND THEN
         CLOSE name_owner_cursor;
         raise_application_error (sde_util.SE_TABLE_NOREGISTERED,
                                  'Registration ' || TO_CHAR (wanted_reg_id) ||
                                  ' not found.');
      END IF;
      CLOSE name_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != reginfo.owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of registration ' ||
                                     TO_CHAR (wanted_reg_id) || '.');
         END IF;
      END IF;

      -- We've verified our permissions and the registration's existence,
      -- so it must be OK to delete them.  Rename the entries for this
      -- table in the column registry, in the table registry and in the
      -- metadata table.

      UPDATE  SDE.column_registry 
      SET     table_name = new_table_name
      WHERE  (owner,table_name) IN
               (SELECT owner,table_name 
                FROM   SDE.table_registry
                WHERE  registration_id = wanted_reg_id);
      
      UPDATE SDE.table_registry SET table_name = new_table_name
      WHERE  registration_id = wanted_reg_id;

      UPDATE SDE.metadata SET object_name = new_table_name
      WHERE  object_name = reginfo.table_name AND
             object_owner = reginfo.owner AND
             object_type = SDE.metadata_util.C_metadata_object_type_table;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END change_registration_table_name;

   PROCEDURE clear_registration_modified (reg_id  IN  registration_id_t)
  /***********************************************************************
  *
  *N  {clear_registration_modified_flags}  --  Clear flags
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure clears the specified registration entries flags
  *   from SDE.mvtables_modified.  It is used when making a multiversion
  *   table single-versioned.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     reg_id  <IN>  ==  (registration_id_t) Registration to clear.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 10/18/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- If the registration exists, make sure we have permission to
      -- modify it.  Nonexistent registrations are always fair game.

      BEGIN
         L_registration_user_can_modify (reg_id);
      EXCEPTION
         WHEN OTHERS THEN
            IF SQLCODE <> sde_util.SE_TABLE_NOREGISTERED THEN
               RAISE;
            END IF;
      END;

      -- Delete any entries associated with this registration entry from
      -- the mvtables_modified table.

      DELETE FROM SDE.mvtables_modified WHERE registration_id = reg_id;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END clear_registration_modified;

   /* Column Registration Functions */

   PROCEDURE insert_registered_column
                                 (column_entry  IN  registered_column_record_t)
  /***********************************************************************
  *
  *N  {insert_registered_column}  --  Insert a column entry into the 
  *                                   COLUMN_REGISTRY table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a entry into the SDE.COLUMN_REGISTRY table. 
  *   Note that because this procedure is usually called multiple times
  *   in order to load a single table's metadata, it does not call COMMIT
  *   like the other DDL functions in this package.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column_list  <IN>  ==  (registered_column_record_t) The column 
  *                             entry to add.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                  03/29/02           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure registered column exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_column_entry_access_OK (column_entry);

      -- Insert the record into the column_registry table.

      INSERT INTO SDE.column_registry
         (table_name,owner,column_name,sde_type,column_size,decimal_digits,
          description,object_flags,object_id)
      VALUES (column_entry.table_name,
              column_entry.owner,
              column_entry.column_name,
              column_entry.sde_type,
              column_entry.column_size,
              column_entry.decimal_digits,
              column_entry.description,
              column_entry.object_flags,
              column_entry.object_id);

   END insert_registered_column;


   PROCEDURE delete_registered_column 
                               (column_entry  IN  registered_column_record_t)
  /***********************************************************************
  *
  *N  {delete_registered_column}  --  Delete an arbitary column metadata entry
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary registered column metadata entry.  
  *   All checking and locking should be performed by the gsrvr, except that
  *   we will check registration ownership if the invoking user is not the 
  *   SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column_entry  <IN>  ==  (registered_column_record_t) The column
  *                              entry to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                  03/29/02           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure registered column exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_column_entry_access_OK (column_entry);

      -- We've verified our permissions and the registration's existence,
      -- so it must be OK to delete them.  Do it, but first delete any
      -- registered column entries from the table.

      DELETE FROM SDE.column_registry 
      WHERE  owner = column_entry.owner AND
             table_name = column_entry.table_name AND
             column_name = column_entry.column_name;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_registered_column;

   PROCEDURE update_registered_column 
                                 (column_entry  IN  registered_column_record_t)
  /***********************************************************************
  *
  *N  {update_registered_column}  --  Update a registered column
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a registered column's record.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check registration ownership if the invoking 
  *   user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column_entry  <IN>  ==  (registered_column_record_t) The column
  *                              entry to update.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                  04/01/02           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure registered column exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_column_entry_access_OK (column_entry);
    
      -- Update the table_registry.

      UPDATE SDE.column_registry
      SET   sde_type = column_entry.sde_type,
            column_size = column_entry.column_size,
            decimal_digits = column_entry.decimal_digits,
            object_flags = column_entry.object_flags,
            description = column_entry.description,
            object_id = column_entry.object_id
      WHERE  owner = column_entry.owner AND
             table_name = column_entry.table_name AND
             column_name = column_entry.column_name;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_registered_column;
  
   PROCEDURE update_registered_column(column_entry    IN  registered_column_record_t,
					                  old_column_name IN column_name_t)
  /***********************************************************************
  *
  *N  {update_registered_column}  --  Update a registered column
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a registered column's record and renames 
  *   the column name. All checking and locking should be performed by 
  *   the gsrvr, except that we will check registration ownership if the
  *   invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column_entry  <IN>  ==  (registered_column_record_t) The column
  *                              entry to update.
  *     old_column_name <IN> == (the old column name that will be renamed
  *                              by the one in the column entry)
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson      04/01/02   Original coding.
  *    Mark Harris        12/12/07   Added option to modify column.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure registered column exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_column_entry_access_OK (column_entry);
    
      -- Update the table_registry.

      UPDATE SDE.column_registry
      SET   column_name = column_entry.column_name,
            sde_type = column_entry.sde_type,
            column_size = column_entry.column_size,
            decimal_digits = column_entry.decimal_digits,
            object_flags = column_entry.object_flags,
            description = column_entry.description,
            object_id = column_entry.object_id
      WHERE  owner = column_entry.owner AND
             table_name = column_entry.table_name AND
             column_name = old_column_name;

   END update_registered_column;


BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END registry_util;

/


Prompt Grants on PACKAGE REGISTRY_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.REGISTRY_UTIL TO PUBLIC WITH GRANT OPTION
/
