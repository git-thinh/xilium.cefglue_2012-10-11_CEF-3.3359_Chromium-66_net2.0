using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xilium.CefGlue;

namespace OffScreenDemo
{
    internal class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {

            string execDir1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string execDir2 = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);

            //execDir2 = Path.Combine(execDir2, "Debugx86") + @"\";
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

            ////// Load CEF. This checks for the correct CEF version.
            ////CefRuntime.Load();

            // Start the secondary CEF process.
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new DemoCefApp();

            // This is where the code path divereges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp) != -1)
            {
                Console.Error.WriteLine("Could not the secondary process.");
            }

            //var exitCode = CefRuntime.ExecuteProcess(cefMainArgs, cefApp);
            //Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            //if (exitCode != -1)
            //    return exitCode;

            ////// Settings for all of CEF (e.g. process management and control).
            ////var cefSettings = new CefSettings
            ////{
            ////    SingleProcess = false,
            ////    MultiThreadedMessageLoop = true
            ////};

            // Start the browser process (a child process).
            ////CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);
            CefRuntime.Initialize(cefMainArgs, settings, cefApp, IntPtr.Zero);

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            //cefWindowInfo.SetAsOffScreen(IntPtr.Zero);
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

            // Settings for the browser window itself (e.g. enable JavaScript?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            var cefClient = new DemoCefClient(1280, 720);

            // Start up the browser instance.
            //CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, "http://www.reddit.com/");
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, "https://google.com.vn/");

            // Hang, to let the browser to do its work.
            Console.WriteLine("Press a key at any time to end the program.");
            Console.ReadKey();

            // Clean up CEF.
            CefRuntime.Shutdown();

            return 0;
        }
    }

}
