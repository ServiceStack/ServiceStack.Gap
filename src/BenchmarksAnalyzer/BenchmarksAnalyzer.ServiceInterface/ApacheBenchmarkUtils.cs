using System.IO;
using System.Text;
using BenchmarksAnalyzer.ServiceModel.Types;
using ServiceStack;

namespace BenchmarksAnalyzer.ServiceInterface
{
    public static class ApacheBenchmarkUtils
    {
        public static TestResult ToTestResult(this FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return ToTestResult(stream);
            }
        }

        public static TestResult ToTestResult(this string fileContents)
        {
            using (var stream = new MemoryStream(fileContents.ToUtf8Bytes()))
            {
                return ToTestResult(stream);
            }
        }

        public static TestResult ToTestResult(this Stream stream)
        {
            var to = new TestResult();
            bool reachedStats = false;
            var sb = new StringBuilder();

            foreach (var line in stream.ReadLines())
            {
                sb.AppendLine(line);

                if (!ParseLine(line, to, ref reachedStats))
                    break;
            }

            to.RawData = sb.ToString();

            return to;
        }

        private static bool ParseLine(string line, TestResult to, ref bool reachedStats)
        {
            if (line.Trim().Length == 0)
                return true;

            var startOfStats = line.StartsWith("Server Software:");
            if (!reachedStats && !startOfStats)
                return true;

            if (startOfStats)
                reachedStats = true;

            if (line.StartsWith("Connection Times (ms)"))
                return true;

            var parts = line.SplitOnFirst(':');
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "Server Software":
                        to.Software = value;
                        break;
                    case "Server Hostname":
                        to.Hostname = value;
                        break;
                    case "Server Port":
                        to.Port = value.ToInt();
                        break;
                    case "Document Path":
                        to.RequestPath = value;
                        break;
                    case "Document Length":
                        to.RequestLength = value.SplitOnFirst(' ')[0].ToInt();
                        break;
                    case "Concurrency Level":
                        to.Concurrency = value.ToInt();
                        break;
                    case "Time taken for tests":
                        to.TimeTaken = double.Parse(value.SplitOnFirst(' ')[0]);
                        break;
                    case "Complete requests":
                        to.TotalRequests = value.ToInt();
                        break;
                    case "Failed requests":
                        to.FailedRequests = value.ToInt();
                        break;
                    case "Total transferred":
                        to.TotalTransferred = value.SplitOnFirst(' ')[0].ToInt();
                        break;
                    case "HTML transferred":
                        to.HtmlTransferred = value.SplitOnFirst(' ')[0].ToInt();
                        break;
                    case "Requests per second":
                        to.RequestsPerSec = double.Parse(value.SplitOnFirst(' ')[0]);
                        break;
                    case "Time per request":
                        to.TimePerRequest = double.Parse(value.SplitOnFirst(' ')[0]); //gets overwritten with avg per concurrent request, i.e. what we want
                        break;
                    case "Transfer rate":
                        to.TransferRate = double.Parse(value.SplitOnFirst(' ')[0]);
                        break;
                }
            
                return true;
            }
            
            if (line.Trim().StartsWith("(Connect:"))
            {
                to.FailedReasons = line.Trim();
                return true;
            }

            return false;
        }
    }
}