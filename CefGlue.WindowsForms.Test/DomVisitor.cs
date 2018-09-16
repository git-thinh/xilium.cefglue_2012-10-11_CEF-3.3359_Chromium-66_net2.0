using System;
using System.Collections.Generic;
using System.Threading;
using Xilium.CefGlue;

namespace CefGlue.WindowsForms.Test
{
  public class DomVisitor : CefDomVisitor, IDisposable {
    #region Public/Internal Properties

    public static readonly Dictionary<string, DomVisitor> Visitors = new Dictionary<string, DomVisitor>();
    public ManualResetEvent Finished { get; private set; }
    public string Id { get; private set; }

    #endregion

    public DomVisitor() {
      Id = Guid.NewGuid().ToString();
      Finished = new ManualResetEvent(false);
      lock (Visitors) {
        Visitors.Add(Id, this);
      }
    }

    #region Private/Protected Methods

    protected override void Visit(CefDomDocument document) {
      Finished.Set();
    }

    #endregion

    public void Dispose() {
      lock (Visitors) {
        Visitors.Remove(Id);
      }
    }
  }
}