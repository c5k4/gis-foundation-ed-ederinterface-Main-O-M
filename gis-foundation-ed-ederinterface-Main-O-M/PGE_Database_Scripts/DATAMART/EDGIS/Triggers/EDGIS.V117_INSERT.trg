Prompt drop Trigger V117_INSERT;
DROP TRIGGER EDGIS.V117_INSERT
/

Prompt Trigger V117_INSERT;
--
-- V117_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V117_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_TRANSFORMER REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',117); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A117 VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.OPERATINGVOLTAGE,:new.COASTALIDC,:new.UNITCOUNT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.SUBTYPECD,:new.LOWSIDEVOLTAGE,:new.HIGHSIDECONFIGURATION,:new.HIGHSIDEPROTECTION,:new.LIVEFRONTIDC,:new.LOADBREAK,:new.CGC12,:new.CLIMATEZONE,:new.SUBMERSIBLEIDC,:new.FIELDINVESTIGATIONCODE,:new.GROUNDEDIDC,:new.REGULATEDOUTPUTIDC,:new.STABILIZINGBANKIDC,:new.LABELTEXT,:new.HIGHSIDEVOLTAGE,:new.DISTRIBUTEDGENERATIONIDC,:new.INTERRUPTERIDC,:new.AUTOIDC,:new.SUBWAYIDC,:new.CUSTAGREEMENTGUID,:new.CEDSASTRUCTUREID,:new.RATEDKVA,:new.SPOTIDC,:new.INSTALLATIONTYPE,:new.LATITUDE,:new.LONGITUDE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.STREET,:new.FUNCTIONALLOC,:new.SUPERORDEQUIP,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSULATINGFLUIDTYPE,:new.NETWORKGROUPNUMBER,:new.VAULTLOCATION,:new.SORTINGORDER,:new.SPOTWITH,:new.INSTRUCTIONS,:new.LISTNAME,:new.SYMBOLSPACE,:new.SUMMERKVA,:new.WINTERKVA,:new.SUMMERPCT,:new.WINTERPCT,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID,:new.PCPGUID);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.TRANSFORMER VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.OPERATINGVOLTAGE,:new.COASTALIDC,:new.UNITCOUNT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.SUBTYPECD,:new.LOWSIDEVOLTAGE,:new.HIGHSIDECONFIGURATION,:new.HIGHSIDEPROTECTION,:new.LIVEFRONTIDC,:new.LOADBREAK,:new.CGC12,:new.CLIMATEZONE,:new.SUBMERSIBLEIDC,:new.FIELDINVESTIGATIONCODE,:new.GROUNDEDIDC,:new.REGULATEDOUTPUTIDC,:new.STABILIZINGBANKIDC,:new.LABELTEXT,:new.HIGHSIDEVOLTAGE,:new.DISTRIBUTEDGENERATIONIDC,:new.INTERRUPTERIDC,:new.AUTOIDC,:new.SUBWAYIDC,:new.CUSTAGREEMENTGUID,:new.CEDSASTRUCTUREID,:new.RATEDKVA,:new.SPOTIDC,:new.INSTALLATIONTYPE,:new.LATITUDE,:new.LONGITUDE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.STREET,:new.FUNCTIONALLOC,:new.SUPERORDEQUIP,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSULATINGFLUIDTYPE,:new.NETWORKGROUPNUMBER,:new.VAULTLOCATION,:new.SORTINGORDER,:new.SPOTWITH,:new.INSTRUCTIONS,:new.LISTNAME,:new.SYMBOLSPACE,:new.SUMMERKVA,:new.WINTERKVA,:new.SUMMERPCT,:new.WINTERPCT,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,:new.SSDGUID,:new.PCPGUID);  ELSE INSERT INTO EDGIS.A117  VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.OPERATINGVOLTAGE,:new.COASTALIDC,:new.UNITCOUNT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.SUBTYPECD,:new.LOWSIDEVOLTAGE,:new.HIGHSIDECONFIGURATION,:new.HIGHSIDEPROTECTION,:new.LIVEFRONTIDC,:new.LOADBREAK,:new.CGC12,:new.CLIMATEZONE,:new.SUBMERSIBLEIDC,:new.FIELDINVESTIGATIONCODE,:new.GROUNDEDIDC,:new.REGULATEDOUTPUTIDC,:new.STABILIZINGBANKIDC,:new.LABELTEXT,:new.HIGHSIDEVOLTAGE,:new.DISTRIBUTEDGENERATIONIDC,:new.INTERRUPTERIDC,:new.AUTOIDC,:new.SUBWAYIDC,:new.CUSTAGREEMENTGUID,:new.CEDSASTRUCTUREID,:new.RATEDKVA,:new.SPOTIDC,:new.INSTALLATIONTYPE,:new.LATITUDE,:new.LONGITUDE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.STREET,:new.FUNCTIONALLOC,:new.SUPERORDEQUIP,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSULATINGFLUIDTYPE,:new.NETWORKGROUPNUMBER,:new.VAULTLOCATION,:new.SORTINGORDER,:new.SPOTWITH,:new.INSTRUCTIONS,:new.LISTNAME,:new.SYMBOLSPACE,:new.SUMMERKVA,:new.WINTERKVA,:new.SUMMERPCT,:new.WINTERPCT,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID,:new.PCPGUID);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (117,current_state);  END IF;END;
/
