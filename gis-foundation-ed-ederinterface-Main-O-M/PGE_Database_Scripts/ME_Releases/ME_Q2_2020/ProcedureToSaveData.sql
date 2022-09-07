spool "D:\Temp\CREATE_PROCEDURE_ED.txt"

create or replace PROCEDURE        PGEDATA.INSERTSESSIONDATA 
(
  SESSIONID IN VARCHAR2,
  SESSIONNAME IN VARCHAR2,
  SESSIONDESCRIPTION IN VARCHAR2,
  CREATEDBY IN VARCHAR2,
  CREATEDON IN VARCHAR2,
  POSTEDBY IN VARCHAR2,
  POSTEDON IN VARCHAR2,   
  EDITXML IN CLOB 
) AS 
BEGIN
   
  INSERT INTO PGEDATA.SESSIONDATA VALUES (PGEDATA.SESSIONDATA_SEQ.NEXTVAL,SESSIONID, SESSIONNAME, SESSIONDESCRIPTION, CREATEDBY, TO_DATE(CREATEDON, 'dd-mon-yy hh:mi:ss'), POSTEDBY, TO_DATE(POSTEDON, 'dd-mon-yy hh:mi:ss'), EDITXML);
  
  COMMIT;
  
END INSERTSESSIONDATA;

/

GRANT EXECUTE ON PGEDATA.INSERTSESSIONDATA TO PGEDATA, SDE, EDGISBO, DMSSTAGING, GIS_I_WRITE, GISINTERFACE, GIS_INTERFACE, GIS_I, SDE_VIEWER, SDE_EDITOR, DAT_EDITOR, MM_ADMIN, SELECT_CATALOG_ROLE;
commit;

/

spool off;