Prompt drop Function TEMP_GET_RULE_MULTI;
DROP FUNCTION RELEDITOR.TEMP_GET_RULE_MULTI
/

Prompt Function TEMP_GET_RULE_MULTI;
--
-- TEMP_GET_RULE_MULTI  (Function) 
--
CREATE OR REPLACE FUNCTION RELEDITOR.Temp_GET_Rule_Multi (
  V_SourceLine  NVARCHAR2,
  V_TrUnitPhase  NVARCHAR2,
  V_TrUnitCount  NUMBER
)
  RETURN NVARCHAR2
AS
v_result NVARCHAR2(50);
BEGIN

if V_SourceLine = 'ABC_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=2  then v_result:='Rule_7A'; end if;
if V_SourceLine = 'ABC_WITHOUT_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=3  then v_result:='Rule_7B'; end if;
if V_SourceLine = 'AB_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=2  then v_result:='Rule_8A'; end if;
if V_SourceLine = 'BC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=2  then v_result:='Rule_9A'; end if;
if V_SourceLine = 'AC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=2  then v_result:='Rule_10A'; end if;
if V_SourceLine = 'ABC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=2  then v_result:='Rule_11A'; end if;
if V_SourceLine = 'ABC_WITH_NEUTRAL' and V_TrUnitPhase='SINGLE' and V_TrUnitCount=3  then v_result:='Rule_11B'; end if;

RETURN v_result;
END Temp_GET_Rule_Multi;
/
