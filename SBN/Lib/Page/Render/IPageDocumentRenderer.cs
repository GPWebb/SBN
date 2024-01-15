using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public interface IPageDocumentRenderer
    {
        XElement RenderDocument(XElement partTemplate, PageDocument pageDocument);
    }
}