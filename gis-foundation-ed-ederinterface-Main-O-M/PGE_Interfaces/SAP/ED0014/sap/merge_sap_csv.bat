setlocal EnableExtensions EnableDelayedExpansion

set DB=102DEV01
SET USER=PGEDATA
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD PGEDATA@102DEV01') do set "PASSWORD=%%a"

sqlldr '%USER%/%PASSWORD%@%DB%' control='control.txt' log='sap_nh_results.log'

if errorlevel 0 echo SQL*Loader execution successful
if errorlevel 2 echo SQL*Loader got executed, but atleast some rows got rejected, check the log file.
if errorlevel 3 echo SQL*Loader encountered an unrecoverable failure,check the logfile for more details
if errorlevel 4 echo SQL*Loader execution encountered OS Specific Error

if "%errorlevel%"=="0" (
	echo execute gis_sap.merge_notificationheaders|sqlplus %USER%/%PASSWORD%@%DB%
	echo Finishing with success
) else (
	echo Finishing with failure
)