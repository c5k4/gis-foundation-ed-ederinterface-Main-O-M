rem Remember to update the AssemblyInfo.cs file as well
set InstallVersion=10.2.1.1
del Telvent_Delivery.log
del TracingCache_Installer.log
del TracingCache.log
MSbuild "%cd%\EDER\externals\telvent.delivery\Telvent.Delivery.CodeLibrary.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" "/p:ReferencePath=%cd%\EDER\Include" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\ProjectOutputs\Telvent_Delivery" >> Telvent_Delivery.log
MSbuild "%cd%\IBM GIS\Standalone Executables\ArcFmWebConfigLimited\ArcFmWebConfigLimited.sln" /p:PreBuildEvent="" /p:PostBuildEvent="" /p:"ReferencePath=%cd%\EDER\Include;%cd%\ProjectOutputs\Telvent_Delivery" "/p:Configuration=Release" /p:Platform="x86" /t:Build "/p:OutputPath=%cd%\ProjectOutputs\WEBRConfig" >> WEBRConfig.log
MSbuild "%cd%\EDER\BuildIntegration\installation\installation.sln" /p:InstallVersion=%InstallVersion% "/p:ReferencePath=%cd%\EDER\Include" "/p:Configuration=WEBRConfig" /t:Build "/p:OutputPath=%cd%\ProjectOutputs\Installers" >> WEBRConfig_Installer.log