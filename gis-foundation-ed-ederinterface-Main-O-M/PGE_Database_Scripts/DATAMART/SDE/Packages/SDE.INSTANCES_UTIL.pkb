Prompt drop Package Body INSTANCES_UTIL;
DROP PACKAGE BODY SDE.INSTANCES_UTIL
/

Prompt Package Body INSTANCES_UTIL;
--
-- INSTANCES_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.instances_util
/***********************************************************************
*
*N  {instances_util.spb}  --  Implementation for instances DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on the instances table.  It should be 
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
*    Gautam Shanbhag         07/21/2004               Original coding.
*E
***********************************************************************/
IS

   /* Public Subprograms. */

PROCEDURE insert_instances (in_instance_id        IN  instanceid_t,
                            in_instance_name IN instance_name_t,
    				in_status	IN status_t)
  /***********************************************************************
  *
*N  {insert_instances}  --  Add record to instances table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
*     This procedure adds an entry to the <master schema>.dbtune table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
*	in_instance_id        == (instance_id_t)  instance id to be entered
*	in_instance_name      == (instance_name_t)  instance name to be entered
*	in_status             == (status_t)  status to be entered
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
*    Gautam Shanbhag         07/21/2004               Original coding
  *E
  ***********************************************************************/
   IS

   BEGIN

      
	    INSERT INTO sde.instances 
      	        (instance_id, 
            	    instance_name, 
	                creation_date,
			    status)
      	VALUES (in_instance_id, 
            	    in_instance_name,
	                SYSDATE,
	                in_status);      
-- Got this far without an exception, it's safe to commit.

    COMMIT;

         EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
END insert_instances;

PROCEDURE delete_instances(in_instance_id  IN instanceid_t)
  /***********************************************************************
  *
*N  {delete_instances}  --  Delete an instance record 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
*     This procedure deletes a record from instances table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
*	in_instance_id        == (instanceid_t)  instance id to be deleted
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
*    Gautam Shanbhag         07/21/2004               Original coding
  *E
  ***********************************************************************/
   IS

   BEGIN
    DELETE FROM sde.instances 
	WHERE instance_id = in_instance_id;

    -- Got this far without an exception, it's safe to commit.

    COMMIT;

     EXCEPTION
            WHEN OTHERS THEN
                 ROLLBACK;
                 RAISE;
END delete_instances;

PROCEDURE check_instance_table_conflicts (in_table_name IN  table_name_t,in_table_owner IN  table_owner_t,current_schema_name IN instance_name_t)
  /***********************************************************************
  *
  *N  {check_instance_table_conflicts}  --  Check if the table is already registered
  * 							in another instance.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a supplied table name against Table_Registry table in all 
  * 	the instances in the Database.If a table is found, a SE_TABLE_OUTSIDE_SCHEMA exception
  *   is raised.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     in_table_name  <IN>  ==  (table_name_t) The table name to check.
  *	  in_table_owner <IN>	==  (table_owner_t) The layer owner to check.	
  *     current_schema_name  <IN>  ==  (schema_name_t) The connected schema name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20417                SE_TABLE_OUTSIDE_SCHEMA
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Gautam Shanbhag                07/26/04           Original coding.
  *E
  ***********************************************************************/
   IS

    c1                  INTEGER; 
    c1_status           INTEGER;
    c2                  INTEGER;
    c2_status           INTEGER;
    table_conflict	BOOLEAN := FALSE;   
    table_loop_done     BOOLEAN := FALSE;
    instance_name       VARCHAR2(160);
	found_instance_name	instance_name_t;
	table_row_count NUMBER;

	BEGIN
     IF NOT dbms_sql.is_open(c1) THEN
       c1 := dbms_sql.open_cursor;
     END IF;
     dbms_sql.parse(c1,
          'SELECT instance_name ' ||
          'FROM   sde.instances ' ||
          'WHERE instance_name <> :current_schema_name', dbms_sql.native);
     dbms_sql.bind_variable(c1, ':current_schema_name', current_schema_name);
     dbms_sql.define_column(c1, 1, instance_name, 120);
     c1_status := dbms_sql.execute(c1);       

     LOOP 
      IF dbms_sql.fetch_rows(c1) = 0 THEN 
         table_loop_done := TRUE;
      ELSE
       dbms_sql.column_value(c1, 1, instance_name);

       IF NOT dbms_sql.is_open(c2) THEN
           c2 := dbms_sql.open_cursor;
       END IF;
       dbms_sql.parse(c2, 
              'SELECT registration_id FROM '||
              TO_CHAR(instance_name)||'.TABLE_REGISTRY ' ||
	        ' WHERE owner = :owner AND table_name = :table_name ', dbms_sql.native);
       dbms_sql.bind_variable(c2, ':owner', in_table_owner);
       dbms_sql.bind_variable(c2, ':table_name', in_table_name);
       dbms_sql.define_column(c2, 1, table_row_count);
       c2_status := dbms_sql.execute(c2);	
       
       IF dbms_sql.fetch_rows(c2) = 0 THEN
         table_row_count := 0;
       ELSE 
         dbms_sql.column_value(c2, 1, table_row_count);
       END IF;
       
       dbms_sql.close_cursor(c2);
			IF table_row_count > 0 THEN
				table_conflict := TRUE;
				table_loop_done := TRUE;
                                found_instance_name := instance_name;
       END IF;
			END IF;
      EXIT WHEN table_loop_done;
     END LOOP;
     dbms_sql.close_cursor(c1);
	IF table_conflict THEN
		  raise_application_error (sde_util.SE_TABLE_OUTSIDE_SCHEMA,
                                  'TABLE Already registered In Schema ' || 
						to_char(found_instance_name));
      END IF;

          
    EXCEPTION 
      WHEN OTHERS THEN
       RAISE;

END check_instance_table_conflicts;

END instances_util;

/


Prompt Grants on PACKAGE INSTANCES_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.INSTANCES_UTIL TO PUBLIC WITH GRANT OPTION
/
