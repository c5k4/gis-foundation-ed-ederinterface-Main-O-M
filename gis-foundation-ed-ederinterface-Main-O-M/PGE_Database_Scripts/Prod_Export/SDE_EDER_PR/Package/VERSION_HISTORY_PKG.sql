--------------------------------------------------------
--  DDL for Package VERSION_HISTORY_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "SDE"."VERSION_HISTORY_PKG" AS
	PROCEDURE SET_VERSIONS_TO_USE(v_lowtime IN NVARCHAR2,v_hightime IN NVARCHAR2);
FUNCTION GET_HIGHVERSION  return  NVARCHAR2;
FUNCTION GET_LOWVERSION  return  NVARCHAR2;
END VERSION_HISTORY_PKG;
