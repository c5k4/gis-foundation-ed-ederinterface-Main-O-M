Prompt drop Package Body KEYSET_UTIL;
DROP PACKAGE BODY SDE.KEYSET_UTIL
/

Prompt Package Body KEYSET_UTIL;
--
-- KEYSET_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.keyset_util
/***********************************************************************
*
*N  {keyset_util.spb}  --  Implementation for keyset DML package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DML operations on the keyset table.  It should be 
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
*    Gautam Shanbhag      03/04/2005               Original coding.
*E
***********************************************************************/
IS

   /* Public Subprograms. */
   PROCEDURE create_keyset_table (tname       IN NVARCHAR2,
					    current_user IN NVARCHAR2,
					    dbtune_str_param IN NCLOB)
  /***********************************************************************
  *
  *N  {create_keyset_table}  --  Create a new keyset table
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure creates a new keyset table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    tname        <IN> == (VARCHAR2)  table name to be created.
  *    current_user <IN> == (VARCHAR2) current user.
  *    dbtune_str_param <IN> == (VARCHAR2) creation config param. 
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
  *    Gautam Shanbhag            03/04/2005               Original coding.
  *E
  ***********************************************************************/
   IS
	sqlstmt CLOB;
   BEGIN
		sqlstmt :='CREATE TABLE '||TO_CHAR(tname)||'(KEYSET_ID INTEGER NOT NULL,';
            sqlstmt :=sqlstmt||'LONG_VAL	INTEGER,STR_VAL		VARCHAR2(256),DBL_VAL		FLOAT(64),';
            sqlstmt :=sqlstmt||'DATE_VAL	DATE ) ';
            sqlstmt :=sqlstmt||TO_CHAR(dbtune_str_param);
		
            EXECUTE IMMEDIATE TO_CHAR(sqlstmt); 

            sqlstmt := 'GRANT ALL ON '||TO_CHAR(tname)||' TO PUBLIC'||''; 
            EXECUTE IMMEDIATE TO_CHAR(sqlstmt); 
	   
         EXCEPTION
            WHEN OTHERS THEN  
                 RAISE;
   END create_keyset_table;

   PROCEDURE delete_keyset (tname       IN NVARCHAR2,
				            keyset_id   IN INTEGER)
  /***********************************************************************
  *
  *N  {delete_keyset}  --  Deletes a keyset from the keyset table
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure truncates the keyset table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    tname       <IN> == (VARCHAR2)  keyset table name.
  *    keyset_id        <IN> == (INTEGER) Keyset_id to be deleted
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
  *    Gautam Shanbhag            03/04/2005               Original coding.
  *E
  ***********************************************************************/
   IS
      cursor_id     INTEGER;
      cursor_status INTEGER;
   BEGIN
      cursor_id := dbms_sql.open_cursor;
      dbms_sql.parse(cursor_id, 'DELETE FROM '||TO_CHAR(tname)||' WHERE keyset_id= :keyset_id', dbms_sql.native);
      dbms_sql.bind_variable(cursor_id, ':keyset_id', keyset_id);
      cursor_status := dbms_sql.execute(cursor_id);
      dbms_sql.close_cursor(cursor_id);
   EXCEPTION
      WHEN OTHERS THEN
        dbms_sql.close_cursor(cursor_id);
        RAISE;
   END delete_keyset;

   PROCEDURE remove_keyset_table (tname       IN NVARCHAR2)
  /***********************************************************************
  *
  *N  {remove_keyset_table}  --  Drops the keyset table
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure drops the keyset table.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    tname       <IN> == (VARCHAR2)  keyset table name.
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
  *    Gautam Shanbhag            03/04/2005               Original coding.
  *E
  ***********************************************************************/
   IS
	PRAGMA AUTONOMOUS_TRANSACTION;
	sqlstmt VARCHAR2(1024);
   BEGIN
		sqlstmt :='DROP TABLE '||TO_CHAR(tname)||'';
            EXECUTE IMMEDIATE sqlstmt; 
    EXCEPTION
            WHEN OTHERS THEN
                 RAISE;
   END remove_keyset_table;

END keyset_util;

/


Prompt Grants on PACKAGE KEYSET_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.KEYSET_UTIL TO PUBLIC WITH GRANT OPTION
/
