﻿namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            using (var application = new DemoAppImpl())
            {
                return application.Run(args);
            }
        }
    }
}
