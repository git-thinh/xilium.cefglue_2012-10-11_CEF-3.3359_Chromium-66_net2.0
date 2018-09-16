using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Xilium.CefGlue;
using Xilium.CefGlue.WindowsForms;

namespace CefGlue.WindowsForms.Test
{
  public class ImageSectionRow
  {
    public IList<Image> Images { get; set; }
  }
  public class ImageSection
  {
    public string Title { get; set; }
    public IList<ImageSectionRow> Rows { get; set; }
  }
  public class TableColumn
  {
    public string HeaderText { get; set; }
  }
  public class TableCell
  {
    public string Value { get; set; }
  }
  public class TableRow
  {
    public IList<TableCell> Cells { get; set; }
  }
  public class TableSection
  {
    public string Title { get; set; }
    public IList<TableColumn> Columns { get; set; }
    public IList<TableRow> Rows { get; set; }
  }
  public class ConversionData
  {
    public IList<ImageSection> ImageSections { get; set; }
    public IList<TableSection> TableSections { get; set; }
  }

  public class DomConverter {
    #region Public/Internal Methods

    public ConversionData Convert(CefWebBrowser browser) {
      Console.WriteLine("Initialize DOM-Visitor");
      ConversionDomVisitor visitor = new ConversionDomVisitor {
        Data = new ConversionData()
      };

      Console.WriteLine("Send message '{0}({1})' to render process", RenderProcessMessages.VisitDom, visitor.Id);
      CefProcessMessage message = CefProcessMessage.Create(RenderProcessMessages.VisitDom);
      message.Arguments.SetString(0, visitor.Id);
      browser.Browser.SendProcessMessage(CefProcessId.Renderer, message);

      Console.WriteLine("Waiting for visitor '{0}' to finish ...", visitor.Id);
      while (!visitor.Finished.WaitOne(10))
        Application.DoEvents();

      Console.WriteLine("Visitor '{0}' finished!", visitor.Id);
      ConversionData data = visitor.Data;
      visitor.Dispose();

      return data;
    }

    #endregion

    #region Nested types (enums, classes, ...)

    private class HtmlImageConverter {
      #region Public/Internal Properties

      public IList<ImageSection> ImageSections { get; private set; }

      #endregion

      #region Constructors

      public HtmlImageConverter() {
        ImageSections = new List<ImageSection>();
      }

      #endregion

      #region Public/Internal Methods

      public void Convert(CefDomNode container) {
        if (container != null) {
          container.Descendants(node => node.HasClass("mb-chart-container"))
            .ForEach(chartContainer =>
                     ImageSections.Add(new ImageSection {
                       Title = getSectionName(chartContainer),
                       Rows = getSectionRows(chartContainer)
                     }));
        }
      }

      #endregion

      #region Private/Protected Methods

      private Image extractImage(CefDomNode node) {
        if (node.IsElement && node.ElementTagName.Equals("img", StringComparison.InvariantCultureIgnoreCase)) {
          string imgData = node.GetAttribute("src");
          if (!string.IsNullOrWhiteSpace(imgData) && imgData.StartsWith("data:image", StringComparison.InvariantCultureIgnoreCase)) {
            // remove image-type markup (eg data:image/png;)
            imgData = imgData.Remove(0, imgData.IndexOf(';') + 1);
            // remove encoding markup (eg base64,)
            imgData = imgData.Remove(0, imgData.IndexOf(',') + 1);
            // remove any line breaks
            imgData = imgData.Replace("\r\n", string.Empty);
            // remove any blanks
            imgData = imgData.Replace(" ", string.Empty);

            if (!string.IsNullOrWhiteSpace(imgData)) {
                // translate embedded image to Image-Objekt
                byte[] buffer = System.Convert.FromBase64String(imgData);
                if (buffer.Length > 0) {
                  return new ImageConverter().ConvertFrom(buffer) as Image;
                }
            }
          }
        }
        return null;
      }

      private string getNodeInnerText(CefDomNode node) {
        if (node != null) {
          string text = node.InnerText;
          if (!string.IsNullOrWhiteSpace(text)) {
            return text;
          }
        }
        return string.Empty;
      }

      private string getSectionName(CefDomNode container) {
        string sectionName = string.Empty;

        if (container != null && container.HasChildren) {
          // div (container) > div.mb-chart-header > ...
          container.Elements(node => node.HasClass("mb-chart-header"))
            .ForEach(headerNode => sectionName += getNodeInnerText(headerNode));
        }

        return sectionName;
      }

      private IList<ImageSectionRow> getSectionRows(CefDomNode container) {
        IList<ImageSectionRow> sectionRows = new List<ImageSectionRow>();

        if (container != null && container.HasChildren) {
          container.Elements(node => node.HasClass("mb-chart-content"))
            .Where(chartContentNode => chartContentNode.HasChildren)
            .Take(1)
            .ForEach(chartContentNode =>
                     chartContentNode.Elements(node => node.HasClass("mb-chart-content-line"))
                       .Where(chartContentLine => chartContentLine.HasChildren)
                       .ForEach(chartContentLine =>
                                sectionRows.Add(new ImageSectionRow {
                                  Images = chartContentLine.Descendants("img").Select(extractImage).Where(img => img != null).ToList()
                                })
                       )
            );
        }

        return sectionRows;
      }

      #endregion
    }

    private class HtmlTableConverter {
      #region Public/Internal Properties

      public IList<TableSection> TableSections { get; private set; }

      #endregion

      #region Constructors

      public HtmlTableConverter() {
        TableSections = new List<TableSection>();
      }

      #endregion

      #region Public/Internal Methods

      public void Convert(CefDomNode container) {
        if (container != null) {
          container.Descendants(node => node.HasClass("mb-table-container"))
            .Where(tableContainerNode => tableContainerNode.HasChildren)
            .ForEach(tableContainerNode =>
                     tableContainerNode.Elements(node => node.HasClass("mb-table-content"))
                       .Where(tableContentContainerNode => tableContentContainerNode.HasChildren)
                       .Take(1)
                       .ForEach(tableContentContainerNode =>
                                tableContentContainerNode.Elements(node => node.HasClass("mb-table"))
                                  .Where(tableNode => tableNode.HasChildren)
                                  .Take(1)
                                  .ForEach(tableNode =>
                                           TableSections.Add(new TableSection {
                                             Title = getSectionName(tableContainerNode),
                                             Columns = getColumns(tableNode),
                                             Rows = getRows(tableNode)
                                           })
                                  )
                       )
            );
        }
      }

      #endregion

      #region Private/Protected Methods

      private IList<TableColumn> getColumns(CefDomNode container) {
        IList<TableColumn> tableColumns = new List<TableColumn>();

        if (container != null && container.HasChildren) {
          // table (container) > thead > tr > th > ...
          container.Elements("thead")
            .Where(theadNode => theadNode.HasChildren)
            .Take(1)
            .ForEach(theadNode =>
                     theadNode.Elements("tr")
                       .Where(trNode => trNode.HasChildren)
                       .Take(1)
                       .ForEach(trNode =>
                                trNode.Elements("th")
                                  .ForEach(thNode =>
                                           tableColumns.Add(new TableColumn { HeaderText = getNodeInnerText(thNode) })
                                  )
                       )
            );
        }

        return tableColumns;
      }

      private string getNodeInnerText(CefDomNode node) {
        if (node != null) {
          string text = node.InnerText;
          if (!string.IsNullOrWhiteSpace(text)) {
            return text;
          }
        }
        return string.Empty;
      }

      private TableRow getRow(CefDomNode trNode) {
        // ... > tr > th|td > ...
        TableRow tableRow = new TableRow
                              {
                                Cells = new List<TableCell>()
                              };

        trNode.Elements("th").Union(trNode.Elements("td"))
          .ForEach(cellNode => tableRow.Cells.Add(new TableCell { Value = getNodeInnerText(cellNode) }));

        return tableRow;
      }

      private IList<TableRow> getRows(CefDomNode container) {
        IList<TableRow> tableRows = new List<TableRow>();

        if (container != null && container.HasChildren) {
          // table (container) > tbody > tr
          container.Elements("tbody")
            .Where(tbodyNody => tbodyNody.HasChildren)
            .Take(1)
            .ForEach(tbodyNode =>
                     tbodyNode.Elements("tr")
                       .Where(trNode => trNode.HasChildren)
                       .ForEach(trNode => tableRows.Add(getRow(trNode)))
            );
        }

        return tableRows;
      }

      private string getSectionName(CefDomNode container) {
        string sectionName = string.Empty;

        if (container != null && container.HasChildren) {
          // div (container) > div.mb-table-header > ...
          container.Elements(node => node.HasClass("mb-table-header"))
            .ForEach(headerNode => sectionName += getNodeInnerText(headerNode));
        }

        return sectionName;
      }

      #endregion
    }

    public class ConversionDomVisitor : DomVisitor {
      #region Public/Internal Properties

      public ConversionData Data { get; set; }

      #endregion

      #region Private/Protected Methods

      private void convertImages(CefDomNode container) {
        HtmlImageConverter imageConverter = new HtmlImageConverter();
        imageConverter.Convert(container);
        Data.ImageSections = imageConverter.ImageSections;
      }

      private void convertTables(CefDomNode container) {
        HtmlTableConverter tableConverter = new HtmlTableConverter();
        tableConverter.Convert(container);
        Data.TableSections = tableConverter.TableSections;
      }

      protected override void Visit(CefDomDocument document) {
        if (document != null && Data != null) {
          convertTables(document.Body);
          convertImages(document.Body);
        }
        base.Visit(document);
      }

      #endregion
    }

    #endregion
  }

  public static class EnumerableExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action) {
      foreach (T obj in values)
        action(obj);
    }
  }

  public static class CefDomNodeExtensions {
    #region Public/Internal Methods

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<CefDomNode> Descendants(this CefDomNode node, Func<CefDomNode, bool> predicate) {
      if (node != null && node.HasChildren) {
        CefDomNode current = node.FirstChild;
        while (current != null) {
          if (current.IsElement) {
            if (predicate(current)) {
              yield return current;
            }
            if (current.HasChildren) {
              foreach (var childNode in current.Descendants(predicate)) {
                yield return childNode;
              }
            }
          }
          current = current.NextSibling;
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<CefDomNode> Descendants(this CefDomNode node, string name) {
      if (node != null && node.HasChildren) {
        CefDomNode current = node.FirstChild;
        while (current != null) {
          if (current.IsElement) {
            if (name.Equals(current.ElementTagName, StringComparison.InvariantCultureIgnoreCase)) {
              yield return current;
            }
            if (current.HasChildren) {
              foreach (var childNode in current.Descendants(name)) {
                yield return childNode;
              }
            }
          }
          current = current.NextSibling;
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<CefDomNode> Elements(this CefDomNode node) {
      if (node != null && node.HasChildren) {
        CefDomNode current = node.FirstChild;
        while (current != null) {
          if (current.IsElement) {
            yield return current;
          }
          current = current.NextSibling;
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<CefDomNode> Elements(this CefDomNode node, Func<CefDomNode, bool> predicate) {
      if (node != null && node.HasChildren) {
        CefDomNode current = node.FirstChild;
        while (current != null) {
          if (current.IsElement && predicate(current)) {
            yield return current;
          }
          current = current.NextSibling;
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<CefDomNode> Elements(this CefDomNode node, string name) {
      if (node != null && node.HasChildren) {
        CefDomNode current = node.FirstChild;
        while (current != null) {
          if (current.IsElement && name.Equals(current.ElementTagName, StringComparison.InvariantCultureIgnoreCase)) {
            yield return current;
          }
          current = current.NextSibling;
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetClasses(this CefDomNode node) {
      if (node != null && node.IsElement) {
        string attributeContent = node.GetAttribute("class");
        if (!string.IsNullOrWhiteSpace(attributeContent)) {
          foreach (string value in attributeContent.Split(' ')) {
            yield return value;
          }
        }
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    public static bool HasClass(this CefDomNode node, string className) {
      return node.GetClasses().Any(className.Equals);
    }

    #endregion
  }
}