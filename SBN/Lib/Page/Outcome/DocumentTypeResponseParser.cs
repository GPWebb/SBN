using System.Collections.Generic;
using System.Data;
using System.Net;

namespace SBN.Lib.Page.Outcome
{
    public class DocumentTypeResponseParser : IDocumentTypeResponseParser
    {
        public IEnumerable<DocumentTypeResponse> Parse(DataTable documentTypeResponseTable)
        {
            foreach (DataRow row in documentTypeResponseTable.Rows)
            {
                yield return new DocumentTypeResponse 
                {
                    DocumentTypeID = row.Field<int>("DocumentTypeID"),
                    Query = row.Field<string>("Query"),
                    ResponseCode = (HttpStatusCode)row.Field<short>("ResponseCode")
                };
            }
        }
    }
}
