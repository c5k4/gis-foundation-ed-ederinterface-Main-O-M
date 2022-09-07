Prompt drop Procedure JET_DATA_MIGRATION;
DROP PROCEDURE WEBR.JET_DATA_MIGRATION
/

Prompt Procedure JET_DATA_MIGRATION;
--
-- JET_DATA_MIGRATION  (Procedure) 
--
CREATE OR REPLACE PROCEDURE WEBR."JET_DATA_MIGRATION"
as
   V_OBJECTID            JET_EQUIPIDTYPE.OBJECTID%Type;
   V_EQUIPMENT_TYPE_ID   JET_EQUIPIDTYPE.EQUIPTYPEID%Type;
   V_EQUIPMENT_TYPE_DESC JET_EQUIPIDTYPE.EQUIPTYPEDESC%Type;
   V_Registration_id     SDE.Table_Registry.registration_id%Type;
   V_Sequence_Types      Varchar2(30);
   V_Sequence_Rules      Varchar2(30);
   V_Sql                 Varchar2(2000);
   V_Row_Count           Number;
begin
   execute immediate 'truncate table Jet_Jobs';
   execute immediate 'Truncate table Jet_EQUIPMENT';

   select registration_id into V_Registration_id from sde.table_registry where table_name='JET_JOBS';
   V_Sequence_Types :=  'R' || V_Registration_id;
   dbms_output.put_line(V_Sequence_Types);

   V_sql := 'Insert into Jet_Jobs (OBJECTID,JOBNUMBER,RESERVEDBY,DIVISION,DESCRIPTION,ENTRYDATE,LASTMODIFIEDDATE,USERAUDIT,STATUS ) ' ;
   V_sql := V_sql || ' Select '   || V_Sequence_Types  || '.nextval , JOB_#, ';
   V_sql := V_sql || ' decode(LENGTH(ltrim(rtrim(user_audit))),4,user_audit,substr(user_audit,instr(user_audit,' || ''',''' || ',-1)+1,4)) RESERVED_BY ';
   V_sql := V_sql || ', DIV_#, DESCRIPTION, ENTRY_DATE, LAST_MODIFIED, USER_AUDIT, 1 ';
   V_sql := V_sql || ' From  EQUIP_NBR_JOBS ';
   V_sql := V_sql || ' Where Job_# is Not Null  ';
   dbms_output.put_line(V_SQL);
   Execute Immediate (V_SQL);

   select registration_id into V_Registration_id from sde.table_registry where table_name='JET_EQUIPMENT';
   V_Sequence_Types :=  'R' || V_Registration_id;
   dbms_output.put_line(V_Sequence_Types);

   V_sql := ' Insert into Jet_EQUIPMENT(OBJECTID,EQUIPTYPEID,JOBNUMBER,OPERATINGNUMBER,ADDRESS,SKETCHLOC,INSTALLCD, ';
   V_sql := V_sql  || 'CITY, ENTRYDATE,LASTMODIFIEDDATE,USERAUDIT,CUSTOWNED,LATITUDE, LONGITUDE,status ) ';
   V_sql := V_sql || ' Select '   || V_Sequence_Types  || '.nextval , decode(a.ID_TYP, 5,16,   ';
   V_sql := V_sql || '                                                                4,15,   ';
   V_sql := V_sql || '                                                                51,12,  ';
   V_sql := V_sql || '                                                                26,19,  ';
   V_sql := V_sql || '                                                                13,5,   ';
   V_sql := V_sql || '                                                                55,9,   ';
   V_sql := V_sql || '                                                                58,13,  ';
   V_sql := V_sql || '                                                                53,2,   ';
   V_sql := V_sql || '                                                                56,11,  ';
   V_sql := V_sql || '                                                                57,10,  ';
   V_sql := V_sql || '                                                                7,4,    ';
   V_sql := V_sql || '                                                                3,14,   ';
   V_sql := V_sql || '                                                                8,3,    ';
   V_sql := V_sql || '                                                                9,17,   ';
   V_sql := V_sql || '                                                                54,6,   ';
   V_sql := V_sql || '                                                                52,1,   ';
   V_sql := V_sql || '                                                                15,18,   ';
   V_sql := V_sql || '                                                                59,7,    ';
   V_sql := V_sql || '                                                                60,8, 0 ), ' ;
   V_sql := V_sql || ' b.Job_#,a.OPER_#,a.ADDRESS,a.SKETCH_LOC,a.INSTALL_CD,a.CITY,a.ENTRY_DATE, ';
   V_sql := V_sql || ' a.LAST_MODIFIED,a.USER_AUDIT,a.CUST_OWNED, 0, 0, 1  ' ;
   V_sql := V_sql || ' from EQUIP_NBRS_RESERVED a, EQUIP_NBR_JOBS b ' ;
   V_sql := V_sql || ' Where b.id = a.id     ';
   V_sql := V_SQL || '   and b.Job_# is not null ';
   dbms_output.put_line(V_SQL);
   Execute Immediate (V_SQL);

   select count(*) into V_Row_Count from EQUIP_NBR_JOBS;
   dbms_output.put_line('No of rows in EQUIP_NBR_JOBS  :  ' || V_Row_Count );

   select count(*) into V_Row_Count from EQUIP_NBR_JOBS Where Job_# is Null;
   dbms_output.put_line('No of Null rows in EQUIP_NBR_JOBS  :  ' || V_Row_Count );

   select count(*) into V_Row_Count from EQUIP_NBRS_RESERVED;
   dbms_output.put_line('No of rows in EQUIP_NBRS_RESERVED  :  ' || V_Row_Count );



   select count(*) into V_Row_Count from Jet_Jobs;
   dbms_output.put_line('No of rows in Jet_Jobs  :  ' || V_Row_Count );
   select count(*) into V_Row_Count from Jet_EQUIPMENT;
   dbms_output.put_line('No of rows migrated in  Jet_EQUIPMENT  :  ' || V_Row_Count );

End;

/
