using System.Xml.Linq;

namespace SBN.Lib.Xml.XslTransform
{
    public interface IXmlTransformer
    {
        XElement Transform(XElement input, XElement transform);
    }
}