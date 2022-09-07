
  CREATE TABLE "PGEDATA"."PGE_CCB_SP_IO_MONITOR" 
   (	"ID" NUMBER NOT NULL ENABLE, 
	"INTERFACETYPE" VARCHAR2(25 BYTE) NOT NULL ENABLE, 
	"STATUS" VARCHAR2(25 BYTE), 
	"DATEMODIFIED" DATE DEFAULT SYSDATE, 
	 CONSTRAINT "PGE_CCB_SP_IO_MONITOR_PK" PRIMARY KEY ("ID")
   ) 
  TABLESPACE "PGE" ;

   COMMENT ON COLUMN "PGEDATA"."PGE_CCB_SP_IO_MONITOR"."INTERFACETYPE" IS 'Inbound or Outbound';
   COMMENT ON COLUMN "PGEDATA"."PGE_CCB_SP_IO_MONITOR"."STATUS" IS 'In-Progress or Insert-Completed';

  CREATE OR REPLACE TRIGGER "PGEDATA"."PGE_CCB_SP_IO_MONITOR_INS" BEFORE INSERT OR UPDATE ON "PGEDATA".PGE_CCB_SP_IO_MONITOR
FOR EACH ROW
BEGIN
  :new.DATEMODIFIED := sysdate;
END;
/
ALTER TRIGGER "PGEDATA"."PGE_CCB_SP_IO_MONITOR_INS" ENABLE;


insert into "PGEDATA"."PGE_CCB_SP_IO_MONITOR" (ID, INTERFACETYPE) values(1, 'Inbound');
insert into "PGEDATA"."PGE_CCB_SP_IO_MONITOR" (ID, INTERFACETYPE) values(2, 'Outbound');
commit;
/


GRANT all on "PGEDATA"."PGE_CCB_SP_IO_MONITOR" to PGEDATA,GIS_I,GISINTERFACE,GIS_I_WRITE, CUSTOMER;
/
create or replace PROCEDURE CCBTOGIS_CHECKSTATUS AS
v_status varchar2(2000);
BEGIN
/* Update status of outbound row in PGE_CCB_SP_IO_MONITOR */
select STATUS into v_status from PGE_CCB_SP_IO_MONITOR where INTERFACETYPE = 'Inbound';
if lower(v_status) != 'insert-completed' then
  raise_application_error(-20101, 'Inbound is being processed. The inbound status in PGE_CCB_SP_IO_MONITOR is ' ||  nvl(v_status, 'NULL'));
else
  begin
    select STATUS into v_status from PGE_CCB_SP_IO_MONITOR where INTERFACETYPE = 'Outbound';
    if ( v_status is not null and lower(v_status) != 'insert-completed') then
      raise_application_error(-20102, 'Outbound is being processed. The outbound status in PGE_CCB_SP_IO_MONITOR is ' ||  nvl(v_status, 'NULL'));
    end if;
    update PGE_CCB_SP_IO_MONITOR set STATUS = 'In-Progress' where INTERFACETYPE = 'Outbound';
    commit;
  end;
end if;
end CCBTOGIS_CHECKSTATUS;
/
grant all on CCBTOGIS_CHECKSTATUS to PGEDATA, GIS_I, GIS_I_WRITE, EDGIS, GISINTERFACE;