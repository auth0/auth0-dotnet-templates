rem #if (OS != "Windows_NT")
#!/bin/sh
dotnet ./registration/cli-wrapper.dll
rem #else
dotnet registration/cli-wrapper.dll
rem #endif
