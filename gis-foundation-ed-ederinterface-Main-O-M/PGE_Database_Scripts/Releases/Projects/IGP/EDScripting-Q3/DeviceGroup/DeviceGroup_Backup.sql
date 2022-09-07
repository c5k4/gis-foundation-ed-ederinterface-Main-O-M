SPOOL C:\Temp\DeviceGroup_Restore.txt;
set serveroutput on;

DECLARE
  gobjids CLOB;
  tempclob CLOB;
  d NUMBER := 0;
  s NUMBER := 0;
  tbl VARCHAR2(25) := 'EDGIS.DeviceGroup';
  procedure grabids  
  IS
    BEGIN
      DBMS_LOB.CREATETEMPORARY( gobjids, TRUE );
      IF tbl != 'EDGIS.DeviceGroup' THEN
        FOR rec IN (SELECT ObjectID from EDGIS.A145 where SubTypeCD = s AND DeviceGroupType = d)    
            LOOP
                IF length(gobjids) > 0 THEN
                  tempclob := ',' || TRIM(TO_CHAR(rec.ObjectID));
                  DBMS_LOB.append(gobjids,tempclob);
                ELSE
                  tempclob := TO_CHAR(rec.ObjectID);
                  DBMS_LOB.append(gobjids,tempclob);
                END IF;
            END LOOP;
      ELSE
          FOR rec IN (SELECT ObjectID from EDGIS.DeviceGroup where SubTypeCD = s AND DeviceGroupType = d)    
            LOOP
                IF length(gobjids) > 0 THEN
                  tempclob := ',' || TRIM(TO_CHAR(rec.ObjectID));
                  DBMS_LOB.append(gobjids,tempclob);
                ELSE
                  tempclob := TO_CHAR(rec.ObjectID);
                  DBMS_LOB.append(gobjids,tempclob);
                END IF;
            END LOOP;
      END IF;
      
      IF length(gobjids) <= 0 THEN
        -- no records found send back -1 for update
        tempclob := TO_CHAR(-1);
        DBMS_LOB.append(gobjids,tempclob);
      END IF;
    END grabids;
    
BEGIN
  -- Base Table: these are the domains being moved and deleted s = subtypecd and d = devicegrouptype
  s := 2;  d := 11;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 2;  d := 14;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 2;  d := 18;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  
  s := 3;  d := 25;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 3;  d := 29;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 3;  d := 31;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.DeviceGroup Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');

  -- A table: these are the domains being moved and deleted s = subtypecd and d = devicegrouptype
  tbl := 'EDGIS.A145';
  s := 2;  d := 11;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 2;  d := 14;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 2;  d := 18;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  
  s := 3;  d := 25;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 3;  d := 29;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');
  s := 3;  d := 31;  grabids();
  DBMS_OUTPUT.PUT_LINE('Update EDGIS.A145 Set DeviceGroupType = ' || d || ' where ObjectID in (' || gobjids || ');');

END;
/
SPOOL OFF;
set serveroutput off;