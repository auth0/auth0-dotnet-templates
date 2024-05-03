rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./Auth0BlazorWebApp/registration/cli-wrapper.dll
rem #else
dotnet ./Auth0BlazorWebApp/registration/cli-wrapper.dll
rem #endif
