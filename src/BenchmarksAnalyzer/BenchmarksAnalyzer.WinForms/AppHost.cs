using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarksAnalyzer.Resources;
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

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var exe = args[0];
                var arg = args[1];

                var help = new HashSet<string> { "-h", "--help", "/h", "/help" };
                if (help.Contains(arg))
                {
                    "Usage: {0}                   # Imports files in current dir".Print(exe);
                    "Usage: {0} /path/to/dir      # Imports files in target dir".Print(exe);
                    "Usage: {0} /path/to/file.zip # Imports files in target .zip".Print(exe);
                }
                else if (File.Exists(arg))
                {
                    BaseDir = Path.GetDirectoryName(arg);
                    UploadFile = Path.GetFileName(arg);
                }
                else if (Directory.Exists(arg))
                {
                    BaseDir = arg;
                }
                else
                {
                    "WARN: {0} is not a valid File or Directory path".Print(arg);
                }
            }
        }

        public override void Configure(Container container)
        {
            Plugins.Add(new RequestLogsFeature());
            Plugins.Add(new CorsFeature());
            Plugins.Add(new PostmanFeature());
            Plugins.Add(new RazorFormat
            {
                LoadFromAssemblies = { typeof(BaseTypeMarker).Assembly },
            });

            SetConfig(new HostConfig
            {
                DebugMode = true,
                EmbeddedResourceBaseTypes = { GetType(), typeof(BaseTypeMarker) },
            });

            container.Register<IDbConnectionFactory>(c =>
                new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));

            ImportData();
        }

        private void ImportData()
        {
            var dir = new FileSystemVirtualPathProvider(this, BaseDir ?? Config.WebHostPhysicalPath);

            var fileSettings = dir.GetFile("app.settings");
            var appSettings = fileSettings != null
                ? new DictionarySettings(fileSettings.ReadAllText().ParseKeyValueText(delimiter: " "))
                : new DictionarySettings();

            var fileServerLabels = dir.GetFile("server.labels");
            var serverLabels = fileServerLabels != null
                ? fileServerLabels.ReadAllText().ParseKeyValueText(delimiter: " ")
                : null;

            var fileTestLabels = dir.GetFile("test.labels");
            var testLabels = fileTestLabels != null
                ? fileTestLabels.ReadAllText().ParseKeyValueText(delimiter: " ")
                : null;

            using (var admin = Resolve<AdminServices>())
            {
                var db = admin.Db;
                db.DropAndCreateTable<TestPlan>();
                db.DropAndCreateTable<TestRun>();
                db.DropAndCreateTable<TestResult>();

                const int planId = 1;
                admin.CreateTestPlan(new TestPlan
                {
                    Id = planId,
                    Name = appSettings.Get("TestPlanName", "Benchmarks"),
                    ServerLabels = serverLabels,
                    TestLabels = testLabels,
                });

                var testRun = admin.CreateTestRun(planId);


                var files = UploadFile != null
                    ? dir.GetAllMatchingFiles(UploadFile)
                    : dir.GetAllMatchingFiles("*.txt")
                        .Concat(dir.GetAllMatchingFiles("*.zip"));

                admin.Request = new BasicRequest
                {
                    Files = files.Map(x => new HttpFile
                    {
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
        public string BaseDir { get; set; }
        public string UploadFile { get; set; }

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