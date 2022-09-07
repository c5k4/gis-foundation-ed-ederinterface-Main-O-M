spool "D:\Temp\SM_RECLOSER.txt"
set define off;
set escape on;
set serverout on;


DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_FAST_CRV' AND ACTIVE_PROFILE = 'SW';

DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_SLOW_CRV' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_TADD_FAST' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_TADD_SLOW' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_TMUL_FAST' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_TMUL_SLOW' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_FAST_CRV' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_SLOW_CRV' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_TADD_FAST' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_TADD_SLOW' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_TMUL_FAST' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_TMUL_SLOW' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'TCC1_FAST_CURVES_USED' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'TCC2_SLOW_CURVES_USED' AND ACTIVE_PROFILE = 'SW';

DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'MIN_RESP_TADDR_PHA' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'MIN_RESP_TADDR_GRO' AND ACTIVE_PROFILE = 'SW';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_OP_F_CRV' AND CONTROL_TYPE = 'BECK';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_OP_F_CRV' AND CONTROL_TYPE = 'BECK';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'GRD_SLOW_CRV_OPS' AND CONTROL_TYPE = 'BECK';
DELETE FROM EDSETT.SM_RECLOSER_REQUIRED WHERE FIELD_NAME = 'PHA_SLOW_CRV_OPS' AND CONTROL_TYPE = 'BECK';

UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'FIRST_RECLO_PHA' WHERE FIELD_NAME = '1ST_RECLO_PHA';
 UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'FIRST_RECLO_GRO' WHERE FIELD_NAME = '1ST_RECLO_GRO';
UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'SEC_RECLO_PHA' WHERE FIELD_NAME = '2ND_RECLO_PHA';
 UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'SEC_RECLO_GRO' WHERE FIELD_NAME = '2ND_RECLO_GRO';
UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'THIRD_RECLO_PHA' WHERE FIELD_NAME = '3RD_RECLO_PHA';
 UPDATE EDSETT.SM_RECLOSER_REQUIRED SET FIELD_NAME = 'THIRD_RECLO_GRO' WHERE FIELD_NAME = '3RD_RECLO_GRO';


ALTER TABLE EDSETT.SM_RECLOSER ADD COLD_PHA_TMUL_FAST Number(4,2);
ALTER TABLE EDSETT.SM_RECLOSER ADD COLD_GRD_TMUL_FAST Number(4,2);
ALTER TABLE EDSETT.SM_RECLOSER_HIST ADD COLD_PHA_TMUL_FAST Number(4,2);
ALTER TABLE EDSETT.SM_RECLOSER_HIST ADD COLD_GRD_TMUL_FAST Number(4,2);


COMMIT;


VARIABLE MAXID NUMBER;
VARIABLE MAXRANGEID NUMBER;

BEGIN
	SELECT MAX(ID) INTO :MAXRANGEID FROM sm_table_range_value;
	SELECT MAX(ID) INTO :MAXID FROM SM_RECLOSER_REQUIRED;
END;
/

Insert into sm_table_range_value values(:MAXRANGEID +1,'SM_RECLOSER','COLD_PHA_TMUL_FAST',0.1,25);
Insert into sm_table_range_value values(:MAXRANGEID +2,'SM_RECLOSER','COLD_GRD_TMUL_FAST',0.1,25);
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+1,'COLD_PHA_TMUL_FAST','N','BECK','','R1');
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+2,'COLD_GRD_TMUL_FAST','N','BECK','','R1');
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+3,'COLD_PHA_TMUL_FAST','N','BECK','','R2');
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+4,'COLD_GRD_TMUL_FAST','N','BECK','','R2');
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+5,'COLD_PHA_TMUL_FAST','N','BECK','','R3');
INSERT INTO EDSETT.SM_RECLOSER_REQUIRED(ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD,ACTIVE_PROFILE) values(:MAXID+6,'COLD_GRD_TMUL_FAST','N','BECK','','R3');										

commit;
set define on;
set escape off;
spool off;