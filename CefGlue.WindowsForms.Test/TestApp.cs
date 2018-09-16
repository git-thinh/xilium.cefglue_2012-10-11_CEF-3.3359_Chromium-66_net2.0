using Xilium.CefGlue;

namespace CefGlue.WindowsForms.Test
{
  public class TestApp : CefApp {
    #region Private/Protected Fields and Constants

    private readonly RenderProcessHandler renderProcessHandler;

    #endregion

    #region Constructors

    public TestApp() {
      renderProcessHandler = new RenderProcessHandler();
    }

    #endregion

    #region Private/Protected Methods

    protected override CefRenderProcessHandler GetRenderProcessHandler() {
      return renderProcessHandler;
    }

    #endregion
  }
}