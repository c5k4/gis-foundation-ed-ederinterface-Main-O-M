--------------------------------------------------------
--  DDL for Procedure TRUNCATETRACINGTABLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRUNCATETRACINGTABLES" AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt := 'select count(*) from cat where table_name=''PGE_ELECDISTNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_ELECDISTNETWORK_TRACE';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_TRACE_TEMP'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_TRACE_TEMP';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_UNDERGROUNDNETWORK_TEMP'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_UndergroundNetwork_Temp';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_UNDERGROUNDNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_UndergroundNetwork_Trace';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_FEEDERFEDNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_FEEDERFEDNETWORK_TRACE';
                execute immediate sqlstmt;
  END IF;

  sqlstmt := 'select count(*) from cat where table_name=''PGE_CACHEDTRACE_TOPROCESS'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_CACHEDTRACE_TOPROCESS';
                execute immediate sqlstmt;
  END IF;
  COMMIT;
END;
