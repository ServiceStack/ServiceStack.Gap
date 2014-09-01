SET TOOLS=..\..\..\tools
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\Chat\bin\Release
SET INPUT=%RELEASE%\Chat.exe
SET INPUT=%INPUT% %RELEASE%\Chat.Resources.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Text.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Client.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Common.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Interfaces.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Server.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.OrmLite.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Redis.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Razor.dll
SET INPUT=%INPUT% %RELEASE%\System.Web.Razor.dll

%ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:Chat.exe /ndebug %INPUT% 

COPY "..\Chat\appsettings.txt" .

%TOOLS%\7za.exe a Chat.zip Chat.exe appsettings.txt README.txt test-fanout-redis-events.bat test-fanout-redis-events.sh

MOVE Chat.zip ..\..\..\deploy
