spool "D:\DA_190401\Logs\me_update\domain_update.txt"
SET DEFINE OFF;


 CREATE TABLE "ETGIS"."ME_SAP_DATA_LOG" 
   (	"SAP_EQUIP_ID" VARCHAR2(20 BYTE), 
	"FIELD_NAME" VARCHAR2(40 BYTE), 
	"OLD_VALUE" VARCHAR2(40 BYTE), 
	"UPDATED_VALUE" VARCHAR2(40 BYTE), 
	"VERSION_NAME" VARCHAR2(40 BYTE)
   );
CREATE TABLE "ETGIS"."DOMAIN_LOOKUP" 
   (	"OBJECTID" NUMBER(*,0) NOT NULL ENABLE, 
	"CODE" NVARCHAR2(40), 
	"CODE_DESC" NVARCHAR2(40), 
	"FEATURE_NAME" NVARCHAR2(40)
   );


Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (1,'001','Alameda','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (2,'002','Alpine','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (3,'003','Amador','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (4,'004','Butte','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (5,'005','Calaveras','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (6,'006','Colusa','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (7,'007','Contra Costa','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (8,'008','Del Norte','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (9,'009','El Dorado','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (10,'010','Fresno','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (11,'011','Glenn','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (12,'012','Humboldt','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (13,'013','Imperial','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (14,'014','Inyo','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (15,'015','Kern','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (16,'016','Kings','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (17,'017','Lake','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (18,'018','Lassen','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (19,'019','Los Angeles','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (20,'020','Madera','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (21,'021','Marin','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (22,'022','Mariposa','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (23,'023','Mendocino','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (24,'024','Merced','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (25,'025','Modoc','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (26,'026','Mono','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (27,'027','Monterey','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (28,'028','Napa','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (29,'029','Nevada','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (30,'030','Orange','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (31,'031','Placer','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (32,'032','Plumas','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (33,'033','Riverside','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (34,'034','Sacramento','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (35,'035','San Benito','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (36,'036','San Bernardino','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (37,'037','San Diego','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (38,'038','San Francisco','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (39,'039','San Joaquin','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (40,'040','San Luis Obispo','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (41,'041','San Mateo','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (42,'042','Santa Barbara','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (43,'043','Santa Clara','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (44,'044','Santa Cruz','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (45,'045','Shasta','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (46,'046','Sierra','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (47,'047','Siskiyou','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (48,'048','Solano','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (49,'049','Sonoma','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (50,'050','Stanislaus','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (51,'051','Sutter','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (52,'052','Tehama','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (53,'053','Trinity','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (54,'054','Tulare','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (55,'055','Tuolumne','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (56,'056','Ventura','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (57,'057','Yolo','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (58,'058','Yuba','County');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (59,'CC','Central Coast','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (60,'DA','De Anza','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (61,'DI','Diablo','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (62,'EB','East Bay','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (63,'ETS-NV','North Valley','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (64,'FR','Fresno','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (65,'HB','Humboldt','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (66,'KE','Kern','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (67,'LP','Los Padres','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (68,'MI','Mission','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (69,'NB','North Bay','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (70,'PN','Peninsula','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (71,'SA','Sacramento','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (72,'SF','San Francisco','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (73,'SI','Sierra','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (74,'SJ','San Jose','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (75,'SL','Skyline','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (76,'SO','Sonoma','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (77,'ST','Stockton','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (78,'YO','Yosemite','Division');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (79,'CONCORD','Concord','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (80,'EUREKA','Eureka','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (81,'FRESNO','Fresno','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (82,'MARTIN','Martin','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (83,'METCALF','Metcalf','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (84,'MIDWAY','Midway','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (85,'MOSSLNDG','Moss Landing','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (86,'PISMOBCH','Pismo Beach','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (87,'SACTO','Sacramento','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (88,'TABLEMTN','Table Mountain','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (89,'VICTOR','Victor','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (90,'LAKEVLLE','Lakeville','WorkCentre');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (91,'0','Neither','HFTD');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (92,'2','Tier 2','HFTD');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (93,'3','Tier 3','HFTD');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (94,'TH','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (95,'T-1','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (96,'TPA','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (97,'3HPD','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (98,'TP','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (99,'WB','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (100,'2PS','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (101,'TPD','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (102,'DV-DE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (103,'SS2','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (104,'DDE','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (105,'SV1','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (106,'TPAD','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (107,'3HP','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (108,'UNKNOWN',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (109,'SS1','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (110,'FSS',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (111,'1VP','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (112,'DV','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (113,'SUSP','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (114,'3P DV','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (115,'DCPOST','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (116,'SV2','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (117,'TPAF','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (118,'TPS','SUSP - POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (119,'SVDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (120,'3P DDE','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (121,'SV-PJ-90','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (122,'TAP','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (123,'DV-PJ','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (124,'SV-PJ','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (125,'FIG4W','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (126,'3P SV','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (127,'3P DDE-A','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (128,'2P DDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (129,'FIG4','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (130,'3WV-T','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (131,'DDE-A','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (132,'TPAT','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (133,'TRP DDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (134,'TPSRD','DEADEND - SUSP','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (135,'DC-DDE-A','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (136,'FDDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (137,'SV-DE-90','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (138,'T1DE','DEADEND - POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (139,'1VPS','SUSP - POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (140,'TCDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (141,'THDE','DEADEND - POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (142,'DE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (143,'DC-DDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (144,'3HPD-S','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (145,'OTHER',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (146,'TPSR','SUSP - POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (147,'FIG4DDE','DEADEND - SUSP','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (148,'LB','DEADEND - SUSP','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (149,'3P TRP D','DEADEND - SUSP','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (150,'3HP-T','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (151,'SV-1B','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (152,'TRP SUSP','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (153,'TCP','PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (154,'TCPDDE','DEADEND - SUSP','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (155,'SUSP-A','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (156,'2P DDE-A','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (157,'T2N','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (158,'3HPS','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (159,'TPS1D','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (160,'SUSP-V','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (161,'TRP DE','DEADEND - PIN','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (162,'2PS TRP','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (163,'T1-1A','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (164,'BDE','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (165,'V2S-G','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (166,'DOUBLE CIRCUIT DOUBLE DEAD END  ANGLE','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (167,'DOUBLE CIRCUIT DOUBLE DEAD ANGLE','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (168,'SVS-3','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (169,'DOUBLE D','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (170,'2PS DDE-','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (171,'DLDM','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (172,'TRIANGLE','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (173,'2PS DDE','DOUBLE DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (174,'T1-F1A','POST','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (175,'V2SL','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (176,'EP DV','DEAD END','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (177,'FIELD ST',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (178,'SINGLE WOOD POLE',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (179,'V2H-G','SUSPENSION','Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (180,'FIELD SW',null,'Pole_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (181,'SUSP','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (182,'SS2','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (183,'DDE','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (184,'UNKNOWN',null,'Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (185,'SS1','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (186,'DC-DDE-A','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (187,'DDE-A','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (188,'DC-DDE','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (189,'3P DDE-A','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (190,'TRP DDE','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (191,'SUSP-A','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (192,'FSS','DEADEND - SUSP','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (193,'TRP SUSP','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (194,'DCPOST','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (195,'SUSP-V','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (196,'TPAF','PIN','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (197,'DE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (198,'1VP','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (199,'FDDE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (200,'DOUBLE CIRCUIT DOUBLE DEAD ANGLE','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (201,'DV','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (202,'3WV-T','DEADEND - SUSP','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (203,'TH','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (204,'SVDE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (205,'DV-DE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (206,'WB','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (207,'DOUBLE CIRCUIT DOUBLE DEAD END  ANGLE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (208,'2PS','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (209,'SUSPENSI','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (210,'T-1','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (211,'TRP DE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (212,'SV1','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (213,'DOUBLE CIRCUIT DOUBLE DEAD  END  ANGLE','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (214,'TPS','SUSP - POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (215,'3P DV','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (216,'3HPD','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (217,'OTHER',null,'Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (218,'FIG4','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (219,'T1DE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (220,'BDE','DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (221,'SV2','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (222,'3HP','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (223,'TPA','PIN','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (224,'3P TRP D','DEADEND - SUSP','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (225,'DV-PJ','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (226,'3P SV','SUSPENSION','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (227,'2P DDE-A','DOUBLE DEAD END','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (228,'TPD','POST','Tower_Insulation_type');
Insert into DOMAIN_LOOKUP (OBJECTID,CODE,CODE_DESC,FEATURE_NAME) values (229,'DOUBLE CIRCUIT DOUBLE DEAD END ANGLE','DOUBLE DEAD END','Tower_Insulation_type');
spool off;

 