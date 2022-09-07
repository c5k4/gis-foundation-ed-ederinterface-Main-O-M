Prompt drop View CS_SRS;
DROP VIEW MDSYS.CS_SRS
/

/* Formatted on 6/27/2019 02:52:00 PM (QP5 v5.313) */
PROMPT View CS_SRS;
--
-- CS_SRS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.CS_SRS
(
    CS_NAME,
    SRID,
    AUTH_SRID,
    AUTH_NAME,
    WKTEXT,
    CS_BOUNDS,
    WKTEXT3D
)
AS
    (SELECT "CS_NAME",
            "SRID",
            "AUTH_SRID",
            "AUTH_NAME",
            "WKTEXT",
            "CS_BOUNDS",
            "WKTEXT3D"
       FROM MDSYS.SDO_CS_SRS)
/


Prompt Trigger CS_SRS_TRIGGER;
--
-- CS_SRS_TRIGGER  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.CS_SRS_TRIGGER
INSTEAD OF
  UPDATE OR
  INSERT OR
  DELETE
ON
  MDSYS.CS_SRS
FOR EACH ROW
BEGIN
  MDSYS.sdo_cs.sdo_cs_context_invalidate;

  if(not(:old.srid is null)) then
    delete from sdo_coord_ref_system crs where crs.srid = :old.srid;
  end if;
  if(not(:new.srid is null)) then
    if(upper(trim(:new.wktext)) like 'GEOGCS%') then
      insert into sdo_coord_ref_system (
        SRID,
        COORD_REF_SYS_NAME,
        COORD_REF_SYS_KIND,
        COORD_SYS_ID,
        DATUM_ID,
        GEOG_CRS_DATUM_ID,
        SOURCE_GEOG_SRID,
        PROJECTION_CONV_ID,
        CMPD_HORIZ_SRID,
        CMPD_VERT_SRID,
        INFORMATION_SOURCE,
        DATA_SOURCE,
        IS_LEGACY,
        LEGACY_CODE,
        LEGACY_WKTEXT,
        LEGACY_CS_BOUNDS,
        IS_VALID,
        SUPPORTS_SDO_GEOMETRY)
      values(
        :new.SRID,
        :new.CS_NAME,
        'GEOGRAPHIC2D',
        null,
        1000000123,
        1000000123,
        null,
        null,
        null,
        null,
        null,
        null,
        'TRUE',
        null,
        :new.WKTEXT,
        :new.CS_BOUNDS,
        'TRUE',
        'TRUE');
    else
      insert into sdo_coord_ref_system (
        SRID,
        COORD_REF_SYS_NAME,
        COORD_REF_SYS_KIND,
        COORD_SYS_ID,
        DATUM_ID,
        GEOG_CRS_DATUM_ID,
        SOURCE_GEOG_SRID,
        PROJECTION_CONV_ID,
        CMPD_HORIZ_SRID,
        CMPD_VERT_SRID,
        INFORMATION_SOURCE,
        DATA_SOURCE,
        IS_LEGACY,
        LEGACY_CODE,
        LEGACY_WKTEXT,
        LEGACY_CS_BOUNDS,
        IS_VALID,
        SUPPORTS_SDO_GEOMETRY)
      values(
        :new.SRID,
        :new.CS_NAME,
        'PROJECTED',
        null,
        null,
        1000000123,
        1000000123,
        null,
        null,
        null,
        null,
        null,
        'TRUE',
        null,
        :new.WKTEXT,
        :new.CS_BOUNDS,
        'TRUE',
        'TRUE');
    end if;
  end if;
END;
/


Prompt Synonym CS_SRS;
--
-- CS_SRS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM CS_SRS FOR MDSYS.CS_SRS
/


Prompt Grants on VIEW CS_SRS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.CS_SRS TO PUBLIC
/
