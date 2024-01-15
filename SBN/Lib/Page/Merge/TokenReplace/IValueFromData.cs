using SBN.Lib.Action.Data;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IValueFromData
    {
        string Value(DataMergeParameter dataMergeParameter, ActionData data);
    }
}