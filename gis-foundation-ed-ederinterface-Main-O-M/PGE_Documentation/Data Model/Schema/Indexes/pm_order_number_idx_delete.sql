DECLARE
--cursor jobnumberlist is select objectclassname,fieldname from sde.mm_field_modelnames where modelname='PGE_JOBNUMBER';
cursor jobnumberlist is select INDEX_NAME from SYS.ALL_INDEXES where index_name like '%_JOBNUM_UPPER_IDX';
sqlstmt varchar2(4000);
regid number;
BEGIN
   FOR jobfield in jobnumberlist loop
	 sqlstmt :='drop index '||jobfield.index_name;
	  -- dbms_output.put_line(sqlstmt);
	  execute immediate sqlstmt;
   END LOOP;
END;
/
