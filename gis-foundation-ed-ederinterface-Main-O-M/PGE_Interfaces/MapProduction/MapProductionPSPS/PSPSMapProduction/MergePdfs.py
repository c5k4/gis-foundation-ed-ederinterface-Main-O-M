import arcpy, os, sys, shutil, time, traceback

# Set file name and remove if it already exists
# Get the parameter values
#

try:
    inputFolder = arcpy.GetParameterAsText(0)
    logFilePath = "logs"
    
    if (__file__ != ""):
        dir_path = os.path.dirname(os.path.realpath(__file__))
	logFilePath = os.path.join(dir_path, "logs")

    logFileName = os.path.join(logFilePath, "MergeAllPDF_" + time.strftime("%Y-%m-%d_%H%M%S") + ".log")

    logFile = open(logFileName,'w')

    # search through all the sub folders and merge PDFs for each folder
    for (dirpath, dirnames, filenames) in os.walk(inputFolder):
        # Iterate over every dir name
        for dirname in dirnames:
            currdir = os.path.join(dirpath, dirname)
            targetPDFFile = currdir + " Segment Map.pdf"
            logFile.write("Merged PDF: " + targetPDFFile + "\n")
                    
            # remove the existing PDF file
            if os.path.exists(targetPDFFile):
                os.remove(targetPDFFile)

            pdfDoc = arcpy.mapping.PDFDocumentCreate(targetPDFFile)

            # File all PDF files in the input folder and add to the list and the nsort it to make sure the overview map appear first
            pdfFileList = []
            for (subdirpath, subdirnames, subfilenames) in os.walk(currdir):
                # Iterate over every file name
                for subfile in subfilenames:
                    pdfFileList.append(os.path.join(subdirpath, subfile))

            pdfFileList.sort()

            # File all PDF files in the input folder and merge them into one PDF
            for pdfFile in pdfFileList:
                pdfDoc.appendPages(pdfFile);
                logFile.write("\tAppend file: " + pdfFile + "\n")

            #Commit changes and delete variable reference
            pdfDoc.saveAndClose()

            # delete the current sub-folder
            shutil.rmtree(currdir) 

            del pdfDoc

    # Close the file
    logFile.close()

except RuntimeError:
    logFile.write("Failed to merge PDF files.\n")
    sys.exit(1)

