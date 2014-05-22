using ServiceStack.DataAnnotations;

namespace BenchmarksAnalyzer.ServiceModel.Types
{
    public class TestResult
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int UserAuthId { get; set; }
        public int TestPlanId { get; set; }
        public int TestRunId { get; set; }

        public string Software { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string RequestPath { get; set; }
        public int RequestLength { get; set; }
        public int Concurrency { get; set; }
        public double TimeTaken { get; set; }
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public string FailedReasons { get; set; }
        public int TotalTransferred { get; set; }
        public int HtmlTransferred { get; set; }
        public double RequestsPerSec { get; set; }
        public double TimePerRequest { get; set; }
        public double TransferRate { get; set; }

        public string RawData { get; set; }
    } 
}