using System;
using ServiceStack.DataAnnotations;

namespace BenchmarksAnalyzer.ServiceModel.Types
{
    public class TestRun
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int UserAuthId { get; set; }
        public int TestPlanId { get; set; }
        public string SeriesId { get; set; }
        public DateTime CreatedDate { get; set; }

        [Ignore]
        public int TestResultsCount { get; set; }
    }
}