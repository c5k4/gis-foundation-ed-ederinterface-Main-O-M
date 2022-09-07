--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_CHECKSTATUS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_CHECKSTATUS" AS
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
