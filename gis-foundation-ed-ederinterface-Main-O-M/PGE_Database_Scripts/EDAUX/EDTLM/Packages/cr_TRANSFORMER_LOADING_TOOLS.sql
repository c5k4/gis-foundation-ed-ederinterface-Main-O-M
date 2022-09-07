CREATE OR REPLACE 
PACKAGE edtlm.transformer_loading_tools
AS
-- a variable to save error messages in functions for calling procedures to use
e_saved_message      VARCHAR2(2000);

   FUNCTION Calc_customer_kva(
      i_trf_id      NUMBER,
      i_trf_hist_id NUMBER,
      i_batch_date  DATE,
      i_season      VARCHAR2,
      i_direction   VARCHAR2)
   RETURN INTEGER;

   FUNCTION Calc_Trf_bank_data(
      i_trf_id          NUMBER,
      i_trf_hist_id     NUMBER,
      i_trf_gen_hist_id NUMBER,
      i_batch_date      DATE,
      i_season          VARCHAR2)
   RETURN INTEGER;

   FUNCTION Calc_yearly_lf(
      i_trf_id      NUMBER,
      i_batch_date  DATE,
      i_direction   VARCHAR2)
   RETURN INTEGER;

   FUNCTION Check_for_indeterminate_status(
      i_trf_id      NUMBER,
      i_batch_date  DATE)
   RETURN VARCHAR2;

   FUNCTION Establish_Seasonal_Peak(
      i_trf_id      NUMBER,
      i_season      VARCHAR2,
      i_batch_date  DATE)
   RETURN INTEGER;

   FUNCTION Get_Trf_Connection_Type(
      i_trf_id      NUMBER)
   RETURN VARCHAR2;

   FUNCTION Ins_Trf_Bank_Hist(
      i_direction        VARCHAR2,
      i_hist_id          NUMBER,
      i_trf_bank_id      NUMBER,
      i_np_kva           NUMBER,
      i_cap_np_kva       NUMBER,
      i_trf_peak_kva     NUMBER,
      i_trf_grp_typ      VARCHAR2,
      i_season           VARCHAR2,
      i_coast_int_ind    NUMBER,
      i_vault_ind        VARCHAR2,
      i_yearly_lf        NUMBER)
   RETURN INTEGER;

   FUNCTION Set_Peak(
      i_trf_id      NUMBER,
      i_season      VARCHAR2,
      i_trf_peak    DATE)
   RETURN INTEGER;

   FUNCTION Set_Peak_Gen(
      i_trf_id      NUMBER,
      i_season      VARCHAR2,
      i_trf_peak    DATE)
   RETURN INTEGER;

   FUNCTION Update_trf_peak_hist(
      i_trf_id      NUMBER,
      i_trf_hist_id NUMBER,
      i_batch_date  DATE)
   RETURN INTEGER;

   FUNCTION Update_trf_peak_gen_hist(
      i_trf_id      NUMBER,
      i_hist_id     NUMBER,
      i_batch_date  DATE)
   RETURN INTEGER;

  PROCEDURE Maintain_tables;

  PROCEDURE Monthly_run (
      i_batch_date DATE);

  PROCEDURE Monthly_TLM_run (
      i_variant    VARCHAR2,
      i_batch_date DATE,
      i_thread     VARCHAR2);

  PROCEDURE Monthly_run_3 (
      i_batch_date DATE,
      i_thread     varchar2);

  PROCEDURE Populate_CCB_data (
      i_refresh_ext_ml_tbl VARCHAR2,
      i_batch_date         DATE);

  PROCEDURE Populate_CDW_data (
      i_batch_date DATE);

 /* given a trf_id and a batch month this procedure will recalculate the loading */
   PROCEDURE Recalc_trf_loading(
      i_trf_id number,
      i_batch_date DATE,
      i_season VARCHAR2,
      o_ErrorMsg OUT VARCHAR2,
      o_ErrorCode OUT VARCHAR2 );

   PROCEDURE Update_trf_peak_undetermined (
      i_trf_id      NUMBER,
      i_batch_date  DATE,
      i_season      VARCHAR2,
      i_direction   VARCHAR2,
      i_value       VARCHAR2);


END Transformer_loading_Tools;
/

GRANT EXECUTE ON edtlm.transformer_loading_tools TO tjj4
/

CREATE OR REPLACE 
PACKAGE BODY edtlm.transformer_loading_tools
AS
-- need to update the gather stats schema when this moves to edtlm...
-- need alter table statements for meter and meter_del_dup for svc_phase
-- need alter table statements for sm_peak_hist and sm_peak_gen_hist to add phase
-- added records to multiplier table for duplex trfs
-- added code_lookup row for undetermined 6
-- added sp_peak_gen_hist_fk idx for performance
-- added trf_bank_peak_gen_hist_index1 for performance
/*
ALTER TABLE SP_PEAK_HIST  ADD (phase VARCHAR2 (1));
ALTER TABLE SP_PEAK_GEN_HIST  ADD (phase VARCHAR2 (1));

INSERT INTO code_lookup (code_typ, code, desc_long, desc_short)
VALUES ('UNDETERMINED','6','No Capability record found','No Capability record found');
INSERT INTO code_lookup (code_typ, code, desc_long, desc_short)
VALUES ('UNDETERMINED','7','No Trf Group record found','No Trf Group record found');
INSERT INTO code_lookup (code_typ, code, desc_long, desc_short)
VALUES ('UNDETERMINED','8','No Load Curve found','No Load Curve found');
INSERT INTO code_lookup (code_typ, code, desc_long, desc_short)
VALUES ('UNDETERMINED','9','Transformer Type is null','Transformer Type is null');
UPDATE code_lookup SET desc_long = 'Duplex unit issue', desc_short = 'Duplex unit issue'
WHERE code_typ = 'UNDETERMINED' AND code = '2';
UPDATE code_lookup SET desc_long = 'Invalid number of units (1-3 is valid)', desc_short = 'Invalid number of units (1-3 is valid)'
WHERE code_typ = 'UNDETERMINED' AND code = '1';

CREATE TABLE TLM_errors
   (Rec_id                    NUMBER,
    REC_type                  VARCHAR2(20),
    Batch_date                DATE,
    error_code                VARCHAR2(20),
    error_msg                 VARCHAR2(200),
    error_date                DATE)
;

*/
FUNCTION Calc_customer_kva(i_trf_id NUMBER, i_trf_hist_id NUMBER, i_batch_date DATE,
         i_season VARCHAR2, i_direction VARCHAR2)
RETURN INTEGER
IS
/*
 Purpose:  Calculate the kva, and populate the sp_peak_hist table for the meters
           on this transformer.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        06/27/19    Initial coding
   TJJ4        07/26/19    added i_tbl to reuse the procedure for Generation
                           if i_direction is RECEIVED then calculate kva for
                           the generation tables otherwise put kva in the
                           standard tlm tables
   TJJ4        08/30/19    converted to SYS_REFCURSOR for performance test
   TJJ4        09/10/19    Populate CCB Data is doing the /10 so remove it from
                           here for the ccb/legacy calculations
   TJJ4        01/23/19    remove customers that have neither a sm nor a ccb record
                           for the given month

*/

CURSOR get_customers_v1
IS
   SELECT m.id, m.service_point_id, m.cust_typ, m.svc_phase,
          decode(sp.service_point_id, NULL, 'L','S') sm_flg,
          sp.sp_peak_kw, sp.vee_sp_kw_flag, sp.sp_peak_time, sp.sp_kw_trf_peak,
          sp.vee_trf_kw_flag, sp.int_len, sp.sp_peak_kvar, sp.trf_peak_kvar,
          to_number(ml.rev_kwhr), to_number(ml.rev_kw), to_number(ml.pfactor)
     FROM meter m, historian.sm_sp_load_hist sp, historian.ccb_meter_load_hist ml
    WHERE trf_id = i_trf_id
      AND sp.batch_date (+) = i_batch_date
      AND nvl(m.rec_status, 'i') <> 'D'
      AND m.service_point_id = sp.service_point_id (+)
      AND m.service_point_id = ml.service_point_id (+)
      AND ml.batch_date (+) = i_batch_date
      AND i_direction = 'DELIVERED'
   UNION all
   SELECT m.id, m.service_point_id, m.cust_typ, m.svc_phase,
          decode(sp.service_point_id, NULL, 'L','S') sm_flg,
          sp.sp_peak_kw, sp.vee_sp_kw_flag, sp.sp_peak_time, sp.sp_kw_trf_peak,
          sp.vee_trf_kw_flag, sp.int_len, sp.sp_peak_kvar, sp.trf_peak_kvar,
          NULL, NULL, NULL
     FROM meter m, historian.sm_sp_gen_load_hist sp
    WHERE trf_id = i_trf_id
      AND sp.batch_date (+) = i_batch_date
      AND nvl(m.rec_status, 'i') <> 'D'
      AND m.service_point_id = sp.service_point_id
      AND i_direction = 'RECEIVED' ;

   v_routine   VARCHAR2(64) := 'TLT.Calc_cust_kva';
   v_hint      INTEGER := 0;

   get_customers        sys_refcursor;

   v_meter_id           sp_peak_hist.meter_id%TYPE;
   v_service_point_id   sp_peak_hist.service_point_id%TYPE;
   v_phase              sp_peak_hist.phase%TYPE;
   v_sp_peak_time       sp_peak_hist.sp_peak_time%TYPE;
   v_sp_peak_kva        sp_peak_hist.sp_peak_kva%TYPE;
   v_sp_kva_trf_peak    sp_peak_hist.sp_kva_trf_peak%TYPE;
   v_vee_sp_kw_flg      sp_peak_hist.vee_sp_kw_flg%TYPE;
   v_int_len            sp_peak_hist.int_len%TYPE;
   v_cust_typ           sp_peak_hist.cust_typ%TYPE;
   v_vee_trf_kw_flg     sp_peak_hist.vee_trf_kw_flg%TYPE;
   v_sm_flg             sp_peak_hist.sm_flg%TYPE;
   v_sp_peak_kw         sp_peak_hist.sp_peak_kw%TYPE;
   v_sp_kw_trf_peak     sp_peak_hist.sp_kw_trf_peak%TYPE;
   v_sp_peak_kvar       NUMBER;  -- this doesn't go in the table...
   v_trf_peak_kvar      NUMBER;  -- ibid... this one is used in the old calcs for determining kva....
   v_rev_kwhr           NUMBER;
   v_rev_kw             sp_peak_hist.sp_peak_kw%TYPE;
   v_pfactor            NUMBER;
   v_seasonal_pf        NUMBER := .85;  -- summer seasonal powerfactor
   v_dom_custs          NUMBER := -1;
   v_count              NUMBER;
   v_load_curve_a_val   NUMBER;
   v_load_curve_b_val   NUMBER;
   v_guid               transformer.global_id%TYPE;

BEGIN

IF i_season = 'W' THEN
   v_seasonal_pf := .95;  -- winter seasonal powerfactor
END IF;

IF i_direction NOT IN ('DELIVERED', 'RECEIVED') THEN
   transformer_loading_tools.e_saved_message := v_routine || ' invalid i_direction: '|| i_direction;
   RETURN -1;
END IF;
IF i_direction = 'DELIVERED' THEN
   OPEN get_customers FOR
      SELECT m.id, m.service_point_id, m.cust_typ, m.svc_phase,
             decode(sp.service_point_id, NULL, 'L','S') sm_flg,
             sp.sp_peak_kw, sp.vee_sp_kw_flag, sp.sp_peak_time, sp.sp_kw_trf_peak,
             sp.vee_trf_kw_flag, sp.int_len, sp.sp_peak_kvar, sp.trf_peak_kvar,
             to_number(ml.rev_kwhr), to_number(ml.rev_kw), to_number(ml.pfactor)
        FROM meter m, historian.sm_sp_load_hist sp, historian.ccb_meter_load_hist ml
       WHERE trf_id = i_trf_id
         AND sp.batch_date (+) = i_batch_date
         AND nvl(m.rec_status, 'i') <> 'D'
         AND m.service_point_id = sp.service_point_id (+)
         AND m.service_point_id = ml.service_point_id (+)
         AND ml.batch_date (+) = i_batch_date
         AND nvl(sp.service_point_id, ml.service_point_id) IS NOT null;
ELSE
   OPEN get_customers FOR
      SELECT m.id, m.service_point_id, m.cust_typ, m.svc_phase,
             decode(sp.service_point_id, NULL, 'L','S') sm_flg,
             sp.sp_peak_kw, sp.vee_sp_kw_flag, sp.sp_peak_time, sp.sp_kw_trf_peak,
             sp.vee_trf_kw_flag, sp.int_len, sp.sp_peak_kvar, sp.trf_peak_kvar,
             NULL, NULL, NULL
        FROM meter m, historian.sm_sp_gen_load_hist sp
       WHERE trf_id = i_trf_id
         AND sp.batch_date (+) = i_batch_date
         AND nvl(m.rec_status, 'i') <> 'D'
         AND m.service_point_id = sp.service_point_id;
END IF;
--OPEN get_customers;
LOOP
   FETCH get_customers INTO
      v_meter_id, v_service_point_id, v_cust_typ, v_phase, v_sm_flg, v_sp_peak_kw, v_vee_sp_kw_flg,
      v_sp_peak_time, v_sp_kw_trf_peak, v_vee_trf_kw_flg, v_int_len, v_sp_peak_kvar,
      v_trf_peak_kvar, v_rev_kwhr, v_rev_kw, v_pfactor;
   EXIT WHEN get_customers%NOTFOUND;
   IF v_sm_flg = 'S' THEN
      IF v_trf_peak_kvar IS NOT NULL THEN
         v_sp_peak_kva := round(SQRT(POWER(v_sp_peak_kw,2) + POWER(v_sp_peak_kvar,2)),1);
         v_sp_kva_trf_peak := round(SQRT(POWER(v_sp_kw_trf_peak,2) + POWER(v_trf_peak_kvar,2)),1);
      ELSIF v_cust_typ IN ('AGR','IND','OTH') THEN
         v_sp_peak_kva := v_sp_peak_kw/.85;
         v_sp_kva_trf_peak := v_sp_kw_trf_peak/.85;
      ELSE
         v_sp_peak_kva := v_sp_peak_kw/v_seasonal_pf;
         v_sp_kva_trf_peak := v_sp_kw_trf_peak/v_seasonal_pf;
      END IF;
   ELSE
      IF v_cust_typ <> 'DOM' THEN
         IF v_rev_kw > 0 AND v_pfactor > 0 THEN
            v_sp_peak_kva := v_rev_kw/(v_pfactor/100);
         ELSIF v_rev_kw > 0 THEN
            IF v_cust_typ IN ('AGR','IND','OTH') THEN
               v_sp_peak_kva := v_rev_kw/.85;
            ELSE -- 'com'
               v_sp_peak_kva := v_rev_kw/v_seasonal_pf;
            END IF;
         ELSE
            IF v_cust_typ IN ('AGR','IND','OTH') THEN
               v_sp_peak_kva := v_rev_kwhr*(1.0/(24*.85*.48));
            ELSE -- 'com'
               v_sp_peak_kva := v_rev_kwhr*(1.0/(24*v_seasonal_pf*.48));
            END IF;
         END IF;
         v_sp_kva_trf_peak := v_sp_peak_kva;
      ELSE
         -- the load curves produce a non-zero kva even when the kwhr is 0
         -- suppress this
         IF v_rev_kwhr IN (0, NULL) THEN
            v_sp_peak_kva := 0;
         ELSE
            IF v_dom_custs = -1 THEN -- we need to initialze the variables
               -- determine how many dom custs are on the trf (sm and legacy both)
               SELECT COUNT(*)
                 INTO v_dom_custs
                 FROM meter
                WHERE trf_id = i_trf_id
                  AND cust_typ = 'DOM'
                  AND nvl(rec_status, 'i') <> 'D';

               -- get the a/b values for this number of custs in this climate zone
               -- average the b value a constant per customer
               SELECT COUNT(*), MIN(K.A_value), MIN(K.B_value)/v_dom_custs, MIN(global_id)
                 INTO v_count, v_load_curve_a_val, v_load_curve_b_val, v_guid
                 FROM transformer t, KVA_conversion_coefficients K
                WHERE t.id = i_trf_id
                  AND nvl(t.rec_status, 'i') <> 'D'
                  AND K.Climate_zone = T.Climate_zone_cd
                  AND K.Season = i_season
                  AND k.customer_count_gt =
                     DECODE(v_dom_custs, 1, 0, 2, 1, 3, 2, 4, 2, 5, 2,
                        DECODE(TRUNC(v_dom_custs/13), 0, 5,
                           DECODE(TRUNC(v_dom_custs/21), 0, 12,
                                                20)));
               IF v_count <> 1 THEN -- there's a problem with the data, probably climate_zone
                  UPDATE trf_peak_hist SET load_undetermined = 8
                   WHERE id = i_trf_hist_id;
                  Update_trf_peak_undetermined (i_trf_id, i_batch_date, i_season, i_direction, 8);
                  CLOSE get_customers;
                  transformer_loading_tools.e_saved_message := v_routine || ' unable to determine load curve error';
                  RETURN 1;
               END IF;
            END IF;
            v_sp_peak_kva := (v_rev_kwhr*v_load_curve_a_val + v_load_curve_b_val)/v_seasonal_pf;
         END IF;
      END IF;
      v_sp_kva_trf_peak := v_sp_peak_kva;
      v_sp_peak_time := trunc(i_batch_date);  -- set as the beginning of the month
      v_sp_peak_kw := v_rev_kw;  -- move it so it inserts below
   END IF;
   -- format it
   v_sp_peak_kva := round(nvl(v_sp_peak_kva,0), 1);
   v_sp_kva_trf_peak := round(nvl(v_sp_kva_trf_peak,0), 1);

   -- store it
   IF i_direction = 'DELIVERED' THEN
v_hint := 20;
      INSERT INTO sp_peak_hist
             (service_point_id,   trf_peak_hist_id, sp_peak_time,    sp_peak_kva,
              sp_kva_trf_peak,    vee_sp_kw_flg,    int_len,         cust_typ,
              vee_trf_kw_flg,     sm_flg,           meter_id,        sp_peak_kw,
              sp_kw_trf_peak,     phase)
      VALUES (v_service_point_id, i_trf_hist_id,    v_sp_peak_time,  v_sp_peak_kva,
              v_sp_kva_trf_peak,  v_vee_sp_kw_flg,  v_int_len,       v_cust_typ,
              v_vee_sp_kw_flg,    v_sm_flg,         v_meter_id,      v_sp_peak_kw,
              v_sp_kw_trf_peak,   v_phase);
   ELSIF i_direction = 'RECEIVED' THEN
v_hint := 30;
      INSERT INTO sp_peak_gen_hist
             (service_point_id,   trf_peak_gen_hist_id, sp_peak_time,    sp_peak_kva,
              sp_kva_trf_peak,    vee_sp_kw_flg,        int_len,         cust_typ,
              vee_trf_kw_flg,     sm_flg,               meter_id,        sp_peak_kw,
              sp_kw_trf_peak,     phase)
      VALUES (v_service_point_id, i_trf_hist_id,        v_sp_peak_time,  v_sp_peak_kva,
              v_sp_kva_trf_peak,  v_vee_sp_kw_flg,      v_int_len,       v_cust_typ,
              v_vee_sp_kw_flg,    v_sm_flg,             v_meter_id,      v_sp_peak_kw,
              v_sp_kw_trf_peak,   v_phase);
   ELSE -- throw an error...
      NULL;  -- the code never gets here because the cursor only selects data for valid i_directions
   END IF;
END LOOP;
CLOSE get_customers;

RETURN 0;

EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END Calc_customer_kva;

FUNCTION Calc_Trf_bank_data(i_trf_id NUMBER, i_trf_hist_id NUMBER,
         i_trf_gen_hist_id NUMBER, i_batch_date DATE, i_season VARCHAR2)
RETURN INTEGER
IS
/*
 Purpose:  Calculate the trf bank data, and populate the trf_bank_peak_hist table,
           if i_trf_gen_hist_id is not null, calculate and populate
           trf_bank_peak_gen_hist as well

           np_kva - the rated kva on the bank at the time of loading
           trf_cap - the kva the transformer bank is capable of serving

     Note: the capability multiplier is selected from a table based on
           Season - summer, winter
           installation type - Overhead, SubSurface, etc
           Coastal vs Interior location
           Vault indicator
           Rated KVA of the bank
           The Transformer Group Type, the trf_types are placed into groups
               so the numbers of multipliers remains small
           An A multiplier is used when the yearly load factor is < 40%
              B multiplier is used when the yearly load factor is >= 40%
               (yeah I don't know why C isn't used either, but... that's what the engineer said)
              as of now 7/11/19 a yearly load factor for any trf that serves a Legacy meter
              is uncalculatable, so assign the B multipliers, they're smaller values making the
              transformer bank less capable of serving load and thus more likely to appear
              as overloaded.  This is a safer option and a worst case scenario for these.
              Calc_yearly_lf populates the yearly_lf value with Null for these

           trf_peak_kva - the portion of the transformer peak kva this unit is supporting

     Note: for multi-bank transformers this gets a bit complicated but basically
           it is simply allocating a percentage of the load to each bank based on
           how many units there are, how the transformer is connected to the system,
           and the service phase. single phase service vs three phase service



   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/11/19     Initial coding
   TJJ4        7/24/19     set 0 np_kva's to .1 for Delta/Delta connection types to
                           result in 66/33 split of single phase load
   TJJ4        7/30/19     added ability to calculate bank_hist for generation as well
                           vault indicator for Duplex trf in capabilities is always 0

*/
CURSOR get_banks
IS
   SELECT b.id, b.np_kva, tgt.trf_grp_type
     FROM transformer_bank b, trf_group_type tgt
    WHERE trf_id = i_trf_id
      AND b.trf_typ = tgt.trf_type
      AND nvl(b.rec_status,'i') <> 'D'
     ORDER BY np_kva DESC;  -- for multi bank trf's the biggest np_kva is the
                            -- Lighter unit, the rest are Power units
                            -- yeah, it makes no sense but Lighter = biggest, Power = smaller

   v_routine   VARCHAR2(64) := 'TLT.Calc_trf_bank_data';
   v_hint      INTEGER := 0;

   v_bank_count       NUMBER;
   v_count            NUMBER;
   v_coast_int_ind    transformer.coast_interior_flg%TYPE;
   v_vault_ind        trf_np_cap_mult.vault_ind%TYPE;
   v_yearly_lf        trf_peak_hist.yearly_lf%TYPE;
   v_trf_peak_kva     trf_peak_hist.trf_peak_kva%TYPE;
   v_calc_trf_kva     trf_peak_hist.trf_peak_kva%TYPE;
   v_trf_kva_non_dom  trf_peak_hist.trf_peak_kva_dom%TYPE;
   v_trf_peak_kva_dom trf_peak_hist.trf_peak_kva_dom%TYPE;

   v_gen_yearly_lf        trf_peak_gen_hist.yearly_lf%TYPE;
   v_gen_trf_peak_kva     trf_peak_gen_hist.trf_peak_kva%TYPE;
   v_gen_calc_trf_kva     trf_peak_gen_hist.trf_peak_kva%TYPE;
   v_gen_trf_kva_non_dom  trf_peak_gen_hist.trf_peak_kva_dom%TYPE;
   v_gen_trf_peak_kva_dom trf_peak_gen_hist.trf_peak_kva_dom%TYPE;

   v_trf_bank_id      transformer_bank.id%TYPE;
   v_np_kva           transformer_bank.np_kva%TYPE;
   v_Power_Mult_kva   transformer_bank.np_kva%TYPE;
   v_trf_grp_type     trf_group_type.trf_grp_type%TYPE;
   v_connection_type  VARCHAR2(2);
   v_L_kva            transformer_bank.np_kva%TYPE;
   v_P_kva            transformer_bank.np_kva%TYPE;
   v_L_pct            NUMBER;
   v_P_pct            NUMBER;
   v_3P_pct           NUMBER;
   v_status           NUMBER;

BEGIN
   SELECT coast_interior_flg, decode(vault, NULL, '0', '1')
     INTO v_coast_int_ind, v_vault_ind
     FROM transformer
    WHERE id = i_trf_id;

   SELECT nvl(yearly_lf, 40), trf_peak_kva, trf_peak_kva_dom
     INTO v_yearly_lf, v_trf_peak_kva, v_trf_peak_kva_dom
     FROM trf_peak_hist
    WHERE id = i_trf_hist_id;

   IF i_trf_gen_hist_id IS NOT NULL THEN
      SELECT nvl(yearly_lf, 40), trf_peak_kva, trf_peak_kva_dom
        INTO v_gen_yearly_lf, v_gen_trf_peak_kva, v_gen_trf_peak_kva_dom
        FROM trf_peak_gen_hist
       WHERE id = i_trf_gen_hist_id;
   END IF;

   v_hint := 10;
   SELECT COUNT(*), max(tgt.trf_grp_type)
     INTO v_bank_count, v_trf_grp_type
     FROM transformer_bank b, trf_group_type tgt
    WHERE trf_id = i_trf_id
      AND nvl(rec_status, 'i') <> 'D'
      AND b.trf_typ = tgt.trf_type;   -- specifically exclude those that aren't going to be found in the group table

   IF v_bank_count = 1 THEN -- the VERY easy case (808000 trfs though)
      v_hint := 20;
      OPEN get_banks;
         FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
            -- if the grp type indicates it's a duplex type, here's where
            -- we'd branch and create a second bank record, ugly, ugly, ugly
            IF v_trf_grp_type IN ('SS-DP', 'PM-DP') THEN  -- duplex trf
               SELECT COUNT(*), MIN(lighter), MIN(power)
                 INTO v_count, v_L_kva, v_P_kva
                 FROM trf_duplex_lookup
                WHERE total = v_np_kva;
               IF v_count = 1 THEN -- it's a valid kva
                  SELECT light_single_phase_pct/100, power_single_phase_pct/100, power_three_phase_pct/100
                    INTO v_L_pct, v_P_pct, v_3P_pct
                    FROM trf_load_dist_flat
                   WHERE connection_typ = 'ODOD';
                  v_vault_ind := '0';
                  v_trf_kva_non_dom := v_trf_peak_kva - v_trf_peak_kva_dom;
                  v_calc_trf_kva := v_trf_peak_kva_dom * v_L_pct + v_trf_kva_non_dom * v_3P_pct;
                  v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_L_kva, v_np_kva,
                                                v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
                  IF i_trf_gen_hist_id IS NOT NULL THEN
                     v_gen_trf_kva_non_dom := v_gen_trf_peak_kva - v_gen_trf_peak_kva_dom;
                     v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_L_pct + v_gen_trf_kva_non_dom * v_3P_pct;
                     v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_L_kva, v_np_kva,
                                                   v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                   v_vault_ind, v_gen_yearly_lf) ;
                     IF v_status <> 0 THEN GOTO message_handler;
                     END IF;
                  END IF;
                  -- Now the Power
                  v_calc_trf_kva := v_trf_peak_kva_dom * v_P_pct + v_trf_kva_non_dom * v_3P_pct;
                  v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_P_kva, v_np_kva,
                                                v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
                  IF i_trf_gen_hist_id IS NOT NULL THEN
                     v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_P_pct + v_gen_trf_kva_non_dom * v_3P_pct;
                     v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_P_kva, v_np_kva,
                                                   v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                   v_vault_ind, v_gen_yearly_lf) ;
                     IF v_status <> 0 THEN GOTO message_handler;
                     END IF;
                  END IF;
               ELSE
                  transformer_loading_tools.e_saved_message := v_routine || ' invalid np_kva for duplex trf.  np_kva: '|| v_np_kva;
                  v_status := -1;
                  GOTO message_handler;
               END IF;
            ELSE
               v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                             v_trf_peak_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
               IF i_trf_gen_hist_id IS NOT NULL THEN
                  v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                                v_gen_trf_peak_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_gen_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
               END IF;
            END IF;
   ELSIF v_bank_count = 2 THEN -- it's time to portion out the load
      v_hint := 50;
      SELECT light_single_phase_pct/100, power_single_phase_pct/100, power_three_phase_pct/100
        INTO v_L_pct, v_P_pct, v_3P_pct
        FROM trf_load_dist_flat
       WHERE connection_typ = 'ODOD';
      OPEN get_banks;
         FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
            IF v_trf_grp_type IN ('SS-DP', 'PM-DP') THEN  -- duplex trf - they're special ;')
               v_Power_Mult_kva := v_np_kva;
                  -- the capability multipliers for duplex trfs are based on the Lighter unit
                  -- the 2 10kva power units have different multipliers because of this
               v_vault_ind := '0';
            END IF;
            -- they're sorted in get_banks, the Lighter is first
            v_trf_kva_non_dom := v_trf_peak_kva - v_trf_peak_kva_dom;
            v_calc_trf_kva := v_trf_peak_kva_dom * v_L_pct + v_trf_kva_non_dom * v_3P_pct;
            v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                          v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                          v_vault_ind, v_yearly_lf) ;
            IF v_status <> 0 THEN GOTO message_handler;
            END IF;
            IF i_trf_gen_hist_id IS NOT NULL THEN
               v_gen_trf_kva_non_dom := v_gen_trf_peak_kva - v_gen_trf_peak_kva_dom;
               v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_L_pct + v_gen_trf_kva_non_dom * v_3P_pct;
               v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                             v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_gen_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
            END IF;
         -- now fetch the power unit
         FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
            IF v_trf_grp_type NOT IN ('SS-DP', 'PM-DP') THEN
               v_Power_Mult_kva := v_np_kva;
            END IF;
            v_calc_trf_kva := v_trf_peak_kva_dom * v_P_pct + v_trf_kva_non_dom * v_3P_pct;
            v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_Power_Mult_kva,
                                          v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                          v_vault_ind, v_yearly_lf) ;
            IF v_status <> 0 THEN GOTO message_handler;
            END IF;
            IF i_trf_gen_hist_id IS NOT NULL THEN
               v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_P_pct + v_gen_trf_kva_non_dom * v_3P_pct;
               v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_Power_Mult_kva,
                                             v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_gen_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
            END IF;
   ELSIF v_bank_count = 3 THEN -- portion out the load further
      v_hint := 80;
      v_trf_kva_non_dom := v_trf_peak_kva - v_trf_peak_kva_dom;
      v_gen_trf_kva_non_dom := v_gen_trf_peak_kva - v_gen_trf_peak_kva_dom;  -- no IF saves some steps
      v_connection_type := Get_Trf_Connection_Type(i_trf_id);
      IF v_connection_type = 'EE' THEN -- it threw an error pass it on
         RETURN -1;
      END IF;
      IF v_connection_type = 'DD' THEN -- calculate the dist % for 1-phase
        -- need a minimum value for np_kva, set to .1, if both L and P are .1 this results
        -- in a 66.6/33.3 % split of the single phase load
         SELECT MAX(decode(np_kva, 0, 0.1, np_kva)), MIN(decode(np_kva, 0, 0.1, np_kva))
           INTO v_L_kva, v_P_kva
           FROM transformer_bank b, trf_group_type tgt
          WHERE trf_id = i_trf_id
            AND nvl(rec_status, 'i') <> 'D'
            AND b.trf_typ = tgt.trf_type;
         v_L_pct := (100.0/(1.0+(v_P_kva/(2.0*v_L_kva))))/100;
         v_P_pct := (100.0/(1.0+(2.0*v_L_kva)/v_P_kva))/100;
         v_3P_pct := 0.333;
      ELSIF v_connection_type IN ('YD','YY') THEN    -- select the % from the table
         SELECT light_single_phase_pct/100, power_single_phase_pct/100, power_three_phase_pct/100
           INTO v_L_pct, v_P_pct, v_3P_pct
           FROM trf_load_dist_flat
          WHERE connection_typ = v_connection_type;
      ELSE  -- procedure returned an unknown connection type cry for help
         transformer_loading_tools.e_saved_message := v_routine || ' unknown connection type returned: ' || v_connection_type ;
         RETURN -1;
      END IF;
      IF v_connection_type IN ('DD', 'YD','YY') THEN    -- needs to have a valid connection type
         OPEN get_banks;
            FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
            -- the lighter unit first -- all three phase load is assumed to be balanced...
            --                           if this assumption changes v_3P_pct needs to be adjusted
               v_calc_trf_kva := v_trf_peak_kva_dom * v_L_pct + v_trf_kva_non_dom * v_3P_pct;
               v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                             v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
               IF i_trf_gen_hist_id IS NOT NULL THEN
                  v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_L_pct + v_gen_trf_kva_non_dom * v_3P_pct;
                  v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                                v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_gen_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
               END IF;
            FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
               v_calc_trf_kva := v_trf_peak_kva_dom * v_P_pct + v_trf_kva_non_dom * v_3P_pct;
               v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                             v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
               IF i_trf_gen_hist_id IS NOT NULL THEN
                  v_gen_calc_trf_kva := v_gen_trf_peak_kva_dom * v_P_pct + v_gen_trf_kva_non_dom * v_3P_pct;
                  v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                                v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_gen_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
               END IF;
            FETCH get_banks INTO v_trf_bank_id, v_np_kva, v_trf_grp_type;
               v_status := Ins_Trf_Bank_Hist('DELIVERED', i_trf_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                             v_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                             v_vault_ind, v_yearly_lf) ;
               IF v_status <> 0 THEN GOTO message_handler;
               END IF;
               IF i_trf_gen_hist_id IS NOT NULL THEN
                  v_status := Ins_Trf_Bank_Hist('RECEIVED', i_trf_gen_hist_id, v_trf_bank_id, v_np_kva, v_np_kva,
                                                v_gen_calc_trf_kva, v_trf_grp_type, i_season, v_coast_int_ind,
                                                v_vault_ind, v_gen_yearly_lf) ;
                  IF v_status <> 0 THEN GOTO message_handler;
                  END IF;
               END IF;
      END IF;
   ELSIF v_bank_count = 0 THEN -- there's a problem with the trf_type and/or trf group type
      UPDATE trf_peak_hist SET load_undetermined = 7
       WHERE id = i_trf_hist_id;
      Update_trf_peak_undetermined (i_trf_id, i_batch_date, i_season, 'DELIVERED', 7);
      --??? what about gen???
   END IF;
   IF v_bank_count IN (1,2,3) THEN -- we opened the cursor...
      CLOSE get_banks;
   END IF;  -- it'll be 0 when the group code can't be found...
RETURN 0;
<<MESSAGE_HANDLER>>
   CLOSE get_banks;
   RETURN v_status;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;


FUNCTION Calc_yearly_lf(i_trf_id NUMBER, i_batch_date  DATE, i_direction VARCHAR2)
RETURN INTEGER
IS
/*
 Purpose:  Calculate the yearly load factor for the transformer for the 12-month
           period ending in I_batch_date.

           For now, if there are Legacy meters at any point in the year return
           a null load factor.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        07/11/19    Initial coding
   TJJ4        10/24/19    added check for undetermined flags, any trf that is
                           undetermined in the preceeding 12 months gets a null lf

*/
   v_routine   VARCHAR2(64) := 'TLT.Calc_yearly_lf';
   v_hint      INTEGER := 0;

   v_sm_cust   NUMBER;
   v_ccb_cust  NUMBER;
   v_avg_kw    NUMBER;
   v_peak_kw   NUMBER;
   v_lf        NUMBER;
   v_undet_flg trf_peak_hist.load_undetermined%TYPE;

BEGIN

   IF i_direction = 'DELIVERED' THEN
      SELECT SUM(sm_cust_total), SUM(ccb_cust_total), AVG(trf_ave_kw),
             AVG(trf_peak_kw), max(load_undetermined)
        INTO v_sm_cust, v_ccb_cust, v_avg_kw, v_peak_kw, v_undet_flg
        FROM trf_peak_hist
       WHERE trf_id = i_trf_id
         AND batch_date BETWEEN add_months(i_batch_date,-11) AND i_batch_date;
   ELSIF i_direction = 'RECEIVED' THEN
      SELECT SUM(sm_cust_total), SUM(ccb_cust_total), AVG(trf_ave_kw),
             AVG(trf_peak_kw), max(load_undetermined)
        INTO v_sm_cust, v_ccb_cust, v_avg_kw, v_peak_kw, v_undet_flg
        FROM trf_peak_gen_hist
       WHERE trf_id = i_trf_id
         AND batch_date BETWEEN add_months(i_batch_date,-11) AND i_batch_date;
   ELSE -- throw an error...
      transformer_loading_tools.e_saved_message := v_routine || ' invalid i_direction: '|| i_direction;
      RETURN -1;
   END IF;

   IF ((v_sm_cust = v_ccb_cust) AND (v_undet_flg IS NULL)) THEN -- okay to return lf
      --round(((avg(trf_ave_kw)/avg(trf_peak_kw))*100),0)
      IF v_peak_kw = 0 THEN
         RETURN NULL;
      ELSE
         RETURN (round(((v_avg_kw/v_peak_kw)*100),0));
      END IF;
   ELSE
      RETURN NULL;
   END IF;

EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;

FUNCTION Check_for_indeterminate_status(i_trf_id NUMBER, i_batch_date DATE)
  RETURN VARCHAR2
IS
/*
 Purpose:  Determine if this transformer is in one of the indeterminable states:

           1. there are 0 or more then 3 units associated to the trf
           2. repurposed - the transformer type is duplex (21, 32) and
                           there are more then 2 units or the number of duplex coded
                           units doesn't match the number of units
           3. Invalid np kva on duplex trf
               Valid:  1 unit np_kva must be in (35, 60, 90, 125, 150)
                           (trf_duplex_lookup.total)
                       2 units np_kva must be valid combinations in trf_duplex_lookup
           4. Invalid np kva on Delta-Delta instalation (3 units)
               Valid: 3 units all np_kva's are the same or 2 units are the same
                      and the third is larger
                      100, 50, 50 <-- good  100, 100, 50 <-- fails
           5. Current Transformer/Spid relationship is different from what
              smart meter had for the given batch month
           6. Capability multipliers not found - this is logged in the ins_trf_bank_hist procedure
           7. Transformer Capability Multipliers not available - logged in calc_trf_bank_data
           8. Load Curve not found - logged in Calc_cust_kva
           9. Transformer Type is null



   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/23/19     Initial coding

*/
   v_routine   VARCHAR2(64) := 'TLT.Chk_for_indet_status';
   v_hint      INTEGER := 0;

   v_bank_count       NUMBER;
   v_duplex_count     NUMBER;
   v_null_trf_typ_cnt NUMBER;
   v_max_kva          transformer_bank.np_kva%TYPE;
   v_tot_kva          transformer_bank.np_kva%TYPE;
   v_min_kva          transformer_bank.np_kva%TYPE;
   v_cgc              transformer.cgc_id%TYPE;
   v_count            NUMBER;
   v_connection_type  VARCHAR2(2);

BEGIN

   SELECT COUNT(*), SUM(decode(b.trf_typ, '21',1, '32',1, 0)), MAX(b.np_kva),
          SUM(b.np_kva), MIN(b.np_kva), sum(decode(b.trf_typ, NULL, 1, 0))
     INTO v_bank_count, v_duplex_count, v_max_kva, v_tot_kva, v_min_kva,
          v_null_trf_typ_cnt
     FROM transformer_bank b
    WHERE trf_id = i_trf_id
      AND nvl(rec_status, 'i') <> 'D' ;
   IF v_bank_count NOT IN (1,2,3) THEN
      RETURN '1';  -- wrong number of units
   END IF;
   IF v_duplex_count > 0 THEN
      IF ((v_duplex_count <> v_bank_count) OR (v_bank_count = 3)) THEN
         RETURN '2'; -- also an error
      END IF;
      IF v_bank_count = 1 THEN
         SELECT COUNT(*)
           INTO v_count
           FROM trf_duplex_lookup
          WHERE total = v_max_kva;
      ELSIF v_bank_count = 2 THEN
         SELECT COUNT(*)
           INTO v_count
           FROM trf_duplex_lookup
          WHERE lighter = v_max_kva
            AND power = v_min_kva;
      END IF;
      IF v_count = 0 THEN
         RETURN '3';
      END IF;
   END IF;
   IF v_bank_count = 3 THEN
      v_connection_type := Get_Trf_Connection_Type(i_trf_id);
      IF ((v_connection_type = 'DD') AND (v_tot_kva-v_max_kva-v_min_kva <> v_min_kva)) THEN
         RETURN '4';
      END IF;
   END IF;
   IF v_null_trf_typ_cnt <> 0 THEN
      RETURN '9';
   END IF;
   -- if any of this transformers meters were on a different trf in the sm data, the month
   -- is indeterminate
/*  performance tuning indicates this isn't the best... try something else
   SELECT COUNT(*)
     INTO v_count
     FROM transformer t, meter m, historian.sm_sp_load_hist h
    WHERE t.id = i_trf_id
      AND t.id = m.trf_id
      AND nvl(m.rec_status, 'I') <> 'D'
      AND m.service_point_id = h.service_point_id
      AND h.batch_date = i_batch_date
      AND cgc_id <> cgc;
*/
   SELECT cgc_id
     INTO v_cgc
     FROM transformer
    WHERE id = i_trf_id;

   WITH mtr AS
     (SELECT service_point_id, v_cgc
        FROM meter m
       WHERE trf_id = i_trf_id
         AND nvl(m.rec_status, 'I') <> 'D')
   SELECT count(*)
     INTO v_count
     FROM mtr, historian.sm_sp_load_hist h
    WHERE mtr.service_point_id = h.service_point_id
      AND h.batch_date = i_batch_date
      AND cgc <> v_cgc;

   IF v_count <> 0 THEN
      RETURN '5';
   END IF;



   -- if the sm data has different spids, the month is indeterminate
/*  we have the cgc... take trf out and see
   SELECT COUNT(*)
     INTO v_count
     FROM (
            SELECT h.cgc, h.service_point_id
              FROM transformer t, historian.sm_sp_load_hist h
             WHERE t.id = i_trf_id
               AND t.cgc_id = h.cgc
               AND h.batch_date = i_batch_date
             MINUS
            SELECT cgc_id, m.service_point_id
              FROM transformer t, meter m
             WHERE t.id = i_trf_id
               AND t.id = m.trf_id
               AND nvl(m.rec_status, 'i') <> 'D')twy;
               */
   SELECT COUNT(*)
     INTO v_count
     FROM (
            SELECT h.cgc, h.service_point_id
              FROM historian.sm_sp_load_hist h
             WHERE v_cgc = h.cgc
               AND h.batch_date = i_batch_date
             MINUS
            SELECT v_cgc, m.service_point_id
              FROM meter m
             WHERE m.trf_id = i_trf_id
               AND nvl(m.rec_status, 'i') <> 'D')twy;

   IF v_count <> 0 THEN
      RETURN '5';
   END IF;
RETURN '0';
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN '-1';

END;

FUNCTION Establish_Seasonal_Peak(i_trf_id NUMBER, i_season VARCHAR2, i_batch_date DATE)
RETURN INTEGER
IS
/*
 Purpose:  Given a transformer id determine if the existing seasonal peak needs
           to be updated and do so.
           i_batch_date is the most recently calculated/recalculated month, it's
           possible that the data has changed and if it was the peak previously, its
           values need to be updated in the peak tables


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        8/02/19     Initial coding
   TJJ4        8/12/19     adding Generation


*/
   v_routine   VARCHAR2(64) := 'TLT.Est_Seas_Peak';
   v_hint      INTEGER := 0;

   v_status             NUMBER;
   v_start_date         trf_peak_hist.batch_date%TYPE;
   v_trf_peak_count     INTEGER;
   v_trf_season_peak    trf_peak.smr_peak_date%TYPE;
   v_curr_trf_seas_peak trf_peak.smr_peak_date%TYPE;

BEGIN
   IF i_season = 'S' THEN
      WITH
      DATA AS
        (SELECT t.trf_id, batch_date, trf_peak_time, id, trf_peak_kva,
                row_number() over (PARTITION BY trf_id ORDER BY trf_peak_kva DESC,
                                                                trf_peak_time DESC) rk
           FROM trf_peak_hist  t
          WHERE batch_date >= (SELECT ADD_MONTHS(MAX(batch_date),-11)
                                 FROM trf_peak_hist
                                WHERE trf_id = i_trf_id)
            AND trf_id = i_trf_id
            AND EXTRACT(MONTH FROM batch_date) BETWEEN 4 AND 10)
      SELECT sum(decode(t.id, NULL, 0, 1)), nvl(min(t.smr_peak_date),SYSDATE), min(DATA.batch_date)
        INTO v_trf_peak_count, v_curr_trf_seas_peak, v_trf_season_peak
        FROM trf_peak t, DATA
       WHERE t.trf_id (+) = DATA.trf_id
         AND rk = 1;
      /* if v_trf_peak_count = 0 then there's no trf_peak record
         if v_trf_peak_count = 1 then there's a trf_peak record
         if v_trf_peak_count is null then there' no trf_peak record or trf_peak_hist records */
   ELSIF i_season = 'W' THEN
      WITH
      DATA AS
        (SELECT t.trf_id, batch_date, trf_peak_time, id, trf_peak_kva,
                row_number() over (PARTITION BY trf_id ORDER BY trf_peak_kva DESC,
                                                                trf_peak_time DESC) rk
           FROM trf_peak_hist  t
          WHERE batch_date >= (SELECT ADD_MONTHS(MAX(batch_date),-11)
                                 FROM trf_peak_hist
                                WHERE trf_id = i_trf_id)
            AND trf_id = i_trf_id
            AND EXTRACT(MONTH FROM batch_date) NOT BETWEEN 4 AND 10)
      SELECT sum(decode(t.id, NULL, 0, 1)), nvl(min(t.wntr_peak_date), SYSDATE), min(DATA.batch_date)
        INTO v_trf_peak_count, v_curr_trf_seas_peak, v_trf_season_peak
        FROM trf_peak t, DATA
       WHERE t.trf_id (+) = DATA.trf_id
         AND rk = 1;
   ELSE
     transformer_loading_tools.e_saved_message := v_routine || ' invalid season: ' || i_season;
     RETURN -1;
   END IF;

   IF ((v_trf_peak_count = 0) AND (v_trf_season_peak IS NOT NULL)) THEN
      v_status := Set_Peak(i_trf_id, i_season, v_trf_season_peak);
      IF v_status <> 0 THEN
         RETURN v_status;
      END IF;
   ELSIF ((v_trf_peak_count = 1) AND
         ((v_curr_trf_seas_peak <> v_trf_season_peak)) OR
          (v_curr_trf_seas_peak = i_batch_date)) THEN
      v_status := Set_peak(i_trf_id, i_season, v_trf_season_peak);
      IF v_status <> 0 THEN
         RETURN v_status;
      END IF;
   END IF;

-- deal with generation here...
   IF i_season = 'S' THEN
      WITH
      DATA AS
        (SELECT t.trf_id, batch_date, trf_peak_time, id, trf_peak_kva,
                row_number() over (PARTITION BY trf_id ORDER BY trf_peak_kva DESC,
                                                                trf_peak_time DESC) rk
           FROM trf_peak_gen_hist  t
          WHERE batch_date >= (SELECT ADD_MONTHS(MAX(batch_date),-11)
                                 FROM trf_peak_hist  -- yes not gen_hist, once a trf has a year of no generation, it has NO generation...
                                WHERE trf_id = i_trf_id)
            AND trf_id = i_trf_id
            AND EXTRACT(MONTH FROM batch_date) BETWEEN 4 AND 10)
      SELECT sum(decode(t.id, NULL, 0, 1)), nvl(min(t.smr_peak_date),SYSDATE), min(DATA.batch_date)
        INTO v_trf_peak_count, v_curr_trf_seas_peak, v_trf_season_peak
        FROM trf_peak_gen t, DATA
       WHERE t.trf_id (+) = DATA.trf_id
         AND rk = 1;
      /* if v_trf_peak_count = 0 then there's no trf_peak_gen record
         if v_trf_peak_count = 1 then there's a trf_peak_gen record
         if v_trf_peak_count is null then there' no peak record or peak_hist records
         if v_trf_season_peak is null, there's no seasonal peak data to record*/
   ELSE -- we already checked season above if we get here, it's W
      WITH
      DATA AS
        (SELECT t.trf_id, batch_date, trf_peak_time, id, trf_peak_kva,
                row_number() over (PARTITION BY trf_id ORDER BY trf_peak_kva DESC,
                                                                trf_peak_time DESC) rk
           FROM trf_peak_gen_hist  t
          WHERE batch_date >= (SELECT ADD_MONTHS(MAX(batch_date),-11)
                                 FROM trf_peak_hist
                                WHERE trf_id = i_trf_id)
            AND trf_id = i_trf_id
            AND EXTRACT(MONTH FROM batch_date) NOT BETWEEN 4 AND 10)
      SELECT sum(decode(t.id, NULL, 0, 1)), nvl(min(t.wntr_peak_date), SYSDATE), min(DATA.batch_date)
        INTO v_trf_peak_count, v_curr_trf_seas_peak, v_trf_season_peak
        FROM trf_peak_gen t, DATA
       WHERE t.trf_id (+) = DATA.trf_id
         AND rk = 1;
   END IF;

   IF ((v_trf_peak_count = 0) AND (v_trf_season_peak IS NOT NULL)) THEN
      v_status := Set_Peak_Gen(i_trf_id, i_season, v_trf_season_peak);
      IF v_status <> 0 THEN
         RETURN v_status;
      END IF;
   ELSIF ((v_trf_peak_count = 1) AND
         ((v_curr_trf_seas_peak <> v_trf_season_peak)) OR
          (v_curr_trf_seas_peak = i_batch_date)) THEN
      v_status := Set_Peak_Gen(i_trf_id, i_season, v_trf_season_peak);
      IF v_status <> 0 THEN
         RETURN v_status;
      END IF;
   ELSIF ((v_trf_peak_count = 1) AND (v_trf_season_peak IS NULL )) THEN -- need to clear the seasonal peak data
      v_status := Set_Peak_Gen(i_trf_id, i_season, v_trf_season_peak);
      IF v_status <> 0 THEN
         RETURN v_status;
      END IF;
   END IF;

   RETURN 0;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;


FUNCTION Get_Trf_Connection_Type(i_trf_id NUMBER)
RETURN VARCHAR2
IS
/*
 Purpose:  Implement the heuristic to determine the connection type the transformer
           has so we can distribute the load correctly.

          The Heuristic is:
          if the trf Seconday Voltage code is not 120/240 3-phase then
            connection type is Wye-Wye/Delta-Wye both of these allocate the
              load evenly across all units so no further checking is needed
              (at this time) to differenciate between them
          for SVC of 120/240 3 phase if trf primay voltage <> circuits primary voltage then
            connection type is Wye-Delta
          otherwise use  Delta-Delta

The Domain: Secondary Voltage has code: 23 w/description:  120/240 Three Phase

Note: the 6th, and 7th characters in a Circuit_id provide circuit_cd which can
      be used to lookup the circuit voltage (should change this someday...)


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/16/19     Initial coding


*/
   v_routine   VARCHAR2(64) := 'TLT.Get_trf_Conn_Type';
   v_hint      INTEGER := 0;

   v_svc           transformer.lowside_voltage%TYPE;
   v_trf_vlt_cd    transformer.operating_voltage%TYPE;
   v_ckt_vlt_cd    VARCHAR2(2);
   v_count_matches NUMBER;

BEGIN
   SELECT lowside_voltage, operating_voltage, substr(substr(CIRCUIT_ID, -4),1,2)
     INTO v_svc, v_trf_vlt_cd, v_ckt_vlt_cd
     FROM transformer
    WHERE id = i_trf_id;

   IF v_svc <> '23' THEN
      RETURN 'YY';
   ELSE
     SELECT COUNT(*)
       INTO v_count_matches
       FROM operating_voltage ov, circuit_voltage cv
      WHERE ov.cd = v_trf_vlt_cd
        AND cv.circuit_voltage = ov.voltage
        AND cv.circuit_cd = v_ckt_vlt_cd;
     IF v_count_matches = 0 THEN -- trf pvc <> circuit pvc
        RETURN 'YD';
     ELSE
        RETURN 'DD';
     END IF;
   END IF;

EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN 'EE';
END;

FUNCTION Ins_Trf_Bank_Hist(
         i_direction VARCHAR2, i_hist_id NUMBER, i_trf_bank_id NUMBER,
         i_np_kva NUMBER, i_cap_np_kva NUMBER, i_trf_peak_kva NUMBER,
         i_trf_grp_typ VARCHAR2, i_season VARCHAR2, i_coast_int_ind NUMBER,
         i_vault_ind VARCHAR2, i_yearly_lf NUMBER)
RETURN INTEGER
IS
/*
 Purpose:  insert a record in trf bank history or trf bank gen history,
           computing the banks capability as you go


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/19/19     Initial coding
   TJJ4        8/20/19     Mark the trf as undetermined if we can't create a bank record


*/

   v_routine   VARCHAR2(64) := 'TLT.Ins_trf_bank_hist';
   v_hint      INTEGER := 0;

   v_vault_ind varchar2(4);
   v_trf_id    transformer.id%TYPE;
   v_batch_date DATE;

BEGIN
-- 8/16/19 -- Vault_indicator is only used for transformers in group types
--            'SS-S','SS-T','UG-S','UG-T'   set the indicator to 0 for all others
--            to be able find the capability multiplier from the table

   IF i_trf_grp_typ IN ('SS-S','SS-T','UG-S','UG-T') THEN
     v_vault_ind := i_vault_ind;
   ELSE
     v_vault_ind := '0';
   END IF;
   IF i_direction = 'DELIVERED' THEN
      INSERT INTO trf_bank_peak_hist
            (trf_peak_hist_id, trf_bank_id, np_kva, trf_peak_kva, trf_cap)
      SELECT i_hist_id,  i_trf_bank_id, i_np_kva,
             round(i_trf_peak_kva,1),
             round(i_np_kva*(decode(trunc(i_yearly_lf/40),
                                   0, m.cap_multiplier_a,
                                      m.cap_multiplier_b)),1)
        FROM trf_np_cap_mult m
       WHERE m.trf_grp_type = i_trf_grp_typ
         AND m.season = i_season
         AND m.coast_interior_flg = decode(i_trf_grp_typ, 'PRI-CUST',0, i_coast_int_ind)
         AND m.vault_ind = v_vault_ind
         AND i_cap_np_kva >= m.kva_low
         AND i_cap_np_kva <= m.kva_high;
      IF SQL%ROWCOUNT = 0 THEN   -- we didn't create a record, somethings wrong log it
         UPDATE trf_peak_hist SET load_undetermined = 6
          WHERE id = i_hist_id;
          SELECT trf_id, batch_date
            INTO v_trf_id, v_batch_date
            FROM trf_peak_hist
           WHERE id = i_hist_id;
         Update_trf_peak_undetermined (v_trf_id, v_batch_date, i_season, i_direction, 6);
      END IF;
   ELSIF i_direction = 'RECEIVED' THEN
      INSERT INTO trf_bank_peak_gen_hist
            (trf_peak_gen_hist_id, trf_bank_id, np_kva, trf_peak_kva, trf_cap)
      SELECT i_hist_id,  i_trf_bank_id, i_np_kva,
             round(i_trf_peak_kva,1),
             round(i_np_kva*(decode(trunc(i_yearly_lf/40),
                                   0, m.cap_multiplier_a,
                                      m.cap_multiplier_b)),1)
        FROM trf_np_cap_mult m
       WHERE m.trf_grp_type = i_trf_grp_typ
         AND m.season = i_season
         AND m.coast_interior_flg = i_coast_int_ind
         AND m.vault_ind = v_vault_ind
         AND i_cap_np_kva >= m.kva_low
         AND i_cap_np_kva <= m.kva_high;
   ELSE -- throw an error...
      transformer_loading_tools.e_saved_message := v_routine || ' invalid i_direction: '|| i_direction;
      RETURN -1;
   END IF;

   RETURN 0;

EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;

FUNCTION Set_Peak(i_trf_id NUMBER, i_season VARCHAR2, i_trf_peak DATE)
RETURN INTEGER
IS
/*
 Purpose:  Set the Peak information for the passed transformer, for the passed season
           to the passed value.

           if no trf_peak record exists create one

           code is assuming the caller sent aligned season and trf_peak

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        8/2/19     Initial coding

*/
   v_routine   VARCHAR2(64) := 'TLT.Set_Peak';
   v_hint      INTEGER := 0;

   v_count            INTEGER;
   v_trf_peak_id      trf_peak.id%TYPE;
   v_trf_peak_hist_id trf_peak_hist.id%TYPE;
   v_trf_kva          trf_peak_hist.trf_peak_kva%TYPE;
   v_sm_cust          trf_peak_hist.sm_cust_total%TYPE;
   v_ccb_cust         trf_peak_hist.ccb_cust_total%TYPE;
   v_yearly_lf        trf_peak_hist.yearly_lf%TYPE;
   v_load_undet       trf_peak_hist.load_undetermined%TYPE;

BEGIN
   SELECT count(*), min(id)
     INTO v_count, v_trf_peak_id
     FROM trf_peak
    WHERE trf_id = i_trf_id ;
   IF v_count = 0 THEN -- we need to create a new record
      v_trf_peak_id := trf_peak_seq.NEXTVAL;
      INSERT INTO trf_peak
             (id, trf_id, data_start_date)
      VALUES (v_trf_peak_id, i_trf_id, i_trf_peak);
   END IF;
v_hint := 10;
   SELECT trf_peak_kva, sm_cust_total, ccb_cust_total, yearly_lf,
          load_undetermined, id
     INTO v_trf_kva,    v_sm_cust,     v_ccb_cust,  v_yearly_lf,
          v_load_undet,      v_trf_peak_hist_id
     FROM trf_peak_hist
    WHERE batch_date = i_trf_peak
      AND trf_id = i_trf_id;
v_hint := 20;
   IF i_season = 'S' THEN
      UPDATE trf_peak
         SET smr_peak_date = i_trf_peak,
             smr_kva = v_trf_kva,
             smr_peak_total_cust_cnt = v_ccb_cust,
             smr_peak_sm_cust_cnt = v_sm_cust,
             smr_lf = v_yearly_lf,
             smr_load_undetermined = v_load_undet
       WHERE trf_id = i_trf_id;
      -- clear the fields for all records...
      UPDATE trf_bank_peak p
         SET smr_kva = NULL,
             smr_cap = NULL,
             smr_pct = NULL
       WHERE trf_peak_id = v_trf_peak_id;
      -- set the fields where the bank_id's match
      UPDATE trf_bank_peak p
         SET (smr_kva, smr_cap, np_kva, smr_pct) =
       (SELECT trf_peak_kva, trf_cap, np_kva,
               decode(h.trf_cap, 0, NULL, round(((h.trf_peak_kva/h.trf_cap)*100),1))
          FROM trf_bank_peak_hist h
         WHERE h.trf_peak_hist_id = v_trf_peak_hist_id
           AND h.trf_bank_id = p.trf_bank_id
           AND h.np_kva = p.np_kva)   -- the duplex trfs that are mapped as only 1 unit have the same bank_id, but different np_kvas
       WHERE p.trf_peak_id = v_trf_peak_id;
      -- add new bank_id's
      INSERT INTO trf_bank_peak
           ( smr_kva, smr_cap, trf_peak_id, trf_bank_id, np_kva, smr_pct)
      SELECT trf_peak_kva, trf_cap, v_trf_peak_id, trf_bank_id, np_kva,
             decode(trf_cap, 0, NULL, round(((trf_peak_kva/trf_cap)*100),1))
        FROM trf_bank_peak_hist h
       WHERE trf_peak_hist_id = v_trf_peak_hist_id
        AND NOT EXISTS (SELECT 'me'  FROM trf_bank_peak
                         WHERE trf_peak_id = v_trf_peak_id
                           AND trf_bank_id = h.trf_bank_id );
   ELSIF i_season = 'W' THEN
      UPDATE trf_peak
         SET wntr_peak_date = i_trf_peak,
             wntr_kva = v_trf_kva,
             wntr_peak_total_cust_cnt = v_ccb_cust,
             wntr_peak_sm_cust_cnt = v_sm_cust,
             wntr_lf = v_yearly_lf,
             wntr_load_undetermined = v_load_undet
       WHERE trf_id = i_trf_id;

      -- clear the fields for all records...
      UPDATE trf_bank_peak p
         SET wntr_kva = NULL,
             wntr_cap = NULL,
             wntr_pct = NULL
       WHERE trf_peak_id = v_trf_peak_id;
v_hint := 70;
      -- set the fields where the bank_id's match
      UPDATE trf_bank_peak p
         SET (wntr_kva, wntr_cap, np_kva, wntr_pct) =
       (SELECT trf_peak_kva, trf_cap, np_kva,
               decode(h.trf_cap, 0, NULL, round(((h.trf_peak_kva/h.trf_cap)*100),1))
          FROM trf_bank_peak_hist h
         WHERE h.trf_peak_hist_id = v_trf_peak_hist_id
           AND h.trf_bank_id = p.trf_bank_id
           AND h.np_kva = p.np_kva)   -- the duplex trfs that are mapped as only 1 unit have the same bank_id, but different np_kvas
       WHERE p.trf_peak_id = v_trf_peak_id;
v_hint := 80;
      -- add new bank_id's
      INSERT INTO trf_bank_peak
           ( wntr_kva, wntr_cap, trf_peak_id, trf_bank_id, np_kva, wntr_pct)
      SELECT trf_peak_kva, trf_cap, v_trf_peak_id, trf_bank_id, np_kva,
             decode(trf_cap, 0, NULL, round(((trf_peak_kva/trf_cap)*100),1))
        FROM trf_bank_peak_hist h
       WHERE trf_peak_hist_id = v_trf_peak_hist_id
        AND NOT EXISTS (SELECT 'me'  FROM trf_bank_peak
                         WHERE trf_peak_id = v_trf_peak_id
                           AND trf_bank_id = h.trf_bank_id );

   ELSE
      transformer_loading_tools.e_saved_message := v_routine || ' invalid season: '|| i_season;
      RETURN -1;
   END IF;
v_hint := 100;
   DELETE FROM trf_bank_peak P
    WHERE trf_peak_id = v_trf_peak_id
      AND wntr_kva is NULL
      AND smr_kva  is NULL
      AND wntr_cap is NULL
      AND smr_cap  is NULL
      AND wntr_pct is NULL
      AND smr_pct  is NULL;

   -- now the denormalized customer type sums the UI needs -- not sure why.... but...
   DELETE
     FROM TRF_PEAK_BY_CUST_TYP
    WHERE trf_peak_id = v_trf_peak_id
      AND season = i_season;
   INSERT INTO TRF_PEAK_BY_CUST_TYP
         (trf_peak_id, season, cust_typ, season_cust_cnt, season_total_kva)
   SELECT v_trf_peak_id, i_season, cust_typ, count(*), nvl(sum(sp_kva_trf_peak),0)
     FROM trf_peak_hist h, sp_peak_hist s
    WHERE h.trf_id = i_trf_id
      AND h.batch_date = i_trf_peak
      AND h.id = s.trf_peak_hist_id
    GROUP BY cust_typ;

   RETURN 0;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;

FUNCTION Set_Peak_Gen(i_trf_id NUMBER, i_season VARCHAR2, i_trf_peak DATE)
RETURN INTEGER
IS
/*
 Purpose:  The function is basically the same as Set_Peak except it's managing
           the generation version of the tables and generation may be removed from
           a transformer, so this procedure needs to know when to delete the
           peak records in addition to creating/updating them

           if i_trf_peak is null, clear the peak data for the season
           if both summer and winter are blank, then delete the whole thing

           if no trf_peak record exists & i_trf_peak is not null create one

           code is assuming the caller sent aligned season and trf_peak

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        8/13/19     Initial coding

*/
   v_routine   VARCHAR2(64) := 'TLT.Set_Peak_Gen';
   v_hint      INTEGER := 0;

   v_count            INTEGER;
   v_trf_peak_id      trf_peak_gen.id%TYPE;
   v_trf_peak_hist_id trf_peak_gen_hist.id%TYPE;
   v_trf_kva          trf_peak_gen_hist.trf_peak_kva%TYPE;
   v_sm_cust          trf_peak_gen_hist.sm_cust_total%TYPE;
   v_ccb_cust         trf_peak_gen_hist.ccb_cust_total%TYPE;
   v_yearly_lf        trf_peak_gen_hist.yearly_lf%TYPE;
   v_load_undet       trf_peak_gen_hist.load_undetermined%TYPE;

BEGIN
   SELECT count(*), min(id)
     INTO v_count, v_trf_peak_id
     FROM trf_peak_gen
    WHERE trf_id = i_trf_id ;
   IF ((v_count = 0) AND (i_trf_peak IS NOT NULL)) THEN -- we need to create a new record
      v_trf_peak_id := trf_peak_gen_seq.NEXTVAL;
      INSERT INTO trf_peak_gen
             (id, trf_id, data_start_date)
      VALUES (v_trf_peak_id, i_trf_id, i_trf_peak);
   END IF;
v_hint := 10;
   IF ((v_count = 0) AND (i_trf_peak IS NULL)) THEN -- there's nothing to do, leave
      RETURN 0;
   ELSIF i_trf_peak IS NULL THEN -- is the other peak also null?
      SELECT count(*)
        INTO v_count
        FROM trf_peak_gen
       WHERE trf_id = i_trf_id
         AND decode(i_season, 'W', smr_peak_date, wntr_peak_date) IS NULL;
      IF v_count = 0 THEN -- both peaks are null time to delete the records
         DELETE
           FROM TRF_PEAK_GEN_BY_CUST_TYP
          WHERE trf_peak_gen_id = v_trf_peak_id;
         DELETE
           FROM trf_bank_peak_gen
          WHERE trf_peak_gen_id = v_trf_peak_id;
         DELETE
           FROM trf_peak_gen
          WHERE id = v_trf_peak_id;
         RETURN 0;
      END IF; -- let the code below set everything to null
   ELSE
      SELECT trf_peak_kva, sm_cust_total, ccb_cust_total, yearly_lf,
             load_undetermined, id
        INTO v_trf_kva,    v_sm_cust,     v_ccb_cust,  v_yearly_lf,
             v_load_undet, v_trf_peak_hist_id
        FROM trf_peak_gen_hist
       WHERE batch_date = i_trf_peak
         AND trf_id = i_trf_id;
   END IF;
v_hint := 20;
   IF i_season = 'S' THEN
      UPDATE trf_peak_gen
         SET smr_peak_date = i_trf_peak,
             smr_kva = v_trf_kva,
             smr_peak_total_cust_cnt = v_ccb_cust,
             smr_peak_sm_cust_cnt = v_sm_cust,
             smr_lf = v_yearly_lf,
             smr_load_undetermined = v_load_undet
       WHERE trf_id = i_trf_id;
      -- clear the fields for all records...
      UPDATE trf_bank_peak_gen p
         SET smr_kva = NULL,
             smr_cap = NULL,
             smr_pct = NULL
       WHERE trf_peak_gen_id = v_trf_peak_id;
      -- set the fields where the bank_id's match
      UPDATE trf_bank_peak_gen p
         SET (smr_kva, smr_cap, np_kva, smr_pct) =
       (SELECT trf_peak_kva, trf_cap, np_kva,
               decode(h.trf_cap, 0, NULL, round(((h.trf_peak_kva/h.trf_cap)*100),1))
          FROM trf_bank_peak_gen_hist h
         WHERE h.trf_peak_gen_hist_id = v_trf_peak_hist_id
           AND h.trf_bank_id = p.trf_bank_id
           AND h.np_kva = p.np_kva)   -- the duplex trfs that are mapped as only 1 unit have the same bank_id, but different np_kvas
      WHERE p.trf_peak_gen_id = v_trf_peak_id;
      -- add new bank_id's
      INSERT INTO trf_bank_peak_gen
           ( smr_kva, smr_cap, trf_peak_gen_id, trf_bank_id, np_kva, smr_pct)
      SELECT trf_peak_kva, trf_cap, v_trf_peak_id, trf_bank_id, np_kva,
             decode(trf_cap, 0, NULL, round(((trf_peak_kva/trf_cap)*100),1))
        FROM trf_bank_peak_gen_hist h
       WHERE trf_peak_gen_hist_id = v_trf_peak_hist_id
        AND NOT EXISTS (SELECT 'me'  FROM trf_bank_peak_gen
                         WHERE trf_peak_gen_id = v_trf_peak_id
                           AND trf_bank_id = h.trf_bank_id );
   ELSIF i_season = 'W' THEN
      UPDATE trf_peak_gen
         SET wntr_peak_date = i_trf_peak,
             wntr_kva = v_trf_kva,
             wntr_peak_total_cust_cnt = v_ccb_cust,
             wntr_peak_sm_cust_cnt = v_sm_cust,
             wntr_lf = v_yearly_lf,
             wntr_load_undetermined = v_load_undet
       WHERE trf_id = i_trf_id;

      -- clear the fields for all records...
      UPDATE trf_bank_peak_gen p
         SET wntr_kva = NULL,
             wntr_cap = NULL,
             wntr_pct = NULL
       WHERE trf_peak_gen_id = v_trf_peak_id;
v_hint := 70;
      -- set the fields where the bank_id's match
      UPDATE trf_bank_peak_gen p
         SET (wntr_kva, wntr_cap, np_kva, wntr_pct) =
       (SELECT trf_peak_kva, trf_cap, np_kva,
               decode(h.trf_cap, 0, NULL, round(((h.trf_peak_kva/h.trf_cap)*100),1))
          FROM trf_bank_peak_gen_hist h
         WHERE h.trf_peak_gen_hist_id = v_trf_peak_hist_id
           AND h.trf_bank_id = p.trf_bank_id
           AND h.np_kva = p.np_kva)   -- the duplex trfs that are mapped as only 1 unit have the same bank_id, but different np_kvas
       WHERE p.trf_peak_gen_id = v_trf_peak_id;
v_hint := 80;
      -- add new bank_id's
      INSERT INTO trf_bank_peak_gen
           ( wntr_kva, wntr_cap, trf_peak_gen_id, trf_bank_id, np_kva, wntr_pct)
      SELECT trf_peak_kva, trf_cap, v_trf_peak_id, trf_bank_id, np_kva,
             decode(trf_cap, 0, NULL, round(((trf_peak_kva/trf_cap)*100),1))
        FROM trf_bank_peak_gen_hist h
       WHERE trf_peak_gen_hist_id = v_trf_peak_hist_id
        AND NOT EXISTS (SELECT 'me'  FROM trf_bank_peak_gen
                         WHERE trf_peak_gen_id = v_trf_peak_id
                           AND trf_bank_id = h.trf_bank_id );

   ELSE
      transformer_loading_tools.e_saved_message := v_routine || ' invalid season: '|| i_season;
      RETURN -1;
   END IF;
v_hint := 100;
   DELETE
     FROM trf_bank_peak_gen P
    WHERE trf_peak_gen_id = v_trf_peak_id
      AND wntr_kva = NULL
      AND smr_kva  = NULL
      AND wntr_cap = NULL
      AND smr_cap  = NULL
      AND wntr_pct = NULL
      AND smr_pct  = NULL;

   -- now the denormalized customer type sums the UI needs -- not sure why.... but...
   DELETE
     FROM TRF_PEAK_GEN_BY_CUST_TYP
    WHERE trf_peak_gen_id = v_trf_peak_id
      AND season = i_season;
   INSERT INTO TRF_PEAK_GEN_BY_CUST_TYP
         (trf_peak_gen_id, season, cust_typ, season_cust_cnt, season_total_kva)
   SELECT v_trf_peak_id, i_season, cust_typ, count(*), nvl(sum(sp_kva_trf_peak),0)
     FROM trf_peak_gen_hist h, sp_peak_gen_hist s
    WHERE h.trf_id = i_trf_id
      AND h.batch_date = i_trf_peak
      AND h.id = s.trf_peak_gen_hist_id
    GROUP BY cust_typ;

   RETURN 0;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;

PROCEDURE Update_trf_peak_undetermined (i_trf_id NUMBER, i_batch_date DATE,
       i_season VARCHAR2, i_direction VARCHAR2, i_value VARCHAR2)
AS
/*
 Purpose:  Maintain the undetermined field in the denormalized peak tables
           when we recalculate a month and the recalculation effort throws an error.

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        10/25/19    Initial coding

*/
BEGIN
   IF i_direction = 'DELIVERED' THEN
      IF i_season = 'S' THEN
         UPDATE trf_peak SET smr_load_undetermined = i_value
         WHERE trf_id = i_trf_id
           AND smr_peak_date = i_batch_date;
      ELSIF i_season = 'W' THEN
         UPDATE trf_peak SET wntr_load_undetermined = i_value
         WHERE trf_id = i_trf_id
           AND wntr_peak_date = i_batch_date;
      END IF;
   ELSIF i_direction = 'RECEIVED' THEN
      IF i_season = 'S' THEN
         UPDATE trf_peak_gen SET smr_load_undetermined = i_value
         WHERE trf_id = i_trf_id
           AND smr_peak_date = i_batch_date;
      ELSIF i_season = 'W' THEN
         UPDATE trf_peak_gen SET wntr_load_undetermined = i_value
         WHERE trf_id = i_trf_id
           AND wntr_peak_date = i_batch_date;
      END IF;
   END IF;
END;

FUNCTION Update_trf_peak_hist(
      i_trf_id NUMBER, i_trf_hist_id NUMBER, i_batch_date  DATE)
RETURN INTEGER
IS
/*
 Purpose:  Update the fields on trf_peak_hist.

          trf_peak_time is the smart meter peak time if a smart meter record exists
                        otherwise default to the beginning of the batch month
           trf_peak_kva is the sum of the sp_kva_trf_peak
                        this value is used for calculating how heavily loaded the trf is
       trf_peak_kva_dom is the kva supplied via a single phase service drop
                        typically this would be Domestic customers, but
                        some cust_type = Dom customers have 3 phase service and
                        some non-dom customers have 1 phase services
          sm_cust_total is the number of smart metered service points for the month
         ccb_cust_total is the total number of customers
            trf_peak_kw is the peak kw measured by sm but only for the smart meters
             trf_avg_kw is the average kw measured by sm again only for smart meters
                        these two values are used to calculate the yearly load factor

           load_factor  (for now) if all the customers are smart metered, calculate a
                        yearly load factor by calling the procedure

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/2/19     Initial coding
   TJJ4        7/23/19    set null kva's to 0


*/

   v_routine   VARCHAR2(64) := 'TLT.Upd_trf_pk_hist';
   v_hint      INTEGER := 0;

   v_Cust_count      NUMBER;
   v_Sm_Mtr_count    NUMBER;
   v_Trf_kva         trf_peak_hist.trf_peak_kva%TYPE;
   v_Sngl_phase_kva  trf_peak_hist.trf_peak_kva_dom%TYPE;
   v_cgc             transformer.cgc_id%TYPE;
   v_Yr_Lf           NUMBER(3);

BEGIN
  -- fetch the data we just calculated
   SELECT COUNT(*), SUM(decode(sm_flg, 'S', 1, 0)), nvl(SUM(sp_kva_trf_peak),0),
          nvl(SUM(decode(phase, '3', 0, sp_kva_trf_peak)),0)   -- everything not coded as 3 phase is assumed to be 1 phase
     INTO v_Cust_count, v_Sm_Mtr_count, v_Trf_kva, v_Sngl_phase_kva
     FROM sp_peak_hist
    WHERE trf_peak_hist_id = i_trf_hist_id;
-- if approved by engineering... for Legacy meters avg could be ccb.revkwhr/24 and peak_kw calculated kvw*seasonal pf...
-- need to fup later on this
   IF v_Sm_Mtr_count > 0 THEN  -- there's some information from smart meter, get it
      SELECT cgc_id
        INTO v_cgc
        FROM transformer
       WHERE id = i_trf_id; -- unique primary key, we can ignore rec_status
      v_hint := 10;
      UPDATE trf_peak_hist h SET
               (ccb_cust_total, sm_cust_total, trf_peak_kva, trf_peak_kw, trf_ave_kw,
                trf_peak_time, trf_peak_kva_dom, yearly_lf) =
       ( SELECT v_Cust_count, v_Sm_Mtr_count, v_Trf_kva, trf_peak_kw, trf_avg_kw,
                trf_peak_time, v_Sngl_phase_kva, NULL
            FROM historian.sm_trf_load_hist
           WHERE cgc = v_cgc
             AND batch_date = i_batch_date)
       WHERE id = i_trf_hist_id;
       v_hint := 20;
       -- need peak kw/avg kw updated before calculating yearly LF...
       IF v_Cust_count = v_Sm_Mtr_count THEN -- for now yearly LF only valid for all sm trfs
          v_Yr_Lf := Calc_yearly_lf(i_trf_id, i_batch_date, 'DELIVERED');
          IF v_Yr_Lf = -1 THEN
             RETURN v_Yr_Lf;  -- pass the error
          ELSIF v_Yr_Lf IS NOT NULL THEN
             UPDATE trf_peak_hist SET yearly_lf = v_Yr_Lf
              WHERE id = i_trf_hist_id;
          END IF;
       END IF;
       -- what if there's a problem with historian???  need to revisit this
   ELSE
      v_hint := 30;
      UPDATE trf_peak_hist SET
         ccb_cust_total   = v_Cust_count,
         sm_cust_total    = v_Sm_Mtr_count,
         trf_peak_kva     = v_Trf_kva,
         trf_peak_kva_dom = v_Sngl_phase_kva,
         yearly_lf        = NULL
       WHERE id = i_trf_hist_id ;
   END IF;

RETURN 0;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END Update_trf_peak_hist;

FUNCTION Update_trf_peak_gen_hist(i_trf_id NUMBER, i_hist_id NUMBER, i_batch_date DATE)
RETURN INTEGER
IS
/*
 Purpose:  Update the fields on trf_peak_gen_hist, all of this information comes
           from smart meter.


          trf_peak_time is the smart meter peak time
           trf_peak_kva is the sum of the sp_kva_trf_peak
       trf_peak_kva_dom is the kva received via a single phase service drop
                        typically this would be Domestic customers, but
                        some cust_type = Dom customers have 3 phase service and
                        some non-dom customers have 1 phase services
          sm_cust_total is the number of smart metered service points with generation
                        for the month
         ccb_cust_total is the total number of service points with generation
                        the same as above currently since ALL generation information
                        is coming from smart meter
            trf_peak_kw is the peak kw measured by sm
             trf_avg_kw is the average kw measured by sm
                        these two values are used to calculate the yearly load factor

           load_factor  the load factor is the sum of the previous years average kva
                        divided by the sum of the peak kva's

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        7/26/19     Initial coding

*/

   v_routine   VARCHAR2(64) := 'TLT.Upd_trf_pk_gen_hist';
   v_hint      INTEGER := 0;

   v_Cust_count      NUMBER;
   v_Sm_Mtr_count    NUMBER;
   v_Trf_kva         trf_peak_gen_hist.trf_peak_kva%TYPE;
   v_Sngl_phase_kva  trf_peak_gen_hist.trf_peak_kva_dom%TYPE;
   v_cgc             transformer.cgc_id%TYPE;
   v_Yr_Lf           NUMBER(3);

BEGIN
  -- fetch the data we just calculated
   SELECT COUNT(*), SUM(decode(sm_flg, 'S', 1, 0)), nvl(SUM(sp_kva_trf_peak),0),
          nvl(SUM(decode(phase, '3', 0, sp_kva_trf_peak)),0)   -- everything not coded as 3 phase is assumed to be 1 phase
     INTO v_Cust_count, v_Sm_Mtr_count, v_Trf_kva, v_Sngl_phase_kva
     FROM sp_peak_gen_hist
    WHERE trf_peak_gen_hist_id = i_hist_id;

   SELECT cgc_id
     INTO v_cgc
     FROM transformer
    WHERE id = i_trf_id; -- unique primary key, we can ignore rec_status
   v_hint := 10;
   UPDATE trf_peak_gen_hist h SET
            (ccb_cust_total, sm_cust_total, trf_peak_kva, trf_peak_kw, trf_ave_kw,
             trf_peak_time, trf_peak_kva_dom, yearly_lf) =
    ( SELECT v_Cust_count, v_Sm_Mtr_count, v_Trf_kva, trf_peak_kw, trf_avg_kw,
             trf_peak_time, v_Sngl_phase_kva, NULL
         FROM historian.sm_trf_gen_load_hist
        WHERE cgc = v_cgc
          AND batch_date = i_batch_date)
    WHERE id = i_hist_id;
   v_hint := 20;
   -- need peak kw/avg kw updated before calculating yearly LF...
   v_Yr_Lf := Calc_yearly_lf(i_trf_id, i_batch_date, 'RECEIVED');
   IF v_Yr_Lf = -1 THEN
      RETURN v_Yr_Lf;  -- pass the error
   END IF;
   v_hint := 30;
   UPDATE trf_peak_gen_hist SET yearly_lf = v_Yr_Lf
    WHERE id = i_hist_id;

RETURN 0;
EXCEPTION
   WHEN OTHERS
      THEN
         transformer_loading_tools.e_saved_message := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         RETURN -1;
END;

PROCEDURE Maintain_tables
AS
/*
 Purpose:  This routine will do maintenance on the tables.

           Currently it will:
           - remove any data from the history tables more than 38 months old
           - update the Data_start_date for all records in trf_peak and trf_peak_gen
           - delete the peak records for transformers that have no history
           - remove any data from historian xxx_hist tables more than 15 months old

           it needs to:
           remove any data more then 38 months old from cyme tables
           rebuild historian's indexes
           recompute historian's statistics

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        09/12/19    Initial coding
   TJJ4        09/25/19    added deletes to cyme tables, call to rebuild historian's
                           indexes and recompute historian's stats
   TJJ4        10/31/19    added some logging to track progress
                           adjusted cyme deletes to 1 month each, oldest first
   TJJ4        06/23/20    added range index scan hint to fix oracle 12 execution plan

*/

   v_batch_date        trf_peak_hist.batch_date%TYPE;
   v_cyme_batch_date   trf_peak_hist.batch_date%TYPE;
   v_count             NUMBER;
   v_log_status        VARCHAR2(10);
   v_totalRowsMigrated NUMBER := 0;

BEGIN
   delete from MONTHLY_LOAD_LOG where table_name='Maintain_tables_hist';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Maintain_tables_hist');

   SELECT add_months(trunc(SYSDATE, 'mm'), -38)
     INTO v_batch_date
     FROM dual;
   -- try removing the constraints & restoring after the deletes...
/*   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST DISABLE CONSTRAINT SP_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST DISABLE CONSTRAINT SP_PEAK_GEN_HIST_UK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST DISABLE CONSTRAINT SP_PEAK_GEN_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST DISABLE CONSTRAINT SP_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST DISABLE CONSTRAINT SP_PEAK_HIST_UK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST DISABLE CONSTRAINT SP_PEAK_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST DISABLE CONSTRAINT SP_PEAK_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST DISABLE CONSTRAINT SP_PEAK_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST DISABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST DISABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST DISABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_HIST DISABLE CONSTRAINT TRF_BANK_PEAK_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_HIST DISABLE CONSTRAINT TRF_BANK_PEAK_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_GEN_HIST DISABLE CONSTRAINT TRF_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_GEN_HIST DISABLE CONSTRAINT TRF_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_HIST DISABLE CONSTRAINT TRF_PEAK_HIST_PK';
*/
   -- clear the Generation table set first, remember the constraints
   DELETE FROM SP_PEAK_GEN_HIST
    WHERE trf_peak_gen_hist_id IN (SELECT id FROM trf_peak_gen_hist
                                    WHERE batch_date < v_batch_date);
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_BANK_PEAK_GEN_HIST
    WHERE trf_peak_gen_hist_id IN (SELECT id FROM trf_peak_gen_hist
                                    WHERE batch_date < v_batch_date);
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_PEAK_GEN_HIST
    WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   -- Now the regular hist tables
   DELETE FROM SP_PEAK_HIST
    WHERE trf_peak_hist_id IN (SELECT id FROM trf_peak_hist
                                WHERE batch_date < v_batch_date);
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_BANK_PEAK_HIST
    WHERE trf_peak_hist_id IN (SELECT id FROM trf_peak_hist
                                WHERE batch_date < v_batch_date);
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_PEAK_HIST
    WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

/*   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_GEN_HIST ENABLE CONSTRAINT TRF_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_GEN_HIST ENABLE CONSTRAINT TRF_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST ENABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST ENABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_GEN_HIST ENABLE CONSTRAINT TRF_BANK_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST ENABLE CONSTRAINT SP_PEAK_GEN_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST ENABLE CONSTRAINT SP_PEAK_GEN_HIST_UK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST ENABLE CONSTRAINT SP_PEAK_GEN_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_GEN_HIST ENABLE CONSTRAINT SP_PEAK_GEN_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_PEAK_HIST ENABLE CONSTRAINT TRF_PEAK_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_HIST ENABLE CONSTRAINT TRF_BANK_PEAK_HIST_FK1';
   EXECUTE IMMEDIATE 'ALTER TABLE TRF_BANK_PEAK_HIST ENABLE CONSTRAINT TRF_BANK_PEAK_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST ENABLE CONSTRAINT SP_PEAK_HIST_UK1';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST ENABLE CONSTRAINT SP_PEAK_HIST_PK';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST ENABLE CONSTRAINT SP_PEAK_HIST_FK2';
   EXECUTE IMMEDIATE 'ALTER TABLE SP_PEAK_HIST ENABLE CONSTRAINT SP_PEAK_HIST_FK1';
*/
   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Maintain_tables_hist',v_totalRowsMigrated);

   delete from MONTHLY_LOAD_LOG where table_name='Maintain_tables_peak';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Maintain_tables_peak');

   -- fix the data_start_dates in the peak tables
  /* 06/23/20 horrible execution plan by Oracle 12 for this.  fixed below
   UPDATE trf_peak_gen t
   SET data_start_date = (SELECT min(batch_date)
                            FROM trf_peak_gen_hist h
                           WHERE h.trf_id = t.trf_id);
*/
   UPDATE trf_peak_gen t
   SET data_start_date = (SELECT /*+ INDEX_RS_ASC(h) */ MIN(batch_date)
                            FROM trf_peak_gen_hist h
                           WHERE h.trf_id = t.trf_id);
    v_totalRowsMigrated := SQL%ROWCOUNT;
      -- 220685 rows in 81.9 seconds in q6q

   UPDATE trf_peak t
   SET data_start_date = (SELECT min(batch_date)
                            FROM trf_peak_hist h
                           WHERE h.trf_id = t.trf_id);
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     -- 905036 rows in 352 sec 5.8 min

   -- all the transformers witll null data_start_dates have no information in the history
   -- tables, so delete them from the peak tables the UI uses.
   DELETE FROM TRF_PEAK_GEN_BY_CUST_TYP
    WHERE trf_peak_gen_id IN (SELECT id FROM trf_peak_gen
                               WHERE data_start_date IS NULL);
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_BANK_PEAK_GEN
    WHERE trf_peak_gen_id IN (SELECT id FROM trf_peak_gen
                               WHERE data_start_date IS NULL);
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_PEAK_GEN WHERE data_start_date IS NULL;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_PEAK_BY_CUST_TYP
    WHERE trf_peak_id IN (SELECT id FROM TRF_PEAK
                           WHERE data_start_date IS NULL);
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_BANK_PEAK
    WHERE trf_peak_id IN (SELECT id FROM TRF_PEAK
                           WHERE data_start_date IS NULL);
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   DELETE FROM TRF_PEAK WHERE data_start_date IS NULL;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Maintain_tables_peak',v_totalRowsMigrated);

   delete from MONTHLY_LOAD_LOG where table_name='Maintain_tables_cyme';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Maintain_tables_cyme');

   BEGIN
      SELECT min(batch_date)
        INTO v_cyme_batch_date
        FROM trf_ccb_meter_load
       WHERE batch_date < v_batch_date;
   EXCEPTION
   WHEN NO_DATA_FOUND THEN
     -- there's no data to delete
     v_cyme_batch_date := ADD_MONTHS(v_batch_date,-1);
   END;

   -- clean up the old data from the cyme interface tables
   delete from SP_CCB_METER_LOAD
    where TRF_CCB_METER_LOAD_ID in (select id from TRF_CCB_METER_LOAD
                                     where batch_date = v_cyme_batch_date);
   v_totalRowsMigrated := SQL%ROWCOUNT;
   delete from TRF_CCB_METER_LOAD where batch_date = v_cyme_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Maintain_tables_cyme',v_totalRowsMigrated);

   delete from MONTHLY_LOAD_LOG where table_name='Maintain_tables_historian';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Maintain_tables_historian');

   SELECT add_months(trunc(SYSDATE, 'mm'), -15)
     INTO v_batch_date
     FROM dual;

   DELETE FROM historian.ccb_meter_load_hist WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := SQL%ROWCOUNT;
   DELETE FROM historian.sm_sp_gen_load_hist WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
   DELETE FROM historian.sm_sp_load_hist WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
   DELETE FROM historian.sm_trf_gen_load_hist WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
   DELETE FROM historian.sm_trf_load_hist WHERE batch_date < v_batch_date;
   v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

   historian.rebuild_indexes();
   historian.gather_my_stats('CCB');
   historian.gather_my_stats('CDW');
   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Maintain_tables_historian',v_totalRowsMigrated);

END;

PROCEDURE Monthly_run ( i_batch_date DATE)
AS

CURSOR get_trfs
IS
  SELECT DISTINCT t.id FROM transformer t, meter m
   WHERE nvl(t.rec_status, 'i')<> 'D'  --AND id IN (10000011062); --,220024646,420009589);
     AND nvl(m.rec_status, 'i') <> 'D'
     AND t.id = m.trf_id
   MINUS
   SELECT trf_id FROM trf_peak_hist WHERE batch_date = i_batch_date  ;
/* these are the 18 unit test trfs
AND id IN (520003119, 120073754,120001920,920049802,520008369,120027693,520045830,
            120017952,520018282,520012085,220070557,220074263,220060351,120027518,
            1820043441,220056334,220007775,2020011746)
*/
--AND ROWNUM < 10000001;

   v_count NUMBER := 0;
   v_log_id NUMBER;
   v_trf_id transformer.id%TYPE;
   O_ERRORMSG VARCHAR2(32000);
   O_ERRORCODE VARCHAR2(32000);
   v_season varchar2(4) := 'S';

BEGIN
  IF to_char(i_batch_date, 'mm') IN ('11', '12','01','02', '03') THEN
     v_season := 'W';
  END IF;
  v_log_id := monthly_load_log_seq.NEXTVAL;
  DELETE from monthly_load_log;
  INSERT INTO monthly_load_log (id, table_name, load_start_ts, num_records_processed, create_dtm)
    VALUES (v_log_id, 'Monthly run', SYSDATE, 0, SYSDATE);
  OPEN get_trfs;
  LOOP
   FETCH get_trfs INTO v_trf_id;
   EXIT WHEN get_trfs%NOTFOUND;
      recalc_trf_loading(v_trf_id, i_batch_date, v_season, o_errormsg, o_errorcode);
      COMMIT;
   v_count := v_count +1;
--   IF mod(v_count,100000) = 0 THEN
--     UPDATE monthly_load_log SET num_records_processed = v_count
--       WHERE id = v_log_id;
--   END IF;
END LOOP;
CLOSE get_trfs;
  UPDATE monthly_load_log SET num_records_processed = v_count, load_end_ts = sysdate
   WHERE id = v_log_id;

END Monthly_run;


PROCEDURE Monthly_TLM_run (i_variant VARCHAR2, i_batch_date DATE, i_thread VARCHAR2)
AS
/*
 Purpose:  This is the driver routine for a monthly TLM run.

           i_variant tells the routine which set of transformers to execute the load for

           Valid i_variants are:
               SMART  - executes TLM for all transformers that have all smart metered accounts.
               LEGACY - executes TLM for all transformers that have a legacy account
               ALL    - executes TLM for all transformers that have customers

           The procedure expects and requires UC4 to execute it for 10 threads
           numbered 0 thru 9 and transformers are selected for each thread based
           on the final digit of their ID in the transformer table.

           If i_batch_date is null, and i_variant is SMART the program will use
           the most recent data from historian.sm_trf_load_hist

           if i_batch_date is null and i_variant is LEGACY or ALL the program will
           use the most recent data from historian.ccb_meter_load_hist

           If i_thread is null, or not between 0..9 the program will return an error.
           if i_variant not in SMART, LEGACY, ALL, the program will return an error.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        09/06/19    Initial coding

*/
   invalid_cdw_data EXCEPTION;
   invalid_ccb_data EXCEPTION;
   invalid_variant  EXCEPTION;
   invalid_thread   EXCEPTION;

   v_batch_date trf_peak_hist.batch_date%TYPE;
   get_trfs        sys_refcursor;
   v_log_name      varchar2(40);

CURSOR get_trfs_v1
IS
   SELECT id
     FROM
        (SELECT t.id, t.cgc_id, count(m.id) mtr_cnt
           FROM transformer t, meter m
          WHERE nvl(t.rec_status, 'i')<> 'D'
            AND nvl(m.rec_status, 'i') <> 'D'
            AND t.id = m.trf_id
          GROUP BY t.id, t.cgc_id  ) trfs,
        (SELECT cgc, count(*) sm_cnt
           FROM historian.sm_sp_load_hist
          WHERE batch_date = v_batch_date
          GROUP BY cgc) sm_trfs
     WHERE cgc_id = cgc
       AND mtr_cnt = sm_cnt
       AND substr(id, -1,1) = i_thread;

   v_count NUMBER := 0;
   v_log_id NUMBER;
   v_trf_id transformer.id%TYPE;
   O_ERRORMSG VARCHAR2(32000);
   O_ERRORCODE VARCHAR2(32000);
   v_season varchar2(4) := 'S';

BEGIN
   IF upper(nvl(i_variant,'none')) NOT IN ('SMART','LEGACY','ALL') THEN
      RAISE invalid_variant;
   END IF;
   IF nvl(i_thread,'T') NOT IN ('0','1','2','3','4','5','6','7','8','9') THEN --BETWEEN '0' AND '9' THEN
      RAISE invalid_thread;
   END IF;
   IF (i_batch_date IS NULL) AND (i_variant = 'SMART') THEN -- find the most recent batch date in the table
      SELECT max(batch_date)
        INTO v_batch_date
        FROM historian.sm_trf_load_hist;
   ELSIF (i_batch_date IS NULL) THEN
      SELECT max(batch_date)
        INTO v_batch_date
        FROM historian.ccb_meter_load_hist;
   ELSE
      v_batch_date := i_batch_date; -- expecting user to get the day right...
   END IF;

-- do a precautionary check on the data just to make sure it appears to be there

   SELECT count(*)
     INTO v_count
     FROM historian.sm_trf_load_hist
    WHERE batch_date = v_batch_date;

   IF v_count < 820000 THEN -- there are 909k trfs, 840k have smart meters in sep 2019
      RAISE invalid_cdw_data;
   END IF;

   IF i_variant <> 'SMART' THEN
      SELECT count(*)
        INTO v_count
        FROM historian.ccb_meter_load_hist
       WHERE batch_date = v_batch_date;
      IF v_count < 5500000 THEN -- there are between 5.5m and 5.6m customers as of sep 2019
         RAISE invalid_ccb_data;
      END IF;
   END IF;

   IF to_char(v_batch_date, 'mm') IN ('11', '12','01','02', '03') THEN
      v_season := 'W';
   END IF;
   v_log_name := 'Monthly run ' || i_variant || ' ' || i_thread;
   v_log_id := monthly_load_log_seq.NEXTVAL;
   DELETE from monthly_load_log WHERE table_name = v_log_name;
   INSERT INTO monthly_load_log (id, table_name, load_start_ts, num_records_processed, create_dtm)
        VALUES (v_log_id, v_log_name, SYSDATE, 0, SYSDATE);

   IF i_variant = 'SMART' THEN
      OPEN get_trfs FOR
         SELECT id
           FROM
              (SELECT t.id, t.cgc_id, count(m.id) mtr_cnt
                 FROM transformer t, meter m
                WHERE nvl(t.rec_status, 'i')<> 'D'
                  AND nvl(m.rec_status, 'i') <> 'D'
                  AND t.id = m.trf_id
                GROUP BY t.id, t.cgc_id  ) trfs,
              (SELECT cgc, count(*) sm_cnt
                 FROM historian.sm_sp_load_hist
                WHERE batch_date = v_batch_date
                GROUP BY cgc) sm_trfs
           WHERE cgc_id = cgc
             AND mtr_cnt = sm_cnt
       --      AND ROWNUM < 1001  --- added 11/19/19 for unit testing
             AND substr(id, -1,1) = i_thread;
   ELSIF i_variant = 'LEGACY' THEN
      OPEN get_trfs FOR
         SELECT id
           FROM
              (SELECT t.id, t.cgc_id, count(m.id) mtr_cnt
                 FROM transformer t, meter m
                WHERE nvl(t.rec_status, 'i')<> 'D'
                  AND nvl(m.rec_status, 'i') <> 'D'
                  AND t.id = m.trf_id
                GROUP BY t.id, t.cgc_id  ) trfs,
              (SELECT cgc, count(*) sm_cnt
                 FROM historian.sm_sp_load_hist
                WHERE batch_date = v_batch_date
                GROUP BY cgc) sm_trfs
           WHERE cgc_id = cgc (+)
             AND mtr_cnt <> nvl(sm_cnt, 0)
        --     AND ROWNUM < 1001  --- added 11/19/19 for unit testing
             AND substr(id, -1,1) = i_thread;
   ELSE -- i_variant is ALL
      OPEN get_trfs FOR
         SELECT DISTINCT t.id
           FROM transformer t, meter m
          WHERE nvl(t.rec_status, 'i')<> 'D'
            AND nvl(m.rec_status, 'i') <> 'D'
            AND t.id = m.trf_id
       --      AND ROWNUM < 11  --- added 11/19/19 for unit testing
            AND substr(t.id, -1,1) = i_thread ;
   END IF;

   v_count := 0;
--   OPEN get_trfs;
   LOOP
      FETCH get_trfs INTO v_trf_id;
      EXIT WHEN get_trfs%NOTFOUND;
         recalc_trf_loading(v_trf_id, v_batch_date, v_season, o_errormsg, o_errorcode);
         COMMIT;
         v_count := v_count +1;
   END LOOP;

CLOSE get_trfs;
   UPDATE monthly_load_log SET num_records_processed = v_count, load_end_ts = sysdate
    WHERE id = v_log_id;

EXCEPTION
WHEN invalid_cdw_data THEN
   RAISE_APPLICATION_ERROR(-20001,'Data loaded into historian.sm_trf_load_hist appears to be missing or incorrect for batch_date: '|| to_char(v_batch_date));
WHEN invalid_ccb_data THEN
   RAISE_APPLICATION_ERROR(-20001,'Data loaded into historian.ccb_meter_load_hist appears to be missing or incorrect for batch_date: '|| to_char(v_batch_date));
WHEN invalid_variant THEN
   RAISE_APPLICATION_ERROR(-20001,'The variant parameter: ' || i_variant || ' is invalid, allowable values are: SMART, LEGACY, or ALL');
WHEN invalid_thread THEN
   RAISE_APPLICATION_ERROR(-20001,'The thread parameter: ' || i_thread || ' is invalid, allowable values are: 0 thru 9');
END Monthly_TLM_run;



PROCEDURE Monthly_run_3 ( i_batch_date DATE, i_thread varchar2)
AS

CURSOR get_trfs
IS
  SELECT DISTINCT t.id FROM transformer t, meter m
   WHERE nvl(t.rec_status, 'i')<> 'D'  --AND id IN (10000011062); --,220024646,420009589);
     AND nvl(m.rec_status, 'i') <> 'D'
     AND t.id = m.trf_id
     AND substr(t.id, -1,1) = i_thread ;
/* these are the 18 unit test trfs
AND id IN (520003119, 120073754,120001920,920049802,520008369,120027693,520045830,
            120017952,520018282,520012085,220070557,220074263,220060351,120027518,
            1820043441,220056334,220007775,2020011746)
*/
--AND ROWNUM < 10000001;

   v_count NUMBER := 0;
   v_log_id NUMBER;
   v_trf_id transformer.id%TYPE;
   O_ERRORMSG VARCHAR2(32000);
   O_ERRORCODE VARCHAR2(32000);
   v_season varchar2(4) := 'S';

BEGIN
  IF to_char(i_batch_date, 'mm') IN ('11', '12','01','02', '03') THEN
     v_season := 'W';
  END IF;
  v_log_id := monthly_load_log_seq.NEXTVAL;
--  DELETE from monthly_load_log;
  INSERT INTO monthly_load_log (id, table_name, load_start_ts, num_records_processed, create_dtm)
    VALUES (v_log_id, 'Monthly run ' || v_log_id, SYSDATE, 0, SYSDATE);
  OPEN get_trfs;
  LOOP
   FETCH get_trfs INTO v_trf_id;
   EXIT WHEN get_trfs%NOTFOUND;
      recalc_trf_loading(v_trf_id, i_batch_date, v_season, o_errormsg, o_errorcode);
      COMMIT;
   v_count := v_count +1;
--   IF mod(v_count,100000) = 0 THEN
--     UPDATE monthly_load_log SET num_records_processed = v_count
--       WHERE id = v_log_id;
--   END IF;
END LOOP;
CLOSE get_trfs;
  UPDATE monthly_load_log SET num_records_processed = v_count, load_end_ts = sysdate
   WHERE id = v_log_id;

END Monthly_run_3;


PROCEDURE Recalc_trf_loading
  ( i_trf_id NUMBER, i_batch_date DATE, i_season VARCHAR2,
    o_ErrorMsg OUT VARCHAR2, o_ErrorCode OUT VARCHAR2 )
/*
Purpose:  Recalculate the loading data for the input transformer for the input month.


   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        06/21/19    Initial coding
   TJJ4        07/26/19    adding Generation
   TJJ4        10/23/19    adding error checking batch_date must be between
                           sysdate - 13 months and sysdate - 1 month
                           if it's a valid batch_date and there are NO records
                           in ccb_meter_load_hist, there were no spids at that
                           time, so no history should exist, if history records
                           do exist, delete them and re-evaluate the peak

*/

AS
   v_routine   VARCHAR2(64) := 'TLT.Recalc_trf_load';
   v_hint      INTEGER := 0;

   v_count           NUMBER;
   v_cust_count      NUMBER;
   v_gh_count        NUMBER;
   v_gen_count       NUMBER;
   v_trf_hist_id     trf_peak_hist.id%TYPE;
   v_trf_gen_hist_id trf_peak_gen_hist.id%TYPE;
   v_status          NUMBER := 0;
   v_indet_status    VARCHAR2(5);

BEGIN

   o_ErrorMsg     := 'Execution Successful';  -- be positive, assume success :D
   o_ErrorCode    := '0';

  -- is this a valid batch_month
  IF i_batch_date < ADD_MONTHS(sysdate,-16) OR i_batch_date > ADD_MONTHS(sysdate,-1) THEN
     GOTO message_handler;  -- this is just a failsafe, calling procedures are expected to do this
  END IF;
  -- at least 1 customer on this trf, must have usage/demand data for this
  -- batch month, when re-evaluating a trf for a historic month, we don't want
  -- to have a history record with 0 usage because none of the service points
  -- were on the trf yet.

  -- need a solution for spids that WERE on the trf that month but are no longer...
  -- Update: "solution" for this is in part, to execute check_for_indeterminate_status
  --         if the loading can not be calculated, either because of a trf/spid
  --         relationship error (addressing the problem above) or any other reason
  --         any existing history will be retained and the undetermined column
  --         will be updated to indicate the recalculation failed because of the
  --         undetermined status

  -- check the trf, can we calculate the loading for the month?
   v_indet_status := Check_for_indeterminate_status(i_trf_id, i_batch_date);

  -- to calculate the loading, there must be some customers on the trf in the month
   SELECT count(*)
      INTO v_cust_count
      FROM historian.sm_sp_load_hist h, meter m
     WHERE h.batch_date = i_batch_date
       AND m.trf_id = i_trf_id
       AND h.service_point_id = m.service_point_id
       AND nvl(m.rec_status, 'i') <> 'D';
   IF v_cust_count = 0 THEN -- check the ccb table
      SELECT count(*)
        INTO v_cust_count
        FROM historian.ccb_meter_load_hist h, meter m
       WHERE h.batch_date = i_batch_date
         AND m.trf_id = i_trf_id
         AND h.service_point_id = m.service_point_id
         AND nvl(m.rec_status, 'i') <> 'D';
   END IF;

-- is there already history for this trf/month?
   SELECT COUNT(*), MIN(id)
     INTO v_count, v_trf_hist_id
     FROM trf_peak_hist
    WHERE batch_date = i_batch_date
      AND trf_id = i_trf_id;
/*
v_count   v_cust_count  v_indet_status  do
   0           0           any          leave the procedure there's nothing to do
   0         not 0         any          create a trf_peak_hist record
   1           0            0           remove ALL the existing history records
   1           0          not 0         keep ALL the existing history records,
                                        update load_undetermined
   1         not 0          0           remove the children
   1         not 0        not 0         keep ALL the existing history records,
                                        update load_undetermined
*/


   IF ((v_count = 0) AND (v_cust_count = 0)) THEN -- no hist and no customer to compute loading for
      GOTO message_handler;
   ELSIF ((v_count = 0) AND (v_cust_count <> 0)) THEN  -- the transformer has no history record, create one
      v_hint := 10;
      v_trf_hist_id := trf_peak_hist_seq.NEXTVAL;
      INSERT INTO trf_peak_hist
             (id, trf_id, batch_date, trf_peak_time, trf_peak_kva, load_undetermined)
      VALUES (v_trf_hist_id, i_trf_id, i_batch_date, i_batch_date, 0,
              decode(v_indet_status,'0', NULL, v_indet_status));
   ELSE  -- there is some history
      IF v_indet_status <> '0' THEN -- current trf configuration will not allow successful recalculation
         v_hint := 20;
         v_indet_status := '1'||v_indet_status; -- update the code to indicate an error on recalculation
-- code below will do this update
--         UPDATE trf_peak_hist SET load_undetermined = v_indet_status WHERE id = v_trf_hist_id;
      ELSE
         v_hint := 25;
      -- delete the children as the next procedure will recreate them
         DELETE FROM sp_peak_hist WHERE trf_peak_hist_id = v_trf_hist_id;
         DELETE FROM trf_bank_peak_hist WHERE trf_peak_hist_id = v_trf_hist_id;
         IF v_cust_count <> 0 THEN
            UPDATE trf_peak_hist SET load_undetermined = NULL
             WHERE id = v_trf_hist_id
               AND load_undetermined IS NOT NULL;
         ELSE
            DELETE trf_peak_hist WHERE id = v_trf_hist_id;
         END IF;
      END IF;
   END IF;

   v_hint := 30;
   SELECT COUNT(*)  -- does this trf have generation data from sm?
     INTO v_gen_count
     FROM historian.sm_trf_gen_load_hist h, transformer t
    WHERE t.id =  i_trf_id
      AND h.cgc = t.cgc_id
      AND h.batch_date = i_batch_date;
   SELECT COUNT(*), MIN(id)  -- did we already create a parent record for it
     INTO v_gh_count, v_trf_gen_hist_id
     FROM trf_peak_gen_hist
    WHERE batch_date = i_batch_date
      AND trf_id = i_trf_id;
   IF ((v_gh_count = 1) AND (v_indet_status = '0')) THEN -- there's existing hist data, and we can recalc
      v_hint := 40;
      DELETE FROM sp_peak_gen_hist WHERE trf_peak_gen_hist_id = v_trf_gen_hist_id;
      DELETE FROM trf_bank_peak_gen_hist WHERE trf_peak_gen_hist_id = v_trf_gen_hist_id;
      IF v_gen_count = 0 THEN -- there's no generation on the trf in sm delete the parent too
         DELETE FROM trf_peak_gen_hist WHERE id = v_trf_gen_hist_id;
      ELSE
         UPDATE trf_peak_gen_hist SET load_undetermined = NULL
          WHERE id = v_trf_gen_hist_id
            AND load_undetermined IS NOT NULL;
      END IF;
   END IF;
   IF ((v_gen_count = 1) AND (v_gh_count = 0)) THEN -- newly added generation
      v_hint := 50;
      v_trf_gen_hist_id := trf_peak_gen_hist_seq.NEXTVAL;
      INSERT INTO trf_peak_gen_hist
             (id, trf_id, batch_date, trf_peak_time, trf_peak_kva)
      VALUES (v_trf_gen_hist_id, i_trf_id, i_batch_date, i_batch_date, 0);
   END IF;

-- 10/24/19  moved this BEFORE the deletion/creation of history records ...
--   v_indet_status := Check_for_indeterminate_status(i_trf_id, i_batch_date);
   IF ((v_indet_status = '0') AND (v_cust_count >0 )) THEN
      v_status := Calc_customer_kva(i_trf_id, v_trf_hist_id, i_batch_date, i_season, 'DELIVERED');
      IF v_status <> 0 THEN GOTO message_handler;
      END IF;
      IF v_trf_gen_hist_id IS NOT NULL THEN
         v_status := Calc_customer_kva(i_trf_id, v_trf_gen_hist_id, i_batch_date, i_season, 'RECEIVED');
         IF v_status <> 0 THEN GOTO message_handler;
         END IF;
      END IF;
      v_status := Update_trf_peak_hist(i_trf_id, v_trf_hist_id, i_batch_date);
      IF v_status <> 0 THEN GOTO message_handler;
      END IF;
      IF v_trf_gen_hist_id IS NOT NULL THEN
         v_status := Update_trf_peak_gen_hist(i_trf_id, v_trf_gen_hist_id, i_batch_date);
         IF v_status <> 0 THEN GOTO message_handler;
         END IF;
      END IF;
      v_status := Calc_Trf_bank_data(i_trf_id, v_trf_hist_id, v_trf_gen_hist_id, i_batch_date, i_season);
      IF v_status <> 0 THEN GOTO message_handler;
      END IF;
      v_status := Establish_Seasonal_Peak(i_trf_id, i_season, i_batch_date);
      IF v_status <> 0 THEN GOTO message_handler;
      END IF;
   ELSIF v_indet_status = '-1' THEN -- an error condition in the function
      v_status := -1;
   ELSIF v_indet_status <> '0' THEN
      UPDATE trf_peak_hist SET load_undetermined = v_indet_status
      WHERE id = v_trf_hist_id;
      Update_trf_peak_undetermined (i_trf_id, i_batch_date, i_season, 'DELIVERED', v_indet_status);
      IF v_trf_gen_hist_id IS NOT NULL THEN
         UPDATE trf_peak_gen_hist SET load_undetermined = v_indet_status
          WHERE id = v_trf_gen_hist_id;
         Update_trf_peak_undetermined (i_trf_id, i_batch_date, i_season, 'RECEIVED', v_indet_status);
      END IF;
   END IF;

<<MESSAGE_HANDLER>>
IF v_status <> 0 THEN
   o_ErrorCode := v_status;
   o_ErrorMsg := transformer_loading_tools.e_saved_message;

   INSERT INTO tlm_errors (rec_id, rec_type, batch_date, error_CODE, error_msg, error_date)
      VALUES (i_trf_id, 'TRF', i_batch_date,o_ErrorCode, o_ErrorMsg, SYSDATE );
   COMMIT;
END IF;

EXCEPTION
   WHEN OTHERS
      THEN
         o_ErrorMsg := v_routine || ' at hint ' || v_hint || ' ' || SQLERRM(SQLCODE);
         o_ErrorCode := SQLCODE;
END Recalc_trf_loading;

PROCEDURE Populate_CCB_data (i_refresh_ext_ml_tbl VARCHAR2, i_batch_date DATE)
AS
/*
 Purpose:  Populate the CCB data.

           If i_refresh_ext_ml_tbl is not No, pull the meter_load data from EDGIS.

           If i_batch_date is null, set batch_date to the most current date available
           in ext_ccb_meter_load. (ext_ccb_meter_load should contain 12 months of data)

           PGEDATA.PGE_EXT_CCB_METER_LOAD@EDGIS --> EXT_CCB_METER_LOAD
           EXT_CCB_METER_LOAD                   --> HISTORIAN.CCB_METER_LOAD_HIST

           refresh statistics on the tables we touch

           Note: ccb delivers the data with an implied decimal in the 10ths place,
                 ext_ccb_meter_load keeps the implied decimal, but when we
                 move the data to historian we put the decimal back
                 (rev_kwhr/10)

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        09/10/19    Initial coding
   TJJ4        09/25/19    added the calls to compute statistics

*/
   invalid_ccb_data    EXCEPTION;
   incomplete_ccb_data EXCEPTION;

   v_batch_date trf_peak_hist.batch_date%TYPE;

   v_count      NUMBER := 0;
   v_log_status VARCHAR2(10);
   v_RowCount   NUMBER := 0;

BEGIN
   delete from MONTHLY_LOAD_LOG where table_name='Populate_CCB_data';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Populate_CCB_data');

   IF upper(nvl(i_refresh_ext_ml_tbl,'T')) NOT IN ('N', 'NO', 'F', 'FALSE') THEN
      SELECT count(*)
        INTO v_count
        FROM pgedata.pge_ext_ccb_meter_load@edgis;
      IF v_count < 66720000 THEN
         RAISE invalid_ccb_data;
      else
         execute immediate 'TRUNCATE table ext_ccb_meter_load';
         INSERT INTO ext_ccb_meter_load
               (service_point_id, unqspid, acct_id, rev_month, rev_kwhr, rev_kw,
                pfactor, sm_sp_status, rev_year)
         SELECT service_point_id, unqspid, acct_id, rev_month, rev_kwhr, rev_kw,
                pfactor, sm_sp_status, rev_year
           FROM pgedata.pge_ext_ccb_meter_load@edgis;   -- 4 mins in q6q
         v_RowCount := v_RowCount + SQL%ROWCOUNT;
         dbms_stats.gather_table_stats('EDTLM', 'ext_ccb_meter_load');
      END IF;
   END IF;

   IF i_batch_date IS NULL THEN
      WITH
      DATA AS
        (SELECT rev_year, rev_month, count(*) row_cnt,
                row_number() over (ORDER BY rev_year DESC, rev_month DESC) rk
           FROM ext_ccb_meter_load
          GROUP BY rev_year, rev_month )
      SELECT max(to_date(rev_year||rev_month||'01', 'yyyymmdd')) batch_date, nvl(sum(row_cnt),0)
      -- note: max and sum ensure we get a row even if the table is empty
        INTO v_batch_date, v_count
        FROM DATA
       WHERE rk = 1;
   ELSE
      v_batch_date := i_batch_date;
      SELECT count(*)
        INTO v_count
        FROM ext_ccb_meter_load
       WHERE to_date(rev_year||rev_month||'01', 'yyyymmdd') = v_batch_date;
   END IF;

   IF v_count < 5560000 THEN
      RAISE incomplete_ccb_data;
   ELSE
      DELETE FROM historian.ccb_meter_load_hist WHERE batch_date = v_batch_date;
      v_RowCount := v_RowCount + SQL%ROWCOUNT;
      INSERT INTO historian.ccb_meter_load_hist
            (batch_date, service_point_id, unqspid, acct_id, rev_kwhr, rev_kw,
             pfactor, sm_sp_status, rev_month, rev_year)
      SELECT v_batch_date, service_point_id, unqspid, acct_id, rev_kwhr/10, rev_kw,
             pfactor, sm_sp_status, rev_month, rev_year
        FROM ext_ccb_meter_load
       WHERE to_date(rev_year||rev_month||'01', 'yyyymmdd') = v_batch_date;
      v_RowCount := v_RowCount + SQL%ROWCOUNT;
      historian.gather_my_stats('CCB');
   END IF;
   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Populate_CCB_data',v_RowCount);
EXCEPTION
WHEN invalid_ccb_data THEN
   v_log_status := LOG_MONTHLY_LOAD_ERROR('Populate_CCB_data','invalid_ccb_data');
   RAISE_APPLICATION_ERROR(-20001,'EDGIS pgedata.pge_ext_ccb_meter_load is too small, need 12 months of 5560000 or 66720000 records');
WHEN incomplete_ccb_data THEN
   v_log_status := LOG_MONTHLY_LOAD_ERROR('Populate_CCB_data','Missing CCB records');
   RAISE_APPLICATION_ERROR(-20001,'ext_ccb_meter_load data missing records for batch_date: '|| to_char(v_batch_date, 'dd-mon-yyyy') || ' expecting at least 5560000 records');
END Populate_CCB_data;

PROCEDURE Populate_CDW_data (i_batch_date DATE)
AS
/*
 Purpose:  Populate the CDW data in Historian using the ext_xxx tables

           EXT_SM_TRF_LOAD      -->  HISTORIAN.SM_TRF_LOAD_HIST
           EXT_SM_TRF_GEN_LOAD  -->  HISTORIAN.SM_TRF_GEN_LOAD_HIST
           EXT_SM_SP_LOAD       -->  HISTORIAN.SM_SP_LOAD_HIST
           EXT_SM_SP_GEN_LOAD   -->  HISTORIAN.SM_SP_GEN_LOAD_HIST

           If i_batch_date is null load whatever data is in EDTLM's EXT tables.
           If the data exists in historian delete it first.

           if i_batch_date is not null load whatever the caller wants, deleting any
           old copy of the i_batch_date first.

   ******************** MODIFICATION HISTORY *********************

   Person      Date        Comments
   ---------   --------    -------------------------------------------
   TJJ4        09/09/19    Initial coding
   TJJ4        09/17/19    added a minimum record check for rest of sm tables
   TJJ4        09/28/19    added call to calculate statistics
*/
   invalid_cdw_data EXCEPTION;

   v_batch_date trf_peak_hist.batch_date%TYPE;

   v_count      NUMBER := 0;
   v_min_date   VARCHAR2(6);
   v_max_date   VARCHAR2(6);
   v_tab        VARCHAR2(50) := 'ext_sm_trf_load';

   v_log_status VARCHAR2(10);
   v_RowCount   NUMBER := 0;

BEGIN
   delete from MONTHLY_LOAD_LOG where table_name='Populate_CDW_data';
   v_log_status := INSERT_MONTHLY_LOAD_LOG('Populate_CDW_data');
-- excluding the subset of records that peaked at midnight at the end of the month,
-- determine what years and months are in the table. Expecting 1 month to load
   SELECT min(substr(trf_peak_time, 1,6)), max(substr(trf_peak_time, 1, 6)), count(*)
     INTO v_min_date, v_max_date, v_count
     FROM ext_sm_trf_load
    WHERE trf_peak_time NOT LIKE '%01:00:00';
-- the code is assuming if ext_sm_trf_load has the right amount of data
-- for the specified batch_month, the other tables were properly loaded too
-- it's expected that the process that loads these tables checks for success
   IF (v_min_date = v_max_date) AND (v_count > 800000) THEN
      IF i_batch_date IS NULL THEN
         v_batch_date := to_date(v_min_date||'01', 'yyyymmdd');
      ELSIF (to_char(i_batch_date, 'yyyymm') = v_min_date) THEN
         v_batch_date := to_date(v_min_date||'01', 'yyyymmdd');
      ELSE
         RAISE invalid_cdw_data;
      END IF;
   ELSE
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_trf_gen_load
    WHERE trf_peak_time NOT LIKE v_min_date||'%'
      AND trf_peak_time NOT LIKE '%01:00:00';
   IF v_count <> 0 THEN
      v_tab := 'ext_sm_trf_gen_load';
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_trf_gen_load
    WHERE trf_peak_time LIKE v_min_date||'%'
      AND trf_peak_time NOT LIKE '%01:00:00';
   IF v_count < 200000 THEN
      v_tab := 'ext_sm_trf_gen_load';
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_sp_load
    WHERE sp_peak_time NOT LIKE v_min_date||'%'
      AND sp_peak_time NOT LIKE '%01:00:00';
   IF v_count <> 0 THEN
      v_tab := 'ext_sm_sp_load';
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_sp_load
    WHERE sp_peak_time LIKE v_min_date||'%'
      AND sp_peak_time NOT LIKE '%01:00:00';
   IF v_count < 5300000 THEN
      v_tab := 'ext_sm_sp_load';
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_sp_gen_load
    WHERE sp_peak_time NOT LIKE v_min_date||'%'
      AND sp_peak_time NOT LIKE '%01:00:00';
   IF v_count <> 0 THEN
      v_tab := 'ext_sm_sp_gen_load';
      RAISE invalid_cdw_data;
   END IF;

   SELECT count(*)
     INTO v_count
     FROM ext_sm_sp_gen_load
    WHERE sp_peak_time LIKE v_min_date||'%'
      AND sp_peak_time NOT LIKE '%01:00:00';
   IF v_count < 350000 THEN
      v_tab := 'ext_sm_sp_gen_load';
      RAISE invalid_cdw_data;
   END IF;

   DELETE FROM HISTORIAN.SM_SP_GEN_LOAD_HIST  WHERE batch_date = v_batch_date;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;
   DELETE FROM HISTORIAN.SM_SP_LOAD_HIST      WHERE batch_date = v_batch_date;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;
   DELETE FROM HISTORIAN.SM_TRF_GEN_LOAD_HIST WHERE batch_date = v_batch_date;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;
   DELETE FROM HISTORIAN.SM_TRF_LOAD_HIST     WHERE batch_date = v_batch_date;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;

   INSERT INTO historian.sm_trf_load_hist
         (cgc, batch_date, trf_peak_kw, trf_peak_time, trf_avg_kw, create_date)
   SELECT cgc, v_batch_date, trf_peak_kw,
          decode(trf_peak_time, to_char(ADD_months(v_batch_date,1),'YYYYMMDD:HH24:MI'),
          to_date(trf_peak_time,'YYYYMMDD:HH24:MI') - INTERVAL '1' SECOND,
          to_date(trf_peak_time,'YYYYMMDD:HH24:MI')) pk_dt, trf_avg_kw, to_char(SYSDATE,'yymmdd')
     FROM ext_sm_trf_load;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;

   INSERT INTO historian.sm_trf_gen_load_hist
         (cgc, batch_date, trf_peak_kw, trf_peak_time, trf_avg_kw, create_date)
   SELECT cgc, v_batch_date, trf_peak_kw,
          decode(trf_peak_time, to_char(ADD_months(v_batch_date,1),'YYYYMMDD:HH24:MI'),
          to_date(trf_peak_time,'YYYYMMDD:HH24:MI') - INTERVAL '1' SECOND,
          to_date(trf_peak_time,'YYYYMMDD:HH24:MI')) pk_dt, trf_avg_kw, to_char(SYSDATE,'yymmdd')
     FROM ext_sm_trf_gen_load;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;

   INSERT INTO historian.sm_sp_load_hist
          (cgc, service_point_id, batch_date, sp_peak_kw, vee_sp_kw_flag,
           sp_peak_time, sp_kw_trf_peak, vee_trf_kw_flag, int_len, sp_peak_kvar,
           trf_peak_kvar, create_date)
    SELECT cgc, service_point_id, v_batch_date, sp_peak_kw, vee_sp_kw_flag,
           decode(sp_peak_time, to_char(ADD_months(v_batch_date,1),'YYYYMMDD:HH24:MI'),
                  to_date(sp_peak_time,'YYYYMMDD:HH24:MI') - INTERVAL '1' SECOND,
                  to_date(sp_peak_time,'YYYYMMDD:HH24:MI')),
           sp_kw_trf_peak, vee_trf_kw_flag, int_len, sp_peak_kvar, trf_peak_kvar,
           to_char(SYSDATE,'yymmdd')
      FROM ext_sm_sp_load;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;

   INSERT INTO historian.sm_sp_gen_load_hist
          (cgc, service_point_id, batch_date, sp_peak_kw, vee_sp_kw_flag,
           sp_peak_time, sp_kw_trf_peak, vee_trf_kw_flag, int_len, sp_peak_kvar,
           trf_peak_kvar, create_date)
    SELECT cgc, service_point_id, v_batch_date, sp_peak_kw, vee_sp_kw_flag,
           decode(sp_peak_time, to_char(ADD_months(v_batch_date,1),'YYYYMMDD:HH24:MI'),
                  to_date(sp_peak_time,'YYYYMMDD:HH24:MI') - INTERVAL '1' SECOND,
                  to_date(sp_peak_time,'YYYYMMDD:HH24:MI')),
           sp_kw_trf_peak, vee_trf_kw_flag, int_len, sp_peak_kvar, trf_peak_kvar,
           to_char(SYSDATE,'yymmdd')
      FROM ext_sm_sp_gen_load;
   v_RowCount := v_RowCount + SQL%ROWCOUNT;

   historian.gather_my_stats('CDW');

   execute immediate 'TRUNCATE table ext_sm_sp_gen_load';
   execute immediate 'TRUNCATE table ext_sm_sp_load';
   execute immediate 'TRUNCATE table ext_sm_trf_gen_load';
   execute immediate 'TRUNCATE table ext_sm_trf_load';
   v_log_status := LOG_MONTHLY_LOAD_SUCCESS('Populate_CDW_data',v_RowCount);

EXCEPTION
WHEN invalid_cdw_data THEN
   v_log_status := LOG_MONTHLY_LOAD_ERROR('Populate_CDW_data','invalid_cdw_data');
   RAISE_APPLICATION_ERROR(-20001,'Data loaded into '||v_tab|| ' is incorrect\incomplete. May not match the passed batch_date or may have too many months');
END Populate_CDW_data;

END Transformer_loading_Tools;
/
