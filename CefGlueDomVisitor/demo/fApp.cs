using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms; 
using Xilium.CefGlue.WindowsForms;

namespace demo
{
    public partial class fApp : Form
    {
        private CefWebBrowser browser;

        public fApp()
        {
            browser = new CefWebBrowser()
            {
                Dock = DockStyle.Fill
            };
            browser.Parent = this;
            browser.BrowserCreated += (se, ev) =>
            {
                //browser.Browser.GetMainFrame().LoadUrl("https://google.com.vn");
                //browser.Browser.GetMainFrame().LoadUrl("http://localhost:60000/index.html");
                //browser.Browser.Reload();
                //browser.Browser.GetMainFrame().LoadUrl("about:blank");
                //browser.Browser.GetMainFrame().LoadUrl("http://localhost:60000/index.html");
                //Btn_Click(btn, new EventArgs() { });
                
            };
        }

        ~fApp() {
            browser.Dispose();
        }
    }
}
