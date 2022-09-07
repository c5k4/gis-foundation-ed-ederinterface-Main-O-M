set basepath=D:\EDGISPub\MapMakerRun_EDGIS
set servicename=ED_M_and_C
set baseinputpath=\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Common\MapMakerInput
d:
%basepath%\Tool\MapDocMaker.exe write "%baseinputpath%\Input\XLS\Map_Prod2_Xls\%servicename%.xls" "%basepath%\Input\MXD\MP2_templates\%servicename%_blank.mxd" "%basepath%\Input\Layers\MP2_Layers\%servicename%" 1200 100 "%basepath%\Output\MP2_Output\%servicename%.mxd" "D:\DatabaseConnectionFiles\Int\eder_in@edgis.sde"
