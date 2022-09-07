--REM The Staging Table for Map Number and Co-ordinate Lookup

CREATE TABLE PGE_MAPNUMBERCOORDLUT 
(
  MAPNUMBER NVARCHAR2(15) 
, LEGACYCOORDINATE NVARCHAR2(75) 
, MAPOFFICE NVARCHAR2(25) 
, XMIN NUMBER(15, 2) 
, YMIN NUMBER(15, 2) 
, XMAX NUMBER(15, 2) 
, YMAX NUMBER(15, 2) 
, EXPORTSTATE NUMBER(2, 0) 
, MAPSCALE NUMBER(4, 0) 
, POLYGONGEOMETRY NVARCHAR2(2000) 
, SERVICETOPROCESS NUMBER(2, 0) 
, LASTMODIFIEDDATE DATE 
, MAPID NUMBER(38, 0) 
);

--REM Lookup for Map Office to the Output Folder.
--REM Populate with the folder location for each Map Office

CREATE TABLE PGE_MAPOFFICEFOLDERLUT 
(
  MAPOFFICECODE NVARCHAR2(10)
,  MAPOFFICE NVARCHAR2(25)
, PDFFOLDER NVARCHAR2(500) 
, TIFFFOLDER NVARCHAR2(500) 
);


--REM Create PGE_COORDMXDLUT the look up table for Coordinatesystem and MXD

CREATE TABLE PGE_COORDMXDLUT 
(
  COORDSYSTEM NVARCHAR2(75) 
, PDFMXD NVARCHAR2(200) 
, TIFFMXD NVARCHAR2(200) 
, MAPSCALE NUMBER(4, 0) 
);

--REM Insert data into the PGE_COORDMXDLUT
--REM Replace COORDSYSTEM Value with proper value as populated by Conversion

Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-25.mxd','TIFF-0401-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-50.mxd','TIFF-0401-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-100.mxd','TIFF-0401-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-200.mxd','TIFF-0401-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-250.mxd','TIFF-0401-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California I FIPS 0401','PDF-0401-500.mxd','TIFF-0401-500.mxd',500);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-25.mxd','TIFF-0402-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-50.mxd','TIFF-0402-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-100.mxd','TIFF-0402-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-200.mxd','TIFF-0402-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-250.mxd','TIFF-0402-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California II FIPS 0402','PDF-0402-500.mxd','TIFF-0402-500.mxd',500);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-25.mxd','TIFF-0403-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-50.mxd','TIFF-0403-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-100.mxd','TIFF-0403-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-200.mxd','TIFF-0403-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-250.mxd','TIFF-0403-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California III FIPS 0403','PDF-0403-500.mxd','TIFF-0403-500.mxd',500);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-25.mxd','TIFF-0404-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-50.mxd','TIFF-0404-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-100.mxd','TIFF-0404-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-200.mxd','TIFF-0404-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-250.mxd','TIFF-0404-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California IV FIPS 0404','PDF-0404-500.mxd','TIFF-0404-500.mxd',500);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-25.mxd','TIFF-0405-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-50.mxd','TIFF-0405-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-100.mxd','TIFF-0405-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-200.mxd','TIFF-0405-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-250.mxd','TIFF-0405-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California V FIPS 0405','PDF-0405-500.mxd','TIFF-0405-500.mxd',500);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-25.mxd','TIFF-0406-25.mxd',25);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-50.mxd','TIFF-0406-50.mxd',50);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-100.mxd','TIFF-0406-100.mxd',100);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-200.mxd','TIFF-0406-200.mxd',200);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-250.mxd','TIFF-0406-250.mxd',250);
Insert into PGE_COORDMXDLUT (COORDSYSTEM,PDFMXD,TIFFMXD,MAPSCALE) values ('NAD 1927 StatePlane California VI FIPS 0406','PDF-0406-500.mxd','TIFF-0406-500.mxd',500);


--REM View exposed to GEMS/MET database for them to query

--CREATE OR REPLACE FORCE VIEW PGE_SDO_VIEW AS 
--  SELECT 
--          MP.MAPNUMBER as DWG_NAME,
--          'ELECTRIC_DISTRIBUTION' as DWG_TYPE,
--          --SDO_GEOMETRY(
--          --  2003,
--          --  NULL,
--          --  NULL,
--          --  SDO_ELEM_INFO_ARRAY(1,1003,3),
--          --  SDO_ORDINATE_ARRAY(MP.XMIN,MP.YMIN, MP.XMAX,MP.YMAX)
--          --) as GEOMETRY, 
--		  SDO_UTIL.FROM_WKTGEOMETRY(MP.POLYGONGEOMETRY) as GEOMETRY,
--          mp.LastModifiedDate as GEOMETRY_LAST_UPDATED, 
--          mp.MapID as MAPID,
--          MP.mapoffice as OFFICE,
--	      'ELECTRIC' as DWG_CATEGORY,
--	      mp.MapScale as MAPSCALE,
--	      'PRIMARY' as GEOMETRY_TYPE,
--          'ACTIVE' as STATUS_LEVEL, 
--		  'RASTER' as FILE_TYPE,
--          mp.xmax as TIF_MAX_X, 
--          mp.ymax as TIF_MAX_Y,
--          mp.xmin as TIF_MIN_X,
--          mp.ymin as TIF_MIN_Y,
--          mp.xmax as MAX_X, 
--          mp.ymax as MAX_Y,
--          mp.xmin as MIN_X,
--          mp.ymin as MIN_Y,
--	      mp.legacycoordinate as COORD_ZONE,
--	      mp.legacycoordinate as COORD_SYSTEM
--          FROM edgis.PGE_MAPNUMBERCOORDLUT mp
-- ;

 --REM Grant Permission to MM_ADMIN users to all tables and Read permission on GEMS READONLY Role to PGE_SDO_VIEW
 GRANT ALL ON PGE_MAPNUMBERCOORDLUT TO MM_ADMIN;
 GRANT ALL ON PGE_MAPOFFICEFOLDERLUT TO MM_ADMIN;
 GRANT ALL ON PGE_COORDMXDLUT TO MM_ADMIN;
 --GRANT ALL ON PGE_SDO_VIEW TO MM_ADMIN;
 --GRANT SELECT ON PGE_SDO_VIEW TO &GEMSREADONLYROLE;