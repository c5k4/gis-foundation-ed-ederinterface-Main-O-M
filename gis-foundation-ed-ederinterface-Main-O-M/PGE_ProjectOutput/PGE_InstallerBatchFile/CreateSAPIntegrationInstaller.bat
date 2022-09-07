rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.0
del SAP_Integration_Installer.log
del SAP_Integration_Solution.log
del Common_Projects.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED006_0013\PGE.Interfaces.SAP.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED006_0013\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\SAP_Integration" >> SAP_Integration_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\PGE.Installer.SAP.Integration\Include" "/p:Configuration=SAP_Integration" /t:Build "/p:OutputPath=%cd%\..\Installers" >> SAP_Integration_Installer.log





