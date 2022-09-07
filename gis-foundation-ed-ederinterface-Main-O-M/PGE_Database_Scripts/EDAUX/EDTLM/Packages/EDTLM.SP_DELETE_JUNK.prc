Prompt drop Procedure SP_DELETE_JUNK;
DROP PROCEDURE EDTLM.SP_DELETE_JUNK
/

Prompt Procedure SP_DELETE_JUNK;
--
-- SP_DELETE_JUNK  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDTLM."SP_DELETE_JUNK"
as
begin

delete from SP_PEAK_GEN_HIST where TRF_PEAK_GEN_HIST_ID = (select id from trf_peak_gen_hist where batch_date = to_date('01-MAR-14'));

delete from  trf_peak_gen_hist where batch_date = to_date('01-MAR-14');

delete from SP_PEAK_HIST where TRF_PEAK_HIST_ID in (select id from trf_peak_hist where batch_date = to_date('01-MAR-14'));

delete from  trf_peak_hist where batch_date = to_date('01-MAR-14');

delete from MONTHLY_LOAD_LOG;

commit;

end sp_delete_junk ;

/
