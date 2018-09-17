using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Xilium.CefGlue;

namespace OffScreenDemo
{

    internal class DemoCefApp : CefApp
    {
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
        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        //protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            // A single CefBrowser instance can handle multiple requests
            //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
            if (frame.IsMain)
            {
                Console.WriteLine("START: {0}", browser.GetMainFrame().Url);
            }
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if (frame.IsMain)
            {
                Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, httpStatusCode);
            }
        }
    }

    internal class DemoCefRenderHandler : CefRenderHandler
    {
        private readonly int _windowHeight;
        private readonly int _windowWidth;

        public DemoCefRenderHandler(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return GetViewRect(browser, ref rect);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = _windowWidth;
            rect.Height = _windowHeight;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // Save the provided buffer (a bitmap image) as a PNG.
            var bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, buffer);
            bitmap.Save("LastOnPaint.png", ImageFormat.Png);
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            return null;
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
        }
    }
}
