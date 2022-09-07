--------------------------------------------------------
--  DDL for Procedure ED008_POPULATESTAGING
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."ED008_POPULATESTAGING" 
IS
delete_count number;
insert_count number;
v_count number;

BEGIN

--Logging
INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
Values (sysdate,'I','ED008_PopulateStaging Stored Procedure Started');


--Initialiing Value
delete_count:=0;
insert_count:=0;

select count(*) into v_count from (
select
   wo.type_op, wo.sapequipid,wo.structureglobalid,wo.SAP_TYPE,wo.circuitid
from (
    select 'INSERT' as type_op, w.sapequipid,w.structureglobalid,w.circuitid,
                DECODE(w.STRUCTURESUBTYPE,'Pad','ED_PADM',
                'Pole Stub','ED_POLE',
                'Vault','ED_VLTS',
                'PrimaryAndSecondaryEnclosure','ED_ENCL',
                'Pole','ED_POLE',
                'Primary Enclosure','ED_ENCL',
                'Electrolier','ED_POLE',
                'Push Brace','ED_POLE',
                'Other Structure','ED_POLE',
                'Pedestal','ED_PADM',
                'Secondary Enclosure','ED_ENCL',
                'DuctJunction','ED_ENCL',
                'Guy Stub','ED_POLE',
                'Tree','ED_POLE',
                'Streetlight Box','ED_ENCL'
                ,NULL) as SAP_TYPE
                from workorderstructure w
                where
     w.structureglobalid in
        (
          select distinct wo2.structureglobalid
          from workorderstructure wo2
		minus
			select distinct wo9.structureglobalid
          from workorderstructure wo9
          where
           (wo9.structureglobalid,wo9.circuitid)
            in (
            select wo10.structureglobalid,wo10.circuitid
            from workorderstructure_prev wo10
            )
         )
) wo
where wo.SAP_TYPE is not null
union
select
 wo7.type_op,wo7.sapequipid,wo7.structureglobalid,wo7.SAP_TYPE,' ' as circuitid
    from (
       select  'DELETE' as type_op,w4.sapequipid,w4.structureglobalid,
        DECODE(w4.STRUCTURESUBTYPE,
    'Pad','ED_PADM',
    'Pole Stub','ED_POLE',
    'Vault','ED_VLTS',
    'PrimaryAndSecondaryEnclosure','ED_ENCL',
    'Pole','ED_POLE',
    'Primary Enclosure','ED_ENCL',
    'Electrolier','ED_POLE',
    'Push Brace','ED_POLE',
    'Other Structure','ED_POLE',
    'Pedestal','ED_PADM',
    'Secondary Enclosure','ED_ENCL',
    'DuctJunction','ED_ENCL',
    'Guy Stub','ED_POLE',
    'Tree','ED_POLE',
    'Streetlight Box','ED_ENCL',
    NULL) as SAP_TYPE
     from workorderstructure_prev w4 where w4.structureglobalid not in
          (
			  select DISTINCT(wo11.structureglobalid) from workorderstructure wo11
          )
    ) wo7 where wo7.SAP_TYPE is not null
    group by wo7.type_op,wo7.structureglobalid,wo7.sapequipid,wo7.SAP_TYPE);



FOR cur in (
select
   wo.type_op, wo.sapequipid,wo.structureglobalid,wo.SAP_TYPE,wo.circuitid
from (
    select 'INSERT' as type_op, w.sapequipid,w.structureglobalid,w.circuitid,
                DECODE(w.STRUCTURESUBTYPE,'Pad','ED_PADM',
                'Pole Stub','ED_POLE',
                'Vault','ED_VLTS',
                'PrimaryAndSecondaryEnclosure','ED_ENCL',
                'Pole','ED_POLE',
                'Primary Enclosure','ED_ENCL',
                'Electrolier','ED_POLE',
                'Push Brace','ED_POLE',
                'Other Structure','ED_POLE',
                'Pedestal','ED_PADM',
                'Secondary Enclosure','ED_ENCL',
                'DuctJunction','ED_ENCL',
                'Guy Stub','ED_POLE',
                'Tree','ED_POLE',
                'Streetlight Box','ED_ENCL'
                ,NULL) as SAP_TYPE
                from workorderstructure w
                where
     w.structureglobalid in
        (
          select distinct wo2.structureglobalid
          from workorderstructure wo2
		minus
			select distinct wo9.structureglobalid
          from workorderstructure wo9
          where
           (wo9.structureglobalid,wo9.circuitid)
            in (
            select wo10.structureglobalid,wo10.circuitid
            from workorderstructure_prev wo10
            )
         )
) wo
where wo.SAP_TYPE is not null
union
select
 wo7.type_op,wo7.sapequipid,wo7.structureglobalid,wo7.SAP_TYPE,' ' as circuitid
    from (
       select  'DELETE' as type_op,w4.sapequipid,w4.structureglobalid,
        DECODE(w4.STRUCTURESUBTYPE,
    'Pad','ED_PADM',
    'Pole Stub','ED_POLE',
    'Vault','ED_VLTS',
    'PrimaryAndSecondaryEnclosure','ED_ENCL',
    'Pole','ED_POLE',
    'Primary Enclosure','ED_ENCL',
    'Electrolier','ED_POLE',
    'Push Brace','ED_POLE',
    'Other Structure','ED_POLE',
    'Pedestal','ED_PADM',
    'Secondary Enclosure','ED_ENCL',
    'DuctJunction','ED_ENCL',
    'Guy Stub','ED_POLE',
    'Tree','ED_POLE',
    'Streetlight Box','ED_ENCL',
    NULL) as SAP_TYPE
     from workorderstructure_prev w4 where w4.structureglobalid not in
          (
			  select DISTINCT(wo11.structureglobalid) from workorderstructure wo11
          )
    ) wo7 where wo7.SAP_TYPE is not null
    group by wo7.type_op,wo7.structureglobalid,wo7.sapequipid,wo7.SAP_TYPE
)



LOOP
  IF CUR.TYPE_OP = 'DELETE' THEN
    INSERT INTO PGEDATA.PGE_ED08_STAGING (RECORDID,CREATIONDATE,PROCESSEDFLAG,GUID,SAPEQUIPMENTID,CLASSNAME,CHARACTERISTICNAME,CLEARINDICATOR)
    Values('ED.0.08.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(ED08_RECORDID_SEQ.nextval,7,0),sysdate,'P',cur.structureglobalid,cur.sapequipid,cur.SAP_TYPE,'CIRCUIT_ID','X');
    delete_count := delete_count + 1;
  ELSE
    INSERT INTO PGEDATA.PGE_ED08_STAGING (RECORDID,CREATIONDATE,PROCESSEDFLAG,GUID,SAPEQUIPMENTID,CLASSNAME,CHARACTERISTICNAME,VALUE)
    Values('ED.0.08.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(ED08_RECORDID_SEQ.nextval,7,0),sysdate,'P',cur.structureglobalid,cur.sapequipid,cur.SAP_TYPE,'CIRCUIT_ID',cur.circuitid);
    insert_count := insert_count + 1;
  END IF;
  END Loop;
commit;
--Logging
INSERT INTO EDGISBO.ED08LOG (datetime,type,logger)
Values (sysdate,'I','ED008_PopulateStaging Processed Insert Count :: ' || insert_count || ' and Delete Count :: ' || delete_count);

IF v_count > 30000 or v_count <5000  THEN
raise_application_error(-20101, 'Job has been done but records counts are not in expected range.');

END IF;

END;
