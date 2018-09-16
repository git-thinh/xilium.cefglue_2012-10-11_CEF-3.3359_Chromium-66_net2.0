using System.Windows.Forms;

namespace CefGlue.WindowsForms.Test
{
  public static class Console {
    public static TextBox Box { get; set; }
    public static void WriteLine(string message, params object[] args)
    {
      string msg = string.Format(message, args);
      System.Console.WriteLine(msg);
      if (Box != null)
        Box.Text = msg;
      if (System.Diagnostics.Debugger.IsAttached)
        System.Diagnostics.Debugger.Log(0, "Info", msg + System.Environment.NewLine);
    }
  }
}