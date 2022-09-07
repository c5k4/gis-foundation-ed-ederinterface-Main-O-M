--------------------------------------------------------
--  DDL for Package Body VERSION_USER_DDL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."VERSION_USER_DDL" 
/***********************************************************************
*
*N  {version_user_ddl.spb}  --  Procedures for user version DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body contains procedures to perform DDL 
*   operations on versions.  It is intended to allow users to perform
*   some of the same operations in SQL as they do via the ArcSDE API or
*   via ArcMap.  It should be compiled by the ArcSDE DBA user; security 
*   is by user name.   
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
*    Peter Aronson             04/18/00               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;
   G_connection_id         INTEGER;

   CURSOR G_version_query_cursor (v_name  IN  version_util.version_name_t,
                                  v_owner IN  version_util.version_name_t) IS
     SELECT v.version_id,v.state_id,v.status
     FROM   SDE.versions v
     WHERE  v.name = v_name AND
            v.owner = v_owner;

   /* Local Subprograms. */

   PROCEDURE L_delete_state_autolock (state_id  IN  version_util.state_id_t)
  /***********************************************************************
  *
  *N  {L_delete_state_autolock}  --  Delete a state autolock w/no exceptions
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a state autolock, without allowing any
  *   exceptions to be propagated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id   <IN>  ==  (state_id_t) The state to delete the lock on.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/20/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      lock_util.delete_state_lock (G_connection_id,
                                   state_id,
                                   lock_util.C_is_autolock);

   EXCEPTION

      WHEN OTHERS THEN
         NULL;

   END L_delete_state_autolock;
   

   PROCEDURE L_delete_state (state_id  IN  version_util.state_id_t)
  /***********************************************************************
  *
  *N  {L_delete_state}  --  Delete a single state w/no exceptions
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a single state, without allowing any
  *   exceptions to be propagated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id   <IN>  ==  (state_id_t) The state to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/20/00           Original coding.
  *E
  ***********************************************************************/
   IS

      PRAGMA AUTONOMOUS_TRANSACTION;

      delete_ids  version_util.state_list_t;

   BEGIN

      delete_ids (1) := state_id;
      version_util.delete_states (delete_ids);

   EXCEPTION

      WHEN OTHERS THEN
         NULL;

   END L_delete_state;
   

   PROCEDURE L_lineage_flag (lineage_name  IN  version_util.state_id_t)
  /***********************************************************************
  *
  *N  {L_lineage_flag}  --  Flag a lineage as having been modified
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the lineage_modified table to indicate a
  *   specific lineage as having been modified.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     lineage_name  <IN>  ==  (state_id_t) The lineage to flag as modified
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 10/09/01           Original coding.
  *E
  ***********************************************************************/
   IS

       PRAGMA AUTONOMOUS_TRANSACTION;

      CURSOR check_tlm_cursor (check_lineage_name  IN  INTEGER) IS 
        SELECT time_last_modified 
        FROM   SDE.lineages_modified 
        WHERE  lineage_name = check_lineage_name 
        FOR UPDATE; 

       current_time_last_modified DATE := SYSDATE;
       found_time_last_modified   DATE; 
       new_time_last_modified     DATE; 
       a_second                   NUMBER := 1/(24 * 60 * 60); 
       lineage_found              BOOLEAN; 

    BEGIN 

       -- Try to fetch the current lineage time last modified.

       OPEN check_tlm_cursor (lineage_name); 
       FETCH check_tlm_cursor INTO found_time_last_modified; 
       lineage_found := check_tlm_cursor%FOUND; 
       CLOSE check_tlm_cursor; 

       IF lineage_found THEN 

          -- If there was already an entry for this lineage, update it.

          IF current_time_last_modified > found_time_last_modified THEN 
             new_time_last_modified := current_time_last_modified; 
          ELSE 
             new_time_last_modified := found_time_last_modified + a_second; 
          END IF; 

          UPDATE SDE.lineages_modified 
          SET    time_last_modified = new_time_last_modified 
          WHERE  lineage_name = L_lineage_flag.lineage_name; 
          
       ELSE 

          -- No previous entry for this lineage, add one.

          INSERT INTO SDE.lineages_modified 
          VALUES (lineage_name,current_time_last_modified); 

       END IF;

       -- Clean old entries from the lineages_modified table.

       DELETE FROM SDE.lineages_modified 
       WHERE  SYSDATE - time_last_modified > 2; 

       -- Got this far without an exception, must be OK to commit.

       COMMIT;

   EXCEPTION
       
       WHEN OTHERS THEN
          ROLLBACK;
          RAISE;

   END L_lineage_flag;


   PROCEDURE L_delete_user_marks
  /***********************************************************************
  *
  *N  {L_delete_user_marks}  --  Delete all mark locks for current connection
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all mark locks held by the current Oracle
  *   session.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/20/04           Original coding.
  *E
  ***********************************************************************/
   IS

       PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

        DELETE FROM SDE.state_locks
        WHERE  sde_id = G_connection_id AND 
               lock_type = SDE.lock_util.C_marked_lock;
        COMMIT;

   EXCEPTION
       
       WHEN OTHERS THEN
          ROLLBACK;
          RAISE;

   END L_delete_user_marks;

   FUNCTION L_get_sde_id RETURN INTEGER
  /***********************************************************************
  *
  *N  {L_get_sde_id}  --  Return session's unique SDE connection id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the current session's unique SDE connection
  *   id.  The first call to this function generates it, and subsequent 
  *   calls return it from session global memory.  If a new id is generated,
  *   then a flag lock is placed for it to indicate it is in use.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (INTEGER) The unique SDE connection id for this
  *                        session.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 06/18/10           Original coding.
  *E
  ***********************************************************************/
   IS

       PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      RETURN pinfo_util.get_sde_id;

   END L_get_sde_id;


   /* Public Subprograms. */

  PROCEDURE create_version (parent_name  IN      version_util.version_name_t,
                            name         IN OUT  version_util.version_name_t,
                            name_rule    IN      PLS_INTEGER,
                            access       IN      PLS_INTEGER,
                            description  IN      NVARCHAR2)
  /***********************************************************************
  *
  *N  {create_version}  --  Create a new version as the child of another
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a user-supplied entry into the SDE.VERSIONS
  *   table.  A unique name may be generated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     parent_name        <IN>  ==  (version_name_t) The name of an existing
  *                                   version to be the parent of the one
  *                                   created; if no owner name is prefixed,
  *                                   the current user is assumed to be the
  *                                   owner.
  *     name           <IN OUT>  ==  (version_name_t) On input, the name of
  *                                   version to be created, on output, the 
  *                                   name of the version as actually created.
  *                                   These will be different when a version
  *                                   that name already exists, and a name
  *                                   rule of C_generate_unique_name is 
  *                                   specified.
  *     name_rule          <IN>  ==  (PLS_INTEGER) The rule used to 
  *                                   determine the name to use, one of:
  *                                     C_generate_unique_name (1)
  *                                     C_take_name_as_given   (2)
  *                                   If C_generate_unique_name is specified
  *                                   and a version with the specified name
  *                                   already exists, then a suffix will be
  *                                   added to make the version name unique.
  *                                   If C_take_name_as_given is specified
  *                                   and a version with the specified name
  *                                   already exists, then an exception will
  *                                   be raised.
  *     access             <IN>  ==  (PLS_INTEGER) The access level users 
  *                                   other than the owner will have to the
  *                                   new version:
  *                                     C_version_private   (0)
  *                                     C_version_public    (1)
  *                                     C_version_protected (2)
  *                                   Private versions can only be accessed
  *                                   by there owners, protected versions
  *                                   can only be written to by their owners,
  *                                   public versions are wide open.
  *     description        <IN>  ==  (VARCHAR2) The new version's description
  *                                   string.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *     -20066                SE_INVALID_PARAM_VALUE
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20172                SE_STATE_NOEXIST
  *     -20177                SE_VERSION_EXISTS
  *     -20298                SE_INVALID_VERSION_ID
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/18/00           Original coding.
  *E
  ***********************************************************************/
   IS

      parsed_name          SDE.versions.name%TYPE;
      parsed_owner         SDE.versions.owner%TYPE; 
      parsed_parent_name   SDE.versions.parent_name%TYPE;
      parsed_parent_owner  SDE.versions.parent_owner%TYPE;
      version_record       version_util.version_record_t;
      parent_info          G_version_query_cursor%ROWTYPE;
      state_lock           lock_util.state_lock_t;
      state_lock_on        BOOLEAN := FALSE;
      binary_status        PLS_INTEGER;
      test_id              version_util.state_id_t;
  
     CURSOR state_test_cursor (state_wanted_id  IN  version_util.state_id_t) IS
       SELECT state_id
       FROM   SDE.states s
       WHERE  s.state_id = state_wanted_id;

     CURSOR version_id_cursor IS
       SELECT SDE.version_id_generator.nextval
       FROM DUAL;

   BEGIN

      -- Initialize.

      IF G_connection_id IS NULL THEN
         G_connection_id := L_get_sde_id;  -- Provides indicator lock.
      END IF;

      -- Check arguments.

      IF parent_name IS NULL THEN
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'Parent version can not be NULL.');
      END IF;
      
      version_util.parse_version_name (name,parsed_name,parsed_owner);
      IF parsed_owner <> G_current_user THEN
         raise_application_error (sde_util.SE_INVALID_VERSION_NAME,
                                  'The new version must be in the current ' ||
                                  'user''s schema');
      END IF;

      IF access IS NULL THEN
         raise_application_error (sde_util.SE_INVALID_PARAM_VALUE,
                                  'NULL is not a valid access type code.');
      ELSIF access < version_util.C_version_private OR
            access > version_util.C_version_protected THEN
         raise_application_error (sde_util.SE_INVALID_PARAM_VALUE,
                                  TO_CHAR (access) || ' is not a valid ' ||'
                                  access type code.');
      END IF;
      
      -- Fetch the proposed parent version.

      version_util.parse_version_name (parent_name,
                                       parsed_parent_name,
                                       parsed_parent_owner);
      OPEN G_version_query_cursor (parsed_parent_name,parsed_parent_owner);
      FETCH G_version_query_cursor INTO parent_info;
      IF G_version_query_cursor%NOTFOUND THEN
         CLOSE G_version_query_cursor;
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || TO_CHAR(parent_name) || ' not found.');
      END IF;
      CLOSE G_version_query_cursor;

      -- Check permissions.  At least one of the following must be true for this
      -- operation:  (1) The parent version must be public or protected, or
      --             (2) The current user is the parent version's owner, or
      --             (3) The current user is the SDE DBA user.

      binary_status := parent_info.status;
      binary_status := binary_status - FLOOR (binary_status / 4) * 4;
      IF binary_status = version_util.C_version_private THEN
         IF TO_CHAR(parsed_parent_owner) <> G_current_user THEN
            IF NOT G_sde_dba THEN
               raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                        'Insufficient access to version ' ||
                                        parent_name);
            END IF;
         END IF;
      END IF;

      -- Lock the underlying state, to make sure it stays still.

      state_lock.sde_id := G_connection_id;
      state_lock.state_id := parent_info.state_id;
      state_lock.autolock := lock_util.C_is_autolock;
      state_lock.lock_type :=lock_util.C_shared_lock;
 
      lock_util.add_state_lock (state_lock);

      -- Now that we have a lock, we safely check to see if the parent version's
      -- state still exists.

      OPEN state_test_cursor (parent_info.state_id);
      FETCH state_test_cursor INTO test_id;
      IF state_test_cursor%NOTFOUND THEN
         CLOSE state_test_cursor;
         raise_application_error (sde_util.SE_STATE_NOEXIST,
                                  'State ' || TO_CHAR (parent_info.state_id) ||
                                  ' from version ' || TO_CHAR(parent_name) ||
                                  ' not found.');
      END IF;
      CLOSE state_test_cursor;

      -- Get a version ID.

      OPEN version_id_cursor;
      FETCH version_id_cursor INTO version_record.version_id;
      CLOSE version_id_cursor;
      IF version_record.version_id IS NULL THEN
         raise_application_error (sde_util.SE_INVALID_VERSION_ID,
                                  'Unable to generate a version ID for ' ||
                                  name);
      END IF;

      -- Build the version structure, and insert it.

      version_record.name := parsed_name;
      version_record.owner := G_current_user;
      version_record.status := access;
      version_record.state_id := parent_info.state_id;
      version_record.description := description;
      version_record.parent_name := parsed_parent_name;
      version_record.parent_owner := parsed_parent_owner;
      version_record.parent_version_id := parent_info.version_id;
      version_record.creation_time := SYSDATE;

      version_util.insert_version (version_record,name_rule,name);

      -- It's now safe to remove the state lock.

      lock_util.delete_state_lock (G_connection_id,
                                   parent_info.state_id,
                                   lock_util.C_is_autolock);
      state_lock_on := FALSE;

   EXCEPTION

      -- If an exception is raised while the state lock is held, release the
      -- state lock.

      WHEN OTHERS THEN
         IF state_lock_on THEN
            L_delete_state_autolock (parent_info.state_id);
         END IF;
         RAISE;

   END create_version;


  PROCEDURE delete_version (name  IN  version_util.version_name_t)
  /***********************************************************************
  *
  *N  {delete_version}  --  Delete a version entry in the VERSIONS table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an existing version's definition.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_name_t) The name of the
  *                                   version to delete, in either <name>
  *                                   or <owner>.<name> format.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20175                SE_VERSION_HAS_CHILDREN
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/19/00           Original coding.
  *E
  ***********************************************************************/
   IS

      object_lock   lock_util.object_lock_t;
      version_info  version_util.version_record_t;
      parsed_name   SDE.versions.name%TYPE;
      parsed_owner  SDE.versions.owner%TYPE;

     CURSOR version_id_cursor 
          (version_wanted_name  IN  version_util.version_name_t,
           version_wanted_owner IN  version_util.version_name_t) IS
       SELECT v.version_id
       FROM   SDE.versions v
       WHERE  v.name = version_wanted_name AND
              v.owner = version_wanted_owner;

   BEGIN
      -- Initialize.

      IF G_connection_id IS NULL THEN
         G_connection_id := L_get_sde_id;  -- Provides indicator lock.
      END IF;

      -- Fetch the version so we have the information needed to place a lock.

      version_util.parse_version_name (name,parsed_name,parsed_owner);

      OPEN version_id_cursor (parsed_name,parsed_owner);
      FETCH version_id_cursor INTO object_lock.object_id;
      IF version_id_cursor%NOTFOUND THEN
         CLOSE version_id_cursor;
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || name || ' not found.');
      END IF;
      CLOSE version_id_cursor;

      -- Place an object lock on the version to be deleted to be sure it isn't
      -- currently in use.

      object_lock.sde_id         := G_connection_id;
      object_lock.object_type    := 1;   -- Version object
      object_lock.application_id := 999; -- Internal application
      object_lock.autolock       := lock_util.C_is_autolock;
      object_lock.lock_type      := lock_util.C_exclusive_lock;
  
      lock_util.add_object_lock (object_lock);

      -- Do the actual work, but use a block to catch exceptions and remove
      -- the lock.

      BEGIN

         version_util.delete_version (name);

      EXCEPTION

         WHEN OTHERS THEN
            lock_util.delete_object_lock (object_lock);
            RAISE;

      END;

      -- Remove the lock.

      lock_util.delete_object_lock (object_lock);

   END delete_version;


  PROCEDURE edit_version (name         IN  version_util.version_name_t,
                          edit_action  IN  PLS_INTEGER)
  /***********************************************************************
  *
  *N  {edit_version}  --  Open or close a version for editing.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure takes a version, and either opens or closes it for
  *   editing.  To open a version for editing, its underlying state is 
  *   closed (if not already), and a new, child state is created, and the
  *   version will be moved to that state.  To close a version, its 
  *   underlying state is closed.  Closing a version also commits any
  *   pending DML statements.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     name         <IN>  ==  (version_name_t) The name of the version to
  *                             alter the editing state of, in either <name>
  *                             or <owner>.<name> format.
  *     edit_action  <IN>  ==  (PLS_INTEGER) Whether the specified version
  *                             is to be opened for editing, or closed:
  *                               C_edit_action_open  (1)
  *                               C_edit_action_close (2)
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20049                SE_LOCK_CONFLICT
  *     -20066                SE_INVALID_PARAM_VALUE
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20172                SE_STATE_NOEXIST
  *     -20174                SE_VERSION_HAS_MOVED
  *     -20176                SE_PARENT_NOT_CLOSED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 04/19/00           Original coding.
  *E
  ***********************************************************************/
   IS

      parsed_name          SDE.versions.name%TYPE;
      parsed_owner         SDE.versions.owner%TYPE; 
      version_info         G_version_query_cursor%ROWTYPE;
      state_lock           lock_util.state_lock_t;
      persistent_lock      lock_util.state_lock_t;
      state_lock_on        BOOLEAN := FALSE;

     CURSOR state_info_cursor (state_wanted_id  IN  version_util.state_id_t) IS
       SELECT closing_time,lineage_name
       FROM   SDE.states s
       WHERE  s.state_id = state_wanted_id;

      state_info           state_info_cursor%ROWTYPE;
      new_state_id         version_util.state_id_t;
      new_lineage_name     version_util.state_id_t;
      binary_status        PLS_INTEGER;
      new_state_time       DATE;     
      state_close_time     DATE;
      start_edit_same_version   EXCEPTION;

   BEGIN

      -- Initialize.

      IF G_connection_id IS NULL THEN
         G_connection_id := L_get_sde_id;  -- Provides indicator lock.
      END IF;

      -- Check arguments.

      IF edit_action IS NULL THEN
         raise_application_error (SDE.sde_util.SE_INVALID_PARAM_VALUE,
                                  'Edit action may not be NULL.');
      ELSIF edit_action < C_edit_action_open OR 
            edit_action > C_edit_action_close THEN
         raise_application_error (SDE.sde_util.SE_INVALID_PARAM_VALUE,
                                  TO_CHAR (edit_action) || ' is not a valid ' ||
                                  'edit action code.');
      END IF;

      -- Get the information we need from the version.
      
      version_util.parse_version_name (name,parsed_name,parsed_owner);

      -- Raise exception if the named version is the DEFAULT version.

      IF parsed_name = 'DEFAULT' AND 
         UPPER(parsed_owner) = UPPER(SDE.sde_util.C_sde_dba) THEN
         raise_application_error (sde_util.SE_MVV_EDIT_DEFAULT,
                                  'Cannot edit the DEFAULT version in STANDARD transaction mode.');
      END IF;

      -- Raise exception if we've started editing on another named (cached) version
      -- which is different from the input named version.

      IF edit_action = C_edit_action_open AND 
         (parsed_name != SDE.version_util.G_version_name OR
          UPPER(parsed_owner) != SDE.version_util.G_version_owner) AND 
         SDE.version_util.G_edit_state = SDE.version_util.C_edit_state_start THEN
         raise_application_error (sde_util.SE_MVV_IN_EDIT_MODE,
                                  'Cannot start edit on a new version with an open edit session to another version.');
      END IF;

      -- No-op if the named version is the same as the cached named version.
     
      IF edit_action = C_edit_action_open AND 
         parsed_name = SDE.version_util.G_version_name AND 
         UPPER(parsed_owner) = SDE.version_util.G_version_owner AND 
         SDE.version_util.G_edit_state = SDE.version_util.C_edit_state_start THEN
        raise start_edit_same_version;
      END IF;

      -- Check if the version name matches the current edit version if the
      -- action is stop edit. Send appropriate error message.

      IF edit_action = C_edit_action_close AND 
         (parsed_name != SDE.version_util.G_version_name OR 
         UPPER(parsed_owner) != SDE.version_util.G_version_owner) AND 
         SDE.version_util.G_edit_state = SDE.version_util.C_edit_state_start THEN
        raise_application_error (sde_util.SE_MVV_NAMEVER_NOT_CURRVER,
          'Cannot stop edit on '||name||' while version '||SDE.version_util.G_version_owner||'.'||SDE.version_util.G_version_name||
          ' is the current edit version.');
      END IF;


      OPEN G_version_query_cursor (parsed_name,parsed_owner);
      FETCH G_version_query_cursor INTO version_info;
      IF G_version_query_cursor%NOTFOUND THEN
         CLOSE G_version_query_cursor;
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || name || ' not found.');
      END IF;
      CLOSE G_version_query_cursor;

      -- Check permissions.  At least one of the following must be true for this
      -- operation:  (1) The parent version must be public, or
      --             (2) The current user is the parent version's owner, or
      --             (3) The current user is the SDE DBA user.

      binary_status := version_info.status;
      binary_status := binary_status - FLOOR (binary_status / 4) * 4;
      IF binary_status <> version_util.C_version_public THEN
         IF parsed_owner <> G_current_user THEN
            IF NOT G_sde_dba THEN
               raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                        'Insufficient access to version ' ||
                                        name);
            END IF;
         END IF;
      END IF;

      -- Lock the version's state if this is a open edit.

      IF edit_action = C_edit_action_open THEN
         state_lock.sde_id := G_connection_id;
         state_lock.state_id := version_info.state_id;
         state_lock.autolock := lock_util.C_is_autolock;
         state_lock.lock_type := lock_util.C_shared_lock;
 
         lock_util.add_state_lock (state_lock);
         state_lock_on := TRUE;
      END IF;

      -- Fetch the information from the version's current state that we need
      -- to create the child state or close the current state.

      OPEN state_info_cursor (version_info.state_id);
      FETCH state_info_cursor INTO state_info;
      IF state_info_cursor%NOTFOUND THEN
         CLOSE state_info_cursor;
         raise_application_error (sde_util.SE_STATE_NOEXIST,
                                  'State ' || 
                                  TO_CHAR (version_info.state_id) ||
                                  ' from version ' || TO_CHAR(name) || ' 
                                  not found.');
      END IF;
      CLOSE state_info_cursor;
      -- Perform version open or close for editing.

      IF edit_action = C_edit_action_close THEN

         -- Commit any DML statements.

         COMMIT;

         -- If we are done editing, close the state. 

         version_util.close_state (version_info.state_id,state_close_time);
         L_lineage_flag (state_info.lineage_name); 

         -- The change is made, we can release our persistent lock. (Ignore
         -- lock nonexist errors, this code be the result of a close after
         -- a forced disconnet.)

         BEGIN
            lock_util.delete_state_lock (G_connection_id,
                                         version_info.state_id,
                                         lock_util.C_is_autolock);
         EXCEPTION
            WHEN OTHERS THEN
               IF SQLCODE <> sde_util.SE_NO_LOCKS THEN
                  RAISE;
               END IF;
         END;         

         -- Delete any mark locks we may have placed.
        
         L_delete_user_marks;

         -- Set the edit state to stop indicating we've
         -- stopped editing in the STANDARD transaction mode.
  
         SDE.version_util.G_edit_state := SDE.version_util.C_edit_state_stop;

      ELSE
    
         -- If we starting editing, we will create a child of the current state,
         -- and move this version on to it.

         -- If the version's current state is open, try to close it.

         IF state_info.closing_time IS NULL THEN
            version_util.close_state (version_info.state_id,state_close_time);
         END IF;

         -- Create the new state.

         version_util.insert_state (version_info.state_id,
                                    state_info.lineage_name,
                                    version_util.C_state_is_open,
                                    new_state_id,
                                    new_lineage_name,
                                    new_state_time); 
         L_lineage_flag (new_lineage_name); 

         -- Unlock the parent state -- we don't need it any longer. 

         lock_util.delete_state_lock (G_connection_id,
                                      version_info.state_id,
                                      lock_util.C_is_autolock);
         state_lock_on := FALSE;

         -- Move the version to the new state.

         version_util.change_version_state (name,
                                            version_info.state_id,
                                            new_state_id);

         -- Now lock the new state with a persistent lock

         persistent_lock.sde_id := G_connection_id;
         persistent_lock.state_id := new_state_id;
         persistent_lock.autolock := lock_util.C_is_autolock;
         persistent_lock.lock_type := lock_util.C_exclusive_lock;
 
         new_state_id := NULL;
         lock_util.add_state_lock (persistent_lock);

         -- Set the now editable version as the current version.

         version_util.set_current_version (name);

         -- Set the name of the edit version

         SDE.version_util.G_version_owner := UPPER(parsed_owner);
         SDE.version_util.G_version_name  := parsed_name;

         -- Set the edit state to start indicating we're
         -- in a STANDARD transaction mode.
  
         SDE.version_util.G_edit_state := SDE.version_util.C_edit_state_start;

         -- Set the Edit mode DEFAULT to FALSE indicating
         -- STANDARD multiversion editing for the MV DML triggers.

         SDE.version_util.G_edit_mode_default := FALSE;

         -- Commit to ensure a begin transacation state

         COMMIT;

      END IF;

   EXCEPTION

      -- Ignore the operation if the start edit is against the 
      -- current edit version. 

      WHEN start_edit_same_version THEN
        NULL;

      -- If an exception is raised while the state lock is held, release the
      -- state lock.  Also, if a state is created, but the version doesn't
      -- move to it, delete that state.

      WHEN OTHERS THEN
         IF state_lock_on THEN
            L_delete_state_autolock (version_info.state_id);
         END IF;
         IF new_state_id IS NOT NULL THEN
            L_delete_state (new_state_id);
         END IF;
         RAISE;

   END edit_version;


   FUNCTION next_row_id (owner            IN  NVARCHAR2,
                         registration_id  IN  NUMBER) RETURN NUMBER
   
  /***********************************************************************
  *
  *N  {next_row_id}  --  Return the a valid row id for a SDE table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns a valid row id for a specified table with
  *   an SDE-maintained row id column.  You should not use this function
  *   directly unless you are providing a replacement for the INSERT or 
  *   UPDATE INSTEAD OF triggers for the multiversion view.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner            <IN>  ==  (VARCHAR2) The owner of the table we
  *                                 want an id for.
  *     registration_id  <IN>  ==  (NUMBER) The registration id of the 
  *                                 table we want an id for.
  *     RETURN          <OUT>  ==  (NUMBER) The id we found.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20169                SE_NO_SDE_ROWID_COLUMN
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 08/01/00           Original coding.
  *E
  ***********************************************************************/
   IS
           
      pipe_result       INTEGER;
      id_count          INTEGER;
      id_start          INTEGER;
      increment_by      INTEGER;
      pipe_name         VARCHAR2(30);
      next_id           NUMBER;
      select_statement  VARCHAR2(64);
      sequence_name     VARCHAR2(64);

     CURSOR increment_by_cursor (s_owner  IN  VARCHAR2,
                                 s_name   IN  VARCHAR2) IS
       SELECT increment_by
       FROM   all_sequences
       WHERE  sequence_owner = s_owner AND
              sequence_name = s_name;
   
   BEGIN
   
      --  See if there are any ids for this table in its pipe.
   
      pipe_name := 'ArcSDE_IdPipe' || TO_CHAR (registration_id);
      pipe_result := DBMS_PIPE.RECEIVE_MESSAGE (pipe_name,0); 
   
      IF pipe_result = 0 THEN
     
         -- Found ids in the pipe, read them.
   
         DBMS_PIPE.UNPACK_MESSAGE (id_start);
         DBMS_PIPE.UNPACK_MESSAGE (id_count);
         next_id := id_start;
         id_start := id_start + 1;
         id_count := id_count - 1;
   
      ELSE
   
         -- Fetch ids from the sequence.  Also get the sequence's
         -- increment by value so we know how many ids we actually got.
    
         sequence_name := 'R' || TO_CHAR (registration_id);

         select_statement := 'SELECT ' || TO_CHAR(owner) || '.' || sequence_name ||
                             '.NEXTVAL FROM DUAL';
         BEGIN
            EXECUTE IMMEDIATE select_statement INTO  next_id;
         EXCEPTION
            WHEN OTHERS THEN
               raise_application_error (sde_util.SE_NO_SDE_ROWID_COLUMN,
                                        'Unable to find or access sequence ' ||
                                        owner || '.r' || 
                                        TO_CHAR (registration_id));
         END;

         OPEN increment_by_cursor (owner,sequence_name);
         FETCH increment_by_cursor INTO increment_by;
         CLOSE increment_by_cursor;

         id_start := next_id + 1;
         id_count := increment_by - 1;

      END IF;
   
      -- Write any remaining ids back into the pipe.
   
      IF id_count > 0 THEN
         DBMS_PIPE.RESET_BUFFER;
         DBMS_PIPE.PACK_MESSAGE (id_start);
         DBMS_PIPE.PACK_MESSAGE (id_count);
         pipe_result := DBMS_PIPE.SEND_MESSAGE (pipe_name,4);
      END IF;
   
      -- Return the id we found.
   
      RETURN next_id;
   
   END next_row_id;

   FUNCTION retrieve_guid RETURN NCHAR
  /***********************************************************************
  *
  *N  {retrieve_guid}  --  Return an ArcSDE-formatted GUID/UUID string
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns a GUID/UUID formatted as a 38 character 
  *   string as specified in RFC 4122, with the addition of curley braces
  *   surrounding it, like so:
  *
  *          {F81D4FAE-7DEC-11D0-A765-00A0C91E6BF6}
  *
  *   The GUID/UUID is generated by Oracle, and the method may vary from
  *   platform to platform, but should be unique in any case.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (NCHAR) The GUID/UUID string generated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             05/29/08        Original coding from example
  *                                              provided by Forrest Jones.
  *E
  ***********************************************************************/
   IS

      guid  NCHAR(38);

   BEGIN

      guid := UPPER (RAWTOHEX (SYS_GUID ()));
      RETURN '{' || SUBSTR (guid,1,8) || '-' || SUBSTR (guid,9,4) || '-' ||
             SUBSTR (guid,13,4) || '-' || SUBSTR (guid,17,4) || '-'||
             SUBSTR (guid,21,12)|| '}';

   END retrieve_guid;
   
   FUNCTION check_mv_release RETURN NVARCHAR2
  /***********************************************************************
  *
  *N  {check_mv_release}  --  Check release function 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the string "STANDARD" indicating the 
  *  changes made from CR158039 are present and that the only 
  *  multiversion view available is the standard multiversion view. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (NVARCHAR2) 'STANDARD'.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt             08/12/10        
  *E
  ***********************************************************************/
   IS
   BEGIN

     Return('DEFAULT 1.0');
     
   END check_mv_release;

  FUNCTION new_branch_state (current_state_id      IN  version_util.state_id_t,
                             current_lineage_name  IN  version_util.state_id_t,
                             new_state_id          OUT version_util.state_id_t) 
  /***********************************************************************
  *
  *N  {new_branch_state}  --  Create a new state off the current state.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function creates a new state off a previously edited 
  *  state of a multiversion view.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (NUMBER) .
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt             08/12/10        
  *E
  ***********************************************************************/
  RETURN NUMBER
  IS
    new_state_time         DATE;
    new_lineage_name       INTEGER;
    x                      INTEGER;
    ret                    INTEGER := -1;
    l_current_state_id     version_util.state_id_t;
    l_current_lineage_name version_util.state_id_t;
   
  BEGIN

    x := 1;
    ret := 0;
    l_current_state_id := current_state_id;
    l_current_lineage_name := current_lineage_name; 

    WHILE x < 4 LOOP 
      BEGIN 
        SDE.version_util.insert_state(l_current_state_id,
                                      l_current_lineage_name,
                                      SDE.version_util.C_state_is_closed,
                                      new_state_id,
                                      new_lineage_name,
                                      new_state_time); 
        SDE.version_util.change_version_state('SDE.DEFAULT',l_current_state_id,new_state_id); 
        ret := 0;

      EXCEPTION WHEN OTHERS Then 
        DECLARE err_code NUMBER := SQLCODE;
        err_msg  VARCHAR2(256) := SQLERRM; 
        BEGIN 
          IF err_code = SDE.sde_util.SE_VERSION_HAS_MOVED Then 
            ret := SDE.sde_util.SE_VERSION_HAS_MOVED; 
          End If; 
        END; 
      END; 
      If ret = SDE.sde_util.SE_VERSION_HAS_MOVED THEN 
         SELECT state_id, lineage_name INTO l_current_state_id, l_current_lineage_name 
         FROM SDE.states 
         WHERE state_id = 
             (SELECT state_id 
              FROM SDE.versions 
              WHERE name = 'DEFAULT' AND owner = 'SDE');
         x := x + 1; 
       ELSE  
         x := 4; 
       End IF;
     End Loop; 

     If ret = SDE.sde_util.SE_VERSION_HAS_MOVED Then 
       raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 
      'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.');
     ElsIF ret != 0 Then 
       raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 
     'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.');
    END If;

    SDE.lock_util.delete_state_locks_by_sde_id(SDE.pinfo_util.get_current_sde_id);

    return(SDE.sde_util.SE_SUCCESS);

  END new_branch_state;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END version_user_ddl;
