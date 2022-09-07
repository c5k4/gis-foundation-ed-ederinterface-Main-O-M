Prompt drop View PGE_SESSIONS_POSTED;
DROP VIEW SDE.PGE_SESSIONS_POSTED
/

/* Formatted on 7/2/2019 01:17:56 PM (QP5 v5.313) */
PROMPT View PGE_SESSIONS_POSTED;
--
-- PGE_SESSIONS_POSTED  (View)
--

CREATE OR REPLACE FORCE VIEW SDE.PGE_SESSIONS_POSTED
(
    VERSION_NAME,
    SESSION_POST_TIME,
    NODE_TYPE_ID,
    NODE_ID,
    SERVER,
    USER_ID,
    USER_NAME,
    DATE_TIME,
    NODE_DESCRIPTION,
    EXTRA_DATA,
    SESSION_ID,
    SESSION_NAME,
    CREATE_USER,
    CURRENT_OWNER,
    CREATE_DATE,
    SESSION_DESCRIPTION,
    HIDDEN,
    DATABASE_ID,
    ENTERPRISE_ID,
    SESSION_TYPE_ID
)
AS
    SELECT ph.version_owner || '.' || ph.version_name VERSION_NAME,
           ph.STATE_CREATION_TIME                     SESSION_POST_TIME,
           edgisph.NODE_TYPE_ID                       AS NODE_TYPE_ID,
           edgisph.node_id                            AS NODE_ID,
           edgisph.SERVER                             AS SERVER,
           edgisph.USER_ID                            AS USER_ID,
           edgisph.USER_NAME                          AS USER_NAME,
           edgisph.DATE_TIME                          AS DATE_TIME,
           edgisph.NODE_DESCRIPTION                   AS NODE_DESCRIPTION,
           edgisph.EXTRA_DATA                         AS EXTRA_DATA,
           edgisph.SESSION_ID                         AS SESSION_ID,
           edgisph.SESSION_NAME                       AS SESSION_NAME,
           edgisph.CREATE_USER                        AS CREATE_USER,
           edgisph.CURRENT_OWNER                      AS CURRENT_OWNER,
           edgisph.CREATE_DATE                        AS CREATE_DATE,
           edgisph.SESSION_DESCRIPTION                AS SESSION_DESCRIPTION,
           edgisph.HIDDEN                             AS HIDDEN,
           edgisph.DATABASE_ID                        AS DATABASE_ID,
           edgisph.ENTERPRISE_ID                      AS ENTERPRISE_ID,
           edgisph.SESSION_TYPE_ID                    AS SESSION_TYPE_ID
      FROM (SELECT version_owner, version_name, STATE_CREATION_TIME
              FROM sde.PGE_VERSIONS_POSTED_HIST a
             WHERE (a.version_owner,
                    a.version_name,
                    TO_CHAR (a.STATE_CREATION_TIME, 'YYYYMMDDHH24MISS')) IN
                       (  SELECT b.version_owner,
                                 b.version_name,
                                 MAX (
                                     TO_CHAR (b.STATE_CREATION_TIME,
                                              'YYYYMMDDHH24MISS'))
                            FROM PGE_VERSIONS_POSTED_HIST b
                        GROUP BY b.version_owner, b.version_name)) ph
           LEFT OUTER JOIN
           (SELECT *
              FROM sde.pge_session_history
             WHERE (node_id, TO_CHAR (date_time, 'YYYYMMDDHH24MISS')) IN
                       (  SELECT node_id,
                                 MAX (TO_CHAR (date_time, 'YYYYMMDDHH24MISS'))
                            FROM sde.pge_session_history
                        GROUP BY node_id)) edgisph
               ON 'SN_' || edgisph.node_id = ph.version_name
     WHERE     ph.STATE_CREATION_TIME <=
               (SELECT MAX (UP.state_creation_time)
                  FROM PGE_NONDEFAULT_SAVED_HIST UP
                 WHERE UP.VERSION_NAME = VERSION_HISTORY_PKG.GET_HIGHVERSION)
           AND ph.STATE_CREATION_TIME >
               (SELECT MAX (dn.state_creation_time)
                  FROM PGE_NONDEFAULT_SAVED_HIST dn
                 WHERE dn.VERSION_NAME = VERSION_HISTORY_PKG.GET_LOWVERSION)
           AND ph.STATE_CREATION_TIME <>
               (SELECT MAX (dn.state_creation_time)
                  FROM PGE_NONDEFAULT_SAVED_HIST dn
                 WHERE dn.VERSION_NAME = VERSION_HISTORY_PKG.GET_LOWVERSION)
/


Prompt Grants on VIEW PGE_SESSIONS_POSTED TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON SDE.PGE_SESSIONS_POSTED TO PUBLIC
/

Prompt Grants on VIEW PGE_SESSIONS_POSTED TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON SDE.PGE_SESSIONS_POSTED TO SDE_EDITOR
/

Prompt Grants on VIEW PGE_SESSIONS_POSTED TO SDE_VIEWER to SDE_VIEWER;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON SDE.PGE_SESSIONS_POSTED TO SDE_VIEWER
/
