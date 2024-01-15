using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IQueryStringReplacer
    {
        string Replace(string input, ValidatedRequest requestParams, Definitions.ParamEncode encode, Definitions.EncodeType encodeType);
    }
}