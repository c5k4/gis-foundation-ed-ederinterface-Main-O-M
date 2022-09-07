import json
import urllib
import urllib2
import yaml

def gentoken(url, username, password, expiration=60):
    query_dict = {'username':   username,
                  'password':   password,
                  'expiration': str(expiration),
                  'client':     'requestip'}
    query_string = urllib.urlencode(query_dict)
    return json.loads(urllib.urlopen(url + "?f=json", query_string).read())['token']

def validateDatastore(server, item, port=6080, token=None):
    validation_url = 'http://{}:{}/arcgis/admin/data/validateDataItem?f=pjson&token={}'.format(server, port,token)
    #print validation_url
    query_dict = {'item':item}#, 'f':'json', 'token':token
    #print query_dict
    query_string = urllib.urlencode(query_dict)
    print item['path']
    output = urllib2.urlopen(validation_url, query_string).read() # The ' ' forces POST
    print output
    if json.loads(output).has_key('status') == False:
        #if status flag available, then success
        if status.has_key(server) == False:
            status[server]=[]

        status[server].append(item['path'])

def retrieveDatastores(server, username, password, port=6080, token=None):
    if token is None:
        token_url = "http://{}:{}/arcgis/admin/generateToken".format(server, port)
        print token_url
        token = gentoken(token_url, username, password)
    datastore_url = "http://{}:{}/arcgis/admin/data/findItems?parentPath=/enterpriseDatabases&ancestorPath=&types=&id=&managed=f&f=pjson&token={}".format(server, port, token)
    #print datastore_url
    output = urllib2.urlopen(datastore_url, ' ').read() # The ' ' forces POST
    #print output
    items = yaml.safe_load(output)['items']
    #items = json.loads(output)['items']
    #print items
    for item in items:
        #print item
        validateDatastore(server, item, port, token)
    
admin_username = "EDGISAdminProd"
admin_password = "EDGISAdminProd!123"
status={}

##server = "PREDGMAPPWG006"
##retrieveDatastores(server,admin_username, admin_password)

serverList = ["PREDGMAPPWG001","PREDGMAPPWG003","PREDGMAPPWG005","PREDGMAPPWG007","PREDGMAPPWG009","PREDGMAPPWG011","PREDGMAPPWG013","PREDGMAPPWG015","PREDGMAPPWG017","PREDGMAPPWG019","PREDGMAPPWG021","PREDGMAPPWG023"]
#serverList = ['PREDGMAPPWG002','PREDGMAPPWG004','PREDGMAPPWG006','PREDGMAPPWG008','PREDGMAPPWG010','PREDGMAPPWG012','PREDGMAPPWG014','PREDGMAPPWG016','PREDGMAPPWG018','PREDGMAPPWG020','PREDGMAPPWG022','PREDGMAPPWG024']
#serverList = ["PREDGMAPPWG006"]
for server in serverList:
    retrieveDatastores(server,admin_username, admin_password)

for s,l in status.items():
    print s + ':' + ','.join(l)

print "Completed"
