--------------------------------------------------------
--  DDL for Procedure GEN_SERVICEPOINT_INFO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."GEN_SERVICEPOINT_INFO" (P_TRANSFORMERGUID IN VARCHAR2, P_GENINFO OUT SYS_REFCURSOR) AS 
v_query varchar2(30000);
BEGIN
  v_query:='select a.GLOBALID,a.SERVICEPOINTID,a.STREETNUMBER,a.STREETNAME1,a.STREETNAME2,a.CITY,a.COUNTY,a.STATE,b.SERVICEPOINTGUID,b.SAPEGINOTIFICATION,Case When b.LIMITED=''Y'' Then ''Yes'' When b.LIMITED=''N'' Then ''No'' Else ''NA'' End "LIMITED",b.GLOBALID as "GEN_GLOBALID" from EDGIS.SERVICEPOINT a left outer join EDGIS.GENERATIONINFO b ON a.GLOBALID = b.SERVICEPOINTGUID where a.TRANSFORMERGUID  = '''|| P_TRANSFORMERGUID ||'''';
  open P_GENINFO for v_query;
END GEN_SERVICEPOINT_INFO;