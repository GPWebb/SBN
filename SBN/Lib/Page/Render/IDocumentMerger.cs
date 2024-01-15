using System.Collections.Generic;

namespace SBN.Lib.Page.Render
{
    public interface IDocumentMerger
    {
        string MergeDocuments(string pageTemplateString, IEnumerable<PageDocument> pageDocuments);
    }
}