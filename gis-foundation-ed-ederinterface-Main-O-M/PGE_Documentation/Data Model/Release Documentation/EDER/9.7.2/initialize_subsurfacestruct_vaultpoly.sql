set trimspool on
set linesize 160
set timing on
set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';

select count(*) from edgis.vaultpoly where structureguid in (select globalid from edgis.subsurfacestructure);

drop table temp_vault_struct;
create table temp_vault_struct (vglobalid char(38),sglobalid char(38));

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb1.globalid from edgis.vaultpoly vp
inner join
(select * from edgis.subsurfacestructure sb2
where sb2.structurenumber not in (
    select va.structurenumber
    from vaultpoly va 
    join subsurfacestructure sb 
	on va.STRUCTURENUMBER=sb.STRUCTURENUMBER 
	where va.status=5
    group by va.structurenumber 
    having count(*)>1 )
) sb1
on vp.structurenumber=sb1.structurenumber
where vp.status=5
group by vp.globalid,sb1.globalid
order by vp.globalid,sb1.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;

insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb1.globalid from 
(
 select 
 vp4.OBJECTID,
 vp4.GLOBALID,
 vp4.STRUCTURENUMBER,
 vp4.DISTRICT,
 vp4.DIVISION,
 vp4.STATUS
from edgis.vaultpoly vp4
    where 
 vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    ) 
 and vp4.status=5
) vp
inner join
(
  select 
  sb2.OBJECTID,
  sb2.GLOBALID,
  sb2.STRUCTURENUMBER,
  sb2.DISTRICT,
  sb2.DIVISION 
    from 
  edgis.subsurfacestructure sb2
  where sb2.structurenumber not in (
    select va.structurenumber
    from vaultpoly va 
    join subsurfacestructure sb 
	on va.STRUCTURENUMBER=sb.STRUCTURENUMBER and va.DISTRICT=sb.DISTRICT
	where va.status=5
    group by va.STRUCTURENUMBER,va.DISTRICT
    having count(*)>1 
	)
  and 
  sb2.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
) sb1
on vp.STRUCTURENUMBER=sb1.STRUCTURENUMBER and vp.DISTRICT=sb1.DISTRICT
where vp.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    ) 
and sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
and vp.status=5	
group by vp.globalid,sb1.globalid
order by vp.globalid,sb1.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;


insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb.globalid 
from 
(
 select * from 
   edgis.vaultpoly vp4
 where 
   vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    )
  and vp4.status=5	
) vp
inner join
(
 select * from 
   edgis.subsurfacestructure sb1
 where 
   sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
   and 
   sb1.subtypecd=3
) sb
on sde.st_envintersects(vp.shape,sb.shape)=1
where vp.status=5
group by vp.globalid,sb.globalid
having count(*)=1
order by vp.globalid,sb.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;

insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb.globalid 
from 
(
 select * from 
   edgis.vaultpoly vp4
 where 
   vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    ) 
  and vp4.status=5
) vp
inner join
(
 select * from 
   edgis.subsurfacestructure sb1
 where 
   sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
   and 
   sb1.subtypecd=6
) sb
on sde.st_envintersects(vp.shape,sb.shape)=1
where vp.status=5
group by vp.globalid,sb.globalid
having count(*)=1
order by vp.globalid,sb.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;

insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb.globalid 
from 
(
 select * from 
   edgis.vaultpoly vp4
 where 
   vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    )
  and vp4.status=5	
) vp
inner join
(
 select * from 
   edgis.subsurfacestructure sb1
 where 
   sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
   and 
   sb1.subtypecd=7
) sb
on sde.st_envintersects(vp.shape,sb.shape)=1
where vp.status=5
group by vp.globalid,sb.globalid
having count(*)=1
order by vp.globalid,sb.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;

insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb.globalid 
from 
(
 select * from 
   edgis.vaultpoly vp4
 where 
   vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    ) 
 and vp4.status=5	
) vp
inner join
(
 select * from 
   edgis.subsurfacestructure sb1
 where 
   sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
   and 
   sb1.subtypecd=5
) sb
on sde.st_envintersects(vp.shape,sb.shape)=1
where vp.status=5
group by vp.globalid,sb.globalid
having count(*)=1
order by vp.globalid,sb.globalid ;

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;

/* insert into temp_vault_struct (vglobalid,sglobalid) 
select vp.globalid,sb.globalid 
from 
(
 select * from 
   edgis.vaultpoly vp4
 where 
   vp4.globalid not in ( 
    select vglobalid 
	   from 
	edgis.temp_vault_struct 
    ) 
 and vp4.status=5	
) vp
inner join
(
 select * from 
   edgis.subsurfacestructure sb1
 where 
   sb1.globalid not in ( 
    select sglobalid 
	   from 
	edgis.temp_vault_struct  )
   and 
   sb1.subtypecd not in (3,5,6,7)
) sb
on sde.st_envintersects(vp.shape,sb.shape)=1
where vp.status=5
group by vp.globalid,sb.globalid
having count(*)=1
order by vp.globalid,sb.globalid ; */

select vglobalid,count(*) from temp_vault_struct group by vglobalid having count(*)>1;
select sglobalid,count(*) from temp_vault_struct group by sglobalid having count(*)>1;

commit;


delete from temp_vault_struct where vglobalid in (
select vglobalid from temp_vault_struct group by vglobalid having count(*)>1 )
or 
sglobalid in (
select sglobalid from temp_vault_struct group by sglobalid having count(*)>1
)
or sglobalid is null
or vglobalid is null;


MERGE INTO edgis.vaultpoly vp
USING (
    select vglobalid,sglobalid structguid
	from temp_vault_struct
) vs
on (vp.globalid=vs.vglobalid)
when matched then update set vp.structureguid=vs.structguid
where exists (select 1 from edgis.vaultpoly where structureguid is null);
commit;

drop table temp_vault_struct ;

select count(*) from edgis.vaultpoly where structureguid in (select globalid from edgis.subsurfacestructure);

