--------------------------------------------------------
--  DDL for Package Body VERSION_HISTORY_TIME_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "UC4ADMIN"."VERSION_HISTORY_TIME_PKG" AS
LOW_VERSION_TIME               DATE ;
PROCEDURE SET_VERSIONSTIME_TO_USE( v_lowtime DATE)
IS
BEGIN
     LOW_VERSION_TIME := v_lowtime;
END;
FUNCTION GET_LOWVERSIONTIME
return DATE is begin return LOW_VERSION_TIME;
END;
END VERSION_HISTORY_TIME_PKG;
