 [System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
 $publish = New-Object System.EnterpriseServices.Internal.Publish
 $publish.GacInstall("C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe")