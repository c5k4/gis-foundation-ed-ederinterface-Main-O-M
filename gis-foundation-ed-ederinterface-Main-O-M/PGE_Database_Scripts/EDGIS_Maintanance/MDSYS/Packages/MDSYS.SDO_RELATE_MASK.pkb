Prompt drop Package Body SDO_RELATE_MASK;
DROP PACKAGE BODY MDSYS.SDO_RELATE_MASK
/

Prompt Package Body SDO_RELATE_MASK;
--
-- SDO_RELATE_MASK  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY MDSYS.sdo_relate_mask as
procedure insert_mask(mask      in varchar2,
                      relation  in varchar2) is
begin
  insert into md$relate values(mask,nls_upper(relation),user);
  commit;
end insert_mask;

procedure update_mask(mask      in varchar2,
                 relation  in varchar2) is
owner varchar2(30);
begin
  select definer into owner from md$relate where sdo_mask=mask;
  if (owner <> user) then
    mderr.raise_md_error('MD','SDO',-13108,'Not definer');
	 return;
  end if;
  update md$relate set sdo_relation=nls_upper(relation) where sdo_mask=mask;
  commit;
end update_mask;

procedure delete_mask(mask      in varchar2) is
owner varchar2(30);
begin
  select definer into owner from md$relate where sdo_mask=mask;
  if (owner <> user) then
    mderr.raise_md_error('MD','SDO',-13108,'Not definer');
	 return;
  end if;
  delete from md$relate where sdo_mask=mask;
  commit;
end delete_mask;
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
