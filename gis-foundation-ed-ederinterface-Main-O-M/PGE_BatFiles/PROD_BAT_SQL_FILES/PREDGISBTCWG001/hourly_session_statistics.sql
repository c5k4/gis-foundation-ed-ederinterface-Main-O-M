set feedback off
set echo off 
set SERVEROUTPUT ON
set serveroutput on size 100000;
set line 250

col session_name format a30
col SessionID format a10
col current_owner format a13


DECLARE

rowval varchar(5000);
rowval1 varchar(1000);
rowval2 varchar(1000);
rowval3 varchar(1000);
rowval4 varchar(1000);
rowval5 varchar(1000);
rowval6 varchar(1000);
rowval7 varchar(1000);
rowval8 varchar(1000);
rowval9 varchar(1000);

 curr_time0 DATE;
 curr_time DATE;
 onehr_time DATE;
 twohr_time DATE;
 threehr_time DATE;
daysBefore NUMBER; 
tempCount NUMBER;
 posted1_count     NUMBER;
 posted2_count     NUMBER;
 posted3_count     NUMBER;
 newsession1_count     NUMBER;
 newsession2_count     NUMBER;
 newsession3_count     NUMBER;
 conflict1_count     NUMBER;
 conflict2_count     NUMBER;
 conflict3_count     NUMBER;
 dp1_count     NUMBER;
 pq1_count     NUMBER;
 postE1_count     NUMBER;
 qaqcE1_count     NUMBER;
 reconcileE1_count     NUMBER;
 pqaqc1_count     NUMBER;
 
 dp3_count     NUMBER;
 pq3_count     NUMBER;
 postE3_count     NUMBER;
 qaqcE3_count     NUMBER;
 reconcileE3_count     NUMBER;
 pqaqc3_count     NUMBER;
 --posted_count
 --newsession_count
 --conflict_count
 --dp_count
 --pq_count
 --postE_count
 --reconcileE_count
 --qaqcE_count
 --pqaqc_count
 
 
 type posted_count IS VARRAY(24) OF INTEGER;
 type newsession_count IS VARRAY(24) OF INTEGER;
 type conflict_count IS VARRAY(24) OF INTEGER;
 type dp_count IS VARRAY(24) OF INTEGER;
 type pq_count IS VARRAY(24) OF INTEGER;
 type postE_count IS VARRAY(24) OF INTEGER;
 type reconcileE_count IS VARRAY(24) OF INTEGER;
 type qaqcE_count IS VARRAY(24) OF INTEGER;
 type pqaqc_count IS VARRAY(24) OF INTEGER;
  posted_countv posted_count:=posted_count();
 newsession_countv newsession_count:=newsession_count();
 conflict_countv conflict_count:=conflict_count();
 dp_countv dp_count:=dp_count();
 pq_countv pq_count:=pq_count();
 postE_countv postE_count:=postE_count();
 reconcileE_countv reconcileE_count:=reconcileE_count();
 qaqcE_countv qaqcE_count:=qaqcE_count();
 pqaqc_countv pqaqc_count:=pqaqc_count();
 
 dp2_count     NUMBER;
 pq2_count     NUMBER;
 postE2_count     NUMBER;
 qaqcE2_count     NUMBER;
 reconcileE2_count     NUMBER;
 pqaqc2_count     NUMBER;
 

BEGIN

 DBMS_OUTPUT.ENABLE(NULL);
daysBefore:=0;

select trunc(sysdate, 'HH') into curr_time0 from dual;
select trunc(sysdate, 'HH')-daysBefore into curr_time from dual;
onehr_time:=curr_time-1/24;
twohr_time:=onehr_time-1/24;
threehr_time:=twohr_time-1/24;


   FOR i IN 1..24 LOOP
   
	 posted_countv.extend();
	 newsession_countv.extend();
 conflict_countv.extend();
 dp_countv.extend();
 pq_countv.extend();
 postE_countv.extend();
 reconcileE_countv.extend();
 qaqcE_countv.extend();
 pqaqc_countv.extend();
 
onehr_time:=(curr_time-i/24);
twohr_time:=(curr_time-(i-1)/24);
--DBMS_OUTPUT.PUT_LINE(TO_CHAR(onehr_time, 'HH:MI:SS MON-DD-YYYY') ||chr(9)|| ' and ' || TO_CHAR(twohr_time, 'HH:MI:SS MON-DD-YYYY'));
	 
	select count(*) into tempCount  from  SDE.GDBM_POST_HISTORY
 where   POST_START_DT  > (curr_time-i/24) and POST_END_DT<=(curr_time-(i-1)/24)  and POST_RESULT = 'POSTED';
 
 
posted_countv(i):=tempCount;
--DBMS_OUTPUT.PUT_LINE(posted_countv(i) ||chr(9));
   
--dbms_output.put_line(posted_countv(i) || chr(9));

/*  select count(*) into tempCount from  PROCESS.MM_SESSION
 where   CREATE_DATE  >  (curr_time-i/24) and CREATE_DATE<=(curr_time-(i-1)/24)  and length(CREATE_USER)=4;
 --dbms_output.put_line(tempCount || chr(9));
  newsession_countv(i):=tempCount; */
  
  select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%Session created%' and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24) and length(USER_NAME)=4;
 newsession_countv(i):=tempCount;
 
 select count(*) into tempCount 
from  
(select substr(REGEXP_SUBSTR(VERSION_NAME, '[^.]+', 1, 2), 4) as SESSION_NUM, version_name
from SDE.GDBM_RECONCILE_HISTORY
where SERVICE_NAME like 'GDBMReconcileOnly%'
and RECONCILE_START_DT  >=  (curr_time-i/24) and SDE.GDBM_RECONCILE_HISTORY.RECONCILE_END_DT<=(curr_time-(i-1)/24)
and RECONCILE_RESULT = 'Conflicts'
group by VERSION_NAME) g
inner join
(select to_char(SESSION_ID) as SESSION_NUM, substr(SESSION_NAME, 1, 40) as SESSION_NAME, CREATE_USER, CURRENT_OWNER, CREATE_DATE, substr(regexp_replace(DESCRIPTION, '[[:space:]]', ' '), 1, 80) as DESCRIPTION
from PROCESS.MM_SESSION where length(CREATE_USER)=4) s
on g.SESSION_NUM = s.SESSION_NUM
order by g.SESSION_NUM ;

 conflict_countv(i):=tempCount;

select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '% New state (Data Processing)%' and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
 dp_countv(i):=tempCount;
select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%DataProcessingToPostQueue% New state (Post Queue)%' and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
 pq_countv(i):=tempCount;
select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%new state (QA/QC Error)%' and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
 qaqcE_countv(i):=tempCount;
select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%ew state (Post Error)%'  and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
postE_countv(i):=tempCount;
select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%ew state (Reconcile Error)%'  and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
 reconcileE_countv(i):=tempCount;
select count(*) into tempCount  from PROCESS.MM_PX_HISTORY a where a.DESCRIPTION like '%New state (Pending QA/QC)%' and DATE_TIME> (curr_time-i/24) and DATE_TIME<=(curr_time-(i-1)/24);
pqaqc_countv(i):=tempCount;

      
   END LOOP;
   
   
   
  rowval:='Statistics About @'|| TO_CHAR (curr_time, 'MM-DD-YY HH24:MI') || ',' ;
  
  
  
   rowval1:='User Sessions Created#,';
   rowval2:='Pending QA/QC#,';
   rowval3:='in Data Processing#,';
   rowval4:='in Post Queue#,';
   rowval5:='in Conflict#,';
   rowval6:='in Reconcile Error#,';
   rowval7:='in QA/QC Error#,';
   rowval8:='in Post Error#,';
   rowval9:='Sessions Posted#,'; 
   
   FOR i IN 1..24 LOOP
	 rowval:=rowval || TO_CHAR ((curr_time-(i-1)/24), 'HH24') || ' to ' || TO_CHAR ((curr_time-i/24) , 'HH24') || ',';
	 --rowval:=rowval || TO_CHAR ((curr_time-(i-1)/24), 'HH24:MI') || ' to ' || TO_CHAR ((curr_time-i/24) , 'HH24:MI') || ',';
	rowval1:=rowval1 || newsession_countv(i)|| ',';
	rowval2:=rowval2 || pqaqc_countv(i)|| ',';
	rowval3:=rowval3 || dp_countv(i)|| ',';
	rowval4:=rowval4 || pq_countv(i)|| ',';
	rowval5:=rowval5 || conflict_countv(i)|| ',';
	rowval6:=rowval6 || reconcileE_countv(i)|| ',';
	rowval7:=rowval7 || qaqcE_countv(i)|| ',';
	rowval8:=rowval8 || postE_countv(i)|| ',';
	rowval9:=rowval9 || posted_countv(i)|| ','; 
	
	 END LOOP; 
   
    DBMS_OUTPUT.PUT_LINE(rowval ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval1 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval2 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval3 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval4 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval5 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval6 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval7 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval8 ||chr(9));
   DBMS_OUTPUT.PUT_LINE(rowval9 ||chr(9));
	
	
 
	
   
 DBMS_OUTPUT.PUT_LINE(chr(10)); 

 
 
 
 
END;
/
quit;