--------------------------------------------------------
--  File created - Friday-September-06-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table MDR_SUBLAP_LCA
--------------------------------------------------------

  CREATE TABLE "EDGIS"."MDR_SUBLAP_LCA" 
   (	"UDC_ID" VARCHAR2(26 BYTE), 
	"LAP_ID" VARCHAR2(26 BYTE), 
	"LCA_NAME" VARCHAR2(26 BYTE), 
	"START_DATE" DATE, 
	"END_DATE" DATE, 
	"CREATE_DATE" DATE, 
	"CREATED_BY" VARCHAR2(26 BYTE), 
	"UPDATE_DATE" DATE, 
	"UPDATED_BY" VARCHAR2(26 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;
REM INSERTING into EDGIS.MDR_SUBLAP_LCA
SET DEFINE OFF;
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCNW','Big Creek/Ventura',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCEN','Big Creek/Ventura',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGP2','Greater Bay Area',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGSF','Greater Bay Area',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGEB','Greater Bay Area',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGSB','Greater Bay Area',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGCC','Greater Bay Area',to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGF1','Greater Fresno Area',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGHB','Humboldt',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGKN','Kern',to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCEC','LA Basin',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCEW','LA Basin',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGNP','None',to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2017 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCLD','None',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGCC','None',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),to_date('31-DEC-2016 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',to_date('31-DEC-2016 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5');
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('SCE','SLAP_SCHD','None',to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('01-JAN-2019 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGZP','None',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGNC','North Coast/North Bay',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGNB','North Coast/North Bay',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGFG','North Coast/North Bay',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGSI','Sierra',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
Insert into EDGIS.MDR_SUBLAP_LCA (UDC_ID,LAP_ID,LCA_NAME,START_DATE,END_DATE,CREATE_DATE,CREATED_BY,UPDATE_DATE,UPDATED_BY) values ('PGAE','SLAP_PGST','Stockton',to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),null,to_date('31-MAR-2009 12.00.00 AM','DD-MON-RRRR HH.MI.SS AM'),'sjd5',null,null);
COMMIT;
--------------------------------------------------------
--  DDL for Index SUBLAP_LCA_PK
--------------------------------------------------------

  --CREATE UNIQUE INDEX "EDGIS"."SUBLAP_LCA_PK" ON "EDGIS"."MDR_SUBLAP_LCA" ("CREATE_DATE", "LAP_ID") 
  --PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  --STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  --PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  --BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  --TABLESPACE "EDGIS" ;
--------------------------------------------------------
--  Constraints for Table MDR_SUBLAP_LCA
--------------------------------------------------------

  --ALTER TABLE "EDGIS"."MDR_SUBLAP_LCA" ADD CONSTRAINT "SUBLAP_LCA_PK" PRIMARY KEY ("CREATE_DATE", "LAP_ID")
  --USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  --STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  --PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  --BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  --TABLESPACE "EDGIS"  ENABLE;
  --ALTER TABLE "EDGIS"."MDR_SUBLAP_LCA" MODIFY ("CREATE_DATE" NOT NULL ENABLE);
  --ALTER TABLE "EDGIS"."MDR_SUBLAP_LCA" MODIFY ("LAP_ID" NOT NULL ENABLE);
/

Prompt Grants on TABLE EDGIS.MDR_SUBLAP_LCA TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.MDR_SUBLAP_LCA TO SDE_VIEWER
/