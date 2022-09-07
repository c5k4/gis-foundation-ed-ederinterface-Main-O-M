Prompt drop Trigger DB_EV_DROP_ST_METADATA;
DROP TRIGGER SDE.DB_EV_DROP_ST_METADATA
/

Prompt Trigger DB_EV_DROP_ST_METADATA;
--
-- DB_EV_DROP_ST_METADATA  (Trigger) 
--
CREATE OR REPLACE TRIGGER SDE.db_ev_drop_st_metadata AFTER DROP ON DATABASE
BEGIN IF  (ora_dict_obj_type = 'TABLE') THEN DELETE FROM sde.st_geometry_columns WHERE owner = ora_dict_obj_owner AND table_name = ora_dict_obj_name; END IF; END;
/
