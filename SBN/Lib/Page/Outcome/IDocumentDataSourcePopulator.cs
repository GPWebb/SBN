using System.Collections.Generic;

namespace SBN.Lib.Page.Outcome
{
    public interface IDocumentDataSourcePopulator
    {
        IAsyncEnumerable<PageDocument> Populate(IEnumerable<PageDocument> pageDocuments);
    }
}