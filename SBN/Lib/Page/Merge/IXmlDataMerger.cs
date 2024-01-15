using System.Xml.Linq;

namespace SBN.Lib.Page.Merge
{
    public interface IXmlDataMerger
    {
        XElement Merge(XElement transformed, XElement data);
    }
}