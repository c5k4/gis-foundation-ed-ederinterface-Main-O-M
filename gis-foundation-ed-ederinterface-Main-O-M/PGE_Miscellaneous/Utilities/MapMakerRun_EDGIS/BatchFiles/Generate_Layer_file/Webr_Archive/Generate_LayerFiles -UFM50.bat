set basepath=D:\EDGISPub\MapMakerRun_EDGIS
set servicename=UFM_50
d:
%basepath%\Tool\MapDocMaker.exe read "%basepath%\Source\%servicename%.mxd" "%basepath%\Input\XLS\%servicename%.xls" "%basepath%\Input\Layers\%servicename%" "" %basepath%\Input\MXD\%servicename%_Blank.mxd"
