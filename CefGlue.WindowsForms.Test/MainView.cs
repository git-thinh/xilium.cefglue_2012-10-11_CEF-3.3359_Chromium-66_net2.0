using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;

namespace CefGlue.WindowsForms.Test
{
    public partial class MainView : Form
    {
        private CefWebBrowser browser;
        private TextBox textBox;

        public MainView()
        {
            InitializeComponent();

            browser = new CefWebBrowser
            {
                Dock = DockStyle.Fill
            };
            browser.Parent = this;

            Button button = new Button
            {
                Dock = DockStyle.Top,
                Text = "Run test",
            };
            button.Parent = this;
            button.Click += (sender, args) => runTest();

            textBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                ReadOnly = true,
            };
            textBox.Parent = this;
            Console.Box = textBox;
        }

        private void runTest()
        {
            // load test file
            string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testFile = Path.Combine(execPath, "test.html");
            Uri testFileUri = new Uri(testFile);
            Console.WriteLine("Loading test file ...");

            //browser.Browser.GetMainFrame().LoadUrl(testFileUri.ToString());
            //browser.Browser.GetMainFrame().LoadUrl("https://google.com.vn");
            browser.Browser.GetMainFrame().LoadUrl("https://localhost:60000/index.html");

            Console.WriteLine("Test file loaded.");

            // wait a bit
            for (int i = 0; i < 25; ++i)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }

            // convert dom content
            Console.WriteLine("Converting DOM ...");
            DomConverter domConverter = new DomConverter();
            ConversionData conversionData = domConverter.Convert(browser);
            if (conversionData == null || conversionData.ImageSections == null || conversionData.TableSections == null)
            {
                Console.WriteLine("DOM conversion failed!");
            }
            else
            {
                Console.WriteLine("DOM conversion succeeded!");
            }
        }
    }
}
