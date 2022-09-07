## Make a SDE connection document with EDGIS username
## Rename the connection document to "Con_edscmt1t_edgis.sde" or modify the connection document name below.
#Import arcpy module
import arcpy, os, string
import utility_gis
import getPassword

def RemoveGpHistory_fd(sdeconn,remove_gp_history_xslt,out_xml):
    arcpy.ClearWorkspaceCache_management()
    print arcpy.ListDatasets()
    
    for fd in arcpy.ListDatasets():
        print arcpy.ListDatasets()
        arcpy.env.workspace = sdeconn + os.sep + fd
        for fc in arcpy.ListFeatureClasses():
            
            name_xml = out_xml + os.sep + str(fc) + ".xml"
            #Process: XSLT Transformation
            arcpy.XSLTransform_conversion(sdeconn + os.sep + fd + os.sep + fc, remove_gp_history_xslt, name_xml, "")
            print "Completed xml coversion on {0} {1}".format(fd,fc)
            # Process: Metadata Importer
            arcpy.MetadataImporter_conversion(name_xml,sdeconn + os.sep + fd + os.sep + fc)
            print "Imported XML on {0}".format(fc)
   
             
def RemoveGpHistory_fc(sdeconn,remove_gp_history_xslt,out_xml):
    arcpy.ClearWorkspaceCache_management()
    arcpy.env.workspace = sdeconn
    for fx in arcpy.ListFeatureClasses():
        
        name_xml = out_xml + os.sep + str(fx) + ".xml"
        #Process: XSLT Transformation
        arcpy.XSLTransform_conversion(sdeconn + os.sep + fx, remove_gp_history_xslt, name_xml, "")
        print "Completed xml coversion on {0}".format(fx)
        # Process: Metadata Importer
        arcpy.MetadataImporter_conversion(name_xml,sdeconn + os.sep + fx)
        print "Imported XML on {0}".format(fx)
   
    
if __name__== "__main__":
    
    # Local variables:
    DBType = 'edschm'
    strDbName     = utility_gis.getDbName(DBType)
    strUsername   = 'edgis'
    strPassword   = getPassword.getPassword(strDbName,strUsername)

    # -------------------------------------------------------------------------
    print 'Creating connection file...'
    # -------------------------------------------------------------------------
    print strPassword
    print strDbName
    sdeconn   = utility_gis.createArcSDEConnectionFileBasic('',strDbName,strUsername,strPassword)
    # -------------------------------------------------------------------------
    
    #sdeconn = "Database Connections\\Con_edgis2q_edgis.sde"
    
    arcpy.env.workspace = sdeconn
    remove_gp_history_xslt = "D:\\Program Files (x86)\\ArcGIS\\Desktop10.2\\Metadata\\Stylesheets\\gpTools\\remove geoprocessing history.xslt"
    out_xml = "D:\\XML_out"
    if not os.path.exists(out_xml):
     os.mkdir(out_xml)
    RemoveGpHistory_fd(sdeconn,remove_gp_history_xslt,out_xml)
    RemoveGpHistory_fc(sdeconn,remove_gp_history_xslt,out_xml)
