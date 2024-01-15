using System.Collections.Generic;
using System.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IPageDocumentParser
    {
        IEnumerable<PageDocument> Parse(DataTable pageDocumentTable);
    }
}