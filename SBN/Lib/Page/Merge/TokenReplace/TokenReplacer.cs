using SBN.Lib.Page.Request;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class TokenReplacer : ITokenReplacer
    {
        private readonly IQueryStringReplacer _queryStringReplacer;
        private readonly IStandardParameters _standardParameters;
        
        public TokenReplacer(IQueryStringReplacer queryStringReplacer,
            IStandardParameters standardParameters)
        {
            _queryStringReplacer = queryStringReplacer;
            _standardParameters = standardParameters;
        }

        public async Task<string> Replace(string input, 
            Definitions.ParamEncode encode, 
            Definitions.EncodeType encodeType, 
            string pageTitle, 
            Guid sessionToken)
        {
            var ret = input;

            if (!string.IsNullOrWhiteSpace(input))
            {
                ret = await _standardParameters.StdParams(ret, encode, encodeType, pageTitle, sessionToken);
            }

            return ret;
        }

        public async Task<string> Replace(string input, 
            Definitions.ParamEncode encode, 
            Definitions.EncodeType encodeType, 
            ValidatedRequest requestParams, 
            string pageTitle,
            Guid sessionToken)
        {
            var ret = _queryStringReplacer.Replace(input, requestParams, encode, encodeType);

            return await Replace(ret, encode, encodeType, pageTitle, sessionToken);
        }
    }
}
