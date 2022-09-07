set heading off;
whenever sqlerror exit sql.sqlcode

-- INSERT FROM EDER DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDER' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDER where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDER'));
commit;
   

-- INSERT FROM EDERSUB DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDERSUB' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDERSUB where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDERSUB'));
commit;
   
--INSERT FROM EDSCHM DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDSCHM' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDSCHM where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDSCHM'));
commit;   

--INSERT FROM EDGMC DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDGMC' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDGMC where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDGMC'));
commit;   

--INSERT FROM EDSUBGMC DATABASE 

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDSUBGMC' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDSUBGMC where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDSUBGMC'));
commit;
   
--INSERT FROM EDWIPGMC DATABASE 

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDWIPGMC' ,  OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDWIPGMC where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDWIPGMC'));
commit;   

--INSERT FROM LBMAINT DATABASE 

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'LBMAINT' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkLBMAINT where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'LBMAINT'));
commit;

--INSERT FROM LANDBASE DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'LANDBASE' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkLANDBASE where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'LANDBASE'));
commit;

--INSERT FROM EDPUB DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDPUB' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDPUB where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDPUB'));
commit;
   

--INSERT FROM EDSUBPUB DATABASE

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDSUBPUB' ,  OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDSUBPUB where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDSUBPUB'));
commit;
   
--INSERT FROM EDSCHMPUB DATABASE 

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDSCHMPUB' ,  OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkEDSCHMPUB where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDSCHMPUB'));
commit;
  
--INSERT FROM WIP DATABASE  

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'WIP' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID from sys.dba_audit_trail@dbLinkWIP where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'WIP'));
commit;
   
--INSERT FROM EDAUX DATABASE  

INSERT INTO INTDATAARCH.dba_audit_trail_backup
(SELECT 'EDAUX' , OS_USERNAME , USERNAME ,USERHOST ,TERMINAL ,  extended_timestamp, OWNER ,OBJ_NAME , ACTION , RETURNCODE , DBID   from sys.dba_audit_trail@dbLinkEDAUX where returncode ='1017' and extended_timestamp not in (select extended_timestamp from  INTDATAARCH.dba_audit_trail_backup where DBNAME = 'EDAUX'));
commit;

exit;
   






