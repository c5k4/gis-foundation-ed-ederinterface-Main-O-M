#Import system modules
import arcpy

# !!!! point to sde file that is connected using EDGIS user (needed to create table in that schema) !!!!

# Change this value to the location of the sde file you are using.
Data_Connection = "Database Connections/PT1T_EDGIS.sde"

arcpy.env.workspace = Data_Connection
# schema and connection user must match to create table in correct location
schema = "EDGIS"

# moveanddelete - [[subtype from, code going from and deleting, subtype to, code going to]] - Only codes are updated, NOT subtypes
moveAndDelete = [[2,11,2,10],[2,14,2,13],[2,18,2,17],[3,25,3,24],[3,29,3,28],[3,31,3,30]]

#updatedesc - [[subtype,code, append desc]] - we're only appending currenlty
#    used in UpdateDomainDescription
updateDesc = [[2,9,'Obsolete Subsurface Cutouts'],[2,12,'Obsolete Subsurface Switch, Three way, Two Ways Switched'],
              [2,15,'Obsolete Subsurface Smart Switch'],[2,19,'Obsolete Subsurface Sectionalizer'],
              [2,26,'Obsolete TGRAL'],[2,27,'Obsolete TGRAM (Three Way)'],
              [2,28,'Obsolete TGRAM (Four Way)'],[2,30,'Obsolete 4kV Junction'],
              [2,34,'Obsolete TGRAM (Two Way)'],
              [3,1,'Obsolete PMH 3'],[3,2,'Obsolete PMH 4 w/1 fuse'],[3,3,'Obsolete PMH 4 w/2 fuses'],
              [3,4,'Obsolete PMH 4 w/3 fuses'],[3,5,'Obsolete PMH 5'],[3,6,'Obsolete PMH 6 w/(1) Hot Leg'],
              [3,7,'Obsolete PMH 9 w/(2) Hot Legs'],[3,8,'Obsolete PMH 11 w/(1) Hot Leg'],
              [3,9,'Obsolete PMH 11 w/(3) single phase Leg'],[3,10,'Obsolete PMH 6 w/(3) 1-phase Leg'],
              [3,11,'Obsolete PMH 9 w/(6)1-phase Leg'],[3,12,'Obsolete PMH 9 w/ (3) 1-phase & (1) 3-phase Leg'],
              [3,28,'Obsolete Padmounted Oil Switch'],
              [3,34,'Padmounted Automatic Circuit Recloser'],[3,35,'Obsolete Padmounted Sectionalizer']]

# subtypetodomain - matches subtypes to domain name
subtypetoDomain = [[2,'Device Group Type - Subsurface'],[3,'Device Group Type - Padmount']]

workspace = Data_Connection
# arcpy.env.workspace = workspace
    
def UpdateDomainDescription():

    """ Update domain description  
        ex: add Obsolete """
    for i in range(len(updateDesc)):
        
        subtype = str(updateDesc[i][0])
        code = str(updateDesc[i][1])
        printMessage("working on " + str(subtype) + " " + str(code))
        newstring = str(updateDesc[i][2])
        # get the matching domain
        domain = GetDomainBasedOnSubType(subtype)
        if domain is not None:
            # add will update an existing domain description
            printMessage("*** Updating Domain to " + newstring + " sub: " + str(subtype) + " code: " + str(code))
            arcpy.AddCodedValueToDomain_management(arcpy.env.workspace, domain.name,code,newstring)
        else:
            printMessage("Missing Domain " + str(subtype))

def DeleteDomainRow():
    # delete unwanted UpdateDomains
    #    no DeviceGroups should belong to rows being removed
    #    UpdateFeatureCode should be run to reassign existing features before deletion
    #    !!! Run the backup and update scripts for domain values first !!!

    startsubtype = 2
    domain = GetDomainBasedOnSubType(str(startsubtype))
    # loop list
    for i in range(len(moveAndDelete)):
        fsubtype = moveAndDelete[i][0]
        fcode = moveAndDelete[i][1]
        tsubtype = moveAndDelete[i][2]
        tcode = moveAndDelete[i][3]
        if fsubtype != startsubtype:
            # subtype changed need to get other domain
            domain = GetDomainBasedOnSubType(str(fsubtype))
            startsubtype = fsubtype

        # delete domain
        if domain is not None:
            printMessage("*** Looking for " + str(fcode) + " : " + domain.name)
            coded_values = domain.codedValues
            for v,d in coded_values.items():
                if str(v) == str(fcode):
                    printMessage("*** Deleting Domain value " + str(fcode) + " : " + domain.name)
                    arcpy.DeleteCodedValueFromDomain_management(workspace,domain.name,fcode)
        else:
            printMessage("Missing Domain " + str(startsubtype))

    return 0

def GetDomainBasedOnSubType(subtype):
    printMessage("    Seeking domain " + str(subtype))
    # select domain name based on subtype array above
    result = None
    for i in range(len(subtypetoDomain)):
        # printMessage("Looking for sub " + str(subtypetoDomain[i][0]) + " -- " + str(subtype))
        if str(subtypetoDomain[i][0]) == subtype:
            # list from above for selecting domain based on subtype
            # printMessage("debug " + str(i))
            domainname = subtypetoDomain[i][1]
            # printMessage("Looking for domain " + domainname)
            domains = arcpy.da.ListDomains(workspace)
            for domain in domains:
                # printMessage("  Found domain " + domain.name)
                if domainname == domain.name:
                    printMessage("    Matching domain " + domain.name)
                    result = domain
                    break
    return result

def addLabelField():
    # Add ADMS Label Field.
    #   Manual: User needs to go into ArcCatalog and the ArcFM Property Manager for DeviceGroup to set not visible or editable.
    arcpy.AddField_management(Data_Connection + "/EDGIS.ElectricDataset/EDGIS.DeviceGroup","ADMSLabel","TEXT","#","#","25","ADMS Label","NULLABLE","NON_REQUIRED","#")
    
def sortDomains():
    arcpy.SortCodedValueDomain_management(Data_Connection,"Device Group Type - Subsurface","CODE","ASCENDING")
    arcpy.SortCodedValueDomain_management(Data_Connection,"Device Group Type - Padmount","CODE","ASCENDING")
    
def printMessage(m):
    print m
    arcpy.AddMessage(m)  

try:
    if arcpy.Describe(Data_Connection).connectionProperties.user == schema:
        # •	AC8 – DEVICEGROUPTYPE labels are updated according to the red ink in Attachment A.
        #           update domain description name - this can occur at any point
        printMessage("UpdateDomainDescription")
        UpdateDomainDescription()

        # •	AC6 - Removal of 6 SCADA-specific DEVICEGROUPTYPE domain values (2.11, 2.14, 2.18, 3.25, 3.29, 3.31 in the attached spreadsheet).
        #        first reassign
        #        The update sql statements need to be executed before this function is run.
        printMessage("DeleteDomainRow")
        DeleteDomainRow()

        printMessage("Add ADMS Label Field")
        addLabelField()

        sortDomains()

        printMessage("Finished!")
except arcpy.ExecuteError:
    print(arcpy.GetMessages())
    ms = arcpy.GetMessages
    for m in ms:
        arcpy.AddError(m)
