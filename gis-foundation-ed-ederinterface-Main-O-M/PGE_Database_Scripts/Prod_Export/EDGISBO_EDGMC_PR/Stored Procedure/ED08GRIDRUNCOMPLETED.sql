--------------------------------------------------------
--  DDL for Procedure ED08GRIDRUNCOMPLETED
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."ED08GRIDRUNCOMPLETED" AS
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE EDGISBO.PGE_CIRCUITTOMAPNUM';
    INSERT INTO EDGISBO.PGE_CIRCUITTOMAPNUM (SELECT * FROM EDGISBO.PGE_CIRCUITTOMAPNUM_TEMP);
    commit;
END ED08GridRunCompleted ;
