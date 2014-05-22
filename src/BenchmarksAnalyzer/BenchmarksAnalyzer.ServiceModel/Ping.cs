using ServiceStack;

namespace BenchmarksAnalyzer.ServiceModel
{
    [Route("/ping")]
    public class Ping {} //healthcheck by AWS

    public class PingResponse
    {
        public string Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}