rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./Auth0BlazorWebApp/registration/cli-wrapper.dll
rem #else
@ECHO OFF
dotnet ./Auth0BlazorWebApp/registration/cli-wrapper.dll
start /b "" cmd /c del "%~f0"&exit /b
rem #endif
