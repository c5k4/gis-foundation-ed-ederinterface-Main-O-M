@echo off
set path_dir=%~dp0
set drive=%path_dir:~0,2%
%drive%
cd %path_dir%


echo %1
for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 DB_SERVER') do (
    set dbser=%%a
)

for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 UX_USER') do (
    set uuser=%%a
)

for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 UX_PASS') do (
    set upass=%%a
)

for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 UX_EXPATH') do (
    set upath=%%a
)

for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 DB_USER') do (
    set dbuser=%%a
)

for /f "delims=" %%a in ('call read_ini.bat FFExpCopy.ini %1 DB_PASS') do (
    set dbpass=%%a
)

rem echo  %dbser% %uuser% %upass% %upath% %dbser% 

FFExport_DB_singledump.bat %1 %uuser% %upass% %upath% %dbser% %dbuser% %dbpass% S0



