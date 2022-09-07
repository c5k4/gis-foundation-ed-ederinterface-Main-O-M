rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.0
del MapProductionInstaller.log
del MapProductionSolution.log
del Common_Projects.log
MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\MapProduction\MapProduction1_0\Map Production FGDB\PGE.Interfaces.MapProduction.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\MapProduction\MapProduction1_0\Map Production FGDB\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\MapProduction" >> MapProductionSolution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=MapProduction" /t:Build "/p:OutputPath=%cd%\..\Installers" >> MapProductionInstaller.log