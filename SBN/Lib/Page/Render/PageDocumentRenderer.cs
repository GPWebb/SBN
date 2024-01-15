using System;
using System.Xml.Linq;
using SBN.Lib.Xml.XPath;
using SBN.Lib.Xml.XslTransform;

namespace SBN.Lib.Page.Render
{
    public class PageDocumentRenderer : IPageDocumentRenderer
    {
        private readonly IXmlTransformer _xmlTransformer;
        private readonly IXPathFacade _xpathFacade;

        public PageDocumentRenderer(IXmlTransformer xmlTransformer, IXPathFacade xpathFacade)
        {
            _xmlTransformer = xmlTransformer;
            _xpathFacade = xpathFacade;
        }

        public XElement RenderDocument(XElement partTemplate, PageDocument pageDocument)
        {
            var target = _xpathFacade.SelectElement(partTemplate, pageDocument.PartPath);
            if (target == null) throw new Exception($"Requested path '{pageDocument.PartPath}' not found within the currently rendered document");

            if (pageDocument.Document == null) throw new Exception("No source document provided to transform");

            XElement transformed;
            transformed = _xmlTransformer.Transform(pageDocument.Document, pageDocument.TransformXsl);

            target.Add(transformed);
            return partTemplate;
        }
    }
}