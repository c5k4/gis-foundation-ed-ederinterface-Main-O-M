--------------------------------------------------------
--  DDL for Procedure SUPPORTSTRUCTUREINSERT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SUPPORTSTRUCTUREINSERT" (clob_sapattributes IN CLOB)
as

v_lanid nvarchar2(24);
v_installjobnumber nvarchar2(28);
n_pldbid number(38,8);
v_pldbid varchar2(20);
v_installationdate varchar2(10);
v_guid varchar2(255);
c_actiontype char(2);
cl_sapattributes clob;
v_assetid varchar2(5);
v_assettype varchar2(255);
p_installjobnumber number(5);
p_installationdate number(5);
p_guid number(5);
p_pldbid number(5);
p_lanid number(5);
n_dqcheck number(3);
n_commascount number(3);
n_count number(3);
v_errormsg nvarchar2(255);
v_isnumber varchar2(5);

begin

--Filtering CLOB value
n_dqcheck:=instr(clob_sapattributes,'"');

--Filtering CLOB value
if n_dqcheck=0 then
  cl_sapattributes:=clob_sapattributes;
else
  cl_sapattributes:=replace(replace(clob_sapattributes,substr(clob_sapattributes,instr(clob_sapattributes,'"',1,1)+1,instr(clob_sapattributes,'"',1,2)-instr(clob_sapattributes,'"',1,1)-1)),'(CLOB) ');
end if;

v_errormsg :='';
v_lanid :='0';
v_installjobnumber:='0';
n_pldbid:=0;
v_installationdate:='0';
v_guid:='0';
c_actiontype:='0';
v_assetid:='0';
v_assettype:='0';

--Counting number of commas
n_commascount:=regexp_count(cl_sapattributes,',');
DBMS_OUTPUT.put_LINE('Number of commas: '||to_char(n_commascount));

--Getting Asset ID
v_assetid:=trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,1)+1,instr(cl_sapattributes,',',1,2)-instr(cl_sapattributes,',',1,1)-1));
DBMS_OUTPUT.put_LINE('ASSET ID: '||v_assetid);

--Filtering out the assets
select count(1) into n_count from PGE_RW_CONFIGURATIONS where ASSETID=v_assetid;
if n_count=1 then
  select assettype,installjobnumber,installationdate,guid,pldbid,lanid into v_assettype,p_installjobnumber,p_installationdate,p_guid,p_pldbid,p_lanid from PGE_RW_CONFIGURATIONS where to_char(assetid)=v_assetid;
  select trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_installjobnumber-1)+1,instr(cl_sapattributes,',',1,p_installjobnumber)-instr(cl_sapattributes,',',1,p_installjobnumber-1)-1)) into v_installjobnumber from dual;

  --Checking the validity of Install Job Number
  if substr(v_installjobnumber,1,1)='1' and length(v_installjobnumber)=9 then

    if p_lanid-1=n_commascount then
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_lanid-1)+1,length(cl_sapattributes)-instr(cl_sapattributes,',',1,p_lanid-1)))) into v_lanid from dual;
    else
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_lanid-1)+1,instr(cl_sapattributes,',',1,p_lanid)-instr(cl_sapattributes,',',1,p_lanid-1)-1))) into v_lanid from dual;
    end if;

    DBMS_OUTPUT.put_LINE('LAN ID: '||v_lanid);

    if v_lanid='0' then
      v_lanid:=null;
    end if;

    DBMS_OUTPUT.put_LINE('Install Job Number: '||v_installjobnumber);

    if p_pldbid is null then
      n_pldbid:=null;
    else
      if p_pldbid-1=n_commascount then
        select trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_pldbid-1)+1,length(cl_sapattributes)-instr(cl_sapattributes,',',1,p_pldbid-1))) into v_pldbid from dual;
      else
        select trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_pldbid-1)+1,instr(cl_sapattributes,',',1,p_pldbid)-instr(cl_sapattributes,',',1,p_pldbid-1)-1)) into v_pldbid from dual;
      end if;
      --DBMS_OUTPUT.put_LINE('PLDBID: '||to_char(n_pldbid));

      v_isnumber:=IS_NUMBER(v_pldbid);
      if v_isnumber='FALSE' then
        v_errormsg:='Invalid PLDBID: '||v_pldbid||';';
        n_pldbid:=null;
      else
        n_pldbid:=to_number(v_pldbid);
      end if;
    end if;

    if n_pldbid=0 then
      n_pldbid:=null;
    end if;

    if p_installationdate-1=n_commascount then
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_installationdate-1)+1,length(cl_sapattributes)-instr(cl_sapattributes,',',1,p_installationdate-1)))) into v_installationdate from dual;
    else
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_installationdate-1)+1,instr(cl_sapattributes,',',1,p_installationdate)-instr(cl_sapattributes,',',1,p_installationdate-1)-1))) into v_installationdate from dual;
    end if;

    v_isnumber:=IS_NUMBER(v_installationdate);

    if v_installationdate='0' then
      v_installationdate:=null;
    elsif length(v_installationdate)<8 or length(v_installationdate)>8 then
      v_errormsg:=v_errormsg||' Invalid Installation Date: '||v_installationdate||';';
      v_installationdate:=null;
    elsif v_isnumber='FALSE' then
      v_errormsg:=v_errormsg||' Invalid Installation Date: '||v_installationdate||';';
      v_installationdate:=null;
    end if;


    if p_guid-1=n_commascount then
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_guid-1)+1,length(cl_sapattributes)-instr(cl_sapattributes,',',1,p_guid-1)))) into v_guid from dual;
    else
      select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,p_guid-1)+1,instr(cl_sapattributes,',',1,p_guid)-instr(cl_sapattributes,',',1,p_guid-1)-1))) into v_guid from dual;
    end if;

    if v_guid='0' then
      v_guid:=null;
    elsif length(v_guid)<>38 then
      v_errormsg:=v_errormsg||' Invalid GUID : '||v_guid||';';
      v_guid:=null;
    end if;

    select Upper(trim(substr(cl_sapattributes,instr(cl_sapattributes,',',1,1)-1,1))) into c_actiontype from dual;
    DBMS_OUTPUT.put_LINE('Action Type: '||c_actiontype);

    if c_actiontype='0' then
      c_actiontype:=null;
    end if;

    DBMS_OUTPUT.put_LINE('ERROR MSG: '||v_errormsg);

    INSERT INTO PGE_SUPPORTSTRUCTURE_RW(LANID,INSTALLJOBNUMBER,PLDBID,RW_DATE,SAPATTRIBUTES,GUID,ASSETTYPE,ACTIONTYPE) VALUES (v_lanid,v_installjobnumber,n_pldbid,to_date(v_installationdate,'YYYYMMDD'),clob_sapattributes,v_guid,v_assettype,c_actiontype);

    if LENGTH(v_errormsg)<>0 then
      INSERT INTO PGE_SUPPORTSTRUCTURE_RW_ERROR(ASSETTYPE,INSTALLJOBNUMBER,ERRORMSG,SAPATTRIBUTES,CREATIONDATE) VALUES (v_assettype,v_installjobnumber,v_errormsg,clob_sapattributes,SYSDATE);
    END IF;

    commit;

  else
  DBMS_OUTPUT.put_LINE('Install Job Number criteria not met');
  end if;

else
DBMS_OUTPUT.put_LINE('Asset ID not valid');
end if;

EXCEPTION WHEN OTHERS THEN

DBMS_OUTPUT.put_LINE('Error');

end;
