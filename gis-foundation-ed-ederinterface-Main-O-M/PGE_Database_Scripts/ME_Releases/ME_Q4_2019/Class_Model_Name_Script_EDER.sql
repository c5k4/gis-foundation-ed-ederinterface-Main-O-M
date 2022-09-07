
spool "D:\Temp\EDER_Cls_Model_MN.txt"
SET SERVEROUTPUT ON
SET DEFINE OFF;

declare 

max_oid NVARCHAR2(64) :=NULL;
   
    
begin
  
DBMS_OUTPUT.PUT_LINE('EDER : Process started');
select max(oid) into max_oid from sde.MM_CLASS_MODELNAMES;
--Delete first if Model name already exist
Delete from sde.MM_CLASS_MODELNAMES where MODELNAME = 'PGE_CUSTOMDISPLAYFIELD' and OBJECTCLASSNAME in 
(
'EDGIS.JOBHISTORYNOTE',
'EDGIS.STREETLIGHT',
'EDGIS.SERVICELOCATION',
'EDGIS.SUPPORTSTRUCTURE',
'EDGIS.PADMOUNTSTRUCTURE',
'EDGIS.SUBSURFACESTRUCTURE',
'EDGIS.ANCHOR',
'EDGIS.CONDUITSYSTEM',
'EDGIS.TRANSFORMER',
'EDGIS.CROSSSECTION10ANNO',
'EDGIS.DEACTIVATEDELECLINESEGANNO',
'EDGIS.DEVICEGROUP50ANNO',
'EDGIS.DEVICEGROUPSCHEM100ANNO',
'EDGIS.FAULTINDICATORSCHEM500ANNO',
'EDGIS.FUSEANNO',
'EDGIS.NETWORKPROTECTORANNO',
'EDGIS.NEUTRALCONDUCTOR50ANNO',
'EDGIS.SUBSTATION50ANNO',
'EDGIS.SUBSTATIONANNO',
'EDGIS.VOLTAGEREGULATORSCHEM100ANNO',
'EDGIS.CROSSSECTIONANNO',
'EDGIS.FUSESCHEM500ANNO',
'EDGIS.PRIGENERATIONSCHEM500ANNO',
'EDGIS.SECONDARYGENERATION50ANNO',
'EDGIS.STEPDOWNANNO',
'EDGIS.STEPDOWNSCHEM500ANNO',
'EDGIS.STREETLIGHT50ANNO',
'EDGIS.TRANSFORMERSCHEM100ANNO',
'EDGIS.ULSPRIUGCONDUCTORANNO',
'EDGIS.VOLTAGEREGULATORSCHEM500ANNO',
'EDGIS.CAPACITORBANKANNO',
'EDGIS.CONDUITSYSTEMANNO',
'EDGIS.DCRECTIFIER50ANNO',
'EDGIS.ELECSTITCHPOINTSCHEM100ANNO',
'EDGIS.FUSESCHEM100ANNO',
'EDGIS.JOBNUMBERANNO',
'EDGIS.NEUTRALCONDUCTORANNO',
'EDGIS.SECONDARYGENERATIONANNO',
'EDGIS.STREETLIGHTANNO',
'EDGIS.SUBSURFACESTRUCTURE50ANNO',
'EDGIS.VAULTPOLY50ANNO',
'EDGIS.DELIVERYPOINT50ANNO',
'EDGIS.PRIOHCONDUCTORANNO',
'EDGIS.SECUGCONDUCTORANNO',
'EDGIS.SWITCHSCHEM500ANNO',
'EDGIS.SECUGCONDUCTOR50ANNO',
'EDGIS.CONDUITSYSTEM50ANNO',
'EDGIS.CROSSSECTION50ANNO',
'EDGIS.CUSTAGREENUMBER50ANNO',
'EDGIS.DCDEVICEANNO',
'EDGIS.FAULTINDICATOR50ANNO',
'EDGIS.FAULTINDICATORANNO',
'EDGIS.FAULTINDICATORSCHEM100ANNO',
'EDGIS.JOBNUMBER50ANNO',
'EDGIS.NETWORKPROTECTORSCHEM500ANNO',
'EDGIS.PRIMARYGENERATION50ANNO',
'EDGIS.PRIUGCONDUCTOR50ANNO',
'EDGIS.STEPDOWN50ANNO',
'EDGIS.SWITCH50ANNO',
'EDGIS.VOLTAGEREGULATORANNO',
'EDGIS.CAPACITORBANK50ANNO',
'EDGIS.DCDEVICE50ANNO',
'EDGIS.DEACTIVATEDELECLINESEG50ANNO',
'EDGIS.ELECSTITCHPOINTSCHEM500ANNO',
'EDGIS.NETWORKPROTECTOR50ANNO',
'EDGIS.PRIGENERATIONSCHEM100ANNO',
'EDGIS.PRIMARYGENERATIONANNO',
'EDGIS.PRIUGCONDUCTORANNO',
'EDGIS.SECOHCONDUCTORANNO',
'EDGIS.TRANSFORMER50ANNO',
'EDGIS.TRANSFORMERSCHEM500ANNO',
'EDGIS.ULSPRIUGCONDUCTOR50ANNO',
'EDGIS.VOLTAGEREGULATOR50ANNO',
'EDGIS.CUSTAGREENUMBERANNO',
'EDGIS.DCCONDUCTOR50ANNO',
'EDGIS.DELIVERYPOINTANNO',
'EDGIS.DEVICEGROUPSCHEM500ANNO',
'EDGIS.DISTBUSBAR50ANNO',
'EDGIS.DYNAMICPROTECTIVEDEVICEANNO',
'EDGIS.DYNPROTDEVSCHEM100ANNO',
'EDGIS.DYNPROTDEVSCHEM500ANNO',
'EDGIS.PRIOHCONDUCTOR50ANNO',
'EDGIS.SECOHCONDUCTOR50ANNO',
'EDGIS.SUPPORTSTRUCTURE50ANNO',
'EDGIS.SUPPORTSTRUCTUREANNO',
'EDGIS.SWITCHANNO',
'EDGIS.SWITCHSCHEM100ANNO',
'EDGIS.TRANSFORMERANNO',
'EDGIS.CAPACITORBANKSCHEM100ANNO',
'EDGIS.CAPACITORBANKSCHEM500ANNO',
'EDGIS.DCRECTIFIERANNO',
'EDGIS.DEVICEGROUPANNO',
'EDGIS.DYNPROTDEVICE50ANNO',
'EDGIS.FUSE50ANNO',
'EDGIS.STEPDOWNSCHEM100ANNO',
'EDGIS.SUBSTATIONSCHEMANNO',
'EDGIS.SUBSURFACESTRUCTUREANNO',
'EDGIS.DUCTANNOTATION',
'EDGIS.DUCTNOTEANNO',
'EDGIS.SchemStreetAnno100',
'EDGIS.SchemStreetAnno500'
);


--Now Inserting Model Name
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+1,'EDGIS.ELECTRICDATASET','EDGIS.JOBHISTORYNOTE','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+2,'EDGIS.ELECTRICDATASET','EDGIS.STREETLIGHT','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+3,'EDGIS.ELECTRICDATASET','EDGIS.SERVICELOCATION','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+4,'EDGIS.ELECTRICDATASET','EDGIS.SUPPORTSTRUCTURE','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+5,'EDGIS.ELECTRICDATASET','EDGIS.PADMOUNTSTRUCTURE','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+6,'EDGIS.ELECTRICDATASET','EDGIS.SUBSURFACESTRUCTURE','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+7,'EDGIS.ELECTRICDATASET','EDGIS.ANCHOR','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+8,'EDGIS.ELECTRICDATASET','EDGIS.CONDUITSYSTEM','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+9,'EDGIS.ELECTRICDATASET','EDGIS.TRANSFORMER','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+10,'EDGIS.ELECTRICDATASET','EDGIS.CROSSSECTION10ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+11,'EDGIS.ELECTRICDATASET','EDGIS.DEACTIVATEDELECLINESEGANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+12,'EDGIS.ELECTRICDATASET','EDGIS.DEVICEGROUP50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+13,'EDGIS.ELECTRICDATASET','EDGIS.DEVICEGROUPSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+14,'EDGIS.ELECTRICDATASET','EDGIS.FAULTINDICATORSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+15,'EDGIS.ELECTRICDATASET','EDGIS.FUSEANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+16,'EDGIS.ELECTRICDATASET','EDGIS.NETWORKPROTECTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+17,'EDGIS.ELECTRICDATASET','EDGIS.NEUTRALCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+18,'EDGIS.ELECTRICDATASET','EDGIS.SUBSTATION50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+19,'EDGIS.ELECTRICDATASET','EDGIS.SUBSTATIONANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+20,'EDGIS.ELECTRICDATASET','EDGIS.VOLTAGEREGULATORSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+21,'EDGIS.ELECTRICDATASET','EDGIS.CROSSSECTIONANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+22,'EDGIS.ELECTRICDATASET','EDGIS.FUSESCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+23,'EDGIS.ELECTRICDATASET','EDGIS.PRIGENERATIONSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+24,'EDGIS.ELECTRICDATASET','EDGIS.SECONDARYGENERATION50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+25,'EDGIS.ELECTRICDATASET','EDGIS.STEPDOWNANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+26,'EDGIS.ELECTRICDATASET','EDGIS.STEPDOWNSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+27,'EDGIS.ELECTRICDATASET','EDGIS.STREETLIGHT50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+28,'EDGIS.ELECTRICDATASET','EDGIS.TRANSFORMERSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+29,'EDGIS.ELECTRICDATASET','EDGIS.ULSPRIUGCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+30,'EDGIS.ELECTRICDATASET','EDGIS.VOLTAGEREGULATORSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+31,'EDGIS.ELECTRICDATASET','EDGIS.CAPACITORBANKANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+32,'EDGIS.ELECTRICDATASET','EDGIS.CONDUITSYSTEMANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+33,'EDGIS.ELECTRICDATASET','EDGIS.DCRECTIFIER50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+34,'EDGIS.ELECTRICDATASET','EDGIS.ELECSTITCHPOINTSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+35,'EDGIS.ELECTRICDATASET','EDGIS.FUSESCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+36,'EDGIS.ELECTRICDATASET','EDGIS.JOBNUMBERANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+37,'EDGIS.ELECTRICDATASET','EDGIS.NEUTRALCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+38,'EDGIS.ELECTRICDATASET','EDGIS.SECONDARYGENERATIONANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+39,'EDGIS.ELECTRICDATASET','EDGIS.STREETLIGHTANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+40,'EDGIS.ELECTRICDATASET','EDGIS.SUBSURFACESTRUCTURE50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+41,'EDGIS.ELECTRICDATASET','EDGIS.VAULTPOLY50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+42,'EDGIS.ELECTRICDATASET','EDGIS.DELIVERYPOINT50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+43,'EDGIS.ELECTRICDATASET','EDGIS.PRIOHCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+44,'EDGIS.ELECTRICDATASET','EDGIS.SECUGCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+45,'EDGIS.ELECTRICDATASET','EDGIS.SWITCHSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+46,'EDGIS.ELECTRICDATASET','EDGIS.SECUGCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+47,'EDGIS.ELECTRICDATASET','EDGIS.CONDUITSYSTEM50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+48,'EDGIS.ELECTRICDATASET','EDGIS.CROSSSECTION50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+49,'EDGIS.ELECTRICDATASET','EDGIS.CUSTAGREENUMBER50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+50,'EDGIS.ELECTRICDATASET','EDGIS.DCDEVICEANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+51,'EDGIS.ELECTRICDATASET','EDGIS.FAULTINDICATOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+52,'EDGIS.ELECTRICDATASET','EDGIS.FAULTINDICATORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+53,'EDGIS.ELECTRICDATASET','EDGIS.FAULTINDICATORSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+54,'EDGIS.ELECTRICDATASET','EDGIS.JOBNUMBER50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+55,'EDGIS.ELECTRICDATASET','EDGIS.NETWORKPROTECTORSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+56,'EDGIS.ELECTRICDATASET','EDGIS.PRIMARYGENERATION50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+57,'EDGIS.ELECTRICDATASET','EDGIS.PRIUGCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+58,'EDGIS.ELECTRICDATASET','EDGIS.STEPDOWN50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+59,'EDGIS.ELECTRICDATASET','EDGIS.SWITCH50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+60,'EDGIS.ELECTRICDATASET','EDGIS.VOLTAGEREGULATORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+61,'EDGIS.ELECTRICDATASET','EDGIS.CAPACITORBANK50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+62,'EDGIS.ELECTRICDATASET','EDGIS.DCDEVICE50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+63,'EDGIS.ELECTRICDATASET','EDGIS.DEACTIVATEDELECLINESEG50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+64,'EDGIS.ELECTRICDATASET','EDGIS.ELECSTITCHPOINTSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+65,'EDGIS.ELECTRICDATASET','EDGIS.NETWORKPROTECTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+66,'EDGIS.ELECTRICDATASET','EDGIS.PRIGENERATIONSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+67,'EDGIS.ELECTRICDATASET','EDGIS.PRIMARYGENERATIONANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+68,'EDGIS.ELECTRICDATASET','EDGIS.PRIUGCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+69,'EDGIS.ELECTRICDATASET','EDGIS.SECOHCONDUCTORANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+70,'EDGIS.ELECTRICDATASET','EDGIS.TRANSFORMER50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+71,'EDGIS.ELECTRICDATASET','EDGIS.TRANSFORMERSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+72,'EDGIS.ELECTRICDATASET','EDGIS.ULSPRIUGCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+73,'EDGIS.ELECTRICDATASET','EDGIS.VOLTAGEREGULATOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+74,'EDGIS.ELECTRICDATASET','EDGIS.CUSTAGREENUMBERANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+75,'EDGIS.ELECTRICDATASET','EDGIS.DCCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+76,'EDGIS.ELECTRICDATASET','EDGIS.DELIVERYPOINTANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+77,'EDGIS.ELECTRICDATASET','EDGIS.DEVICEGROUPSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+78,'EDGIS.ELECTRICDATASET','EDGIS.DISTBUSBAR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+79,'EDGIS.ELECTRICDATASET','EDGIS.DYNAMICPROTECTIVEDEVICEANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+80,'EDGIS.ELECTRICDATASET','EDGIS.DYNPROTDEVSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+81,'EDGIS.ELECTRICDATASET','EDGIS.DYNPROTDEVSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+82,'EDGIS.ELECTRICDATASET','EDGIS.PRIOHCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+83,'EDGIS.ELECTRICDATASET','EDGIS.SECOHCONDUCTOR50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+84,'EDGIS.ELECTRICDATASET','EDGIS.SUPPORTSTRUCTURE50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+85,'EDGIS.ELECTRICDATASET','EDGIS.SUPPORTSTRUCTUREANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+86,'EDGIS.ELECTRICDATASET','EDGIS.SWITCHANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+87,'EDGIS.ELECTRICDATASET','EDGIS.SWITCHSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+88,'EDGIS.ELECTRICDATASET','EDGIS.TRANSFORMERANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+89,'EDGIS.ELECTRICDATASET','EDGIS.CAPACITORBANKSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+90,'EDGIS.ELECTRICDATASET','EDGIS.CAPACITORBANKSCHEM500ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+91,'EDGIS.ELECTRICDATASET','EDGIS.DCRECTIFIERANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+92,'EDGIS.ELECTRICDATASET','EDGIS.DEVICEGROUPANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+93,'EDGIS.ELECTRICDATASET','EDGIS.DYNPROTDEVICE50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+94,'EDGIS.ELECTRICDATASET','EDGIS.FUSE50ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+95,'EDGIS.ELECTRICDATASET','EDGIS.STEPDOWNSCHEM100ANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+96,'EDGIS.ELECTRICDATASET','EDGIS.SUBSTATIONSCHEMANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+97,'EDGIS.ELECTRICDATASET','EDGIS.SUBSURFACESTRUCTUREANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+98,'EDGIS.UFMDATASET','EDGIS.DUCTANNOTATION','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+99,'EDGIS.UFMDATASET','EDGIS.DUCTNOTEANNO','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+100,'EDGIS.ELECTRICDATASET','EDGIS.SchemStreetAnno100','0','-1','PGE_CUSTOMDISPLAYFIELD');
Insert into SDE.MM_CLASS_MODELNAMES (OID,MODELDATASETNAME,OBJECTCLASSNAME,SUBTYPEUSED,SUBTYPE,MODELNAME)  values (max_oid+101,'EDGIS.ELECTRICDATASET','EDGIS.SchemStreetAnno500','0','-1','PGE_CUSTOMDISPLAYFIELD');


Commit;


DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');

end;
/
spool off;

