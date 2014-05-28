using System;
using System.Windows.Forms;
using BenchmarksAnalyzer.WinForms;
using CefSharp;

namespace BenchmarksAnalyzer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Cef.Initialize(new CefSettings());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var appHost = new AppHost("http://localhost:1337/")
                .Start();

            Application.Run(new FormMain(appHost.GetStartUrl()));
        }
    }
}
