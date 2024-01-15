using System.Collections.Generic;
using System.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IDocumentTypeResponseParser
    {
        IEnumerable<DocumentTypeResponse> Parse(DataTable documentTypeResponseTable);
    }
}