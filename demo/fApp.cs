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
        Button btn;

        public fApp()
        {
            InitializeComponent();

            browser = new CefWebBrowser
            {
                Dock = DockStyle.Fill
            };
            browser.Parent = this;
            browser.BrowserCreated += (se, ev) => {
                browser.Browser.GetMainFrame().LoadUrl("https://google.com.vn");
                //browser.Browser.GetMainFrame().LoadUrl("http://localhost:60000/index.html");
                //browser.Browser.Reload();
                //browser.Browser.GetMainFrame().LoadUrl("about:blank");
                //browser.Browser.GetMainFrame().LoadUrl("http://localhost:60000/index.html");
                //Btn_Click(btn, new EventArgs() { });
                
            };

            //btn = new Button() { Text = "Test" };
            //btn.Click += Btn_Click;
            //this.Controls.Add(btn);
            //btn.BringToFront();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            //browser.Browser.GetMainFrame().LoadUrl(testFileUri.ToString());
            //browser.Browser.GetMainFrame().LoadUrl("https://google.com.vn");
            browser.Browser.GetMainFrame().LoadUrl("http://localhost:60000/index.html");

            //Console.WriteLine("Test file loaded.");

            //// wait a bit
            //for (int i = 0; i < 25; ++i)
            //{
            //    Application.DoEvents();
            //    System.Threading.Thread.Sleep(10);
            //}
        }

        private void fApp_Load(object sender, EventArgs e)
        {
            this.Width = 1024;
            this.Height = 600;
        }
    }
}
