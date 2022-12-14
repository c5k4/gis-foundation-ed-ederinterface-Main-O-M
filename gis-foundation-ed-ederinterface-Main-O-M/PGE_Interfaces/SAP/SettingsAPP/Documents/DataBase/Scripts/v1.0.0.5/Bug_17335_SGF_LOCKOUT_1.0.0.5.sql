spool SGF_LOCKOUT_1.0.0.5.log


ALTER TABLE SM_RECLOSER ADD  SGF_LOCKOUT  NUMBER(1,0) ;
ALTER TABLE SM_RECLOSER ADD  ALT_SGF_LOCKOUT  NUMBER(1,0) ;
ALTER TABLE SM_RECLOSER ADD  ALT3_SGF_LOCKOUT  NUMBER(1,0) ;

ALTER TABLE SM_RECLOSER_HIST ADD  SGF_LOCKOUT  NUMBER(1,0) ;
ALTER TABLE SM_RECLOSER_HIST ADD  ALT_SGF_LOCKOUT  NUMBER(1,0) ;
ALTER TABLE SM_RECLOSER_HIST ADD  ALT3_SGF_LOCKOUT  NUMBER(1,0) ;


Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5443,'SGF_LOCKOUT','N','4C',null);
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5445,'SGF_LOCKOUT','N','4C','NONE');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5446,'SGF_LOCKOUT','N','4C','OFF');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5447,'SGF_LOCKOUT','N','4C','ON');

Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5448,'SGF_LOCKOUT','N','6',null);
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5449,'SGF_LOCKOUT','N','6','NONE');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5450,'SGF_LOCKOUT','N','6','OFF');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5451,'SGF_LOCKOUT','N','6','ON');

Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5453,'ALT_SGF_LOCKOUT','N','6',null);
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5454,'ALT_SGF_LOCKOUT','N','6','NONE');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5455,'ALT_SGF_LOCKOUT','N','6','OFF');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5457,'ALT_SGF_LOCKOUT','N','6','ON');

Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5458,'ALT3_SGF_LOCKOUT','N','6',null);
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5459,'ALT3_SGF_LOCKOUT','N','6','NONE');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5460,'ALT3_SGF_LOCKOUT','N','6','OFF');
Insert into EDSETT.SM_RECLOSER_REQUIRED (ID,FIELD_NAME,REQUIRED,CONTROL_TYPE,SFG_CD) values (5461,'ALT3_SGF_LOCKOUT','N','6','ON');



commit;


spool off
