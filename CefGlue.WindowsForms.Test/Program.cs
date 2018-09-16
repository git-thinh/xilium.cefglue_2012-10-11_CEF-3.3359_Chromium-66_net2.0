using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Xilium.CefGlue;

namespace CefGlue.WindowsForms.Test {
  static class Program {
    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    private static int Main(string[] args)
    {
      string execDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      CefRuntime.Load();

      CefSettings settings = new CefSettings {
        SingleProcess = System.Diagnostics.Debugger.IsAttached,
        MultiThreadedMessageLoop = true,
        LogSeverity = CefLogSeverity.Verbose,
        LogFile = "cef.log",
        //AutoDetectProxySettingsEnabled = true,        
        ResourcesDirPath = execDir,
        //BrowserSubprocessPath = Path.Combine(execDir, "cefclient.exe"),
      };

      CefMainArgs mainArgs = new CefMainArgs(args);
      CefApp app = new TestApp();

      var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
      Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
      if (exitCode != -1)
        return exitCode;

      // guard if something wrong
      foreach (var arg in args) { if (arg.StartsWith("--type=")) { return -2; } }

      CefRuntime.Initialize(mainArgs, settings, app);

      if (!settings.MultiThreadedMessageLoop) {
        Application.Idle += (sender, e) => CefRuntime.DoMessageLoopWork();
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainView());

      CefRuntime.Shutdown();
      return 0;
    }
  }
}
