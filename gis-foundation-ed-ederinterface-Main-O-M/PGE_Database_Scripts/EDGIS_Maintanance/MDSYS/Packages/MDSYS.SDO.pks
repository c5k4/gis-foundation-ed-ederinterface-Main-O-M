Prompt drop Package SDO;
DROP PACKAGE MDSYS.SDO
/

Prompt Package SDO;
--
-- SDO  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO as


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function compare (hhcode1 IN RAW, hhcode2 IN RAW, hhcode3 IN RAW:=NULL)
return VARCHAR2;
pragma restrict_references(compare, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function latlontocode (latitude IN NUMBER, longitude IN NUMBER)
return RAW;
pragma restrict_references(latlontocode, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function datetodim (dateval IN DATE, component IN BINARY_INTEGER:=0)
return RAW;
pragma restrict_references(datetodim, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function bvaluetodim (bval IN NUMBER, lbound IN NUMBER, ubound IN NUMBER,
                 precision IN BINARY_INTEGER)
return RAW;
pragma restrict_references(bvaluetodim, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function hvaluetodim (hval1 IN BINARY_INTEGER, bit_length1 IN BINARY_INTEGER,
                 hval2  IN BINARY_INTEGER:=0, bit_length2  IN BINARY_INTEGER:=0,
                 hval3  IN BINARY_INTEGER:=0, bit_length3  IN BINARY_INTEGER:=0,
                 hval4  IN BINARY_INTEGER:=0, bit_length4  IN BINARY_INTEGER:=0,
                 hval5  IN BINARY_INTEGER:=0, bit_length5  IN BINARY_INTEGER:=0,
                 hval6  IN BINARY_INTEGER:=0, bit_length6  IN BINARY_INTEGER:=0,
                 hval7  IN BINARY_INTEGER:=0, bit_length7  IN BINARY_INTEGER:=0,
                 hval8  IN BINARY_INTEGER:=0, bit_length8  IN BINARY_INTEGER:=0,
                 hval9  IN BINARY_INTEGER:=0, bit_length9  IN BINARY_INTEGER:=0,
                 hval10 IN BINARY_INTEGER:=0, bit_length10 IN BINARY_INTEGER:=0,
                 hval11 IN BINARY_INTEGER:=0, bit_length11 IN BINARY_INTEGER:=0,
                 hval12 IN BINARY_INTEGER:=0, bit_length12 IN BINARY_INTEGER:=0,
                 hval13 IN BINARY_INTEGER:=0, bit_length13 IN BINARY_INTEGER:=0,
                 hval14 IN BINARY_INTEGER:=0, bit_length14 IN BINARY_INTEGER:=0,
                 hval15 IN BINARY_INTEGER:=0, bit_length15 IN BINARY_INTEGER:=0,
                 hval16 IN BINARY_INTEGER:=0, bit_length16 IN BINARY_INTEGER:=0)
return RAW;
pragma restrict_references(hvaluetodim, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function encode (dimension1  IN RAW,       dimension2  IN RAW:=NULL,
                 dimension3  IN RAW:=NULL, dimension4  IN RAW:=NULL,
                 dimension5  IN RAW:=NULL, dimension6  IN RAW:=NULL,
                 dimension7  IN RAW:=NULL, dimension8  IN RAW:=NULL,
                 dimension9  IN RAW:=NULL, dimension10 IN RAW:=NULL,
                 dimension11 IN RAW:=NULL, dimension12 IN RAW:=NULL,
                 dimension13 IN RAW:=NULL, dimension14 IN RAW:=NULL,
                 dimension15 IN RAW:=NULL, dimension16 IN RAW:=NULL,
                 dimension17 IN RAW:=NULL, dimension18 IN RAW:=NULL,
                 dimension19 IN RAW:=NULL, dimension20 IN RAW:=NULL,
                 dimension21 IN RAW:=NULL, dimension22 IN RAW:=NULL,
                 dimension23 IN RAW:=NULL, dimension24 IN RAW:=NULL,
                 dimension25 IN RAW:=NULL, dimension26 IN RAW:=NULL,
                 dimension27 IN RAW:=NULL, dimension28 IN RAW:=NULL,
                 dimension29 IN RAW:=NULL, dimension30 IN RAW:=NULL,
                 dimension31 IN RAW:=NULL, dimension32 IN RAW:=NULL)
return RAW;
pragma restrict_references(encode, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function decode (hhcode IN RAW, dimension_num IN BINARY_INTEGER)
return RAW;
pragma restrict_references(decode, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function to_lat (dimension IN RAW)
return NUMBER;
pragma restrict_references(to_lat, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function to_lon (dimension IN RAW)
return  NUMBER;
pragma restrict_references(to_lon, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function to_date (dimension IN RAW)
return DATE;
pragma restrict_references(to_date, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function to_bvalue (dimension IN RAW, lbound IN NUMBER, ubound IN NUMBER)
return NUMBER;
pragma restrict_references(to_bvalue, WNDS, RNDS, WNPS, RNPS);


--
-- NAME:
--
-- DESCRIPTION:
--
-- ARGUMENTS:
--
-- RETURNS:
--

function to_hvalue (dimension IN RAW, bit_position IN BINARY_INTEGER,
                                      bit_length   IN BINARY_INTEGER)
return NUMBER;
pragma restrict_references(to_hvalue, WNDS, RNDS, WNPS, RNPS);


end SDO;
/


Prompt Synonym SDO;
--
-- SDO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO FOR MDSYS.SDO
/


Prompt Grants on PACKAGE SDO TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO TO PUBLIC
/
