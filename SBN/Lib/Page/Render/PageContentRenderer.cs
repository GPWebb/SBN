using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Render
{
    public class PageContentRenderer : IPageContentRenderer
    {
        private readonly IPartPreRenderer _partPreRenderer;
        private readonly IPageDocumentRenderer _pageDocumentRenderer;

        public PageContentRenderer(IPartPreRenderer partPreRenderer,
            IPageDocumentRenderer pageDocumentRenderer)
        {
            _partPreRenderer = partPreRenderer;
            _pageDocumentRenderer = pageDocumentRenderer;
        }

        public async Task<XElement> RenderPageContents(XElement pageTemplate,
            IEnumerable<string> permissions,
            IEnumerable<PagePart> pageParts,
            IEnumerable<ActionData> actionData,
            IEnumerable<PageDocument> pageDocuments,
            int maxPosition,
            string pageTitle,
            ValidatedRequest requestParams)
        {
            try
            {
                var sortedParts = pageParts?.OrderBy(x => x.Position);
                var sortedDocuments = pageDocuments?.OrderBy(x => x.Position);
                for (var i = 0; i <= maxPosition; i++)
                {
                    var parts = sortedParts?.Where(x => x.Position == i);
                    var documents = sortedDocuments?.Where(x => x.Position == i);

                    if (parts != null)
                    {
                        foreach (var part in parts)
                        {
                            pageTemplate = await _partPreRenderer.PreRenderPart(pageTemplate,
                                permissions,
                                actionData,
                                part,
                                pageTitle,
                                requestParams);
                        }
                    }

                    if (documents != null)
                    {
                        foreach (var document in documents)
                        {
                            pageTemplate = _pageDocumentRenderer.RenderDocument(pageTemplate, document);
                        }
                    }
                }

                return pageTemplate;
            }
            catch(Exception ex)
            {
                throw new Exception("Error rendering page content", ex);
            }
        }
    }
}
