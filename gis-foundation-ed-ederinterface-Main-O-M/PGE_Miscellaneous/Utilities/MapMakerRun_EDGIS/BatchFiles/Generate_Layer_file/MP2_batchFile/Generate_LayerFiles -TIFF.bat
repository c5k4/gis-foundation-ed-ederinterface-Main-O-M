set basepath=D:\EDGISPub\MapMakerRun_EDGIS
set servicename=TIFF
set baseinputpath=\\SFSHARE04-NAS2\sfgispoc_data\ApplicationDevelopment\EDGIS_ReArchitecture\MapDeployment\Common\MapMakerInput

d:
%basepath%\Tool\MapDocMaker.exe read "%basepath%\Source\%servicename%.mxd" "%baseinputpath%\Input\XLS\%servicename%.xls" "%basepath%\Input\Layers\%servicename%" "" %basepath%\Input\MXD\%servicename%_Blank.mxd"
