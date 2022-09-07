--------------------------------------------------------
--  DDL for Package Body VERSION_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."VERSION_UTIL" 
/***********************************************************************
*
*N  {version_util.spb}  --  Implementation for version and state DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on versions and states.  It should be compiled by the
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
*    Peter Aronson             12/15/98               Original coding.
*E
***********************************************************************/
IS

  /* Type Definitions. */

   TYPE lineage_table_t IS TABLE OF BOOLEAN INDEX BY BINARY_INTEGER;
   TYPE state_id_table_t IS TABLE OF state_id_t INDEX BY BINARY_INTEGER;

   TYPE state2_id_t IS TABLE OF integer INDEX BY BINARY_INTEGER;

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;
   G_current_state_id      state_id_t;      --  Currently set state.
   G_current_lineage_name  SDE.states.lineage_name%Type;
   G_protected             BOOLEAN NOT NULL DEFAULT FALSE;
   G_lineage_cleaned       BOOLEAN NOT NULL DEFAULT FALSE;
   G_state_id_index        BINARY_INTEGER;
   G_active_state_id       state_id_t;
   G_current_writable      BOOLEAN NOT NULL DEFAULT FALSE;
   G_lineage_list          state2_id_t;
   G_lineage_table         lineage_table_t; --  Contains current lineage.
   G_max_session_state_id  state_id_t DEFAULT C_base_state_id;
   G_use_state_id_sequence BOOLEAN NOT NULL DEFAULT FALSE;

   CURSOR G_state_owner_cursor (state_wanted_id  IN  state_id_t) IS
     SELECT owner,closing_time
     FROM   SDE.states s
     WHERE  s.state_id = state_wanted_id;

   CURSOR G_version_state_cursor (version_wanted_name  IN  version_name_t,
                                  version_wanted_owner IN  version_name_t) IS
     SELECT v.state_id,v.status
     FROM   SDE.versions v
     WHERE  v.name = version_wanted_name AND
            v.owner = version_wanted_owner;

   /* Local Subprograms. */

   PROCEDURE L_state_user_can_modify (state_id  IN  state_id_t)
  /***********************************************************************
  *
  *N  {L_state_user_can_modify}  --  Can current user modify state?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the state specified by ID exists and is
  *   modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id  <IN>  ==  (state_id_t) State to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 02/19/99           Original coding.
  *E
  ***********************************************************************/
   IS

      state_owner           G_state_owner_cursor%ROWTYPE;

   BEGIN

      -- Make sure that the state exists, and that the current user can write
      -- to it.

      OPEN G_state_owner_cursor (state_id);
      FETCH G_state_owner_cursor INTO state_owner;
      IF G_state_owner_cursor%NOTFOUND THEN
         CLOSE G_state_owner_cursor;
         raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                  'State ' || TO_CHAR (state_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_state_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != state_owner.owner THEN
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of state ' ||
                                     TO_CHAR (state_id) || '.');
         END IF;
      END IF;
    
   END L_state_user_can_modify;

   
   PROCEDURE L_version_user_can_modify (version_name  IN version_name_t) 
  /***********************************************************************
  *
  *N  {L_version_user_can_modify}  --  Can current user modify version?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the version specified by name exists and is
  *   modifiable by the current user (who must be owner or SDE DBA).  As
  *   a special case, no one, even the DBA, can modify the DEFAULT version.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version_name  <IN>  ==  (version_name_t) Version to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 02/19/99           Original coding.
  *E
  ***********************************************************************/
   IS

      version_exists           G_version_state_cursor%ROWTYPE;
      parsed_name              version_name_t;
      parsed_owner             version_name_t;

   BEGIN

      -- Parse the version name.

      parse_version_name (version_name,parsed_name,parsed_owner);

      -- Make sure this is not the default version.

      IF parsed_owner = SDE.sde_util.C_sde_dba AND
         parsed_name = C_default_version THEN
         raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                  'The default version may not be deleted ' ||
                                  'or renamed!');
      END IF;

      -- If we are not the DBA, make sure that we are the owner.

      IF NOT G_sde_dba THEN
         IF G_current_user != parsed_owner THEN
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     G_current_user ||
                                     ' Not owner of Version ' || version_name ||
                                     '.');
         END IF;
      END IF;

      -- Make sure that the version exists.

      OPEN G_version_state_cursor (parsed_name,parsed_owner);
      FETCH G_version_state_cursor INTO version_exists;
      IF G_version_state_cursor%NOTFOUND THEN
         CLOSE G_version_state_cursor;
         raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || version_name || ' not found.');
      END IF;
      CLOSE G_version_state_cursor;
    
   END L_version_user_can_modify;


   PROCEDURE L_set_base_state
  /***********************************************************************
  *
  *N  {L_set_base_state}  --  Make the current query state the base state.
  *
  ***********************************************************************/
   IS
   BEGIN
       G_lineage_list.DELETE;
       G_lineage_list (C_base_state_id) := 0;
       G_current_state_id := C_base_state_id;
   END L_set_base_state;

   FUNCTION L_delimited_find (source_string  IN  NVARCHAR2,
                              find_char      IN  NVARCHAR2) RETURN NUMBER
  /***********************************************************************
  *
  *N  {L_delimited_find}  --  Find a character in a string that might
  *                           have double-quote delimited sections to
  *                           skip over.
  *
  ***********************************************************************/
   IS

      found_at           NUMBER NOT NULL DEFAULT 0;
      source_length      NUMBER NOT NULL DEFAULT 0;
      inside_delimiters  BOOLEAN DEFAULT FALSE;
      this_char          NVARCHAR2(1);

   BEGIN

      -- See if it is easy.

      found_at := INSTR (source_string,N'"');
      IF found_at = 0 THEN
         RETURN INSTR (source_string,find_char);
      END IF;

      -- OK, we'll do it using a scan.

      source_length := LENGTH (source_string);
      FOR here IN 1 .. source_length LOOP
         this_char := SUBSTR (source_string,here,1);
         IF (NOT inside_delimiters AND this_char = find_char) THEN
            RETURN here;
         END IF;
         IF inside_delimiters THEN
            IF this_char = '"' THEN
               inside_delimiters := FALSE;
            END IF;
         ELSE
            IF this_char = N'"' THEN
               inside_delimiters := TRUE;
            END IF;
         END IF;
      END LOOP;

      -- Didn't find it.
   
      RETURN 0;

   END L_delimited_find;


   PROCEDURE L_insert_state_using_seq (parent_state_id      IN  state_id_t,
                                       parent_lineage_name  IN  state_id_t,
                                       open_or_closed       IN  PLS_INTEGER,
                                       new_state_id         OUT state_id_t,
                                       new_lineage_name     OUT state_id_t,
                                       new_state_time       OUT DATE)
  /***********************************************************************
  *
  *N  {L_insert_state_using_seq}  --  Insert a state entry into the
  *                                   STATES table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE.STATES
  *   table, using a sequence to generate the new state id.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     parent_state_id      <IN>  ==  (state_id_t) The new state's parent's
  *                                     state id.
  *     parent_lineage_name  <IN>  ==  (state_id_t) The parent state's
  *                                     lineage name.  This will be the new
  *                                     state's lineage name too, unless the
  *                                     parent state already has children.
  *     open_or_closed       <IN>  ==  (PLS_INTEGER) Controls the status of
  *                                     the state to be created:
  *                                      C_state_is_open (1)   : the state is
  *                                                              created as open;
  *                                      C_state_is_closed (2) : the state is
  *                                                              created as
  *                                                              closed.
  *     new_state_id        <OUT>  ==  (state_id_t) The unique ID of the newly
  *                                     created state.
  *     new_lineage_name    <OUT>  ==  (state_id_t) The new lineage_name ID
  *                                     assigined during the state insert.
  *     new_state_time      <OUT>  ==  (DATE) The time time create for the
  *                                     new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS
     CURSOR state_id_cursor IS
       SELECT SDE.state_id_generator_nc.nextval id
       FROM   DUAL;

     CURSOR lock_lineage (lineage_name_wanted  IN  NUMBER,
                          lineage_id_wanted    IN  NUMBER) IS
       SELECT lineage_id
       FROM SDE.state_lineages
       WHERE lineage_name = lineage_name_wanted
       AND lineage_id <= lineage_id_wanted
       FOR UPDATE;

      locked_lineage     state_list_t;
      closing_time       DATE; -- This defaults to NULL, which we need.
      creation_time      DATE DEFAULT SYSDATE;
      marked             SDE.lock_util.state_lock_t;

   BEGIN

      new_state_time := creation_time;

      -- Closing time to be the same as the creation time.

      IF open_or_closed = C_state_is_closed THEN
         closing_time := creation_time;
      END IF;

      -- Get the next state id.  We do it here, in a seperate step in case
      -- We are splitting off a new lineage, which will take two inserts.

      OPEN state_id_cursor;
      FETCH state_id_cursor INTO new_state_id;
      CLOSE state_id_cursor;

      -- Insert the record into the states table.  If fails due to to an
      -- duplicate index error, we know we have to create a new lineage.

      BEGIN

         INSERT INTO SDE.states
                   (state_id,
                    owner,
                    creation_time,
                    closing_time,
                    parent_state_id,
                    lineage_name)
         VALUES    (new_state_id,
                    G_current_user   /* owner    */,
                    creation_time,
                    closing_time,
                    parent_state_id,
                    parent_lineage_name);
        new_lineage_name := parent_lineage_name;

      EXCEPTION

         -- If we have a duplicate index failure here, odds are we need
         -- to create a new lineage.  We will use our own state id as the
         -- new lineage name.

         WHEN DUP_VAL_ON_INDEX THEN

            INSERT INTO SDE.states
                      (state_id,
                       owner,
                       creation_time,
                       closing_time,
                       parent_state_id,
                       lineage_name)
            VALUES    (new_state_id,
                       G_current_user   /* owner    */,
                       creation_time,
                       closing_time,
                       parent_state_id,
                       new_state_id);
            new_lineage_name := new_state_id;

      END;

      -- If we created a new lineage, insert it into the STATE_LINEAGE table
      -- in normalized form.

      IF new_lineage_name <> parent_lineage_name THEN

         OPEN lock_lineage (parent_lineage_name,parent_state_id);
         FETCH lock_lineage BULK COLLECT INTO locked_lineage LIMIT 1;
         CLOSE lock_lineage;

         INSERT INTO SDE.state_lineages
           SELECT new_lineage_name,l.lineage_id
           FROM   SDE.state_lineages l
           WHERE  l.lineage_name = parent_lineage_name AND
                  l.lineage_id <= parent_state_id;
      END IF;

      -- We also insert a row for this state, as if it were in its own
      -- state lineage.

      INSERT INTO SDE.state_lineages
                  (lineage_name,
                   lineage_id)
      VALUES      (new_lineage_name,
                   new_state_id);

      -- Place a mark on the new state so that it doesn't get cleaned up
      -- by compress.  Do it before the commit so it won't ever be both
      -- visible and unmarked at the same time.

      marked.sde_id    := SDE.pinfo_util.get_current_sde_id;
      marked.state_id  := new_state_id;
      marked.lock_type := SDE.lock_util.C_marked_lock;
      marked.autolock  := SDE.lock_util.C_is_autolock;
      SDE.lock_util.add_state_lock (marked);

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END L_insert_state_using_seq;


   PROCEDURE L_new_edit_state_using_seq (parent_state_id      IN  state_id_t,
                                         new_state_id         OUT state_id_t,
                                         new_lineage_name     OUT state_id_t,
                                         new_state_time       OUT DATE)
  /***********************************************************************
  *
  *N  {L_new_edit_state_using_seq}  --  Close parent state and create child
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure closes a state, making it unmodifiable and a legal
  *   candidate to be a parent state, then inserts a server-supplied entry
  *   into the SDE.STATES table as closed state's child, using a sequence
  *   to avoid conflicts around determining the new state id.  A unique row
  *   id is generated and returned, and a exclusive user lock is placed on
  *   the new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     parent_state_id      <IN>  ==  (state_id_t) The new state's parent's
  *                                     state id.
  *     new_state_id        <OUT>  ==  (state_id_t) The unique ID of the newly
  *                                     created state.
  *     new_lineage_name    <OUT>  ==  (state_id_t) The new lineage_name ID
  *                                     assigined during the state insert.
  *     new_state_time      <OUT>  ==  (DATE) The time time create for the
  *                                     new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS

     CURSOR state_id_cursor IS
       SELECT SDE.state_id_generator_nc.nextval id
       FROM   DUAL;

     CURSOR  parent_info_cursor (state_wanted_id  IN  state_id_t) IS
       SELECT owner,closing_time,lineage_name
       FROM   SDE.states s
       WHERE  s.state_id = state_wanted_id;

     CURSOR lock_lineage (lineage_name_wanted  IN  NUMBER,
                          lineage_id_wanted    IN  NUMBER) IS
       SELECT lineage_id
       FROM SDE.state_lineages
       WHERE lineage_name = lineage_name_wanted
       AND lineage_id <= lineage_id_wanted
       FOR UPDATE;

      locked_lineage     state_list_t;
      parent_info        parent_info_cursor%ROWTYPE;
      closing_time       DATE; -- This defaults to NULL, which we need.
      creation_time      DATE DEFAULT SYSDATE;
      sde_id             state_id_t;

   BEGIN

      new_state_time := creation_time;

      -- Make sure that the parent state exists, and that the current user can
      -- write to it.

      OPEN parent_info_cursor (parent_state_id);
      FETCH parent_info_cursor INTO parent_info;
      IF parent_info_cursor%NOTFOUND THEN
         CLOSE parent_info_cursor;
         raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                  'State ' || TO_CHAR (parent_state_id) ||
                                  ' not found.');
      END IF;
      CLOSE parent_info_cursor;
      IF parent_info.closing_time IS NULL THEN
         IF NOT G_sde_dba THEN
            IF G_current_user != parent_info.owner THEN
               raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                        'Not owner of state ' ||
                                        TO_CHAR (parent_state_id) || '.');
            END IF;
         END IF;
      END IF;

      -- If it is open, close the parent state.

      IF parent_info.closing_time IS NULL THEN
         UPDATE SDE.states
         SET    closing_time = L_new_edit_state_using_seq.creation_time
         WHERE  state_id = L_new_edit_state_using_seq.parent_state_id;
      END IF;

      -- Get the next state id.  We do it here, in a seperate step in case
      -- We are splitting off a new lineage, which will take two inserts.

      OPEN state_id_cursor;
      FETCH state_id_cursor INTO new_state_id;
      CLOSE state_id_cursor;

      -- Insert the record into the states table.  If fails due to to an
      -- duplicate index error, we know we have to create a new lineage.

      BEGIN

         new_lineage_name := parent_info.lineage_name;

        INSERT INTO SDE.states
                    (state_id,
                     owner,
                     creation_time,
                     closing_time,
                     parent_state_id,
                     lineage_name)
        VALUES      (new_state_id,
                     G_current_user   /* owner    */,
                     creation_time,
                     closing_time,
                     parent_state_id,
                     new_lineage_name);

         EXCEPTION

         -- If we have a duplicate index failure here, odds are we need
         -- to create a new lineage.  We will use our own state id as the
         -- new lineage name.

         WHEN DUP_VAL_ON_INDEX THEN

         new_lineage_name := new_state_id;

         INSERT INTO SDE.states
                    (state_id,
                     owner,
                     creation_time,
                     closing_time,
                     parent_state_id,
                     lineage_name)
         VALUES     (new_state_id,
                     G_current_user   /* owner    */,
                     creation_time,
                     closing_time,
                     parent_state_id,
                     new_lineage_name);
      END;

      -- If we created a new lineage, insert it into the STATE_LINEAGE table
      -- in normalized form.

      IF new_lineage_name <> parent_info.lineage_name THEN

         OPEN lock_lineage (parent_info.lineage_name,parent_state_id);
         FETCH lock_lineage BULK COLLECT INTO locked_lineage LIMIT 1;
         CLOSE lock_lineage;

         INSERT INTO SDE.state_lineages
         SELECT new_lineage_name,l.lineage_id
         FROM   SDE.state_lineages l
         WHERE  l.lineage_name = parent_info.lineage_name AND
                l.lineage_id <= parent_state_id;
      END IF;

      -- We also insert a row for this state, as if it were in its own
      -- state lineage.

      INSERT INTO SDE.state_lineages
                  (lineage_name,
                   lineage_id)
      VALUES      (new_lineage_name,
                   new_state_id);

      -- Place a lock entry in the STATE_LOCKS table.  Doing this directly
      -- is both safe and necessary.  Safe, as this is a newly created state
      -- so there can not be a conflict; necessary as this function needs to
      -- be efficient and secure, this is the only way to avoid rechecking
      -- the current user's access rights.

      sde_id := SDE.pinfo_util.get_current_sde_id;

      INSERT INTO SDE.state_locks
        (sde_id,state_id,autolock,lock_type)
      VALUES
        (sde_id,new_state_id,
        SDE.lock_util.C_is_not_autolock,SDE.lock_util.C_exclusive_lock);

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.
      --
      -- COMMIT;
      --
      -- However, the commit has been moved from here so that it can be
      -- used to start a new user transaction; thus, the commit is performed
      -- in the dblayer code that invokes this stored procedure instead of
      -- here.

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END L_new_edit_state_using_seq;

   /* Public Subprograms. */

   PROCEDURE insert_state (parent_state_id      IN  state_id_t,
                           parent_lineage_name  IN  state_id_t,
                           open_or_closed       IN  PLS_INTEGER,
                           new_state_id         OUT state_id_t,
                           new_lineage_name     OUT state_id_t,
                           new_state_time       OUT DATE) 
  /***********************************************************************
  *
  *N  {insert_state}  --  insert a state entry into the STATES table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE.STATES
  *   table.  A unique row id is generated and returned.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     parent_state_id      <IN>  ==  (state_id_t) The new state's parent's
  *                                     state id.
  *     parent_lineage_name  <IN>  ==  (state_id_t) The parent state's 
  *                                     lineage name.  This will be the new
  *                                     state's lineage name too, unless the
  *                                     parent state already has children.
  *     open_or_closed       <IN>  ==  (PLS_INTEGER) Controls the status of 
  *                                     the state to be created: 
  *                                      C_state_is_open (1)   : the state is 
  *                                                              created as open;
  *                                      C_state_is_closed (2) : the state is 
  *                                                              created as
  *                                                              closed.
  *     new_state_id        <OUT>  ==  (state_id_t) The unique ID of the newly
  *                                     created state.
  *     new_lineage_name    <OUT>  ==  (state_id_t) The new lineage_name ID
  *                                     assigined during the state insert.
  *     new_state_time      <OUT>  ==  (DATE) The time time create for the
  *                                     new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS

      PRAGMA AUTONOMOUS_TRANSACTION;

     CURSOR state_id_cursor IS
       SELECT SDE.state_id_generator_nc.nextval id
       FROM   DUAL;

     CURSOR new_id_cursor IS
       SELECT MAX (state_id) + 1 new_id
       FROM   SDE.states;

     CURSOR lock_lineage (lineage_name_wanted  IN  NUMBER,
                          lineage_id_wanted    IN  NUMBER) IS 
       SELECT lineage_id 
       FROM SDE.state_lineages
       WHERE lineage_name = lineage_name_wanted
       AND lineage_id <= lineage_id_wanted
       FOR UPDATE;

      locked_lineage     state_list_t;
      closing_time       DATE; -- This defaults to NULL, which we need.
      creation_time      DATE DEFAULT SYSDATE;
      marked             SDE.lock_util.state_lock_t;

   BEGIN

      -- See if we are generating state ids with a sequence.

      IF G_use_state_id_sequence THEN
         L_insert_state_using_seq (parent_state_id,
                                   parent_lineage_name,
                                   open_or_closed,
                                   new_state_id,
                                   new_lineage_name,
                                   new_state_time);
         RETURN;
      END IF;

      new_state_time := creation_time; 

      -- Closing time to be the same as the creation time.

      IF open_or_closed = C_state_is_closed THEN
         closing_time := creation_time;
      END IF;

      -- The following loop is repeated until we have successfully inserted
      -- our row with a valid state id.  We need this loop dispite the lock
      -- because of replication -- a process on another database could have
      -- derived the same number as it wouldn't see our lock.

      <<state_id_loop>> LOOP

         -- The following block is present to catch the DUP_VAL_ON_INDEX 
         -- exception that occurs when a state is inserted that has an id
         -- already present in the SDE.states table, so that it may be
         -- properly handled.

         BEGIN

            -- Start by fetching the current max state id + 1.  If we've
            -- seen numbers this high or higher before, base our id off
            -- the highest number we've seen in our session to prevent
            -- the problematic recycling of state ids within a session.
     
            OPEN new_id_cursor;
            FETCH new_id_cursor INTO new_state_id;
            IF new_id_cursor%NOTFOUND THEN
               new_state_id := 1;
            END IF;
            CLOSE new_id_cursor;

            IF new_state_id > G_max_session_state_id THEN
               G_max_session_state_id := new_state_id;
            ELSIF new_state_id <= G_max_session_state_id THEN
               G_max_session_state_id := G_max_session_state_id + 1;
               new_state_id := G_max_session_state_id;
            END IF;

            -- Insert the record into the states table; set the owner 
            -- from the current environment.

            INSERT INTO SDE.states 
                        (state_id,
                         owner,
                         creation_time,
                         closing_time,
                         parent_state_id,
                         lineage_name)
            VALUES      (new_state_id,
                         G_current_user                 /* owner    */,
                         creation_time,
                         closing_time,
                         parent_state_id,
                         new_state_id);          /* Temporary value. */
            EXIT state_id_loop;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
               NULL;
         END;
      END LOOP;

         -- Update the lineage_name to be the same as the parent state's.  
         -- If this fails due to to an duplicate index error, we know we 
         -- have to create a new lineage.

         BEGIN

            UPDATE SDE.states 
            SET    lineage_name = parent_lineage_name
            WHERE  state_id = new_state_id;
            new_lineage_name := parent_lineage_name;

         EXCEPTION

            -- If we have a duplicate index failure here, odds are we need
            -- to create a new lineage.  We will use our own state id as the
            -- new lineage name, which is already in the state record.

            WHEN DUP_VAL_ON_INDEX THEN

               new_lineage_name := new_state_id;

         END;
 

      -- If we created a new lineage, insert it into the STATE_LINEAGE table
      -- in normalized form.
        
      IF new_lineage_name <> parent_lineage_name THEN

         OPEN lock_lineage (parent_lineage_name,parent_state_id);
         FETCH lock_lineage BULK COLLECT INTO locked_lineage LIMIT 1;
         CLOSE lock_lineage;

         INSERT INTO SDE.state_lineages
           SELECT new_lineage_name,l.lineage_id
           FROM   SDE.state_lineages l
           WHERE  l.lineage_name = parent_lineage_name AND
                  l.lineage_id <= parent_state_id;
      END IF;

      -- We also insert a row for this state, as if it were in its own
      -- state lineage.

      INSERT INTO SDE.state_lineages
                  (lineage_name,
                   lineage_id)
      VALUES      (new_lineage_name,
                   new_state_id);

      -- Place a mark on the new state so that it doesn't get cleaned up
      -- by compress.  Do it before the commit so it won't ever be both
      -- visible and unmarked at the same time.

      marked.sde_id    := SDE.pinfo_util.get_current_sde_id;
      marked.state_id  := new_state_id;
      marked.lock_type := SDE.lock_util.C_marked_lock;
      marked.autolock  := SDE.lock_util.C_is_autolock;
      SDE.lock_util.add_state_lock (marked);

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END insert_state;


   PROCEDURE del_mvmod_base_save(reg_list     IN registry_util.reg_list_t,
                                 high_id      IN state_id_t,
                                 lineage_name IN state_id_t)
  /***********************************************************************
  *
  *N  {del_mvmod_base_save}  --  Delete from MVTABLES_MODIFIED
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes from the mvtables_modified table based on
  *   a list of registration ids and a subquery that forms a join to the
  *   states table. The low state id is the base state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    reg_list      <IN>  ==  (registry_util.reg_list_t) Array of 
  *                             registration ids.
  *    high_id        <IN>  ==  (state_id_t) The high state id for the
  *                              subquery that forms a join to the states
  *                              table.
  *    lineage_name   <IN>  ==  (state_id_t) Lineage name to be used in 
  *                              the subquery.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke            07/01/04           Original coding.
  *E
  ***********************************************************************/
   IS

      BEGIN

      FORALL del_mvtable_regid IN reg_list.FIRST..reg_list.LAST
      DELETE FROM SDE.mvtables_modified
      WHERE  registration_id = reg_list(del_mvtable_regid) AND
             state_id IN (SELECT state_id 
                          FROM SDE.states 
                          WHERE state_id > C_base_state_id AND 
                                state_id <= high_id AND
                                lineage_name = del_mvmod_base_save.lineage_name);

      END del_mvmod_base_save;


   PROCEDURE L_delete_states (state_list  IN  state_list_t)
  /***********************************************************************
  *
  *N  {L_delete_states}  --  Delete an arbitary list of states
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary list of states.  All checking
  *   and locking should be performed by the gsrvr, except that we will
  *   check state ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_list  <IN>  ==  (state_list_t) List of states (by id) to 
  *                            delete.  Note: deletes of the base state 
  *                            are quietly ignored.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *    Peter Aronson                 03/22/01           Don't delete base state.
  *E
  ***********************************************************************/
   IS

     CURSOR state_test_cursor (state_wanted_id  IN  state_id_t) IS
       SELECT s.owner,s.lineage_name
       FROM   SDE.states s
       WHERE  s.state_id = state_wanted_id;

     CURSOR lineage_test_cursor (lineage_wanted_name  IN  state_id_t) IS
       SELECT s.lineage_name
       FROM   SDE.states s
       WHERE  (s.lineage_name = lineage_wanted_name OR 
               s.lineage_name = -lineage_wanted_name) AND
              ROWNUM = 1;

      state_info           state_test_cursor%ROWTYPE;
      state_id_index       BINARY_INTEGER;
      lineage_name_table   state_id_table_t;
      lineage_name_count   BINARY_INTEGER;
      lineage_name_index   BINARY_INTEGER;
      duplicate            BOOLEAN;
      lineage_name         INTEGER;
      ordered_state_list   state_list_t;
      ordered_state_from   BINARY_INTEGER;
      ordered_state_pos    BINARY_INTEGER;

   BEGIN

      -- Invert the state list to avoid violating the RI constraints.

      ordered_state_from := state_list.LAST;
      ordered_state_pos := 1;
      WHILE ordered_state_from IS NOT NULL LOOP
        ordered_state_list(ordered_state_pos) := state_list(ordered_state_from); 
        ordered_state_from := state_list.PRIOR (ordered_state_from);
        ordered_state_pos := ordered_state_pos + 1;
      END LOOP;

      -- Loop through the table of state ids, fetching each state in turn to
      -- make sure it exists, and (if we are not the SDE DBA) that the current
      -- user owns it.  Also, accumulate a list of unique lineage names.

      lineage_name_count := 0;
      state_id_index := state_list.FIRST;
      WHILE state_id_index IS NOT NULL LOOP
         OPEN state_test_cursor (state_list (state_id_index));
         FETCH state_test_cursor INTO state_info;
         IF state_test_cursor%NOTFOUND THEN
            CLOSE state_test_cursor;
            raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                     'State ' || 
                                     TO_CHAR (state_list (state_id_index)) ||
                                     ' not found.');
         END IF;
         CLOSE state_test_cursor;
         IF NOT G_sde_dba THEN
            IF G_current_user != state_info.owner THEN
               raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                        'Not owner of state ' ||
                                        TO_CHAR (state_list (state_id_index)) ||
                                        '.');
            END IF;
         END IF;
         state_id_index := state_list.NEXT (state_id_index);
         lineage_name_index := 1;
         duplicate := FALSE;
         WHILE lineage_name_index <= lineage_name_count AND NOT duplicate LOOP
            IF lineage_name_table (lineage_name_index) = state_info.lineage_name
            THEN
               duplicate := TRUE;
            ELSE
               lineage_name_index := lineage_name_index + 1;
            END IF;
         END LOOP;
         IF NOT duplicate THEN
            lineage_name_count := lineage_name_count + 1;
            lineage_name_table (lineage_name_count) := state_info.lineage_name;
         END IF;
      END LOOP;

      -- We've verified our permissions and the states' existence, so it must
      -- be OK to delete them.  Do it.

      FORALL del_states IN ordered_state_list.FIRST..ordered_state_list.LAST
         DELETE FROM SDE.states
         WHERE  state_id = ordered_state_list (del_states) AND 
                state_id <> C_base_state_id;

      -- Delete any rows associated with this state in the state_lineages 
      -- and mvtables_modified tables.  (This step used to be performed by table
      -- constraints with DELETE CASCADE, but this proved to be too buggy at
      -- Oracle 8.0.*, and we had to remove them.)

      -- Delete from state_lineages by lineage_id.

      FORALL by_lineage_id IN state_list.FIRST..state_list.LAST
         DELETE FROM SDE.state_lineages
         WHERE  lineage_id = state_list (by_lineage_id) AND 
                lineage_id <> C_base_state_id;

      -- Delete from mvtables_modified. 

      FORALL del_mvtable IN state_list.FIRST..state_list.LAST
         DELETE FROM SDE.mvtables_modified
         WHERE  state_id = state_list (del_mvtable) AND 
                state_id <> C_base_state_id;

      -- Delete any automatically placed exclusive state locks.

      FORALL del_statelock IN state_list.FIRST..state_list.LAST
         DELETE FROM SDE.state_locks
         WHERE  state_id = state_list (del_statelock) AND 
                state_id <> C_base_state_id AND
                autolock = SDE.lock_util.C_is_autolock AND
                lock_type = SDE.lock_util.C_exclusive_lock;

      -- Check our accumulation of lineage names by querying the states
      -- table for each one.  Any that are no longer present in the states
      -- table should also be cleared out of the state_lineages table too.
  
      FOR name_index IN lineage_name_table.FIRST .. lineage_name_table.LAST LOOP
         OPEN lineage_test_cursor (lineage_name_table (name_index));
         FETCH lineage_test_cursor INTO lineage_name;
         IF lineage_test_cursor%FOUND THEN
             lineage_name_table.DELETE (name_index);
             lineage_name_count := lineage_name_count - 1;
         END IF;
         CLOSE lineage_test_cursor;
      END LOOP;

      IF lineage_name_count > 0 THEN
         lineage_name_index := 1;
         FOR name_index IN 1 .. lineage_name_table.LAST LOOP
            IF lineage_name_table.EXISTS (name_index) THEN
               lineage_name_table (lineage_name_index) := 
                                                lineage_name_table (name_index);
               lineage_name_index := lineage_name_index + 1;
            END IF;
         END LOOP;
      
         FORALL name_index IN 1 .. lineage_name_count
            DELETE FROM SDE.state_lineages
            WHERE  lineage_name = lineage_name_table (name_index);
      END IF;

   END L_delete_states;


   PROCEDURE delete_states (state_list  IN  state_list_t)
  /***********************************************************************
  *
  *N  {delete_states}  --  Delete an arbitary list of states
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary list of states.  All checking
  *   and locking should be performed by the gsrvr, except that we will
  *   check state ownership if the invoking user is not the SDE DBA.
  *   This procedure performs a main-line transaction commit.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_list  <IN>  ==  (state_list_t) List of states (by id) to 
  *                            delete.  Note: deletes of the base state 
  *                            are quietly ignored.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *     -20175                SE_STATE_HAS_CHILDREN
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *    Peter Aronson                 03/22/01           Don't delete base state.
  *    Peter Aronson                 02/14/06           Check for children.
  *E
  ***********************************************************************/
   IS

     CURSOR child_count_cursor (state_wanted_id  IN  state_id_t) IS
       SELECT COUNT(s.state_id) child_count
       FROM   SDE.states s
       WHERE  s.parent_state_id = state_wanted_id;

      child_count  INTEGER;

   BEGIN

      -- If we are deleting a single state, we add an additional check to make
      -- sure that this state has no child states.  This prevents some potential
      -- timing problems with compress.

      IF state_list.COUNT = 1 THEN
         OPEN child_count_cursor (state_list(1));
         FETCH child_count_cursor INTO child_count;
         CLOSE child_count_cursor;
         IF child_count <> 0 THEN
            raise_application_error (SDE.sde_util.SE_STATE_HAS_CHILDREN,
                                     'State ' || TO_CHAR (state_list(1)) ||
                                     ' can not be deleted, it has children.');
         END IF;
      END IF;

      -- Do the actual deletes.

      L_delete_states (state_list);

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_states;


   PROCEDURE close_state (state_id          IN  state_id_t,
                          state_close_time  OUT DATE )
  /***********************************************************************
  *
  *N  {close_state}  --  Close a state
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure closes a state, making it unmodifiable and a legal
  *   candidate to be a parent state.  All checking and locking should be
  *   performed by the gsrvr, except that we will check state ownership if
  *   the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id  <IN>  ==  (state_id_t) State to be closed.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      state_close_time := SYSDATE;

      -- Make sure that the state exists, and that the current user can write
      -- to it.

      L_state_user_can_modify (state_id);
    
      -- Perform the actual update that closes the state.

      UPDATE SDE.states
      SET    closing_time = state_close_time
      WHERE  state_id = close_state.state_id;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END close_state;


   PROCEDURE open_state (state_id          IN   state_id_t,
                         state_close_time  OUT  DATE)
  /***********************************************************************
  *
  *N  {open_state}  --  Open a state
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure opens a state, making it modifiable and not a legal
  *   candidate to be a parent state.  All checking and locking should be
  *   performed by the gsrvr, except that we will check state ownership if
  *   the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id           <IN>  ==  (state_id_t) State to be opened.
  *     state_close_time  <OUT>  ==  (DATE) The time the state was closed.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      state_closing_time     DATE DEFAULT NULL;
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
 
      state_close_time := SYSDATE;  

      -- Make sure that the state exists, and that the current user can write
      -- to it.

      L_state_user_can_modify (state_id);
    
      -- Perform the actual update that opens the state.

      UPDATE SDE.states
      SET    closing_time = state_closing_time
      WHERE  state_id = open_state.state_id;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END open_state;


   PROCEDURE trim_state (high_state_id  IN  state_id_t,
                         low_state_id   IN  state_id_t,
                         delete_list    IN  state_list_t)
  /***********************************************************************
  *
  *N  {trim_state}  --  Update high state for trim + remove trim'ed states
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the high state's definition for trim. 
  *   Additionally, this procedure deletes all states being removed by trim.
  *   All checking and locking should be performed by the gsrvr, except 
  *   that we will check state ownership if the invoking user is not the SDE
  *   DBA.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     high_state_id  <IN>  ==  (state_id_t) The high state in the trim;
  *                               the state to be updated.
  *     low_state_id   <IN>  ==  (state_id_t) The low state in the trim.
  *     delete_list    <IN>  ==  (state_list_t) List of states (by id) to 
  *                               delete.  Note: deletes of the base state 
  *                               are quietly ignored.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *    Peter Aronson                 10/06/00           Became trim_state.
  *    Peter Aronson                 07/08/03           Add state deleting.
  *    Peter Aronson                 11/10/05           Partial re-write for
  *                                                     ref integrity support.
  *E
  ***********************************************************************/
   IS
 
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Make sure that the high state exists, and that the current user can
      -- write to it.

      L_state_user_can_modify (high_state_id);
    
      -- If the low state is the base state, convert the high state into the 
      -- base state.

      IF low_state_id = C_base_state_id THEN

         -- We need to delete any modified flags before changing the high state
         -- to be the base state, or the states<->mvtables_modified integrity
         -- constraint will be violated, aborting the following UPDATE.
         -- Similarly, we must also remove old state_lineages entries.
 
         DELETE FROM SDE.mvtables_modified
         WHERE  state_id  = high_state_id;
 
         DELETE FROM SDE.state_lineages
         WHERE  lineage_id = high_state_id;

         -- We need to make sure that there is a 0,0 entry in the 
         -- state_lineages, so we will insert it, but ignore the error in case
         -- it already exists.

         BEGIN
            INSERT INTO SDE.state_lineages
                        (lineage_name,lineage_id)
            VALUES      (C_base_state_id,C_base_state_id);
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
               NULL;
         END;

         -- Make sure the base state is closed and proper.
 
         UPDATE SDE.states
         SET    parent_state_id = C_base_state_id,
                owner = sde_util.C_sde_dba,
                closing_time = NVL (closing_time,SYSDATE),
                lineage_name = C_base_state_id
         WHERE state_id = C_base_state_id;

         -- Make the lineage_name negative of any immediate child state 
         -- of the state becoming the base state, so that when we update
         -- the parent_state_id to become the base_state_id, we don't 
         -- violate the states_uk constraint on parent_state_id and 
         -- lineage_name.

         UPDATE SDE.states
         SET    lineage_name = -1 * lineage_name
         WHERE  parent_state_id = high_state_id;

         -- Update the parent_id of any immediate child state of the state
         -- becoming the base state to be the base state.

         UPDATE SDE.states
         SET    parent_state_id = C_base_state_id
         WHERE  parent_state_id = high_state_id;

         -- Update any versions based on the state becoming the base state
         -- to point at the base state instead.

         UPDATE SDE.versions
         SET    state_id = C_base_state_id
         WHERE  state_id = high_state_id;
        
         -- Remove the high_state now that it has been compressed.

         DELETE FROM SDE.states
         WHERE  state_id = high_state_id;
        
         -- Delete the states being removed by the trim.
  
         L_delete_states (delete_list);

         -- Uninvert the inverted lineage names; once the delete is done
         -- it is safe to put them back.

         UPDATE SDE.states
         SET    lineage_name = -1 * lineage_name
         WHERE  lineage_name < 0;

      -- If the low state is not the base state, give the high state the
      -- low state's previous parent id.

      ELSE

         -- Update the parent_id but also invert the lineage id to avoid
         -- violating states_uk.

         UPDATE SDE.states
         SET    parent_state_id = (SELECT parent_state_id
                                   FROM SDE.states 
                                   WHERE state_id = low_state_id),
                lineage_name = -1 * lineage_name
         WHERE  state_id = high_state_id;

         -- Delete the states being removed by the trim.
 
         L_delete_states (delete_list);

         -- Return the lineage id to a positive number.

         UPDATE SDE.states
         SET    lineage_name = -1 * lineage_name
         WHERE  state_id = high_state_id;

      END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END trim_state;


   PROCEDURE new_edit_state (parent_state_id      IN  state_id_t,
                             new_state_id         OUT state_id_t,
                             new_lineage_name     OUT state_id_t,
                             new_state_time       OUT DATE)
  /***********************************************************************
  *
  *N  {new_edit_state}  --  Close parent state and create child
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure closes a state, making it unmodifiable and a legal
  *   candidate to be a parent state, then inserts a server-supplied entry
  *   into the SDE.STATES table as closed state's child.  A unique row id 
  *   is generated and returned, and a exclusive user lock is placed on the
  *   new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     parent_state_id      <IN>  ==  (state_id_t) The new state's parent's
  *                                     state id.
  *     new_state_id        <OUT>  ==  (state_id_t) The unique ID of the newly
  *                                     created state.
  *     new_lineage_name    <OUT>  ==  (state_id_t) The new lineage_name ID
  *                                     assigined during the state insert.
  *     new_state_time      <OUT>  ==  (DATE) The time time create for the
  *                                     new state.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
     CURSOR state_id_cursor IS
       SELECT SDE.state_id_generator_nc.nextval id
       FROM   DUAL;

     CURSOR new_id_cursor IS
       SELECT MAX (state_id) + 1 new_id
       FROM   SDE.states;

     CURSOR  parent_info_cursor (state_wanted_id  IN  state_id_t) IS
       SELECT owner,closing_time,lineage_name
       FROM   SDE.states s
       WHERE  s.state_id = state_wanted_id;

     CURSOR lock_lineage (lineage_name_wanted  IN  NUMBER,
                          lineage_id_wanted    IN  NUMBER) IS 
       SELECT lineage_id 
       FROM SDE.state_lineages
       WHERE lineage_name = lineage_name_wanted
       AND lineage_id <= lineage_id_wanted
       FOR UPDATE;

      locked_lineage     state_list_t;
      parent_info        parent_info_cursor%ROWTYPE;
      closing_time       DATE; -- This defaults to NULL, which we need.
      creation_time      DATE DEFAULT SYSDATE;
      sde_id             state_id_t;

   BEGIN

      -- See if we are generating state ids with a sequence.

      IF G_use_state_id_sequence THEN
         L_new_edit_state_using_seq (parent_state_id,
                                     new_state_id,
                                     new_lineage_name,
                                     new_state_time);
         RETURN;
      END IF;

      new_state_time := creation_time; 

      -- Make sure that the parent state exists, and that the current user can
      -- write to it.

      OPEN parent_info_cursor (parent_state_id);
      FETCH parent_info_cursor INTO parent_info;
      IF parent_info_cursor%NOTFOUND THEN
         CLOSE parent_info_cursor;
         raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                  'State ' || TO_CHAR (parent_state_id) ||
                                  ' not found.');
      END IF;
      CLOSE parent_info_cursor;
      IF parent_info.closing_time IS NULL THEN
         IF NOT G_sde_dba THEN
            IF G_current_user != parent_info.owner THEN
               raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                        'Not owner of state ' ||
                                        TO_CHAR (parent_state_id) || '.');
            END IF;
         END IF;
      END IF;

      -- If it is open, close the parent state.

      IF parent_info.closing_time IS NULL THEN
         UPDATE SDE.states
         SET    closing_time = new_edit_state.creation_time
         WHERE  state_id = new_edit_state.parent_state_id;
      END IF;

      -- The following loop is repeated until we have successfully inserted
      -- our row with a valid state id.  We need this loop dispite the lock
      -- because of replication -- a process on another database could have
      -- derived the same number as it wouldn't see our lock.

      <<state_id_loop>> LOOP

         -- The following block is present to catch the DUP_VAL_ON_INDEX 
         -- exception that occurs when a state is inserted that has an id
         -- already present in the SDE.states table, so that it may be
         -- properly handled.

         BEGIN

            -- Start by fetching the current max state id + 1.  If we've
            -- seen numbers this high or higher before, base our id off
            -- the highest number we've seen in our session to prevent
            -- the problematic recycling of state ids within a session.

            OPEN new_id_cursor;
            FETCH new_id_cursor INTO new_state_id;
            IF new_id_cursor%NOTFOUND THEN
               new_state_id := 1;
            END IF;
            CLOSE new_id_cursor;

            IF new_state_id > G_max_session_state_id THEN
               G_max_session_state_id := new_state_id;
            ELSIF new_state_id <= G_max_session_state_id THEN
               G_max_session_state_id := G_max_session_state_id + 1;
                new_state_id := G_max_session_state_id;
            END IF;

            -- Insert the record into the states table; set the owner 
            -- from the current environment.

            INSERT INTO SDE.states 
                        (state_id,
                         owner,
                         creation_time,
                         closing_time,
                         parent_state_id,
                         lineage_name)
            VALUES      (new_state_id,
                         G_current_user                 /* owner    */,
                         creation_time,
                         closing_time,
                         parent_state_id,
                         new_state_id);         /* Temporary value. */
            EXIT state_id_loop;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
               NULL;
         END;
      END LOOP;

         -- Update the lineage_name to be the same as the parent state's.  
         -- If this fails due to to an duplicate index error, we know we 
         -- have to create a new lineage.

         BEGIN

            UPDATE SDE.states 
            SET    lineage_name = parent_info.lineage_name
            WHERE  state_id = new_state_id;
            new_lineage_name := parent_info.lineage_name;

         EXCEPTION

            -- If we have a duplicate index failure here, odds are we need
            -- to create a new lineage.  We will use our own state id as the
            -- new lineage name, which is already in the state record.

            WHEN DUP_VAL_ON_INDEX THEN

               new_lineage_name := new_state_id;

         END;

      -- If we created a new lineage, insert it into the STATE_LINEAGE table
      -- in normalized form.
        
      IF new_lineage_name <> parent_info.lineage_name THEN

         OPEN lock_lineage (parent_info.lineage_name,parent_state_id);
         FETCH lock_lineage BULK COLLECT INTO locked_lineage LIMIT 1;
         CLOSE lock_lineage;

         INSERT INTO SDE.state_lineages
           SELECT new_lineage_name,l.lineage_id
           FROM   SDE.state_lineages l
           WHERE  l.lineage_name = parent_info.lineage_name AND
                  l.lineage_id <= parent_state_id;
      END IF;

      -- We also insert a row for this state, as if it were in its own
      -- state lineage.

      INSERT INTO SDE.state_lineages
                  (lineage_name,
                   lineage_id)
      VALUES      (new_lineage_name,
                   new_state_id);

      -- Place a lock entry in the STATE_LOCKS table.  Doing this directly
      -- is both safe and necessary.  Safe, as this is a newly created state
      -- so there can not be a conflict; necessary as this function needs to
      -- be efficient and secure, this is the only way to avoid rechecking
      -- the current user's access rights.

      sde_id := SDE.pinfo_util.get_current_sde_id;

      INSERT INTO SDE.state_locks
        (sde_id,state_id,autolock,lock_type)
      VALUES
        (sde_id,new_state_id,
        SDE.lock_util.C_is_not_autolock,SDE.lock_util.C_exclusive_lock);

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.  
      --
      -- COMMIT;
      --
      -- However, the commit has been moved from here so that it can be
      -- used to start a new user transaction; thus, the commit is performed
      -- in the dblayer code that invokes this stored procedure instead of
      -- here.

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END new_edit_state;


   PROCEDURE insert_version (version           IN   version_record_t,
                             name_rule         IN   PLS_INTEGER,
                             new_version_name  OUT  SDE.versions.name%TYPE)
  /***********************************************************************
  *
  *N  {insert_version}  --  Insert a version entry into the VERSIONS table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE.VERSIONS
  *   table.  A unique name may be generated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_record_t) The description of
  *                                   the version to insert.
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
  *     new_version_name  <OUT>  ==  (version_name_t) The version name as
  *                                   actually inserted.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20177                SE_VERSION_EXISTS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

      suffix              INTEGER NOT NULL DEFAULT 0;
      local_version_name  NVARCHAR2(65);
      local_owner         SDE.sde_util.identifier_t;

   BEGIN

      new_version_name := version.name;
      local_version_name := new_version_name;
      IF G_sde_dba THEN
         local_owner := version.owner;
      ELSE
         local_owner := G_current_user;
      END IF;

      -- The following loop is to repeat attempts at generating a usable 
      -- version name.

      <<name_loop>>
      LOOP

         -- The following block is present to catch the DUP_VAL_ON_INDEX 
         -- exception that occurs when a version is inserted that has a name
         -- already present in the SDE.versions table, so that it may be
         -- properly handled.

         BEGIN
            INSERT INTO SDE.versions
                        (name,
                         owner,
                         version_id,
                         status,
                         state_id,
                         description,
                         parent_name,
                         parent_owner,
                         parent_version_id,
                         creation_time)
            VALUES      (local_version_name,
                         local_owner,
                         version.version_id,
                         version.status,
                         version.state_id,
                         version.description,
                         version.parent_name,
                         version.parent_owner,
                         version.parent_version_id,
                         SYSDATE);
            EXIT name_loop;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
               IF name_rule = C_take_name_as_given THEN
                  raise_application_error (SDE.sde_util.SE_VERSION_EXIST,
                                     'Version ' || version.name || 
                                     ' already exists.');
               END IF;
               suffix := suffix + 1;
         END;
         local_version_name := RTRIM (version.name) || TO_NCHAR (suffix);
         IF LENGTH (RTRIM (local_version_name)) > 64 THEN
            raise_application_error (SDE.sde_util.SE_VERSION_EXIST,
                                     'Unable to generate a name for ' ||
                                     version.name || '.');
         END IF;
         new_version_name := local_version_name;
      END LOOP name_loop;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END insert_version;


   PROCEDURE insert_version_in_trans 
                                (version           IN   version_record_t,
                                 name_rule         IN   PLS_INTEGER,
                                 new_version_name  OUT  SDE.versions.name%TYPE)
  /***********************************************************************
  *
  *N  {insert_version_in_trans}  --  Insert version entry into VERSIONS table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE.VERSIONS
  *   table without a commit.  A unique name may be generated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_record_t) The description of
  *                                   the version to insert.
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
  *     new_version_name  <OUT>  ==  (version_name_t) The version name as
  *                                   actually inserted.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20177                SE_VERSION_EXISTS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 07/18/06           Original coding.
  *E
  ***********************************************************************/
   IS
     
      suffix              INTEGER NOT NULL DEFAULT 0;
      local_version_name  NVARCHAR2(65);
      local_owner         SDE.sde_util.identifier_t;

   BEGIN

      new_version_name := version.name;
      local_version_name := new_version_name;
      
      IF G_sde_dba THEN
         local_owner := version.owner;
      ELSE
         local_owner := G_current_user;
      END IF;

      -- The following loop is to repeat attempts at generating a usable 
      -- version name.

      <<name_loop>>
      LOOP

         -- The following block is present to catch the DUP_VAL_ON_INDEX 
         -- exception that occurs when a version is inserted that has a name
         -- already present in the SDE.versions table, so that it may be
         -- properly handled.

         BEGIN
            INSERT INTO SDE.versions
                        (name,
                         owner,
                         version_id,
                         status,
                         state_id,
                         description,
                         parent_name,
                         parent_owner,
                         parent_version_id,
                         creation_time)
            VALUES      (local_version_name,
                         local_owner,
                         version.version_id,
                         version.status,
                         version.state_id,
                         version.description,
                         version.parent_name,
                         version.parent_owner,
                         version.parent_version_id,
                         SYSDATE);
            EXIT name_loop;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
               IF name_rule = C_take_name_as_given THEN
                  raise_application_error (SDE.sde_util.SE_VERSION_EXIST,
                                     'Version ' || version.name || 
                                     ' already exists.');
               END IF;
               suffix := suffix + 1;
         END;
         local_version_name := RTRIM (version.name) || TO_NCHAR (suffix);
         IF LENGTH (RTRIM (local_version_name)) > 64 THEN
            raise_application_error (SDE.sde_util.SE_VERSION_EXIST,
                                     'Unable to generate a name for ' ||
                                     version.name || '.');
         END IF;
         new_version_name := local_version_name;
      END LOOP name_loop;

   END insert_version_in_trans;


   PROCEDURE update_version (version  IN  version_record_t)
  /***********************************************************************
  *
  *N  {update_version}  --  Update a version entry into the VERSIONS table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates an existing version's definition.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_record_t) The description of
  *                                   the version to update.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20126                SE_VERSION_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Make sure that the current user is SDE DBA or owner the version.

      IF NOT G_sde_dba THEN
         IF version.owner != G_current_user THEN    
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of Version ' || 
                                     version.owner || '.' || 
                                     version.name || '.');
         END IF;
      END IF;

      -- Perform the update.

      UPDATE SDE.versions
      SET    status = version.status,
             description = version.description
      WHERE  owner = version.owner AND
             name = version.name;

      -- Make sure that something was updated.

      IF SQL%NOTFOUND THEN
         raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || version.owner || '.' ||
                                  version.name || ' not found.');
      END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END update_version;


   PROCEDURE delete_version (name  IN  version_name_t)
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
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

      parsed_name   SDE.versions.name%TYPE;
      parsed_owner  SDE.versions.owner%TYPE;
      v_state_id    state_id_t;  
      
      CURSOR find_children_cursor IS
        SELECT v.name
        FROM   SDE.versions v
        WHERE  v.parent_name = parsed_name AND
               v.parent_owner = parsed_owner;

     child_record   find_children_cursor%ROWTYPE;
     child_found    BOOLEAN;

   BEGIN

      -- Make sure we have delete privileges on this version.

      L_version_user_can_modify (name);

      -- Make sure that this version has no children.

      parse_version_name (name,parsed_name,parsed_owner);
      OPEN find_children_cursor;
      FETCH find_children_cursor INTO child_record;
      child_found := find_children_cursor%FOUND;
      CLOSE find_children_cursor;
      IF child_found THEN
         raise_application_error (SDE.sde_util.SE_VERSION_HAS_CHILDREN,
                                  'Version ' || name || 
                                  ' can not be deleted, as it has children.');
      END IF;

      SELECT state_id into v_state_id
      FROM SDE.versions
      WHERE name = parsed_name 
      AND owner = parsed_owner;

      IF (G_version_owner = parsed_owner AND G_version_name = parsed_name) OR
         (G_current_state_id = v_state_id AND G_edit_mode_default = FALSE) THEN
         raise_application_error (SDE.sde_util.SE_MVV_VERSION_IN_USE,
                                  'Version ' || name || 
                                  ' can not be deleted, it is currently in use.');
      END IF;
      
      -- Perform the delete.

      DELETE FROM SDE.versions
      WHERE  name = parsed_name AND owner = parsed_owner;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_version;


   PROCEDURE change_version_state (name          IN  version_name_t,
                                   old_state_id  IN  state_id_t,
                                   new_state_id  IN  state_id_t)
  /***********************************************************************
  *
  *N  {change_version_state}  --  Change version from one state to another
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure moves a version from being based on one state to
  *   being based on another state.  It must be based on the old state,
  *   or an exception will be raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_name_t) The name of the
  *                                   version to change state for, in 
  *                                   either <name> or <owner>.<name> 
  *                                   format.
  *     old_state_id       <IN>  ==  (state_id_t) The state the version
  *                                   must currently be based on.
  *     new_state_id       <IN>  ==  (state_id_t) The state the version
  *                                   is to be based on now.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS     (Not raised yet.)
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20174                SE_VERSION_HAS_MOVED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

      version_exists  G_version_state_cursor%ROWTYPE;
      parsed_name     SDE.versions.name%TYPE;
      parsed_owner    SDE.versions.owner%TYPE;

   BEGIN

      -- Try to perform the update, assuming the version is at the specified
      -- old state.  If it fails, we will need a query to distinguish between
      -- the version not existing, and it existing, but already have had its
      -- state changed.

      parse_version_name (name,parsed_name,parsed_owner);
      UPDATE SDE.versions
      SET    state_id = new_state_id
      WHERE  name = parsed_name AND 
             owner = parsed_owner AND 
             state_id = old_state_id;

      IF SQL%NOTFOUND THEN
         OPEN G_version_state_cursor (parsed_name,parsed_owner);
         FETCH G_version_state_cursor INTO version_exists;
         IF G_version_state_cursor%NOTFOUND THEN
            CLOSE G_version_state_cursor;
            raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                     'Version ' || name || ' not found.');
         END IF;
         CLOSE G_version_state_cursor;
         raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED,
                                  'Version ' || name || ' is no longer state ' ||
                                  TO_CHAR (old_state_id) || '.');
      END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END change_version_state;


   PROCEDURE change_version_state_in_trans (name          IN  version_name_t,
                                            old_state_id  IN  state_id_t,
                                            new_state_id  IN  state_id_t)
  /***********************************************************************
  *
  *N  {change_version_state_in_trans}  --  Change version state in transaction
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure moves a version from being based on one state to
  *   being based on another state.  It must be based on the old state,
  *   or an exception will be raised.  Unlike change_version_state, this
  *   procedure works in the current transaction environment.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_name_t) The name of the
  *                                   version to change state for, in 
  *                                   either <name> or <owner>.<name> 
  *                                   format.
  *     old_state_id       <IN>  ==  (state_id_t) The state the version
  *                                   must currently be based on.
  *     new_state_id       <IN>  ==  (state_id_t) The state the version
  *                                   is to be based on now.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS     (Not raised yet.)
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20174                SE_VERSION_HAS_MOVED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             12/18/98         Original coding.
  *    Peter Aronson             01/25/05         Stole from change_version_state
  *E
  ***********************************************************************/
   IS
     
      version_exists  G_version_state_cursor%ROWTYPE;
      parsed_name     SDE.versions.name%TYPE;
      parsed_owner    SDE.versions.owner%TYPE;

   BEGIN

      -- Try to perform the update, assuming the version is at the specified
      -- old state.  If it fails, we will need a query to distinguish between
      -- the version not existing, and it existing, but already have had its
      -- state changed.

      parse_version_name (name,parsed_name,parsed_owner);
      UPDATE SDE.versions
      SET    state_id = new_state_id
      WHERE  name = parsed_name AND 
             owner = parsed_owner AND 
             state_id = old_state_id;

      IF SQL%NOTFOUND THEN
         OPEN G_version_state_cursor (parsed_name,parsed_owner);
         FETCH G_version_state_cursor INTO version_exists;
         IF G_version_state_cursor%NOTFOUND THEN
            CLOSE G_version_state_cursor;
            raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                     'Version ' || name || ' not found.');
         END IF;
         CLOSE G_version_state_cursor;
         raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED,
                                  'Version ' || TO_CHAR(name) || ' is no longer state ' ||
                                  TO_CHAR (old_state_id) || '.');
      END IF;

   END change_version_state_in_trans;

   PROCEDURE rename_version (owner     IN  SDE.versions.owner%TYPE,
                             old_name  IN  SDE.versions.name%TYPE,
                             new_name  IN  SDE.versions.name%TYPE)
  /***********************************************************************
  *
  *N  {rename_version}  --  Rename a version entry in the VERSIONS table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure renames an existing version.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner              <IN>  ==  (SDE.versions.owner%TYPE) The owner of
  *                                   the version to be renamed.
  *     old_name           <IN>  ==  (SDE.versions.name%TYPE) The old name 
  *                                   of the version to rename; must be a
  *                                   simple name.
  *     new_name           <IN>  ==  (SDE.versions.name%TYPE) The new name
  *                                   of the version to rename; must be a 
  *                                   simple name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *     -20177                SE_VERSION_EXISTS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

      test_name                version_name_t;
      parsed_name              version_name_t;
      parsed_owner             version_name_t;

   BEGIN

      -- Make sure we have modify privileges on this version.

      parse_version_name (old_name,parsed_name,parsed_owner);
      test_name := owner || N'.' || parsed_name;
      L_version_user_can_modify (test_name);

      -- Perform the update of the version row; use a block to catch any
      -- violation of the unique constraint on version name.

      BEGIN
         UPDATE SDE.versions
         SET    name = new_name
         WHERE  name = old_name AND
                owner = rename_version.owner;
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX THEN
            raise_application_error (SDE.sde_util.SE_VERSION_EXIST,
                                     'A version named ' || new_name ||
                                     ' already exists.');
      END;

      -- Now, rename the parent version field in versions that had the old
      -- version name as their parent version.

      UPDATE SDE.versions
      SET    parent_name = new_name
      WHERE  parent_name = old_name AND
             parent_owner = rename_version.owner;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END rename_version;


   PROCEDURE touch_lineage (lineage_name        IN  state_id_t,
                            time_last_modified  IN  DATE)
  /***********************************************************************
  *
  *N  {touch_lineage}  --  Update the time last modified for a lineage
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the time last modified for a specified
  *   lineage.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     lineage_name        <IN>  ==  (state_id_t) The name of the lineage
  *                                    to have its time update.
  *     time_last_modified  <IN>  ==  (DATE) The time the lineage was last
  *                                    modified.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 06/05/03           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;


      CURSOR check_tlm_cursor (check_lineage_name  IN  INTEGER) IS
        SELECT time_last_modified 
        FROM   SDE.lineages_modified 
        WHERE  lineage_name = check_lineage_name 
        FOR UPDATE; 

      CURSOR test_and_lock_cursor IS
        SELECT 1
        FROM   SDE.lineages_modified
        WHERE  SYSDATE - time_last_modified > 2
        FOR UPDATE NOWAIT;

        found_time_last_modified   DATE; 
        new_time_last_modified     DATE; 
        a_second                   NUMBER := 1/(24 * 60 * 60); 
        lineage_found              BOOLEAN; 
        dummy                      NUMBER;
        lock_conflict              EXCEPTION;

      PRAGMA EXCEPTION_INIT (lock_conflict,-54);

   BEGIN 
      OPEN check_tlm_cursor (lineage_name); 
      FETCH check_tlm_cursor INTO found_time_last_modified; 
      lineage_found := check_tlm_cursor%FOUND; 
      CLOSE check_tlm_cursor; 

      IF lineage_found THEN 

         IF time_last_modified > found_time_last_modified THEN 
            new_time_last_modified := time_last_modified; 
         ELSE 
            new_time_last_modified := found_time_last_modified + a_second; 
         END IF; 

         UPDATE SDE.lineages_modified 
         SET    time_last_modified = new_time_last_modified 
         WHERE  lineage_name = touch_lineage.lineage_name; 
          
      ELSE 

         INSERT INTO SDE.lineages_modified 
         VALUES (lineage_name,time_last_modified); 

      END IF;

      IF NOT G_lineage_cleaned THEN 

         G_lineage_cleaned := TRUE;

         BEGIN

            OPEN test_and_lock_cursor;
            FETCH test_and_lock_cursor INTO dummy;
            CLOSE test_and_lock_cursor;

         DELETE FROM SDE.lineages_modified 
         WHERE  SYSDATE - time_last_modified > 2; 

         EXCEPTION

            WHEN lock_conflict THEN
              NULL;

         END;

      END IF; 

      COMMIT; 

   END touch_lineage;


   PROCEDURE parse_version_name (version_name  IN  version_name_t,
                                 parsed_name   OUT SDE.versions.name%TYPE,
                                 parsed_owner  OUT SDE.versions.owner%TYPE)
  /***********************************************************************
  *
  *N  {parse_version_name}  --  Parse a version name into name and owner
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure parses the supplied version name into name and owner
  *   parts.  If the name is a simple name, then current user name is 
  *   returned as owner.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version_name  <IN>  ==  (version_name_t) Version to be parsed.
  *     parsed_name  <OUT>  ==  (SDE.versions.name%TYPE) The name part of
  *                              of version_name.
  *     parsed_owner <OUT>  ==  (SDE.versions.owner%TYPE) The owner part of
  *                              version_name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20171                SE_INVALID_VERSION_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 02/19/99           Original coding.
  *E
  ***********************************************************************/
   IS

      dot_at   NUMBER NOT NULL DEFAULT 0;

   BEGIN

      dot_at := L_delimited_find (version_name,N'.');
      IF dot_at <> 0 THEN
         parsed_owner := SUBSTR (version_name,1,dot_at - 1);
         IF SUBSTR (parsed_owner,1,1) <> N'"' THEN
           parsed_owner := UPPER (SUBSTR (version_name,1,dot_at - 1));
         END IF;
         parsed_name := SUBSTR (version_name,dot_at + 1);
      ELSE
         parsed_name := version_name;
         parsed_owner := G_current_user;
      END IF;
      IF RTRIM (parsed_name,N' ') IS NULL OR
         RTRIM (parsed_owner,N' ') IS NULL THEN
         RAISE VALUE_ERROR;
      END IF;

   EXCEPTION

      WHEN OTHERS THEN
         raise_application_error (sde_util.SE_INVALID_VERSION_NAME,
                                  '"' || NVL (version_name,'(null)') || 
                                  '" is not a valid version name.');

   END parse_version_name;


   PROCEDURE range_delete_ok (start_state  IN  state_id_t,
                              end_state    IN  state_id_t)
  /***********************************************************************
  *
  *N  {range_delete_ok}  --  Check if legal to range delete
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function determines if it is legal to delete a range of 
  *   states specified as a low state and a high state.  To be legal, all 
  *   of the following conditions must be true:
  *
  *   (1) There must be no branches in the range leading from the start
  *       state to the end state.
  *   (2) The end state must be a leaf node.
  *   (3) The current user must either own all of the states to be deleted,
  *       or be the SDE DBA.
  *   (4) No state to be deleted may be in use by a version.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     start_state   <IN>  ==  (state_id_t) The lower state in the range to
  *                              be deleted.
  *     end_state     <IN>  ==  (state_id_t) The upper state in the range to
  *                              be deleted.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *     -20175                SE_STATE_HAS_CHILDREN
  *     -20179                SE_STATE_USED_BY_VERSION
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             12/18/98                 Original coding.
  *E
  ***********************************************************************/
   IS
      CURSOR state_tree_cursor IS 
        SELECT s.state_id,
               s.owner,
               s.parent_state_id
        FROM   SDE.states s
        START WITH s.state_id = start_state
        CONNECT BY PRIOR s.state_id = s.parent_state_id
        ORDER BY   s.parent_state_id;

      CURSOR version_check_cursor (check_state_id IN state_id_t) IS
        SELECT v.name
        FROM   SDE.versions v
        WHERE  v.state_id = check_state_id;

      start_state_found     BOOLEAN NOT NULL DEFAULT FALSE;
      end_state_found       BOOLEAN NOT NULL DEFAULT FALSE;
      last_parent_state_id  state_id_t NOT NULL DEFAULT -1;
      version_name          version_name_t;

   BEGIN

      -- Loop through the state tree, looking for illegal entries.

      FOR state_record IN state_tree_cursor LOOP
         start_state_found := TRUE;

          -- Test (1), no branches.

         IF last_parent_state_id = state_record.parent_state_id THEN
            raise_application_error (SDE.sde_util.SE_STATE_HAS_CHILDREN,
                                     'State ' ||
                                     TO_CHAR (last_parent_state_id) ||
                                     ' in range has too many children.');
         END IF;
         last_parent_state_id := state_record.parent_state_id;

         -- Test (2), end state must be a leaf node.

         IF end_state_found THEN
            raise_application_error (SDE.sde_util.SE_STATE_HAS_CHILDREN,
                                     'End State may not have children.');
         END IF;
         end_state_found := state_record.state_id = end_state;

         -- Test (3), unless we are the SDE DBA, we must own all the states.

         IF NOT G_sde_dba THEN
           IF G_current_user != state_record.owner THEN
             raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                      'Not owner of state ' ||
                                      TO_CHAR (state_record.state_id) || '.');
           END IF;
        END IF;

        -- Test (4), no state must be used by a version.

        OPEN version_check_cursor (state_record.state_id);
        FETCH version_check_cursor INTO version_name;
        CLOSE version_check_cursor;
        IF version_name IS NOT NULL THEN
           raise_application_error (SDE.sde_util.SE_STATE_USED_BY_VERSION,
                                    'State ' ||
                                    TO_CHAR (state_record.state_id) ||
                                    ' is used by version ' || version_name ||
                                    '.');
        END IF;

      END LOOP;

      -- Make sure the end state exists.

      IF NOT end_state_found OR NOT start_state_found THEN
         raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                  'Start or End stare is missing.');
      END IF;

   END range_delete_ok;


   PROCEDURE flag_mvtable_modified 
                               (mvtable_id  IN  registry_util.registration_id_t,
                                state_id    IN  state_id_t)
  /***********************************************************************
  *
  *N  {flag_mvtable_modified}  --  Flag a multiversion table as modified
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function flags a multiversion table (identified by id) as
  *   modified in the specified state.  This information is recorded in
  *   the SDE.MVTABLES_MODIFIED table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     mvtable_id  <IN>  ==  (sde_util.registration_id_t) The unique
  *                            registration ID of the multiversion table
  *                            to be flagged as modified.
  *     state_id    <IN>  ==  (state_id_t) State which the mvtable was
  *                            modified in.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             02/17/99                 Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN
      INSERT INTO SDE.mvtables_modified
      VALUES (state_id,mvtable_id);
   EXCEPTION 
      -- Ignore duplicate entries
      WHEN DUP_VAL_ON_INDEX THEN
         NULL;
   END flag_mvtable_modified;


   PROCEDURE current_version_writable
  /***********************************************************************
  *
  *N  {current_versions_writable}  --  Check if current user can write to
  *                                    current version
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure raises an appropriate exception if for any reason
  *   the current set version in not writable: owned by a different user, 
  *   closed or no longer exists.  It is used as a test by intellegent 
  *   multiversion table view triggers.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/18/98           Original coding.
  *E
  ***********************************************************************/
   IS

      state_info           G_state_owner_cursor%ROWTYPE;

   BEGIN

      -- Is this a protected state owned by someone else?

      IF G_protected THEN
         raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                  'Current version is protected, and you ' ||
                                  'are not the owner.');
      END IF;

      -- Make sure that the state exists, and that the current user can write
      -- to it.

      IF G_active_state_id IS NULL THEN
         G_active_state_id := G_current_state_id;
      END IF;

      IF G_current_writable = FALSE OR G_current_state_id <> G_active_state_id THEN

         G_active_state_id := G_current_state_id;
         OPEN G_state_owner_cursor (G_current_state_id);
         FETCH G_state_owner_cursor INTO state_info;
         IF G_state_owner_cursor%NOTFOUND THEN
            CLOSE G_state_owner_cursor;
            raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                                     'State ' || TO_CHAR (G_current_state_id) ||
                                     ' not found.');
      END IF;
      CLOSE G_state_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != state_info.owner THEN
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of state ' ||
                                     TO_CHAR (G_current_state_id) || '.');
         END IF;
      END IF;
      IF state_info.closing_time IS NOT NULL THEN
         raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                  'State ' || TO_CHAR (G_current_state_id) || 
                                  ' is closed.');
      END IF;
    
         G_current_writable := TRUE;

      END IF;
     
   END current_version_writable;

   PROCEDURE set_default
  /***********************************************************************
  *
  *N  {set_default}  --  Set current session to Default version.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function sets the current session to the Default version 
  * by resetting the global state_id and global flags back to their 
  * default setting for the Default version.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                 11/17/2010           Original coding.
  *E
  ***********************************************************************/
   IS
      not_found      boolean;
      version_info   G_version_state_cursor%ROWTYPE;

      CURSOR C_get_lineage_name IS
        SELECT distinct l.lineage_name 
        FROM   SDE.state_lineages l,
               SDE.states s
        WHERE  s.state_id = version_info.state_id AND
               l.lineage_name = s.lineage_name AND
               l.lineage_id <= version_info.state_id;

   BEGIN

     G_edit_mode_default := TRUE;
     G_default_version_set := FALSE;
     G_version_owner := SDE.sde_util.C_sde_dba;
     G_version_name  := UPPER('DEFAULT');
     G_edit_state := C_edit_state_stop;
     G_lineage_list.DELETE;

     OPEN G_version_state_cursor (C_default_version,sde_util.C_sde_dba);
     FETCH G_version_state_cursor INTO version_info;
     not_found := G_version_state_cursor%NOTFOUND;
     CLOSE G_version_state_cursor;
     IF not_found THEN
        raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                 'No DEFAULT version');
     END IF;

     OPEN C_get_lineage_name;
     FETCH C_get_lineage_name INTO G_current_lineage_name;
     CLOSE C_get_lineage_name;

     G_current_state_id := version_info.state_id;

   END set_default;

   PROCEDURE set_current_version (version_name  IN  version_name_t)
  /***********************************************************************
  *
  *N  {set_current_version}  --  Set current query state by version name.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function sets the current state for intelligent multiversion
  *   views by version name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     version            <IN>  ==  (version_name_t) The name of the
  *                                   version to set, in either <name>
  *                                   or <owner>.<name> format.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20172                SE_STATE_NOEXIST
  *     -20126                SE_VERSION_NOEXIST
  *     -20171                SE_INVALID_VERSION_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 03/18/99           Original coding.
  *E
  ***********************************************************************/
   IS

      version_info             G_version_state_cursor%ROWTYPE;
      parsed_name              version_name_t;
      parsed_owner             version_name_t;
      not_found                BOOLEAN;
      first_state_found        BOOLEAN DEFAULT FALSE;
      binary_status            PLS_INTEGER;
      lineage_name             number;
      ret                      number;

      CURSOR state_tree_cursor IS
        SELECT l.lineage_id state_id
        FROM   SDE.state_lineages l,
               SDE.states s
        WHERE  s.state_id = version_info.state_id AND
               l.lineage_name = s.lineage_name AND
               l.lineage_id <= version_info.state_id;

   BEGIN

      -- Parse the version name.

      parse_version_name (version_name,parsed_name,parsed_owner);

      IF (parsed_owner != G_version_owner OR
         parsed_name != G_version_name) AND 
         G_edit_state = C_edit_state_start THEN
        raise_application_error (SDE.sde_util.SE_MVV_IN_EDIT_MODE,
                                  'Cannot set version with an open transaction to another version');
      END IF;

      -- Fetch the state id.

      OPEN G_version_state_cursor (parsed_name,parsed_owner);
      FETCH G_version_state_cursor INTO version_info;
      not_found := G_version_state_cursor%NOTFOUND;
      CLOSE G_version_state_cursor;
      IF not_found THEN
         raise_application_error (SDE.sde_util.SE_VERSION_NOEXIST,
                                  'Version ' || version_name || ' not found.');
      END IF;

      -- Check the version status: if private, we must be owner to continue,
      -- if protected, note for future use.

      binary_status := version_info.status;
      binary_status := binary_status - FLOOR (binary_status / 4) * 4;
      IF binary_status = C_version_private THEN
         IF NOT G_sde_dba THEN
            IF G_current_user != parsed_owner THEN
               raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                        G_current_user ||
                                        ' Not owner of Version ' || 
                                        version_name || '.');
            END IF;
         END IF;
         G_protected := FALSE;
      ELSIF binary_status = C_version_protected THEN
         G_protected := ((NOT G_sde_dba) AND (G_current_user != parsed_owner));
      ELSE
         G_protected := FALSE;
      END IF;

      -- If the requested state is already set, do nothing.

      IF version_info.state_id = G_current_state_id THEN
         NULL;

      -- If setting the base state, do it w/o Database access.

      ELSIF version_info.state_id = version_util.C_base_state_id THEN
        L_set_base_state;            

      -- Otherwise, load the lineage into the table from the database.
      ELSE

        G_current_state_id := version_info.state_id;

      END IF;


      -- We succeeded, so it's time to set the default version set
      -- flag.

     G_default_version_set := parsed_name = C_default_version AND
                              parsed_owner = SDE.sde_util.C_sde_dba;

      -- Set the version name to the cached version name 

     G_version_owner := UPPER(parsed_owner);
     G_version_name := parsed_name;

     G_edit_mode_default := FALSE;
     lineage_name := get_lineage();

     G_state_id_index := G_lineage_list.FIRST;
     WHILE G_state_id_index IS NOT NULL LOOP
       IF NOT first_state_found THEN -- On first pass, initialize.
         G_lineage_table.DELETE;
         G_current_state_id := version_info.state_id;
         first_state_found := TRUE;
       END IF;
      
       G_lineage_table (G_lineage_list (G_state_id_index)) := TRUE;
       G_state_id_index := G_lineage_list.NEXT (G_state_id_index);

     END LOOP;

     -- Make sure we found *something*.

     IF NOT first_state_found THEN
       raise_application_error (SDE.sde_util.SE_STATE_NOEXIST,
                               'State ' ||
                                TO_CHAR (version_info.state_id) ||
                               ' not found.');
     END IF;

   END set_current_version;

   FUNCTION in_current_lineage (state_id   IN NUMBER)
  /***********************************************************************
  *
  *N  {in_current_lineage}  --  Is supplied state ID in current lineage?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     Is the supplied state id in the currently set (by set_current_state)
  *   state's lineage?
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_id    <IN>  ==  (NUMBER) State to test.
  *     RETURN     <OUT>  ==  (NUMBER) If in lineage:
  *                                      sde_util.C_true;
  *                                    If not in lineage:
  *                                      sde_util.C_false;
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             03/18/99                 Original coding.
  *E
  ***********************************************************************/
  RETURN NUMBER 

  IS 
    i             integer;

  BEGIN
    IF (G_default_version_set != TRUE AND 
        G_edit_mode_default = TRUE) THEN -- DEFAULT MODE

   i := G_lineage_list.first;
      while i IS NOT NULL LOOP
        if state_id = G_lineage_list(i) Then 
          return(SDE.sde_util.C_true);
        End If;
        i := G_lineage_list.next(i);
      End Loop;
    
      return(SDE.sde_util.C_false);
   
 ELSE -- STANDARD MODE
   BEGIN -- We need this block to catch unset entry access.
     IF G_lineage_table (state_id) THEN
       return(SDE.sde_util.C_true);
     ELSE
          return(SDE.sde_util.C_false);
        END IF;
   
   EXCEPTION
  WHEN NO_DATA_FOUND THEN
    return(SDE.sde_util.C_false);
   END;
    END IF;
     
   END in_current_lineage;

  FUNCTION current_state RETURN NUMBER
  /***********************************************************************
  *
  *N  {current_state}  --  Return currently set state id.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     Return the currently set (by set_current_state) state id.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN     <OUT>  ==  (NUMBER) The current state id.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson             03/18/99                 Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      RETURN G_current_state_id;

   END current_state;

   PROCEDURE current_version_not_default
  /***********************************************************************
  *
  *N  {current_version_not_default}  --  Make sure the current version is
  *                                       not the DEFAULT version
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure raises an appropriate exception if the currently set 
  *   version is the default version (<dba_schema>.DEFAULT).  This used by
  *   multiversion views on archiving tables to make sure they don't make 
  *   changes that will not eventually be detected and carried over to the
  *   archive table.
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
  *    Peter Aronson                 10/21/08           Original coding.
  *E
  ***********************************************************************/
   IS

      state_info           G_state_owner_cursor%ROWTYPE;

   BEGIN

      -- Is this a protected state owned by someone else?

      IF G_default_version_set THEN
         raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                  'You may not use this interface to modify ' ||
                                  'archiving tables in the DEFAULT version.');
      END IF;

   END current_version_not_default;

FUNCTION get_lineage
 /***********************************************************************
  *
  *N  {get_lineage}  --  Get the current lineage for the current version
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure get the current lineage for the current version
  *  set to the session. 
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
  *    Kevin Watt                 12/18/10           Original coding.
  *E
  ***********************************************************************/
RETURN NUMBER
IS

  CURSOR C_get_lineage_name (version_name version_name_t,
                              version_owner version_name_t )IS
        SELECT l.lineage_id
        FROM   SDE.state_lineages l,
               SDE.states s,
               (SELECT v.state_id,v.status 
                FROM   SDE.versions v 
                WHERE  v.name = version_name AND 
                       v.owner = version_owner) v 
        WHERE  s.state_id = v.state_id AND 
               l.lineage_name = s.lineage_name AND 
               l.lineage_id <= v.state_id;

BEGIN

  G_lineage_list.DELETE;

  If ((G_default_version_set != TRUE 
          And G_edit_mode_default = TRUE) OR -- DEFAULT Mode
      (G_default_version_set = TRUE 
          And G_edit_mode_default = FALSE ))Then -- STANDARD Mode, DEFAULT version
     OPEN C_get_lineage_name (C_default_version,SDE.sde_util.C_sde_dba);
     FETCH C_get_lineage_name BULK COLLECT INTO G_lineage_list;
     CLOSE C_get_lineage_name;
  Elsif G_default_version_set = FALSE And
         G_edit_mode_default = FALSE Then  -- STANDARD Mode, Named version
     OPEN C_get_lineage_name (G_version_name,G_version_owner);
     FETCH C_get_lineage_name BULK COLLECT INTO G_lineage_list;
     CLOSE C_get_lineage_name;
  End If;

   return(1);
END get_lineage;

FUNCTION get_lineage_list 
 /***********************************************************************
  *
  *N  {get_lineage_list}  --  Get the current lineage for the current version
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure get the current lineage for the Default version
  *  only. Named versions use set_current_version or edit_version to set
  *  the lineage list once per call. 
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
  *    Kevin Watt                 12/18/10           Original coding.
  *E
  ***********************************************************************/
RETURN NUMBER
IS
  
  ret number;

BEGIN

  If G_default_version_set != TRUE And 
     G_edit_mode_default = TRUE Then
    ret := get_lineage;
  End If;

  return(1);

END get_lineage_list;

PROCEDURE preserve_edits (preserve_mode IN BOOLEAN DEFAULT TRUE)
  /***********************************************************************
  *
  *N  {preserve_edits}  --  Set the Default SQL editing mode to preserve
  *                         edits even if the Default references state 0 (TRUE).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function sets the Default edit mode to always preserve edits
  *  made with SQL. This will cause inserts, updates, and deletes to always
  *  add new records to the delta and archive (_H) tables.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Forrest Jones                 06/27/2019           Original coding.
  *E
  ***********************************************************************/
   IS

   preserve_mode_val    INTEGER NOT NULL DEFAULT 1;

   BEGIN

     IF preserve_mode = TRUE THEN
       preserve_mode_val := 1;
     ELSE
       preserve_mode_val := 0;
     END IF;

     BEGIN
       SDE.SVR_CONFIG_UTIL.insert_server_config(C_def_preserve_edits_param, '', preserve_mode_val);
       EXCEPTION 
         WHEN DUP_VAL_ON_INDEX THEN
           SDE.SVR_CONFIG_UTIL.update_server_config(C_def_preserve_edits_param, '', preserve_mode_val);
         WHEN OTHERS THEN
           RAISE;
     END;

     G_edit_mode_def_preserve_edits := preserve_mode;

   END preserve_edits;

PROCEDURE set_batch_mode (sql_batch_mode IN BOOLEAN DEFAULT TRUE)
  /***********************************************************************
  *
  *N  {set_default_sql_batch_mode}  --  Set SQL editing batch mode (TRUE).
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function sets the batch edit mode to TRUE or FALSE for edits
  *  made with SQL. The GDB_FROM_DATE and GDB_TO_DATE columns in the 
  *  archive table are of a DATE Datatype. This means that they cannot 
  *  store times with more precision than a second (ie: they cannot store
  *  fractional seconds). This is a known Oracle limitation with the DATE
  *  datatype. So if multiple updates to a given record occur within less
  *  than a second of each other, you may encounter a unique constraint
  *  violation error. Update edit triggers read the global variable this
  *  method sets to indicate if it should pause one second at completion
  *  to ensure the next edit will avoid the constraint violation.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Forrest Jones                 07/01/2019           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

     G_sql_batch_edit := sql_batch_mode;

   END set_batch_mode;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);
   G_current_state_id := C_base_state_id;

   -- The following block fetches the DEFAULT version's state id and sets
   -- it as the current default state.  Much of -- this code is duplicated 
   -- from set_current_version() in order to avoid strange PRAGMA 
   -- RESTRICT_REFERENCES problems.

   DECLARE

      version_info             G_version_state_cursor%ROWTYPE;
      not_found                BOOLEAN;
      binary_status            PLS_INTEGER;
      svr_cfg_num_val          INTEGER;

      CURSOR state_tree_cursor IS
        SELECT l.lineage_id state_id
        FROM   SDE.state_lineages l,
               SDE.states s
        WHERE  s.state_id = version_info.state_id AND
               l.lineage_name = s.lineage_name AND
               l.lineage_id <= version_info.state_id;

      CURSOR svr_config_cursor (param_wanted  IN  NVARCHAR2) IS
        SELECT num_prop_value
        FROM   SDE.server_config
        WHERE  prop_name = param_wanted;

   BEGIN

      -- Fetch the state id.

      OPEN G_version_state_cursor (C_default_version,sde_util.C_sde_dba);
      FETCH G_version_state_cursor INTO version_info;
      not_found := G_version_state_cursor%NOTFOUND;
      CLOSE G_version_state_cursor;
      IF not_found THEN
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'No DEFAULT version');
      END IF;

      -- Check the version status: if protected, note for future use.

      binary_status := version_info.status;
      binary_status := binary_status - FLOOR (binary_status / 4) * 4;
      IF binary_status = C_version_private THEN
         G_protected := FALSE;
         raise_application_error (sde_util.SE_VERSION_NOEXIST,
                                  'Inaccessible DEFAULT version');
      ELSIF binary_status = C_version_protected THEN
         G_protected := NOT G_sde_dba;
      ELSE
         G_protected := FALSE;
      END IF;

      -- load the lineage into the table from the database.

      G_current_state_id := version_info.state_id;

      -- Fetch the sql default preserve edits mode

      OPEN svr_config_cursor (C_def_preserve_edits_param);
      FETCH svr_config_cursor INTO svr_cfg_num_val;
      not_found := svr_config_cursor%NOTFOUND;
      IF not_found THEN
        svr_cfg_num_val := -1;
      END IF;
      CLOSE svr_config_cursor;

      IF svr_cfg_num_val = 1 THEN
        G_edit_mode_def_preserve_edits := TRUE;
      ELSE
        G_edit_mode_def_preserve_edits := FALSE;
      END IF;

      -- See if we are generating state ids using a sequence or
      -- not.

      OPEN svr_config_cursor (C_def_stateid_sequence_param);
      FETCH svr_config_cursor INTO svr_cfg_num_val;
      not_found := svr_config_cursor%NOTFOUND;
      IF not_found THEN
        svr_cfg_num_val := -1;
      END IF;
      CLOSE svr_config_cursor;

      IF svr_cfg_num_val = 1 THEN
        G_use_state_id_sequence := TRUE;
      ELSE
        G_use_state_id_sequence := FALSE;
      END IF;

   EXCEPTION

      WHEN OTHERS THEN
         IF SQLCODE <> sde_util.SE_VERSION_NOEXIST THEN
            RAISE;
         END IF;

   END;

END version_util;
