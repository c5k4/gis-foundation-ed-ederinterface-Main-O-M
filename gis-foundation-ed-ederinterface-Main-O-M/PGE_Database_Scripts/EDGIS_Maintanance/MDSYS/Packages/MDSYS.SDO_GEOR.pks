Prompt drop Package SDO_GEOR;
DROP PACKAGE MDSYS.SDO_GEOR
/

Prompt Package SDO_GEOR;
--
-- SDO_GEOR  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_GEOR AUTHID CURRENT_USER AS


-- ---------------------------------------------------------------------
--           Create and Process GeoRaster Objects
-- ---------------------------------------------------------------------

--
-- NAME:
--	init constructor functions
--
-- DESCRIPTION
--	init()s return a GeoRaster Object which is empty except that a RDT
--      table name and a raster ID are assigned to it. The RDT and ID are
--      required so that this object can be registered in the sysdata table
--      All other attributes are NULL.
--
--      If rasterDataTable or rasterID is not specified, it is generated
--      automatically.
--
-- ARGUMENTS
--      rasterDataTable - RDT table name
--      rasterID        - rasterID number
--
-- RETURNS
--      An GeoRaster object with rasterDataTable and rasterID initialized
--

FUNCTION init
(
   rasterDataTable IN VARCHAR2 DEFAULT NULL,
   rasterID        IN NUMBER DEFAULT NULL
)
RETURN MDSYS.SDO_GEORASTER DETERMINISTIC;


--
-- NAME:
--	createBlank functions
--
-- DESCRIPTION
--	create a blank GeoRaster object which has normal size as
--      any other GeoRaster objects except that all its cells
--      have the same value, such as 100, 5, 54.6.
--
--      NOTE, the spatialExtent will always have NULL srid and
--      integer coordinates until it is georeferenced to another CS.
--	Initially, the blank object is not georeferenced.
--
-- ARGUMENTS
--      rasterType - the GeoRaster Type defined by Oracle.
--                   the number of dimensions is implied in it
--      ultCoord   - Upper-Left corner cell coordinate (integer) in
--                   cell space. The default is (0,0)
--      dimSizes   - the size (cell numbers) along each dimension
--      cellValue  - the cell value for all raster cells
--
--      rasterDataTable and rasterID have the similar implications
--      as in init() functions. See init() comments
--
-- RETURNS
--      A Blank GeoRaster object
--

FUNCTION createBlank
(
   rasterType      IN INTEGER,
   ultCoord        IN MDSYS.SDO_NUMBER_ARRAY,
   dimSizes        IN MDSYS.SDO_NUMBER_ARRAY,
   cellValue       IN NUMBER,
   rasterDataTable IN VARCHAR2 DEFAULT NULL,
   rasterID        IN NUMBER DEFAULT NULL
)
return MDSYS.SDO_GEORASTER DETERMINISTIC;


--
-- NAME:
--      copy procedure
--
-- DESCRIPTION
--      Makes a simple copy of an existing GeoRaster object.
--
-- ARGUMENTS
--      inGeoRaster    - The SDO_GEORASTER object to be copied
--      outGeoRaster   - The new GeoRaster Object
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE copy
(
   inGeoRaster    IN     MDSYS.SDO_GEORASTER,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER
);

--
-- NAME:
--      changeFormat procedure
--
-- DESCRIPTION
--      Change the storage format of an existing GeoRaster object.
--
-- ARGUMENTS
--      georaster    - The SDO_GEORASTER object whose format to be changed
--      storageParam - A string specifying storage parameters
--                     The details are explained in GeoRaster Manual
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE changeFormat
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   storageParam     IN VARCHAR2,
   bgValues     IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--	changeFormatCopy procedure
--
-- DESCRIPTION
--	Makes a copy of an existing GeoRaster object with the same or
--      different storage format.
--
-- ARGUMENTS
--      inGeoRaster    - The SDO_GEORASTER object to be copied
--      pyramidLevel   - The pyramid level of the source GeoRaster object
--      storageParam   - A string specifying storage parameters for the copy
--                       The details are explained in GeoRaster Manual
--      outGeoRaster   - The new GeoRaster Object
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE changeFormatCopy
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE changeFormatCopy
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel   IN NUMBER,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--      subset procedure
--
-- DESCRIPTION
--      This procedure does either spatial crop/cut/clip, or layer/band
--      subset, or both.
--
-- ARGUMENTS
--      inGeoRaster    - The source SDO_GEORASTER object
--      pyramidLevel   - The pyramid level of the source GeoRaster object
--      cropArea       - The crop area. If it is of SDO_GEOMETRY type,
--                       it can be in any coordinate space. If it is
--                       of SDO_NUMBER_ARRAY, it is in cell space only
--      layerNumbers   - The logical layers
--      bandNumbers    - The physical bands in cell space
--      storageParam   - A string specifying storage parameters for the result.
--                       The details are explained in GeoRaster Manual
--      outGeoRaster   - The new GeoRaster Object
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE subset
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   cropArea       IN MDSYS.SDO_GEOMETRY,
   layerNumbers   IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
   polygonClip    IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE subset
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   cropArea       IN MDSYS.SDO_NUMBER_ARRAY,
   bandNumbers    IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE subset
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel   IN NUMBER,
   cropArea       IN MDSYS.SDO_GEOMETRY,
   layerNumbers   IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
   polygonClip    IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE subset
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel   IN NUMBER,
   cropArea       IN MDSYS.SDO_NUMBER_ARRAY,
   bandNumbers    IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--      scale procedure
--
-- DESCRIPTION
--      This procedure enlarges or reduces a georaster object and
--      overwrites it.
--
-- ARGUMENTS
--      inGeoRaster    - The source SDO_GEORASTER object
--      scaleParam     - The scale factor or factors
--      resampleParam  - The resampling method
--      storageParam   - A string specifying storage parameters for the result
--                       The details are explained in GeoRaster Manual
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE scale
(
   inGeoRaster    IN OUT MDSYS.SDO_GEORASTER,
   scaleParam     IN VARCHAR2,
   resampleParam  IN VARCHAR2,
   storageParam   IN VARCHAR2,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);


--
-- NAME:
--      scaleCopy procedure
--
-- DESCRIPTION
--      This procedure enlarges or reduces a georaster object and
--      write the result as another georaster object.
--
-- ARGUMENTS
--      inGeoRaster    - The source SDO_GEORASTER object
--      pyramidLevel   - The pyramid level of the source GeoRaster object
--      scaleParam     - The scale factor or factors
--      resampleParam  - The resampling method
--      storageParam   - A string specifying storage parameters for the result
--                       The details are explained in GeoRaster Manual
--      outGeoRaster   - The new GeoRaster Object
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE scaleCopy
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   scaleParam     IN VARCHAR2,
   resampleParam  IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE scaleCopy
(
   inGeoRaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel   IN NUMBER,
   scaleParam     IN VARCHAR2,
   resampleParam  IN VARCHAR2,
   storageParam   IN VARCHAR2,
   outGeoRaster   IN OUT MDSYS.SDO_GEORASTER,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--      generatePyramid
--
-- DESCRIPTION
--      This procedure generates pyramids, which are stored together with
--      the original data.
--
--ARGUMENTS
--      georaster     - The source SDO_GEORASTER object
--      pyramidParams - The pyramid parameters
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE generatePyramid
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   pyramidParams IN     VARCHAR2,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);


--
-- NAME:
--      deletePyramid
--
-- DESCRIPTION
--      This procedure deletes pyramids of a georaster object.
--
--ARGUMENTS
--      georaster     - The source SDO_GEORASTER object
--
-- RETURNS
--      A GeoRaster object
--
PROCEDURE deletePyramid
(
   georaster     IN OUT MDSYS.SDO_GEORASTER
);



--
-- NAME:
--      generateSpatialExtent
--
-- DESCRIPTION
--      This function generates the spatialExtent of a georaster object
--      using its SRS (spatial reference system) information.
--      The srid of the generated spatialExtent is decided by the SRS.
--      The srid is null if the georaster object is not referenced.
--
--ARGUMENTS
--      georaster - The GeoRaster object on which the spatialExtent is
--                  to be generated
--      height    - Z value in the 3D GeoReference system
--
-- RETURNS
--      the spatialExtent geometry
--
FUNCTION generateSpatialExtent
(
   georaster  IN MDSYS.SDO_GEORASTER,
   height     IN NUMBER DEFAULT NULL
)
return MDSYS.SDO_GEOMETRY DETERMINISTIC;

--
-- NAME:
--      mosaic
--
-- DESCRIPTION
--      This procedure mosaics multiple GeoRaster objects into one. The
--      source GeoRaster objects must be prepared tiles or blocks that
--      are rectified and referenced onto the same coordinate system
--      with affine transformation. They must have the same number of
--      layers/bands and must be spatially aligned along the row and column
--      dimensions. They must also have the same mapping between the band
--      and layer numbers.
--
--ARGUMENTS
--      georasterTableName  - the table containing all source georaster objects
--      georasterColumnName - the column of the georasterTable, in which all
--                            georaster objects are to be masaicked
--      georaster           - the result, i.e., the mosaic georaster object
--      storageParam        - storage parameter specification
--      bgValues            - background values for filling in the gaps
--
-- RETURNS
--      a mosaic georaster object
--
PROCEDURE mosaic
(
   georasterTableName  IN     VARCHAR2,
   georasterColumnName IN     VARCHAR2,
   georaster           IN OUT MDSYS.SDO_GEORASTER,
   storageParam        IN     VARCHAR2,
   bgValues            IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--      validateForMosaic
--
-- DESCRIPTION
--      check whether tiles in the given table are mosaickable
--
-- ARGUMENTS
--      georasterTableName  - the table containing all source georaster objects
--      georasterColumnName - the column of the georasterTable, in which all
--                            georaster objects are to be masaicked
--      resultTableName     - the name of the table to store the result
-- NOTES
--   A result table must be created before calling this procedure. The format
--   of the result table is:
--    (description VARCHAR2(80) NOT NULL, rowid1 ROWID, rowid2 ROWID)
--
PROCEDURE validateForMosaic
(
   georasterTableName  IN VARCHAR2,
   georasterColumnName IN VARCHAR2,
   resultTableName     IN VARCHAR2
);

-- NAME:
--      mosaic
--
-- DESCRIPTION
--      This procedure mosaics multiple GeoRaster objects retrieved from a
--      cursor and return the mosaicked image in the specified window of
--      interest. The source GeoRaster objects must be prepared tiles or
--      blocks that are rectified and referenced onto the same coordinate
--      system with affine transformation. They must have the same number of
--      layers/bands and must be spatially aligned along the row and column
--      dimensions. They must also have the same mapping between the band
--      and layer numbers.
--
--ARGUMENTS
--      georasters          - the source georaster objects
--      window              - the cropping window
--      bandNumbers         - band selection
--      rasterBlob          - result lob locator
--      metadata            - output georaster metadata
--      interleaving        - result interleaving type
--      cellDepth           - result cellDepth
--      bgValues            - background values for filling in the gaps
--
-- NOTES
--  1) result cellDepth can not be 1BIT, 2BIT or 4BIT; this restriction
--     should be lifted after the LOB r/w handling is moved to C
--  2) the result is always uncompressed; this restriction
--     should be lifted later (see 4) below)
--  3) sdo_geor.getRasterSubset is called to retrieve raster data from the
--     source objects; bgValues is passed into sdo_geor.getRasterSubset but
--     ignored otherwise
--  4) interleaving/cellDepth should be replaced by a combined storageParam
--     when the LOB r/w is handled in C, to be consistent with other
--     interfaces
--
PROCEDURE mosaic
(
 georasters     IN     SYS_REFCURSOR,
 window         IN     SDO_GEOMETRY,
 bandNumbers    IN     SDO_RANGE_ARRAY,
 rasterBlob     IN OUT NOCOPY BLOB,
 metadata       OUT    XMLType,
 interleaving   IN     VARCHAR2 DEFAULT '8BIT_U',
 cellDepth      IN     VARCHAR2 DEFAULT 'BIP',
 bgValues       IN     SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--      mergeLayers
--
-- DESCRIPTION
--      append selected layers of a GeoRaster object to another GeoRaster object
--
-- ARGUMENTS
--      targetGeoraster    - the GeoRaster object to be appended to
--      sourceGeoraster    - the source GeoRaster object
--      sourceLayerNumbers - source layers to be appended to the target
--      bgValues           - background filling values
-- NOTES
--   The source and target GeoRaster objects must have the same row and cloumn
--   dimension sizes. They also must cover the same area in the model (if both
--   are georeferenced) or cell (if neither is georeferenced) space.
--
PROCEDURE mergeLayers(
   targetGeoraster     IN OUT MDSYS.SDO_GEORASTER,
   sourceGeoraster     IN     MDSYS.SDO_GEORASTER,
   sourceLayerNumbers  IN     VARCHAR2 DEFAULT NULL,
   bgValues            IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--     mergeLayers
--
-- DESCRIPTION
--      union selected layers of two GeoRaster objects to generate a new
--      GeoRaster object
--
-- ARGUMENTS
--      source1Georaster    - the first source GeoRaster object
--      source1LayerNumbers - source layers of the first object to be included
--      source2Georaster    - the second GeoRaster object
--      source2LayerNumbers - source layers of the second object to be included
--      storageParam        - the storage parameter for the output object
--      outGeoraster        - the result GeoRaster object
--      bgValues            - background filling values
-- NOTES
--   The two source GeoRaster objects must have the same row and cloumn
--   dimension sizes and the same georeferencing.
--
--   If storageParam is NULL, the result object has the same storage
--   attributes as those of the first source object.
--
PROCEDURE mergeLayers(
   source1Georaster     IN     MDSYS.SDO_GEORASTER,
   source1LayerNumbers  IN     VARCHAR2,
   source2Georaster     IN     MDSYS.SDO_GEORASTER,
   source2LayerNumbers  IN     VARCHAR2,
   storageParam         IN     VARCHAR2,
   outGeoraster         IN OUT MDSYS.SDO_GEORASTER,
   bgValues             IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--     updateRaster
--
-- DESCRIPTION
--     Update a specified area or the overlapping parts of one GeoRaster
--     object with selected bands or layers of another GeoRaster object.
--
-- ARGUMENTS
--     targetGeoraster    - the GeoRaster object to be updated
--     targetPyramidLevel - the target pyramid level to be updated
--     targetLayerNumbers - the layers to be updated
--     targetBandNumbers  - the bands to be updated
--     targetArea         - the area to be updated
--     sourceGeoraster    - the source GeoRaster object
--     sourcePyramidLevel - the source pyramid level
--     sourceLayerNumbers - the source layers to be considered
--     sourceBandNumbers  - the source bands to be considered
--     updateUpperPyramids - whether to update upper level pyramids
--     bgValues           - background filling values
--
-- NOTES
--   If georeferenced, both objects should be in the same coordinate system.
--   Both GeoRaster objects should have the same cell depth and spatial
--   resolution (if georeferenced).
--   If both are not georeferenced, the UTLCoordinates will be considered to
--   co-locate them into each other.
--   The two georaster objects could have different dimensions and sizes.
--   If targetArea is not specified (NULL), all the overlapping parts are
--   updated.
--   The band/layer numbers should have a one-to-one matching.
--   If the source georaster object is not large enough to fill in the target
--   area, the uncovered area will not be updated.
--   If the source georaster object is sparse, background filling values
--   are considered.
--   If the target GeoRaster objects has pyramids and/or is compressed, the
--   updates should be automatically reflected in the pyramids and/or be
--   compressed as well.
--
PROCEDURE updateRaster(
   targetGeoraster    IN OUT MDSYS.SDO_GEORASTER,
   targetPyramidLevel IN     NUMBER,
   targetLayerNumbers IN     VARCHAR2,
   targetArea         IN     MDSYS.SDO_GEOMETRY,
   sourceGeoraster    IN     MDSYS.SDO_GEORASTER,
   sourcePyramidLevel IN     NUMBER,
   sourceLayerNumbers IN     VARCHAR2,
   updateUpperPyramids IN    VARCHAR2,
   bgValues           IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE updateRaster(
   targetGeoraster    IN OUT MDSYS.SDO_GEORASTER,
   targetPyramidLevel IN     NUMBER,
   targetBandNumbers  IN     VARCHAR2,
   targetArea         IN     MDSYS.SDO_NUMBER_ARRAY,
   sourceGeoraster    IN     MDSYS.SDO_GEORASTER,
   sourcePyramidLevel IN     NUMBER,
   sourceBandNumbers  IN     VARCHAR2,
   updateUpperPyramids IN    VARCHAR2,
   bgValues           IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--
-- NAME:
--     createTemplate
--
-- DESCRIPTION
--     Generate template metadata for a georaster object, and optionally
--     insert entries with empty raster blocks into the RDT.
--
-- ARGUMENTS
--     georaster       - the georaster object
--     rasterType      - the raster type
--     rasterSpec      - the raster specification
--     maskLayers      - layers with bitmap mask
--     initRdtEntry    - whether to insert the RDT entries
--
-- NOTES
--
PROCEDURE createTemplate
(
   georaster   IN OUT MDSYS.SDO_GEORASTER,
   rasterType      IN NUMBER,
   rasterSpec      IN VARCHAR2,
   maskLayers      IN VARCHAR2 DEFAULT NULL,
   initRdtEntry    IN VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--     getRasterBlockLocator
--
-- DESCRIPTION
--     Get the LOB locator of a specified raster block
--
-- ARGUMENTS
--     georaster       - the source GeoRaster object
--     pyramidLevel    - the pyramid level
--     bandBlockNumber - the band block number
--     rowBlockNumber  - the row block number
--     colBlockNumber  - the column block number
--     loc             - the in/out lob locator
--     isBitmapMask    - 'true' if access a rasterblock of a bitmap mask
--     lock_for_write  - lock for write
--
-- NOTES
--     If the value for isBitmapMask is not 'true' (case insensitive),
--     a regular raster block is accessed; otherwise, a bitmap mask block
--     is being accessed.
--
--     If the value for lock_for_write is not 'true' (case insensitive),
--     the row in the RDT is not locked.
--
PROCEDURE getRasterBlockLocator
(
   georaster         IN MDSYS.SDO_GEORASTER,
   pyramidLevel      IN NUMBER,
   bandBlockNumber   IN NUMBER,
   rowBlockNumber    IN NUMBER,
   columnBlockNumber IN NUMBER,
   loc               IN OUT NOCOPY BLOB,
   isBitmapMask      IN VARCHAR2 DEFAULT NULL,
   lock_for_write    IN VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--   validateBlockMBR
--
-- DESCRIPTION
--   check whether the blockMBR geometries in the RDT are valid
--
-- ARGUMENTS
--   georaster - the GeoRaster object whose blockMBRs are to be checked
--
-- RETURNS
--   'NULL' if the GeoRaster object is NULL;
--   'TRUE' if the blockMBRs are valid;
--    otherwise an error number
--
-- NOTES
--   Each blockMBR is a rectangle defined in the cell space and must be
--   IDENTICAL to following geometry, respectively:
--     MDSYS.SDO_GEOMETRY(2003,NULL,NULL,
--               MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,3),
--               MDSYS.SDO_ORDINATE_ARRAY(R0+rowBlockNumber*RSZ,
--                         C0+columnBlockNumber*CSZ,
--                         R0+(rowBlockNumber+1)*RSZ-1,
--                         C0+(columnBlockNumber+1)*CSZ-1))
--   where:
--     R0 is the upper-left row coordinate in the cell space;
--     C0 is the upper-left column coordinate in the cell space;
--     RSZ is the row block size;
--     CSZ is the column block size

--   The dimension information of a blockMBR geometry must be in the order
--   of (ROW, COLUMN).

FUNCTION validateBlockMBR
(
   georaster IN MDSYS.SDO_GEORASTER
) RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

--
-- NAME:
--   generateBlockMBR
--
-- DESCRIPTION
--   generate correct blockMBR geometries in the RDT
--
-- ARGUMENTS
--   georaster - the GeoRaster object whose blockMBRs are to be generated
--
-- NOTES
--   The raster rows in the RDT are updated with the correct blockMBR values,
--   but the GeoRaster object itself is not changed.
--
PROCEDURE generateBlockMBR
(
   georaster IN MDSYS.SDO_GEORASTER
);

-- ---------------------------------------------------------------------
--  Compress and Decompress GeoRaster Objects
--
--  They are all embedded into "storageParam" in related procedures
--  using keyword "compression"
-- ---------------------------------------------------------------------
--
-- NAME:
--      calcCompressionRatio function
--
-- DESCRIPTION
--      calculates the compression ratio of a compressed GeoRaster
--      object on the fly.
--
-- ARGUMENTS
--      georaster  - The compressed SDO_GEORASTER object
--
-- RETURNS
--      A Number
--
FUNCTION calcCompressionRatio
(
   georaster IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
--  Update/Modify/Change GeoRaster Object Cell (or say, Raster) Data
--
--  These functions will change cell values, so use them cautiously.
-- ---------------------------------------------------------------------
--
-- NAME:
--      changeCellValue procedure
--
-- DESCRIPTION
--      This procedure changes all cells inside a window of each
--      specified layer/band to have a specified value
--
-- ARGUMENTS
--      georaster      - The SDO_GEORASTER object to be changed
--      window         - The window inside which each cell will be changed
--                       to have the newCellValue. If it is of SDO_GEOMETRY type,
--                       it can be in any coordinate space. If it is
--                       of SDO_NUMBER_ARRAY, it is in cell space only
--      layerNumbers   - The logical layers whose cells to be changed
--      bandNumbers    - The physical bands in cell space to be changed
--      newCellValue   - the new cell value for each cell inside the window
--                       of each specified band/layer
--      bgValues       - background values for filling sparse data
--
-- RETURNS
--      the source GeoRaster object with cells changed
--
PROCEDURE changeCellValue
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   window        IN     MDSYS.SDO_NUMBER_ARRAY,
   bandNumbers   IN     VARCHAR2,
   newCellValue  IN     NUMBER,
   bgValues      IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE changeCellValue
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   window        IN     MDSYS.SDO_GEOMETRY,
   layerNumbers  IN     VARCHAR2,
   newCellValue  IN     NUMBER,
   bgValues      IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);


-- ---------------------------------------------------------------------
--    Georeference GeoRaster Objects
--
-- Other georeferencing or related functions include:
-- 1. setSRS() adds, modifies and deletes georeferencing info
-- 2. importFrom() loads ArcInfo World File from file or CLOB
-- 3. the tool GeoRasterLoader loads ArcInfo World File from file
-- ---------------------------------------------------------------------

--
-- NAME:
--      georeference
--
-- DESCRIPTION
--      georeference a GeoRaster object using the provided
--      cell-to-model transformation coefficients.
--      in 10iR1, it means the following affine transformation:
--        x = A * col + B * row + C
--        y = D * col + E * row + F
--      where A, B, C, D, E, F are given coefficients.
--      The result will be the following model-to-cell tranformation:
--        row = a + b * x + c * y
--        col = d + e * x + f * y
--      where a, b, c, d, e, f will be stored in the SRS of the
--      GeoRaster object.
--
--      NOTE: the spatial resolutions might not be set by this function.
--            If so, call setSpatialResolutions() to do it separately.
--
-- ARGUMENTS
--      georaster      - the georaster object to be georeferenced
--      srid           - the model space onto which the georaster object
--                       is to be georeferenced
--      modelCoordinateLocation -
--                       the modelCoordinateLocation of each cell/pixel,
--                       only CENTER (0) and UPPERLEFT (1) are allowed.
--      xCoefficients  - A, B, C
--      yCoefficients  - D, E, F
--
-- RETURNS
--      the input georaster object for update.
--      the original existing SRS (if any) is overwritten.
--
PROCEDURE georeference
(
   georaster               IN OUT MDSYS.SDO_GEORASTER,
   srid                    IN     NUMBER,
   modelCoordinateLocation IN     NUMBER,
   xCoefficients           IN     MDSYS.SDO_NUMBER_ARRAY,
   yCoefficients           IN     MDSYS.SDO_NUMBER_ARRAY
);


-- ---------------------------------------------------------------------
--           Query GeoRaster Object Cell (or say, Raster) Data
-- ---------------------------------------------------------------------

--
-- NAME:
--      getModelCoordinate
--
-- DESCRIPTION
--      Given a spatial coordinate, i.e., (row, column, vertical) of
--      a cell in the cell coordinate space, its corresponding spatial
--      coordinate (X, Y, Z) in the model coordinate space
--      is calculated and returned.
--
-- ARGUMENTS
--      georaster      - the georaster object to query upon.
--      pyramidLevel   - the pyramid level in which the cell is.
--      cellCoordinate - number array containing row, column,
--                       and vertical if its spatial dimension is 3
--      modelCoordinate - a SDO_GEOMETRY object in the model space that
--                        contains X, Y and/or Z
--      height         - Z value in 3D GeoReference system
--
-- RETURNS
--      a SDO_GEOMETRY object containing X, Y and/or Z
--
FUNCTION getModelCoordinate
(
   georaster         IN MDSYS.SDO_GEORASTER,
   pyramidLevel      IN NUMBER,
   cellCoordinate    IN MDSYS.SDO_NUMBER_ARRAY,
   height            IN NUMBER DEFAULT NULL
)
RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

PROCEDURE getModelCoordinate
(
   georaster         IN MDSYS.SDO_GEORASTER,
   pyramidLevel      IN NUMBER,
   cellCoordinate    IN MDSYS.SDO_GEOMETRY,
   modelCoordinate   OUT MDSYS.SDO_GEOMETRY,
   height            IN NUMBER DEFAULT NULL
);

--
-- NAME:
--      getCellCoordinate
--
-- DESCRIPTION
--      Given a spatial coordinate, i.e., (X, Y, Z) of a point in the model
--      coordinate space, its corresponding spatial coordinate (row, column,
--      vertical) in the cell coordinate space is calculated and returned.
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the pyramid level in which the cell is.
--      modelCoordinate - a SDO_GEOMETRY point containing X, Y,
--                        and Z if its spatial dimension is 3
--      subCell         - whether floating cell coordinates are returned
--
-- RETURNS
--      either a cellCoordinate array or a SDO_GEOMETRY object containing
--      row, column and/or vertical
--
-- NOTES
--      If the output type is MDSYS.SDO_NUMBER_ARRY, the input must be a point
--      geometry; otherwise, the input can be geometry of any types
--
--      Cell coordinates are returned as floating numbers if subCell is
--      'TRUE' (case insensitive). Otherwise, they are returned as integers.
--
FUNCTION getCellCoordinate
(
   georaster         IN MDSYS.SDO_GEORASTER,
   pyramidLevel      IN NUMBER,
   modelCoordinate   IN MDSYS.SDO_GEOMETRY,
   subCell           IN VARCHAR2 DEFAULT NULL,
   height            IN NUMBER DEFAULT NULL,
   vert_id           IN NUMBER DEFAULT NULL,
   ellipsoidal       IN VARCHAR2 DEFAULT NULL
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

PROCEDURE getCellCoordinate
(
   georaster         IN MDSYS.SDO_GEORASTER,
   pyramidLevel      IN NUMBER,
   modelCoordinate   IN MDSYS.SDO_GEOMETRY,
   cellCoordinate    OUT MDSYS.SDO_GEOMETRY,
   subCell           IN VARCHAR2 DEFAULT NULL,
   height            IN NUMBER DEFAULT NULL,
   vert_id           IN NUMBER DEFAULT NULL,
   ellipsoidal       IN VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--      getCellCoordinate
--
-- DESCRIPTION
--      Convert cell coordinates between different pyramid levels.
--
-- ARGUMENTS
--      georaster            - the georaster object to query upon
--      sourcepyramidLevel   - the source pyramid level
--      sourceCellCoordinate - the source cell coordinates
--      targetPyramidLevel   - the target pyramid level
--      subCell           - whether floating cell coordinates are returned
--
-- RETURNS
--      either a sdo_number_array or a SDO_GEOMETRY object containing
--      the corresponding cell coordinates at the target pyramid level.
--
-- NOTES
--      Cell coordinates are returned as floating numbers if subCell is
--      'TRUE' (case insensitive). Otherwise, they are returned as integers.
--

FUNCTION getCellCoordinate
(
   georaster            IN MDSYS.SDO_GEORASTER,
   sourcePyramidLevel   IN NUMBER,
   sourceCellCoordinate IN MDSYS.SDO_NUMBER_ARRAY,
   targetPyramidLevel   IN NUMBER,
   subCell              IN VARCHAR2 DEFAULT NULL
) RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCellCoordinate
(
   georaster            IN MDSYS.SDO_GEORASTER,
   sourcePyramidLevel   IN NUMBER,
   sourceCellCoordinate IN MDSYS.SDO_GEOMETRY,
   targetPyramidLevel   IN NUMBER,
   subCell              IN VARCHAR2 DEFAULT NULL
) RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

--
-- NAME:
--      getCellValue functions
--
-- DESCRIPTION
--      The functions retrieve the value of a single cell
--      located anywhere in the georaster object by specifying
--      its row/column/band numbers in its cell coordinate space
--      or by specifying a point geometry in either model
--      coordinate space or cell coordinate space
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      rowNumber       - row number in cell space
--      colNumber       - column number in cell space
--      bandNumber      - band number in cell space (not model space)
--      layerNumber     - logical layer number
--      ptGeom          - a point geometry in either cell space
--                        or model space
-- RETURNS
--      Number
--
FUNCTION getCellValue
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   rowNumber    IN NUMBER,
   colNumber    IN NUMBER,
   bandNumber   IN NUMBER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCellValue
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   ptGeom       IN MDSYS.SDO_GEOMETRY,
   layerNumber  IN NUMBER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCellValue
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   rowNumber    IN NUMBER,
   colNumber    IN NUMBER,
   bands        IN VARCHAR2
)
RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCellValue
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   ptGeom       IN MDSYS.SDO_GEOMETRY,
   layers       IN VARCHAR2
)
RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;
--
-- NAME:
--      evaluate functions
--
-- DESCRIPTION
--      Based on different interpolation methods specified by parameter
--      interpolationMethod, the functions will evaluate and return values
--      for the cells located anywhere in the georaster object by specifying
--      its row/column/band numbers in its cell coordinate space
--      or by specifying a point geometry in either model
--      coordinate space or cell coordinate space
--      The only difference between evaluate and evaluateDouble is that evaluate
--      returns values based on the georaster object cellDepth, but evaluateDouble
--      always returns double numbers.
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      row             - row number in cell space
--      column          - column number in cell space
--      bands           - band number array in cell space (not model space)
--      layers          - logical layer number array
--      ptGeom          - a point geometry in either cell space
--                        or model space
-- RETURNS
--      SDO_Number_array
--
--FUNCTION evaluate
--(
--   georaster    IN MDSYS.SDO_GEORASTER,
--   pyramidLevel IN NUMBER,
--   row          IN NUMBER,
--   column       IN NUMBER,
--   bands        IN VARCHAR2,
--   interpolationMethod  IN VARCHAR2
--)
--RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

--FUNCTION evaluate
--(
--   georaster    IN MDSYS.SDO_GEORASTER,
--   pyramidLevel IN NUMBER,
--   ptGeom       IN MDSYS.SDO_GEOMETRY,
--   layers       IN VARCHAR2,
--   interpolationMethod  IN VARCHAR2
--)
--RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION evaluateDouble
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   row          IN NUMBER,
   column       IN NUMBER,
   bands        IN VARCHAR2,
   interpolationMethod  IN VARCHAR2
)
RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION evaluateDouble
(
   georaster    IN MDSYS.SDO_GEORASTER,
   pyramidLevel IN NUMBER,
   ptGeom       IN MDSYS.SDO_GEOMETRY,
   layers       IN VARCHAR2,
   interpolationMethod  IN VARCHAR2
)
RETURN SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;


--
-- NAME:
--      getRasterBlocks functions
--
-- DESCRIPTION
--      This function returns a sdo_rasterset table containing
--      all sub-blocks of the specified pramid level which are
--      inside and touch the given window
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      window          - a rectanglar window in cell space
--
-- RETURNS
--      SDO_RASTERSET
--
FUNCTION getRasterBlocks
(
   georaster        IN MDSYS.SDO_GEORASTER,
   pyramidLevel     IN NUMBER,
   window           IN MDSYS.SDO_NUMBER_ARRAY
)
return MDSYS.SDO_RASTERSET DETERMINISTIC;

--
-- NAME:
--      getRasterBlocks functions
--
-- DESCRIPTION
--      This function returns a sdo_rasterset table containing
--      all sub-blocks of the specified pramid level which are
--      inside and touch the given window
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      window          - a SDO geometry
--
-- RETURNS
--      SDO_RASTERSET
--
FUNCTION getRasterBlocks
(
   georaster        IN MDSYS.SDO_GEORASTER,
   pyramidLevel     IN NUMBER,
   window           IN MDSYS.SDO_GEOMETRY
)
return MDSYS.SDO_RASTERSET DETERMINISTIC;


--
-- NAME:
--      getRasterSubset
--
-- DESCRIPTION
--      This function returns a single blob holding the mosaic of
--      all sub-blocks of the specified pramid level which are
--      inside and touch the given window
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      inWindow        - the window of interest
--      bandNumbers     - bandNumbers
--      rasterBlob      - holds the result, i.e., the mosaicked
--                        raster subset
--      outWindow       - the output window
--      storageParam    - storage parameters for the output
--      bgValues        - background values
--
-- RETURNS
--      BLOB
--
-- NOTES
--      if the type of inWindow is SDO_NUMBER_ARRAY, it specifies a rectanglar
--      window in the cell space
--      outWindow contains the coordinates of the upper-left and bottom-right
--      corners of the output window in the cell space
--
PROCEDURE getRasterSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   pyramidLevel   IN     NUMBER,
   window         IN     MDSYS.SDO_NUMBER_ARRAY,
   bandNumbers    IN     VARCHAR2,
   rasterBlob     IN OUT NOCOPY BLOB,
   storageParam   IN     VARCHAR2 DEFAULT NULL,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE getRasterSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   pyramidLevel   IN     NUMBER,
   inWindow       IN     MDSYS.SDO_NUMBER_ARRAY,
   bandNumbers    IN     VARCHAR2,
   rasterBlob     IN OUT NOCOPY BLOB,
   outWindow      OUT    SDO_NUMBER_ARRAY,
   storageParam   IN     VARCHAR2 DEFAULT NULL,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE getRasterSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   pyramidLevel   IN     NUMBER,
   window         IN     MDSYS.SDO_GEOMETRY,
   layerNumbers   IN     VARCHAR2,
   rasterBlob     IN OUT NOCOPY BLOB,
   storageParam   IN     VARCHAR2 DEFAULT NULL,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
   polygonClip    IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE getRasterSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   pyramidLevel   IN     NUMBER,
   inWindow       IN     MDSYS.SDO_GEOMETRY,
   layerNumbers   IN     VARCHAR2,
   rasterBlob     IN OUT NOCOPY BLOB,
   outWindow      OUT    MDSYS.SDO_NUMBER_ARRAY,
   storageParam   IN     VARCHAR2 DEFAULT NULL,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
   polygonClip    IN     VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--      getRasterData
--
-- DESCRIPTION
--      This function returns a single blob holding all raster data
--      of the input georaster object at the specified pyramid level.
--      If it is blocked, the mosaic of all blocks of the specified
--      pramid level is returned.
--
-- ARGUMENTS
--      georaster       - the georaster object to query upon
--      pyramidLevel    - the level in pyramid
--      rasterBlob      - holds the result, i.e., the raster data
--      storageParam    - storage parameters for the output
--      bgValues        - background values
--
-- RETURNS
--      BLOB
--
PROCEDURE getRasterData
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   pyramidLevel   IN     NUMBER,
   rasterBlob     IN OUT NOCOPY BLOB,
   storageParam   IN     VARCHAR2 DEFAULT NULL,
   bgValues       IN     MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);


-- ---------------------------------------------------------------------
--        Query GeoRaster Object Metadata and Ancillary Data
-- ---------------------------------------------------------------------

FUNCTION getVersion
(
  georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC  PARALLEL_ENABLE;

FUNCTION getID
(
  georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION isBlank
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBlankCellValue
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getDefaultColorLayer
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getDefaultRed
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getDefaultBlue
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getDefaultGreen
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSpatialDimNumber
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSpatialDimSizes
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBandDimSize
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCellDepth
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getNODATA
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getULTCoordinate
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getInterleavingType
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getCompressionType
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBlockingType
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBlockSize
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getPyramidType
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getPyramidMaxLevel
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION isSpatialReferenced
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION isRectified
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION isOrthoRectified
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getModelSRID
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSpatialResolutions
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSRS
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_GEOR_SRS DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getModelCoordLocation
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBeginDateTime
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN TIMESTAMP WITH TIME ZONE DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getEndDateTime
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN TIMESTAMP WITH TIME ZONE DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSpectralUnit
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getSpectralResolution
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

-- for all layer-related get and set functions,
-- layerNumber = 0 means the object layer
-- layerDimension defaults to 'BAND' if not specified

FUNCTION getLayerDimension
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN MDSYS.SDO_STRING_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getTotalLayerNumber
(
   georaster    IN MDSYS.SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getLayerOrdinate
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getLayerID
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getScaling
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBinType
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBinTable
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBinFunction
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getStatistics
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_NUMBER_ARRAY DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getHistogram
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_GEOR_HISTOGRAM DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getHistogramTable
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION hasGrayScale
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getGrayScale
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_GEOR_GRAYSCALE DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getGrayScaleTable
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION hasPseudoColor
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getColorMap
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN MDSYS.SDO_GEOR_COLORMAP DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getColorMapTable
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getVAT
(
   georaster    IN MDSYS.SDO_GEORASTER,
   layerNumber  IN NUMBER
)
RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
--
--   statistics functions
--
-- ---------------------------------------------------------------------

Function  generateStatistics
(
    georaster         IN OUT SDO_GEORASTER,
    samplingFactor    IN VARCHAR2,
    samplingWindow    IN SDO_GEOMETRY,
    histogram         IN VARCHAR2,
    layerNumbers      IN VARCHAR2 DEFAULT NULL,
    useBin            IN VARCHAR2 DEFAULT 'TRUE',
    binFunction       IN MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
    nodata            IN VARCHAR2 DEFAULT 'FALSE'
)
return Varchar2;

Function generateStatistics
(
    georaster         IN OUT SDO_GEORASTER,
    samplingFactor    IN VARCHAR2,
    samplingWindow    IN SDO_NUMBER_ARRAY,
    histogram         IN VARCHAR2,
    layerNumbers      IN VARCHAR2 DEFAULT NULL,
    useBin            IN VARCHAR2 DEFAULT 'TRUE',
    binFunction       IN MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL,
    nodata            IN VARCHAR2 DEFAULT 'FALSE'
)
return Varchar2;



-- ---------------------------------------------------------------------
--    Update and Delete GeoRaster Object Metadata and Ancillary Data
--
--  SET procedures function as both UPDATE/MODIFY and DELETE.
--  In SET procedures, if the parameter is given as null, it will DELETE
--  the corresponding item/data in the georaster object only if the item
--  is allowed to be deleted (optional or minOccurs=0 in xml schema) and
--  without other dependency and inter-relationship issues.
--  Validation will be done in those procedures in both cases.
--  To avoid losing data, use them cautiously.
-- ---------------------------------------------------------------------

-- change the last two digits only
PROCEDURE setRasterType
(
  georaster     IN OUT MDSYS.SDO_GEORASTER,
  rasterType    IN     NUMBER
);

PROCEDURE setID
(
  georaster     IN OUT MDSYS.SDO_GEORASTER,
  id            IN     VARCHAR2
);

PROCEDURE setBlankCellValue
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   value        IN     NUMBER
);

PROCEDURE setDefaultColorLayer
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   defaultRGB   IN     MDSYS.SDO_NUMBER_ARRAY
);

PROCEDURE setDefaultRed
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   defaultRed   IN     NUMBER
);

PROCEDURE setDefaultGreen
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   defaultGreen IN     NUMBER
);

PROCEDURE setDefaultBlue
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   defaultBlue  IN     NUMBER
);

-- only if not spatial, temporal and band referenced
PROCEDURE setULTCoordinate
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   ultCoord     IN     MDSYS.SDO_NUMBER_ARRAY
);

PROCEDURE setSpatialReferenced
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   isReferenced IN     VARCHAR2
);

-- set TRUE only if spatial referenced
PROCEDURE setRectified
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   isRectified  IN     VARCHAR2
);

-- set TRUE only if rectified
PROCEDURE setOrthoRectified
(
   georaster        IN OUT MDSYS.SDO_GEORASTER,
   isOrthoRectified IN     VARCHAR2
);

PROCEDURE setModelSRID
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   srid         IN     NUMBER
);

PROCEDURE setSpatialResolutions
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   resolutions  IN     MDSYS.SDO_NUMBER_ARRAY
);

PROCEDURE setSRS
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   srs          IN     MDSYS.SDO_GEOR_SRS
);

-- value can be set only if spatial referenced
PROCEDURE setModelCoordLocation
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   modelCoordLoc IN     VARCHAR2
);


PROCEDURE setBeginDateTime
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   beginTime    IN     TIMESTAMP WITH TIME ZONE
);

PROCEDURE setEndDateTime
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   endTime      IN     TIMESTAMP WITH TIME ZONE
);

PROCEDURE setSpectralUnit
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   unit         IN     VARCHAR2
);

PROCEDURE setSpectralResolution
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   resolution   IN     NUMBER
);

PROCEDURE setLayerOrdinate
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   ordinate     IN     NUMBER
);

PROCEDURE setLayerID
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   id           IN     VARCHAR2
);

PROCEDURE setScaling
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   scalingFunc  IN     MDSYS.SDO_NUMBER_ARRAY
);

-- change binType if necessary
PROCEDURE setBinTable
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   tableName    IN     VARCHAR2
);

PROCEDURE setBinFunction(
   georaster   IN OUT MDSYS.SDO_GEORASTER,
   layerNumber IN     NUMBER,
   binFunction IN     MDSYS.SDO_NUMBER_ARRAY
);

PROCEDURE setGrayScale
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   grayScale    IN     MDSYS.SDO_GEOR_GRAYSCALE
);

PROCEDURE setGrayScaleTable
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   tableName    IN     VARCHAR2
);

PROCEDURE setColorMap
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   colorMap     IN     MDSYS.SDO_GEOR_COLORMAP
);

PROCEDURE setColorMapTable
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   tableName    IN     VARCHAR2
);

PROCEDURE setVAT
(
   georaster    IN OUT MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   vatName      IN     VARCHAR2
);

PROCEDURE setVersion
(
  georaster    IN OUT MDSYS.SDO_GEORASTER,
  majorVersion IN VARCHAR2,
  minorVersion IN VARCHAR2
);

PROCEDURE setStatistics
(
  georaster    IN OUT MDSYS.SDO_GEORASTER,
  layerNumber  IN NUMBER,
  statistics   IN MDSYS.SDO_NUMBER_ARRAY
);

--
-- NOTES
--   1) Whenever histogram is updated, new statistics must be provided.
--   2) If statistics is set with different samplingFactor/samplingWindow,
--      existing histogram is removed if not reset.
--   3) The samplingWindow must be inside the extent in the cell space.
--   4) If not specified, the default value for samplingFactor is 1.
--   5) If not specified, the default value for samplingWindow is the
--      whole image and it is removed from the metadata.
--
PROCEDURE setStatistics
(
  georaster    IN OUT MDSYS.SDO_GEORASTER,
  layerNumber  IN NUMBER,
  statistics   IN MDSYS.SDO_NUMBER_ARRAY,
  histogram    IN MDSYS.SDO_GEOR_HISTOGRAM,
  samplingFactor IN NUMBER DEFAULT NULL,
  samplingWindow IN MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

-- NOTES
--   1) The histogram table cannot be set if no statistics exist.
--   2) The histogram table is removed if histogramTable is NULL.
--   3) The samplingWindow must be inside the extent in the cell space.
--   4) If not specified, the default value for samplingFactor is 1.
--   5) If not specified, the default value for samplingWindow is the
--      whole image and it is removed from the metadata.
--
PROCEDURE setStatistics
(
  georaster    IN OUT MDSYS.SDO_GEORASTER,
  layerNumber  IN NUMBER,
  statistics   IN MDSYS.SDO_NUMBER_ARRAY,
  histogramTable IN VARCHAR2,
  samplingFactor IN NUMBER DEFAULT NULL,
  samplingWindow IN MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
);

--PROCEDURE setHistogram
--(
--  georaster    IN OUT MDSYS.SDO_GEORASTER,
--  layerNumber  IN NUMBER,
--  histogram    IN MDSYS.SDO_GEOR_HISTOGRAM,
--  samplingFactor IN NUMBER DEFAULT NULL,
--  samplingWindow IN MDSYS.SDO_NUMBER_ARRAY DEFAULT NULL
--);

PROCEDURE setHistogramTable
(
  georaster    IN OUT MDSYS.SDO_GEORASTER,
  layerNumber  IN NUMBER,
  tableName    IN VARCHAR2
);

PROCEDURE setSourceInfo
(
   georaster   IN OUT MDSYS.SDO_GEORASTER,
   sourceInfo  IN VARCHAR2
);

PROCEDURE addSourceInfo
(
   georaster   IN OUT MDSYS.SDO_GEORASTER,
   sourceInfo  IN VARCHAR2
);

FUNCTION getSourceInfo
(
   georaster   IN SDO_GEORASTER
) RETURN MDSYS.SDO_STRING2_ARRAY DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
--           multiple nodata values/value ranges
-- ---------------------------------------------------------------------

-- remove one NODATA value
PROCEDURE deleteNODATA
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   layerNumber   IN     NUMBER,
   NODATA        IN     NUMBER
);

-- remove multiple NODATA values or value ranges
PROCEDURE deleteNODATA
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   layerNumber   IN     NUMBER,
   NODATA        IN     MDSYS.SDO_RANGE_ARRAY
);

-- add one NODATA value
PROCEDURE addNODATA
(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   layerNumber   IN     NUMBER,
   NODATA        IN     NUMBER
);

-- add multiple NODATA values or value ranges
PROCEDURE addNODATA(
   georaster     IN OUT MDSYS.SDO_GEORASTER,
   layerNumber   IN     NUMBER,
   NODATA        IN     MDSYS.SDO_RANGE_ARRAY
);

-- retrieve all NODATA values and value ranges
FUNCTION getNODATA
(
   georaster     IN     MDSYS.SDO_GEORASTER,
   layerNumber   IN     NUMBER
) RETURN MDSYS.SDO_RANGE_ARRAY DETERMINISTIC  PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
--           Bitmap Mask Support
-- ---------------------------------------------------------------------

--
-- NAME:
--   setBitmapMask
--
-- DESCRIPTION
--   Attach, replace or remove a bitmap mask to a given GeoRaster layer
--
-- ARGUMENTS
--   georaster   - the GeoRaster object to be operated on
--   layerNumber - an object layer (0) or a subLayer (>0)
--   mask        - the 1BIT bitmap mask
--   NODATAMask  - spacifies whether the mask is going to be a NODATA mask
--
-- NOTES
--   If the GeoRaster object is NULL, nothing is to be done.
--   If the mask is NULL, any exisiting bitmap mask associated with the given
--   layer is removed.
--   If the mask is not a 1BIT GeoRaster object or has wrong dimension size,
--   error ORA-13470 is raised.
--   If the mask is not NULL or invalid, any exisiting bitmap mask associated
--   with the given layer is replaced with the new one.
--

PROCEDURE setBitmapMask
(
   georaster   IN OUT MDSYS.SDO_GEORASTER,
   layerNumber IN     NUMBER,
   mask        IN     MDSYS.SDO_GEORASTER,
   NODATAMask  IN     VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--   getBitmapMask
--
-- DESCRIPTION
--   Retrieve a bitmap mask as a standalone GeoRaster object
--
-- ARGUMENTS
--   georaster    - the source GeoRaster object
--   layerNumber  - an object layer (0) or a subLayer (>0)
--   storageParam - the storage parameter for the output GeoRaster object
--   mask         - the result GeoRaster object
--
-- NOTES
--   If the source GeoRaster object is NULL or there is no bitmap mask
--   associated with the given layer, a NULL GeoRaster object is returned.
--
PROCEDURE getBitmapMask
(
   georaster    IN     MDSYS.SDO_GEORASTER,
   layerNumber  IN     NUMBER,
   storageParam IN     VARCHAR2,
   mask         IN OUT MDSYS.SDO_GEORASTER
);

--
-- NAME:
--   getBitmapMaskValue
--
-- DESCRIPTION
--   Retrieve a cell value of a bitmap mask
--
-- ARGUMENTS
--   georaster    - the source GeoRaster object
--   layerNumber  - an object layer (0) or a subLayer (>0)
--   pyramidLevel - the pyramid level
--   rowNumber    - row number in cell space
--   colNumber    - column number in cell space
--   ptGeom       - a point geometry in either cell space
--                  or model space
-- RETURNS
--   Number
--
-- NOTES
--   If the source GeoRaster object is NULL or there is no bitmap mask
--   associated with the specfied raster layer, NULL is returned.
--   If the point position is out of scope, ORA-13415 is raised.
--
FUNCTION getBitmapMaskValue
(
   georaster    IN    MDSYS.SDO_GEORASTER,
   layerNumber  IN    NUMBER,
   pyramidLevel IN    NUMBER,
   rowNumber    IN    NUMBER,
   colNumber    IN    NUMBER
) RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

FUNCTION getBitmapMaskValue
(
   georaster    IN    MDSYS.SDO_GEORASTER,
   layerNumber  IN    NUMBER,
   pyramidLevel IN    NUMBER,
   ptGeom       IN    MDSYS.SDO_GEOMETRY
) RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

--
-- NAME:
--   getBitmapMaskSubset
--
-- DESCRIPTION
--   Retrieve a subset of a bitmap mask
--
-- ARGUMENTS
--   georaster    - the source GeoRaster object
--   layerNumber  - an object layer (0) or a subLayer (>0)
--   pyramidLevel - the pyramid level
--   window/inWindow - the area of interest
--   rasterBlob   - the BLOB holding the output
--   outWindow    - the actual output window
--   storageParam - storage parameters for the output
--
-- NOTES
--   If the source GeoRaster object is NULL or there is no bitmap mask
--   associated with the specfied raster layer, the result BLOB is empty.
--   If not specified, the storageParm for the output follows that of
--   the source GeoRaster object.
--
PROCEDURE getBitmapMaskSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   layerNumber    IN     NUMBER,
   pyramidLevel   IN     NUMBER,
   window         IN     MDSYS.SDO_NUMBER_ARRAY,
   rasterBlob     IN OUT NOCOPY BLOB,
   storageParam   IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE getBitmapMaskSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   layerNumber    IN     NUMBER,
   pyramidLevel   IN     NUMBER,
   window         IN     MDSYS.SDO_GEOMETRY,
   rasterBlob     IN OUT NOCOPY BLOB,
   storageParam   IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE getBitmapMaskSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   layerNumber    IN     NUMBER,
   pyramidLevel   IN     NUMBER,
   inWindow       IN     MDSYS.SDO_NUMBER_ARRAY,
   rasterBlob     IN OUT NOCOPY BLOB,
   outWindow      OUT    MDSYS.SDO_NUMBER_ARRAY,
   storageParam   IN     VARCHAR2 DEFAULT NULL
);

PROCEDURE getBitmapMaskSubset
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   layerNumber    IN     NUMBER,
   pyramidLevel   IN     NUMBER,
   inWindow       IN     MDSYS.SDO_GEOMETRY,
   rasterBlob     IN OUT NOCOPY BLOB,
   outWindow      OUT    MDSYS.SDO_NUMBER_ARRAY,
   storageParam   IN     VARCHAR2 DEFAULT NULL
);

--
-- NAME:
--   hasBitmapMask
--
-- DESCRIPTION
--   Check whether a bitmap map exists for the specified GeoRaster layer
--
-- ARGUMENTS
--   georaster    - the source GeoRaster object
--   layerNumber  - an object layer (0) or a subLayer (>0)
--
-- RETURN
--   'NULL' if the GeoRaster object is NULL
--   'TRUE' if a bitmap mask exists
--   'FALSE' otherwise
--
FUNCTION hasBitmapMask
(
   georaster      IN     MDSYS.SDO_GEORASTER,
   layerNumber    IN     NUMBER
) RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

--
-- NAME:
--   hasNODATAMask
--
-- DESCRIPTION
--   Check whether a NODATA bitmap map exists for the specified GeoRaster layer
--
-- ARGUMENTS
--   georaster    - the source GeoRaster object
--   layerNumber  - an object layer (0) or a subLayer (>0)
--
-- RETURN
--   'NULL' if the GeoRaster object is NULL
--   'TRUE' if a bitmap mask exists and is a NODATA mask
--   'FALSE' otherwise
--
FUNCTION hasNODATAMask
(
    georaster     IN     MDSYS.SDO_GEORASTER,
    layerNumber   IN     NUMBER
) RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
--           Validate GeoRaster Objects
-- ---------------------------------------------------------------------

--
-- NAME:
--      validateGeoRaster
--
-- DESCRIPTION
--      validate a GeoRaster object
--
-- ARGUMENTS
--      georaster    - The SDO_GEORASTER object to be validated
--
-- RETURNS
--      1. 'TRUE' if the georaster is valid
--      2. 'NULL' if the georaster object is null
--      3. A Oracle standard error code for known errors
--      4. 'FALSE' for unknown errors
--
FUNCTION validateGeoRaster
(
   georaster  IN MDSYS.SDO_GEORASTER
)
return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


--
-- NAME:
--      schemaValidate
--
-- DESCRIPTION
--      Validate GeoRaster Metadata against the GeoRaster XML Schema
--      It explains the reasons if the metadata is not valid
--
-- ARGUMENTS
--      georaster - The SDO_GEORASTER object of which the metadata is
--                  to be validated
--
-- RETURNS
--      1. 'TRUE' if the georaster is valid against XML schema
--      2. 'NULL' if the georaster or its metadata is null
--      3. Otherwise it prints Oracle standard error codes for
--         detailed errors and stop directly
--
FUNCTION schemaValidate
(
   georaster  IN MDSYS.SDO_GEORASTER
)
return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
-- import from and export to other raster sources/formats:
--   local FILE, HTTP file, ORDImage, BLOB, BFILE, GeoHeader files, etc
-- ---------------------------------------------------------------------

--
-- NAME:
--      importFrom procedures
--
-- DESCRIPTION
--      import from FILE, HTTP into a GeoRaster object
--
-- ARGUMENTS
--      sourceFormat   - source format, such as JPEG, TIFF, etc.
--      sourceType     - FILE or HTTP, etc.
--      sourceName     - source file name with fullpath
--      storageParam   - the GeoRaster object storage formats
--      georaster      - original and result GeoRaster object
--                       It must be an existing GeoRaster object
--      r_sourceFormat - raster sourceFormat as above
--      r_sourceType   - raster sourceType as above
--      r_sourceName   - raster sourceName as above
--      r_sourceBlob   - Blob holding source image file
--      h_sourceFormat - geo-header sourceFormat as above
--      h_sourceType   - geo-header sourceType as above
--      h_sourceName   - geo-header sourceName as above
--      h_sourceClob   - Clob holding source geo-header file
--
-- RETURNS
--      sdo_georaster object
--

PROCEDURE importFrom
(
   georaster      IN OUT MDSYS.SDO_GEORASTER,
   storageParam   IN     VARCHAR2,
   r_sourceFormat IN     VARCHAR2,
   r_sourceType   IN     VARCHAR2,
   r_sourceName   IN     VARCHAR2,
   h_sourceFormat IN     VARCHAR2 default null,
   h_sourceType   IN     VARCHAR2 default null,
   h_sourceName   IN     VARCHAR2 default null
);

PROCEDURE importFrom
(
   georaster      IN OUT MDSYS.SDO_GEORASTER,
   storageParam   IN     VARCHAR2,
   r_sourceFormat IN     VARCHAR2,
   r_sourceBlob   IN     BLOB,
   h_sourceFormat IN     VARCHAR2 default null,
   h_sourceClob   IN     CLOB default null
);

--
-- NAME:
--      exportTo procedures
--
-- DESCRIPTION
--      export GeoRaster object to other types
--
-- ARGUMENTS
--      destFormat   - dest format, such as JPEG, TIFF, etc.
--      destType     - FILE, etc.
--      destName     - dest file name with fullpath
--      subsetParam  - subset/crop parameters for output a subset
--      georaster    - the GeoRaster object to be exported
--      r_destFormat - raster destFormat as above
--      r_destType   - raster destType as above
--      r_destName   - raster destName as above
--      r_destBlob   - Blob to hold the output image file
--      h_destFormat - geo-header destFormat as above
--      h_destType   - geo-header destType as above
--      h_destName   - geo-header destName as above
--      h_destClob   - Clob to hold the output geo-header file
--
-- RETURNS
--

PROCEDURE exportTo
(
   georaster      IN  MDSYS.SDO_GEORASTER,
   subsetParam    IN  VARCHAR2,
   r_destFormat   IN  VARCHAR2,
   r_destType     IN  VARCHAR2,
   r_destName     IN  VARCHAR2,
   h_destFormat   IN  VARCHAR2 default null,
   h_destType     IN  VARCHAR2 default null,
   h_destName     IN  VARCHAR2 default null
);

PROCEDURE exportTo
(
   georaster      IN      MDSYS.SDO_GEORASTER,
   subsetParam    IN      VARCHAR2,
   destFormat     IN      VARCHAR2,
   destBlob       IN  OUT NOCOPY BLOB
);

PROCEDURE exportTo
(
   georaster      IN      MDSYS.SDO_GEORASTER,
   subsetParam    IN      VARCHAR2,
   r_destFormat   IN      VARCHAR2,
   r_destBlob     IN  OUT NOCOPY BLOB,
   h_destFormat   IN      VARCHAR2,
   h_destClob     IN  OUT NOCOPY CLOB
);

--
-- NAME:
--      getGeoreferenceType  function
--
-- DESCRIPTION
--      retrieve GeoReference type for the specified
--      GeoRaster object, so far, only three types are
--      supported by this function as below, for all other
--      types, UNKNOWN will return
--
--              1    --    Unknown
--              2    --    Affine Transform
--              3    --    Direct Linear Transform (DLT)
--              4    --    Rational Polynomial Coefficient (RPC)
--
-- ARGUMENTS
--      georaster   - geoRaster object
--
-- RETURNS
--      an integer to represent GeoReference type
FUNCTION getGeoreferenceType
(
  georaster    IN   SDO_GEORASTER
)
return number;

--
-- NAME:
--      isLossless
--
-- DESCRIPTION
--      Returns a string value that indicates whether or not the GeoRaster
--      object is compressed losslessly or lossily, or not compressed at all.
--
-- ARGUMENTS
--      georaster - The SDO_GEORASTER object of which losslessness is to be
--      determined.
--
-- RETURNS
--      1. 'TRUE' if GeoRaster object is losslessly compressed
--      2. 'NULL' if the georaster or its metadata is null, or if the
--         GeoRaster object is not compressed.
--      3. 'FALSE' if GeoRaster object is lossily compressed
--
FUNCTION isLossless
(
  geor    IN   SDO_GEORASTER
)
return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
--    Reproject GeoRaster Objects
--
-- ---------------------------------------------------------------------

--
-- NAME:
--      reproject
--
-- DESCRIPTION
--      Re-project a geo-referenced GeoRaster object into a new projection
--      defined by SRID which is different from the current projection.
--
--      NOTE:
--         (1) The input GeoRaster object should have a supported projection;
--         (2) If not georeferenced, use setSRS to setup if the projection
--             was known;
--         (3) The new projection should be valid and different from current one.
--
-- ARGUMENTS
--      inGeoRaster    - The source SDO_GEORASTER object to be re-projected
--      pyramidLevel   - The pyramid level of the source GeoRaster object
--      cropArea       - The crop area to be re-projected. If it is of
--                     - SDO_GEOMETRY type, it can be in any coordinate space.
--                       If it is of SDO_NUMBER_ARRAY, it is in cell space only
--      layerNumbers   - The logical layers
--      bandNumbers    - The physical bands in cell space
--      resampleParam  - The resampling method
--      storageParam   - A string specifying storage parameters for the result.
--                       The details of the parameters are explained in
--                       GeoRaster Manual
--      outSRID        - The SRID defines the new projection
--      outGeoRaster   - The re-projected persistent GeoRaster Object
--      rasterBlob     - The re-projected transient GeoRaster Object
--      outArea        - The re-projected transient GeoRaster Object extent
--                       in model coordinates.
--      outWindow      - The re-projected transient GeoRaster Object extent
--                       in cell coordinates.
--      bgValues       - background values for filling empty pixels
--                       one for each band
--
-- RETURNS
--      In the outGeoRaster is the re-projected GeoRaster object

-- For the persistent reprojection, the new GeoRatser is stored in table
PROCEDURE reproject (
     inGeoraster       IN SDO_GEORASTER,
     resampleParam     IN VARCHAR2,
     storageParam      IN VARCHAR2,
     outSRID           IN NUMBER,
     outGeoraster      IN OUT SDO_GEORASTER,
     bgValues          IN SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE reproject (
     inGeoraster       IN SDO_GEORASTER,
     pyramidLevel      IN NUMBER,
     cropArea          IN SDO_GEOMETRY,
     layerNumbers      IN VARCHAR2,
     resampleParam     IN VARCHAR2,
     storageParam      IN VARCHAR2,
     outSRID           IN NUMBER,
     outGeoraster      IN OUT SDO_GEORASTER,
     bgValues          IN SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE reproject (
     inGeoraster       IN SDO_GEORASTER,
     pyramidLevel      IN NUMBER,
     cropArea          IN SDO_NUMBER_ARRAY,
     bandNumbers       IN VARCHAR2,
     resampleParam    IN VARCHAR2,
     storageParam      IN VARCHAR2,
     outSRID           IN NUMBER,
     outGeoraster      IN OUT SDO_GEORASTER,
     bgValues          IN SDO_NUMBER_ARRAY  DEFAULT NULL
);

-- For the transient reprojection (on the fly),
-- the new GeoRatser is stored as BLOB
PROCEDURE reproject (
     inGeoraster       IN SDO_GEORASTER,
     pyramidLevel      IN NUMBER,
     cropArea          IN SDO_GEOMETRY,
     layerNumbers      IN VARCHAR2,
     resampleParam     IN VARCHAR2,
     storageParam      IN VARCHAR2,
     outSRID           IN NUMBER,
     rasterBlob        IN OUT NOCOPY BLOB,
     outArea           OUT SDO_GEOMETRY,
     outWindow         OUT SDO_NUMBER_ARRAY,
     bgValues          IN SDO_NUMBER_ARRAY DEFAULT NULL
);

PROCEDURE reproject (
     inGeoraster       IN SDO_GEORASTER,
     pyramidLevel      IN NUMBER,
     cropArea          IN SDO_NUMBER_ARRAY,
     bandNumbers       IN VARCHAR2,
     resampleParam     IN VARCHAR2,
     storageParam      IN VARCHAR2,
     outSRID           IN NUMBER,
     rasterBlob        IN OUT NOCOPY BLOB,
     outArea           OUT SDO_GEOMETRY,
     outWindow         OUT SDO_NUMBER_ARRAY,
     bgValues          IN SDO_NUMBER_ARRAY  DEFAULT NULL
);


-- ---------------------------------------------------------------------
--    Georeference GeoRaster Using Control Points (GCPs)
--
-- ---------------------------------------------------------------------

--
-- NAME:
--      georeference
--
-- DESCRIPTION
--      This function will georeference an image using a set of
--      control points (GCPs).
--
-- ARGUMENTS
--
--      georaster      - The source SDO_GEORASTER object to be georeferenced
--      FFMethodType   - The (rational) polynomial function used as georeference model
--      nGCP           - Number of GCPs provided
--      GCPs           - The GCP collection. Each GCP represents a points pair, one on
--                       image and another in some coordinate system or another image.
--      gcpGeorefModel - A data structure contains FFMethodType, nGCP, GCPs,solutionAccuracy
--      storeGCP       - A flag indicate whether the GCPs should be stored in the georaster
--                       metadata.
--      SRID           - The GCP's model coordinates SRID
--      modelCoordLoc  - The location on cell for the corresponding model coords
--
-- RETURNS
--      The solution accuracy (rowRMS, colRMS, totalRMS) and the ground positionning
--      accuracy (xRMS, yRMS, (zRMS), totalModelRMS) if check points provided
--
Function georeference(
     inGeoraster      IN OUT SDO_GEORASTER,
     FFMethodType     IN VARCHAR2,
     nGCP             IN Number,
     GCPs             IN SDO_GEOR_GCP_COLLECTION,
     storeGCP         IN VARCHAR2 DEFAULT 'TRUE',
     SRID             IN Number DEFAULT NULL,
     modelCoordinateLocation  IN Number DEFAULT NULL
)Return MDSYS.SDO_NUMBER_ARRAY;        -- RMSs


Function georeference(
    inGeoraster       IN OUT SDO_GEORASTER,
    gcpGeorefModel    IN SDO_GEOR_GCPGEOREFTYPE,
    storeGCP          IN VARCHAR2 DEFAULT 'TRUE',
    SRID              IN Number DEFAULT NULL,
    modelCoordinateLocation  IN Number DEFAULT NULL
)Return MDSYS.SDO_NUMBER_ARRAY;        -- RMSs

-- For this function, the GCPs should be already stored in the metadata
--
Function georeference(
    inGeoraster      IN OUT SDO_GEORASTER,
    FFMethod         IN VARCHAR2 DEFAULT NULL,
    SRID              IN Number DEFAULT NULL,
    modelCoordinateLocation  IN Number DEFAULT NULL
)Return MDSYS.SDO_NUMBER_ARRAY;        -- RMSs

--
-- Georeference XML data manipulation functions
--
Function getGCPGeorefModel(
    inGeoraster       IN SDO_GEORASTER
)Return MDSYS.SDO_GEOR_GCPGEOREFTYPE;

PROCEDURE setGCPGeorefModel(
    inGeoraster       IN OUT SDO_GEORASTER,
    gcpGeorefModel    IN SDO_GEOR_GCPGEOREFTYPE
);


Function getGCPGeorefMethod(
    inGeoraster       IN SDO_GEORASTER
)Return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


PROCEDURE setGCPGeorefMethod(
    inGeoraster       IN OUT SDO_GEORASTER,
    georefMethod          IN VARCHAR2
);

Function getControlPoint(
    inGeoraster       IN SDO_GEORASTER,
    controlPointID    IN VARCHAR2
)Return SDO_GEOR_GCP DETERMINISTIC PARALLEL_ENABLE;

PROCEDURE setControlPoint(
    inGeoraster       IN OUT SDO_GEORASTER,
    controlPoint      IN SDO_GEOR_GCP
);

PROCEDURE deleteControlPoint(
    inGeoraster       IN OUT SDO_GEORASTER,
    controlPointID    IN VARCHAR2
);


END SDO_GEOR;
/


Prompt Synonym SDO_GEOR;
--
-- SDO_GEOR  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_GEOR FOR MDSYS.SDO_GEOR
/


Prompt Grants on PACKAGE SDO_GEOR TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_GEOR TO PUBLIC
/
