Prompt drop Procedure UNIQUE_SLCDX;
DROP PROCEDURE PGEDATA.UNIQUE_SLCDX
/

Prompt Procedure UNIQUE_SLCDX;
--
-- UNIQUE_SLCDX  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.UNIQUE_SLCDX
AS
BEGIN
  execute immediate ' truncate table slcdx_uniquedata reuse storage';
insert /*+ append */ into pgedata.slcdx_uniquedata
 (select rownum, office, person_name, account_number, badge_number, fixture_code,
   status, status_flag, receive_date, retire_date, install_date, removal_date,
   change_of_party_date, descriptive_address, map_number, rate_schedule,
   item_type_code, operating_schedule, service, fixture_manufacturer, pole_type,
   pole_length, suspension, pole_use, sp_id, sa_id, prem_id, tot_code,
   tot_terr_desc, inventory_date, inventoried_by, sp_item_hist, unique_sp_id,
   gis_id, ballast_ch_dt, lamp_ch_dt, line_share_sw, lite_size, lite_type,
   lumn_ch_dt, pcell_ch_dt, pcell, pole_ch_dt, pole_pt_dt, strt_ch_dt,
   litesize_type, litetype_type, halfhradj_type, mail_addr1, mail_addr2,
   mail_city, mail_state, mail_zip from pgedata.slcdx_data
  where
    --sp_id not in ( select sp_id from stl.v_badspids) and
    (sp_id,install_date,REMOVAL_DATE,status,status_flag) in
       (   select distinct sp_id,
              max(INSTALL_DATE) over (partition by sp_id) install_date,
              max(REMOVAL_DATE) over (partition by sp_id) REMOVAL_DATE,
              min(status) over (partition by sp_id) status,
              min(status_flag) over (partition by sp_id) status_flag
           from pgedata.SLCDX_DATA
       )
  );
execute immediate 'analyze table slcdx_uniquedata compute statistics for all indexed columns for all  indexes for table';
     dbms_output.put_line(' update_stl_ccb-->Populated SLCDX_UNIQUEDATA table' ||
     to_Char(SYSDATE,'dd-mm-yyyy-hh-mi-ss')  );
end;
/


Prompt Grants on PROCEDURE UNIQUE_SLCDX TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.UNIQUE_SLCDX TO GIS_I
/
