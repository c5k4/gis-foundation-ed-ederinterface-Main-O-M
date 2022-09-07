rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.4
del ED11_12_Installer.log
del ED11_12_Solution.log
del Common_Projects.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED11_12\PGE.Interfaces.ED11_12.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED11_12\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\ED11_12" >> ED11_12_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\PGE.Installer.ED11_12\Include" "/p:Configuration=ED11_12" /t:Build "/p:OutputPath=%cd%\..\Installers" >> ED11_12_Installer.log


