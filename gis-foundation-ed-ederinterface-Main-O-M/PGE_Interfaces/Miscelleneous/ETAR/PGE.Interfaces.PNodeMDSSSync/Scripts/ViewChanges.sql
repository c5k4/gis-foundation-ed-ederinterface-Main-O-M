
spool D:\Temp\ViewChanges.txt

DROP VIEW GIS_CIRCUIT_FNM_VALUES;


/* Formatted on 05/22/20 10:56:52 AM (QP5 v5.313) */
CREATE OR REPLACE FORCE VIEW GIS_CIRCUIT_CIRCUIT_COMPRESSED
(
    CIRCUITID,
    UPSTREAM_CIRCUITID,
    LEVEL_NUM,
    CREATEUSER,
    DATECREATED
)
AS
      SELECT circuitid,
             --downstream_circuitid,
             upstream_circuitid,
             level_num,
             createuser,
             datecreated
        FROM edgis.gis_circuit_circuit
       WHERE (circuitid, level_num) IN (  SELECT circuitid, MAX (level_num)
                                            FROM edgis.gis_circuit_circuit
                                        GROUP BY circuitid)
    ORDER BY circuitid;





/* Formatted on 05/22/20 11:19:22 AM (QP5 v5.313) */
CREATE OR REPLACE FORCE VIEW GIS_FNM_VERSION_DATES
(
    VERSION_ORDER,
    FNM_VERSION,
    VERSION_START_DATE,
    VERSION_END_DATE
)
AS
      SELECT ROWNUM version_order,
             a.fnm_version,
             a.version_start_date,
             a.version_end_date
        FROM (  SELECT DISTINCT
                       first_fnm_version
                           fnm_version,
                       first_fnm_releasedate
                           version_start_date,
                       LEAD (first_fnm_releasedate - 1,
                             1,
                             TO_DATE ('31-DEC-9999'))
                       OVER (ORDER BY first_fnm_releasedate)
                           AS version_end_date
                  FROM (SELECT DISTINCT first_fnm_version, first_fnm_releasedate
                          FROM edgis.fnm_complete
                        UNION
                        SELECT DISTINCT
                               latest_fnm_version, latest_fnm_releasedate
                          FROM edgis.fnm_complete)
              ORDER BY first_fnm_releasedate) a
    ORDER BY version_order;




/* Formatted on 05/22/20 11:00:31 AM (QP5 v5.313) */
CREATE OR REPLACE FORCE VIEW GIS_FNM_LCA
(
    BUS_ID,
    CNODE_ID,
    LAP_ID,
    LCA_ID,
    UDC_ID,
    RES_TYPE,
    FIRST_FNM_VERSION,
    LATEST_FNM_VERSION,
    START_DATE,
    END_DATE
)
AS
    WITH
        gis_fnm
        AS
            (SELECT a.bus_id,
                    a.cnode_id,
                    a.lap_id,
                    a.udc_id,
                    a.res_type,
                    a.first_fnm_version,
                    a.first_fnm_releasedate,
                    a.latest_fnm_version,
                    a.latest_fnm_releasedate,
                    b.version_start_date start_date,
                    c.version_end_date   end_date
               FROM edgis.fnm_complete  a
                    INNER JOIN edgis.gis_fnm_version_dates b
                        ON a.first_fnm_version = b.fnm_version
                    INNER JOIN edgis.gis_fnm_version_dates c
                        ON a.latest_fnm_version = c.fnm_version)
      SELECT a.bus_id,
             a.cnode_id,
             a.lap_id
                 lap_id,
             NVL (b.lca_name, 'Indeterminate')
                 lca_id,
             a.udc_id,
             a.res_type,
             a.first_fnm_version,
             a.latest_fnm_version,
             GREATEST (a.start_date, NVL (b.start_date, a.start_date))
                 start_date,
             LEAST (a.end_date, NVL (b.end_date, a.end_date))
                 end_date
        FROM edgis.gis_fnm a
             LEFT OUTER JOIN edgis.gis_sublap_lca b
                 ON     a.lap_id = b.lap_id
                    AND (a.start_date <= b.end_date)
                    AND (a.end_date >= b.start_date)
    ORDER BY a.bus_id,
             a.cnode_id,
             GREATEST (a.start_date, NVL (b.start_date, a.start_date));

GRANT SELECT ON GIS_CIRCUIT_CIRCUIT_COMPRESSED TO SDE_VIEWER;
GRANT SELECT ON GIS_FNM_VERSION_DATES TO SDE_VIEWER;
GRANT SELECT ON GIS_FNM_LCA TO SDE_VIEWER;

spool off



