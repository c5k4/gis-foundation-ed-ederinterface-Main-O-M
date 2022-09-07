Prompt drop Package Body LOCK_UTIL;
DROP PACKAGE BODY SDE.LOCK_UTIL
/

Prompt Package Body LOCK_UTIL;
--
-- LOCK_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.lock_util
/***********************************************************************
*
*N  {lock_util.spb}  --  Implementation for ArcSDE Shared Lock Package
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   operations on ArcSDE's shared layer, state, table and object locks.
*   It should be compiled by the SDE DBA user.
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
*    Peter Aronson             01/25/00               Original coding.
*E
***********************************************************************/
IS

   PROCEDURE L_dbms_lock_allocate_unique (lock_name    IN  VARCHAR2,
                                          lock_handle OUT  VARCHAR2)
  /***********************************************************************
  *
  *N  {L_dbms_lock_allocate_unique}  --  Perform a wrapped ALLOCATE_UNIQUE
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure performs a DBMS_LOCK.ALLOCATE_UNIQUE wrapped in 
  *   an autonomous transaction to avoid the commit side effect of the 
  *   the call.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     lock_name    <IN>  ==  (VARCHAR2) The "user" name to lock.
  *     lock_handle <OUT>  ==  (VARCHAR2) The system lock name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 09/21/10           Original coding.
  *E
  ***********************************************************************/
   IS

      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      DBMS_LOCK.ALLOCATE_UNIQUE (lock_name,lock_handle);

   END L_dbms_lock_allocate_unique;


   -- The following functions perform operations for layer locks stored in
   -- the SDE.LAYER_LOCKS table.  

   PROCEDURE L_delete_layer_lock (sde_id    IN  pinfo_util.sde_id_t,
                                  layer_id  IN  layer_id_t,
                                  autolock  IN  VARCHAR2)
  /***********************************************************************
  *
  *N  {L_delete_layer_lock}  --  Delete the specified layer lock
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a layer lock specified by owner (sde_id),
  *   layer (layer_id), and by whether it is an autolock or not.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (pinfo_util.sde_id_t) The owning session's id for
  *                          lock to delete.
  *     layer_id  <IN>  ==  (layer_id_t) The layer id for the lock to delete.
  *     autolock  <IN>  ==  (VARCHAR2) The autolock flag value for the lock
  *                          to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      DELETE FROM SDE.layer_locks
      WHERE  sde_id = L_delete_layer_lock.sde_id AND
             layer_id = L_delete_layer_lock.layer_id AND
             autolock = L_delete_layer_lock.autolock;
      IF SQL%NOTFOUND THEN
         raise_application_error (sde_util.SE_NO_LOCKS,
                                  'Lock <' || TO_CHAR (sde_id) || ',' || 
                                  TO_CHAR (layer_id) || ',' || autolock ||
                                  '> not found, not deleted.');
      END IF;

   END L_delete_layer_lock;

   
   PROCEDURE L_check_layer_lock_conflicts (layer_lock  IN  layer_lock_t)
  /***********************************************************************
  *
  *N  {L_check_layer_lock_conflicts}  --  Check a lock against existing locks
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a supplied layer lock against the LAYER_LOCKS
  *   table for conflicts; if one is found, a SE_LOCK_CONFLICT exception
  *   is raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer_lock  <IN>  ==  (layer_lock_t) The layer lock to check.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      CURSOR locks_cursor IS
        SELECT sde_id,lock_type,autolock,layer_id
        FROM   SDE.layer_locks
        WHERE  layer_id = layer_lock.layer_id AND
               (sde_id <> layer_lock.sde_id OR 
                autolock = layer_lock.autolock) AND
               (lock_type = SDE.lock_util.C_exclusive_lock OR 
                layer_lock.lock_type = SDE.lock_util.C_exclusive_lock) AND   
               ((maxx >= layer_lock.minx AND maxy >= layer_lock.miny AND
                 layer_lock.maxx >= minx AND layer_lock.maxy >= miny) OR
                 (minx IS NULL OR layer_lock.minx IS NULL));

      TYPE lock_table_t IS TABLE OF locks_cursor%ROWTYPE INDEX BY BINARY_INTEGER;

      found_lock          locks_cursor%ROWTYPE;
      lock_conflict       BOOLEAN;
      loop_done           BOOLEAN;
      lock_name           VARCHAR2(30);
      lock_handle         VARCHAR2(128);
      lock_status         INTEGER;
      invalid_lock_list   lock_table_t;
      invalid_lock_count  BINARY_INTEGER;

   BEGIN

      -- Find any conflicting locks.  The query we use is sensitive about
      -- whether we are trying to place an exclusive lock (in which case we
      -- have to consider all locks as possibly conflicting), or a shared lock
      -- (in which case we only have to worry about conflicting with exclusive
      -- locks).  In either case, the query will include a range expression so
      -- composed that a lock with NULL envelope variables will always match
      -- any other lock.  This is because a NULL envelope indicates a layer-
      -- wide lock.  With all of the about constraints in place, if any rows
      -- are returned, we probably have a conflict.  The last thing we have to
      -- check is if the connection owning the lock has somehow died without
      -- cleaning up.
      
      OPEN locks_cursor;
      lock_conflict := FALSE;
      loop_done := FALSE;
      invalid_lock_count := 0;

      WHILE NOT loop_done LOOP 
         FETCH locks_cursor INTO found_lock;
         IF locks_cursor%FOUND THEN

            -- We found a matching layer lock.  See if the owning connection's
            -- Oracle lock for it's connection id is still out there.  If not,
            -- then this lock is invalid.

            lock_name := SDE.pinfo_util.C_connection_lock_prefix || 
                         TO_CHAR (found_lock.sde_id);
            DBMS_LOCK.ALLOCATE_UNIQUE (lock_name,lock_handle);
            lock_status := DBMS_LOCK.REQUEST (lock_handle,
                                              DBMS_LOCK.X_MODE,
                                              0,
                                              TRUE);
            IF lock_status <> 0 THEN
               lock_conflict := TRUE;
               loop_done := TRUE;
               CLOSE locks_cursor;
            ELSE
               invalid_lock_count := invalid_lock_count + 1;
               invalid_lock_list (invalid_lock_count) := found_lock;
            END IF;
         ELSE
            loop_done := TRUE;
            CLOSE locks_cursor;
         END IF;
      END LOOP;

      -- Delete any invalid locks we may have found.

      FOR lock_entry IN 1 .. invalid_lock_count LOOP
         found_lock := invalid_lock_list (lock_entry);
         L_delete_layer_lock (found_lock.sde_id,
                              found_lock.layer_id,
                              found_lock.autolock);
      END LOOP;      

      -- If we found a lock conflict, raise an appropriate exception.
 
      IF lock_conflict THEN
         raise_application_error (sde_util.SE_LOCK_CONFLICT,
                                  'Attempt to place [' || 
                                  layer_lock.lock_type || layer_lock.autolock ||
                                  '] lock on layer ' || 
                                  TO_CHAR (layer_lock.layer_id) ||
                                  ' by server ' || TO_CHAR (layer_lock.sde_id)
                                  || ' conflicted with [' || 
                                  found_lock.lock_type  || found_lock.autolock
                                  || '] lock placed by server ' || 
                                  TO_CHAR (found_lock.sde_id) || '.');
      END IF;

   END L_check_layer_lock_conflicts;


   PROCEDURE add_layer_lock (layer_lock  IN  layer_lock_t)
  /***********************************************************************
  *
  *N  {add_layer_lock}  --  Add a lock to LAYER_LOCKS, checking for conflicts
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a new layer lock into the LAYER_LOCKS table,
  *   checking for conflicts, and deleting (even if there is a conflict)
  *   any existing non-auto lock on this layer already owned by this user.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer_lock  <IN>  ==  (layer_lock_t) The description of the lock
  *                            to add.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Start by locking the LAYER_LOCKS table to avoid undetected lock
      -- conflicts due to serialization issues.

      IF layer_lock.lock_type = SDE.lock_util.C_exclusive_lock THEN
         LOCK TABLE SDE.layer_locks IN EXCLUSIVE MODE;
      ELSE
         LOCK TABLE SDE.layer_locks IN ROW SHARE MODE;
      END IF;

      -- If this is not an autolock, delete any existing regular lock on this 
      -- layer owned by this user.  Commit it immeadiately if successfuly, since
      -- the lock is to be removed even if we subsequently encounter a lock
      -- conflict (this behavior is unique to layer locks).

      IF layer_lock.autolock <> C_is_autolock THEN
         BEGIN
            L_delete_layer_lock (layer_lock.sde_id,
                                 layer_lock.layer_id,
                                 layer_lock.autolock);
            COMMIT;
            IF layer_lock.lock_type = SDE.lock_util.C_exclusive_lock THEN
               LOCK TABLE SDE.layer_locks IN EXCLUSIVE MODE;
            ELSE
               LOCK TABLE SDE.layer_locks IN ROW SHARE MODE;
            END IF;
         EXCEPTION
            WHEN OTHERS THEN
              IF SQLCODE <> sde_util.SE_NO_LOCKS THEN
                 RAISE;
              END IF;
         END;
      END IF;
            
      -- Check for conflicts.
      
      L_check_layer_lock_conflicts (layer_lock);

      -- If there's no conflict, then we can insert our lock.
      
      INSERT INTO SDE.layer_locks
        (sde_id,layer_id,autolock,lock_type,minx,miny,maxx,maxy)
      VALUES
        (layer_lock.sde_id,layer_lock.layer_id,
        layer_lock.autolock,layer_lock.lock_type,layer_lock.minx,
        layer_lock.miny,layer_lock.maxx,layer_lock.maxy);

      -- If we've gotten this far without an exception, it's OK to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END add_layer_lock;
   
   
   PROCEDURE delete_layer_lock (sde_id    IN  pinfo_util.sde_id_t,
                                layer_id  IN  layer_id_t,
                                autolock  IN  VARCHAR2)   
  /***********************************************************************
  *
  *N  {delete_layer_lock}  --  Delete a lock from LAYER_LOCKS
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a layer lock from the LAYER_LOCKS table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (sde_id_t) The process id for the lock to delete.
  *     layer_id  <IN>  ==  (layer_id_t) The layer id for the lock to delete.
  *     autolock  <IN>  ==  (VARCHAR2) The autolock type for the lock to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      L_delete_layer_lock (sde_id,layer_id,autolock);
      COMMIT;

   END delete_layer_lock;


   PROCEDURE delete_layer_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_layer_locks_by_sde_id}  --  Delete all layer locks for a sde_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all layer locks identified by a particular
  *   sde_id form the LAYER_LOCKS table.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) The process id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      DELETE FROM SDE.layer_locks
      WHERE  sde_id = delete_layer_locks_by_sde_id.sde_id;
      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_layer_locks_by_sde_id;


   PROCEDURE update_layer_lock (layer_lock  IN  layer_lock_t)
  /***********************************************************************
  *
  *N  {update_layer_lock}  --  Update lock in LAYER_LOCKS, check conflict
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates an existing layer lock into the LAYER_LOCKS
  *   table, checking for conflicts.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer_lock  <IN>  ==  (layer_lock_t) The new description of the lock
  *                            to modify, identified by sde_id, layer id and
  *                            autolock, which do not change.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Start by locking the LAYER_LOCKS table to avoid undetected lock
      -- conflicts due to serialization issues.

      IF layer_lock.lock_type = SDE.lock_util.C_exclusive_lock THEN
         LOCK TABLE SDE.layer_locks IN EXCLUSIVE MODE;
      ELSE
         LOCK TABLE SDE.layer_locks IN ROW SHARE MODE;
      END IF;

      -- Delete the lock we are to update.  If it doesn't exist, this'll
      -- throw an exception, which is what we want.  If it does exist, this'll
      -- get it out of the way so we can test for conflicts.

      L_delete_layer_lock (layer_lock.sde_id,
                           layer_lock.layer_id,
                           layer_lock.autolock);
            
      -- Check for conflicts.
      
      L_check_layer_lock_conflicts (layer_lock);

      -- If there's no conflict, then we can insert our (updated) lock.
      
      INSERT INTO SDE.layer_locks
        (sde_id,layer_id,autolock,lock_type,minx,miny,maxx,maxy)
      VALUES
        (layer_lock.sde_id,layer_lock.layer_id,
         layer_lock.autolock,layer_lock.lock_type,layer_lock.minx,
         layer_lock.miny,layer_lock.maxx,layer_lock.maxy);

      -- If we've gotten this far without an exception, it's OK to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END update_layer_lock;
   
   
   -- The following functions perform operations for state locks stored in
   -- the SDE.STATE_LOCKS table.  

   PROCEDURE L_delete_state_lock (sde_id    IN  pinfo_util.sde_id_t,
                                  state_id  IN  state_id_t,
                                  autolock  IN  VARCHAR2,
                                  marked    IN  BOOLEAN)
  /***********************************************************************
  *
  *N  {L_delete_state_lock}  --  Delete the specified state lock
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a state lock specified by owner (sde_id),
  *   state (state_id), and by whether it is an autolock or not.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (pinfo_util.sde_id_t) The owning process's id for
  *                          lock to delete.
  *     state_id  <IN>  ==  (state_id_t) The state id for the lock to delete.
  *     autolock  <IN>  ==  (VARCHAR2) The autolock flag value for the lock
  *                          to delete.
  *     marked    <IN>  ==  (BOOLEAN) If TRUE, it is OK to delete marks as
  *                          well as other locks.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      IF marked THEN

         DELETE FROM SDE.state_locks
         WHERE  sde_id = L_delete_state_lock.sde_id AND
                state_id = L_delete_state_lock.state_id AND
                autolock = L_delete_state_lock.autolock;

      ELSE

         DELETE FROM SDE.state_locks
         WHERE  sde_id = L_delete_state_lock.sde_id AND
                state_id = L_delete_state_lock.state_id AND
                autolock = L_delete_state_lock.autolock AND
                lock_type <> SDE.lock_util.C_marked_lock;
      END IF;

      IF SQL%NOTFOUND THEN
         raise_application_error (sde_util.SE_NO_LOCKS,
                                  'Lock <' || TO_CHAR (sde_id) || ',' || 
                                  TO_CHAR (state_id) || ',' || autolock ||
                                  '> not found, not deleted.');
      END IF;

   END L_delete_state_lock;

   
   PROCEDURE L_check_state_lock_conflicts (state_lock  IN  state_lock_t)
  /***********************************************************************
  *
  *N  {L_check_state_lock_conflicts}  --  Check a lock against existing locks
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a supplied state lock against the STATE_LOCKS
  *   table for conflicts; if one is found, a SE_LOCK_CONFLICT exception
  *   is raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_lock  <IN>  ==  (state_lock_t) The state lock to check.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      CURSOR locks_cursor IS
        SELECT sde_id,lock_type,autolock,state_id
        FROM   SDE.state_locks
        WHERE ((state_id = state_lock.state_id AND
               (sde_id <> state_lock.sde_id OR 
                autolock = state_lock.autolock) AND
                (lock_type = SDE.lock_util.C_exclusive_lock OR 
                 state_lock.lock_type = SDE.lock_util.C_exclusive_lock)) OR
               (lock_type = SDE.lock_util.C_exclusive_lock_all OR
                state_lock.lock_type = SDE.lock_util.C_exclusive_lock_all)) AND
                lock_type <> SDE.lock_util.C_marked_lock;

      TYPE lock_table_t IS TABLE OF locks_cursor%ROWTYPE INDEX BY BINARY_INTEGER;

      found_lock          locks_cursor%ROWTYPE;
      lock_conflict       BOOLEAN;
      loop_done           BOOLEAN;
      lock_name           VARCHAR2(30);
      lock_handle         VARCHAR2(128);
      lock_status         INTEGER;
      invalid_lock_list   lock_table_t;
      invalid_lock_count  BINARY_INTEGER;

   BEGIN

      -- Find any conflicting locks.  The query we use is sensitive about
      -- whether we are trying to place an exclusive lock (in which case we
      -- have to consider all locks as possibly conflicting), or a shared lock
      -- (in which case we only have to worry about conflicting with exclusive
      -- locks).  With all of the about constraints in place, if any rows
      -- are returned, we probably have a conflict.  The last thing we have to
      -- check is if the connection owning the lock has somehow died without
      -- cleaning up.
      
      OPEN locks_cursor;
      lock_conflict := FALSE;
      loop_done := FALSE;
      invalid_lock_count := 0;

      WHILE NOT loop_done LOOP 
         FETCH locks_cursor INTO found_lock;
         IF locks_cursor%FOUND THEN

            -- We found a matching state lock.  See if the owning connection's
            -- Oracle lock for its connection id is still out there.  If not,
            -- then this lock is invalid.

            lock_name := SDE.pinfo_util.C_connection_lock_prefix || 
                         TO_CHAR (found_lock.sde_id);
            L_dbms_lock_allocate_unique (lock_name,lock_handle);
            lock_status := DBMS_LOCK.REQUEST (lock_handle,
                                              DBMS_LOCK.X_MODE,
                                              0,
                                              TRUE);
            IF lock_status <> 0 THEN
               lock_conflict := TRUE;
               loop_done := TRUE;
               CLOSE locks_cursor;
            ELSE
               invalid_lock_count := invalid_lock_count + 1;
               invalid_lock_list (invalid_lock_count) := found_lock;
            END IF;
         ELSE
            loop_done := TRUE;
            CLOSE locks_cursor;
         END IF;
      END LOOP;

      -- Delete any invalid locks we may have found.

      FOR lock_entry IN 1 .. invalid_lock_count LOOP
         found_lock := invalid_lock_list (lock_entry);
         L_delete_state_lock (found_lock.sde_id,
                              found_lock.state_id,
                              found_lock.autolock,
                              TRUE);
      END LOOP;      

      -- If we found a lock conflict, raise an appropriate exception.
 
      IF lock_conflict THEN
         raise_application_error (sde_util.SE_LOCK_CONFLICT,
                                  'Attempt to place [' || 
                                  state_lock.lock_type || state_lock.autolock ||
                                  '] lock on state ' || 
                                  TO_CHAR (state_lock.state_id) ||
                                  ' by server ' || TO_CHAR (state_lock.sde_id)
                                  || ' conflicted with [' || 
                                  found_lock.lock_type  || found_lock.autolock
                                  || '] lock placed by server ' || 
                                  TO_CHAR (found_lock.sde_id) || '.');
      END IF;

   END L_check_state_lock_conflicts;


   PROCEDURE add_state_lock (state_lock  IN  state_lock_t)
  /***********************************************************************
  *
  *N  {add_state_lock}  --  Add a lock to STATE_LOCKS, checking for conflicts
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a new state lock into the STATE_LOCKS table,
  *   checking for conflicts.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     state_lock  <IN>  ==  (state_lock_t) The description of the lock
  *                            to add.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Marks don't conflict and it doesn't hurt if they are duplicates,
      -- so skip all that for them.

      IF state_lock.lock_type <> SDE.lock_util.C_marked_lock THEN

         -- Start by locking the STATE_LOCKS table to avoid undetected lock
         -- conflicts due to serialization issues.  

         IF state_lock.lock_type = SDE.lock_util.C_exclusive_lock OR
            state_lock.lock_type = SDE.lock_util.C_exclusive_lock_all THEN
            LOCK TABLE SDE.state_locks IN EXCLUSIVE MODE;
         ELSE
            LOCK TABLE SDE.state_locks IN ROW SHARE MODE;
         END IF;

         -- Delete any existing lock on this state owned by this user.  
         -- This gets it out of the way during conflict checking (it will be
         -- restored via rollback if a conflict is detected).

         BEGIN
            L_delete_state_lock (state_lock.sde_id,
                                 state_lock.state_id,
                                 state_lock.autolock,
                                 FALSE);
         EXCEPTION
            WHEN OTHERS THEN
              IF SQLCODE <> sde_util.SE_NO_LOCKS THEN
                 RAISE;
              END IF;
         END;
            
         -- Check for conflicts.
        
         L_check_state_lock_conflicts (state_lock);

      END IF;

      -- If there's no conflict, then we can insert our lock.
      
      INSERT INTO SDE.state_locks
        (sde_id,state_id,autolock,lock_type)
      VALUES
        (state_lock.sde_id,state_lock.state_id,
        state_lock.autolock,state_lock.lock_type);

      -- If we've gotten this far without an exception, it's OK to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END add_state_lock;
   
   
   PROCEDURE delete_state_lock (sde_id    IN  pinfo_util.sde_id_t,
                                state_id  IN  state_id_t,
                                autolock  IN  VARCHAR2)   
  /***********************************************************************
  *
  *N  {delete_state_lock}  --  Delete a lock from STATE_LOCKS
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a state lock from the STATE_LOCKS table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (sde_id_t) The process id for the lock to delete.
  *     state_id  <IN>  ==  (state_id_t) The state id for the lock to delete.
  *     autolock  <IN>  ==  (VARCHAR2) The autolock type for the lock to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      L_delete_state_lock (sde_id,state_id,autolock,FALSE);
      COMMIT;

   END delete_state_lock;


   PROCEDURE delete_state_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_state_locks_by_sde_id}  --  Delete all state locks for a sde_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all state locks identified by a particular
  *   sde_id form the STATE_LOCKS table.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) The process id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      DELETE FROM SDE.state_locks
      WHERE  sde_id = delete_state_locks_by_sde_id.sde_id;
      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_state_locks_by_sde_id;


   -- The following functions perform operations for table locks stored in
   -- the SDE.TABLE_LOCKS table.  

   PROCEDURE L_delete_table_lock (sde_id    IN  pinfo_util.sde_id_t,
                                  table_id  IN  table_id_t)
  /***********************************************************************
  *
  *N  {L_delete_table_lock}  --  Delete the specified table lock
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a table lock specified by owner (sde_id),
  *   table (table_id), and by whether it is an autolock or not.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (pinfo_util.sde_id_t) The owning process's id for
  *                          lock to delete.
  *     table_id  <IN>  ==  (table_id_t) The table id for the lock to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      DELETE FROM SDE.table_locks
      WHERE  sde_id = L_delete_table_lock.sde_id AND
             registration_id = table_id;
      IF SQL%NOTFOUND THEN
         raise_application_error (sde_util.SE_NO_LOCKS,
                                  'Lock <' || TO_CHAR (sde_id) || ',' || 
                                  TO_CHAR (table_id) || 
                                  '> not found, not deleted.');
      END IF;

   END L_delete_table_lock;

   
   PROCEDURE L_check_table_lock_conflicts (table_lock  IN  table_lock_t)
  /***********************************************************************
  *
  *N  {L_check_table_lock_conflicts}  --  Check a lock against existing locks
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a supplied table lock against the TABLE_LOCKS
  *   table for conflicts; if one is found, a SE_LOCK_CONFLICT exception
  *   is raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     table_lock  <IN>  ==  (table_lock_t) The table lock to check.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      CURSOR locks_cursor IS
        SELECT sde_id,lock_type,registration_id
        FROM   SDE.table_locks
        WHERE  registration_id = table_lock.registration_id AND
               (lock_type = SDE.lock_util.C_exclusive_lock OR 
                table_lock.lock_type = SDE.lock_util.C_exclusive_lock);

      TYPE lock_table_t IS TABLE OF locks_cursor%ROWTYPE INDEX BY BINARY_INTEGER;

      found_lock          locks_cursor%ROWTYPE;
      lock_conflict       BOOLEAN;
      loop_done           BOOLEAN;
      lock_name           VARCHAR2(30);
      lock_handle         VARCHAR2(128);
      lock_status         INTEGER;
      invalid_lock_list   lock_table_t;
      invalid_lock_count  BINARY_INTEGER;

   BEGIN

      -- Find any conflicting locks.  The query we use is sensitive about
      -- whether we are trying to place an exclusive lock (in which case we
      -- have to consider all locks as possibly conflicting), or a shared lock
      -- (in which case we only have to worry about conflicting with exclusive
      -- locks).  With all of the about constraints in place, if any rows
      -- are returned, we have a probable conflict.  The last thing we have to
      -- check is if the connection owning the lock has somehow died without
      -- cleaning up.
      
      OPEN locks_cursor;
      lock_conflict := FALSE;
      loop_done := FALSE;
      invalid_lock_count := 0;

      WHILE NOT loop_done LOOP 
         FETCH locks_cursor INTO found_lock;
         IF locks_cursor%FOUND THEN

            -- We found a matching table lock.  See if the owning connection's
            -- Oracle lock for it's connection id is still out there.  If not,
            -- then this lock is invalid.

            lock_name := SDE.pinfo_util.C_connection_lock_prefix || 
                         TO_CHAR (found_lock.sde_id);
            L_dbms_lock_allocate_unique (lock_name,lock_handle);
            lock_status := DBMS_LOCK.REQUEST (lock_handle,
                                              DBMS_LOCK.X_MODE,
                                              0,
                                              TRUE);
            IF lock_status <> 0 THEN
               lock_conflict := TRUE;
               loop_done := TRUE;
               CLOSE locks_cursor;
            ELSE
               invalid_lock_count := invalid_lock_count + 1;
               invalid_lock_list (invalid_lock_count) := found_lock;
            END IF;
         ELSE
            loop_done := TRUE;
            CLOSE locks_cursor;
         END IF;
      END LOOP;

      -- Delete any invalid locks we may have found.

      FOR lock_entry IN 1 .. invalid_lock_count LOOP
         found_lock := invalid_lock_list (lock_entry);
         L_delete_table_lock (found_lock.sde_id,
                              found_lock.registration_id);
      END LOOP;      

      -- If we found a lock conflict, raise an appropriate exception.
 
      IF lock_conflict THEN
         raise_application_error (sde_util.SE_LOCK_CONFLICT,
                                  'Attempt to place [' || 
                                  table_lock.lock_type || '] lock on table ' || 
                                  TO_CHAR (table_lock.registration_id) ||
                                  ' by server ' || TO_CHAR (table_lock.sde_id)
                                  || ' conflicted with [' || 
                                  found_lock.lock_type  || 
                                  '] lock placed by server ' || 
                                  TO_CHAR (found_lock.sde_id) || '.');
      END IF;

   END L_check_table_lock_conflicts;


   PROCEDURE add_table_lock (table_lock  IN  table_lock_t)
  /***********************************************************************
  *
  *N  {add_table_lock}  --  Add a lock to TABLE_LOCKS, checking for conflicts
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a new table lock into the TABLE_LOCKS table,
  *   checking for conflicts.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     table_lock  <IN>  ==  (table_lock_t) The description of the lock
  *                            to add.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Start by locking the TABLE_LOCKS table to avoid undetected lock
      -- conflicts due to serialization issues.

      IF table_lock.lock_type = SDE.lock_util.C_exclusive_lock THEN
         LOCK TABLE SDE.table_locks IN EXCLUSIVE MODE;
      ELSE
         LOCK TABLE SDE.table_locks IN ROW SHARE MODE;
      END IF;

      -- Delete any existing lock on this table owned by this user.  
      -- This gets it out of the way during conflict checking (it will be
      -- restored via rollback if a conflict is detected).

      BEGIN
         L_delete_table_lock (table_lock.sde_id,
                              table_lock.registration_id);
      EXCEPTION
         WHEN OTHERS THEN
           IF SQLCODE <> sde_util.SE_NO_LOCKS THEN
              RAISE;
           END IF;
      END;
            
      -- Check for conflicts.
      
      L_check_table_lock_conflicts (table_lock);

      -- If there's no conflict, then we can insert our lock.
      
      INSERT INTO SDE.table_locks
        (sde_id,registration_id,lock_type)
      VALUES
        (table_lock.sde_id,table_lock.registration_id,table_lock.lock_type);

      -- If we've gotten this far without an exception, it's OK to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END add_table_lock;
   
   
   PROCEDURE delete_table_lock (sde_id    IN  pinfo_util.sde_id_t,
                                table_id  IN  table_id_t)   
  /***********************************************************************
  *
  *N  {delete_table_lock}  --  Delete a lock from TABLE_LOCKS
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a table lock from the TABLE_LOCKS table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id    <IN>  ==  (sde_id_t) The process id for the lock to delete.
  *     table_id  <IN>  ==  (table_id_t) The table id for the lock to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      L_delete_table_lock (sde_id,table_id);
      COMMIT;

   END delete_table_lock;


   PROCEDURE delete_table_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_table_locks_by_sde_id}  --  Delete all table locks for a sde_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all table locks identified by a particular
  *   sde_id form the TABLE_LOCKS table.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) The process id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      DELETE FROM SDE.table_locks
      WHERE  sde_id = delete_table_locks_by_sde_id.sde_id;
      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_table_locks_by_sde_id;


   -- The following functions perform operations for object locks stored in
   -- the SDE.OBJECT_LOCKS table.  

   PROCEDURE L_delete_object_lock (object_lock  IN  object_lock_t)
  /***********************************************************************
  *
  *N  {L_delete_object_lock}  --  Delete the specified object lock
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a object lock specified by owner (sde_id),
  *   object (object_id,object_type,application_id), and by whether it is
  *   an autolock or not.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     object_lock  <IN>  ==  (object_lock_t) The description of the object
  *                             to be deleted.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      DELETE FROM SDE.object_locks
      WHERE  sde_id = object_lock.sde_id AND
             object_id      = object_lock.object_id AND
             object_type    = object_lock.object_type AND
             application_id = object_lock.application_id AND
             autolock       = object_lock.autolock;
      IF SQL%NOTFOUND THEN
         raise_application_error (sde_util.SE_NO_LOCKS,
                                  'Lock <' || TO_CHAR (object_lock.sde_id) ||
                                  ',' || TO_CHAR (object_lock.object_id) ||
                                  ',' || TO_CHAR (object_lock.object_type) ||
                                  ',' || TO_CHAR (object_lock.application_id) 
                                  || ',' || object_lock.autolock || 
                                  '> not found, not deleted.');
      END IF;

   END L_delete_object_lock;

   
   PROCEDURE L_check_object_lock_conflicts (object_lock  IN  object_lock_t)
  /***********************************************************************
  *
  *N  {L_check_object_lock_conflicts}  --  Check a lock vs existing locks
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a supplied object lock against the OBJECT_LOCKS
  *   table for conflicts; if one is found, a SE_LOCK_CONFLICT exception
  *   is raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     object_lock  <IN>  ==  (object_lock_t) The object lock to check.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      CURSOR locks_cursor IS
        SELECT sde_id,lock_type,autolock,object_id,object_type,application_id
        FROM   SDE.object_locks
        WHERE  object_id = object_lock.object_id AND
               object_type = object_lock.object_type AND
               application_id = object_lock.application_id AND
               (sde_id <> object_lock.sde_id OR 
                autolock = object_lock.autolock) AND
               (lock_type = SDE.lock_util.C_exclusive_lock OR 
                object_lock.lock_type = SDE.lock_util.C_exclusive_lock);

      TYPE lock_table_t IS TABLE OF locks_cursor%ROWTYPE INDEX BY BINARY_INTEGER;

      found_lock          locks_cursor%ROWTYPE;
      lock_conflict       BOOLEAN;
      loop_done           BOOLEAN;
      lock_name           VARCHAR2(30);
      lock_handle         VARCHAR2(128);
      lock_status         INTEGER;
      delete_lock         object_lock_t;
      invalid_lock_list   lock_table_t;
      invalid_lock_count  BINARY_INTEGER;

   BEGIN

      -- Find any conflicting locks.  The query we use is sensitive about
      -- whether we are trying to place an exclusive lock (in which case we
      -- have to consider all locks as possibly conflicting), or a shared lock
      -- (in which case we only have to worry about conflicting with exclusive
      -- locks).  With all of the about constraints in place, if any rows
      -- are returned, we probably have a conflict.  The last thing we have to
      -- check is if the connection owning the lock has somehow died without
      -- cleaning up.
      
      OPEN locks_cursor;
      lock_conflict := FALSE;
      loop_done := FALSE;
      invalid_lock_count := 0;

      WHILE NOT loop_done LOOP 
         FETCH locks_cursor INTO found_lock;
         IF locks_cursor%FOUND THEN

            -- We found a matching object lock.  See if the owning connection's
            -- Oracle lock for it's connection id is still out there.  If not,
            -- then this lock is invalid.

            lock_name := SDE.pinfo_util.C_connection_lock_prefix || 
                         TO_CHAR (found_lock.sde_id);
            L_dbms_lock_allocate_unique (lock_name,lock_handle);
            lock_status := DBMS_LOCK.REQUEST (lock_handle,
                                              DBMS_LOCK.X_MODE,
                                              0,
                                              TRUE);
            IF lock_status <> 0 THEN
               lock_conflict := TRUE;
               loop_done := TRUE;
               CLOSE locks_cursor;
            ELSE
               invalid_lock_count := invalid_lock_count + 1;
               invalid_lock_list (invalid_lock_count) := found_lock;
            END IF;
         ELSE
            loop_done := TRUE;
            CLOSE locks_cursor;
         END IF;
      END LOOP;

      -- Delete any invalid locks we may have found.

      FOR lock_entry IN 1 .. invalid_lock_count LOOP
         found_lock := invalid_lock_list (lock_entry);
         delete_lock.sde_id := found_lock.sde_id;
         delete_lock.object_id := found_lock.object_id;
         delete_lock.object_type := found_lock.object_type;
         delete_lock.application_id := found_lock.application_id;
         delete_lock.autolock := found_lock.autolock;
         L_delete_object_lock (delete_lock);
      END LOOP;      

      -- If we found a lock conflict, raise an appropriate exception.
 
      IF lock_conflict THEN
         raise_application_error (sde_util.SE_LOCK_CONFLICT,
                                  'Attempt to place [' || 
                                  object_lock.lock_type || 
                                  object_lock.autolock ||
                                  '] lock on object (' || 
                                  TO_CHAR (object_lock.object_id) || ',' ||
                                  TO_CHAR (object_lock.object_type) || ',' ||
                                  TO_CHAR (object_lock.application_id) ||
                                  ') by server ' || TO_CHAR (object_lock.sde_id)
                                  || ' conflicted with [' || 
                                  found_lock.lock_type || found_lock.autolock ||
                                  '] lock placed by server ' || 
                                  TO_CHAR (found_lock.sde_id) || '.');
      END IF;

   END L_check_object_lock_conflicts;


   PROCEDURE add_object_lock (object_lock  IN  object_lock_t)
  /***********************************************************************
  *
  *N  {add_object_lock}  --  Add lock to OBJECT_LOCKS, checking for conflicts
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a new object lock into the OBJECT_LOCKS table,
  *   checking for conflicts.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     object_lock  <IN>  ==  (object_lock_t) The description of the lock
  *                            to add.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20049                SE_LOCK_CONFLICT
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

      -- Start by locking the OBJECT_LOCKS table to avoid undetected lock
      -- conflicts due to serialization issues.

      IF object_lock.lock_type = SDE.lock_util.C_exclusive_lock THEN
         LOCK TABLE SDE.object_locks IN EXCLUSIVE MODE;
      ELSE
         LOCK TABLE SDE.object_locks IN ROW SHARE MODE;
      END IF;

      -- Delete any existing lock on this object owned by this user.  
      -- This gets it out of the way during conflict checking (it will be
      -- restored via rollback if a conflict is detected).

      BEGIN
         L_delete_object_lock (object_lock);
      EXCEPTION
         WHEN OTHERS THEN
           IF SQLCODE <> sde_util.SE_NO_LOCKS THEN
              RAISE;
           END IF;
      END;
            
      -- Check for conflicts.
      
      L_check_object_lock_conflicts (object_lock);

      -- If there's no conflict, then we can insert our lock.
      
      INSERT INTO SDE.object_locks
        (sde_id,object_id,object_type,application_id,autolock,lock_type)
      VALUES
        (object_lock.sde_id,object_lock.object_id,
         object_lock.object_type,object_lock.application_id,
         object_lock.autolock,object_lock.lock_type);

      -- If we've gotten this far without an exception, it's OK to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END add_object_lock;
   
   
   PROCEDURE delete_object_lock (object_lock  IN  object_lock_t)
  /***********************************************************************
  *
  *N  {delete_object_lock}  --  Delete a lock from OBJECT_LOCKS
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a object lock from the OBJECT_LOCKS table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     object_lock  <IN>  ==  (object_lock_t) The description of the object
  *                             to be deleted.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20048                SE_NO_LOCKS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      L_delete_object_lock (object_lock);
      COMMIT;

   END delete_object_lock;


   PROCEDURE delete_object_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_object_locks_by_sde_id}  --  Delete all object locks for a sde_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all object locks identified by a particular
  *   sde_id form the OBJECT_LOCKS table.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) The process id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/26/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      DELETE FROM SDE.object_locks
      WHERE  sde_id = delete_object_locks_by_sde_id.sde_id;
      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_object_locks_by_sde_id;

   PROCEDURE delete_all_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_all_locks_by_sde_id}  --  Delete all locks for a sde_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all layer, table, state, object locks identified 
  *   by a particular sde_id form the LAYER_LOCKS, TABLE_LOCKS, STATE_LOCKS 
  *   and OBJECT_LOCKS tables.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) The process id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal                 10/08/04           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      DELETE FROM SDE.layer_locks
      WHERE  sde_id = delete_all_locks_by_sde_id.sde_id;


      DELETE FROM SDE.state_locks
      WHERE  sde_id = delete_all_locks_by_sde_id.sde_id;

      
      DELETE FROM SDE.table_locks
      WHERE  sde_id = delete_all_locks_by_sde_id.sde_id;


      DELETE FROM SDE.object_locks
      WHERE  sde_id = delete_all_locks_by_sde_id.sde_id;


      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_all_locks_by_sde_id;


   PROCEDURE delete_all_locks_by_pid (sde_id  IN  pinfo_util.sde_id_t)
  /***********************************************************************
  *
  *N  {delete_all_locks_by_pid}  --  Delete all locks for a pid
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all layer, table, state, object locks identified 
  *   by a server_id form the LAYER_LOCKS, TABLE_LOCKS, STATE_LOCKS 
  *   and OBJECT_LOCKS tables.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id  <IN>  ==  (sde_id_t) sde id to delete locks by.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal                 10/08/04           Original coding.
  *    Sanjay Magal                 10/15/08      Add logic for proxies.
  *    Sanjay Magal                 01/04/09      Add logic for proxies.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      DELETE FROM SDE.layer_locks
      WHERE  sde_id in (SELECT sde_id from SDE.process_information
      WHERE sde_id = delete_all_locks_by_pid.sde_id
      OR (proxy_yn = 'Y' AND parent_sde_id = delete_all_locks_by_pid.sde_id));

      DELETE FROM SDE.state_locks
      WHERE  sde_id in (SELECT sde_id from SDE.process_information
      WHERE sde_id = delete_all_locks_by_pid.sde_id
      OR (proxy_yn = 'Y' AND parent_sde_id = delete_all_locks_by_pid.sde_id));
      
      DELETE FROM SDE.table_locks
      WHERE  sde_id in (SELECT sde_id from SDE.process_information
      WHERE sde_id = delete_all_locks_by_pid.sde_id
      OR (proxy_yn = 'Y' AND parent_sde_id = delete_all_locks_by_pid.sde_id));

      DELETE FROM SDE.object_locks
      WHERE  sde_id in (SELECT sde_id from SDE.process_information
      WHERE sde_id = delete_all_locks_by_pid.sde_id
      OR (proxy_yn = 'Y' AND parent_sde_id = delete_all_locks_by_pid.sde_id));

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_all_locks_by_pid;

   PROCEDURE delete_all_orphaned_locks
  /***********************************************************************
  *
  *N  {delete_all_locks_by_sde_id}  --  Delete all locks w/o pinfo entry
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes all layer, table, state, object locks which
  *   lack a PROCESS_INFORMATION table row sharing their sde_id from the 
  *   LAYER_LOCKS, TABLE_LOCKS, STATE_LOCKS and OBJECT_LOCKS tables.     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 12/15/05           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      DELETE FROM SDE.layer_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);

      DELETE FROM SDE.state_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);
      
      DELETE FROM SDE.table_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);

      DELETE FROM SDE.object_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

   END delete_all_orphaned_locks;


END lock_util;

/


Prompt Grants on PACKAGE LOCK_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.LOCK_UTIL TO PUBLIC WITH GRANT OPTION
/
