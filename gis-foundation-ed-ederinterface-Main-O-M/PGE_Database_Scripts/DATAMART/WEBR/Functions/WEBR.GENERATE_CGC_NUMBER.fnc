Prompt drop Function GENERATE_CGC_NUMBER;
DROP FUNCTION WEBR.GENERATE_CGC_NUMBER
/

Prompt Function GENERATE_CGC_NUMBER;
--
-- GENERATE_CGC_NUMBER  (Function) 
--
CREATE OR REPLACE FUNCTION WEBR."GENERATE_CGC_NUMBER" RETURN varchar2 AS
     V_CGC_Number  char(12);
BEGIN
---------------------------------------------------------------------------------------------
--- VERSION NO : 1.1J
--- Funcation will Generate Unique CGC Number starting with '0'.
---------------------------------------------------------------------------------------------
    V_CGC_Number := '1' || substr( '00000000000' || ltrim(rtrim(Gen_cgc_num.nextval)),length(Gen_cgc_num.currval)+1, 11);
    RETURN V_CGC_Number;
END Generate_Cgc_Number;

/
