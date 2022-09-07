--------------------------------------------------------
--  DDL for Procedure SP_ED06_POTENTIAL_SKIPPY
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "INTDATAARCH"."SP_ED06_POTENTIAL_SKIPPY" (startdate IN INTEGER,enddate IN INTEGER,retenDate IN INTEGER) 
AS
v_sqlQuery NVARCHAR2(10000);
v_Value INTEGER;
begin

Select Count(*) INTO v_Value from PGE_GISSAP_ASSETSYNCH  where (sysdate - processedtime) < 0.2 AND PROCESSEDFLAG = 'D';
DBMS_OUTPUT.PUT_LINE('ED06 RECORDS SHARED WITH SAP COUNT : ' || v_Value);

DBMS_OUTPUT.PUT_LINE('REPORT PERIOD : ' || to_Char(sysdate - startdate,'MM-DD-YYYY') || ' TO ' || to_Char(sysdate - enddate,'MM-DD-YYYY'));

-- Check Insert/ Update Changes
ED06_Potential_Skippy_IU(startdate,enddate);

-- Check Delete Changes
ED06_Potential_Skippy_D(startdate,enddate);

Select Count(*) INTO v_Value from INTDATAARCH.ED06_Potential_Skippy where REPROCESSED is null;
DBMS_OUTPUT.PUT_LINE('POTENTIAL SKIPPY RECORDS COUNT : ' || v_Value);

Select Count(*) INTO v_Value from INTDATAARCH.ED06_Potential_Skippy where REPROCESSED is null AND ACTION <> 'D';
DBMS_OUTPUT.PUT_LINE('POTENTIAL SKIPPY INSERT/UPDATE RECORDS COUNT : ' || v_Value);

Select Count(*) INTO v_Value from INTDATAARCH.ED06_Potential_Skippy where REPROCESSED is null AND ACTION = 'D';
DBMS_OUTPUT.PUT_LINE('POTENTIAL SKIPPY DELETE RECORDS COUNT : ' || v_Value);

Select Count(*) INTO v_Value from INTDATAARCH.ED06_Potential_Skippy where (FEAT_GLOBALID,CAPTURE_DATE) in 
(
Select FEAT_GLOBALID,CAPTURE_DATE from INTDATAARCH.ED06_Potential_Skippy group by FEAT_GLOBALID,CAPTURE_DATE having Count(*) > 1)
AND REPROCESSED is NULL;
DBMS_OUTPUT.PUT_LINE('POTENTIAL REPEATED SKIPPY RECORDS COUNT : ' || v_Value);

-- Assigning Duplicate Tags
UPDATE INTDATAARCH.ED06_Potential_Skippy SET REPROCESSED = 'D' where (FEAT_GLOBALID,CAPTURE_DATE) in 
(
Select FEAT_GLOBALID,CAPTURE_DATE from INTDATAARCH.ED06_Potential_Skippy group by FEAT_GLOBALID,CAPTURE_DATE having Count(*) > 1)
AND REPROCESSED is NULL;

COMMIT;

--Change NULL(New) - > 0 (In-Trans)
UPDATE INTDATAARCH.ED06_Potential_Skippy SET REPROCESSED = '0' where REPROCESSED is NULL;
COMMIT;

-- Reprocessing of Skippy Records
UPDATE INTDATAARCH.PGE_GDBM_AH_INFO SET ED06_PICKED = NULL WHERE (FEAT_GLOBALID, FEAT_CLASSNAME, CAPTURE_DATE) in
( select FEAT_GLOBALID, FEAT_CLASSNAME, CAPTURE_DATE FROM INTDATAARCH.ED06_POTENTIAL_SKIPPY where REPROCESSED = '0');
COMMIT;

Select Count(*) INTO v_Value from INTDATAARCH.ED06_Potential_Skippy where REPROCESSED = '0';
DBMS_OUTPUT.PUT_LINE('POTENTIAL SKIPPY SET FOR RE-PROCESSING RECORDS COUNT : ' || v_Value);
DBMS_OUTPUT.NEW_LINE;

--Change 0 - > 1 (Sent for Reprocess)
UPDATE INTDATAARCH.ED06_Potential_Skippy SET REPROCESSED = '1' where REPROCESSED = '0';
COMMIT;

--Truncate Old Data
v_sqlQuery := 'Delete from INTDATAARCH.ED06_Potential_Skippy where RUNDATE < sysdate - ' || retenDate || ' AND REPROCESSED <> ''D'' ';
    DBMS_OUTPUT.PUT_LINE('DELETED POTENTIAL SKIPPY RECORDS OLDER THAN ' || retenDate || ' DAYs');
    execute immediate v_sqlQuery;
    COMMIT;

end;
