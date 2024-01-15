using System.Xml.Linq;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IMergeFromDocument
    {
        string Merge(DataMergeParameter dataMergeParameter, XElement document);
    }
}