Prompt drop View TOKENSESSIONMAP$;
DROP VIEW MDSYS.TOKENSESSIONMAP$
/

/* Formatted on 6/27/2019 02:51:51 PM (QP5 v5.313) */
PROMPT View TOKENSESSIONMAP$;
--
-- TOKENSESSIONMAP$  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.TOKENSESSIONMAP$
(
    SESSIONID,
    TOKENID,
    EXPIRYTIME,
    EXPIRYOFFSET
)
AS
    SELECT sessionId,
           tokenId,
           expiryTime,
           expiryOffset
      FROM MDSYS.TokenSessionMap_t$
     WHERE    (tokenId IN
                   (SELECT tokenId
                      FROM MDSYS.CurrentSessionTokenMap$
                     WHERE sessionId IN
                               (SELECT DBMS_SESSION.unique_session_id
                                  FROM DUAL)))
           OR expiryTime < SYSDATE
/


Prompt Grants on VIEW TOKENSESSIONMAP$ TO SPATIAL_WFS_ADMIN to SPATIAL_WFS_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.TOKENSESSIONMAP$ TO SPATIAL_WFS_ADMIN
/

Prompt Grants on VIEW TOKENSESSIONMAP$ TO SPATIAL_WFS_ADMIN_USR to SPATIAL_WFS_ADMIN_USR;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.TOKENSESSIONMAP$ TO SPATIAL_WFS_ADMIN_USR
/

Prompt Grants on VIEW TOKENSESSIONMAP$ TO WFS_USR_ROLE to WFS_USR_ROLE;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.TOKENSESSIONMAP$ TO WFS_USR_ROLE
/