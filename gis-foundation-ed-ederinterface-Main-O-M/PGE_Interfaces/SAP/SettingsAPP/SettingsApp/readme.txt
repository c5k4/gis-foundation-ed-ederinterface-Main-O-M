*************
Notes:
*************
1. Updating Identity column for Id in edmx using model browser doesn't update backend XML of edmx file. So it must be updated manually. For that right click on edmx file--> open with XML editor--> Check for all identity columns and see if it has StoredGeneratedPattern = "Identity".
	<Property Name="ID" Type="number" Nullable="false" StoreGeneratedPattern="Identity" />


***********************
Post deployment steps
***********************
1. Make sure Oracle.ManagedDataAccess.dll and Oracle.ManagedDataAccessDTC.dll files are there in the bin folder. If not copy them from the "Components" folder and paste into the "bin" folder.
2. Update web.config file for following two items.
	i) Entity Framework's connectionstring to respective environment's database.
		<add name="SettingsEntities" connectionString="metadata=res://*/SettingsAppModel.csdl|res://*/SettingsAppModel.ssdl|res://*/SettingsAppModel.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=&quot;DATA SOURCE=edgisdbqa01.comp.pge.com:1521/EDGISA2Q;PASSWORD=edsett;PERSIST SECURITY INFO=True;USER ID=EDSETT&quot;" providerName="System.Data.EntityClient" />

	ii) Environment variable in web.config to respective environment.
		<add key="Environment" value="DEV" />



		