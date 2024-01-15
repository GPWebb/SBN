using System.Collections.Generic;
using System.Linq;
using System.Net;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Page.Outcome
{
    public class PageDocumentChecker : IPageDocumentChecker
    {
        private readonly IXPathFacade _xpathFacade;

        public PageDocumentChecker(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        public HttpStatusCode? Check(IEnumerable<PageDocument> pageDocuments, IEnumerable<DocumentTypeResponse> documentTypeResponses)
        {
            if (documentTypeResponses != null)
            {
                foreach (var response in documentTypeResponses)
                {
                    foreach (var document in pageDocuments.Where(d => d.DocumentTypeID == response.DocumentTypeID))
                    {
                        var element = (bool)_xpathFacade
                            .Select(document.Document, response.Query)
                            .FirstOrDefault();

                        if (element == true) return response.ResponseCode;
                    }
                }
            }

            return null;
        }
    }
}
