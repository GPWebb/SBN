using System.Collections.Generic;
using System.Data;
using SBN.Lib.DB.Lib;

namespace SBN.Lib.Page.Outcome
{
    public class PageDocumentParser : IPageDocumentParser
    {
        public IEnumerable<PageDocument> Parse(DataTable pageDocumentTable)
        {
            foreach (DataRow row in pageDocumentTable.Rows)
            {
                yield return new PageDocument
                {
                    DocumentTypeID = row.Field<int>("DocumentTypeID"),
                    Document = row.XmlField("Document"),
                    TransformXsl = row.XmlField("TransformXsl"),
                    Position = row.Field<int>("Position"),
                    PartPath = row.Field<string>("PartPath")
                };
            }
        }
    }
}
