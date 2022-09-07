Prompt drop Procedure DATA_LOAD_VALIDATION;
DROP PROCEDURE EDTLM.DATA_LOAD_VALIDATION
/

Prompt Procedure DATA_LOAD_VALIDATION;
--
-- DATA_LOAD_VALIDATION  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDTLM."DATA_LOAD_VALIDATION" (runDate DATE)
AS
  invalid_cdw_data EXCEPTION;
  invalid_ccb_data EXCEPTION;
  v_cnt            NUMBER;
  v_tab            VARCHAR2(30);
  v_month          NUMBER;
  v_count          NUMBER;

BEGIN

-- Validate CDW: data for current month and next month should exist in EXT tables

  -- EXT_SM_SP_GEN_LOAD
  SELECT count(*) into v_count FROM EXT_SM_SP_GEN_LOAD where
  to_number(to_char(to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(runDate,'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_SP_GEN_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;
  SELECT count(*) into v_count FROM EXT_SM_SP_GEN_LOAD where
  to_number(to_char(to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(add_months(runDate,1),'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_SP_GEN_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;

  -- EXT_SM_SP_LOAD
  SELECT count(*) into v_count FROM EXT_SM_SP_LOAD where
  to_number(to_char(to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(runDate,'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_SP_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;
  SELECT count(*) into v_count FROM EXT_SM_SP_LOAD where
  to_number(to_char(to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(add_months(runDate,1),'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_SP_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;

  -- EXT_SM_TRF_GEN_LOAD
  SELECT count(*) into v_count FROM EXT_SM_TRF_GEN_LOAD where
  to_number(to_char(to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(runDate,'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_TRF_GEN_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;
  SELECT count(*) into v_count FROM EXT_SM_TRF_GEN_LOAD where
  to_number(to_char(to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(add_months(runDate,1),'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_TRF_GEN_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;

  -- EXT_SM_TRF_LOAD
  SELECT count(*) into v_count FROM EXT_SM_TRF_LOAD where
  to_number(to_char(to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(runDate,'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_TRF_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;
  SELECT count(*) into v_count FROM EXT_SM_TRF_LOAD where
  to_number(to_char(to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI'),'mm')) = to_number(TO_CHAR(add_months(runDate,1),'mm'));
  IF v_count  = 0 THEN
    v_tab     := 'EXT_SM_TRF_LOAD' ;
    RAISE invalid_cdw_data;
  END IF;

--  Validate if loaded CCB data is latest

/*
  -- vtago 10/19/15
  -- this will not work anymore because with the new CCB interface, we could have records with current and future revenue months

  SELECT to_number(DECODE(rev_month, 'O','10',
                                     'N','11',
                                     'D','12',
                                          rev_month))
  INTO v_month
  FROM EXT_CCB_METER_LOAD
  WHERE rownum = 1;


  -- -4 value is for temporary testing, it needs to be replaced with -2/v_date_val after testing

   IF v_month  <> to_number(TO_CHAR(runDate,'mm')) THEN
    v_tab := 'EXT_CCB_METER_LOAD' ;
    RAISE invalid_ccb_data;
  END IF;
*/

  -- vtago 10/19/15
  -- new validation
  -- count number of records with rev_month = runDate
  -- if number < 5 million, raise error
  -- i set an arbitrary number of 5 million here since there are roughly 5.5 million meters
  select count(*) into v_count from EXT_CCB_METER_LOAD where REV_MONTH = to_number(TO_CHAR(runDate,'mm'));
  IF v_count < 5000000 THEN
    v_tab := 'EXT_CCB_METER_LOAD' ;
    RAISE invalid_ccb_data;
  END IF;
  -- END new validation

  --  Copy  EXT_ data to  STG_ tables
  execute immediate 'truncate table STG_CCB_METER_LOAD';
  execute immediate 'truncate table STG_SM_SP_LOAD';
  execute immediate 'truncate table STG_SM_TRF_LOAD';
  execute immediate 'truncate table STG_SM_SP_GEN_LOAD';
  execute immediate 'truncate table STG_SM_TRF_GEN_LOAD';

  INSERT
  INTO STG_CCB_METER_LOAD
    (
      SERVICE_POINT_ID,
      UNQSPID,
      ACCT_ID,
      REV_MONTH,
      REV_KWHR,
      REV_KW,
      PFACTOR,
      SM_SP_STATUS
    )
  SELECT SERVICE_POINT_ID,
    UNQSPID,
    ACCT_ID,
    REV_MONTH,
    -- vtago 10/19/15
    -- CCB interface will now send us exact month number
    -- CASE REV_MONTH
    -- WHEN 'O' THEN '10'
    -- WHEN 'N' THEN '11'
    -- WHEN 'D' THEN '12'
    -- ELSE REV_MONTH
    -- END,
    cast(REV_KWHR as number)/10,
    REV_KW,
    PFACTOR,
    SM_SP_STATUS
  FROM EXT_CCB_METER_LOAD
  -- vtago 10/19/15
  -- added where condition
  WHERE
    REV_MONTH = to_number(TO_CHAR(runDate,'mm'));


  --  Copy CDW EXT data from two months ago to STG tables




  INSERT INTO STG_SM_TRF_LOAD
    ( CGC, TRF_PEAK_KW, TRF_PEAK_TIME, TRF_AVG_KW
    )
  SELECT CGC,
    TRF_PEAK_KW,
    -- when smart meter measures peak at midnight, it sets the date correctly as next day
    -- however, it includes the record as last month's data so we need to subtract
    -- ONE second to it so it shows up as current month's peak
    decode(TRF_PEAK_TIME, TO_CHAR(add_months(runDate,1),'YYYYMMDD:HH24:MI'),
        to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI') - interval '1' second,
        to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI')),
    TRF_AVG_KW
  FROM EXT_SM_TRF_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_TRF_LOAD);


  INSERT
  INTO STG_SM_SP_LOAD
    (
      CGC,
      SERVICE_POINT_ID,
      SP_PEAK_KW,
      VEE_SP_KW_FLAG,
      SP_PEAK_TIME,
      SP_KW_TRF_PEAK,
      VEE_TRF_KW_FLAG,
      INT_LEN,
      SP_PEAK_KVAR,
      TRF_PEAK_KVAR
    )
  SELECT
    a.CGC,
    a.SERVICE_POINT_ID,
    a.SP_PEAK_KW,
    a.VEE_SP_KW_FLAG,
    -- when smart meter measures peak at midnight, it sets the date correctly as next day
    -- however, it includes the record as last month's data so we need to subtract
    -- ONE second to it so it shows up as current month's peak
    decode(SP_PEAK_TIME, TO_CHAR(add_months(runDate,1),'YYYYMMDD:HH24:MI'),
        to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI') - interval '1' second,
        to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI')),
    a.SP_KW_TRF_PEAK,
    a.VEE_TRF_KW_FLAG,
    a.INT_LEN,
    a.SP_PEAK_KVAR,
    a.TRF_PEAK_KVAR
  FROM EXT_SM_SP_LOAD a, TRANSFORMER b
  WHERE
    a.cgc = b.CGC_ID and
    to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_SP_LOAD)
    and (b.REC_STATUS <>'D' or b.REC_STATUS IS NULL);


    INSERT
  INTO STG_SM_TRF_GEN_LOAD
    (
      CGC,
      TRF_PEAK_KW,
      TRF_PEAK_TIME,
      TRF_AVG_KW
    )
  SELECT CGC,
    TRF_PEAK_KW,
    -- when smart meter measures peak at midnight, it sets the date correctly as next day
    -- however, it includes the record as last month's data so we need to subtract
    -- ONE second to it so it shows up as current month's peak
    decode(TRF_PEAK_TIME, TO_CHAR(add_months(runDate,1),'YYYYMMDD:HH24:MI'),
        to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI') - interval '1' second,
        to_date(TRF_PEAK_TIME,'YYYYMMDD:HH24:MI')),
    TRF_AVG_KW
  FROM EXT_SM_TRF_GEN_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_TRF_GEN_LOAD);


  INSERT
  INTO STG_SM_SP_GEN_LOAD
    (
      CGC,
      SERVICE_POINT_ID,
      SP_PEAK_KW,
      VEE_SP_KW_FLAG,
      SP_PEAK_TIME,
      SP_KW_TRF_PEAK,
      VEE_TRF_KW_FLAG,
      INT_LEN,
      SP_PEAK_KVAR,
      TRF_PEAK_KVAR
    )
  SELECT
    a.CGC,
    a.SERVICE_POINT_ID,
    a.SP_PEAK_KW,
    a.VEE_SP_KW_FLAG,
    -- when smart meter measures peak at midnight, it sets the date correctly as next day
    -- however, it includes the record as last month's data so we need to subtract
    -- ONE second to it so it shows up as current month's peak
    decode(SP_PEAK_TIME, TO_CHAR(add_months(runDate,1),'YYYYMMDD:HH24:MI'),
        to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI') - interval '1' second,
        to_date(SP_PEAK_TIME,'YYYYMMDD:HH24:MI')),
    a.SP_KW_TRF_PEAK,
    a.VEE_TRF_KW_FLAG,
    a.INT_LEN,
    a.SP_PEAK_KVAR,
    a.TRF_PEAK_KVAR
  FROM
    EXT_SM_SP_GEN_LOAD a, TRANSFORMER b
  WHERE
    a.cgc = b.CGC_ID and
    to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_SP_GEN_LOAD)
    and (b.REC_STATUS <>'D' or b.REC_STATUS IS NULL);

  -- vtago 10/19/15
  -- we cannot truncate anymore
  -- execute immediate 'truncate table EXT_CCB_METER_LOAD';
  DELETE
  FROM EXT_CCB_METER_LOAD
  WHERE
    REV_MONTH = to_number(TO_CHAR(runDate,'mm'));

  DELETE
  FROM EXT_SM_SP_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_SP_LOAD);
  DELETE
  FROM EXT_SM_TRF_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_TRF_LOAD);
  DELETE
  FROM EXT_SM_SP_GEN_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_SP_GEN_LOAD);
  DELETE
  FROM EXT_SM_TRF_GEN_LOAD
  WHERE to_date(CREATE_DATE, 'mmddyy') = (select min(to_date(CREATE_DATE, 'mmddyy'))from EXT_SM_TRF_GEN_LOAD);

  COMMIT;
EXCEPTION
WHEN invalid_cdw_data THEN
  RAISE_APPLICATION_ERROR(-20001,' Data loaded into this CDW table is not letest :'||v_tab||':'||runDate);
WHEN invalid_ccb_data THEN
  RAISE_APPLICATION_ERROR(-20001,'Data loaded into this CCB table is not letest :'||v_tab||':'||runDate);
END DATA_LOAD_VALIDATION;
/
