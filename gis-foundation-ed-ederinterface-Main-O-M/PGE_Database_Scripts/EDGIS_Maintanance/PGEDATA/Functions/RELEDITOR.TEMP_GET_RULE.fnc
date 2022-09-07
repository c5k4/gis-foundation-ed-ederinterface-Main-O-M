Prompt drop Function TEMP_GET_RULE;
DROP FUNCTION RELEDITOR.TEMP_GET_RULE
/

Prompt Function TEMP_GET_RULE;
--
-- TEMP_GET_RULE  (Function) 
--
CREATE OR REPLACE FUNCTION RELEDITOR.Temp_GET_Rule (
  V_SourceLine  NVARCHAR2,
  V_TrUnitPhase  NVARCHAR2
)
  RETURN NVARCHAR2
AS
v_result NVARCHAR2(50);
BEGIN

if V_SourceLine = 'A_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_1'; end if;
if V_SourceLine = 'B_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_2'; end if;
if V_SourceLine = 'C_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_3'; end if;
if V_SourceLine = 'AB_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_4'; end if;
if V_SourceLine = 'BC_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_5'; end if;
if V_SourceLine = 'AC_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_6'; end if;
if V_SourceLine = 'ABC_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_7'; end if;
if V_SourceLine = 'AB_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_8'; end if;
if V_SourceLine = 'BC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_9'; end if;
if V_SourceLine = 'AC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_10'; end if;
if V_SourceLine = 'ABC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' then v_result:='Rule_11'; end if;
if V_SourceLine = 'ABC_WITH_NEUTRAL' and V_TrUnitPhase='THREE' then v_result:='Rule_12'; end if;
if V_SourceLine = 'ABC_WITHOUT_NEUTRAL' and V_TrUnitPhase='THREE' then v_result:='Rule_13'; end if;

RETURN v_result;
END Temp_GET_Rule;
/
