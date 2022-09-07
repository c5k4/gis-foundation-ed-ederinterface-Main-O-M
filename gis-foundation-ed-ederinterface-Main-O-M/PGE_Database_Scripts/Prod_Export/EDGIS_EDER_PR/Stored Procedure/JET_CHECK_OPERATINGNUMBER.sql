--------------------------------------------------------
--  DDL for Procedure JET_CHECK_OPERATINGNUMBER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."JET_CHECK_OPERATINGNUMBER" (P_OPERATINGNUMBER IN VARCHAR2, P_IS_OPNO_EXISTS OUT VARCHAR2)
AS

  v_count NUMBER;  
  v_operatingnumber varchar2(255);
  
BEGIN
    
    v_operatingnumber := '' || P_OPERATINGNUMBER || '';
    P_IS_OPNO_EXISTS := 'FALSE';
    
    --dbms_output.put_line('Executing check for Transformer');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM TRANSFORMER WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;  
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;    

    --dbms_output.put_line('Executing check for SWITCH');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM SWITCH WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;

    --dbms_output.put_line('Executing check for FUSE');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM FUSE WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;

    --dbms_output.put_line('Executing check for OPENPOINT');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM OPENPOINT WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
    
    --dbms_output.put_line('Executing check for DYNAMICPROTECTIVEDEVICE');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM DYNAMICPROTECTIVEDEVICE WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
      
    --dbms_output.put_line('Executing check for CAPACITORBANK');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM CAPACITORBANK WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
    
    --dbms_output.put_line('Executing check for VOLTAGEREGULATOR');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM VOLTAGEREGULATOR WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
  
    --dbms_output.put_line('Executing check for STEPDOWN');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM STEPDOWN WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;

    --dbms_output.put_line('Executing check for DCRECTIFIER');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM DCRECTIFIER WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;

   --dbms_output.put_line('Executing check for FAULTINDICATOR');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM FAULTINDICATOR WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
    
    --dbms_output.put_line('Executing check for LOADCHECKPOINT');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM LOADCHECKPOINT WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;
  
    --dbms_output.put_line('Executing check for NETWORKPROTECTOR');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM NETWORKPROTECTOR WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;

    --dbms_output.put_line('Executing check for PRIMARYMETER');
    SELECT COUNT(OPERATINGNUMBER) INTO v_count FROM PRIMARYMETER WHERE UPPER(OPERATINGNUMBER) = UPPER(v_operatingnumber) FETCH FIRST ROW ONLY;
    IF (v_count > 0) THEN
     P_IS_OPNO_EXISTS := 'TRUE';
     RETURN;
    END IF;


END;
