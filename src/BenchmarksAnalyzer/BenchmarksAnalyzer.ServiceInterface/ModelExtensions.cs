using BenchmarksAnalyzer.ServiceModel.Types;
using ServiceStack;

namespace BenchmarksAnalyzer.ServiceInterface
{
    public static class ModelExtensions
    {
        public static DisplayResult ToDisplayResult(this TestResult from)
        {
            var to = from.ConvertTo<DisplayResult>();
            to.Host = from.Hostname;
            return to;
        }
    }
}