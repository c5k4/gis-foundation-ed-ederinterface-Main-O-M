Prompt drop Function GENERATE_OPERATING_NUMBER;
DROP FUNCTION WEBR.GENERATE_OPERATING_NUMBER
/

Prompt Function GENERATE_OPERATING_NUMBER;
--
-- GENERATE_OPERATING_NUMBER  (Function) 
--
CREATE OR REPLACE FUNCTION WEBR."GENERATE_OPERATING_NUMBER" (V_Equipment_Type_ID Number, V_Customer_Own_Flag char, V_no_oper number ) RETURN varchar2 AS
     V_Operating_Number  char(9);
     V_Operating_String  Varchar2(500);
     V_OLD_Even_FLAG     char(1);
     V_PreFix            Varchar2(3);
     V_Number            Number(9);
     V_Max_value         Number;
     V_Mod_value         Number;
     V_Cnt               Number;
     V_Sel_Oper_Num     Number;
BEGIN
---------------------------------------------------------------------------------------------
--- VERSION NO : 1.1J
--- Funcation will Generate Operaqting Number for given Equipment Type and Customer Type.
---------------------------------------------------------------------------------------------
   V_cnt :=1;
   While V_cnt <= V_no_oper
   Loop
        if V_Customer_Own_Flag ='0' then
           Select OddEven, Prefixchars into V_OLD_Even_FLAG,V_PreFix from JET_EQUIPTYPERULES where Equiptypeid = V_Equipment_Type_ID;
           if V_OLD_Even_FLAG='E' then
              V_Mod_value := 0;
           End if;
           if V_OLD_Even_FLAG='O' then
              V_Mod_value := 1;
           End if;
           if ( V_PreFix is Null or rtrim(ltrim(V_PreFix)) = '' or V_PreFix='T') then
              V_Max_value := '999999';
           Else
              V_Max_value := '99999';
           End if;
        Else
           V_PreFix := 'CUS';
           V_Max_value := '9999';
        End if;

        if V_OLD_Even_FLAG='E' or V_OLD_Even_FLAG='O' then
           Loop
              select round(dbms_random.value(1,v_maX_vALUE),0) INTO v_NUMBER from dual;
              exit when ( mod(v_number,2)= V_Mod_value );
           End Loop;
        Else
           select round(dbms_random.value(1,v_maX_vALUE),0) INTO v_NUMBER from dual;
        End if;

--      dbms_output.put_line(V_Prefix ||  rtrim(ltrim(to_char(V_number,'999999999'))) || ' ' || v_maX_vALUE );
        V_Operating_Number:= V_Prefix ||  rtrim(ltrim(to_char(V_number,'999999999')));

        V_Sel_Oper_Num := Null;

        select Nvl(count(*),0) into V_Sel_Oper_Num  from  JET_EQUIPMENT where OPERATINGNUMBER = V_Operating_Number;

        if V_Sel_Oper_Num = 0  then
           V_cnt := V_Cnt +1 ;
           if V_cnt <= V_no_oper then
              V_Operating_String := V_Operating_String ||  rtrim(V_Operating_Number) || ',';
           else
              V_Operating_String := V_Operating_String ||  rtrim(V_Operating_Number) ;
           End if;
        End if;
    End loop;

    RETURN V_Operating_String;
END Generate_operating_Number;

/
