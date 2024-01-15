using System.Xml.Linq;
using System.Xml.Xsl;

namespace SBN.Lib.Xml.XslTransform
{
    public interface ITransformCompiler
    {
        XslCompiledTransform CompileTransform(XElement transform);
    }
}