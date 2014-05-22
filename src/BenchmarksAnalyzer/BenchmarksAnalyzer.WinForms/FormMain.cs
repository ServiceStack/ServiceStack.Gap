using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace BenchmarksAnalyzer.WinForms
{
    public partial class FormMain : Form
    {
        private readonly WebView webView;

        public FormMain(string loadUrl)
        {
            InitializeComponent();

            Text = "Apache Benchmarks Analyzer";
            WindowState = FormWindowState.Maximized;

            webView = new WebView(loadUrl)
            {
                Dock = DockStyle.Fill,
            };
            this.panelMain.Controls.Add(webView);
        }
    }
}
