Prompt drop Function TEMP_GET_TR_PHASE;
DROP FUNCTION RELEDITOR.TEMP_GET_TR_PHASE
/

Prompt Function TEMP_GET_TR_PHASE;
--
-- TEMP_GET_TR_PHASE  (Function) 
--
CREATE OR REPLACE FUNCTION RELEDITOR.Temp_GET_TR_PHASE (
  V_Tr_Sub_Type  NUMBER,
  V_Tr_Type  NUMBER
)
  RETURN NVARCHAR2
AS
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
END Temp_GET_TR_PHASE;
/
