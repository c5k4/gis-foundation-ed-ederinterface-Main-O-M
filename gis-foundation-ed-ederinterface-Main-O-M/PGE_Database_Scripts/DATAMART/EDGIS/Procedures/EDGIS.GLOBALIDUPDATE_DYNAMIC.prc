Prompt drop Procedure GLOBALIDUPDATE_DYNAMIC;
DROP PROCEDURE EDGIS.GLOBALIDUPDATE_DYNAMIC
/

Prompt Procedure GLOBALIDUPDATE_DYNAMIC;
--
-- GLOBALIDUPDATE_DYNAMIC  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.GLOBALIDUPDATE_DYNAMIC IS
  /* Formatted on 11/10/2016 2:42:18 PM (QP5 v5.252.13127.32847) */
   l_cursor           SYS_REFCURSOR;
   l_objectid         NUMBER;

   CURSOR ALL_TABLES_REQUIRED
   IS
      SELECT a.owner, a.table_name
        FROM all_tab_columns a
       WHERE     a.column_name = 'GLOBALID'
             AND a.table_name IN (SELECT table_name
                                    FROM sde.table_registry
                                   WHERE OWNER = 'EDGIS')
             AND a.table_name LIKE '%ANNO'
             AND a.OWNER = 'EDGIS';

   SQLSTM             VARCHAR2 (2000);
   sqlstmobjectid     VARCHAR2 (4000);
   countobj           NUMBER;
   reg_num            NUMBER;
   count_num          NUMBER;
   row_count          NUMBER;
   validglobalid      VARCHAR2 (50);
   reqobjectid        NUMBER;
   atableexistcount   NUMBER;
   query_str          VARCHAR2 (2000);
   reqobjectidcount   NUMBER;
BEGIN
   FOR tables IN ALL_TABLES_REQUIRED
   LOOP
      countobj := 0;
      sqlstmobjectid :=
            'select count(column_name) from all_tab_columns where column_name =''OBJECTID'' and table_name='''
         || tables.table_name
         || ''' and owner='''
         || tables.owner
         || ''' ';


      DBMS_OUTPUT.put_line (sqlstmobjectid);

      EXECUTE IMMEDIATE sqlstmobjectid INTO countobj;


      IF countobj = 0
      THEN
         DBMS_OUTPUT.put_line (
               'Unable to find OBJECTID in the table specified of '
            || tables.owner
            || '.'
            || tables.table_name);
      ELSE
         DBMS_OUTPUT.put_line (
               'ABLE to find OBJECTID in the table specified of '
            || tables.owner
            || '.'
            || tables.table_name);
         --atable verrification
         sqlstm :=
               'select registration_id from sde.table_registry where table_name='''
            || tables.table_name
            || ''' and owner='''
            || tables.owner
            || ''' ';
         --DBMS_OUTPUT.put_line (sqlstm);

         EXECUTE IMMEDIATE sqlstm INTO reg_num;

         sqlstm :=
               'select count(*) from all_tables where table_name=''A'
            || reg_num
            || ''' and owner='''
            || tables.owner
            || ''' ';
         --DBMS_OUTPUT.put_line (sqlstm);

         EXECUTE IMMEDIATE sqlstm INTO atableexistcount;

         --DBMS_OUTPUT.put_line (sqlstm);


         IF atableexistcount = 1
         THEN
            DBMS_OUTPUT.PUT_LINE (tables.owner || '.A' || reg_num);
            EDGIS.GET_CURSOR_GLOBALIDDYNAMIC (
               p_tab   => tables.owner || '.A' || reg_num,
               p_cur   => l_cursor);

            LOOP
               FETCH l_cursor INTO l_objectid;

               EXIT WHEN l_cursor%NOTFOUND;
             --  DBMS_OUTPUT.PUT_LINE (l_objectid);
               -- FIND is there any valid GLOBALID for atleast one record
               query_str :=
                     'SELECT MAX(GLOBALID) FROM '
                  || tables.owner
                  || '.A'
                  || reg_num
                  || ' where OBJECTID ='
                  || l_objectid
                  || ' and   GLOBALID is not null and GLOBALID <>''{00000000-0000-0000-0000-000000000000}''';
               --DBMS_OUTPUT.PUT_LINE (query_str);

               EXECUTE IMMEDIATE query_str INTO validglobalid;

               IF validglobalid IS NULL OR validglobalid = ''
               THEN
                  validglobalid := SDE.GDB_UTIL.NEXT_GLOBALID;
               END IF;

               sqlstm :=
                     'UPDATE '
                  || tables.owner
                  || '.A'
                  || reg_num
                  || ' SET GLOBALID ='''
                  || validglobalid
                  || ''' where OBJECTID ='
                  || l_objectid
                  || ' and ( GLOBALID is null or GLOBALID =''{00000000-0000-0000-0000-000000000000}'')';


               EXECUTE IMMEDIATE sqlstm;

               COMMIT;

               -- Base Table update for Same OBJECTID
               row_count := row_count + 1;
               --DBMS_OUTPUT.PUT_LINE(l_objectid);
               sqlstm :=
                     'update '
                  || tables.owner
                  || '.'
                  || tables.table_name
                  || ' SET GLOBALID ='''
                  || validglobalid
                  || ''' where OBJECTID ='
                  || l_objectid
                  || ' and ( GLOBALID is null or GLOBALID =''{00000000-0000-0000-0000-000000000000}'')';

               --  DBMS_OUTPUT.PUT_LINE(sqlstm);
               EXECUTE IMMEDIATE sqlstm;

               COMMIT;
            END LOOP;

            CLOSE l_cursor;

            DBMS_OUTPUT.PUT_LINE (
               ' Globalids  are updated A Tables: ' || row_count);
         END IF;



         EDGIS.GET_CURSOR_GLOBALIDDYNAMIC (
            p_tab   => tables.owner || '.' || tables.table_name,
            p_cur   => l_cursor);
         row_count := 0;

         LOOP
            FETCH l_cursor INTO l_objectid;

            EXIT WHEN l_cursor%NOTFOUND;

            row_count := row_count + 1;
            --DBMS_OUTPUT.PUT_LINE(l_objectid);
            validglobalid := SDE.GDB_UTIL.NEXT_GLOBALID;

            sqlstm :=
                  'update '
               || tables.owner
               || '.'
               || tables.table_name
               || ' SET GLOBALID ='''
               || validglobalid
               || ''' where OBJECTID ='
               || l_objectid
               || ' and ( GLOBALID is null or GLOBALID =''{00000000-0000-0000-0000-000000000000}'')';

            --  DBMS_OUTPUT.PUT_LINE(sqlstm);
            EXECUTE IMMEDIATE sqlstm;

            COMMIT;
         END LOOP;

         CLOSE l_cursor;

         DBMS_OUTPUT.PUT_LINE (' Globalids  are updated : ' || row_count);
      END IF;
   END LOOP;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.PUT_LINE (
         'Unexpected error other in GLOBALIDUPDATE_DYNAMIC');
END;
/
