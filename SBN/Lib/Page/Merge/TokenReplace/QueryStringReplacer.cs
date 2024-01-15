using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class QueryStringReplacer : IQueryStringReplacer
    {
        private readonly IQueryStringParameterReplacer _queryStringParameterReplacer;

        public QueryStringReplacer(IQueryStringParameterReplacer queryStringParameterReplacer)
        {
            _queryStringParameterReplacer = queryStringParameterReplacer;
        }

        public string Replace(string input, ValidatedRequest requestParams, Definitions.ParamEncode encode, Definitions.EncodeType encodeType)
        {
            string output = input;

            if (output != null)
            {
                while (output.Contains("[?"))
                {
                    output = _queryStringParameterReplacer.Replace(requestParams, encode, encodeType, output, Definitions.ParamMissingBehaviour.Blank);
                }
            }
            return output;
        }
    }

}
