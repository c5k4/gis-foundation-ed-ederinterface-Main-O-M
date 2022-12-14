Prompt drop Procedure SP_RTU_NOTES_SPECIALCOND_UPD;
DROP PROCEDURE EDSETT.SP_RTU_NOTES_SPECIALCOND_UPD
/

Prompt Procedure SP_RTU_NOTES_SPECIALCOND_UPD;
--
-- SP_RTU_NOTES_SPECIALCOND_UPD  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_RTU_NOTES_SPECIALCOND_UPD"
AS
BEGIN



--- Get the CEDSA data for DEVICE & REMOTE_TERM_UNIT for migratinng data for NOTES and RTU fields
execute IMMEDIATE 'TRUNCATE TABLE CEDSA_DEVICE_TEMP' ;
execute IMMEDIATE 'TRUNCATE TABLE CEDSA_REMOTE_TERM_UNIT';
execute IMMEDIATE 'TRUNCATE TABLE CEDSA_STRUCTURE';

INSERT INTO CEDSA_DEVICE_TEMP  SELECT * FROM  EDSETTCEDSA.DEVICE;
INSERT INTO CEDSA_REMOTE_TERM_UNIT  SELECT * FROM  EDSETTCEDSA.REMOTE_TERM_UNIT;
INSERT INTO CEDSA_STRUCTURE  SELECT * FROM  EDSETTCEDSA.STRUCTURE;


--- Data Migration for Relcoser field - MULTI_FUNCTIONAL
BEGIN
FOR I IN (SELECT  RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.MULTI_FUNCTIONAL  FROM CEDSA_RECLOSER  CD ,SM_RECLOSER RE WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_RECLOSER  SET
	MULTI_FUNCTIONAL=I.MULTI_FUNCTIONAL
WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE ;

END LOOP;
END;

UPDATE SM_RECLOSER  SET  MULTI_FUNCTIONAL='Y' WHERE  MULTI_FUNCTIONAL='1';
UPDATE SM_RECLOSER  SET  MULTI_FUNCTIONAL='N' WHERE  MULTI_FUNCTIONAL='0';



--- Data Migration for Regulator fields - SEASON_OFF,SWITCH_POSITION,EMERGENCY_ONLY

BEGIN
FOR I IN (SELECT RE.DEVICE_ID,CD.SEASON_OFF,CD.SWITCH_POSITION,CD.EMERGENCY_ONLY ,RE.CURRENT_FUTURE FROM CEDSA_REGULATOR CD,SM_REGULATOR RE WHERE
CD.DEVICE_ID = RE.DEVICE_ID)
LOOP

UPDATE SM_REGULATOR SET
	SEASON_OFF=I.SEASON_OFF
	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

UPDATE SM_REGULATOR SET
	SWITCH_POSITION=I.SWITCH_POSITION
	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

UPDATE SM_REGULATOR SET
	EMERGENCY_ONLY=I.EMERGENCY_ONLY
	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;


END LOOP;
END;



--- Data Migration for Sectionaliser - FLISR


BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.FLISR  FROM CEDSA_SCADA CD ,SM_SECTIONALIZER RE WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_SECTIONALIZER SET  FLISR=I.FLISR  WHERE DEVICE_ID =I.DEVICE_ID;

END LOOP;
END;

UPDATE SM_SECTIONALIZER  SET  FLISR='Y' WHERE  FLISR='1';
UPDATE SM_SECTIONALIZER  SET  FLISR='N' WHERE  FLISR='0';

--- RTU Update for Sectionalizer

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION,CD.MANF_CD    FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_SECTIONALIZER SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_SECTIONALIZER  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;

UPDATE  SM_SECTIONALIZER
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_SECTIONALIZER
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;





--- RTU Update for Capacitor

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION,CD.MANF_CD   FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_CAPACITOR SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_CAPACITOR  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD =I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;



UPDATE  SM_CAPACITOR
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_CAPACITOR
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;




--- RTU Update for Recloser

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION,CD.MANF_CD   FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_RECLOSER SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_RECLOSER  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;



UPDATE  SM_RECLOSER
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_RECLOSER
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;



--- RTU Update for Interrupter

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION,CD.MANF_CD   FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_INTERRUPTER SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_INTERRUPTER  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;

UPDATE  SM_INTERRUPTER
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_INTERRUPTER
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;



--- RTU Update for Switch

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION,CD.MANF_CD   FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_SWITCH SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_SWITCH SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;


UPDATE  SM_SWITCH
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_SWITCH
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;




--- RTU Update for Regulator

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION ,CD.MANF_CD  FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_REGULATOR SE   WHERE
CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_REGULATOR  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;



UPDATE  SM_REGULATOR
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_REGULATOR
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;



--- RTU Update for Circuit Breaker

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.MODEL_#,CD.SERIAL_#,CD.FIRMWARE_VERSION ,CD.SOFTWARE_VERSION ,CD.MANF_CD  FROM CEDSA_REMOTE_TERM_UNIT  CD ,SM_CIRCUIT_BREAKER SE
WHERE CD.DEVICE_ID =SE.DEVICE_ID )
LOOP
UPDATE SM_CIRCUIT_BREAKER  SET
RTU_MODEL_NUM=I.MODEL_#,RTU_SERIAL_NUM=I.SERIAL_#,RTU_FIRMWARE_VERSION=I.FIRMWARE_VERSION,RTU_SOFTWARE_VERSION=I.SOFTWARE_VERSION,RTU_MANF_CD=I.MANF_CD
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;


UPDATE  SM_CIRCUIT_BREAKER
SET RTU_EXIST='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC ) ;

UPDATE  SM_CIRCUIT_BREAKER
SET RTU_EXIST='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_REMOTE_TERM_UNIT SC) ;





--- Data Migration  from NOTES to ENGINEERING_COMMENTS for all devices

BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_SECTIONALIZER RE
WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_SECTIONALIZER SET  ENGINEERING_COMMENTS=I.NOTES WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;


BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_INTERRUPTER RE
WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_INTERRUPTER SET  ENGINEERING_COMMENTS=I.NOTES WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;


BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_REGULATOR RE
WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_REGULATOR SET  ENGINEERING_COMMENTS=I.NOTES
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;


BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_SWITCH RE
WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_SWITCH SET  ENGINEERING_COMMENTS=I.NOTES
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;


BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_CAPACITOR RE
WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_CAPACITOR SET  ENGINEERING_COMMENTS=I.NOTES
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;



BEGIN
FOR I IN (SELECT RE.DEVICE_ID,RE.CURRENT_FUTURE,CD.NOTES FROM CEDSA_DEVICE_TEMP CD ,SM_RECLOSER RE WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP

UPDATE SM_RECLOSER SET  ENGINEERING_COMMENTS=I.NOTES
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;




--- Migrating the SPECIAL_CONDITIONS field data from CEDSA_STRUCTURE to all DEVICE tables


BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_CAPACITOR RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_CAPACITOR SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;


BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_RECLOSER RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_RECLOSER SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;



BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_REGULATOR RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_REGULATOR SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;



BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_SWITCH RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_SWITCH SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;



BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_INTERRUPTER RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_INTERRUPTER SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;




BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_SECTIONALIZER RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_SECTIONALIZER  SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;




BEGIN
FOR I IN (SELECT RES.DEVICE_ID,RES.CURRENT_FUTURE,CD.SPECIAL_CONDITIONS FROM CEDSA_STRUCTURE CD,
  (SELECT CT.DEVICE_ID,CT.STRUC_ID,RE.CURRENT_FUTURE FROM CEDSA_DEVICE_TEMP CT , SM_CIRCUIT_BREAKER RE  WHERE  CT.DEVICE_ID = RE.DEVICE_ID) RES
WHERE RES.STRUC_ID=CD.STRUC_ID)

LOOP

UPDATE SM_CIRCUIT_BREAKER  SET  SPECIAL_CONDITIONS=I.SPECIAL_CONDITIONS
 	WHERE DEVICE_ID =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;

END LOOP;
END;





COMMIT;
END  SP_RTU_NOTES_SPECIALCOND_UPD;

/
