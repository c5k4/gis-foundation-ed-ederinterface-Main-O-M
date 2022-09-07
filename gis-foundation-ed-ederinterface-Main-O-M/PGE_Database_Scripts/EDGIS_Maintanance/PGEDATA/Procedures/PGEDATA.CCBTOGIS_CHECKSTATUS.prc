Prompt drop Procedure CCBTOGIS_CHECKSTATUS;
DROP PROCEDURE PGEDATA.CCBTOGIS_CHECKSTATUS
/

Prompt Procedure CCBTOGIS_CHECKSTATUS;
--
-- CCBTOGIS_CHECKSTATUS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.CCBTOGIS_CHECKSTATUS AS
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


Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO CCB_EI_INTERFACE to CCB_EI_INTERFACE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO CCB_EI_INTERFACE
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO EDGIS
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO GISINTERFACE
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO GIS_I
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_CHECKSTATUS TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_CHECKSTATUS TO IGPEDITOR
/