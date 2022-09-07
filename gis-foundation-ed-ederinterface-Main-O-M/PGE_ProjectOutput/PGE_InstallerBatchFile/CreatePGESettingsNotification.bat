rem Remember to update the SolutionInfo.cs file as well
set InstallVersion=10.8.1.0

del SettingsNotification_Installer.log
del SettingsNotification_Solution.log
del Common_Projects.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Interfaces\SAP\SettingsEmailNotification\PGE.Interfaces.SettingsEmailNotification.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Interfaces\SAP\SettingsEmailNotification\Include;%cd%\..\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\SettingsNotification" >> SettingsNotification_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=SettingsNotification" /t:Build "/p:OutputPath=%cd%\..\Installers" >> SettingsNotification_Installer.log
