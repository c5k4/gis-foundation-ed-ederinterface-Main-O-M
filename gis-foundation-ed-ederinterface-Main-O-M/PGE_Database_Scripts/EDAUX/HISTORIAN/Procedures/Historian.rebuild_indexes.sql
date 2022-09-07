CREATE OR REPLACE 
procedure rebuild_indexes is
/*   rebuild the indexes in historian that are type normal and normal/rev
*/
CURSOR get_indexes
IS
   SELECT 'ALTER INDEX historian.'||index_name||' '||
          'REBUILD ONLINE parallel (degree 21 ) NOLOGGING' text
    FROM all_indexes
   WHERE owner ='HISTORIAN'
     AND table_name LIKE '%_HIST'
     AND INDEX_TYPE IN ('NORMAL', 'NORMAL/REV')
     AND TEMPORARY='N';

CURSOR fix_parallel
IS
   SELECT 'ALTER INDEX historian.'||index_name||' '||'NOPARALLEL'
     FROM all_indexes
    WHERE owner ='HISTORIAN'
     AND INDEX_TYPE IN ('NORMAL', 'NORMAL/REV')
     AND TEMPORARY='N';

   v_text  varchar2(2000);

begin
   OPEN get_indexes;
      LOOP
      FETCH get_indexes INTO v_text;
      EXIT WHEN get_indexes%NOTFOUND;
      execute immediate v_text;
   END LOOP;
   CLOSE get_indexes;

   OPEN fix_parallel;
      LOOP
      FETCH fix_parallel INTO v_text;
      EXIT WHEN fix_parallel%NOTFOUND;
      execute immediate v_text;
   END LOOP;
   CLOSE fix_parallel;
end rebuild_indexes;
/

GRANT EXECUTE ON rebuild_indexes TO edtlm
/


