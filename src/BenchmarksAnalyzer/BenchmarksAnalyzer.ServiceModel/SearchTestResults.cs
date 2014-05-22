using System.Collections.Generic;
using BenchmarksAnalyzer.ServiceModel.Types;
using ServiceStack;
using ServiceStack.Text;

namespace BenchmarksAnalyzer.ServiceModel
{
    [Route("/testplans/{TestPlanId}/results", "GET")]
    [Route("/testplans/{TestPlanId}/testruns/{TestRunId}/results", "GET")]
    public class SearchTestResults : IReturn<SearchTestResultsResponse>
    {
        public int TestPlanId { get; set; }
        public int? TestRunId { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string RequestPath { get; set; }
    }

    [Csv(CsvBehavior.FirstEnumerable)]
    public class SearchTestResultsResponse
    {
        public int TestPlanId { get; set; }
        public int? TestRunId { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string RequestPath { get; set; }

        public int Total { get; set; }

        public List<DisplayResult> Results { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }
}