using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarksAnalyzer.ServiceInterface;
using BenchmarksAnalyzer.ServiceModel;
using BenchmarksAnalyzer.ServiceModel.Types;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.Host;
using ServiceStack.OrmLite;
using ServiceStack.Razor;
using ServiceStack.Text;
using ServiceStack.VirtualPath;
using ServiceStack.Web;

namespace BenchmarksAnalyzer
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost(string baseUrl)
            : base("HTTP Benchmarks Analyzer", typeof(WebServices).Assembly)
        {
            BaseUrl = baseUrl;
        }

        public override void Configure(Container container)
        {
            Plugins.Add(new RequestLogsFeature());
            Plugins.Add(new CorsFeature());
            Plugins.Add(new PostmanFeature());
            Plugins.Add(new RazorFormat
            {
                LoadFromAssemblies = { typeof(Resources.BaseTypeMarker).Assembly },
            });

            SetConfig(new HostConfig {
                DebugMode = true,
                EmbeddedResourceBaseTypes = { GetType(), typeof(Resources.BaseTypeMarker) },
            });

            container.Register<IDbConnectionFactory>(c =>
                new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));

            ImportData();
        }

        private void ImportData()
        {
            var appSettings = File.Exists("~/app.settings".MapAbsolutePath())
                ? new TextFileSettings("~/app.settings".MapAbsolutePath())
                : new DictionarySettings();

            var serverLabelsPath = "~/server.labels".MapAbsolutePath();
            var serverLabels = File.Exists(serverLabelsPath)
                ? serverLabelsPath.ReadAllText().ParseKeyValueText(delimiter: " ")
                : null;

            var testLabelsPath = "~/test.labels".MapAbsolutePath();
            var testLabels = File.Exists(testLabelsPath)
                ? testLabelsPath.ReadAllText().ParseKeyValueText(delimiter: " ")
                : null;

            using (var admin = Resolve<AdminServices>())
            {
                var db = admin.Db;
                db.DropAndCreateTable<TestPlan>();
                db.DropAndCreateTable<TestRun>();
                db.DropAndCreateTable<TestResult>();

                const int planId = 1;
                admin.CreateTestPlan(new TestPlan {
                    Id = planId,
                    Name = appSettings.Get("TestPlanName", "Benchmarks"),
                    ServerLabels = serverLabels,
                    TestLabels = testLabels,
                });

                var testRun = admin.CreateTestRun(planId);

                var dir = new FileSystemVirtualPathProvider(this, Config.WebHostPhysicalPath);
                var files = dir.GetAllMatchingFiles("*.txt")
                    .Concat(dir.GetAllMatchingFiles("*.zip"));

                admin.Request = new BasicRequest
                {
                    Files = files.Map(x => new HttpFile {
                        ContentLength = x.Length,
                        ContentType = MimeTypes.GetMimeType(x.Name),
                        FileName = x.Name,
                        InputStream = x.OpenRead(),
                    } as IHttpFile).ToArray()
                };

                if (admin.Request.Files.Length > 0)
                {
                    admin.Post(new UploadTestResults
                    {
                        TestPlanId = 1,
                        TestRunId = testRun.Id,
                        CreateNewTestRuns = true,
                    });
                }
            }
        }

        public string BaseUrl { get; set; }

        public AppHost Start()
        {
            Init();
            var listenUrl = BaseUrl.Replace("localhost", "*").Replace("127.0.0.1", "*");
            Start(listenUrl);
            return this;
        }

        public string GetStartUrl()
        {
            using (var db = Resolve<IDbConnectionFactory>().Open())
            {
                var testResult = db.Single<TestResult>(q => q.OrderBy(x => x.Id));
                var testPlan = testResult != null ? db.SingleById<TestPlan>(testResult.TestPlanId) : null;

                return testPlan != null
                    ? BaseUrl.CombineWith("{0}?id={1}".Fmt(testPlan.Slug, testResult.TestRunId)) 
                    : BaseUrl;
            }
        }
    }

    public static class AppUtils
    {
        public static void ExportMonoSqliteDll()
        {
            if (Env.IsMono)
                return; //Uses system sqlite3.so or sqlite3.dylib

            var resPath = "{0}.sqlite3.dll".Fmt(typeof(AppHost).Namespace);

            var resInfo = typeof(AppHost).Assembly.GetManifestResourceInfo(resPath);
            if (resInfo == null)
                throw new Exception("Couldn't load sqlite3.dll");

            var dllBytes = typeof(AppHost).Assembly.GetManifestResourceStream(resPath).ReadFully();
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filePath = Path.Combine(dirPath, "sqlite3.dll");

            File.WriteAllBytes(filePath, dllBytes);
        }        
    }
}