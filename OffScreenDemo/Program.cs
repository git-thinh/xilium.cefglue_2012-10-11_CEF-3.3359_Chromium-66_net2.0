using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace OffScreenDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Load CEF. This checks for the correct CEF version.
            CefRuntime.Load();

            // Start the secondary CEF process.
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new DemoCefApp();

            // This is where the code path divereges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp) != -1)
            {
                Console.Error.WriteLine("Could not the secondary process.");
            }

            // Settings for all of CEF (e.g. process management and control).
            var cefSettings = new CefSettings
            {
                SingleProcess = false,
                MultiThreadedMessageLoop = true
            };

            // Start the browser process (a child process).
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp);

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsOffScreen(IntPtr.Zero);

            // Settings for the browser window itself (e.g. enable JavaScript?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            var cefClient = new DemoCefClient(1280, 720);

            // Start up the browser instance.
            CefBrowserHost.CreateBrowser(
                cefWindowInfo,
                cefClient,
                cefBrowserSettings,
                "http://www.reddit.com/");

            // Hang, to let the browser to do its work.
            Console.WriteLine("Press a key at any time to end the program.");
            Console.ReadKey();

            // Clean up CEF.
            CefRuntime.Shutdown();
        }
    }
     
}
