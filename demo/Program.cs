using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Xilium.CefGlue;

namespace demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            //////string execDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //////CefRuntime.Load();
            //////CefSettings settings = new CefSettings
            //////{
            //////    SingleProcess = System.Diagnostics.Debugger.IsAttached,
            //////    MultiThreadedMessageLoop = true,
            //////    LogSeverity = CefLogSeverity.Verbose,
            //////    LogFile = "cef.log",
            //////    //AutoDetectProxySettingsEnabled = true,        
            //////    ResourcesDirPath = execDir,
            //////    //BrowserSubprocessPath = Path.Combine(execDir, "cefclient.exe"),
            //////};


            string execDir1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string execDir2 = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);

            execDir2 = Path.Combine(execDir2, "Debugx86") + @"\";

            CefRuntime.Load(execDir2);

            var settings = new CefSettings();
            //settings.MultiThreadedMessageLoop = MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;            
            settings.MultiThreadedMessageLoop = true;
            settings.SingleProcess = true;
            settings.LogSeverity = CefLogSeverity.Verbose;
            settings.LogFile = "cef.log";
            //settings.ResourcesDirPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);
            settings.ResourcesDirPath = execDir2;
            settings.RemoteDebuggingPort = 20480;
            settings.NoSandbox = true;

            CefMainArgs mainArgs = new CefMainArgs(new string[] { });
            CefApp app = new TestApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
            Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            if (exitCode != -1)
                return exitCode;

            //////// guard if something wrong
            //////foreach (var arg in args) { if (arg.StartsWith("--type=")) { return -2; } }

            //////CefRuntime.Initialize(mainArgs, settings, app);
            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);


            if (!settings.MultiThreadedMessageLoop)
            {
                Application.Idle += (sender, e) => CefRuntime.DoMessageLoopWork();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fApp());

            CefRuntime.Shutdown();

            return 0;
        }
    }

    public class RenderProcessHandler : CefRenderProcessHandler
    {
        #region Private/Protected Methods

        protected override void OnBrowserDestroyed(CefBrowser browser) { }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            ////////////if (message.Name == RenderProcessMessages.VisitDom)
            ////////////{
            ////////////    string visitorId = message.Arguments.GetString(0);
            ////////////    lock (DomVisitor.Visitors)
            ////////////    {
            ////////////        DomVisitor visitor = null;
            ////////////        if (DomVisitor.Visitors.TryGetValue(visitorId, out visitor))
            ////////////        {
            ////////////            browser.GetMainFrame().VisitDom(visitor);
            ////////////            return true;
            ////////////        }
            ////////////    }
            ////////////}

            //return base.OnProcessMessageReceived(browser, sourceProcess, message);

            return true;
        }

        #endregion
    }

    public static class RenderProcessMessages
    {
        #region Public/Internal Properties

        public static string VisitDom
        {
            get { return "Renderer.Messages.VisitDom"; }
        }

        #endregion
    }

    public class TestApp : CefApp
    {
        #region Private/Protected Fields and Constants

        private readonly RenderProcessHandler renderProcessHandler;

        #endregion

        #region Constructors

        public TestApp()
        {
            renderProcessHandler = new RenderProcessHandler();
        }

        #endregion

        #region Private/Protected Methods

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return renderProcessHandler;
        }

        #endregion
    }
}
