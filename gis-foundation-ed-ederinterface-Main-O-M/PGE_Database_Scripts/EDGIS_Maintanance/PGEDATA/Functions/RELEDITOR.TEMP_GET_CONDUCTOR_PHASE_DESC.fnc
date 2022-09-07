Prompt drop Function TEMP_GET_CONDUCTOR_PHASE_DESC;
DROP FUNCTION RELEDITOR.TEMP_GET_CONDUCTOR_PHASE_DESC
/

Prompt Function TEMP_GET_CONDUCTOR_PHASE_DESC;
--
-- TEMP_GET_CONDUCTOR_PHASE_DESC  (Function) 
--
CREATE OR REPLACE FUNCTION RELEDITOR.Temp_GET_Conductor_Phase_Desc (
  V_Code  NUMBER
)
  RETURN NVARCHAR2
AS
v_result NVARCHAR2(50);
BEGIN

if V_Code = 4 then v_result:='A'; end if;
if V_Code = 2 then v_result:='B'; end if;
if V_Code = 1 then v_result:='C'; end if;
if V_Code = 6 then v_result:='AB'; end if;
if V_Code = 5 then v_result:='AC'; end if;
if V_Code = 3 then v_result:='BC'; end if;
if V_Code = 7 then v_result:='ABC'; end if;
if V_Code = 10 then v_result:='CN'; end if;
if V_Code = 8 then v_result:='N'; end if;
if V_Code = 9 then v_result:='PN'; end if;
if V_Code = 11 then v_result:='RCN'; end if;
if V_Code = 12 then v_result:='Unknown'; end if;

RETURN v_result;
END Temp_GET_Conductor_Phase_Desc;
/
