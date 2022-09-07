Prompt drop Trigger PGE_STREETLIGHT_INV_UPDATE;
DROP TRIGGER EDGIS.PGE_STREETLIGHT_INV_UPDATE
/

Prompt Trigger PGE_STREETLIGHT_INV_UPDATE;
--
-- PGE_STREETLIGHT_INV_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.PGE_STREETLIGHT_INV_UPDATE
AFTER UPDATE ON EDGIS.STREETLIGHT_INV
FOR EACH ROW
DECLARE
  v_User VARCHAR2(20);
  v_RegId VARCHAR2(20);
BEGIN

  -- Get the current user
  SELECT sys_context('USERENV', 'SESSION_USER')
  INTO v_User
  FROM dual;

  -- If its not the GIS write user (aka, its not the sync process)
  IF UPPER(v_User) != 'GIS_I_WRITE' AND :old.GIS_ID IS NULL AND :new.GIS_ID IS NOT NULL AND (:new.TRANSACTION_='1' OR :new.TRANSACTION_='2') THEN

    -- Grab the reg ID
    SELECT registration_id
    INTO v_RegId
    FROM sde.table_registry
    WHERE table_name='STREETLIGHT_INV_STG';

    -- And copy the entered row into the staging table as well
    INSERT INTO streetlight_inv_stg
    (objectid, newbadge, fmetricom, frevenue, far1, far2, far3, farother, maintnote, fappliance1, fappliance2, fappliance3, fappliance4, fappliance5, meter, paintpole,
    transaction_,far4, far5, diffbadge, difffix, diffaddr, diffmap, diffrs, diffit, office, person_name, account_number, badge_number, fixture_code, status,
    status_flag, receive_date, retire_date, install_date, removal_date, change_of_party_date, descriptive_address, map_number, rate_schedule, item_type_code,
    operating_schedule, service, fixture_manufacturer, pole_type, pole_length, suspension, pole_use, sp_id, sa_id, prem_id, tot_code, tot_terr_desc,
    inventory_date, inventoried_by, sp_item_hist, unique_sp_id, gis_id, ballast_ch_dt, lamp_ch_dt, line_share_sw, lite_size, lite_type, lumn_ch_dt, pcell_ch_dt,
    pcell, pole_ch_dt, pole_pt_dt, strt_ch_dt, litesize_type, litetype_type, halfhradj_type, mail_addr1, mail_addr2, mail_city, mail_state, mail_zip,
    hist_gems_map_rate, hist_gems_badgenum, hist_gems_aka, userid, hist_gems_map_num, nearest_st, cityname, streetnm, altnm, new_grid_mapnum, gems_distr_mapnum,
    ccb_overwrite_flag, last_modify_date, offset_angle, map_number_new, shape)
    VALUES
    ((SELECT SDE.VERSION_USER_DDL.NEXT_ROW_ID('edgis',v_RegId) FROM DUAL), :new.newbadge, :new.fmetricom, :new.frevenue, :new.far1, :new.far2, :new.far3, :new.farother, :new.maintnote, :new.fappliance1, :new.fappliance2,
    :new.fappliance3, :new.fappliance4, :new.fappliance5, :new.meter, :new.paintpole, :new.transaction_, :new.far4, :new.far5, :new.diffbadge, :new.difffix,
    :new.diffaddr, :new.diffmap, :new.diffrs, :new.diffit, :new.office, :new.person_name, :new.account_number, :new.badge_number, :new.fixture_code, :new.status,
    :new.status_flag, :new.receive_date, :new.retire_date, :new.install_date, :new.removal_date, :new.change_of_party_date, :new.descriptive_address, :new.map_number,
    :new.rate_schedule, :new.item_type_code, :new.operating_schedule, :new.service, :new.fixture_manufacturer, :new.pole_type, :new.pole_length, :new.suspension,
    :new.pole_use, :new.sp_id, :new.sa_id, :new.prem_id, :new.tot_code, :new.tot_terr_desc, :new.inventory_date, :new.inventoried_by, :new.sp_item_hist,
    :new.unique_sp_id, :new.gis_id, :new.ballast_ch_dt, :new.lamp_ch_dt, :new.line_share_sw, :new.lite_size, :new.lite_type, :new.lumn_ch_dt, :new.pcell_ch_dt,
    :new.pcell, :new.pole_ch_dt, :new.pole_pt_dt, :new.strt_ch_dt, :new.litesize_type, :new.litetype_type, :new.halfhradj_type, :new.mail_addr1, :new.mail_addr2,
    :new.mail_city, :new.mail_state, :new.mail_zip, :new.hist_gems_map_rate, :new.hist_gems_badgenum, :new.hist_gems_aka, :new.userid, :new.hist_gems_map_num,
    :new.nearest_st, :new.cityname, :new.streetnm, :new.altnm, :new.new_grid_mapnum, :new.gems_distr_mapnum, :new.ccb_overwrite_flag, :new.last_modify_date,
    :new.offset_angle, :new.map_number_new, :new.shape);

  END IF;

END;
/
