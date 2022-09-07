--------------------------------------------------------
--  DDL for Package EDGIS_SAP
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "EDGIS"."EDGIS_SAP" AS
    PROCEDURE POPULATE_NOTIFICATION_FC;
    FUNCTION GET_SAP_SUBTYPE(notif_type IN NVARCHAR2) RETURN NUMBER;
END EDGIS_SAP;
