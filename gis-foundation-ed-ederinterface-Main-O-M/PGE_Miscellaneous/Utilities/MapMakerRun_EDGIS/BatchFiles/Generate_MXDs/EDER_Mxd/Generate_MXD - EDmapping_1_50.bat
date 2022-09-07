set basepath=D:\EDGISPub\MapMakerRun_EDGIS
set servicename=Edmapping_1_50
set baseinputpath=\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Common\MapMakerInput
d:
%basepath%\Tool\MapDocMaker.exe write "%baseinputpath%\Input\XLS\%servicename%.xls" "%basepath%\Input\MXD\%servicename%_blank.mxd" "%basepath%\Input\Layers\%servicename%" 1200 100 "%basepath%\Output\%servicename%.mxd" "D:\DatabaseConnectionFiles\Int\eder_in@edgis.sde"
