import arcpy
import csv
import arcgisscripting
import time
from arcpy import env


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
    
    fixIJY()
    
    app.Shutdown
   
    del app, runtime, au

def addUpdateToFeatureClass(fc_to_guid, guid_to_upd, fc, guid, upd):
    
    current_guids = fc_to_guid.get(fc, None)
    if current_guids is None:
        fc_to_guid[fc] = [guid,]
    else:
        current_guids.append(guid)
        fc_to_guid[fc] = current_guids

    guid_to_upd[guid] = upd
    
    return fc_to_guid, guid_to_upd

def fixIJY():
    env.workspace = r"Database Connections/EDGISM4Q_edgis_ijy.sde"

    with open(r'ijy.log', 'wb') as logfile:
        print 'Beginning InstallJobYear Fixes at',time.strftime("%Y-%m-%d %H:%M:%S")
        with open(r'ijy.csv', 'rU') as inputfile:
            featureclasses_to_guids = {}
            guids_to_updates = {}
            
            csvreader = csv.reader(inputfile)
            rowcount = 0
            for csv_row in csvreader:
                rowcount = rowcount + 1
                (featureclass,
                globalid,
                installjobyear) = csv_row

                # Don't process the header
                if rowcount == 1:
                    continue                

                # Empty year means NULL
                if installjobyear.strip() == '':
                    installjobyear = None

                (featureclasses_to_guids,
                guids_to_updates) = addUpdateToFeatureClass(featureclasses_to_guids,
                                                           guids_to_updates,
                                                           featureclass,
                                                           globalid,
                                                           installjobyear)

            fcs = featureclasses_to_guids.keys()
            fcs.sort()
            
            for fc in fcs: #['NEUTRALCONDUCTOR','PRIMARYGENERATION']:
                print fc, len(featureclasses_to_guids[fc])
                globalid_sql = '({0})'

                globalid_lists = []
                globalid_current_list = ''
                current_count = 1
                for guid in featureclasses_to_guids[fc]:
                    globalid_current_list = globalid_current_list + "'{0}',".format(guid)
                    if current_count % 1000 == 0 or (current_count == len(featureclasses_to_guids[fc])):
                        globalid_current_list = globalid_current_list.rstrip(',')
                        globalid_lists.append(globalid_sql.format(globalid_current_list))
                        globalid_current_list = ''
                    current_count = current_count + 1

                for guid_list in globalid_lists:
                    cursor = arcpy.UpdateCursor('edgis.{0}'.format(fc),
                                                "globalid IN {0}".format(guid_list),
                                                None,
                                                "GLOBALID,INSTALLJOBYEAR")
                    cursor_row = cursor.next()
                    while cursor_row:
                        row_globalid = cursor_row.getValue('GLOBALID')
                        update_ijy = guids_to_updates[row_globalid]

                        error_flag = False
                        #print '{0},{1},{2}'.format(fc,row_globalid,update_ijy)
                        logfile.write('{0},{1},{2}'.format(fc,row_globalid,update_ijy))
                        cursor_row.setValue('INSTALLJOBYEAR', update_ijy)
                        try:
                            cursor.updateRow(cursor_row)
                        except RuntimeError, re:
                            error_flag = True
                            
                        if error_flag:
                            logfile.write(',fail\n')
                        else:
                            logfile.write(',success\n')
                        cursor_row = cursor.next()
                    del cursor_row
                    del cursor
                
        print 'Completed InstallJobYear Fixes at',time.strftime("%Y-%m-%d %H:%M:%S")

useArcFM()
