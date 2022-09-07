@ECHO OFF
setlocal enabledelayedexpansion
SET PROG=%1
SET RETURN=0

SET DOTS=.............................................................................
SET LINE=-----------------------------------------------------------------------------

ECHO.
cd /d D:\edgisdbmaint
ECHO %LINE% 
ECHO Run at: %DATE% %TIME%
ECHO As: %USERNAME%
ECHO On system: %COMPUTERNAME%

SET CMD=%1 %2 %3 %4 %5 %6 %7 %8 %9

ECHO.

ECHO Using: %0  %CMD%

ECHO %LINE%

ECHO.

if "%PROG%"=="" (
  ECHO "No command line argumants given, exiting...
  SET RETURN=1
  GOTO End
)

for /f "tokens=1,2,3,4,5 delims=_" %%a in ("%1%") do (
  set PT1=%%a
  set PT2=%%b
  set PT3=%%c
  set PARAM1=%%d
  set PARAM2=%%e
)

set TLM_PROG=%PT1%_%PT2%_%PT3%_RUN.bat

if "%TLM_PROG%"=="ED_TLM_MONTHLY_RUN.bat" (
  echo %LINE%
  echo Executing %TLM_PROG% %PARAM1% %PARAM2% ...       
  call %TLM_PROG% %PARAM1% %PARAM2%
  set RETURN=!ERRORLEVEL!
  GOTO End
)

echo %LINE%
echo Executing %PROG%.bat ...       
call %PROG%.bat
set RETURN=%ERRORLEVEL%


:End
  ECHO.
  ECHO %LINE%
  ECHO Ended at: %DATE% %TIME%
  ECHO Return Code: %RETURN%
  ECHO %LINE%
  ECHO.
rem uc4 likely needs this...
exit %RETURN%
