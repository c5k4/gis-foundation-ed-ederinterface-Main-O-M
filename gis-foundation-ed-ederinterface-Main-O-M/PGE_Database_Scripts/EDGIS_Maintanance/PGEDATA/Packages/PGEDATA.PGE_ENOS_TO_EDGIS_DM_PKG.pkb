Prompt drop Package Body PGE_ENOS_TO_EDGIS_DM_PKG;
DROP PACKAGE BODY PGEDATA.PGE_ENOS_TO_EDGIS_DM_PKG
/

Prompt Package Body PGE_ENOS_TO_EDGIS_DM_PKG;
--
-- PGE_ENOS_TO_EDGIS_DM_PKG  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY PGEDATA."PGE_ENOS_TO_EDGIS_DM_PKG" AS

-- Run all the steps required for data migration
PROCEDURE Pge_data_mgrtn_sp    IS
  h_out   VARCHAR2 (50) := NULL;
  outflag VARCHAR2 (50) := NULL;
BEGIN
  -- truncate all stg_2 table
  h_out := PGE_ENOS_TO_EDGIS_DM_PKG.Pge_delete_stg2_data_sp ('DM') ;
  dbms_output.Put_line (h_out) ;

  -- Data Validation in stg_1
  pge_enos_to_edgis_dm_pkg.Pge_smry_eqpmnt_dtl_vldtn_sp ('DM') ;

  -- Data creation in stg_2 from stg_1 using procedure
  pge_enos_to_edgis_dm_pkg.Pge_sap_2_stg2_data_mgrtn_sp (outflag) ;
  dbms_output.Put_line ('Output is - ' ||outflag) ;

  -- Data validation in stg_2 before CEDSA merge
  pge_enos_to_edgis_dm_pkg.pge_cedsa_data_vldtn_sp;

  -- Merge CEDSA Data
  pge_enos_to_edgis_dm_pkg.pge_cedsa_2_sap_process_sp;

  -- Data copy from stg_2 generationInfo to actual main edgis.GenerationInfo table
  pge_enos_to_edgis_dm_pkg.Pge_gen_mgrtn_to_main_edgis_sp ('DM') ;

  -- Service Location (GenCategory ) Update and capture in stg table for primary
  pge_enos_to_edgis_dm_pkg.pge_updt_gen_ctgy_sp_loc_sp;

  -- CEDSA_MATCH_FOUND updation in stg_1 from stg_2
  pge_enos_to_edgis_dm_pkg.pge_cedsa_not_merged_sp;

  -- GUID updation in stg_1 from stg_2
  pge_enos_to_edgis_dm_pkg.pge_stg2_guid_updt_stg1_sp;

END;
---------------------------------------------------------------
-- Validate the data in the first stage table
PROCEDURE Pge_smry_eqpmnt_dtl_vldtn_sp  (input_dm_di_flag IN VARCHAR2)  IS
BEGIN

  IF input_dm_di_flag = 'DM' THEN

    -- Record should be Inserted for data migration
     UPDATE pgedata.gen_summary_stage sm
     SET sm.status           = 'F',
         sm.status_message     = 'Invalid action for generation details'
     WHERE sm.status      IS NULL
      AND (Upper (sm.action) <> 'I'  OR sm.action IS NULL) ;

    -- Mark the corresponding equipment record failed
     UPDATE pgedata.gen_equipment_stage eq
     SET eq.status       = 'F',
         eq.status_message = 'Invalid action for generation details'
     WHERE eq.status  IS NULL
          AND EXISTS
            (SELECT 2
         	   FROM pgedata.gen_summary_stage sm
        	   WHERE sm.service_point_id = eq.service_point_id
      	       AND sm.status               = 'F'
      	       AND sm.status_message       = 'Invalid action for generation details'
          ) ;

     -- Mark the equipment record failed if action type is other than Insert
     UPDATE pgedata.gen_equipment_stage
     SET status = 'F',
         status_message = 'Invalid action'
     WHERE status IS NULL
       AND (Upper (action) <> 'I'  OR action  IS NULL) ;

    -- Mark the other equipment record as failed for the record updated in the above step
     UPDATE pgedata.gen_equipment_stage eq1
     SET eq1.status  = 'F', eq1.status_message = 'Other equipments failed due Invalid action for any equipment'
     WHERE eq1.status  IS NULL
       AND EXISTS
        (SELECT 2
         	FROM pgedata.gen_equipment_stage eq
        	WHERE eq.status       = 'F'
            AND eq.service_point_id = eq1.service_point_id
            AND eq.status_message   = 'Invalid action'
        ) ;

    -- Mark the the summary record failed for above updated record
      UPDATE pgedata.gen_summary_stage sm
      SET sm.status       = 'F',   sm.status_message = 'Generation failed due to Invalid action in equipment details'
      WHERE sm.status  IS NULL
       AND EXISTS
       (SELECT 2
        FROM pgedata.gen_equipment_stage eq
        WHERE eq.status       = 'F'
            AND eq.service_point_id = sm.service_point_id
            AND eq.status_message   = 'Invalid action'
        );

    --Mark the summary record failed if generation already exists
      MERGE INTO    GEN_SUMMARY_STAGE trg USING   (
       SELECT SERVICEPOINTID FROM EDGIS.ZZ_MV_SERVICEPOINT SP, EDGIS.ZZ_MV_GENERATIONINFO GEN WHERE SP.GLOBALID= GEN.SERVICEPOINTGUID
        ) src
ON      (trg.service_point_id = src.SERVICEPOINTID)
WHEN MATCHED THEN UPDATE
    SET STATUS ='F', STATUS_MESSAGE ='Already Generation Exist';


    --Mark the equipment record failed if Generation failed
    UPDATE gen_equipment_stage
    SET status              ='F',
      status_message        ='Failed due to Generatin failed'
    WHERE service_point_id IN
      (SELECT service_point_id
      FROM gen_summary_stage
      WHERE status      ='F'
      AND status_message='Already Generation Exist'
      );

  ELSIF input_dm_di_flag = 'DI' THEN

	   -- Check the action type

--Added to Fail Records  with Action 'D' in Eqp and I in Summ Start
     update gen_equipment_stage eq set status='W', status_message='Invalid Action' where eq.action='D' and
exists (select 2 from gen_summary_stage sm where sm.action='I' and sm.service_point_id= eq.service_point_id);

    --Added to Fail Records  with Action 'D' in Eqp and I in Summ End

     UPDATE pgedata.gen_summary_stage sm
     SET sm.status  = 'F',
         sm.status_message  = 'Invalid action for generation details'
     WHERE sm.status   IS NULL
       AND (Upper (sm.action) NOT IN ('I', 'D', 'U')  OR sm.action IS NULL) ;

     UPDATE pgedata.gen_equipment_stage
     SET status  = 'F', status_message  = 'Invalid action for equipment details'
      WHERE status IS NULL
        AND (Upper (action) NOT IN ('I', 'D', 'U') OR action  IS NULL) ;

     UPDATE pgedata.gen_summary_stage sm
     SET sm.status  = 'F',    sm.status_message  = 'Null GUID with update/delete action for generation'
     WHERE sm.status  IS NULL
         AND Upper (sm.action) IN ('D', 'U')
         AND sm.guid  IS NULL;

 update pgedata.gen_equipment_stage eq set eq.status='F', eq.status_message='Failed due to generation failed'
    where eq.status is null and EXISTS
    (select 2 from pgedata.gen_summary_stage sm where sm.status='F' and sm.status_message='Null GUID with update/delete action for generation'
    and sm.service_point_id= eq.service_point_id);


     UPDATE pgedata.gen_summary_stage sm
     SET sm.status  = 'F',   sm.status_message  = 'No generation available for update or delete'
     WHERE sm.status     IS NULL
       AND Upper (sm.action) IN ('U', 'D')
       AND NOT EXISTS
     	 (SELECT 2 FROM edgis.zz_mv_generationinfo gn WHERE gn.globalid = sm.guid ) ;

     UPDATE pgedata.gen_equipment_stage eq
     SET eq.status  = 'F',   eq.status_message  = 'No data available for update or delete'
     WHERE eq.status     IS NULL
       AND EXISTS
     	 (SELECT 2 FROM pgedata.gen_summary_stage sm WHERE sm.service_point_id= eq.service_point_id
       AND sm.status='F' and sm.status_message='No generation available for update or delete' ) ;


     UPDATE pgedata.gen_equipment_stage eq
     SET eq.status          = 'F',
         eq.status_message    = 'Invalid action of equipment details for delete generation'
     WHERE eq.status     IS NULL
       AND Upper (eq.action) <> 'D'
       AND EXISTS
      (SELECT 2
          FROM pgedata.gen_summary_stage sm
          WHERE sm.status      IS NULL
            AND Upper (sm.action)   = 'D'
            AND sm.service_point_id = eq.service_point_id
      ) ;
  END IF;

  COMMIT;

 -- Check for service point id
  UPDATE pgedata.gen_summary_stage sm
  SET sm.status            = 'F',    sm.status_message      = 'Null service point ID for generation'
   WHERE sm.status       IS NULL
       AND sm.service_point_id IS NULL;

   UPDATE pgedata.gen_equipment_stage eq
   SET eq.status       = 'F',    eq.status_message = 'Null service point ID for generation'
    WHERE eq.status  IS NULL
         AND EXISTS
  	  (SELECT 2
      	   FROM pgedata.gen_summary_stage sm
      	  WHERE sm.service_point_id = eq.service_point_id
  	       AND sm.status               = 'F'
    	       AND sm.status_message       = 'Null service point ID for generation'
    	 ) ;

   UPDATE pgedata.gen_summary_stage sm
   SET sm.status       = 'F',     sm.status_message = 'Generation SP ID not exist in EDGIS'
   WHERE sm.status  IS NULL
   AND NOT EXISTS
   	 (SELECT 2
      	 FROM edgis.zz_mv_servicepoint sp
      	WHERE sp.servicepointid = sm.service_point_id
    	) ;

   UPDATE pgedata.gen_equipment_stage eq
   SET eq.status       = 'F',    eq.status_message = 'Generation SP ID not exist in EDGIS'
   WHERE eq.status  IS NULL
   AND EXISTS
   	 (SELECT 2
       	FROM pgedata.gen_summary_stage sm
      	WHERE sm.service_point_id = eq.service_point_id
   	AND sm.status               = 'F'
    	AND sm.status_message       = 'Generation SP ID not exist in EDGIS'
  	 ) ;

   UPDATE pgedata.gen_summary_stage sm
   SET sm.status            = 'F',    sm.status_message      = 'Duplicate service point ID for generation'
   WHERE sm.status       IS NULL
       AND sm.service_point_id IN
    	(SELECT service_point_id
      	 FROM pgedata.gen_summary_stage
      	WHERE status IS NULL
   	GROUP BY service_point_id
    	HAVING COUNT ( *) > 1
   	 ) ;

   UPDATE pgedata.gen_equipment_stage eq
   SET eq.status       = 'F',
    eq.status_message = 'Duplicate service point ID for generation'
    WHERE eq.status  IS NULL
         AND EXISTS
    (SELECT 2
       FROM pgedata.gen_summary_stage sm
      WHERE sm.service_point_id = eq.service_point_id
    AND sm.status               = 'F'
    AND sm.status_message       = 'Duplicate service point ID for generation'
    ) ;

   UPDATE pgedata.gen_summary_stage
   SET status   = 'F',    status_message      = 'Multiple records in EDGIS.SERVICEPOINT for generation SPID'
   WHERE status  IS NULL
      AND service_point_id IN
    (SELECT servicepointid
     FROM edgis.zz_mv_servicepoint
     GROUP BY servicepointid
     HAVING COUNT ( *) > 1
    ) ;

  UPDATE pgedata.gen_equipment_stage eq
  SET eq.status       = 'F',    eq.status_message = 'Multiple records in EDGIS.SERVICEPOINT for generation SPID'
  WHERE eq.status  IS NULL
       AND EXISTS
  	 (SELECT 2
      	 FROM pgedata.gen_summary_stage sm
      	WHERE sm.service_point_id = eq.service_point_id
           	AND sm.status               = 'F'
           	AND sm.status_message       = 'Multiple records in EDGIS.SERVICEPOINT for generation SPID'
  	  ) ;

  UPDATE pgedata.gen_equipment_stage
  SET status            = 'F',   status_message      = 'Null service point ID'
  WHERE status       IS NULL
       AND service_point_id IS NULL;

  UPDATE pgedata.gen_equipment_stage eq
  SET eq.status       = 'F',    eq.status_message = 'No generation for this equipment'
  WHERE eq.status  IS NULL
       AND NOT EXISTS
    	(SELECT 2
       	FROM pgedata.gen_summary_stage sm
      	WHERE sm.service_point_id = eq.service_point_id
   	 ) ;

   UPDATE pgedata.gen_equipment_stage eq1
   SET eq1.status       = 'F',    eq1.status_message = 'Other equipments failed due to No generation for this equipment error'
  WHERE eq1.status  IS NULL
       AND EXISTS
   	 (SELECT 2
       	  FROM pgedata.gen_equipment_stage eq
      	  WHERE eq.status       = 'F'
    	      AND eq.service_point_id = eq1.service_point_id
    	     AND eq.status_message   = 'No generation for this equipment'
  	  ) ;

  UPDATE pgedata.gen_equipment_stage eq
  SET eq.status         = 'F',    eq.status_message   = 'Duplicate SAP equipment ID'
  WHERE eq.status    IS NULL
      AND sap_equipment_id IN
  	 (SELECT sap_equipment_id
      	 FROM pgedata.gen_equipment_stage
      	WHERE status IS NULL
   	GROUP BY sap_equipment_id
    	HAVING COUNT ( *) > 1
   	 ) ;

  UPDATE pgedata.gen_equipment_stage eq1
  SET eq1.status       = 'F',    eq1.status_message = 'Other equipments failed due to Duplicate SAP equipment ID'
  WHERE eq1.status  IS NULL
       AND EXISTS
   	 (SELECT 2
       	 FROM pgedata.gen_equipment_stage eq
      	 WHERE eq.status       = 'F'
    	     AND eq.service_point_id = eq1.service_point_id
    	     AND eq.status_message   = 'Duplicate SAP equipment ID'
    	) ;

   UPDATE pgedata.gen_summary_stage sm
   SET sm.status       = 'F',    sm.status_message = 'Generation failed due to Duplicate SAP equipment ID for any equipment'
   WHERE sm.status  IS NULL
        AND EXISTS
   	 (SELECT 2
       	 FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       = 'F'
    	AND eq.service_point_id = sm.service_point_id
    	AND eq.status_message   = 'Duplicate SAP equipment ID'
   	 ) ;

  -- Invalid gen_tech_cd
   UPDATE pgedata.gen_equipment_stage eq
   SET eq.status           = 'F',    eq.status_message     = 'Invalid GEN_TECH_CD'
    WHERE eq.status      IS NULL
        AND eq.gen_tech_cd NOT IN ('GEN.SYNCH', 'GEN.INDCT', 'GEN.INVEXT', 'GEN.INVINC', 'PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR') ;

   UPDATE pgedata.gen_equipment_stage eq1
   SET eq1.status       = 'F', eq1.status_message = 'Other equipments failed due to Invalid GEN_TECH_CD'
   WHERE eq1.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       = 'F'
    	     AND eq.service_point_id = eq1.service_point_id
    	     AND eq.status_message   = 'Invalid GEN_TECH_CD'
    	) ;

   UPDATE pgedata.gen_summary_stage sm
   SET sm.status       = 'F',    sm.status_message = 'Generation failed due to Invalid GEN_TECH_CD in equipment details'
    WHERE sm.status  IS NULL
         AND EXISTS
 	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       = 'F'
    	     AND eq.service_point_id = sm.service_point_id
    	     AND eq.status_message   = 'Invalid GEN_TECH_CD'
    	) ;

    UPDATE pgedata.gen_equipment_stage eq
   SET     eq.status_message   = 'No DC generator against INVERTER'
   WHERE eq.status    IS NULL
        AND gen_tech_cd      IN ('GEN.INVEXT', 'GEN.INVINC')
       AND Upper (eq.action) = 'I'
      AND NOT EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq2
      	WHERE eq2.status        IS NULL
    	    AND Upper (action)         = 'I'
    	    AND gen_tech_cd           IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
    	    AND eq2.service_point_id   = eq.service_point_id
    	    AND eq2.gen_tech_equipment = eq.sap_equipment_id
    	) ;

   UPDATE pgedata.gen_equipment_stage eq1
   SET  eq1.status_message = 'equipments failed due to No DC generator against INVERTER'
   WHERE eq1.status  IS NULL
        AND EXISTS
    	(SELECT 2
      	 FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status      IS NULL
    	     AND eq.service_point_id = eq1.service_point_id
    	     AND eq.status_message   = 'No DC generator against INVERTER'
  	  ) ;

   UPDATE pgedata.gen_summary_stage sm
   SET  sm.status_message = 'No DC generator against INVERTER'
   WHERE sm.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status     IS NULL
    	     AND eq.service_point_id = sm.service_point_id
    	    AND eq.status_message   = 'No DC generator against INVERTER'
    	) ;

   UPDATE pgedata.gen_equipment_stage eq
   SET  eq.status_message        = 'Null GEN_TECH_EQUIPMENT for DC generator'
   WHERE eq.status         IS NULL
       AND Upper (eq.action)      = 'I'
      AND eq.gen_tech_cd        IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
      AND eq.gen_tech_equipment IS NULL;

   UPDATE pgedata.gen_equipment_stage eq1
   SET eq1.status_message = 'equipments failed due to Null GEN_TECH_EQUIPMENT for attached DC generator'
   WHERE eq1.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       IS NULL
    	     AND eq.service_point_id = eq1.service_point_id
    	     AND eq.status_message   = 'Null GEN_TECH_EQUIPMENT for DC generator'
  	  ) ;

   UPDATE pgedata.gen_summary_stage sm
   SET  sm.status_message = 'Null GEN_TECH_EQUIPMENT for DC generator'
   WHERE sm.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       IS NULL
    	     AND eq.service_point_id = sm.service_point_id
    	    AND eq.status_message   = 'Null GEN_TECH_EQUIPMENT for DC generator'
    	) ;

   UPDATE pgedata.gen_equipment_stage eq
   SET eq.status       = 'F',    eq.status_message = 'No INVERTER against DC generator'
   WHERE eq.status  IS NULL
        AND gen_tech_cd    IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
       AND NOT EXISTS
    	(SELECT 2
      	 FROM pgedata.gen_equipment_stage eq2
      	WHERE eq2.status       IS NULL
    	     AND gen_tech_cd         IN ('GEN.INVEXT', 'GEN.INVINC')
    	    AND eq2.service_point_id = eq.service_point_id
   	 ) ;

   UPDATE pgedata.gen_equipment_stage eq1
   SET eq1.status       = 'F',    eq1.status_message = 'Other equipments failed due to No INVERTER against DC generator'
    WHERE eq1.status  IS NULL
        AND EXISTS
       	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       = 'F'
    	    AND eq.service_point_id = eq1.service_point_id
    	    AND eq.status_message   = 'No INVERTER against DC generator'
   	 ) ;

   UPDATE pgedata.gen_summary_stage sm
   SET sm.status       = 'F',   sm.status_message = 'Generation failed due to No INVERTER against DC generator'
   WHERE sm.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       = 'F'
    	     AND eq.service_point_id = sm.service_point_id
    	     AND eq.status_message   = 'No INVERTER against DC generator'
    	) ;

     UPDATE pgedata.gen_equipment_stage eq
   SET     eq.status_message = 'DC generator not attached with correct inverter'
   WHERE eq.status  IS NULL
        AND eq.gen_tech_cd IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
       AND NOT EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq2
      	WHERE eq2.status      IS NULL
    	AND eq2.gen_tech_cd     IN ( 'GEN.INVEXT', 'GEN.INVINC')
    	AND eq2.service_point_id = eq.service_point_id
    	AND eq2.sap_equipment_id = eq.gen_tech_equipment
    	) ;

   UPDATE pgedata.gen_equipment_stage eq1
   SET    eq1.status_message = 'DC generator not attached with correct inverter'
    WHERE eq1.status  IS NULL
         AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status      IS NULL
    	     AND eq.service_point_id = eq1.service_point_id
    	    AND eq.status_message   = 'DC generator not attached with correct inverter'
    	) ;

   UPDATE pgedata.gen_summary_stage sm
   SET   sm.status_message = 'DC generator not attached with correct inverter'
   WHERE sm.status  IS NULL
        AND EXISTS
    	(SELECT 2
       	FROM pgedata.gen_equipment_stage eq
      	WHERE eq.status       IS NULL
    	AND eq.service_point_id = sm.service_point_id
    	AND eq.status_message   = 'DC generator not attached with correct inverter'
    	) ;

--Adding validation to Fail Null GEN_TECH_CD Case Start
update gen_equipment_stage eq set eq.status='F', eq.status_message='Equipment Failed due Null GEN_TECH_CD' where gen_tech_cd is null;

update gen_summary_stage sm set sm.status='F', sm.status_message='Generation Failed due Null GEN_TECH_CD in Equipment'
where exists
(
select 2 FROM gen_equipment_stage eq where eq.status='F' and eq.gen_tech_cd is null and sm.service_point_id=eq.service_point_id
);

update  gen_equipment_stage eq set eq.status='F', eq.status_message='Other Equipment Failed due to Null GEN_TECH_CD'
where exists
(
select 2 from gen_equipment_stage eq1 where eq1.gen_tech_cd is null and eq1.status ='F' and eq.service_point_id=eq1.service_point_id
) and status is null;
--Adding validation to Fail Null GEN_TECH_CD Case End

  COMMIT;

END;
-----------------------------------------------------------------
-- Calculate Generation Type function
FUNCTION Pge_cal_gen_type_f  ( spid NVARCHAR2)  RETURN VARCHAR2    IS
  h_cnt NUMBER (10)        := 0;
  h_gen_type NVARCHAR2 (6) := NULL;
BEGIN
   SELECT COUNT (DISTINCT r.gen_tech_cd)
   INTO h_cnt
   FROM pgedata.pgedata_sm_generation_stage g
     JOIN pgedata.pgedata_sm_protection_stage p
        ON g.id = p.parent_id
     JOIN pgedata.pgedata_sm_generator_stage r
        ON p.id           = r.protection_id
   WHERE p.parent_type  = 'GENERATION'
        AND g.service_point_id = spid;

  IF h_cnt  = 1 THEN
      SELECT DISTINCT Upper (r.gen_tech_cd)
      INTO h_gen_type
      FROM pgedata.pgedata_sm_generation_stage g
        JOIN pgedata.pgedata_sm_protection_stage p
            ON g.id = p.parent_id
        JOIN pgedata.pgedata_sm_generator_stage r
          ON p.id           = r.protection_id
      WHERE p.parent_type  = 'GENERATION'
           AND g.service_point_id = spid;
  ELSIF (h_cnt  > 1) THEN
     		 h_gen_type  := 'MIXD';
  END IF;

 RETURN h_gen_type;

END;
---------------------------------------------------------------
-- Find project name
FUNCTION Pge_find_prj_name_f  ( guid CHAR)  RETURN VARCHAR2  IS
  h_cnt NUMBER (10)         := 0;
  h_prj_name NVARCHAR2 (85) := NULL;
BEGIN
  SELECT DISTINCT r.project_name
  INTO h_prj_name
  FROM pgedata.pgedata_sm_generation_stage g
    JOIN pgedata.pgedata_sm_protection_stage p
       ON g.id = p.parent_id
   JOIN pgedata.pgedata_sm_generator_stage r
       ON p.id            = r.protection_id
   WHERE r.project_name IS NOT NULL
       AND p.parent_type       = 'GENERATION'
       AND g.global_id         = guid
       AND ROWNUM              < 2;

RETURN h_prj_name;
END;
---------------------------------------------------------------------------------------------------------
--Convert the gen_tech_cd
FUNCTION Pge_gen_tech_cd_conv_f ( sap_val NVARCHAR2)  RETURN NVARCHAR2      IS
  h_sap_val NVARCHAR2 (11) := NULL;
  h_con_val NVARCHAR2 (6)  := NULL;
BEGIN
  h_sap_val := Upper (Trim (sap_val)) ;
  h_con_val :=
  CASE h_sap_val
  WHEN 'GEN.INDCT' THEN
    'INDCT'
  WHEN 'GEN.SYNCH' THEN
    'SYNCH'
  WHEN 'GEN.INVEXT' THEN
    'INVEXT'
  WHEN 'GEN.INVINC' THEN
    'INVINC'
  WHEN 'PWR.PVPNL' THEN
    'PVPNL'
  WHEN 'PWR.BATT' THEN
    'BATT'
  WHEN 'PWR.WIND' THEN
    'WIND'
  WHEN 'PWR.GENRTR' THEN
    'GENRTR'
  END;

  RETURN h_con_val;

END;
---------------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------------
--Convert the gen_type
FUNCTION Pge_gen_type_rev_conv ( sap_val NVARCHAR2,act NVARCHAR2)  RETURN NVARCHAR2      IS
  h_sap_val NVARCHAR2 (6) := NULL;
  h_con_val NVARCHAR2 (50)  := NULL;
  h_sap_val_update NVARCHAR2 (50) := NULL;
  h_con_val_update NVARCHAR2 (50)  := NULL;
  h_Val_To_Return NVARCHAR2 (50)  := NULL;
BEGIN

 if act = 'I'
  THEN

  h_sap_val := Upper (Trim (sap_val)) ;
  h_con_val :=
  CASE h_sap_val
    WHEN 'SYNCH' THEN
    'Synchronous'
    WHEN 'INVEXT' THEN
    'Inverter ? External'
    WHEN 'INVINC' THEN
    'Inverter ? Incorporated'
    WHEN 'INDCT' THEN
    'Induction'
    WHEN 'OTHR' THEN
    'Other'
    WHEN 'MIXD' THEN
    'Mixed'
    WHEN 'NONE' THEN
    'None'
  END;

   h_Val_To_Return := h_con_val;

  END IF;

 if act = 'U'
  THEN

  h_sap_val_update := Trim(sap_val);
  h_con_val_update :=
  CASE h_sap_val_update
    WHEN 'Synchronous' THEN
    'Synchronous'
    WHEN 'Inverter ? External' THEN
    'Inverter ? External'
    WHEN 'Inverter ? Incorporated' THEN
    'Inverter ? Incorporated'
    WHEN 'Induction' THEN
    'Induction'
    WHEN 'Other'  THEN
    'Other'
    WHEN 'Mixed'  THEN
    'Mixed'
    WHEN 'None' THEN
    'None'
  END;

  h_sap_val_update := Upper (Trim (sap_val)) ;
  h_con_val_update :=
  CASE h_sap_val_update
    WHEN 'SYNCH' THEN
    'Synchronous'
    WHEN 'INVEXT' THEN
    'Inverter ? External'
    WHEN 'INVINC' THEN
    'Inverter ? Incorporated'
    WHEN 'INDCT' THEN
    'Induction'
    WHEN 'OTHR' THEN
    'Other'
    WHEN 'MIXD' THEN
    'Mixed'
    WHEN 'NONE' THEN
    'None'
  END;

  h_Val_To_Return := h_con_val_update;

  END IF;

IF h_Val_To_Return is Null or Trim(h_Val_To_Return) = ''
THEN

h_Val_To_Return := sap_val;

END IF;

  RETURN h_Val_To_Return;

END;

---------------------------------------------------------------------------------------------------------------

-- Delete the records from the stage table
FUNCTION PGE_DELETE_STG2_DATA_SP  (input_dm_di_flag IN VARCHAR2)  RETURN VARCHAR2   IS
  h_delete_flg VARCHAR2 (10) := NULL;
BEGIN
  IF input_dm_di_flag IN ('DM', 'DI') THEN
    SAVEPOINT err_found;
    BEGIN
       DELETE FROM pgedata.pgedata_sm_gen_equipment_stage;
       DELETE FROM pgedata.pgedata_sm_generator_stage;
       DELETE FROM pgedata.pgedata_sm_protection_stage;
       DELETE FROM pgedata.pgedata_sm_generation_stage;
       DELETE FROM pgedata.pgedata_generationinfo_stage;
      COMMIT;
      h_delete_flg := 'TRUE';
    EXCEPTION
    WHEN OTHERS THEN
      ROLLBACK TO err_found;
      h_delete_flg := 'FALSE';
    END;
  ELSE
    h_delete_flg := 'FALSE';
  END IF;
  RETURN h_delete_flg;
END;
---------------------------------------------------------------------------------------------------------------
--Populate the stage 2 tables and update the status
PROCEDURE Pge_sap_2_stg2_data_mgrtn_sp ( outflag OUT VARCHAR2)    IS
  h_user NVARCHAR2 (15) := NULL;
BEGIN
   SELECT USER INTO h_user FROM dual;

   INSERT  INTO pgedata.pgedata_generationinfo_stage
    (
      globalid           ,
      service_point_id   ,
      servicepointguid   ,
      datecreated        ,
      gentype            ,
      createdby          ,
      sapeginotification ,
      projectname        ,
      programtype        ,
      effratingmachkw    ,
      effratinginvkw     ,
      effratingmachkva   ,
      effratinginvkva    ,
      backupgen          ,
      maxstoragecapacity ,
      chargedemandkw     ,
      gensymbology       ,
      powersource        ,
      servicelocationguid,
      status
    )
   SELECT Sys_guid ()      ,
    SM.service_point_id    ,
    SP.globalid            ,
    SYSDATE                ,
    NULL                   ,
    h_user                 ,
    SM.sap_egi_notification,
    SM.project_name        ,
    SM.program_type        ,
    SM.eff_rating_mach_kw  ,
    SM.eff_rating_inv_kw   ,
    SM.eff_rating_mach_kva ,
    SM.eff_rating_inv_kva  ,
    'N'                    ,
    SM.max_storage_capacity,
    SM.charge_demand_kw    ,
    CASE
      WHEN Trim (SP.primarymeterguid) IS NOT NULL
      THEN 'Primary'
      WHEN Trim (SP.transformerguid) IS NOT NULL
      THEN 'Secondary'
      ELSE ''
    END AS Gen_Syml       ,
    SM.power_source       ,
    SP.servicelocationguid,
    'S'
     FROM pgedata.gen_summary_stage SM
     INNER JOIN edgis.zz_mv_servicepoint SP
       ON SM.service_point_id = SP.servicepointid
     WHERE SM.status  IS NULL;

  INSERT
     INTO pgedata.pgedata_sm_generation_stage
    (
      service_point_id    ,
      global_id           ,
      sap_egi_notification,
      project_name        ,
      power_source        ,
      eff_rating_mach_kw  ,
      eff_rating_inv_kw   ,
      eff_rating_mach_kva ,
      eff_rating_inv_kva  ,
      max_storage_capacity,
      charge_demand_kw    ,
      program_type        ,
      datecreated         ,
      gen_type            ,
      createdby           ,
      export_kw           ,
      direct_transfer_trip,
      status
    )
   SELECT service_point_id                               ,
    globalid                                             ,
    sapeginotification                                   ,
    projectname                                          ,
    powersource                                          ,
    effratingmachkw                                      ,
    effratinginvkw                                       ,
    effratingmachkva                                     ,
    effratinginvkva                                      ,
    maxstoragecapacity                                   ,
    chargedemandkw                                       ,
    programtype                                          ,
    datecreated                                          ,
    gentype                                              ,
    createdby                                            ,
    (NVL (effratingmachkw, 0) + NVL (effratinginvkw, 0)),
    'N'                                                  ,
    status
    FROM pgedata.pgedata_generationinfo_stage;

   INSERT
     INTO pgedata.pgedata_sm_protection_stage
    (
      parent_type    ,
      parent_id      ,
      protection_type,
      datecreated    ,
      createdby      ,
      export_kw      ,
      notes          ,
      status
    )
   SELECT 'GENERATION',
    A.id              ,
    'UNSP'            ,
    A.datecreated     ,
    A.createdby       ,
    A.export_kw       ,
    A.notes           ,
    A.status
   FROM pgedata.pgedata_sm_generation_stage A;

   INSERT
     INTO pgedata.pgedata_sm_generator_stage
    (
      sap_equipment_id    ,
      protection_id       ,
      sap_queue_number    ,
      sap_egi_notification,
      datecreated         ,
      createdby           ,
      power_source        ,
      project_name        ,
      manufacturer        ,
      model               ,
      inverter_efficiency ,
      nameplate_rating    ,
      quantity            ,
      power_factor        ,
      eff_rating_kw       ,
      eff_rating_kva      ,
      rated_voltage       ,
      number_of_phases    ,
      gen_tech_cd         ,
      pto_date            ,
      program_type        ,
      control_cd          ,
      connection_cd       ,
      status_cd           ,
      ss_reactance        ,
      ss_resistance       ,
      trans_reactance     ,
      trans_resistnace    ,
      subtrans_reactance  ,
      subtrans_resistance ,
      neg_reactance       ,
      neg_resistance      ,
      zero_reactance      ,
      zero_resistance     ,
      grd_reactance       ,
      grd_resistance      ,
      enos_equip_ref      ,
      enos_proj_ref       ,
      gen_tech_equipment  ,
      certification       ,
      tech_type_cd        ,
      status
    )
   SELECT sap_equipment_id                                       ,
    A.id                                                         ,
    Eqp.sap_queue_number                                         ,
    Eqp.sap_egi_notification                                     ,
    SYSDATE                                                      ,
    h_user                                                       ,
    Eqp.power_source                                             ,
    Eqp.project_name                                             ,
    Eqp.manufacturer                                             ,
    Eqp.model                                                    ,
    Eqp.inverter_efficiency                                      ,
    Eqp.nameplate_rating                                         ,
    Eqp.quantity                                                 ,
    Eqp.power_factor                                             ,
    Eqp.eff_rating_kw                                            ,
    Eqp.eff_rating_kva                                           ,
    Eqp.rated_voltage                                            ,
    Eqp.number_of_phases                                         ,
    pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (gen_tech_cd),
    Eqp.pto_date                                                 ,
    Eqp.program_type                                             ,
    'NONE'                                                       ,
    NULL                                                         ,
    'UNSP'                                                       ,
    Eqp.ss_reactance                                             ,
    Eqp.ss_resistance                                            ,
    Eqp.trans_reactance                                          ,
    Eqp.trans_resistance                                         ,
    Eqp.subtrans_reactance                                       ,
    Eqp.subtrans_resistance                                      ,
    Eqp.neg_reactance                                            ,
    Eqp.neg_resistance                                           ,
    Eqp.zero_reactance                                           ,
    Eqp.zero_resistance                                          ,
    Eqp.grd_reactance                                            ,
    Eqp.grd_resistance                                           ,
    Eqp.enos_equip_ref                                           ,
    Eqp.enos_proj_ref                                            ,
    Eqp.gen_tech_equipment                                       ,
    Eqp.certification                                            ,
    Eqp.tech_type_cd                                             ,
    'S'
     FROM pgedata.gen_equipment_stage Eqp
     INNER JOIN pgedata.pgedata_sm_generation_stage B
       ON Eqp.service_point_id = B.service_point_id
     INNER JOIN pgedata.pgedata_sm_protection_stage A
       ON A.parent_id = B.id
     WHERE Eqp.status IS NULL
       AND Eqp.gen_tech_cd = 'GEN.SYNCH'
       AND A.parent_type   = 'GENERATION';

   INSERT
   INTO pgedata.pgedata_sm_generator_stage
    (
      sap_equipment_id    ,
      protection_id       ,
      sap_queue_number    ,
      sap_egi_notification,
      datecreated         ,
      createdby           ,
      power_source        ,
      project_name        ,
      manufacturer        ,
      model               ,
      inverter_efficiency ,
      nameplate_rating    ,
      quantity            ,
      power_factor        ,
      eff_rating_kw       ,
      eff_rating_kva      ,
      rated_voltage       ,
      number_of_phases    ,
      gen_tech_cd         ,
      pto_date            ,
      program_type        ,
      control_cd          ,
      connection_cd       ,
      status_cd           ,
      ss_reactance        ,
      ss_resistance       ,
      trans_reactance     ,
      trans_resistnace    ,
      subtrans_reactance  ,
      subtrans_resistance ,
      neg_reactance       ,
      neg_resistance      ,
      zero_reactance      ,
      zero_resistance     ,
      grd_reactance       ,
      grd_resistance      ,
      enos_equip_ref      ,
      enos_proj_ref       ,
      gen_tech_equipment  ,
      certification       ,
      tech_type_cd        ,
      status
    )
   SELECT sap_equipment_id                                       ,
    A.id                                                         ,
    Eqp.sap_queue_number                                         ,
    Eqp.sap_egi_notification                                     ,
    SYSDATE                                                      ,
    h_user                                                       ,
    Eqp.power_source                                             ,
    Eqp.project_name                                             ,
    Eqp.manufacturer                                             ,
    Eqp.model                                                    ,
    Eqp.inverter_efficiency                                      ,
    Eqp.nameplate_rating                                         ,
    Eqp.quantity                                                 ,
    Eqp.power_factor                                             ,
    Eqp.eff_rating_kw                                            ,
    Eqp.eff_rating_kva                                           ,
    Eqp.rated_voltage                                            ,
    Eqp.number_of_phases                                         ,
    pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (gen_tech_cd),
    Eqp.pto_date                                                 ,
    Eqp.program_type                                             ,
    'NONE'                                                       ,
    NULL                                                         ,
    'UNSP'                                                       ,
    Eqp.ss_reactance                                             ,
    Eqp.ss_resistance                                            ,
    Eqp.trans_reactance                                          ,
    Eqp.trans_resistance                                         ,
    Eqp.subtrans_reactance                                       ,
    Eqp.subtrans_resistance                                      ,
    Eqp.neg_reactance                                            ,
    Eqp.neg_resistance                                           ,
    Eqp.zero_reactance                                           ,
    Eqp.zero_resistance                                          ,
    Eqp.grd_reactance                                            ,
    Eqp.grd_resistance                                           ,
    Eqp.enos_equip_ref                                           ,
    Eqp.enos_proj_ref                                            ,
    Eqp.gen_tech_equipment                                       ,
    Eqp.certification                                            ,
    Eqp.tech_type_cd                                             ,
    'S'
     FROM pgedata.gen_equipment_stage Eqp
     INNER JOIN pgedata.pgedata_sm_generation_stage B
                    ON Eqp.service_point_id = B.service_point_id
     INNER JOIN pgedata.pgedata_sm_protection_stage A
                   ON A.parent_id = B.id
     WHERE Eqp.status IS NULL
         AND Eqp.gen_tech_cd = 'GEN.INDCT'
         AND A.parent_type   = 'GENERATION';

   INSERT
   INTO pgedata.pgedata_sm_generator_stage
    (
      sap_equipment_id    ,
      protection_id       ,
      sap_queue_number    ,
      sap_egi_notification,
      datecreated         ,
      createdby           ,
      power_source        ,
      project_name        ,
      manufacturer        ,
      model               ,
      inverter_efficiency ,
      nameplate_rating    ,
      quantity            ,
      power_factor        ,
      eff_rating_kw       ,
      eff_rating_kva      ,
      rated_voltage       ,
      number_of_phases    ,
      mode_of_inverter    ,
      gen_tech_cd         ,
      pto_date            ,
      program_type        ,
      control_cd          ,
      connection_cd       ,
      status_cd           ,
      enos_equip_ref      ,
      enos_proj_ref       ,
      gen_tech_equipment  ,
      certification       ,
      tech_type_cd        ,
      status
    )
   SELECT sap_equipment_id                                       ,
    A.id                                                         ,
    Eqp.sap_queue_number                                         ,
    Eqp.sap_egi_notification                                     ,
    SYSDATE                                                      ,
    h_user                                                       ,
    Eqp.power_source                                             ,
    Eqp.project_name                                             ,
    Eqp.manufacturer                                             ,
    Eqp.model                                                    ,
    Eqp.inverter_efficiency                                      ,
    Eqp.nameplate_rating                                         ,
    Eqp.quantity                                                 ,
    Eqp.power_factor                                             ,
    Eqp.eff_rating_kw                                            ,
    Eqp.eff_rating_kva                                           ,
    Eqp.rated_voltage                                            ,
    Eqp.number_of_phases                                         ,
    Eqp.mode_of_inv                                              ,
    pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (gen_tech_cd),
    Eqp.pto_date                                                 ,
    Eqp.program_type                                             ,
    'NONE'                                                       ,
    NULL                                                         ,
    'UNSP'                                                       ,
    Eqp.enos_equip_ref                                           ,
    Eqp.enos_proj_ref                                            ,
    Eqp.gen_tech_equipment                                       ,
    Eqp.certification                                            ,
    Eqp.tech_type_cd                                             ,
    'S'
     FROM pgedata.gen_equipment_stage Eqp
     INNER JOIN pgedata.pgedata_sm_generation_stage B
             ON Eqp.service_point_id = B.service_point_id
     INNER JOIN pgedata.pgedata_sm_protection_stage A
             ON A.parent_id  = B.id
     WHERE Eqp.status  IS NULL
       AND Eqp.gen_tech_cd IN ('GEN.INVEXT', 'GEN.INVINC')
       AND A.parent_type    = 'GENERATION';

   INSERT
   INTO pgedata.pgedata_sm_gen_equipment_stage
    (
      generator_id        ,
      sap_equipment_id    ,
      datecreated         ,
      createdby           ,
      gen_tech_cd         ,
      manufacturer        ,
      model               ,
      ptc_rated_kw        ,
      nameplate_rating    ,
      quantity            ,
      max_storage_capacity,
      rated_discharge     ,
      charge_demand_kw    ,
      grid_charged        ,
      enos_equip_ref      ,
      enos_proj_ref       ,
      status              ,
      program_type
    )
   SELECT C.id                                                       ,
    Eqp.sap_equipment_id                                             ,
    SYSDATE                                                          ,
    h_user                                                           ,
    pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (Eqp.gen_tech_cd),
    Eqp.manufacturer                                                 ,
    Eqp.model                                                        ,
    Eqp.ptc_rating                                                   ,
    Eqp.nameplate_rating                                             ,
    Eqp.quantity                                                     ,
    Eqp.max_storage_capacity                                         ,
    Eqp.rated_discharge                                              ,
    Eqp.charge_demand_kw                                             ,
    Eqp.grid_charged                                                 ,
    Eqp.enos_equip_ref                                               ,
    Eqp.enos_proj_ref                                                ,
    'S'                                                              ,
    Eqp.program_type
     FROM pgedata.gen_equipment_stage Eqp
     INNER JOIN pgedata.pgedata_sm_generation_stage B
             ON Eqp.service_point_id = B.service_point_id
     INNER JOIN pgedata.pgedata_sm_protection_stage A
             ON A.parent_id = B.id
     INNER JOIN pgedata.pgedata_sm_generator_stage C
             ON C.protection_id    = A.id
     WHERE Eqp.status   IS NULL
           AND Eqp.gen_tech_equipment = C.sap_equipment_id
           AND Eqp.gen_tech_cd       IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
           AND A.parent_type          = 'GENERATION'
           AND C.gen_tech_cd         IN ('INVEXT', 'INVINC') ;

  -- Udpate the flag in stage table
  merge INTO pgedata.gen_equipment_stage stg_1 USING pgedata.pgedata_sm_generator_stage stg_2 ON (stg_1.sap_equipment_id = stg_2.sap_equipment_id)
  WHEN matched THEN
  UPDATE
  SET stg_1.status = 'S',  status_message  = 'Successfully Generator created'
  WHERE stg_1.status IS NULL;

  merge INTO pgedata.gen_equipment_stage stg_1 USING pgedata.pgedata_sm_gen_equipment_stage stg_2 ON (stg_1.sap_equipment_id = stg_2.sap_equipment_id)
  WHEN matched THEN
  UPDATE
  SET stg_1.status = 'S',    status_message  = 'Successfully Generator created'
  WHERE stg_1.status IS NULL;

  merge INTO pgedata.gen_summary_stage stg_1 USING pgedata.pgedata_sm_generation_stage stg_2 ON (stg_1.service_point_id = stg_2.service_point_id)
  WHEN matched THEN
  UPDATE
  SET stg_1.status = 'S',    status_message  = 'Successfully Generation created'
  WHERE stg_1.status IS NULL;

  -- Update generation category. mixed or a particualr type
  UPDATE pgedata.pgedata_generationinfo_stage
  SET gentype = pge_enos_to_edgis_dm_pkg.Pge_cal_gen_type_f (service_point_id) ;

  merge INTO pgedata.pgedata_sm_generation_stage trgt USING pgedata.pgedata_generationinfo_stage src ON (trgt.global_id = src.globalid)
  WHEN matched THEN
  UPDATE SET trgt.gen_type = src.gentype;

COMMIT;
outflag := 'TRUE';
END;
--------------------------------------------------------------------------------------------------------------------------------
-- Validate the record with the CEDSA data

PROCEDURE Pge_cedsa_data_vldtn_sp IS
BEGIN
  -- Delete the data from the temporary table

   DELETE FROM pgedata.pgedata_sap_cedsa_migration;


    -- Update the flag for CEDSA match
   UPDATE pgedata.pgedata_sm_generation_stage sm
   SET sm.status_message = 'SP ID not exist in CEDSA', sm.cedsa_match_found  = 'N'
   WHERE sm.cedsa_match_found IS NULL
     AND NOT EXISTS
    	(SELECT 2
       	FROM pgedata.generation_conv cds
    	  JOIN pgedata.meter_conv mtr
          ON mtr.meter_id         = cds.meter_id
      	WHERE mtr.service_point_id = sm.service_point_id
   	 ) ;

   UPDATE pgedata.pgedata_sm_generator_stage
   SET status_message = 'Generation SP ID not exist in CEDSA', cedsa_match_found = 'N'
   WHERE cedsa_match_found IS NULL
     AND protection_id  IN
   	 (SELECT p.id
      FROM pgedata.pgedata_sm_protection_stage p
   	  JOIN pgedata.pgedata_sm_generation_stage s
        ON p.parent_id     = s.id
      WHERE p.parent_type   = 'GENERATION'
    	  AND s.cedsa_match_found = 'N'
        AND s.status_message    = 'SP ID not exist in CEDSA'
    	) ;

   UPDATE pgedata.pgedata_sm_gen_equipment_stage
   SET status_message = 'Generation SP ID not exist in CEDSA',   cedsa_match_found = 'N'
   WHERE cedsa_match_found IS NULL
     AND generator_id IN
   	 (SELECT id
      FROM pgedata.pgedata_sm_generator_stage
    	WHERE gen_tech_cd  IN ('INVE', 'INVI')
        AND cedsa_match_found = 'N'
        AND status_message    = 'Generation SP ID not exist in CEDSA'
  	  ) ;
  COMMIT;

END;
--------------------------------------------------------------------------------------------------------------------------------
--Update the generation information with the data in CEDSA
PROCEDURE Pge_cedsa_2_sap_process_sp      IS
  h_cedsa_gen_id NUMBER (10)        := 0;
  h_sap_spid NVARCHAR2 (10)         := 0;
  h_notes NVARCHAR2 (2000)          := NULL;
  h_notes_gntr NVARCHAR2 (2000)     := NULL;
  h_cedsa_protc_id   NUMBER (10)    := 0;
  h_sap_new_protc_id NUMBER (10)    := 0;
  h_user             VARCHAR2 (20)  := NULL;
  h_cnt_1            NUMBER (10)    := 0;
  h_cnt_2            NUMBER (10)    := 0;
  h_cnt_3            NUMBER (10)    := 0;
  h_cnt_4            NUMBER (10)    := 0;
  h_cnt_5            NUMBER (10)    := 0;
  h_cnt_6            NUMBER (10)    := 0;
  h_cnt_7            NUMBER (10)    := 0;
  h_sap_manf         VARCHAR2 (100) := NULL;
  h_sap_mdl          VARCHAR2 (100) := NULL;
  h_sap_qnty         NUMBER (6)     := 0;
  h_sap_gen_tech NVARCHAR2 (6)      := NULL;
  h_sap_enos_eq_ref VARCHAR2 (10)   := NULL;
  h_id_gntr         NUMBER (10)     := 0;
  h_id_gntr_eqp     NUMBER (10)     := 0;
  h_cnt_mig         NUMBER (10)     := 0;
  h_status_cd NVARCHAR2 (4)         := NULL;
TYPE t_bulk_sap
IS
  TABLE OF pgedata.pgedata_sm_generator_stage%ROWTYPE;
  v_bulk_eq T_BULK_SAP := T_bulk_sap () ;
TYPE t_bulk_sap_2
IS
  TABLE OF pgedata.pgedata_sm_gen_equipment_stage%ROWTYPE;
  v_bulk_eq_2 T_BULK_SAP_2 := T_bulk_sap_2 () ;

BEGIN --begin_1
   SELECT USER
     INTO h_user
     FROM dual;

  FOR i IN
  (SELECT id,
    service_point_id
     FROM pgedata.pgedata_sm_generation_stage
    WHERE cedsa_match_found IS NULL
  )
  LOOP
    h_sap_spid := i.service_point_id;
    BEGIN --begin_2
	       -- Select the generation id and notes from the CEDSA
       SELECT cds.generation_id,    cds.notes
       INTO h_cedsa_gen_id,    h_notes
       FROM pgedata.generation_conv cds
         JOIN pgedata.meter_conv mtr
           ON mtr.meter_id         = cds.meter_id
       WHERE mtr.service_point_id = h_sap_spid;

      -- Update the notes and flag
      IF h_notes  IS NOT NULL THEN
        UPDATE pgedata.pgedata_sm_generation_stage
        SET notes           = h_notes,  cedsa_match_found = 'Y'
        WHERE id          = i.id;
      ELSE
        UPDATE pgedata.pgedata_sm_generation_stage
        SET cedsa_match_found = 'N'
        WHERE id            = i.id;
      END IF;

      BEGIN --begin_3
	        -- Find the count of equipment etc in CEDSA and compare with the data received from SAP
         SELECT id
         INTO h_sap_new_protc_id
         FROM pgedata.pgedata_sm_protection_stage
         WHERE parent_type = 'GENERATION'
           AND parent_id       = i.id;

         SELECT protection_id
         INTO h_cedsa_protc_id
         FROM pgedata.protection_conv
         WHERE generation_id = h_cedsa_gen_id;

         SELECT COUNT ( *)
         INTO h_cnt_1
         FROM pgedata.induction_conv
         WHERE protection_id = h_cedsa_protc_id;

         SELECT COUNT ( *)
         INTO h_cnt_2
         FROM pgedata.synchronous_conv
         WHERE protection_id = h_cedsa_protc_id;

         SELECT COUNT ( *)
         INTO h_cnt_3
         FROM pgedata.inverter_conv
         WHERE protection_id = h_cedsa_protc_id;

         SELECT COUNT ( *)
         INTO h_cnt_4
         FROM pgedata.dc_generation_conv
         WHERE protection_id = h_cedsa_protc_id;

         h_cnt_7 := (h_cnt_1 + h_cnt_2 + h_cnt_3) ;

         SELECT COUNT ( *)
         INTO h_cnt_5
         FROM pgedata.pgedata_sm_generator_stage g
           JOIN pgedata.pgedata_sm_protection_stage p
             ON g.protection_id = p.id
         WHERE p.parent_type   = 'GENERATION'
           AND p.parent_id     = i.id;

         SELECT COUNT (A.generator_id)
         INTO h_cnt_6
         FROM pgedata.pgedata_sm_gen_equipment_stage A
            JOIN pgedata.pgedata_sm_generator_stage B
               ON A.generator_id = B.id
            JOIN pgedata.pgedata_sm_protection_stage C
               ON B.protection_id = C.id
          WHERE B.gen_tech_cd  IN ('INVINC', 'INVEXT')
               AND C.parent_id         = i.id
               AND C.parent_type       = 'GENERATION';

          IF (h_cnt_5  < h_cnt_7) THEN
	          --additional generation in CEDSA
           INSERT  INTO pgedata.pgedata_sap_cedsa_migration
           (
              spid         ,
              cedsa_gen_id ,
              sap_gen_id   ,
              enos_equip_id,
              status       ,
              MESSAGE      ,
              gen_tech     ,
              generator_id
            )
           SELECT h_sap_spid                ,
            h_cedsa_gen_id                  ,
            i.id                            ,
            enos_eqp_id                     ,
            NULL                            ,
            'Additional Generation in CEDSA',
            'INV'                           ,
            inverter_id
             FROM pgedata.inverter_conv
            WHERE protection_id = h_cedsa_protc_id
                 AND enos_eqp_id  IN
                  (SELECT enos_eqp_id
                  FROM pgedata.inverter_conv
                  WHERE protection_id = h_cedsa_protc_id
                  MINUS
                  SELECT A.enos_equip_ref
                  FROM pgedata.pgedata_sm_generator_stage A
                    JOIN pgedata.pgedata_sm_protection_stage B
                      ON A.protection_id = B.id
                  WHERE A.gen_tech_cd  IN ('INVEXT', 'INVINC')
                    AND B.parent_type       = 'GENERATION'
                    AND B.parent_id         = i.id
                   ) ;

           -- Check for any existing record in temporary table
           SELECT COUNT ( *)
           INTO h_cnt_mig
           FROM pgedata.pgedata_sap_cedsa_migration
           WHERE spid     = h_sap_spid
             AND cedsa_gen_id = h_cedsa_gen_id
             AND MESSAGE      = 'Additional Generation in CEDSA'
             AND gen_tech     = 'INV'
             AND sap_gen_id   = i.id;

	         -- If no record then insert the record
            IF h_cnt_mig   = 0 THEN
             INSERT INTO pgedata.pgedata_sap_cedsa_migration
              (
                spid         ,
                cedsa_gen_id ,
                sap_gen_id   ,
                enos_equip_id,
                status       ,
                MESSAGE      ,
                gen_tech     ,
                generator_id ,
                manufacturer ,
                model        ,
                quantity
              )
             SELECT h_sap_spid                ,
              h_cedsa_gen_id                  ,
              i.id                            ,
              enos_eqp_id                     ,
              NULL                            ,
              'Additional Generation in CEDSA',
              'INV'                           ,
              inverter_id                     ,
              manf_cd                         ,
              model_cd                        ,
              quantity
             FROM pgedata.inverter_conv
             WHERE protection_id              = h_cedsa_protc_id
               AND (manf_cd, model_cd, quantity) IN
                (SELECT manf_cd,
                  model_cd     ,
                  quantity
                 FROM pgedata.inverter_conv
                 WHERE protection_id = h_cedsa_protc_id
                 MINUS
                 SELECT A.manufacturer,
                  A.model             ,
                  A.quantity
                 FROM pgedata.pgedata_sm_generator_stage A
                  JOIN pgedata.pgedata_sm_protection_stage B
                    ON A.protection_id = B.id
                 WHERE A.gen_tech_cd  IN ('INVEXT', 'INVINC')
                     AND B.parent_type       = 'GENERATION'
                     AND B.parent_id         = i.id
                 ) ;
            END IF;

              INSERT   INTO pgedata.pgedata_sap_cedsa_migration
              (
               spid         ,
               cedsa_gen_id ,
               sap_gen_id   ,
               enos_equip_id,
               status       ,
               MESSAGE      ,
               gen_tech     ,
               generator_id ,
               manufacturer ,
               model
               )
              SELECT h_sap_spid                ,
               h_cedsa_gen_id                  ,
               i.id                            ,
               generator_id                    ,
               NULL                            ,
               'Additional Generation in CEDSA',
               'IND'                           ,
               generator_id                    ,
               manf_cd                         ,
               model_cd
              FROM pgedata.induction_conv
              WHERE protection_id    = h_cedsa_protc_id
               AND (manf_cd, model_cd) IN
             		(SELECT manf_cd,
              			model_cd
                 FROM pgedata.induction_conv
              	 WHERE protection_id = h_cedsa_protc_id
                 MINUS
             		 SELECT A.manufacturer,
                      A.model
               	 FROM pgedata.pgedata_sm_generator_stage A
            	  	  JOIN pgedata.pgedata_sm_protection_stage B
                      ON A.protection_id = B.id
             		 WHERE A.gen_tech_cd   = 'INDCT'
            	     AND B.parent_type       = 'GENERATION'
            	     AND B.parent_id         = i.id
            		 ) ;

              INSERT INTO pgedata.pgedata_sap_cedsa_migration
              (
               spid         ,
               cedsa_gen_id ,
               sap_gen_id   ,
               enos_equip_id,
               status       ,
               MESSAGE      ,
               gen_tech     ,
               generator_id ,
               manufacturer ,
               model
              )
              SELECT h_sap_spid                ,
               h_cedsa_gen_id                  ,
               i.id                            ,
               NULL                            ,
               NULL                            ,
               'Additional Generation in CEDSA',
               'SYN'                           ,
               generator_id                    ,
               manf_cd                         ,
               model_cd
              FROM pgedata.synchronous_conv
              WHERE protection_id    = h_cedsa_protc_id
          	   AND (manf_cd, model_cd) IN
                  (SELECT manf_cd,
              			model_cd
                   FROM pgedata.synchronous_conv
                   WHERE protection_id = h_cedsa_protc_id
                   MINUS
                   SELECT A.manufacturer,
              			A.model
                   FROM pgedata.pgedata_sm_generator_stage A
            		   JOIN pgedata.pgedata_sm_protection_stage B
                  	  ON A.protection_id = B.id
               		WHERE A.gen_tech_cd   = 'SYNCH'
            		    AND B.parent_type       = 'GENERATION'
            		    AND B.parent_id         = i.id
                  ) ;

          ELSIF (h_cnt_6 < h_cnt_4) THEN
            INSERT  INTO pgedata.pgedata_sap_cedsa_migration
            (
              spid         ,
              cedsa_gen_id ,
              sap_gen_id   ,
              enos_equip_id,
              status       ,
              MESSAGE      ,
              gen_tech     ,
              generator_id
            )
            SELECT h_sap_spid                ,
             h_cedsa_gen_id                  ,
             i.id                            ,
             enos_eqp_id                     ,
             NULL                            ,
             'Additional Generator in CEDSA',
             'DC'                            ,
             generator_id
            FROM pgedata.dc_generation_conv
            WHERE protection_id = h_cedsa_protc_id
          	AND enos_eqp_id      IN
                  (SELECT enos_eqp_id
                   FROM pgedata.dc_generation_conv
               		 WHERE protection_id = h_cedsa_protc_id
                   MINUS
                   SELECT A.enos_equip_ref
                   FROM pgedata.pgedata_sm_gen_equipment_stage A
            		   JOIN pgedata.pgedata_sm_generator_stage B
                    ON A.generator_id = B.id
                   JOIN pgedata.pgedata_sm_protection_stage C
                 	  ON B.protection_id = C.id
              		 WHERE B.gen_tech_cd  IN ('INVEXT', 'INVINC')
            		     AND C.parent_type       = 'GENERATION'
            		     AND C.parent_id         = i.id
                    ) ;

             SELECT COUNT ( *)
             INTO h_cnt_mig
             FROM pgedata.pgedata_sap_cedsa_migration
             WHERE spid     = h_sap_spid
          	   AND cedsa_gen_id = h_cedsa_gen_id
          	   AND MESSAGE      = 'Additional Generator in CEDSA'
          	   AND gen_tech     = 'DC'
               AND sap_gen_id   = i.id;

            IF h_cnt_mig  = 0 THEN
              INSERT INTO pgedata.pgedata_sap_cedsa_migration
              (
                spid         ,
                cedsa_gen_id ,
                sap_gen_id   ,
                enos_equip_id,
                status       ,
                MESSAGE      ,
                gen_tech     ,
                generator_id ,
                manufacturer ,
                model        ,
                quantity
              )
             SELECT h_sap_spid                ,
               h_cedsa_gen_id                  ,
               i.id                            ,
               enos_eqp_id                     ,
               NULL                            ,
               'Additional Generator in CEDSA',
               'DC'                            ,
               generator_id                    ,
               manf_cd                         ,
               model_cd                        ,
               quantity
              FROM pgedata.dc_generation_conv
              WHERE protection_id  = h_cedsa_protc_id
               AND (manf_cd, model_cd, quantity) IN
              		(SELECT manf_cd,
                 		 model_cd     ,
                		 quantity
                   FROM pgedata.dc_generation_conv
                	 WHERE protection_id = h_cedsa_protc_id
                  MINUS
                   SELECT a.manufacturer,
                		A.model             ,
                		A.quantity
                 	 FROM pgedata.pgedata_sm_gen_equipment_stage A
              		 JOIN pgedata.pgedata_sm_generator_stage B
                     ON A.generator_id = B.id
              		 JOIN pgedata.pgedata_sm_protection_stage C
                     ON B.protection_id = C.id
                	 WHERE B.gen_tech_cd  IN ('INVEXT', 'INVINC')
              		   AND C.parent_type       = 'GENERATION'
              		   AND C.parent_id         = i.id
              		 ) ;
            END IF;
          END IF;

      -- Find the match for generator in CEDSA
        SELECT * bulk collect
        INTO v_bulk_eq
        FROM pgedata.pgedata_sm_generator_stage
        WHERE protection_id IN
          (SELECT id
            FROM pgedata.pgedata_sm_protection_stage
            WHERE parent_type = 'GENERATION'
              AND parent_id       = i.id
           ) ;

        FOR k IN 1 .. v_bulk_eq.count
        LOOP
          h_id_gntr         := V_bulk_eq (k) .id;
          h_sap_gen_tech    := V_bulk_eq (k) .gen_tech_cd;
          h_sap_manf        := V_bulk_eq (k) .manufacturer;
          h_sap_mdl         := V_bulk_eq (k) .model;
          h_sap_qnty        := V_bulk_eq (k) .quantity;
          h_sap_enos_eq_ref := V_bulk_eq (k) .enos_equip_ref;
          h_status_cd       := NULL;

         IF h_sap_gen_tech IN ('INVEXT', 'INVINC') THEN -- INVERTER
            BEGIN                                    --4
              -- Find match based on enos_ref_id
               SELECT notes,   status_cd
                 INTO h_notes_gntr,     h_status_cd
                 FROM pgedata.inverter_conv
                WHERE notes          IS NOT NULL
                     AND Trim (enos_eqp_id) IS NOT NULL
                     AND protection_id       = h_cedsa_protc_id
                     AND enos_eqp_id         = h_sap_enos_eq_ref;

              --- Update the notes
               UPDATE pgedata.pgedata_sm_generator_stage
               SET notes           = h_notes_gntr      ,
                      status_cd         = h_status_cd       ,
                      protection_id     = h_sap_new_protc_id,
                     cedsa_match_found = 'Y'
                WHERE id  = h_id_gntr;

            EXCEPTION
            WHEN no_data_found THEN
              -- when match not found based on enos_ref. Try to match with the other parameters
              BEGIN --5
                 SELECT notes,   status_cd
                   INTO h_notes_gntr,   h_status_cd
                   FROM pgedata.inverter_conv
                   WHERE notes    IS NOT NULL
                         AND protection_id = h_cedsa_protc_id
                         AND manf_cd       = h_sap_manf
                         AND model_cd      = h_sap_mdl
                         AND quantity      = h_sap_qnty;

                  UPDATE pgedata.pgedata_sm_generator_stage
                         SET notes           = h_notes_gntr      ,
                 	 status_cd         = h_status_cd       ,
                  	protection_id     = h_sap_new_protc_id,
                  	cedsa_match_found = 'Y'
                  WHERE id          = h_id_gntr;

              EXCEPTION
              WHEN no_data_found THEN
                -- No match found
                 UPDATE pgedata.pgedata_sm_generator_stage
                 SET cedsa_match_found = 'N'
                  WHERE id            = h_id_gntr;

              WHEN OTHERS THEN
                 UPDATE pgedata.pgedata_sm_generator_stage
                 SET cedsa_match_found = 'N',  status_message      = 'Other error'
                 WHERE id            = h_id_gntr;
              END; --5
            WHEN OTHERS THEN
              UPDATE pgedata.pgedata_sm_generator_stage
              SET cedsa_match_found = 'N',   status_message      = 'Other error'
               WHERE id            = h_id_gntr;
            END; --4

	--Check for equipment record now
            SELECT * bulk collect
            INTO v_bulk_eq_2
            FROM pgedata.pgedata_sm_gen_equipment_stage
            WHERE generator_id = V_bulk_eq (k) .id;

            FOR indx IN 1 .. v_bulk_eq_2.count
            LOOP
              h_id_gntr_eqp     := V_bulk_eq_2 (indx) .id;
              h_sap_enos_eq_ref := V_bulk_eq_2 (indx) .enos_equip_ref;
              h_sap_manf        := V_bulk_eq_2 (indx) .manufacturer;
              h_sap_mdl         := V_bulk_eq_2 (indx) .model;
              h_sap_qnty        := V_bulk_eq_2 (indx) .quantity;
            BEGIN         --6
	            -- Try to find match with enos_equip_id
             SELECT notes
             INTO h_notes_gntr
             FROM pgedata.dc_generation_conv
             WHERE notes          IS NOT NULL
                   AND Trim (enos_eqp_id) IS NOT NULL
                   AND protection_id       = h_cedsa_protc_id
                   AND enos_eqp_id         = h_sap_enos_eq_ref;

             UPDATE pgedata.pgedata_sm_gen_equipment_stage
             SET notes           = h_notes_gntr,   cedsa_match_found = 'Y'
             WHERE id          = h_id_gntr_eqp;
            EXCEPTION
            WHEN no_data_found THEN
              BEGIN --7
		-- Try to find match based on other parameters
                SELECT notes
                INTO h_notes_gntr
                FROM pgedata.dc_generation_conv
                WHERE notes    IS NOT NULL
                     AND protection_id = h_cedsa_protc_id
                     AND manf_cd       = h_sap_manf
                     AND model_cd      = h_sap_mdl
                     AND quantity      = h_sap_qnty;

                 UPDATE pgedata.pgedata_sm_gen_equipment_stage
                 SET notes           = h_notes_gntr,    cedsa_match_found = 'Y'
                 WHERE id          = h_id_gntr_eqp;
               EXCEPTION
               WHEN no_data_found THEN
                  -- No match found
                  UPDATE pgedata.pgedata_sm_gen_equipment_stage
                  SET cedsa_match_found = 'N'
                  WHERE id            = h_id_gntr_eqp;
               WHEN OTHERS THEN
                  UPDATE pgedata.pgedata_sm_gen_equipment_stage
                  SET cedsa_match_found = 'N',  status_message      = 'Other error'
                  WHERE id            = h_id_gntr_eqp;
               END; --7
            WHEN OTHERS THEN
                UPDATE pgedata.pgedata_sm_gen_equipment_stage
                SET cedsa_match_found = 'N',   status_message      = 'Other error'
                WHERE id            = h_id_gntr_eqp ;
            END; --6
            END LOOP;
         ELSIF h_sap_gen_tech = 'INDCT' THEN -- INDUCTION_GENERATION
            BEGIN
               SELECT notes,     status_cd
               INTO h_notes_gntr, h_status_cd
               FROM pgedata.induction_conv
               WHERE notes    IS NOT NULL
                   AND protection_id = h_cedsa_protc_id
                   AND manf_cd       = h_sap_manf
                   AND model_cd      = h_sap_mdl;

               UPDATE pgedata.pgedata_sm_generator_stage
               SET notes           = h_notes_gntr      ,
                      status_cd         = h_status_cd       ,
                      protection_id     = h_sap_new_protc_id,
                     cedsa_match_found = 'Y'
               WHERE id          = h_id_gntr;
             EXCEPTION
             WHEN no_data_found THEN
               UPDATE pgedata.pgedata_sm_generator_stage
               SET cedsa_match_found = 'N'
               WHERE id            = h_id_gntr;
             WHEN OTHERS THEN
               UPDATE pgedata.pgedata_sm_generator_stage
               SET cedsa_match_found = 'N',  status_message      = 'Other error'
               WHERE id            = h_id_gntr;
            END;
         ELSIF h_sap_gen_tech = 'SYNCH' THEN --SYNCHRONOUS_GENERATION
            BEGIN
               SELECT notes,    status_cd
                INTO h_notes_gntr,     h_status_cd
                FROM pgedata.synchronous_conv
                WHERE notes    IS NOT NULL
              	    AND protection_id = h_cedsa_protc_id
              	    AND manf_cd       = h_sap_manf
              	    AND model_cd      = h_sap_mdl;

                UPDATE pgedata.pgedata_sm_generator_stage
                SET notes           = h_notes_gntr      ,
                       status_cd         = h_status_cd       ,
                       protection_id     = h_sap_new_protc_id,
                       cedsa_match_found = 'Y'
                WHERE id          = h_id_gntr;
              EXCEPTION
              WHEN no_data_found THEN
                 UPDATE pgedata.pgedata_sm_generator_stage
                 SET cedsa_match_found = 'N'
                 WHERE id            = h_id_gntr;
              WHEN OTHERS THEN
                 UPDATE pgedata.pgedata_sm_generator_stage
                 SET cedsa_match_found = 'N',  status_message      = 'Other error'
                 WHERE id            = h_id_gntr;
              END;
         END IF;
        END LOOP;
      EXCEPTION
      WHEN no_data_found THEN
         UPDATE pgedata.pgedata_sm_generator_stage
         SET cedsa_match_found = 'N',
             status_message      = 'No protection exist in CEDSA'
         WHERE protection_id = h_sap_new_protc_id;

         UPDATE pgedata.pgedata_sm_gen_equipment_stage
         SET cedsa_match_found = 'N',
             status_message      = 'No protection exist in CEDSA'
         WHERE generator_id IN
         	 (SELECT id
             	FROM pgedata.pgedata_sm_generator_stage
            	WHERE gen_tech_cd  IN ('INVEXT', 'INVINC')
          	    AND cedsa_match_found = 'N'
          	    AND protection_id     = h_sap_new_protc_id
          	    AND status_message    = 'No protection exist in CEDSA'
          	) ;
      WHEN OTHERS THEN
         UPDATE pgedata.pgedata_sm_generator_stage
         SET cedsa_match_found = 'N',
             status_message      = 'Multiple protection exist in CEDSA'
         WHERE protection_id = h_sap_new_protc_id;

         UPDATE pgedata.pgedata_sm_gen_equipment_stage
         SET cedsa_match_found = 'N',
             status_message      = 'Multiple protection exist in CEDSA'
         WHERE generator_id IN
         	 (SELECT id
             	FROM pgedata.pgedata_sm_generator_stage
            	WHERE gen_tech_cd  IN ('INVEXT', 'INVINC')
         	     AND cedsa_match_found = 'N'
         	     AND protection_id     = h_sap_new_protc_id
          	   AND status_message    = 'Multiple protection exist in CEDSA'
          	) ;
       END; --end_3;
    EXCEPTION
    WHEN no_data_found THEN
      UPDATE pgedata.pgedata_sm_generation_stage
      SET status_message  = 'SP ID not exist in CEDSA',
          cedsa_match_found = 'N'
      WHERE id          = i.id;

      UPDATE pgedata.pgedata_sm_generator_stage
      SET status_message     = 'Generation SP ID not exist in CEDSA',  cedsa_match_found    = 'N'
      WHERE protection_id IN    (SELECT id FROM pgedata.pgedata_sm_protection_stage WHERE parent_id = i.id  ) ;

      UPDATE pgedata.pgedata_sm_gen_equipment_stage
      SET status_message    = 'Generation SP ID not exist in CEDSA',
        	cedsa_match_found   = 'N'
      WHERE generator_id IN
       	 (SELECT A.id --SAP_EQUIPMENT_ID
           	  FROM pgedata.pgedata_sm_generator_stage A
         	    JOIN pgedata.pgedata_sm_protection_stage B
             	  ON A.protection_id = B.id
          	  WHERE B.parent_id     = i.id
        	      AND A.gen_tech_cd      IN ('INVEXT', 'INVINC')
        	      AND cedsa_match_found   = 'N'
        	) ;
      WHEN OTHERS THEN
        UPDATE pgedata.pgedata_sm_generation_stage
        SET status_message  = 'Mutiple SP ID not exist in CEDSA',
            cedsa_match_found = 'N'
        WHERE id          = i.id;

         UPDATE pgedata.pgedata_sm_generator_stage
         SET status_message     = 'Mutiple SP ID not exist in CEDSA',
              cedsa_match_found    = 'N'
         WHERE protection_id IN
        	(SELECT id FROM pgedata.pgedata_sm_protection_stage WHERE parent_id = i.id) ;

         UPDATE pgedata.pgedata_sm_gen_equipment_stage
         SET status_message    = 'Mutiple SP ID not exist in CEDSA',
              cedsa_match_found   = 'N'
         WHERE generator_id IN
       	 (SELECT A.id --SAP_EQUIPMENT_ID
           	FROM pgedata.pgedata_sm_generator_stage A
        	  JOIN pgedata.pgedata_sm_protection_stage B
             	    ON A.protection_id = B.id
          	  WHERE B.parent_id     = i.id
        	       AND A.gen_tech_cd      IN ('INVEXT', 'INVINC')
        	       AND cedsa_match_found   = 'N'
        	) ;
    	END; --end_2
    COMMIT;
  END LOOP;
  COMMIT;
END; --end_1
--------------------------------------------------------------------------------------------------------------------------------
-- Insert into the main tables in EDER
PROCEDURE Pge_gen_mgrtn_to_main_edgis_sp ( input_dm_di_flag IN VARCHAR2)    IS
  h_reg_id INTEGER;
BEGIN
  IF input_dm_di_flag = 'DM' THEN
    SAVEPOINT err_found;
    BEGIN
       SELECT registration_id
         INTO h_reg_id
         FROM sde.table_registry
        WHERE table_name = 'GENERATIONINFO'
             AND owner          = 'EDGIS';

       INSERT   INTO edgis.zz_mv_generationinfo
        (
          objectid          ,
          globalid          ,
          servicepointguid  ,
          datecreated       ,
          createdby         ,
          sapeginotification,
          projectname       ,
          gentype           ,
          programtype       ,
          effratingmachkw   ,
          effratinginvkw    ,
          effratingmachkva  ,
          effratinginvkva   ,
          backupgen         ,
          maxstoragecapacity,
          chargedemandkw    ,
          gensymbology      ,
          powersource
        )
       SELECT sde.version_user_ddl.Next_row_id ('EDGIS', h_reg_id),
        '{'
        ||SUBSTR (Upper (Rawtohex (Sys_guid ())), 1, 8)
        ||'-'
        ||SUBSTR (Upper (Rawtohex (Sys_guid ())), 9, 4)
        ||'-'
        ||SUBSTR (Upper (Rawtohex (Sys_guid ())), 13, 4)
        ||'-'
        ||SUBSTR (Upper (Rawtohex (Sys_guid ())), 17, 4)
        ||'-'
        ||SUBSTR (Upper (Rawtohex (Sys_guid ())), 21, 12)
        ||'}'                 ,
        stg.servicepointguid  ,
        stg.datecreated       ,
        stg.createdby         ,
        stg.sapeginotification,
        stg.projectname       ,
        stg.gentype           ,
        stg.programtype       ,
        stg.effratingmachkw   ,
        stg.effratinginvkw    ,
        stg.effratingmachkva  ,
        stg.effratinginvkva   ,
        stg.backupgen         ,
        stg.maxstoragecapacity,
        stg.chargedemandkw    ,
        stg.gensymbology      ,
        stg.powersource
         FROM pgedata.pgedata_generationinfo_stage stg;

      --update actual GUID on basis of SERVICEPOINTGUID in Stg_2 from main
       merge INTO pgedata.pgedata_generationinfo_stage stg USING edgis.zz_mv_generationinfo main ON (main.servicepointguid = stg.servicepointguid)
       WHEN matched THEN
       UPDATE SET stg.globalid = main.globalid;

      --update GUID in PGEDATA_SM_GENERATION_STAGE on basis of SERVICE_POINT_ID  from PGEDATA_GENERATIONINFO_STAGE
       merge INTO pgedata.pgedata_sm_generation_stage sm_gen USING pgedata.pgedata_generationinfo_stage gen_info ON (sm_gen.service_point_id = gen_info.service_point_id)
       WHEN matched THEN
       UPDATE SET sm_gen.global_id = gen_info.globalid;
       COMMIT;
    EXCEPTION
    WHEN OTHERS THEN
      ROLLBACK TO err_found;
      dbms_output.Put_line (SQLERRM) ;
    END;
  END IF;
END;
--------------------------------------------------------------------------------------------------------------------------------
--Service Location (GenCategory ) Update and capture in stg table for primary
PROCEDURE Pge_updt_gen_ctgy_sp_loc_sp     IS
  h_cnt_2 NUMBER (18) := 0;
  h_id    NUMBER      := 0;
BEGIN
  --Commenting in case of Multiple DM Files Load
  --DELETE  FROM pgedata.pgedata_stage_service_loc_prim;
-- Preprod issue

--UPDATE edgis.ZZ_MV_SERVICELOCATION SET GENCATEGORY =0 WHERE GENCATEGORY<>0 ;
update edgis.zz_mv_servicelocation set gencategory = 2 where globalid in (select servicelocationguid from pgedata_generationinfo_stage where upper(gensymbology)  = 'SECONDARY');

insert into pgedata_stage_service_loc_prim (ID, globalid, gencategory, labeltext) select (select NVL(max(id),0) from pgedata_stage_service_loc_prim)+rownum as ID, servicelocationguid,1 as gencategory, pge_enos_to_edgis_dm_pkg.F_GET_GENERATION_LABEL(servicelocationguid) as labeltext from pgedata.pgedata_generationinfo_stage stg where Upper (stg.gensymbology)  = 'PRIMARY' and servicelocationguid is not null;

-- Preprod issue

COMMIT;
END;

----------------------------------------------------------------------------------------------------------------------------
--Update the stage tables with the CESDA status
PROCEDURE Pge_cedsa_not_merged_sp IS
BEGIN
  SAVEPOINT err_found_2;
  BEGIN
  merge INTO pgedata.gen_equipment_stage stg_1 USING pgedata.pgedata_sm_generator_stage stg_2 ON (stg_1.sap_equipment_id = stg_2.sap_equipment_id)
  WHEN matched THEN
     UPDATE
     SET stg_1.cedsa_match_found          = stg_2.cedsa_match_found
     WHERE stg_1.status                 = 'S'
       AND Upper (stg_2.cedsa_match_found) IN ('Y', 'N') ;

    merge INTO pgedata.gen_equipment_stage stg_1 USING pgedata.pgedata_sm_gen_equipment_stage stg_2 ON (stg_1.sap_equipment_id = stg_2.sap_equipment_id)
    WHEN matched THEN
    UPDATE
    SET stg_1.cedsa_match_found          = stg_2.cedsa_match_found
    WHERE stg_1.status                 = 'S'
      AND Upper (stg_2.cedsa_match_found) IN ('Y', 'N') ;

    merge INTO pgedata.gen_summary_stage stg_1 USING pgedata.pgedata_sm_generation_stage stg_2 ON (stg_1.service_point_id = stg_2.service_point_id)
    WHEN matched THEN
    UPDATE
    SET stg_1.cedsa_match_found          = stg_2.cedsa_match_found
    WHERE stg_1.status                 = 'S'
      AND Upper (stg_2.cedsa_match_found) IN ('Y', 'N') ;

    COMMIT;
  END;
EXCEPTION
WHEN OTHERS THEN
  ROLLBACK TO err_found_2;
  dbms_output.Put_line (SQLERRM) ;
END;
------------------------------------------------------------------------------------------------------------------------------
--Update GUID in the stage tables
PROCEDURE Pge_stg2_guid_updt_stg1_sp IS
BEGIN
 SAVEPOINT err_found_2;
 BEGIN
   merge INTO pgedata.gen_summary_stage stg_1 USING pgedata.pgedata_sm_generation_stage stg_2 ON (stg_1.service_point_id = stg_2.service_point_id)
   WHEN matched THEN
   UPDATE
   SET stg_1.guid               = stg_2.global_id
   WHERE  stg_1.status             = 'S';

   UPDATE pgedata.gen_equipment_stage eqp
   SET guid =
      (SELECT guid
         FROM pgedata.gen_summary_stage smry
        WHERE smry.service_point_id = eqp.service_point_id
      )
    WHERE status     = 'S';

    COMMIT;
  END;
EXCEPTION
WHEN OTHERS THEN
  ROLLBACK TO err_found_2;
  dbms_output.Put_line (SQLERRM) ;
END;
--------------------------------------------------------------------------------------------------------------------------------
--Function to process the daily SAP records
PROCEDURE Pge_sap_to_stg2_di_sp  ( input_action IN VARCHAR2, out_flag OUT VARCHAR2)     IS
  h_gen_type NVARCHAR2 (6) := NULL;
  h_user     VARCHAR2 (35)     := NULL;
  h_globalid VARCHAR2 (38)     := NULL;
  h_spid NVARCHAR2 (10)        := NULL;
  h_action     VARCHAR2 (1)        := NULL;
  h_action_eq  VARCHAR2 (1)        := NULL;
  h_gen_symbl  VARCHAR2 (30)       := NULL;
  h_prm_mtr_id VARCHAR2 (38)       := NULL;
  h_trnsfr_id  VARCHAR2 (38)       := NULL;
  h_prtcn_id   NUMBER (10)         := 0;
  h_prctn_typ NVARCHAR2 (8)        := NULL;
  h_gen_tech NVARCHAR2 (6)         := NULL;
  h_eq_id        VARCHAR2 (18)            := NULL;
  h_gen_tech_eq  VARCHAR2 (18)            := NULL;
  h_cnt_dc       NUMBER (10)              := 0;
  h_success      NUMBER (1)               := 0;
  h_next_flg     NUMBER (1)               := 0;
  h_cnt_11       NUMBER (10)              := 0;
  h_eq_id_dc     VARCHAR2 (18)            := NULL;
  h_action_eq_dc VARCHAR2 (1)             := NULL;
  h_lctn_guid    CHAR (38)                := NULL;
  h_gntr_id_3    NUMBER (10)              := 0;
  h_success_insert    NUMBER (1)              := 0;
TYPE t_bulk_collect
IS
  TABLE OF pgedata.gen_equipment_stage%ROWTYPE;
  v_bulk_eq T_BULK_COLLECT   := T_bulk_collect () ;
  v_bulk_eq_2 T_BULK_COLLECT := T_bulk_collect () ; -- for DC generator

BEGIN                                               --1
 SELECT USER
 INTO h_user
 FROM dual;

  FOR i  IN
  (SELECT *
   FROM pgedata.gen_summary_stage
   WHERE status    IS NULL
       AND Upper (action) = Upper (input_action)
  )
  LOOP
    h_spid   := i.service_point_id;
    h_action := Upper (i.action) ;
    BEGIN --2
	       -- Check for service point
      SELECT globalid         ,
        	Trim (primarymeterguid),
        	Trim (transformerguid) ,
        	servicelocationguid
      INTO h_globalid,
        	h_prm_mtr_id    ,
        	h_trnsfr_id     ,
        	h_lctn_guid
      FROM edgis.zz_mv_servicepoint
      WHERE servicepointid = h_spid;

      IF h_prm_mtr_id       IS NOT NULL THEN
       	 h_gen_symbl         := 'Primary';
      ELSIF h_trnsfr_id     IS NOT NULL THEN
        	h_gen_symbl         := 'Secondary';
      ELSE
       	 h_gen_symbl := NULL;
      END IF;

      --SAVEPOINT err_found ;
      SAVEPOINT err_found2;
      BEGIN --3
        SAVEPOINT err_found3;

        IF h_action = 'I' THEN
	           -- Check whehter record exist or not
           SELECT COUNT ( *)
            INTO h_cnt_11
            FROM edgis.zz_mv_generationinfo
            WHERE servicepointguid = h_globalid;

          IF h_cnt_11              = 0 THEN
	            -- Record doens't exist in EDER. Insert new record
             INSERT   INTO pgedata.pgedata_generationinfo_stage
              (
                globalid          ,
                service_point_id  ,
                servicepointguid  ,
                datecreated       ,
                gentype           ,
                createdby         ,
                sapeginotification,
                projectname       ,
                programtype       ,
                effratingmachkw   ,
                effratinginvkw    ,
                effratingmachkva  ,
                effratinginvkva   ,
                backupgen         ,
                maxstoragecapacity,
                chargedemandkw    ,
                gensymbology      ,
                powersource       ,
                action            ,
                servicelocationguid --added on 14 feb 17
              )
              VALUES
              (
                Sys_guid ()           ,
                i.service_point_id    ,
                h_globalid            ,
                SYSDATE               ,
                h_gen_type            ,
                h_user                ,
                i.sap_egi_notification,
                i.project_name        ,
                i.program_type        ,
                i.eff_rating_mach_kw  ,
                i.eff_rating_inv_kw   ,
                i.eff_rating_mach_kva ,
                i.eff_rating_inv_kva  ,
                'N'                   ,
                i.max_storage_capacity,
                i.charge_demand_kw    ,
                h_gen_symbl           ,
                i.power_source        ,
                'I'                   ,
                h_lctn_guid
              ) ;

            --start code for insert new in SM_GENERATION data
             INSERT    INTO pgedata.pgedata_sm_generation_stage
              (
                service_point_id    ,
                global_id           ,
                sap_egi_notification,
                project_name        ,
                power_source        ,
                eff_rating_mach_kw  ,
                eff_rating_inv_kw   ,
                eff_rating_mach_kva ,
                eff_rating_inv_kva  ,
                max_storage_capacity,
                charge_demand_kw    ,
                program_type        ,
                datecreated         ,
                gen_type,
                createdby           ,
                export_kw           ,
                direct_transfer_trip,
                action
              )
             SELECT service_point_id                               ,
              globalid                                             ,
              sapeginotification                                   ,
              projectname                                          ,
              powersource                                          ,
              effratingmachkw                                      ,
              effratinginvkw                                       ,
              effratingmachkva                                     ,
              effratinginvkva                                      ,
              maxstoragecapacity                                   ,
              chargedemandkw                                       ,
              programtype                                          ,
              datecreated                                          ,
              gentype                                              ,
              createdby                                            ,
              (NVL (effratingmachkw, 0) + NVL (effratinginvkw, 0)),
              'N'                                                  ,
              'I'
              FROM pgedata.pgedata_generationinfo_stage
              WHERE service_point_id = i.service_point_id;

            --end code for insert new in SM_GENERATION data
            -- start code for create default protection
             h_prctn_typ := 'UNSP';
             INSERT   INTO pgedata.pgedata_sm_protection_stage
              (
                parent_type    ,
                parent_id      ,
                protection_type,
                datecreated    ,
                createdby      ,
                export_kw      ,
                notes          ,
                action
              )
             SELECT 'GENERATION',
              A.id              ,
              h_prctn_typ       ,
              A.datecreated     ,
              A.createdby       ,
              A.export_kw       ,
              A.notes           ,
              'I'
              FROM pgedata.pgedata_sm_generation_stage A,
                   pgedata.pgedata_generationinfo_stage B
              WHERE A.global_id    = B.globalid
                AND B.service_point_id = i.service_point_id;

            -- end code for create default protection

            h_next_flg := 1;
          ELSE
            ROLLBACK TO err_found3;
            h_next_flg := 0;

            UPDATE pgedata.gen_summary_stage
            SET status       = 'F',
                status_message = 'Already generation exist'
            WHERE objectid = i.objectid;
          END IF;

        ELSIF h_action IN ('U', 'D') THEN
          -- GUID already validated in validation SP. No need to check it again where it is null or not null
          SELECT COUNT ( *)
          INTO h_cnt_11
          FROM edgis.zz_mv_generationinfo
          WHERE globalid = i.guid;

          IF h_cnt_11      = 1 THEN
            IF h_action    = 'U' THEN

	-- Update the stage table
            UPDATE pgedata.pgedata_generationinfo_stage
            SET datemodified     = SYSDATE               ,
                modifiedby         = h_user                ,
                sapeginotification = i.sap_egi_notification,
                projectname        = i.project_name        ,
                programtype        = i.program_type        ,
                effratingmachkw    = i.eff_rating_mach_kw  ,
                effratinginvkw     = i.eff_rating_inv_kw   ,
                effratingmachkva   = i.eff_rating_mach_kva ,
                effratinginvkva    = i.eff_rating_inv_kva  ,
                maxstoragecapacity = i.max_storage_capacity,
                chargedemandkw     = i.charge_demand_kw    ,
                powersource        = i.power_source        ,
                service_point_id   = i.service_point_id    ,
                action             = 'U'
             WHERE globalid     = i.guid;
--Changes to update Export_KW
             merge INTO pgedata.pgedata_sm_generation_stage trgt USING pgedata.pgedata_generationinfo_stage src ON (trgt.global_id = src.globalid AND src.globalid = i.guid)
             WHEN matched THEN
             UPDATE
               SET trgt.service_point_id   = src.service_point_id  ,
                trgt.date_modified        = src.datemodified      ,
                trgt.modifiedby           = src.modifiedby        ,
                trgt.sap_egi_notification = src.sapeginotification,
                trgt.project_name         = src.projectname       ,
                trgt.gen_type             = src.gentype           ,
                trgt.program_type         = src.programtype       ,
                trgt.power_source         = src.powersource       ,
                trgt.eff_rating_mach_kw   = src.effratingmachkw   ,
                trgt.eff_rating_inv_kw    = src.effratinginvkw    ,
                trgt.eff_rating_mach_kva  = src.effratingmachkva  ,
                trgt.eff_rating_inv_kva   = src.effratinginvkva   ,
                trgt.backup_generation    = src.backupgen         ,
                trgt.max_storage_capacity = src.maxstoragecapacity,
                trgt.charge_demand_kw     = src.chargedemandkw    ,
                trgt.action               = 'U'                   ,
                trgt.export_kw            =nvl(src.effratingmachkw,0) + nvl(src.effratinginvkw ,0) ;

              merge INTO pgedata.pgedata_sm_protection_stage trgt
              USING pgedata.pgedata_sm_generation_stage src ON (trgt.parent_id = src.id AND src.global_id = i.guid AND trgt.parent_type = 'GENERATION')
              WHEN matched THEN
              UPDATE
              SET trgt.date_modified = src.date_modified,
                trgt.modifiedby      = src.modifiedby   ,
                trgt.export_kw       = src.export_kw    ,
                trgt.action          = 'U';

              h_next_flg := 1;

            ELSE -- delete case
              UPDATE pgedata.pgedata_sm_gen_equipment_stage
              SET action            = 'D'
              WHERE generator_id IN
                	(SELECT id --SAP_EQUIPMENT_ID
                   	FROM pgedata.pgedata_sm_generator_stage
                  	WHERE gen_tech_cd IN ('INVEXT', 'INVINC')
                	    AND protection_id   IN
                  		(SELECT id
                     		FROM pgedata.pgedata_sm_protection_stage
                    		WHERE parent_type = 'GENERATION'
                  		    AND parent_id      IN
                    			(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid)
                  		)
               	 ) ;

               UPDATE pgedata.pgedata_sm_generator_stage
               SET action             = 'D'
               WHERE protection_id IN
                (SELECT id
                   FROM pgedata.pgedata_sm_protection_stage
                  WHERE parent_type = 'GENERATION'
                    AND parent_id      IN
                  (SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid )
               	 ) ;

              UPDATE pgedata.pgedata_sm_protection_stage
              SET action          = 'D'
              WHERE parent_type = 'GENERATION'
             	    AND parent_id      IN
               	 (SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  ) ;

              UPDATE pgedata.pgedata_sm_generation_stage
              SET action        = 'D'
              WHERE global_id = i.guid;

              UPDATE pgedata.pgedata_generationinfo_stage
              SET action       = 'D'
              WHERE globalid = i.guid;

              h_next_flg := 1;
           END IF;
          ELSE
            ROLLBACK TO err_found3;
            h_next_flg := 0;

            UPDATE pgedata.gen_summary_stage
            SET status       = 'F',    status_message = 'No/Multiple Generation exist for update/delete'
            WHERE objectid = i.objectid;
          END IF;
        END IF;
       EXCEPTION
       WHEN OTHERS THEN
        ROLLBACK TO err_found3;
        dbms_output.Put_line (SQLERRM) ;
        h_next_flg := 0;

        UPDATE pgedata.gen_summary_stage
        SET status       = 'F',  status_message = 'Other error'
         WHERE objectid = i.objectid;
      END; --3

      dbms_output.Put_line (h_next_flg) ;
     -- Record loaded into the stage table. Process further
      IF h_next_flg = 1 THEN
       -- start code for equipment details
        SELECT * bulk collect
        INTO v_bulk_eq
        FROM pgedata.gen_equipment_stage
        WHERE status      IS NULL
          AND service_point_id = i.service_point_id
          AND gen_tech_cd     IN ('GEN.SYNCH', 'GEN.INDCT', 'GEN.INVINC', 'GEN.INVEXT') ;

        IF v_bulk_eq.count   = 0 THEN
          dbms_output.Put_line ('No equipment for SP ID :' ||i.service_point_id) ;
          h_success := 3;
        END IF;

        FOR k IN 1 .. v_bulk_eq.count
        LOOP
          h_eq_id       := V_bulk_eq (k) .sap_equipment_id;
          h_action_eq   := Upper (V_bulk_eq (k) .action) ;
          h_gen_tech    := pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f ( V_bulk_eq (k) .gen_tech_cd) ;
          h_gen_tech_eq := Upper (V_bulk_eq (k) .gen_tech_equipment) ;
          --char length is 16
          dbms_output.Put_line ('For SP ID ' ||h_spid ||', Equpment ID is ' ||h_eq_id) ;
          BEGIN
            IF h_action = 'I' THEN
               SELECT A.id
               INTO h_prtcn_id
               FROM pgedata.pgedata_sm_protection_stage A,
              	    pgedata.pgedata_sm_generation_stage B
               WHERE A.parent_type  = 'GENERATION'
                     AND A.parent_id        = B.id
                     AND B.service_point_id = i.service_point_id;
            ELSE
               SELECT A.id
               INTO h_prtcn_id
               FROM pgedata.pgedata_sm_protection_stage A,
                    pgedata.pgedata_sm_generation_stage B
               WHERE A.parent_type = 'GENERATION'
              	    AND A.parent_id       = B.id
              	    AND B.global_id       = i.guid;
            END IF;

            IF h_gen_tech    = 'SYNCH' THEN -- SYNCHRONOUS
              IF h_action_eq = 'I' THEN   -- checking Eqp action
                IF h_action IN ('I', 'U') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  BEGIN --6
                     INSERT   INTO pgedata.pgedata_sm_generator_stage
                      (
                        sap_equipment_id    ,
                        protection_id       ,
                        sap_queue_number    ,
                        sap_egi_notification,
                        datecreated         ,
                        createdby           ,
                        power_source        ,
                        project_name        ,
                        manufacturer        ,
                        model               ,
                        inverter_efficiency ,
                        nameplate_rating    ,
                        quantity            ,
                        power_factor        ,
                        eff_rating_kw       ,
                        eff_rating_kva      ,
                        rated_voltage       ,
                        number_of_phases    ,
                        gen_tech_cd         ,
                        pto_date            ,
                        program_type        ,
                        control_cd          ,
                        connection_cd       ,
                        status_cd           ,
                        ss_reactance        ,
                        ss_resistance       ,
                        trans_reactance     ,
                        trans_resistnace    ,
                        subtrans_reactance  ,
                        subtrans_resistance ,
                        neg_reactance       ,
                        neg_resistance      ,
                        zero_reactance      ,
                        zero_resistance     ,
                        grd_reactance       ,
                        grd_resistance      ,
                        enos_equip_ref      ,
                        enos_proj_ref       ,
                        gen_tech_equipment  ,
                        certification       ,
                        action              ,
                        tech_type_cd        ,
                        backup_gen
                      )
                      VALUES
                      (
                        h_eq_id                            ,
                        h_prtcn_id                         ,
                        V_bulk_eq (k) .sap_queue_number    ,
                        V_bulk_eq (k) .sap_egi_notification,
                        SYSDATE                            ,
                        h_user                             ,
                        V_bulk_eq (k) .power_source        ,
                        V_bulk_eq (k) .project_name        ,
                        V_bulk_eq (k) .manufacturer        ,
                        V_bulk_eq (k) .model               ,
                        V_bulk_eq (k) .inverter_efficiency ,
                        V_bulk_eq (k) .nameplate_rating    ,
                        V_bulk_eq (k) .quantity            ,
                        V_bulk_eq (k) .power_factor        ,
                        V_bulk_eq (k) .eff_rating_kw       ,
                        V_bulk_eq (k) .eff_rating_kva      ,
                        V_bulk_eq (k) .rated_voltage       ,
                        V_bulk_eq (k) .number_of_phases    ,
                        h_gen_tech                         ,
                        V_bulk_eq (k) .pto_date            ,
                        V_bulk_eq (k) .program_type        ,
                        'NONE'                             ,
                        NULL                               ,
                        'UNSP'                             ,
                        V_bulk_eq (k) .ss_reactance        ,
                        V_bulk_eq (k) .ss_resistance       ,
                        V_bulk_eq (k) .trans_reactance     ,
                        V_bulk_eq (k) .trans_resistance    ,
                        V_bulk_eq (k) .subtrans_reactance  ,
                        V_bulk_eq (k) .subtrans_resistance ,
                        V_bulk_eq (k) .neg_reactance       ,
                        V_bulk_eq (k) .neg_resistance      ,
                        V_bulk_eq (k) .zero_reactance      ,
                        V_bulk_eq (k) .zero_resistance     ,
                        V_bulk_eq (k) .grd_reactance       ,
                        V_bulk_eq (k) .grd_resistance      ,
                        V_bulk_eq (k) .enos_equip_ref      ,
                        V_bulk_eq (k) .enos_proj_ref       ,
                        V_bulk_eq (k) .gen_tech_equipment  ,
                        V_bulk_eq (k) .certification       ,
                        h_action_eq                        ,
                        V_bulk_eq (k) .tech_type_cd        ,
                        V_bulk_eq (k) .backup_gen
                      ) ;

                    h_success := 1;
                    h_success_insert :=4;
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line  (SQLERRM)  ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',  status_message = 'SG Generator creation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',     status_message     = 'Failed due to other eqp failed'
                    WHERE status      IS NULL
                      AND service_point_id = i.service_point_id;

                    UPDATE pgedata.gen_summary_stage
                    SET status       = 'F', status_message = 'SG Generator creation failed due to exception capture'
                    WHERE objectid = i.objectid;

                    EXIT;
                  END;
                ELSE
                   ROLLBACK TO err_found2;
                   h_success := 0;

                   UPDATE pgedata.gen_equipment_stage
                   SET status       = 'F',   status_message = 'Invalid action in generation for respective new equipment'
                   WHERE objectid = V_bulk_eq (k) .objectid;

                   UPDATE pgedata.gen_equipment_stage
                   SET status           = 'F',    status_message     = 'Failed due to other eqp failed'
                   WHERE status      IS NULL
                     AND service_point_id = i.service_point_id;

                   UPDATE pgedata.gen_summary_stage
                   SET status       = 'F', status_message = 'Invalid action in generation for respective new equipment'
                   WHERE objectid = i.objectid;

                   EXIT;
                END IF;
              ELSIF h_action_eq = 'U' THEN
                IF h_action     = 'U' THEN
                  ---- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  BEGIN
                    -- start code to update PGEDATA_SM_GENERATOR_STAGE
                    UPDATE pgedata.pgedata_sm_generator_stage
                    SET sap_egi_notification = V_bulk_eq (k) .sap_egi_notification,
                      power_source           = V_bulk_eq (k) .power_source        ,
                      project_name           = V_bulk_eq (k) .project_name        ,
                      manufacturer           = V_bulk_eq (k) .manufacturer        ,
                      model                  = V_bulk_eq (k) .model               ,
                      inverter_efficiency    = V_bulk_eq (k) .inverter_efficiency ,
                      nameplate_rating       = V_bulk_eq (k) .nameplate_rating    ,
                      quantity               = V_bulk_eq (k) .quantity            ,
                      power_factor           = V_bulk_eq (k) .power_factor        ,
                      eff_rating_kw          = V_bulk_eq (k) .eff_rating_kw       ,
                      eff_rating_kva         = V_bulk_eq (k) .eff_rating_kva      ,
                      rated_voltage          = V_bulk_eq (k) .rated_voltage       ,
                      number_of_phases       = V_bulk_eq (k) .number_of_phases    ,
                      pto_date               = V_bulk_eq (k) .pto_date            ,
                      program_type           = V_bulk_eq (k) .program_type        ,
                      ss_reactance           = V_bulk_eq (k) .ss_reactance        ,
                      ss_resistance          = V_bulk_eq (k) .ss_resistance       ,
                      trans_reactance        = V_bulk_eq (k) .trans_reactance     ,
                      trans_resistnace       = V_bulk_eq (k) .trans_resistance    ,
                      subtrans_reactance     = V_bulk_eq (k) .subtrans_reactance  ,
                      subtrans_resistance    = V_bulk_eq (k) .subtrans_resistance ,
                      neg_reactance          = V_bulk_eq (k) .neg_reactance       ,
                      neg_resistance         = V_bulk_eq (k) .neg_resistance      ,
                      zero_reactance         = V_bulk_eq (k) .zero_reactance      ,
                      zero_resistance        = V_bulk_eq (k) .zero_resistance     ,
                      grd_reactance          = V_bulk_eq (k) .grd_reactance       ,
                      grd_resistance         = V_bulk_eq (k) .grd_resistance      ,
                      gen_tech_equipment     = V_bulk_eq (k) .gen_tech_equipment  ,
                      certification          = V_bulk_eq (k) .certification       ,
                      action                 = h_action_eq                        ,
                      tech_type_cd           = V_bulk_eq (k) .tech_type_cd        ,
                      backup_gen             = V_bulk_eq (k) .backup_gen
                      WHERE sap_equipment_id = h_eq_id
                           AND gen_tech_cd          = h_gen_tech
                           AND protection_id       IN
                      	(SELECT id
                         	FROM pgedata.pgedata_sm_protection_stage
                        	WHERE parent_type = 'GENERATION'
                      	    AND parent_id      IN
                        		(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  )
                     	 ) ;

                    h_success := 1;
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line (SQLERRM) ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',   status_message = 'SG Generator updation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                     UPDATE pgedata.gen_equipment_stage
                     SET status           = 'F',    status_message     = 'Failed due to other eqp failed'
                     WHERE status      IS NULL
                       AND service_point_id = i.service_point_id;

                     UPDATE pgedata.gen_summary_stage
                     SET status       = 'F',  status_message = 'Failed due to respective eqp failed'
                     WHERE objectid = i.objectid;
                     EXIT;
                  END;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',   status_message = 'Invalid action in generation for respective equipment'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',  status_message     = 'Failed due to other eqp failed'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',  status_message = 'Failed due to generator failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              ELSIF h_action_eq = 'D' THEN
                IF h_action    IN ('U', 'D') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  -- start code to delete PGEDATA_SM_GENERATOR_STAGE
                   UPDATE pgedata.pgedata_sm_generator_stage
                   SET action               = 'D'
                   WHERE sap_equipment_id = h_eq_id
                        AND gen_tech_cd          = h_gen_tech
                        AND protection_id       IN
                    	(SELECT id
                       	FROM pgedata.pgedata_sm_protection_stage
                      	WHERE parent_type = 'GENERATION'
                    	    AND parent_id      IN
                     		 (SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid )
                  	  ) ;
                   -- end code to delete PGEDATA_SM_GENERATOR_STAGE
                   h_success := 1;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',  status_message = 'Invalid action in generation for respective equipment'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',  status_message     = 'Failed due to other eqp failed'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',   status_message = 'Failed due to generator failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              END IF;
            ELSIF h_gen_tech = 'INDCT' THEN --INDUCTION
              IF h_action_eq = 'I' THEN   -- checking Eqp action
                IF h_action IN ('I', 'U') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  BEGIN --6
                     INSERT   INTO pgedata.pgedata_sm_generator_stage
                      (
                        sap_equipment_id    ,
                        protection_id       ,
                        sap_queue_number    ,
                        sap_egi_notification,
                        datecreated         ,
                        createdby           ,
                        power_source        ,
                        project_name        ,
                        manufacturer        ,
                        model               ,
                        inverter_efficiency ,
                        nameplate_rating    ,
                        quantity            ,
                        power_factor        ,
                        eff_rating_kw       ,
                        eff_rating_kva      ,
                        rated_voltage       ,
                        number_of_phases    ,
                        gen_tech_cd         ,
                        pto_date            ,
                        program_type        ,
                        control_cd          ,
                        connection_cd       ,
                        status_cd           ,
                        ss_reactance        ,
                        ss_resistance       ,
                        trans_reactance     ,
                        trans_resistnace    ,
                        subtrans_reactance  ,
                        subtrans_resistance ,
                        neg_reactance       ,
                        neg_resistance      ,
                        zero_reactance      ,
                        zero_resistance     ,
                        grd_reactance       ,
                        grd_resistance      ,
                        enos_equip_ref      ,
                        enos_proj_ref       ,
                        gen_tech_equipment  ,
                        certification       ,
                        action              ,
                        tech_type_cd        ,
                        backup_gen
                      )
                      VALUES
                      (
                        h_eq_id                            ,
                        h_prtcn_id                         ,
                        V_bulk_eq (k) .sap_queue_number    ,
                        V_bulk_eq (k) .sap_egi_notification,
                        SYSDATE                            ,
                        h_user                             ,
                        V_bulk_eq (k) .power_source        ,
                        V_bulk_eq (k) .project_name        ,
                        V_bulk_eq (k) .manufacturer        ,
                        V_bulk_eq (k) .model               ,
                        V_bulk_eq (k) .inverter_efficiency ,
                        V_bulk_eq (k) .nameplate_rating    ,
                        V_bulk_eq (k) .quantity            ,
                        V_bulk_eq (k) .power_factor        ,
                        V_bulk_eq (k) .eff_rating_kw       ,
                        V_bulk_eq (k) .eff_rating_kva      ,
                        V_bulk_eq (k) .rated_voltage       ,
                        V_bulk_eq (k) .number_of_phases    ,
                        h_gen_tech                         ,
                        V_bulk_eq (k) .pto_date            ,
                        V_bulk_eq (k) .program_type        ,
                        'NONE'                             ,
                        NULL                               ,
                        'UNSP'                             ,
                        V_bulk_eq (k) .ss_reactance        ,
                        V_bulk_eq (k) .ss_resistance       ,
                        V_bulk_eq (k) .trans_reactance     ,
                        V_bulk_eq (k) .trans_resistance    ,
                        V_bulk_eq (k) .subtrans_reactance  ,
                        V_bulk_eq (k) .subtrans_resistance ,
                        V_bulk_eq (k) .neg_reactance       ,
                        V_bulk_eq (k) .neg_resistance      ,
                        V_bulk_eq (k) .zero_reactance      ,
                        V_bulk_eq (k) .zero_resistance     ,
                        V_bulk_eq (k) .grd_reactance       ,
                        V_bulk_eq (k) .grd_resistance      ,
                        V_bulk_eq (k) .enos_equip_ref      ,
                        V_bulk_eq (k) .enos_proj_ref       ,
                        V_bulk_eq (k) .gen_tech_equipment  ,
                        V_bulk_eq (k) .certification       ,
                        h_action_eq                        ,
                        V_bulk_eq (k) .tech_type_cd        ,
                        V_bulk_eq (k) .backup_gen
                      ) ;

                     h_success := 1;
                     h_success_insert :=4;
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line    ( SQLERRM   )  ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',  status_message = 'IG Generator creation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',     status_message     = 'Failed due to other eqp failed'
                    WHERE status      IS NULL
                      AND service_point_id = i.service_point_id;

                    UPDATE pgedata.gen_summary_stage
                    SET status       = 'F',  status_message = 'IG Generator creation failed due to exception capture'
                    WHERE objectid = i.objectid;

                    EXIT;
                  END; --6
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',   status_message = 'Invalid action in generation for respective new equipment'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',   status_message     = 'Failed due to other eqp failed'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',   status_message = 'Invalid action in generation for respective new equipment'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              ELSIF h_action_eq = 'U' THEN
                IF h_action     = 'U' THEN
                  ---- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  BEGIN
                    -- start code to update PGEDATA_SM_GENERATOR_STAGE
                    UPDATE pgedata.pgedata_sm_generator_stage
                    SET sap_egi_notification = V_bulk_eq (k) .sap_egi_notification,
                      power_source           = V_bulk_eq (k) .power_source        ,
                      project_name           = V_bulk_eq (k) .project_name        ,
                      manufacturer           = V_bulk_eq (k) .manufacturer        ,
                      model                  = V_bulk_eq (k) .model               ,
                      inverter_efficiency    = V_bulk_eq (k) .inverter_efficiency ,
                      nameplate_rating       = V_bulk_eq (k) .nameplate_rating    ,
                      quantity               = V_bulk_eq (k) .quantity            ,
                      power_factor           = V_bulk_eq (k) .power_factor        ,
                      eff_rating_kw          = V_bulk_eq (k) .eff_rating_kw       ,
                      eff_rating_kva         = V_bulk_eq (k) .eff_rating_kva      ,
                      rated_voltage          = V_bulk_eq (k) .rated_voltage       ,
                      number_of_phases       = V_bulk_eq (k) .number_of_phases    ,
                      pto_date               = V_bulk_eq (k) .pto_date            ,
                      program_type           = V_bulk_eq (k) .program_type        ,
                      ss_reactance           = V_bulk_eq (k) .ss_reactance        ,
                      ss_resistance          = V_bulk_eq (k) .ss_resistance       ,
                      trans_reactance        = V_bulk_eq (k) .trans_reactance     ,
                      trans_resistnace       = V_bulk_eq (k) .trans_resistance    ,
                      subtrans_reactance     = V_bulk_eq (k) .subtrans_reactance  ,
                      subtrans_resistance    = V_bulk_eq (k) .subtrans_resistance ,
                      neg_reactance          = V_bulk_eq (k) .neg_reactance       ,
                      neg_resistance         = V_bulk_eq (k) .neg_resistance      ,
                      zero_reactance         = V_bulk_eq (k) .zero_reactance      ,
                      zero_resistance        = V_bulk_eq (k) .zero_resistance     ,
                      grd_reactance          = V_bulk_eq (k) .grd_reactance       ,
                      grd_resistance         = V_bulk_eq (k) .grd_resistance      ,
                      gen_tech_equipment     = V_bulk_eq (k) .gen_tech_equipment  ,
                      certification          = V_bulk_eq (k) .certification       ,
                      action                 = h_action_eq                        ,
                      tech_type_cd           = V_bulk_eq (k) .tech_type_cd        ,
                      backup_gen             = V_bulk_eq (k) .backup_gen
                      WHERE sap_equipment_id = h_eq_id
                          AND gen_tech_cd          = h_gen_tech
                          AND protection_id       IN
                      	(SELECT id
                         	FROM pgedata.pgedata_sm_protection_stage
                        	WHERE parent_type = 'GENERATION'
                      	    AND parent_id      IN
                        		(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid   )
                      	);

                    h_success := 1;
                    -- end code to update PGEDATA_SM_GENERATOR_STAGE
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line (SQLERRM) ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',  status_message = 'SG Generator updation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',  status_message     = 'Failed due to other eqp failed'
                    WHERE status      IS NULL
                      AND service_point_id = i.service_point_id;

                    UPDATE pgedata.gen_summary_stage
                    SET status       = 'F',   status_message = 'Failed due to respective eqp failed'
                    WHERE objectid = i.objectid;

                    EXIT;
                  END;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;


                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',     status_message = 'Invalid action in generation for respective equipment'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                 UPDATE pgedata.gen_equipment_stage
                 SET status           = 'F',  status_message     = 'Failed due to other eqp failed'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                 UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',   status_message = 'Failed due to generator failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              ELSIF h_action_eq = 'D' THEN
                IF h_action    IN ('U', 'D') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  -- start code to delete PGEDATA_SM_GENERATOR_STAGE

                  UPDATE pgedata.pgedata_sm_generator_stage
                  SET action               = 'D'
                  WHERE sap_equipment_id = h_eq_id
                      AND gen_tech_cd          = h_gen_tech
                      AND protection_id       IN
                    	(SELECT id
                       	FROM pgedata.pgedata_sm_protection_stage
                     	 WHERE parent_type = 'GENERATION'
                         AND parent_id      IN
                      		(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid )
                    	) ;
                   -- end code to delete PGEDATA_SM_GENERATOR_STAGE
                   h_success := 1;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',  status_message = 'Invalid action in generation for respective equipment'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',   status_message     = 'Failed due to other eqp failed'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',  status_message = 'Failed due to generator failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              END IF;
            ELSIF h_gen_tech IN ('INVEXT', 'INVINC') THEN -- 'INVERTER'
              IF h_action_eq  = 'I' THEN              -- checking Eqp action
                IF h_action  IN ('I', 'U') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  BEGIN
                     INSERT
                       INTO pgedata.pgedata_sm_generator_stage
                      (
                        sap_equipment_id    ,
                        protection_id       ,
                        sap_queue_number    ,
                        sap_egi_notification,
                        datecreated         ,
                        createdby           ,
                        power_source        ,
                        project_name        ,
                        manufacturer        ,
                        model               ,
                        inverter_efficiency ,
                        nameplate_rating    ,
                        quantity            ,
                        power_factor        ,
                        eff_rating_kw       ,
                        eff_rating_kva      ,
                        rated_voltage       ,
                        number_of_phases    ,
                        mode_of_inverter    ,
                        gen_tech_cd         ,
                        pto_date            ,
                        program_type        ,
                        gen_tech_equipment  ,
                        certification       ,
                        action              ,
                        tech_type_cd        ,
                        control_cd          ,
                        connection_cd       ,
                        status_cd           ,
                        backup_gen
                      )
                      VALUES
                      (
                        h_eq_id                            ,
                        h_prtcn_id                         ,
                        V_bulk_eq (k) .sap_queue_number    ,
                        V_bulk_eq (k) .sap_egi_notification,
                        SYSDATE                            ,
                        h_user                             ,
                        V_bulk_eq (k) .power_source        ,
                        V_bulk_eq (k) .project_name        ,
                        V_bulk_eq (k) .manufacturer        ,
                        V_bulk_eq (k) .model               ,
                        V_bulk_eq (k) .inverter_efficiency ,
                        V_bulk_eq (k) .nameplate_rating    ,
                        V_bulk_eq (k) .quantity            ,
                        V_bulk_eq (k) .power_factor        ,
                        V_bulk_eq (k) .eff_rating_kw       ,
                        V_bulk_eq (k) .eff_rating_kva      ,
                        V_bulk_eq (k) .rated_voltage       ,
                        V_bulk_eq (k) .number_of_phases    ,
                        V_bulk_eq (k) .mode_of_inv         ,
                        h_gen_tech                         ,
                        V_bulk_eq (k) .pto_date            ,
                        V_bulk_eq (k) .program_type        ,
                        V_bulk_eq (k) .gen_tech_equipment  ,
                        V_bulk_eq (k) .certification       ,
                        h_action_eq                        ,
                        V_bulk_eq (k) .tech_type_cd        ,
                        'NONE'                             ,
                        NULL                               ,
                        'UNSP'                             ,
                        V_bulk_eq (k) .backup_gen
                      ) ;

                     h_success := 1;
                     h_success_insert :=4;
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line   (  SQLERRM   )   ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',   status_message = 'INVERTER creation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',   status_message     = 'Failed due to respective inverter failed'
                    WHERE status      IS NULL
                      AND service_point_id = i.service_point_id;

                    UPDATE pgedata.gen_summary_stage
                    SET status       = 'F',  status_message = 'Failed due to respective inverter failed'
                    WHERE objectid = i.objectid;

                    EXIT;
                  END;

                  -- start of code for DC generation
                  SELECT COUNT ( *)
                  INTO h_cnt_dc
                  FROM pgedata.gen_equipment_stage
                  WHERE Upper (action) = 'I'
                  	AND status            IS NULL
                  	AND gen_tech_cd       IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
                  	AND gen_tech_equipment = h_eq_id
                  	AND service_point_id   = i.service_point_id;

                  -- more than one DC records for single inverter can exist
                  IF h_cnt_dc > 0 THEN
                    BEGIN
                       SELECT id
                       INTO h_gntr_id_3
                       FROM pgedata.pgedata_sm_generator_stage
                       WHERE protection_id = h_prtcn_id
                      	AND sap_equipment_id  = h_eq_id
                      	AND gen_tech_cd       = h_gen_tech;

                       INSERT
                         INTO pgedata.pgedata_sm_gen_equipment_stage
                        (
                          generator_id        ,
                          sap_equipment_id    ,
                          datecreated         ,
                          createdby           ,
                          gen_tech_cd         ,
                          manufacturer        ,
                          model               ,
                          ptc_rated_kw        ,
                          nameplate_rating    ,
                          quantity            ,
                          max_storage_capacity,
                          rated_discharge     ,
                          charge_demand_kw    ,
                          grid_charged        ,
                          action              ,
                          program_type
                        )
                       SELECT h_gntr_id_3                                            ,
                        sap_equipment_id                                             ,
                        SYSDATE                                                      ,
                        h_user                                                       ,
                        pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (gen_tech_cd),
                        manufacturer                                                 ,
                        model                                                        ,
                        ptc_rating                                                   ,
                        nameplate_rating                                             ,
                        quantity                                                     ,
                        max_storage_capacity                                         ,
                        rated_discharge                                              ,
                        charge_demand_kw                                             ,
                        grid_charged                                                 ,
                        'I'                                                          ,
                        program_type
                        FROM pgedata.gen_equipment_stage
                        WHERE Upper (action) = 'I'
                      	AND status            IS NULL
                      	AND gen_tech_cd       IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
                     	  AND gen_tech_equipment = h_eq_id
                      	AND service_point_id   = i.service_point_id;

                       h_success := 2;
                       UPDATE pgedata.gen_equipment_stage
                       SET status             = 'S',  status_message       = 'DC Generator created successfully'
                       WHERE Upper (action) = 'I'
                       	 AND status            IS NULL
                      	 AND gen_tech_cd       IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
                      	 AND gen_tech_equipment = h_eq_id
                      	 AND service_point_id   = i.service_point_id;
                    EXCEPTION
                    WHEN OTHERS THEN
                      ROLLBACK TO err_found2;
                      dbms_output.Put_line (SQLERRM) ;
                      h_success := 0;

                      UPDATE pgedata.gen_equipment_stage
                      SET status       = 'F',    status_message = 'DC GNTR creation failed due to exception capture'
                      WHERE objectid = V_bulk_eq (k) .objectid;

                      UPDATE pgedata.gen_equipment_stage
                      SET status           = 'F',  status_message     = 'Failed due to respective DC GENTR failed'
                      WHERE status      IS NULL
                      	AND service_point_id = i.service_point_id;

                      UPDATE pgedata.gen_summary_stage
                      SET status       = 'F',   status_message = 'Failed due to respective DC GENTR failed'
                      WHERE objectid = i.objectid;

                      EXIT;
                    END;
                  ELSE
                    dbms_output.Put_line ( 'No DC generation for Invertor of SPId : EqpId : GenTechEqp ' ||h_spid ||' : ' ||h_eq_id ||' : ' ||h_gen_tech_eq) ;
                    --ROLLBACK TO err_found2;
                    h_success := 2;

                   /* UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',  status_message = 'No DC Generator for this inverter'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',  status_message     = 'Failed due to respective inverter failed'
                    WHERE status      IS NULL
                    	AND service_point_id = i.service_point_id;

                   UPDATE pgedata.gen_summary_stage
                    SET status       = 'F',  status_message = 'Failed due to respective inverter failed'
                    WHERE objectid = i.objectid;
                    */
                    --EXIT;
                  END IF;
                  -- end of code for DC generation
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',  status_message = 'Invalid action for this inverter'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',  status_message     = 'Failed due to respective inverter action is invalid'
                  WHERE status      IS NULL
                  	AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',  status_message = 'Failed due to respective inverter failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              ELSIF h_action_eq = 'U' THEN
                IF h_action     = 'U' THEN
                  ---- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                 BEGIN
                    -- start code to update PGEDATA_SM_GENERATOR_STAGE
                    UPDATE pgedata.pgedata_sm_generator_stage
                    SET sap_egi_notification = V_bulk_eq (k) .sap_egi_notification,
                      power_source           = V_bulk_eq (k) .power_source        ,
                      project_name           = V_bulk_eq (k) .project_name        ,
                      manufacturer           = V_bulk_eq (k) .manufacturer        ,
                      model                  = V_bulk_eq (k) .model               ,
                      inverter_efficiency    = V_bulk_eq (k) .inverter_efficiency ,
                      nameplate_rating       = V_bulk_eq (k) .nameplate_rating    ,
                      quantity               = V_bulk_eq (k) .quantity            ,
                      power_factor           = V_bulk_eq (k) .power_factor        ,
                      eff_rating_kw          = V_bulk_eq (k) .eff_rating_kw       ,
                      eff_rating_kva         = V_bulk_eq (k) .eff_rating_kva      ,
                      rated_voltage          = V_bulk_eq (k) .rated_voltage       ,
                      number_of_phases       = V_bulk_eq (k) .number_of_phases    ,
                      mode_of_inverter       = V_bulk_eq (k) .mode_of_inv         ,
                      pto_date               = V_bulk_eq (k) .pto_date            ,
                      program_type           = V_bulk_eq (k) .program_type        ,
                      gen_tech_equipment     = V_bulk_eq (k) .gen_tech_equipment  ,
                      certification          = V_bulk_eq (k) .certification       ,
                      action                 = h_action_eq                        ,
                      tech_type_cd           = V_bulk_eq (k) .tech_type_cd        ,
                      backup_gen             = V_bulk_eq (k) .backup_gen
                      WHERE sap_equipment_id = h_eq_id
                    	AND gen_tech_cd          = h_gen_tech
                    	AND protection_id       IN
                      		(SELECT id
                        		 FROM pgedata.pgedata_sm_protection_stage
                        		WHERE parent_type = 'GENERATION'
                      		    AND parent_id      IN
                        			(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid   )
                     		 ) ;

                    h_success := 1;
                  EXCEPTION
                  WHEN OTHERS THEN
                    ROLLBACK TO err_found2;
                    dbms_output.Put_line (SQLERRM) ;
                    h_success := 0;

                    UPDATE pgedata.gen_equipment_stage
                    SET status       = 'F',   status_message = 'inverter updation failed due to exception capture'
                    WHERE objectid = V_bulk_eq (k) .objectid;

                    UPDATE pgedata.gen_equipment_stage
                    SET status           = 'F',   status_message     = 'Failed due to respective inverter updation failed'
                    WHERE status      IS NULL
                      AND service_point_id = i.service_point_id;

                    UPDATE pgedata.gen_summary_stage
                    SET status       = 'F',   status_message = 'Failed due to respective inverter updation failed'
                    WHERE objectid = i.objectid;

                    EXIT;
                  END;

                  -- start of code for DC generation
                  SELECT * bulk collect
                  INTO v_bulk_eq_2
                  FROM pgedata.gen_equipment_stage
                  WHERE status        IS NULL
                  	AND gen_tech_cd       IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
                 	  AND gen_tech_equipment = h_eq_id
                  	AND service_point_id   = i.service_point_id;

                  FOR p IN 1 .. v_bulk_eq_2.count
                  LOOP
                    h_eq_id_dc     := V_bulk_eq_2 (p) .sap_equipment_id;
                    h_action_eq_dc := Upper (V_bulk_eq_2 (p) .action) ;
                    dbms_output.Put_line ('For SP ID ' ||h_spid ||', Equpment ID is ' ||h_eq_id ||' and DC Eqp ID is ' ||h_eq_id_dc) ;

                    IF h_action_eq_dc = 'I' THEN
                      BEGIN
                         SELECT id
                         INTO h_gntr_id_3
                         FROM pgedata.pgedata_sm_generator_stage
                         WHERE sap_equipment_id = h_eq_id
                         	 AND gen_tech_cd          = h_gen_tech;

                         INSERT  INTO pgedata.pgedata_sm_gen_equipment_stage
                          (
                            generator_id        ,
                            sap_equipment_id    ,
                            datecreated         ,
                            createdby           ,
                            gen_tech_cd         ,
                            manufacturer        ,
                            model               ,
                            ptc_rated_kw        ,
                            nameplate_rating    ,
                            quantity            ,
                            max_storage_capacity,
                            rated_discharge     ,
                            charge_demand_kw    ,
                            grid_charged        ,
                            action              ,
                            program_type
                          )
                          VALUES
                          (
                            h_gntr_id_3                                                                   ,
                            h_eq_id_dc                                                                    ,
                            SYSDATE                                                                       ,
                            h_user                                                                        ,
                            pge_enos_to_edgis_dm_pkg.Pge_gen_tech_cd_conv_f (V_bulk_eq_2 (p) .gen_tech_cd),
                            V_bulk_eq_2 (p) .manufacturer                                                 ,
                            V_bulk_eq_2 (p) .model                                                        ,
                            V_bulk_eq_2 (p) .ptc_rating                                                   ,
                            V_bulk_eq_2 (p) .nameplate_rating                                             ,
                            V_bulk_eq_2 (p) .quantity                                                     ,
                            V_bulk_eq_2 (p) .max_storage_capacity                                         ,
                            V_bulk_eq_2 (p) .rated_discharge                                              ,
                            V_bulk_eq_2 (p) .charge_demand_kw                                             ,
                            V_bulk_eq_2 (p) .grid_charged                                                 ,
                            h_action_eq_dc                                                                ,
                            V_bulk_eq_2 (p).program_type
                          ) ;

                         h_success := 2;

                         UPDATE pgedata.gen_equipment_stage
                         SET status       = 'S',  status_message = 'DC Generator updated successfully'
                         WHERE objectid = V_bulk_eq_2 (p) .objectid;

                       EXCEPTION
                       WHEN OTHERS THEN
                         ROLLBACK TO err_found2;
                         dbms_output.Put_line (SQLERRM) ;
                         h_success := 0;

                         UPDATE pgedata.gen_equipment_stage
                         SET status       = 'F', status_message = 'DC GNTR creation failed due to exception capture'
                         WHERE objectid = V_bulk_eq_2 (p) .objectid;

                         UPDATE pgedata.gen_equipment_stage
                         SET status               = 'F',   status_message         = 'Failed due to respective DC GENTR failed'
                         WHERE service_point_id = i.service_point_id;

                         UPDATE pgedata.gen_summary_stage
                         SET status       = 'F',   status_message = 'Failed due to respective DC GENTR failed'
                         WHERE objectid = i.objectid;

                        EXIT;
                      END;
                    ELSIF h_action_eq_dc = 'U' THEN
                      UPDATE pgedata.pgedata_sm_gen_equipment_stage trgt
                      SET trgt.manufacturer       = V_bulk_eq_2 (p) .manufacturer        ,
                        trgt.model                = V_bulk_eq_2 (p) .model               ,
                        trgt.ptc_rated_kw         = V_bulk_eq_2 (p) .ptc_rating          ,
                        trgt.nameplate_rating     = V_bulk_eq_2 (p) .nameplate_rating    ,
                        trgt.quantity             = V_bulk_eq_2 (p) .quantity            ,
                        trgt.max_storage_capacity = V_bulk_eq_2 (p) .max_storage_capacity,
                        trgt.rated_discharge      = V_bulk_eq_2 (p) .rated_discharge     ,
                        trgt.charge_demand_kw     = V_bulk_eq_2 (p) .charge_demand_kw    ,
                        trgt.grid_charged         = V_bulk_eq_2 (p) .grid_charged        ,
                        trgt.action               = h_action_eq_dc                       ,
                        trgt.program_type         = V_bulk_eq_2 (p).program_type
                        WHERE sap_equipment_id    = h_eq_id_dc
                   	 AND generator_id           IN
                        		(SELECT id --SAP_EQUIPMENT_ID
                           		FROM pgedata.pgedata_sm_generator_stage
                          		WHERE protection_id IN
                         			 (SELECT id
                             		FROM pgedata.pgedata_sm_protection_stage
                            			WHERE parent_type = 'GENERATION'
                          			    AND parent_id      IN
                            				(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  )
                         			 )
                        		) ;

                      		h_success         := 2;
                    ELSIF h_action_eq_dc = 'D' THEN
                      UPDATE pgedata.pgedata_sm_gen_equipment_stage
                      SET action               = h_action_eq_dc
                       WHERE sap_equipment_id = h_eq_id_dc
                      	AND generator_id        IN
                       		 (SELECT id --SAP_EQUIPMENT_ID
                           	FROM pgedata.pgedata_sm_generator_stage
                         		WHERE protection_id IN
                         			 (SELECT id
                             		FROM pgedata.pgedata_sm_protection_stage
                            		WHERE parent_type = 'GENERATION'
                          	      AND parent_id      IN
                            				(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  )
                              )
                        		) ;

                     	h_success := 2;
                    END IF;
                  END LOOP;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',  status_message = 'Invalid action for this inverter'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',  status_message     = 'Failed due to respective inverter action is invalid'
                  WHERE status      IS NULL
                  	AND service_point_id = i.service_point_id;

                  UPDATE pgedata.gen_summary_stage
                  SET status       = 'F',   status_message = 'Failed due to respective inverter failed'
                  WHERE objectid = i.objectid;

                  EXIT;
                END IF;
              ELSIF h_action_eq = 'D' THEN
                IF h_action    IN ('U', 'D') THEN
                  -- Validating Summary action for that Eqp action
                  dbms_output.Put_line ('Good. Both actions are valid') ;
                  -- start code to delete PGEDATA_SM_GENERATOR_STAGE

                  UPDATE pgedata.pgedata_sm_gen_equipment_stage
                  SET action            = 'D'
                  WHERE generator_id IN
                    	(SELECT id --SAP_EQUIPMENT_ID
                       FROM pgedata.pgedata_sm_generator_stage
                       WHERE gen_tech_cd IN ('INVEXT', 'INVINC')
                    	   AND protection_id   IN
                      	(SELECT id
                       	 FROM pgedata.pgedata_sm_protection_stage
                       	 WHERE parent_type = 'GENERATION'
                      	   AND parent_id      IN
                        	(SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  )
                     		 )
                   	   ) ;

                  UPDATE pgedata.pgedata_sm_generator_stage
                  SET action          = 'D'
                  WHERE gen_tech_cd = h_gen_tech
                 	  AND protection_id  IN
                    		(SELECT id
                      	 FROM pgedata.pgedata_sm_protection_stage
                      	 WHERE parent_type = 'GENERATION'
                    		   AND parent_id      IN
                     			 (SELECT id FROM pgedata.pgedata_sm_generation_stage WHERE global_id = i.guid  )
                  		  ) ;
                  -- end code to delete PGEDATA_SM_GENERATOR_STAGE
                  h_success := 1;
                ELSE
                  ROLLBACK TO err_found2;
                  h_success := 0;

                  UPDATE pgedata.gen_equipment_stage
                  SET status       = 'F',  status_message = 'Invalid action for this inverter'
                  WHERE objectid = V_bulk_eq (k) .objectid;

                  UPDATE pgedata.gen_equipment_stage
                  SET status           = 'F',  status_message     = 'Failed due to respective inverter action is invalid'
                  WHERE status      IS NULL
                    AND service_point_id = i.service_point_id;

                   UPDATE pgedata.gen_summary_stage
                   SET status       = 'F',  status_message = 'Failed due to respective inverter failed'
                   WHERE objectid = i.objectid;

                   EXIT;
                END IF;
              END IF;
            END IF;
          EXCEPTION
          WHEN no_data_found THEN
            dbms_output.Put_line ('No protection record exist for SP ID ' ||h_spid) ;
            ROLLBACK TO err_found2;
            h_success := 0;

            UPDATE pgedata.gen_equipment_stage
            SET status       = 'F',   status_message = 'No protection record exist'
            WHERE objectid = V_bulk_eq (k) .objectid;

            UPDATE pgedata.gen_equipment_stage
            SET status           = 'F',  status_message     = 'No protection record exist'
            WHERE status      IS NULL
              AND service_point_id = i.service_point_id;

            UPDATE pgedata.gen_summary_stage
            SET status       = 'F',  status_message = 'Failed due to respective inverter failed'
            WHERE objectid = i.objectid;

         WHEN OTHERS THEN
            dbms_output.Put_line (SQLERRM ||' : SP ID for ' ||h_spid) ;
            ROLLBACK TO err_found2;
            h_success := 0;

            UPDATE pgedata.gen_equipment_stage
            SET status       = 'F',  status_message = 'Failed due to invalid protection'
            WHERE objectid = V_bulk_eq (k) .objectid;

            UPDATE pgedata.gen_equipment_stage
            SET status           = 'F',   status_message     = 'Failed due to invalid protection'
            WHERE status      IS NULL
            AND service_point_id = i.service_point_id;

            UPDATE pgedata.gen_summary_stage
            SET status       = 'F',    status_message = 'Failed due to respective inverter failed'
            WHERE objectid = i.objectid;
         END; --4
        END LOOP;
        -- end code for equipment details
        IF h_success_insert = 4 THEN
         UPDATE pgedata.gen_equipment_stage
          SET status               = 'S',   status_message         = 'Generator Created successfully'
          WHERE service_point_id = i.service_point_id
          	AND status              IS NULL
          	AND gen_tech_cd         IN ('GEN.SYNCH', 'GEN.INDCT', 'GEN.INVEXT', 'GEN.INVINC') ;

         UPDATE pgedata.gen_summary_stage
          SET status       = 'S',  status_message = 'Successfully generation Created'
          WHERE objectid = i.objectid;

          END IF;

        IF h_success IN (1, 2) THEN
          UPDATE pgedata.gen_equipment_stage
          SET status               = 'S',   status_message         = 'Generator updated successfully'
          WHERE service_point_id = i.service_point_id
          	AND status              IS NULL
          	AND gen_tech_cd         IN ('GEN.SYNCH', 'GEN.INDCT', 'GEN.INVEXT', 'GEN.INVINC') ;

          UPDATE pgedata.gen_equipment_stage
          SET status              = 'S',   status_message        = 'DC Generator updated successfully'
          WHERE status         IS NULL
             AND service_point_id    = i.service_point_id
             AND gen_tech_cd        IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
             AND gen_tech_equipment IN
             	(SELECT sap_equipment_id
               FROM pgedata.gen_equipment_stage
               WHERE service_point_id = i.service_point_id
            	   AND status               = 'S'
           	     AND gen_tech_cd         IN ('GEN.INVEXT', 'GEN.INVINC')
           	 ) ;

          UPDATE pgedata.gen_summary_stage
          SET status       = 'S',  status_message = 'Successfully generation updated(I/D/U)'
          WHERE objectid = i.objectid and status is null;

          COMMIT;
        ELSIF h_success = 3 THEN
          UPDATE pgedata.gen_summary_stage
          SET status       = 'S',  status_message = 'Successfully generation updated(I/D/U)'
          WHERE objectid = i.objectid;

          COMMIT;
        ELSE
          ROLLBACK TO err_found2;

          ----Changes for Specific Message start
          UPDATE pgedata.gen_equipment_stage
          SET status               = 'F',   status_message         = 'Failed Due to Invalid Data'
          WHERE service_point_id = i.service_point_id
          	AND status              IS NULL
         	  AND gen_tech_cd         IN ('GEN.SYNCH', 'GEN.INDCT', 'GEN.INVEXT', 'GEN.INVINC') ;

          UPDATE pgedata.gen_equipment_stage
          SET status              = 'F',  status_message        = 'Failed Due to Invalid Data'
          WHERE status         IS NULL
          	AND service_point_id    = i.service_point_id
         	  AND gen_tech_cd        IN ('PWR.PVPNL', 'PWR.BATT', 'PWR.WIND', 'PWR.GENRTR')
          	AND gen_tech_equipment IN
           		 (SELECT sap_equipment_id
               	FROM pgedata.gen_equipment_stage
              		WHERE service_point_id = i.service_point_id
            			AND status               = 'F'
            			AND gen_tech_cd         IN ('GEN.INVEXT', 'GEN.INVINC')
            		) ;

          UPDATE pgedata.gen_summary_stage
          SET status       = 'F',  status_message = 'Failed Due to Invalid Data'
          WHERE objectid = i.objectid;
        END IF;
      ELSE
        dbms_output.Put_line ('Generation not created/updated/delete and did not go for eqp dtls.') ;

        UPDATE pgedata.gen_equipment_stage
        SET status               = 'F',   status_message         = 'Failed due to generation failed'
        WHERE service_point_id = i.service_point_id
        	AND status              IS NULL;

        UPDATE pgedata.gen_summary_stage
        SET status       = 'F',  status_message = 'Failed Due to Invalid Data'
        WHERE objectid = i.objectid
          AND status      IS NULL;

          ----Changes for Specific Message Eend
      END IF;
    END;
  END LOOP;

   UPDATE pgedata.pgedata_generationinfo_stage
   SET gentype    = pge_enos_to_edgis_dm_pkg.Pge_cal_gen_type_f (service_point_id)
   WHERE action = 'I';

   merge INTO pgedata.pgedata_sm_generation_stage trgt USING pgedata.pgedata_generationinfo_stage src ON (trgt.global_id = src.globalid)
   WHEN matched THEN
     UPDATE SET trgt.gen_type = src.gentype WHERE trgt.action = 'I';

	update pgedata.pgedata_sm_generator_stage set backup_gen='Y' where backup_gen='X';

    update pgedata.pgedata_sm_generation_stage set BACKUP_GENERATION = 'N'
      where SAP_EGI_NOTIFICATION in (select distinct(sap_egi_notification) from pgedata.pgedata_sm_generator_stage where BACKUP_GEN='N');

     update pgedata.pgedata_sm_generation_stage set BACKUP_GENERATION = 'Y'
      where SAP_EGI_NOTIFICATION in (select distinct(sap_egi_notification) from pgedata.pgedata_sm_generator_stage where BACKUP_GEN='Y');

    merge INTO pgedata.pgedata_generationinfo_stage trgt USING pgedata.pgedata_sm_generation_stage src ON (trgt.globalid = src.global_id)
   WHEN matched THEN
     UPDATE SET trgt.backupgen = src.backup_generation WHERE src.backup_generation is not null;
--declare genttype varchar2(50);
--BEGIN
--select gen_type into genttype from pgedata_sm_generation_stage;
----delete from tmp_Test;
----commit;
--insert into tmp_Test values(genttype,'');
--END;

-- Updating the description and not the code for gentype field in sm_generation table
--   UPDATE pgedata.pgedata_sm_generation_stage SET gen_type = pge_enos_to_edgis_dm_pkg.Pge_gen_type_rev_conv(gen_type,'I') WHERE action = 'I';

   -- Updating the description and not the code for gentype field in sm_generation table
--   UPDATE pgedata.pgedata_sm_generation_stage SET gen_type = pge_enos_to_edgis_dm_pkg.Pge_gen_type_rev_conv(gen_type,'U') WHERE action = 'U';


   UPDATE pgedata.pgedata_generationinfo_stage
   SET status      = 'S'
   WHERE status IS NULL;

   UPDATE pgedata.pgedata_sm_generation_stage
   SET status      = 'S'
   WHERE status IS NULL;

   UPDATE pgedata.pgedata_sm_protection_stage
   SET status      = 'S'
   WHERE status IS NULL;

   UPDATE pgedata.pgedata_sm_generator_stage
   SET status      = 'S'
   WHERE status IS NULL;

   UPDATE pgedata.pgedata_sm_gen_equipment_stage
   SET status      = 'S'
   WHERE status IS NULL;

   out_flag := 'TRUE';

  COMMIT;

END;
----------------------------------------------------------------------------------
-- Function to get the Label Text

FUNCTION F_GET_GENERATION_LABEL (Input_servicelocationguid varchar2) return varchar2 Is

-- To get the generation info for that service location guid
CURSOR C1 Is
SELECT gen.projectname,
       NVL (gen.effratingmachkw, 0) erm,
       NVL (gen.effratinginvkw, 0) eri,
       sp.servicepointid spid
FROM edgis.zz_mv_servicepoint sp, edgis.zz_mv_generationinfo gen -- pgedata.pgedata_generationinfo_stage gen
WHERE sp.servicelocationguid = Input_servicelocationguid
  AND sp.globalid = gen.servicepointguid
ORDER BY 1;

h_proj_name  varchar2(40) := NULL;
h_erm 	     number := 0;
h_eri 	     number := 0;
h_label_text varchar2(200);
h_total_size number := 0;
h_spid_cnt   number := 0;
h_prv_sp_id  varchar2(100) := NULL;

BEGIN

 --Get the connected service point and generation
 For i in c1 loop

    -- See if the project name already captured. Only one to be selected
    if h_proj_name is null then
       h_proj_name := i.projectname;
    end if;

--dbms_output.put_line(h_proj_name||'-'||i.projectname);

    -- Sum the size
    h_erm := h_erm + i.erm;
    h_eri := h_eri + i.eri;

    -- Check how many service point linked that service location and connected to generation
    if h_prv_sp_id <> i.spid then
        h_spid_cnt := h_spid_cnt + 1;
        h_prv_sp_id := i.spid;
    end if;

 End Loop;

 -- Prepare the label. Label text length is 200. leaving 25 character for size etc
 If h_proj_name is not null then
    h_label_text  := substr(h_proj_name,1,175) ||' ';
 End If;

--dbms_output.put_line('1'||h_label_text);

 -- calculate the total size
 h_total_size := h_erm + h_eri;

 -- add size to the label
 If h_total_size < 10 Then
    h_label_text  := h_label_text || to_char(round(h_total_size,1)) || ' KW';

 elsif h_total_size >= 10 and h_total_size < 1000 Then
     h_label_text  := h_label_text || to_char(round(h_total_size,0)) || ' KW';

 else
     h_label_text  := h_label_text || to_char(round(h_total_size/1000,1)) || ' MW';
 end if;

--dbms_output.put_line('2'||h_label_text);
 return h_label_text;

Exception
WHEN OTHERS Then
	return 'Error';
END;
--------------------------------------------------------------------------------------
-- Procedure to update the label

PROCEDURE PGE_UPDATE_GENERATION_LABEL IS
BEGIN

  UPDATE EDGIS.ZZ_MV_SERVICELOCATION
  SET labeltext = PGE_ENOS_TO_EDGIS_DM_PKG.F_GET_GENERATION_LABEL(globalid)
  WHERE globalid IN
     ( SELECT distinct sp.servicelocationguid
       FROM edgis.zz_mv_servicepoint sp, edgis.zz_mv_generationinfo gen
       where  sp.globalid = gen.servicepointguid);

  COMMIT;

END;
--------------------------------------------------------------------------------
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
