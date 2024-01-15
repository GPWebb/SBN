using System.Xml.Linq;
using System.Xml.Xsl;

namespace SBN.Lib.Xml.XslTransform
{
    public interface IXmlCompiledTransformer
    {
        XElement Transform(XElement input, XslCompiledTransform transformer);
    }
}