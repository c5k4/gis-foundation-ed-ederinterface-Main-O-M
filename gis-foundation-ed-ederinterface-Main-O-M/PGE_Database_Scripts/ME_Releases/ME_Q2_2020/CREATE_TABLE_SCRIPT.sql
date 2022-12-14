spool "D:\Temp\CREATE_TABLE_ED.txt"

CREATE TABLE PGEDATA.SESSIONDATA (
ID NUMBER(10) PRIMARY KEY,
SESSIONID NUMBER(38),
SESSIONNAME VARCHAR2(64),
SESSIONDESCRIPTION VARCHAR2(255),
CREATEDBY VARCHAR2(32),
CREATEDON DATE,
POSTEDBY VARCHAR2(32),
POSTEDON  DATE,
EDITEDDATA CLOB
);


COMMIT;


Grant select on PGEDATA.SESSIONDATA to PGEDATA, SDE, EDGISBO, DMSSTAGING, GIS_I_WRITE, GISINTERFACE, GIS_INTERFACE, GIS_I, SDE_VIEWER, SDE_EDITOR, DAT_EDITOR, MM_ADMIN, SELECT_CATALOG_ROLE;

COMMIT;

/

CREATE SEQUENCE  "PGEDATA"."SESSIONDATA_SEQ"  MINVALUE 1 MAXVALUE 9999999999 INCREMENT BY 1 START WITH 1 NOCACHE  NOCYCLE ;
commit;

/

CREATE TABLE "PGEDATA"."SESSIONCURRENTOWNER"  (
	"SESSIONID" NUMBER(38,0), 
	"CURRENTOWNER" VARCHAR2(32 BYTE), 
	"DATETIME" DATE
   );
commit;
Grant SELECT,UPDATE,DELETE,INSERT on PGEDATA.SESSIONCURRENTOWNER to PGEDATA, SDE, EDGISBO, DMSSTAGING, GIS_I_WRITE, GISINTERFACE, GIS_INTERFACE, GIS_I, SDE_VIEWER, SDE_EDITOR, DAT_EDITOR, MM_ADMIN, SELECT_CATALOG_ROLE;

commit;

/

 CREATE TABLE "PGEDATA"."TRACKED_FEATURES" 
   (	"FEATURECLASSNAME" VARCHAR2(64 BYTE), 
	"OBJECTTYPE" VARCHAR2(20 BYTE), 
	"FEATURESUBTYPECD" VARCHAR2(50 BYTE), 
	"ATTRIBUTES" VARCHAR2(1024 BYTE)
   );
COMMIT;

INSERT INTO PGEDATA.TRACKED_FEATURES VALUES ('EDGIS.TRANSFORMER', 'FEATURECLASS', '1', 'PHASEDESIGNATION,LATITUDE,LONGITUDE');
INSERT INTO PGEDATA.TRACKED_FEATURES VALUES ('EDGIS.SWITCH', 'FEATURECLASS', '2', 'CCRATING');
INSERT INTO PGEDATA.TRACKED_FEATURES VALUES ('EDGIS.FUSE', 'FEATURECLASS', '1', 'LINKRATING,LINKTYPE,PARTNUMBER,PHASEDESIGNATION');

INSERT INTO PGEDATA.TRACKED_FEATURES VALUES ('EDGIS.OPENPOINT', 'FEATURECLASS', '1,2', 'RATEDAMPS');

INSERT INTO PGEDATA.TRACKED_FEATURES VALUES ('EDGIS.TRANSFORMERUNIT', 'TABLE', '1', 'RATEDKVA,PHASEDESIGNATION');

COMMIT;

Grant select on PGEDATA.TRACKED_FEATURES to PGEDATA, SDE, EDGISBO, DMSSTAGING, GIS_I_WRITE, GISINTERFACE, GIS_INTERFACE, GIS_I, SDE_VIEWER, SDE_EDITOR, DAT_EDITOR, MM_ADMIN, SELECT_CATALOG_ROLE;

COMMIT;

/

spool off;