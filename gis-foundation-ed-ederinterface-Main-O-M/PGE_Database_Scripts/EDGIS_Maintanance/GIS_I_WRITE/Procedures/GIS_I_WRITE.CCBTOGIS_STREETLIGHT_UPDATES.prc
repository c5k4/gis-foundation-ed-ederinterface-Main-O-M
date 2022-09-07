Prompt drop Procedure CCBTOGIS_STREETLIGHT_UPDATES;
DROP PROCEDURE GIS_I_WRITE.CCBTOGIS_STREETLIGHT_UPDATES
/

Prompt Procedure CCBTOGIS_STREETLIGHT_UPDATES;
--
-- CCBTOGIS_STREETLIGHT_UPDATES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE GIS_I_WRITE."CCBTOGIS_STREETLIGHT_UPDATES" AS
  v_table_id        VARCHAR2(20);
  v_sapSeqName      VARCHAR2(20);
  v_ATableSyncSql   VARCHAR2(10000);
  v_ATableSql       VARCHAR2(10000);
  v_ATableSql2      VARCHAR2(10000);
BEGIN

  -- Get the sequence name for the SAP sync table
  SELECT 'EDGIS.R' || (SELECT registration_id
                FROM sde.table_registry
                WHERE table_name='PGE_GISSAP_REPROCESSASSETSYNC')
  INTO v_sapSeqName
  FROM Dual;

  -- Get the streetlight FC's A table
  SELECT 'A' || (SELECT registration_id
                FROM sde.table_registry
                WHERE table_name='STREETLIGHT')
  INTO v_table_id
  FROM Dual;

  -- Insert rows from the streetlight base table that we'll update into the sync table
  INSERT INTO edgis.pge_gissap_reprocessassetsync (objectid, assetid, featureclassname, datecreated, createduser)
    SELECT gis_i_write.getnextsapseq(v_sapSeqName), sl.globalid, 'EDGIS.STREETLIGHT', sysdate, 'EDGIS'
    FROM edgis.streetlight sl
    INNER JOIN pgedata.v_slcdx_data_active sub_slc ON sl.uniquespid=sub_slc.unique_sp_id
    WHERE sl.globalid NOT IN (SELECT assetid FROM edgis.pge_gissap_reprocessassetsync)
    AND sl.customerowned<>'Y'
    AND (
          sl.office<>sub_slc.office OR
          (sl.office is null AND sub_slc.office is not null) OR
          sl.customername<>sub_slc.person_name OR
          (sl.customername is null AND sub_slc.person_name is not null) OR
          sl.accountnumber<>sub_slc.account_number OR
          (sl.accountnumber is null AND sub_slc.account_number is not null) OR
          sl.statusflag<>sub_slc.status_flag OR
          (sl.statusflag is null AND sub_slc.status_flag is not null) OR
          (to_char(sub_slc.receive_date,'yyyymmdd') > 18991231 AND
            (sl.receivedate<>sub_slc.receive_date OR
            (sl.receivedate is null AND sub_slc.receive_date is not null))
          ) OR
          (to_char(sub_slc.install_date,'yyyymmdd') > 18991231 AND
            (sl.installationdate<>sub_slc.install_date OR
            (sl.installationdate is null AND sub_slc.install_date is not null))
          ) OR
          (to_char(sub_slc.removal_date,'yyyymmdd') > 18991231 AND
            (sl.removedate<>sub_slc.removal_date OR
            (sl.removedate is null AND sub_slc.removal_date is not null))
          ) OR
          (to_char(sub_slc.change_of_party_date,'yyyymmdd') > 18991231 AND
            (sl.changeofpartydate<>sub_slc.change_of_party_date OR
            (sl.changeofpartydate is null AND sub_slc.change_of_party_date is not null))
          ) OR
          sl.descriptiveaddress<>sub_slc.descriptive_address OR
          (sl.descriptiveaddress is null AND sub_slc.descriptive_address is not null) OR
          sl.rateschedule<>sub_slc.rate_schedule OR
          (sl.rateschedule is null AND sub_slc.rate_schedule is not null) OR
          sl.itemtypecode<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '') FROM DUAL) OR
          (sl.itemtypecode is null AND sub_slc.item_type_code is not null) OR
          sl.operatingschedule<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '') FROM DUAL) OR
          (sl.operatingschedule is null AND sub_slc.operating_schedule is not null) OR
          sl.fixturemanufacturer<>sub_slc.fixture_manufacturer OR
          (sl.fixturemanufacturer is null AND sub_slc.fixture_manufacturer is not null) OR
          sl.poletype<>sub_slc.pole_type OR
          (sl.poletype is null AND sub_slc.pole_type is not null) OR
          sl.polelength<>sub_slc.pole_length OR
          (sl.polelength is null AND sub_slc.pole_length is not null) OR
          sl.suspension<>sub_slc.suspension OR
          (sl.suspension is null AND sub_slc.suspension is not null) OR
          sl.poleuse<>sub_slc.pole_use OR
          (sl.poleuse is null AND sub_slc.pole_use is not null) OR
          sl.serviceagreementid<>sub_slc.sa_id OR
          (sl.serviceagreementid is null AND sub_slc.sa_id is not null) OR
          sl.premiseid<>sub_slc.prem_id OR
          (sl.premiseid is null AND sub_slc.prem_id is not null) OR
          sl.totcode<>sub_slc.tot_code OR
          (sl.totcode is null AND sub_slc.tot_code is not null) OR
          sl.townterrdesc<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '') FROM DUAL) OR
          (sl.townterrdesc is null AND sub_slc.tot_terr_desc is not null) OR
          (to_char(sub_slc.inventory_date,'yyyymmdd') > 18991231 AND
            (sl.inventorydate<>sub_slc.inventory_date OR
            (sl.inventorydate is null AND sub_slc.inventory_date is not null))
          ) OR
          sl.inventoriedby<>SUBSTR(sub_slc.inventoried_by,1,5) OR
          (sl.inventoriedby is null AND sub_slc.inventoried_by is not null) OR
          sl.spitemhistory<>sub_slc.sp_item_hist OR
          (sl.spitemhistory is null AND sub_slc.sp_item_hist is not null) OR
          (to_char(sub_slc.ballast_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.ballastchangedate<>sub_slc.ballast_ch_dt OR
            (sl.ballastchangedate is null AND sub_slc.ballast_ch_dt is not null))
          ) OR
          (to_char(sub_slc.lamp_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.lampchangedate<>sub_slc.lamp_ch_dt OR
            (sl.lampchangedate is null AND sub_slc.lamp_ch_dt is not null))
          ) OR
          (to_char(sub_slc.lumn_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.lumnchangedate<>sub_slc.lumn_ch_dt OR
            (sl.lumnchangedate is null AND sub_slc.lumn_ch_dt is not null))
          ) OR
          (to_char(sub_slc.pcell_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.pcellchangedate<>sub_slc.pcell_ch_dt OR
            (sl.pcellchangedate is null AND sub_slc.pcell_ch_dt is not null))
          ) OR
          sl.pcell<>CASE WHEN sub_slc.pcell='CHGPCELL' THEN 'CHGCELL'
                      ELSE sub_slc.pcell
                    END OR
          (sl.pcell is null AND sub_slc.pcell is not null) OR
          (to_char(sub_slc.pole_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.polechangedate<>sub_slc.pole_ch_dt OR
            (sl.polechangedate is null AND sub_slc.pole_ch_dt is not null))
          ) OR
          (to_char(sub_slc.pole_pt_dt,'yyyymmdd') > 18991231 AND
            (sl.polepaintdate<>sub_slc.pole_pt_dt OR
            (sl.polepaintdate is null AND sub_slc.pole_pt_dt is not null))
          ) OR
          (to_char(sub_slc.strt_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.strtchdt<>sub_slc.strt_ch_dt OR
            (sl.strtchdt is null AND sub_slc.strt_ch_dt is not null))
          ) OR
          sl.litesizetype<>sub_slc.litesize_type OR
          (sl.litesizetype is null AND sub_slc.litesize_type is not null) OR
          sl.litetypetype<>sub_slc.litetype_type OR
          (sl.litetypetype is null AND sub_slc.litetype_type is not null) OR
          sl.halfhradjtype<>sub_slc.halfhradj_type OR
          (sl.halfhradjtype is null AND sub_slc.halfhradj_type is not null) OR
          sl.mailaddr1<>sub_slc.mail_addr1 OR
          (sl.mailaddr1 is null AND sub_slc.mail_addr1 is not null) OR
          sl.mailaddr2<>sub_slc.mail_addr2 OR
          (sl.mailaddr2 is null AND sub_slc.mail_addr2 is not null) OR
          sl.mailcity<>sub_slc.mail_city OR
          (sl.mailcity is null AND sub_slc.mail_city is not null) OR
          sl.mailstate<>sub_slc.mail_state OR
          (sl.mailstate is null AND sub_slc.mail_state is not null) OR
          sl.mailzip<>sub_slc.mail_zip OR
          (sl.mailzip is null AND sub_slc.mail_zip is not null)
      );

  -- Insert rows from the streetlights A table that we'll update into the sync table
  v_ATableSyncSql := 'INSERT INTO edgis.pge_gissap_reprocessassetsync (objectid, assetid, featureclassname, datecreated, createduser)' ||
    ' SELECT gis_i_write.getnextsapseq(''' || v_sapSeqName || '''), sla.globalid, ''EDGIS.STREETLIGHT'', sysdate, ''EDGIS''' ||
    ' FROM edgis.' || v_table_id || ' sla' ||
    ' INNER JOIN pgedata.v_slcdx_data_active sub_slc ON sla.uniquespid=sub_slc.unique_sp_id' ||
    ' WHERE sla.globalid NOT IN (SELECT assetid FROM edgis.pge_gissap_reprocessassetsync)' ||
    ' AND sla.customerowned<>''Y'' ' ||
    ' AND (' ||
          ' sla.office<>sub_slc.office OR' ||
          '(sla.office is null AND sub_slc.office is not null) OR ' ||
          'sla.customername<>sub_slc.person_name OR ' ||
          '(sla.customername is null AND sub_slc.person_name is not null) OR ' ||
          'sla.accountnumber<>sub_slc.account_number OR ' ||
          '(sla.accountnumber is null AND sub_slc.account_number is not null) OR ' ||
          'sla.statusflag<>sub_slc.status_flag OR ' ||
          '(sla.statusflag is null AND sub_slc.status_flag is not null) OR ' ||
          '(to_char(sub_slc.receive_date,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.receivedate<>sub_slc.receive_date OR ' ||
            '(sla.receivedate is null AND sub_slc.receive_date is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.install_date,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.installationdate<>sub_slc.install_date OR ' ||
            '(sla.installationdate is null AND sub_slc.install_date is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.removal_date,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.removedate<>sub_slc.removal_date OR ' ||
            '(sla.removedate is null AND sub_slc.removal_date is not null)) ' ||
          ') OR ' ||
          '(to_char(sub_slc.change_of_party_date,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.changeofpartydate<>sub_slc.change_of_party_date OR ' ||
            '(sla.changeofpartydate is null AND sub_slc.change_of_party_date is not null))' ||
          ') OR ' ||
          'sla.descriptiveaddress<>sub_slc.descriptive_address OR ' ||
          '(sla.descriptiveaddress is null AND sub_slc.descriptive_address is not null) OR ' ||
          'sla.rateschedule<>sub_slc.rate_schedule OR ' ||
          '(sla.rateschedule is null AND sub_slc.rate_schedule is not null) OR ' ||
          'sla.itemtypecode<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '''') FROM DUAL) OR ' ||
          '(sla.itemtypecode is null AND sub_slc.item_type_code is not null) OR ' ||
          'sla.operatingschedule<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '''') FROM DUAL) OR ' ||
          '(sla.operatingschedule is null AND sub_slc.operating_schedule is not null) OR ' ||
          'sla.fixturemanufacturer<>sub_slc.fixture_manufacturer OR ' ||
          '(sla.fixturemanufacturer is null AND sub_slc.fixture_manufacturer is not null) OR ' ||
          'sla.poletype<>sub_slc.pole_type OR ' ||
          '(sla.poletype is null AND sub_slc.pole_type is not null) OR ' ||
          'sla.polelength<>sub_slc.pole_length OR ' ||
          '(sla.polelength is null AND sub_slc.pole_length is not null) OR ' ||
          'sla.suspension<>sub_slc.suspension OR ' ||
          '(sla.suspension is null AND sub_slc.suspension is not null) OR ' ||
          'sla.poleuse<>sub_slc.pole_use OR ' ||
          '(sla.poleuse is null AND sub_slc.pole_use is not null) OR ' ||
          'sla.serviceagreementid<>sub_slc.sa_id OR ' ||
          '(sla.serviceagreementid is null AND sub_slc.sa_id is not null) OR ' ||
          'sla.premiseid<>sub_slc.prem_id OR ' ||
          '(sla.premiseid is null AND sub_slc.prem_id is not null) OR ' ||
          'sla.totcode<>sub_slc.tot_code OR ' ||
          '(sla.totcode is null AND sub_slc.tot_code is not null) OR ' ||
          'sla.townterrdesc<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '''') FROM DUAL) OR ' ||
          '(sla.townterrdesc is null AND sub_slc.tot_terr_desc is not null) OR ' ||
          '(to_char(sub_slc.inventory_date,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.inventorydate<>sub_slc.inventory_date OR ' ||
            '(sla.inventorydate is null AND sub_slc.inventory_date is not null))' ||
          ') OR ' ||
          'sla.inventoriedby<>SUBSTR(sub_slc.inventoried_by,1,5) OR ' ||
          '(sla.inventoriedby is null AND sub_slc.inventoried_by is not null) OR ' ||
          'sla.spitemhistory<>sub_slc.sp_item_hist OR ' ||
          '(sla.spitemhistory is null AND sub_slc.sp_item_hist is not null) OR ' ||
          '(to_char(sub_slc.ballast_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.ballastchangedate<>sub_slc.ballast_ch_dt OR ' ||
            '(sla.ballastchangedate is null AND sub_slc.ballast_ch_dt is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.lamp_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.lampchangedate<>sub_slc.lamp_ch_dt OR ' ||
            '(sla.lampchangedate is null AND sub_slc.lamp_ch_dt is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.lumn_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.lumnchangedate<>sub_slc.lumn_ch_dt OR ' ||
            '(sla.lumnchangedate is null AND sub_slc.lumn_ch_dt is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.pcell_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.pcellchangedate<>sub_slc.pcell_ch_dt OR ' ||
            '(sla.pcellchangedate is null AND sub_slc.pcell_ch_dt is not null))' ||
          ') OR ' ||
          ' sla.pcell<>CASE WHEN sub_slc.pcell=''CHGPCELL'' THEN ''CHGCELL'' ELSE sub_slc.pcell END OR' ||
          '(sla.pcell is null AND sub_slc.pcell is not null) OR ' ||
          '(to_char(sub_slc.pole_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.polechangedate<>sub_slc.pole_ch_dt OR ' ||
            '(sla.polechangedate is null AND sub_slc.pole_ch_dt is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.pole_pt_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.polepaintdate<>sub_slc.pole_pt_dt OR ' ||
            '(sla.polepaintdate is null AND sub_slc.pole_pt_dt is not null))' ||
          ') OR ' ||
          '(to_char(sub_slc.strt_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
            '(sla.strtchdt<>sub_slc.strt_ch_dt OR ' ||
            '(sla.strtchdt is null AND sub_slc.strt_ch_dt is not null))' ||
          ') OR ' ||
          'sla.litesizetype<>sub_slc.litesize_type OR ' ||
          '(sla.litesizetype is null AND sub_slc.litesize_type is not null) OR ' ||
          'sla.litetypetype<>sub_slc.litetype_type OR ' ||
          '(sla.litetypetype is null AND sub_slc.litetype_type is not null) OR ' ||
          'sla.halfhradjtype<>sub_slc.halfhradj_type OR ' ||
          '(sla.halfhradjtype is null AND sub_slc.halfhradj_type is not null) OR ' ||
          'sla.mailaddr1<>sub_slc.mail_addr1 OR ' ||
          '(sla.mailaddr1 is null AND sub_slc.mail_addr1 is not null) OR ' ||
          'sla.mailaddr2<>sub_slc.mail_addr2 OR ' ||
          '(sla.mailaddr2 is null AND sub_slc.mail_addr2 is not null) OR ' ||
          'sla.mailcity<>sub_slc.mail_city OR ' ||
          '(sla.mailcity is null AND sub_slc.mail_city is not null) OR ' ||
          'sla.mailstate<>sub_slc.mail_state OR ' ||
          '(sla.mailstate is null AND sub_slc.mail_state is not null) OR ' ||
          'sla.mailzip<>sub_slc.mail_zip OR ' ||
          '(sla.mailzip is null AND sub_slc.mail_zip is not null)' ||
      ')';

  -- Execute dynamic SQL on A table
  dbms_output.put_line (v_ATableSyncSql);
  EXECUTE IMMEDIATE v_ATableSyncSql;

  -- Blast updates to the Streetlight base table
  MERGE INTO edgis.streetlight sl
    USING  (SELECT  slc.unique_sp_id,
                    slc.office, slc.person_name, slc.account_number,
                    slc.status_flag, slc.receive_date,
                    slc.retire_date, slc.install_date, slc.removal_date,
                    slc.change_of_party_date, slc.descriptive_address,
                    slc.rate_schedule, slc.item_type_code, slc.operating_schedule,
                    slc.fixture_manufacturer, slc.pole_type, slc.pole_length,
                    slc.suspension, slc.pole_use, slc.sa_id, slc.prem_id,
                    slc.tot_code, slc.tot_terr_desc, slc.inventory_date,
                    slc.inventoried_by, slc.sp_item_hist, slc.ballast_ch_dt,
                    slc.lamp_ch_dt, slc.lumn_ch_dt, slc.pcell_ch_dt, slc.pcell, slc.pole_ch_dt,
                    slc.pole_pt_dt, slc.strt_ch_dt, slc.litesize_type, slc.litetype_type,
                    slc.halfhradj_type, slc.mail_addr1, slc.mail_addr2, slc.mail_city,
                    slc.mail_state, slc.mail_zip
            FROM pgedata.v_slcdx_data_active slc
            GROUP BY  slc.unique_sp_id,
                      slc.office, slc.person_name, slc.account_number,
                      slc.status_flag, slc.receive_date,
                      slc.retire_date, slc.install_date, slc.removal_date,
                      slc.change_of_party_date, slc.descriptive_address,
                      slc.rate_schedule, slc.item_type_code, slc.operating_schedule,
                      slc.fixture_manufacturer, slc.pole_type, slc.pole_length,
                      slc.suspension, slc.pole_use, slc.sa_id, slc.prem_id,
                      slc.tot_code, slc.tot_terr_desc, slc.inventory_date,
                      slc.inventoried_by, slc.sp_item_hist, slc.ballast_ch_dt,
                      slc.lamp_ch_dt, slc.lumn_ch_dt, slc.pcell_ch_dt, slc.pcell, slc.pole_ch_dt,
                      slc.pole_pt_dt, slc.strt_ch_dt, slc.litesize_type, slc.litetype_type,
                      slc.halfhradj_type, slc.mail_addr1, slc.mail_addr2, slc.mail_city,
                      slc.mail_state, slc.mail_zip) sub_slc
    ON (sl.uniquespid=sub_slc.unique_sp_id)
  WHEN MATCHED THEN
    UPDATE SET  sl.office=sub_slc.office,
                sl.customername=sub_slc.person_name,
                sl.accountnumber=sub_slc.account_number,
                sl.statusflag=sub_slc.status_flag,
                sl.receivedate=CASE
                  WHEN to_char(sub_slc.receive_date,'yyyymmdd') > 18991231 THEN sub_slc.receive_date
                END,
                sl.installationdate=CASE
                  WHEN to_char(sub_slc.install_date,'yyyymmdd') > 18991231 THEN sub_slc.install_date
                END,
                sl.removedate=CASE
                  WHEN to_char(sub_slc.removal_date,'yyyymmdd') > 18991231 THEN sub_slc.removal_date
                END,
                sl.changeofpartydate=CASE
                  WHEN to_char(sub_slc.change_of_party_date, 'yyyymmdd') > 18991231 THEN sub_slc.change_of_party_date
                END,
                sl.descriptiveaddress=sub_slc.descriptive_address,
                sl.rateschedule=sub_slc.rate_schedule,
                sl.itemtypecode=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '') FROM DUAL),
                sl.operatingschedule=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '') FROM DUAL),
                sl.fixturemanufacturer=sub_slc.fixture_manufacturer,
                sl.poletype=sub_slc.pole_type,
                sl.polelength=sub_slc.pole_length,
                sl.suspension=sub_slc.suspension,
                sl.poleuse=sub_slc.pole_use,
                sl.serviceagreementid=sub_slc.sa_id,
                sl.premiseid=sub_slc.prem_id,
                sl.totcode=sub_slc.tot_code,
                sl.townterrdesc=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '') FROM DUAL),
                sl.inventorydate=CASE
                  WHEN to_char(sub_slc.inventory_date,'yyyymmdd') > 18991231 THEN sub_slc.inventory_date
                END,
                sl.inventoriedby=SUBSTR(sub_slc.inventoried_by,1,5),
                sl.spitemhistory=sub_slc.sp_item_hist,
                sl.ballastchangedate=CASE
                  WHEN to_char(sub_slc.ballast_ch_dt, 'yyyymmdd') > 18991231 THEN sub_slc.ballast_ch_dt
                END,
                sl.lampchangedate=CASE
                  WHEN to_char(sub_slc.lamp_ch_dt, 'yyyymmdd') > 18991231 THEN sub_slc.lamp_ch_dt
                END,
                sl.lumnchangedate=CASE
                  WHEN to_char(sub_slc.lumn_ch_dt, 'yyyymmdd') > 18991231 THEN sub_slc.lumn_ch_dt
                END,
                sl.pcellchangedate=CASE
                  WHEN to_char(sub_slc.pcell_ch_dt, 'yyyymmdd') > 18991231 THEN sub_slc.lumn_ch_dt
                END,
                sl.pcell=CASE
                  WHEN sub_slc.pcell='CHGPCELL' THEN 'CHGCELL'
                  ELSE sub_slc.pcell
                END,
                sl.polechangedate=CASE
                  WHEN to_char(sub_slc.pole_ch_dt, 'yyyymmdd') > 18991231 THEN sub_slc.lumn_ch_dt
                END,
                sl.polepaintdate=CASE
                  WHEN to_char(sub_slc.pole_pt_dt, 'yyyymmdd') > 18991231 THEN sub_slc.lumn_ch_dt
                END,
                sl.strtchdt=CASE
                  WHEN to_char(sub_slc.strt_ch_dt,'yyyymmdd') > 18991231 THEN sub_slc.strt_ch_dt
                END,
                sl.litesizetype=sub_slc.litesize_type,
                sl.litetypetype=sub_slc.litetype_type,
                sl.halfhradjtype=sub_slc.halfhradj_type,
                sl.mailaddr1=sub_slc.mail_addr1,
                sl.mailaddr2=sub_slc.mail_addr2,
                sl.mailcity=sub_slc.mail_city,
                sl.mailstate=sub_slc.mail_state,
                sl.mailzip=sub_slc.mail_zip
    WHERE sl.office<>sub_slc.office OR
          (sl.office is null AND sub_slc.office is not null) OR
          sl.customername<>sub_slc.person_name OR
          (sl.customername is null AND sub_slc.person_name is not null) OR
          sl.accountnumber<>sub_slc.account_number OR
          (sl.accountnumber is null AND sub_slc.account_number is not null) OR
          sl.statusflag<>sub_slc.status_flag OR
          (sl.statusflag is null AND sub_slc.status_flag is not null) OR
          (to_char(sub_slc.receive_date,'yyyymmdd') > 18991231 AND
            (sl.receivedate<>sub_slc.receive_date OR
            (sl.receivedate is null AND sub_slc.receive_date is not null))
          ) OR
          (to_char(sub_slc.install_date,'yyyymmdd') > 18991231 AND
            (sl.installationdate<>sub_slc.install_date OR
            (sl.installationdate is null AND sub_slc.install_date is not null))
          ) OR
          (to_char(sub_slc.removal_date,'yyyymmdd') > 18991231 AND
            (sl.removedate<>sub_slc.removal_date OR
            (sl.removedate is null AND sub_slc.removal_date is not null))
          ) OR
          (to_char(sub_slc.change_of_party_date,'yyyymmdd') > 18991231 AND
            (sl.changeofpartydate<>sub_slc.change_of_party_date OR
            (sl.changeofpartydate is null AND sub_slc.change_of_party_date is not null))
          ) OR
          sl.descriptiveaddress<>sub_slc.descriptive_address OR
          (sl.descriptiveaddress is null AND sub_slc.descriptive_address is not null) OR
          sl.rateschedule<>sub_slc.rate_schedule OR
          (sl.rateschedule is null AND sub_slc.rate_schedule is not null) OR
          sl.itemtypecode<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '') FROM DUAL) OR
          (sl.itemtypecode is null AND sub_slc.item_type_code is not null) OR
          sl.operatingschedule<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '') FROM DUAL) OR
          (sl.operatingschedule is null AND sub_slc.operating_schedule is not null) OR
          sl.fixturemanufacturer<>sub_slc.fixture_manufacturer OR
          (sl.fixturemanufacturer is null AND sub_slc.fixture_manufacturer is not null) OR
          sl.poletype<>sub_slc.pole_type OR
          (sl.poletype is null AND sub_slc.pole_type is not null) OR
          sl.polelength<>sub_slc.pole_length OR
          (sl.polelength is null AND sub_slc.pole_length is not null) OR
          sl.suspension<>sub_slc.suspension OR
          (sl.suspension is null AND sub_slc.suspension is not null) OR
          sl.poleuse<>sub_slc.pole_use OR
          (sl.poleuse is null AND sub_slc.pole_use is not null) OR
          sl.serviceagreementid<>sub_slc.sa_id OR
          (sl.serviceagreementid is null AND sub_slc.sa_id is not null) OR
          sl.premiseid<>sub_slc.prem_id OR
          (sl.premiseid is null AND sub_slc.prem_id is not null) OR
          sl.totcode<>sub_slc.tot_code OR
          (sl.totcode is null AND sub_slc.tot_code is not null) OR
          sl.townterrdesc<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '') FROM DUAL) OR
          (sl.townterrdesc is null AND sub_slc.tot_terr_desc is not null) OR
          (to_char(sub_slc.inventory_date,'yyyymmdd') > 18991231 AND
            (sl.inventorydate<>sub_slc.inventory_date OR
            (sl.inventorydate is null AND sub_slc.inventory_date is not null))
          ) OR
          sl.inventoriedby<>SUBSTR(sub_slc.inventoried_by,1,5) OR
          (sl.inventoriedby is null AND sub_slc.inventoried_by is not null) OR
          sl.spitemhistory<>sub_slc.sp_item_hist OR
          (sl.spitemhistory is null AND sub_slc.sp_item_hist is not null) OR
          (to_char(sub_slc.ballast_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.ballastchangedate<>sub_slc.ballast_ch_dt OR
            (sl.ballastchangedate is null AND sub_slc.ballast_ch_dt is not null))
          ) OR
          (to_char(sub_slc.lamp_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.lampchangedate<>sub_slc.lamp_ch_dt OR
            (sl.lampchangedate is null AND sub_slc.lamp_ch_dt is not null))
          ) OR
          (to_char(sub_slc.lumn_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.lumnchangedate<>sub_slc.lumn_ch_dt OR
            (sl.lumnchangedate is null AND sub_slc.lumn_ch_dt is not null))
          ) OR
          (to_char(sub_slc.pcell_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.pcellchangedate<>sub_slc.pcell_ch_dt OR
            (sl.pcellchangedate is null AND sub_slc.pcell_ch_dt is not null))
          ) OR
          sl.pcell<>sub_slc.pcell OR
          (sl.pcell is null AND sub_slc.pcell is not null) OR
          (to_char(sub_slc.pole_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.polechangedate<>sub_slc.pole_ch_dt OR
            (sl.polechangedate is null AND sub_slc.pole_ch_dt is not null))
          ) OR
          (to_char(sub_slc.pole_pt_dt,'yyyymmdd') > 18991231 AND
            (sl.polepaintdate<>sub_slc.pole_pt_dt OR
            (sl.polepaintdate is null AND sub_slc.pole_pt_dt is not null))
          ) OR
          (to_char(sub_slc.strt_ch_dt,'yyyymmdd') > 18991231 AND
            (sl.strtchdt<>sub_slc.strt_ch_dt OR
            (sl.strtchdt is null AND sub_slc.strt_ch_dt is not null))
          ) OR
          sl.litesizetype<>sub_slc.litesize_type OR
          (sl.litesizetype is null AND sub_slc.litesize_type is not null) OR
          sl.litetypetype<>sub_slc.litetype_type OR
          (sl.litetypetype is null AND sub_slc.litetype_type is not null) OR
          sl.halfhradjtype<>sub_slc.halfhradj_type OR
          (sl.halfhradjtype is null AND sub_slc.halfhradj_type is not null) OR
          sl.mailaddr1<>sub_slc.mail_addr1 OR
          (sl.mailaddr1 is null AND sub_slc.mail_addr1 is not null) OR
          sl.mailaddr2<>sub_slc.mail_addr2 OR
          (sl.mailaddr2 is null AND sub_slc.mail_addr2 is not null) OR
          sl.mailcity<>sub_slc.mail_city OR
          (sl.mailcity is null AND sub_slc.mail_city is not null) OR
          sl.mailstate<>sub_slc.mail_state OR
          (sl.mailstate is null AND sub_slc.mail_state is not null) OR
          sl.mailzip<>sub_slc.mail_zip OR
          (sl.mailzip is null AND sub_slc.mail_zip is not null);

  -- Blast updates to the Streetlights A table
  v_ATableSql := 'MERGE INTO edgis.' || v_table_id || ' sla ' ||
    'USING  (SELECT  slc.unique_sp_id, slc.office, slc.person_name, slc.account_number, slc.status_flag, slc.receive_date, slc.retire_date, slc.install_date, slc.removal_date, slc.change_of_party_date,
    slc.descriptive_address, slc.rate_schedule, slc.item_type_code, slc.operating_schedule, slc.fixture_manufacturer, slc.pole_type, slc.pole_length, slc.suspension, slc.pole_use, slc.sa_id,
    slc.prem_id, slc.tot_code, slc.tot_terr_desc, slc.inventory_date, slc.inventoried_by, slc.sp_item_hist, slc.ballast_ch_dt, slc.lamp_ch_dt, slc.lumn_ch_dt, slc.pcell_ch_dt, slc.pcell, slc.pole_ch_dt,
    slc.pole_pt_dt, slc.strt_ch_dt, slc.litesize_type, slc.litetype_type, slc.halfhradj_type, slc.mail_addr1, slc.mail_addr2, slc.mail_city, slc.mail_state, slc.mail_zip
    FROM pgedata.v_slcdx_data_active slc
    GROUP BY  slc.unique_sp_id, slc.office, slc.person_name, slc.account_number, slc.status_flag, slc.receive_date, slc.retire_date, slc.install_date, slc.removal_date, slc.change_of_party_date,
    slc.descriptive_address, slc.rate_schedule, slc.item_type_code, slc.operating_schedule, slc.fixture_manufacturer, slc.pole_type, slc.pole_length, slc.suspension, slc.pole_use, slc.sa_id,
    slc.prem_id,  slc.tot_code, slc.tot_terr_desc, slc.inventory_date, slc.inventoried_by, slc.sp_item_hist, slc.ballast_ch_dt, slc.lamp_ch_dt, slc.lumn_ch_dt, slc.pcell_ch_dt, slc.pcell, slc.pole_ch_dt,
    slc.pole_pt_dt, slc.strt_ch_dt, slc.litesize_type, slc.litetype_type, slc.halfhradj_type, slc.mail_addr1, slc.mail_addr2, slc.mail_city, slc.mail_state, slc.mail_zip) sub_slc' ||
    ' ON (sla.uniquespid=sub_slc.unique_sp_id) ';

  v_ATableSql2 := ' WHEN MATCHED THEN ' ||
    'UPDATE SET sla.office=sub_slc.office,' ||
    'sla.customername=sub_slc.person_name,' ||
    'sla.accountnumber=sub_slc.account_number,' ||
    'sla.statusflag=sub_slc.status_flag,' ||
    'sla.receivedate=CASE ' ||
      'WHEN to_char(sub_slc.receive_date,''yyyymmdd'') > 18991231 THEN sub_slc.receive_date ' ||
    'END,' ||
    'sla.installationdate=CASE ' ||
      'WHEN to_char(sub_slc.install_date,''yyyymmdd'') > 18991231 THEN sub_slc.install_date ' ||
    'END,' ||
    'sla.removedate=CASE ' ||
      'WHEN to_char(sub_slc.removal_date,''yyyymmdd'') > 18991231 THEN sub_slc.removal_date ' ||
    'END,' ||
    'sla.changeofpartydate=CASE ' ||
      'WHEN to_char(sub_slc.change_of_party_date,''yyyymmdd'') > 18991231 THEN sub_slc.change_of_party_date ' ||
    'END,' ||
    'sla.descriptiveaddress=sub_slc.descriptive_address,' ||
    'sla.rateschedule=sub_slc.rate_schedule,' ||
    'sla.itemtypecode=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '''') FROM DUAL),' ||
    'sla.operatingschedule=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '''') FROM DUAL),' ||
    'sla.fixturemanufacturer=sub_slc.fixture_manufacturer,' ||
    'sla.poletype=sub_slc.pole_type,' ||
    'sla.polelength=sub_slc.pole_length,' ||
    'sla.suspension=sub_slc.suspension,' ||
    'sla.poleuse=sub_slc.pole_use,' ||
    'sla.serviceagreementid=sub_slc.sa_id,' ||
    'sla.premiseid=sub_slc.prem_id,' ||
    'sla.totcode=sub_slc.tot_code,' ||
    'sla.townterrdesc=(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '''') FROM DUAL),' ||
    'sla.inventorydate=CASE ' ||
      'WHEN to_char(sub_slc.inventory_date,''yyyymmdd'') > 18991231 THEN sub_slc.inventory_date ' ||
    'END,' ||
    'sla.inventoriedby=SUBSTR(sub_slc.inventoried_by,1,5),' ||
    'sla.spitemhistory=sub_slc.sp_item_hist,' ||
    'sla.ballastchangedate=CASE ' ||
      'WHEN to_char(sub_slc.ballast_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.ballast_ch_dt ' ||
    'END,' ||
    'sla.lampchangedate=CASE ' ||
      'WHEN to_char(sub_slc.lamp_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.lamp_ch_dt ' ||
    'END,' ||
    'sla.lumnchangedate=CASE ' ||
      'WHEN to_char(sub_slc.lumn_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.lumn_ch_dt ' ||
    'END,' ||
    'sla.pcellchangedate=CASE ' ||
      'WHEN to_char(sub_slc.pcell_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.pcell_ch_dt ' ||
    'END,' ||
    'sla.pcell=CASE
        WHEN sub_slc.pcell=''CHGPCELL'' THEN ''CHGCELL''
        ELSE sub_slc.pcell
     END,' ||
    'sla.polechangedate=CASE ' ||
      'WHEN to_char(sub_slc.pole_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.pole_ch_dt ' ||
    'END,' ||
    'sla.polepaintdate=CASE ' ||
      'WHEN to_char(sub_slc.pole_pt_dt,''yyyymmdd'') > 18991231 THEN sub_slc.pole_pt_dt ' ||
    'END,' ||
    'sla.strtchdt=CASE ' ||
      'WHEN to_char(sub_slc.strt_ch_dt,''yyyymmdd'') > 18991231 THEN sub_slc.strt_ch_dt ' ||
    'END,' ||
    'sla.litesizetype=sub_slc.litesize_type,' ||
    'sla.litetypetype=sub_slc.litetype_type,' ||
    'sla.halfhradjtype=sub_slc.halfhradj_type,' ||
    'sla.mailaddr1=sub_slc.mail_addr1,' ||
    'sla.mailaddr2=sub_slc.mail_addr2,' ||
    'sla.mailcity=sub_slc.mail_city,' ||
    'sla.mailstate=sub_slc.mail_state,' ||
    'sla.mailzip=sub_slc.mail_zip' ||
    ' WHERE sla.office<>sub_slc.office OR ' ||
    '(sla.office is null AND sub_slc.office is not null) OR ' ||
    'sla.customername<>sub_slc.person_name OR ' ||
    '(sla.customername is null AND sub_slc.person_name is not null) OR ' ||
    'sla.accountnumber<>sub_slc.account_number OR ' ||
    '(sla.accountnumber is null AND sub_slc.account_number is not null) OR ' ||
    'sla.statusflag<>sub_slc.status_flag OR ' ||
    '(sla.statusflag is null AND sub_slc.status_flag is not null) OR ' ||
    '(to_char(sub_slc.receive_date,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.receivedate<>sub_slc.receive_date OR ' ||
      '(sla.receivedate is null AND sub_slc.receive_date is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.install_date,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.installationdate<>sub_slc.install_date OR ' ||
      '(sla.installationdate is null AND sub_slc.install_date is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.removal_date,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.removedate<>sub_slc.removal_date OR ' ||
      '(sla.removedate is null AND sub_slc.removal_date is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.change_of_party_date,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.changeofpartydate<>sub_slc.change_of_party_date OR ' ||
      '(sla.changeofpartydate is null AND sub_slc.change_of_party_date is not null))' ||
    ') OR ' ||
    'sla.descriptiveaddress<>sub_slc.descriptive_address OR ' ||
    '(sla.descriptiveaddress is null AND sub_slc.descriptive_address is not null) OR ' ||
    'sla.rateschedule<>sub_slc.rate_schedule OR ' ||
    '(sla.rateschedule is null AND sub_slc.rate_schedule is not null) OR ' ||
    'sla.itemtypecode<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_ITEMTYPECODE_LOOKUP WHERE ccb=sub_slc.item_type_code), '''') FROM DUAL) OR ' ||
    '(sla.itemtypecode is null AND sub_slc.item_type_code is not null) OR ' ||
    'sla.operatingschedule<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_OPSCHEDULE_LOOKUP WHERE ccb=sub_slc.operating_schedule), '''') FROM DUAL) OR ' ||
    '(sla.operatingschedule is null AND sub_slc.operating_schedule is not null) OR ' ||
    'sla.fixturemanufacturer<>sub_slc.fixture_manufacturer OR ' ||
    '(sla.fixturemanufacturer is null AND sub_slc.fixture_manufacturer is not null) OR ' ||
    'sla.poletype<>sub_slc.pole_type OR ' ||
    '(sla.poletype is null AND sub_slc.pole_type is not null) OR ' ||
    'sla.polelength<>sub_slc.pole_length OR ' ||
    '(sla.polelength is null AND sub_slc.pole_length is not null) OR ' ||
    'sla.suspension<>sub_slc.suspension OR ' ||
    '(sla.suspension is null AND sub_slc.suspension is not null) OR ' ||
    'sla.poleuse<>sub_slc.pole_use OR ' ||
    '(sla.poleuse is null AND sub_slc.pole_use is not null) OR ' ||
    'sla.serviceagreementid<>sub_slc.sa_id OR ' ||
    '(sla.serviceagreementid is null AND sub_slc.sa_id is not null) OR ' ||
    'sla.premiseid<>sub_slc.prem_id OR ' ||
    '(sla.premiseid is null AND sub_slc.prem_id is not null) OR ' ||
    'sla.totcode<>sub_slc.tot_code OR ' ||
    '(sla.totcode is null AND sub_slc.tot_code is not null) OR ' ||
    'sla.townterrdesc<>(SELECT COALESCE((SELECT gis FROM pgedata.CCBTOGIS_TOWNTERR_LOOKUP WHERE ccb=sub_slc.tot_terr_desc), '''') FROM DUAL) OR ' ||
    '(sla.townterrdesc is null AND sub_slc.tot_terr_desc is not null) OR ' ||
    '(to_char(sub_slc.inventory_date,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.inventorydate<>sub_slc.inventory_date OR ' ||
      '(sla.inventorydate is null AND sub_slc.inventory_date is not null))' ||
    ') OR ' ||
    'sla.inventoriedby<>sub_slc.inventoried_by OR ' ||
    '(sla.inventoriedby is null AND sub_slc.inventoried_by is not null) OR ' ||
    'sla.spitemhistory<>sub_slc.sp_item_hist OR ' ||
    '(sla.spitemhistory is null AND sub_slc.sp_item_hist is not null) OR ' ||
    '(to_char(sub_slc.ballast_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.ballastchangedate<>sub_slc.ballast_ch_dt OR ' ||
      '(sla.ballastchangedate is null AND sub_slc.ballast_ch_dt is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.lamp_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.lampchangedate<>sub_slc.lamp_ch_dt OR ' ||
      '(sla.lampchangedate is null AND sub_slc.lamp_ch_dt is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.lumn_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.lumnchangedate<>sub_slc.lumn_ch_dt OR ' ||
      '(sla.lumnchangedate is null AND sub_slc.lumn_ch_dt is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.pcell_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.pcellchangedate<>sub_slc.pcell_ch_dt OR ' ||
      '(sla.pcellchangedate is null AND sub_slc.pcell_ch_dt is not null))' ||
    ') OR ' ||
    'sla.pcell<>sub_slc.pcell OR ' ||
    '(sla.pcell is null AND sub_slc.pcell is not null) OR ' ||
    '(to_char(sub_slc.pole_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.polechangedate<>sub_slc.pole_ch_dt OR ' ||
      '(sla.polechangedate is null AND sub_slc.pole_ch_dt is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.pole_pt_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.polepaintdate<>sub_slc.pole_pt_dt OR ' ||
      '(sla.polepaintdate is null AND sub_slc.pole_pt_dt is not null))' ||
    ') OR ' ||
    '(to_char(sub_slc.strt_ch_dt,''yyyymmdd'') > 18991231 AND ' ||
      '(sla.strtchdt<>sub_slc.strt_ch_dt OR ' ||
      '(sla.strtchdt is null AND sub_slc.strt_ch_dt is not null))' ||
    ') OR ' ||
    'sla.litesizetype<>sub_slc.litesize_type OR ' ||
    '(sla.litesizetype is null AND sub_slc.litesize_type is not null) OR ' ||
    'sla.litetypetype<>sub_slc.litetype_type OR ' ||
    '(sla.litetypetype is null AND sub_slc.litetype_type is not null) OR ' ||
    'sla.halfhradjtype<>sub_slc.halfhradj_type OR ' ||
    '(sla.halfhradjtype is null AND sub_slc.halfhradj_type is not null) OR ' ||
    'sla.mailaddr1<>sub_slc.mail_addr1 OR ' ||
    '(sla.mailaddr1 is null AND sub_slc.mail_addr1 is not null) OR ' ||
    'sla.mailaddr2<>sub_slc.mail_addr2 OR ' ||
    '(sla.mailaddr2 is null AND sub_slc.mail_addr2 is not null) OR ' ||
    'sla.mailcity<>sub_slc.mail_city OR ' ||
    '(sla.mailcity is null AND sub_slc.mail_city is not null) OR ' ||
    'sla.mailstate<>sub_slc.mail_state OR ' ||
    '(sla.mailstate is null AND sub_slc.mail_state is not null) OR ' ||
    'sla.mailzip<>sub_slc.mail_zip OR ' ||
    '(sla.mailzip is null AND sub_slc.mail_zip is not null)';

  dbms_output.put_line (v_ATableSql || v_ATableSql2);
  EXECUTE IMMEDIATE v_ATableSql || v_ATableSql2;

  -- Were done
  COMMIT;
  dbms_output.put_line('CCB->GIS Update processed successfully');

EXCEPTION
  WHEN OTHERS THEN
    ROLLBACK;
    dbms_output.put_line('CCB->GIS Update cancelled due to error');
    RAISE;
END CCBTOGIS_STREETLIGHT_UPDATES;
/
