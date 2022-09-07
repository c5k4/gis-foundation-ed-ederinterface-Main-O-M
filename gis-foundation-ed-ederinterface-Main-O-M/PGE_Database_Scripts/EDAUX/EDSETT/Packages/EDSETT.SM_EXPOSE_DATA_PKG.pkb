Prompt drop Package Body SM_EXPOSE_DATA_PKG;
DROP PACKAGE BODY EDSETT.SM_EXPOSE_DATA_PKG
/

Prompt Package Body SM_EXPOSE_DATA_PKG;
--
-- SM_EXPOSE_DATA_PKG  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY EDSETT."SM_EXPOSE_DATA_PKG"
AS



PROCEDURE SP_SM_SET_DATE_PARAMS(
    I_From     IN DATE,
    I_To           IN DATE)
AS
BEGIN

  DATEFROM := I_From;
  DATETO :=I_To;

END SP_SM_SET_DATE_PARAMS;


FUNCTION GET_DATEFROM
  RETURN DATE
IS
BEGIN
  RETURN DATEFROM;
END;

FUNCTION GET_DATETO
  RETURN DATE
IS
BEGIN
  RETURN DATETO;
END;

END SM_EXPOSE_DATA_PKG;

/


Prompt Grants on PACKAGE SM_EXPOSE_DATA_PKG TO GIS_I to GIS_I;
GRANT EXECUTE ON EDSETT.SM_EXPOSE_DATA_PKG TO GIS_I
/