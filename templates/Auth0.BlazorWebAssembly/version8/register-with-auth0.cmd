rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./Client/registration/cli-wrapper.dll
dotnet ./Server/registration/cli-wrapper.dll
rem #else
dotnet Client/registration/cli-wrapper.dll
dotnet Server/registration/cli-wrapper.dll
rem #endif
