using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IQueryStringParameterReplacer
    {
        string Replace(ValidatedRequest requestParams,
            Definitions.ParamEncode encode,
            Definitions.EncodeType encodeType,
            string output,
            Definitions.ParamMissingBehaviour paramMissingBehaviour);
    }
}