* Disable the default site running on port 80 before deploying any web application on the server.

==========  UC4 Executables Deployment for Settings =====================
1) Create a folder structure "D:\Settings\Utility" on the batch server
2) Copy BatchFiles folder from "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\Settings\Utiliity\1.0.0.0"
	location to "D:\Settings\Utility" in the batch server.
3) Copy executable from "#EnvironmentFolder" under "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\Settings\Utiliity\1.0.0.0"
   location to "D:\Settings\Utility\".

==========  UC4 Executables Deployment for TLM =====================

1) Create a folder structure "D:\TLM\Utility" on the batch server
2) Copy BatchFiles folder from "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\TLM\Utiliity"
	location to "D:\TLM\Utility" in the batch server.
3) Copy executable from "#EnvironmentFolder" under "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\TLM\Utiliity\1.0.0.0"
   location to "D:\TLM\Utility\".


=== Web Deployment - Settings ====

1) Create a folder structure "D:\inetpub\SettingsApp"
2) Copy files from "#EnvironmentFolder" under "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\Settings\WebApplication\1.0.0.0"
   location to "D:\inetpub\SettingsApp"

=== Web Deployment - TLM ===

1) Create a folder structure "D:\inetpub\TLM"
2) Copy files from "#EnvironmentFolder" under "\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_Delivery\TLM\WebApplication\1.0.0.0"
   location to "D:\inetpub\TLM"



== Compilation of Utility project ==
-- Debugging --
1) When debugging set the "Platform Targer" to "x86"
2) Remove the reference to Oracle.DataAccess
3) Add the reference to Oracle.DataAccess from .Net tab.

-- Release to other envirionment 
1) When ready to deploy set the "Platform Targer" to "x64"
2) Remove the reference to Oracle.DataAccess
3) Add the reference to Oracle.DataAccess from "Components-64Bit" folder.


