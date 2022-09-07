# Demonstrates how to stop or start all services in a folder

# For Http calls
import httplib, urllib, json

# For system tools
import sys

# For reading passwords without echoing
import getpass

# For reading arguments
import getopt

# For DateTime
import datetime


# Defines the entry point into the script
def main(argv):
    writeInLog(" ")
    i = datetime.datetime.now()
    writeInLog("%s" % i + " *Start*")
    username = ''
    password = ''
    servername = ''
    operation = ''
    status = ''
    try:
      opts, args = getopt.getopt(argv,"hu:p:s:o:", ["username=","password=","servername=","operation="])
    except getopt.GetoptError:
        print 'StartorStopServices.py -u <username> -p <password> -s <servername> -o <START/STOP>'
        sys.exit(2)
    for opt, arg in opts:
        if opt == '-h':
              print 'StartorStopServices.py -u <username> -p <password> -s <servername> -o <START/STOP>'
              sys.exit()
        elif opt in ("-u", "--username"):
                username = arg
        elif opt in ("-p", "--password"):
                password = arg
        elif opt in ("-s", "--servername"):
                servername = arg
        elif opt in ("-o", "--operation"):
                operation = arg

       # Ask for admin/publisher user name and password
    #username = raw_input("Enter user name: ")
    #password = getpass.getpass(password)
    
    # Ask for server name
    #serverName = raw_input("Enter server name: ")
    serverPort = 6080
    

   # folder = raw_input("Enter the folder name or ROOT for the root location: ")
   # stopOrStart = raw_input("Enter whether you want to START or STOP all services: ")
   
    # Check to make sure stop/start parameter is a valid value
    if str.upper(operation) != "START" and str.upper(operation) != "STOP":
        print "Invalid STOP/START parameter entered"
        i = datetime.datetime.now()
        writeInLog("%s" % i + " Invalid STOP/START parameter entered")
        return
    
    # Get a token
    token = getToken(username, password, servername, serverPort)
    if token == "":
        print "Could not generate a token with the username and password provided."
        i = datetime.datetime.now()
        writeInLog("%s" % i + " Could not generate a token with the username and password provided.")
        return
    
    # Construct URL to read folder
    #if str.upper(folder) == "ROOT":
    #    folder = ""
    #else:
    #    folder += "/"
            
    folderURL = "/arcgis/admin/services"
    
    # This request only needs the token and the response formatting parameter 
    params = urllib.urlencode({'token': token, 'f': 'json'})
    
    headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    
    # Connect to URL and post parameters    
    httpConn = httplib.HTTPConnection(servername, serverPort)
    httpConn.request("POST", folderURL, params, headers)
    
    # Read response
    response = httpConn.getresponse()
    if (response.status != 200):
        httpConn.close()
        print "Could not read folder information."
        i = datetime.datetime.now()
        writeInLog("%s" % i + " Could not read folder information.")
        return
    else:
        data = response.read()
        
        # Check that data returned is not an error object
        if not assertJsonSuccess(data):          
            print "Error when reading folder information. " + str(data)
            i = datetime.datetime.now()
            writeInLog("%s" % i + " Error when reading folder information. " + str(data))
        else:
            print "Processed folder information successfully."
            i = datetime.datetime.now()
            writeInLog("%s" % i + " Processed folder information successfully.")

        # Deserialize response into Python object
        dataObj = json.loads(data)
        httpConn.close()

        # Loop through each service in the folder and stop or start it  
        for FolderName in dataObj['folders']:  
            folderURL = "/arcgis/admin/services/" + FolderName
    
            # This request only needs the token and the response formatting parameter 
            params = urllib.urlencode({'token': token, 'f': 'json'})
    
            headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    
            # Connect to URL and post parameters    
            httpConn = httplib.HTTPConnection(servername, serverPort)
            httpConn.request("POST", folderURL, params, headers)
    
            # Read response
            response = httpConn.getresponse()
            if (response.status != 200):
                httpConn.close()
                print "Could not read folder information."
                i = datetime.datetime.now()
                writeInLog("%s" % i + " Could not read folder information.")
                return
            else:
                data = response.read()
        
                # Check that data returned is not an error object
                if not assertJsonSuccess(data):          
                    print "Error when reading folder information. " + str(data)
                    i = datetime.datetime.now()
                    writeInLog("%s" % i + " Error when reading folder information. " + str(data))
                else:
                    print " "
                    print "Now processing services for " + FolderName + " folder ..."
                    i = datetime.datetime.now()
                    writeInLog(" ")
                    writeInLog("%s" % i + " Now processing services for " + FolderName + " folder ...")

                # Deserialize response into Python object
                dataObj = json.loads(data)
                httpConn.close()

            for item in dataObj['services']:

                fullSvcName = item['serviceName'] + "." + item['type']
                if  fullSvcName ==  "Search.IndexingLauncher" or fullSvcName == "Search.IndexGenerator":
                    continue
                
                # Construct URL to stop or start service, then make the request                
                stopOrStartURL = "/arcgis/admin/services/" + FolderName + "/" + fullSvcName + "/" + operation
                print stopOrStartURL
                i=0;
                while(i<3):
                    httpConn.request("POST", stopOrStartURL, params, headers)
            
                    # Read stop or start response
                    stopStartResponse = httpConn.getresponse()
                    if (stopStartResponse.status != 200):
                        httpConn.close()
                        i = i + 1 
                        if(i==3):
                            print "Error while executing stop or start."
                            j = datetime.datetime.now()
                            writeInLog("%s" % j + " Error while executing stop or start.")
                            return
                    else:
                        stopStartData = stopStartResponse.read()
                    
                
                    # Check that data returned is not an error object
                        if not assertJsonSuccess(stopStartData):
                            if str.upper(operation) == "START":
                                print "Error returned when starting service " + fullSvcName + "."
                                j = datetime.datetime.now()
                                writeInLog("%s" % j +  " Error returned when starting service " + fullSvcName + ".")
                            else:
                                print "Error returned when stopping service " + fullSvcName + "."
                                j = datetime.datetime.now()
                                writeInLog("%s" % j + " Error returned when stopping service " + fullSvcName + ".")

                            print str(stopStartData)
                            writeInLog(str(stopStartData))
                    
                        else:

                            if(operation == "START"):
                                status = "Started"
                            else:
                                status = "Stopped"
                            print "Service " + fullSvcName + " " + status + " successfully."
                            j = datetime.datetime.now()
                            writeInLog("%s" % j + " Service " + fullSvcName + " " + status + " successfully.")
                            httpConn.close()
                            break 
        i = datetime.datetime.now()
        writeInLog("%s" % i + " *END*")
        return
    


# A function to generate a token given username, password and the adminURL.
def getToken(username, password, servername, serverPort):
    # Token URL is typically http://server[:port]/arcgis/admin/generateToken
    tokenURL = "/arcgis/admin/generateToken"
    
    params = urllib.urlencode({'username': username, 'password': password, 'client': 'requestip', 'f': 'json'})
    
    headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    
    # Connect to URL and post parameters
    httpConn = httplib.HTTPConnection(servername, serverPort)
    httpConn.request("POST", tokenURL, params, headers)
    
    # Read response
    response = httpConn.getresponse()
    if (response.status != 200):
        httpConn.close()
        print "Error while fetching tokens from admin URL. Please check the URL and try again."
        j = datetime.datetime.now()
        writeInLog("%s" % j + " Error while fetching tokens from admin URL. Please check the URL and try again.")
        return
    else:
        data = response.read()
        httpConn.close()
        
        # Check that data returned is not an error object
        if not assertJsonSuccess(data):            
            return
        
        # Extract the token from it
        token = json.loads(data)        
        return token['token']            
        

# A function that checks that the input JSON object 
#  is not an error object.
def assertJsonSuccess(data):
    obj = json.loads(data)
    if 'status' in obj and obj['status'] == "error":
        print "Error: JSON object returns an error. " + str(obj)
        j = datetime.datetime.now()
        writeInLog("%s" % j + " Error: JSON object returns an error. " + str(obj))
        return False
    else:
        return True

# function for write a log
def writeInLog(msg):
    filename = str(datetime.date.today())    
    f = open(filename + ".log", "a")
    f.write(msg+"\n")
    f.close()    
        
# Script start
if __name__ == "__main__":
    main(sys.argv[1:])
