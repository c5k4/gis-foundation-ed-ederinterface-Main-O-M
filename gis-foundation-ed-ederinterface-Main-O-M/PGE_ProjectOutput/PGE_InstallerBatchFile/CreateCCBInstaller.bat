rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.0
del CCB_Installer.log
del CCB_Solution.log
del Common_Projects.log
MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\CCB\\PGE.Interfaces.CCBtoGISInterface\PGE.Interfaces.CCBtoGISInterface.sln" /p:"ReferencePath=%cd%\PGE_Interfaces\Include;%cd%\ProjectOutputs\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\CCB" >> CCB_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=CCB" /t:Build "/p:OutputPath=%cd%\..\Installers" >> CCB_Installer.log