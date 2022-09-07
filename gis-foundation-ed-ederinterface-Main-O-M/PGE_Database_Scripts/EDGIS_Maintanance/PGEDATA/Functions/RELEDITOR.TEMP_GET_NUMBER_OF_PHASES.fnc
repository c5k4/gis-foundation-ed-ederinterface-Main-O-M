Prompt drop Function TEMP_GET_NUMBER_OF_PHASES;
DROP FUNCTION RELEDITOR.TEMP_GET_NUMBER_OF_PHASES
/

Prompt Function TEMP_GET_NUMBER_OF_PHASES;
--
-- TEMP_GET_NUMBER_OF_PHASES  (Function) 
--
CREATE OR REPLACE FUNCTION RELEDITOR.Temp_GET_Number_Of_Phases (
  V_Code  NUMBER
)
  RETURN NUMBER
AS
v_result NUMBER;
BEGIN

if V_Code = 1 then v_result:=1; end if;
if V_Code = 2 then v_result:=1; end if;
if V_Code = 3 then v_result:=2; end if;
if V_Code = 4 then v_result:=1; end if;
if V_Code = 5 then v_result:=2; end if;
if V_Code = 6 then v_result:=2; end if;
if V_Code = 7 then v_result:=3; end if;

RETURN v_result;
END Temp_GET_Number_Of_Phases;
/
