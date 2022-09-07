set heading off;
whenever sqlerror exit sql.sqlcode

-- Create log file for run to review execution times and errors
spool Customer_info.txt;

alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';

-- Turn on per SQL event to track the length of time of each execution
set timing on;

update pgedata.pge_ccb_sp_io_monitor set status = 'In-Progress'
where interfacetype = 'Inbound' and status = 'Insert-Completed';
commit;

-- analyze the staging table, we don't distinguish between a full load an an incremental
-- disabling these as we don't have sufficient privileges - consider reenabling if privs are granted
--exec dbms_stats.delete_table_stats('PGEDATA','PGE_CCBTOEDGIS_STG');
--exec dbms_stats.gather_table_stats('PGEDATA','PGE_CCBTOEDGIS_STG');

-- delete any service points that no longer exist in the servicepoint table unless
-- they exist in the staging table  (8-9 mins in development)
SELECT 'Delete service points not in ServicePoint',sysdate from dual;
/* explain plan indicates this is more costly, try second statement instead 
DELETE FROM customer_info
 WHERE servicepointid IN (
       SELECT servicepointid
         FROM customer_info c
        WHERE NOT EXISTS (SELECT DISTINCT servicepointid 
                            FROM servicepoint z
                           WHERE z.servicepointid = c.servicepointid )
       MINUS
       SELECT servicepointid
       FROM pge_ccbtoedgis_stg);
*/

DELETE FROM customer_info 
 WHERE servicepointid IN (
         SELECT servicepointid
          FROM customer_info  
         MINUS
          (SELECT servicepointid 
             FROM servicepoint
           union
           SELECT servicepointid
             FROM pge_ccbtoedgis_stg
           union
           SELECT servicepointid
             FROM edgis.a71   ));

-- add new servicepoints from staging table (doesn't care if the action is I, U or null)
-- only consider the latest record for each service point
-- Note: EI is not populating mailstreetname2 in the staging table, use mailaddr2 instead
SELECT 'Adding new service points',sysdate from dual;
INSERT INTO customer_info 
        (servicepointid, mailname1, mailname2, areacode, phonenum, 
         mailstreetnum, mailstreetname1, mailstreetname2, mailcity,
         mailstate, mailzipcode, sensitivecustomeridc, lifesupportidc, 
         medicalbaselineidc, communicationpreference)
  SELECT DISTINCT s.servicepointid, s.mailname1, s.mailname2, s.areacode, s.phonenum, 
         s.mailstreetnum, s.mailstreetname1, s.mailaddr2, s.mailcity, 
         s.mailstate, s.mailzipcode, s.sensitivecustomeridc, s.lifesupportidc, 
         s.medicalbaseline, s.scadacomm
    FROM customer_info c, pge_ccbtoedgis_stg s
  WHERE c.servicepointid (+) = s.servicepointid 
    AND c.servicepointid IS NULL
    AND nvl(s.action, 'x') <> 'D'
    AND nvl(datecreated,SYSDATE) = (SELECT nvl(max(datecreated),SYSDATE)
                                      FROM pge_ccbtoedgis_stg
                                     WHERE servicepointid = s.servicepointid
                                       AND nvl(action, 'x') <> 'D');

-- update any changed service points (process the staged record with the most
-- recent date, if there are two with the same date, just pick one arbitrarilly)

SELECT 'Updating changed service points',sysdate from dual;

UPDATE customer_info c set
       (mailname1, mailname2, areacode, phonenum, mailstreetnum,
        mailstreetname1, mailstreetname2, mailcity, mailstate, mailzipcode,
        sensitivecustomeridc, lifesupportidc, medicalbaselineidc, 
        communicationpreference) = 
(SELECT mailname1, mailname2, areacode, phonenum, mailstreetnum,
        mailstreetname1, mailaddr2, mailcity, mailstate, mailzipcode,
        sensitivecustomeridc, lifesupportidc, medicalbaseline, 
        scadacomm 
   FROM pge_ccbtoedgis_stg s 
  WHERE s.servicepointid = c.servicepointid
    and nvl(s.action, 'x') <> 'D'
    AND nvl(datecreated,sysdate) = 
                      (SELECT nvl(max(datecreated),sysdate)
                         FROM pge_ccbtoedgis_stg x 
                        where x.servicepointid = s.servicepointid
                          AND nvl(action, 'x') <> 'D')
    AND rownum = 1)  -- arbitrarilly pick one if there are two for the same datecreated
WHERE c.servicepointid IN 
     (SELECT a.servicepointid
        FROM pge_ccbtoedgis_stg a
       WHERE (nvl(c.mailname1, ' ') <> nvl(a.mailname1, ' ') OR
              nvl(c.mailname2, ' ') <> nvl(a.mailname2, ' ') OR
              nvl(c.areacode,  0  ) <> nvl(a.areacode,  0  ) OR
              nvl(c.phonenum,  ' ') <> nvl(a.phonenum,  ' ') OR
          nvl(c.mailstreetnum, ' ') <> nvl(a.mailstreetnum,   ' ') OR
        nvl(c.mailstreetname1, ' ') <> nvl(a.mailstreetname1, ' ') OR
        nvl(c.mailstreetname2, ' ') <> nvl(a.mailaddr2, ' ') OR
              nvl(c.mailcity,  ' ') <> nvl(a.mailcity,  ' ') OR
              nvl(c.mailstate, ' ') <> nvl(a.mailstate,   ' ') OR
            nvl(c.mailzipcode, ' ') <> nvl(a.mailzipcode, ' ') OR
         nvl(c.lifesupportidc, ' ') <> nvl(a.lifesupportidc, ' ') OR
     nvl(c.medicalbaselineidc, ' ') <> nvl(a.medicalbaseline,   ' ') OR
   nvl(c.sensitivecustomeridc, ' ') <> nvl(a.sensitivecustomeridc, ' ') OR
nvl(c.communicationpreference, ' ') <> nvl(a.scadacomm, ' ')));                        


exec dbms_stats.delete_table_stats(user,'CUSTOMER_INFO');
exec dbms_stats.gather_table_stats(user,'CUSTOMER_INFO');

commit;

SELECT 'setting pge_ccb_sp_io_monitor to Completed',sysdate from dual;

update pgedata.pge_ccb_sp_io_monitor set status = 'Insert-Completed'
where interfacetype = 'Inbound' ;

spool off;

exit;


