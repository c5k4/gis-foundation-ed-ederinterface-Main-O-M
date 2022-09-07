--------------------------------------------------------
--  DDL for Package VERSION_HISTORY_TIME_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "SDE"."VERSION_HISTORY_TIME_PKG" AS
	PROCEDURE SET_VERSIONSTIME_TO_USE(v_lowtime DATE);
FUNCTION GET_LOWVERSIONTIME  return  DATE;
END VERSION_HISTORY_TIME_PKG;
