spool "D:\Temp\PGE_CODES_AND_DESCRIPTIONS.txt"

CREATE TABLE "ETGIS"."PGE_CODES_AND_DESCRIPTIONS" 
   (	"DOMAIN_NAME" NVARCHAR2(200), 
	"CODE" NVARCHAR2(200), 
	"DESCRIPTION" NVARCHAR2(200)
   ) ;

Commit;

Spool off;