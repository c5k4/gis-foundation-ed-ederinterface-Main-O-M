Prompt drop Procedure FLDCMP;
DROP PROCEDURE PGEDATA.FLDCMP
/

Prompt Procedure FLDCMP;
--
-- FLDCMP  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.FLDCMP(
    in_ID IN NUMBER DEFAULT 0)
AS
  v_diffbadge VARCHAR2(1);
  v_diffFix   VARCHAR2(1);
  v_diffAddr  VARCHAR2(1);
  v_diffMap   VARCHAR2(1);
  v_diffRS    VARCHAR2(1);
  v_diffIt    VARCHAR2(1);
  fname       VARCHAR2 (40);
  foo         NUMBER(10);
  CURSOR cmpCur
  IS
    SELECT s.badge_number sb,
      s.fixture_code sc ,
      s.descriptive_address sd,
      s.map_number sm ,
      s.rate_schedule sr ,
      s.item_type_code si,
      f.badge_number fb,
      f.fixture_code fc,
      f.descriptive_address fd,
      f.map_number fm,
      f.rate_schedule fr,
      f.item_type_code fi,
      s.sp_item_hist sh,
      f.transaction tr
    FROM slcdx_data s,
      fieldpts f
    WHERE s.sp_item_hist     =f.sp_item_hist
    AND to_number (f.gis_id) > in_ID;
BEGIN
  UPDATE fieldpts
  SET prem_id =
    (SELECT MIN(prem_id)
    FROM slcdx_data
    WHERE fieldpts.account_number=slcdx_data.account_number
    )
  WHERE EXISTS
    (SELECT *
    FROM slcdx_data
    WHERE fieldpts.account_number=slcdx_data.account_number
    )
  AND prem_id           = ' '
  AND to_number(gis_id) > in_ID;

  UPDATE fieldpts SET office = NULL WHERE office IN ('',' ');
  UPDATE fieldpts SET newbadge = NULL WHERE newbadge IN ('',' ');
  UPDATE fieldpts SET rate_schedule = NULL WHERE rate_schedule IN ('',' ');

  FOR foo IN cmpCur
  LOOP
    v_diffBadge   :=1;
    v_diffFix     :=1;
    v_diffAddr    :=1;
    v_diffMap     :=1;
    v_diffRS      :=1;
    v_diffIt      :=1;
    IF foo.fb     <> foo.sb THEN
      v_diffBadge :=2;
    END IF;
   -- UPDATE fieldpts SET diffBadge = v_diffBadge WHERE sp_item_hist=foo.sh;
    IF foo.fc   <> foo.sc THEN
      v_diffFix :=2;
    END IF;
  --  UPDATE fieldpts SET diffFix = v_diffFix WHERE sp_item_hist=foo.sh;
    IF foo.fd    <> foo.sd THEN
      v_diffAddr :=2;
    END IF;
  --  UPDATE fieldpts SET diffAddr = v_diffAddr WHERE sp_item_hist=foo.sh;
    IF foo.fm   <> foo.sm THEN
      v_diffMap :=2;
    END IF;
  --  UPDATE fieldpts SET diffMap = v_diffMap WHERE sp_item_hist=foo.sh;
    IF foo.fr  <> foo.sr THEN
      v_diffRs :=2;
    END IF;
  --  UPDATE fieldpts SET diffRs = v_diffRs WHERE sp_item_hist=foo.sh;
    IF foo.fi  <> foo.si THEN
      v_diffIT :=2;
    END IF;
    UPDATE fieldpts
    SET diffBadge = v_diffBadge,diffFix = v_diffFix,diffAddr = v_diffAddr,
    diffMap = v_diffMap,diffRs = v_diffRs,diffIt = v_diffIt
    WHERE sp_item_hist=foo.sh;

  END LOOP;
  UPDATE fieldpts SET diffbadge = 2 WHERE newbadge IS NOT NULL;
  COMMIT;
END;
/


Prompt Grants on PROCEDURE FLDCMP TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.FLDCMP TO GIS_I
/
