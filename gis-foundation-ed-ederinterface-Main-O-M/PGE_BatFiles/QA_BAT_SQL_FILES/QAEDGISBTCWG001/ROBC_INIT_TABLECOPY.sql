
TRUNCATE TABLE EDGIS.ROBC;
TRUNCATE TABLE EDGIS.PARTIALCURTAILPOINT;


Insert into EDGIS.PARTIALCURTAILPOINT (SELECT * FROM EDGIS.PARTIALCURTAILPOINT@dbLinkEDER );

Insert into EDGIS.ROBC (SELECT * FROM EDGIS.ROBC@dbLinkEDER );
commit ;

