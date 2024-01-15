using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace SBN.Lib.Xml.XslTransform
{
    public class TransformCompiler : ITransformCompiler
    {
        public XslCompiledTransform CompileTransform(XElement transform)
        {
            try
            {
                using (var stringReader = new StringReader(transform.ToString()))
                {
                    using (XmlReader xsltReader = XmlReader.Create(stringReader))
                    {
                        var transformer = new XslCompiledTransform();
                        transformer.Load(xsltReader);

                        return transformer;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error compiling transform", e);
            }
        }
    }
}