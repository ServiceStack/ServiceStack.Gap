SET TOOLS=..\..\..\tools
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\BenchmarksAnalyzer\bin\x86\Release
SET INPUT=%RELEASE%\BenchmarksAnalyzer.exe
SET INPUT=%INPUT% %RELEASE%\BenchmarksAnalyzer.Resources.dll
SET INPUT=%INPUT% %RELEASE%\BenchmarksAnalyzer.ServiceInterface.dll
SET INPUT=%INPUT% %RELEASE%\BenchmarksAnalyzer.ServiceModel.dll
SET INPUT=%INPUT% %RELEASE%\Ionic.Zip.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Text.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Client.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Common.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Interfaces.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.OrmLite.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.OrmLite.Sqlite.dll
SET INPUT=%INPUT% %RELEASE%\Mono.Data.Sqlite.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Razor.dll
SET INPUT=%INPUT% %RELEASE%\System.Web.Razor.dll

%ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:BenchmarksAnalyzer.exe /ndebug %INPUT% 

COPY "%RELEASE%\server.labels" .
COPY "%RELEASE%\test.labels" .
COPY "%RELEASE%\Database Performance in ASP.NET.zip" .
COPY %TOOLS%\ab.exe .

%TOOLS%\7za.exe a BenchmarksAnalyzer.zip BenchmarksAnalyzer.exe server.labels test.labels "Database Performance in ASP.NET.zip" ab.exe README.txt

MOVE BenchmarksAnalyzer.zip ..\..\..\deploy
