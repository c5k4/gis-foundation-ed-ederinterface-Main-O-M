for /f %%a in ('PGE_DBPasswordManagement.exe GETPASSWORD UC4ADMIN@EDER') do set "PWD=%%a"
sqlplus -s uc4admin/%PWD%@EDER @D:\edgisdbmaint\PopulateInfoGatherStat_Analyze.sql