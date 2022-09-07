Prompt drop Procedure UPDATE_STL_CCB;
DROP PROCEDURE PGEDATA.UPDATE_STL_CCB
/

Prompt Procedure UPDATE_STL_CCB;
--
-- UPDATE_STL_CCB  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.UPDATE_STL_CCB
IS
BEGIN
  UPDATE PGEDATA.v_ccbupdate
  SET account_number     = account_number2,
    badge_number         = badge_number2,
    change_of_party_date = change_of_party_date2,
    descriptive_address  = descriptive_address2,
    fixture_code         = fixture_code2,
    fixture_manufacturer = fixture_manufacturer2,
    install_date         = install_date2,
    item_type_code       = item_type_code2,
    map_number           = map_number2,
    office               = office2,
    operating_schedule   = operating_schedule2,
    person_name          = person_name2,
    pole_length          = pole_length2,
    pole_type            = pole_type2,
    pole_use             = pole_use2,
    prem_id              = prem_id2,
    rate_schedule        = rate_schedule2,
    receive_date         = receive_date2,
    removal_date         = removal_date2,
    retire_date          = retire_date2,
    sp_id                = sp_id2,
    sa_id                = sa_id2,
    service              = service2,
    sp_item_hist         = sp_item_hist2,
    status               = status2,
    status_flag          = status_flag2,
    suspension           = suspension2,
    tot_code             = tot_code2,
    tot_terr_desc        = tot_terr_desc2,
    unique_sp_id         = unique_sp_id2,
    ballast_ch_dt        = ballast_ch_dt2,
    halfhradj_type       = halfhradj_type2,
    lamp_ch_dt           = lamp_ch_dt2,
    line_share_sw        = line_share_sw2,
    lite_size            = lite_size2,
    lite_type            = lite_type2,
    litesize_type        = litesize_type2,
    litetype_type        = litetype_type2,
    lumn_ch_dt           = lumn_ch_dt2,
    mail_addr1           = mail_addr12,
    mail_addr2           = mail_addr22,
    mail_city            = mail_city2,
    mail_state           = mail_state2,
    mail_zip             = mail_zip2,
    pcell                = pcell2,
    pcell_ch_dt          = pcell_ch_dt2,
    pole_ch_dt           = pole_ch_dt2,
    pole_pt_dt           = pole_pt_dt2,
    strt_ch_dt           = strt_ch_dt2;
  COMMIT;
  dbms_output.put_line(' update_stl_ccb-->updated STREETLIGHT_INV base table' || TO_CHAR(SYSDATE,'dd-mm-yyyy-hh-mi-ss') );
  UPDATE EDGIS.STREETLIGHT_INV
  SET offset_angle = DECODE(fixture_code, '1',60, '2',240, '3',300, '4',120, '5',0, '6',180, '7',90, '8',270, '9',210, 30);
  COMMIT;
  -- EXECUTE immediate 'analyze table EDGIS.streetlight_INV compute statistics for all indexed columns for all indexes for table';
END update_stl_ccb;
/


Prompt Grants on PROCEDURE UPDATE_STL_CCB TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.UPDATE_STL_CCB TO GIS_I
/
