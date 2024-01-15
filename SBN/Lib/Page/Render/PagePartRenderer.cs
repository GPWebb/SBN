using SBN.Lib.Page.Merge;
using SBN.Lib.Xml.XPath;
using SBN.Lib.Xml.XslTransform;
using System;
using System.Xml.Linq;

namespace SBN.Lib.Page.Render
{
    public class PagePartRenderer : IPagePartRenderer
    {
        private readonly IXmlTransformer _xmlTransformer;
        private readonly IXmlDataMerger _xmlDataMerger;
        private readonly IXPathFacade _xpathFacade;

        public PagePartRenderer(IXmlTransformer xmlTransformer,
            IXmlDataMerger xmlDataMerger,
            IXPathFacade xpathFacade)
        {
            _xmlTransformer = xmlTransformer;
            _xmlDataMerger = xmlDataMerger;
            _xpathFacade = xpathFacade;
        }

        public XElement RenderPart(XElement partTemplate, PagePart pagePart, XElement data)
        {
            var target = _xpathFacade.SelectElement(partTemplate, pagePart.PartPath);
            if (target == null) throw new Exception($"Requested path '{pagePart.PartPath}' not found within the currently rendered document");

            if (pagePart.PartXML == null && pagePart.Source_ActionID == null)
            {
                throw new Exception($"No source data provided to transform for adding to {pagePart.PartPath}");
            }

            //TODO SOLID, one day... it't not terrible as-is
            XElement transformed = null;
            if (pagePart.PartXML != null)
            {
                transformed = _xmlTransformer.Transform(pagePart.PartXML, pagePart.Transform);

                if (data != null)
                {
                    transformed = _xmlDataMerger.Merge(transformed, data);
                }
            }
            else if (pagePart.Source_ActionID != null)
            {
                if (data != null) transformed = _xmlTransformer.Transform(data, pagePart.Transform);
            }
            else
            {
                throw new Exception("No content supplied to render");
            }

            if (transformed != null) target.Add(transformed);
            return partTemplate;
        }
    }
}