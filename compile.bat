@ECHO OFF
SETLOCAL EnableDelayedExpansion

:: msbuild path
SET vsInstallerRoot=%programfiles(x86)%\Microsoft Visual Studio\Installer
FOR /f "usebackq tokens=1* delims=: " %%i IN (`"%vsInstallerRoot%\vswhere.exe" -latest -prerelease -requires Microsoft.Component.MSBuild`) DO (
  IF /i "%%i"=="installationPath" SET vsRoot=%%j
)

SET MSBuild="%vsRoot%\MSBuild\Current\Bin\MSBuild.exe"

:: msbuild params
SET MSBuildParam=
SET MSBuildParam=%MSBuildParam% /maxcpucount
SET MSBuildParam=%MSBuildParam% /nologo
SET MSBuildParam=%MSBuildParam% /nr:false
SET MSBuildParam=%MSBuildParam% /p:AllowedReferenceRelatedFileExtensions=none
SET MSBuildParam=%MSBuildParam% /property:Configuration=Release
SET MSBuildParam=%MSBuildParam% /target:Build
SET MSBuildParam=%MSBuildParam% /verbosity:Normal

SET Solutions="%~dp0\*.sln"

FOR %%a IN (%Solutions%) DO (
	%MSBuild% %MSBuildParam% %%~fa
	IF NOT %errorlevel%==0 PAUSE
)

ENDLOCAL
EXIT /b
