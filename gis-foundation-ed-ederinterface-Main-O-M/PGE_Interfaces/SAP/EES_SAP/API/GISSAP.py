import arcpy
import json
import os
import datetime
import logging
import sys

def get_GIS_Globals(pmOrderNumber,orderType):

    #Set logging
    logging.basicConfig(level=logging.INFO,filename='GISSAP.log', filemode='a', format='%(name)s - %(levelname)s - %(message)s')
    logging.info('Process started at: ' +str(datetime.datetime.now()))
    # Esri start of added variables
    g_Insulation_lyr = u'Insulation_Area_Lyr'
    g_Corrosion_lyr = u'Corrosion_Area_Lyr'
    g_ElecSnow_lyr = u'ElecSnowloadingArea_Lyr'
    g_HFTD_lyr = u'HighFireThreatDis_Lyr'
    g_CDFSRA_lyr = u'CDF_SRA_Lyr'
    g_RCZ_lyr = u'RCZ_Lyr'
    g_SurgeProtection_lyr = u'SurgeProtectionDistricts_lyr'
    g_POH_lyr = u'in_memory\\Primary_OH_Lyr'
    g_PUG_lyr = u'Primary_UG_Lyr'
    g_ETOH_lyr = u'OH_LineSegment_Lyr'
    g_ETUG_lyr = u'UG_LineSegment_Lyr'
    # Esri end of added variables



    #Input Parameters for GP service
    totalTime = datetime.datetime.now()
    scriptStart = datetime.datetime.now()

    pmOrderNum = pmOrderNumber
    systemType = orderType
    logging.info('Requesting Data of Order Number: '+ str(pmOrderNum) +'; System Type: '+str(systemType))
    
    
    wipEDSdePath = r"D:\api\connections\Connection to WIP.sde"
    EDSdePath = r"D:\api\connections\gm_target_edgis_gmed1q.sde"
    wipETSdePath = r"D:\api\connections\Connection to ETGIS.sde"
    landbaseSdePath = r"D:\api\connections\Connection to lbgis1q.sde"


    arcpy.AddMessage("Reading Parameter")
    arcpy.AddMessage("Reading Parameter Time: "+str(datetime.datetime.now() - scriptStart))
    wipDataList=[]

    # Quering Data from WEBR.WIP Feature Class
    scriptStart = datetime.datetime.now()
    try:
        if systemType == "ED":
            wipSdeinfoPath=os.path.join(wipEDSdePath,"WEBR.WIP")
            fields = ['INSTALLJOBNUMBER','SHAPE']
            query = "INSTALLJOBNUMBER = " + "'" + pmOrderNum + "'"
            arcpy.MakeFeatureLayer_management(os.path.join(EDSdePath,'EDGIS.PriOHConductor'), g_POH_lyr)
            arcpy.MakeFeatureLayer_management(os.path.join(EDSdePath,'EDGIS.PriUGConductor'), g_PUG_lyr)
        elif systemType == "ET":
            wipSdeinfoPath=os.path.join(wipETSdePath,"ETGIS.T_WIPCLOUD")
            fields = ['PM_ORDER_NO','SHAPE']
            query = "PM_ORDER_NO = " + "'" + pmOrderNum + "'"
            arcpy.MakeFeatureLayer_management(os.path.join(wipETSdePath,'ETGIS.T_OHLINESEGMENT'), g_ETOH_lyr)
            arcpy.MakeFeatureLayer_management(os.path.join(wipETSdePath,'ETGIS.T_UGLINESEGMENT'), g_ETUG_lyr)
            
        arcpy.Delete_management('wipfc')
        wipCount=0
        arcpy.MakeFeatureLayer_management(wipSdeinfoPath,"wipfc", query )
        with arcpy.da.SearchCursor(wipSdeinfoPath, fields,query) as cursor:
            for row in cursor:
                wipDataList.append(row)
                wipCount += 1
        arcpy.AddMessage("Total Wip Polygon Found: "+str(wipCount))
    except Exception as e:
        arcpy.AddMessage("Failed To Execute. Error in Creating Connection. Error:"+ str(e))
        sys.exit()

    arcpy.AddMessage("WIP data received from WIP feat class")

    arcpy.AddMessage("Time Taken to Get WIP Info: "+str(datetime.datetime.now() - scriptStart))

    # Get high fire threat area
    scriptStart = datetime.datetime.now()

    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.HighFireThreatDist'), 'HighFireThreatDist_lyr')
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.InsulationAreas'), 'InsulationAreas_lyr')
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.ElecCorrosionArea'), g_Corrosion_lyr)
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.ElecSnowloadingArea'), g_ElecSnow_lyr)
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.CDF_SRA'), g_CDFSRA_lyr)
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.PGE_RCZ'), g_RCZ_lyr)
    arcpy.MakeFeatureLayer_management(os.path.join(landbaseSdePath,'LBGIS.SurgeProtectionDistricts'), g_SurgeProtection_lyr)


    arcpy.AddMessage("Time Taken to Fetch Layers: "+str(datetime.datetime.now() - scriptStart))

    scriptStart = datetime.datetime.now()
    HighFireThreatDict={}
    fieldsDis = ['CPUC_FIRE_MAP']

    pOP_Voltage= []
    tLine_no = []

    highFireThreatArea = {}
    hftd_value = ""
    InsulationDistrictCode = {}
    Insulation_Area = ""
    CorrosionDistrict = {}
    Corrosion_District = ""
    LoadingDistrict = {}
    Loading_District=""
    CDFSRA = {}
    CDF_SRA = ""
    RaptorArea = "No"
    LADistrict = {}
    LAD = ""
    count=0
    #flag=0
    try:
        for index in range(len(wipDataList)):
            try:
                if(systemType == "ED"):
                    arcpy.SelectLayerByLocation_management(g_POH_lyr, 'intersect', 'wipfc')    
                    with arcpy.da.SearchCursor(g_POH_lyr, ['OPERATINGVOLTAGE']) as cursor:
                        for row in cursor:
                            pOP_Voltage.append(row[0])
                            
                    arcpy.SelectLayerByLocation_management(g_PUG_lyr, 'intersect', 'wipfc')    
                    with arcpy.da.SearchCursor(g_PUG_lyr, ['OPERATINGVOLTAGE']) as cursor:
                        for row in cursor:
                            pOP_Voltage.append(row[0])
                elif(systemType == "ET"):
                    arcpy.SelectLayerByLocation_management(g_ETOH_lyr, 'intersect', 'wipfc')    
                    with arcpy.da.SearchCursor(g_ETOH_lyr, ['NOMINAL_VOLTAGE','TLINE_NO']) as cursor:
                        for row in cursor:
                            pOP_Voltage.append(str(row[0]))
                            tLine_no.append(str(row[1]))
                            
                    arcpy.SelectLayerByLocation_management(g_ETUG_lyr, 'intersect', 'wipfc')    
                    with arcpy.da.SearchCursor(g_ETUG_lyr, ['NOMINAL_VOLTAGE','TLINE_NO']) as cursor:
                        for row in cursor:
                            pOP_Voltage.append(str(row[0]))
                            tLine_no.append(str(row[1]))
            except:
                arcpy.AddMessage("Could not obtain Primary Operating Voltage!!!")
                
            arcpy.SelectLayerByLocation_management('HighFireThreatDist_lyr', 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor("HighFireThreatDist_lyr", fieldsDis) as cursor:
                for row in cursor:
                    highFireThreatArea[row[0]] = row[0]
            
            arcpy.SelectLayerByLocation_management('InsulationAreas_lyr', 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor("InsulationAreas_lyr", ['CODE']) as cursor:
                for row in cursor:
                    InsulationDistrictCode[row[0]]=row[0]
                    

            arcpy.SelectLayerByLocation_management(g_Corrosion_lyr, 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor(g_Corrosion_lyr, ['CORROSION']) as cursor:
                for row in cursor:
                    CorrosionDistrict[row[0]] = row[0]

            arcpy.SelectLayerByLocation_management(g_ElecSnow_lyr, 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor(g_ElecSnow_lyr, ['SNOWLOAD']) as cursor:
                for row in cursor:
                    LoadingDistrict[row[0]] = row[0]

            arcpy.SelectLayerByLocation_management(g_CDFSRA_lyr, 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor(g_CDFSRA_lyr, ['SRA']) as cursor:
                for row in cursor:
                    CDFSRA[row[0]] = row[0]
            
            arcpy.SelectLayerByLocation_management(g_RCZ_lyr, 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor(g_RCZ_lyr, ['ZONE']) as cursor:
                for row in cursor:
                    count+=1
            if count >0:
                RaptorArea = "Yes"
            else:
                RatorArea = "No"

            arcpy.SelectLayerByLocation_management(g_SurgeProtection_lyr, 'intersect', 'wipfc')    
            with arcpy.da.SearchCursor(g_SurgeProtection_lyr, ['DISTRICT_NUMBER']) as cursor:
                for row in cursor:
                    LADistrict[row[0]] = row[0]
     
    except:
        arcpy.AddMessage("Error Occured in Fetching Data")
    #Calculating HFTD
    logging.info("highFireThreatArea: "+str(highFireThreatArea))
    if "Tier 3" in highFireThreatArea:
        hftd_value = "Tier 3"
    elif "Tier 2" in highFireThreatArea:
        hftd_value = "Tier 2"
    elif "Tier 1" in highFireThreatArea:
        hftd_value = "Tier 1"
    else:
        hftd_value = "Null"
    #Calculating Insulation District
    logging.info("InsulationDistrictCode: "+str(InsulationDistrictCode))
    if "AA" in InsulationDistrictCode:
        Insulation_Area = "AA"
    elif "A" in InsulationDistrictCode:
        Insulation_Area = "A"
    elif "B" in InsulationDistrictCode:
        Insulation_Area = "B"
    elif "C" in InsulationDistrictCode:
        Insulation_Area = "C"
    elif "D" in InsulationDistrictCode:
        Insulation_Area = "D"
    else:
        Insulation_Area = "Null"

    #Calculating Corrosion District
    logging.info("CorrosionDistrict: "+str(CorrosionDistrict))
    if "severe" in CorrosionDistrict:
        Corrosion_District = "severe"
    elif "moderate" in CorrosionDistrict:
        Corrosion_District = "moderate"
    else:
        Corrosion_District = "Non-Corrosion Area"

     #Calculating Loading District
    logging.info("LoadingDistrict: "+str(LoadingDistrict))
    if "Heavy" in LoadingDistrict:
        Loading_District = "Heavy"
    elif "Intermediate" in LoadingDistrict:
        Loading_District = "Intermediate"
    elif "Light" in LoadingDistrict:
        Loading_District = "Light"
    else:
        Loading_District = "Null"

    #Calculating CDF_SRA
    logging.info("CDFSRA: "+str(CDFSRA))
    if "FRA" in CDFSRA or "SRA" in CDFSRA:
        CDF_SRA = "Yes"
    elif "LRA" in CDFSRA:
        CDF_SRA = "No"
        hftd_value = "Null"
    else:
        CDF_SRA = "Null"

    #Calculating LA District
    logging.info("LADistrict: "+str(LADistrict))
    if "District 1" in LADistrict:
        LAD = "District 1"
    elif "District 2" in LADistrict:
        LAD = "District 2"
    elif "District 3" in LADistrict:
        LAD = "District 3"
    else:
        LAD = "Null"
    pOP_Voltage_Desc = []
    tLine_unique = []
    #Fetching Domain Description for Primary Operating Voltage
    try:
        if(systemType== "ED"):
            cvdTable = arcpy.DomainToTable_management(EDSdePath, 'Primary Voltage','in_memory/cvdTable', 'codeField', 'descriptionField')
            rows = arcpy.SearchCursor(cvdTable)
            domain={}
            for row in rows:  
                domain[row.codeField] = row.descriptionField
            #Cleanup
            del row
            del rows
            
            for voltage in pOP_Voltage:
                if domain[voltage] not in pOP_Voltage_Desc:
                    pOP_Voltage_Desc.append(str(domain[voltage]))

            del pOP_Voltage
            del domain
        elif(systemType=="ET"):
            cvdTable = arcpy.DomainToTable_management(wipETSdePath, 'Transmission Voltage','in_memory/cvdTable', 'codeField', 'descriptionField')
            rows = arcpy.SearchCursor(cvdTable)
            domain={}
            for row in rows:  
                domain[row.codeField] = row.descriptionField
            #Cleanup
            del row
            del rows
            
            for voltage in pOP_Voltage:
                if domain[voltage] not in pOP_Voltage_Desc:
                    pOP_Voltage_Desc.append(str(domain[voltage]))
            for line_no in  tLine_no:
                if line_no not in tLine_unique:
                    tLine_unique.append(line_no)
                    
            #Cleanup
            del pOP_Voltage
            del domain
        
    except Exception as e:
        arcpy.AddMessage(str(e))
        
    #Creating Output Json
    #scriptStart = datetime.datetime.now()
    sapDataList=[]
	

    lbDict={}
    
    lbDict["Primary_Operating_Voltage"]=pOP_Voltage_Desc
    lbDict["Fire_Area"]=hftd_value
    lbDict["Insulation_District"]=Insulation_Area
    lbDict["Corrosion_District"]=Corrosion_District
    lbDict["Loading_District"]=Loading_District
    lbDict["CDF_SRA"]=CDF_SRA
    lbDict["Raptor_Area"]=RaptorArea
    lbDict["LA_District"]=LAD
    if(systemType=="ET"):
        lbDict["TLINE_NO"] = tLine_unique        

    sapDataList.append(lbDict)
                           
                                   
    DataForSAP={}
    #DataForSAP["GISDataset"]=sapDataList
    #GisInfoDataJson=json.dumps(DataForSAP)
                                   
    arcpy.AddMessage("Total Time: "+str(datetime.datetime.now() - totalTime))

    #Setting Output Parameter
    logging.info("Fina Ouput Data :"+str(sapDataList))
    arcpy.AddMessage(sapDataList)
    return sapDataList

if __name__ == "__main__":
    get_GIS_Globals('31207126','ED')
