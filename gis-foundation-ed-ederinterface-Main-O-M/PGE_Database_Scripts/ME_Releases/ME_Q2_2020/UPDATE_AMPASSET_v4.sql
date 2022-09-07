SET SQLBLANKLINES ON
SPOOL "D:\TEMP\UPDATE_AMPASSET.TXT"
SELECT CURRENT_TIMESTAMP FROM DUAL;
VARIABLE my_version NVARCHAR2(30); 
EXEC :my_version := 'RELEDITOR.UPDATE_AMPASSET'
exec sde.version_user_ddl.delete_version(:my_version)
EXEC sde.version_user_ddl.create_version('SDE.DEFAULT', :my_version, sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'versioned view edit version');
EXEC sde.version_util.set_current_version(:my_version)
EXEC sde.version_user_ddl.edit_version(:my_version,1)
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_DISTBUSBAR SET AMPYEAR = NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_NETWORKPROTECTOR SET AMPYEAR = NVL(NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR),YEAROFMANUFACTURE);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_NEUTRALCONDUCTOR SET AMPYEAR = NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_OPENPOINT SET AMPYEAR = NVL(NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR),YEARMANUFACTURED) WHERE SUBTYPECD NOT IN ('4','11');
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_PRIMARYRISER SET AMPYEAR = NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_PRIOHCONDUCTOR SET AMPYEAR = INSTALLJOBYEAR;
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_PRIOHCONDUCTORINFO SET AMPYEAR = EXTRACT(YEAR FROM INSTALLATIONDATE);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_PRIUGCONDUCTOR SET AMPYEAR = INSTALLJOBYEAR;
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_PRIUGCONDUCTORINFO SET AMPYEAR = EXTRACT(YEAR FROM INSTALLATIONDATE);
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_SWITCH SET AMPYEAR = NVL(NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR),YEARMANUFACTURED) WHERE SUBTYPECD IN ('3','5','7');
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_TRANSFORMER SET AMPYEAR = NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR) WHERE SUBTYPECD ='5';
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_TRANSFORMERUNIT SET AMPYEAR = NVL(NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR),YEARMANUFACTURED) WHERE SUBTYPECD ='3';
SELECT CURRENT_TIMESTAMP FROM DUAL;
UPDATE EDGIS.ZZ_MV_VAULTPOLY SET AMPYEAR = NVL(EXTRACT(YEAR FROM INSTALLATIONDATE),INSTALLJOBYEAR);
SELECT CURRENT_TIMESTAMP FROM DUAL;
COMMIT;
EXEC sde.version_user_ddl.edit_version(:my_version,2);
SELECT CURRENT_TIMESTAMP FROM DUAL;
SPOOL OFF