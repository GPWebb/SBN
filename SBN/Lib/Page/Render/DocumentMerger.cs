using System;
using System.Collections.Generic;
using System.Linq;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Page.Render
{
    public class DocumentMerger : IDocumentMerger
    {
        private readonly IXPathFacade _xpathFacade;

        public DocumentMerger(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        public string MergeDocuments(string pageTemplateString, IEnumerable<PageDocument> pageDocuments)
        {
            while (pageTemplateString.Contains("[/"))
            {
                var position = pageTemplateString.IndexOf("[/");
                var query = pageTemplateString.Substring(position + 1, pageTemplateString.IndexOf("]", position) - position - 1);

                var nodes = pageDocuments.SelectMany(x => _xpathFacade.SelectElements(x.Document, query));

                if (!nodes.Any()) throw new Exception($"Requested path '{query}' not found");
                if (nodes.Count() > 1) throw new Exception($"Expected 1 node match for '{query}', found {nodes.Count()}");

                pageTemplateString = pageTemplateString.Replace($"[{query}]", nodes.Single().Value);
            }

            return pageTemplateString;
        }
    }
}
