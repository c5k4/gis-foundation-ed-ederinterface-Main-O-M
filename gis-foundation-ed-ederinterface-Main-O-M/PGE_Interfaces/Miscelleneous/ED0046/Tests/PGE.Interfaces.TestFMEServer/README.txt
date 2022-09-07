This tool can be used to run performance tests against an FME Server by firing a CSV (exports.csv) off asynchronously to an FME Server instance.
It can also be used to submit a job.

1) Populate the contents of exports.csv with your desired jobs. It's a comma-separated file with the following fields:
- email address
- extent polygon
- file name
- scale

You can get extent polygons by using ed_master_coords.mxd in the folder above this.

You can do this against failed jobs from FME by going into the FME Web User interface and selecting jobs e.g. http://edgisappprd13/fmeserver/jobs
Select the job and then click the Request Data tab. You can find the values above from:
- email_to
- _GEODBInSearchFeature_GEODATABASE_SDE
- output_file_name
- scale
Copy and paste each value onto its own line in exports.csv e.g.
p1pc@pge.com,1984263 13567917 1984263 13568344 1984959 13568344 1984959 13567917 1984263 13567917,20140110_074546_GISToCAD,100

2) Open a command prompt and run the TestFMEServer.exe with the following arguments, changing the first part of the URL to the FME Server you are hitting e.g. edgisappprd13/14 for production:
TestFMEServer.exe -m async -e p1pc@pge.com -r  http://edgisappprd14/fmedatadownload/EDGIS/edgis_to_cad.fmw
TestFMEServer.exe -m async -e p1pc@pge.com -r  http://edgiswebdev01:8080/fmedatadownload/EDGIS/edgis_to_cad.fmw

3) Press a key to end the process.