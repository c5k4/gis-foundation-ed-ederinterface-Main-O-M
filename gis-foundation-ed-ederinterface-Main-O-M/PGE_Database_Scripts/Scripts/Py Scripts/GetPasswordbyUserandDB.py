# For reading arguments
import getopt
import getPassword
import sys, getopt

def main(argv):
   strDbName = ''
   strUsername = ''
   try:
      opts, args = getopt.getopt(argv,"hd:u:",["dbname=","username="])
   except getopt.GetoptError:
      print 'GetPasswordbyUsersandDB.py -d <dbname> -u <username>'
      sys.exit(2)
   for opt, arg in opts:
      if opt == '-h':
         print 'GetPasswordbyUsersandDB.py -d <dbname> -u <username>'
         sys.exit()
      elif opt in ("-d", "--dbname"):
         strDbName = arg
      elif opt in ("-u", "--username"):
         strUsername = arg
   print 'dbname is ', strDbName
   print 'username is ', strUsername
   strPassword        = getPassword.getPassword(strDbName,strUsername)
   print strPassword
   sys.stdout = str(strPassword)
   return strPassword    

if __name__ == "__main__":
    strPassword = main(sys.argv[1:])
    sys.exit(strPassword)
    #main(sys.argv[1:])
   
