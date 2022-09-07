CONTENTS OF THIS FILE
---------------------
 * Introduction
 * Requirements
 * Recommended modules

INTRODUCTION
------------

Use PGE_DBPasswordManagement utility to generate password for Database users
 
REQUIREMENTS
------------

This module requires the following modules:

 * READ Only (Available on All Servers)
    <add key="CONNECTIONPATH" value="D:\Program Files (x86)\Miner and Miner\PG&amp;E Custom Components\PGE.PasswordMgmt\Connections.xml"/>
 * WRITE (Avaialable on Citrix & Batch 001 & 002 Servers)
    <add key="WCONNECTIONPATH" value="\\rcnas01-smb\edgisrearch-fs01\Connections\Credentials\[Environment]\Connections.xml"/>

RECOMMENDED MODULES
-------------------

 * Use this option if Single password need to be generated first value as UserName and second value as DB Instance
                 PGE_DBPasswordManagement GENPASSWORD <USERNAME>@<DBINSTANCENAME>

 * Use this option if multiple passwords need to be generated. Provide path of CSV (without Header) with USERNAME,INSTANCE format and this tool will generate a CSV file in same path with PGE_DBPasswordManagement_ as prefix having USERNAME,INSTACE, Password Text, Password Encryption
                 PGE_DBPasswordManagement GENPASSWORDF <CSVFileFullPath>

 * Use this option to Encrypt any existing Text.  Use everything in CAPSLOCK except passwords
                 PGE_DBPasswordManagement ENCRYPT <PasswordText>

 * Use this option to Encrypt all Text in a file for each in new line. Use everything in CAPSLOCK except passwords
                 PGE_DBPasswordManagement ENCRYPTF <FilePath>

 * Use this option to Decrypt any existing encryption
                 PGE_DBPasswordManagement DECRYPT <PasswordEncryption>

 * Use this option to Decrypt all encrypted text in a file for each in new line
                 PGE_DBPasswordManagement DECRYPTF <FilePath>

 * Use this option for checking on password format
                 PGE_DBPasswordManagement GETPASSWORD <USERNAME>@<DBINSTANCENAME>

 * Use this option to get SDE file created and get path as return value
                 PGE_DBPasswordManagement GETSDEFILE <USERNAME>@<DBINSTANCENAME>

 * Use this option to add single new entry to Connections.xml file
                 PGE_DBPasswordManagement ADMINADD <USERNAME>@"<Password>"@<DBINSTANCENAME>

 * Use this option to add multple new entry to Connections.xml file. Pass path of CSV without Header with USERNAME,Password,INSTANCE
                 PGE_DBPasswordManagement ADMINADDF <FilePath>

 * Use this option to remove one entry from Connections.xml file
                 PGE_DBPasswordManagement ADMINREMOVE <USERNAME>@<Password>@<DBINSTANCENAME>

 * Use PGE_DBPasswordManagement FORMAT for checking on password format