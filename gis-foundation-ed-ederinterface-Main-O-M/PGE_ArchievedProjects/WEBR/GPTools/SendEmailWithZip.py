import arcgisscripting, smtplib, os, sys, traceback, zipfile, uuid, shutil, urllib

from email.MIMEMultipart import MIMEMultipart
from email.MIMEBase import MIMEBase
from email.MIMEText import MIMEText
from email.Utils import COMMASPACE, formatdate
from email import Encoders

finalOutputDir = "\\\\wsgo496902\\arcgisserver\\directories\\arcgisoutput\\PrintExtracts\\"
baseUrl = "http://wsgo496902:6080/arcgis/rest/directories/arcgisoutput/PrintExtracts/"

zipLink = False

gp = arcgisscripting.create(9.3)

#**********************************************************************
# Description:
#   Emails a file. File is assumed to be a zip file. Routine either attaches
#   the zip file to the email or sends the URL to the zip file.
#
# Parameters:
#   1 - File to send.
#   2 - Email address to send file.
#   3 - Name of outgoing email server.
#   4 - Output boolean success flag.
#**********************************************************************

def send_mail(send_from, send_to, subject, text, f, server, smtpUser="", smtpPwd=""):
    try:
        msg = MIMEMultipart()
        msg['From'] = send_from
        msg['To'] = COMMASPACE.join(send_to)
        msg['Date'] = formatdate(localtime=True)
        msg['Subject'] = subject
        msg.attach( MIMEText(text) )

        if not f is None:
            part = MIMEBase('application', "zip")   # Change if different file type sent.
            part.set_payload( open(f,"rb").read() )
            Encoders.encode_base64(part)
            part.add_header('Content-Disposition', 'attachment; filename="%s"' % os.path.basename(f))
            msg.attach(part)
            
        smtp = smtplib.SMTP(server)
        
        # If your server requires user/password
        if smtpUser != "" and smtpPwd != "":
            smtp.login(smtpUser, smtpPwd)
        
        smtp.sendmail(send_from, send_to, msg.as_string())
        smtp.close()
    except:
        tb = sys.exc_info()[2]
        tbinfo = traceback.format_tb(tb)[0]
        pymsg = "PYTHON ERRORS:\nTraceback Info:\n" + tbinfo + "\nError Info:\n    " + \
                str(sys.exc_type)+ ": " + str(sys.exc_value) + "\n"
        raise Exception("SendEmailError:" + pymsg)

   
if __name__ == '__main__':

    sendto = gp.GetParameterAsText(0).split(";")
    fromaddr = gp.GetParameterAsText(1)
    subject = gp.GetParameterAsText(2)
    text = gp.GetParameterAsText(3)
    zipfile_in = gp.GetParameterAsText(4).replace("\\",os.sep)
    maxsize = int(gp.GetParameterAsText(5)) * 1000000
    smtpMailServer = gp.GetParameterAsText(6)
    smtpUser = gp.GetParameterAsText(7)
    smtpPwd = gp.GetParameterAsText(8)
    
    try: 
        zipsize = os.path.getsize(zipfile_in)
        #Message"Zip file size1 = "
        gp.AddMessage(gp.GetIDMessage(86156) + str(zipsize))
        if  zipsize >= maxsize or maxsize > 10000000:
            gp.AddMessage("Input file or maxsize too large. Zipping input file...")
            univUniqID = str(uuid.uuid1())
#            shutil.copy2(webr_zip, finalOutputDir)
            if zipLink == True:
                webr_zip = os.path.join(os.environ["TEMP"], "_ags_" + univUniqID + ".zip")
                zout = zipfile.ZipFile(webr_zip, "w", zipfile.ZIP_DEFLATED)
                zout.write(zipfile_in)
                zout.close()
                zipsize = os.path.getsize(zout.filename)
                gp.AddMessage(gp.GetIDMessage(86156) + str(zipsize))
                zipfile_in = zout.filename
            else:
                webr_zip = os.path.join(os.environ["TEMP"], "_ags_" + univUniqID + ".pdf")
                os.rename(zipfile_in, webr_zip)
                zipfile_in = os.path.basename(webr_zip)
#            if not os.path.exists(finalOutputDir): os.makedirs(finalOutputDir)
            finalOutputFile = os.path.join(finalOutputDir, zipfile_in)
            arcpy.AddMessage("ZIP FILE NAME: " + finalOutputFile)
            shutil.copy2(webr_zip, finalOutputDir)
            urlFile = baseUrl + os.path.basename(zipfile_in)
            arcpy.AddMessage("URL FILE: " + urlFile)
            #Normalize the URL to fix things like spaces in zipFileName
            fullUrl = urllib.quote(urlFile, safe="%/:=&?~#+!$,;'@()*[]")
            text = "Your WEBR Extract completed.\n\nPlease find your PDF file at: \n"
            text = text + fullUrl
            arcpy.AddMessage("EmailText: " + text)
            send_mail(fromaddr, sendto, subject, text, None, smtpMailServer, smtpUser, smtpPwd)
            #Message "Sent zipfile to %s from %s"
            gp.AddIDMessage("INFORMATIVE", 86154, sendto, fromaddr)
            gp.SetParameterAsText(9, "True")
        elif  zipsize <= maxsize:
            send_mail(fromaddr, sendto, subject, text, zipfile_in, smtpMailServer, smtpUser, smtpPwd)
            #Message "Sent zipfile to %s from %s"
            gp.AddIDMessage("INFORMATIVE", 86154, sendto, fromaddr)
            gp.SetParameterAsText(9, "True")
        else:
            #Message "The resulting zip file is too large (%sMB).  Must be less than %MB.  Please
            # digitize a smaller Area of Interest."
            gp.AddIDMessage("ERROR", 86155, str(round(zipsize / 1000000.0, 2)),
                            str(round(maxsize / 1000000.0, 2)))
            gp.SetParameterAsText(9, "False")
            raise Exception

    except:
        # Return any python specific errors as well as any errors from the geoprocessor
        tb = sys.exc_info()[2]
        tbinfo = traceback.format_tb(tb)[0]
        pymsg = "PYTHON ERRORS:\nTraceback Info:\n" + tbinfo + "\nError Info:\n    " + \
                str(sys.exc_type)+ ": " + str(sys.exc_value) + "\n"
        gp.AddError(pymsg)
        #Message "Unable to send email"
		#gp.AddIDMessage("ERROR", 86157)
        gp.AddError("ERROR, Unable to send email")