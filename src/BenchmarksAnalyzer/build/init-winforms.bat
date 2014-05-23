REM COPY cef dlls to WinForms working directory

SET CEF=..\..\..\lib\cef

COPY %CEF%\* ..\BenchmarksAnalyzer.WinForms\bin\x86\Release

MD ..\BenchmarksAnalyzer.WinForms\bin\x86\Release\locales

COPY %CEF%\locales\* ..\BenchmarksAnalyzer.WinForms\bin\x86\Release\locales

