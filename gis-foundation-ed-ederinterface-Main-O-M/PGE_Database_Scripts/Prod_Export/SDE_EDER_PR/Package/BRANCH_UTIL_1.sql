--------------------------------------------------------
--  DDL for Package Body BRANCH_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."BRANCH_UTIL" 
/***********************************************************************
*
*N  {branch_util.spb}  --  Implementation for BRANCHES table  
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to 
*   perform DML operations on the BRANCHES table. It should be compiled 
*   by the SDE DBA user; security is by user name.   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2015 ESRI
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
*    Kevin Watt             12/15/2015               Original coding.
*E
***********************************************************************/
IS

  -- Package globals

  G_current_user          SDE.sde_util.identifier_t;
  G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
  G_check_default         CONSTANT PLS_INTEGER := 1;
  G_nocheck_default       CONSTANT PLS_INTEGER := 0;

  CURSOR G_branch_exists_cursor (branch_name  IN  branch_name_t,
                                 branch_owner IN  branch_name_t)
  IS
     SELECT status
     FROM   SDE.branches 
     WHERE  name = branch_name AND
            owner = branch_owner;

  CURSOR G_branch_get_name_cursor (branch_id_i  IN  branch_id_t)
  IS
     SELECT owner, name
     FROM   SDE.branches
     WHERE  branch_id = branch_id_i;

  CURSOR G_mb_tables_get_cursor (regid_i  IN  mb_tables_regid_t)
  IS
     SELECT start_moment, behavior_map
     FROM   SDE.multibranch_tables 
     WHERE  registration_id = regid_i;


  -- Local Procedures and Functions

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

  PROCEDURE L_parse_branch_name (branch_name     IN  branch_name_t,
                                 parsed_name     OUT branch_name_t,
                                 parsed_owner    OUT branch_owner_t)
  /***********************************************************************
  *
  *N  {L_parse_branch_name}  --  Parse a branch name into name and owner
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure parses the supplied branch name into name and owner
  *   parts.  If the name is a simple name, then current user name is 
  *   returned as owner.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     i_branch_name    <IN>   ==  (version_name_t) Version to be parsed.
  *     o_parsed_name    <OUT>  ==  (SDE.versions.name%TYPE) The name part
  *                                  of i_branch_name.
  *     o_parsed_owner   <OUT>  ==  (SDE.versions.owner%TYPE) The owner 
  *                                  part of the i_branch_name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20591                SE_INVALID_BRANCH_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson              02/19/1999           Original coding.
  *    Kevin Watt                 12/18/2015           Lifted from
  *                                                    version_util and
  *                                                    revised for BRANCHES
  *E
  ***********************************************************************/
   IS

      dot_at   NUMBER NOT NULL DEFAULT 0;

   BEGIN

      dot_at := L_delimited_find (branch_name,N'.');
      IF dot_at <> 0 THEN
         parsed_owner := SUBSTR (branch_name,1,dot_at - 1);
         IF SUBSTR (parsed_owner,1,1) <> N'"' THEN
           parsed_owner := UPPER (SUBSTR (branch_name,1,dot_at - 1));
         END IF;
         parsed_name := SUBSTR (branch_name,dot_at + 1);
      ELSE
         parsed_name := branch_name;
         parsed_owner := G_current_user;
      END IF;
      IF RTRIM (parsed_name,N' ') IS NULL OR
         RTRIM (parsed_owner,N' ') IS NULL THEN
         RAISE VALUE_ERROR;
      END IF;

   EXCEPTION

      WHEN OTHERS THEN
         raise_application_error (SDE.sde_util.SE_INVALID_BRANCH_NAME,
                                  '"' || NVL (branch_name,'(null)') || 
                                  '" is not a valid branch name.');

   END L_parse_branch_name;


  PROCEDURE L_branch_user_can_modify (i_branch_name  IN branch_name_t) 
  /***********************************************************************
  *
  *N  {L_branch_user_can_modify}  --  Can current user modify branch name?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the branch specified by name exists and is
  *   not the default branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_name   <IN>  ==  (branch_name_t) Branch to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20587                SE_BRANCH_NOEXIST
  *     -20591                SE_INVALID_BRANCH_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson              02/19/1999           Original coding.
  *    Kevin Watt                 12/18/2015           Lifted from
  *                                                    version_util and
  *                                                    revised for BRANCHES
  *               .
  *E
  ***********************************************************************/
   IS

      branch_exists            G_branch_exists_cursor%ROWTYPE;
      parsed_name              branch_name_t;
      parsed_owner             branch_owner_t;

   BEGIN

      -- Parse the branch name.

      L_parse_branch_name (i_branch_name,parsed_name,parsed_owner);

      -- Make sure this is not the default branch.

      IF parsed_owner = SDE.sde_util.C_sde_dba AND
         parsed_name = C_default_branch THEN
         raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                  'The default branch may not be deleted ' ||
                                  'or renamed.');
      END IF;

      -- Make sure that the branch exists.

      OPEN G_branch_exists_cursor (parsed_name,parsed_owner);
      FETCH G_branch_exists_cursor INTO branch_exists;
      IF G_branch_exists_cursor%NOTFOUND THEN
         CLOSE G_branch_exists_cursor;
         raise_application_error (SDE.sde_util.SE_BRANCH_NOEXIST,
                                  'Branch ' || i_branch_name || ' not found.');
      END IF;

      -- Make sure the name only matches one branch.

      FETCH G_branch_exists_cursor INTO branch_exists;
      IF G_branch_exists_cursor%ROWCOUNT > 1 THEN
         CLOSE G_branch_exists_cursor;
         raise_application_error (SDE.sde_util.SE_INVALID_BRANCH_NAME,
                                  'Name ' || i_branch_name || ' matches more than 1 branch.');
      END IF;

      CLOSE G_branch_exists_cursor;
    
   END L_branch_user_can_modify;

  PROCEDURE L_branch_user_can_modify_by_id (i_branch_id     IN branch_id_t,
                                            i_check_default IN PLS_INTEGER) 
  /***********************************************************************
  *
  *N  {L_branch_user_can_modify_by_id}  --  Can current user modify branch name?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the branch specified by id exists and is
  *   not the default branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     i_branch_id     <IN>  ==  (branch_id_t) Branch to be tested.
  *     i_check_default <IN>  ==  (PLS_INTEGER) If non-zero, check if default
  *                                branch, and if so, fail.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20587                SE_BRANCH_NOEXIST
  *     -20591                SE_INVALID_BRANCH_NAME
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson              08/27/2016           Original coding.
  *               .
  *E
  ***********************************************************************/
   IS

      branch_name              branch_name_t;
      branch_owner             branch_owner_t;

   BEGIN

      -- Get the branch name and and owner.

      OPEN G_branch_get_name_cursor (i_branch_id);
      FETCH G_branch_get_name_cursor INTO branch_owner, branch_name;
      IF G_branch_get_name_cursor%NOTFOUND THEN
         CLOSE G_branch_get_name_cursor;
         raise_application_error (SDE.sde_util.SE_BRANCH_NOEXIST,
                                  'Branch ' || i_branch_id || ' not found.');
      END IF;
      CLOSE G_branch_get_name_cursor;
      
      -- Make sure this is not the default branch.

      IF (i_check_default <> 0) THEN
         IF branch_owner = SDE.sde_util.C_sde_dba AND
            branch_name = C_default_branch THEN
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     'The default branch may not be deleted ' ||
                                     'or renamed.');
         END IF;
      ELSE
         IF branch_owner = SDE.sde_util.C_sde_dba AND
            branch_name = C_default_branch AND
            NOT G_sde_dba THEN
            raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                     'The default branch may only have its ' ||
                                     'status changed by the SDE DBA user.');
         END IF;
      END IF;

   END L_branch_user_can_modify_by_id;

-- Session Global Procedures 

  PROCEDURE get_branch_session       (branch_id             OUT  INTEGER,
                                      edit_moment           OUT  TIMESTAMP,
                                      ancestor_moment       OUT  TIMESTAMP,
                                      username              OUT  NVARCHAR2)
  /***********************************************************************
  *
  *N  {get_branch_session}  --  Get branch session globals 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure gets the branch session globals.
  *  Branch session globals are used during editing by the edit views and
  *  triggers. 
  *
  *  edit_moment - The moment set to the gdb_from_date
  * 
  *  ancestor_moment - Used by Branch editing to return rows from 
  *                    DEFAULT as the base moment of the 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id         <OUT>  == (INTEGER)  G_branch_id
  *     edit_moment       <OUT> ==  (TIMESTAMP)  G_edit_moment
  *     ancestor_moment   <OUT> ==  (TIMESTAMP)  G_branch_moment
  *     username          <OUT> ==  (NVARCHAR2)  G_username
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    branch_id := G_branch_id;
    edit_moment := G_edit_moment;
    ancestor_moment := G_ancestor_moment;
    username := G_username;

  END get_branch_session;

  PROCEDURE set_branch_session       (branch_id             IN      INTEGER,
                                      edit_moment           IN OUT  TIMESTAMP,
                                      ancestor_moment       IN      TIMESTAMP,
                                      username              IN      NVARCHAR2)
  /***********************************************************************
  *
  *N  {set_branch_session}  --  Set branch session globals 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure sets the branch session globals.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id         <IN>  == (INTEGER)  G_branch_id
  *     edit_moment       <IN> ==  (TIMESTAMP)  G_edit_moment
  *     ancestor_moment   <IN> ==  (TIMESTAMP)  G_ancestor_moment
  *     username          <IN> ==  (NVARCHAR2)  G_username
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    IF edit_moment = to_timestamp('9999.12.31 23:59:59','yyyy.mm.dd HH24:MI:SS') THEN
      G_edit_moment := sys_extract_utc(systimestamp);
      edit_moment := G_edit_moment;
    ELSE
      G_edit_moment := edit_moment;
    END IF;

    G_branch_id := branch_id;
    G_edit_moment := edit_moment;
    G_ancestor_moment := G_ancestor_moment;
    G_username := username;

  END set_branch_session;

   --BRANCHES Procedures and Functions

  PROCEDURE insert_branch             (branch              IN   branch_record_t)
  /***********************************************************************
  *
  *N  {insert_branch}  --  Insert a branch entry into the BRANCHES table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts an entry into the SDE.BRANCHES table.
  *
  *  Default values are used for BRANCH_ID, CREATION_TIME, BRANCH_MOMENT
  *  and ANCESTOR_MOMENT. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *       branch             <IN>  ==  (branch_record_t) Input BRANCH
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

    l_branch_name       NVARCHAR2(65);
    l_owner             SDE.sde_util.identifier_t;

BEGIN

    l_branch_name := branch.name;
    l_owner := branch.owner;

    BEGIN
       INSERT INTO SDE.branches
                        (name,
                         owner,
                         description,
                         status,
                         service_name,
                         branch_guid)
               VALUES   (l_branch_name,
                         l_owner,
                         branch.description,
                         branch.status,
                         branch.service_name,
                         branch.branch_guid);
       EXCEPTION
          WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error (SDE.sde_util.SE_BRANCH_EXISTS,
                                     'A branch named ' || branch.owner ||
                                     '.'||  branch.name ||
                                     ' already exists for service ' ||
                                     branch.service_name || '.');
       END;

    COMMIT;

  END insert_branch;

  PROCEDURE delete_branch             (branch_id_i         IN   INTEGER)
  /***********************************************************************
  *
  *N  {delete_branch}  --  Delete a branch entry from the BRANCHES table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an entry from the SDE.BRANCHES table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i             <IN>  ==  (INTEGER) Input BRANCH_ID
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    -- Make sure the current user has permission to do this.
    L_branch_user_can_modify_by_id (branch_id_i,G_check_default);

    DELETE FROM SDE.branches 
    WHERE branch_id = branch_id_i;

    COMMIT;

  END delete_branch;

  PROCEDURE update_branch_moment     (branch_id_i          IN   INTEGER,
                                      branch_moment_i      IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {update_branch_moment}  --  Update BRANCHES branch_moment.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the branch_moment. The transaction
  *  is controlled by the client. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     branch_moment_i       <IN>  ==  (TIMESTAMP) new branch_moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    IF (branch_id_i = C_default_branch_id) THEN
       raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                'This field of the Default Branch' ||
                                ' may not be updated.');
    END IF;

    UPDATE SDE.branches SET branch_moment = branch_moment_i
    WHERE branch_id = branch_id_i;

  END update_branch_moment;

  PROCEDURE update_ancestor_moment    (branch_id_i         IN   INTEGER,
                                       ancestor_moment_i   IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {update_ancestor_moment}  --  Update BRANCHES ancestor_moment.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the ancestor_moment. The previous value is
  *    copied to the previous_ancestor_moment column The transaction
  *  is controlled by the client. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     ancestor_moment_i     <IN>  ==  (TIMESTAMP) new ancestor_moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    IF (branch_id_i = C_default_branch_id) THEN
       raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                'This field of the Default Branch' ||
                                ' may not be updated.');
    END IF;

    UPDATE SDE.branches
    SET ancestor_moment = ancestor_moment_i,
        previous_ancestor_moment = ancestor_moment
    WHERE branch_id = branch_id_i;

  END update_ancestor_moment;
  
  PROCEDURE update_reconcile_moment   (branch_id_i         IN   INTEGER,
                                       reconcile_moment_i  IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {update_reconcile_moment}  --  Update BRANCHES last_reconcile_moment.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the last_reconcile_moment. The transaction
  *  is controlled by the client.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     reconcile_moment_i    <IN>  ==  (TIMESTAMP) new reconcile_moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               03/29/2017           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    IF (branch_id_i = C_default_branch_id) THEN
       raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                'This field of the Default Branch' ||
                                ' may not be updated.');
    END IF;

    UPDATE SDE.branches SET last_reconcile_moment = reconcile_moment_i
    WHERE branch_id = branch_id_i;

  END update_reconcile_moment;

  PROCEDURE update_status             (branch_id_i         IN   INTEGER,
                                       status_i            IN   INTEGER)
  /***********************************************************************
  *
  *N  {update_status}  --  Update BRANCHES status.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the status. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     status_i              <IN>  ==  (INTEGER) new status
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    -- Make sure the current user has permission to do this.
    L_branch_user_can_modify_by_id (branch_id_i,G_nocheck_default);

    -- Don't allow the default branch to be set to private or hidden.

    if (branch_id_i = C_default_branch_id AND
        (status_i < 1 OR status_i > 3)) THEN
        raise_application_error (SDE.sde_util.SE_OPERATION_NOT_ALLOWED,
                                 'The default branch may not be made ' ||
                                 'private or hidden.');
    END IF;

    UPDATE SDE.branches SET status = status_i
    WHERE branch_id = branch_id_i;

    COMMIT;

  END update_status;

  PROCEDURE update_description        (branch_id_i         IN   INTEGER,
                                       description_i       IN   NVARCHAR2)
  /***********************************************************************
  *
  *N  {update_description}  --  Update BRANCHES description.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the description. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     description_i         <IN>  ==  (NVARCHAR2) new description
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    -- Make sure the current user has permission to do this.
    L_branch_user_can_modify_by_id (branch_id_i,G_check_default);

    UPDATE SDE.branches SET description = description_i
    WHERE branch_id = branch_id_i;

    COMMIT;

  END update_description;
 
  PROCEDURE update_validation_moment (branch_id_i          IN   INTEGER,
                                      validation_moment_i  IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {update_validation_moment}  --  Update BRANCHES validation_moment.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the validation_moment. The transaction
  *  is controlled by the client. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     validation_moment_i   <IN>  ==  (TIMESTAMP) new branch_moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    IF (branch_id_i = C_default_branch_id) THEN
       raise_application_error (SDE.sde_util.SE_NO_PERMISSIONS,
                                'This field of the Default Branch' ||
                                ' may not be updated.');
    END IF;

    UPDATE SDE.branches SET validation_moment = validation_moment_i
    WHERE branch_id = branch_id_i;

    COMMIT;

  END update_validation_moment;

  PROCEDURE update_owner        (branch_id_i         IN   INTEGER,
                                 new_owner_i         IN   NVARCHAR2)
  /***********************************************************************
  *
  *N  {update_owner}  --  Update BRANCHES owner.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the owner, changing the branch's ownership. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_ID
  *     new_owner_i           <IN>  ==  (NVARCHAR2) new owner
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               08/02/2017           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    -- Make sure the current user has permission to do this.
    L_branch_user_can_modify_by_id (branch_id_i,G_check_default);

    BEGIN
      UPDATE SDE.branches SET owner = new_owner_i
      WHERE branch_id = branch_id_i;
    EXCEPTION
       WHEN DUP_VAL_ON_INDEX THEN
          raise_application_error (SDE.sde_util.SE_BRANCH_EXISTS,
                                   'A branch of this name ' ||
                                   ' already exists for ' || new_owner_i);
    END;

    COMMIT;

  END update_owner;

  PROCEDURE rename_branch             (owner_i             IN  branch_owner_t,
                                       old_name            IN  branch_name_t,
                                       new_name            IN  branch_name_t)
  /***********************************************************************
  *
  *N  {rename_branch}  --  Rename branch from the BRANCHES table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure renames the existing branch to the new name branch.
  *   This function has been deprecated at Pro 2.2 as it does not take
  *   service name into account when choosing the row to update, potentially
  *   resulting in multiple branch entries being renamed in different
  *   services.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     owner_i               <IN>  ==  (branch_owner_t) current user
  *     old_name              <IN>  ==  (branch_name_t) old branch name
  *     new_name              <IN>  ==  (branch_name_t) new branch name
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20587                SE_BRANCH_NOEXIST
  *     -20591                SE_INVALID_BRANCH_NAME
  *     -20586                SE_BRANCH_EXISTS
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *H  History:
  *
  *    Peter Aronson              12/18/1998           Original coding.
  *    Kevin Watt                 12/18/2015           Lifted from
  *                                                    version_util and
  *                                                    revised for BRANCHES
  *E
  ***********************************************************************/
   IS

      test_name                branch_name_t;
      parsed_name              branch_name_t;
      parsed_owner             branch_owner_t;

   BEGIN

      -- Make sure we have modify privileges on this version.

      L_parse_branch_name (old_name,parsed_name,parsed_owner);
      test_name := owner_i || N'.' || parsed_name;
      L_branch_user_can_modify (test_name);

      -- Perform the update of the BRANCHES row; use a block to catch any
      -- violation of the unique constraint on branch name.

      BEGIN
         UPDATE SDE.branches
         SET    name = new_name
         WHERE  name = old_name AND
                owner = rename_branch.owner_i;
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX THEN
            raise_application_error (SDE.sde_util.SE_BRANCH_EXISTS,
                                     'A branch named ' || new_name ||
                                     ' already exists.');
      END;

      COMMIT;

   END rename_branch;

  PROCEDURE update_name         (branch_id_i         IN   INTEGER,
                                 new_name_i          IN   branch_name_t)
  /***********************************************************************
  *
  *N  {update_name}  --  Update BRANCHES name.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the branch's name. This function replaces
  *   rename branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i           <IN>  ==  (INTEGER) Input BRANCH_IDb
  *     new_name_i            <IN>  ==  (branch_name_t) new name
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20587                SE_BRANCH_NOEXIST
  *     -20586                SE_BRANCH_EXISTS
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               05/04/2018           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    -- Make sure the current user has permission to do this.

    L_branch_user_can_modify_by_id (branch_id_i,G_check_default);

    BEGIN
      UPDATE SDE.branches SET name = new_name_i
      WHERE branch_id = branch_id_i;
    EXCEPTION
       WHEN DUP_VAL_ON_INDEX THEN
          raise_application_error (SDE.sde_util.SE_BRANCH_EXISTS,
                                   'A branch named ' || new_name_i ||
                                   ' already exists.');
    END;

    COMMIT;

  END update_name;

  -- Functions

  FUNCTION get_branch_id
/***********************************************************************
  *
  *N  {get_branch_id}  --  Returns session G_branch_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the session G_branch_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          12/23/2015           Original coding.
  *E
  ***********************************************************************/
  RETURN INTEGER IS
  BEGIN

    return G_branch_id;

  End get_branch_id;

  FUNCTION get_ancestor_moment
/***********************************************************************
  *
  *N  {get_ancestor_moment}  --  Returns session G_ancestor_moment
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the session G_ancestor_moment
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          12/23/2015           Original coding.
  *E
  ***********************************************************************/
  RETURN TIMESTAMP IS
  BEGIN

    return G_ancestor_moment;

  End get_ancestor_moment;

  FUNCTION get_edit_moment
/***********************************************************************
  *
  *N  {get_edit_moment}  --  Returns session G_edit_moment
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the session G_edit_moment
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt          12/23/2015           Original coding.
  *E
  ***********************************************************************/
  RETURN TIMESTAMP IS
  BEGIN

    return G_edit_moment;

  End get_edit_moment;

  FUNCTION get_database_time_in_utc
  /***********************************************************************
  *
  *N  {get_database_time_in_utc}  -- Returns database time in UTC
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the database time in UTC.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal          02/2018           Original coding.
  *E
  ***********************************************************************/
  RETURN DATE IS
    l_utc_time    DATE; 
  BEGIN 

  SELECT TO_DATE (
           TO_CHAR (SYS_EXTRACT_UTC (CURRENT_TIMESTAMP),
                    'DD-MON-YYYY HH24:MI:SS'),
           'DD-MON-YYYY HH24:MI:SS') INTO l_utc_time
  FROM DUAL;

  return l_utc_time;

  End get_database_time_in_utc;
  
  FUNCTION get_utc_time_string
  /***********************************************************************
  *
  *N  {get_utc_time_string}  -- Returns database time in UTC
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the database time in UTC, including
  *   milliseconds as a string in the format 'YYYY-MM-DD HH24:MI:SS.FF3'.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal          02/2018           Original coding.
  *E
  ***********************************************************************/
  RETURN VARCHAR2 IS
    l_utc_time    VARCHAR2(32); 
  BEGIN 

  SELECT TO_CHAR (SYS_EXTRACT_UTC (SYSTIMESTAMP), 
                  'YYYY-MM-DD HH24:MI:SS.FF3') INTO l_utc_time
  FROM DUAL;

  return l_utc_time;

  End get_utc_time_string;

  FUNCTION branch_get_last_edit_moment (branch_id_i   IN   branch_id_t)
/***********************************************************************
  *
  *N  {branch_get_last_edit_moment}  --  Returns last edit moment
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function queries the branch tables modified table to find
  *   the most recent edit moment for the specified branch.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal          03/2018           Original coding.
  *E
  ***********************************************************************/
  Return TIMESTAMP IS
    l_edit_moment    TIMESTAMP; 
  BEGIN

    SELECT MAX (edit_moment) INTO l_edit_moment
 FROM SDE.branch_tables_modified 
 WHERE branch_id = branch_id_i;
    
 return l_edit_moment;

  End branch_get_last_edit_moment;

  PROCEDURE insert_branch_tables_mod    (branch_id_i        IN   branch_id_t,
                                         reg_id_i           IN   INTEGER,
                                         edit_moment_i      IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {insert_branch_tables_mod}  --  INSERT into BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts into the BRANCH_TABLES_MODIFIED table. 
  * The transaction is controlled by the client. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i     <IN>  ==  (branch_is_t) Branch ID
  *     reg_id_i        <IN>  ==  (INTEGER) registration ID
  *     edit_moment_i   <IN>  ==  (TIMESTAMP) edit moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    INSERT INTO SDE.branch_tables_modified (branch_id,
                                            registration_id,
                                            edit_moment)
                                    VALUES (branch_id_i,
                                            reg_id_i,
                                            edit_moment_i);
  EXCEPTION 
    WHEN DUP_VAL_ON_INDEX THEN
      NULL;

  END insert_branch_tables_mod;

  PROCEDURE delete_branch_tables_mod (branch_id_i     IN   branch_id_t,
                                      delete_moment_i IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {delete_branch_tables_mod}  --  Delete entries past a point in time
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *    This procedure deletes all entries in the BRANCH_TABLES_MODIFIED
  *  table created after the specified moment.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i     <IN>  ==  (branch_is_t) Branch ID
  *     delete_moment_i <IN>  ==  (TIMESTAMP) delete moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               03/10/2016           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE  branch_id = branch_id_i AND
           edit_moment > delete_moment_i;

  END delete_branch_tables_mod;

  PROCEDURE reconcile_branch_tables_mod (branch_id_i        IN   branch_id_t,
                                         reconcile_moment_i IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {reconcile_branch_tables_mod}  --  Reconcile step for BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure winnows and updates the BRANCH_TABLES_MODIFIED
  *   table for reconcile, removing redundent entries and updating the
  *   surviver's edit moment to the reconcile moment for the specified
  *   branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i        <IN>  ==  (branch_is_t) Branch ID
  *     reconcile_moment_i <IN>  ==  (TIMESTAMP) reconcile moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               02/16/2016           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE  rowid NOT IN
        (SELECT rowid
         FROM   
           (SELECT rowid,ROW_NUMBER() OVER (PARTITION BY registration_id 
                   ORDER BY edit_moment DESC) rn
            FROM   SDE.branch_tables_modified
            WHERE  branch_id = branch_id_i)
         WHERE  rn = 1)
     AND branch_id = branch_id_i;

     UPDATE SDE.branch_tables_modified SET edit_moment = reconcile_moment_i
     WHERE  branch_id = branch_id_i;

  END reconcile_branch_tables_mod;

  PROCEDURE reconcile_branch_table_mod (branch_id_i        IN   branch_id_t,
                                        reg_id_i           IN   INTEGER,
                                        reconcile_moment_i IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {reconcile_branch_table_mod}  --  Reconcile step for BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure winnows and updates the BRANCH_TABLES_MODIFIED
  *   table for reconcile, removing redundent entries and updating the
  *   surviver's edit moment to the reconcile moment for the specified
  *   branch and table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i        <IN>  ==  (branch_is_t) Branch ID
  *     reg_id_i           <IN>  ==  (INTEGER) Table registration ID
  *     reconcile_moment_i <IN>  ==  (TIMESTAMP) reconcile moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               02/23/2017           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE  rowid NOT IN
        (SELECT rowid
         FROM   
           (SELECT rowid,ROW_NUMBER() OVER (PARTITION BY registration_id 
                   ORDER BY edit_moment DESC) rn
            FROM   SDE.branch_tables_modified
            WHERE  branch_id = branch_id_i AND registration_id = reg_id_i)
         WHERE  rn = 1)
     AND branch_id = branch_id_i AND registration_id = reg_id_i;

     UPDATE SDE.branch_tables_modified SET edit_moment = reconcile_moment_i
     WHERE  branch_id = branch_id_i AND registration_id = reg_id_i;

  END reconcile_branch_table_mod;

  PROCEDURE trim_branch_tables_mod (branch_id_i   IN   branch_id_t,
                                    branch_moment_i IN TIMESTAMP,
                                    trim_moment_i   IN TIMESTAMP)
  /***********************************************************************
  *
  *N  {trim_branch_tables_mod}  --  Trim step for BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure winnows and updates the BRANCH_TABLES_MODIFIED
  *   table for trim, removing redundent entries and updating the
  *   surviver's edit moment to the trim moment for the those entries
  *   with an edit moment > than the branch moment and in the specified
  *   branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i     <IN>  ==  (branch_is_t) Branch ID
  *     branch_moment_i <IN>  ==  (TIMESTAMP) trim moment
  *     trim_moment_i   <IN>  ==  (TIMESTAMP) branch moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               03/10/2016           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE  rowid NOT IN
        (SELECT rowid
         FROM   
           (SELECT rowid,ROW_NUMBER() OVER (PARTITION BY registration_id 
                   ORDER BY edit_moment DESC) rn
            FROM   SDE.branch_tables_modified
            WHERE  branch_id = branch_id_i)
         WHERE  rn = 1)
     AND branch_id = branch_id_i;

     UPDATE SDE.branch_tables_modified SET edit_moment = trim_moment_i
     WHERE  branch_id = branch_id_i and edit_moment > branch_moment_i;

  END trim_branch_tables_mod;

  PROCEDURE post_branch_tables_mod (branch_id_i   IN   branch_id_t,
                                    post_moment_i IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {post_branch_tables_mod}  --  Post step for BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the BRANCH_TABLES_MODIFIED table for Post,
  *   updating the posted branch's entries to move them to the Default
  *   branch and updating the edit moment to the post moment.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i   <IN>  ==  (branch_is_t) Branch ID
  *     post_moment_i <IN>  ==  (TIMESTAMP) Post moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               05/05/2016           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    UPDATE SDE.branch_tables_modified
    SET    branch_id = 0,
           edit_moment = post_moment_i
    WHERE  branch_id = branch_id_i;

  END post_branch_tables_mod;

  PROCEDURE post_branch_table_mod (branch_id_i   IN   branch_id_t,
                                   reg_id_i      IN   INTEGER,
                                   post_moment_i IN   TIMESTAMP)
  /***********************************************************************
  *
  *N  {post_branch_table_mod}  --  Post step for BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the BRANCH_TABLES_MODIFIED table for Post,
  *   updating the posted branch _ table entries to move them to the Default
  *   branch and updating the edit moment to the post moment.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i   <IN>  ==  (branch_is_t) Branch ID
  *     reg_id_i      <IN>  ==  (INTEGER) Table reg id
  *     post_moment_i <IN>  ==  (TIMESTAMP) Post moment
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson               02/22/2017           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    UPDATE SDE.branch_tables_modified
    SET    branch_id = 0,
           edit_moment = post_moment_i
    WHERE  branch_id = branch_id_i AND registration_id = reg_id_i;

  END post_branch_table_mod;

PROCEDURE delete_branch_tables_mod_by_id  (reg_id_i           IN   INTEGER)
  /***********************************************************************
  *
  *N  {delete_branch_tables_mod_by_id}  --  Delete by registration ID from
  *                                         BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes from the BRANCH_TABLES_MODIFIED table by 
  *  registration ID. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     reg_id_i   <IN>  ==  (INTEGER) Registration ID
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt               08/11/2016           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE registration_id = reg_id_i;

  END delete_branch_tables_mod_by_id;

PROCEDURE delete_branch_tables_mod_purge  (branch_id_i   IN   branch_id_t,
                                           reg_id_i      IN   INTEGER)
  /***********************************************************************
  *
  *N  {delete_branch_tables_mod_purge}  --  Delete by registration
  *                                         ID and branch from
  *                                         BRANCH_TABLES_MODIFIED.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes from the BRANCH_TABLES_MODIFIED table by 
  *  registration ID and branch.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     branch_id_i   <IN>  ==  (branch_is_t) Branch ID
  *     reg_id_i      <IN>  ==  (INTEGER) Registration ID
  *
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson            02/22/2017           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.branch_tables_modified
    WHERE registration_id = reg_id_i AND branch_id = branch_id_i;

  END delete_branch_tables_mod_purge;

  PROCEDURE insert_mb_tables            (mbt      IN   mb_tables_record_t)
  /***********************************************************************
  *
  *N  {insert_mb_tables}  --  INSERT into MULTIBRANCH_TABLES.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts into the MULTIBRANCH_TABLES table. 
  *  The current UTC moment is set to the start_moment at insert. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     mbt             <IN>  ==  (mb_tables_record_t) multibranch_tables
  *                                                    record
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    INSERT INTO SDE.multibranch_tables (registration_id, 
                                        behavior_map)
                                VALUES (mbt.registration_id, 
                                        mbt.behavior_map);

  END insert_mb_tables;

  PROCEDURE delete_mb_tables           (regid_i    IN   mb_tables_regid_t)
  /***********************************************************************
  *
  *N  {delete_mb_tables}  --  DELETE row from  MULTIBRANCH_TABLES.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a row from the MULTIBRANCH_TABLES table
  *  by registration_id.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     regid_i         <IN>  ==  (mb_tables_regid_t)  registration_id
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    DELETE FROM SDE.multibranch_tables 
    WHERE registration_id = regid_i;

  END delete_mb_tables;

  PROCEDURE update_mb_tables_behavior_map (regid_i    IN   mb_tables_regid_t,
                                           behavior_i  IN   mb_tables_behavior_t)
  /***********************************************************************
  *
  *N  {update_mb_tables_behavior_map}  --  UPDATE behavior_map
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the MULTIBRANCH_TABLES behavior_map 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     regid_i         <IN>  ==  (mb_tables_regid_t)  registration_id
  *     behavior_i      <IN>  ==  (mb_tables_behavior_t)  behavior map
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    UPDATE SDE.multibranch_tables SET behavior_map = behavior_i
    WHERE registration_id = regid_i;

  END update_mb_tables_behavior_map;

  PROCEDURE get_mb_tables_by_id     (regid_i      IN   mb_tables_regid_t,
                                     mbt          OUT  mb_tables_record_t)
  /***********************************************************************
  *
  *N  {get_mb_tables_by_id}  --  Get MULTIBRANCH_TABLES row by 
  *                              registration_id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure gets a rows from the MULTIBRANCH_TABLES by
  *  registration_id 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     regid_i         <IN>  ==  (mb_tables_regid_t)  registration_id
  *     mbt             <OUT> ==  (mb_tables_record_t)  behavior map
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt                  12/18/2015           Original coding.
  *E
  ***********************************************************************/
  IS

  BEGIN

    OPEN G_mb_tables_get_cursor (regid_i);

    FETCH G_mb_tables_get_cursor INTO mbt.start_moment, mbt.behavior_map;
    IF G_mb_tables_get_cursor%NOTFOUND THEN
      raise_application_error (SDE.sde_util.SE_TABLE_NOT_MULTIBRANCH,
       'Registration ID' || regid_i || ' not found in MULTIBRANCH_TABLES.');
    END IF;
    CLOSE G_mb_tables_get_cursor;

 END get_mb_tables_by_id;

  BEGIN
  /***********************************************************************
   *
   *N  {Global Initialization}  --  Initialize Global state
   *
   ***********************************************************************/

    G_current_user := sde_util.sde_user;
    G_sde_dba := (G_current_user = sde_util.C_sde_dba);

    G_edit_moment := to_timestamp('12.31.9999 23:59:59.000','mm.dd.yyyy HH24:MI:SS.FF3');
    G_ancestor_moment := to_timestamp('12.31.9999 23:59:59.000','mm.dd.yyyy HH24:MI:SS.FF3');
    G_branch_id := 0;
    G_username := '';
    G_session_guid := '';

END branch_util;
