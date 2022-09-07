spool D:\TEMP\PGE_IGP_PHASE_UPDATE_FUNCTIONS.txt

select current_timestamp from dual;
--------------------------------------------------------
--  DDL for Function IGP_GET_NORMAL_STATUS
--------------------------------------------------------
create or replace FUNCTION "IGP_GET_NORMAL_STATUS" (
  V_Code  NUMBER   
)
  RETURN NVARCHAR2
AS
v_result NVARCHAR2(25);
BEGIN 
 
if V_Code = 0 then v_result:='Open'; end if;
if V_Code = 1 then v_result:='Closed'; end if;
if V_Code = 2 then v_result:='Not Applicable'; end if;

RETURN v_result;
END IGP_GET_NORMAL_STATUS;
/
--------------------------------------------------------
--  DDL for Function IGP_GET_PHASE_FROMCODE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_GET_PHASE_FROMCODE" (
  V_Code  NUMBER   
)
  RETURN NVARCHAR2
AS
v_result NVARCHAR2(15);
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
END IGP_GET_Phase_FromCode;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_COND_PHASE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_COND_PHASE" (
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
END IGP_TU_GET_Cond_Phase;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_NUMBER_OF_PHASES
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_NUMBER_OF_PHASES" (
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
END IGP_TU_GET_Number_Of_Phases;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_PRIMARYVOLT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_PRIMARYVOLT" (
  V_Code  NUMBER
)
  RETURN NUMBER
AS
v_result NUMBER(10);
BEGIN

if V_Code = 0 then v_result:=480; end if;
if V_Code = 1 then v_result:=2400; end if;
if V_Code = 2 then v_result:=4160; end if;
if V_Code = 3 then v_result:=4800; end if;
if V_Code = 35 then v_result:=6900; end if;
if V_Code = 4 then v_result:=7200; end if;
if V_Code = 5 then v_result:=12000; end if;
if V_Code = 6 then v_result:=17000; end if;
if V_Code = 65 then v_result:=19900; end if;
if V_Code = 8 then v_result:=21000; end if;
if V_Code = 7 then v_result:=22000; end if;
if V_Code = 85 then v_result:=34500; end if;
if V_Code = 9 then v_result:=44000; end if;

RETURN v_result;
END IGP_TU_GET_PrimaryVolt;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_RULE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_RULE" (
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
END IGP_TU_GET_Rule;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_RULE_MULTI
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_RULE_MULTI" (
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
END IGP_TU_GET_Rule_Multi;

/
--------------------------------------------------------
--  DDL for Function IGP_TU_GET_TRANS_PHASE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "PGEIGPDATA"."IGP_TU_GET_TRANS_PHASE" (
  V_Tr_Sub_Type  NUMBER,
  V_Tr_Type  NUMBER
)
  RETURN NVARCHAR2
IS
v_result NVARCHAR2(50);
BEGIN

if V_Tr_Sub_Type = 1 AND V_Tr_Type =1 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =2 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =3 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =4 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =5 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =6 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =12 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =13 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =14 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =22 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =24 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =25 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =26 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =27 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =28 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =29 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =30 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =31 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =34 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =35 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =36 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =37 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =44 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =45 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =46 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =47 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =77 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =88 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 1 AND V_Tr_Type =99 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =7 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =8 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =12 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =16 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =18 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =19 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =20 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =21 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =77 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =88 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 3 AND V_Tr_Type =99 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =3 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =9 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =10 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =11 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =12 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =15 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =32 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =33 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =48 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =49 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =50 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =51 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =52 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =53 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =54 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =55 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =56 then v_result:='THREE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =77 then v_result:='SINGLE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =88 then v_result:='EXCLUDE'; end if;
if V_Tr_Sub_Type = 2 AND V_Tr_Type =99 then v_result:='EXCLUDE'; end if;

RETURN v_result;
END IGP_TU_GET_TRANS_PHASE;

/
grant all on PGEIGPDATA.IGP_GET_PHASE_FROMCODE to IGPEDITOR;
--------------------------------------------- Functions Ends--------------------------------------------------------
select current_timestamp from dual;
spool off