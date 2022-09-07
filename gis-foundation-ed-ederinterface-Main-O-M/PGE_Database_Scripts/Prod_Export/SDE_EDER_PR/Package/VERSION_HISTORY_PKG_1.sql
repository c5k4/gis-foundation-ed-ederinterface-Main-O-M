--------------------------------------------------------
--  DDL for Package Body VERSION_HISTORY_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."VERSION_HISTORY_PKG" AS
LOW_VERSION_NAME               NVARCHAR2(64) ;
HIGH_VERSION_NAME               NVARCHAR2(64);
PROCEDURE SET_VERSIONS_TO_USE( v_lowtime NVARCHAR2, v_hightime NVARCHAR2)
IS
BEGIN
     LOW_VERSION_NAME := v_lowtime;
	 HIGH_VERSION_NAME := v_hightime;
END;
FUNCTION GET_HIGHVERSION
return NVARCHAR2 is begin return HIGH_VERSION_NAME;
END;
FUNCTION GET_LOWVERSION
return NVARCHAR2 is begin return LOW_VERSION_NAME;
END;
END VERSION_HISTORY_PKG;
