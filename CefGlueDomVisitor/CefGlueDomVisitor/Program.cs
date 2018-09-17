using System;
using System.Collections.Generic;
using System.IO;
using Xilium.CefGlue;

namespace CefGlueDomVisitor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize CEF.
            CefRuntime.Load();
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new DemoCefApp();
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp) != -1)
            {
                Console.Error.WriteLine("CefRuntime could not the secondary process.");
            }

            // Start the browser process.
            CefRuntime.Initialize(cefMainArgs, new CefSettings
            {
                SingleProcess = false,
                MultiThreadedMessageLoop = true
            }, cefApp);
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsOffScreen(IntPtr.Zero);

            // Start up the browser instance.
            var cefBrowserSettings = new CefBrowserSettings();
            var cefClient = new DemoCefClient(1280, 720);
            const string url = "https://news.ycombinator.com/";
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);

            // Hang, to let the browser to do its work.
            Console.WriteLine("Press a key at any time to end the program.");
            Console.ReadKey();

            CefRuntime.Shutdown();
        }
    }

    internal class DemoCefClient : CefClient
    {
        private readonly DemoCefLoadHandler _loadHandler;
        private readonly DemoCefRenderHandler _renderHandler;

        public DemoCefClient(int windowWidth, int windowHeight)
        {
            _renderHandler = new DemoCefRenderHandler(windowWidth, windowHeight);
            _loadHandler = new DemoCefLoadHandler();
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }
    }

    internal class DemoCefLoadHandler : CefLoadHandler
    {
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            browser.SendProcessMessage(CefProcessId.Renderer, CefProcessMessage.Create("GetHackerNewsTitles"));
        }
    }

    internal class DemoCefApp : CefApp
    {
        private readonly DemoCefRenderProcessHandler _renderProcessHandler;

        public DemoCefApp()
        {
            _renderProcessHandler = new DemoCefRenderProcessHandler();
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }
    }

    internal class DemoCefRenderProcessHandler : CefRenderProcessHandler
    {
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (message.Name == "GetHackerNewsTitles")
            {
                CefFrame mainFrame = browser.GetMainFrame();
                mainFrame.VisitDom(new DemoCefDomVisitor());
                return true;
            }

            return false;
        }
    }

    internal class DemoCefDomVisitor : CefDomVisitor
    {
        protected override void Visit(CefDomDocument document)
        {
            //File.WriteAllLines("HackerNewsTitles.txt", GetHackerNewsTitles(document.Root));
        }

        private IEnumerable<string> GetHackerNewsTitles(CefDomNode node)
        {
            if (IsHackerNewsTitle(node))
            {
                yield return node.FirstChild.InnerText;
            }

            CefDomNode child = node.FirstChild;
            while (child != null)
            {
                foreach (string title in GetHackerNewsTitles(child))
                {
                    yield return title;
                }
                child = child.NextSibling;
            }
        }

        private bool IsHackerNewsTitle(CefDomNode node)
        {
            return
                node.NodeType == CefDomNodeType.Element &&
                node.ElementTagName == "TD" &&
                node.HasAttribute("class") &&
                node.GetAttribute("class") == "title" &&
                node.FirstChild.NextSibling != null;
        }
    }
}