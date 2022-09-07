set WEBR_USERNAME=WEBR
set WIP_INSTANCE=WIP
for /f %%a in ('D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe GETPASSWORD WEBR@WIP') do set "WEBR_PASSWORD=%%a"

sdelayer -o alter -l webr.pge_outagexfmr_spvw,shape  -E calc -i sde:oracle11g:%WIP_INSTANCE% -u %WEBR_USERNAME% -p %WEBR_PASSWORD%