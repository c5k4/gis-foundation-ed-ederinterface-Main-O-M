import arcpy
import arcgisscripting
import datetime

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
    
    fixOpenPointOperatingNumbers()
    
    app.Shutdown
   
    del app, runtime, au

def fixOpenPointOperatingNumbers():
    arcpy.env.workspace = 'Database Connections\EDGISM4Q_edgis_test.sde'
    fc = u'OPENPOINT'
    field_opnum = u'OPERATINGNUMBER'
    current_opnum = ""

    cursor = arcpy.UpdateCursor(fc)
    #fields = arcpy.ListFields(fc)
    #print [field.name for field in fields]

    updatecount = 0
    totalcount = 0
    with open('c:/PGE_Reports/OpenPointABCD.csv', 'wb') as logfile:
        logfile.write('GlobalID,Old OperatingNumber,New OperatingNumber\n')
        for row in cursor:
            totalcount = totalcount + 1
            current_opnum = row.getValue(field_opnum)
            new_opnum = ""
            if current_opnum is None:
                continue
            lower_current_opnum = current_opnum.strip().lower()
            if lower_current_opnum.endswith('a') or lower_current_opnum.endswith('b') or lower_current_opnum.endswith('c') or lower_current_opnum.endswith('d'):
                new_opnum = current_opnum.rstrip('abcdABCD')
                logfile.write(row.getValue('GLOBALID') + ',' + current_opnum + ',' + new_opnum + '\n')
                row.setValue(field_opnum, new_opnum)
                cursor.updateRow(row)
                updatecount = updatecount + 1
            if updatecount >= 1150:
                break
        del row
        del cursor

    print 'Updated {0} out of {1}'.format(updatecount,totalcount)



print 'Start time: {0}'.format(datetime.datetime.now())
useArcFM()
print 'End time: {0}'.format(datetime.datetime.now())
