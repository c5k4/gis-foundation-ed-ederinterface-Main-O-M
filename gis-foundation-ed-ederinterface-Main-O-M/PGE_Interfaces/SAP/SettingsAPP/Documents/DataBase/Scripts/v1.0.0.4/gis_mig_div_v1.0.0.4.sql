---------- Version : 1.0.0.4  ----------------


--------------------------------------------------------
--  File created - Friday-July-11-2014   
--------------------------------------------------------

spool gis_mig_div_v1.0.0.4.log

REM INSERTING into EDSETT.GIS_MIG_DIV
SET DEFINE OFF;
Insert into EDSETT.GIS_MIG_DIV (DIV_#,DIV_NAME) values (13,'Peninsula');
Insert into EDSETT.GIS_MIG_DIV (DIV_#,DIV_NAME) values (12,'Fresno');
Insert into EDSETT.GIS_MIG_DIV (DIV_#,DIV_NAME) values (11,'Yosmeti');
Insert into EDSETT.GIS_MIG_DIV (DIV_#,DIV_NAME) values (10,'Stockton');
Insert into EDSETT.GIS_MIG_DIV (DIV_#,DIV_NAME) values (null,null);

commit;

spool off