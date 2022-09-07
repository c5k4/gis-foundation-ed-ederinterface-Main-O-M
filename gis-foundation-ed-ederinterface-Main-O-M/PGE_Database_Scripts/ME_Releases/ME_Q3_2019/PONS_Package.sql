CREATE OR REPLACE PACKAGE "PONS_RO"."PONS" 
   as



     ----- PROCEDURE TO GET CUSTOMER INFO THROUGH GLOBALIDS OF SERVICEPOINTS AS BOX UNCHECKED -------

     procedure CUST_INFO_SERVICEPOINTS_INCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER);

     ----- PROCEDURE TO GET CUSTOMER INFO THROUGH GLOBALIDS OF SERVICEPOINTS AS BOX UNCHECKED -------

     procedure CUST_INFO_SERVICEPOINTS_EXCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER);




     ------ PROCEDURE TO GENERATE CUSTOMER INFO VIA CIRCUITIDS AS UNCHECKED ------

     PROCEDURE CUST_INFO_CIRCUITID_INCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER);

     ------ PROCEDURE TO GENERATE CUSTOMER INFO VIA CIRCUITIDS AS CHECKED ------

     PROCEDURE CUST_INFO_CIRCUITID_EXCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER);


    ------ PROCEDURE TO GENERATE SHUTDOWNID -----

    PROCEDURE GEN_SHUTDOWNID(
          p_shutdownId OUT NUMBER,
          p_error OUT VARCHAR2,
          p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS OF PRIMARY METERS -----

    PROCEDURE CUST_INFO_PM_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS OF PRIMARY METERS -----

    PROCEDURE CUST_INFO_PM_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

  ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS AS BOX UNCHECKED -----

    PROCEDURE CUST_INFO_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS AS BOX CHECKED -----

    PROCEDURE CUST_INFO_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

   PROCEDURE GETDIVISION_FEEDER(
            P_DIVISION IN NUMBER,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS AS BOX UNCHECKED -----
    PROCEDURE LOADINGINFO_CUSTOMER_CGC(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);
   end PONS; -- Package Ends

/


CREATE OR REPLACE PACKAGE BODY "PONS_RO"."PONS" 
   as

------------------ GET CUSTOMER INFORMATION USING SERVICEPOINTIDS WHERE BOX IS UNCHECKED------------

procedure CUST_INFO_SERVICEPOINTS_INCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER)
as

          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql VARCHAR2(32767);
          v_sql_final clob default '';
          v_sql3 VARCHAR2(32767);
          V_TEMP_STRING CLOB DEFAULT '';
          V_FINAL_STRING CLOB DEFAULT '';

        BEGIN

          SELECT REPLACE(P_INPUTSTRING,',',CHR(39)||','||CHR(39)) into v_temp_string FROM DUAL;

          v_final_string:=CHR(39)||v_temp_string||CHR(39);
          dbms_output.put_line(v_final_string);

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;

          -----------------------------------------

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          (
          c.MailStreetNum is not null or
          c.MailStreetName1 is not null or
          c.MailStreetName2 is not null or
          c.MailZipCode is not null or
          C.MailCity is not null or
          c.MailState is not null
          )
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          a.servicepointid in (';

          v_sql3:=') UNION
          select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          a.servicepointid in (';

          v_sql_final:=v_sql||v_final_string||v_sql3||v_final_string||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,mailstreetname1,mailstreetname2,mailstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.ZZ_MV_SERVICEPOINT a,EDGIS.ZZ_MV_TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          (
          c.MailStreetNum is not null or
          c.MailStreetName1 is not null or
          c.MailStreetName2 is not null or
          c.MailZipCode is not null or
          C.MailCity is not null or
          c.MailState is not null
          )
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          a.servicepointid in (';

          v_sql3:=')
          UNION
          select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          a.servicepointid in (';

          v_sql_final:=v_sql||v_final_string||v_sql3||v_final_string||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,mailstreetname1,mailstreetname2,mailstreetnum';

          END IF;

          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);
            --p_error_text := True;


          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

END  CUST_INFO_SERVICEPOINTS_INCL;

------------------ GET CUSTOMER INFORMATION USING SERVICEPOINTIDS WHERE BOX IS UNCHECKED------------

procedure CUST_INFO_SERVICEPOINTS_EXCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER)
as

          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql VARCHAR2(32767);
          v_sql_final clob default '';
          v_sql3 VARCHAR2(32767);
          V_TEMP_STRING CLOB DEFAULT '';
          V_FINAL_STRING CLOB DEFAULT '';

        BEGIN

          SELECT REPLACE(P_INPUTSTRING,',',CHR(39)||','||CHR(39)) into v_temp_string FROM DUAL;

          v_final_string:=CHR(39)||v_temp_string||CHR(39);
          dbms_output.put_line(v_final_string);

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;

          -----------------------------------------

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
          )
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          a.servicepointid in (';

          v_sql3:=') UNION
          select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,
            upper(a.PREMISEID)as PREMISEID ,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          a.servicepointid in (';

          v_sql_final:=v_sql||v_final_string||v_sql3||v_final_string||') order by SOURCESIDEDEVICEID, CGC12';

          ELSE

          sde.version_util.set_current_version(i_version);

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,
            upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.ZZ_MV_SERVICEPOINT a,EDGIS.ZZ_MV_TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
          )
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          a.servicepointid in (';

          v_sql3:=')
          UNION
          select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,
            upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.ZZ_MV_SERVICEPOINT a,EDGIS.ZZ_MV_TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
          a.cgc12=B.CGC12
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          a.servicepointid in (';

          v_sql_final:=v_sql||v_final_string||v_sql3||v_final_string||') order by SOURCESIDEDEVICEID, CGC12';

          END IF;


          open p_cursor for v_sql_final ;


            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);
            --p_error_text := True;


          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

END  CUST_INFO_SERVICEPOINTS_EXCL;



------------------ GET CUSTOMER INFORMATION USING CIRCUITIDS AS BOX UNCHECKED ------------

    PROCEDURE CUST_INFO_CIRCUITID_INCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER)

    AS


          NO_DATA_FOUND EXCEPTION;

          v_servicepointids clob default '';
          v_cgc12 VARCHAR2(30000);
          i_flag VARCHAR2(5);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_servicepointids2 clob default '';
          v_cgc122 VARCHAR2(30000);
          v_sql VARCHAR2(30000);
          v_sql2 VARCHAR2(30000);
          v_sql_final clob default '';
          v_sql3 VARCHAR2(30000);
          v_sql4 VARCHAR2(30000);
          v_rows number;
          v_servicepointid varchar2(50);
          v_final_string varchar2(30000);
          v_temp_string varchar2(30000);
          v_abc varchar2(300);

        BEGIN

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_servicepointids:='';
          v_cgc12:='';
          v_rows:=0;


          SELECT REPLACE(P_INPUTSTRING,',',CHR(39)||','||CHR(39)) into v_temp_string FROM DUAL;
          dbms_output.put_line(v_temp_string);
          v_final_string:=CHR(39)||v_temp_string||CHR(39);
          dbms_output.put_line(v_final_string);


-----------------------------------------------------------------

          if i_version is null then

          v_sql:='select
            a.servicepointid as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select servicepointid from edgis.servicepoint where cgc12 in
          (select cgc12 from edgis.PRIMARYMETER where CIRCUITID in ('||v_final_string||'))';

          v_sql2:=')
          UNION all
          select
          a.servicepointid as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.PRIMARYMETER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.SERVICEPOINTID in
              (select servicepointid from edgis.servicepoint where cgc12 in
              (select cgc12 from edgis.PRIMARYMETER where CIRCUITID in ('||v_final_string||')))';

          v_sql3:='
          union all
          select
            a.servicepointid as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select servicepointid from edgis.servicepoint where cgc12 in
          (select cgc12 from edgis.transformer where CIRCUITID in ('||v_final_string||'))';

          v_sql4:=')
          UNION all
          select
          a.servicepointid as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.SERVICEPOINTID in
              (select servicepointid from edgis.servicepoint where cgc12 in
              (select cgc12 from edgis.transformer where CIRCUITID in ('||v_final_string||'))';



          v_sql_final:=v_sql||v_sql2||v_sql3||v_sql4||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,MAILstreetname1,MAILstreetname2,MAILstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

          v_sql:='select
            a.servicepointid as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.zz_mv_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.zz_mv_PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
          (select cgc12 from edgis.zz_mv_PRIMARYMETER where CIRCUITID in ('||v_final_string||'))';

          v_sql2:=')
          UNION all
          select
          a.servicepointid as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID ,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.zz_mv_SERVICEPOINT a,EDGIS.zz_mv_PRIMARYMETER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.SERVICEPOINTID in
              (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
              (select cgc12 from edgis.zz_mv_PRIMARYMETER where CIRCUITID in ('||v_final_string||')))';

          v_sql3:='
          union all
          select
            a.servicepointid as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.zz_mv_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.zz_mv_TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
          (select cgc12 from edgis.zz_mv_transformer where CIRCUITID in ('||v_final_string||'))';

          v_sql4:=')
          UNION all
          select
          a.servicepointid as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID ,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.zz_mv_SERVICEPOINT a,EDGIS.zz_mv_TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.SERVICEPOINTID in
              (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
              (select cgc12 from edgis.zz_mv_transformer where CIRCUITID in ('||v_final_string||'))';



          v_sql_final:=v_sql||v_sql2||v_sql3||v_sql4||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,MAILstreetname1,MAILstreetname2,MAILstreetnum';

          END IF;


          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

        EXCEPTION

        WHEN NO_DATA_FOUND THEN
          p_success := 0;
          p_error   := 'No Data Found';
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

        WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);


    END CUST_INFO_CIRCUITID_INCL;

------------------ GET CUSTOMER INFORMATION USING CIRCUITIDS AS BOX CHECKED ------------

    PROCEDURE CUST_INFO_CIRCUITID_EXCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER)

    AS

          NO_DATA_FOUND EXCEPTION;

          v_servicepointids VARCHAR2(30000);
          v_cgc12 VARCHAR2(30000);
          i_flag VARCHAR2(5);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_servicepointids2 VARCHAR2(30000);
          v_cgc122 VARCHAR2(30000);
          v_sql VARCHAR2(30000);
          v_sql2 VARCHAR2(30000);
          v_sql_final VARCHAR2(30000);
          v_sql3 VARCHAR2(30000);
          v_sql4 VARCHAR2(30000);
          v_rows number;
          v_servicepointid VARCHAR2(50);
          v_final_string varchar2(30000);
          v_temp_string varchar2(30000);


        BEGIN

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_servicepointids:='';
          v_cgc12:='';
          v_rows:=0;
          ---------------------------------------

          SELECT REPLACE(P_INPUTSTRING,',',CHR(39)||','||CHR(39)) into v_temp_string FROM DUAL;
          dbms_output.put_line(v_temp_string);
          v_final_string:=CHR(39)||v_temp_string||CHR(39);
          dbms_output.put_line(v_final_string);

          ---------------------------------------

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select distinct servicepointid from
          (select servicepointid from edgis.servicepoint where cgc12 in
          (select cgc12 from edgis.PRIMARYMETER where CIRCUITID in ('||v_final_string||')))';

          v_sql2:=')
          UNION
          select
          upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.PRIMARYMETER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
              )
              and
              a.SERVICEPOINTID in
              (select distinct servicepointid from
              (select servicepointid from edgis.servicepoint where cgc12 in
              (select cgc12 from edgis.PRIMARYMETER where CIRCUITID in ('||v_final_string||'))))';

          v_sql3:='
          union
          select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select distinct servicepointid from
          (select servicepointid from edgis.servicepoint where cgc12 in
          (select cgc12 from edgis.transformer where CIRCUITID in ('||v_final_string||')))';

          v_sql4:=')
          UNION
          select
          upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,EDGIS.TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
              )
              and
              a.SERVICEPOINTID in
              (select distinct servicepointid from
              (select servicepointid from edgis.servicepoint where cgc12 in
              (select cgc12 from edgis.transformer where CIRCUITID in ('||v_final_string||')))';



          v_sql_final:=v_sql||v_sql2||v_sql3||v_sql4||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,MAILstreetname1,MAILstreetname2,MAILstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.zz_mv_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.zz_mv_PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select distinct servicepointid from
          (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
          (select cgc12 from edgis.zz_mv_PRIMARYMETER where CIRCUITID in ('||v_final_string||')))';

          v_sql2:=')
          UNION
          select
          upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.zz_mv_SERVICEPOINT a,EDGIS.zz_mv_PRIMARYMETER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
             )
              and
              a.SERVICEPOINTID in
              (select distinct servicepointid from
              (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
              (select cgc12 from edgis.zz_mv_PRIMARYMETER where CIRCUITID in ('||v_final_string||'))))';

          v_sql3:='
          union
          select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as MailStreetNum,
            upper(a.StreetName1) as MailStreetName1,
            upper(a.StreetName2) as MailStreetName2,
            upper(a.Zip) as MailZipCode,
            upper(a.City) as MailCity,
            upper(a.State) as MailState,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.zz_mv_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.zz_mv_TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
            a.StreetNumber is not null or
            a.StreetName1 is not null or
            a.StreetName2 is not null or
            a.Zip is not null or
            a.City is not null or
            a.State is not null
          )
          and
          a.servicepointid in
          (select distinct servicepointid from
          (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
          (select cgc12 from edgis.zz_mv_transformer where CIRCUITID in ('||v_final_string||')))';

          v_sql4:=')
          UNION
          select
          upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID) as PREMISEID ,
            upper(a.PREMISETYPE) as PREMISETYPE
          from  EDGIS.zz_mv_SERVICEPOINT a,EDGIS.zz_mv_TRANSFORMER b, CUSTOMER.CUSTOMER_INFO C
          where
              a.cgc12=B.CGC12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
              )
              and
              a.SERVICEPOINTID in
              (select distinct servicepointid from
              (select servicepointid from edgis.zz_mv_servicepoint where cgc12 in
              (select cgc12 from edgis.zz_mv_transformer where CIRCUITID in ('||v_final_string||')))';



          v_sql_final:=v_sql||v_sql2||v_sql3||v_sql4||') order by Sourcesidedeviceid,CGC12,servicepointid,ord,MAILstreetname1,MAILstreetname2,MAILstreetnum';

          END IF;

          open p_cursor for v_sql_final ;

          ---------------------------------------
            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

        EXCEPTION
        WHEN NO_DATA_FOUND THEN
         p_success := 0;
         p_error   := 'No Data Found';
         dbms_output.put_line(p_success);
         dbms_output.put_line(p_error);

        WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);


    END CUST_INFO_CIRCUITID_EXCL;




------------------------ Generate Shutdown ID ------------------------

        PROCEDURE GEN_SHUTDOWNID(
            p_shutdownId OUT NUMBER,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
        AS

        BEGIN
            select PONSSEQ_SHUTDOWN_ID.nextval into p_shutdownID from dual;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);
            --p_error_text := True;

        EXCEPTION
        WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
        END GEN_SHUTDOWNID;


------------------------ Customer Info Using TRANSFORMER CGC NUMBER AS BOX UNCHECKED ------------------------

   PROCEDURE CUST_INFO_PM_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS

          v_cgc12 VARCHAR2(30000);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql clob default '';
          v_sql_final clob default '';
          v_sql3 clob default '';

        BEGIN


          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_cgc12:=P_INPUTSTRING;



              if i_version is null then

              v_sql:='select
              upper(a.servicepointid) as servicepointid,
              UPPER(C.MAILNAME1) as MAILNAME1,
              UPPER(C.MAILNAME2) as MAILNAME2,
              decode( C.COMMUNICATIONPREFERENCE,null,null,
                          decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                                decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                                )
                     ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
              upper(c.MailStreetNum) as MailStreetNum,
              upper(c.MailStreetName1) as MailStreetName1,
              upper(c.MailStreetName2) as MailStreetName2,
              upper(c.MailZipCode) as MailZipCode,
              upper(c.MailCity) as MailCity,
              upper(c.MailState) as MailState,
              CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId)as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
              from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.PRIMARYMETER B
              where
              a.cgc12=b.cgc12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

              v_sql3:=')) UNION
              select
              upper(a.servicepointid) as servicepointid,
              CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
              CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
              CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
              is null then A.CUSTOMERTYPE
              else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
              upper(a.StreetNumber) as StreetNumber,
              upper(a.StreetName1) as StreetName1,
              upper(a.StreetName2) as StreetName2,
              upper(a.Zip) as Zip,
              upper(a.City) as City,
              upper(a.State) as State,
              CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
              is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
              else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId) as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
              from  EDGIS.SERVICEPOINT a
              LEFT JOIN CUSTOMER.CUSTOMER_INFO C
              ON
              A.SERVICEPOINTID=C.SERVICEPOINTID
              left JOIN EDGIS.PRIMARYMETER B
              ON
              A.CGC12=B.CGC12
              where
              (
                a.StreetNumber is not null or
                a.StreetName1 is not null or
                a.StreetName2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
              )
              and
              b.cgc12 in ('||v_cgc12||')
              and
              a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';


              v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

              ELSE

              sde.version_util.set_current_version(i_version);

              v_sql:='select
              upper(a.servicepointid) as servicepointid,
              UPPER(C.MAILNAME1) as MAILNAME1,
              UPPER(C.MAILNAME2) as MAILNAME2,
              decode( C.COMMUNICATIONPREFERENCE,null,null,
                          decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                                decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                                )
                     ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
              upper(c.MailStreetNum) as MailStreetNum,
              upper(c.MailStreetName1) as MailStreetName1,
              upper(c.MailStreetName2) as MailStreetName2,
              upper(c.MailZipCode) as MailZipCode,
              upper(c.MailCity) as MailCity,
              upper(c.MailState) as MailState,
              CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId)as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
              from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_PRIMARYMETER B
              where
              a.cgc12=b.cgc12
              and
              a.servicepointid=C.SERVICEPOINTID
              and
              (
              c.MailStreetNum is not null or
              c.MailStreetName1 is not null or
              c.MailStreetName2 is not null or
              c.MailZipCode is not null or
              C.MailCity is not null or
              c.MailState is not null
              )
              and
              a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

              v_sql3:=')) UNION
              select
              upper(a.servicepointid) as servicepointid,
              CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
              CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
              CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
              is null then A.CUSTOMERTYPE
              else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
              upper(a.StreetNumber) as StreetNumber,
              upper(a.StreetName1) as StreetName1,
              upper(a.StreetName2) as StreetName2,
              upper(a.Zip) as Zip,
              upper(a.City) as City,
              upper(a.State) as State,
              CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
              is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
              else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId) as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
              from  EDGIS.ZZ_MV_SERVICEPOINT a
              LEFT JOIN CUSTOMER.CUSTOMER_INFO C
              ON
              A.SERVICEPOINTID=C.SERVICEPOINTID
              left JOIN EDGIS.ZZ_MV_PRIMARYMETER B
              ON
              A.CGC12=B.CGC12
              where
              (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
              )
              and
              b.cgc12 in ('||v_cgc12||')
              and
              a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';


              v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

              END IF;

          open p_cursor for v_sql_final ;
          p_error                                       := 'NO ORA ERRORS';
          p_success                                     := 1;
          dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END CUST_INFO_PM_CGC_INCL;

   ------------------------ Customer Info Using TRANSFORMER CGC NUMBER AS BOX CHECKED ------------------------

   PROCEDURE CUST_INFO_PM_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS

          v_cgc12 VARCHAR2(30000);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql clob default '';
          v_sql_final clob default '';
          v_sql3 clob default '';


        BEGIN


          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_cgc12:=P_INPUTSTRING;
          -----------------------------------------

          if i_version is null then

          v_sql:='select
          upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.PRIMARYMETER B
          where
          a.cgc12=b.cgc12
          and
          a.servicepointid=C.SERVICEPOINTID
          and
          (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
          )
          and
          a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

          v_sql3:=')) UNION
              select
              upper(a.servicepointid) as servicepointid,
              CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
              CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
              CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
              is null then A.CUSTOMERTYPE
              else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
              upper(a.StreetNumber) as StreetNumber,
              upper(a.StreetName1) as StreetName1,
              upper(a.StreetName2) as StreetName2,
              upper(a.Zip) as Zip,
              upper(a.City) as City,
              upper(a.State) as State,
              CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
              is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
              else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId) as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''2'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
              from  EDGIS.SERVICEPOINT a
              LEFT JOIN CUSTOMER.CUSTOMER_INFO C
              ON
              A.SERVICEPOINTID=C.SERVICEPOINTID
              left JOIN EDGIS.PRIMARYMETER B
              ON
              A.CGC12=B.CGC12
              where
              (
                a.StreetNumber is not null or
                a.StreetName1 is not null or
                a.StreetName2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
              )
              and
              b.cgc12 in ('||v_cgc12||')
              and
              a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';


          v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by SOURCESIDEDEVICEID, CGC12';

          ELSE

          sde.version_util.set_current_version(i_version);

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' as ORD,  upper(a.PREMISEID) as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
          from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_PRIMARYMETER B
          where
          A.CGC12=B.CGC12
          AND
          a.servicepointid=C.SERVICEPOINTID
          and
          (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
          )
          and
          a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

          v_sql3:=')) UNION
              select
              upper(a.servicepointid) as servicepointid,
              CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
              CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
              CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
              is null then A.CUSTOMERTYPE
              else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
              upper(a.StreetNumber) as StreetNumber,
              upper(a.StreetName1) as StreetName1,
              upper(a.StreetName2) as StreetName2,
              upper(a.Zip) as Zip,
              upper(a.City) as City,
              upper(a.State) as State,
              CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
              is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
              else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
              CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
              upper(b.SourceSideDeviceId) as SourceSideDeviceId,
              upper(A.accountnum) as ACCOUNTID,
              A.DIVISION,
              '''||i_user||''' as CURRUSER,
              upper(A.METERNUMBER) as METERNUMBER,
              ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
              from  EDGIS.ZZ_MV_SERVICEPOINT a
              LEFT JOIN CUSTOMER.CUSTOMER_INFO C
              ON
              A.SERVICEPOINTID=C.SERVICEPOINTID
              left JOIN EDGIS.ZZ_MV_PRIMARYMETER B
              ON
              A.CGC12=B.CGC12
              where
              (
                a.StreetNumber is not null or
                a.StreetName1 is not null or
                a.StreetName2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
              )
              and
              b.cgc12 in ('||v_cgc12||')
              and
              a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';


          v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by SOURCESIDEDEVICEID, CGC12';

          END IF;

          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END CUST_INFO_PM_CGC_EXCL;

   ----------------------------------------------------------------------------------------



   ------------------------ Customer Info Using TRANSFORMER CGC NUMBER AS BOX UNCHECKED ------------------------

   PROCEDURE CUST_INFO_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS

          v_cgc12 VARCHAR2(30000);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql clob default '';
          v_sql2 clob default '';
          v_sql4 clob default '';
          v_sql_final clob default '';
          v_sql3 clob default '';
          v_final_string varchar2(30000);
          v_str varchar(32000);

        BEGIN

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_cgc12:=P_INPUTSTRING;

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE) as PREMISETYPE
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

           v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';


          v_sql_final:=v_sql||v_cgc12||v_sql2||v_cgc12||v_sql3||v_cgc12||v_sql4||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

            v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID ,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

          v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

          END IF;

          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END CUST_INFO_CGC_INCL;

   ------------------------ Customer Info Using TRANSFORMER CGC NUMBER AS BOX CHECKED ------------------------

   PROCEDURE CUST_INFO_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS


         v_cgc12 VARCHAR2(30000);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql clob default '';
          v_sql2 clob default '';
          v_sql4 clob default '';
          v_sql_final clob default '';
          v_sql3 clob default '';
          v_final_string varchar2(30000);
          v_str varchar(32000);

        BEGIN

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_cgc12:=P_INPUTSTRING;

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

           v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';


          v_sql_final:=v_sql||v_cgc12||v_sql2||v_cgc12||v_sql3||v_cgc12||v_sql4||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

            v_sql:='select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''1'' AS ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
                      a.Streetnumber is null and
                      a.StreetName1 is null and
                      a.StreetName2 is null and
                      a.Zip is null and
                      a.City is null and
                      a.State is null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            upper(A.accountnum) as ACCOUNTID,
            A.DIVISION,
            '''||i_user||''' as CURRUSER,
            upper(A.METERNUMBER) as METERNUMBER,
            ''2'' as ORD,  upper(a.PREMISEID)as PREMISEID,
            upper(a.PREMISETYPE)as PREMISETYPE
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

          v_sql_final:=v_sql||v_cgc12||v_sql3||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

          END IF;

          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END CUST_INFO_CGC_EXCL;

   ----------------------------------------------------------------------------------------

   PROCEDURE GETDIVISION_FEEDER(
            P_DIVISION IN NUMBER,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS
        v_sql VARCHAR2(2000);

    BEGIN
      -- Change the alias name TX_BANK_CIRCUITID  to TX_BANK_CODE once the changes have been deployed from the code
        v_sql := 'SELECT
              SUB.STATIONNUMBER,
              PCS.SUBSTATIONNAME,
              '||''''||'Bank '||''''||'|| NVL(PCS.TX_BANK_CODE,0) TX_BANK_CODE,
              PCS.TO_CIRCUITID
          FROM
              EDGIS.PGE_CIRCUITSOURCE PCS,
              EDGIS.ELECTRICSTITCHPOINT ESP,
              EDGIS.SUBSTATION SUB
          WHERE
              PCS.SUBSTATION_GLOBALID = SUB.GLOBALID
              AND
              PCS.TO_POINT_FC_GUID = ESP.GLOBALID
              AND
              ESP.SUBTYPECD = 2
              AND
              SUB.DIVISION = '||P_DIVISION||'
              AND
              PCS.TO_CIRCUITID IS NOT NULL
          ORDER BY
              PCS.SUBSTATIONNAME,
              PCS.TX_BANK_CODE,
              PCS.TO_CIRCUITID
          ';

open p_cursor for v_sql ;
            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END GETDIVISION_FEEDER;

      
    ------------------ GET CUSTOMER INFORMATION FOR LOADING INFORMATION WINDOW USING CGC NUMBERS ------------
PROCEDURE LOADINGINFO_CUSTOMER_CGC(
             P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER)
   AS

          v_cgc12 VARCHAR2(30000);
          i_version VARCHAR2(15);
          I_USER VARCHAR2(10);
          v_sql clob default '';
          v_sql2 clob default '';
          v_sql4 clob default '';
          v_sql_final clob default '';
          v_sql3 clob default '';
          v_final_string varchar2(30000);
          v_str varchar(32000);

        BEGIN

          if upper(i_version) <> 'SDE.DEFAULT' then
           i_version:=P_VERSION;
          end if;

          select user into i_user from dual;
          v_cgc12:=P_INPUTSTRING;

          if i_version is null then

          v_sql:='select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            ''1'' AS ORD
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            ''2'' as ORD 
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

           v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            ''1'' AS ORD  
            from  EDGIS.SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            ''2'' as ORD 
            from  EDGIS.SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.SERVICEPOINT where cgc12 in (';


          v_sql_final:=v_sql||v_cgc12||v_sql2||v_cgc12||v_sql3||v_cgc12||v_sql4||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';

          ELSE

          sde.version_util.set_current_version(i_version);

            v_sql:='select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            ''1'' AS ORD
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_TRANSFORMER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql2:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            ''2'' as ORD  
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_TRANSFORMER B
            ON
            A.CGC12=B.CGC12
            where
            (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
            )
            and
            b.cgc12 in ('||v_cgc12||')
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql3:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            UPPER(C.MAILNAME1) as MAILNAME1,
            UPPER(C.MAILNAME2) as MAILNAME2,
            decode( C.COMMUNICATIONPREFERENCE,null,null,
                        decode(C.LIFESUPPORTIDC, ''L'', ''*'',
                              decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL)
                              )
                   ) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) as CUSTOMERTYPE,
            upper(c.MailStreetNum) as MailStreetNum,
            upper(c.MailStreetName1) as MailStreetName1,
            upper(c.MailStreetName2) as MailStreetName2,
            upper(c.MailZipCode) as MailZipCode,
            upper(c.MailCity) as MailCity,
            upper(c.MailState) as MailState,
            CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END AS PHONENUM,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId)as SourceSideDeviceId,
            ''1'' AS ORD 
            from  EDGIS.ZZ_MV_SERVICEPOINT a,CUSTOMER.CUSTOMER_INFO C,EDGIS.ZZ_MV_PRIMARYMETER B
            where
            a.cgc12=b.cgc12
            and
            a.servicepointid=C.SERVICEPOINTID
            and
            (
            c.MailStreetNum is not null or
            c.MailStreetName1 is not null or
            c.MailStreetName2 is not null or
            c.MailZipCode is not null or
            C.MailCity is not null or
            c.MailState is not null
            )
            and
            a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

            v_sql4:=')) UNION
            select
            upper(a.servicepointid) as servicepointid, a.globalid as SERVIEPOINTGLOBALID,
            CASE when (UPPER(C.MAILNAME1)) is null then UPPER(A.MAILNAME1) else UPPER(C.MAILNAME1) end as MAILNAME1,
            CASE when (UPPER(C.MAILNAME2)) is null then UPPER(A.MAILNAME2) else UPPER(C.MAILNAME2) end as MAILNAME2,
            CASE when (decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC))
            is null then A.CUSTOMERTYPE
            else decode( C.COMMUNICATIONPREFERENCE,null,null,decode(C.LIFESUPPORTIDC, ''L'', ''*'',decode(C.MEDICALBASELINEIDC, ''M'', ''*'', NULL))) || A.CUSTOMERTYPE ||decode(C.LIFESUPPORTIDC, ''L'', C.LIFESUPPORTIDC, C.MEDICALBASELINEIDC) end as CUSTOMERTYPE,
            upper(a.StreetNumber) as StreetNumber,
            upper(a.StreetName1) as StreetName1,
            upper(a.StreetName2) as StreetName2,
            upper(a.Zip) as Zip,
            upper(a.City) as City,
            upper(a.State) as State,
            CASE WHEN (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END)
            is null then (CASE WHEN C.AREACODE is not null THEN (C.areacode||'' ''||(CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END)) ELSE (CASE WHEN LENGTH(C.PHONENUM)=7 THEN (SUBSTR(C.PHONENUM,1,3)||'' ''||SUBSTR(C.PHONENUM,4,7)) ELSE C.PHONENUM END) END)
            else (CASE WHEN A.AREACODE is not null THEN (A.areacode||'' ''||(CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END)) ELSE (CASE WHEN LENGTH(A.PHONENUM)=7 THEN (SUBSTR(A.PHONENUM,1,3)||'' ''||SUBSTR(A.PHONENUM,4,7)) ELSE A.PHONENUM END) END) end AS PHONENUM ,
            CASE WHEN UPPER(B.OPERATINGNUMBER) IS NULL THEN CAST(B.CGC12 AS NVARCHAR2(12)) ELSE UPPER(B.OPERATINGNUMBER) END AS CGC12,
            upper(b.SourceSideDeviceId) as SourceSideDeviceId,
            ''2'' as ORD
            from  EDGIS.ZZ_MV_SERVICEPOINT a
            LEFT JOIN CUSTOMER.CUSTOMER_INFO C
            ON
            A.SERVICEPOINTID=C.SERVICEPOINTID
            left JOIN EDGIS.ZZ_MV_PRIMARYMETER B
            ON
            A.CGC12=B.CGC12
            where
          (
                a.Streetnumber is not null or
                a.Streetname1 is not null or
                a.Streetname2 is not null or
                a.Zip is not null or
                a.City is not null or
                a.State is not null
          )
          and
          b.cgc12 in ('||v_cgc12||')
          and
          a.servicepointid in (select servicepointid from EDGIS.ZZ_MV_SERVICEPOINT where cgc12 in (';

          v_sql_final:= v_sql||v_cgc12||v_sql3||v_cgc12||')) order by Sourcesidedeviceid,CGC12,servicepointid,ORD,mailstreetname1,mailstreetname2,mailstreetnum';
          

          END IF;

          open p_cursor for v_sql_final ;

            p_error                                       := 'NO ORA ERRORS';
            p_success                                     := 1;
            dbms_output.put_line(p_success);

          EXCEPTION WHEN OTHERS THEN
          p_success := 0;
          p_error   := sqlerrm;
          dbms_output.put_line(p_success);
          dbms_output.put_line(p_error);

   END LOADINGINFO_CUSTOMER_CGC;
    
    ----------------------------------------------------------------------------------------
end PONS;

/

