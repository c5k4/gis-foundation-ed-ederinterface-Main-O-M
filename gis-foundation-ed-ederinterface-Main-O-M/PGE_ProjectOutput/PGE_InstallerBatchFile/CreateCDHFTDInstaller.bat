rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.2
del CDHFTD_Installer.log
del CDHFTD_Solution.log
del Common_Projects.log
MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_BatchApplication\PGE_ChangeDetection\PGE_ChangeDetection_HFTD_FIA\PGE.BatchApplication.PGE_ChangeDetection_HFTD_FIA.sln" /p:"ReferencePath=%cd%\PGE_BatchApplication\PGE_ChangeDetection\Include;%cd%\ProjectOutputs\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\CDHFTD" >> CDHFTD_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=CDHFTD" /t:Build "/p:OutputPath=%cd%\..\Installers" >> CDHFTD_Installer.log