using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue;
using Xilium.CefGlue.WindowsForms;

namespace demo
{
    public class fBrowser : Form
    {
        //const string url = "https://dictionary.cambridge.org/grammar/british-grammar/modals-and-modality/would";
        const string url = "about:blank";

        CefWebBrowser browserCtl;
         

        public fBrowser()
        {

            ////////////CefRuntime.Load();

            ////////////var settings = new CefSettings();
            //////////////settings.MultiThreadedMessageLoop = MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;            
            ////////////settings.SingleProcess = false;
            ////////////settings.LogSeverity = CefLogSeverity.Verbose;
            ////////////settings.LogFile = "cef.log";
            ////////////settings.ResourcesDirPath = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);
            ////////////settings.RemoteDebuggingPort = 20480;
            ////////////settings.NoSandbox = true;

            ////////////var mainArgs = new CefMainArgs(new string[] { });
            ////////////var app = new DemoCefApp();
            ////////////CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);


            ////////////browserCtl = new CefWebBrowser();
            //////////////tabPage.Tag = browserCtl;
            //////////////browserCtl.Parent = tabPage;
            ////////////this.Tag = browserCtl;
            ////////////browserCtl.Parent = this;
            ////////////browserCtl.Dock = DockStyle.Fill;
            ////////////browserCtl.BringToFront();

            ////////////var browser = browserCtl.WebBrowser;
            ////////////browser.StartUrl = url;

            ////////////this.Controls.Add(browserCtl);

            browserCtl = new CefWebBrowser
            {
                Dock = DockStyle.Fill
            };
            browserCtl.Parent = this;
            this.Controls.Add(browserCtl);

            this.Shown += (se, ev) =>
            {
                this.FormClosing += FBrowser_FormClosing;
                browserCtl.Browser.GetMainFrame().LoadUrl("https://google.com.vn");
                //browserCtl.Browser.GetMainFrame().LoadUrl("https://localhost:60000/index.html");

                // wait a bit
                for (int i = 0; i < 25; ++i)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(10);
                }
            };
        }

        private void FBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (browserCtl != null)
                browserCtl.Dispose();

            ////////CefRuntime.Shutdown();
            ////////GC.SuppressFinalize(this);
        }

        ~fBrowser()
        {
        }
    }

    internal sealed class DemoCefApp : CefApp
    {
        //private CefBrowserProcessHandler _browserProcessHandler = new DemoBrowserProcessHandler();
        //private CefRenderProcessHandler _renderProcessHandler = new DemoRenderProcessHandler();

        //protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        //{
        //    Console.WriteLine("OnBeforeCommandLineProcessing: {0} {1}", processType, commandLine);

        //    // TODO: currently on linux platform location of locales and pack files are determined
        //    // incorrectly (relative to main module instead of libcef.so module).
        //    // Once issue http://code.google.com/p/chromiumembedded/issues/detail?id=668 will be resolved
        //    // this code can be removed.
        //    if (CefRuntime.Platform == CefRuntimePlatform.Linux)
        //    {
        //        var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
        //        path = Path.GetDirectoryName(path);

        //        commandLine.AppendSwitch("resources-dir-path", path);
        //        commandLine.AppendSwitch("locales-dir-path", Path.Combine(path, "locales"));
        //    }
        //}

        //protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        //{
        //    return _browserProcessHandler;
        //}

        //protected override CefRenderProcessHandler GetRenderProcessHandler()
        //{
        //    return _renderProcessHandler;
        //}
    }
}
