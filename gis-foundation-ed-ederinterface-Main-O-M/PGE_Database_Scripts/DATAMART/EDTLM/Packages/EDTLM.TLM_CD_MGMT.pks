Prompt drop Package TLM_CD_MGMT;
DROP PACKAGE EDTLM.TLM_CD_MGMT
/

Prompt Package TLM_CD_MGMT;
--
-- TLM_CD_MGMT  (Package) 
--
CREATE OR REPLACE PACKAGE EDTLM."TLM_CD_MGMT" AS
procedure ProcessXML(XmlContent varchar2
--,ErrorMessage OUT VARCHAR2,IsSuccess OUT CHAR
);
procedure LogErrors(GLOBALID varchar2,
          ERRORCODE varchar2,
          ERRORMSG varchar2,
          TRANSDATE timestamp,
          TRANSTYPE char,
          APPTYPE char,PROCFOR varchar2);
END TLM_CD_MGMT;

/


Prompt Grants on PACKAGE TLM_CD_MGMT TO GIS_I to GIS_I;
GRANT EXECUTE ON EDTLM.TLM_CD_MGMT TO GIS_I
/
