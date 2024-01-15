using System.Collections.Generic;
using System.Net;

namespace SBN.Lib.Page.Outcome
{
    public interface IPageDocumentChecker
    {
        HttpStatusCode? Check(IEnumerable<PageDocument> pageDocuments, IEnumerable<DocumentTypeResponse> documentTypeResponses);
    }
}