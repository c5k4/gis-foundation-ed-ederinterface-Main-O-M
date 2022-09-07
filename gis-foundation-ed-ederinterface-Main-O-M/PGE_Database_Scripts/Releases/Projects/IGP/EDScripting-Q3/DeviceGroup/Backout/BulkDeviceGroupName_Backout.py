#Import system modules
import arcpy

# !!!! point to sde file that is connected using EDGIS user (needed to create table in that schema) !!!!
Data_connection = "Database Connections/PT1D_EDGIS.sde"

arcpy.env.workspace = Data_connection
# schema and connection user must match to create table in correct location
schema = "EDGIS"

# moveanddelete - [[subtype from, code going from and deleting, subtype to, code going to]] - Only codes are updated, NOT subtypes
adddomain = [[2,11,'Subsurface Switch, Two way, One Way Switched (Supervisory Controlled)'],
                 [2,14,'Subsurface Switch, Three Way, Three Ways Switched (Two Switches Supervisory Controlled Only)'],
                 [2,18,'Subsurface Interrupter (Supervisory Controlled)'],
                 [3,25,'Padmounted Capacitor (Supervisory Controlled)'],
                 [3,29,'Padmounted Oil Switch (Supervisory Controlled)'],
                 [3,31,'Padmounted Interrupter (Supervisory Controlled)']]

#updatedesc - [[subtype,code, append desc]] - we're only appending currenlty
#    used in UpdateDomainDescription
updateDesc = [[2,9,'Subsurface Cutouts'],[2,12,'Subsurface Switch, Three way, Two Ways Switched'],
              [2,15,'Subsurface Smart Switch'],[2,19,'Subsurface Sectionalizer'],
              [2,26,'TGRAL'],[2,27,'TGRAM (Three Way)'],
              [2,28,'TGRAM (Four Way)'],[2,30,'4kV Junction'],
              [2,34,'TGRAM (Two Way)'],
              [3,1,'PMH 3'],[3,2,'PMH 4 w/1 fuse'],[3,3,'PMH 4 w/2 fuses'],
              [3,4,'PMH 4 w/3 fuses'],[3,5,'PMH 5'],[3,6,'PMH 6 w/(1) Hot Leg'],
              [3,7,'PMH 9 w/(2) Hot Legs'],[3,8,'PMH 11 w/(1) Hot Leg'],
              [3,9,'PMH 11 w/(3) single phase Leg'],[3,10,'PMH 6 w/(3) 1-phase Leg'],
              [3,11,'PMH 9 w/(6)1-phase Leg'],[3,12,'PMH 9 w/ (3) 1-phase & (1) 3-phase Leg'],
              [3,28,'Padmounted Oil Switch'],
              [3,34,'Padmounted Automatic Circuit Recloser(Supervisory Controlled)'],[3,35,'Padmounted Sectionalizer']]

# subtypetodomain - matches subtypes to domain name
subtypetoDomain = [[2,'Device Group Type - Subsurface'],[3,'Device Group Type - Padmount']]

workspace = Data_connection
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

def AddDomainRow():
    # add UpdateDomains
    #    no DeviceGroups should belong to rows being removed
    #    UpdateFeatureCode should be run to reassign existing features before deletion
    #    !!! Run the backup and update scripts for domain values first !!!

    startsubtype = 2
    domain = GetDomainBasedOnSubType(str(startsubtype))
    # loop list
    for i in range(len(adddomain)):
        fsubtype = adddomain[i][0]
        fcode = adddomain[i][1]
        value = adddomain[i][2]
        if fsubtype != startsubtype:
            # subtype changed need to get other domain
            domain = GetDomainBasedOnSubType(str(fsubtype))
            startsubtype = fsubtype

        # add domain row
        if domain is not None:
            if domain.domainType == 'CodedValue':
                coded_values = domain.codedValues
                # add back
                printMessage("*** Adding Domain value " + str(fcode) + " : " + domain.name)
                arcpy.AddCodedValueToDomain_management(workspace, domain.name, fcode, value)
                break
                
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

def deleteLabelField():
    # Add ADMS Label Field.
    #   Manual: User needs to go into ArcCatalog and the ArcFM Property Manager for DeviceGroup to set not visible or editable.
    arcpy.DeleteField_management(Data_connection + "/EDGIS.ElectricDataset/EDGIS.DeviceGroup", "ADMSLABEL")


def printMessage(m):
    print m
    arcpy.AddMessage(m)  

try:
    if arcpy.Describe(Data_connection).connectionProperties.user == schema:
        # •	AC6 - Removal of 6 SCADA-specific DEVICEGROUPTYPE domain values (2.11, 2.14, 2.18, 3.25, 3.29, 3.31 in the attached spreadsheet).
        #        first reassign
        #        The update sql statements need to be executed before this function is run.
        printMessage("AddDomainRow")
        AddDomainRow()
       
        # •	AC8 – DEVICEGROUPTYPE labels are updated according to the red ink in Attachment A.
        #           update domain description name - this can occur at any point
        printMessage("UpdateDomainDescription")
        UpdateDomainDescription()

        printMessage("Delete ADMS Label Field")
     #   deleteLabelField()

        printMessage("Finished!")
except arcpy.ExecuteError:
    print(arcpy.GetMessages())
    ms = arcpy.GetMessages
    for m in ms:
        arcpy.AddError(m)
