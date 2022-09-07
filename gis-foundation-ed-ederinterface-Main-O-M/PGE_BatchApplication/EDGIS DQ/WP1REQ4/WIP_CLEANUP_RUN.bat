set DB=WEBR
SET USER=WEBR
SET WIP_CLEANUP=C:\WIP_CLEANUP

for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD WEBR@WEBR') do set "PASSWORD=%%a"

rem echo execute exec WIP CLEANUP|sqlplus %USER%/%PASSWORD%@%DB%
sqlplus %USER%/%PASSWORD%@%DB% @%WIP_CLEANUP%\WIP_CLEANUP.sql

if "%errorlevel%"=="0" (
                echo Finishing with success
) else (
                echo Finishing with failure
)
