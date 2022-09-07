
 
spool D:\Temp\IdxTrgrConstrChanges.txt

CREATE INDEX FNM_COMPLETE_PNODE_BUS ON FNM_COMPLETE
(CNODE_ID, BUS_ID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );


CREATE INDEX FNM_HISTORY_PNODE_BUS ON FNM_HISTORY
(CNODE_ID, BUS_ID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );

CREATE INDEX FNM_PNODE_BUS ON FNM
(CNODE_ID, BUS_ID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );


CREATE INDEX GIS_PNODEINFO_CIRCUITID ON GIS_PNODEINFO
(CIRCUITID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );

CREATE INDEX GIS_PNODEINFO_FNM_PNODE_GUID ON GIS_PNODEINFO
(PNODE_FNMGUID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );

CREATE INDEX GIS_PNODEINFO_LINE_GUID ON GIS_PNODEINFO
(LINE_GUID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );


CREATE INDEX GIS_SERVICEPOINTS_INFO_CIRCUIT ON GIS_SERVICEPOINTS_INFO
(FEEDERID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );

CREATE INDEX GIS_SERVICEPOINTS_INFO_SA ON GIS_SERVICEPOINTS_INFO
(SERVICEAGREEMENTID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );


CREATE INDEX GIS_SERVICEPOINTS_INFO_SP ON GIS_SERVICEPOINTS_INFO
(SERVICEPOINTID)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );


CREATE INDEX GIS_SPP_FEEDER_SUBLAP_FEEDER ON GIS_SPP_FEEDER_SUBLAP
(FEEDER_NUM)
LOGGING
TABLESPACE EDGIS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );



CREATE OR REPLACE TRIGGER EDGIS.GIS_PNODEINFO_HISTORY
AFTER UPDATE OF QA_FLAG ON GIS_PNODEINFO
FOR EACH ROW 
when (new.qa_flag is not null)
BEGIN
  insert into EDGIS.GIS_PNODEINFO_HISTORY
            (
                LAP_ID,
                ORDER_NUM,
                MAX_BRANCH,
                MIN_BRANCH,
                TREELEVEL,
                LCA_ID,
                PNODE_DATEMODIFIED,
                PNODE_LASTUSER,
                PNODE_DATECREATED,
                PNODE_CREATIONUSER,
                PNODE_FNMGUID,
                FNM_GLOBALID,
                BUS_ID,
                FNM_CNODEID,
                PNODE_CNODEID,
                CIRCUITID,
                SUBPNODE_OID,
                LINE_OID,
                LINE_GUID,
                LINE_FC_NAME,
                QA_FLAG,
                ACTIONDATE
            )            
            values
            ( 
                :new.LAP_ID,
                :new.ORDER_NUM,
                :new.MAX_BRANCH,
                :new.MIN_BRANCH,
                :new.TREELEVEL,
                :new.LCA_ID,
                :new.PNODE_DATEMODIFIED,
                :new.PNODE_LASTUSER,
                :new.PNODE_DATECREATED,
                :new.PNODE_CREATIONUSER,
                :new.PNODE_FNMGUID,
                :new.FNM_GLOBALID,
                :new.BUS_ID,
                :new.FNM_CNODEID,
                :new.PNODE_CNODEID,
                :new.CIRCUITID,
                :new.SUBPNODE_OID,
                :new.LINE_OID,
                :new.LINE_GUID,
                :new.LINE_FC_NAME,
                :new.QA_FLAG,
                sysdate
            );      
END;
/

ALTER TRIGGER  EDGIS.GIS_PNODEINFO_HISTORY ENABLE;




spool off
