rem Remember to update the AssemblyInfo.cs file as well
set InstallVersion=10.8.1.0
del Common_Projects.log
del ED08TaxReport_Installer.log
del ED08TaxReport.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\SAP\PGE.Interfaces.ED008\PGE.Interfaces.ED08TaxReport\PGE.Interfaces.ED08TaxReport.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\SAP\PGE.Interface.ED008\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\ED08TaxReport" >> ED08TaxReport.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\PGE.Installer.ED08TaxReport\Include" "/p:Configuration=ED08TaxReport" /t:Build "/p:OutputPath=%cd%\..\Installers" >> ED08TaxReport_Installer.log


