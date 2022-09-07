Prompt drop Package SDO_RELATE_MASK;
DROP PACKAGE MDSYS.SDO_RELATE_MASK
/

Prompt Package SDO_RELATE_MASK;
--
-- SDO_RELATE_MASK  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_relate_mask authid definer as
procedure insert_mask(mask      in varchar2,
                 relation  in varchar2);

procedure update_mask(mask      in varchar2,
                 relation  in varchar2);

procedure delete_mask(mask      in varchar2);
end sdo_relate_mask;
/


Prompt Synonym SDO_RELATE_MASK;
--
-- SDO_RELATE_MASK  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_RELATE_MASK FOR MDSYS.SDO_RELATE_MASK
/


Prompt Grants on PACKAGE SDO_RELATE_MASK TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_RELATE_MASK TO PUBLIC
/
