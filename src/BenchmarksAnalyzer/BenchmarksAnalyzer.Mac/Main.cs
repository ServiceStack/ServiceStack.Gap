using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace BenchmarksAnalyzer.Mac
{
    public static class MainClass
    {
        public static AppHost App;

        static void Main (string[] args)
        {
            App = new AppHost("http://localhost:1337/")
                .Start();

            NSApplication.Init ();
            NSApplication.Main (args);
        }
    }
}

