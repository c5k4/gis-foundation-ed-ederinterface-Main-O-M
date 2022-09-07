-- Start of DDL Script for User HISTORIAN

CREATE USER historian
IDENTIFIED BY xxx
DEFAULT TABLESPACE EDTLM
TEMPORARY TABLESPACE TEMP
PROFILE PROF_APP
/
GRANT CREATE PROCEDURE TO historian
/
GRANT CREATE SESSION TO historian
/
GRANT CREATE TABLE TO historian
/
GRANT UNLIMITED TABLESPACE TO historian
/


-- End of DDL Script for User HISTORIAN

CREATE TABLE historian.ccb_meter_load_hist
    (batch_date                     DATE NOT NULL,
    service_point_id               VARCHAR2(10 BYTE) NOT NULL,
    unqspid                        VARCHAR2(10 BYTE),
    acct_id                        VARCHAR2(10 BYTE),
    rev_kwhr                       VARCHAR2(11 BYTE),
    rev_kw                         VARCHAR2(8 BYTE),
    pfactor                        VARCHAR2(3 BYTE),
    sm_sp_status                   VARCHAR2(10 BYTE),
    rev_month                      VARCHAR2(2 BYTE),
    rev_year                       VARCHAR2(4 BYTE))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  edtlm
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  NOLOGGING
/

GRANT DELETE ON historian.ccb_meter_load_hist TO edtlm
/
GRANT INSERT ON historian.ccb_meter_load_hist TO edtlm
/
GRANT SELECT ON historian.ccb_meter_load_hist TO edtlm
/


--DROP INDEX historian.ccb_meter_load_hist_pk;
CREATE UNIQUE INDEX historian.ccb_meter_load_hist_pk ON historian.ccb_meter_load_hist
  (
    batch_date                      ASC,
    service_point_id                ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/

--DROP TABLE historian.sm_sp_gen_load_hist;
CREATE TABLE historian.sm_sp_gen_load_hist
    (cgc                            NUMBER(12,0) NOT NULL,
    service_point_id               VARCHAR2(10 BYTE) NOT NULL,
    batch_date                     DATE,
    sp_peak_kw                     NUMBER,
    vee_sp_kw_flag                 CHAR(1 BYTE),
    sp_peak_time                   DATE,
    sp_kw_trf_peak                 NUMBER,
    vee_trf_kw_flag                CHAR(1 BYTE),
    int_len                        NUMBER,
    sp_peak_kvar                   NUMBER,
    trf_peak_kvar                  NUMBER,
    create_date                    VARCHAR2(6 BYTE))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  edtlm
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  NOLOGGING
/

GRANT DELETE ON historian.sm_sp_gen_load_hist TO edtlm
/
GRANT INSERT ON historian.sm_sp_gen_load_hist TO edtlm
/
GRANT SELECT ON historian.sm_sp_gen_load_hist TO edtlm
/

--DROP INDEX historian.sm_sp_gen_load_hist_pk;
CREATE UNIQUE INDEX historian.sm_sp_gen_load_hist_pk ON historian.sm_sp_gen_load_hist
  (
    batch_date                      ASC,
    service_point_id                ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/


CREATE TABLE historian.sm_sp_load_hist
    (cgc                            NUMBER(12,0) NOT NULL,
    service_point_id               VARCHAR2(10 BYTE) NOT NULL,
    batch_date                     DATE,
    sp_peak_kw                     NUMBER,
    vee_sp_kw_flag                 CHAR(1 BYTE),
    sp_peak_time                   DATE,
    sp_kw_trf_peak                 NUMBER,
    vee_trf_kw_flag                CHAR(1 BYTE),
    int_len                        NUMBER,
    sp_peak_kvar                   NUMBER,
    trf_peak_kvar                  NUMBER,
    create_date                    VARCHAR2(6 BYTE))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  edtlm
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  NOLOGGING
/

GRANT DELETE ON historian.sm_sp_load_hist TO edtlm
/
GRANT INSERT ON historian.sm_sp_load_hist TO edtlm
/
GRANT SELECT ON historian.sm_sp_load_hist TO edtlm
/

--DROP INDEX historian.sm_sp_load_hist_cgc_idx ;
--DROP index historian.sm_sp_load_hist_pk;

CREATE UNIQUE INDEX historian.sm_sp_load_hist_cgc_idx ON historian.sm_sp_load_hist
  (
    batch_date                      ASC,
    cgc                             ASC,
    service_point_id                ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/

CREATE UNIQUE INDEX historian.sm_sp_load_hist_pk ON historian.sm_sp_load_hist
  (
    batch_date                      ASC,
    service_point_id                ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/


CREATE TABLE historian.sm_trf_gen_load_hist
    (cgc                            NUMBER(12,0) NOT NULL,
    batch_date                     DATE,
    trf_peak_kw                    NUMBER,
    trf_peak_time                  DATE,
    trf_avg_kw                     NUMBER,
    create_date                    VARCHAR2(6 BYTE))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  edtlm
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  NOLOGGING
/

GRANT DELETE ON historian.sm_trf_gen_load_hist TO edtlm
/
GRANT INSERT ON historian.sm_trf_gen_load_hist TO edtlm
/
GRANT SELECT ON historian.sm_trf_gen_load_hist TO edtlm
/
--DROP INDEX historian.sm_trf_gen_load_hist_pk;

CREATE UNIQUE INDEX historian.sm_trf_gen_load_hist_pk ON historian.sm_trf_gen_load_hist
  (
    batch_date                      ASC,
    cgc                             ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/

CREATE TABLE historian.sm_trf_load_hist
    (cgc                            NUMBER(12,0) NOT NULL,
    batch_date                     DATE,
    trf_peak_kw                    NUMBER,
    trf_peak_time                  DATE,
    trf_avg_kw                     NUMBER,
    create_date                    VARCHAR2(6 BYTE))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  edtlm
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  NOLOGGING
/

GRANT DELETE ON historian.sm_trf_load_hist TO edtlm
/
GRANT INSERT ON historian.sm_trf_load_hist TO edtlm
/
GRANT SELECT ON historian.sm_trf_load_hist TO edtlm
/

--DROP INDEX historian.sm_trf_load_hist_pk;

CREATE UNIQUE INDEX historian.sm_trf_load_hist_pk ON historian.sm_trf_load_hist
  (
    batch_date                      ASC,
    cgc                             ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  edtlmidx
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
NOLOGGING
/


CREATE OR REPLACE 
PROCEDURE historian.gather_my_stats (i_variant VARCHAR2) is
/*   gather the stats on the Historian schema tables
     i_variant = 'CCB' gathers stats on ccb_meter_load_hist
     i_variant = 'CDW' gathers stats on
         sm_sp_gen_load_hist, sm_sp_load_hist
         sm_trf_gen_load_hist, sm_trf_load_hist
     any other variant does nothing
*/

begin
  IF i_variant = 'CCB' THEN
     dbms_stats.gather_table_stats('HISTORIAN', 'ccb_meter_load_hist');
  ELSIF i_variant = 'CDW' THEN
     dbms_stats.gather_table_stats('HISTORIAN', 'sm_sp_gen_load_hist');
     dbms_stats.gather_table_stats('HISTORIAN', 'sm_sp_load_hist');
     dbms_stats.gather_table_stats('HISTORIAN', 'sm_trf_gen_load_hist');
     dbms_stats.gather_table_stats('HISTORIAN', 'sm_trf_load_hist');
  END IF ;
end gather_my_stats;
/

GRANT EXECUTE ON historian.gather_my_stats TO edtlm
/

CREATE OR REPLACE 
procedure           historian.rebuild_indexes is
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

GRANT EXECUTE ON historian.rebuild_indexes TO edtlm
/

SELECT object_name, object_type, status
  FROM dba_objects
WHERE owner = 'HISTORIAN' 
AND status <> 'VALID';

SELECT object_type, count(*)
FROM dba_objects
where owner = 'HISTORIAN'
group BY object_type;
