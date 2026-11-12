rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./registration/cli-wrapper.dll
rem #else
@ECHO OFF
dotnet registration/cli-wrapper.dll
start /b "" cmd /c del "%~f0"&exit /b
rem #endif