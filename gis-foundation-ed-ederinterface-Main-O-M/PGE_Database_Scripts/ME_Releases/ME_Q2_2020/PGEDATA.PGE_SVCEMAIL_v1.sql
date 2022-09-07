EXEC SDE.VERSION_UTIL.SET_CURRENT_VERSION('SDE.DEFAULT')
Prompt drop TABLE PGEDATA.PGE_SVCEMAIL;
DROP TABLE PGEDATA.PGE_SVCEMAIL CASCADE CONSTRAINTS
/

Prompt Table PGEDATA.PGE_SVCEMAIL;
--
-- PGE_SVCEMAIL  (Table) 
--
--CREATE TABLE PGEDATA.PGE_SVCEMAIL ( DEVICETYPE VARCHAR2(30 BYTE) NOT NULL, GLOBALID VARCHAR2(30 BYTE) NOT NULL, SUBTYPE VARCHAR2(20 BYTE), OPERATINGNUMBER NVARCHAR2(15), CIRCUITID VARCHAR2(10 BYTE), DIVISION VARCHAR2(20 BYTE), LOCATION NVARCHAR2(80), SUPERVISORYCONTROL CHAR(1 BYTE) NOT NULL, STATUS INTEGER NOT NULL, MODIFIEDON DATE NOT NULL, EMAILED CHAR(1 BYTE) ) TABLESPACE PGE PCTUSED 0 PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( PCTINCREASE 0 BUFFER_POOL DEFAULT ) LOGGING NOCOMPRESS NOCACHE MONITORING

CREATE TABLE PGEDATA.PGE_SVCEMAIL AS 
SELECT 'SWITCH' DEVICETYPE, T.GLOBALID GLOBALID, S.DESCRIPTION SUBTYPE, T.OPERATINGNUMBER OPERATINGNUMBER, T.CIRCUITID CIRCUITID, 
C.SUBSTATIONNAME || ' ' || C.CIRCUITNAME CIRCUITNAME,
DECODE(T.DIVISION,17,'Central Coast',16,'De Anza',9,'Diablo',6,'East Bay',12,'Fresno',20,'Humboldt',13,'Kern',18,'Los Padres',14,'Mission',5,'North Bay',2,'North Valley',8,'Peninsula',3,'Sacramento',7,'San Francisco',15,'San Jose',4,'Sierra',19,'Skyline',1,'Sonoma',10,'Stockton',11,'Yosemite') DIVISION,
'Lat: '|| ROUND(SDE.ST_X(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) || ', Long: ' || ROUND(SDE.ST_Y(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) LOCATION,
T.SUPERVISORYCONTROL SUPERVISORYCONTROL,
T.STATUS STATUS,
NVL(T.DATEMODIFIED, T.DATECREATED) MODIFIEDON,
TO_DATE('04/30/2020','MM/DD/YYYY') EMAILEDON
FROM
EDGIS.ZZ_MV_SWITCH T,
EDGIS.PGE_SUBTYPECD S,
EDGIS.ZZ_MV_CIRCUITSOURCE C
WHERE
T.STATUS = 5
AND
T.SUPERVISORYCONTROL = 'Y'
AND
UPPER(S.FEATURECLASS) = 'EDGIS.SWITCH'
AND
S.SUBTYPECD = T.SUBTYPECD
ANDT.CIRCUITID = C.CIRCUITID
UNION
SELECT 'DYNAMICPROTECTIVEDEVICE' DEVICETYPE, T.GLOBALID GLOBALID, S.DESCRIPTION SUBTYPE, T.OPERATINGNUMBER OPERATINGNUMBER, T.CIRCUITID CIRCUITID, 
C.SUBSTATIONNAME || ' ' || C.CIRCUITNAME CIRCUITNAME,
DECODE(T.DIVISION,17,'Central Coast',16,'De Anza',9,'Diablo',6,'East Bay',12,'Fresno',20,'Humboldt',13,'Kern',18,'Los Padres',14,'Mission',5,'North Bay',2,'North Valley',8,'Peninsula',3,'Sacramento',7,'San Francisco',15,'San Jose',4,'Sierra',19,'Skyline',1,'Sonoma',10,'Stockton',11,'Yosemite') DIVISION,
'Lat: '|| ROUND(SDE.ST_X(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) || ', Long: ' || ROUND(SDE.ST_Y(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) LOCATION,
T.SUPERVISORYCONTROL SUPERVISORYCONTROL,
T.STATUS STATUS,
NVL(T.DATEMODIFIED, T.DATECREATED) MODIFIEDON,
TO_DATE('04/30/2020','MM/DD/YYYY') EMAILEDON
FROM
EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE T,
EDGIS.PGE_SUBTYPECD S,
EDGIS.ZZ_MV_CIRCUITSOURCE C
WHERE
T.STATUS = 5
AND
T.SUPERVISORYCONTROL = 'Y'
AND
UPPER(S.FEATURECLASS) = 'EDGIS.DYNAMICPROTECTIVEDEVICE'
AND
S.SUBTYPECD = T.SUBTYPECD
AND
T.CIRCUITID = C.CIRCUITID
UNION
SELECT 'OPENPOINT' DEVICETYPE, T.GLOBALID GLOBALID, S.DESCRIPTION SUBTYPE, T.OPERATINGNUMBER OPERATINGNUMBER, T.CIRCUITID CIRCUITID, 
C.SUBSTATIONNAME || ' ' || C.CIRCUITNAME CIRCUITNAME,
DECODE(T.DIVISION,17,'Central Coast',16,'De Anza',9,'Diablo',6,'East Bay',12,'Fresno',20,'Humboldt',13,'Kern',18,'Los Padres',14,'Mission',5,'North Bay',2,'North Valley',8,'Peninsula',3,'Sacramento',7,'San Francisco',15,'San Jose',4,'Sierra',19,'Skyline',1,'Sonoma',10,'Stockton',11,'Yosemite') DIVISION,
'Lat: '|| ROUND(SDE.ST_X(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) || ', Long: ' || ROUND(SDE.ST_Y(SDE.ST_TRANSFORM(T.SHAPE, 4269)), 5) LOCATION,
T.SUPERVISORYCONTROL SUPERVISORYCONTROL,
T.STATUS STATUS,
NVL(T.DATEMODIFIED, T.DATECREATED) MODIFIEDON,
TO_DATE('04/30/2020','MM/DD/YYYY') EMAILEDON
FROM
EDGIS.ZZ_MV_OPENPOINT T,
EDGIS.PGE_SUBTYPECD S,
EDGIS.ZZ_MV_CIRCUITSOURCE C
WHERE
T.STATUS = 5
AND
T.SUPERVISORYCONTROL = 'Y'
AND
UPPER(S.FEATURECLASS) = 'EDGIS.OPENPOINT'
AND
S.SUBTYPECD = T.SUBTYPECD
AND
T.CIRCUITID = C.CIRCUITID
/

ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(SUBTYPE VARCHAR2(250 BYTE))
/


ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(OPERATINGNUMBER NVARCHAR2(15))
/


ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(CIRCUITID NVARCHAR2(15))
/


ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(DIVISION VARCHAR2(20 BYTE))
/

ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(CIRCUITNAME NVARCHAR2(80))
/

ALTER TABLE PGEDATA.PGE_SVCEMAIL
MODIFY(LOCATION VARCHAR2(100 BYTE))
/

Prompt Index PGEDATA.IDX_SVCE_GUIDCLASS;
--
-- IDX_SVCE_GUIDCLASS  (Index) 
--
CREATE INDEX PGEDATA.IDX_SVCE_GUIDCLASS ON PGEDATA.PGE_SVCEMAIL (GLOBALID, DEVICETYPE) LOGGING TABLESPACE PGE PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( PCTINCREASE 0 BUFFER_POOL DEFAULT )
/

DELETE PGEDATA.PGE_SVCEMAIL WHERE MODIFIEDON > TO_DATE('04/30/2020','MM/DD/YYYY');
COMMIT;
