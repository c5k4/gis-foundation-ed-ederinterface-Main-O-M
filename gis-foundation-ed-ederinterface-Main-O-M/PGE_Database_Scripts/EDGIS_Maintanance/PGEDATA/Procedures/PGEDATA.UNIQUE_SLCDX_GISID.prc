Prompt drop Procedure UNIQUE_SLCDX_GISID;
DROP PROCEDURE PGEDATA.UNIQUE_SLCDX_GISID
/

Prompt Procedure UNIQUE_SLCDX_GISID;
--
-- UNIQUE_SLCDX_GISID  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.UNIQUE_SLCDX_GISID
AS
BEGIN
  EXECUTE immediate ' truncate table slcdx_uniquedata_gisid reuse storage';
  EXECUTE immediate 'drop index SLCDX_GISIDX';
  INSERT /*+ append */
  INTO PGEDATA.slcdx_uniquedata_gisid
  SELECT office,
    person_name,
    account_number,
    badge_number,
    fixture_code,
    status,
    status_flag,
    receive_date,
    retire_date,
    install_date,
    removal_date,
    change_of_party_date,
    descriptive_address,
    map_number,
    rate_schedule,
    item_type_code,
    operating_schedule,
    service,
    fixture_manufacturer,
    pole_type,
    pole_length,
    suspension,
    pole_use,
    sp_id,
    sa_id,
    prem_id,
    tot_code,
    tot_terr_desc,
    inventory_date,
    inventoried_by,
    sp_item_hist,
    unique_sp_id,
    gis_id,
    ballast_ch_dt,
    lamp_ch_dt,
    line_share_sw,
    lite_size,
    lite_type,
    lumn_ch_dt,
    pcell_ch_dt,
    pcell,
    pole_ch_dt,
    pole_pt_dt,
    strt_ch_dt,
    litesize_type,
    litetype_type,
    halfhradj_type,
    mail_addr1,
    mail_addr2,
    mail_city,
    mail_state,
    mail_zip,
    rownum
  FROM PGEDATA.slcdx_data
  WHERE (gis_id,status,status_flag,install_date,sp_id) IN
    (SELECT gis_id,
      status,
      status_flag,
      install_date,
      sp_id
    FROM
      (SELECT gis_id,
        MAX(INSTALL_DATE) over (partition BY gis_id) install_date,
        MAX(sp_id) over (partition BY gis_id) sp_id,
        MIN(status) over (partition BY gis_id) status,
        MIN(status_flag) over (partition BY gis_id) status_flag,
        row_number() over(partition BY gis_id ORDER BY STATUS) rn
      FROM PGEDATA.SLCDX_DATA
      )
    WHERE rn=1
    );
  COMMIT;
  --dont know why, but the next statement inserts another 500 odd records
  INSERT
    /*+ append */
  INTO PGEDATA.slcdx_uniquedata_gisid
  SELECT office,
    person_name,
    account_number,
    badge_number,
    fixture_code,
    status,
    status_flag,
    receive_date,
    retire_date,
    install_date,
    removal_date,
    change_of_party_date,
    descriptive_address,
    map_number,
    rate_schedule,
    item_type_code,
    operating_schedule,
    service,
    fixture_manufacturer,
    pole_type,
    pole_length,
    suspension,
    pole_use,
    sp_id,
    sa_id,
    prem_id,
    tot_code,
    tot_terr_desc,
    inventory_date,
    inventoried_by,
    sp_item_hist,
    unique_sp_id,
    a.gis_id,
    ballast_ch_dt,
    lamp_ch_dt,
    line_share_sw,
    lite_size,
    lite_type,
    lumn_ch_dt,
    pcell_ch_dt,
    pcell,
    pole_ch_dt,
    pole_pt_dt,
    strt_ch_dt,
    litesize_type,
    litetype_type,
    halfhradj_type,
    mail_addr1,
    mail_addr2,
    mail_city,
    mail_state,
    mail_zip,
    rownum
  FROM PGEDATA.slcdx_data a,(SELECT gis_id
    FROM SLCDX_DATA
    WHERE gis_id NOT IN
      (SELECT gis_id FROM slcdx_uniquedata_gisid
      )
    )b
  WHERE a.status   ='A'
  AND a.status_flag='A'
  AND a.gis_id  = b.gis_id
    ;
  COMMIT;
  --pretty lame way to remove dupes introduced by the previous statement
  DELETE
  FROM slcdx_uniquedata_gisid t1
  WHERE rowid <>
    (SELECT MAX(rowid) FROM slcdx_uniquedata_gisid t2 WHERE t1.gis_id =t2.gis_id
    );
  COMMIT;
  EXECUTE immediate 'create unique index SLCDX_GISIDX on slcdx_uniquedata_gisid(gis_id)';
  EXECUTE immediate 'analyze table slcdx_uniquedata_gisid compute statistics for all indexed columns for all indexes for table';
  dbms_output.put_line(' slcdx_uniquedata_gisid-->Populated SLCDX_UNIQUEDATA_GISID table' || TO_CHAR(SYSDATE,'dd-mm-yyyy-hh-mi-ss') );
END;
/


Prompt Grants on PROCEDURE UNIQUE_SLCDX_GISID TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.UNIQUE_SLCDX_GISID TO GIS_I
/
