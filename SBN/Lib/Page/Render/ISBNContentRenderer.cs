using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public interface ISBNContentRenderer
    {
        XElement Render(XElement pageTemplate);
    }
}
