using System.Collections.Generic;
using BenchmarksAnalyzer.ServiceModel.Types;
using ServiceStack;

namespace BenchmarksAnalyzer.ServiceModel
{
    [Route("/testplans", "POST")]
    public class CreateTestPlan : IReturn<TestPlan>
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    [Route("/testplans/{Id}/delete", "POST DELETE")]
    public class DeleteTestPlan
    {
        public int Id { get; set; }
    }

    [Route("/testplans/{Id}/labels", "POST")]
    public class UpdateTestPlanLabels : IReturn<TestPlan>
    {
        public int Id { get; set; }
        public string ServerLabels { get; set; }
        public string TestLabels { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/testplans", "GET")]
    public class FindTestPlans : IReturn<List<TestPlan>> { }

    [Route("/testplans/{Id}")]
    public class GetTestPlan : IReturn<TestPlan>
    {
        public int Id { get; set; }
    }
}