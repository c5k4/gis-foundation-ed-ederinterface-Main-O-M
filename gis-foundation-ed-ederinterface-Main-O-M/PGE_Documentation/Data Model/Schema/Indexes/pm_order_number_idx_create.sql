DECLARE
cursor jobnumberlist is select objectclassname,fieldname from sde.mm_field_modelnames where modelname='PGE_JOBNUMBER';
sqlstmt varchar2(4000);
regid number;
BEGIN
   FOR jobfield in jobnumberlist loop
         select registration_id into regid from sde.table_registry where OWNER||'.'||TABLE_NAME= jobfield.objectclassname;
                 sqlstmt :='create index J'||regid||'_jobnum_upper_idx on '||jobfield.objectclassname||'(UPPER('||jobfield.fieldname||'))';
                  -- dbms_output.put_line(sqlstmt);
                  execute immediate sqlstmt;
   END LOOP;
END;
/
-- Create indexes on A Tables.
DECLARE
cursor jobnumberlist is select objectclassname,fieldname from sde.mm_field_modelnames where modelname='PGE_JOBNUMBER' and OBJECTCLASSNAME in (
select OWNER||'.'||TABLE_NAME from sde.table_registry where bitand(object_flags,8)=8);
sqlstmt varchar2(4000);
regid number;
ownerid varchar2(300);
BEGIN
   FOR jobfield in jobnumberlist loop
         select registration_id,owner into regid,ownerid from sde.table_registry where OWNER||'.'||TABLE_NAME= jobfield.objectclassname;
                 sqlstmt :='create index JA'||regid||'_jobnum_upper_idx on '||ownerid||'.A'||regid||'(UPPER('||jobfield.fieldname||'))';
                   dbms_output.put_line(sqlstmt);
                  execute immediate sqlstmt;
   END LOOP;
END;
/
