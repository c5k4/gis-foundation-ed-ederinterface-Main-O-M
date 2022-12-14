
DROP TABLE PGE_CIRCUITTOMAPNUM;
DROP TABLE WORKORDERSTRUCTURE;
DROP TABLE WORKORDERSTRUCTURE_PREV;
DROP TABLE WORKORDERSTRUCTURE_TEMP;
DROP TABLE TAXREPORTSTAGINGSTATUS;
DROP TABLE PGE_CIRCUITTOMAPNUM_TEMP;
DROP TABLE WORKORDERSTRUCTURE_STAGING; 

CREATE TABLE PGE_CIRCUITTOMAPNUM
(CIRCUITID NVARCHAR2(9),
MAPSCALE NUMBER,
MAPGRIDNUM NVARCHAR2(10));

CREATE TABLE PGE_CIRCUITTOMAPNUM_TEMP
(CIRCUITID NVARCHAR2(9),
MAPSCALE NUMBER,
MAPGRIDNUM NVARCHAR2(10));

CREATE TABLE WORKORDERSTRUCTURE
(STRUCTUREGLOBALID CHAR(38), 
STRUCTUREOBJECTID NUMBER(38), 
STRUCTURETYPE VARCHAR2(150 CHAR), 
STRUCTURESUBTYPE VARCHAR2(150 CHAR), 
CIRCUITID VARCHAR2(9), 
WORKORDER VARCHAR2(150 CHAR), 
REGION VARCHAR2(50 CHAR),
SAPEQUIPID VARCHAR2(50 CHAR)
);

CREATE TABLE WORKORDERSTRUCTURE_PREV
(STRUCTUREGLOBALID CHAR(38), 
STRUCTUREOBJECTID NUMBER(38), 
STRUCTURETYPE VARCHAR2(150 CHAR), 
STRUCTURESUBTYPE VARCHAR2(150 CHAR), 
CIRCUITID VARCHAR2(9), 
WORKORDER VARCHAR2(150 CHAR), 
REGION VARCHAR2(50 CHAR),
SAPEQUIPID VARCHAR2(50 CHAR)
);
   
CREATE TABLE WORKORDERSTRUCTURE_TEMP
(STRUCTUREGLOBALID CHAR(38), 
STRUCTUREOBJECTID NUMBER(38), 
STRUCTURETYPE VARCHAR2(150 CHAR), 
STRUCTURESUBTYPE VARCHAR2(150 CHAR), 
CIRCUITID VARCHAR2(9), 
WORKORDER VARCHAR2(150 CHAR), 
REGION VARCHAR2(50 CHAR),
SAPEQUIPID VARCHAR2(50 CHAR)
);

CREATE TABLE WORKORDERSTRUCTURE_STAGING 
(STRUCTUREGLOBALID CHAR(38), 
STRUCTUREOBJECTID NUMBER(38), 
STRUCTURETYPE VARCHAR2(150 CHAR), 
STRUCTURESUBTYPE VARCHAR2(150 CHAR), 
CIRCUITID VARCHAR2(9), 
WORKORDER VARCHAR2(150 CHAR), 
REGION VARCHAR2(50 CHAR),
SAPEQUIPID VARCHAR2(50 CHAR)
);


GRANT SELECT ON EDGISBO.WORKORDERSTRUCTURE_STAGING TO EDGIS;
GRANT SELECT ON EDGISBO.WORKORDERSTRUCTURE  TO EDGIS;

CREATE TABLE TAXREPORTSTAGINGSTATUS
("ISTABLELOCKED" NUMBER(1,0) DEFAULT 0 NOT NULL ENABLE, 
"STATUS" VARCHAR2(100 CHAR)
);

create or replace 
PROCEDURE TruncateED08TempStructTable AS
BEGIN
	execute immediate 'Truncate table edgisbo.WORKORDERSTRUCTURE_TEMP'; 
END TruncateED08TempStructTable ;
/

create or replace 
PROCEDURE TRUN_CIRCTOMAPNUMTEMPTABLE AS
BEGIN
	execute immediate 'Truncate table edgisbo.PGE_CIRCUITTOMAPNUM_TEMP'; 
END TRUN_CIRCTOMAPNUMTEMPTABLE ;
/

INSERT INTO EDGISBO.TAXREPORTSTAGINGSTATUS 
SELECT 0, 'NA' FROM DUAL WHERE NOT EXISTS (SELECT COUNT(*)  FROM EDGISBO.TAXREPORTSTAGINGSTATUS HAVING COUNT(*)>0);
COMMIT;

create or replace PROCEDURE ED08RunCompleted AS
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE EDGISBO.WORKORDERSTRUCTURE_PREV';
    INSERT INTO EDGISBO.WORKORDERSTRUCTURE_PREV (SELECT * FROM EDGISBO.WORKORDERSTRUCTURE);
    EXECUTE IMMEDIATE 'TRUNCATE TABLE EDGISBO.WORKORDERSTRUCTURE';
    INSERT INTO EDGISBO.WORKORDERSTRUCTURE (SELECT * FROM EDGISBO.WORKORDERSTRUCTURE_TEMP);
    commit;
END ED08RunCompleted ;
/

create or replace PROCEDURE ED08GridRunCompleted AS
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE EDGISBO.PGE_CIRCUITTOMAPNUM';
    INSERT INTO EDGISBO.PGE_CIRCUITTOMAPNUM (SELECT * FROM EDGISBO.PGE_CIRCUITTOMAPNUM_TEMP);
    commit;
END ED08GridRunCompleted ;
/

create or replace PROCEDURE PopulateTaxReportTable
IS
TYPE WorkStructureTableType IS TABLE OF workorderstructure_staging%ROWTYPE;
workstructureTable$ WorkStructureTableType;
BEGIN
   SELECT * BULK COLLECT INTO workstructureTable$
     FROM workorderstructure_staging;
     FORALL x in workstructureTable$.First..workstructureTable$.Last
      INSERT INTO workorderstructure VALUES workstructureTable$(x);   
     commit;
END;
/

create or replace procedure TruncateTaxReportTables(tablename IN VARCHAR2)
AS
BEGIN
IF (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE') OR (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE_STAGING') THEN
   execute immediate 'TRUNCATE TABLE ' || tablename; 
END IF;
END;
/

GRANT SELECT ON EDGISBO.WORKORDERSTRUCTURE_PREV TO EDGIS;
GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE_PREV TO GIS_I;
GRANT SELECT ON EDGISBO.WORKORDERSTRUCTURE  TO EDGIS;
GRANT SELECT ON EDGISBO.TAXREPORTSTAGINGSTATUS TO EDGIS;
GRANT UPDATE ON EDGISBO.TAXREPORTSTAGINGSTATUS TO EDGIS;
GRANT ALL ON EDGISBO.TAXREPORTSTAGINGSTATUS TO GIS_I;
GRANT ALL ON EDGISBO.TAXREPORTSTAGINGSTATUS TO EDGIS;

GRANT EXECUTE ON EDGISBO.TruncateED08TempStructTable TO EDGIS;
GRANT EXECUTE ON EDGISBO.TRUN_CIRCTOMAPNUMTEMPTABLE TO EDGIS;
GRANT EXECUTE ON EDGISBO.ED08GridRunCompleted TO EDGIS;
GRANT EXECUTE ON EDGISBO.ED08RunCompleted TO EDGIS;
GRANT EXECUTE ON EDGISBO.ED08RunCompleted TO GIS_I;
GRANT EXECUTE ON EDGISBO.TruncateTaxReportTables TO EDGIS;
GRANT EXECUTE ON EDGISBO.PopulateTaxReportTable TO EDGIS;
GRANT EXECUTE ON EDGISBO.TruncateTaxReportTables TO GIS_I;
GRANT EXECUTE ON EDGISBO.PopulateTaxReportTable TO GIS_I;

GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE_TEMP TO GIS_I;
GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE_TEMP TO GIS_I;
GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE_TEMP TO EDGIS;
GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE_TEMP TO EDGIS;
GRANT ALL ON EDGISBO.PGE_CIRCUITTOMAPNUM_TEMP TO GIS_I;
GRANT ALL ON EDGISBO.PGE_CIRCUITTOMAPNUM_TEMP TO EDGIS;
GRANT ALL ON EDGISBO.PGE_CIRCUITTOMAPNUM TO GIS_I;
GRANT ALL ON EDGISBO.PGE_CIRCUITTOMAPNUM TO EDGIS;

GRANT ALL ON EDGISBO.WORKORDERSTRUCTURE  TO GIS_I;
