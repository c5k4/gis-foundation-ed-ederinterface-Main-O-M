set basepath=D:\EDGISPub\MapMakerRun_EDGIS
set servicename=Substation
set baseinputpath=\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Common\MapMakerInput
d:
%basepath%\Tool\MapDocMaker.exe write "%baseinputpath%\Input\XLS\Eder_XLS\%servicename%.xls" "%basepath%\Input\MXD\Eder_templates\%servicename%_blank.mxd" "%basepath%\Input\Layers\EDER_LAYERS_FILE\%servicename%" 100 100 "%basepath%\Output\Eder_mxds\%servicename%.mxd" "D:\DatabaseConnectionFiles\QA\edersub_qa@edgis.sde"
