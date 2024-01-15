using System.Xml.Linq;

namespace SBN.Lib.Sys
{
    public interface IXMLSerializerNoNamespace
    {
        XElement Serialize<T>(T dataObject);
    }
}