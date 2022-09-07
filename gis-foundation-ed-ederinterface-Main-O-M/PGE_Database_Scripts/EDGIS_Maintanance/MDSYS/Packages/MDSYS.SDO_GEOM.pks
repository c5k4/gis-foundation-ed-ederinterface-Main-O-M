Prompt drop Package SDO_GEOM;
DROP PACKAGE MDSYS.SDO_GEOM
/

Prompt Package SDO_GEOM;
--
-- SDO_GEOM  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_GEOM authid current_user as

------------------------------------------------------------------------------
-- PART I. Functions and procedures for non-object model                    --
------------------------------------------------------------------------------

/*
-- Package constants which represent geometry element types for calling
-- the init_element() function.
*/

POINT_TYPE       CONSTANT INTEGER := MDSYS.MD2.POINT_TYPE;
LINESTRING_TYPE  CONSTANT INTEGER := MDSYS.MD2.LINESTRING_TYPE;
POLYGON_TYPE     CONSTANT INTEGER := MDSYS.MD2.POLYGON_TYPE;

/*
-- Storage type
-- SDO_NORMALIZED_STORAGE indicates pre-8.1.3 schema
-- SDO_OBJECT_STORAGE indicates 8.1.3 schema
*/

SDO_NORMALIZED_STORAGE  CONSTANT INTEGER := 805;
SDO_OBJECT_STORAGE      CONSTANT INTEGER := 813;

/*
-- This pacakge constant represents the maximum number of ordinate values
-- in the parameter lists for add_nodes() and interact().
*/

MAX_ORD_PARAMS CONSTANT INTEGER := 250;

--
--  NAME:
--     init_element - INITialize an ELEMENT of a geometric object
--  DESCRIPTION:
--     This function generates a unique sequence number for each element of a
--     geometric object.  The user must ensure that each GID is unique in a
--     data set layer.
--  ARGUMENTS:
--     layer  - SDO geometry table name (LAYER_sdogeom) input argument
--     gid    - Geometry IDentifier input argument
--  RETURNS:
--     a unique sequence number for each element of a geometric object

function init_element (layer IN VARCHAR2, gid IN NUMBER)
return INTEGER;


--
--  NAME:
--     add_nodes - ADD the NODES of the element components of geometric objects
--                 to the SDO geometry table
--  DESCRIPTION:
--     This procedure adds vertex coordinates representing the linestring
--     segments of the element components of geometric objects to the geometry
--     table.  A unique sequence number is generated for each row of linestring
--     segments represented by the vertex coordinates in the parameter list.
--     Iterative calls to this function with the same GID and ESEQ are used to
--     add linestring segments represented by the vertex coordinates to an
--     element of a geometric object.
--  ARGUMENTS:
--     layer  - geometry table name (LAYER_sdogeom) input argument
--     gid    - Geometry IDentifier input argument
--     eseq   - Element SEQuence number input argument
--     etype  - Element TYPE input argument (see types defined above)
--     ordn   - ORDinate Nth input argument (up to 250 per call, 2 required)
/*
procedure add_nodes (layer  IN VARCHAR2, gid IN NUMBER, eseq IN INTEGER,
                     etype  IN INTEGER, ord1 IN NUMBER, ord2 IN  NUMBER,
                     ord3   IN NUMBER:=NULL, ord4   IN NUMBER:=NULL,
                     ord5   IN NUMBER:=NULL, ord6   IN NUMBER:=NULL,
                     ord7   IN NUMBER:=NULL, ord8   IN NUMBER:=NULL,
                     ord9   IN NUMBER:=NULL, ord10  IN NUMBER:=NULL,
                     ord11  IN NUMBER:=NULL, ord12  IN NUMBER:=NULL,
                     ord13  IN NUMBER:=NULL, ord14  IN NUMBER:=NULL,
                     ord15  IN NUMBER:=NULL, ord16  IN NUMBER:=NULL,
                     ord17  IN NUMBER:=NULL, ord18  IN NUMBER:=NULL,
                     ord19  IN NUMBER:=NULL, ord20  IN NUMBER:=NULL,
                     ord21  IN NUMBER:=NULL, ord22  IN NUMBER:=NULL,
                     ord23  IN NUMBER:=NULL, ord24  IN NUMBER:=NULL,
                     ord25  IN NUMBER:=NULL, ord26  IN NUMBER:=NULL,
                     ord27  IN NUMBER:=NULL, ord28  IN NUMBER:=NULL,
                     ord29  IN NUMBER:=NULL, ord30  IN NUMBER:=NULL,
                     ord31  IN NUMBER:=NULL, ord32  IN NUMBER:=NULL,
                     ord33  IN NUMBER:=NULL, ord34  IN NUMBER:=NULL,
                     ord35  IN NUMBER:=NULL, ord36  IN NUMBER:=NULL,
                     ord37  IN NUMBER:=NULL, ord38  IN NUMBER:=NULL,
                     ord39  IN NUMBER:=NULL, ord40  IN NUMBER:=NULL,
                     ord41  IN NUMBER:=NULL, ord42  IN NUMBER:=NULL,
                     ord43  IN NUMBER:=NULL, ord44  IN NUMBER:=NULL,
                     ord45  IN NUMBER:=NULL, ord46  IN NUMBER:=NULL,
                     ord47  IN NUMBER:=NULL, ord48  IN NUMBER:=NULL,
                     ord49  IN NUMBER:=NULL, ord50  IN NUMBER:=NULL,
                     ord51  IN NUMBER:=NULL, ord52  IN NUMBER:=NULL,
                     ord53  IN NUMBER:=NULL, ord54  IN NUMBER:=NULL,
                     ord55  IN NUMBER:=NULL, ord56  IN NUMBER:=NULL,
                     ord57  IN NUMBER:=NULL, ord58  IN NUMBER:=NULL,
                     ord59  IN NUMBER:=NULL, ord60  IN NUMBER:=NULL,
                     ord61  IN NUMBER:=NULL, ord62  IN NUMBER:=NULL,
                     ord63  IN NUMBER:=NULL, ord64  IN NUMBER:=NULL,
                     ord65  IN NUMBER:=NULL, ord66  IN NUMBER:=NULL,
                     ord67  IN NUMBER:=NULL, ord68  IN NUMBER:=NULL,
                     ord69  IN NUMBER:=NULL, ord70  IN NUMBER:=NULL,
                     ord71  IN NUMBER:=NULL, ord72  IN NUMBER:=NULL,
                     ord73  IN NUMBER:=NULL, ord74  IN NUMBER:=NULL,
                     ord75  IN NUMBER:=NULL, ord76  IN NUMBER:=NULL,
                     ord77  IN NUMBER:=NULL, ord78  IN NUMBER:=NULL,
                     ord79  IN NUMBER:=NULL, ord80  IN NUMBER:=NULL,
                     ord81  IN NUMBER:=NULL, ord82  IN NUMBER:=NULL,
                     ord83  IN NUMBER:=NULL, ord84  IN NUMBER:=NULL,
                     ord85  IN NUMBER:=NULL, ord86  IN NUMBER:=NULL,
                     ord87  IN NUMBER:=NULL, ord88  IN NUMBER:=NULL,
                     ord89  IN NUMBER:=NULL, ord90  IN NUMBER:=NULL,
                     ord91  IN NUMBER:=NULL, ord92  IN NUMBER:=NULL,
                     ord93  IN NUMBER:=NULL, ord94  IN NUMBER:=NULL,
                     ord95  IN NUMBER:=NULL, ord96  IN NUMBER:=NULL,
                     ord97  IN NUMBER:=NULL, ord98  IN NUMBER:=NULL,
                     ord99  IN NUMBER:=NULL, ord100 IN NUMBER:=NULL,
                     ord101 IN NUMBER:=NULL, ord102 IN NUMBER:=NULL,
                     ord103 IN NUMBER:=NULL, ord104 IN NUMBER:=NULL,
                     ord105 IN NUMBER:=NULL, ord106 IN NUMBER:=NULL,
                     ord107 IN NUMBER:=NULL, ord108 IN NUMBER:=NULL,
                     ord109 IN NUMBER:=NULL, ord110 IN NUMBER:=NULL,
                     ord111 IN NUMBER:=NULL, ord112 IN NUMBER:=NULL,
                     ord113 IN NUMBER:=NULL, ord114 IN NUMBER:=NULL,
                     ord115 IN NUMBER:=NULL, ord116 IN NUMBER:=NULL,
                     ord117 IN NUMBER:=NULL, ord118 IN NUMBER:=NULL,
                     ord119 IN NUMBER:=NULL, ord120 IN NUMBER:=NULL,
                     ord121 IN NUMBER:=NULL, ord122 IN NUMBER:=NULL,
                     ord123 IN NUMBER:=NULL, ord124 IN NUMBER:=NULL,
                     ord125 IN NUMBER:=NULL, ord126 IN NUMBER:=NULL,
                     ord127 IN NUMBER:=NULL, ord128 IN NUMBER:=NULL,
                     ord129 IN NUMBER:=NULL, ord130 IN NUMBER:=NULL,
                     ord131 IN NUMBER:=NULL, ord132 IN NUMBER:=NULL,
                     ord133 IN NUMBER:=NULL, ord134 IN NUMBER:=NULL,
                     ord135 IN NUMBER:=NULL, ord136 IN NUMBER:=NULL,
                     ord137 IN NUMBER:=NULL, ord138 IN NUMBER:=NULL,
                     ord139 IN NUMBER:=NULL, ord140 IN NUMBER:=NULL,
                     ord141 IN NUMBER:=NULL, ord142 IN NUMBER:=NULL,
                     ord143 IN NUMBER:=NULL, ord144 IN NUMBER:=NULL,
                     ord145 IN NUMBER:=NULL, ord146 IN NUMBER:=NULL,
                     ord147 IN NUMBER:=NULL, ord148 IN NUMBER:=NULL,
                     ord149 IN NUMBER:=NULL, ord150 IN NUMBER:=NULL,
                     ord151 IN NUMBER:=NULL, ord152 IN NUMBER:=NULL,
                     ord153 IN NUMBER:=NULL, ord154 IN NUMBER:=NULL,
                     ord155 IN NUMBER:=NULL, ord156 IN NUMBER:=NULL,
                     ord157 IN NUMBER:=NULL, ord158 IN NUMBER:=NULL,
                     ord159 IN NUMBER:=NULL, ord160 IN NUMBER:=NULL,
                     ord161 IN NUMBER:=NULL, ord162 IN NUMBER:=NULL,
                     ord163 IN NUMBER:=NULL, ord164 IN NUMBER:=NULL,
                     ord165 IN NUMBER:=NULL, ord166 IN NUMBER:=NULL,
                     ord167 IN NUMBER:=NULL, ord168 IN NUMBER:=NULL,
                     ord169 IN NUMBER:=NULL, ord170 IN NUMBER:=NULL,
                     ord171 IN NUMBER:=NULL, ord172 IN NUMBER:=NULL,
                     ord173 IN NUMBER:=NULL, ord174 IN NUMBER:=NULL,
                     ord175 IN NUMBER:=NULL, ord176 IN NUMBER:=NULL,
                     ord177 IN NUMBER:=NULL, ord178 IN NUMBER:=NULL,
                     ord179 IN NUMBER:=NULL, ord180 IN NUMBER:=NULL,
                     ord181 IN NUMBER:=NULL, ord182 IN NUMBER:=NULL,
                     ord183 IN NUMBER:=NULL, ord184 IN NUMBER:=NULL,
                     ord185 IN NUMBER:=NULL, ord186 IN NUMBER:=NULL,
                     ord187 IN NUMBER:=NULL, ord188 IN NUMBER:=NULL,
                     ord189 IN NUMBER:=NULL, ord190 IN NUMBER:=NULL,
                     ord191 IN NUMBER:=NULL, ord192 IN NUMBER:=NULL,
                     ord193 IN NUMBER:=NULL, ord194 IN NUMBER:=NULL,
                     ord195 IN NUMBER:=NULL, ord196 IN NUMBER:=NULL,
                     ord197 IN NUMBER:=NULL, ord198 IN NUMBER:=NULL,
                     ord199 IN NUMBER:=NULL, ord200 IN NUMBER:=NULL,
                     ord201 IN NUMBER:=NULL, ord202 IN NUMBER:=NULL,
                     ord203 IN NUMBER:=NULL, ord204 IN NUMBER:=NULL,
                     ord205 IN NUMBER:=NULL, ord206 IN NUMBER:=NULL,
                     ord207 IN NUMBER:=NULL, ord208 IN NUMBER:=NULL,
                     ord209 IN NUMBER:=NULL, ord210 IN NUMBER:=NULL,
                     ord211 IN NUMBER:=NULL, ord212 IN NUMBER:=NULL,
                     ord213 IN NUMBER:=NULL, ord214 IN NUMBER:=NULL,
                     ord215 IN NUMBER:=NULL, ord216 IN NUMBER:=NULL,
                     ord217 IN NUMBER:=NULL, ord218 IN NUMBER:=NULL,
                     ord219 IN NUMBER:=NULL, ord220 IN NUMBER:=NULL,
                     ord221 IN NUMBER:=NULL, ord222 IN NUMBER:=NULL,
                     ord223 IN NUMBER:=NULL, ord224 IN NUMBER:=NULL,
                     ord225 IN NUMBER:=NULL, ord226 IN NUMBER:=NULL,
                     ord227 IN NUMBER:=NULL, ord228 IN NUMBER:=NULL,
                     ord229 IN NUMBER:=NULL, ord230 IN NUMBER:=NULL,
                     ord231 IN NUMBER:=NULL, ord232 IN NUMBER:=NULL,
                     ord233 IN NUMBER:=NULL, ord234 IN NUMBER:=NULL,
                     ord235 IN NUMBER:=NULL, ord236 IN NUMBER:=NULL,
                     ord237 IN NUMBER:=NULL, ord238 IN NUMBER:=NULL,
                     ord239 IN NUMBER:=NULL, ord240 IN NUMBER:=NULL,
                     ord241 IN NUMBER:=NULL, ord242 IN NUMBER:=NULL,
                     ord243 IN NUMBER:=NULL, ord244 IN NUMBER:=NULL,
                     ord245 IN NUMBER:=NULL, ord246 IN NUMBER:=NULL,
                     ord247 IN NUMBER:=NULL, ord248 IN NUMBER:=NULL,
                     ord249 IN NUMBER:=NULL, ord250 IN NUMBER:=NULL);

*/

--
--  NAME:
--     validate_geometry - VALIDATE a GEOMETRY in SDO geometry table
--     validate_layer    - VALIDATE all geometries in the specified LAYER
--  DESCRIPTION:
--     validate_geometry function examines the properties of elements of a
--     geometry. Coordinates must be within the bounds designated in the
--     SDODIM table. Point elements must have exactly one (only one)
--     coordinate per row. Linestring elements must have at least two
--     coordinates (representing one segment). Polygon elements must have
--     at least four coordinates (representing three segments). The very
--     first coordinate must be repeated as the last coordinate to close
--     the polygon. The first coordinate of a continuation row for both
--     linestring and polygon elements must be equal to the last coordinate
--     of the previous row.
--
--  ARGUMENTS:
--     layer         - geometry table name (LAYER_sdogeom) input argument
--     gid           - Geometry IDentifier input argument
--     result_table  - the result table where validation result is stored
--
--  RETURNS:
--     'TRUE' if the geometry is valid. Otherwise it returns Oracle
--     standard error code for known error or 'FALSE' for unknown error.
--     validate_layer procedure populates the given result_table with
--     validation results
--

function validate_geometry(layer IN VARCHAR2, gid IN NUMBER)
return VARCHAR2;
-- pragma restrict_references(validate_geometry, WNDS, WNPS, RNPS );

procedure validate_layer(layer IN VARCHAR2, result_table IN VARCHAR2);

function check_bounds(TableName IN VARCHAR2, gid IN NUMBER, Eseq IN INTEGER,
                      Seq IN INTEGER, CoordWidth IN NUMBER,
		      Dim1LB IN NUMBER, Dim1UB IN NUMBER,
		      Dim2LB IN NUMBER, Dim2UB IN NUMBER)
	 return INTEGER;
-- pragma restrict_references(check_bounds, WNDS, WNPS, RNPS);

procedure get_ords(TableName IN VARCHAR2, gid IN NUMBER, Eseq IN INTEGER,
                   Seq IN INTEGER, ColumnIndex IN INTEGER,
                   XRtn OUT NUMBER, YRtn OUT NUMBER);
pragma restrict_references(get_ords, WNDS, WNPS, RNPS);


--
--  NAME:
--     interact - do two geometric objects INTERACT with each other?
--  DESCRIPTION:
--     This function determines whether or not two geometric objects in either
--     the same or different SDO geometry tables (LAYER_sdogeom) interact with
--     each other.
--  ARGUMENTS:
--     layer       - geometry table name (LAYER_sdogeom) input argument
--     gid         - Geometry IDentifier input argument
--     x_tolerance - allowed variance of ordinate values within dimension1
--     y_tolerance - allowed variance of ordinate values within dimension 2
--     vNNN        - list of up to 250 input values describing the elements
--                   of a geometric object with the following structure:
--                   etype, # of ordinate values, list of ordinate values.
--                   For example the following list could be used to describe
--                   a four point polygon with with a triangular hole :
--                   SDO_GEOM.POLYGON_TYPE, 10, ord1, ord2, ord3, ord4, ord5,
--                   ord6, ord7, ord8, ord9, ord10,
--                   SDO_GEOM.POLYGON_TYPE, 8, ord1, ord2, ord3, ord4, ord5,
--                   ord6, ord7, ord8
--                   The following example could be used to describe a two
--                   element (single segment) linestring:
--                   SDO_GEOM.LINESTRING_TYPE, 4, ord1, ord2, ord3, ord4,
--                   SDO_GEOM.LINESTRING_TYPE, 4, ord1, ord2, ord3, ord4
--  RETURNS:
--     'TRUE' or 'FALSE'

function interact (layer IN VARCHAR2, gid1 IN NUMBER, gid2 IN NUMBER)
return VARCHAR2;
-- pragma restrict_references(interact, WNDS, WNPS, RNPS);

function interact (layer1 IN VARCHAR2, gid1 IN NUMBER,
                   layer2 IN VARCHAR2, gid2 IN NUMBER)
return VARCHAR2;
-- pragma restrict_references(interact, WNDS, WNPS, RNPS);

function interact(layer1 IN VARCHAR2, gid1 IN NUMBER,
                  x_tolerance IN NUMBER, y_tolerance IN NUMBER,
                     v000 IN NUMBER:=NULL, v001 IN NUMBER:=NULL,
                     v002 IN NUMBER:=NULL, v003 IN NUMBER:=NULL,
                     v004 IN NUMBER:=NULL, v005 IN NUMBER:=NULL,
                     v006 IN NUMBER:=NULL, v007 IN NUMBER:=NULL,
                     v008 IN NUMBER:=NULL, v009 IN NUMBER:=NULL,
                     v010 IN NUMBER:=NULL, v011 IN NUMBER:=NULL,
                     v012 IN NUMBER:=NULL, v013 IN NUMBER:=NULL,
                     v014 IN NUMBER:=NULL, v015 IN NUMBER:=NULL,
                     v016 IN NUMBER:=NULL, v017 IN NUMBER:=NULL,
                     v018 IN NUMBER:=NULL, v019 IN NUMBER:=NULL,
                     v020 IN NUMBER:=NULL, v021 IN NUMBER:=NULL,
                     v022 IN NUMBER:=NULL, v023 IN NUMBER:=NULL,
                     v024 IN NUMBER:=NULL, v025 IN NUMBER:=NULL,
                     v026 IN NUMBER:=NULL, v027 IN NUMBER:=NULL,
                     v028 IN NUMBER:=NULL, v029 IN NUMBER:=NULL,
                     v030 IN NUMBER:=NULL, v031 IN NUMBER:=NULL,
                     v032 IN NUMBER:=NULL, v033 IN NUMBER:=NULL,
                     v034 IN NUMBER:=NULL, v035 IN NUMBER:=NULL,
                     v036 IN NUMBER:=NULL, v037 IN NUMBER:=NULL,
                     v038 IN NUMBER:=NULL, v039 IN NUMBER:=NULL,
                     v040 IN NUMBER:=NULL, v041 IN NUMBER:=NULL,
                     v042 IN NUMBER:=NULL, v043 IN NUMBER:=NULL,
                     v044 IN NUMBER:=NULL, v045 IN NUMBER:=NULL,
                     v046 IN NUMBER:=NULL, v047 IN NUMBER:=NULL,
                     v048 IN NUMBER:=NULL, v049 IN NUMBER:=NULL,
                     v050 IN NUMBER:=NULL, v051 IN NUMBER:=NULL,
                     v052 IN NUMBER:=NULL, v053 IN NUMBER:=NULL,
                     v054 IN NUMBER:=NULL, v055 IN NUMBER:=NULL,
                     v056 IN NUMBER:=NULL, v057 IN NUMBER:=NULL,
                     v058 IN NUMBER:=NULL, v059 IN NUMBER:=NULL,
                     v060 IN NUMBER:=NULL, v061 IN NUMBER:=NULL,
                     v062 IN NUMBER:=NULL, v063 IN NUMBER:=NULL,
                     v064 IN NUMBER:=NULL, v065 IN NUMBER:=NULL,
                     v066 IN NUMBER:=NULL, v067 IN NUMBER:=NULL,
                     v068 IN NUMBER:=NULL, v069 IN NUMBER:=NULL,
                     v070 IN NUMBER:=NULL, v071 IN NUMBER:=NULL,
                     v072 IN NUMBER:=NULL, v073 IN NUMBER:=NULL,
                     v074 IN NUMBER:=NULL, v075 IN NUMBER:=NULL,
                     v076 IN NUMBER:=NULL, v077 IN NUMBER:=NULL,
                     v078 IN NUMBER:=NULL, v079 IN NUMBER:=NULL,
                     v080 IN NUMBER:=NULL, v081 IN NUMBER:=NULL,
                     v082 IN NUMBER:=NULL, v083 IN NUMBER:=NULL,
                     v084 IN NUMBER:=NULL, v085 IN NUMBER:=NULL,
                     v086 IN NUMBER:=NULL, v087 IN NUMBER:=NULL,
                     v088 IN NUMBER:=NULL, v089 IN NUMBER:=NULL,
                     v090 IN NUMBER:=NULL, v091 IN NUMBER:=NULL,
                     v092 IN NUMBER:=NULL, v093 IN NUMBER:=NULL,
                     v094 IN NUMBER:=NULL, v095 IN NUMBER:=NULL,
                     v096 IN NUMBER:=NULL, v097 IN NUMBER:=NULL,
                     v098 IN NUMBER:=NULL, v099 IN NUMBER:=NULL,
                     v100 IN NUMBER:=NULL, v101 IN NUMBER:=NULL,
                     v102 IN NUMBER:=NULL, v103 IN NUMBER:=NULL,
                     v104 IN NUMBER:=NULL, v105 IN NUMBER:=NULL,
                     v106 IN NUMBER:=NULL, v107 IN NUMBER:=NULL,
                     v108 IN NUMBER:=NULL, v109 IN NUMBER:=NULL,
                     v110 IN NUMBER:=NULL, v111 IN NUMBER:=NULL,
                     v112 IN NUMBER:=NULL, v113 IN NUMBER:=NULL,
                     v114 IN NUMBER:=NULL, v115 IN NUMBER:=NULL,
                     v116 IN NUMBER:=NULL, v117 IN NUMBER:=NULL,
                     v118 IN NUMBER:=NULL, v119 IN NUMBER:=NULL,
                     v120 IN NUMBER:=NULL, v121 IN NUMBER:=NULL,
                     v122 IN NUMBER:=NULL, v123 IN NUMBER:=NULL,
                     v124 IN NUMBER:=NULL, v125 IN NUMBER:=NULL,
                     v126 IN NUMBER:=NULL, v127 IN NUMBER:=NULL,
                     v128 IN NUMBER:=NULL, v129 IN NUMBER:=NULL,
                     v130 IN NUMBER:=NULL, v131 IN NUMBER:=NULL,
                     v132 IN NUMBER:=NULL, v133 IN NUMBER:=NULL,
                     v134 IN NUMBER:=NULL, v135 IN NUMBER:=NULL,
                     v136 IN NUMBER:=NULL, v137 IN NUMBER:=NULL,
                     v138 IN NUMBER:=NULL, v139 IN NUMBER:=NULL,
                     v140 IN NUMBER:=NULL, v141 IN NUMBER:=NULL,
                     v142 IN NUMBER:=NULL, v143 IN NUMBER:=NULL,
                     v144 IN NUMBER:=NULL, v145 IN NUMBER:=NULL,
                     v146 IN NUMBER:=NULL, v147 IN NUMBER:=NULL,
                     v148 IN NUMBER:=NULL, v149 IN NUMBER:=NULL,
                     v150 IN NUMBER:=NULL, v151 IN NUMBER:=NULL,
                     v152 IN NUMBER:=NULL, v153 IN NUMBER:=NULL,
                     v154 IN NUMBER:=NULL, v155 IN NUMBER:=NULL,
                     v156 IN NUMBER:=NULL, v157 IN NUMBER:=NULL,
                     v158 IN NUMBER:=NULL, v159 IN NUMBER:=NULL,
                     v160 IN NUMBER:=NULL, v161 IN NUMBER:=NULL,
                     v162 IN NUMBER:=NULL, v163 IN NUMBER:=NULL,
                     v164 IN NUMBER:=NULL, v165 IN NUMBER:=NULL,
                     v166 IN NUMBER:=NULL, v167 IN NUMBER:=NULL,
                     v168 IN NUMBER:=NULL, v169 IN NUMBER:=NULL,
                     v170 IN NUMBER:=NULL, v171 IN NUMBER:=NULL,
                     v172 IN NUMBER:=NULL, v173 IN NUMBER:=NULL,
                     v174 IN NUMBER:=NULL, v175 IN NUMBER:=NULL,
                     v176 IN NUMBER:=NULL, v177 IN NUMBER:=NULL,
                     v178 IN NUMBER:=NULL, v179 IN NUMBER:=NULL,
                     v180 IN NUMBER:=NULL, v181 IN NUMBER:=NULL,
                     v182 IN NUMBER:=NULL, v183 IN NUMBER:=NULL,
                     v184 IN NUMBER:=NULL, v185 IN NUMBER:=NULL,
                     v186 IN NUMBER:=NULL, v187 IN NUMBER:=NULL,
                     v188 IN NUMBER:=NULL, v189 IN NUMBER:=NULL,
                     v190 IN NUMBER:=NULL, v191 IN NUMBER:=NULL,
                     v192 IN NUMBER:=NULL, v193 IN NUMBER:=NULL,
                     v194 IN NUMBER:=NULL, v195 IN NUMBER:=NULL,
                     v196 IN NUMBER:=NULL, v197 IN NUMBER:=NULL,
                     v198 IN NUMBER:=NULL, v199 IN NUMBER:=NULL,
                     v200 IN NUMBER:=NULL, v201 IN NUMBER:=NULL,
                     v202 IN NUMBER:=NULL, v203 IN NUMBER:=NULL,
                     v204 IN NUMBER:=NULL, v205 IN NUMBER:=NULL,
                     v206 IN NUMBER:=NULL, v207 IN NUMBER:=NULL,
                     v208 IN NUMBER:=NULL, v209 IN NUMBER:=NULL,
                     v210 IN NUMBER:=NULL, v211 IN NUMBER:=NULL,
                     v212 IN NUMBER:=NULL, v213 IN NUMBER:=NULL,
                     v214 IN NUMBER:=NULL, v215 IN NUMBER:=NULL,
                     v216 IN NUMBER:=NULL, v217 IN NUMBER:=NULL,
                     v218 IN NUMBER:=NULL, v219 IN NUMBER:=NULL,
                     v220 IN NUMBER:=NULL, v221 IN NUMBER:=NULL,
                     v222 IN NUMBER:=NULL, v223 IN NUMBER:=NULL,
                     v224 IN NUMBER:=NULL, v225 IN NUMBER:=NULL,
                     v226 IN NUMBER:=NULL, v227 IN NUMBER:=NULL,
                     v228 IN NUMBER:=NULL, v229 IN NUMBER:=NULL,
                     v230 IN NUMBER:=NULL, v231 IN NUMBER:=NULL,
                     v232 IN NUMBER:=NULL, v233 IN NUMBER:=NULL,
                     v234 IN NUMBER:=NULL, v235 IN NUMBER:=NULL,
                     v236 IN NUMBER:=NULL, v237 IN NUMBER:=NULL,
                     v238 IN NUMBER:=NULL, v239 IN NUMBER:=NULL,
                     v240 IN NUMBER:=NULL, v241 IN NUMBER:=NULL,
                     v242 IN NUMBER:=NULL, v243 IN NUMBER:=NULL,
                     v244 IN NUMBER:=NULL, v245 IN NUMBER:=NULL,
                     v246 IN NUMBER:=NULL, v247 IN NUMBER:=NULL,
                     v248 IN NUMBER:=NULL, v249 IN NUMBER:=NULL)
return VARCHAR2;
-- pragma restrict_references(interact, WNDS, WNPS, RNPS);

--
-- NAME:
--	relate - determine the relationship between to geometries
-- DESCRIPTION
--	Relate operates on two geometries and, based on the mask,
--	can determine their relationship.  There are three interfaces to relate.
--	The first operates geometries in a single SDO layer.  The second
--	operates on two geometries in two different layers and the third
--	on its first geometry in an SDO layer and the second geometry described
--	in its arguement list.
--
-- ARGUMENTS
--	LAYER	- geometry table name (LAYER_sdogeom) input argument
--	GID	- Geometry IDentifier input argument
--	MASK	- relationship mask
--		any combionation of
--  		DISJOINT
--  		TOUCH
--  		EQUAL
--  		INSIDE
--  		CONTAINS
--  		COVEREDBY
--  		COVERS
--  		OVERLAPBDYDISJOINT
--  		OVERLAPBDYINTERSECT
--		OR DETERMINE
--		OR ANYINTERACT
--     x_tolerance - allowed variance of ordinate values within dimension1
--     y_tolerance - allowed variance of ordinate values within dimension 2
--     vNNN        - list of up to 250 input values describing the elements
--                   of a geometric object with the following structure:
--                   etype, # of ordinate values, list of ordinate values.
--                   For example the following list could be used to describe
--                   a four point polygon with with a triangular hole :
--                   SDO_GEOM.POLYGON_TYPE, 10, ord1, ord2, ord3, ord4, ord5,
--                   ord6, ord7, ord8, ord9, ord10,
--                   SDO_GEOM.POLYGON_TYPE, 8, ord1, ord2, ord3, ord4, ord5,
--                   ord6, ord7, ord8
--                   The following example could be used to describe a two
--                   element (single segment) linestring:
--                   SDO_GEOM.LINESTRING_TYPE, 4, ord1, ord2, ord3, ord4,
--                   SDO_GEOM.LINESTRING_TYPE, 4, ord1, ord2, ord3, ord4
--
-- RETURNS
--	If any combination of the following mask options is specified as the
--	mask argument, relate returns the relation if it matches passes mask
--	otherwise FALSE.
--
--  	DISJOINT
--  	TOUCH
--  	EQUAL
--  	INSIDE
--  	CONTAINS
--  	COVEREDBY
--  	COVERS
--  	OVERLAPBDYDISJOINT
--  	OVERLAPBDYINTERSECT
--
--	RELATE will return one of the above relations if the following mask is
--	specified:
--
--	DETERMINE
--
--	RELATE will return TRUE or FALSE for if the following mask is specified :
--
--	ANYINTERACT
--

function relate (layer IN VARCHAR2, gid1 IN NUMBER,
                 mask  IN VARCHAR2,
                 gid2  IN NUMBER )
return VARCHAR2 ;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);


function relate (layer1 IN VARCHAR2, gid1  IN  NUMBER,
                 mask   IN VARCHAR2,
                 layer2 IN  VARCHAR2, gid2 IN NUMBER )
return VARCHAR2 ;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);

function relate (layer1 IN VARCHAR2, gid1  IN  NUMBER, nord1 IN NUMBER,
                 mask   IN VARCHAR2,
                 layer2 IN  VARCHAR2, gid2 IN NUMBER, nord2 IN NUMBER,
                 tolerance1 IN NUMBER, tolerance2 IN NUMBER)
return VARCHAR2 ;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);

function relate (layer1 IN VARCHAR2, gid1  IN NUMBER,
                mask   IN VARCHAR2,
                x_tolerance IN NUMBER, y_tolerance IN NUMBER,
                     v000 IN NUMBER:=NULL, v001 IN NUMBER:=NULL,
                     v002 IN NUMBER:=NULL, v003 IN NUMBER:=NULL,
                     v004 IN NUMBER:=NULL, v005 IN NUMBER:=NULL,
                     v006 IN NUMBER:=NULL, v007 IN NUMBER:=NULL,
                     v008 IN NUMBER:=NULL, v009 IN NUMBER:=NULL,
                     v010 IN NUMBER:=NULL, v011 IN NUMBER:=NULL,
                     v012 IN NUMBER:=NULL, v013 IN NUMBER:=NULL,
                     v014 IN NUMBER:=NULL, v015 IN NUMBER:=NULL,
                     v016 IN NUMBER:=NULL, v017 IN NUMBER:=NULL,
                     v018 IN NUMBER:=NULL, v019 IN NUMBER:=NULL,
                     v020 IN NUMBER:=NULL, v021 IN NUMBER:=NULL,
                     v022 IN NUMBER:=NULL, v023 IN NUMBER:=NULL,
                     v024 IN NUMBER:=NULL, v025 IN NUMBER:=NULL,
                     v026 IN NUMBER:=NULL, v027 IN NUMBER:=NULL,
                     v028 IN NUMBER:=NULL, v029 IN NUMBER:=NULL,
                     v030 IN NUMBER:=NULL, v031 IN NUMBER:=NULL,
                     v032 IN NUMBER:=NULL, v033 IN NUMBER:=NULL,
                     v034 IN NUMBER:=NULL, v035 IN NUMBER:=NULL,
                     v036 IN NUMBER:=NULL, v037 IN NUMBER:=NULL,
                     v038 IN NUMBER:=NULL, v039 IN NUMBER:=NULL,
                     v040 IN NUMBER:=NULL, v041 IN NUMBER:=NULL,
                     v042 IN NUMBER:=NULL, v043 IN NUMBER:=NULL,
                     v044 IN NUMBER:=NULL, v045 IN NUMBER:=NULL,
                     v046 IN NUMBER:=NULL, v047 IN NUMBER:=NULL,
                     v048 IN NUMBER:=NULL, v049 IN NUMBER:=NULL,
                     v050 IN NUMBER:=NULL, v051 IN NUMBER:=NULL,
                     v052 IN NUMBER:=NULL, v053 IN NUMBER:=NULL,
                     v054 IN NUMBER:=NULL, v055 IN NUMBER:=NULL,
                     v056 IN NUMBER:=NULL, v057 IN NUMBER:=NULL,
                     v058 IN NUMBER:=NULL, v059 IN NUMBER:=NULL,
                     v060 IN NUMBER:=NULL, v061 IN NUMBER:=NULL,
                     v062 IN NUMBER:=NULL, v063 IN NUMBER:=NULL,
                     v064 IN NUMBER:=NULL, v065 IN NUMBER:=NULL,
                     v066 IN NUMBER:=NULL, v067 IN NUMBER:=NULL,
                     v068 IN NUMBER:=NULL, v069 IN NUMBER:=NULL,
                     v070 IN NUMBER:=NULL, v071 IN NUMBER:=NULL,
                     v072 IN NUMBER:=NULL, v073 IN NUMBER:=NULL,
                     v074 IN NUMBER:=NULL, v075 IN NUMBER:=NULL,
                     v076 IN NUMBER:=NULL, v077 IN NUMBER:=NULL,
                     v078 IN NUMBER:=NULL, v079 IN NUMBER:=NULL,
                     v080 IN NUMBER:=NULL, v081 IN NUMBER:=NULL,
                     v082 IN NUMBER:=NULL, v083 IN NUMBER:=NULL,
                     v084 IN NUMBER:=NULL, v085 IN NUMBER:=NULL,
                     v086 IN NUMBER:=NULL, v087 IN NUMBER:=NULL,
                     v088 IN NUMBER:=NULL, v089 IN NUMBER:=NULL,
                     v090 IN NUMBER:=NULL, v091 IN NUMBER:=NULL,
                     v092 IN NUMBER:=NULL, v093 IN NUMBER:=NULL,
                     v094 IN NUMBER:=NULL, v095 IN NUMBER:=NULL,
                     v096 IN NUMBER:=NULL, v097 IN NUMBER:=NULL,
                     v098 IN NUMBER:=NULL, v099 IN NUMBER:=NULL,
                     v100 IN NUMBER:=NULL, v101 IN NUMBER:=NULL,
                     v102 IN NUMBER:=NULL, v103 IN NUMBER:=NULL,
                     v104 IN NUMBER:=NULL, v105 IN NUMBER:=NULL,
                     v106 IN NUMBER:=NULL, v107 IN NUMBER:=NULL,
                     v108 IN NUMBER:=NULL, v109 IN NUMBER:=NULL,
                     v110 IN NUMBER:=NULL, v111 IN NUMBER:=NULL,
                     v112 IN NUMBER:=NULL, v113 IN NUMBER:=NULL,
                     v114 IN NUMBER:=NULL, v115 IN NUMBER:=NULL,
                     v116 IN NUMBER:=NULL, v117 IN NUMBER:=NULL,
                     v118 IN NUMBER:=NULL, v119 IN NUMBER:=NULL,
                     v120 IN NUMBER:=NULL, v121 IN NUMBER:=NULL,
                     v122 IN NUMBER:=NULL, v123 IN NUMBER:=NULL,
                     v124 IN NUMBER:=NULL, v125 IN NUMBER:=NULL,
                     v126 IN NUMBER:=NULL, v127 IN NUMBER:=NULL,
                     v128 IN NUMBER:=NULL, v129 IN NUMBER:=NULL,
                     v130 IN NUMBER:=NULL, v131 IN NUMBER:=NULL,
                     v132 IN NUMBER:=NULL, v133 IN NUMBER:=NULL,
                     v134 IN NUMBER:=NULL, v135 IN NUMBER:=NULL,
                     v136 IN NUMBER:=NULL, v137 IN NUMBER:=NULL,
                     v138 IN NUMBER:=NULL, v139 IN NUMBER:=NULL,
                     v140 IN NUMBER:=NULL, v141 IN NUMBER:=NULL,
                     v142 IN NUMBER:=NULL, v143 IN NUMBER:=NULL,
                     v144 IN NUMBER:=NULL, v145 IN NUMBER:=NULL,
                     v146 IN NUMBER:=NULL, v147 IN NUMBER:=NULL,
                     v148 IN NUMBER:=NULL, v149 IN NUMBER:=NULL,
                     v150 IN NUMBER:=NULL, v151 IN NUMBER:=NULL,
                     v152 IN NUMBER:=NULL, v153 IN NUMBER:=NULL,
                     v154 IN NUMBER:=NULL, v155 IN NUMBER:=NULL,
                     v156 IN NUMBER:=NULL, v157 IN NUMBER:=NULL,
                     v158 IN NUMBER:=NULL, v159 IN NUMBER:=NULL,
                     v160 IN NUMBER:=NULL, v161 IN NUMBER:=NULL,
                     v162 IN NUMBER:=NULL, v163 IN NUMBER:=NULL,
                     v164 IN NUMBER:=NULL, v165 IN NUMBER:=NULL,
                     v166 IN NUMBER:=NULL, v167 IN NUMBER:=NULL,
                     v168 IN NUMBER:=NULL, v169 IN NUMBER:=NULL,
                     v170 IN NUMBER:=NULL, v171 IN NUMBER:=NULL,
                     v172 IN NUMBER:=NULL, v173 IN NUMBER:=NULL,
                     v174 IN NUMBER:=NULL, v175 IN NUMBER:=NULL,
                     v176 IN NUMBER:=NULL, v177 IN NUMBER:=NULL,
                     v178 IN NUMBER:=NULL, v179 IN NUMBER:=NULL,
                     v180 IN NUMBER:=NULL, v181 IN NUMBER:=NULL,
                     v182 IN NUMBER:=NULL, v183 IN NUMBER:=NULL,
                     v184 IN NUMBER:=NULL, v185 IN NUMBER:=NULL,
                     v186 IN NUMBER:=NULL, v187 IN NUMBER:=NULL,
                     v188 IN NUMBER:=NULL, v189 IN NUMBER:=NULL,
                     v190 IN NUMBER:=NULL, v191 IN NUMBER:=NULL,
                     v192 IN NUMBER:=NULL, v193 IN NUMBER:=NULL,
                     v194 IN NUMBER:=NULL, v195 IN NUMBER:=NULL,
                     v196 IN NUMBER:=NULL, v197 IN NUMBER:=NULL,
                     v198 IN NUMBER:=NULL, v199 IN NUMBER:=NULL,
                     v200 IN NUMBER:=NULL, v201 IN NUMBER:=NULL,
                     v202 IN NUMBER:=NULL, v203 IN NUMBER:=NULL,
                     v204 IN NUMBER:=NULL, v205 IN NUMBER:=NULL,
                     v206 IN NUMBER:=NULL, v207 IN NUMBER:=NULL,
                     v208 IN NUMBER:=NULL, v209 IN NUMBER:=NULL,
                     v210 IN NUMBER:=NULL, v211 IN NUMBER:=NULL,
                     v212 IN NUMBER:=NULL, v213 IN NUMBER:=NULL,
                     v214 IN NUMBER:=NULL, v215 IN NUMBER:=NULL,
                     v216 IN NUMBER:=NULL, v217 IN NUMBER:=NULL,
                     v218 IN NUMBER:=NULL, v219 IN NUMBER:=NULL,
                     v220 IN NUMBER:=NULL, v221 IN NUMBER:=NULL,
                     v222 IN NUMBER:=NULL, v223 IN NUMBER:=NULL,
                     v224 IN NUMBER:=NULL, v225 IN NUMBER:=NULL,
                     v226 IN NUMBER:=NULL, v227 IN NUMBER:=NULL,
                     v228 IN NUMBER:=NULL, v229 IN NUMBER:=NULL,
                     v230 IN NUMBER:=NULL, v231 IN NUMBER:=NULL,
                     v232 IN NUMBER:=NULL, v233 IN NUMBER:=NULL,
                     v234 IN NUMBER:=NULL, v235 IN NUMBER:=NULL,
                     v236 IN NUMBER:=NULL, v237 IN NUMBER:=NULL,
                     v238 IN NUMBER:=NULL, v239 IN NUMBER:=NULL,
                     v240 IN NUMBER:=NULL, v241 IN NUMBER:=NULL,
                     v242 IN NUMBER:=NULL, v243 IN NUMBER:=NULL,
                     v244 IN NUMBER:=NULL, v245 IN NUMBER:=NULL,
                     v246 IN NUMBER:=NULL, v247 IN NUMBER:=NULL,
                     v248 IN NUMBER:=NULL, v249 IN NUMBER:=NULL )
return VARCHAR2 ;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);


------------------------------------------------------------------------------
-- PART II. Functions and procedures for object model                       --
------------------------------------------------------------------------------

--
-- NAME:
--	relate - object relate function
-- DESCRIPTION
--	This function operates on two geometries and, based on the mask,
--      can determine their relationship.
--
-- ARGUMENTS
--      geom1     - the first geometry object.
--      dim1      - the dimensional information array corresponding to the
--                  first geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      mask      - relationship mask, any combionation of:
--  	            DISJOINT, TOUCH, EQUAL, INSIDE, CONTAINS, COVEREDBY,
--  	            COVERS, OVERLAPBDYDISJOINT, OVERLAPBDYINTERSECT,
--		    OR DETERMINE, OR ANYINTERACT
--      geom2     - the second geometry object.
--      dim2      - the dimensional information array corresponding to the
--                  second geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      tol       - tolerance value.
--
-- RETURNS
--	If any combination of the following mask options is specified as the
--	mask argument, relate returns the relation if it matches passes mask
--	otherwise 'FALSE':
--  	DISJOINT, TOUCH, EQUAL, INSIDE, CONTAINS, COVEREDBY, COVERS,
--  	OVERLAPBDYDISJOINT, OVERLAPBDYINTERSECT.
--
--	Relate will return one of the above relations if the following mask is
--	specified: 'DETERMINE'.
--
--	Relate will return 'TRUE' or 'FALSE' for if the following mask is
--      specified: 'ANYINTERACT'.
--

function relate(geom1 IN MDSYS.SDO_GEOMETRY,
                dim1  IN MDSYS.SDO_DIM_ARRAY,
                mask  IN VARCHAR2,
                geom2 IN MDSYS.SDO_GEOMETRY,
                dim2  IN MDSYS.SDO_DIM_ARRAY)
return VARCHAR2 PARALLEL_ENABLE;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);


function relate(geom1 IN MDSYS.SDO_GEOMETRY,
                mask  IN VARCHAR2,
                geom2 IN MDSYS.SDO_GEOMETRY,
                tol   IN NUMBER)
return VARCHAR2 PARALLEL_ENABLE;
-- pragma restrict_references(relate, WNDS, WNPS, RNPS);


--
-- NAME:
--	within_distance - object within distance function
-- DESCRIPTION
--      This function operates on two geometries and determines if they
--      are within the distance of <dist>.
--
-- ARGUMENTS
--      geom1     - the first geometry object.
--      dim1      - the dimensional information array corresponding to the
--                  first geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      dist      - Euclidean distance value.
--      geom2     - the second geometry object.
--      dim2      - the dimensional information array corresponding to the
--                  second geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      tol       - tolerance value.
--
-- RETURNS
--      'TRUE' if the two geometries are within distance of <dist> from
--      each other, or 'FALSE' otherwise.
--

function within_distance(geom1 IN MDSYS.SDO_GEOMETRY,
                         dim1  IN MDSYS.SDO_DIM_ARRAY,
                         dist  IN NUMBER,
                         geom2 IN MDSYS.SDO_GEOMETRY,
                         dim2  IN MDSYS.SDO_DIM_ARRAY,
                         units IN VARCHAR2:=NULL)
return VARCHAR2 PARALLEL_ENABLE;
 -- pragma restrict_references(within_distance, WNDS, WNPS, RNPS);


function within_distance(geom1 IN MDSYS.SDO_GEOMETRY,
                         dist  IN NUMBER,
                         geom2 IN MDSYS.SDO_GEOMETRY,
                         tol   IN NUMBER,
                         units IN VARCHAR2:=NULL)
return VARCHAR2 PARALLEL_ENABLE;
 -- pragma restrict_references(within_distance, WNDS, WNPS, RNPS);


--
-- NAME:
--      sdo_buffer -- buffer generation
-- DESCRIPTION
--      This function generates a buffer polygon for the input geometry
--      object with buffer distance <dist>.
--
-- ARGUMENTS
--      geom      - the input geometry object.
--      dim       - the dimensional information array corresponding to the
--                  input geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      dist      - Euclidean distance value.
--      params    - Unit of the above dist; arc_tolerance for geodetic buffering
--      tol       - tolerance value.
--
-- RETURNS
--      This function returns a geometry object representing the buffer
--      polygon.
--

function sdo_buffer(geom IN MDSYS.SDO_GEOMETRY,
                    dim  IN MDSYS.SDO_DIM_ARRAY,
                    dist IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_buffer, WNDS, WNPS, TRUST);


function sdo_buffer(geom IN MDSYS.SDO_GEOMETRY,
                    dist IN NUMBER,
                    tol  IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_buffer, WNDS, WNPS, TRUST);

function sdo_buffer(geom IN MDSYS.SDO_GEOMETRY,
                    dim  IN MDSYS.SDO_DIM_ARRAY,
                    dist IN NUMBER,
                    params IN VARCHAR2)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_buffer, WNDS, WNPS, TRUST);

function sdo_buffer(geom IN MDSYS.SDO_GEOMETRY,
                    dist IN NUMBER,
                    tol  IN NUMBER,
                    params IN VARCHAR2)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_buffer, WNDS, WNPS, TRUST);


-- Geometry operations
--
-- NAME:
--      sdo_union        -- union ('OR' operation) of two geometry objects.
--      sdo_difference   -- difference ('MINUS' operation) of two geometry
--                          objects.
--      sdo_xor          -- symetric difference('XOR' operation) of two
--                          geometry objects.
--      sdo_intersection -- intersection ('AND' operation) of two geometry
--                          objects.
--
-- DESCRIPTION
--      These four functions perform topological operations on two geometry
--      objects.
--
-- ARGUMENTS
--      geom1     - the first geometry object.
--      dim1      - the dimensional information array corresponding to the
--                  first geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      geom2     - the second geometry object.
--      dim2      - the dimensional information array corresponding to the
--                  second geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      tol       - tolerance value.
--
-- RETURNS
--      A geometry object representing the result of this geometry operation.
--

function sdo_union(geom1 IN MDSYS.SDO_GEOMETRY,
                   dim1  IN MDSYS.SDO_DIM_ARRAY,
                   geom2 IN MDSYS.SDO_GEOMETRY,
                   dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_union, WNDS, WNPS, RNPS);

function sdo_difference(geom1 IN MDSYS.SDO_GEOMETRY,
                        dim1  IN MDSYS.SDO_DIM_ARRAY,
                        geom2 IN MDSYS.SDO_GEOMETRY,
                        dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_difference, WNDS, WNPS, RNPS);

function sdo_xor(geom1 IN MDSYS.SDO_GEOMETRY,
                 dim1  IN MDSYS.SDO_DIM_ARRAY,
                 geom2 IN MDSYS.SDO_GEOMETRY,
                 dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_xor, WNDS, WNPS, RNPS);

function sdo_intersection(geom1 IN MDSYS.SDO_GEOMETRY,
                          dim1  IN MDSYS.SDO_DIM_ARRAY,
                          geom2 IN MDSYS.SDO_GEOMETRY,
                          dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_intersection, WNDS, WNPS, RNPS);


function sdo_union(geom1 IN MDSYS.SDO_GEOMETRY,
                   geom2 IN MDSYS.SDO_GEOMETRY,
                   tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_union, WNDS, WNPS, RNPS);

function sdo_difference(geom1 IN MDSYS.SDO_GEOMETRY,
                        geom2 IN MDSYS.SDO_GEOMETRY,
                        tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_difference, WNDS, WNPS, RNPS);

function sdo_xor(geom1 IN MDSYS.SDO_GEOMETRY,
                 geom2 IN MDSYS.SDO_GEOMETRY,
                 tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_xor, WNDS, WNPS, RNPS);

function sdo_intersection(geom1 IN MDSYS.SDO_GEOMETRY,
                          geom2 IN MDSYS.SDO_GEOMETRY,
                          tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_intersection, WNDS, WNPS, RNPS);


-- Deprecated polygon operation interfaces
--
-- NAME:
--      sdo_poly_union        -- union ('OR' operation) of two polygons
--      sdo_poly_difference   -- difference ('MINUS' operation) of two polygons
--      sdo_poly_xor          -- symetric difference('XOR' operation) of two
--                               polygons
--      sdo_poly_intersection -- intersection ('AND' operation) of two polygons
--
-- DESCRIPTION
--      These four functions are deprecated starting from version 8.1.6.
--      Please use geometry operations sdo_union, sdo_difference,
--      sdo_xor, and sdo_intersection instead of the above polygon specific
--      functions.
--

function sdo_poly_union(geom1 IN MDSYS.SDO_GEOMETRY,
                        dim1  IN MDSYS.SDO_DIM_ARRAY,
                        geom2 IN MDSYS.SDO_GEOMETRY,
                        dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC;
-- pragma restrict_references(sdo_poly_union, WNDS, WNPS, RNPS);

function sdo_poly_difference(geom1 IN MDSYS.SDO_GEOMETRY,
                             dim1  IN MDSYS.SDO_DIM_ARRAY,
                             geom2 IN MDSYS.SDO_GEOMETRY,
                             dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC;
-- pragma restrict_references(sdo_poly_difference, WNDS, WNPS, RNPS);

function sdo_poly_xor(geom1 IN MDSYS.SDO_GEOMETRY,
                      dim1  IN MDSYS.SDO_DIM_ARRAY,
                      geom2 IN MDSYS.SDO_GEOMETRY,
                      dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC;
-- pragma restrict_references(sdo_poly_xor, WNDS, WNPS, RNPS);

function sdo_poly_intersection(geom1 IN MDSYS.SDO_GEOMETRY,
                               dim1  IN MDSYS.SDO_DIM_ARRAY,
                               geom2 IN MDSYS.SDO_GEOMETRY,
                               dim2  IN MDSYS.SDO_DIM_ARRAY)
return MDSYS.SDO_GEOMETRY DETERMINISTIC;
-- pragma restrict_references(sdo_poly_intersection, WNDS, WNPS, RNPS);


--
-- NAME:
--      sdo_length -- compute length for one-dimensional geometry
--                    (linestring), or perimeter for two-dimensional
--                    geometry (polygon)
-- DESCRIPTION
--      If the input polygon contains one or more holes, this function
--      calculates perimeters for the exterior boundary and all the holes,
--      and then returns the sum of all the perimeters.
--
-- ARGUMENTS
--      geom      - the input geometry object.
--      dim       - the dimensional information array corresponding to the
--                  input geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      tol       - tolerance value.
--      count_shared_edges - Default 1(counting shared edges once) or
--                                   2(counting shared edges twice)
--
-- RETURNS
--      This function returns length or perimeter of a geometry object.
--

function sdo_length(geom IN MDSYS.SDO_GEOMETRY,
                    dim  IN MDSYS.SDO_DIM_ARRAY,
                    count_shared_edges IN NUMBER DEFAULT NULL)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_length, WNDS, WNPS, RNPS);


function sdo_length(geom IN MDSYS.SDO_GEOMETRY,
                    tol  IN NUMBER,
                    count_shared_edges IN NUMBER DEFAULT NULL)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_length, WNDS, WNPS, RNPS);


function sdo_length(geom IN MDSYS.SDO_GEOMETRY,
                    dim  IN MDSYS.SDO_DIM_ARRAY,
                    unit IN VARCHAR2,
                    count_shared_edges IN NUMBER DEFAULT NULL)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_length, WNDS, WNPS, RNPS);


function sdo_length(geom IN MDSYS.SDO_GEOMETRY,
                    tol  IN NUMBER,
                    unit IN VARCHAR2,
                    count_shared_edges IN NUMBER DEFAULT NULL)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_length, WNDS, WNPS, RNPS);


--
-- NAME:
--      sdo_area -- compute area for two-dimensional geometry (polygon)
-- DESCRIPTION
--      This function returns correct area for polygon-with-holes.
--
-- ARGUMENTS
--      geom      - the input geometry object.
--      dim       - the dimensional information array corresponding to the
--                  input geometry object, usually selected from
--                  SDO_GEOM_METADATA table.
--      tol       - tolerance value.
--
-- RETURNS
--      This function returns area of a geometry object.
--

function sdo_area(geom IN MDSYS.SDO_GEOMETRY,
                  dim  IN MDSYS.SDO_DIM_ARRAY)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_area, WNDS, WNPS, RNPS);


function sdo_area(geom IN MDSYS.SDO_GEOMETRY,
                  tol  IN NUMBER)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_area, WNDS, WNPS, RNPS);


function sdo_area(geom IN MDSYS.SDO_GEOMETRY,
                  dim  IN MDSYS.SDO_DIM_ARRAY,
                  unit IN VARCHAR2)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_area, WNDS, WNPS, RNPS);

function sdo_area(geom IN MDSYS.SDO_GEOMETRY,
                  tol  IN NUMBER,
                  unit IN VARCHAR2)
return NUMBER PARALLEL_ENABLE;
-- pragma restrict_references(sdo_area, WNDS, WNPS, RNPS);

function sdo_volume(geom IN MDSYS.SDO_GEOMETRY,
                  tol  IN NUMBER,
                  unit IN VARCHAR2 DEFAULT NULL)
return NUMBER PARALLEL_ENABLE;

--
-- NAME:
--         sdo_distance
--
-- DESCRIPTION
--         Compute the distance between two geometries
--
-- ARGUMENTS
--        geom1    - the first geometry object
--        dim1     - the dimensional information array corresponding to the
--                   first geometry object, usually selected from
--                   SDO_GEOM_METADATA table.
--        geom2    - the second geometry object.
--        dim2     - the dimensional information array corresponding to the
--                   second geometry object, usually selected from
--                   SDO_GEOM_METADATA table.
--        tol      - tolerance value.
--        unit     - the unit for the returned distance
--
-- RETURNS
--        the minimum distance between the two given geometries.
--
function   sdo_distance(geom1 IN mdsys.sdo_geometry,
                      dim1  IN mdsys.sdo_dim_array,
                      geom2 IN mdsys.sdo_geometry,
                      dim2  IN mdsys.sdo_dim_array)
return NUMBER PARALLEL_ENABLE;
--pragma restrict_references(sdo_distance, WNDS, WNPS, RNPS);


function   sdo_distance(geom1 IN mdsys.sdo_geometry,
                        geom2 IN mdsys.sdo_geometry,
                        tol   IN NUMBER)
return NUMBER PARALLEL_ENABLE;
--pragma restrict_references(sdo_distance, WNDS, WNPS, RNPS);

function   sdo_distance(geom1 IN mdsys.sdo_geometry,
                      dim1  IN mdsys.sdo_dim_array,
                      geom2 IN mdsys.sdo_geometry,
                      dim2  IN mdsys.sdo_dim_array,
                      unit  IN varchar2)
return NUMBER PARALLEL_ENABLE;
--pragma restrict_references(sdo_distance, WNDS, WNPS, RNPS);


function   sdo_distance(geom1 IN mdsys.sdo_geometry,
                        geom2 IN mdsys.sdo_geometry,
                        tol   IN NUMBER,
                        unit  IN varchar2)
return NUMBER PARALLEL_ENABLE;
--pragma restrict_references(sdo_distance, WNDS, WNPS, RNPS);


--
-- NAME:
--
--      sdo_closest_points
-- DESCRIPTION
--      computes the minimum inter-geometry distance and the
--      coordinates of closest point pair
--
-- ARGUMENTS
--        geom1 - the first geometry
--        geom2 - the second geometry
--        tolerance - tolerance which is a number
--        unit  - depends on the coordinate system
-- RETURNS
--       -9999 - if any portion of either geometry has arc(s);
--       dist  - the distance - otherwise
--       geoma - the coordinates of first point of closest approach as a geometry object
--       geomb - the coordinates of second point of closest approach as a geometry object
--
-- Note that: dim1 and dim2 - dimensions of geom1 and geom2 respectively are computed internally.

procedure sdo_closest_points(geom1 IN mdsys.sdo_geometry,
                             geom2 IN mdsys.sdo_geometry,
                             tolerance IN NUMBER,
                             unit      IN VARCHAR2,
                             dist      OUT NUMBER,
                             geoma     OUT mdsys.sdo_geometry,
                             geomb     OUT mdsys.sdo_geometry);
--pragma restrict_references(sdo_closest_points, WNDS, WNPS, RNPS);


--
-- NAME:
--
--	sdo_convexhull
-- DESCRIPTION
--	calculates the convex hull of the given geometry.
--
--      If the geometry is of point type, or has less than three
--	points in total this function will return null.
--
-- ARGUMENTS
--        geom1    - the geometry object
--        dim1     - the dimensional information array corresponding to the
--                   geometry object, usually selected from
--                   SDO_GEOM_METADATA table.
--        tol      - tolerance value.
-- RETURNS
--      A geometry object representing the result of polygon operation.
--

function   sdo_convexhull(geom1 IN mdsys.sdo_geometry,
			  dim1  IN mdsys.sdo_dim_array)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_convexhull, WNDS, WNPS, RNPS);

function   sdo_convexhull(geom1 IN mdsys.sdo_geometry,
			  tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_convexhull, WNDS, WNPS, RNPS);

function   sdo_triangulate(geom IN mdsys.sdo_geometry,
			  tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_triangulate, WNDS, WNPS, RNPS);

-- computes the concavehull of the geometry, assuming
-- that points are likely on the boundary,
-- length: to stop, when the longest edge in the concave hull
-- is shorter than it.
function   sdo_concavehull_boundary(geom       IN mdsys.sdo_geometry,
			            tol        IN NUMBER,
                                    length     IN NUMBER DEFAULT NULL)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_concavehull_boundary, WNDS, WNPS, RNPS);

-- computes the concavehull of the geometry, assuming
-- that points are not only on the boundary, but also inside.
-- raius : output, the radius of stopping circle
function   sdo_concavehull(geom       IN mdsys.sdo_geometry,
                           tol        IN NUMBER,
                           radius    OUT NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_concavehull, WNDS, WNPS, RNPS);

-- computes the concavehull of the geometry, assuming
-- that points are not only on the boundary, but also inside
function   sdo_concavehull(geom       IN mdsys.sdo_geometry,
                           tol        IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_concavehull, WNDS, WNPS, RNPS);

-- computes the alpha shape of the geometry
-- radius: alpha value = 1/radius
-- flag: 0 -> include points and edges,
--       1 -> doesn't include points and edges
function sdo_alpha_shape(geom       IN mdsys.sdo_geometry,
                         tol        IN NUMBER,
                         radius     IN NUMBER DEFAULT NULL,
                         flag       IN BINARY_INTEGER DEFAULT 0)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;


--
-- NAME:
--
--	sdo_centroid
-- DESCRIPTION
--	calculates the centroid of the given geometry.
--
--      The geometry must be of Polygon type; otherwise
--	this function will return null (even for
--      multi-polygon).
--
-- ARGUMENTS
--        geom1    - the polygon-typed geometry object
--        dim1     - the dimensional information array corresponding to the
--                   geometry object, usually selected from
--                   USER_SDO_GEOM_METADATA table.
--        tol      - tolerance value.
-- RETURNS
--      A point geometry object representing the centroid.
--

function   sdo_centroid(geom1 IN mdsys.sdo_geometry,
	                dim1  IN mdsys.sdo_dim_array)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- pragma restrict_references(sdo_centroid, WNDS, WNPS, RNPS);

function   sdo_centroid(geom1 IN mdsys.sdo_geometry,
                        tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
--pragma restrict_references(sdo_centroid, WNDS, WNPS, RNPS);

--
-- NAME:
--
-- SDO_MBR
-- DESCRIPTION
-- Returns the MBR of the geometry object. The result will be
-- of type x003 with (1003,3,3) element. That is a rectangle
-- object in 2 dimensions.
--

function sdo_mbr(geom IN mdsys.sdo_geometry,
                        dim IN mdsys.sdo_dim_array)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_mbr, WNDS, WNPS, RNPS);

function sdo_mbr(geom IN mdsys.sdo_geometry)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_mbr, WNDS, WNPS, RNPS);

function sdo_g3dconv(geom IN mdsys.sdo_geometry,
                     dim IN mdsys.sdo_dim_array)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
--pragma restrict_references(sdo_g3dconv, WNDS, WNPS, RNPS);

function sdo_g3dconv(geom IN mdsys.sdo_geometry)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
--pragma restrict_references(sdo_g3dconv, WNDS, WNPS, RNPS);


function sdo_max_mbr_ordinate(geom IN mdsys.sdo_geometry,
                              dim IN mdsys.sdo_dim_array,
			      ordinate_pos IN NUMBER)
return NUMBER PARALLEL_ENABLE;
pragma restrict_references(sdo_max_mbr_ordinate, WNDS, WNPS, RNPS);

function sdo_max_mbr_ordinate(geom IN mdsys.sdo_geometry,
			      ordinate_pos IN NUMBER)
return NUMBER PARALLEL_ENABLE;
pragma restrict_references(sdo_max_mbr_ordinate, WNDS, WNPS, RNPS);


function sdo_min_mbr_ordinate(geom IN mdsys.sdo_geometry,
                              dim IN mdsys.sdo_dim_array,
			      ordinate_pos IN NUMBER)
return NUMBER PARALLEL_ENABLE;
pragma restrict_references(sdo_min_mbr_ordinate, WNDS, WNPS, RNPS);

function sdo_min_mbr_ordinate(geom IN mdsys.sdo_geometry,
			      ordinate_pos IN NUMBER)
return NUMBER PARALLEL_ENABLE;
pragma restrict_references(sdo_min_mbr_ordinate, WNDS, WNPS, RNPS);



--
-- NAME:
--
--	sdo_pointonsurface
-- DESCRIPTION
--	Returns a point that is guaranteed to be on the surface of
--      the given geometry (polygon type).
--
--      Currently supports only geometries of Polygon type.
--
-- ARGUMENTS
--        geom1    - the polygon geometry object
--        dim1     - the dimensional information array corresponding to the
--                   geometry object, usually selected from
--                   USER_SDO_GEOM_METADATA table.
--        tol      - tolerance value.
-- RETURNS
--      A point geometry object representing the point
--

function   sdo_pointonsurface(geom1 IN mdsys.sdo_geometry,
  			      dim1  IN mdsys.sdo_dim_array)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_pointonsurface, WNDS, WNPS, RNPS);

function   sdo_pointonsurface(geom1 IN mdsys.sdo_geometry,
  			      tol   IN NUMBER)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_pointonsurface, WNDS, WNPS, RNPS);


--
-- NAME:
--	validate_geometry - validate a geometrie object
--	validate_layer    - validate all geometry objects in a column
-- DESCRIPTION
--	Validate geometry based on OGIS rules. Only check known SDO
--      data types. Elements of unknown type are considered valid.
--
--	If a geometry contains multiple polygon rings, validate_geometry
--      andvalidate_layer only check each ring against itself. No ring-ring
--      relationship is examined at this time.
--
-- ARGUMENTS
--	theGeometry	- the geometry object to be validated
--      theDimInfo      - the dimension information corresponding to this
--                        geometry object, usually selected from
--                        SDO_GEOM_METADATA table.
--      tolerance       - tolerance value.
--	geom_table	- geometry object table name
--	geom_column	- geometry object column name
--      pkey_column     - primary key column name
--      result_table    - result table name
--
-- RETURNS
--      'TRUE' if theGeometry is valid. Otherwise it returns Oracle
--      standard error code for known error or 'FALSE' for unknown error.
--      validate_layer procedure populates the given result_table with
--      validation results.
--

function validate_geometry(theGeometry IN MDSYS.SDO_GEOMETRY,
                           theDimInfo  IN MDSYS.SDO_DIM_ARRAY)
return VARCHAR2 PARALLEL_ENABLE;
pragma restrict_references(validate_geometry, WNDS, WNPS, RNPS);

function validate_geometry(theGeometry IN MDSYS.SDO_GEOMETRY,
                           tolerance   IN NUMBER)
return VARCHAR2 PARALLEL_ENABLE;
pragma restrict_references(validate_geometry, WNDS, WNPS, RNPS);

function validate_geometry_with_context(theGeometry IN MDSYS.SDO_GEOMETRY,
                                      theDimInfo  IN MDSYS.SDO_DIM_ARRAY,
                                      conditional IN VARCHAR2 := 'TRUE',
                                      flag10g     IN VARCHAR2 := 'FALSE')
return VARCHAR2 PARALLEL_ENABLE;
pragma restrict_references(validate_geometry, WNDS, WNPS, RNPS);

function validate_geometry_with_context(theGeometry IN MDSYS.SDO_GEOMETRY,
                                        tolerance   IN NUMBER,
                                        conditional IN VARCHAR2 := 'TRUE',
                                        flag10g     IN VARCHAR2 := 'FALSE')
return VARCHAR2 PARALLEL_ENABLE;
pragma restrict_references(validate_geometry, WNDS, WNPS, RNPS);

procedure validate_layer(geom_table  IN VARCHAR2,
                         geom_column IN VARCHAR2,
                         pkey_column IN VARCHAR2,
                         result_table IN VARCHAR2,
                         commit_interval IN NUMBER := -1);

procedure validate_layer_with_context(geom_table  IN VARCHAR2,
                         geom_column IN VARCHAR2,
                         result_table IN VARCHAR2,
                         commit_interval IN NUMBER := -1,
                         conditional IN VARCHAR2 := 'TRUE',
                         flag10g     IN VARCHAR2 := 'FALSE');

--
-- NAME:
--
--	sdo_arc_densify
-- DESCRIPTION
--      If the geometry contains arcs, this function will densify/segment
--      all acrs in the geometry.
--
-- ARGUMENTS
--        geom    - the geometry object
--        dim     - the dimensional information array corresponding to the
--                   geometry object, usually selected from
--                   SDO_GEOM_METADATA table.
--        tol      - tolerance value.
-- RETURNS
--      A geometry object representing the result of polygon operation.
--

function   sdo_arc_densify(geom IN mdsys.sdo_geometry,
			   dim  IN mdsys.sdo_dim_array,
                           params IN VARCHAR2)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_arc_densify, WNDS, WNPS, RNPS);

function   sdo_arc_densify(geom IN mdsys.sdo_geometry,
			   tol  IN NUMBER,
                           params IN VARCHAR2)
return MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(sdo_arc_densify, WNDS, WNPS, RNPS);

function get_dim_array(geom IN MDSYS.SDO_GEOMETRY, tol IN NUMBER)
return MDSYS.SDO_DIM_ARRAY DETERMINISTIC PARALLEL_ENABLE;
pragma restrict_references(get_dim_array, WNDS, WNPS, RNPS);

FUNCTION sdo_insert_log_filter(ind_schema   IN VARCHAR2,
                               ind_name     IN VARCHAR2,
                               indpart_name IN VARCHAR2,
                               min_x        IN NUMBER,
                               max_x        IN NUMBER,
                               min_y        IN NUMBER,
                               max_y        IN NUMBER,
                               min_z        IN NUMBER,
                               max_z        IN NUMBER)
  RETURN VARCHAR2 PARALLEL_ENABLE;

end SDO_GEOM;
/


Prompt Synonym SDO_GEOM;
--
-- SDO_GEOM  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_GEOM FOR MDSYS.SDO_GEOM
/


Prompt Grants on PACKAGE SDO_GEOM TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_GEOM TO PUBLIC
/
