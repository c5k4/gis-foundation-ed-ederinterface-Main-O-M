--Top CMCS sessions with lesser edits
UPDATE SDE.GDBM_POST_QUEUE SET PRIORITY = 10 WHERE VERSION_NAME IN 
(
	SELECT SUBSTR(VERSION, -9) FROM 
	(
		SELECT VERSION, COUNT(*)
		FROM SDE.MM_EDITED_FEATURES
		WHERE 
		VERSION IN (SELECT VERSION_OWNER ||'.'|| VERSION_NAME FROM SDE.GDBM_POST_QUEUE WHERE PRIORITY < 10)
		AND 
		VERSION IN (SELECT CREATE_USER||'.SN_'||SESSION_ID FROM PROCESS.MM_SESSION WHERE SESSION_NAME LIKE '%CMCS%' OR UPPER(SESSION_NAME) LIKE '-PARADISE%')
		GROUP BY VERSION
		ORDER BY COUNT(*) ASC
	)
	WHERE ROWNUM <= 10
)
;
COMMIT;