import arcpy
import arcgisscripting
import datetime

fc_transformer = u'TRANSFORMER'
fc_devicegroup = u'DEVICEGROUP'
fc_openpoint = u'OPENPOINT'

fieldname_livefrontidc = u'LIVEFRONTIDC'
fieldname_structureguid = u'STRUCTUREGUID'
fieldname_globalid = u'GLOBALID'
fieldname_subtypecd = u'SUBTYPECD'
livefrontidc_na_value = 'NA'

def useArcFM():
    from arcgisscripting import create
    from win32com.client import Dispatch
    gp = create(10.1)
    
    app = Dispatch('Miner.Framework.Dispatch.MMAppInitializeDispatch') 
    au = Dispatch('Miner.Framework.Dispatch.MMAutoupdaterDispatch')
    runtime = Dispatch('Miner.Framework.Dispatch.MMRuntimeEnvironmentDispatch')
    
    from enumerations import mmRuntimeMode,mmLicensedProductCode,mmAutoUpdaterMode  # contains enumeration constants for working with MMAppInitialize and MMAutoUpdater classes

    runtime.RuntimeMode = mmRuntimeMode.mmRuntimeModeArcServer
    app.Initialize(mmLicensedProductCode.mmLPArcFM) 
    au.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents
    
    livefrontidc()
    
    app.Shutdown
   
    del app, runtime, au

def find_NA_errors():
    updatecount = 0
    with open('c:/PGE_Reports/Transformer_LiveFrontIDC_NA.sql', 'wb') as logfile:
        # All transformers in Subtype 1 or 8 should have livefrontidc = 'NA'
        transformer_cursor = arcpy.SearchCursor(fc_transformer,
                                                "subtypecd in (1,8) and (livefrontidc != 'NA' or livefrontidc is null)",
                                                None,
                                                "GLOBALID,LIVEFRONTIDC")
        
        transformer_row = None
        for transformer_row in transformer_cursor:
            current_livefrontidc = transformer_row.getValue(fieldname_livefrontidc)
            current_globalid = transformer_row.getValue(fieldname_globalid)
            logfile.write("update edgis.transformer set livefrontidc = 'NA' where globalid = '{0}';\n".format(current_globalid))
            logfile.write("update edgis.A117 set livefrontidc = 'NA' where globalid = '{0}';\n".format(current_globalid))
            updatecount = updatecount + 1
            
        del transformer_row
        del transformer_cursor
    print "Found {0} Transformers that should have 'NA' value".format(updatecount)
    return updatecount

def find_NULL_errors():
    updatecount = 0
    totalcount = 0
    with open('c:/PGE_Reports/Transformer_LiveFrontIDC_NULL.sql', 'wb') as logfile:
        # Transformers in Subtype 2,3,4,7 should have livefrontidc = NULL instead of 'N'
        # if the OpenPoint in the same devicegroup is not subtype 1 or 2
        transformer_cursor = arcpy.SearchCursor(fc_transformer,
                                                "subtypecd in (2,3,4,7) and livefrontidc = 'N'",
                                                None,
                                                "GLOBALID,STRUCTUREGUID,LIVEFRONTIDC")
        
        transformer_row = None
        transformer_cache = {}
        structureguid_cache = []
        for transformer_row in transformer_cursor:            
            current_livefrontidc = transformer_row.getValue(fieldname_livefrontidc)
            current_structureguid = transformer_row.getValue(fieldname_structureguid)
            current_globalid = transformer_row.getValue(fieldname_globalid)
            transformer_cache[current_globalid] = (current_structureguid, current_livefrontidc)
            structureguid_cache.append(current_structureguid)
        del transformer_row
        del transformer_cursor
        
        xfmr_structureguid_query_helper = '('        
        transformer_mini_cache = {}
        transformer_ctr = 0
        for xfmr_globalid in transformer_cache.keys():
            current_structureguid,current_livefrontidc = transformer_cache[xfmr_globalid]
            transformer_mini_cache[xfmr_globalid] = (current_structureguid,current_livefrontidc)
            xfmr_structureguid_query_helper = xfmr_structureguid_query_helper + "'{0}',".format(current_structureguid)
            transformer_ctr = transformer_ctr + 1
            if (transformer_ctr % 1000 == 0 and transformer_ctr > 0) or transformer_ctr == len(structureguid_cache):
                xfmr_structureguid_query_helper = xfmr_structureguid_query_helper.rstrip(',') + ')'
                mini_updated,mini_processed = process_1000_xfmr(transformer_mini_cache, xfmr_structureguid_query_helper, logfile)
                updatecount = updatecount + mini_updated
                totalcount = totalcount + mini_processed
                transformer_mini_cache = {}
                xfmr_structureguid_query_helper = '('
    print 'Found {0} DeviceGroup-related Transformers that should have LiveFrontIDC=NULL'.format(updatecount, totalcount)
    return (updatecount,totalcount)
        

def process_1000_xfmr(transformer_cache, xfmr_structureguid_query_helper, logfile):
    devicegroup_cursor = arcpy.SearchCursor(fc_devicegroup,
                                            "globalid IN {0}".format(xfmr_structureguid_query_helper),
                                            None,
                                            "GLOBALID")
    devicegroup_row = None
    dg_query_helper = '('
    devicegroup_cache = set()
    for devicegroup_row in devicegroup_cursor:
        current_globalid = devicegroup_row.getValue(fieldname_globalid)
        devicegroup_cache.add(current_globalid)
        dg_query_helper = dg_query_helper + "'{0}',".format(current_globalid)
        
    del devicegroup_row
    del devicegroup_cursor
    dg_query_helper = dg_query_helper.rstrip(',') + ')'
    
    openpoint_cursor = arcpy.SearchCursor(fc_openpoint,
                                          "structureguid IN {0}".format(dg_query_helper),
                                          None,
                                          "GLOBALID,STRUCTUREGUID,SUBTYPECD")
    
    # There should be one or fewer openpoints
    openpoint_row = None
    openpoint_cache = {}
    for openpoint_row in openpoint_cursor:
        current_globalid = openpoint_row.getValue(fieldname_globalid)
        current_structureguid = openpoint_row.getValue(fieldname_structureguid)
        current_subtypecd = openpoint_row.getValue(fieldname_subtypecd)
        openpoint_cache[current_structureguid] = (current_globalid,current_subtypecd)
    del openpoint_row
    del openpoint_cursor
    
    noopenpoint_updatecount = 0
    openpoint_updatecount = 0
    totalcount = 0
    
    for xfmr_globalid in transformer_cache.keys():
        current_structureguid,current_livefrontidc = transformer_cache[xfmr_globalid]
        if current_structureguid in devicegroup_cache:
            if current_structureguid in openpoint_cache:
                current_openpoint_globalid,current_openpoint_subtypecd = openpoint_cache[current_structureguid]
                if current_openpoint_subtypecd != 1 and current_openpoint_subtypecd != 2:
                    logfile.write("update edgis.transformer set livefrontidc = NULL where globalid = '{0}';\n".format(xfmr_globalid))
                    logfile.write("update edgis.A117 set livefrontidc = NULL where globalid = '{0}';\n".format(xfmr_globalid))
                    openpoint_updatecount = openpoint_updatecount + 1
            else:
                logfile.write("update edgis.transformer set livefrontidc = NULL where globalid = '{0}';\n".format(xfmr_globalid))
                logfile.write("update edgis.A117 set livefrontidc = NULL where globalid = '{0}';\n".format(xfmr_globalid))
                noopenpoint_updatecount = noopenpoint_updatecount + 1
        totalcount = totalcount + 1        

    #print "Updated {0} noopen and {1} open out of {2} Transformers with NULL value".format(noopenpoint_updatecount, openpoint_updatecount, totalcount)
    return ((openpoint_updatecount + noopenpoint_updatecount),totalcount)

def livefrontidc():
    arcpy.env.workspace = 'Database Connections\EDGISM4Q_edgis_livefront.sde'
    find_NA_errors()
    find_NULL_errors()    

print 'Start time: {0}'.format(datetime.datetime.now())
useArcFM()
print 'End time: {0}'.format(datetime.datetime.now())
