rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.0
del ED07_Installer.log
del ED07_Solution.log
del Common_Projects.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED007\PGE.Interfaces.LoadingDataInOracle\PGE.Interfaces.LoadingDataInOracle.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\SAP\PGE.Interface.ED007\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\ED07" >> ED07_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=ED07" /t:Build "/p:OutputPath=%cd%\..\Installers" >> ED07_Installer.log
