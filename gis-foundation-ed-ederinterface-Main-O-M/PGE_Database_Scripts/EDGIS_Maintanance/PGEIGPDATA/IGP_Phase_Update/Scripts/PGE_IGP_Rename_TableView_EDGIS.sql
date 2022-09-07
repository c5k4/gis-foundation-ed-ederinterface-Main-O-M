
spool D:\TEMP\Rename_View.txt

declare
  procedure run(p_sql varchar2) as
  begin
    execute immediate p_sql;
  end;
begin		
 run('rename "PGE_IGP_EXP_REPORT_EVW" to ZZ_MV_PGE_IGP_EXP_REPORT');
 run('alter view  "EDGIS".ZZ_MV_PGE_IGP_EXP_REPORT compile');
end;
/
spool off
