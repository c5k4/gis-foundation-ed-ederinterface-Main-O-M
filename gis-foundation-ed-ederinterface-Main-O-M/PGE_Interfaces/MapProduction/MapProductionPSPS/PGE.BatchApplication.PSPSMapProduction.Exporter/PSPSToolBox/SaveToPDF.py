import arcpy
import os
import string

arcpy.env.overwriteOutput = True

def main():

    arcpy.AddMessage("  Collecting input parameters")

    # Get Initial Coordinates
    xMin = arcpy.GetParameterAsText(0)
    if xMin == '#' or not xMin:
        arcpy.AddError("Cannot proceed if xMin is not passed")

    yMin = arcpy.GetParameterAsText(1)
    if yMin == '#' or not yMin:
        arcpy.AddError("Cannot proceed if yMin is not passed")

    xMax = arcpy.GetParameterAsText(2)
    if xMax == '#' or not xMax:
        arcpy.AddError("Cannot proceed if xMax is not passed")

    yMax = arcpy.GetParameterAsText(3)
    if yMax == '#' or not yMax:
        arcpy.AddError("Cannot proceed if yMax is not passed")

    xMin = float(xMin)
    yMin = float(yMin)
    xMax = float(xMax)
    yMax = float(yMax)


    #Get Map Number
    outputName=arcpy.GetParameter(4)
    arcpy.AddMessage("Map Number obtained:"+outputName)

    # Get Layout Folder
    LayoutsFolderPath = arcpy.GetParameterAsText(5)
    if LayoutsFolderPath == '#' or not LayoutsFolderPath:
        arcpy.AddError("Cannot proceed if the Layout Folder is not passed")
        return 

    arcpy.AddMessage("Layout folder found"+LayoutsFolderPath)

    # Get template document
    Layouts = arcpy.GetParameterAsText(6)
    if Layouts == '#' or not Layouts:
        arcpy.AddError(Layouts + " does not exist. File not found:" + mxdName)
        return
    arcpy.AddMessage("MXD Name found:"+Layouts)
    LayoutArray=string.split(Layouts,";")

    # Get the requested scale
    mapScale = arcpy.GetParameterAsText(7)
    if mapScale == '#' or not mapScale:
        mapScale = 0 # provide a default value if unspecified
    mapScale = float(mapScale)
    
    # layers only or layers and attributes
    attributesFlag = arcpy.GetParameterAsText(8)
    if attributesFlag.lower() == 'true':
        layers_string = "LAYERS_AND_ATTRIBUTES"
    else:
        layers_string = "LAYERS_ONLY"

    # Get the requested DPI
    dpi = arcpy.GetParameterAsText(9)
    if dpi == '#' or not dpi:
        dpi = 300 # provide a default value if unspecified


    # Get pdf file path to create
    outputPDFPath = arcpy.GetParameterAsText(10)
    if outputPDFPath == '#' or not outputPDFPath:
        arcpy.AddError("Cannot proceed without specifying an output path for the pdf file")
        return


    cnt=0
    for Layout in LayoutArray:
        cnt=cnt+1
        arcpy.AddMessage(cnt)

        fileName=str(outputName)
        if LayoutArray.__len__() > 1: 
            fileName=str(outputName)+"_"+str(cnt)

        arcpy.AddMessage(fileName)
        
        mxdName = os.path.join(LayoutsFolderPath, Layout)

        arcpy.AddMessage("Modified MXDName"+mxdName)
    
        if not (os.path.exists(mxdName)):
            arcpy.AddError(Layout + " does not exist. File not found:" + mxdName)
            return

        mapDoc = arcpy.mapping.MapDocument(mxdName)

        # Build the extent in the Layout document
        dataFrame = arcpy.mapping.ListDataFrames(mapDoc)[0]

        # Set the scale if necessary
        if mapScale > 0:
            arcpy.AddMessage("  Setting scale to: " + str(mapScale))
            dataFrame.scale = mapScale
        
        arcpy.AddMessage("  Processing Map Extent")
        #dataFrame.extent = calculateExtent_Original(xMin,xMax,yMin,yMax,dataFrame,srIn)
        dataFrame.extent = arcpy.CreateObject('Extent', xMin, yMin, xMax, yMax)

        # Set the Map title
        #if title != " ":
        #    for elm in arcpy.mapping.ListLayoutElements(mapDoc, "TEXT_ELEMENT"):
        #        if elm.text == "Map Title":
        #           elm.text = title

        # Export PDF
        arcpy.AddMessage("  Saving as PDF")
        arcpy.mapping.ExportToPDF(mapDoc,os.path.join(outputPDFPath,fileName+".pdf"),resolution=dpi,layers_attributes=layers_string)

        del mapDoc
    #del srIn
    del dataFrame
    
def calculateExtent_Original(xMin,xMax,yMin,yMax,dataFrame,srIn):
    # Make input extent coordinates in the same sr as the data frame
    #pnt1 = arcpy.Point(xMin, yMin)
    #pnt2 = arcpy.Point(xMin, yMax)
    #pnt3 = arcpy.Point(xMax, yMax)
    #pnt4 = arcpy.Point(xMax, yMin)
    #ptArray = arcpy.Array()
    #ptArray.add(pnt1)
    #ptArray.add(pnt2)
    #ptArray.add(pnt3)
    #ptArray.add(pnt4)

    pnt=arcpy.Point()
    arcpy.AddMessage("SRIN"+srIn.name)
    arcpy.AddMessage("Creating in-memory featureclass")
    
    fc=arcpy.CreateFeatureclass_management("in_memory","extentPnts","POINT","", "DISABLED", "DISABLED", srIn)

    arcpy.AddMessage("adding points to in-memory featureclass")
    cur = arcpy.InsertCursor(fc)

    pnt.ID = 1
    pnt.X = xMin
    pnt.Y = yMin

    feat = cur.newRow()
    feat.shape = pnt
    cur.insertRow(feat)

    pnt.ID=2
    pnt.X=xMax
    pnt.Y=yMax

    feat = cur.newRow()
    feat.shape = pnt
    cur.insertRow(feat)

    arcpy.AddMessage("added points to in-memory featureclass")
    
    arcpy.AddMessage("searching polygon from in-memory featureclass")
    rows = arcpy.SearchCursor("in_memory\extentPnts", "", dataFrame.spatialReference,"","")

    arcpy.AddMessage("Searched in memory featureclass")

    
    rows.reset()
    row = rows.next()
    newPntLL = row.shape.getPart(0)
    #newpoly=row.shape

    row = rows.next()
    newPntUR = row.shape.getPart(0)

    #myExtent = arcpy.CreateObject('Extent', xMin, yMin, xMax, yMax)
    myExtent = arcpy.CreateObject('Extent', newPntLL.X,newPntLL.Y , newPntUR.X, newPntUR.Y)
    arcpy.AddMessage("New extent: %s %s %s %s" % (myExtent.XMin, myExtent.YMin, myExtent.XMax, myExtent.YMax))

    #myExtent=row.shape.Extent
    #myExtent=polygon.extent
    return myExtent


if __name__ == "__main__":
    main()    
