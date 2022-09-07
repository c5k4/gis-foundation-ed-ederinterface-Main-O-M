rem Remember to update the AssemblyInfo.cs file as well
set InstallVersion=10.8.1.0
del Common_Projects.log
del TracingCache_Installer.log
del TracingCache.log

MSbuild "%cd%\..\..\PGE_CommonProject\PGE.Common.DeliveryProject\PGE.Common.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\..\..\PGE_CommonProject\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\Common_Projects" >> Common_Projects.log
MSbuild "%cd%\..\..\PGE_BatchApplication\PGE_Cached_Tracing\PGE.BatchApplication.PGE_Cached_Tracing.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\..\..\PGE_BatchApplication\Include;%cd%\ProjectOutputs\Common_Projects" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\..\..\PGE_ProjectOutput\CachedTracing" >> TracingCache.log
MSbuild "%cd%\..\..\PGE_InstallerProjects\PGE.Installer.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\..\..\PGE_InstallerProjects\Include" "/p:Configuration=TracingCache" /t:Build "/p:OutputPath=%cd%\..\Installers" >> Desktop_Installer.log