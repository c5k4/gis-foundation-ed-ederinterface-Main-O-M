--------------------------------------------------------
--  DDL for Procedure ED008_PREPARETEMPTABLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."ED008_PREPARETEMPTABLES" 
AS
BEGIN

--Clear log table
delete from EDGISBO.ED08Log;

--Delete Sequence
--EXECUTE IMMEDIATE 'DROP SEQUENCE ED08_RECORDID_SEQ';
--Logging
--INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
--VALUES (SYSDATE,'I','ED08_RECORDID_SEQ Sequence Dropped');
--COMMIT;

--Create Sequence
--EXECUTE IMMEDIATE 'CREATE SEQUENCE  "EDGISBO"."ED08_RECORDID_SEQ"  MINVALUE 1 MAXVALUE 9999999 INCREMENT BY 1';
--Logging
--INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
--VALUES (SYSDATE,'I','ED08_RECORDID_SEQ Sequence Created');
--COMMIT;

--Clear SubType DESCRIPTION Table
--DELETE FROM EDGISBO.ED08SUBTYPEVALUE;

--Logging


--Populate SubType DESCRIPTION Table


--Logging


--Drop EDGISBO.TEMPED08 Table
--EXECUTE IMMEDIATE 'DROP TABLE EDGISBO.TEMPED08Line';
EXECUTE IMMEDIATE 'TRUNCATE TABLE PGEDATA.ED08LINE_T' ;
--Logging
INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
VALUES (SYSDATE,'I','table ed08line_t truncated');
COMMIT;

--Logging
--INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
--VALUES (SYSDATE,'I','Dropped EDGISBO.TEMPED08Line Table');
--COMMIT;

--Create EDGISBO.TEMPED08 Table
INSERT INTO  PGEDATA.ED08LINE_T(OBJECTID, GLOBALID, SHAPE, NAME, CIRCUITID)
(SELECT sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T'), GLOBALID, SHAPE, 'EDGIS.DCConductor', CIRCUITID FROM EDGIS.ZZ_MV_DCConductor WHERE sde.st_isempty(shape) = 0 UNION
SELECT sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T'),GLOBALID, SHAPE, 'EDGIS.DistBusBar' , CIRCUITID FROM EDGIS.ZZ_MV_DistBusBar WHERE sde.st_isempty(shape) = 0 UNION
SELECT  sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T') ,GLOBALID, SHAPE, 'EDGIS.TransformerLead' , CIRCUITID FROM EDGIS.ZZ_MV_TransformerLead WHERE sde.st_isempty(shape) = 0 UNION
SELECT sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T')  ,GLOBALID, SHAPE, 'EDGIS.PRIOHCONDUCTOR' , CIRCUITID FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR WHERE sde.st_isempty(shape) = 0 UNION
SELECT  sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T'),GLOBALID, SHAPE, 'EDGIS.PRIUGCONDUCTOR' , CIRCUITID FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR WHERE sde.st_isempty(shape) = 0 UNION
SELECT sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T') ,GLOBALID, SHAPE, 'EDGIS.SECOHCONDUCTOR', CIRCUITID FROM EDGIS.ZZ_MV_SECOHCONDUCTOR WHERE sde.st_isempty(shape) = 0 UNION
SELECT   sde.gdb_util.next_rowid('PGEDATA', 'ED08LINE_T'),GLOBALID, SHAPE, 'EDGIS.SECUGCONDUCTOR' , CIRCUITID FROM EDGIS.ZZ_MV_SECUGCONDUCTOR WHERE SDE.ST_ISEMPTY(SHAPE) = 0 );
COMMIT;

--Logging
INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
VALUES (SYSDATE,'I','Inserted values in  PGEDATA.ED08LINE_T');
COMMIT;

--Create Index
--EXECUTE IMMEDIATE 'CREATE INDEX EDGISBO.INX_GUID ON EDGISBO.TEMPED08Line (GLOBALID) NOLOGGING TABLESPACE ARCFM_IX PCTFREE 0 INITRANS 4 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )';
--Create Spatial Index
--EXECUTE IMMEDIATE 'CREATE INDEX "EDGISBO"."ED08LSHAP" ON EDGISBO.TEMPED08Line ("SHAPE") INDEXTYPE IS SDE.ST_SPATIAL_INDEX  PARAMETERS (''ST_GRIDS= 1997.62764460089 ST_SRID = 2 ST_COMMIT_ROWS = 10000 TABLESPACE ARCFM_SPATIAL PCTFREE 0 INITRANS 4 STORAGE (FREELISTS 4 MINEXTENTS 1 PCTINCREASE 0) '')';

--Logging
--INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
--VALUES (SYSDATE,'I','Created EDGISBO.TEMPED08Line Indexes');
--COMMIT;

--Drop EDGISBO.TEMPED08STRUCT Table
--EXECUTE IMMEDIATE 'DROP TABLE EDGISBO.TEMPED08STRUCT';
EXECUTE IMMEDIATE 'TRUNCATE TABLE PGEDATA.ED08STRUCT_T';
--Logging
INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
VALUES (SYSDATE,'I','TRUNCATED TABLE PGEDATA.ED08STRUCT_T');
COMMIT;

--Logging
--INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
--VALUES (SYSDATE,'I','Dropped EDGISBO.TEMPED08STRUCT Table');
--COMMIT;

--Create EDGISBO.TempED08Struct Table
Insert into  PGEDATA.ED08STRUCT_T (OBJECTID,GLOBALID, FEATOBJECTID, STRUCTURETYPE, INSTALLJOBNUMBER, REGION, SAPEQUIPID, SUBTYPECD, SHAPE, NAME)
(SELECT  sde.gdb_util.next_rowid('PGEDATA', 'ED08STRUCT_T'),GLOBALID, OBJECTID, 'Support Structures', INSTALLJOBNUMBER, REGION, SAPEQUIPID, SUBTYPECD, SHAPE, 'EDGIS.SUPPORTSTRUCTURE' FROM EDGIS.ZZ_MV_SUPPORTSTRUCTURE WHERE SHAPE IS NOT NULL AND STATUS = '5' AND sde.st_isempty(shape) = 0 UNION
SELECT  sde.gdb_util.next_rowid('PGEDATA', 'ED08STRUCT_T'),GLOBALID, OBJECTID, 'Subsurface Structure', INSTALLJOBNUMBER, REGION, SAPEQUIPID, SUBTYPECD, SHAPE, 'EDGIS.SUBSURFACESTRUCTURE' FROM EDGIS.ZZ_MV_SUBSURFACESTRUCTURE WHERE SHAPE IS NOT NULL AND STATUS = '5' AND sde.st_isempty(shape) = 0  UNION
SELECT  sde.gdb_util.next_rowid('PGEDATA', 'ED08STRUCT_T'),GLOBALID, OBJECTID, 'Padmount Structure', INSTALLJOBNUMBER, REGION, SAPEQUIPID, SUBTYPECD, SHAPE, 'EDGIS.PADMOUNTSTRUCTURE' FROM EDGIS.ZZ_MV_PADMOUNTSTRUCTURE WHERE SHAPE IS NOT NULL AND STATUS = '5' AND SDE.ST_ISEMPTY(SHAPE) = 0 );

--Logging
INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
VALUES (SYSDATE,'I','Inserted values in  PGEDATA.ED08STRUCT_T Table');
COMMIT;

--Create Index
--EXECUTE IMMEDIATE 'CREATE INDEX EDGISBO.INX_SGUID ON EDGISBO.TempED08Struct (GLOBALID) NOLOGGING TABLESPACE ARCFM_IX PCTFREE 0 INITRANS 4 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT )';
--Create Spatial Index
--EXECUTE IMMEDIATE 'CREATE INDEX "EDGISBO"."ED08SSHAPE" ON EDGISBO.TEMPED08STRUCT ("SHAPE") INDEXTYPE IS SDE.ST_SPATIAL_INDEX  PARAMETERS (''ST_GRIDS= 1997.62764460089 ST_SRID = 2 ST_COMMIT_ROWS = 10000 TABLESPACE ARCFM_SPATIAL PCTFREE 0 INITRANS 4 STORAGE (FREELISTS 4 MINEXTENTS 1 PCTINCREASE 0) '')';

--Logging
--INSERT INTO EDGISBO.ED08LOG (DATETIME,TYPE,LOGGER)
--VALUES (SYSDATE,'I','Created EDGISBO.TempED08Struct Indexes');
--COMMIT;

END ED008_PrepareTempTables;
