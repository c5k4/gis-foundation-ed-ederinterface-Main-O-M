Prompt drop Procedure CCBTOGIS_SP_ACTION_DELETES;
DROP PROCEDURE PGEDATA.CCBTOGIS_SP_ACTION_DELETES
/

Prompt Procedure CCBTOGIS_SP_ACTION_DELETES;
--
-- CCBTOGIS_SP_ACTION_DELETES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.ccbtogis_sp_action_deletes AS
/*
PURPOSE: This procedure identifies service points that need to be deleted
         from the edgis service point table.  EI sends daily delta's with
         action = 'D' for deletes, and once a month it sends the full population
         of service points with action set to null

 MODIFICATION HISTORY
 Person      Date    Comments
 ---------   ------  -------------------------------------------
 IBM         ???     Initial Coding
 TJJ4        031418  the monthly mass synch occurs the 2nd weekend of the month
                     and sends us the full population of 5.5m service points
                     any service point in EDGIS not in this full population run
                     needs to be deleted.
                     records in the mass send have the 'action' column set to null

                     To identify a mass send from the regular delta runs we:
                     1. check the run date, for runs between the 7th and 18th
                        day of the month, check further to see if the mass data
                        is in the staging table, once data is processed from the
                        staging table it is deleted
                     2. check if there are > 5.5 million records with action null
                     if both are true, assume it's a run with the mass synch data,
                     be aware the staging table may also have delta records with
                     non-null actions
                     As a further failsafe do not delete more than 2500 records

*/

rowcnt number;
BEGIN
   dbms_output.put_line('Populating the delete values into the Action Table');
   -- add any delta deletes
   insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION,SERVICEPOINTID, DATEINSERTED)
       select 'GISD', sp1.SERVICEPOINTID, max(sp1.DATECREATED) /* CCBTOGIS_SP_ACTION_DELETES_V2_GISD*/
         from PGEDATA.PGE_CCBTOEDGIS_STG sp1
        where sp1.servicepointid in
             (select distinct STG.SERVICEPOINTID
                from PGEDATA.PGE_CCBTOEDGIS_STG STG
               where STG.ACTION = 'D'
                 and STG.SERVICEPOINTID is not null
              )
        group by sp1.servicepointid;
   if extract(day from sysdate) between 7 and 18 then -- this is between the 7th and 18th
      select count(*)
        into rowcnt
        from pgedata.pge_ccbtoedgis_stg
       where action is null;
      if rowcnt > 5500000 then   -- there are more than 5.5 m spids
         insert into pgedata.pge_ccb_sp_action (action, servicepointid, dateinserted)
           select 'GISD', sp.servicepointid, sysdate
             from edgis.zz_mv_servicepoint sp, pgedata.pge_ccbtoedgis_stg stg
            where sp.servicepointid = stg.servicepointid (+)
              and stg.servicepointid is null;
              --and rownum < 2500;   -- a failsafe to keep us from overdeleting
      end if;
   end if;
   commit;
END CCBTOGIS_SP_ACTION_DELETES;

/


Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO EDGIS
/

Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO GIS_I
/

Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO IGPEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_SP_ACTION_DELETES TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_SP_ACTION_DELETES TO SDE
/
