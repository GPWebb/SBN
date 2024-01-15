using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace SBN.Lib.Xml.XslTransform
{
    public class XmlCompiledTransformer : IXmlCompiledTransformer
    {
        public XElement Transform(XElement input, XslCompiledTransform transformer)
        {
            using (XmlReader inputReader = input.CreateReader())
            {
                var output = new XDocument();

                using (XmlWriter outputWriter = output.CreateWriter())
                {
                    transformer.Transform(inputReader, outputWriter);
                }

                return output.Root;
            }
        }
    }
}