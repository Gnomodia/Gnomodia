:: Copies Gnomoria.exe and gnomorialib.dll to <root>\lib\Gnomoria\.
:: Looks in %PROGRAMFILES(X86)% by default. Pass the path to set a
:: custom path.
::
:: E.g.:
::  > dev_setup
:: Or
::  > dev_setup E:\Games\Steam

@ECHO OFF

SET _STEAM_ROOT=%PROGRAMFILES(X86)%/Steam
IF NOT "%1"=="" (
	SET _STEAM_ROOT=%1
)

xcopy "%_STEAM_ROOT%\SteamApps\common\Gnomoria\Gnomoria.exe" %~dp0lib\Gnomoria\ /S /Y /I

@ECHO.

xcopy "%_STEAM_ROOT%\SteamApps\common\Gnomoria\gnomorialib.dll" %~dp0lib\Gnomoria\ /S /Y /I