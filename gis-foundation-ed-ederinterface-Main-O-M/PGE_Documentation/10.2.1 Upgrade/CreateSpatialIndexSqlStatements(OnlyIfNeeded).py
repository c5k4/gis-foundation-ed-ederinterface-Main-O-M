import sys
import time
# Import arcpy module
import arcpy
from arcpy import env
import os

def main():
    # Local variables:
    DataConnection = sys.argv[1]
    EDGIS_All = DataConnection
    EDGIS_CommonFeaturesDataset = DataConnection + "\\EDGIS.CommonFeaturesDataset"
    EDGIS_ConversionFeaturesDataset = DataConnection + "\\EDGIS.ConversionFeaturesDataset"
    EDGIS_ElectricDataset = DataConnection + "\\EDGIS.ElectricDataset"
    EDGIS_PGEElectricLandbase = DataConnection + "\\EDGIS.PGEElectricLandbase"
    EDGIS_SubstationDataset = DataConnection + "\\EDGIS.SubstationDataset"
    EDGIS_UFMDataset = DataConnection + "\\EDGIS.UFMDataset"

    DataSources = [EDGIS_All, EDGIS_CommonFeaturesDataset, EDGIS_ConversionFeaturesDataset, EDGIS_ElectricDataset, EDGIS_PGEElectricLandbase, EDGIS_SubstationDataset, EDGIS_UFMDataset]

    count = 0
    f = open("CreateSpatialIndexes.sql", "w")

    for DataSource in DataSources:
        env.workspace = DataSource
        fcList = arcpy.ListFeatureClasses()
        for fc in fcList:
            start = time.clock()
            try:
                print fc + ":"
                print "Calculating default grid indices"
                result = arcpy.CalculateDefaultGridIndex_management(fc)
                grid1 = result.getOutput(0)
                grid2 = result.getOutput(1)
                grid3 = result.getOutput(2)
                try:
                    print "Deleting current spatial index"
                    arcpy.RemoveSpatialIndex_management(fc)
                except:
                    print "No spatial index to remove"
                try:
                    f.write("create index A" + str(count) + "_IX1 on " + fc + "(SHAPE) indextype is sde.st_spatial_index parameters('st_grids=" + grid1 + "," + grid2 + "," + grid3 + " st_srid=2');\n")
                    count = count + 1
                except:
                    print "Error writing spatial index: " + fc
            except:
                print "Error calculating grid index"
                print arcpy.GetMessages()
            elapsed = (time.clock() - start)
            print "Elapsed time: ", elapsed
            print ""
            print ""
    f.close()

if __name__ == '__main__': main()
