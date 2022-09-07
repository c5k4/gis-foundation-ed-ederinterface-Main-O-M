Prompt drop Package SDO_CS;
DROP PACKAGE MDSYS.SDO_CS
/

Prompt Package SDO_CS;
--
-- SDO_CS  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_cs AUTHID current_user AS
/*
  TYPE EPSG_IDS
  IS TABLE OF NUMBER;
*/
  -- for TRANSFORM operator: trusted callout interface
  FUNCTION transform_orig(geom IN mdsys.sdo_geometry,
                     dim  IN mdsys.sdo_dim_array,
                     to_srid IN NUMBER)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  FUNCTION transform_orig(geom IN mdsys.sdo_geometry,
                     tolerance  IN number,
                     to_srid IN NUMBER)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  FUNCTION transform_orig(geom   IN mdsys.sdo_geometry,
                     to_srid   IN NUMBER)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  FUNCTION transform_orig(geom   IN mdsys.sdo_geometry,
                     dim    IN mdsys.sdo_dim_array,
                     to_srname IN VARCHAR2)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
--  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  FUNCTION transform_orig(geom   IN mdsys.sdo_geometry,
                     tolerance    IN number,
                     to_srname IN VARCHAR2)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
--  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  FUNCTION transform_orig(geom   IN mdsys.sdo_geometry,
                     to_srname IN VARCHAR2)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
--  PRAGMA restrict_references(transform_orig, wnds, rnps, wnps);

  PROCEDURE transform_layer( table_in   IN  VARCHAR2,
                            column_in  IN  VARCHAR2,
                            table_out  IN  VARCHAR2,
                            to_srid    IN  NUMBER   );
  FUNCTION viewport_transform(geom IN mdsys.sdo_geometry,
                               to_srid   IN NUMBER)
              RETURN mdsys.sdo_geometry DETERMINISTIC;
  PRAGMA restrict_references(viewport_transform, wnds, rnps, wnps);
--  FUNCTION viewport_transform(geom IN mdsys.sdo_geometry,
--                               to_srname   IN VARCHAR2)
--              RETURN mdsys.sdo_geometry DETERMINISTIC;
--  PRAGMA restrict_references(transform, wnds, rnps, wnps);
  FUNCTION validate_wkt(srid in NUMBER)
     RETURN varchar2;
  PRAGMA restrict_references(validate_wkt, wnds, rnps, wnps);

  FUNCTION intl_validate_wkt(srid in NUMBER)
     RETURN varchar2;
  PRAGMA restrict_references(intl_validate_wkt, wnds, rnps, wnps);

  FUNCTION internal_read_proj_from_wkt(
    wkt VARCHAR2)
      RETURN VARCHAR2;
  PRAGMA restrict_references(internal_read_proj_from_wkt, wnds, rnps, wnps);

  PROCEDURE transform_layer(
    table_in  IN VARCHAR2,
    column_in IN VARCHAR2,
    table_out IN VARCHAR2,
    use_plan  IN TFM_PLAN);

  PROCEDURE transform_layer(
    table_in  IN VARCHAR2,
    column_in IN VARCHAR2,
    table_out IN VARCHAR2,
    use_case  IN VARCHAR2,
    to_srid   IN NUMBER);

  FUNCTION determine_default_chain(
    source_srid IN NUMBER,
    target_srid IN NUMBER)
      RETURN SDO_SRID_CHAIN;

  FUNCTION determine_chain(
    transient_rule_set  IN SDO_TRANSIENT_RULE_SET,
    use_case            IN VARCHAR2,
    source_srid         IN NUMBER,
    target_srid         IN NUMBER)
      RETURN TFM_PLAN;

  FUNCTION internal_det_chain_VARCHAR(
    plan IN TFM_PLAN)
      RETURN VARCHAR2;

  FUNCTION internal_det_chain(
    transient_rule_set  IN SDO_TRANSIENT_RULE_SET,
    use_case            IN VARCHAR2,
    source_srid         IN NUMBER,
    target_srid         IN NUMBER,
    explain             IN BOOLEAN,
    explanation         OUT VARCHAR2)
      RETURN TFM_PLAN;

  FUNCTION internal_det_srid_wkt(
    srid1 NUMBER)
      RETURN VARCHAR2;

  FUNCTION internal_epsg_param_to_legacy(
    param_id  NUMBER,
    method_id NUMBER)
      RETURN VARCHAR2;

  FUNCTION map_oracle_srid_to_epsg(
    legacy_srid NUMBER)
      RETURN NUMBER;

  FUNCTION map_epsg_srid_to_oracle(
    epsg_srid NUMBER)
      RETURN NUMBER;

  PROCEDURE create_pref_concatenated_op(
    op_id     IN NUMBER,
    op_name   IN VARCHAR2,
    use_plan  IN TFM_PLAN,
    use_case  IN VARCHAR2);

  PROCEDURE create_concatenated_op(
    op_id     IN NUMBER,
    op_name   IN VARCHAR2,
    use_plan  IN TFM_PLAN);

  PROCEDURE delete_op(
    op_id     IN NUMBER);

  PROCEDURE add_preference_for_op(
    op_id       IN NUMBER,
    source_crs  IN NUMBER   DEFAULT NULL,
    target_crs  IN NUMBER   DEFAULT NULL,
    use_case    IN VARCHAR2 DEFAULT NULL);

  PROCEDURE revoke_preference_for_op(
    op_id       IN NUMBER,
    source_crs  IN NUMBER   DEFAULT NULL,
    target_crs  IN NUMBER   DEFAULT NULL,
    use_case    IN VARCHAR2 DEFAULT NULL);

  PROCEDURE create_obvious_epsg_rules(
    use_case    IN VARCHAR2 DEFAULT NULL);

  PROCEDURE delete_all_epsg_rules(
    use_case    IN VARCHAR2 DEFAULT NULL);

  FUNCTION transform_to_base_unit(
    value           IN NUMBER,
    source_unit_id  IN NUMBER)
      RETURN NUMBER;

  FUNCTION transform_to_wkt_param_unit(
    value                     IN NUMBER,
    source_unit_id            IN NUMBER,
    target_unit_id_if_length  IN NUMBER)
      RETURN NUMBER;

  PROCEDURE create_crs_using_legacy_proj(
    epsg_srid   IN NUMBER,
    new_srid    IN NUMBER);

  ------------------------------------------------------------------------------

  PROCEDURE internal_get_densification_res(
    crs_srid    IN NUMBER,
    minimum     IN NUMBER,
    maximum     IN NUMBER,
    resolution  OUT NUMBER,
    num_steps   OUT NUMBER);
  PRAGMA restrict_references(internal_get_densification_res, wnds, rnps, wnps);

  FUNCTION internal_densify_prior_to_tfm(
    mbr SDO_GEOMETRY)
      RETURN SDO_GEOMETRY;
  PRAGMA restrict_references(internal_densify_prior_to_tfm, wnds, rnps, wnps);

  ------------------------------------------------------------------------------

  FUNCTION transform(
    geom    IN MDSYS.SDO_GEOMETRY,
    dim     IN MDSYS.SDO_DIM_ARRAY,
    to_srid IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;
  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform(
    geom      IN MDSYS.SDO_GEOMETRY,
    tolerance IN NUMBER,
    to_srid   IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform(
    geom    IN MDSYS.SDO_GEOMETRY,
    to_srid IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform_using_plan(
    geom     IN MDSYS.SDO_GEOMETRY,
    use_plan IN TFM_PLAN)
      RETURN MDSYS.SDO_GEOMETRY;

  FUNCTION transform(
    geom     IN MDSYS.SDO_GEOMETRY,
    use_plan IN TFM_PLAN)
      RETURN MDSYS.SDO_GEOMETRY;

  FUNCTION transform_orig_using_rules(
    geom     IN  mdsys.sdo_geometry,
    dim      IN  mdsys.sdo_dim_array,
    use_case IN VARCHAR2,
    tfmChain IN VARCHAR2,
    to_srid  IN  NUMBER)
      RETURN mdsys.sdo_geometry;
  PRAGMA restrict_references(transform_orig_using_rules, wnds, rnps, wnps);

  FUNCTION transform_orig_using_rules(
    geom     IN MDSYS.SDO_GEOMETRY,
    use_case IN VARCHAR2,
    tfmChain IN VARCHAR2,
    to_srid  IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform_orig_using_rules, wnds, rnps, wnps);

  FUNCTION transform_using_case_name(
    geom     IN MDSYS.SDO_GEOMETRY,
    use_case IN VARCHAR2,
    to_srid  IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform_using_case_name, wnds, rnps, wnps);

  FUNCTION transform(
    geom     IN MDSYS.SDO_GEOMETRY,
    use_case IN VARCHAR2,
    to_srid  IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform(
    geom      IN MDSYS.SDO_GEOMETRY,
    dim       IN MDSYS.SDO_DIM_ARRAY,
    to_srname IN  VARCHAR2)
      RETURN MDSYS.SDO_GEOMETRY;
--  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform(
    geom      IN MDSYS.SDO_GEOMETRY,
    tolerance IN NUMBER,
    to_srname IN VARCHAR2)
      RETURN MDSYS.SDO_GEOMETRY;
--  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  FUNCTION transform(
    geom      IN MDSYS.SDO_GEOMETRY,
    to_srname IN VARCHAR2)
      RETURN MDSYS.SDO_GEOMETRY;
--  PRAGMA restrict_references(transform, wnds, rnps, wnps);

  ------------------------------------------------------------------------------

  PROCEDURE convert_NADCON_to_XML(
    LAA_CLOB  IN CLOB,
    LOA_CLOB  IN CLOB,
    xml_grid  OUT XMLTYPE);

  PROCEDURE convert_NTv2_to_XML(
    NTv2_CLOB IN CLOB,
    xml_grid  OUT XMLTYPE);

  PROCEDURE convert_XML_to_NADCON(
    xml_grid  IN XMLTYPE,
    LAA_CLOB  OUT CLOB,
    LOA_CLOB  OUT CLOB);

  PROCEDURE convert_XML_to_NTv2(
    xml_grid  IN XMLTYPE,
    NTv2_CLOB OUT CLOB);

  PROCEDURE convert_XML_table_to_NTv2(
    ntv2_file_id IN NUMBER,
    NTv2_CLOB    OUT CLOB);

  PROCEDURE convert_NTv2_to_XML_table(
    NTv2_CLOB    IN CLOB,
    ntv2_file_id OUT NUMBER);

  PROCEDURE delete_NTv2_xml_data(
    ntv2_file_id in number);

  function determine_nadcon_coverage(
    coord_op_id number)
      return sdo_geometry;

  FUNCTION find_geog_crs(
    reference_srid          IN NUMBER,
    is_legacy               IN VARCHAR2,
    max_rel_num_difference  IN NUMBER DEFAULT 0.000001)
      RETURN SDO_SRID_LIST;

  FUNCTION find_geog_crs(
    reference_wkt           IN VARCHAR2,
    is_legacy               IN VARCHAR2,
    max_rel_num_difference  IN NUMBER DEFAULT 0.000001)
      RETURN SDO_SRID_LIST;

  FUNCTION find_proj_crs(
    reference_srid          IN NUMBER,
    is_legacy               IN VARCHAR2,
    max_rel_num_difference  IN NUMBER DEFAULT 0.000001)
      RETURN SDO_SRID_LIST;

  FUNCTION find_proj_crs(
    reference_wkt           IN VARCHAR2,
    is_legacy               IN VARCHAR2,
    max_rel_num_difference  IN NUMBER DEFAULT 0.000001)
      RETURN SDO_SRID_LIST;

  FUNCTION get_epsg_data_version
    RETURN VARCHAR2;

  ------------------------------------------------------------------------------

  FUNCTION from_OGC_SimpleFeature_SRS(
    wkt VARCHAR2)
      RETURN VARCHAR2;

  FUNCTION to_OGC_SimpleFeature_SRS(
    wkt VARCHAR2)
      RETURN VARCHAR2;

  ------------------------------------------------------------------------------

  PROCEDURE intl_populate_datum_3params(
    datum_id  NUMBER,
    op_id     NUMBER);

  PROCEDURE intl_populate_datum_7params(
    datum_id  NUMBER,
    op_id     NUMBER);

  ------------------------------------------------------------------------------

  FUNCTION to_USNG(
    geom              SDO_GEOMETRY,
    accuracyInMeters  NUMBER,
    datum             VARCHAR2 DEFAULT 'NAD83')
      RETURN
        VARCHAR2;

  FUNCTION from_USNG(
    usng              VARCHAR2,
    srid              NUMBER,
    datum             VARCHAR2 DEFAULT 'NAD83')
      RETURN SDO_GEOMETRY;

  ------------------------------------------------------------------------------

  PROCEDURE update_wkts_for_all_epsg_crs;

  PROCEDURE update_wkts_for_epsg_crs(
    srid NUMBER);

  PROCEDURE update_wkts_for_epsg_param(
    coord_op_id   NUMBER,
    parameter_id  NUMBER);

  PROCEDURE update_wkts_for_epsg_op(
    coord_op_id   NUMBER);

  PROCEDURE update_wkts_for_epsg_datum(
    datum_id NUMBER);

  PROCEDURE update_wkts_for_epsg_ellips(
    ellipsoid_id NUMBER);

  PROCEDURE update_wkts_for_epsg_pm(
    prime_meridian_id NUMBER);

  function disambiguate_proj_name(
    srid number,
    wkt varchar2)
      return varchar2;

  function disambiguate_proj_name(
    srid number)
      return varchar2;

  ------------------------------------------------------------------------------

  function is_within_tolerance(
    a               number,
    b               number,
    rel_tolerance   number)
      return number;

  FUNCTION lookup_epsg_param(
    coord_op_id     number,
    parameter_id    number)
      return number;

  PROCEDURE find_srid(
    srid                OUT NUMBER,
    epsg_srid_geog      IN NUMBER DEFAULT NULL,
    epsg_srid_proj      IN NUMBER DEFAULT NULL,
    datum_id            IN NUMBER DEFAULT NULL,
    ellips_id           IN NUMBER DEFAULT NULL,
    pm_id               IN NUMBER DEFAULT NULL,
    proj_method_id      IN NUMBER DEFAULT NULL,
    coord_ref_sys_kind  IN VARCHAR2 DEFAULT NULL,
    semi_major_axis     IN NUMBER DEFAULT NULL,
    semi_minor_axis     IN NUMBER DEFAULT NULL,
    inv_flattening      IN NUMBER DEFAULT NULL,
    params              IN MDSYS.epsg_params DEFAULT NULL);

  PROCEDURE find_srid(
    srid                OUT NUMBER,
    epsg_srid_geog      IN NUMBER DEFAULT NULL,
    epsg_srid_proj      IN NUMBER DEFAULT NULL,
    datum_id            IN NUMBER DEFAULT NULL,
    ellips_id           IN NUMBER DEFAULT NULL,
    pm_id               IN NUMBER DEFAULT NULL,
    proj_method_id      IN NUMBER DEFAULT NULL,
    proj_op_id          IN NUMBER DEFAULT NULL,
    coord_ref_sys_kind  IN VARCHAR2 DEFAULT NULL,
    semi_major_axis     IN NUMBER DEFAULT NULL,
    semi_minor_axis     IN NUMBER DEFAULT NULL,
    inv_flattening      IN NUMBER DEFAULT NULL,
    params              IN MDSYS.epsg_params DEFAULT NULL,
    max_rel_num_difference  IN NUMBER DEFAULT 0.000001);

  function fill_in_default_units(
    params mdsys.epsg_params,
    crs number)
      return mdsys.epsg_params;

  PROCEDURE sdo_cs_context_invalidate;

  PROCEDURE find_epsg_params(
    srid                IN  NUMBER,
    epsg_srid_geog      OUT NUMBER,
    datum_id            OUT NUMBER,
    ellips_id           OUT NUMBER,
    pm_id               OUT NUMBER,
    proj_method_id      OUT NUMBER,
    proj_op_id          OUT NUMBER,
    coord_ref_sys_kind  OUT VARCHAR2,
    semi_major_axis     OUT NUMBER,
    semi_minor_axis     OUT NUMBER,
    inv_flattening      OUT NUMBER,
    params              OUT MDSYS.epsg_params);

  function get_crs_dimensionality(
    srid number)
      return number;
  pragma restrict_references(get_crs_dimensionality, WNDS, WNPS, RNPS);

  function make_3d(
    geom2d in mdsys.sdo_geometry,
    height in number default 0,
    target_srid in number default null)
      return mdsys.sdo_geometry;

  function make_2d(
    geom3d in mdsys.sdo_geometry,
    target_srid in number default null)
      return mdsys.sdo_geometry;

  function convert_2d_to_3d(
    geom2d      mdsys.sdo_geometry,
    height      number             default 0,
    vert_id     number             default 9903,
    ellipsoidal varchar2           default 'TRUE',
    uom_id      number             default 9001)
      return mdsys.sdo_geometry;

  function convert_2d_srid_to_3d(
    srid2d      number,
    vert_id     number             default 9903,
    ellipsoidal varchar2           default 'TRUE',
    uom_id      number             default 9001)
      return number;

  ------------------------------------------------

  FUNCTION transform_across_dims(
    geom      IN MDSYS.SDO_GEOMETRY,
    to_srid   IN NUMBER)
      RETURN MDSYS.SDO_GEOMETRY;
  PRAGMA restrict_references(transform_across_dims, wnds, rnps, wnps);

  PROCEDURE transform_layer_across_dims(
    table_in  IN VARCHAR2,
    column_in IN VARCHAR2,
    table_out IN VARCHAR2,
    to_srid   IN NUMBER);

  PROCEDURE transform_layer_across_dims(
    table_in  IN VARCHAR2,
    column_in IN VARCHAR2,
    table_out IN VARCHAR2,
    use_plan  IN TFM_PLAN);

  PROCEDURE transform_layer_across_dims(
    table_in  IN VARCHAR2,
    column_in IN VARCHAR2,
    table_out IN VARCHAR2,
    use_case  IN VARCHAR2,
    to_srid   IN NUMBER);

  ------------------------------------------------

  function reformat_gtype(
    geom1 mdsys.sdo_geometry,
    srid2 number)
      return mdsys.sdo_geometry;

    PROCEDURE register_SrsNameSpace ( SrsNameSpace IN varchar2,
                                      SrsName IN varchar2, sdo_srid in number);
    FUNCTION get_SRID_for_NameSpace(  SrsNameSpace IN varchar2,
                                    SrsName IN varchar2) return NUMBER;
    FUNCTION get_SrsName_for_NameSpace(  SrsNameSpace IN varchar2,
                                    sdo_srid IN number) return varchar2;

  function convert_mixed_axes_units(
    srid           number,
    unitless_value number,
    output_unit    varchar2)
      return number;

  function get_3d_wkt(
    srid number)
      return varchar2;

  function generate_crs_from_wkt(
    wkt                    varchar2, -- Input WKT
    proj_op_id             number,   -- ID of new proj operation, if required
    proj_op_name           varchar2, -- Name of new proj operation, if required
    datum_tfm_op_id        number,   -- ID of new datum tfm operation, if required
    datum_tfm_op_name      varchar2, -- Name of new datum tfm operation, if required
    exclude_existing_elems varchar2 default 'FALSE') -- Exclude elements that already exist in mdsys
      return CLOB;

  function generate_epsg_rule_for_3785(
    source_srid     number,
    datum_tfm_op_id number,
    concat_op_id    number)
      return CLOB;

  function get_wkt_path(
    wkt           varchar2,
    path          varchar2,
    default_value varchar2 default null)
      return varchar2;
  PRAGMA restrict_references(get_wkt_path, wnds, rnps, wnps);

  function tfm_axis_orientation_to_wkt(
    axis_orientation VARCHAR2)
      return VARCHAR2;

  procedure update_wkt_with_7_params(
    params     varchar2,
    datum_name varchar2);

  procedure update_wkts_with_hc_params;

  procedure determine_srid_units(
    srid        in  number,
    uom_id1     out number,
    uom_id2     out number,
    uom_id3     out number);

  function transform_ogc_cs_wkt_to_srs(
    wktext varchar2)
      return varchar2;

END sdo_cs;
/


Prompt Synonym SDO_CS;
--
-- SDO_CS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_CS FOR MDSYS.SDO_CS
/


Prompt Grants on PACKAGE SDO_CS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_CS TO PUBLIC
/
