Prompt drop Package PONS;
DROP PACKAGE PONS_RO.PONS
/

Prompt Package PONS;
--
-- PONS  (Package) 
--
CREATE OR REPLACE PACKAGE PONS_RO.PONS
   as



     ----- PROCEDURE TO GET CUSTOMER INFO THROUGH GLOBALIDS OF SERVICEPOINTS AS BOX UNCHECKED -------

     procedure CUST_INFO_SERVICEPOINTS_INCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER);

     ----- PROCEDURE TO GET CUSTOMER INFO THROUGH GLOBALIDS OF SERVICEPOINTS AS BOX UNCHECKED -------

     procedure CUST_INFO_SERVICEPOINTS_EXCL(P_INPUTSTRING IN CLOB,
                            P_VERSION IN VARCHAR2,
                            P_CURSOR out sys_refcursor,
                            P_ERROR OUT VARCHAR2,
                            P_SUCCESS OUT NUMBER);




     ------ PROCEDURE TO GENERATE CUSTOMER INFO VIA CIRCUITIDS AS UNCHECKED ------

     PROCEDURE CUST_INFO_CIRCUITID_INCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER);

     ------ PROCEDURE TO GENERATE CUSTOMER INFO VIA CIRCUITIDS AS CHECKED ------

     PROCEDURE CUST_INFO_CIRCUITID_EXCL (P_INPUTSTRING in VARCHAR2,
                                    P_VERSION in VARCHAR2,
                                    p_cursor out sys_refcursor,
                                    p_error OUT VARCHAR2,
                                    p_success OUT NUMBER);


    ------ PROCEDURE TO GENERATE SHUTDOWNID -----

    PROCEDURE GEN_SHUTDOWNID(
          p_shutdownId OUT NUMBER,
          p_error OUT VARCHAR2,
          p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS OF PRIMARY METERS -----

    PROCEDURE CUST_INFO_PM_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS OF PRIMARY METERS -----

    PROCEDURE CUST_INFO_PM_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

  ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS AS BOX UNCHECKED -----

    PROCEDURE CUST_INFO_CGC_INCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

    ----- PROCEDURE TO GET CUSTOMER INFORMATION VIA CGC NUMBERS AS BOX CHECKED -----

    PROCEDURE CUST_INFO_CGC_EXCL(
            P_INPUTSTRING IN VARCHAR2,
            P_VERSION in VARCHAR2,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

   PROCEDURE GETDIVISION_FEEDER(
            P_DIVISION IN NUMBER,
            p_cursor OUT sys_refcursor,
            p_error OUT VARCHAR2,
            p_success OUT NUMBER);

   end PONS; -- Package Ends
/
