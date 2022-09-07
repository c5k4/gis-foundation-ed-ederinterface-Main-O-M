create table SUBCircuitSource as (select * from circuitsource);
alter table SUBCircuitSource drop column objectid;
alter table SUBCircuitSource drop column Globalid;
alter table SUBCircuitSource rename column deviceguid to deviceguid2;
commit;
