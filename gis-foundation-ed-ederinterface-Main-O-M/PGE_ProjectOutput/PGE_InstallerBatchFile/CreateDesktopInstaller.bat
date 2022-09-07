rem Remember to update the SolutionInfo.c7s file as well
set InstallVersion=10.8.1.16
del Common_Projects.log
del Desktop_Installer.log
del Desktop_Solution.log
MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_Desktop\PGE.Desktop.EDER\PGE.Desktop.EDER.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_Desktop\PGE.Desktop.EDER\Include;%cd%\..\PGE_ProjectOutput\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\..\PGE_ProjectOutput\Desktop" >> Desktop_Solution.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=Desktop" /t:Build "/p:OutputPath=%cd%\..\Installers" >> Desktop_Installer.log