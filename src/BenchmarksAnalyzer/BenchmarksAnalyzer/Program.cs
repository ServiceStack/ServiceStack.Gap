using System.Diagnostics;
using System.Threading;
using ServiceStack.Text;

namespace BenchmarksAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppUtils.ExportMonoSqliteDll();

            var appHost = new AppHost("http://localhost:1337/")
                .Start();

            "Listening on {0}".Print(appHost.BaseUrl);
            "Type Ctrl+C to quit..\n".Print();

            var startUrl = appHost.GetStartUrl();
            if (startUrl == appHost.BaseUrl)
            {
                "No .txt or .zip Apache Benchmark Results found, skipping initial import...".Print();
                "Use ab utility to generate Apache Benchmark .txt files to analyze.".Print();
            }

            Process.Start(startUrl);
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
