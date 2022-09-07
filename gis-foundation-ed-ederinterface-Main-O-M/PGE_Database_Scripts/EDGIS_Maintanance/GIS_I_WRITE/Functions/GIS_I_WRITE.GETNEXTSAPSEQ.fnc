Prompt drop Function GETNEXTSAPSEQ;
DROP FUNCTION GIS_I_WRITE.GETNEXTSAPSEQ
/

Prompt Function GETNEXTSAPSEQ;
--
-- GETNEXTSAPSEQ  (Function) 
--
CREATE OR REPLACE FUNCTION GIS_I_WRITE."GETNEXTSAPSEQ"
( SeqName IN VARCHAR2
) RETURN NUMBER AS
     v_nextval INTEGER;
     v_select VARCHAR2(100);

BEGIN

     v_select := 'select '||SeqName||'.nextval from dual';
     EXECUTE IMMEDIATE v_select INTO v_nextval;

     RETURN v_nextval;

END GETNEXTSAPSEQ;
/
