using System.Collections.Generic;
using System.Linq;
using BenchmarksAnalyzer.ServiceModel;
using BenchmarksAnalyzer.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;

namespace BenchmarksAnalyzer.ServiceInterface
{
    public class WebServices : Service
    {
        public TestPlan Any(GetTestPlan request)
        {
            var plan = Db.SingleById<TestPlan>(request.Id);

            if (plan == null)
                throw HttpError.NotFound("Plan {0} does not exist".Fmt(request.Id));

            return plan;
        }

        public object Any(SearchTestResults request)
        {
            var testRun = request.TestRunId != null
                ? Db.SingleById<TestRun>(request.TestRunId.Value)
                : Db.Select<TestRun>(q => q
                    .Where(x => x.TestPlanId == request.TestPlanId)
                    .OrderByDescending(x => x.Id))
                    .FirstOrDefault();

            if (testRun == null)
                return new List<TestResult>();

            var query = Db.From<TestResult>()
                .Where(q => q.TestPlanId == request.TestPlanId
                    && q.TestRunId == testRun.Id);

            if (request.Host != null)
                query.Where(q => q.Hostname.Contains(request.Host));
            if (request.Port != null)
                query.Where(q => q.Port == request.Port);
            if (request.RequestPath != null)
                query.Where(q => q.RequestPath.Contains(request.RequestPath));

            var results = Db.Select(query.Limit(request.Skip, request.Take));
            var total = Db.Count(query);

            var response = request.ConvertTo<SearchTestResultsResponse>();
            response.Total = (int)total;
            response.Results = results.ConvertAll(x => x.ToDisplayResult());

            return response;
        }

        public object Any(ViewTestPlan request)
        {
            var testPlan = Db.Single<TestPlan>(q => q.Slug == request.Slug);

            if (testPlan == null)
                throw HttpError.NotFound(request.Slug);

            var testRun = request.Id != null
                ? Db.Single<TestRun>(x =>
                    x.TestPlanId == testPlan.Id && x.Id == request.Id)
                : Db.Select<TestRun>(q =>
                        q.Where(x => x.TestPlanId == testPlan.Id)
                    .OrderByDescending(x => x.Id))
                    .FirstOrDefault();

            var testResults = Db.Select<TestResult>(q => q.TestRunId == testRun.Id);

            return new ViewTestPlanResponse
            {
                TestPlan = testPlan,
                TestRun = testRun,
                Results = testResults.ConvertAll(x => x.ToDisplayResult())
            };
        }
    }
}