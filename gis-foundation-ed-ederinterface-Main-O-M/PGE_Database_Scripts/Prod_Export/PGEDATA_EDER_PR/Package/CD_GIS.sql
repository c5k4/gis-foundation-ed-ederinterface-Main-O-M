--------------------------------------------------------
--  DDL for Package CD_GIS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PGEDATA"."CD_GIS" AS
	FUNCTION IS_FIELD_CD(t_name IN NVARCHAR2,field_name IN NVARCHAR2,cd_type NVARCHAR2) RETURN BOOLEAN;
    PROCEDURE SET_FIELD_CD(t_name IN NVARCHAR2, GLOBALID IN NVARCHAR2, cd_type NVARCHAR2) ;
    PROCEDURE UNSET_FIELD_CD(t_name IN NVARCHAR2, GLOBALID IN NVARCHAR2, cd_type NVARCHAR2) ;
END CD_GIS;
