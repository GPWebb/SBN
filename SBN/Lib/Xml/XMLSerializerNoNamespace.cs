using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SBN.Lib.Sys
{
    public class XMLSerializerNoNamespace : IXMLSerializerNoNamespace
    {
        public XElement Serialize<T>(T dataObject)
        {
            var serializer = new XmlSerializer(typeof(T));

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var sw = new StringWriter();
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true });

            serializer.Serialize(xmlWriter, dataObject, ns);
            return XElement.Parse(sw.ToString());
        }
    }
}
