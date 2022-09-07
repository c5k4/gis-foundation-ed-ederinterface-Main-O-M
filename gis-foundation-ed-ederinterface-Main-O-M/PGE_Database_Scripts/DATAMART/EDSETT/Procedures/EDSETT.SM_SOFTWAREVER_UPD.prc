Prompt drop Procedure SM_SOFTWAREVER_UPD;
DROP PROCEDURE EDSETT.SM_SOFTWAREVER_UPD
/

Prompt Procedure SM_SOFTWAREVER_UPD;
--
-- SM_SOFTWAREVER_UPD  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT.SM_SOFTWAREVER_UPD

AS

BEGIN


BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_SECTIONALIZER CD ,SM_SECTIONALIZER RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_SECTIONALIZER  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID  AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_RECLOSER CD ,SM_RECLOSER RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_RECLOSER  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;


BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_SWITCH CD ,SM_SWITCH RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_SWITCH  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;



BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_INTERRUPTER CD ,SM_INTERRUPTER RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_INTERRUPTER  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID  AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;


BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_CAPACITOR CD ,SM_CAPACITOR RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_CAPACITOR  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;



BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SOFTWARE_VERSION  FROM CEDSA_REGULATOR_BANK CD ,SM_REGULATOR RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_REGULATOR  SET  SOFTWARE_VERSION = I.SOFTWARE_VERSION   WHERE DEVICE_ID =I.DEVICE_ID AND SOFTWARE_VERSION IS NULL;
END LOOP;
END;

COMMIT;

END SM_SOFTWAREVER_UPD;
/