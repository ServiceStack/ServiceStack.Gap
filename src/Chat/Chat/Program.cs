using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            LogManager.LogFactory = new ConsoleLogFactory();
#endif

            var help = new HashSet<string> { "-h", "--help", "/h", "/help" };
            if (args.Any(help.Contains))
            {
                "Usage: Chat.exe                          # Run using default appsettings.txt".Print();
                "Usage: Chat.exe /port=1337               # Run on port 1337".Print();
                "Usage: Chat.exe /redis=localhost:6379    # Run using RedisServerEvents".Print();
                "Usage: Chat.exe /background=/img/bg.jpg  # Run using background image".Print();
                return;
            }

            //default with settings defined in appsettings.txt
            var customSettings = new FileInfo("appsettings.txt");
            var appSettings = new DictionarySettings();
            if (customSettings.Exists)
                appSettings = new TextFileSettings(customSettings.FullName);
            else
                ExportFile(customSettings.Name);

            var port = appSettings.Get("port", 1337);
            var redisHost = appSettings.GetString("redis");
            var background = appSettings.GetString("background");

            //override with command line args if any
            var portArg = args.FirstOrDefault(x => x.Contains("port"));
            if (portArg != null)
                int.TryParse(portArg.SplitOnFirst('=').Last(), out port);

            var redisArg = args.FirstOrDefault(x => x.Contains("redis"));
            var useRedis = redisArg != null;
            if (useRedis)
            {
                var parts = redisArg.SplitOnFirst('=');
                redisHost = parts.Length > 1 ? parts[1] : (redisHost ?? "localhost");
            }

            var backgroundArg = args.FirstOrDefault(x => x.Contains("background"));
            if (backgroundArg != null)
            {
                var parts = backgroundArg.SplitOnFirst('=');
                background = parts.Length > 1 ? parts[1] : background;
            }

            new AppHost {
                    RedisHost = redisHost,
                    Background = background,
                }
                .Init()
                .Start("http://*:{0}/".Fmt(port));

            var url = "http://localhost:{0}/".Fmt(port);
            "Listening on {0} with {1} on {2}".Print(url, redisHost != null ? "RedisServerEvents on " + redisHost : "MemoryServerEvents", background);
            "Type Ctrl+C to quit..\n".Print();
            Process.Start(url);
            Thread.Sleep(Timeout.Infinite);
        }

        static void ExportFile(string fileName)
        {
            var settingsPath = "{0}.{1}".Fmt(typeof(AppHost).Namespace, fileName);

            var settingsInfo = typeof(AppHost).Assembly.GetManifestResourceInfo(settingsPath);
            if (settingsInfo == null)
                return;

            var settingsContents = typeof(AppHost).Assembly.GetManifestResourceStream(settingsPath).ReadFully();

            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filePath = Path.Combine(dirPath, fileName);

            File.WriteAllBytes(filePath, settingsContents);
        }
    }
}
