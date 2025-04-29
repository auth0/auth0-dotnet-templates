rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./Client/registration/cli-wrapper.dll
dotnet ./Server/registration/cli-wrapper.dll
rem #else
@ECHO OFF
dotnet Client/registration/cli-wrapper.dll
dotnet Server/registration/cli-wrapper.dll
start /b "" cmd /c del "%~f0"&exit /b
rem #endif
