WHENEVER SQLERROR EXIT SQL.SQLCODE
TRUNCATE TABLE SDE."PGE_SESSION_HISTORY";
COPY FROM SDE/&1@EDER INSERT SDE."PGE_SESSION_HISTORY" USING SELECT * FROM SDE."PGE_SESSION_HISTORY";
TRUNCATE TABLE "CUSTOMER"."CUSTOMER_INFO";
INSERT INTO "CUSTOMER"."CUSTOMER_INFO" (SELECT * FROM CUSTOMER.CUSTOMER_INFO@CIEDER);
COMMIT;
exit;