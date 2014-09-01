RMDIR ..\Chat /s /q
MD ..\Chat
XCOPY /E ..\..\..\..\Chat\src\Chat ..\Chat
RMDIR ..\Chat\bin /s /q
RMDIR ..\Chat\obj /s /q
DEL ..\Chat\*.user