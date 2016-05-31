@ECHO OFF
SETLOCAL

SET remove=obj,out

FOR /d /r %%d IN (%remove%) DO IF EXIST "%%d" (
	ECHO Remove: %%d
	RMDIR /s /q "%%d"
)

ENDLOCAL