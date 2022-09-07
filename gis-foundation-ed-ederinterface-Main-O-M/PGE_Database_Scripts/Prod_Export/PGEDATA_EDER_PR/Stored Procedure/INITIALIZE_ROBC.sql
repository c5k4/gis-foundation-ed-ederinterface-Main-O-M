--------------------------------------------------------
--  DDL for Procedure INITIALIZE_ROBC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."INITIALIZE_ROBC" 
As
   V_OBJECTID            EDGIS.ROBC.OBJECTID%Type;
   V_Registration_id     SDE.Table_Registry.registration_id%Type;
   V_Sequence_Types      Varchar2(30);
   V_Sequence_Rules      Varchar2(30);
   V_Sql                 Varchar2(2000);
   V_Globalid            EDGIS.ROBC.Globalid%type;
   V_DESIREDROBC         CEDSA_CIRCUIT_ROBCS.ROBC%type;
   V_desiredsubblock     CEDSA_CIRCUIT_ROBCS.subblock%type;
   V_Row_Count           Number;

   CURSOR insert_cur is
   select  b.GLOBALID ,decode(a.ROBC,'F',60,a.ROBC) , a.SUBBLOCK from CEDSA_CIRCUIT_ROBCS a, EDGIS.CIRCUITSOURCE b
    where a.ROBC is not null
      and b.CIRCUITID = decode(length(ltrim(rtrim(a.FEEDER_NUM))),9,a.FEEDER_NUM,'0' || ltrim(rtrim(a.FEEDER_NUM)))
      and a.FEEDER_NUM NOT in ( select feeder_num from ( select a.FEEDER_NUM, count(*) reccount
                                                           from CEDSA_CIRCUIT_ROBCS a, EDGIS.CIRCUITSOURCE b
                                                          where b.CIRCUITID = decode(length(ltrim(rtrim(a.FEEDER_NUM))),9,a.FEEDER_NUM,'0' || ltrim(rtrim(a.FEEDER_NUM)))
                                                          group by a.FEEDER_NUM having count(*) > 1 ));
begin
   select registration_id into V_Registration_id from sde.table_registry where table_name='ROBC';
   V_Sequence_Types :=  'R' || V_Registration_id;
   dbms_output.put_line(V_Sequence_Types);

   Open insert_cur;
   Fetch insert_cur into V_Globalid,V_DESIREDROBC,V_desiredsubblock;

   While insert_cur%found Loop
        V_Sql := 'insert into EDGIS.ROBC (OBJECTID,GLOBALID, CIRCUITSOURCEGUID,DESIREDROBC, desiredsubblock) ';
        V_sql := V_sql || ' values (edgis.'   || V_Sequence_Types  || '.nextval ,edgis.gdb_guid(), ''' || V_Globalid || ''',''';
        V_sql := V_sql ||  V_DESIREDROBC || ''',''' || nvl(V_desiredsubblock,'') || ''')' ;

--        dbms_output.put_line(V_sql);
        execute immediate (v_sql);
        Fetch insert_cur into V_Globalid,V_DESIREDROBC,V_desiredsubblock;
   End loop;

   V_Row_Count := 0;
   Select count(*) into V_Row_Count from  CEDSA_CIRCUIT_ROBCS;
   dbms_output.put_line( 'Rows in the CSV file : ' || V_Row_Count );
   Select count(*) into V_Row_Count from CEDSA_CIRCUIT_ROBCS where robc is null and subblock is null;
   dbms_output.put_line( 'Rows have Null value for ROBC and Subblock the CSV file : ' || V_Row_Count );

   Select count(*) into V_Row_Count from EDgis.ROBC ;
   dbms_output.put_line( 'Rows migrated in the EDGIS ( ROBC table ) : ' || V_Row_Count );

End;
