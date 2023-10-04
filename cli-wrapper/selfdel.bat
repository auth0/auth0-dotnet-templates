@ECHO OFF
TIMEOUT /t 1 /nobreak > NUL
TASKKILL /pid %1 > NUL
RMDIR /S /Q %2
