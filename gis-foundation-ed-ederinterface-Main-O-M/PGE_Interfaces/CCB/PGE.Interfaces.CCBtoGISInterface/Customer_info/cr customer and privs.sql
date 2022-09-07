
-- move customer schema from lower environment and truncate customer_info table
-- alternatively:

CREATE USER customer IDENTIFIED BY
DEFAULT TABLESPACE USERS TEMPORARY TABLESPACE TEMP PROFILE PROF_APP;

GRANT CONNECT, RESOURCE TO customer;
GRANT SELECT_CATALOG_ROLE TO customer;
GRANT CREATE SYNONYM TO customer;
GRANT CREATE ROLE TO customer;
GRANT CREATE TABLE TO customer;
GRANT CREATE PROCEDURE TO customer;
GRANT CREATE VIEW TO customer;
GRANT SDE_VIEWER TO customer;
GRANT CREATE MATERIALIZED VIEW TO customer;
GRANT CREATE DATABASE LINK TO customer;

grant analyze any to customer;
Grant select, update on PGEDATA.PGE_CCB_SP_IO_MONITOR to customer; 
 
--Also password should not expire, nor ID as it will be for system access.

CREATE SYNONYM pge_ccbtoedgis_stg FOR pgedata.pge_ccbtoedgis_stg;
CREATE SYNONYM servicepoint FOR edgis.zz_mv_servicepoint;

CREATE TABLE customer_info
   (servicepointid                 NVARCHAR2(10),
    mailname1                      NVARCHAR2(50),
    mailname2                      NVARCHAR2(50),
    areacode                       NUMBER(3,0),
    phonenum                       NVARCHAR2(13),
    mailstreetnum                  NVARCHAR2(12),
    mailstreetname1                NVARCHAR2(64),
    mailstreetname2                NVARCHAR2(64),
    mailcity                       NVARCHAR2(30),
    mailstate                      NVARCHAR2(2),
    mailzipcode                    NVARCHAR2(10),
    sensitivecustomeridc           NVARCHAR2(2),
    lifesupportidc                 NVARCHAR2(2),
    medicalbaselineidc             NVARCHAR2(2),
    communicationpreference        NVARCHAR2(5))
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  cedsa_data;

GRANT DELETE ON customer_info TO customer_editor
/
GRANT INSERT ON customer_info TO customer_editor
/
GRANT SELECT ON customer_info TO pons_ro
/
GRANT SELECT ON customer_info TO customer_editor
/
GRANT SELECT ON customer_info TO customer_viewer
/
GRANT UPDATE ON customer_info TO customer_editor
/




-- Constraints for CUSTOMER_INFO

ALTER TABLE customer_info
ADD CONSTRAINT customer_info_pk PRIMARY KEY (servicepointid)
USING INDEX
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  cedsa_data;

