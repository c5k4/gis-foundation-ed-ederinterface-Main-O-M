import arcpy
import sys
import os
##arcpy.env.workspace ="D:\\Neha\\Sonic.sde"
path=sys.argv[1]
domainName=sys.argv[2]
xlsFilePath =path+"\\"+domainName+".xls"#the file you want to convert in table if you are working with sde use sde_ before name
tableName = path+"\\"+"Temp"+"2.dbf"# Give name to new db table
sheetName =domainName # the xls sheet out of all xls sheet given in bottom tab of xls file
codeField ="TlDom" #name of domain field you modified
descField="TlDes"  #domain field Description you modified
dWorkspace =path+"\\ConnectionDB.sde"# path of workspce where you want to add modified or newly created domain
domName =domainName  # new name of domain
domDesc =""#domainName+"Des"  # new name of des column
# Process: Create a table from an existing xls file

arcpy.ExcelToTable_conversion(xlsFilePath, tableName,sheetName)#"D:\\Neha\\sde_"+tableName+".dbf"
domTable = tableName#"D:\\Neha\\sde_"+tableName+".dbf"
# Process: Create a domain from an existing table
arcpy.TableToDomain_management(domTable, codeField, descField, dWorkspace, domName, domDesc, "REPLACE")
if os.path.exists(tableName):
    os.remove(tableName)
    os.remove(path+"\\"+"Temp"+"2.cpg")
    os.remove(path+"\\"+"Temp"+"2.dbf.xml")
    print("File one Removed!")
else:
    print("File one does not exist")
##if os.path.exists(path+"sde_"+domainName+".dbf"):
##    os.remove(path+"sde_"+domainName+".dbf")
##    os.remove(path+"sde_"+domainName+".dbf.xml")
##    os.remove(path+"sde_"+domainName+".cpg")
##    print("File two Removed!")
##else:
##    print("File two does not exist")
