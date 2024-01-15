using System.Xml.Linq;
using System.Xml.Xsl;

namespace SBN.Lib.Xml.XslTransform
{
    public interface ITransformCacher
    {
        XslCompiledTransform GetTransformer(XElement transform);
    }
}