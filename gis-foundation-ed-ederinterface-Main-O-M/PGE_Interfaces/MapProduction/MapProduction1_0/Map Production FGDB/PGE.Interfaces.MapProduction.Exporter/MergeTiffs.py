##==================================
##Mosaic To New Raster
##Usage: MosaicToNewRaster_management inputs;inputs... output_location raster_dataset_name_with_extension {coordinate_system_for_the_raster} ##                                    8_BIT_UNSIGNED | 1_BIT | 2_BIT | 4_BIT | 8_BIT_SIGNED | 16_BIT_UNSIGNED | 16_BIT_SIGNED | 32_BIT_FLOAT
##                                    32_BIT_UNSIGNED | 32_BIT_SIGNED | | 64_BIT {cellsize} number_of_bands {MAXIMUM | FIRST | BLEND  | MEAN 
##                                    | MAXIMUM | MAXIMUM} {FIRST | REJECT | MAXIMUM | MATCH}                               
try:
    import arcpy
    import os

    baseDirectory = arcpy.GetParameterAsText(0)
    baseFilename = arcpy.GetParameterAsText(1)
    coordinateSystem = arcpy.GetParameterAsText(2)
    error = ""
    #baseDirectory = "c:\\testing"
    #baseFilename = "O22"
    #coordinateSystem = "2"
    ##Mosaic several TIFF images to a new TIFF image
    ##"C:\Program Files (x86)\ArcGIS\Desktop10.0\Coordinate Systems\Projected Coordinate Systems\State Plane\NAD 1927 (US Feet)\NAD 1927 StatePlane California V FIPS 0402.prj"

    arcpy.env.workspace = baseDirectory
    arcpy.env.compression = "'CCITT Group 4' 75"
    deleteBaseFileName = baseDirectory + "\\" + baseFilename

    print "Using BaseDirectory " + baseDirectory
    print "Using DeleteBaseFileName " + deleteBaseFileName
    

    if coordinateSystem == "1":
        coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_I_FIPS_0401',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-122.0],PARAMETER['Standard_Parallel_1',40.0],PARAMETER['Standard_Parallel_2',41.66666666666666],PARAMETER['Latitude_Of_Origin',39.33333333333334],UNIT['Foot_US',0.3048006096012192]]"
    elif coordinateSystem == "2":
        coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_II_FIPS_0402',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-122.0],PARAMETER['Standard_Parallel_1',38.33333333333334],PARAMETER['Standard_Parallel_2',39.83333333333334],PARAMETER['Latitude_Of_Origin',37.66666666666666],UNIT['Foot_US',0.3048006096012192]]"
    elif coordinateSystem == "3":
        coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_III_FIPS_0403',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-120.5],PARAMETER['Standard_Parallel_1',37.06666666666667],PARAMETER['Standard_Parallel_2',38.43333333333333],PARAMETER['Latitude_Of_Origin',36.5],UNIT['Foot_US',0.3048006096012192]]"
    elif coordinateSystem == "4":
        coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_IV_FIPS_0404',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-119.0],PARAMETER['Standard_Parallel_1',36.0],PARAMETER['Standard_Parallel_2',37.25],PARAMETER['Latitude_Of_Origin',35.33333333333334],UNIT['Foot_US',0.3048006096012192]]"
    elif coordinateSystem == "5":
        coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_V_FIPS_0405',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-118.0],PARAMETER['Standard_Parallel_1',34.03333333333333],PARAMETER['Standard_Parallel_2',35.46666666666667],PARAMETER['Latitude_Of_Origin',33.5],UNIT['Foot_US',0.3048006096012192]]"
    
    print "Using CoordinateSystem " + coordinateSystem
    
    ##Delete any current files that might be there before merging
    for x in range(0, 25):
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.aux.xml")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.ovr")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.xml")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tfw")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif")
        except:
            error = "No file to delete"

    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tfw")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.aux.xml")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.ovr")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.xml")
    except:
        error = "No file to delete"

    ##Perform the merges.  These have to be separated out due to what appears to be limitations in the Esri geoprocessor tool
    
    arcpy.MosaicToNewRaster_management(baseFilename + "_0.tif;" + baseFilename + "_1.tif;" + baseFilename + "_2.tif",baseDirectory, baseFilename + "_Merge0.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_3.tif;" + baseFilename + "_4.tif",baseDirectory, baseFilename + "_Merge1.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_5.tif;" + baseFilename + "_6.tif;" + baseFilename + "_7.tif",baseDirectory, baseFilename + "_Merge2.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_8.tif;" + baseFilename + "_9.tif",baseDirectory, baseFilename + "_Merge3.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_10.tif;" + baseFilename + "_11.tif;" + baseFilename + "_12.tif",baseDirectory, baseFilename + "_Merge4.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_13.tif;" + baseFilename + "_14.tif",baseDirectory, baseFilename + "_Merge5.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_15.tif;" + baseFilename + "_16.tif;" + baseFilename + "_17.tif",baseDirectory, baseFilename + "_Merge6.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_18.tif;" + baseFilename + "_19.tif",baseDirectory, baseFilename + "_Merge7.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_20.tif;" + baseFilename + "_21.tif;" + baseFilename + "_22.tif",baseDirectory, baseFilename + "_Merge8.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_23.tif;" + baseFilename + "_24.tif",baseDirectory, baseFilename + "_Merge9.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")

    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge0.tif;" + baseFilename + "_Merge1.tif",baseDirectory, baseFilename + "_Merge10.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge2.tif;" + baseFilename + "_Merge3.tif",baseDirectory, baseFilename + "_Merge11.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge4.tif;" + baseFilename + "_Merge5.tif",baseDirectory, baseFilename + "_Merge12.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge6.tif;" + baseFilename + "_Merge7.tif",baseDirectory, baseFilename + "_Merge13.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge8.tif;" + baseFilename + "_Merge9.tif",baseDirectory, baseFilename + "_Merge14.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")

    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge10.tif;" + baseFilename + "_Merge11.tif;" + baseFilename + "_Merge12.tif",baseDirectory, baseFilename + "_Merge15.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")
    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge13.tif;" + baseFilename + "_Merge14.tif",baseDirectory, baseFilename + "_Merge16.tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")

    arcpy.MosaicToNewRaster_management(baseFilename + "_Merge15.tif;" + baseFilename + "_Merge16.tif",baseDirectory, baseFilename + "_CompletedMerge" + ".tif", coordinateSystem, "1_BIT", "0", "1", "MAXIMUM","FIRST")



    ##Delete left over files created during the merge process
    for x in range(0, 25):
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.aux.xml")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.ovr")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif.xml")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tfw")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_Merge" + str(x) + ".tif")
        except:
            error = "No file to delete"

    for x in range(0, 25):
        try:
            os.remove(deleteBaseFileName + "_" + str(x) + ".tif")
        except:
            error = "No file to delete"
        try:
            os.remove(deleteBaseFileName + "_" + str(x) + ".tfw")
        except:
            error = "No file to delete"

    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.aux.xml")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.ovr")
    except:
        error = "No file to delete"
    try:
        os.remove(deleteBaseFileName + "_CompletedMerge" + ".tif.xml")
    except:
        error = "No file to delete"
    
except:
    print "Mosaic To New Raster example failed."
    print arcpy.GetMessages()
