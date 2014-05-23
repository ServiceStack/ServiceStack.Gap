SET TOOLS=..\..\..\tools
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\BenchmarksAnalyzer.WinForms\bin\x86\Release
SET INPUT=%RELEASE%\BenchmarksAnalyzer.WinForms.exe
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

REM Fails to load CEF from ILMerged .exe
REM %ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:BenchmarksAnalyzer.Windows.exe /ndebug %INPUT% 

SET INPUT=%INPUT% %RELEASE%\sqlite3.dll
SET INPUT=%INPUT% %RELEASE%\CefSharp.BrowserSubprocess.exe
SET INPUT=%INPUT% %RELEASE%\icudt.dll
SET INPUT=%INPUT% %RELEASE%\libcef.dll
SET INPUT=%INPUT% %RELEASE%\cef.pak 
SET INPUT=%INPUT% %RELEASE%\devtools_resources.pak
SET INPUT=%INPUT% %RELEASE%\CefSharp.dll
SET INPUT=%INPUT% %RELEASE%\CefSharp.Core.dll
SET INPUT=%INPUT% %RELEASE%\CefSharp.WinForms.dll

%TOOLS%\7za.exe a BenchmarksAnalyzer.Windows.zip server.labels test.labels "Database Performance in ASP.NET.zip" ab.exe README.txt %INPUT%       
%TOOLS%\7za.exe a -r BenchmarksAnalyzer.Windows.zip %RELEASE%\locales locales

MOVE BenchmarksAnalyzer.Windows.zip ..\..\..\deploy
