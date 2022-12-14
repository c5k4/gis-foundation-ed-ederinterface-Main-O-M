--User account in ED-Datamart, used to create DB-Link from Substation Environment.
DROP USER ETAR_DM_SUB_DBLNK_RO CASCADE;
CREATE USER ETAR_DM_SUB_DBLNK_RO
  IDENTIFIED BY "eDGISDM_$1928"
  DEFAULT TABLESPACE PGE
  TEMPORARY TABLESPACE TEMP
  PROFILE PROF_APP
  ACCOUNT UNLOCK;
  
  -- Roles for ETAR_DM_SUB_DBLNK_RO 
GRANT CONNECT TO ETAR_DM_SUB_DBLNK_RO;
GRANT RESOURCE TO ETAR_DM_SUB_DBLNK_RO;
GRANT SDE_VIEWER TO ETAR_DM_SUB_DBLNK_RO;
GRANT SM_USER TO ETAR_DM_SUB_DBLNK_RO;
ALTER USER ETAR_DM_SUB_DBLNK_RO DEFAULT ROLE ALL;

-- 1 System Privilege for ETAR_DM_SUB_DBLNK_RO 
GRANT UNLIMITED TABLESPACE TO ETAR_DM_SUB_DBLNK_RO;
