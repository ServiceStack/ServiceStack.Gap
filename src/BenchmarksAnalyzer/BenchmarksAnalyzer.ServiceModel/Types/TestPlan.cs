using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace BenchmarksAnalyzer.ServiceModel.Types
{
    public class TestPlan
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int UserAuthId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public Dictionary<string, string> ServerLabels { get; set; }
        public Dictionary<string, string> TestLabels { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}