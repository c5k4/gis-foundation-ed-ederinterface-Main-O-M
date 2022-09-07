Prompt drop Package SDO_GCDR;
DROP PACKAGE MDSYS.SDO_GCDR
/

Prompt Package SDO_GCDR;
--
-- SDO_GCDR  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_GCDR AUTHID current_user as
  procedure validateCountryRow( country VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateInLineStreetTypeRow( country VARCHAR2,
    pos VARCHAR2,
    sep VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateStreetTypeRow( country VARCHAR2,
    pos VARCHAR2,
    sep VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateSecondUnitRow( country VARCHAR2,
    pos VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateStreetPrefixRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateStreetSuffixRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateRegionRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateCityRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateLocalityDictRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validateStreetDictRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validatePoBoxRow( country VARCHAR2,
    out VARCHAR2,
    keys SDO_keywordArray);

  -- used by country profile validation trigger, should not be documented
  procedure validatePlaceNameRow( country VARCHAR2,
    out VARCHAR2);

  -- create parser profile tables
  procedure create_profile_tables ;

  -- used by java geocoder client, should not be documented
  function batch_geocode(address_list in varchar2, gc_username in varchar2, max_result_count number)
  return sdo_keywordarray deterministic;

  -- geocode an input address specified by address lines and return
  -- the first matched address as a SDO_GEO_ADDR object
  function geocode(
    username varchar2,
    addr_lines SDO_keywordArray,
    country VARCHAR2,
    match_mode VARCHAR2) return SDO_GEO_Addr deterministic ;

  -- geocode an input address specified by address lines and return
  -- the the coordinates of the first matched address as sdo_geometry
  function geocode_as_geometry(
    username varchar2,
    addr_lines SDO_keywordArray,
    country VARCHAR2) return SDO_GEOMETRY deterministic;

  -- geocode an input address specified by a SDO_GEO_ADDR object and return
  -- the first matched address as a SDO_GEO_ADDR object
  function geocode_addr(
    gc_username varchar2,
    address SDO_GEO_ADDR) return SDO_geo_Addr deterministic;

  -- geocode an input address specified by a address lines and return all
  -- matched addresses as a VARRAY of SDO_GEO_ADDR objects
  function geocode_all(
    gc_username varchar2,
    addr_lines SDO_keywordArray,
    country VARCHAR2,
    match_mode varchar2,
    max_res_num number default 4000) return SDO_ADDR_ARRAY deterministic;

  -- geocode an input address specified by a SDO_GEO_ADDR object and return all
  -- matched addresses as a VARRAY of SDO_GEO_ADDR objects
  function geocode_addr_all(
    gc_username varchar2,
    address SDO_GEO_ADDR,
    max_res_num number default 4000) return SDO_ADDR_ARRAY deterministic ;

  -- reverse-geocode a location specified by longitude and latitude into
  -- address as a sdo_geo_addr object
  function reverse_geocode(
    username varchar2,
    longitude number,
    latitude number,
    country VARCHAR2) return SDO_GEO_Addr deterministic;

  -- reverse-geocode a location specified by longitude and latitude into
  -- address as a sdo_geo_addr object
  function reverse_geocode(
    username varchar2,
    location sdo_geometry,
    country VARCHAR2) return SDO_GEO_Addr deterministic ;

end SDO_GCDR;
/


Prompt Synonym SDO_GCDR;
--
-- SDO_GCDR  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_GCDR FOR MDSYS.SDO_GCDR
/


Prompt Grants on PACKAGE SDO_GCDR TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_GCDR TO PUBLIC
/
