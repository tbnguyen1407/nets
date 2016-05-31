@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET MSBuild="%programfiles(x86)%\MSBuild\15.0\Bin\MSBuild.exe"

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
PAUSE
EXIT /b