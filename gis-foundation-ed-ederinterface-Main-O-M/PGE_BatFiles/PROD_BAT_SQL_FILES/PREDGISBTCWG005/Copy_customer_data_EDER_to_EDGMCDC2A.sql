WHENEVER SQLERROR EXIT SQL.SQLCODE
TRUNCATE TABLE "CUSTOMER"."CUSTOMER_INFO";
INSERT INTO "CUSTOMER"."CUSTOMER_INFO" (SELECT * FROM CUSTOMER.CUSTOMER_INFO@CIEDER);
COMMIT;
exit;