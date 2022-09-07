--------------------------------------------------------
--  DDL for Package LAYERS_INFO
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "SDE"."LAYERS_INFO" AS
	FUNCTION GETMINXFORSHAPE(t_owner IN NVARCHAR2,t_name IN NVARCHAR2,t_shapenum IN NUMBER) RETURN NUMBER;
	FUNCTION GETMINYFORSHAPE(t_owner IN NVARCHAR2,t_name IN NVARCHAR2,t_shapenum IN NUMBER) RETURN NUMBER;
    FUNCTION GETNEXTROWID(t_owner IN NVARCHAR2,t_name IN NVARCHAR2) RETURN NUMBER;
END LAYERS_INFO;
