rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.3
del DMS_Integration_Installer.log
del DMS_Integration_Solution.log
del Common_Projects.log
MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\DMS\ED0050\PGE.Interfaces.ED.Integration.DMS.sln" /p:"ReferencePath=%cd%\PGE_Interfaces\Include;%cd%\ProjectOutputs\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\DMS_Integration" >> DMS_Integration_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=DMS_Integration" /t:Build "/p:OutputPath=%cd%\..\Installers" >> DMS_Installer.log