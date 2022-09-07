Prompt drop Trigger V190821_UPDATE;
DROP TRIGGER EDGIS.V190821_UPDATE
/

Prompt Trigger V190821_UPDATE;
--
-- V190821_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V190821_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_GENERATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A190821 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.TOTALKW,:new.DEVICEID,:new.GENERATIONID,:new.METERID,:new.SERVICEPOINTID,:new.DIVNO,:new.GENERATORNAME,:new.GENTYPE,:new.NOTES,:new.SERVICEPOINTGUID,:new.STATUS_CD,:new.MANF_CD,:new.MODEL_CD,:new.ENOS_EQP_ID,:new.ENOS_REF_ID,:new.QUANTITY,:new.NAMEPLATE_VOLTAGE,:new.KW_OUT,:new.NP_KVA,:new.EXPORT_CD,:new.DC_RATING,:new.DC_EFFICIENCY,:new.DC_CAPACITYKW,:new.DC_INVERTER_EQP_ID,:new.INV_INVERTERID,:new.INV_MASTER,:new.PROT_PROTECTION_CD,:new.PROT_SEC_POS_RESISTANCE,:new.PROT_SEC_POS_REACTANCE,:new.PROT_SEC_ZERO_RESISTANCE,:new.PROT_SEC_ZERO_REACTANCE,:new.PROT_SEC_LENGTH,:new.PROT_SEC_TYPE_CONDUCTOR,:new.SYN_KVAR_OUT,:new.SYN_POWER_FACTOR_PCT,:new.SYN_MAX_KVAR,:new.SYN_MIN_KVAR,:new.SYN_REGULATION_CD,:new.SYN_UNSAT_SYNC_RESISTANCE,:new.SYN_UNSAT_SYNC_REACTANCE,:new.SYN_SAT_SYNC_RESISTANCE,:new.SYN_SAT_SYNC_REACTANCE,:new.SYN_SAT_SUBTRANS_RESISTANCE,:new.SYN_SAT_SUBTRANS_REACTANCE,:new.SYN_NEG_RESISTANCE,:new.SYN_NEG_REACTANCE,:new.SYN_ZERO_RESISTANCE,:new.SYN_ZERO_REACTANCE,:new.SYN_GRD_RESISTANCE,:new.SYN_GRD_REACTANCE,:new.IND_PHASE_CD,:new.IND_POWER_FACTOR1,:new.IND_POWER_FACTOR2,:new.IND_POWER_FACTOR3,:new.IND_POWER_FACTOR4,:new.IND_POWER_FACTOR5,:new.IND_POS_RESISTANCE,:new.IND_POS_REACTANCE,:new.IND_GRD_RESISTANCE,:new.IND_GRD_REACTANCE,:new.IND_SUBTRANS_REACTANCE,:new.IND_NEG_RESISTANCE,:new.IND_NEG_REACTANCE,:new.IND_ZERO_RESISTANCE,:new.IND_ZERO_REACTANCE,:new.IND_NEMA_CD,:new.IND_OPERATION_MODE_CD,:new.POWER_SOURCE_CD,current_state); INSERT INTO EDGIS.D190821 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A190821 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,TOTALKW = :new.TOTALKW,DEVICEID = :new.DEVICEID,GENERATIONID = :new.GENERATIONID,METERID = :new.METERID,SERVICEPOINTID = :new.SERVICEPOINTID,DIVNO = :new.DIVNO,GENERATORNAME = :new.GENERATORNAME,GENTYPE = :new.GENTYPE,NOTES = :new.NOTES,SERVICEPOINTGUID = :new.SERVICEPOINTGUID,STATUS_CD = :new.STATUS_CD,MANF_CD = :new.MANF_CD,MODEL_CD = :new.MODEL_CD,ENOS_EQP_ID = :new.ENOS_EQP_ID,ENOS_REF_ID = :new.ENOS_REF_ID,QUANTITY = :new.QUANTITY,NAMEPLATE_VOLTAGE = :new.NAMEPLATE_VOLTAGE,KW_OUT = :new.KW_OUT,NP_KVA = :new.NP_KVA,EXPORT_CD = :new.EXPORT_CD,DC_RATING = :new.DC_RATING,DC_EFFICIENCY = :new.DC_EFFICIENCY,DC_CAPACITYKW = :new.DC_CAPACITYKW,DC_INVERTER_EQP_ID = :new.DC_INVERTER_EQP_ID,INV_INVERTERID = :new.INV_INVERTERID,INV_MASTER = :new.INV_MASTER,PROT_PROTECTION_CD = :new.PROT_PROTECTION_CD,PROT_SEC_POS_RESISTANCE = :new.PROT_SEC_POS_RESISTANCE,PROT_SEC_POS_REACTANCE = :new.PROT_SEC_POS_REACTANCE,PROT_SEC_ZERO_RESISTANCE = :new.PROT_SEC_ZERO_RESISTANCE,PROT_SEC_ZERO_REACTANCE = :new.PROT_SEC_ZERO_REACTANCE,PROT_SEC_LENGTH = :new.PROT_SEC_LENGTH,PROT_SEC_TYPE_CONDUCTOR = :new.PROT_SEC_TYPE_CONDUCTOR,SYN_KVAR_OUT = :new.SYN_KVAR_OUT,SYN_POWER_FACTOR_PCT = :new.SYN_POWER_FACTOR_PCT,SYN_MAX_KVAR = :new.SYN_MAX_KVAR,SYN_MIN_KVAR = :new.SYN_MIN_KVAR,SYN_REGULATION_CD = :new.SYN_REGULATION_CD,SYN_UNSAT_SYNC_RESISTANCE = :new.SYN_UNSAT_SYNC_RESISTANCE,SYN_UNSAT_SYNC_REACTANCE = :new.SYN_UNSAT_SYNC_REACTANCE,SYN_SAT_SYNC_RESISTANCE = :new.SYN_SAT_SYNC_RESISTANCE,SYN_SAT_SYNC_REACTANCE = :new.SYN_SAT_SYNC_REACTANCE,SYN_SAT_SUBTRANS_RESISTANCE = :new.SYN_SAT_SUBTRANS_RESISTANCE,SYN_SAT_SUBTRANS_REACTANCE = :new.SYN_SAT_SUBTRANS_REACTANCE,SYN_NEG_RESISTANCE = :new.SYN_NEG_RESISTANCE,SYN_NEG_REACTANCE = :new.SYN_NEG_REACTANCE,SYN_ZERO_RESISTANCE = :new.SYN_ZERO_RESISTANCE,SYN_ZERO_REACTANCE = :new.SYN_ZERO_REACTANCE,SYN_GRD_RESISTANCE = :new.SYN_GRD_RESISTANCE,SYN_GRD_REACTANCE = :new.SYN_GRD_REACTANCE,IND_PHASE_CD = :new.IND_PHASE_CD,IND_POWER_FACTOR1 = :new.IND_POWER_FACTOR1,IND_POWER_FACTOR2 = :new.IND_POWER_FACTOR2,IND_POWER_FACTOR3 = :new.IND_POWER_FACTOR3,IND_POWER_FACTOR4 = :new.IND_POWER_FACTOR4,IND_POWER_FACTOR5 = :new.IND_POWER_FACTOR5,IND_POS_RESISTANCE = :new.IND_POS_RESISTANCE,IND_POS_REACTANCE = :new.IND_POS_REACTANCE,IND_GRD_RESISTANCE = :new.IND_GRD_RESISTANCE,IND_GRD_REACTANCE = :new.IND_GRD_REACTANCE,IND_SUBTRANS_REACTANCE = :new.IND_SUBTRANS_REACTANCE,IND_NEG_RESISTANCE = :new.IND_NEG_RESISTANCE,IND_NEG_REACTANCE = :new.IND_NEG_REACTANCE,IND_ZERO_RESISTANCE = :new.IND_ZERO_RESISTANCE,IND_ZERO_REACTANCE = :new.IND_ZERO_REACTANCE,IND_NEMA_CD = :new.IND_NEMA_CD,IND_OPERATION_MODE_CD = :new.IND_OPERATION_MODE_CD,POWER_SOURCE_CD = :new.POWER_SOURCE_CD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d190821 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d190821 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A190821 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.TOTALKW,:new.DEVICEID,:new.GENERATIONID,:new.METERID,:new.SERVICEPOINTID,:new.DIVNO,:new.GENERATORNAME,:new.GENTYPE,:new.NOTES,:new.SERVICEPOINTGUID,:new.STATUS_CD,:new.MANF_CD,:new.MODEL_CD,:new.ENOS_EQP_ID,:new.ENOS_REF_ID,:new.QUANTITY,:new.NAMEPLATE_VOLTAGE,:new.KW_OUT,:new.NP_KVA,:new.EXPORT_CD,:new.DC_RATING,:new.DC_EFFICIENCY,:new.DC_CAPACITYKW,:new.DC_INVERTER_EQP_ID,:new.INV_INVERTERID,:new.INV_MASTER,:new.PROT_PROTECTION_CD,:new.PROT_SEC_POS_RESISTANCE,:new.PROT_SEC_POS_REACTANCE,:new.PROT_SEC_ZERO_RESISTANCE,:new.PROT_SEC_ZERO_REACTANCE,:new.PROT_SEC_LENGTH,:new.PROT_SEC_TYPE_CONDUCTOR,:new.SYN_KVAR_OUT,:new.SYN_POWER_FACTOR_PCT,:new.SYN_MAX_KVAR,:new.SYN_MIN_KVAR,:new.SYN_REGULATION_CD,:new.SYN_UNSAT_SYNC_RESISTANCE,:new.SYN_UNSAT_SYNC_REACTANCE,:new.SYN_SAT_SYNC_RESISTANCE,:new.SYN_SAT_SYNC_REACTANCE,:new.SYN_SAT_SUBTRANS_RESISTANCE,:new.SYN_SAT_SUBTRANS_REACTANCE,:new.SYN_NEG_RESISTANCE,:new.SYN_NEG_REACTANCE,:new.SYN_ZERO_RESISTANCE,:new.SYN_ZERO_REACTANCE,:new.SYN_GRD_RESISTANCE,:new.SYN_GRD_REACTANCE,:new.IND_PHASE_CD,:new.IND_POWER_FACTOR1,:new.IND_POWER_FACTOR2,:new.IND_POWER_FACTOR3,:new.IND_POWER_FACTOR4,:new.IND_POWER_FACTOR5,:new.IND_POS_RESISTANCE,:new.IND_POS_REACTANCE,:new.IND_GRD_RESISTANCE,:new.IND_GRD_REACTANCE,:new.IND_SUBTRANS_REACTANCE,:new.IND_NEG_RESISTANCE,:new.IND_NEG_REACTANCE,:new.IND_ZERO_RESISTANCE,:new.IND_ZERO_REACTANCE,:new.IND_NEMA_CD,:new.IND_OPERATION_MODE_CD,:new.POWER_SOURCE_CD,current_state); INSERT INTO EDGIS.D190821 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.GENERATION SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,TOTALKW = :new.TOTALKW,DEVICEID = :new.DEVICEID,GENERATIONID = :new.GENERATIONID,METERID = :new.METERID,SERVICEPOINTID = :new.SERVICEPOINTID,DIVNO = :new.DIVNO,GENERATORNAME = :new.GENERATORNAME,GENTYPE = :new.GENTYPE,NOTES = :new.NOTES,SERVICEPOINTGUID = :new.SERVICEPOINTGUID,STATUS_CD = :new.STATUS_CD,MANF_CD = :new.MANF_CD,MODEL_CD = :new.MODEL_CD,ENOS_EQP_ID = :new.ENOS_EQP_ID,ENOS_REF_ID = :new.ENOS_REF_ID,QUANTITY = :new.QUANTITY,NAMEPLATE_VOLTAGE = :new.NAMEPLATE_VOLTAGE,KW_OUT = :new.KW_OUT,NP_KVA = :new.NP_KVA,EXPORT_CD = :new.EXPORT_CD,DC_RATING = :new.DC_RATING,DC_EFFICIENCY = :new.DC_EFFICIENCY,DC_CAPACITYKW = :new.DC_CAPACITYKW,DC_INVERTER_EQP_ID = :new.DC_INVERTER_EQP_ID,INV_INVERTERID = :new.INV_INVERTERID,INV_MASTER = :new.INV_MASTER,PROT_PROTECTION_CD = :new.PROT_PROTECTION_CD,PROT_SEC_POS_RESISTANCE = :new.PROT_SEC_POS_RESISTANCE,PROT_SEC_POS_REACTANCE = :new.PROT_SEC_POS_REACTANCE,PROT_SEC_ZERO_RESISTANCE = :new.PROT_SEC_ZERO_RESISTANCE,PROT_SEC_ZERO_REACTANCE = :new.PROT_SEC_ZERO_REACTANCE,PROT_SEC_LENGTH = :new.PROT_SEC_LENGTH,PROT_SEC_TYPE_CONDUCTOR = :new.PROT_SEC_TYPE_CONDUCTOR,SYN_KVAR_OUT = :new.SYN_KVAR_OUT,SYN_POWER_FACTOR_PCT = :new.SYN_POWER_FACTOR_PCT,SYN_MAX_KVAR = :new.SYN_MAX_KVAR,SYN_MIN_KVAR = :new.SYN_MIN_KVAR,SYN_REGULATION_CD = :new.SYN_REGULATION_CD,SYN_UNSAT_SYNC_RESISTANCE = :new.SYN_UNSAT_SYNC_RESISTANCE,SYN_UNSAT_SYNC_REACTANCE = :new.SYN_UNSAT_SYNC_REACTANCE,SYN_SAT_SYNC_RESISTANCE = :new.SYN_SAT_SYNC_RESISTANCE,SYN_SAT_SYNC_REACTANCE = :new.SYN_SAT_SYNC_REACTANCE,SYN_SAT_SUBTRANS_RESISTANCE = :new.SYN_SAT_SUBTRANS_RESISTANCE,SYN_SAT_SUBTRANS_REACTANCE = :new.SYN_SAT_SUBTRANS_REACTANCE,SYN_NEG_RESISTANCE = :new.SYN_NEG_RESISTANCE,SYN_NEG_REACTANCE = :new.SYN_NEG_REACTANCE,SYN_ZERO_RESISTANCE = :new.SYN_ZERO_RESISTANCE,SYN_ZERO_REACTANCE = :new.SYN_ZERO_REACTANCE,SYN_GRD_RESISTANCE = :new.SYN_GRD_RESISTANCE,SYN_GRD_REACTANCE = :new.SYN_GRD_REACTANCE,IND_PHASE_CD = :new.IND_PHASE_CD,IND_POWER_FACTOR1 = :new.IND_POWER_FACTOR1,IND_POWER_FACTOR2 = :new.IND_POWER_FACTOR2,IND_POWER_FACTOR3 = :new.IND_POWER_FACTOR3,IND_POWER_FACTOR4 = :new.IND_POWER_FACTOR4,IND_POWER_FACTOR5 = :new.IND_POWER_FACTOR5,IND_POS_RESISTANCE = :new.IND_POS_RESISTANCE,IND_POS_REACTANCE = :new.IND_POS_REACTANCE,IND_GRD_RESISTANCE = :new.IND_GRD_RESISTANCE,IND_GRD_REACTANCE = :new.IND_GRD_REACTANCE,IND_SUBTRANS_REACTANCE = :new.IND_SUBTRANS_REACTANCE,IND_NEG_RESISTANCE = :new.IND_NEG_RESISTANCE,IND_NEG_REACTANCE = :new.IND_NEG_REACTANCE,IND_ZERO_RESISTANCE = :new.IND_ZERO_RESISTANCE,IND_ZERO_REACTANCE = :new.IND_ZERO_REACTANCE,IND_NEMA_CD = :new.IND_NEMA_CD,IND_OPERATION_MODE_CD = :new.IND_OPERATION_MODE_CD,POWER_SOURCE_CD = :new.POWER_SOURCE_CD WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A190821 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.TOTALKW,:new.DEVICEID,:new.GENERATIONID,:new.METERID,:new.SERVICEPOINTID,:new.DIVNO,:new.GENERATORNAME,:new.GENTYPE,:new.NOTES,:new.SERVICEPOINTGUID,:new.STATUS_CD,:new.MANF_CD,:new.MODEL_CD,:new.ENOS_EQP_ID,:new.ENOS_REF_ID,:new.QUANTITY,:new.NAMEPLATE_VOLTAGE,:new.KW_OUT,:new.NP_KVA,:new.EXPORT_CD,:new.DC_RATING,:new.DC_EFFICIENCY,:new.DC_CAPACITYKW,:new.DC_INVERTER_EQP_ID,:new.INV_INVERTERID,:new.INV_MASTER,:new.PROT_PROTECTION_CD,:new.PROT_SEC_POS_RESISTANCE,:new.PROT_SEC_POS_REACTANCE,:new.PROT_SEC_ZERO_RESISTANCE,:new.PROT_SEC_ZERO_REACTANCE,:new.PROT_SEC_LENGTH,:new.PROT_SEC_TYPE_CONDUCTOR,:new.SYN_KVAR_OUT,:new.SYN_POWER_FACTOR_PCT,:new.SYN_MAX_KVAR,:new.SYN_MIN_KVAR,:new.SYN_REGULATION_CD,:new.SYN_UNSAT_SYNC_RESISTANCE,:new.SYN_UNSAT_SYNC_REACTANCE,:new.SYN_SAT_SYNC_RESISTANCE,:new.SYN_SAT_SYNC_REACTANCE,:new.SYN_SAT_SUBTRANS_RESISTANCE,:new.SYN_SAT_SUBTRANS_REACTANCE,:new.SYN_NEG_RESISTANCE,:new.SYN_NEG_REACTANCE,:new.SYN_ZERO_RESISTANCE,:new.SYN_ZERO_REACTANCE,:new.SYN_GRD_RESISTANCE,:new.SYN_GRD_REACTANCE,:new.IND_PHASE_CD,:new.IND_POWER_FACTOR1,:new.IND_POWER_FACTOR2,:new.IND_POWER_FACTOR3,:new.IND_POWER_FACTOR4,:new.IND_POWER_FACTOR5,:new.IND_POS_RESISTANCE,:new.IND_POS_REACTANCE,:new.IND_GRD_RESISTANCE,:new.IND_GRD_REACTANCE,:new.IND_SUBTRANS_REACTANCE,:new.IND_NEG_RESISTANCE,:new.IND_NEG_REACTANCE,:new.IND_ZERO_RESISTANCE,:new.IND_ZERO_REACTANCE,:new.IND_NEMA_CD,:new.IND_OPERATION_MODE_CD,:new.POWER_SOURCE_CD,current_state); INSERT INTO EDGIS.D190821 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A190821 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,TOTALKW = :new.TOTALKW,DEVICEID = :new.DEVICEID,GENERATIONID = :new.GENERATIONID,METERID = :new.METERID,SERVICEPOINTID = :new.SERVICEPOINTID,DIVNO = :new.DIVNO,GENERATORNAME = :new.GENERATORNAME,GENTYPE = :new.GENTYPE,NOTES = :new.NOTES,SERVICEPOINTGUID = :new.SERVICEPOINTGUID,STATUS_CD = :new.STATUS_CD,MANF_CD = :new.MANF_CD,MODEL_CD = :new.MODEL_CD,ENOS_EQP_ID = :new.ENOS_EQP_ID,ENOS_REF_ID = :new.ENOS_REF_ID,QUANTITY = :new.QUANTITY,NAMEPLATE_VOLTAGE = :new.NAMEPLATE_VOLTAGE,KW_OUT = :new.KW_OUT,NP_KVA = :new.NP_KVA,EXPORT_CD = :new.EXPORT_CD,DC_RATING = :new.DC_RATING,DC_EFFICIENCY = :new.DC_EFFICIENCY,DC_CAPACITYKW = :new.DC_CAPACITYKW,DC_INVERTER_EQP_ID = :new.DC_INVERTER_EQP_ID,INV_INVERTERID = :new.INV_INVERTERID,INV_MASTER = :new.INV_MASTER,PROT_PROTECTION_CD = :new.PROT_PROTECTION_CD,PROT_SEC_POS_RESISTANCE = :new.PROT_SEC_POS_RESISTANCE,PROT_SEC_POS_REACTANCE = :new.PROT_SEC_POS_REACTANCE,PROT_SEC_ZERO_RESISTANCE = :new.PROT_SEC_ZERO_RESISTANCE,PROT_SEC_ZERO_REACTANCE = :new.PROT_SEC_ZERO_REACTANCE,PROT_SEC_LENGTH = :new.PROT_SEC_LENGTH,PROT_SEC_TYPE_CONDUCTOR = :new.PROT_SEC_TYPE_CONDUCTOR,SYN_KVAR_OUT = :new.SYN_KVAR_OUT,SYN_POWER_FACTOR_PCT = :new.SYN_POWER_FACTOR_PCT,SYN_MAX_KVAR = :new.SYN_MAX_KVAR,SYN_MIN_KVAR = :new.SYN_MIN_KVAR,SYN_REGULATION_CD = :new.SYN_REGULATION_CD,SYN_UNSAT_SYNC_RESISTANCE = :new.SYN_UNSAT_SYNC_RESISTANCE,SYN_UNSAT_SYNC_REACTANCE = :new.SYN_UNSAT_SYNC_REACTANCE,SYN_SAT_SYNC_RESISTANCE = :new.SYN_SAT_SYNC_RESISTANCE,SYN_SAT_SYNC_REACTANCE = :new.SYN_SAT_SYNC_REACTANCE,SYN_SAT_SUBTRANS_RESISTANCE = :new.SYN_SAT_SUBTRANS_RESISTANCE,SYN_SAT_SUBTRANS_REACTANCE = :new.SYN_SAT_SUBTRANS_REACTANCE,SYN_NEG_RESISTANCE = :new.SYN_NEG_RESISTANCE,SYN_NEG_REACTANCE = :new.SYN_NEG_REACTANCE,SYN_ZERO_RESISTANCE = :new.SYN_ZERO_RESISTANCE,SYN_ZERO_REACTANCE = :new.SYN_ZERO_REACTANCE,SYN_GRD_RESISTANCE = :new.SYN_GRD_RESISTANCE,SYN_GRD_REACTANCE = :new.SYN_GRD_REACTANCE,IND_PHASE_CD = :new.IND_PHASE_CD,IND_POWER_FACTOR1 = :new.IND_POWER_FACTOR1,IND_POWER_FACTOR2 = :new.IND_POWER_FACTOR2,IND_POWER_FACTOR3 = :new.IND_POWER_FACTOR3,IND_POWER_FACTOR4 = :new.IND_POWER_FACTOR4,IND_POWER_FACTOR5 = :new.IND_POWER_FACTOR5,IND_POS_RESISTANCE = :new.IND_POS_RESISTANCE,IND_POS_REACTANCE = :new.IND_POS_REACTANCE,IND_GRD_RESISTANCE = :new.IND_GRD_RESISTANCE,IND_GRD_REACTANCE = :new.IND_GRD_REACTANCE,IND_SUBTRANS_REACTANCE = :new.IND_SUBTRANS_REACTANCE,IND_NEG_RESISTANCE = :new.IND_NEG_RESISTANCE,IND_NEG_REACTANCE = :new.IND_NEG_REACTANCE,IND_ZERO_RESISTANCE = :new.IND_ZERO_RESISTANCE,IND_ZERO_REACTANCE = :new.IND_ZERO_REACTANCE,IND_NEMA_CD = :new.IND_NEMA_CD,IND_OPERATION_MODE_CD = :new.IND_OPERATION_MODE_CD,POWER_SOURCE_CD = :new.POWER_SOURCE_CD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (190821,current_state);  END IF; END;
/