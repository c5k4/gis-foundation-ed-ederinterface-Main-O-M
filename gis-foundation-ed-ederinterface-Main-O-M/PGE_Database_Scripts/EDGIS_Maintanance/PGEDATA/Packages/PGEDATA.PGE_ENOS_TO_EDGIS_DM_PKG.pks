Prompt drop Package PGE_ENOS_TO_EDGIS_DM_PKG;
DROP PACKAGE PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG
/

Prompt Package PGE_ENOS_TO_EDGIS_DM_PKG;
--
-- PGE_ENOS_TO_EDGIS_DM_PKG  (Package) 
--
CREATE OR REPLACE PACKAGE PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG
AS
	--Author :- TCS
	--Created Date :- 2-Nov-16
	-- Last Modified :- 4-May-17
	--Purpose :- For data migration from SAP and CEDSA to EDGIS

	-- Whole dmigration
	PROCEDURE PGE_DATA_MGRTN_SP ;

	-- Validation on stg_1 after SAP data loading
	PROCEDURE PGE_SMRY_EQPMNT_DTL_VLDTN_SP (Input_DM_DI_Flag IN varchar2)  ;

	-- Calculate Gen Type function
	FUNCTION PGE_CAL_GEN_TYPE_F(SPID nvarchar2 ) return varchar2;

        -- Find Project name from generator to generation function
	FUNCTION PGE_FIND_PRJ_NAME_F(GUID char ) return varchar2;

	 -- convert Gen_tech_cd function
	FUNCTION PGE_GEN_TECH_CD_CONV_F(SAP_VAL nvarchar2) return nvarchar2;

  	 -- convert type reverse function
	FUNCTION PGE_GEN_type_rev_conv(SAP_VAL nvarchar2,act nvarchar2) return nvarchar2;

	-- Delete all stg_2 tables before any process in stg_2
	FUNCTION PGE_DELETE_STG2_DATA_SP  (Input_DM_DI_Flag IN varchar2)  return varchar2  ;

	--For DM, Migrate data from stg_1 to stg_2
	PROCEDURE PGE_SAP_2_STG2_DATA_MGRTN_SP (OutFlag OUT varchar2);

	--empty table PGEDATA.PGEDATA_SAP_CEDSA_MIGRATION, Validate stg_2 table PGEDATA.PGEDATA_SM_GENERATION_STAGE and mark STATUS as failed
	PROCEDURE PGE_CEDSA_DATA_VLDTN_SP   ;

	--merge Stg_2 SAP data with CEDSA
	PROCEDURE PGE_CEDSA_2_SAP_PROCESS_SP ;

	-- for data migration , copy data(GENERATIONINFO) from stg_2 to main EDGIS only.
	PROCEDURE PGE_GEN_MGRTN_TO_MAIN_EDGIS_SP (Input_DM_DI_Flag IN varchar2);

	-- update Service location
	PROCEDURE PGE_UPDT_GEN_CTGY_SP_LOC_SP;

  	--For DI, Migrate data from stg_1 to stg_2
	PROCEDURE PGE_SAP_TO_STG2_DI_SP (Input_Action IN varchar2, Out_Flag OUT varchar2);

	-- update both stg_1 tables CEDSA_MATCH_FOUND flag from stg_2
	PROCEDURE PGE_CEDSA_NOT_MERGED_SP ;

	-- update both stg_1 tables GUID flag from stg_2
	PROCEDURE PGE_STG2_GUID_UPDT_STG1_SP ;

	-- Function to get the label text based on servicelocationguid
	FUNCTION F_GET_GENERATION_LABEL (Input_servicelocationguid varchar2) return varchar2;

	-- Procedur to update label text in servicelocation table
	PROCEDURE PGE_UPDATE_GENERATION_LABEL;


END PGE_ENOS_TO_EDGIS_DM_PKG;
/


Prompt Grants on PACKAGE PGE_ENOS_TO_EDGIS_DM_PKG TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG TO GIS_I_WRITE
/

Prompt Grants on PACKAGE PGE_ENOS_TO_EDGIS_DM_PKG TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG TO IGPCITEDITOR
/

Prompt Grants on PACKAGE PGE_ENOS_TO_EDGIS_DM_PKG TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG TO IGPEDITOR
/
