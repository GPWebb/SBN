using System;
using System.Xml.Linq;

namespace SBN.Lib.Xml.XslTransform
{
    public class XmlTransformer : IXmlTransformer
    {
        private readonly ITransformCacher _transformCacher;
        private readonly IXmlCompiledTransformer _xmlCompiledTransformer;

        public XmlTransformer(ITransformCacher transformCacher,
            IXmlCompiledTransformer xmlCompiledTransformer)
        {
            _transformCacher = transformCacher;
            _xmlCompiledTransformer = xmlCompiledTransformer;
        }

        public XElement Transform(XElement input, XElement transform)
        {
            try
            {
                var transformer = _transformCacher.GetTransformer(transform);

                return _xmlCompiledTransformer.Transform(input, transformer);
            }
            catch (Exception e)
            {
                throw new Exception($"Error transforming document { input }: {e.Message}", e);
            }
        }
    }
}
