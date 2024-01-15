using System;
using System.Xml.Linq;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Page.Render
{
    public class SBNContentRenderer : ISBNContentRenderer
    {
        private readonly IXPathFacade _xpathFacade;

        public SBNContentRenderer(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        public XElement Render(XElement pageTemplate)
        {
            try
            {
                foreach (var element in _xpathFacade.SelectElements(pageTemplate, "//SBN"))
                {
                    var path = element.Attribute("Path").Value;

                    if (string.IsNullOrWhiteSpace(path)) throw new Exception($"Target path not specified, cannot move nodes");

                    var nodesToMove = element.DescendantNodes();

                    var target = _xpathFacade.SelectElement(pageTemplate, path);

                    if (target == null) throw new Exception($"Path '{path}' not found in page template, cannot move nodes");

                    target.Add(nodesToMove);

                    element.Remove();
                }

                return pageTemplate;
            }
            catch(Exception ex)
            {
                throw new Exception("Error rendering SBN content", ex);
            }
        }
    }
}
