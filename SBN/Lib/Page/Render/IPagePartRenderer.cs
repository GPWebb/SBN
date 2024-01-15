using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public interface IPagePartRenderer
    {
        XElement RenderPart(XElement pageTemplate, PagePart pagePart, XElement data);
    }
}